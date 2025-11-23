# เทคโนโลยีและเครื่องมือ

## Backend Stack

### Framework และ Runtime
- .NET 8.0 (SDK 8.0.100)
- ASP.NET Core 8.0
- C# (ใช้ file-scoped namespaces และ top-level statements)

### สถาปัตยกรรม
- Clean Architecture (จาก Jason Taylor Template v9.0.8)
- CQRS Pattern ด้วย MediatR
- Repository Pattern
- Dependency Injection

### ไลบรารีสำคัญ
- **ORM**: Entity Framework Core 8.0.8 + Npgsql (PostgreSQL)
- **Validation**: FluentValidation
- **Mapping**: AutoMapper
- **Background Jobs**: Hangfire (PostgreSQL storage)
- **Authentication**: ASP.NET Core Identity + JWT Bearer
- **API Documentation**: NSwag (OpenAPI/Swagger)
- **Logging**: Serilog + Seq
- **File Storage**: MinIO
- **Push Notifications**: WebPush
- **LDAP**: Novell.Directory.Ldap.NETStandard
- **Thai ID Card**: ThaiNationalIDCard.NET
- **CSV**: CsvHelper

### การจัดการ Packages
- ใช้ Central Package Management (Directory.Packages.props)
- ไม่ระบุเวอร์ชันใน .csproj แต่ละไฟล์

## Frontend Stack

### Framework และ Runtime
- Vue 3.5.13 (Composition API)
- TypeScript 5.7.2
- Vite 5.4.11

### UI Framework
- Vuetify 3.9.0 (Material Design)
- Materio Admin Template

### ไลบรารีสำคัญ
- **State Management**: Pinia
- **Router**: Vue Router 4
- **Charts**: ApexCharts + vue3-apexcharts
- **Calendar**: FullCalendar
- **Maps**: @fawmi/vue-google-maps, Leaflet
- **Utils**: @vueuse/core, moment.js
- **Notifications**: SweetAlert2
- **Face Recognition**: face-api.js
- **Excel**: xlsx, jszip
- **Icons**: Remixicon, @iconify/vue

### เครื่องมือพัฒนา
- ESLint + TypeScript ESLint
- Stylelint
- Vite DevTools
- Auto Import (unplugin-auto-import)

## Database
- PostgreSQL (ใช้ Npgsql provider)
- Entity Framework Core Migrations

## คำสั่งที่ใช้บ่อย

### Backend (.NET)

#### Build
```bash
dotnet build -tl
```

#### Run Development Server
```bash
cd src/Web
dotnet watch run
```
เข้าถึงได้ที่: https://localhost:5001

#### Run Tests
```bash
dotnet test
```

#### Database Migrations
```bash
# สร้าง migration ใหม่
dotnet ef migrations add <MigrationName> --project src/Infrastructure --startup-project src/Web

# อัพเดท database
dotnet ef database update --project src/Infrastructure --startup-project src/Web
```

#### Scaffold Use Case (Command/Query)
```bash
cd src/Application

# สร้าง Command
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int

# สร้าง Query
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

### Frontend (Vue)

#### Install Dependencies
```bash
cd src/client_web
npm install
```

#### Run Development Server
```bash
npm run dev
```

#### Build for Production
```bash
npm run build
```

#### Preview Production Build
```bash
npm run preview
```

#### Lint และ Fix
```bash
npm run lint
```

#### Type Check
```bash
npm run typecheck
```

## Configuration Files

### Backend
- `appsettings.json` - การตั้งค่าหลัก
- `appsettings.Development.json` - การตั้งค่าสำหรับ Development
- `.editorconfig` - กำหนด code style
- `global.json` - ระบุเวอร์ชัน .NET SDK
- `Directory.Packages.props` - จัดการเวอร์ชัน NuGet packages

### Frontend
- `vite.config.ts` - การตั้งค่า Vite
- `tsconfig.json` - การตั้งค่า TypeScript
- `.eslintrc.cjs` - กฎ ESLint
- `.stylelintrc.json` - กฎ Stylelint
- `.env` - ตัวแปร environment

## API Documentation
- Swagger UI: https://localhost:5001/api
- Hangfire Dashboard: https://localhost:5001/hangfire

## CORS Configuration
อนุญาต origins:
- http://localhost:5173 (Vite dev server)
- https://localhost:5173
- http://localhost:8098
- https://localhost:5001
- https://crm.nti.co.th
# เทคโนโลยีและเครื่องมือ

## สถาปัตยกรรม
โปรเจคนี้ใช้ **Clean Architecture** ตาม template จาก [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 9.0.8

## Backend Stack

### .NET Core
- .NET 8.0 (SDK version 8.0.100)
- ASP.NET Core Web API
- C# with nullable reference types enabled

### ฐานข้อมูล
- **PostgreSQL** - ฐานข้อมูลหลัก
- **Entity Framework Core 8.0.8** - ORM
- **Npgsql** - PostgreSQL provider

### Libraries และ Frameworks สำคัญ
- **MediatR** - CQRS pattern implementation
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Hangfire** - Background job processing (ใช้ PostgreSQL storage)
- **Serilog** - Logging (ส่งไปยัง Seq)
- **NSwag** - OpenAPI/Swagger generation
- **WebPush** - Push notifications
- **MinIO** - Object storage สำหรับไฟล์แนบ
- **JWT Bearer** - Authentication
- **Novell.Directory.Ldap** - LDAP authentication
- **ThaiNationalIDCard.NET** - Smart card reader

### Testing
- **NUnit** - Testing framework
- **FluentAssertions** - Assertion library
- **Moq** - Mocking framework
- **Testcontainers** - Integration testing with containers
- **Respawn** - Database cleanup for tests

## Frontend Stack

### Vue.js 3
- **Vue 3.5.13** - Progressive JavaScript framework
- **TypeScript 5.7.2** - Type-safe JavaScript
- **Vite 5.4.11** - Build tool และ dev server

### UI Framework
- **Vuetify 3.9.0** - Material Design component framework
- **Materio Template** - Admin template

### State Management และ Routing
- **Pinia 2.3.0** - State management
- **Vue Router 4.5.0** - Client-side routing

### Libraries สำคัญ
- **FullCalendar** - Calendar และ scheduling
- **ApexCharts** - Data visualization
- **Leaflet** - Maps integration
- **SweetAlert2** - Alert dialogs
- **Moment.js** - Date/time manipulation
- **XLSX/JSZip** - Excel export
- **face-api.js** - Face recognition

## Infrastructure

### Containerization
- **Docker** และ **Docker Compose**
- **nginx** - Reverse proxy

### Services
- **PostgreSQL 17.2** - Database
- **Redis** - Caching
- **MinIO** - Object storage
- **Seq** - Log aggregation
- **Hangfire Dashboard** - Background job monitoring

### Deployment
- **Azure** - Cloud platform (มี bicep templates ใน infra/)
- **Jenkins** - CI/CD (Jenkinfile.nti)

## คำสั่งที่ใช้บ่อย

### Backend

#### Build
```bash
dotnet build -tl
```

#### Run Development Server
```bash
cd src/Web
dotnet watch run
```
เข้าถึงได้ที่: https://localhost:5001

#### Run Tests
```bash
dotnet test
```

#### Database Migrations
```bash
cd src/Infrastructure
dotnet ef migrations add <MigrationName> --startup-project ../Web
dotnet ef database update --startup-project ../Web
```

#### Scaffold New Use Case
```bash
cd src/Application

# Command
dotnet new ca-usecase --name CreateTodoList --feature-name TodoLists --usecase-type command --return-type int

# Query
dotnet new ca-usecase -n GetTodos -fn TodoLists -ut query -rt TodosVm
```

### Frontend

#### Install Dependencies
```bash
cd src/client_web
npm install
```

#### Run Development Server
```bash
npm run dev
```
เข้าถึงได้ที่: http://localhost:5173

#### Build for Production
```bash
npm run build
```

#### Preview Production Build
```bash
npm run preview
```

#### Lint และ Format
```bash
npm run lint
npm run typecheck
```

### Docker

#### Start All Services
```bash
docker-compose up -d
```

#### Stop All Services
```bash
docker-compose down
```

#### View Logs
```bash
docker-compose logs -f [service-name]
```

## API Documentation
- Swagger UI: https://localhost:5001/api
- Hangfire Dashboard: https://localhost:5001/hangfire

## Ports
- Backend API: 8080 (container), 8800 (host)
- Frontend: 8888 (container), 8098 (host)
- PostgreSQL: 5432
- Redis: 6379
- MinIO: 9000 (API), 9001 (Console)
- Seq: 5341
- nginx: 8081
