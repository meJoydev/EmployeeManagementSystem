using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using EMS.Application.Interfaces;
using EMS.Application.Services;
using EMS.Application.Validators;
using EMS.Application.DTOs.Employee;
using EMS.Infrastructure.Persistence;
using EMS.Infrastructure.Persistence.Repositories;
using EMS.Infrastructure.Reports;
using EMS.Infrastructure.Security;
using EMS.API.Middleware;

var builder = WebApplication.CreateBuilder(args);       // This creates a builder object, it's like an empty construction site where we'll assemble our entire application piece by piece before it actually starts running.

// 1. Database
builder.Services.AddDbContext<EmsDbContext>(options =>  // AddDbContext<EmsDbContext>(options => options.UseMySql(...)) : This line tells the application to use MySQL as the database provider whenever any class asks for EmsDbContext
    options.UseMySql(                                   
        builder.Configuration.GetConnectionString("DefaultConnection"),                             // ServerVersion.AutoDetect(...) is a Pomelo (MySQL provider) method that automatically detects the version of the MySQL server you're connecting to based on the connection string to generate compatible SQL commands
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))    // builder.Configuration.GetConnectionString("DefaultConnection") automatically reads the Connectionstring from appsettings.json
    ));

// 2. Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();              // Registering builder.Services for dependency injection. Whenever a class needs an INTERFACE, it will get an instance of a class that implements that interface(eg: EmployeeRepository)
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();          // Here the AddScoped method means that the same instance will be used within a single HTTP request, but a new instance will be created for each new request.
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<ISalaryRecordRepository, SalaryRecordRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 3. Security
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();                      // Same registration pattern, for the two security interfaces
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// 4. Services
builder.Services.AddScoped<EmployeeService>();                                      // No interface here, so we just registerd the concrete class directly without any abstraction. This is fine if we don't need to mock or replace this service in the future. If we did, we would create an interface for it and register that instead.
builder.Services.AddScoped<AttendanceService>();                                    // Whenever a class asks for an instance of EmployeeService or AttendanceService, the DI container will directly provide an instance of those classes.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<SalaryService>();

// 5. Reports
builder.Services.AddScoped<PdfReportGenerator>();                                   // Since ReportService's constructor needs both PdfReportGenerator and ExcelReportGenerator so we need to register both first before we register ReportService.
builder.Services.AddScoped<ExcelReportGenerator>();
builder.Services.AddScoped<IReportService, ReportService>();

// 6. Validation
builder.Services.AddValidatorsFromAssemblyContaining<CreateEmployeeValidator>();    // This scans the entire Application project (wherever CreateEmployeeValidator lives) and automatically registers every FluentValidation validator it finds

// 7. Controllers
builder.Services.AddControllers();                                                  // It enables the whole controller/routing/action system that lets [HttpGet], [HttpPost], etc. work on our controller classes

// 8. Global exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();                     // Registers our Global handler with dependency injection, so the framework knows it exists.
builder.Services.AddProblemDetails();                                               // Registers the ProblemDetails middleware, which automatically formats error responses in a standard way (RFC 7807) so that clients can easily understand what went wrong. It's a required companion service that the exception-handling middleware uses internally.

// 9. JWT Authentication
builder.Services.AddAuthentication(options =>                                       // To add Authentication services to the application. Here AddAuthentication internally creates an empty "settings object" for us, hands it to us as OPTIONS, and whatever we set on it inside those curly braces gets applied.
{                                                                                   // Here "Scheme" means choosing which method our app uses to figure out who's making a request. ASP.NET Core supports multiple possible methods like cookies, JWT tokens, Windows auth, third-party logins (Google, Facebook) 
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;     // DefaultAuthenticateScheme : Default scheme used by the app to authenticate user (when someone loggs in), use JWT Bearer to check. JwtBearerDefaults provides a default constant string "Bearer"
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;        // DefaultChallengeScheme : when someone's NOT logged in but tries to access something protected, respond using JWT Bearer's specific failure behavior means returning a 401 Unauthorized with a WWW-Authenticate: Bearer header (whichmeans you need a valid Bearer token to access this)
})                                                                                  // AddAuthentication(...) set up the general "I'm using authentication" system
.AddJwtBearer(options =>                                                            // .AddJwtBearer(...) specifically configures the JWT Bearer method of authentication, which is what we want to use for our API. It sets up how the app should validate incoming JWT tokens in requests.
{                                                                                   // JwtBearerDefaults and TokenValidationParameters come from the JWT NuGet package
    options.TokenValidationParameters = new TokenValidationParameters               // TokenValidationParameters is a class that defines how the app should validate incoming JWT tokens. It has properties like ValidateIssuer, ValidateAudience, ValidateLifetime, and IssuerSigningKey that we set below to ensure the token is valid and trustworthy.
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],                          // This is where appsettings.json finally connects to this code. It reads the "Jwt:Issuer" value from the configuration, which is the expected issuer of the JWT token. If the token's issuer doesn't match this value, the token will be rejected as invalid.
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(                                // As we know (From JwtTokenGenerator.cs) SymmetricSecurityKey means the same key is used both to sign and to verify(which we're using here). This line reads your secret from appsettings.json and wraps it the same way, so verification uses the identical key that was used to create the token in the first place.
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
    };
});

builder.Services.AddAuthorization();                                                // [Authorize] attribute we'll use on controllers/actions later — the actual mechanism that says "only let authenticated users past this point."

// 10. CORS
builder.Services.AddCors(options =>                                                 // CORS (Cross-Origin Resource Sharing) is an exception mechanism which allows a web application running at one origin (protocol + domain + port) to access resources from a server at a different origin.
{                                                                                   // Browsers prevent this by default, using a rule called the Same-Origin Policy: JavaScript running on one website (one "origin") is blocked from making requests to a different website for security reasons.
    options.AddPolicy("AllowReactApp", policy =>                                    // We're using CORS to allow our React frontend (running on localhost:5173) to make HTTP requests to our ASP.NET Core backend (running on localhost:5000). This is necessary because the browser enforces the Same-Origin Policy, which would otherwise block these requests.
    {
        policy.WithOrigins("http://localhost:5173")                                 // Only allow requests from exactly this address not allow everyone.
              .AllowAnyHeader()                                                     // Allow any HTTP header (like Content-Type, Authorization, etc.) in requests from the allowed origin.
              .AllowAnyMethod();                                                    // Allow any HTTP method (like GET, POST, PUT, DELETE, etc.) in requests from the allowed origin.
    });
});

// 11. Swagger
builder.Services.AddEndpointsApiExplorer();                                         // These two lines together enable Swagger
builder.Services.AddSwaggerGen(options =>                                           // “hand me a settings object”
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme               // This adds a security definition to Swagger, which tells it that our API uses Bearer tokens for authentication. This allows Swagger to include an "Authorize" button in its UI, where you can paste a JWT token to authenticate your requests.
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste only the token here (no 'Bearer ' prefix)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement                   // This adds a security requirement to Swagger, which tells it that all endpoints require the Bearer token defined above. This means that when you try to call any endpoint from the Swagger UI, it will include the token in the Authorization header of the request.
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference                                    // This creates a reference to the security scheme we defined above, so Swagger knows that this requirement applies to the "Bearer" scheme we just set up.
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()                                                   // This empty array means that no specific scopes are required for this security scheme. In other words, just having a valid Bearer token is enough to access the endpoints; we don't need to specify any additional permissions or roles.
        }
    });
});

var app = builder.Build();                                                          // Here we finally build the app based on everything we registered above.

// 12. Middleware pipeline
app.UseExceptionHandler();                                                          // It must be first so it wraps everything after it and catches any unhandled exceptions that bubble up through the middleware pipeline.

if (app.Environment.IsDevelopment())                                                // This check ensures Swagger only activates when you're running locally in development mode. Swagger is useful for testing our API endpoints easily.
{                                                                                   // In production, we usually don't want Swagger to expose our entire API's structure ("here's every endpoint and how to call it" to the whole internet.) publicly for security reasons.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();                                                          // Automatically redirects any plain http:// requests to secure https:// ensuring that all communication with the server is encrypted and secure.

app.UseCors("AllowReactApp");                                                       // This tells the app to actually use the CORS policy we defined earlier. It must be placed before UseAuthentication and UseAuthorization in the middleware pipeline, otherwise the CORS headers won't be applied to requests that require authentication or authorization.

app.UseAuthentication();                                                            // Authentication is "check this person's ID, confirm who they claim to be" (checking the token's signature, reading the claims).
app.UseAuthorization();                                                             // Authorization is "now that we know who they are, check if they're allowed to do this specific thing" (checking their Role claim against what an endpoint requires).

app.MapControllers();                                                               // This tells the app to look for controller classes and map their routes (like /api/employees) to the appropriate methods. Without this, none of our controllers would actually respond to HTTP requests.

app.Run();