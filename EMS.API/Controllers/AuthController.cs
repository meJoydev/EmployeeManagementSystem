using Microsoft.AspNetCore.Mvc;
using EMS.Application.DTOs.Auth;
using EMS.Application.Interfaces;

namespace EMS.API.Controllers
{
    [ApiController]                                                         // This attribute indicates that this class is a Web API controller — automatically handle things like model validation errors, JSON formatting, and request binding the standard API way.
    [Route("api/[controller]")]                                             // This attribute defines the route for the controller. The [controller] token is replaced with the name of the controller class, minus the "Controller" suffix. In this case, it will be "api/auth".
    public class AuthController : ControllerBase                            // Inherits from ControllerBase, which provides the basic functionality for handling HTTP requests, responses and helper methods like Ok(), Unauthorized(), Conflict()
    {
        private readonly IAuthService _authService;                         // Stores the instance of the IAuthService interface as private field to use it here.

        public AuthController(IAuthService authService)                     // Takes the context of the IAuthService (DI)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)          // This method runs when someone hits the endpoint "api/auth/register" with a POST request. It takes a RegisterDto object as input, which contains the new user's registration data (username, email, password).
        {
            //try                                                           // NOTE : Removed try catch block because the exception is now handled inside the AuthService.cs file and it returns a proper response to the controller instead of crashing the app.
            //{
            var result = await _authService.RegisterAsync(dto);             // Wait till it check if the email's taken, hash the password, save the new user, generate a login token and return an AuthResponseDto object to the controller.
            return StatusCode(StatusCodes.Status201Created, result);        // It only returns a 201 Created response with result as response body unlike CreatedAtAction(...) which also sends the location header because we don't have a "GetById" endpoint for users yet.
            //}
            //catch (InvalidOperationException ex)                          // If the email is already taken, the AuthService throws an InvalidOperationException, which is caught here as mentioned inside AuthService.cs. The controller then returns a 409 Conflict response with the error message to the client.
            //{
            //    return Conflict(new { message = ex.Message });            // Conflict(...) is another helper that builds a response with status code 409 Conflict, which means "this request clashes with something that already exists." 
            //}                                                             // new { message = ex.Message } builds a small object containing just the error text ("An account with this email already exists."), and sends that back to React as JSON instead of a crash or a generic error page.
        }                                                                   // Without try catch : If someone tried registering with a duplicate email, AuthService would still throw the exception but the controller would not catch it, it will crash and return a generic 500 Internal Server Error to the client.
                                                                            // React would have no clean way to know the specific problem was: duplicate email
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)                // This method runs when someone hits the endpoint "api/auth/login" with a POST request. It takes a LoginDto object as input, which contains the user's login credentials (email and password).
        {
            //try
            //{
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            //}
            //catch (UnauthorizedAccessException ex)
            //{
            //    return Unauthorized(new { message = ex.Message });       // Unauthorized(...) is a helper that builds a response with status code 401 Unauthorized, which means "the request requires user authentication."
            //}
        }
    }
}