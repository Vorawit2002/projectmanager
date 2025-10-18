# Fix: เพิ่มคอลัมน์ตำแหน่งใน UserListView

## ปัญหา
UserListView ไม่แสดงข้อมูลตำแหน่ง (Position) ของพนักงาน

## การแก้ไข

### 1. เพิ่ม Position field ใน UserDto
**File:** `src/Application/Users/Queries/GetAllUsers/UserDto.cs`

```csharp
public class UserDto
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageProfile { get; set; }
    public string? Position { get; set; }  // ✅ เพิ่มใหม่
    public string? Department { get; set; }
    public Guid? DepartmentId { get; set; }
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
}
```

### 2. ดึงข้อมูล Position จาก Employee
**File:** `src/Application/Users/Queries/GetAllUsers/GetAllUsersQuery.cs`

```csharp
var userDto = new UserDto
{
    UserId = user.Id,
    Username = user.UserName ?? string.Empty,
    Email = user.Email ?? string.Empty,
    FirstName = employee?.FirstName ?? user.FirstName ?? ExtractFirstNameFromEmail(user.Email),
    LastName = employee?.LastName ?? user.LastName ?? ExtractLastNameFromEmail(user.Email),
    ImageProfile = user.ImageProfile ?? employee?.ImageProfile,
    Position = employee?.Position,  // ✅ เพิ่มใหม่
    Department = employee?.Departments?.Name,
    DepartmentId = employee?.DepartmentId,
    Roles = roles.ToList(),
    IsActive = employee?.isActive ?? !user.IsRevoked,
    LastLoginDate = user.LastPasswordChangeDate
};
```

### 3. เพิ่มคอลัมน์ใน UserListView
**File:** `src/client_web/src/views/MasterData/Users/UserListView.vue`

```typescript
headers: [
  { title: 'รูปโปรไฟล์', value: 'imageProfile', align: 'center', sortable: false },
  { title: 'ชื่อ-นามสกุล', value: 'fullName' },
  { title: 'อีเมล', value: 'email' },
  { title: 'ตำแหน่ง', value: 'position' },  // ✅ เพิ่มใหม่
  { title: 'แผนก', value: 'department' },
  { title: 'Role', value: 'roles', sortable: false },
  { title: 'สถานะ', value: 'isActive', align: 'center' },
  { title: 'จัดการ', value: 'actions', align: 'center', sortable: false },
]
```

## ผลลัพธ์

### Before
```
| รูปโปรไฟล์ | ชื่อ-นามสกุล | อีเมล | แผนก | Role | สถานะ | จัดการ |
```

### After
```
| รูปโปรไฟล์ | ชื่อ-นามสกุล | อีเมล | ตำแหน่ง | แผนก | Role | สถานะ | จัดการ |
```

## การทดสอบ

### ✅ Build Status
- Build succeeded
- No diagnostics errors
- No warnings

### 🧪 Manual Testing
1. เปิด UserListView
2. ตรวจสอบว่ามีคอลัมน์ "ตำแหน่ง" แสดงขึ้นมา
3. ตรวจสอบว่าข้อมูลตำแหน่งแสดงถูกต้อง (ถ้ามี Employee record)
4. ตรวจสอบว่าถ้าไม่มี Employee record จะแสดงเป็นค่าว่าง

## หมายเหตุ

- ข้อมูล Position จะแสดงเฉพาะ User ที่มี Employee record
- ถ้า User ไม่มี Employee record คอลัมน์ตำแหน่งจะว่างเปล่า
- ข้อมูลดึงมาจาก `Employees.Position`

## Files Modified

1. `src/Application/Users/Queries/GetAllUsers/UserDto.cs`
2. `src/Application/Users/Queries/GetAllUsers/GetAllUsersQuery.cs`
3. `src/client_web/src/views/MasterData/Users/UserListView.vue`

---

**Status:** ✅ Fixed  
**Date:** 18 ตุลาคม 2025  
**Build:** Success
