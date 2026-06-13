# Hospital Management System
**Service Oriented Architecture – Final Project**
Redona Rasimi | South East European University | 2026

---

## Overview
A RESTful Web API for managing core hospital operations: patient records, doctor directory, and appointment scheduling. Built with ASP.NET Core (.NET 10), Entity Framework Core, JWT Authentication, and SQL Server.

## Architecture
The project follows a layered architecture with clear separation of concerns:

```
Controllers  →  Services  →  Repositories  →  DbContext (SQL Server)
     ↑               ↑
   DTOs          AutoMapper
```

- **Models** – EF Core entities (Patient, Doctor, Appointment, User)
- **DTOs** – Data Transfer Objects for input/output (never expose raw entities)
- **Repositories** – Interface + implementation, handles all DB access with `AsNoTracking()` for read queries
- **Services** – Business logic layer (e.g., conflict detection for appointments, validation)
- **Controllers** – Thin layer: validate input, call service, return HTTP responses

## Technologies
| Technology | Purpose |
|---|---|
| ASP.NET Core 10 | Web API framework |
| Entity Framework Core 10 | ORM / database access |
| SQL Server (MSSQLSERVER01) | Database |
| AutoMapper 13 | Entity ↔ DTO mapping |
| JWT Bearer | Authentication |
| BCrypt.Net | Password hashing |
| Swagger / Swashbuckle | API documentation & testing UI |
| xUnit + NSubstitute | Unit testing |

## Setup Instructions

### Prerequisites
- Visual Studio 2022 (v17.12+)
- .NET 10 SDK
- SQL Server (MSSQLSERVER01) or adjust connection string in `appsettings.json`

### Running the Project
1. Open `HospitalManagementSystem.sln` in Visual Studio 2022
2. The database and tables are created automatically on first run (EF Core migrations)
3. Press **F5** or click **Run** — Swagger UI opens at `https://localhost:{port}`

### First Login
A default admin user is seeded automatically:
- **Username:** `admin`
- **Password:** `Admin@123`

Use the `POST /api/auth/login` endpoint to get a JWT token, then click **Authorize** in Swagger and paste: `Bearer <your_token>`

## API Endpoints

### Auth
| Method | Endpoint | Access |
|---|---|---|
| POST | /api/auth/login | Public |
| POST | /api/auth/register | Admin only |

### Patients
| Method | Endpoint | Access |
|---|---|---|
| GET | /api/patients | Admin, Doctor |
| GET | /api/patients/{id} | Admin, Doctor, Patient |
| POST | /api/patients | Admin |
| PUT | /api/patients/{id} | Admin, Doctor |
| DELETE | /api/patients/{id} | Admin |

### Doctors
| Method | Endpoint | Access |
|---|---|---|
| GET | /api/doctors | Public |
| GET | /api/doctors/{id} | Public |
| POST | /api/doctors | Admin |
| PUT | /api/doctors/{id} | Admin |
| DELETE | /api/doctors/{id} | Admin |

### Appointments
| Method | Endpoint | Access |
|---|---|---|
| GET | /api/appointments | Admin, Doctor |
| GET | /api/appointments/{id} | Admin, Doctor, Patient |
| GET | /api/appointments/patient/{id} | Admin, Doctor, Patient |
| GET | /api/appointments/doctor/{id} | Admin, Doctor |
| POST | /api/appointments | Admin, Doctor, Patient |
| PUT | /api/appointments/{id} | Admin, Doctor |
| PATCH | /api/appointments/{id}/cancel | Admin, Doctor, Patient |

## Business Rules
- Appointments must be scheduled in the future
- A doctor cannot have two appointments within 30 minutes of each other
- Patients and Doctors are soft-deleted (IsActive = false), not removed from DB
- Only Admins can register new users

## Running Tests
In Visual Studio: **Test → Run All Tests**
Or via terminal: `dotnet test`
