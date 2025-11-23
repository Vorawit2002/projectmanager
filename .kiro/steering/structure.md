# โครงสร้างโปรเจกต์

## ภาพรวม
โปรเจกต์ใช้สถาปัตยกรรม Clean Architecture แบ่งเป็น layers ที่แยกความรับผิดชอบชัดเจน

## โครงสร้าง Solution

```
ProjectManagement/
├── src/                          # Source code หลัก
│   ├── Domain/                   # Domain Layer (Core Business Logic)
│   ├── Application/         # Use cases, interfaces, CQRS handlers
│   ├── Infrastructure/      # External concerns (DB, Email, Storage)
│   ├── Web/                 # API endpoints, Program.cs
│   └── client_web/          # Vue.js frontend application
├── tests/
│   ├── Domain.UnitTests/
│   ├── Application.UnitTests/
│   ├── Application.FunctionalTests/
│   └── Infrastructure.IntegrationTests/
├── infra/                   # Azure Bicep templates
├── docs/                    # Documentation
└── nginx/                   # nginx configuration
```

## Clean Architecture Layers

### 1. Domain Layer (`src/Domain/`)
**ไม่มี dependencies ภายนอก** - เป็น core ของระบบ

```
Domain/
├── Common/                  # Base classes
│   ├── BaseEntity.cs
│   ├── BaseAuditableEntity.cs
│   └── BaseEvent.cs
├── Entities/                # Domain entities
│   ├── ActivityPlan.cs
│   ├── Project.cs
│   ├── Organization.cs
│   ├── Employee.cs
│   └── ...
├── Enums/                   # Domain enumerations
├── ValueObjects/            # Value objects (Colour, etc.)
├── Constants/               # Domain constants (Roles, Policies)
└── Exceptions/              # Domain exceptions
```

**หลักการ:**
- Entities ต้องสืบทอดจาก `BaseEntity` หรือ `BaseAuditableEntity`
- ใช้ Value Objects สำหรับ concepts ที่ไม่มี identity
- Domain events สืบทอดจาก `BaseEvent`

### 2. Application Layer (`src/Application/`)
**Dependencies:** Domain only

```
Application/
├── Common/
│   ├── Behaviours/          # MediatR pipeline behaviors
│   ├── Interfaces/          # Service interfaces
│   ├── Models/              # DTOs, Result types
│   ├── Mappings/            # AutoMapper profiles
│   └── Security/            # Authorization attributes
├── [FeatureName]/           # Feature folders (CQRS)
│   ├── Commands/
│   │   ├── CreateXCommand.cs
│   │   └── CreateXCommandValidator.cs
│   └── Queries/
│       ├── GetXQuery.cs
│       └── XDto.cs
└── DependencyInjection.cs
```

**หลักการ:**
- ใช้ **CQRS pattern** - แยก Commands และ Queries
- แต่ละ Command/Query มี Validator ของตัวเอง (FluentValidation)
- ใช้ MediatR สำหรับ request handling
- DTOs ใช้สำหรับ data transfer เท่านั้น ไม่มี business logic

**Feature Folders:**
- `ActivityPlans/` - จัดการแผนกิจกรรมและนัดหมาย
- `Projects/` - จัดการโครงการ
- `Organizations/` - จัดการองค์กรลูกค้า
- `Employees/` - จัดการพนักงาน
- `CheckInCheckOuts/` - เช็คอิน-เช็คเอาท์
- `PushNotifications/` - การแจ้งเตือน
- `Authentication/` - การยืนยันตัวตน
- และอื่นๆ

### 3. Infrastructure Layer (`src/Infrastructure/`)
**Dependencies:** Application, Domain

```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── ApplicationDbContextInitialiser.cs
│   ├── Configurations/      # EF Core entity configurations
│   ├── Interceptors/        # EF Core interceptors
│   └── Migrations/          # EF Core migrations
├── Identity/
│   ├── IdentityService.cs
│   ├── JwtTokenService.cs
│   └── Authorization/       # Permission handlers
├── FileStorage/
│   └── StoragePathService.cs
├── EmailSenderService.cs
├── MinIOService.cs
├── PushNotificationService.cs
├── SmartCardService.cs
└── DependencyInjection.cs
```

**หลักการ:**
- Implement interfaces ที่ define ใน Application layer
- Entity configurations แยกไฟล์ (IEntityTypeConfiguration)
- ใช้ Interceptors สำหรับ cross-cutting concerns (Audit, Domain Events)

### 4. Web Layer (`src/Web/`)
**Dependencies:** Application, Infrastructure

```
Web/
├── Endpoints/               # Minimal API endpoints
│   ├── ActivityPlanEndpoint.cs
│   ├── ProjectEndpoint.cs
│   └── ...
├── Infrastructure/
│   ├── CustomExceptionHandler.cs
│   ├── EndpointGroupBase.cs
│   └── WebApplicationExtensions.cs
├── Services/
│   └── CurrentUser.cs
├── Pages/                   # Razor Pages (Hangfire login)
├── wwwroot/                 # Static files
├── Program.cs               # Application entry point
└── appsettings.json
```

**หลักการ:**
- ใช้ **Minimal APIs** แทน Controllers
- Endpoints จัดกลุ่มตาม feature (EndpointGroupBase)
- Exception handling แบบ centralized
- CORS configuration สำหรับ frontend

## Frontend Structure (`src/client_web/`)

```
client_web/
├── src/
│   ├── @core/               # Core utilities และ components
│   ├── @layouts/            # Layout components
│   ├── assets/              # Static assets (images, styles)
│   ├── components/          # Reusable Vue components
│   ├── composables/         # Vue composables
│   ├── layouts/             # Page layouts
│   ├── pages/               # Page components
│   ├── plugins/             # Vue plugins (Vuetify, Router, etc.)
│   ├── router/              # Vue Router configuration
│   ├── services/            # API service layer
│   ├── stores/              # Pinia stores
│   ├── utils/               # Utility functions
│   ├── views/               # View components
│   ├── client.ts            # Generated API client (NSwag)
│   ├── App.vue
│   └── main.ts
├── public/                  # Public static files
├── docker/                  # Docker configuration
├── tests/                   # Test guides
├── index.html
├── vite.config.ts
└── package.json
```

**หลักการ:**
- ใช้ **Composition API** (Vue 3)
- State management ด้วย **Pinia stores**
- API calls ผ่าน generated client (`client.ts`) จาก NSwag
- Components แยกตาม feature และ reusability

## Testing Structure

```
tests/
├── Domain.UnitTests/        # Domain logic tests
├── Application.UnitTests/   # Application logic tests
├── Application.FunctionalTests/  # End-to-end API tests
│   ├── BaseTestFixture.cs
│   ├── CustomWebApplicationFactory.cs
│   └── [Feature]/Tests.cs
└── Infrastructure.IntegrationTests/  # Infrastructure tests
```

**หลักการ:**
- Unit tests สำหรับ business logic
- Functional tests ใช้ WebApplicationFactory
- Integration tests ใช้ Testcontainers

## Configuration Files

### Root Level
- `Directory.Build.props` - Shared MSBuild properties
- `Directory.Packages.props` - Central package management
- `global.json` - .NET SDK version
- `.editorconfig` - Code style rules
- `docker-compose.yml` - Local development environment

### Frontend
- `vite.config.ts` - Vite configuration
- `tsconfig.json` - TypeScript configuration
- `.eslintrc.cjs` - ESLint rules
- `.prettierrc.json` - Prettier formatting
- `.stylelintrc.json` - CSS linting

## Naming Conventions

### Backend (C#)
- **Entities:** PascalCase (e.g., `ActivityPlan`, `Organization`)
- **Commands:** `[Verb][Entity]Command` (e.g., `CreateProjectCommand`)
- **Queries:** `Get[Entity][Suffix]Query` (e.g., `GetProjectsWithPaginationQuery`)
- **DTOs:** `[Entity]Dto` หรือ `[Entity]Vm` (ViewModel)
- **Validators:** `[Command/Query]Validator`
- **Interfaces:** `I[Name]` (e.g., `IApplicationDbContext`)

### Frontend (TypeScript/Vue)
- **Components:** PascalCase (e.g., `ActivityCalendar.vue`)
- **Composables:** `use[Name]` (e.g., `useAuth`)
- **Stores:** `use[Name]Store` (e.g., `useActivityStore`)
- **Files:** kebab-case สำหรับ utilities (e.g., `date-utils.ts`)

## Important Patterns

### CQRS (Command Query Responsibility Segregation)
- **Commands** - เปลี่ยนแปลง state (Create, Update, Delete)
- **Queries** - อ่านข้อมูลเท่านั้น (Get, List)

### Repository Pattern
- ใช้ `ApplicationDbContext` โดยตรงใน handlers
- ไม่มี generic repository layer

### Dependency Injection
- Register services ใน `DependencyInjection.cs` ของแต่ละ layer
- ใช้ extension methods: `AddApplicationServices()`, `AddInfrastructureServices()`, `AddWebServices()`

## Data Flow

```
Request → Endpoint → MediatR → Handler → DbContext → Database
                        ↓
                   Validators
                   Behaviors (Logging, Performance, etc.)
```

## File Organization Rules

1. **Feature-based organization** - จัดกลุ่มตาม business feature ไม่ใช่ technical type
2. **One class per file** - ยกเว้น nested classes ที่เกี่ยวข้องกันมาก
3. **Co-locate related files** - Validator อยู่ใกล้ Command/Query
4. **Separate concerns** - Commands, Queries, DTOs แยกไฟล์
          # Application Layer (Use Cases)
│   ├── Infrastructure/           # Infrastructure Layer (External Services)
│   ├── Web/                      # Presentation Layer (API)
│   └── client_web/               # Frontend (Vue 3)
├── tests/                        # Test projects
│   ├── Domain.UnitTests/
│   ├── Application.UnitTests/
│   ├── Application.FunctionalTests/
│   └── Infrastructure.IntegrationTests/
├── docs/                         # เอกสารประกอบ
├── Directory.Packages.props      # Central package management
├── Directory.Build.props         # Build properties
├── global.json                   # .NET SDK version
└── ProjectManagement.sln         # Solution file
```

## Layer แต่ละชั้น

### 1. Domain Layer (`src/Domain/`)
**หน้าที่**: เก็บ business logic หลักและ domain entities ไม่มี dependencies กับ layer อื่น

```
Domain/
├── Common/              # Base classes, interfaces
├── Constants/           # ค่าคงที่
├── Entities/            # Domain entities
├── Enums/              # Enumerations
├── Exceptions/         # Domain exceptions
└── ValueObjects/       # Value objects
```

**หลักการ**:
- ไม่ขึ้นกับ layer อื่นใด
- เป็น pure business logic
- ใช้ MediatR สำหรับ domain events

### 2. Application Layer (`src/Application/`)
**หน้าที่**: Use cases, business rules, CQRS commands/queries

```
Application/
├── Common/
│   ├── Behaviours/      # MediatR pipeline behaviours
│   ├── Interfaces/      # Service interfaces
│   ├── Mappings/        # AutoMapper profiles
│   ├── Models/          # DTOs, ViewModels
│   └── Validators/      # FluentValidation validators
├── [FeatureName]/       # Feature folders (ตามชื่อ entity)
│   ├── Commands/        # CQRS Commands
│   │   ├── Create[Entity]/
│   │   ├── Update[Entity]/
│   │   └── Delete[Entity]/
│   └── Queries/         # CQRS Queries
│       ├── Get[Entity]/
│       └── Get[Entities]List/
└── DependencyInjection.cs
```

**Feature Folders ที่มี**:
- ActivityPlans, ActivityPlanAttachments, ActivityPlanContacts
- Projects, ProjectContacts
- Organizations, OrganizationContacts
- Employees, Departments
- CheckInCheckOuts
- EmailLogs, EmailMessageSettings, SMTPSettings
- PushNotifications, PushSubscriptions
- Authentication, Users

**หลักการ**:
- แต่ละ feature มี folder แยก
- ใช้ CQRS pattern (Commands แยกจาก Queries)
- Commands/Queries มี Validator แยกไฟล์
- ขึ้นกับ Domain layer เท่านั้น

### 3. Infrastructure Layer (`src/Infrastructure/`)
**หน้าที่**: Implementation ของ external services, database, file storage

```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/          # EF Core entity configurations
│   ├── Interceptors/            # EF Core interceptors
│   └── Migrations/              # Database migrations
├── Identity/                    # Identity implementation
├── FileStorage/                 # MinIO file storage
├── Services/                    # Service implementations
│   ├── EmailSenderService.cs
│   ├── PushNotificationService.cs
│   ├── MinIOService.cs
│   └── SmartCardS