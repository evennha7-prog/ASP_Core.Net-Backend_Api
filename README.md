# Notes API — ASP.NET Core Backend

This is the backend REST API for the **Notes Application**, built with **ASP.NET Core Web API (.NET 10)**, **Dapper ORM**, and **SQL Server**.

## Getting Started

### 1. Configure Connection String
Open `appsettings.json` and set your local SQL Server instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=Backend_DB;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
}
```

### 2. Run the Application
```bash
dotnet run
```

The server will start on:
- HTTP: `http://localhost:5291`
- Interactive API Reference (Scalar): `http://localhost:5291/scalar/v1`

> **Automatic Database Creation & Seed Data:**
> When the API launches, it automatically checks if `Backend_DB` and the tables (`Users`, `Notes`) exist. If not, it creates them and seeds a ready-to-test demo user:
> - **Username:** `testadmin`
> - **Password:** `AdminPass123!`

## Architecture
- **Controllers:** `AuthController.cs`, `NotesController.cs`
- **Services:** `AuthService.cs`, `NoteService.cs`
- **Repositories (Dapper):** `UserRepository.cs`, `NoteRepository.cs`
- **Security:** JWT Bearer authentication, strict `UserId` tenant isolation, BCrypt password hashing.
