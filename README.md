# Employee Management System (Backend)

ASP.NET Core 8 Web API using Clean Architecture (Domain, Application, Infrastructure, API) with MySQL.

## Features
- JWT login and registration
- Employee CRUD, including bulk create and bulk delete
- Departments, attendance tracking, monthly salary generation
- PDF and Excel reports (employees, departments, attendance, salary)

## Setup
1. Install .NET 8 SDK and MySQL.
2. In `EMS.API`, run `dotnet user-secrets init`, then set:
   - `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=ems_db;User=root;Password=YOUR_PASSWORD;"`
   - `dotnet user-secrets set "Jwt:Secret" "A_RANDOM_STRING_OF_32_PLUS_CHARACTERS"`
3. Apply migrations: `dotnet ef database update --project EMS.Infrastructure --startup-project EMS.API`
4. Run `EMS.API` and open `https://localhost:7093/swagger`.

## Design assumptions
- Salary: 30-day month. Absent = full day deducted, HalfDay = half day, Leave = paid.
- Employees are soft-deleted (`IsActive = false`) to keep history.
- Bulk create is all-or-nothing (one transaction).
- Validation is called manually (FluentValidation); errors are handled by a global exception handler.
