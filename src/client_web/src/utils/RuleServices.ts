// เบอร์มือถือ
export const phoneRules = [
  (v: any) => !!v || 'กรุณาระบุเบอร์โทรศัพท์',
  // (v: string) => {
  //   const digits = v.replace(/-/g, '');
  //   return (digits.length >= 9 && digits.length <= 12) || 'กรุณากรอกเบอร์โทรศัพท์ให้ถูกต้อง';
  // },
]

//ระบุหน่วยงาน
export const agencyRules = [
  (v: string) => !!v || 'กรุณาระบุชื่อหน่วยงาน',
]
//ระบุชื่อย่อหน่วยงาน
export const agencyShortNameRules = [
  (v: string) => !!v || 'กรุณาระบุชื่อย่อหน่วยงาน',
]
//เลือกหน่วยงาน
export const selectAgencyRules = [
  (v: string | null | undefined) => v !== null && v !== undefined || 'กรุณาเลือกหน่วยงาน',
]

//ประเภทหน่วยงาน
export const agencyTypeRules = [
  (v: number | null | undefined) => v !== null && v !== undefined || 'กรุณาระบุประเภทหน่วยงาน'
]

//เลือกประเภทโครงการ 
export const projectTypeRules = [
  (v: number | null | undefined) => v !== null && v !== undefined || 'กรุณาเลือกประเภทโครงการ',
]

//ตำแหน่งงาน
export const positionRules = [
  (v: string) => !!v || 'กรุณาระบุตำแหน่งงาน',
]

//ที่อยู่
export const addressRules = [
  (v: string) => !!v || 'กรุณาระบุที่อยู่ของหน่วยงาน',
]

//พิกัด
export const coordinateRules = [
  (v: string) => !!v || 'กรุณาระบุพิกัดของหน่วยงาน',
]

//คำนำหน้า
export const prefixRules = [
  (v: string) => !!v || 'กรุณาระบุคำนำหน้าชื่อ',
]

//ชื่อประเภทกิจกรรม
export const eventTypeNameRules = [
  (v: string) => !!v || 'กรุณาระบุชื่อประเภทกิจกรรม',
]

//ชื่อ
export const nameRules = [
  (v: string) => !!v || 'กรุณาระบุชื่อ',
]

//นามสกุล
export const surnameRules = [
  (v: string) => !!v || 'กรุณาระบุนามสกุล',
]

//เว็บไซต์
export const websiteRules = [
  (v: string) => !!v || 'กรุณาระบุเว็บไซต์ของหน่วยงาน',
  (v: string) => /^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([\/\w .-]*)*\/?$/.test(v) || 'กรุณากรอก URL เว็บไซต์ที่ถูกต้อง',
]

//แฟกซ์
export const faxRules = [
  (v: any) => !!v || 'กรุณาระบุหมายเลขแฟกซ์',
  // (v: any) => /^[0-9]{9,10}$/.test(v) || 'กรุณากรอกหมายเลขแฟกซ์ 9-10 หลัก',
]

//อีเมล
export const emailRules = [
  (v: string) => !!v || 'กรุณาระบุอีเมล',
  (v: string) => /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(v) || 'กรุณากรอกอีเมลที่ถูกต้อง',
]

//Line ID
export const lineIdRules = [
  (v: string) => !!v || 'กรุณาระบุ Line ID',
]

//รหัสโครงการ
export const projectCodeRules = [
  (v: string) => !!v || 'กรุณาระบุรหัสโครงการ',
]

//ชื่อโครงการ
export const projectNameRules = [
  (v: string) => !!v || 'กรุณาระบุชื่อโครงการ',
]
//ชื่อย่อโครงการ
export const projectShortNameRules = [
  (v: string) => !!v || 'กรุณาระบุชื่อย่อโครงการ',
]
//เลขที่สัญญา
export const contractNumberRules = [
  (v: string) => !!v || 'กรุณาระบุเลขที่สัญญา',
]

//วันที่เริ่มต้นโครงการ
export const projectStartDateRules = [
  (v: string) => !!v || 'กรุณาระบุวันที่เริ่มต้นโครงการ',
]

//วันที่สิ้นสุดโครงการ
export const projectEndDateRules = [
  (v: string) => !!v || 'กรุณาระบุวันที่สิ้นสุดโครงการ',
]

//วันที่ลงนามสัญญา
export const contractDateRules = [
  (v: string) => !!v || 'กรุณาระบุวันที่ลงนามสัญญา',
]

//วันที่สิ้นสุดประกัน
export const insuranceEndDateRules = [
  (v: string) => !!v || 'กรุณาระบุวันที่สิ้นสุดประกัน',
]

//มูลค่าโครงการ
export const projectValueRules = [
  (v: any) => !!v || 'กรุณาระบุมูลค่าโครงการ',
]

//เลือกโครงการ
export const selectProjectRules = [
  (v: string | null | undefined) => v !== null && v !== undefined || 'กรุณาเลือกโครงการ',
]

//เลือกผู้ติดต่องาน
export const selectContactRules = [
  (v: string | null | undefined) => v !== null && v !== undefined || 'กรุณาเลือกผู้ติดต่องาน',
]

//วันที่เริ่มต้นนัดหมาย
export const appointmentStartDateRules = [
  (v: string) => !!v || 'กรุณาระบุวันที่เริ่มต้นนัดหมาย',
]

//วันที่สิ้นสุดนัดหมาย
export const appointmentEndDateRules = [
  (v: string) => !!v || 'กรุณาระบุวันที่สิ้นสุดนัดหมาย',
]

//เลือกหน่วยงานหรือลูกค้า
export const selectAgencyOrCustomerRules = [
  (v: string | null | undefined) => v !== null && v !== undefined || 'กรุณาเลือกหน่วยงานหรือลูกค้า',
]

//เลือกลูกค้าที่นัดพบ
export const selectCustomerRules = [
  (v: unknown) =>
    Array.isArray(v) && v.length > 0
      ? true
      : 'กรุณาเลือกลูกค้าที่นัดพบ',
]

//สถานที่
export const appointmentLocationRules = [
  (v: string) => !!v || 'กรุณาระบุสถานที่',
]

//วัตถุประสงค์ 
export const appointmentPurposeRules = [
  (v: string) => !!v || 'กรุณาระบุวัตถุประสงค์',
]

//ค่าใช้จ่าย
export const appointmentCostRules = [
  (v: any) => v !== null && v !== undefined && v !== '' || 'กรุณาระบุค่าใช้จ่าย',
]

//ระบุวัตถุประสงค์อื่น ๆ
export const otherPurposeRules = [
  (v: string) => !!v || 'กรุณาระบุวัตถุประสงค์อื่น ๆ',
  (v: string) => (v && v.length >= 10) || 'ระบุตัวอักษรอย่างน้อย 10 ตัวอักษร',
]

// costDetailRules
export const CostDetailRules = [
  (v: string) => !!v || 'กรุณาระบุรายละเอียดค่าใช้จ่าย',
  (v: string) => (v && v.length >= 10) || 'ระบุตัวอักษรอย่างน้อย 10 ตัวอักษร',
]

//สรุปผลการนัดพบ
export const SummaryRules = [
  (v: string) => !!v || 'กรุณาระบุรายละเอียดสรุปผลการนัดพบ',
  (v: string) => (v && v.length >= 10) || 'ระบุตัวอักษรอย่างน้อย 10 ตัวอักษร',
]


//ระบุจำนวนเงิน
export const amountRules = [
  (v: string | number) => !!v || 'กรุณาระบุจำนวนเงิน',
  (v: string | number) => {
    const value = typeof v === 'number' ? v.toString() : v;
    return /^[\d,]+$/.test(value) || 'กรุณากรอกเฉพาะตัวเลข';
  },
  (v: string | number) => {
    const value = typeof v === 'number' ? v.toString() : (v || '').toString();
    const num = parseFloat(value.replace(/,/g, ''));
    return num > 0 || 'จำนวนเงินต้องมากกว่า 0';
  },
]

//สรุปผลการนัดพบ
export const appointmentSummaryRules = [
  (v: string) => !!v || 'ระบุรายละเอียดสรุปผลการนัดพบ',
  (v: string) => v.length >= 10 || 'กรุณากรอกรายละเอียดอย่างน้อย 10 ตัวอักษร'
]

//สิ่งที่ต้องดำเนินการ
export const actionItemsRules = [
  (v: string) => !!v || 'กรุณาระบุสิ่งที่ต้องดำเนินการ',
  (v: string) => v.length >= 10 || 'กรุณากรอกรายละเอียดอย่างน้อย 10 ตัวอักษร'
]
