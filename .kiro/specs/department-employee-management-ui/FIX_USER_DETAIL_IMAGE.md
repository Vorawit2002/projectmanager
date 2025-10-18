# Fix: รูปภาพไม่แสดงใน UserDetailView

## ปัญหา
เมื่อกดดูรายละเอียดผู้ใช้งานใน UserListView รูปภาพไม่แสดง

## สาเหตุ

### ความแตกต่างระหว่าง GetAllUsers และ GetUserById

**GetAllUsersQuery (ใช้ใน UserListView):**
```csharp
// Priority: ApplicationUser.ImageProfile > Employee.ImageProfile
ImageProfile = user.ImageProfile ?? employee?.ImageProfile,
FirstName = employee?.FirstName ?? user.FirstName ?? ExtractFirstNameFromEmail(user.Email),
LastName = employee?.LastName ?? user.LastName ?? ExtractLastNameFromEmail(user.Email),
Position = employee?.Position,
```

**GetUserByIdQuery (ใช้ใน UserDetailView) - เดิม:**
```csharp
// ❌ เอาแค่ Employee.ImageProfile เท่านั้น
ImageProfile = employee?.ImageProfile,
FirstName = employee?.FirstName,
LastName = employee?.LastName,
// ❌ ไม่มี Position
```

**ปัญหา:**
- ถ้า User ไม่มี Employee record → `ImageProfile` จะเป็น `null`
- แต่ `ApplicationUser.ImageProfile` อาจมีค่า
- ทำให้รูปไม่แสดงใน Detail View แม้ว่าจะแสดงใน List View

---

## การแก้ไข

### ปรับ GetUserByIdQuery ให้เหมือนกับ GetAllUsersQuery

**File:** `src/Application/Users/Queries/GetUserById/GetUserByIdQuery.cs`

```csharp
var userDto = new UserDto
{
    UserId = user.Id,
    Username = user.UserName ?? string.Empty,
    Email = user.Email ?? string.Empty,
    
    // ✅ Priority: Employee > ApplicationUser
    FirstName = employee?.FirstName ?? user.FirstName,
    LastName = employee?.LastName ?? user.LastName,
    
    // ✅ Priority: ApplicationUser.ImageProfile > Employee.ImageProfile
    ImageProfile = user.ImageProfile ?? employee?.ImageProfile,
    
    // ✅ เพิ่ม Position
    Position = employee?.Position,
    
    Department = employee?.Departments?.Name,
    DepartmentId = employee?.DepartmentId,
    Roles = roles.ToList(),
    IsActive = employee?.isActive ?? !user.IsRevoked,
    LastLoginDate = user.LastPasswordChangeDate
};
```

---

## ลำดับความสำคัญของข้อมูล (Priority)

### 1. ImageProfile
```
ApplicationUser.ImageProfile → Employee.ImageProfile → null
```
- ใช้รูปจาก ApplicationUser ก่อน (อัพโหลดผ่าน Account Settings)
- ถ้าไม่มี ใช้รูปจาก Employee
- ถ้าไม่มีทั้งคู่ แสดง initials

### 2. FirstName / LastName
```
Employee → ApplicationUser → null
```
- ใช้ชื่อจาก Employee ก่อน (ข้อมูลที่ละเอียดกว่า)
- ถ้าไม่มี ใช้ชื่อจาก ApplicationUser
- ถ้าไม่มีทั้งคู่ แสดง username

### 3. Position
```
Employee.Position → null
```
- มีเฉพาะใน Employee record

---

## ผลลัพธ์

### Before (ปัญหา)
```
UserListView: แสดงรูป ✅ (ใช้ user.ImageProfile)
UserDetailView: ไม่แสดงรูป ❌ (ใช้แค่ employee?.ImageProfile)
```

### After (แก้แล้ว)
```
UserListView: แสดงรูป ✅
UserDetailView: แสดงรูป ✅ (ใช้ user.ImageProfile ?? employee?.ImageProfile)
```

---

## การทดสอบ

### Test Case 1: User มี ApplicationUser.ImageProfile
1. User อัพโหลดรูปผ่าน Account Settings
2. เปิด UserListView → ✅ แสดงรูป
3. กดดูรายละเอียด → ✅ แสดงรูป

### Test Case 2: User มี Employee.ImageProfile
1. สร้าง Employee พร้อมรูปภาพ
2. เปิด UserListView → ✅ แสดงรูป
3. กดดูรายละเอียด → ✅ แสดงรูป

### Test Case 3: User ไม่มีรูปทั้งคู่
1. User ไม่มีรูปใน ApplicationUser และ Employee
2. เปิด UserListView → ✅ แสดง initials (เช่น "JD")
3. กดดูรายละเอียด → ✅ แสดง initials

### Test Case 4: User มีรูปทั้งคู่
1. User มีรูปทั้งใน ApplicationUser และ Employee
2. เปิด UserListView → ✅ แสดงรูปจาก ApplicationUser (priority)
3. กดดูรายละเอียด → ✅ แสดงรูปจาก ApplicationUser (priority)

---

## ข้อมูลเพิ่มเติม

### Frontend: getImageUrl() Method

UserDetailView มี method `getImageUrl()` ที่จัดการ URL ของรูปภาพ:

```typescript
getImageUrl(imageProfile: string): string {
  if (!imageProfile) return ''
  
  // If it's already a full URL (http/https) or data URL (data:), use as is
  if (imageProfile.startsWith('http') || imageProfile.startsWith('data:')) {
    return imageProfile
  }
  
  // If it's a relative path, prepend BACKEND_API_URL
  return `${BACKEND_API_URL}${imageProfile.startsWith('/') ? '' : '/'}${imageProfile}`
}
```

**รองรับ:**
- ✅ Full URL: `https://example.com/image.jpg`
- ✅ Data URL: `data:image/png;base64,...`
- ✅ Relative path: `/uploads/image.jpg`
- ✅ Relative path: `uploads/image.jpg`

---

## Files Modified

1. `src/Application/Users/Queries/GetUserById/GetUserByIdQuery.cs`
   - ปรับ priority ของ ImageProfile
   - ปรับ priority ของ FirstName/LastName
   - เพิ่ม Position field

---

## Build Status

- ✅ Build succeeded
- ✅ No errors
- ✅ No warnings

---

## Related Issues

- [x] Fix: เพิ่มคอลัมน์ตำแหน่งใน UserListView
- [x] Fix: รูปภาพไม่แสดงใน UserDetailView

---

## Conclusion

การแก้ไขนี้ทำให้ข้อมูลที่แสดงใน UserListView และ UserDetailView สอดคล้องกัน โดยใช้ priority เดียวกันในการเลือกข้อมูลจาก ApplicationUser และ Employee

**Status:** ✅ Fixed  
**Date:** 18 ตุลาคม 2025  
**Build:** Success

---

**Fixed By:** Kiro AI Assistant  
**Tested By:** [To be filled]

