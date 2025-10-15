# Requirements Document

## Introduction

โปรเจกต์นี้ต้องการเปลี่ยนระบบ authentication จากการใช้ OpenID Connect กับ ntiportal เป็นระบบ local authentication ที่รันได้ 100% บน local โดยไม่ต้องพึ่งพา external service ใดๆ พร้อมทั้งเพิ่มหน้า login/register ใหม่ และย้าย MinIO จากการอัพโหลดไปยัง external server มาเป็น Docker container ที่รันใน local environment

## Requirements

### Requirement 1: Local Authentication System

**User Story:** ในฐานะผู้ใช้งาน ฉันต้องการสามารถ login และ register ด้วยระบบ local authentication เพื่อที่จะไม่ต้องพึ่งพา ntiportal ในการเข้าสู่ระบบ

#### Acceptance Criteria

1. WHEN ผู้ใช้เข้าถึงหน้า login THEN ระบบ SHALL แสดงฟอร์ม login ที่มีช่อง username/email และ password
2. WHEN ผู้ใช้กรอก username/email และ password ที่ถูกต้อง THEN ระบบ SHALL ทำการ authenticate และสร้าง JWT token สำหรับ session
3. WHEN ผู้ใช้กรอก username/email หรือ password ที่ไม่ถูกต้อง THEN ระบบ SHALL แสดง error message ที่ชัดเจน
4. WHEN ผู้ใช้ login สำเร็จ THEN ระบบ SHALL redirect ไปยังหน้าหลักของแอปพลิเคชัน
5. WHEN ผู้ใช้ logout THEN ระบบ SHALL ลบ authentication token และ redirect กลับไปหน้า login
6. IF ผู้ใช้ยังไม่ได้ authenticate THEN ระบบ SHALL redirect ไปยังหน้า login เมื่อพยายามเข้าถึง protected routes

### Requirement 2: User Registration System

**User Story:** ในฐานะผู้ใช้ใหม่ ฉันต้องการสามารถสร้างบัญชีผู้ใช้ใหม่ได้เอง เพื่อที่จะสามารถเข้าใช้งานระบบได้

#### Acceptance Criteria

1. WHEN ผู้ใช้เข้าถึงหน้า register THEN ระบบ SHALL แสดงฟอร์มลงทะเบียนที่มีช่อง username, email, password, และ confirm password
2. WHEN ผู้ใช้กรอกข้อมูลครบถ้วนและถูกต้อง THEN ระบบ SHALL สร้างบัญชีผู้ใช้ใหม่ในฐานข้อมูล
3. WHEN ผู้ใช้กรอก email ที่มีอยู่ในระบบแล้ว THEN ระบบ SHALL แสดง error message ว่า email นี้ถูกใช้งานแล้ว
4. WHEN ผู้ใช้กรอก password ที่ไม่ตรงกับ confirm password THEN ระบบ SHALL แสดง error message
5. WHEN ผู้ใช้กรอก password ที่ไม่ตามเงื่อนไข (ความยาวน้อยกว่า 6 ตัวอักษร) THEN ระบบ SHALL แสดง error message
6. WHEN ผู้ใช้ลงทะเบียนสำเร็จ THEN ระบบ SHALL redirect ไปยังหน้า login พร้อมแสดงข้อความยืนยัน

### Requirement 3: Remove ntiportal Dependencies

**User Story:** ในฐานะ developer ฉันต้องการลบ dependencies ทั้งหมดที่เกี่ยวข้องกับ ntiportal เพื่อให้ระบบสามารถรันได้แบบ standalone

#### Acceptance Criteria

1. WHEN ระบบเริ่มทำงาน THEN ระบบ SHALL ไม่มีการเรียกใช้ OpenID Connect configuration ที่ชี้ไปยัง ntiportal
2. WHEN ผู้ใช้ login THEN ระบบ SHALL ไม่มีการ redirect ไปยัง ntiportal authorize endpoint
3. WHEN ผู้ใช้ logout THEN ระบบ SHALL ไม่มีการ redirect ไปยัง ntiportal manage page
4. WHEN ระบบสร้าง share link THEN ระบบ SHALL ไม่มีการสร้าง URL ที่ชี้ไปยัง ntiportal authorize endpoint
5. WHEN ระบบตรวจสอบ Line user ID THEN ระบบ SHALL ไม่มีการเรียก ntiportal API
6. IF มีการอ้างอิงถึง ntiportal ในโค้ด THEN ระบบ SHALL ลบหรือแทนที่ด้วย local implementation

### Requirement 4: JWT Token Authentication

**User Story:** ในฐานะระบบ ฉันต้องการใช้ JWT token สำหรับการ authenticate API requests เพื่อความปลอดภัยและความสะดวกในการจัดการ session

#### Acceptance Criteria

1. WHEN ผู้ใช้ login สำเร็จ THEN ระบบ SHALL สร้าง JWT token ที่มี claims ของผู้ใช้ (user id, username, email, roles)
2. WHEN ผู้ใช้เรียกใช้ protected API THEN ระบบ SHALL ตรวจสอบ JWT token ใน Authorization header
3. WHEN JWT token หมดอายุ THEN ระบบ SHALL ส่ง 401 Unauthorized response
4. WHEN JWT token ไม่ถูกต้อง THEN ระบบ SHALL ส่ง 401 Unauthorized response
5. IF JWT token ถูกต้อง THEN ระบบ SHALL อนุญาตให้เข้าถึง protected resources
6. WHEN สร้าง JWT token THEN ระบบ SHALL กำหนด expiration time (เช่น 24 ชั่วโมง)

### Requirement 5: Frontend Login/Register Pages

**User Story:** ในฐานะผู้ใช้ ฉันต้องการหน้า UI ที่สวยงามและใช้งานง่ายสำหรับ login และ register

#### Acceptance Criteria

1. WHEN ผู้ใช้เข้าถึงหน้า login THEN ระบบ SHALL แสดงฟอร์ม login ที่มี design สอดคล้องกับระบบเดิม
2. WHEN ผู้ใช้คลิกลิงก์ "สมัครสมาชิก" THEN ระบบ SHALL นำไปยังหน้า register
3. WHEN ผู้ใช้อยู่ที่หน้า register และคลิก "เข้าสู่ระบบ" THEN ระบบ SHALL นำกลับไปยังหน้า login
4. WHEN มี error เกิดขึ้น THEN ระบบ SHALL แสดง error message ที่ชัดเจนและเป็นภาษาไทย
5. WHEN ผู้ใช้กำลัง submit form THEN ระบบ SHALL แสดง loading indicator
6. IF ผู้ใช้ login สำเร็จ THEN ระบบ SHALL เก็บ JWT token ใน localStorage หรือ cookie

### Requirement 6: Local MinIO Docker Container

**User Story:** ในฐานะ developer ฉันต้องการให้ MinIO รันใน Docker container แทนการเชื่อมต่อไปยัง external server เพื่อให้ระบบรันได้แบบ standalone

#### Acceptance Criteria

1. WHEN ระบบเริ่มทำงานด้วย docker-compose THEN ระบบ SHALL เริ่ม MinIO container พร้อมกับ services อื่นๆ
2. WHEN MinIO container เริ่มทำงาน THEN ระบบ SHALL สร้าง default access key และ secret key
3. WHEN แอปพลิเคชันต้องการอัพโหลดไฟล์ THEN ระบบ SHALL เชื่อมต่อไปยัง MinIO container ใน Docker network
4. WHEN MinIO container restart THEN ระบบ SHALL คงข้อมูลไฟล์ที่อัพโหลดไว้ผ่าน Docker volume
5. IF ต้องการเข้าถึง MinIO console THEN ระบบ SHALL expose port สำหรับ web UI
6. WHEN อัพเดต MinIO configuration THEN ระบบ SHALL อ่านค่าจาก environment variables ใน docker-compose

### Requirement 7: Update Configuration Files

**User Story:** ในฐานะ developer ฉันต้องการอัพเดต configuration files ทั้งหมดให้ชี้ไปยัง local services

#### Acceptance Criteria

1. WHEN ระบบอ่าน appsettings.json THEN ระบบ SHALL ไม่มี OpenIDConnectSettings ที่ชี้ไปยัง ntiportal
2. WHEN ระบบอ่าน docker-compose.yml THEN ระบบ SHALL มี MinIO service configuration
3. WHEN ระบบอ่าน environment variables THEN ระบบ SHALL ใช้ MinIO URL ที่ชี้ไปยัง local container
4. WHEN frontend อ่าน constants THEN ระบบ SHALL ไม่มี PortalOpenId URL
5. IF มี configuration ที่อ้างอิงถึง ntiportal THEN ระบบ SHALL ลบหรือแทนที่ด้วย local values
6. WHEN ระบบเริ่มทำงาน THEN ระบบ SHALL ใช้ JWT authentication แทน OpenID Connect

### Requirement 8: Remove Line Bot Integration

**User Story:** ในฐานะ developer ฉันต้องการลบ Line Bot integration ออกจากระบบทั้งหมด เพื่อให้ระบบเรียบง่ายและไม่ต้องพึ่งพา external services

#### Acceptance Criteria

1. WHEN ระบบเริ่มทำงาน THEN ระบบ SHALL ไม่มี Line Bot related endpoints
2. WHEN ตรวจสอบโค้ด THEN ระบบ SHALL ไม่มีการเรียก ntiportal API สำหรับ Line user ID mapping
3. IF มี check-in/check-out commands ที่เกี่ยวข้องกับ Line THEN ระบบ SHALL ลบ commands เหล่านั้น
4. WHEN ตรวจสอบฐานข้อมูล THEN ระบบ SHALL ไม่จำเป็นต้องมี Line user ID fields (optional: อาจเก็บไว้ถ้ามีข้อมูลเดิม)
5. IF มี dependencies ที่เกี่ยวข้องกับ Line Bot THEN ระบบ SHALL ลบ dependencies เหล่านั้น
6. WHEN ระบบรัน THEN ระบบ SHALL ไม่มี error จากการลบ Line Bot features

### Requirement 9: Migration and Backward Compatibility

**User Story:** ในฐานะ developer ฉันต้องการให้มีการ migrate ข้อมูลผู้ใช้เดิมและรองรับการทำงานแบบเดิมได้ในระหว่างการ transition

#### Acceptance Criteria

1. WHEN รัน migration script THEN ระบบ SHALL สร้าง default password สำหรับผู้ใช้เดิมที่มีอยู่
2. WHEN ผู้ใช้เดิม login ครั้งแรก THEN ระบบ SHALL แจ้งให้เปลี่ยน password
3. IF มีข้อมูล employee ที่ยังไม่มี user account THEN ระบบ SHALL สามารถสร้าง user account ได้
4. WHEN migrate ข้อมูล THEN ระบบ SHALL คง roles และ permissions เดิมไว้
5. IF มีข้อมูลที่เกี่ยวข้องกับ OpenID THEN ระบบ SHALL จัดการหรือลบข้อมูลเหล่านั้น
6. WHEN migration เสร็จสิ้น THEN ระบบ SHALL สามารถรันได้โดยไม่ต้องพึ่งพา ntiportal

### Requirement 10: Security and Password Management

**User Story:** ในฐานะระบบ ฉันต้องการจัดการ password อย่างปลอดภัยและมีฟีเจอร์สำหรับการเปลี่ยน password

#### Acceptance Criteria

1. WHEN ผู้ใช้สร้าง password THEN ระบบ SHALL hash password ด้วย bcrypt หรือ PBKDF2
2. WHEN ผู้ใช้ login THEN ระบบ SHALL เปรียบเทียบ hashed password
3. WHEN ผู้ใช้ต้องการเปลี่ยน password THEN ระบบ SHALL มี API endpoint สำหรับการเปลี่ยน password
4. WHEN เปลี่ยน password THEN ระบบ SHALL ตรวจสอบ old password ก่อน
5. IF password ไม่ตรงตามเงื่อนไข THEN ระบบ SHALL แสดง validation error
6. WHEN เก็บ password THEN ระบบ SHALL ไม่เก็บ plain text password ในฐานข้อมูล
