# Requirements Document

## Introduction

ระบบจัดการ Master Data และ Profile นี้มีวัตถุประสงค์เพื่อเพิ่มความสามารถในการจัดการข้อมูลหลักของระบบ ได้แก่ บทบาทผู้ใช้งาน (Roles) และแผนก (Departments) ผ่านหน้า Master Data พร้อมทั้งปรับปรุงหน้า Account Settings ให้สามารถแสดงข้อมูลที่ครบถ้วนจากการลงทะเบียน รวมถึงเพิ่มฟีเจอร์การอัพโหลดรูปโปรไฟล์ ระบบนี้จะช่วยให้ผู้ดูแลระบบสามารถจัดการข้อมูลหลักและสิทธิ์การเข้าถึงได้ง่ายขึ้น และผู้ใช้งานสามารถจัดการข้อมูลส่วนตัวของตนเองได้อย่างสมบูรณ์

## Requirements

### Requirement 1: Role Management Master Data Page

**User Story:** ในฐานะผู้ดูแลระบบ ฉันต้องการหน้าจัดการ Roles ใน Master Data เพื่อที่จะสามารถสร้าง แก้ไข และลบบทบาทผู้ใช้งานได้

#### Acceptance Criteria

1. WHEN ผู้ดูแลระบบเข้าถึงเมนู Master Data THEN ระบบ SHALL แสดงตัวเลือก "จัดการ Roles" ในเมนู
2. WHEN ผู้ดูแลระบบคลิกที่ "จัดการ Roles" THEN ระบบ SHALL แสดงหน้ารายการ Roles ทั้งหมดในรูปแบบตาราง
3. WHEN หน้ารายการ Roles แสดงผล THEN ระบบ SHALL แสดงคอลัมน์: ชื่อ Role, คำอธิบาย, วันที่สร้าง, วันที่แก้ไขล่าสุด, และปุ่มจัดการ
4. WHEN ผู้ดูแลระบบคลิกปุ่ม "เพิ่ม Role" THEN ระบบ SHALL แสดง dialog form สำหรับกรอกข้อมูล Role ใหม่
5. WHEN ผู้ดูแลระบบกรอกข้อมูล Role และคลิก "บันทึก" THEN ระบบ SHALL ตรวจสอบความถูกต้องของข้อมูลและบันทึกลงฐานข้อมูล
6. IF ชื่อ Role ซ้ำกับที่มีอยู่แล้ว THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "ชื่อ Role นี้มีอยู่แล้ว"
7. WHEN ผู้ดูแลระบบคลิกปุ่ม "แก้ไข" ของ Role THEN ระบบ SHALL แสดง dialog form พร้อมข้อมูล Role ที่เลือก
8. WHEN ผู้ดูแลระบบคลิกปุ่ม "ลบ" ของ Role THEN ระบบ SHALL แสดงข้อความยืนยันการลบ
9. WHEN ผู้ดูแลระบบยืนยันการลบ Role THEN ระบบ SHALL ตรวจสอบว่า Role นั้นไม่ถูกใช้งานโดยผู้ใช้คนใดก่อนลบ
10. IF Role ถูกใช้งานอยู่ THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "ไม่สามารถลบ Role นี้ได้เนื่องจากมีผู้ใช้งานอยู่"
11. WHEN มีการเปลี่ยนแปลงข้อมูล Role THEN ระบบ SHALL แสดงข้อความแจ้งผลการดำเนินการ (สำเร็จหรือล้มเหลว)

### Requirement 2: Account Settings Profile Data Integration

**User Story:** ในฐานะผู้ใช้งาน ฉันต้องการให้หน้า Account Settings แสดงข้อมูลที่ฉันกรอกตอนลงทะเบียน เพื่อที่จะสามารถตรวจสอบและแก้ไขข้อมูลได้อย่างถูกต้อง

#### Acceptance Criteria

1. WHEN ผู้ใช้งานเข้าสู่หน้า Account Settings THEN ระบบ SHALL ดึงข้อมูล FirstName และ LastName จากตาราง ApplicationUser หรือ Employee
2. IF ข้อมูล Employee ไม่มี FirstName หรือ LastName THEN ระบบ SHALL ดึงข้อมูลจาก registration data ที่เก็บไว้
3. WHEN ข้อมูลถูกโหลด THEN ระบบ SHALL แสดงข้อมูลในฟอร์ม: คำนำหน้า, ชื่อจริง, นามสกุล, แผนก, ตำแหน่ง, อีเมล, และเบอร์โทรศัพท์
4. WHEN ผู้ใช้งานแก้ไขข้อมูลและคลิก "บันทึกการเปลี่ยนแปลง" THEN ระบบ SHALL ตรวจสอบความถูกต้องของข้อมูลทั้งหมด
5. WHEN ข้อมูลถูกต้อง THEN ระบบ SHALL บันทึกการเปลี่ยนแปลงลงตาราง Employee
6. WHEN การบันทึกสำเร็จ THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "บันทึกการเปลี่ยนแปลงเสร็จสิ้น"
7. IF การบันทึกล้มเหลว THEN ระบบ SHALL แสดงข้อความแจ้งข้อผิดพลาดที่เกิดขึ้น

### Requirement 3: Profile Image Upload

**User Story:** ในฐานะผู้ใช้งาน ฉันต้องการอัพโหลดรูปโปรไฟล์ของฉัน เพื่อที่จะทำให้บัญชีของฉันมีความเป็นส่วนตัวมากขึ้น

#### Acceptance Criteria

1. WHEN ผู้ใช้งานเข้าสู่หน้า Account Settings THEN ระบบ SHALL แสดงรูปโปรไฟล์ปัจจุบัน หรือรูป avatar เริ่มต้นถ้ายังไม่มีรูป
2. WHEN ผู้ใช้งานคลิกที่รูปโปรไฟล์หรือปุ่ม "อัพโหลดรูปใหม่" THEN ระบบ SHALL เปิด file picker สำหรับเลือกไฟล์รูปภาพ
3. WHEN ผู้ใช้งานเลือกไฟล์รูปภาพ THEN ระบบ SHALL ตรวจสอบว่าไฟล์เป็นประเภท JPEG, PNG, JPG หรือ GIF เท่านั้น
4. IF ไฟล์ไม่ใช่ประเภทที่รองรับ THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "กรุณาเลือกไฟล์รูปภาพประเภท JPEG, PNG, JPG หรือ GIF"
5. WHEN ไฟล์ถูกเลือก THEN ระบบ SHALL ตรวจสอบขนาดไฟล์ไม่เกิน 5MB
6. IF ไฟล์มีขนาดเกิน 5MB THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "ขนาดไฟล์ต้องไม่เกิน 5MB"
7. WHEN ไฟล์ผ่านการตรวจสอบ THEN ระบบ SHALL แสดง preview รูปภาพที่เลือก
8. WHEN ผู้ใช้งานคลิก "บันทึก" THEN ระบบ SHALL อัพโหลดรูปภาพไปยัง MinIO storage
9. WHEN การอัพโหลดสำเร็จ THEN ระบบ SHALL บันทึก URL ของรูปภาพลงในฟิลด์ ImageProfile ของตาราง Employee
10. WHEN การบันทึกสำเร็จ THEN ระบบ SHALL อัพเดทรูปโปรไฟล์ที่แสดงบนหน้าจอทันที
11. WHEN ผู้ใช้งานคลิกปุ่ม "รีเซ็ท" THEN ระบบ SHALL ลบรูปโปรไฟล์ปัจจุบันและแสดงรูป avatar เริ่มต้น
12. IF การอัพโหลดล้มเหลว THEN ระบบ SHALL แสดงข้อความแจ้งข้อผิดพลาดและไม่เปลี่ยนแปลงรูปโปรไฟล์เดิม

### Requirement 4: Department Management Master Data Page

**User Story:** ในฐานะผู้ดูแลระบบ ฉันต้องการหน้าจัดการแผนกใน Master Data เพื่อที่จะสามารถสร้าง แก้ไข และลบแผนกได้

#### Acceptance Criteria

1. WHEN ผู้ดูแลระบบเข้าถึงเมนู Master Data THEN ระบบ SHALL แสดงตัวเลือก "จัดการแผนก" ในเมนู
2. WHEN ผู้ดูแลระบบคลิกที่ "จัดการแผนก" THEN ระบบ SHALL แสดงหน้ารายการแผนกทั้งหมดในรูปแบบตาราง
3. WHEN หน้ารายการแผนกแสดงผล THEN ระบบ SHALL แสดงคอลัมน์: ชื่อแผนก, สถานะ (Active/Inactive), วันที่สร้าง, วันที่แก้ไขล่าสุด, และปุ่มจัดการ
4. WHEN ผู้ดูแลระบบคลิกปุ่ม "เพิ่มแผนก" THEN ระบบ SHALL แสดง dialog form สำหรับกรอกข้อมูลแผนกใหม่
5. WHEN ผู้ดูแลระบบกรอกข้อมูลแผนกและคลิก "บันทึก" THEN ระบบ SHALL ตรวจสอบความถูกต้องของข้อมูลและบันทึกลงฐานข้อมูล
6. IF ชื่อแผนกซ้ำกับที่มีอยู่แล้ว THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "ชื่อแผนกนี้มีอยู่แล้ว"
7. WHEN ผู้ดูแลระบบคลิกปุ่ม "แก้ไข" ของแผนก THEN ระบบ SHALL แสดง dialog form พร้อมข้อมูลแผนกที่เลือก
8. WHEN ผู้ดูแลระบบคลิกปุ่ม "ลบ" ของแผนก THEN ระบบ SHALL แสดงข้อความยืนยันการลบ
9. WHEN ผู้ดูแลระบบยืนยันการลบแผนก THEN ระบบ SHALL ตรวจสอบว่าแผนกนั้นไม่ถูกใช้งานโดยพนักงานคนใดก่อนลบ
10. IF แผนกถูกใช้งานอยู่ THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "ไม่สามารถลบแผนกนี้ได้เนื่องจากมีพนักงานสังกัดอยู่"
11. WHEN มีการเปลี่ยนแปลงข้อมูลแผนก THEN ระบบ SHALL แสดงข้อความแจ้งผลการดำเนินการ (สำเร็จหรือล้มเหลว)
12. WHEN ผู้ดูแลระบบต้องการค้นหาแผนก THEN ระบบ SHALL มีช่องค้นหาที่สามารถค้นหาตามชื่อแผนก
13. WHEN ผู้ดูแลระบบต้องการกรองข้อมูล THEN ระบบ SHALL มีตัวกรองสถานะ (Active/Inactive)

### Requirement 5: User Management and Role Assignment

**User Story:** ในฐานะ Admin ฉันต้องการหน้าจัดการผู้ใช้งาน (Users) เพื่อที่จะสามารถดูรายชื่อผู้ใช้ทั้งหมด และกำหนด Role ให้กับผู้ใช้งานแต่ละคนได้

#### Acceptance Criteria

1. WHEN Admin เข้าถึงเมนู Master Data THEN ระบบ SHALL แสดงตัวเลือก "จัดการผู้ใช้งาน (Users)" ในเมนู
2. WHEN Admin คลิกที่ "จัดการผู้ใช้งาน" THEN ระบบ SHALL แสดงหน้ารายการผู้ใช้งานทั้งหมดในรูปแบบตาราง
3. WHEN หน้ารายการผู้ใช้งานแสดงผล THEN ระบบ SHALL แสดงคอลัมน์: รูปโปรไฟล์, ชื่อ-นามสกุล, อีเมล, แผนก, Role ปัจจุบัน, สถานะ (Active/Inactive), และปุ่มจัดการ
4. WHEN Admin คลิกปุ่ม "กำหนด Role" ของผู้ใช้งาน THEN ระบบ SHALL แสดง dialog form สำหรับเลือก Role
5. WHEN dialog form แสดงผล THEN ระบบ SHALL โหลดรายการ Roles ทั้งหมด (Admin, Manager, User, Viewer) จากฐานข้อมูล
6. WHEN Admin เลือก Role และคลิก "บันทึก" THEN ระบบ SHALL อัพเดท Role ใน AspNetUserRoles สำหรับ ApplicationUser
7. WHEN Admin เลือก Role และคลิก "บันทึก" THEN ระบบ SHALL อัพเดท Role ในตาราง Employee ถ้ามีข้อมูล Employee ที่เชื่อมโยง
8. WHEN การบันทึกสำเร็จ THEN ระบบ SHALL แสดงข้อความแจ้งเตือน "กำหนด Role สำเร็จ" และอัพเดทตารางทันที
9. IF การบันทึกล้มเหลว THEN ระบบ SHALL แสดงข้อความแจ้งข้อผิดพลาด
10. WHEN Admin ต้องการค้นหาผู้ใช้งาน THEN ระบบ SHALL มีช่องค้นหาที่สามารถค้นหาตามชื่อ, อีเมล, หรือแผนก
11. WHEN Admin ต้องการกรองข้อมูล THEN ระบบ SHALL มีตัวกรอง Role และสถานะ (Active/Inactive)
12. WHEN Admin คลิกที่ชื่อผู้ใช้งาน THEN ระบบ SHALL แสดงรายละเอียดผู้ใช้งานแบบเต็ม รวมถึงประวัติการเข้าสู่ระบบล่าสุด

### Requirement 6: Role-Based Permissions and Navigation

**User Story:** ในฐานะผู้ใช้งาน ฉันต้องการให้ระบบแสดงเมนูและฟีเจอร์ที่เหมาะสมกับ Role ของฉัน เพื่อที่จะเข้าถึงเฉพาะฟังก์ชันที่ฉันมีสิทธิ์ใช้งาน

#### Acceptance Criteria

1. WHEN ผู้ใช้งานเข้าสู่ระบบ THEN ระบบ SHALL ตรวจสอบ Role ของผู้ใช้งานจาก AspNetUserRoles
2. WHEN ผู้ใช้งานมี Role เป็น "Admin" THEN ระบบ SHALL แสดง Navigation Items ทั้งหมดรวมถึง Master Data (Roles, Departments, Event Types, Organizations), Activity Plans, Projects, Employees, Check-in/Check-out, และ Reports
3. WHEN ผู้ใช้งานมี Role เป็น "Manager" THEN ระบบ SHALL แสดง Navigation Items ทั้งหมดรวมถึง Master Data (Roles, Departments, Event Types, Organizations), Activity Plans, Projects, Employees, Check-in/Check-out, และ Reports
4. WHEN ผู้ใช้งานมี Role เป็น "User" THEN ระบบ SHALL แสดง Navigation Items: Activity Plans, Projects, Check-in/Check-out, และ Reports (ไม่แสดง Master Data และ Employees)
5. WHEN ผู้ใช้งานมี Role เป็น "Viewer" THEN ระบบ SHALL แสดง Navigation Items: Activity Plans, Projects, Check-in/Check-out, และ Reports (ไม่แสดง Master Data และ Employees)
6. WHEN ผู้ใช้งานมี Role เป็น "Viewer" THEN ระบบ SHALL ซ่อน Master Data menu items ทั้งหมด (Roles, Departments, Event Types, Organizations)
7. WHEN ผู้ใช้งานไม่มี Role ที่กำหนด THEN ระบบ SHALL แสดงเฉพาะเมนู Account Settings และ Logout
8. WHEN ผู้ใช้งานพยายามเข้าถึง URL ของหน้า Master Data โดยที่มี Role เป็น "User" หรือ "Viewer" THEN ระบบ SHALL แสดงหน้า 403 Forbidden หรือ redirect ไปหน้า Dashboard
9. WHEN Navigation Items โหลด THEN ระบบ SHALL ซ่อนเมนูที่ผู้ใช้งานไม่มีสิทธิ์เข้าถึงโดยอัตโนมัติ

### Requirement 7: Role-Based Data Filtering

**User Story:** ในฐานะผู้ใช้งาน ฉันต้องการให้ระบบกรองข้อมูลตาม Role และแผนกของฉัน เพื่อที่จะเห็นเฉพาะข้อมูลที่เกี่ยวข้องกับฉัน

#### Acceptance Criteria

1. WHEN ผู้ใช้งานมี Role เป็น "Admin" THEN ระบบ SHALL แสดงข้อมูลทั้งหมดในระบบโดยไม่มีการกรอง
2. WHEN ผู้ใช้งานมี Role เป็น "Manager" THEN ระบบ SHALL กรองข้อมูลให้แสดงเฉพาะพนักงานและข้อมูลที่เกี่ยวข้องในแผนกเดียวกัน
3. WHEN ผู้ใช้งานมี Role เป็น "Manager" AND เข้าถึงหน้า Activity Plans THEN ระบบ SHALL แสดง Activity Plans ของพนักงานทุกคนในแผนกเดียวกัน
4. WHEN ผู้ใช้งานมี Role เป็น "Manager" AND เข้าถึงหน้า Employees THEN ระบบ SHALL แสดงรายชื่อพนักงานในแผนกเดียวกันเท่านั้น
5. WHEN ผู้ใช้งานมี Role เป็น "User" THEN ระบบ SHALL กรองข้อมูลให้แสดงเฉพาะข้อมูลของตัวเองเท่านั้น
6. WHEN ผู้ใช้งานมี Role เป็น "User" AND เข้าถึงหน้า Activity Plans THEN ระบบ SHALL แสดงเฉพาะ Activity Plans ที่สร้างโดยตัวเองหรือที่ตัวเองเป็น Contact
7. WHEN ผู้ใช้งานมี Role เป็น "User" AND เข้าถึงหน้า Projects THEN ระบบ SHALL แสดงเฉพาะ Projects ที่ตัวเองเป็น Contact หรือมีส่วนเกี่ยวข้อง
8. WHEN ผู้ใช้งานมี Role เป็น "Viewer" THEN ระบบ SHALL กรองข้อมูลให้แสดงเฉพาะข้อมูลของตัวเองเท่านั้น (เหมือน User)
9. WHEN ผู้ใช้งานมี Role เป็น "Viewer" AND เข้าถึงหน้า Activity Plans THEN ระบบ SHALL แสดงเฉพาะ Activity Plans ที่สร้างโดยตัวเองหรือที่ตัวเองเป็น Contact (read-only)
10. WHEN ระบบกรองข้อมูล THEN ระบบ SHALL ใช้ DepartmentId จาก Employee table เพื่อเปรียบเทียบแผนก
11. WHEN ระบบกรองข้อมูล THEN ระบบ SHALL ใช้ EmployeeId หรือ ApplicationUserId เพื่อระบุตัวตนของผู้ใช้งาน
12. WHEN API endpoints ถูกเรียกใช้ THEN ระบบ SHALL ตรวจสอบ Role และใช้ filter ที่เหมาะสมก่อนส่งข้อมูลกลับ

### Requirement 8: Role-Based Action Permissions

**User Story:** ในฐานะผู้ใช้งาน ฉันต้องการให้ระบบควบคุมการกระทำ (Create, Edit, Delete) ตาม Role ของฉัน เพื่อป้องกันการแก้ไขข้อมูลที่ไม่มีสิทธิ์

#### Acceptance Criteria

1. WHEN ผู้ใช้งานมี Role เป็น "Admin" THEN ระบบ SHALL อนุญาตให้ Create, Edit, Delete ข้อมูลทั้งหมดในระบบ
2. WHEN ผู้ใช้งานมี Role เป็น "Manager" THEN ระบบ SHALL อนุญาตให้ Create, Edit, Delete ข้อมูลของพนักงานในแผนกเดียวกัน
3. WHEN ผู้ใช้งานมี Role เป็น "Manager" AND พยายาม Edit ข้อมูลพนักงานต่างแผนก THEN ระบบ SHALL แสดงข้อความ "คุณไม่มีสิทธิ์แก้ไขข้อมูลนี้"
4. WHEN ผู้ใช้งานมี Role เป็น "User" THEN ระบบ SHALL อนุญาตให้ Create, Edit, Delete เฉพาะข้อมูลของตัวเอง
5. WHEN ผู้ใช้งานมี Role เป็น "User" AND พยายาม Edit ข้อมูลของผู้อื่น THEN ระบบ SHALL แสดงข้อความ "คุณไม่มีสิทธิ์แก้ไขข้อมูลนี้"
6. WHEN ผู้ใช้งานมี Role เป็น "Viewer" THEN ระบบ SHALL อนุญาตเฉพาะการ View ข้อมูล (ไม่สามารถ Create, Edit, Delete)
7. WHEN ผู้ใช้งานมี Role เป็น "Viewer" AND พยายาม Create, Edit, หรือ Delete THEN ระบบ SHALL แสดงข้อความ "คุณไม่มีสิทธิ์ดำเนินการนี้" และปิดการใช้งานปุ่มที่เกี่ยวข้อง
8. WHEN หน้าจอโหลด THEN ระบบ SHALL ซ่อนหรือปิดการใช้งานปุ่ม Create, Edit, Delete ตาม Role ของผู้ใช้งาน
9. WHEN API endpoints รับคำสั่ง Create, Edit, Delete THEN ระบบ SHALL ตรวจสอบ Role และ Permissions ก่อนดำเนินการ
10. IF ผู้ใช้งานไม่มีสิทธิ์ THEN API SHALL ส่ง HTTP Status 403 Forbidden พร้อมข้อความแจ้งเตือน
