# Frontend Register Flow Testing Guide

## Test 16.3: Frontend Register Flow

This guide provides detailed test cases for the frontend registration functionality.

## Prerequisites

1. Backend API is running at `http://localhost:5000`
2. Frontend is running at `http://localhost:3000` (or configured port)
3. Database is accessible and can create new users

## Test Cases

### Test Case 1: Register New User Successfully

**Objective:** Verify that new users can register successfully

**Steps:**
1. Navigate to `http://localhost:3000/register`
2. Fill in all fields:
   - อีเมล: `newuser@example.com`
   - ชื่อผู้ใช้: `newuser`
   - รหัสผ่าน: `NewUser1234!`
   - ยืนยันรหัสผ่าน: `NewUser1234!`
   - ชื่อ: `New`
   - นามสกุล: `User`
3. Click "ลงทะเบียน" button

**Expected Results:**
- ✓ Loading indicator appears during registration
- ✓ Success message is displayed: "ลงทะเบียนสำเร็จ"
- ✓ User is redirected to `/login` page
- ✓ Success notification/toast appears
- ✓ No errors in console

**Verification:**
- Try logging in with the new credentials
- Should be able to login successfully

---

### Test Case 2: Register with Duplicate Email

**Objective:** Verify error handling for duplicate email addresses

**Steps:**
1. First, register a user with email: `duplicate@example.com`
2. Navigate to `/register` again
3. Try to register another user with the same email: `duplicate@example.com`
4. Use different username: `differentuser`
5. Fill in other fields
6. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Error message is displayed: "อีเมลนี้ถูกใช้งานแล้ว" or "Email is already taken"
- ✓ User remains on registration page
- ✓ Form data is preserved (except password fields)
- ✓ Email field is highlighted as invalid

---

### Test Case 3: Register with Duplicate Username

**Objective:** Verify error handling for duplicate usernames

**Steps:**
1. Register a user with username: `testuser`
2. Navigate to `/register` again
3. Try to register with same username: `testuser`
4. Use different email: `different@example.com`
5. Fill in other fields
6. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Error message is displayed: "ชื่อผู้ใช้นี้ถูกใช้งานแล้ว" or similar
- ✓ User remains on registration page
- ✓ Username field is highlighted as invalid

---

### Test Case 4: Password Too Short

**Objective:** Verify password length validation

**Steps:**
1. Navigate to `/register`
2. Fill in all fields
3. Enter password: `123` (less than 6 characters)
4. Enter confirm password: `123`
5. Try to submit or move to next field

**Expected Results:**
- ✓ Validation error appears: "รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร"
- ✓ Password field is highlighted as invalid
- ✓ Submit button may be disabled
- ✓ Form cannot be submitted

---

### Test Case 5: Password Mismatch

**Objective:** Verify password confirmation validation

**Steps:**
1. Navigate to `/register`
2. Fill in all fields
3. Enter password: `Test1234!`
4. Enter confirm password: `Different1234!`
5. Click "ลงทะเบียน" or move focus away from confirm password field

**Expected Results:**
- ✓ Validation error appears: "รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน"
- ✓ Confirm password field is highlighted as invalid
- ✓ Form cannot be submitted

---

### Test Case 6: Invalid Email Format

**Objective:** Verify email format validation

**Steps:**
1. Navigate to `/register`
2. Enter invalid email formats:
   - `notanemail`
   - `missing@domain`
   - `@nodomain.com`
   - `spaces in@email.com`
3. Try to submit or move to next field

**Expected Results:**
- ✓ Validation error appears: "รูปแบบอีเมลไม่ถูกต้อง"
- ✓ Email field is highlighted as invalid
- ✓ Form cannot be submitted

---

### Test Case 7: Required Field Validation - Email

**Objective:** Verify email field is required

**Steps:**
1. Navigate to `/register`
2. Leave email field empty
3. Fill in other fields
4. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Validation error appears: "กรุณากรอกอีเมล"
- ✓ Email field is highlighted
- ✓ Form is not submitted

---

### Test Case 8: Required Field Validation - Username

**Objective:** Verify username field is required

**Steps:**
1. Navigate to `/register`
2. Leave username field empty
3. Fill in other fields
4. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Validation error appears: "กรุณากรอกชื่อผู้ใช้"
- ✓ Username field is highlighted
- ✓ Form is not submitted

---

### Test Case 9: Required Field Validation - Password

**Objective:** Verify password field is required

**Steps:**
1. Navigate to `/register`
2. Leave password field empty
3. Fill in other fields
4. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Validation error appears: "กรุณากรอกรหัสผ่าน"
- ✓ Password field is highlighted
- ✓ Form is not submitted

---

### Test Case 10: Required Field Validation - Confirm Password

**Objective:** Verify confirm password field is required

**Steps:**
1. Navigate to `/register`
2. Fill in password but leave confirm password empty
3. Fill in other fields
4. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Validation error appears: "กรุณายืนยันรหัสผ่าน"
- ✓ Confirm password field is highlighted
- ✓ Form is not submitted

---

### Test Case 11: Required Field Validation - First Name

**Objective:** Verify first name field is required

**Steps:**
1. Navigate to `/register`
2. Leave first name field empty
3. Fill in other fields
4. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Validation error appears: "กรุณากรอกชื่อ"
- ✓ First name field is highlighted
- ✓ Form is not submitted

---

### Test Case 12: Required Field Validation - Last Name

**Objective:** Verify last name field is required

**Steps:**
1. Navigate to `/register`
2. Leave last name field empty
3. Fill in other fields
4. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Validation error appears: "กรุณากรอกนามสกุล"
- ✓ Last name field is highlighted
- ✓ Form is not submitted

---

### Test Case 13: All Fields Empty

**Objective:** Verify validation when all fields are empty

**Steps:**
1. Navigate to `/register`
2. Leave all fields empty
3. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Validation errors appear for all required fields
- ✓ All fields are highlighted as invalid
- ✓ Form is not submitted

---

### Test Case 14: Loading State

**Objective:** Verify loading indicators during registration

**Steps:**
1. Navigate to `/register`
2. Fill in all fields with valid data
3. Click "ลงทะเบียน"
4. Observe UI during API call

**Expected Results:**
- ✓ Loading spinner/indicator appears
- ✓ Submit button is disabled during loading
- ✓ Button text changes (e.g., "กำลังลงทะเบียน...")
- ✓ Form fields are disabled during loading
- ✓ Loading state clears after response

---

### Test Case 15: Password Strength Indicator (if implemented)

**Objective:** Verify password strength indicator

**Steps:**
1. Navigate to `/register`
2. Enter various passwords:
   - Weak: `123456`
   - Medium: `Test1234`
   - Strong: `Test1234!@#`

**Expected Results:**
- ✓ Strength indicator updates in real-time
- ✓ Color changes based on strength (red/yellow/green)
- ✓ Text indicates strength level
- ✓ Suggestions for stronger password (if weak)

---

### Test Case 16: Password Visibility Toggle

**Objective:** Verify show/hide password functionality

**Steps:**
1. Navigate to `/register`
2. Enter password: `Test1234!`
3. Click eye icon on password field
4. Click eye icon on confirm password field

**Expected Results:**
- ✓ Password becomes visible as plain text
- ✓ Confirm password becomes visible
- ✓ Icons change to indicate visibility state
- ✓ Clicking again hides the passwords

---

### Test Case 17: Navigation to Login Page

**Objective:** Verify link to login page

**Steps:**
1. Navigate to `/register`
2. Look for "เข้าสู่ระบบ" or "มีบัญชีอยู่แล้ว?" link
3. Click the link

**Expected Results:**
- ✓ User is navigated to `/login` page
- ✓ Login form is displayed
- ✓ No data loss warning (form is empty)

---

### Test Case 18: Form Reset

**Objective:** Verify form can be reset/cleared

**Steps:**
1. Navigate to `/register`
2. Fill in all fields
3. Look for "ล้างข้อมูล" or reset button (if available)
4. Click reset

**Expected Results:**
- ✓ All fields are cleared
- ✓ Validation errors are cleared
- ✓ Form returns to initial state

---

### Test Case 19: Keyboard Navigation

**Objective:** Verify keyboard accessibility

**Steps:**
1. Navigate to `/register`
2. Use Tab key to navigate through all fields
3. Press Enter in last field to submit

**Expected Results:**
- ✓ Tab key moves focus in logical order
- ✓ All fields are accessible via keyboard
- ✓ Focus indicators are visible
- ✓ Enter key submits form (if all fields valid)

---

### Test Case 20: Network Error Handling

**Objective:** Verify error handling when backend is unavailable

**Steps:**
1. Stop the backend server
2. Navigate to `/register`
3. Fill in all fields with valid data
4. Click "ลงทะเบียน"

**Expected Results:**
- ✓ Error message is displayed: "ไม่สามารถเชื่อมต่อกับเซิร์ฟเวอร์ได้"
- ✓ User remains on registration page
- ✓ Form data is preserved
- ✓ Loading state clears

---

### Test Case 21: Special Characters in Fields

**Objective:** Verify handling of special characters

**Steps:**
1. Navigate to `/register`
2. Try entering special characters in various fields:
   - Username: `test<script>alert('xss')</script>`
   - First Name: `Test'OR'1'='1`
   - Last Name: `User<img src=x>`
3. Submit form

**Expected Results:**
- ✓ Special characters are properly escaped
- ✓ No XSS vulnerabilities
- ✓ No SQL injection vulnerabilities
- ✓ Form submits successfully or shows appropriate validation

---

### Test Case 22: Responsive Design

**Objective:** Verify registration page works on different screen sizes

**Steps:**
1. Open register page on desktop (1920x1080)
2. Open register page on tablet (768x1024)
3. Open register page on mobile (375x667)

**Expected Results:**
- ✓ Layout adapts to screen size
- ✓ All fields are visible and accessible
- ✓ Form is usable on all devices
- ✓ No horizontal scrolling required
- ✓ Buttons are appropriately sized for touch

---

### Test Case 23: Browser Compatibility

**Objective:** Verify registration works across different browsers

**Browsers to Test:**
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

**Expected Results:**
- ✓ Registration functionality works in all browsers
- ✓ UI renders correctly in all browsers
- ✓ Validation works consistently
- ✓ No console errors

---

### Test Case 24: Terms and Conditions (if implemented)

**Objective:** Verify terms acceptance checkbox

**Steps:**
1. Navigate to `/register`
2. Fill in all fields
3. Leave terms checkbox unchecked
4. Try to submit

**Expected Results:**
- ✓ Validation error: "กรุณายอมรับข้อกำหนดและเงื่อนไข"
- ✓ Form cannot be submitted
- ✓ Checkbox is highlighted

---

### Test Case 25: Username Format Validation

**Objective:** Verify username format requirements

**Steps:**
1. Navigate to `/register`
2. Try various username formats:
   - With spaces: `test user`
   - With special chars: `test@user`
   - Too short: `ab`
   - Too long: `verylongusernamethatexceedslimit`

**Expected Results:**
- ✓ Invalid formats show validation errors
- ✓ Valid formats are accepted
- ✓ Clear error messages explain requirements

---

## Test Data

### Valid Registration Data

| Field | Value |
|-------|-------|
| Email | newuser@example.com |
| Username | newuser |
| Password | NewUser1234! |
| Confirm Password | NewUser1234! |
| First Name | New |
| Last Name | User |

### Invalid Test Data

| Field | Invalid Value | Expected Error |
|-------|---------------|----------------|
| Email | notanemail | รูปแบบอีเมลไม่ถูกต้อง |
| Email | (empty) | กรุณากรอกอีเมล |
| Username | (empty) | กรุณากรอกชื่อผู้ใช้ |
| Password | 123 | รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร |
| Password | (empty) | กรุณากรอกรหัสผ่าน |
| Confirm Password | Different1234! | รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน |
| First Name | (empty) | กรุณากรอกชื่อ |
| Last Name | (empty) | กรุณากรอกนามสกุล |

## Automated Test Checklist

If implementing automated tests, ensure coverage for:

- [ ] Successful registration
- [ ] Duplicate email error
- [ ] Duplicate username error
- [ ] Password validation (length, strength)
- [ ] Password mismatch error
- [ ] Email format validation
- [ ] All required field validations
- [ ] Loading states
- [ ] Error message display
- [ ] Redirect after success
- [ ] Navigation to login page

## Debugging Tips

### Check Network Request
```javascript
// In DevTools Network tab, check the registration request:
// Request URL: http://localhost:5000/api/auth/register
// Method: POST
// Request Body:
{
  "email": "newuser@example.com",
  "username": "newuser",
  "password": "NewUser1234!",
  "confirmPassword": "NewUser1234!",
  "firstName": "New",
  "lastName": "User"
}
```

### Check Form Validation
```javascript
// In Vue DevTools, inspect form validation state
// Or check validation errors in component data
```

### Check Console for Errors
```javascript
// Open DevTools Console (F12)
// Look for any JavaScript errors or warnings
// Check for failed API calls
```

## Test Results Template

| Test Case | Status | Notes |
|-----------|--------|-------|
| 1. Register Successfully | ⬜ Pass / ⬜ Fail | |
| 2. Duplicate Email | ⬜ Pass / ⬜ Fail | |
| 3. Duplicate Username | ⬜ Pass / ⬜ Fail | |
| 4. Password Too Short | ⬜ Pass / ⬜ Fail | |
| 5. Password Mismatch | ⬜ Pass / ⬜ Fail | |
| 6. Invalid Email Format | ⬜ Pass / ⬜ Fail | |
| 7. Email Required | ⬜ Pass / ⬜ Fail | |
| 8. Username Required | ⬜ Pass / ⬜ Fail | |
| 9. Password Required | ⬜ Pass / ⬜ Fail | |
| 10. Confirm Password Required | ⬜ Pass / ⬜ Fail | |
| 11. First Name Required | ⬜ Pass / ⬜ Fail | |
| 12. Last Name Required | ⬜ Pass / ⬜ Fail | |
| 13. All Fields Empty | ⬜ Pass / ⬜ Fail | |
| 14. Loading State | ⬜ Pass / ⬜ Fail | |
| 15. Password Strength | ⬜ Pass / ⬜ Fail / ⬜ N/A | |
| 16. Password Visibility | ⬜ Pass / ⬜ Fail | |
| 17. Navigate to Login | ⬜ Pass / ⬜ Fail | |
| 18. Form Reset | ⬜ Pass / ⬜ Fail / ⬜ N/A | |
| 19. Keyboard Navigation | ⬜ Pass / ⬜ Fail | |
| 20. Network Error | ⬜ Pass / ⬜ Fail | |
| 21. Special Characters | ⬜ Pass / ⬜ Fail | |
| 22. Responsive Design | ⬜ Pass / ⬜ Fail | |
| 23. Browser Compatibility | ⬜ Pass / ⬜ Fail | |
| 24. Terms Acceptance | ⬜ Pass / ⬜ Fail / ⬜ N/A | |
| 25. Username Format | ⬜ Pass / ⬜ Fail | |

**Tested By:** _______________  
**Date:** _______________  
**Browser:** _______________  
**OS:** _______________
