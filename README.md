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
│   │   └── Persistence/            # EF Core ApplicationDbContext & Entity Configurations
│   ├── SkillHub.API/               # REST API presentation layer (scaffolded)
│   └── SkillHub.Web/               # ASP.NET Core MVC presentation layer (scaffolded)
└── README.md
```

---

## Technologies Actually Used

* **Language & Runtime:** C# 12 / .NET 10 SDK (`net10.0`)
* **ORM:** Entity Framework Core `10.0.4`
* **Database Driver:** `Npgsql.EntityFrameworkCore.PostgreSQL` `10.0.3`
* **Source Control:** Git / GitHub

---

## Current Development Status

| Project | Status | Details |
| :--- | :---: | :--- |
| **`SkillHub.Domain`** | Completed | Entities (`Instructor`, `Workshop`, `Participant`, `Enrollment`), rules (capacity, non-overlapping schedules, 24h participant cancellation vs employee cancellation), `EnrollmentStatus` enum and `DomainException`. Compiles with 0 errors and 0 warnings. |
| **`SkillHub.Application`** | Completed | DTOs, Repository interfaces, Service contracts (`IDateTimeProvider`, `ICurrentUserService`, `IAuthService`, `IEmailService`, `IExcelImportService`, `IPdfService`) and Use Cases. Compiles with 0 errors and 0 warnings. |
| **`SkillHub.Infrastructure`** | In Progress | **Stage 1 Completed:** EF Core persistence foundation, `ApplicationDbContext`, entity configurations with unique constraints (`Workshop.Code`, `Instructor.Email`, `Participant.Email`), relationships and performance indexes. Compiles with 0 errors and 0 warnings. Pending: Repositories implementation, Identity/JWT, Excel import & PDF services. |
| **`SkillHub.API`** | Scaffolded | Base Web API project created; controllers and DI wiring pending. |
| **`SkillHub.Web`** | Scaffolded | Base MVC project created; views and client integration pending. |

---

## Building the Solution

### Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/)

### Compilation
To build the solution and verify all projects:

```bash
# Build the entire solution
dotnet build

# Or build individual layers
dotnet build src/SkillHub.Domain/SkillHub.Domain.csproj
dotnet build src/SkillHub.Application/SkillHub.Application.csproj
dotnet build src/SkillHub.Infrastructure/SkillHub.Infrastructure.csproj
```

*Note: Database configuration, migrations execution, and runtime startup instructions will be documented as the infrastructure and presentation layers are integrated.*

---

## Author

* **Developer:** Leonela (`648leonela`)
* **Repository:** [https://github.com/648leonela/Prueba-de-Desempe-o-SkillHub](https://github.com/648leonela/Prueba-de-Desempe-o-SkillHub)
