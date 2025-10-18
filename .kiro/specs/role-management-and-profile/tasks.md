# แผนการพัฒนา

- [x] 1. ตั้งค่า Role Constants และ Policies

  - สร้างหรืออัพเดท `src/Domain/Constants/Roles.cs` เพื่อเพิ่ม Admin, Manager, User, Viewer roles
  - สร้างหรืออัพเดท `src/Domain/Constants/Policies.cs` เพื่อเพิ่ม authorization policies (CanManageUsers, CanManageMasterData, CanViewMasterData, CanViewDepartmentData, CanViewOwnData, CanModifyData, ReadOnly)
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 8.1, 8.2, 8.4, 8.6_

- [x] 2. สร้าง Database Migration สำหรับ Roles และอัพเดท Employee

  - สร้าง migration เพื่อ seed ข้อมูล AspNetRoles table ด้วย Admin, Manager, User, Viewer roles
  - สร้าง migration เพื่อเพิ่ม ImageProfile column ใน Employee table ถ้ายังไม่มี
  - สร้าง migration เพื่ออัพเดท Roles column ใน Employee table
  - สร้าง seed data สำหรับ default admin user (username: admin, password: Admin@123)
  - _Requirements: 5.5, 3.9, 1.1_

- [x] 3. ปรับปรุง Identity Service ด้วย User Management Methods

  - เพิ่ม GetAllUsersAsync method ใน IIdentityService interface
  - เพิ่ม GetUserByIdAsync method ใน IIdentityService interface
  - เพิ่ม AssignRoleAsync method ใน IIdentityService interface
  - เพิ่ม RemoveRoleAsync method ใน IIdentityService interface
  - เพิ่ม GetUserRolesAsync method ใน IIdentityService interface
  - เพิ่ม GetUsersByDepartmentAsync method ใน IIdentityService interface
  - Implement methods เหล่านี้ใน IdentityService class
  - _Requirements: 5.2, 5.5, 5.6, 5.7, 5.8_

- [x] 4. สร้าง User Management Query Handlers
- [x] 4.1 สร้าง GetAllUsersQuery และ Handler

  - สร้าง GetAllUsersQuery พร้อม SearchTerm, RoleFilter, IsActiveFilter properties
  - สร้าง UserDto พร้อม UserId, Username, Email, FirstName, LastName, ImageProfile, Department, DepartmentId, Roles, IsActive, LastLoginDaten
  - Implement GetAllUsersQueryHandler เพื่อดึงข้อมูล users จาก database พร้อม filters
  - Join กับ Employee table เพื่อดึงข้อมูล department และ profile
  - _Requirements: 5.2, 5.3, 5.10, 5.11_

- [x] 4.2 สร้าง GetUserByIdQuery และ Handler

  - สร้าง GetUserByIdQuery พร้อม UserId property
  - Implement GetUserByIdQueryHandler เพื่อดึงข้อมูล user เดี่ยวพร้อมรายละเอียดทั้งหมด
  - รวมข้อมูล last login date และ role history ถ้ามี
  - _Requirements: 5.12_

- [x] 5. สร้าง Role Assignment Command Handler

  - สร้าง AssignRoleCommand พร้อม UserId และ RoleName properties
  - สร้าง AssignRoleCommandValidator เพื่อ validate role name
  - Implement AssignRoleCommandHandler เพื่อกำหนด role ให้ user
  - อัพเดท Employee.Roles field เมื่อกำหนด role
  - Return success/failure result พร้อมข้อความที่เหมาะสม
  - _Requirements: 5.6, 5.7, 5.8, 5.9_

- [x] 6. สร้าง Data Filtering Service

  - สร้าง IDataFilterService interface พร้อม ApplyRoleBasedFilter และ CanAccessResource methods
  - Implement DataFilterService ใน Infrastructure layer
  - Implement ApplyRoleBasedFilter เพื่อกรอง queries ตาม user role (Admin: ข้อมูลทั้งหมด, Manager: ข้อมูลแผนก, User/Viewer: ข้อมูลตัวเอง)
  - Implement CanAccessResource เพื่อตรวจสอบว่า user สามารถเข้าถึง resource ได้หรือไม่
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 7.8, 7.9, 7.10, 7.11, 7.12_

- [x] 7. สร้าง Authorization Handlers และ Policies

  - สร้าง DepartmentAuthorizationHandler เพื่อตรวจสอบการเข้าถึงตามแผนก
  - สร้าง RoleAuthorizationHandler สำหรับ role-based policies
  - ลงทะเบียน authorization policies ใน DependencyInjection (CanManageUsers, CanManageMasterData, CanViewMasterData, ฯลฯ)
  - กำหนด policy requirements (Admin only, Admin+Manager, ฯลฯ)
  - _Requirements: 6.7, 6.8, 8.3, 8.9, 8.10_

- [x] 8. สร้าง User Management API Endpoints

  - สร้าง GET /api/users endpoint พร้อม [Authorize(Policy = "CanManageUsers")] เพื่อดึงข้อมูล users ทั้งหมด
  - สร้าง GET /api/users/{id} endpoint เพื่อดึงข้อมูล user ตาม id
  - สร้าง POST /api/users/assign-role endpoint เพื่อกำหนด role ให้ user
  - สร้าง DELETE /api/users/{id}/roles/{roleName} endpoint เพื่อลบ role จาก user
  - เพิ่ม error handling และ return status codes ที่เหมาะสม (401, 403, 400)
  - _Requirements: 5.1, 5.2, 5.4, 5.5, 5.6, 5.7, 5.8, 5.9_

- [x] 9. อัพเดท API Endpoints ที่มีอยู่ด้วย Data Filtering

  - อัพเดท Activity Plans endpoints เพื่อใช้ DataFilterService สำหรับ role-based filtering
  - อัพเดท Projects endpoints เพื่อใช้ DataFilterService สำหรับ role-based filtering
  - อัพเดท Employees endpoints เพื่อใช้ DataFilterService สำหรับ role-based filtering
  - อัพเดท Organizations endpoints เพื่อตรวจสอบ CanViewMasterData policy
  - อัพเดท Departments endpoints เพื่อตรวจสอบ CanViewMasterData policy
  - เพิ่ม authorization checks ก่อน Create/Edit/Delete operations
  - _Requirements: 7.3, 7.4, 7.5, 7.6, 7.7, 7.8, 7.12, 8.1, 8.2, 8.3, 8.4, 8.5, 8.9_

- [x] 10. สร้าง Profile Image Upload Endpoint

  - สร้าง POST /api/users/profile-image endpoint เพื่ออัพโหลดรูป profile
  - Validate file type (JPEG, PNG, JPG, GIF) และ size (max 5MB)
  - อัพโหลดรูปไปยัง MinIO storage โดยใช้ MinIOService
  - อัพเดท Employee.ImageProfile ด้วย MinIO URL
  - Return image URL ใน response
  - _Requirements: 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 3.10, 3.12_

- [x] 11. อัพเดท Account Settings Query เพื่อรวมข้อมูลจากการลงทะเบียน

  - อัพเดท GetAccountSettingsQuery เพื่อดึง FirstName, LastName จาก Employee table
  - Fallback ไปใช้ข้อมูล ApplicationUser ถ้าข้อมูล Employee ไม่มี
  - รวม ImageProfile URL ใน response
  - รวม Department, Position, Phone, TitleName ใน response
  - _Requirements: 2.1, 2.2, 2.3_

- [x] 12. สร้าง Account Settings Update Command

  - สร้าง UpdateAccountSettingsCommand พร้อม FirstName, LastName, TitleName, Position, Phone, DepartmentId
  - สร้าง UpdateAccountSettingsCommandValidator
  - Implement UpdateAccountSettingsCommandHandler เพื่ออัพเดท Employee table
  - Return success/failure result พร้อมข้อความที่เหมาะสม
  - _Requirements: 2.4, 2.5, 2.6, 2.7_

- [x] 13. อัพเดท Frontend Role Service

  - อัพเดทหรือสร้าง `src/client_web/src/utils/RoleService.ts` พร้อม role constants (ADMIN, MANAGER, USER, VIEWER)
  - เพิ่ม isAdmin, isManager, isUser, isViewer methods
  - เพิ่ม canAccessMasterData method (return true สำหรับ Admin และ Manager)
  - เพิ่ม canModifyData method (return false สำหรับ Viewer)
  - เพิ่ม canViewDepartmentData method (return true สำหรับ Admin และ Manager)
  - _Requirements: 6.2, 6.3, 6.4, 6.5, 6.6, 8.1, 8.2, 8.4, 8.6_

- [x] 14. อัพเดท Navigation Items ด้วย Roles ใหม่

  - อัพเดท `src/client_web/src/layouts/components/NavItems.vue` เพื่อใช้ role names ใหม่ (Admin, Manager, User, Viewer)
  - เพิ่ม Master Data section พร้อม roles: ['Admin', 'Manager']
  - เพิ่ม Activity Plans, Projects, Check-in/Check-out พร้อม roles: ['Admin', 'Manager', 'User', 'Viewer']
  - เพิ่ม Employees พร้อม roles: ['Admin', 'Manager']
  - เพิ่ม Reports พร้อม roles: ['Admin', 'Manager', 'User', 'Viewer']
  - ลบ Master Data items สำหรับ User และ Viewer roles
  - _Requirements: 6.2, 6.3, 6.4, 6.5, 6.6, 6.9_

- [x] 15. สร้าง Route Guards สำหรับ Role-Based Access

  - สร้างหรืออัพเดท `src/client_web/src/router/guards.ts` พร้อม roleGuard function
  - ตรวจสอบว่า user มี required roles จาก route meta หรือไม่
  - Redirect ไป /not-authorized ถ้า user ไม่มี required role
  - เพิ่ม roleGuard ไปยัง routes ที่ต้องการ specific roles (Master Data, User Management)
  - _Requirements: 6.7, 6.8_

- [x] 16. อัพเดท Auth Store ด้วย Role Management

  - อัพเดท `src/client_web/src/stores/auth.ts` เพื่อจัดการ roles ใหม่ (Admin, Manager, User, Viewer)
  - อัพเดท generateMenu method เพื่อสร้าง menu ตาม roles ใหม่
  - อัพเดท restoreSession เพื่อดึงและเก็บ user roles อย่างถูกต้อง
  - เพิ่ม hasRole method เพื่อตรวจสอบว่า user มี specific role(s) หรือไม่
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 17. สร้างหน้า User Management (Frontend)
- [x] 17.1 สร้าง UserListView Component

  - สร้าง `src/client_web/src/views/MasterData/Users/UserListView.vue`
  - แสดงตาราง users พร้อม columns: รูปโปรไฟล์, ชื่อ, อีเมล, แผนก, Role, สถานะ, Actions
  - เพิ่มฟังก์ชันค้นหาตามชื่อ, อีเมล, แผนก
  - เพิ่มตัวกรองตาม role และสถานะ (Active/Inactive)
  - เพิ่มปุ่ม "กำหนด Role" สำหรับแต่ละ user
  - ดึงข้อมูล users จาก GET /api/users endpoint
  - _Requirements: 5.1, 5.2, 5.3, 5.10, 5.11_

- [x] 17.2 สร้าง AssignRoleDialog Component

  - สร้าง dialog component เพื่อกำหนด role ให้ user
  - แสดง dropdown พร้อม roles ที่มี (Admin, Manager, User, Viewer)
  - เรียก POST /api/users/assign-role endpoint เมื่อ user คลิก "บันทึก"
  - แสดงข้อความ success/error โดยใช้ SweetAlert2
  - Refresh รายการ user หลังจากกำหนด role สำเร็จ
  - _Requirements: 5.4, 5.5, 5.6, 5.7, 5.8, 5.9_

- [x] 17.3 สร้าง UserDetailView Component

  - สร้าง component เพื่อแสดงรายละเอียด user
  - แสดงข้อมูล user ทั้งหมดรวมถึง last login date
  - แสดง role history ถ้ามี
  - เพิ่ม link จากชื่อ user ใน UserListView
  - _Requirements: 5.12_

- [x] 18. อัพเดทหน้า Account Settings (Frontend)
- [x] 18.1 อัพเดท AccountSettingsAccount Component

  - อัพเดท `src/client_web/src/views/pages/account-settings/AccountSettingsAccount.vue` เพื่อดึงข้อมูลจาก API ที่อัพเดทแล้ว
  - แสดง FirstName, LastName, Email, TitleName, Position, Phone, Department จาก Employee table
  - เพิ่มการแสดงรูป profile พร้อมรูปปัจจุบันหรือ default avatar
  - เพิ่มปุ่ม "อัพโหลดรูปใหม่" เพื่อเปิด file picker
  - _Requirements: 2.1, 2.2, 2.3, 3.1_

- [x] 18.2 Implement การอัพโหลดรูป Profile

  - เพิ่ม file input พร้อม accept="image/jpeg,image/png,image/jpg,image/gif"
  - Validate file type และ size (max 5MB) ฝั่ง client
  - แสดง preview ของรูปที่เลือกก่อนอัพโหลด
  - เรียก POST /api/users/profile-image endpoint เพื่ออัพโหลดรูป
  - อัพเดทรูปที่แสดงหลังจากอัพโหลดสำเร็จ
  - แสดงข้อความ error ถ้าอัพโหลดล้มเหลว
  - เพิ่มปุ่ม "รีเซ็ท" เพื่อลบรูป profile
  - _Requirements: 3.2, 3.3, 3.4, 3.5, 3.6, 3.7, 3.8, 3.9, 3.10, 3.11, 3.12_

- [x] 18.3 Implement การอัพเดท Account Settings

  - เพิ่ม form validation สำหรับ required fields
  - เรียก PUT /api/account-settings endpoint เมื่อ user คลิก "บันทึกการเปลี่ยนแปลง"
  - แสดงข้อความ success หลังจากอัพเดทสำเร็จ
  - แสดงข้อความ error ถ้าอัพเดทล้มเหลว
  - _Requirements: 2.4, 2.5, 2.6, 2.7_

- [x] 19. เพิ่ม Data Filtering ใน Frontend Components

  - อัพเดท Activity Plans list component เพื่อใช้ role-based filtering (แสดงทั้งหมดสำหรับ Admin, แผนกสำหรับ Manager, ตัวเองสำหรับ User/Viewer)
  - อัพเดท Projects list component เพื่อใช้ role-based filtering
  - อัพเดท Employees list component เพื่อใช้ role-based filtering (ซ่อนสำหรับ User/Viewer)
  - อัพเดท Organizations list component เพื่อตรวจสอบว่า user สามารถเข้าถึง Master Data ได้หรือไม่
  - อัพเดท Departments list component เพื่อตรวจสอบว่า user สามารถเข้าถึง Master Data ได้หรือไม่
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 7.8, 7.9_

- [x] 20. เพิ่ม Action Permission Controls ใน Frontend

  - ปิดหรือซ่อนปุ่ม Create/Edit/Delete สำหรับ Viewer role
  - แสดง read-only mode สำหรับ Viewer role
  - ตรวจสอบ role ก่อนอนุญาตให้ทำ Create/Edit/Delete operations
  - แสดงข้อความ "คุณไม่มีสิทธิ์ดำเนินการนี้" ถ้า Viewer พยายามแก้ไขข้อมูล
  - ปิดปุ่มสำหรับ User role เมื่อพยายามเข้าถึงข้อมูลของผู้อื่น
  - ปิดปุ่มสำหรับ Manager role เมื่อพยายามเข้าถึงข้อมูลของแผนกอื่น
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 8.7, 8.8_

- [x] 21. เพิ่ม User Management ใน Navigation Menu

  - เพิ่ม "จัดการผู้ใช้งาน (Users)" menu item ใต้ Master Data section
  - กำหนด roles เป็น ['Admin'] เท่านั้น
  - Link ไปยัง /MasterData/UserListView route
  - _Requirements: 5.1_

- [x] 22. สร้างหน้า Not Authorized

  - สร้าง `src/client_web/src/views/pages/NotAuthorized.vue` page
  - แสดงข้อความ "คุณไม่มีสิทธิ์เข้าถึงหน้านี้"
  - เพิ่มปุ่มเพื่อกลับไปยัง dashboard
  - เพิ่ม route สำหรับ /not-authorized
  - _Requirements: 6.7, 6.8_

- [x] 23. อัพเดท API Client ด้วย Endpoints ใหม่

  - Regenerate หรืออัพเดท `src/client_web/src/client.ts` พร้อม endpoints ใหม่ (GET /api/users, POST /api/users/assign-role, POST /api/users/profile-image, PUT /api/account-settings)
  - เพิ่ม TypeScript interfaces สำหรับ UserDto, AssignRoleDto, ProfileImageUploadDto
  - _Requirements: 5.2, 5.6, 3.8, 2.5_

- [x] 24. Run Database Migrations และ Seed Data

  - Run migrations เพื่อสร้าง/อัพเดท AspNetRoles table
  - Run migrations เพื่ออัพเดท Employee table พร้อม ImageProfile column
  - Seed Admin, Manager, User, Viewer roles
  - Seed default admin user (username: admin, password: Admin@123)
  - ตรวจสอบว่า roles ถูกสร้างอย่างถูกต้องใน database
  - _Requirements: 5.5, 3.9, 1.1_

- [x] 25. ทดสอบ Role-Based Access Control
  - ทดสอบ Admin สามารถเข้าถึงทุกหน้าและทำทุกอย่างได้
  - ทดสอบ Manager สามารถเข้าถึง Master Data และเห็นข้อมูลแผนก
  - ทดสอบ User ไม่สามารถเข้าถึง Master Data และเห็นเฉพาะข้อมูลตัวเอง
  - ทดสอบ Viewer ไม่สามารถเข้าถึง Master Data และไม่สามารถแก้ไขข้อมูล
  - ทดสอบการเข้าถึงที่ไม่ได้รับอนุญาต return 403 error
  - ทดสอบ navigation items ถูกซ่อน/แสดงอย่างถูกต้องตาม role
  - _Requirements: 6.2, 6.3, 6.4, 6.5, 6.6, 6.7, 6.8, 6.9, 7.1, 7.2, 7.3, 7.4, 7.5, 7.6, 7.7, 7.8, 8.1, 8.2, 8.4, 8.6, 8.7_
