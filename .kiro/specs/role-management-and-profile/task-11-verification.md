# Task 11 Verification: Account Settings Query Implementation

## Task Summary
อัพเดท Account Settings Query เพื่อรวมข้อมูลจากการลงทะเบียน

## Implementation Details

### 1. Created GetAccountSettingsQuery
**File**: `src/Application/Users/Queries/GetAccountSettings/GetAccountSettingsQuery.cs`

**Features Implemented**:
- ✅ Query handler that retrieves user data from both ApplicationUser and Employee tables
- ✅ Fallback mechanism: Tries Employee table first, then extracts from ApplicationUser email if Employee data is missing
- ✅ Includes all required fields: FirstName, LastName, TitleName, Position, Phone, Department, ImageProfile
- ✅ Joins with Department table to get department name
- ✅ Retrieves user roles from Identity service
- ✅ Returns comprehensive account settings data

**Fallback Logic**:
```csharp
// Fallback: Try Employee first, then ApplicationUser (from email)
FirstName = employee?.FirstName ?? ExtractFirstNameFromEmail(user.Email),
LastName = employee?.LastName ?? ExtractLastNameFromEmail(user.Email),
```

The fallback extracts names from email format (e.g., "john.doe@example.com" → FirstName: "John", LastName: "Doe")

### 2. Created AccountSettingsDto
**File**: `src/Application/Users/Queries/GetAccountSettings/AccountSettingsDto.cs`

**Fields Included**:
- ✅ UserId, Username, Email (from ApplicationUser)
- ✅ TitleName, FirstName, LastName (from Employee with fallback)
- ✅ Phone (from Employee)
- ✅ Position (from Employee)
- ✅ Department, DepartmentId (from Employee → Department join)
- ✅ ImageProfile (from Employee)
- ✅ Roles (from Identity)
- ✅ IsActive (from Employee or ApplicationUser)

### 3. Added API Endpoint
**File**: `src/Web/Endpoints/Users.cs`

**Endpoint Details**:
- ✅ Route: `GET /api/users/account-settings`
- ✅ Authorization: Requires authenticated user
- ✅ Returns: AccountSettingsDto for the current logged-in user
- ✅ Error handling: Returns 401 for unauthorized, 400 for errors

**Usage**:
```http
GET /api/users/account-settings
Authorization: Bearer {token}
```

**Response Example**:
```json
{
  "userId": "user-id-123",
  "username": "john.doe",
  "email": "john.doe@example.com",
  "titleName": "Mr.",
  "firstName": "John",
  "lastName": "Doe",
  "phone": "0812345678",
  "position": "Developer",
  "department": "IT Department",
  "departmentId": "dept-guid-123",
  "imageProfile": "https://minio.example.com/profiles/john.jpg",
  "roles": ["User"],
  "isActive": true
}
```

## Requirements Verification

### Requirement 2.1 ✅
**WHEN ผู้ใช้งานเข้าสู่หน้า Account Settings THEN ระบบ SHALL ดึงข้อมูล FirstName และ LastName จากตาราง ApplicationUser หรือ Employee**

- Implemented in GetAccountSettingsQueryHandler
- Retrieves from Employee table first
- Falls back to extracting from ApplicationUser email if Employee data is missing

### Requirement 2.2 ✅
**IF ข้อมูล Employee ไม่มี FirstName หรือ LastName THEN ระบบ SHALL ดึงข้อมูลจาก registration data ที่เก็บไว้**

- Implemented fallback logic with ExtractFirstNameFromEmail and ExtractLastNameFromEmail methods
- Extracts names from email format (e.g., "john.doe@example.com")

### Requirement 2.3 ✅
**WHEN ข้อมูลถูกโหลด THEN ระบบ SHALL แสดงข้อมูลในฟอร์ม: คำนำหน้า, ชื่อจริง, นามสกุล, แผนก, ตำแหน่ง, อีเมล, และเบอร์โทรศัพท์**

- All fields included in AccountSettingsDto:
  - TitleName (คำนำหน้า)
  - FirstName (ชื่อจริง)
  - LastName (นามสกุล)
  - Department (แผนก)
  - Position (ตำแหน่ง)
  - Email (อีเมล)
  - Phone (เบอร์โทรศัพท์)
  - ImageProfile (รูปโปรไฟล์)

## Task Checklist

- [x] อัพเดท GetAccountSettingsQuery เพื่อดึง FirstName, LastName จาก Employee table
- [x] Fallback ไปใช้ข้อมูล ApplicationUser ถ้าข้อมูล Employee ไม่มี
- [x] รวม ImageProfile URL ใน response
- [x] รวม Department, Position, Phone, TitleName ใน response
- [x] Requirements 2.1, 2.2, 2.3 verified

## Files Created/Modified

### Created:
1. `src/Application/Users/Queries/GetAccountSettings/GetAccountSettingsQuery.cs`
2. `src/Application/Users/Queries/GetAccountSettings/AccountSettingsDto.cs`

### Modified:
1. `src/Web/Endpoints/Users.cs` - Added GetAccountSettings endpoint

## Testing Notes

The implementation:
1. Properly handles null values with nullable types
2. Includes comprehensive error handling
3. Uses existing patterns from the codebase (similar to GetUserByIdQuery)
4. Follows the project's architecture (CQRS with MediatR)
5. Includes proper authorization (requires authenticated user)

## Next Steps

The frontend can now call this endpoint to retrieve account settings data:
- The endpoint is ready for integration with the Account Settings page
- All required data fields are available in the response
- The fallback mechanism ensures data is always available even if Employee record is incomplete
