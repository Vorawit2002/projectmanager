# Frontend Role-Based Access Control Manual Test Guide

## Overview
This guide provides step-by-step instructions for manually testing the frontend Role-Based Access Control (RBAC) implementation.

## Prerequisites

1. Backend API is running
2. Frontend application is running
3. Database has been seeded with roles and test users
4. Default admin user exists (username: `admin`, password: `Admin@123`)

## Test Users Setup

Create the following test users for comprehensive testing:

| Username | Email | Password | Role | Department |
|----------|-------|----------|------|------------|
| admin | admin@test.com | Admin@123 | Admin | IT |
| manager1 | manager1@test.com | Manager@123 | Manager | IT |
| manager2 | manager2@test.com | Manager@123 | Manager | HR |
| user1 | user1@test.com | User@123 | User | IT |
| user2 | user2@test.com | User@123 | User | HR |
| viewer1 | viewer1@test.com | Viewer@123 | Viewer | IT |

## Test Scenarios

### 1. Admin Role Tests (Requirements 6.2, 7.1, 8.1)

#### Test 1.1: Admin Navigation Access
**Steps:**
1. Login as `admin@test.com` / `Admin@123`
2. Check the navigation menu

**Expected Results:**
- ✅ Master Data menu is visible
- ✅ Master Data submenu shows: Roles, Departments, Event Types, Organizations, Users
- ✅ Activity Plans menu is visible
- ✅ Projects menu is visible
- ✅ Employees menu is visible
- ✅ Check-in/Check-out menu is visible
- ✅ Reports menu is visible

#### Test 1.2: Admin Data Access
**Steps:**
1. Login as admin
2. Navigate to Employees page
3. Check the employee list

**Expected Results:**
- ✅ All employees from all departments are visible
- ✅ Can see employees from IT and HR departments

#### Test 1.3: Admin Actions
**Steps:**
1. Login as admin
2. Navigate to Master Data > Users
3. Select a user and click "กำหนด Role"
4. Change the role and save

**Expected Results:**
- ✅ "กำหนด Role" button is visible and enabled
- ✅ Can successfully assign roles
- ✅ Success message is displayed
- ✅ User list updates with new role

#### Test 1.4: Admin Create/Edit/Delete
**Steps:**
1. Login as admin
2. Navigate to Activity Plans
3. Check for Create/Edit/Delete buttons

**Expected Results:**
- ✅ "Create" button is visible and enabled
- ✅ "Edit" button is visible for all records
- ✅ "Delete" button is visible for all records

---

### 2. Manager Role Tests (Requirements 6.3, 7.2, 7.3, 7.4, 8.2)

#### Test 2.1: Manager Navigation Access
**Steps:**
1. Login as `manager1@test.com` / `Manager@123`
2. Check the navigation menu

**Expected Results:**
- ✅ Master Data menu is visible
- ✅ Master Data submenu shows: Departments, Event Types, Organizations
- ✅ Activity Plans menu is visible
- ✅ Projects menu is visible
- ✅ Employees menu is visible
- ✅ Check-in/Check-out menu is visible
- ✅ Reports menu is visible
- ❌ "Users" submenu under Master Data is NOT visible

#### Test 2.2: Manager Department Data Filtering
**Steps:**
1. Login as `manager1@test.com` (IT Department)
2. Navigate to Employees page
3. Check the employee list

**Expected Results:**
- ✅ Only employees from IT department are visible
- ❌ Employees from HR department are NOT visible

#### Test 2.3: Manager Activity Plans Filtering
**Steps:**
1. Login as `manager1@test.com` (IT Department)
2. Navigate to Activity Plans
3. Check the activity plans list

**Expected Results:**
- ✅ Only activity plans from IT department employees are visible
- ❌ Activity plans from HR department are NOT visible

#### Test 2.4: Manager Cross-Department Access
**Steps:**
1. Login as `manager1@test.com` (IT Department)
2. Try to access an activity plan created by HR department user (if URL is known)

**Expected Results:**
- ❌ Access is denied or redirected
- ✅ Error message: "คุณไม่มีสิทธิ์เข้าถึงข้อมูลนี้"

#### Test 2.5: Manager Modify Department Data
**Steps:**
1. Login as `manager1@test.com`
2. Navigate to Activity Plans
3. Try to edit an activity plan from IT department

**Expected Results:**
- ✅ "Edit" button is visible and enabled
- ✅ Can successfully edit the record
- ✅ Success message is displayed

---

### 3. User Role Tests (Requirements 6.4, 7.5, 7.6, 7.7, 8.4)

#### Test 3.1: User Navigation Access
**Steps:**
1. Login as `user1@test.com` / `User@123`
2. Check the navigation menu

**Expected Results:**
- ❌ Master Data menu is NOT visible
- ✅ Activity Plans menu is visible
- ✅ Projects menu is visible
- ✅ Check-in/Check-out menu is visible
- ✅ Reports menu is visible
- ❌ Employees menu is NOT visible

#### Test 3.2: User Data Filtering
**Steps:**
1. Login as `user1@test.com`
2. Navigate to Activity Plans
3. Check the activity plans list

**Expected Results:**
- ✅ Only own activity plans are visible
- ❌ Activity plans from other users are NOT visible

#### Test 3.3: User Master Data Access Attempt
**Steps:**
1. Login as `user1@test.com`
2. Try to access Master Data URL directly (e.g., `/MasterData/Departments`)

**Expected Results:**
- ❌ Access is denied
- ✅ Redirected to "Not Authorized" page or Dashboard
- ✅ Error message is displayed

#### Test 3.4: User Modify Own Data
**Steps:**
1. Login as `user1@test.com`
2. Navigate to Activity Plans
3. Try to edit own activity plan

**Expected Results:**
- ✅ "Edit" button is visible for own records
- ✅ Can successfully edit own records
- ❌ Cannot edit other users' records

#### Test 3.5: User Modify Others' Data
**Steps:**
1. Login as `user1@test.com`
2. Try to access edit URL for another user's activity plan (if URL is known)

**Expected Results:**
- ❌ Access is denied
- ✅ Error message: "คุณไม่มีสิทธิ์แก้ไขข้อมูลนี้"

---

### 4. Viewer Role Tests (Requirements 6.5, 8.6, 8.7)

#### Test 4.1: Viewer Navigation Access
**Steps:**
1. Login as `viewer1@test.com` / `Viewer@123`
2. Check the navigation menu

**Expected Results:**
- ❌ Master Data menu is NOT visible
- ✅ Activity Plans menu is visible
- ✅ Projects menu is visible
- ✅ Check-in/Check-out menu is visible
- ✅ Reports menu is visible
- ❌ Employees menu is NOT visible

#### Test 4.2: Viewer Read-Only Access
**Steps:**
1. Login as `viewer1@test.com`
2. Navigate to Activity Plans
3. Check for action buttons

**Expected Results:**
- ❌ "Create" button is NOT visible or disabled
- ❌ "Edit" button is NOT visible or disabled
- ❌ "Delete" button is NOT visible or disabled
- ✅ Can view the list of activity plans

#### Test 4.3: Viewer Data Filtering
**Steps:**
1. Login as `viewer1@test.com`
2. Navigate to Activity Plans
3. Check the activity plans list

**Expected Results:**
- ✅ Only own activity plans are visible (read-only)
- ❌ Activity plans from other users are NOT visible

#### Test 4.4: Viewer Modify Attempt
**Steps:**
1. Login as `viewer1@test.com`
2. Try to access edit URL directly (if known)

**Expected Results:**
- ❌ Access is denied
- ✅ Error message: "คุณไม่มีสิทธิ์ดำเนินการนี้"

#### Test 4.5: Viewer Master Data Access Attempt
**Steps:**
1. Login as `viewer1@test.com`
2. Try to access Master Data URL directly

**Expected Results:**
- ❌ Access is denied
- ✅ Redirected to "Not Authorized" page
- ✅ Error message is displayed

---

### 5. Navigation Item Visibility Tests (Requirement 6.9)

#### Test 5.1: Navigation Items by Role
**Steps:**
1. Login with each role (Admin, Manager, User, Viewer)
2. Document which menu items are visible

**Expected Results:**

| Menu Item | Admin | Manager | User | Viewer |
|-----------|-------|---------|------|--------|
| Master Data | ✅ | ✅ | ❌ | ❌ |
| - Roles | ✅ | ❌ | ❌ | ❌ |
| - Departments | ✅ | ✅ | ❌ | ❌ |
| - Event Types | ✅ | ✅ | ❌ | ❌ |
| - Organizations | ✅ | ✅ | ❌ | ❌ |
| - Users | ✅ | ❌ | ❌ | ❌ |
| Activity Plans | ✅ | ✅ | ✅ | ✅ |
| Projects | ✅ | ✅ | ✅ | ✅ |
| Employees | ✅ | ✅ | ❌ | ❌ |
| Check-in/Check-out | ✅ | ✅ | ✅ | ✅ |
| Reports | ✅ | ✅ | ✅ | ✅ |

---

### 6. Route Guard Tests (Requirements 6.7, 6.8)

#### Test 6.1: Unauthorized Route Access
**Steps:**
1. Login as `user1@test.com` (User role)
2. Try to access `/MasterData/Users` directly in browser

**Expected Results:**
- ❌ Access is denied
- ✅ Redirected to `/not-authorized` page
- ✅ Page shows: "คุณไม่มีสิทธิ์เข้าถึงหน้านี้"
- ✅ "Back to Dashboard" button is visible

#### Test 6.2: Manager Accessing Admin-Only Page
**Steps:**
1. Login as `manager1@test.com` (Manager role)
2. Try to access `/MasterData/Users` directly

**Expected Results:**
- ❌ Access is denied
- ✅ Redirected to `/not-authorized` page

#### Test 6.3: Viewer Accessing Master Data
**Steps:**
1. Login as `viewer1@test.com` (Viewer role)
2. Try to access `/MasterData/Departments` directly

**Expected Results:**
- ❌ Access is denied
- ✅ Redirected to `/not-authorized` page

---

### 7. Account Settings Tests

#### Test 7.1: Profile Image Upload
**Steps:**
1. Login as any user
2. Navigate to Account Settings
3. Click "อัพโหลดรูปใหม่"
4. Select an image file (JPEG, PNG, JPG, or GIF, < 5MB)
5. Save

**Expected Results:**
- ✅ File picker opens
- ✅ Preview of selected image is shown
- ✅ Image uploads successfully
- ✅ Profile image updates immediately
- ✅ Success message is displayed

#### Test 7.2: Profile Image Validation
**Steps:**
1. Login as any user
2. Navigate to Account Settings
3. Try to upload invalid file (e.g., PDF, > 5MB)

**Expected Results:**
- ❌ Upload is rejected
- ✅ Error message: "กรุณาเลือกไฟล์รูปภาพประเภท JPEG, PNG, JPG หรือ GIF" or "ขนาดไฟล์ต้องไม่เกิน 5MB"

---

### 8. User Management Tests (Admin Only)

#### Test 8.1: User List Access
**Steps:**
1. Login as `admin@test.com`
2. Navigate to Master Data > Users

**Expected Results:**
- ✅ User list page loads
- ✅ Shows all users with columns: Profile Image, Name, Email, Department, Role, Status, Actions
- ✅ Search box is visible
- ✅ Role filter is visible
- ✅ Status filter is visible

#### Test 8.2: Assign Role
**Steps:**
1. Login as admin
2. Navigate to Master Data > Users
3. Click "กำหนด Role" for a user
4. Select a different role
5. Click "บันทึก"

**Expected Results:**
- ✅ Dialog opens with role dropdown
- ✅ Shows all roles: Admin, Manager, User, Viewer
- ✅ Role is assigned successfully
- ✅ Success message: "กำหนด Role สำเร็จ"
- ✅ User list updates with new role

#### Test 8.3: Search and Filter Users
**Steps:**
1. Login as admin
2. Navigate to Master Data > Users
3. Use search box to search by name
4. Use role filter to filter by role
5. Use status filter to filter by active/inactive

**Expected Results:**
- ✅ Search filters results correctly
- ✅ Role filter shows only users with selected role
- ✅ Status filter shows only active or inactive users

---

## Test Results Template

Use this template to document your test results:

```
Test Date: _______________
Tester: _______________
Environment: _______________

| Test ID | Test Name | Status | Notes |
|---------|-----------|--------|-------|
| 1.1 | Admin Navigation Access | ☐ Pass ☐ Fail | |
| 1.2 | Admin Data Access | ☐ Pass ☐ Fail | |
| 1.3 | Admin Actions | ☐ Pass ☐ Fail | |
| 1.4 | Admin Create/Edit/Delete | ☐ Pass ☐ Fail | |
| 2.1 | Manager Navigation Access | ☐ Pass ☐ Fail | |
| 2.2 | Manager Department Data Filtering | ☐ Pass ☐ Fail | |
| 2.3 | Manager Activity Plans Filtering | ☐ Pass ☐ Fail | |
| 2.4 | Manager Cross-Department Access | ☐ Pass ☐ Fail | |
| 2.5 | Manager Modify Department Data | ☐ Pass ☐ Fail | |
| 3.1 | User Navigation Access | ☐ Pass ☐ Fail | |
| 3.2 | User Data Filtering | ☐ Pass ☐ Fail | |
| 3.3 | User Master Data Access Attempt | ☐ Pass ☐ Fail | |
| 3.4 | User Modify Own Data | ☐ Pass ☐ Fail | |
| 3.5 | User Modify Others' Data | ☐ Pass ☐ Fail | |
| 4.1 | Viewer Navigation Access | ☐ Pass ☐ Fail | |
| 4.2 | Viewer Read-Only Access | ☐ Pass ☐ Fail | |
| 4.3 | Viewer Data Filtering | ☐ Pass ☐ Fail | |
| 4.4 | Viewer Modify Attempt | ☐ Pass ☐ Fail | |
| 4.5 | Viewer Master Data Access Attempt | ☐ Pass ☐ Fail | |
| 5.1 | Navigation Items by Role | ☐ Pass ☐ Fail | |
| 6.1 | Unauthorized Route Access | ☐ Pass ☐ Fail | |
| 6.2 | Manager Accessing Admin-Only Page | ☐ Pass ☐ Fail | |
| 6.3 | Viewer Accessing Master Data | ☐ Pass ☐ Fail | |
| 7.1 | Profile Image Upload | ☐ Pass ☐ Fail | |
| 7.2 | Profile Image Validation | ☐ Pass ☐ Fail | |
| 8.1 | User List Access | ☐ Pass ☐ Fail | |
| 8.2 | Assign Role | ☐ Pass ☐ Fail | |
| 8.3 | Search and Filter Users | ☐ Pass ☐ Fail | |
```

## Common Issues and Troubleshooting

### Issue: Navigation items not hiding
**Solution:** Check that the Auth Store is properly loading user roles and the NavItems component is using the correct role checks.

### Issue: Route guards not working
**Solution:** Verify that route guards are registered in the router and that the route meta includes the required roles.

### Issue: Data filtering not working
**Solution:** Check that the DataFilterService is properly applied in the backend query handlers and that the frontend is sending the correct user context.

### Issue: 403 errors not showing properly
**Solution:** Verify that the error handling middleware is configured and that the frontend is catching and displaying 403 errors correctly.

## Notes

- All tests should be performed in a clean test environment
- Document any deviations from expected results
- Take screenshots of any issues encountered
- Report bugs with detailed reproduction steps
