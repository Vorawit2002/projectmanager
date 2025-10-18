# Debug: Employee รูปและอีเมลไม่แสดง

## ปัญหา
EmployeeListView ยังไม่แสดงรูปภาพและอีเมลของพนักงาน แม้ว่าจะเพิ่ม `getImageUrl()` method แล้ว

## การ Debug

### Step 1: ตรวจสอบ Console Log

เปิด Browser Developer Tools (F12) และดูที่ Console tab

**คาดว่าจะเห็น:**
```
Employee data from backend: [...]
First employee: { id: "...", email: "...", imageProfile: "..." }
Email: john@example.com
ImageProfile: /uploads/image.jpg
```

**ถ้าเห็น:**
```
Email: undefined
ImageProfile: undefined
```
→ **ปัญหา:** Backend ไม่ส่งข้อมูลมา

**ถ้าเห็น:**
```
Email: null
ImageProfile: null
```
→ **ปัญหา:** ข้อมูลในฐานข้อมูลเป็น null

---

### Step 2: ตรวจสอบ Network Tab

1. เปิด Developer Tools (F12)
2. ไปที่ tab "Network"
3. Refresh หน้า EmployeeListView
4. หา request ที่เรียก `getEmployeeWithPagination`
5. คลิกดู Response

**ตรวจสอบ Response:**
```json
{
  "items": [
    {
      "id": "...",
      "userId": "...",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john@example.com",  // ← ตรวจสอบว่ามีหรือไม่
      "imageProfile": "/uploads/...", // ← ตรวจสอบว่ามีหรือไม่
      "position": "Manager",
      "departmentId": "...",
      "isActive": true
    }
  ],
  "totalCount": 10
}
```

**ถ้า email และ imageProfile เป็น null:**
→ ข้อมูลในฐานข้อมูลไม่มี

---

### Step 3: ตรวจสอบข้อมูลในฐานข้อมูล

#### Option A: ใช้ SQL Query (ถ้ามี psql)
```sql
SELECT 
    "Id",
    "UserId",
    "FirstName",
    "LastName",
    "Email",
    "ImageProfile",
    "Position"
FROM "Employees"
LIMIT 5;
```

#### Option B: ใช้ Backend Endpoint
สร้าง Employee ใหม่ผ่าน UI และตรวจสอบว่า:
1. กรอก Email
2. อัพโหลดรูปภาพ
3. บันทึก
4. Refresh หน้า EmployeeListView

---

## สาเหตุที่เป็นไปได้

### 1. ข้อมูลในฐานข้อมูลไม่มี Email/ImageProfile

**วิธีแก้:**
- สร้าง Employee ใหม่พร้อม Email และรูปภาพ
- หรือ Update Employee ที่มีอยู่

### 2. AutoMapper ไม่ Map ข้อมูล

**ตรวจสอบ:**
```csharp
// EmployeeDto.cs
private class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<Employee, EmployeeDto>(); // ✅ ถูกต้อง
    }
}
```

**ถ้าไม่ถูกต้อง:** แก้ไข mapping

### 3. Frontend ไม่ได้ดึงข้อมูลมา

**ตรวจสอบ:**
```typescript
// EmployeeListView.vue
const response = await client.getEmployeeWithPagination(this.request)
this.data = response.items || []
console.log('Employee data:', this.data) // ← ดู console
```

### 4. TypeScript Client ไม่ได้ Generate

**วิธีแก้:**
```bash
cd src/Web
dotnet build
```

NSwag จะ generate client.ts ใหม่

---

## วิธีแก้ไขแบบชั่วคราว

### ถ้าข้อมูลในฐานข้อมูลไม่มี Email

แก้ไข template ให้ดึงจาก ApplicationUser:

```vue
<!-- Option: ดึง email จาก User -->
<td>{{ item.email || getUserEmail(item.userId) || '-' }}</td>
```

เพิ่ม method:
```typescript
async getUserEmail(userId: string): Promise<string> {
  try {
    const user = await client.getUserById(userId)
    return user.email || '-'
  } catch {
    return '-'
  }
}
```

**แต่วิธีนี้ไม่แนะนำ** เพราะจะทำให้ช้า (เรียก API หลายครั้ง)

---

## วิธีแก้ไขที่ถูกต้อง

### 1. Sync ข้อมูล Email จาก ApplicationUser

สร้าง migration script เพื่อ sync email:

```sql
UPDATE "Employees" e
SET "Email" = u."Email"
FROM "AspNetUsers" u
WHERE e."UserId" = u."Id"
AND (e."Email" IS NULL OR e."Email" = '');
```

### 2. แก้ไข GetEmployeeWithPagination Query

ถ้าต้องการดึง email จาก ApplicationUser:

```csharp
public async Task<PaginatedList<EmployeeDto>> Handle(...)
{
    var query = _context.Employees
        .Include(x => x.Departments)
        .Include(x => x.User) // ← เพิ่มนี้
        .AsQueryable();
    
    // Apply filters...
    
    var employees = await query
        .OrderBy(x => x.FirstName)
        .ToListAsync();
    
    var dtos = employees.Select(e => new EmployeeDto
    {
        Id = e.Id,
        UserId = e.UserId,
        FirstName = e.FirstName,
        LastName = e.LastName,
        Email = e.Email ?? e.User?.Email ?? string.Empty, // ← Priority
        ImageProfile = e.User?.ImageProfile ?? e.ImageProfile, // ← Priority
        Position = e.Position,
        DepartmentId = e.DepartmentId,
        Departments = e.Departments,
        isActive = e.isActive,
        // ... other fields
    }).ToList();
    
    return new PaginatedList<EmployeeDto>(
        dtos.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList(),
        dtos.Count,
        request.PageNumber,
        request.PageSize
    );
}
```

---

## ขั้นตอนการแก้ไข (แนะนำ)

### 1. ตรวจสอบข้อมูล
```bash
# เปิด browser console และดู log
# ตรวจสอบว่า email และ imageProfile มีค่าหรือไม่
```

### 2. ถ้าข้อมูลเป็น null
```bash
# Option A: สร้าง Employee ใหม่พร้อมข้อมูลครบ
# Option B: Sync ข้อมูลจาก ApplicationUser (ใช้ SQL script ด้านบน)
```

### 3. ถ้า Backend ไม่ส่งข้อมูล
```bash
# Rebuild backend
cd src/Web
dotnet build

# Restart backend
dotnet run
```

### 4. ถ้า Frontend ไม่แสดง
```bash
# Clear browser cache
# Hard refresh (Ctrl+Shift+R)
```

---

## Test Cases

### Test 1: สร้าง Employee ใหม่
1. ไปที่ EmployeeListView
2. คลิก "เพิ่มพนักงาน"
3. กรอกข้อมูล:
   - ✅ UserId (เลือก User)
   - ✅ FirstName
   - ✅ LastName
   - ✅ **Email** ← สำคัญ!
   - ✅ **อัพโหลดรูป** ← สำคัญ!
   - ✅ Position
   - ✅ Department
4. บันทึก
5. ตรวจสอบว่าแสดงรูปและอีเมล

### Test 2: Update Employee ที่มีอยู่
1. เลือก Employee ที่ไม่มีรูป/อีเมล
2. คลิก "แก้ไข"
3. เพิ่ม Email และอัพโหลดรูป
4. บันทึก
5. ตรวจสอบว่าแสดงรูปและอีเมล

---

## Expected Results

### ถ้าทำถูกต้อง:

**Console Log:**
```
Employee data from backend: [...]
First employee: { 
  id: "abc-123", 
  email: "john@example.com", 
  imageProfile: "/uploads/profile.jpg" 
}
Email: john@example.com
ImageProfile: /uploads/profile.jpg
```

**UI:**
```
| รูป | ชื่อ-นามสกุล | อีเมล           | ตำแหน่ง | แผนก | สถานะ |
| ✅  | John Doe     | john@email.com  | Manager | IT   | ใช้งาน |
```

---

## Next Steps

1. ✅ เพิ่ม console.log (เสร็จแล้ว)
2. ⏳ ตรวจสอบ console และ network tab
3. ⏳ ระบุปัญหาที่แท้จริง
4. ⏳ แก้ไขตามสาเหตุ

---

**Status:** 🔍 Debugging  
**Date:** 18 ตุลาคม 2025

