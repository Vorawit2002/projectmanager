# Local Authentication Testing Report

## Overview

This document provides comprehensive testing instructions and validation for the local authentication system implementation.

## Test Environment Setup

### Prerequisites
1. Docker and Docker Compose installed
2. PostgreSQL database running
3. MinIO container running
4. Application backend running
5. Frontend application running

### Starting the Environment

```bash
# Start all services
docker-compose up -d

# Verify services are running
docker-compose ps

# Check logs
docker-compose logs -f
```

## Testing Tasks

### 16.1 Backend Authentication Endpoints ✓

**Status:** Ready for Testing

**Test Files Created:**
- `tests/Application.FunctionalTests/Authentication/Commands/LoginTests.cs`
- `tests/Application.FunctionalTests/Authentication/Commands/RegisterTests.cs`
- `tests/Application.FunctionalTests/Authentication/Queries/GetCurrentUserTests.cs`
- `tests/Application.FunctionalTests/Authentication/MANUAL_TEST_GUIDE.md`

**Manual Testing Guide:** See `tests/Application.FunctionalTests/Authentication/MANUAL_TEST_GUIDE.md`

**Key Tests:**
1. ✓ POST /api/auth/login - Login with email
2. ✓ POST /api/auth/login - Login with username
3. ✓ POST /api/auth/login - Invalid credentials
4. ✓ POST /api/auth/register - Register new user
5. ✓ POST /api/auth/register - Duplicate email
6. ✓ POST /api/auth/register - Password validation
7. ✓ GET /api/auth/me - Get current user (authenticated)
8. ✓ GET /api/auth/me - Unauthorized access

### 16.2 Frontend Login Flow

**Test Scenarios:**

#### Test 1: Login with Email
1. Navigate to `/login`
2. Enter email: `testuser@example.com`
3. Enter password: `Test1234!`
4. Click "เข้าสู่ระบบ"
5. **Expected:** Redirect to dashboard, token stored in localStorage

#### Test 2: Login with Username
1. Navigate to `/login`
2. Enter username: `testuser`
3. Enter password: `Test1234!`
4. Click "เข้าสู่ระบบ"
5. **Expected:** Redirect to dashboard, token stored in localStorage

#### Test 3: Login with Invalid Credentials
1. Navigate to `/login`
2. Enter email: `testuser@example.com`
3. Enter password: `WrongPassword!`
4. Click "เข้าสู่ระบบ"
5. **Expected:** Error message displayed: "อีเมลหรือรหัสผ่านไม่ถูกต้อง"

#### Test 4: Form Validation
1. Navigate to `/login`
2. Leave email field empty
3. Click "เข้าสู่ระบบ"
4. **Expected:** Validation error: "กรุณากรอกอีเมลหรือชื่อผู้ใช้"
5. Enter email, leave password empty
6. **Expected:** Validation error: "กรุณากรอกรหัสผ่าน"

#### Test 5: Loading State
1. Navigate to `/login`
2. Enter valid credentials
3. Click "เข้าสู่ระบบ"
4. **Expected:** Loading indicator shown during API call
5. Button disabled during loading

### 16.3 Frontend Register Flow

**Test Scenarios:**

#### Test 1: Register New User
1. Navigate to `/register`
2. Fill in all fields:
   - Email: `newuser@example.com`
   - Username: `newuser`
   - Password: `NewUser1234!`
   - Confirm Password: `NewUser1234!`
   - First Name: `New`
   - Last Name: `User`
3. Click "ลงทะเบียน"
4. **Expected:** Success message, redirect to login page

#### Test 2: Register with Duplicate Email
1. Navigate to `/register`
2. Enter email that already exists: `testuser@example.com`
3. Fill in other fields
4. Click "ลงทะเบียน"
5. **Expected:** Error message: "อีเมลนี้ถูกใช้งานแล้ว"

#### Test 3: Password Validation
1. Navigate to `/register`
2. Enter password: `123` (too short)
3. **Expected:** Validation error: "รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร"

#### Test 4: Confirm Password Mismatch
1. Navigate to `/register`
2. Enter password: `Test1234!`
3. Enter confirm password: `Different1234!`
4. **Expected:** Validation error: "รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน"

#### Test 5: Email Format Validation
1. Navigate to `/register`
2. Enter invalid email: `notanemail`
3. **Expected:** Validation error: "รูปแบบอีเมลไม่ถูกต้อง"

### 16.4 Protected Routes and Token Expiration

**Test Scenarios:**

#### Test 1: Redirect to Login When No Token
1. Clear localStorage (remove token)
2. Navigate to `/dashboard` or any protected route
3. **Expected:** Redirect to `/login`

#### Test 2: Token Expiration Handling
1. Login successfully
2. Manually set token expiration to past date in localStorage
3. Navigate to any protected route
4. **Expected:** Redirect to `/login` with message "เซสชันหมดอายุ กรุณาเข้าสู่ระบบใหม่"

#### Test 3: Logout Functionality
1. Login successfully
2. Navigate to dashboard
3. Click logout button
4. **Expected:** 
   - Token removed from localStorage
   - Redirect to `/login`
   - Cannot access protected routes

#### Test 4: Token Persistence
1. Login successfully
2. Close browser tab
3. Open new tab and navigate to application
4. **Expected:** Still logged in (if token not expired)

### 16.5 MinIO Integration

**Test Scenarios:**

#### Test 1: File Upload
1. Login to application
2. Navigate to a feature that uploads files (e.g., attachments)
3. Select a file and upload
4. **Expected:** File uploaded successfully to MinIO

#### Test 2: File Download
1. Upload a file (from Test 1)
2. Click download button
3. **Expected:** File downloads successfully

#### Test 3: MinIO Console Access
1. Open browser to `http://localhost:9001`
2. Login with credentials:
   - Username: `minioadmin`
   - Password: `minioadmin`
3. **Expected:** MinIO console loads successfully
4. Navigate to buckets
5. **Expected:** See `projectmanagement` bucket with uploaded files

#### Test 4: MinIO Container Persistence
1. Upload files to MinIO
2. Stop docker-compose: `docker-compose down`
3. Start docker-compose: `docker-compose up -d`
4. **Expected:** Files still exist in MinIO

### 16.6 Data Migration

**Test Scenarios:**

#### Test 1: Migration Script Execution
1. Ensure you have existing users in database
2. Login as Administrator
3. Call migration endpoint:
```bash
curl -X POST http://localhost:5000/api/auth/migrate-users \
  -H "Authorization: Bearer ADMIN_TOKEN"
```
4. **Expected:** Success response with migration results

#### Test 2: Existing Users Can Login
1. After running migration
2. Login with existing user credentials
3. **Expected:** Login successful

#### Test 3: RequirePasswordChange Flag
1. Login with migrated user
2. Check if prompted to change password
3. **Expected:** User should be prompted to change password on first login

#### Test 4: Roles Preserved
1. Login with migrated user that had roles
2. Check user roles in response
3. **Expected:** Roles are preserved from old system

## Validation Checklist

### Backend
- [ ] All authentication endpoints respond correctly
- [ ] JWT tokens are generated with correct claims
- [ ] Password hashing works properly (bcrypt/PBKDF2)
- [ ] Token validation works correctly
- [ ] Error messages are in Thai
- [ ] Unauthorized requests return 401
- [ ] Invalid requests return 400 with error details

### Frontend
- [ ] Login page renders correctly
- [ ] Register page renders correctly
- [ ] Form validation works on all fields
- [ ] Error messages display in Thai
- [ ] Loading states show during API calls
- [ ] Successful login redirects to dashboard
- [ ] Successful register redirects to login
- [ ] Token is stored in localStorage
- [ ] Token is sent in Authorization header

### Security
- [ ] Passwords are never stored in plain text
- [ ] JWT secret key is secure (min 32 characters)
- [ ] Tokens expire after configured time (24 hours)
- [ ] Protected routes require valid token
- [ ] Invalid/expired tokens are rejected
- [ ] CORS is configured properly

### Integration
- [ ] MinIO container starts with docker-compose
- [ ] Files can be uploaded to MinIO
- [ ] Files can be downloaded from MinIO
- [ ] MinIO console is accessible
- [ ] MinIO data persists across restarts
- [ ] No references to ntiportal remain
- [ ] No references to Line Bot remain
- [ ] No references to OpenID Connect remain

### Migration
- [ ] Migration script runs successfully
- [ ] Existing users can login
- [ ] RequirePasswordChange flag is set correctly
- [ ] User roles are preserved
- [ ] No data loss during migration

## Known Issues

Document any issues found during testing here:

1. **Issue:** [Description]
   - **Severity:** High/Medium/Low
   - **Steps to Reproduce:** [Steps]
   - **Expected:** [Expected behavior]
   - **Actual:** [Actual behavior]
   - **Status:** Open/Fixed

## Test Results Summary

| Test Category | Total Tests | Passed | Failed | Skipped |
|--------------|-------------|--------|--------|---------|
| Backend Endpoints | 8 | - | - | - |
| Frontend Login | 5 | - | - | - |
| Frontend Register | 5 | - | - | - |
| Protected Routes | 4 | - | - | - |
| MinIO Integration | 4 | - | - | - |
| Data Migration | 4 | - | - | - |
| **Total** | **30** | **-** | **-** | **-** |

## Recommendations

1. Run all manual tests before deploying to production
2. Create automated E2E tests for critical flows
3. Monitor authentication errors in production
4. Set up alerts for failed login attempts
5. Regularly review and rotate JWT secret keys
6. Implement rate limiting on login endpoint
7. Add account lockout after multiple failed attempts
8. Consider implementing refresh tokens for better UX

## Sign-off

- [ ] All tests passed
- [ ] No critical issues found
- [ ] Documentation updated
- [ ] Ready for production deployment

**Tested by:** _________________  
**Date:** _________________  
**Approved by:** _________________  
**Date:** _________________
