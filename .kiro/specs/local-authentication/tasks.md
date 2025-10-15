# Implementation Plan

- [x] 1. Setup JWT Infrastructure

  - สร้าง JWT token service และ configuration พื้นฐาน
  - _Requirements: 4.1, 4.2, 4.6_

- [x] 1.1 Create JWT configuration in appsettings.json

  - เพิ่ม JwtSettings section ใน appsettings.json และ appsettings.Development.json
  - กำหนด SecretKey, Issuer, Audience, ExpirationHours
  - _Requirements: 4.1, 4.6_

- [x] 1.2 Implement IJwtTokenService interface

  - สร้าง interface ใน src/Application/Common/Interfaces/IJwtTokenService.cs
  - กำหนด methods: GenerateToken, ValidateToken, GetTokenExpiration
  - _Requirements: 4.1, 4.2_

- [x] 1.3 Implement JwtTokenService class

  - สร้าง class ใน src/Infrastructure/Identity/JwtTokenService.cs
  - Implement การสร้าง JWT token พร้อม claims (userId, email, username, roles)
  - Implement token validation logic
  - _Requirements: 4.1, 4.2, 4.6_

- [x] 1.4 Update DependencyInjection.cs for JWT authentication

  - แก้ไข src/Infrastructure/DependencyInjection.cs
  - ลบ OpenID Connect configuration
  - เพิ่ม JWT Bearer authentication configuration
  - Register IJwtTokenService
  - _Requirements: 3.1, 4.1, 7.1_

- [x] 2. Update Domain and Database Schema

  - อัพเดต ApplicationUser entity และสร้าง migration
  - _Requirements: 9.1, 10.1_

- [x] 2.1 Update ApplicationUser entity

  - แก้ไข src/Domain/Entities/ApplicationUser.cs
  - เพิ่ม properties: RequirePasswordChange, LastPasswordChangeDate
  - _Requirements: 9.1, 9.2_

- [x] 2.2 Create database migration

  - สร้าง EF Core migration สำหรับ ApplicationUser changes
  - รัน migration เพื่ออัพเดต database schema
  - _Requirements: 9.1_

- [x] 3. Implement Authentication Commands and Queries

  - สร้าง CQRS commands และ queries สำหรับ authentication
  - _Requirements: 1.1, 1.2, 1.3, 2.1, 2.2_

- [x] 3.1 Create LoginCommand and Handler

  - สร้าง src/Application/Authentication/Commands/Login/LoginCommand.cs
  - สร้าง LoginCommandHandler ที่ validate credentials และสร้าง JWT token
  - สร้าง LoginDto และ LoginResponseDto
  - _Requirements: 1.1, 1.2, 1.3, 4.1_

- [x] 3.2 Create RegisterCommand and Handler

  - สร้าง src/Application/Authentication/Commands/Register/RegisterCommand.cs
  - สร้าง RegisterCommandHandler ที่สร้าง user ใหม่
  - สร้าง RegisterDto พร้อม validation
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 3.3 Create RefreshTokenCommand and Handler

  - สร้าง src/Application/Authentication/Commands/RefreshToken/RefreshTokenCommand.cs
  - Implement logic สำหรับ refresh JWT token
  - _Requirements: 4.3_

- [x] 3.4 Update GetCurrentUserQuery

  - แก้ไข src/Application/Authentication/Queries/GetApplicationUserProfileQuery.cs
  - อัพเดตให้ทำงานกับ JWT authentication
  - _Requirements: 1.1, 4.2_

- [x] 4. Update Identity Service

  - เพิ่ม methods ใหม่ใน IIdentityService และ IdentityService
  - _Requirements: 1.1, 1.2, 2.1, 10.1, 10.2_

- [x] 4.1 Update IIdentityService interface

  - แก้ไข src/Application/Common/Interfaces/IIdentityService.cs
  - เพิ่ม methods: AuthenticateAsync, RegisterUserAsync, FindByEmailOrUsernameAsync
  - _Requirements: 1.1, 2.1_

- [x] 4.2 Implement new methods in IdentityService

  - แก้ไข src/Infrastructure/Identity/IdentityService.cs
  - Implement AuthenticateAsync สำหรับ validate credentials
  - Implement RegisterUserAsync สำหรับสร้าง user ใหม่
  - Implement FindByEmailOrUsernameAsync
  - _Requirements: 1.1, 1.2, 2.1, 10.1, 10.2_

- [x] 5. Create Authentication Endpoints

  - สร้าง API endpoints สำหรับ authentication
  - _Requirements: 1.1, 1.4, 2.1, 2.6_

- [x] 5.1 Create AuthenticationEndpoint class

  - สร้าง src/Web/Endpoints/AuthenticationEndpoint.cs
  - Implement POST /api/auth/login endpoint
  - Implement POST /api/auth/register endpoint
  - Implement POST /api/auth/refresh-token endpoint
  - Implement GET /api/auth/me endpoint (require authorization)
  - _Requirements: 1.1, 1.4, 2.1, 2.6_

- [x] 5.2 Update existing AuthenEndpoint

  - แก้ไข src/Web/Endpoints/AuthenEndpoint.cs
  - อัพเดตให้ทำงานกับ JWT authentication
  - _Requirements: 1.1_

- [x] 6. Implement Frontend Login Page

  - สร้างหน้า login ใหม่พร้อม UI และ logic
  - _Requirements: 1.1, 1.2, 1.3, 5.1, 5.5_

- [x] 6.1 Create Login page component

  - สร้าง src/client_web/src/pages/login.vue (อาจมีอยู่แล้ว - แก้ไข)
  - สร้างฟอร์ม login ด้วย Vuetify components
  - เพิ่ม fields: email/username, password
  - เพิ่ม "Remember me" checkbox
  - เพิ่มลิงก์ไปหน้า register
  - _Requirements: 1.1, 5.1, 5.5_

- [x] 6.2 Implement login form validation

  - เพิ่ม validation rules สำหรับ email/username และ password
  - แสดง error messages เป็นภาษาไทย
  - _Requirements: 1.2, 1.3, 5.4_

- [x] 6.3 Implement login submission logic

  - เรียก auth store login action
  - จัดการ loading state
  - จัดการ error responses
  - Redirect หลัง login สำเร็จ
  - _Requirements: 1.1, 1.4, 5.5_

- [x] 7. Implement Frontend Register Page

  - สร้างหน้า register ใหม่พร้อม UI และ logic
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 5.2, 5.3, 5.4, 5.5_

- [x] 7.1 Create Register page component

  - สร้าง src/client_web/src/pages/register.vue (อาจมีอยู่แล้ว - แก้ไข)
  - สร้างฟอร์ม register ด้วย Vuetify components
  - เพิ่ม fields: email, username, password, confirm password, firstName, lastName
  - เพิ่มลิงก์กลับไปหน้า login
  - _Requirements: 2.1, 5.2_

- [x] 7.2 Implement register form validation

  - เพิ่ม validation rules สำหรับทุก fields
  - ตรวจสอบ password strength
  - ตรวจสอบ password และ confirm password ตรงกัน
  - แสดง error messages เป็นภาษาไทย
  - _Requirements: 2.3, 2.4, 2.5, 5.4_

- [x] 7.3 Implement register submission logic

  - เรียก auth store register action
  - จัดการ loading state
  - จัดการ error responses (email ซ้ำ, password ไม่ผ่าน)
  - Redirect ไปหน้า login หลัง register สำเร็จ
  - _Requirements: 2.2, 2.6, 5.5_

- [x] 8. Update Auth Store

  - แก้ไข auth store ให้ทำงานกับ JWT authentication
  - _Requirements: 1.1, 1.4, 1.5, 2.1, 4.2, 5.6_

- [x] 8.1 Remove OpenID Connect logic from auth store

  - แก้ไข src/client_web/src/stores/auth.ts
  - ลบ methods: restoreData, objEmployeeByUsernameAD
  - ลบ OpenID related state properties
  - ลบการเรียก ntiportal APIs
  - _Requirements: 3.2, 3.3_

- [x] 8.2 Implement new login action

  - สร้าง login action ที่เรียก POST /api/auth/login
  - เก็บ JWT token ใน localStorage
  - อัพเดต state จาก response
  - _Requirements: 1.1, 1.4, 5.6_

- [x] 8.3 Implement new register action

  - สร้าง register action ที่เรียก POST /api/auth/register
  - จัดการ success/error responses
  - _Requirements: 2.1, 2.2_

- [x] 8.4 Implement new logout action

  - แก้ไข logout action
  - ลบการเรียก ntiportal revocation endpoint
  - Clear token และ state
  - Redirect ไปหน้า login
  - _Requirements: 1.5, 3.2_

- [x] 8.5 Implement token restoration logic

  - สร้าง method สำหรับ restore session จาก localStorage token
  - Validate token expiration
  - เรียก GET /api/auth/me เพื่อดึงข้อมูล user
  - _Requirements: 1.6, 4.2_

- [x] 9. Update Router and Navigation Guards

  - อัพเดต router configuration และ guards
  - _Requirements: 1.6, 3.1, 3.2_

- [x] 9.1 Remove OpenId router file

  - ลบ src/client_web/src/plugins/router/OpenId.ts
  - _Requirements: 3.1_

- [x] 9.2 Update router index.ts

  - แก้ไข src/client_web/src/plugins/router/index.ts
  - ลบ OpenId routes import
  - อัพเดต beforeEach guard ให้ตรวจสอบ JWT token
  - ตรวจสอบ token expiration
  - Redirect ไป /login ถ้า token หมดอายุหรือไม่มี
  - _Requirements: 1.6, 4.3_

- [x] 9.3 Update routes.ts

  - แก้ไข src/client_web/src/plugins/router/routes.ts
  - ตรวจสอบว่ามี /login และ /register routes
  - ลบ OpenID callback routes ถ้ามี
  - _Requirements: 3.1_

- [x] 10. Update Frontend Constants and Configuration

  - อัพเดต constants และลบ OpenID configuration
  - _Requirements: 3.1, 3.4, 7.4_

- [x] 10.1 Update constants.ts

  - แก้ไข src/client_web/src/constants.ts
  - ลบ: PortalOpenId, ClientId, ClientSecret, code_verifier, code_challenge
  - เพิ่ม: AUTH_ENDPOINTS object
  - _Requirements: 3.4, 7.4_

- [x] 10.2 Remove OpenID login page

  - ลบ src/client_web/src/views/pages/OpenIds/LoginOpenId.vue
  - _Requirements: 3.2_

- [x] 11. Setup MinIO Docker Container

  - เพิ่ม MinIO service ใน docker-compose และอัพเดต configuration
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 11.1 Add MinIO service to docker-compose.yml

  - แก้ไข docker-compose.yml
  - เพิ่ม minio service configuration
  - เพิ่ม minio_data volume
  - กำหนด ports 9000 (API) และ 9001 (Console)
  - _Requirements: 6.1, 6.2, 6.5_

- [x] 11.2 Update application environment variables

  - แก้ไข docker-compose.yml
  - ลบ MinIO external server environment variables
  - เพิ่ม MinIO local container environment variables
  - อัพเดต depends_on ให้รวม minio
  - _Requirements: 6.3, 7.3_

- [x] 11.3 Update MinIOService configuration

  - แก้ไข src/Infrastructure/MinIOService.cs (ถ้าจำเป็น)
  - ตรวจสอบว่า endpoint configuration ถูกต้อง
  - _Requirements: 6.3_

- [x] 11.4 Update appsettings.json for MinIO

  - แก้ไข src/Web/appsettings.json
  - อัพเดต MinIO configuration section
  - _Requirements: 6.3, 7.3_

- [x] 12. Remove Line Bot Integration

  - ลบ Line Bot related code และ dependencies
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6_

- [x] 12.1 Remove Line Bot API calls from CheckInCheckOut commands

  - แก้ไข src/Application/CheckInCheckOuts/Commands/CreateCheckInCheckOutForLineCommand.cs
  - ลบการเรียก ntiportal API สำหรับ Line user ID
  - จัดการ error case เมื่อไม่มี Line integration
  - _Requirements: 8.2, 8.5_

- [x] 12.2 Remove Line Bot API calls from CheckInCheckOut queries

  - แก้ไข src/Application/CheckInCheckOuts/Queries/CheckLineUserIdCheckInCheckOutToDayQuery.cs
  - ลบการเรียก ntiportal API สำหรับ Line user ID
  - _Requirements: 8.2, 8.5_

- [x] 12.3 Update CheckInCheckOut endpoints

  - แก้ไข src/Web/Endpoints/CheckInCheckOutEndpoint.cs
  - ลบ Line-related endpoints ถ้ามี
  - _Requirements: 8.1_

- [x] 13. Remove ntiportal References

  - ลบ references ทั้งหมดที่ชี้ไปยัง ntiportal
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6_

- [x] 13.1 Remove ntiportal from appsettings.json

  - แก้ไข src/Web/appsettings.json
  - ลบ OpenIDConnectSettings section
  - _Requirements: 3.1, 7.1_

- [x] 13.2 Remove ntiportal from docker-compose files

  - แก้ไข docker-compose.yml และ docker-compose-nti.yml
  - ลบ Authority environment variable
  - _Requirements: 3.1, 7.2_

- [x] 13.3 Remove ntiportal from ShareActivityPlan command

  - แก้ไข src/Application/ActivityPlans/Commands/ShareActivityPlan/ShareActivityPlanCommand.cs
  - ลบการสร้าง ntiportal authorize URL
  - สร้าง local share link แทน
  - _Requirements: 3.4, 3.6_

- [x] 13.4 Remove ntiportal from ShareDialog component

  - แก้ไข src/client_web/src/views/AppointmentPlan/ShareDialog.vue
  - ลบ openIdShareLink logic
  - ใช้ direct share link เท่านั้น
  - _Requirements: 3.4_

- [x] 14. Create Data Migration Script

  - สร้าง script สำหรับ migrate users เดิม
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_

- [x] 14.1 Create migration command for existing users

  - สร้าง src/Application/Authentication/Commands/MigrateExistingUsers/MigrateExistingUsersCommand.cs
  - สร้าง default password สำหรับ users ที่ไม่มี password
  - ตั้งค่า RequirePasswordChange = true
  - คง roles และ permissions เดิม
  - _Requirements: 9.1, 9.2, 9.4_

- [x] 14.2 Create migration endpoint

  - สร้าง endpoint ใน AuthenticationEndpoint สำหรับรัน migration
  - ต้อง require Administrator role
  - _Requirements: 9.1_

- [x] 15. Update API Client and Error Handling

  - อัพเดต API client และ error handling
  - _Requirements: 1.2, 1.3, 2.3, 5.4_

- [x] 15.1 Update NSwag client generation

  - รัน NSwag เพื่อ generate client code ใหม่
  - ตรวจสอบว่า authentication endpoints ถูก generate
  - _Requirements: 1.1, 2.1_

- [x] 15.2 Create error handling utilities

  - สร้าง error codes และ messages เป็นภาษาไทย
  - สร้าง utility functions สำหรับ map error codes
  - _Requirements: 1.2, 1.3, 2.3, 5.4_

- [x] 15.3 Update API client interceptor

  - อัพเดต HTTP interceptor ให้ใส่ JWT token ใน Authorization header
  - จัดการ 401 Unauthorized response (redirect to login)
  - _Requirements: 4.2, 4.3_

- [x] 16. Testing and Validation

  - ทดสอบระบบทั้งหมด
  - _Requirements: All_

- [x] 16.1 Test backend authentication endpoints

  - ทดสอบ POST /api/auth/login
  - ทดสอบ POST /api/auth/register
  - ทดสอบ GET /api/auth/me
  - ทดสอบ error cases
  - _Requirements: 1.1, 1.2, 1.3, 2.1, 2.2, 2.3_

- [x] 16.2 Test frontend login flow

  - ทดสอบ login ด้วย email
  - ทดสอบ login ด้วย username
  - ทดสอบ login ด้วย credentials ผิด
  - ทดสอบ validation
  - _Requirements: 1.1, 1.2, 1.3, 5.1, 5.4_

- [x] 16.3 Test frontend register flow

  - ทดสอบ register user ใหม่
  - ทดสอบ register ด้วย email ซ้ำ
  - ทดสอบ password validation
  - ทดสอบ confirm password
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [x] 16.4 Test protected routes and token expiration

  - ทดสอบ redirect ไป login เมื่อไม่มี token
  - ทดสอบ token expiration handling
  - ทดสอบ logout
  - _Requirements: 1.5, 1.6, 4.3_

- [x] 16.5 Test MinIO integration

  - ทดสอบ file upload
  - ทดสอบ file download
  - ทดสอบ MinIO console access
  - _Requirements: 6.1, 6.3, 6.4_

- [x] 16.6 Test data migration

  - ทดสอบ migration script กับ test data
  - ตรวจสอบว่า users เดิม login ได้
  - ตรวจสอบ RequirePasswordChange flag
  - _Requirements: 9.1, 9.2, 9.3, 9.4_

- [ ] 17. Documentation and Cleanup

  - อัพเดต documentation และทำความสะอาดโค้ด
  - _Requirements: All_

- [ ] 17.1 Update README.md

  - อัพเดต setup instructions
  - เพิ่มข้อมูลเกี่ยวกับ JWT authentication
  - เพิ่มข้อมูลเกี่ยวกับ MinIO Docker setup
  - _Requirements: 6.1, 7.6_

- [ ] 17.2 Remove unused dependencies

  - ตรวจสอบและลบ packages ที่ไม่ใช้แล้ว
  - อัพเดต package.json และ .csproj files
  - _Requirements: 3.6, 8.5_

- [ ] 17.3 Code cleanup
  - ลบ commented code
  - ลบ unused imports
  - Format code ตาม project standards
  - _Requirements: All_
