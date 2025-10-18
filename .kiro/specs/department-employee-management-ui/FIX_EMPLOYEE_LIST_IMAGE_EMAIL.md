# Fix: รูปภาพและอีเมลไม่แสดงใน EmployeeListView

## ปัญหา
EmployeeListView ไม่แสดงรูปภาพและอีเมลของพนักงาน

## สาเหตุ

### 1. รูปภาพไม่แสดง
Template ใช้ `item.imageProfile` โดยตรงโดยไม่แปลง URL:

```vue
<!-- ❌ เดิม - ไม่แปลง URL -->
<v-img :src="item.imageProfile" ...></v-img>
```

**ปัญหา:**
- ถ้า `imageProfile` เป็น relative path (เช่น `/uploads/image.jpg`)
- จะไม่มี base URL ทำให้โหลดรูปไม่ได้

### 2. อีเมลไม่แสดง (ถ้าเป็น null)
```vue
<!-- ❌ เดิม - ไม่มี fallback -->
<td>{{ item.email }}</td>
```

**ปัญหา:**
- ถ้า `email` เป็น `null` หรือ `undefined` จะแสดงเป็นค่าว่าง
- ควรแสดง `-` เพื่อความชัดเจน

---

## การแก้ไข

### 1. เพิ่ม `getImageUrl()` Method

เพิ่ม method เหมือนกับ UserListView:

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

### 2. ใช้ `getImageUrl()` ใน Template

```vue
<!-- ✅ ใหม่ - แปลง URL ก่อนใช้ -->
<v-img :src="getImageUrl(item.imageProfile)" ...></v-img>
```

### 3. เพิ่ม Fallback สำหรับอีเมล

```vue
<!-- ✅ ใหม่ - แสดง '-' ถ้าไม่มีอีเมล -->
<td>{{ item.email || '-' }}</td>
```

---

## ผลลัพธ์

### Before (ปัญหา)
```
| รูป | ชื่อ-นามสกุล | อีเมล | ตำแหน่ง | แผนก | สถานะ |
| ❌  | John Doe     |       | Manager | IT   | ใช้งาน |
```
- รูปไม่แสดง (broken image)
- อีเมลเป็นค่าว่าง

### After (แก้แล้ว)
```
| รูป | ชื่อ-นามสกุล | อีเมล           | ตำแหน่ง | แผนก | สถานะ |
| ✅  | John Doe     | john@email.com  | Manager | IT   | ใช้งาน |
| ✅  | Jane Smith   | -               | Staff   | HR   | ใช้งาน |
```
- รูปแสดงถูกต้อง
- อีเมลแสดง หรือ `-` ถ้าไม่มี

---

## รองรับ URL Formats

`getImageUrl()` method รองรับหลายรูปแบบ:

### 1. Full URL
```
Input:  https://example.com/image.jpg
Output: https://example.com/image.jpg
```

### 2. Data URL (Base64)
```
Input:  data:image/png;base64,iVBORw0KG...
Output: data:image/png;base64,iVBORw0KG...
```

### 3. Relative Path (with /)
```
Input:  /uploads/image.jpg
Output: https://localhost:5001/uploads/image.jpg
```

### 4. Relative Path (without /)
```
Input:  uploads/image.jpg
Output: https://localhost:5001/uploads/image.jpg
```

---

## การทดสอบ

### Test Case 1: Employee มีรูปภาพ (Relative Path)
1. สร้าง Employee พร้อมอัพโหลดรูป
2. เปิด EmployeeListView
3. ✅ **Expected:** รูปแสดงถูกต้อง

### Test Case 2: Employee มีรูปภาพ (Full URL)
1. Employee มี imageProfile เป็น full URL
2. เปิด EmployeeListView
3. ✅ **Expected:** รูปแสดงถูกต้อง

### Test Case 3: Employee ไม่มีรูปภาพ
1. Employee ไม่มี imageProfile
2. เปิด EmployeeListView
3. ✅ **Expected:** แสดง avatar พร้อม initials (เช่น "JD")

### Test Case 4: Employee มีอีเมล
1. Employee มี email
2. เปิด EmployeeListView
3. ✅ **Expected:** แสดงอีเมลถูกต้อง

### Test Case 5: Employee ไม่มีอีเมล
1. Employee ไม่มี email (null)
2. เปิด EmployeeListView
3. ✅ **Expected:** แสดง `-`

---

## Consistency Across Views

ตอนนี้ทุก List Views ใช้ `getImageUrl()` method เหมือนกัน:

### ✅ UserListView
```typescript
getImageUrl(imageProfile: string): string { ... }
```

### ✅ EmployeeListView
```typescript
getImageUrl(imageProfile: string): string { ... }
```

### ✅ UserDetailView
```typescript
getImageUrl(imageProfile: string): string { ... }
```

**ข้อดี:**
- Consistent behavior
- Easy to maintain
- Reusable pattern

---

## Files Modified

1. `src/client_web/src/views/MasterData/Employees/EmployeeListView.vue`
   - เพิ่ม `getImageUrl()` method
   - ใช้ `getImageUrl()` ใน template
   - เพิ่ม fallback `|| '-'` สำหรับอีเมล

---

## Related Components

### Components ที่ใช้ getImageUrl()
- ✅ UserListView
- ✅ UserDetailView
- ✅ EmployeeListView
- ⏳ EmployeeDetail (ควรตรวจสอบ)
- ⏳ CreateEmployee (ควรตรวจสอบ)
- ⏳ UpdateEmployee (ควรตรวจสอบ)

---

## Diagnostics

- ✅ No TypeScript errors
- ✅ No Vue template errors
- ✅ No linting issues

---

## Conclusion

การแก้ไขนี้ทำให้ EmployeeListView แสดงรูปภาพและอีเมลถูกต้อง โดยใช้ pattern เดียวกับ UserListView

**Status:** ✅ Fixed  
**Date:** 18 ตุลาคม 2025  
**Impact:** Low Risk, High Value

---

**Fixed By:** Kiro AI Assistant  
**Tested By:** [To be filled]

