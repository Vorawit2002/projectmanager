# Database Schema Reference (Data Dictionary)

เอกสารพจนานุกรมข้อมูล (Data Dictionary) สำหรับระบบ **Project Management** ครอบคลุมทุกตารางในฐานข้อมูล (Domain, Identity, Hangfire)

> **หมายเหตุ (Common Fields):**
> ทุกตารางในกลุ่ม Domain Entity จะมีคอลัมน์มาตรฐาน:
> *   `Created` (Timestamp), `CreatedBy` (String)
> *   `LastModified` (Timestamp), `LastModifiedBy` (String)
> *   `GCRecord` (Boolean): เก็บสถานะการลบข้อมูล (Soft Delete)

---

## 1. กลุ่มบุคลากร (People & Identity)

### 1.1 ข้อมูลพนักงาน (Employees)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสพนักงาน | UUID | PK, ห้ามซ้ำ | PK |
| 2 | UserId | รหัสผู้ใช้ระบบ | Varchar(450) | FK(AspNetUsers), UK | FK, UK |
| 3 | DepartmentId | รหัสแผนก | UUID | FK(Departments) | FK |
| 4 | TitleName | คำนำหน้า | Varchar | - | - |
| 5 | FirstName | ชื่อจริง | Varchar(100) | - | - |
| 6 | LastName | นามสกุล | Varchar(100) | - | - |
| 7 | Email | อีเมล | Varchar(256) | ห้ามว่าง | - |
| 8 | Position | ตำแหน่ง | Varchar(100) | - | - |
| 9 | Phone | เบอร์โทร | Varchar(20) | - | - |
| 10 | ImageProfile | รูปโปรไฟล์ | Text | Base64 | - |
| 11 | isActive | สถานะใช้งาน | Boolean | - | - |
| 12 | Subscription | สมัครสมาชิก | Boolean | - | - |
| 13 | Roles | บทบาท | Varchar | - | - |
| 14 | Group | กลุ่ม | Varchar | - | - |

### 1.2 ผู้ใช้งานระบบ (AspNetUsers)
ตารางหลักสำหรับเก็บข้อมูล Login (Identity System)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสผู้ใช้ | Varchar(450) | PK | PK |
| 2 | UserName | ชื่อเข้าระบบ | Varchar(256) | UK | UK |
| 3 | NormalizedUserName| ชื่อสำหรับค้นหา | Varchar(256) | - | - |
| 4 | Email | อีเมล | Varchar(256) | - | - |
| 5 | EmailConfirmed | ยืนยันอีเมล | Boolean | False | - |
| 6 | PasswordHash | รหัสผ่าน (Hash)| Varchar | - | - |
| 7 | PhoneNumber | เบอร์โทร | Varchar | - | - |
| 8 | FirstName | ชื่อจริง | Varchar | (Custom) | - |
| 9 | LastName | นามสกุล | Varchar | (Custom) | - |
| 10 | IsRevoked | ระงับใช้ | Boolean | (Custom) | - |
| 11 | RevokeStart | วันเริ่มระงับ | DateTime | (Custom) | - |
| 12 | RevokeEnd | วันสิ้นสุดระงับ | DateTime | (Custom) | - |
| 13 | RequirePasswordChange | ต้องเปลี่ยนรหัส | Boolean | (Custom) | - |
| 14 | LastPasswordChangeDate | วันที่เปลี่ยนรหัสล่าสุด | DateTime | (Custom) | - |
| 15 | ImageProfile | รูปโปรไฟล์ | Varchar | (Custom) | - |

### 1.3 บทบาท (AspNetRoles)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสบทบาท | Varchar(450) | PK | PK |
| 2 | Name | ชื่อบทบาท | Varchar(256) | UK | UK |
| 3 | NormalizedName | ชื่อสำหรับค้นหา | Varchar(256) | - | - |

### 1.4 การจับคู่ผู้ใช้-บทบาท (AspNetUserRoles)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | UserId | รหัสผู้ใช้ | Varchar(450) | PK, FK | PK, FK |
| 2 | RoleId | รหัสบทบาท | Varchar(450) | PK, FK | PK, FK |

### 1.5 ข้อมูลการเข้าสู่ระบบ (AspNetUserLogins)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | LoginProvider | ผู้ให้บริการ | Varchar(128) | PK | PK |
| 2 | ProviderKey | คีย์จากผู้ให้บริการ| Varchar(128) | PK | PK |
| 3 | UserId | รหัสผู้ใช้ | Varchar(450) | FK | FK |

### 1.6 เคลมของผู้ใช้ (AspNetUserClaims)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | Int | Auto | PK |
| 2 | UserId | รหัสผู้ใช้ | Varchar(450) | FK | FK |
| 3 | ClaimType | ประเภทเคลม | Varchar | - | - |
| 4 | ClaimValue | ค่าของเคลม | Varchar | - | - |

### 1.7 โทเค็นของผู้ใช้ (AspNetUserTokens)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | UserId | รหัสผู้ใช้ | Varchar(450) | PK, FK | PK, FK |
| 2 | LoginProvider | ผู้ให้บริการ | Varchar(128) | PK | PK |
| 3 | Name | ชื่อโทเค็น | Varchar(128) | PK | PK |
| 4 | Value | ค่าโทเค็น | Varchar | - | - |

### 1.8 เคลมของบทบาท (AspNetRoleClaims)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | Int | Auto | PK |
| 2 | RoleId | รหัสบทบาท | Varchar(450) | FK | FK |
| 3 | ClaimType | ประเภทเคลม | Varchar | - | - |
| 4 | ClaimValue | ค่าของเคลม | Varchar | - | - |

---

## 2. กลุ่มองค์กร (Organization)

### 2.1 แผนก (Departments)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | Name | ชื่อแผนก | Varchar | ห้ามว่าง | - |
| 3 | IsActive | สถานะใช้งาน | Boolean | True | - |

### 2.2 หน่วยงาน/ลูกค้า (Organizations)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | Name | ชื่อหน่วยงาน | Varchar | ห้ามว่าง | - |
| 3 | ShortName | ชื่อย่อ | Varchar | - | - |
| 4 | TypeOrganization| ประเภท | Int/Enum | - | - |
| 5 | Address | ที่อยู่ | Varchar | - | - |
| 6 | Coordinates | พิกัด | Varchar | - | - |
| 7 | WebSite | เว็บไซต์ | Varchar | - | - |
| 8 | Phone | เบอร์โทร | Varchar | - | - |
| 9 | Fax | แฟกซ์ | Varchar | - | - |

### 2.3 ผู้ติดต่อหน่วยงาน (OrganizationContacts)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | OrganizationId | รหัสหน่วยงาน | UUID | FK | FK |
| 3 | TitleName | คำนำหน้า | Varchar | ห้ามว่าง | - |
| 4 | FirstName | ชื่อ | Varchar | ห้ามว่าง | - |
| 5 | LastName | นามสกุล | Varchar | - | - |
| 6 | Position | ตำแหน่ง | Varchar | - | - |
| 7 | Email | อีเมล | Varchar | - | - |
| 8 | Phone | เบอร์โทร | Varchar | - | - |
| 9 | Fax | แฟกซ์ | Varchar | - | - |
| 10 | LineId | ไลน์ไอดี | Varchar | - | - |
| 11 | AttachmentId | รหัสรูปโปรไฟล์ | UUID | FK | FK |

---

## 3. กลุ่มโครงการ (Project)

### 3.1 โครงการ (Projects)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | ProjectCode | รหัสโปรเจค | Varchar | ห้ามว่าง | - |
| 3 | ProjectName | ชื่อโปรเจค | Varchar | ห้ามว่าง | - |
| 4 | ShortName | ชื่อย่อ | Varchar | - | - |
| 5 | ContractNumber | เลขที่สัญญา | Varchar | - | - |
| 6 | ContractSignedDate | วันที่ลงนามสัญญา | DateTime | - | - |
| 7 | StartDate | วันเริ่มงาน | DateTime | - | - |
| 8 | EndDate | วันจบงาน | DateTime | - | - |
| 9 | WarrantyEndDate | วันสิ้นสุดรับประกัน | DateTime | - | - |
| 10 | OrganizationId | รหัสลูกค้า | UUID | FK | FK |
| 11 | ProjectCost | งบประมาณโครงการ | Decimal | - | - |
| 12 | ProjectType | ประเภทโครงการ | Int/Enum | - | - |

### 3.2 ผู้ติดต่อโครงการ (ProjectContacts)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | ProjectId | รหัสโปรเจค | UUID | FK | FK |
| 3 | OrganizationContactId| รหัสผู้ติดต่อ | UUID | FK | FK |

---

## 4. กลุ่มกิจกรรม (Activity)

### 4.1 แผนงาน/กิจกรรม (ActivityPlans)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | EmployeeId | รหัสพนักงาน | UUID | FK | FK |
| 3 | Objective | วัตถุประสงค์ | Varchar | - | - |
| 4 | ObjectiveDetail | รายละเอียดวัตถุประสงค์ | Varchar | - | - |
| 5 | detail | รายละเอียด | Varchar | - | - |
| 6 | ProjectId | รหัสโปรเจค | UUID | FK | FK |
| 7 | OrganizationId | รหัสองค์กร | UUID | FK | FK |
| 8 | AllDay | ทั้งวัน | Boolean | - | - |
| 9 | StartDate | เริ่มต้น | DateTime | ห้ามว่าง | - |
| 10 | EndDate | สิ้นสุด | DateTime | ห้ามว่าง | - |
| 11 | Location | สถานที่ | Varchar | - | - |
| 12 | HaveCost | มีค่าใช้จ่าย | Boolean | - | - |
| 13 | CostDetail | รายละเอียดค่าใช้จ่าย | Varchar | - | - |
| 14 | Cost | ค่าใช้จ่าย | Decimal | - | - |
| 15 | OutSide | นอกสถานที่ | Boolean | - | - |
| 16 | EventTypeId | ประเภท | UUID | FK | FK |

### 4.2 บันทึกงาน (PlanNotes)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | ActivityPlanId | รหัสกิจกรรม | UUID | FK | FK |
| 3 | Summary | สรุปงาน | Varchar | ห้ามว่าง | - |
| 4 | ToDoNext | สิ่งที่ต้องทำต่อไป | Varchar | - | - |
| 5 | Remarks | หมายเหตุ | Varchar | - | - |

### 4.3 ประเภทกิจกรรม (EventTypes)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | Name | ชื่อประเภท | Varchar | ห้ามว่าง | - |
| 3 | EventTypeCode | รหัสประเภทกิจกรรม | Varchar | - | - |

### 4.4 ผู้ติดต่องาน (ActivityPlanContacts)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | ActivityPlanId | รหัสกิจกรรม | UUID | FK | FK |
| 3 | OrganizationContactId | รหัสผู้ติดต่อ | UUID | FK | FK |

### 4.5 ไฟล์แนบงาน (ActivityPlanAttachments)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | ActivityPlanId | รหัสกิจกรรม | UUID | FK | FK |
| 3 | AttachmentId | รหัสไฟล์ | UUID | FK | FK |

---

## 5. กลุ่มลงเวลา (Time Tracking)

### 5.1 การลงเวลา (CheckInCheckOuts)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | EmployeeId | รหัสพนักงาน | UUID | FK | FK |
| 3 | Location | สถานที่เช็คอิน | Varchar | - | - |
| 4 | LocationCheckOut | สถานที่เช็คเอาท์ | Varchar | - | - |
| 5 | Lat | ละติจูด | Varchar | - | - |
| 6 | Long | ลองจิจูด | Varchar | - | - |
| 7 | IPAddress | ไอพีแอดเดรส | Varchar | - | - |
| 8 | CheckIn | เวลาเข้า | DateTime | ห้ามว่าง | - |
| 9 | CheckOut | เวลาออก | DateTime | - | - |
| 10 | OrganizationId | รหัสองค์กร | UUID | FK | FK |
| 11 | ProjectId | รหัสโครงการ | UUID | FK | FK |
| 12 | CheckInCheckOutTypes | ประเภทเช็คอิน | Int/Enum | - | - |
| 13 | Types | ประเภท | Varchar | - | - |

### 5.2 ไฟล์แนบลงเวลา (CheckInCheckOutAttachments)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | CheckInCheckOutId| รหัสลงเวลา | UUID | FK | FK |
| 3 | Type | ประเภท | Int/Enum | - | - |
| 4 | AttachmentId | รหัสไฟล์ | UUID | FK | FK |

---

## 6. ระบบเมลและแจ้งเตือน (Notification)

### 6.1 ตั้งค่าเมล (EmailMessageSettings)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | SettingCode | รหัสตั้งค่า | Varchar | ห้ามว่าง | - |
| 3 | SettingName | ชื่อตั้งค่า | Varchar | ห้ามว่าง | - |
| 4 | MailSubject | หัวข้อเมล | Varchar | ห้ามว่าง | - |
| 5 | MailBody | เนื้อหาเมล | Varchar | ห้ามว่าง | - |
| 6 | IsActive | สถานะใช้งาน | Boolean | ห้ามว่าง | - |
| 7 | Remark | หมายเหตุ | Varchar | - | - |
| 8 | Sendtime | เวลาส่ง | Varchar | - | - |

### 6.2 รอบส่งเมล (EmailScheduleSettings)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | ScheduleName | ชื่อตารางเวลา | Varchar | - | - |
| 3 | HangfireJobId | รหัสงาน Hangfire | Varchar | - | - |
| 4 | EmailMessageSettingId | รหัสข้อความ | UUID | FK | FK |
| 5 | SendMailDate | วันที่ส่ง | DateTime | - | - |
| 6 | IsEnabled | เปิดใช้งาน | Boolean | - | - |

### 6.3 ประวัติเมล (EmailLogs)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | Subject | หัวข้อ | Varchar | ห้ามว่าง | - |
| 3 | SentTo | ส่งถึง | Varchar | ห้ามว่าง | - |
| 4 | Massage | ข้อความ | Varchar | ห้ามว่าง | - |
| 5 | SendBy | ส่งโดย | Varchar | ห้ามว่าง | - |
| 6 | SendType | ประเภทการส่ง | Varchar | - | - |
| 7 | RefEntityId | รหัสอ้างอิง | Varchar | - | - |
| 8 | RefEntityClass | คลาสอ้างอิง | Varchar | - | - |
| 9 | SendDate | วันที่ส่ง | DateTime | - | - |
| 10 | SendStatus | สถานะการส่ง | Boolean | - | - |

### 6.4 ตั้งค่า SMTP (SMTPSettings)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | ConfigName | ชื่อการตั้งค่า | Varchar | ห้ามว่าง | - |
| 3 | SMTPServer | เซิร์ฟเวอร์ | Varchar | ห้ามว่าง | - |
| 4 | SMTPPort | พอร์ต | Varchar | ห้ามว่าง | - |
| 5 | SMTPAuthentication | ประเภทการยืนยัน | Int/Enum | - | - |
| 6 | SMTPUserName | ชื่อผู้ใช้ | Varchar | ห้ามว่าง | - |
| 7 | SMTPPassword | รหัสผ่าน | Varchar | ห้ามว่าง | - |
| 8 | SMTPEnableSSL | เปิดใช้ SSL | Boolean | ห้ามว่าง | - |
| 9 | IsActive | สถานะใช้งาน | Boolean | - | - |
| 10 | Remark | หมายเหตุ | Varchar | - | - |

### 6.5 แจ้งเตือน (PushSubscriptions)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | EmployeeId | รหัสพนักงาน | UUID | FK | FK |
| 3 | Endpoint | ปลายทาง | Varchar | ห้ามว่าง | - |
| 4 | P256dh | คีย์ P256dh | Varchar | - | - |
| 5 | Auth | คีย์ Auth | Varchar | - | - |
| 6 | IsActive | สถานะใช้งาน | Boolean | - | - |

### 6.6 ข้อมูลไฟล์ (Attachments)
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสรายการ | UUID | PK | PK |
| 2 | NameFile | ชื่อไฟล์ | Varchar | ห้ามว่าง | - |
| 3 | PathFile | พาธ | Varchar | ห้ามว่าง | - |
| 4 | FileSize | ขนาดไฟล์ | Long | ห้ามว่าง | - |
| 5 | FileExtension | นามสกุลไฟล์ | Varchar | ห้ามว่าง | - |
| 6 | BucketOriginalName | ชื่อต้นฉบับ | Varchar | ห้ามว่าง | - |
| 7 | BucketOriginalPath | พาธต้นฉบับ | Varchar | ห้ามว่าง | - |

---

## 7. ระบบตารางงานอัตโนมัติ (Hangfire)
เป็นตารางระบบภายในที่สร้างโดยอัตโนมัติ (Internal System Tables)

### 7.1 Hangfire.Job
เก็บข้อมูลงานที่ต้องประมวลผล
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสงาน | BigInt | Auto | PK |
| 2 | StateId | สถานะปัจจุบัน | BigInt | FK | FK |
| 3 | StateName | ชื่อสถานะ | Varchar | - | - |
| 4 | InvocationData | ข้อมูลการเรียก | Json | - | - |
| 5 | Arguments | พารามิเตอร์ | Json | - | - |
| 6 | CreatedAt | วันที่สร้าง | Timestamp | - | - |

### 7.2 Hangfire.State
เก็บประวัติสถานะของงาน
| ลำดับ | คุณสมบัติ | คำอธิบาย | ประเภท | ตรวจสอบ | คีย์ |
| :---: | :--- | :--- | :---: | :--- | :---: |
| 1 | Id | รหัสสถานะ | BigInt | Auto | PK |
| 2 | JobId | รหัสงาน | BigInt | FK | FK |
| 3 | Name | ชื่อสถานะ | Varchar | - | - |
| 4 | Reason | เหตุผล | Varchar | - | - |
| 5 | CreatedAt | วันที่สร้าง | Timestamp | - | - |

### 7.3 ตารางอื่นๆ ของ Hangfire (Internal)
*   **Hangfire.Server**: เก็บข้อมูล Server Node ที่ทำงานอยู่
*   **Hangfire.Set**: เก็บข้อมูล Key-Value Set สำหรับงาน
*   **Hangfire.Counter**: ตารางนับจำนวนงาน
*   **Hangfire.Hash**: เก็บข้อมูล Hash
*   **Hangfire.List**: เก็บรายการคิวงาน
*   **Hangfire.AggregatedCounter**: ตารางสรุปยอดนับ
*   **Hangfire.Schema**: เก็บข้อมูลเวอร์ชันของ Schema

---

## สรุปจำนวนตารางทั้งหมด: 36 ตาราง

| กลุ่ม | จำนวน | รายชื่อตาราง |
|-------|-------|--------------|
| **Domain Entities** | 20 ตาราง | Employees, Departments, Organizations, OrganizationContacts, Projects, ProjectContacts, ActivityPlans, ActivityPlanContacts, ActivityPlanAttachments, EventTypes, PlanNotes, CheckInCheckOuts, CheckInCheckOutAttachments, Attachments, SMTPSettings, EmailMessageSettings, EmailScheduleSettings, EmailLogs, PushSubscriptions, (ApplicationUser รวมใน Identity) |
| **Identity System** | 7 ตาราง | AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserTokens, AspNetRoleClaims |
| **Hangfire System** | 9 ตาราง | Job, State, Server, Set, Counter, Hash, List, AggregatedCounter, Schema |
