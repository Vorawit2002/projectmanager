# วิธีแก้ปัญหา ERR_HTTP2_PROTOCOL_ERROR

## สาเหตุ
การโหลด navigation properties ทั้งหมด (Employees, Projects, Organizations, EventTypes) ทำให้ response มีขนาดใหญ่เกินไป ทำให้เกิด HTTP/2 protocol error

## การแก้ไข
แก้ไขโดยการ **project เฉพาะ fields ที่จำเป็น** แทนการโหลดทั้ง object:

### ก่อนแก้ไข:
```csharp
Employees = activity.Employees,  // โหลดทุก field รวมถึง navigation properties ซ้อนกัน
```

### หลังแก้ไข:
```csharp
Employees = activity.Employees != null ? new EmployeeDto
{
    Id = activity.Employees.Id,
    FirstName = activity.Employees.FirstName,
    LastName = activity.Employees.LastName,
    TitleName = activity.Employees.TitleName,
    ImageProfile = activity.Employees.ImageProfile,
    DepartmentId = activity.Employees.DepartmentId
} : null,
```

## ขั้นตอนการทดสอบ

### 1. Restart Backend Server
```bash
# หยุด server ปัจจุบัน (Ctrl+C)
# จากนั้นรันใหม่
cd src/Web
dotnet run
```

### 2. Clear Browser Cache
- กด F12 เปิด DevTools
- คลิกขวาที่ปุ่ม Refresh
- เลือก "Empty Cache and Hard Reload"

### 3. ทดสอบ
1. เข้าหน้า Customer Appointment Plan List
2. ตรวจสอบว่าข้อมูลโหลดได้โดยไม่มี error
3. ลองออกจากหน้าแล้วเข้ามาใหม่ - ควรโหลดได้ปกติ

## ผลลัพธ์ที่คาดหวัง
- ✅ ไม่มี `ERR_HTTP2_PROTOCOL_ERROR` 
- ✅ Response size ลดลงมาก (จาก MB เหลือ KB)
- ✅ โหลดเร็วขึ้น
- ✅ ข้อมูลแสดงครบถ้วนตามที่ต้องการ

## หมายเหตุ
การแก้ไขนี้ใช้ได้กับทุก query ที่มีปัญหาคล้ายกัน - ให้ระบุ fields ที่ต้องการชัดเจนแทนการโหลดทั้ง navigation property
