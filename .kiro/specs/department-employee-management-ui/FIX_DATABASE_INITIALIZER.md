# Fix: ปิดการลบ Users ทุกครั้งที่รัน Backend

## ปัญหา
ทุกครั้งที่รัน backend (dotnet build หรือ dotnet run) ระบบจะลบ users ทั้งหมดและสร้างแค่ admin user ใหม่

### Build Log (เดิม)
```
info: ProjectManagement.Infrastructure.Data.ApplicationDbContextInitialiser[0]
      Deleted 1 existing users
info: ProjectManagement.Infrastructure.Data.ApplicationDbContextInitialiser[0]
      Created admin user: admin
```

**ผลกระทบ:**
- Users ที่สร้างไว้หายไปทุกครั้ง
- ต้องสร้าง users ใหม่ทุกครั้งที่รัน backend
- ข้อมูล Employee ที่เชื่อมกับ User หายไป

---

## สาเหตุ

**File:** `src/Infrastructure/Data/ApplicationDbContextInitialiser.cs`

```csharp
public async Task TrySeedAsync()
{
    // ❌ โค้ดนี้ลบ users ทั้งหมดทุกครั้ง
    var existingUsers = await _userManager.Users.ToListAsync();
    foreach (var user in existingUsers)
    {
        await _userManager.DeleteAsync(user);
    }
    _logger.LogInformation("Deleted {Count} existing users", existingUsers.Count);
    
    // ... สร้าง admin user ใหม่
}
```

**เหตุผล:**
- โค้ดนี้เขียนไว้สำหรับ development/testing
- ใช้เพื่อ reset database ให้มีแค่ admin user
- แต่ไม่เหมาะสำหรับการใช้งานจริง

---

## การแก้ไข

### ลบโค้ดที่ลบ Users ออก

**Before:**
```csharp
public async Task TrySeedAsync()
{
    // Clear all existing users first
    var existingUsers = await _userManager.Users.ToListAsync();
    foreach (var user in existingUsers)
    {
        await _userManager.DeleteAsync(user);
    }
    _logger.LogInformation("Deleted {Count} existing users", existingUsers.Count);

    // Default roles
    // ...
}
```

**After:**
```csharp
public async Task TrySeedAsync()
{
    // Default roles
    // ...
}
```

### ผลลัพธ์

ตอนนี้ระบบจะ:
- ✅ **ไม่ลบ** users ที่มีอยู่
- ✅ สร้าง roles ถ้ายังไม่มี
- ✅ สร้าง admin user **เฉพาะถ้ายังไม่มี**
- ✅ Import master data (Organizations, EventTypes, Projects)

---

## Build Log (ใหม่)

```
18
Import Organization successfully.
2
Import EventTypes successfully.
3
1
✅ Import ProjectsList67-68 CSV completed.
info: Hangfire.PostgreSql.PostgreSqlStorage[0]
      Start installing Hangfire SQL objects...
info: Hangfire.PostgreSql.PostgreSqlStorage[0]
      Hangfire SQL objects installed.
Done.
```

**สังเกต:** ไม่มีข้อความ "Deleted X existing users" แล้ว! 🎉

---

## การทดสอบ

### Test Case 1: Users ไม่หายหลังรัน Backend
1. สร้าง user ใหม่ผ่าน UI
2. Restart backend (dotnet run)
3. ✅ **Expected:** User ยังอยู่ ไม่หายไป

### Test Case 2: Admin User ถูกสร้างถ้ายังไม่มี
1. ลบ admin user ออกจาก database
2. Restart backend
3. ✅ **Expected:** Admin user ถูกสร้างใหม่

### Test Case 3: Roles ถูกสร้างถ้ายังไม่มี
1. ลบ roles ออกจาก database
2. Restart backend
3. ✅ **Expected:** Roles (Admin, Manager, User, Viewer) ถูกสร้างใหม่

---

## ข้อควรระวัง

### ⚠️ Admin User
โค้ดยังคงสร้าง admin user ถ้ายังไม่มี:
```csharp
if (_userManager.Users.All(u => u.UserName != administrator.UserName))
{
    await _userManager.CreateAsync(administrator, "Admin123!");
    await _userManager.AddToRolesAsync(administrator, new [] { adminRole.Name });
    _logger.LogInformation("Created admin user: {Username}", administrator.UserName);
}
```

**Credentials:**
- Username: `admin`
- Password: `Admin123!`
- Email: `admin@projectmanagement.com`
- Role: `Admin`

### 💡 Best Practice

ถ้าต้องการ reset database ในอนาคต:
1. ใช้ migration: `dotnet ef database drop`
2. หรือลบ database ด้วย SQL client
3. จากนั้น run backend ใหม่

**อย่า** uncomment โค้ดลบ users กลับมา!

---

## Files Modified

1. `src/Infrastructure/Data/ApplicationDbContextInitialiser.cs`
   - ลบโค้ดที่ลบ users ทั้งหมด (บรรทัด 85-91)

---

## Impact Assessment

### ✅ Positive Impacts
- Users ไม่หายหลังรัน backend
- ข้อมูล Employee ที่เชื่อมกับ User ไม่หาย
- ไม่ต้องสร้าง users ใหม่ทุกครั้ง
- เหมาะสำหรับการใช้งานจริง

### ⚠️ Considerations
- ถ้ามี users เก่าที่ไม่ต้องการ ต้องลบด้วยตัวเอง
- Admin user จะถูกสร้างเฉพาะครั้งแรกเท่านั้น

### 🚫 No Breaking Changes
- ไม่กระทบ users ที่มีอยู่
- ไม่กระทบ functionality อื่นๆ
- Backward compatible

---

## Verification

### ✅ Build Status
- Build succeeded
- No errors
- No warnings

### 🧪 Manual Testing Required
1. [ ] สร้าง user ใหม่
2. [ ] Restart backend
3. [ ] ตรวจสอบว่า user ยังอยู่
4. [ ] ตรวจสอบว่า Employee data ยังอยู่

---

## Conclusion

การแก้ไขนี้ทำให้ระบบเหมาะสำหรับการใช้งานจริงมากขึ้น โดย users จะไม่หายหลังจากรัน backend แล้ว

**Status:** ✅ Fixed  
**Date:** 18 ตุลาคม 2025  
**Build:** Success  
**Impact:** Low Risk, High Value

---

**Fixed By:** Kiro AI Assistant  
**Tested By:** [To be filled]  
**Approved By:** [To be filled]

