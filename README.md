# Employee Management System (Backend)

A REST API that replaces manual HR record-keeping: manage employees and departments, track daily attendance, generate monthly salaries from attendance, and export PDF/Excel reports. Built with **ASP.NET Core 8** using **Clean Architecture**, with **MySQL** as the database.

**Status:** Backend complete. The React + TypeScript frontend is in progress and will be added to this repository.

## Features
- **Authentication:** register and login with BCrypt-hashed passwords and JWT bearer tokens (8-hour expiry)
- **Employees:** create, view, update and soft-delete, plus **bulk create** (all-or-nothing) and **bulk delete**
- **Departments:** create and list, with live employee counts
- **Attendance:** mark Present / Absent / Leave / HalfDay with one record per employee per day (re-marking corrects the existing record)
- **Salary:** generate a month's salary records for all active employees, with deductions calculated from attendance
- **Reports:** download PDF and Excel reports for the employee directory, departments, attendance and salary
- **Validation and errors:** FluentValidation on every write endpoint and a single global exception handler for consistent error responses

## Tech stack
| Area | Technology |
|------|------------|
| Framework | ASP.NET Core 8 Web API |
| Architecture | Clean Architecture, Repository pattern, Dependency Injection |
| Database | MySQL with Entity Framework Core (Pomelo provider) |
| Auth | JWT Bearer, BCrypt.Net-Next |
| Validation | FluentValidation |
| Reports | QuestPDF (PDF), ClosedXML (Excel) |
| API docs | Swagger / Swashbuckle |

## Architecture
```
EMS.API             Controllers, Program.cs, global exception handler
EMS.Infrastructure  EF Core DbContext, configurations, repositories, JWT, BCrypt, report generators
EMS.Application     DTOs, interfaces, services, validators, custom exceptions
EMS.Domain          Entities and enums (no dependencies)
```
Dependencies point inward only: `API → Infrastructure → Application → Domain`. The Application layer defines interfaces such as `IEmployeeRepository` and `IReportService`, and Infrastructure implements them, so the database or a report library can be swapped without touching business logic.

**Request flow:** `React → Controller → Validator → Service → Repository → DbContext → MySQL`, with responses mapped through DTOs so entities and password hashes are never exposed.

## Setup
1. Install .NET 8 SDK and MySQL.
2. In `EMS.API`, run `dotnet user-secrets init`, then set:
   - `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=ems_db;User=root;Password=YOUR_PASSWORD;"`
   - `dotnet user-secrets set "Jwt:Secret" "A_RANDOM_STRING_OF_32_PLUS_CHARACTERS"`
3. Apply migrations: `dotnet ef database update --project EMS.Infrastructure --startup-project EMS.API`
4. Run `EMS.API` and open `https://localhost:7093/swagger`.

## API overview
All endpoints except register and login require a JWT.
| Area | Endpoints |
|------|-----------|
| Auth | `POST /api/auth/register`, `POST /api/auth/login` |
| Employees | `GET /api/employee`, `GET /api/employee/{id}`, `POST /api/employee`, `PUT /api/employee/{id}`, `DELETE /api/employee/{id}`, `POST /api/employee/bulk`, `DELETE /api/employee/bulk` |
| Departments | `GET /api/department`, `GET /api/department/{id}`, `POST /api/department` |
| Attendance | `POST /api/attendance/mark`, `GET /api/attendance/employee/{id}`, `GET /api/attendance/date/{date}` |
| Salary | `POST /api/salary/generate`, `GET /api/salary/month/{month}/year/{year}`, `GET /api/salary/employee/{id}` |
| Reports | `GET /api/report/employees/{pdf\|excel}`, `/departments/{pdf\|excel}`, `/attendance/{pdf\|excel}?date=`, `/salary/{pdf\|excel}?month=&year=` |

## Design assumptions
- Salary: 30-day month. Absent = full day deducted, HalfDay = half day, Leave = paid.
- Employees are soft-deleted (`IsActive = false`) to keep history.
- Bulk create is all-or-nothing (one transaction).
- Validation is called manually (FluentValidation); errors are handled by a global exception handler.

## Author
**Joydev Chand**
GitHub: [@meJoydev](https://github.com/meJoydev)
