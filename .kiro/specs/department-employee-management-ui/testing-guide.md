# Testing Guide - Department & Employee Management UI

## Overview
This guide provides instructions for manually testing the Department and Employee Management UI features. All tests should be performed to verify the implementation meets the requirements.

## Prerequisites
1. Backend API must be running
2. Database must have test data
3. Test users with different roles (Admin, Manager, Viewer) must be available
4. Browser developer tools should be available for testing

## Test Environment Setup

### Test Users Required
- **Admin User**: Full access to all features
- **Manager User**: Full access to all features  
- **Viewer User**: No access to Master Data features

### Test Data Required
- At least 15 departments for pagination testing
- At least 20 employees for pagination testing
- Employees assigned to different departments
- Mix of active and inactive records

## Running the Application

```bash
# Navigate to client_web directory
cd src/client_web

# Install dependencies (if not already done)
npm install

# Start development server
npm run dev
```

The application should be available at `http://localhost:5173` (or the configured port).

## Test Execution Instructions

### How to Use test-results.md
1. Open `.kiro/specs/department-employee-management-ui/test-results.md`
2. Fill in the test execution date and tester name
3. For each test case:
   - Follow the steps listed
   - Update Status: ✅ Pass / ❌ Fail / ⏳ Pending / 🚫 Blocked
   - Record actual results
   - Add notes for any issues or observations
4. Update the summary section at the end
5. Sign off when testing is complete


## Detailed Testing Instructions by Task

### 10.1 Routes Testing

#### Objective
Verify that routing works correctly and role-based access control is enforced.

#### Test Steps

**1. Test Department Route Access**
```
URL: http://localhost:5173/MasterData/DepartmentListView
Expected: Page loads with department list (for Admin/Manager)
```

**2. Test Employee Route Access**
```
URL: http://localhost:5173/MasterData/EmployeeListView
Expected: Page loads with employee list (for Admin/Manager)
```

**3. Test Role-Based Access**
- Login with different user roles
- Verify Admin and Manager can access both pages
- Verify Viewer is redirected with error message

**4. Test Unauthorized Access**
- Try accessing routes without authentication
- Verify redirect to login page

#### Common Issues to Check
- Routes not registered in router
- Permission checks not working
- Redirect not functioning
- Error messages not displaying

---

### 10.2 Department Management Testing

#### Objective
Verify all CRUD operations work correctly for departments.

#### Test Steps

**1. View Department List**
- Navigate to DepartmentListView
- Verify table displays with correct columns
- Verify data loads from API
- Check loading indicator appears during data fetch

**2. Search Functionality**
- Type in search box
- Verify real-time filtering works
- Test with partial matches
- Test with no matches
- Clear search and verify all data returns

**3. Create Department**
- Click "เพิ่มแผนก" button
- Verify drawer opens from right
- Fill in department name
- Toggle isActive checkbox
- Click save
- Verify success message
- Verify new department appears in list
- Verify row is highlighted

**4. Edit Department**
- Click edit button on a department
- Verify drawer opens with existing data
- Modify department name
- Click save
- Verify success message
- Verify changes appear in list
- Verify row is highlighted

**5. Delete Department**
- Click delete button
- Verify confirmation dialog appears
- Click confirm
- Verify success message
- Verify department removed from list

**6. View Department Details**
- Click view button
- Verify drawer opens
- Verify all fields display correctly
- Verify fields are read-only
- Click close button

**7. Pagination**
- Change items per page
- Verify data reloads
- Navigate to next page
- Navigate to previous page
- Verify page numbers update correctly

#### Validation Testing
- Try to save without department name
- Try to save with very short name (< 2 chars)
- Try to save with very long name (> 100 chars)
- Verify validation messages display

---

### 10.3 Employee Management Testing

#### Objective
Verify all CRUD operations and filtering work correctly for employees.

#### Test Steps

**1. View Employee List**
- Navigate to EmployeeListView
- Verify table displays with correct columns
- Verify profile images display (or default avatar)
- Verify data loads from API

**2. Search Functionality**
- Search by employee name
- Search by email
- Search by position
- Verify real-time filtering works
- Clear search

**3. Filter by Department**
- Select a department from dropdown
- Verify only employees from that department display
- Clear filter
- Verify all employees return

**4. Filter by Status**
- Select "ใช้งาน" (Active)
- Verify only active employees display
- Select "ไม่ใช้งาน" (Inactive)
- Verify only inactive employees display
- Clear filter

**5. Create Employee with Image**
- Click "เพิ่มพนักงาน" button
- Fill in all required fields:
  - userId
  - firstName
  - lastName
  - email
- Select department
- Upload profile image (valid JPG/PNG)
- Verify image preview displays
- Click save
- Verify success message
- Verify new employee appears with image
- Verify row is highlighted

**6. Create Employee without Image**
- Create employee without uploading image
- Verify default avatar displays in list

**7. Edit Employee**
- Click edit button
- Verify drawer opens with existing data
- Verify existing image displays (if any)
- Modify employee information
- Optionally upload new image
- Click save
- Verify success message
- Verify changes appear in list

**8. Delete Employee**
- Click delete button
- Verify confirmation dialog
- Confirm deletion
- Verify success message
- Verify employee removed

**9. View Employee Details**
- Click view button
- Verify all fields display correctly
- Verify profile image displays
- Verify department name displays
- Verify fields are read-only

**10. Pagination**
- Test pagination controls
- Change items per page
- Navigate between pages

#### Validation Testing
- Try to save without required fields
- Test email format validation
- Test phone format validation (10 digits)
- Try to upload invalid image type (e.g., .txt)
- Try to upload image > 5MB
- Verify validation messages display

---

### 10.4 Responsive Design Testing

#### Objective
Verify UI works correctly on different screen sizes.

#### Test Steps

**1. Desktop Testing (1920x1080)**
- Open browser developer tools
- Set viewport to 1920x1080
- Navigate to both Department and Employee views
- Verify all columns display
- Verify buttons are properly sized
- Open drawers
- Verify drawer width is appropriate (~550px)
- Test all CRUD operations

**2. Tablet Testing (768x1024)**
- Set viewport to 768x1024
- Navigate to both views
- Verify layout adjusts appropriately
- Verify table is scrollable horizontally if needed
- Open drawers
- Verify drawer width adjusts
- Test form interactions

**3. Mobile Testing (375x667)**
- Set viewport to 375x667
- Navigate to both views
- Verify mobile-friendly layout
- Verify table columns adjust or hide
- Open drawers
- Verify drawer takes full width
- Test form interactions
- Verify buttons are touch-friendly

**4. Drawer Behavior**
- Test drawer open/close on all screen sizes
- Verify smooth animations
- Verify drawer content is scrollable
- Test ESC key to close drawer
- Test clicking outside drawer (if applicable)

**5. Keyboard Navigation**
- Press ESC when drawer is open
- Verify drawer closes
- Test tab navigation through forms
- Verify focus indicators are visible

#### Browser Testing
Test on multiple browsers:
- Chrome
- Firefox
- Safari (if on Mac)
- Edge

---

### 10.5 Error Handling Testing

#### Objective
Verify application handles errors gracefully.

#### Test Steps

**1. API Error Simulation**
- Stop backend server
- Try to load department list
- Verify error message displays
- Try to create a department
- Verify error message displays
- Restart backend
- Verify operations work again

**2. Network Error Simulation**
- Open browser developer tools
- Go to Network tab
- Set throttling to "Offline"
- Try any operation
- Verify network error message
- Set back to "Online"

**3. Validation Errors**
- Test all validation scenarios:
  - Empty required fields
  - Invalid email format
  - Invalid phone format
  - Invalid image type
  - Image too large
- Verify field-level error messages
- Verify form submission is blocked

**4. Server Errors**
- Try to delete a department that has employees
- Verify appropriate error message
- Try to create duplicate data (if applicable)
- Verify error handling

**5. Authorization Errors**
- Login as Viewer
- Try to access restricted pages
- Verify redirect and error message
- Try to manipulate URL to access restricted features

#### Error Message Checklist
- [ ] Messages are in Thai
- [ ] Messages are clear and helpful
- [ ] Messages use SweetAlert
- [ ] Messages don't expose sensitive information
- [ ] Loading states are handled

---

### 10.6 Permission-Based Feature Testing

#### Objective
Verify features show/hide based on user permissions.

#### Test Steps

**1. Admin User Testing**
- Login as Admin
- Navigate to DepartmentListView
- Verify "เพิ่มแผนก" button is visible
- Verify edit buttons are visible on all rows
- Verify delete buttons are visible on all rows
- Verify view buttons are visible on all rows
- Test all CRUD operations successfully
- Navigate to EmployeeListView
- Verify all action buttons are visible
- Test all CRUD operations successfully

**2. Manager User Testing**
- Login as Manager
- Repeat all tests from Admin user
- Verify Manager has same access as Admin

**3. Viewer User Testing**
- Login as Viewer (or user without Admin/Manager role)
- Try to navigate to DepartmentListView
- Verify access denied or redirect
- Verify error message displays
- Try to navigate to EmployeeListView
- Verify access denied or redirect

**4. Permission Method Testing**
- Verify `canAccessMasterData()` works correctly
- Verify `canModifyMasterData()` works correctly
- Verify `canAccessDepartmentData()` works correctly
- Check console for any permission-related errors

**5. Button Visibility Testing**
- For each user role, verify:
  - Create buttons show/hide correctly
  - Edit buttons show/hide correctly
  - Delete buttons show/hide correctly
  - View buttons always visible (if user can access page)

---

## Common Issues and Troubleshooting

### Issue: Routes not working
**Solution**: 
- Check router configuration in `MasterData.ts`
- Verify components are properly imported
- Check for typos in route paths

### Issue: Permission checks failing
**Solution**:
- Verify RoleService is imported
- Check auth store has correct user roles
- Verify role names match exactly (case-sensitive)

### Issue: API calls failing
**Solution**:
- Verify backend is running
- Check BACKEND_API_URL constant
- Check network tab in browser dev tools
- Verify API endpoints match backend

### Issue: Images not displaying
**Solution**:
- Check image URL format
- Verify MinIO or file storage is configured
- Check CORS settings
- Verify image file was uploaded successfully

### Issue: Drawer not opening
**Solution**:
- Check dialog state variables
- Verify v-navigation-drawer props
- Check z-index in styles
- Look for JavaScript errors in console

### Issue: Validation not working
**Solution**:
- Verify validation rules are defined
- Check v-form ref is set correctly
- Verify form validation is called before submit

---

## Performance Checklist

- [ ] Initial page load is under 3 seconds
- [ ] Search filtering is responsive (< 300ms)
- [ ] Pagination changes load quickly
- [ ] Drawer animations are smooth
- [ ] No memory leaks (check with dev tools)
- [ ] Images load efficiently
- [ ] No unnecessary API calls

---

## Accessibility Checklist

- [ ] All buttons have tooltips
- [ ] Form fields have labels
- [ ] Keyboard navigation works
- [ ] Focus indicators are visible
- [ ] Color contrast is sufficient
- [ ] Screen reader compatible (basic)

---

## Security Checklist

- [ ] Routes are protected by authentication
- [ ] Role-based access control works
- [ ] Unauthorized users are redirected
- [ ] Sensitive data is not exposed in errors
- [ ] API calls include authentication tokens
- [ ] XSS protection (Vue auto-escapes)

---

## Sign-off Criteria

Before marking testing as complete, ensure:

1. ✅ All 35 test cases have been executed
2. ✅ All critical bugs are fixed
3. ✅ All requirements are met
4. ✅ Performance is acceptable
5. ✅ Security checks pass
6. ✅ Accessibility basics are covered
7. ✅ Documentation is updated

---

## Next Steps After Testing

1. Document all issues found in test-results.md
2. Create bug tickets for any failures
3. Retest after fixes are applied
4. Get stakeholder approval
5. Prepare for deployment

