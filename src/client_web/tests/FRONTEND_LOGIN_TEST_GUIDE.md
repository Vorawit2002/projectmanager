# Frontend Login Flow Testing Guide

## Test 16.2: Frontend Login Flow

This guide provides detailed test cases for the frontend login functionality.

## Prerequisites

1. Backend API is running at `http://localhost:5000`
2. Frontend is running at `http://localhost:3000` (or configured port)
3. At least one test user exists in the database:
   - Email: `testuser@example.com`
   - Username: `testuser`
   - Password: `Test1234!`

## Test Cases

### Test Case 1: Login with Email

**Objective:** Verify that users can login using their email address

**Steps:**
1. Open browser and navigate to `http://localhost:3000/login`
2. Verify the login page loads correctly
3. In the "อีเมลหรือชื่อผู้ใช้" field, enter: `testuser@example.com`
4. In the "รหัสผ่าน" field, enter: `Test1234!`
5. Click the "เข้าสู่ระบบ" button

**Expected Results:**
- ✓ Loading indicator appears during authentication
- ✓ No error messages are displayed
- ✓ User is redirected to the dashboard/home page
- ✓ JWT token is stored in localStorage (key: `token` or similar)
- ✓ User information is stored in the auth store
- ✓ Navigation bar shows user as logged in

**Verification:**
```javascript
// Open browser console and check:
localStorage.getItem('token') // Should return JWT token
// Check auth store state
```

---

### Test Case 2: Login with Username

**Objective:** Verify that users can login using their username

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. In the "อีเมลหรือชื่อผู้ใช้" field, enter: `testuser`
3. In the "รหัสผ่าน" field, enter: `Test1234!`
4. Click the "เข้าสู่ระบบ" button

**Expected Results:**
- ✓ Loading indicator appears
- ✓ User is successfully authenticated
- ✓ Redirect to dashboard
- ✓ Token stored in localStorage
- ✓ Same behavior as email login

---

### Test Case 3: Login with Invalid Password

**Objective:** Verify proper error handling for incorrect password

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. In the "อีเมลหรือชื่อผู้ใช้" field, enter: `testuser@example.com`
3. In the "รหัสผ่าน" field, enter: `WrongPassword123!`
4. Click the "เข้าสู่ระบบ" button

**Expected Results:**
- ✓ Loading indicator appears briefly
- ✓ Error message is displayed: "อีเมลหรือรหัสผ่านไม่ถูกต้อง" (or similar)
- ✓ User remains on login page
- ✓ No token is stored in localStorage
- ✓ Password field is cleared or remains filled (based on UX design)
- ✓ Error message is styled appropriately (red color, icon, etc.)

---

### Test Case 4: Login with Non-existent User

**Objective:** Verify error handling for non-existent users

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. In the "อีเมลหรือชื่อผู้ใช้" field, enter: `nonexistent@example.com`
3. In the "รหัสผ่าน" field, enter: `Test1234!`
4. Click the "เข้าสู่ระบบ" button

**Expected Results:**
- ✓ Error message is displayed: "อีเมลหรือรหัสผ่านไม่ถูกต้อง"
- ✓ User remains on login page
- ✓ No token is stored

---

### Test Case 5: Form Validation - Empty Email

**Objective:** Verify client-side validation for required fields

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Leave the "อีเมลหรือชื่อผู้ใช้" field empty
3. Enter password: `Test1234!`
4. Click the "เข้าสู่ระบบ" button

**Expected Results:**
- ✓ Validation error appears: "กรุณากรอกอีเมลหรือชื่อผู้ใช้"
- ✓ Form is not submitted
- ✓ No API call is made
- ✓ Email field is highlighted/marked as invalid

---

### Test Case 6: Form Validation - Empty Password

**Objective:** Verify password field validation

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Enter email: `testuser@example.com`
3. Leave the "รหัสผ่าน" field empty
4. Click the "เข้าสู่ระบบ" button

**Expected Results:**
- ✓ Validation error appears: "กรุณากรอกรหัสผ่าน"
- ✓ Form is not submitted
- ✓ Password field is highlighted as invalid

---

### Test Case 7: Form Validation - Both Fields Empty

**Objective:** Verify validation when all fields are empty

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Leave both fields empty
3. Click the "เข้าสู่ระบบ" button

**Expected Results:**
- ✓ Validation errors appear for both fields
- ✓ Form is not submitted

---

### Test Case 8: Loading State

**Objective:** Verify loading indicators during authentication

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Enter valid credentials
3. Click the "เข้าสู่ระบบ" button
4. Observe the UI during the API call

**Expected Results:**
- ✓ Loading spinner/indicator appears
- ✓ Login button is disabled during loading
- ✓ Login button text changes (e.g., "กำลังเข้าสู่ระบบ...")
- ✓ Form fields are disabled during loading
- ✓ Loading state clears after response

---

### Test Case 9: Remember Me Functionality (if implemented)

**Objective:** Verify remember me checkbox functionality

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Enter valid credentials
3. Check the "จดจำฉัน" checkbox (if available)
4. Click login
5. Close browser completely
6. Reopen browser and navigate to application

**Expected Results:**
- ✓ User is still logged in (if remember me was checked)
- ✓ Token persists across browser sessions

---

### Test Case 10: Navigation to Register Page

**Objective:** Verify link to registration page

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Look for "สมัครสมาชิก" or "ลงทะเบียน" link
3. Click the link

**Expected Results:**
- ✓ User is navigated to `/register` page
- ✓ Registration form is displayed

---

### Test Case 11: Password Visibility Toggle

**Objective:** Verify show/hide password functionality

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Enter password: `Test1234!`
3. Click the eye icon (if available) to show password
4. Click again to hide password

**Expected Results:**
- ✓ Password is visible as plain text when eye icon is clicked
- ✓ Password is hidden (dots/asterisks) when clicked again
- ✓ Icon changes to indicate current state

---

### Test Case 12: Keyboard Navigation

**Objective:** Verify keyboard accessibility

**Steps:**
1. Navigate to `http://localhost:3000/login`
2. Use Tab key to navigate through form fields
3. Press Enter in password field to submit

**Expected Results:**
- ✓ Tab key moves focus between fields in logical order
- ✓ Pressing Enter in password field submits the form
- ✓ Focus indicators are visible

---

### Test Case 13: Network Error Handling

**Objective:** Verify error handling when backend is unavailable

**Steps:**
1. Stop the backend server
2. Navigate to `http://localhost:3000/login`
3. Enter valid credentials
4. Click login

**Expected Results:**
- ✓ Error message is displayed: "ไม่สามารถเชื่อมต่อกับเซิร์ฟเวอร์ได้"
- ✓ User remains on login page
- ✓ Loading state clears

---

### Test Case 14: Responsive Design

**Objective:** Verify login page works on different screen sizes

**Steps:**
1. Open login page on desktop (1920x1080)
2. Open login page on tablet (768x1024)
3. Open login page on mobile (375x667)

**Expected Results:**
- ✓ Layout adapts to screen size
- ✓ All elements are visible and accessible
- ✓ Form is usable on all devices
- ✓ No horizontal scrolling required

---

### Test Case 15: Browser Compatibility

**Objective:** Verify login works across different browsers

**Browsers to Test:**
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

**Expected Results:**
- ✓ Login functionality works in all browsers
- ✓ UI renders correctly in all browsers
- ✓ No console errors

---

## Automated Test Checklist

If implementing automated tests, ensure coverage for:

- [ ] Successful login with email
- [ ] Successful login with username
- [ ] Failed login with invalid credentials
- [ ] Form validation (empty fields)
- [ ] Loading states
- [ ] Error message display
- [ ] Token storage
- [ ] Redirect after login
- [ ] Navigation to register page

## Test Data

### Valid Test Users

| Email | Username | Password | Roles |
|-------|----------|----------|-------|
| testuser@example.com | testuser | Test1234! | [] |
| admin@example.com | admin | Admin1234! | [Administrator] |

### Invalid Test Data

| Type | Value | Expected Error |
|------|-------|----------------|
| Invalid email | notanemail | อีเมลหรือรหัสผ่านไม่ถูกต้อง |
| Wrong password | WrongPass123! | อีเมลหรือรหัสผ่านไม่ถูกต้อง |
| Empty email | (empty) | กรุณากรอกอีเมลหรือชื่อผู้ใช้ |
| Empty password | (empty) | กรุณากรอกรหัสผ่าน |

## Debugging Tips

### Check Token in Console
```javascript
// View stored token
console.log(localStorage.getItem('token'));

// Decode JWT token (without verification)
function parseJwt(token) {
  const base64Url = token.split('.')[1];
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
  const jsonPayload = decodeURIComponent(atob(base64).split('').map(c => {
    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
  }).join(''));
  return JSON.parse(jsonPayload);
}

console.log(parseJwt(localStorage.getItem('token')));
```

### Check Auth Store State
```javascript
// In Vue DevTools, inspect the auth store
// Or in console:
console.log(window.$nuxt.$store.state.auth); // Adjust based on your setup
```

### Check Network Requests
1. Open DevTools (F12)
2. Go to Network tab
3. Filter by "auth" or "login"
4. Check request/response details

## Test Results Template

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1. Login with Email | ⬜ Pass / ⬜ Fail | |
| 2. Login with Username | ⬜ Pass / ⬜ Fail | |
| 3. Invalid Password | ⬜ Pass / ⬜ Fail | |
| 4. Non-existent User | ⬜ Pass / ⬜ Fail | |
| 5. Empty Email Validation | ⬜ Pass / ⬜ Fail | |
| 6. Empty Password Validation | ⬜ Pass / ⬜ Fail | |
| 7. Both Fields Empty | ⬜ Pass / ⬜ Fail | |
| 8. Loading State | ⬜ Pass / ⬜ Fail | |
| 9. Remember Me | ⬜ Pass / ⬜ Fail / ⬜ N/A | |
| 10. Navigate to Register | ⬜ Pass / ⬜ Fail | |
| 11. Password Visibility | ⬜ Pass / ⬜ Fail | |
| 12. Keyboard Navigation | ⬜ Pass / ⬜ Fail | |
| 13. Network Error | ⬜ Pass / ⬜ Fail | |
| 14. Responsive Design | ⬜ Pass / ⬜ Fail | |
| 15. Browser Compatibility | ⬜ Pass / ⬜ Fail | |

**Tested By:** _______________  
**Date:** _______________  
**Browser:** _______________  
**OS:** _______________
