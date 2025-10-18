# Test Results - Department & Employee Management UI

## Test Execution Date
Date: [To be filled during testing]
Tester: [To be filled during testing]

## 10.1 ตรวจสอบ routes ทั้งหมดทำงานได้

### Test Case 1.1: ทดสอบการนำทางไปยัง DepartmentListView
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Admin or Manager
  2. Navigate to `/MasterData/DepartmentListView`
  3. Verify page loads successfully
  4. Verify page title displays "จัดการแผนก"
- **Expected Result**: Page loads with department list
- **Actual Result**: 
- **Notes**: 

### Test Case 1.2: ทดสอบการนำทางไปยัง EmployeeListView
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Admin or Manager
  2. Navigate to `/MasterData/EmployeeListView`
  3. Verify page loads successfully
  4. Verify page title displays "จัดการพนักงาน"
- **Expected Result**: Page loads with employee list
- **Actual Result**: 
- **Notes**: 

### Test Case 1.3: ตรวจสอบการควบคุมการเข้าถึงตาม role (Admin)
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Admin user
  2. Navigate to DepartmentListView
  3. Verify all action buttons are visible (Create, Edit, Delete)
  4. Navigate to EmployeeListView
  5. Verify all action buttons are visible
- **Expected Result**: Admin has full access to all features
- **Actual Result**: 
- **Notes**: 

### Test Case 1.4: ตรวจสอบการควบคุมการเข้าถึงตาม role (Manager)
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Manager user
  2. Navigate to DepartmentListView
  3. Verify all action buttons are visible
  4. Navigate to EmployeeListView
  5. Verify all action buttons are visible
- **Expected Result**: Manager has full access to all features
- **Actual Result**: 
- **Notes**: 

### Test Case 1.5: ทดสอบการ redirect เมื่อไม่มีสิทธิ์เข้าถึง (Viewer)
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Viewer user (or user without Admin/Manager role)
  2. Try to navigate to `/MasterData/DepartmentListView`
  3. Verify redirect occurs
  4. Try to navigate to `/MasterData/EmployeeListView`
  5. Verify redirect occurs
- **Expected Result**: User is redirected to not-authorized or login page
- **Actual Result**: 
- **Notes**: 

---

## 10.2 ทดสอบขั้นตอนการจัดการแผนก

### Test Case 2.1: ทดสอบการดูรายการแผนก
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to DepartmentListView
  2. Verify data table displays
  3. Verify columns: ชื่อแผนก, สถานะ, วันที่สร้าง, จัดการ
  4. Verify data loads from API
- **Expected Result**: Department list displays correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 2.2: ทดสอบการค้นหาแผนก
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to DepartmentListView
  2. Type search term in search box
  3. Verify table filters in real-time
  4. Clear search
  5. Verify all data returns
- **Expected Result**: Search filters departments by name
- **Actual Result**: 
- **Notes**: 

### Test Case 2.3: ทดสอบการสร้างแผนกใหม่
- **Status**: ⏳ Pending
- **Steps**:
  1. Click "เพิ่มแผนก" button
  2. Verify drawer opens
  3. Fill in department name
  4. Set isActive checkbox
  5. Click save
  6. Verify success message
  7. Verify new department appears in list
  8. Verify row is highlighted
- **Expected Result**: New department is created successfully
- **Actual Result**: 
- **Notes**: 

### Test Case 2.4: ทดสอบการแก้ไขแผนกที่มีอยู่
- **Status**: ⏳ Pending
- **Steps**:
  1. Click edit button on a department
  2. Verify drawer opens with existing data
  3. Modify department name
  4. Click save
  5. Verify success message
  6. Verify changes appear in list
  7. Verify row is highlighted
- **Expected Result**: Department is updated successfully
- **Actual Result**: 
- **Notes**: 

### Test Case 2.5: ทดสอบการลบแผนก
- **Status**: ⏳ Pending
- **Steps**:
  1. Click delete button on a department
  2. Verify confirmation dialog appears
  3. Click confirm
  4. Verify success message
  5. Verify department is removed from list
- **Expected Result**: Department is deleted successfully
- **Actual Result**: 
- **Notes**: 

### Test Case 2.6: ทดสอบการดูรายละเอียดแผนก
- **Status**: ⏳ Pending
- **Steps**:
  1. Click view button on a department
  2. Verify drawer opens
  3. Verify all fields display correctly (name, isActive, created, createdBy, etc.)
  4. Verify fields are read-only
  5. Click close
- **Expected Result**: Department details display correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 2.7: ตรวจสอบ pagination ทำงานถูกต้อง
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to DepartmentListView
  2. Verify pagination controls display
  3. Change page size
  4. Verify data reloads
  5. Navigate to next page
  6. Verify new data loads
  7. Navigate to previous page
  8. Verify data loads correctly
- **Expected Result**: Pagination works correctly
- **Actual Result**: 
- **Notes**: 

---

## 10.3 ทดสอบขั้นตอนการจัดการพนักงาน

### Test Case 3.1: ทดสอบการดูรายการพนักงาน
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to EmployeeListView
  2. Verify data table displays
  3. Verify columns: รูป, ชื่อ, อีเมล, ตำแหน่ง, แผนก, สถานะ, จัดการ
  4. Verify data loads from API
  5. Verify profile images display
- **Expected Result**: Employee list displays correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 3.2: ทดสอบการค้นหาพนักงาน
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to EmployeeListView
  2. Type search term in search box
  3. Verify table filters by name, email, or position
  4. Clear search
  5. Verify all data returns
- **Expected Result**: Search filters employees correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 3.3: ทดสอบการกรองตามแผนก
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to EmployeeListView
  2. Select a department from filter dropdown
  3. Verify only employees from that department display
  4. Clear filter
  5. Verify all employees return
- **Expected Result**: Department filter works correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 3.4: ทดสอบการกรองตามสถานะ
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to EmployeeListView
  2. Select "ใช้งาน" from status filter
  3. Verify only active employees display
  4. Select "ไม่ใช้งาน"
  5. Verify only inactive employees display
  6. Clear filter
- **Expected Result**: Status filter works correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 3.5: ทดสอบการสร้างพนักงานใหม่พร้อมรูปภาพ
- **Status**: ⏳ Pending
- **Steps**:
  1. Click "เพิ่มพนักงาน" button
  2. Verify drawer opens
  3. Fill in all required fields (userId, firstName, lastName, email)
  4. Select department
  5. Upload profile image (valid format and size)
  6. Verify image preview displays
  7. Click save
  8. Verify success message
  9. Verify new employee appears in list with image
  10. Verify row is highlighted
- **Expected Result**: New employee is created with image
- **Actual Result**: 
- **Notes**: 

### Test Case 3.6: ทดสอบการแก้ไขพนักงานที่มีอยู่
- **Status**: ⏳ Pending
- **Steps**:
  1. Click edit button on an employee
  2. Verify drawer opens with existing data
  3. Verify existing image displays (if any)
  4. Modify employee information
  5. Optionally upload new image
  6. Click save
  7. Verify success message
  8. Verify changes appear in list
  9. Verify row is highlighted
- **Expected Result**: Employee is updated successfully
- **Actual Result**: 
- **Notes**: 

### Test Case 3.7: ทดสอบการลบพนักงาน
- **Status**: ⏳ Pending
- **Steps**:
  1. Click delete button on an employee
  2. Verify confirmation dialog appears
  3. Click confirm
  4. Verify success message
  5. Verify employee is removed from list
- **Expected Result**: Employee is deleted successfully
- **Actual Result**: 
- **Notes**: 

### Test Case 3.8: ทดสอบการดูรายละเอียดพนักงาน
- **Status**: ⏳ Pending
- **Steps**:
  1. Click view button on an employee
  2. Verify drawer opens
  3. Verify all fields display correctly
  4. Verify profile image displays (or default avatar)
  5. Verify department name displays
  6. Verify fields are read-only
  7. Click close
- **Expected Result**: Employee details display correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 3.9: ตรวจสอบ pagination ทำงานถูกต้อง
- **Status**: ⏳ Pending
- **Steps**:
  1. Navigate to EmployeeListView
  2. Verify pagination controls display
  3. Change page size
  4. Verify data reloads
  5. Navigate to next page
  6. Verify new data loads
  7. Navigate to previous page
  8. Verify data loads correctly
- **Expected Result**: Pagination works correctly
- **Actual Result**: 
- **Notes**: 

---

## 10.4 ทดสอบ responsive design

### Test Case 4.1: ทดสอบบนเดสก์ท็อป (1920x1080)
- **Status**: ⏳ Pending
- **Steps**:
  1. Set browser viewport to 1920x1080
  2. Navigate to DepartmentListView
  3. Verify all columns display
  4. Verify buttons are properly sized
  5. Open create/edit drawer
  6. Verify drawer width is appropriate
  7. Repeat for EmployeeListView
- **Expected Result**: UI displays correctly on desktop
- **Actual Result**: 
- **Notes**: 

### Test Case 4.2: ทดสอบบนแท็บเล็ต (768x1024)
- **Status**: ⏳ Pending
- **Steps**:
  1. Set browser viewport to 768x1024
  2. Navigate to DepartmentListView
  3. Verify layout adjusts appropriately
  4. Verify table is scrollable if needed
  5. Open create/edit drawer
  6. Verify drawer width adjusts for tablet
  7. Repeat for EmployeeListView
- **Expected Result**: UI displays correctly on tablet
- **Actual Result**: 
- **Notes**: 

### Test Case 4.3: ทดสอบบนมือถือ (375x667)
- **Status**: ⏳ Pending
- **Steps**:
  1. Set browser viewport to 375x667
  2. Navigate to DepartmentListView
  3. Verify layout is mobile-friendly
  4. Verify table columns adjust or hide appropriately
  5. Open create/edit drawer
  6. Verify drawer takes full width on mobile
  7. Repeat for EmployeeListView
- **Expected Result**: UI displays correctly on mobile
- **Actual Result**: 
- **Notes**: 

### Test Case 4.4: ตรวจสอบ drawers ทำงานถูกต้องบนทุกอุปกรณ์
- **Status**: ⏳ Pending
- **Steps**:
  1. Test drawer open/close on desktop
  2. Test drawer open/close on tablet
  3. Test drawer open/close on mobile
  4. Verify drawer animations work smoothly
  5. Verify drawer content is scrollable if needed
- **Expected Result**: Drawers work correctly on all devices
- **Actual Result**: 
- **Notes**: 

### Test Case 4.5: ทดสอบปุ่ม ESC และ back button
- **Status**: ⏳ Pending
- **Steps**:
  1. Open a drawer
  2. Press ESC key
  3. Verify drawer closes
  4. Open a drawer again
  5. Click browser back button
  6. Verify appropriate behavior
- **Expected Result**: ESC and back button work correctly
- **Actual Result**: 
- **Notes**: 

---

## 10.5 ทดสอบการจัดการข้อผิดพลาด

### Test Case 5.1: ทดสอบสถานการณ์ API error
- **Status**: ⏳ Pending
- **Steps**:
  1. Simulate API error (disconnect network or use dev tools)
  2. Try to load department list
  3. Verify error message displays
  4. Try to create a department
  5. Verify error message displays
  6. Repeat for employee operations
- **Expected Result**: Appropriate error messages display
- **Actual Result**: 
- **Notes**: 

### Test Case 5.2: ทดสอบ validation errors
- **Status**: ⏳ Pending
- **Steps**:
  1. Open create department drawer
  2. Try to save without filling required fields
  3. Verify validation errors display
  4. Fill invalid data (e.g., too short name)
  5. Verify validation errors display
  6. Repeat for employee form
  7. Test email format validation
  8. Test phone format validation
  9. Test image file type validation
  10. Test image file size validation
- **Expected Result**: Validation errors display correctly
- **Actual Result**: 
- **Notes**: 

### Test Case 5.3: ทดสอบ network errors
- **Status**: ⏳ Pending
- **Steps**:
  1. Disconnect network
  2. Try to perform any operation
  3. Verify network error message displays
  4. Reconnect network
  5. Verify operations work again
- **Expected Result**: Network errors are handled gracefully
- **Actual Result**: 
- **Notes**: 

### Test Case 5.4: ตรวจสอบข้อความแจ้งข้อผิดพลาดแสดงถูกต้อง
- **Status**: ⏳ Pending
- **Steps**:
  1. Review all error messages from previous tests
  2. Verify messages are in Thai
  3. Verify messages are clear and helpful
  4. Verify messages use SweetAlert
- **Expected Result**: All error messages are appropriate
- **Actual Result**: 
- **Notes**: 

---

## 10.6 ทดสอบฟีเจอร์ตามสิทธิ์

### Test Case 6.1: ทดสอบในฐานะผู้ใช้ Admin (เข้าถึงได้ทั้งหมด)
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Admin user
  2. Navigate to DepartmentListView
  3. Verify "เพิ่มแผนก" button is visible
  4. Verify edit buttons are visible
  5. Verify delete buttons are visible
  6. Verify view buttons are visible
  7. Test all CRUD operations
  8. Navigate to EmployeeListView
  9. Verify all action buttons are visible
  10. Test all CRUD operations
- **Expected Result**: Admin has full access to all features
- **Actual Result**: 
- **Notes**: 

### Test Case 6.2: ทดสอบในฐานะผู้ใช้ Manager (เข้าถึงได้ทั้งหมด)
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Manager user
  2. Navigate to DepartmentListView
  3. Verify "เพิ่มแผนก" button is visible
  4. Verify edit buttons are visible
  5. Verify delete buttons are visible
  6. Verify view buttons are visible
  7. Test all CRUD operations
  8. Navigate to EmployeeListView
  9. Verify all action buttons are visible
  10. Test all CRUD operations
- **Expected Result**: Manager has full access to all features
- **Actual Result**: 
- **Notes**: 

### Test Case 6.3: ทดสอบในฐานะผู้ใช้ Viewer (ไม่สามารถเข้าถึง)
- **Status**: ⏳ Pending
- **Steps**:
  1. Login as Viewer user (or user without Admin/Manager role)
  2. Try to navigate to DepartmentListView
  3. Verify access is denied or redirected
  4. Try to navigate to EmployeeListView
  5. Verify access is denied or redirected
- **Expected Result**: Viewer cannot access these pages
- **Actual Result**: 
- **Notes**: 

### Test Case 6.4: ตรวจสอบปุ่มแสดง/ซ่อนตามสิทธิ์
- **Status**: ⏳ Pending
- **Steps**:
  1. Verify canModifyMasterData() method works correctly
  2. Verify canAccessMasterData() method works correctly
  3. Verify canAccessDepartmentData() method works correctly
  4. Test with different user roles
  5. Verify buttons show/hide based on permissions
- **Expected Result**: Buttons display based on user permissions
- **Actual Result**: 
- **Notes**: 

---

## Summary

### Overall Test Results
- **Total Test Cases**: 35
- **Passed**: 0
- **Failed**: 0
- **Pending**: 35
- **Blocked**: 0

### Issues Found
[List any issues discovered during testing]

### Recommendations
[List any recommendations for improvements]

### Sign-off
- **Tester**: ___________________
- **Date**: ___________________
- **Status**: ⏳ Testing Not Started / ✅ Approved / ❌ Rejected
