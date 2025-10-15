# Manual Testing Guide for Authentication Endpoints

This guide provides step-by-step instructions for manually testing the authentication endpoints.

## Prerequisites

1. Ensure the application is running (via docker-compose or locally)
2. Have a tool like Postman, curl, or similar HTTP client ready
3. Base URL: `http://localhost:5000` (adjust if different)

## Test 16.1: Backend Authentication Endpoints

### Test 1: POST /api/auth/register - Register New User

**Request:**

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "username": "testuser",
    "password": "Test1234!",
    "confirmPassword": "Test1234!",
    "firstName": "Test",
    "lastName": "User"
  }'
```

**Expected Response (200 OK):**

```json
{
  "message": "ลงทะเบียนสำเร็จ"
}
```

### Test 2: POST /api/auth/register - Duplicate Email

**Request:**

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "username": "testuser2",
    "password": "Test1234!",
    "confirmPassword": "Test1234!",
    "firstName": "Test",
    "lastName": "User"
  }'
```

**Expected Response (400 Bad Request):**

```json
{
  "errors": ["Email 'testuser@example.com' is already taken."]
}
```

### Test 3: POST /api/auth/register - Password Mismatch

**Request:**

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser3@example.com",
    "username": "testuser3",
    "password": "Test1234!",
    "confirmPassword": "DifferentPassword!",
    "firstName": "Test",
    "lastName": "User"
  }'
```

**Expected Response (400 Bad Request):**

```json
{
  "errors": ["รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน"]
}
```

### Test 4: POST /api/auth/login - Login with Email

**Request:**

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "testuser@example.com",
    "password": "Test1234!"
  }'
```

**Expected Response (200 OK):**

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "guid-here",
  "email": "testuser@example.com",
  "username": "testuser",
  "firstName": "Test",
  "lastName": "User",
  "roles": [],
  "tokenExpiration": "2025-10-16T11:41:38Z"
}
```

### Test 5: POST /api/auth/login - Login with Username

**Request:**

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "testuser",
    "password": "Test1234!"
  }'
```

**Expected Response (200 OK):**
Same as Test 4

### Test 6: POST /api/auth/login - Invalid Credentials

**Request:**

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "testuser@example.com",
    "password": "WrongPassword!"
  }'
```

**Expected Response (400 Bad Request):**

```json
{
  "errors": ["อีเมลหรือรหัสผ่านไม่ถูกต้อง"]
}
```

### Test 7: GET /api/auth/me - Get Current User (Authenticated)

**Request:**

```bash
curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

**Expected Response (200 OK):**

```json
{
  "id": "guid-here",
  "userName": "testuser",
  "email": "testuser@example.com",
  "displayName": "testuser",
  "roleNames": []
}
```

### Test 8: GET /api/auth/me - Unauthorized (No Token)

**Request:**

```bash
curl -X GET http://localhost:5000/api/auth/me
```

**Expected Response (401 Unauthorized)**

### Test 9: GET /api/auth/me - Invalid Token

**Request:**

```bash
curl -X GET http://localhost:5000/api/auth/me \
  -H "Authorization: Bearer invalid.token.here"
```

**Expected Response (401 Unauthorized)**

## Test Checklist

- [ ] Register new user successfully
- [ ] Register with duplicate email fails
- [ ] Register with password mismatch fails
- [ ] Register with weak password fails
- [ ] Login with email successfully
- [ ] Login with username successfully
- [ ] Login with invalid password fails
- [ ] Login with non-existent user fails
- [ ] Get current user with valid token
- [ ] Get current user without token fails (401)
- [ ] Get current user with invalid token fails (401)
- [ ] Token includes correct user information
- [ ] Token includes user roles

## Notes

- Replace `YOUR_TOKEN_HERE` with the actual JWT token received from login
- Adjust the base URL if your application runs on a different port
- Check the token expiration time is set correctly (default: 24 hours)
- Verify that passwords are hashed in the database (never stored as plain text)
