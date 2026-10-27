# SkillHub

**SkillHub** is a modular workshop and training management platform developed with **C# / .NET 10** following **Clean Architecture** principles.

---

## Architecture & Project Structure

The solution is organized into layers, ensuring loose coupling and strict dependency rules (dependencies point inwards):

```text
SkillHub/
├── src/
│   ├── SkillHub.Domain/            # Enterprise business rules & entities (Zero external dependencies)
│   │   ├── Entities/               # Instructor, Workshop, Participant, Enrollment
│   │   ├── Enums/                  # EnrollmentStatus
│   │   └── Exceptions/             # DomainException
│   ├── SkillHub.Application/       # Application business logic & use cases
│   │   ├── Common/Exceptions/      # NotFoundException, ValidationException, ConflictException
│   │   ├── DTOs/                   # Request/Response data transfer objects
│   │   ├── Interfaces/             # Repositories & Service abstractions (UnitOfWork, etc.)
│   │   └── UseCases/               # Workshop, Participant & Enrollment application flows
│   ├── SkillHub.Infrastructure/    # External concerns, database & adapters
│   │   ├── Persistence/            # EF Core ApplicationDbContext & Entity Configurations
│   │   │   ├── Configurations/     # Fluent API entity mappings & indexes
│   │   │   └── Migrations/         # EF Core Code First migrations
│   │   ├── Repositories/           # Concrete EF Core repository implementations
│   │   └── Services/               # Infrastructure service implementations (DateTimeProvider)
│   ├── SkillHub.API/               # REST API presentation layer (scaffolded, DI wired)
│   └── SkillHub.Web/               # ASP.NET Core MVC presentation layer (scaffolded)
├── dotnet-tools.json               # Local manifest for dotnet tools (dotnet-ef)
├── SkillHub.slnx                   # Solution file
└── README.md
```

---

## Technologies Actually Used

* **Language & Runtime:** C# 12 / .NET 10 SDK (verified version `10.0.113`)
* **ORM:** Entity Framework Core `10.0.4`
* **Database Driver:** `Npgsql.EntityFrameworkCore.PostgreSQL` `10.0.3`
* **Database Engine:** PostgreSQL (local or containerized, compatible with version 15+)
* **Database Tooling:** Entity Framework Core .NET CLI Tools (`dotnet-ef` `10.0.4`)
* **Source Control:** Git / GitHub

---

## Current Development Status

| Component | Status | Details |
| :--- | :---: | :--- |
| **`SkillHub.Domain`** | Completed | Entities (`Instructor`, `Workshop`, `Participant`, `Enrollment`), rules (capacity, non-overlapping schedules, 24h participant cancellation vs employee cancellation), `EnrollmentStatus` enum and `DomainException`. Verified: compiles with 0 errors and 0 warnings. |
| **`SkillHub.Application`** | Completed | DTOs, Repository interfaces, Service contracts (`IDateTimeProvider`, `ICurrentUserService`, `IAuthService`, `IEmailService`, `IExcelImportService`, `IPdfService`) and Use Cases. Verified: compiles with 0 errors and 0 warnings. |
| **`SkillHub.Infrastructure`** | Persistence Completed | `ApplicationDbContext` implementing `IUnitOfWork`, 4 entity configurations with unique constraints (`Workshop.Code`, `Instructor.Email`, `Participant.Email`), composite performance indexes, 4 concrete repositories (`WorkshopRepository`, `ParticipantRepository`, `EnrollmentRepository`, `InstructorRepository`), `DateTimeProvider`, `DependencyInjection.cs`, and initial EF Core migration `InitialCreate`. Verified: compiles with 0 errors and 0 warnings. |
| **`SkillHub.API`** | In Progress | Scaffolded Web API with OpenAPI/Swagger enabled. Dependency injection wired to `SkillHub.Infrastructure` via `AddInfrastructureServices`. Endpoints implementation pending. |
| **`SkillHub.Web`** | Scaffolded | Base MVC project created. Razor views and integration pending. |

---

## Getting Started: Project Initialization

This section provides verified instructions to set up, build, configure, and initialize the SkillHub platform on your local environment.

### Prerequisites

Verify that the following tools are installed on your workstation:

1. **.NET SDK 10.0+**:
   ```bash
   dotnet --version
   # Expected output: 10.0.113 or higher
   ```
2. **Git**:
   ```bash
   git --version
   ```
3. **PostgreSQL 15+**:
   Ensure PostgreSQL is installed and running locally, or available via Docker.
   ```bash
   # On Linux systemd:
   systemctl is-active postgresql
   # Or verify port connection:
   pg_isready -h localhost -p 5432
   ```
4. **Local .NET EF Tools (`dotnet-ef`)**:
   The repository includes a local tool manifest (`dotnet-tools.json`). You can restore and verify the EF Core CLI tool with:
   ```bash
   dotnet tool restore
   dotnet dotnet-ef --version
   # Expected output: 10.0.4
   ```

### Clone the Repository

Clone the project from GitHub and navigate into the root directory:

```bash
git clone git@github.com:648leonela/Prueba-de-Desempe-o-SkillHub.git
cd Prueba-de-Desempe-o-SkillHub
```

*(If using HTTPS: `git clone https://github.com/648leonela/Prueba-de-Desempe-o-SkillHub.git`)*

### Restore and Build

Restore all NuGet packages and build the entire solution:

```bash
# Restore local tools
dotnet tool restore

# Restore NuGet dependencies
dotnet restore

# Build the complete solution
dotnet build
```

Currently, all business logic and persistence components compile with **0 errors and 0 warnings**.

To run automated tests (test suites will be implemented in the testing phase):
```bash
dotnet test
```

### Configure PostgreSQL

SkillHub uses PostgreSQL via Entity Framework Core. To prevent credential leaks in version control, database connection strings are configured using ASP.NET Core environment variables.

1. **Create the target database in PostgreSQL**:
   Using `psql` or your database client (e.g., pgAdmin, DBeaver):
   ```sql
   CREATE DATABASE skillhub_db;
   ```

2. **Set the environment variable (Recommended for Development & Production)**:
   Define `ConnectionStrings__DefaultConnection` in your shell or deployment profile (replace with your actual PostgreSQL user, password, and host):
   ```bash
   export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=skillhub_db;Username=postgres;Password=YOUR_SECRET_PASSWORD"
   ```

3. **Verify the configuration safely**:
   Ensure the variable is exported without printing sensitive passwords in plain text:
   ```bash
   python3 -c "import os; cs = os.environ.get('ConnectionStrings__DefaultConnection', ''); print('Connection configured:', bool(cs), '| Database:', cs.split('Database=')[-1].split(';')[0] if 'Database=' in cs else 'None')"
   ```

### Initialize the Database

The project includes the verified initial Code First migration: `20261027223007_InitialCreate`.

1. **Apply migrations to the configured database**:
   Once your PostgreSQL connection string is set in `ConnectionStrings__DefaultConnection`, execute:
   ```bash
   dotnet dotnet-ef database update \
     --project src/SkillHub.Infrastructure/SkillHub.Infrastructure.csproj \
     --startup-project src/SkillHub.API/SkillHub.API.csproj
   ```

2. **Verify the resulting database schema**:
   Connect to `skillhub_db` and verify the created tables:
   ```sql
   \dt
   -- Expected tables:
   -- "Instructors"
   -- "Workshops"
   -- "Participants"
   -- "Enrollments"
   -- "__EFMigrationsHistory"
   ```

3. **Generating future migrations** *(Only when modifying entities in Domain)*:
   ```bash
   dotnet dotnet-ef migrations add <MigrationName> \
     --project src/SkillHub.Infrastructure/SkillHub.Infrastructure.csproj \
     --startup-project src/SkillHub.API/SkillHub.API.csproj \
     --output-dir Persistence/Migrations
   ```

### Run the Application

#### Starting the REST API:
```bash
dotnet run --project src/SkillHub.API/SkillHub.API.csproj
```

* **Identify local ports:** The console output displays the active listening endpoints (e.g., `http://localhost:5000` or `https://localhost:5001`).
* **OpenAPI / Swagger documentation:** When running in the Development environment, access OpenAPI documentation at:
  ```text
  http://localhost:5000/openapi/v1.json
  ```
* **Current functional scope:** The API host successfully builds, registers infrastructure and dependency injection, and connects to PostgreSQL. Dedicated REST controller endpoints for participant registration, workshop management, and enrollments will be exposed in the subsequent API phase.

### Troubleshooting

* **`MSB3277` Dependency version conflict warnings:**
  * Ensure EF Core packages match the installed runtime. In this project, `Microsoft.EntityFrameworkCore.Design` (`10.0.4`) and `Npgsql.EntityFrameworkCore.PostgreSQL` (`10.0.3`) are aligned on the `10.0.4` dependency tree.
* **`dotnet-ef: command not found`:**
  * Run `dotnet tool restore` in the solution root to activate the local `dotnet-ef` tool from `dotnet-tools.json`.
* **PostgreSQL `Peer authentication failed` / `Connection refused`:**
  * Ensure PostgreSQL service is active (`systemctl status postgresql`).
  * Verify that `ConnectionStrings__DefaultConnection` uses TCP connection parameters (`Host=localhost;Port=5432;...`) rather than relying on local peer sockets.
* **Port in use (`EADDRINUSE`):**
  * If port 5000/5001 is already bound by another process, customize the launch profile in `src/SkillHub.API/Properties/launchSettings.json` or pass `--urls "http://localhost:5050"`.

### Development Workflow

* **Build verification:** `dotnet build`
* **Code First migrations:** Handled in `SkillHub.Infrastructure/Persistence/Migrations/`
* **Pending development phases:**
  1. API REST Controllers and Middleware (`SkillHub.API`)
  2. ASP.NET Core Identity & JWT Authentication (`IAuthService`)
  3. Excel Workshop Importer (`IExcelImportService`)
  4. PDF Certificate Generation (`IPdfService`)
  5. Automated Testing (Unit and Integration tests)
  6. Web Presentation Layer (`SkillHub.Web`)

---

## Author

* **Developer:** Leonela (`648leonela`)
* **Repository:** [https://github.com/648leonela/Prueba-de-Desempe-o-SkillHub](https://github.com/648leonela/Prueba-de-Desempe-o-SkillHub)
