# แผนการพัฒนา (Implementation Plan)

- [x] 1. ตั้งค่า routing และการนำทางสำหรับหน้าแผนกและพนักงาน

  - เพิ่ม routes สำหรับ DepartmentListView และ EmployeeListView ใน MasterData.ts
  - ตั้งค่า route meta ด้วย requiresAuth และ roles (Admin, Manager)
  - ตรวจสอบว่า routes สามารถเข้าถึงได้และถูกป้องกันด้วย role guards
  - _Requirements: 1.2, 2.2, 6.1, 6.2, 6.5_

- [x] 2. สร้าง component DepartmentListView

  - [x] 2.1 สร้าง DepartmentListView.vue พร้อมโครงสร้าง data table

    - สร้างไฟล์ component พร้อม template, script และ style sections
    - กำหนด data properties (header, search, data, pagination, dialogs, loading state)
    - สร้างส่วน header พร้อมชื่อหน้าและปุ่มสร้าง (พร้อมตรวจสอบสิทธิ์)
    - สร้างช่องค้นหาพร้อมการกรองแบบ real-time
    - สร้าง v-data-table-server พร้อม pagination
    - เพิ่มคอลัมน์ปุ่มจัดการ (ดู, แก้ไข, ลบ) พร้อมตรวจสอบสิทธิ์
    - _Requirements: 1.1, 1.2, 1.3, 1.10, 3.1_

  - [x] 2.2 สร้าง methods สำหรับโหลดข้อมูลและ pagination

    - เขียน method initialize() เพื่อดึงข้อมูลแผนกจาก API
    - สร้าง methods handlePageChange() และ handlePageSizeChange()
    - เพิ่มการจัดการ loading state
    - จัดการ API errors ด้วย SweetAlert
    - _Requirements: 1.1, 1.9_

  - [x] 2.3 สร้าง methods สำหรับจัดการ dialog

    - เขียน methods OpenDialogCreate(), CloseDialogCreate()
    - เขียน methods OpenDialogEdit(), CloseDialogEdit()
    - เขียน methods OpenDialogDetail(), CloseDialogDetail()
    - สร้างการ highlight แถวหลังจากสร้าง/แก้ไข
    - เพิ่ม method scrollToHighlightedRow()
    - _Requirements: 1.4, 1.6, 1.7_

  - [x] 2.4 สร้างฟังก์ชันลบข้อมูล

    - เขียน method OpenDelete() พร้อม confirmation dialog
    - เรียก deleteDepartment API
    - รีเฟรชรายการหลังลบสำเร็จ
    - จัดการ errors อย่างเหมาะสม
    - _Requirements: 1.8, 1.9_

  - [x] 2.5 เพิ่ม methods สำหรับตรวจสอบสิทธิ์

    - สร้าง canAccessMasterData() โดยใช้ RoleService
    - สร้าง canModifyMasterData() โดยใช้ RoleService
    - เพิ่ม mounted lifecycle hook พร้อมตรวจสอบสิทธิ์และ redirect
    - _Requirements: 1.2, 1.3, 6.5_

  - [x] 2.6 เพิ่ม responsive styling และตั้งค่า drawer
    - เพิ่ม scoped styles สำหรับ card, table และ buttons
    - ตั้งค่า v-navigation-drawer สำหรับ create/edit/detail
    - เพิ่มความกว้าง drawer แบบ responsive สำหรับมือถือ
    - สร้าง handlers สำหรับปุ่ม ESC และ back button
    - _Requirements: 5.1, 5.2, 5.3, 6.4_

- [x] 3. สร้าง component CreateDepartment

  - [x] 3.1 สร้าง CreateDepartment.vue พร้อมโครงสร้างฟอร์ม

    - สร้างไฟล์ component พร้อม drawer layout
    - เพิ่มฟิลด์ฟอร์ม: name (VTextField), isActive (VCheckbox)
    - กำหนด form data properties และ validation rules
    - เพิ่มปุ่มบันทึกและยกเลิก
    - _Requirements: 1.4, 5.4, 7.1_

  - [x] 3.2 สร้างการตรวจสอบฟอร์ม

    - เพิ่ม validation rules สำหรับฟิลด์ name (required, min/max length)
    - สร้างการตรวจสอบฟอร์มเมื่อส่งข้อมูล
    - แสดงข้อความ validation error
    - _Requirements: 5.5, 7.1, 7.5_

  - [x] 3.3 สร้างฟังก์ชันบันทึกข้อมูล

    - เขียน method save() เพื่อเรียก createDepartment API
    - จัดการการสร้างสำเร็จพร้อมข้อความแจ้งเตือน
    - ส่งข้อมูลที่สร้างกลับไปยัง parent เพื่อ highlight
    - จัดการ API errors พร้อมข้อความที่เหมาะสม
    - _Requirements: 1.5, 1.9, 5.7_

  - [x] 3.4 สร้างฟังก์ชันยกเลิกและรีเซ็ต
    - เขียน method cancel() เพื่อปิด drawer
    - เขียน method resetForm() เพื่อล้างข้อมูลฟอร์ม
    - _Requirements: 5.4_

- [x] 4. สร้าง component UpdateDepartment

  - [x] 4.1 สร้าง UpdateDepartment.vue พร้อมโครงสร้างฟอร์ม

    - สร้างไฟล์ component พร้อม drawer layout
    - เพิ่ม props สำหรับ id และ CloseDialogEdit callback
    - เพิ่มฟิลด์ฟอร์มเหมือน CreateDepartment
    - กำหนด form data properties
    - _Requirements: 1.6, 5.4_

  - [x] 4.2 สร้างการโหลดข้อมูล

    - เขียน method loadDepartment() เพื่อดึงข้อมูลแผนกตาม ID
    - เติมข้อมูลลงในฟิลด์ฟอร์ม
    - จัดการ loading state
    - จัดการ errors ถ้าไม่พบแผนก
    - _Requirements: 1.6_

  - [x] 4.3 สร้างฟังก์ชันอัพเดท
    - เขียน method save() เพื่อเรียก updateDepartment API
    - จัดการการอัพเดทสำเร็จพร้อมข้อความแจ้งเตือน
    - ปิด drawer และรีเฟรชรายการใน parent
    - จัดการ API errors อย่างเหมาะสม
    - _Requirements: 1.7, 1.9, 5.7_

- [x] 5. สร้าง component DepartmentDetail

  - [x] 5.1 สร้าง DepartmentDetail.vue พร้อม layout แบบ read-only

    - สร้างไฟล์ component พร้อม drawer layout
    - เพิ่ม props สำหรับ id และ CloseDialogDetail callback
    - ออกแบบ layout แบบ read-only สำหรับฟิลด์ทั้งหมด
    - เพิ่มปุ่มปิด
    - _Requirements: 1.10_

  - [x] 5.2 สร้างการโหลดและแสดงข้อมูล
    - เขียน method loadDepartment() เพื่อดึงข้อมูลแผนกตาม ID
    - แสดงฟิลด์ทั้งหมดของแผนก (name, isActive, audit fields)
    - จัดรูปแบบวันที่อย่างเหมาะสม
    - จัดการ loading state และ errors
    - _Requirements: 1.10_

- [x] 6. สร้าง component EmployeeListView

  - [x] 6.1 สร้าง EmployeeListView.vue พร้อมโครงสร้าง data table

    - สร้างไฟล์ component พร้อม template, script และ style sections
    - กำหนด data properties (header, search, data, pagination, dialogs, filters, loading state)
    - สร้างส่วน header พร้อมชื่อหน้าและปุ่มสร้าง (พร้อมตรวจสอบสิทธิ์)
    - สร้างช่องค้นหาพร้อมการกรองแบบ real-time
    - เพิ่มฟิลด์กรองสำหรับแผนกและสถานะ
    - สร้าง v-data-table-server พร้อม pagination
    - เพิ่มคอลัมน์ปุ่มจัดการพร้อมตรวจสอบสิทธิ์
    - เพิ่มคอลัมน์รูปโปรไฟล์พร้อมแสดงรูปขนาดย่อ
    - _Requirements: 2.1, 2.2, 2.3, 2.9, 3.2, 3.3, 3.4, 8.5_

  - [x] 6.2 สร้าง methods สำหรับโหลดข้อมูลและ pagination

    - เขียน method initialize() เพื่อดึงข้อมูลพนักงานจาก API
    - เขียน method loadDepartments() เพื่อดึงข้อมูลแผนกสำหรับ filter
    - สร้าง methods handlePageChange() และ handlePageSizeChange()
    - เพิ่มการจัดการ loading state
    - จัดการ API errors ด้วย SweetAlert
    - _Requirements: 2.1, 2.10_

  - [x] 6.3 สร้างฟังก์ชันกรองข้อมูล

    - เขียน method applyFilters() เพื่อใช้ตัวกรองแผนกและสถานะ
    - เขียน method clearFilters() เพื่อรีเซ็ตตัวกรองทั้งหมด
    - อัพเดท API request ด้วยพารามิเตอร์ตัวกรอง
    - รีเฟรชข้อมูลเมื่อตัวกรองเปลี่ยน
    - _Requirements: 3.3, 3.4, 3.5_

  - [x] 6.4 สร้าง methods สำหรับจัดการ dialog

    - เขียน methods OpenDialogCreate(), CloseDialogCreate()
    - เขียน methods OpenDialogEdit(), CloseDialogEdit()
    - เขียน methods OpenDialogDetail(), CloseDialogDetail()
    - สร้างการ highlight แถวหลังจากสร้าง/แก้ไข
    - เพิ่ม method scrollToHighlightedRow()
    - _Requirements: 2.4, 2.6, 2.7_

  - [x] 6.5 สร้างฟังก์ชันลบข้อมูล

    - เขียน method OpenDelete() พร้อม confirmation dialog
    - เรียก deleteEmployee API
    - รีเฟรชรายการหลังลบสำเร็จ
    - จัดการ errors อย่างเหมาะสม
    - _Requirements: 2.8, 2.10_

  - [x] 6.6 เพิ่ม helper methods

    - สร้าง getDepartmentName() เพื่อแปลง department ID เป็นชื่อแผนก
    - สร้าง canAccessDepartmentData() โดยใช้ RoleService
    - สร้าง canModifyMasterData() โดยใช้ RoleService
    - เพิ่ม mounted lifecycle hook พร้อมตรวจสอบสิทธิ์
    - _Requirements: 2.2, 2.3, 6.5_

  - [x] 6.7 เพิ่ม responsive styling และตั้งค่า drawer
    - เพิ่ม scoped styles สำหรับ card, table, buttons และรูปโปรไฟล์
    - ตั้งค่า v-navigation-drawer สำหรับ create/edit/detail
    - เพิ่มความกว้าง drawer แบบ responsive สำหรับมือถือ
    - สร้าง handlers สำหรับปุ่ม ESC และ back button
    - จัดรูปแบบรูปโปรไฟล์ขนาดย่อ
    - _Requirements: 5.1, 5.2, 5.3, 8.5_

- [x] 7. สร้าง component CreateEmployee

  - [x] 7.1 สร้าง CreateEmployee.vue พร้อมโครงสร้างฟอร์ม

    - สร้างไฟล์ component พร้อม drawer layout
    - เพิ่มฟิลด์ฟอร์มทั้งหมด (userId, titleName, firstName, lastName, email, position, phone, imageProfile, isActive, departmentId, subscription, roles, group)
    - กำหนด form data properties และ validation rules
    - เพิ่มปุ่มบันทึกและยกเลิก
    - _Requirements: 2.4, 4.1, 5.4, 7.2, 7.3, 7.4_

  - [x] 7.2 สร้าง dropdown แผนก

    - เขียน method loadDepartments() เพื่อดึงข้อมูลแผนกที่ใช้งาน
    - เติมข้อมูลลง VSelect ด้วยตัวเลือกแผนก
    - เรียงแผนกตามตัวอักษร
    - แสดงข้อความถ้าไม่มีแผนก
    - _Requirements: 4.1, 4.2, 4.3, 4.4_

  - [x] 7.3 สร้างฟังก์ชันอัพโหลดรูปภาพ

    - เพิ่ม VFileInput สำหรับอัพโหลดรูปภาพ
    - เขียน method handleImageUpload() เพื่อตรวจสอบประเภทและขนาดไฟล์
    - เขียน method previewImage() เพื่อแสดงตัวอย่างรูปภาพ
    - ตรวจสอบประเภทไฟล์ (jpg, png, gif) และขนาด (สูงสุด 5MB)
    - แสดง validation errors สำหรับไฟล์ที่ไม่ถูกต้อง
    - _Requirements: 8.1, 8.2, 8.3, 8.4_

  - [x] 7.4 สร้างการตรวจสอบฟอร์ม

    - เพิ่ม validation rules สำหรับฟิลด์บังคับ (userId, firstName, lastName, email)
    - เพิ่มการตรวจสอบรูปแบบอีเมล
    - เพิ่มการตรวจสอบรูปแบบเบอร์โทร (ไม่บังคับ, 10 หลัก)
    - สร้างการตรวจสอบฟอร์มเมื่อส่งข้อมูล
    - แสดง validation errors แบบ field-level
    - _Requirements: 5.5, 7.2, 7.3, 7.4, 7.5_

  - [x] 7.5 สร้างฟังก์ชันบันทึกข้อมูล

    - เขียน method save() เพื่อเรียก createEmployee API
    - จัดการการอัพโหลดรูปภาพถ้ามีการเลือกไฟล์
    - จัดการการสร้างสำเร็จพร้อมข้อความแจ้งเตือน
    - ส่งข้อมูลที่สร้างกลับไปยัง parent เพื่อ highlight
    - จัดการ API errors พร้อมข้อความที่เหมาะสม
    - _Requirements: 2.5, 2.10, 5.7, 8.7_

  - [x] 7.6 สร้างฟังก์ชันยกเลิกและรีเซ็ต
    - เขียน method cancel() เพื่อปิด drawer
    - เขียน method resetForm() เพื่อล้างข้อมูลฟอร์มและตัวอย่างรูปภาพ
    - _Requirements: 5.4_

- [x] 8. สร้าง component UpdateEmployee

  - [x] 8.1 สร้าง UpdateEmployee.vue พร้อมโครงสร้างฟอร์ม

    - สร้างไฟล์ component พร้อม drawer layout
    - เพิ่ม props สำหรับ id และ CloseDialogEdit callback
    - เพิ่มฟิลด์ฟอร์มทั้งหมดเหมือน CreateEmployee
    - กำหนด form data properties
    - _Requirements: 2.6, 5.4_

  - [x] 8.2 สร้างการโหลดข้อมูล

    - เขียน method loadEmployee() เพื่อดึงข้อมูลพนักงานตาม ID
    - เขียน method loadDepartments() เพื่อดึงข้อมูลแผนก
    - เติมข้อมูลลงในฟิลด์ฟอร์ม
    - แสดงรูปโปรไฟล์เดิมถ้ามี
    - จัดการ loading state และ errors
    - _Requirements: 2.6, 4.1_

  - [x] 8.3 สร้างฟังก์ชันอัพโหลดรูปภาพ

    - เพิ่ม VFileInput สำหรับอัพโหลดรูปภาพ
    - เขียน method handleImageUpload() เพื่อตรวจสอบไฟล์
    - เขียน method previewImage() เพื่อแสดงตัวอย่างรูปภาพใหม่
    - อนุญาตให้เปลี่ยนรูปภาพเดิม
    - ตรวจสอบประเภทและขนาดไฟล์
    - _Requirements: 8.1, 8.2, 8.3, 8.4_

  - [x] 8.4 สร้างฟังก์ชันอัพเดท
    - เขียน method save() เพื่อเรียก updateEmployee API
    - จัดการการอัพโหลดรูปภาพถ้ามีการเลือกไฟล์ใหม่
    - จัดการการอัพเดทสำเร็จพร้อมข้อความแจ้งเตือน
    - ปิด drawer และรีเฟรชรายการใน parent
    - จัดการ API errors อย่างเหมาะสม
    - _Requirements: 2.7, 2.10, 5.7, 8.7_

- [x] 9. สร้าง component EmployeeDetail

  - [x] 9.1 สร้าง EmployeeDetail.vue พร้อม layout แบบ read-only

    - สร้างไฟล์ component พร้อม drawer layout
    - เพิ่ม props สำหรับ id และ CloseDialogDetail callback
    - ออกแบบ layout แบบ read-only สำหรับฟิลด์ทั้งหมด
    - เพิ่มส่วนแสดงรูปโปรไฟล์
    - เพิ่มปุ่มปิด
    - _Requirements: 2.9, 8.5, 8.6_

  - [x] 9.2 สร้างการโหลดและแสดงข้อมูล
    - เขียน method loadEmployee() เพื่อดึงข้อมูลพนักงานตาม ID
    - เขียน method getDepartmentName() เพื่อแสดงชื่อแผนก
    - แสดงฟิลด์ทั้งหมดของพนักงานรวมถึงรูปโปรไฟล์
    - แสดง avatar เริ่มต้นถ้าไม่มีรูปภาพ
    - จัดรูปแบบวันที่อย่างเหมาะสม
    - จัดการ loading state และ errors
    - _Requirements: 2.9, 8.5, 8.6_

- [x] 10. การรวมและทดสอบขั้นสุดท้าย

  - [x] 10.1 ตรวจสอบ routes ทั้งหมดทำงานได้

    - ทดสอบการนำทางไปยัง DepartmentListView
    - ทดสอบการนำทางไปยัง EmployeeListView
    - ตรวจสอบการควบคุมการเข้าถึงตาม role
    - ทดสอบการ redirect เมื่อไม่มีสิทธิ์เข้าถึง
    - _Requirements: 6.1, 6.2, 6.5_

  - [x] 10.2 ทดสอบขั้นตอนการจัดการแผนก

    - ทดสอบการดูรายการแผนก
    - ทดสอบการค้นหาแผนก
    - ทดสอบการสร้างแผนกใหม่
    - ทดสอบการแก้ไขแผนกที่มีอยู่
    - ทดสอบการลบแผนก
    - ทดสอบการดูรายละเอียดแผนก
    - ตรวจสอบ pagination ทำงานถูกต้อง
    - _Requirements: 1.1, 1.4, 1.5, 1.6, 1.7, 1.8, 3.1_

  - [x] 10.3 ทดสอบขั้นตอนการจัดการพนักงาน

    - ทดสอบการดูรายการพนักงาน
    - ทดสอบการค้นหาพนักงาน
    - ทดสอบการกรองตามแผนก
    - ทดสอบการกรองตามสถานะ
    - ทดสอบการสร้างพนักงานใหม่พร้อมรูปภาพ
    - ทดสอบการแก้ไขพนักงานที่มีอยู่
    - ทดสอบการลบพนักงาน
    - ทดสอบการดูรายละเอียดพนักงาน
    - ตรวจสอบ pagination ทำงานถูกต้อง
    - _Requirements: 2.1, 2.4, 2.5, 2.6, 2.7, 2.8, 3.2, 3.3, 3.4, 8.1-8.7_

  - [x] 10.4 ทดสอบ responsive design

    - ทดสอบบนเดสก์ท็อป (1920x1080)
    - ทดสอบบนแท็บเล็ต (768x1024)
    - ทดสอบบนมือถือ (375x667)
    - ตรวจสอบ drawers ทำงานถูกต้องบนทุกอุปกรณ์
    - ทดสอบปุ่ม ESC และ back button
    - _Requirements: 5.1, 5.2, 5.3, 6.4_

  - [x] 10.5 ทดสอบการจัดการข้อผิดพลาด

    - ทดสอบสถานการณ์ API error
    - ทดสอบ validation errors
    - ทดสอบ network errors
    - ตรวจสอบข้อความแจ้งข้อผิดพลาดแสดงถูกต้อง
    - _Requirements: 1.9, 2.10, 5.5, 7.5_

  - [x] 10.6 ทดสอบฟีเจอร์ตามสิทธิ์
    - ทดสอบในฐานะผู้ใช้ Admin (เข้าถึงได้ทั้งหมด)
    - ทดสอบในฐานะผู้ใช้ Manager (เข้าถึงได้ทั้งหมด)
    - ทดสอบในฐานะผู้ใช้ Viewer (ไม่สามารถเข้าถึง)
    - ตรวจสอบปุ่มแสดง/ซ่อนตามสิทธิ์
    - _Requirements: 1.2, 1.3, 2.2, 2.3, 6.5_
