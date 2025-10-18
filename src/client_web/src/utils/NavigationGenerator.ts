import router from '@/router'
import {
  GeneratedAllMenu,
  GeneratedExcludeMenu,
  GeneratedIncludeMenu,
  type GroupedNavMenu,
} from './NavigationServices'
// admin/dev
export function AdministratorMenu() {
  let selectionMenu = new SelectionMenu()
  selectionMenu.Menu = selectionMenu.Menu = [
    { menuName: 'ดิจิทัลไฟล์', menuOrder: 1 },
    { menuName: 'ข้อมูลเอกสารจดหมายเหตุ(ตัวอย่าง)', menuOrder: 1 },
    { menuName: 'ข้อมูลจดหมายราชการ', menuOrder: 2 },
    { menuName: 'บทความ', menuOrder: 3 },
    { menuName: 'รายชื่อสมาชิก', menuOrder: 1 },
    { menuName: 'รายชื่อเจ้าหน้าที่', menuOrder: 1 },
    { menuName: 'ตำแหน่งเจ้าหน้าที่', menuOrder: 2 },
    { menuName: 'ข้อมูลหน่วยงาน', menuOrder: 3 },
    { menuName: 'ข้อมูลคำนำหน้าชื่อ', menuOrder: 4 },
    { menuName: 'ข้อมูลกลุ่มงาน/ฝ่าย', menuOrder: 5 },
    { menuName: 'ข้อมูลสัญชาติ', menuOrder: 6 },
    { menuName: 'ข้อมูลเชื้อชาติ', menuOrder: 7 },
    // { menuName: 'ข้อมูลศาสนา', menuOrder: 8 },
    { menuName: 'ข้อมูลระดับการศึกษา', menuOrder: 9 },
    { menuName: 'ข้อมูลอาชีพ', menuOrder: 10 },
    { menuName: 'ประเภทการขอทำสำเนา', menuOrder: 11 },
    { menuName: 'ประเภทสำเนา', menuOrder: 12 },
    { menuName: 'ตั้งค่าราคาการขอทำสำเนา', menuOrder: 13 },
    { menuName: 'วัตถุประสงค์การขอทำสำเนา', menuOrder: 14 },
    { menuName: 'เหตุผลการไม่อนุมัติการขอทำสำเนา', menuOrder: 14 },
    { menuName: 'ตั้งค่า SMTP', menuOrder: 2 },
    { menuName: 'ตั้งค่าข้อความ E-mail', menuOrder: 3 },
    { menuName: 'รายการ Caching', menuOrder: 1 },
  ]

  selectionMenu.Group = selectionMenu.Group = [
    { groupName: 'ข้อมูลเจ้าหน้าที่', groupOrder: 1 },
    { groupName: 'ข้อมูลพื้นฐาน', groupOrder: 2 },
    { groupName: 'เกี่ยวกับสมาชิก', groupOrder: 2 },
    { groupName: 'ข้อมูลเอกสารจดหมายเหตุ', groupOrder: 1 },
  ]

    selectionMenu.Title = selectionMenu.Title = [
      { titleName: 'ระบบจัดเก็บเอกสารจดหมายเหตุ', titleOrder: 1 },
      { titleName: 'เมนูเกี่ยวกับผู้ดูแลระบบ', titleOrder: 2 },
    ]
  return GeneratedIncludeMenu(selectionMenu) as unknown as GroupedNavMenu
}
// เจ้าหน้าที่
export function NATEmployeeMenu() {
  let selectionMenu = new SelectionMenu()
  selectionMenu.Menu = selectionMenu.Menu = [
    { menuName: 'ปฏิทิน', menuOrder: 1 },
    { menuName: 'ข้อมูลเอกสารจดหมายเหตุ(ตัวอย่าง)', menuOrder: 2 },
    { menuName: 'ข้อมูลจดหมายราชการ', menuOrder: 3 },
    { menuName: 'บทความ', menuOrder: 4 },
    { menuName: 'รายชื่อสมาชิก', menuOrder: 1 },
    { menuName: 'ที่ต้องพิจารณาอนุมัติ', menuOrder: 1 },
    { menuName: 'รายการที่พิจารณาแล้ว', menuOrder: 2 },
    { menuName: 'ชำระเงินแล้ว', menuOrder: 3 },
    { menuName: 'รายการที่ไม่ผ่านการอนุมัติ', menuOrder: 4 },
    { menuName: 'ลงนามใบเสร็จรับเงิน', menuOrder: 5 },
    { menuName: 'รายการที่ลงนามใบเสร็จรับเงินแล้ว', menuOrder: 6 },
    { menuName: 'รายชื่อเจ้าหน้าที่', menuOrder: 1 },
    { menuName: 'ตำแหน่งเจ้าหน้าที่', menuOrder: 2 },
    { menuName: 'ข้อมูลหน่วยงาน', menuOrder: 3 },
    { menuName: 'ข้อมูลคำนำหน้าชื่อ', menuOrder: 4 },
    { menuName: 'ข้อมูลกลุ่มงาน/ฝ่าย', menuOrder: 5 },
    { menuName: 'ข้อมูลสัญชาติ', menuOrder: 6 },
    { menuName: 'ข้อมูลเชื้อชาติ', menuOrder: 7 },
    // { menuName: 'ข้อมูลศาสนา', menuOrder: 8 },
    { menuName: 'ข้อมูลระดับการศึกษา', menuOrder: 9 },
    { menuName: 'ข้อมูลอาชีพ', menuOrder: 10 },
    { menuName: 'ประเภทการขอทำสำเนา', menuOrder: 11 },
    { menuName: 'ประเภทสำเนา', menuOrder: 12 },
    { menuName: 'ตั้งค่าราคาการขอทำสำเนา', menuOrder: 13 },
    { menuName: 'วัตถุประสงค์การขอทำสำเนา', menuOrder: 14 },
    { menuName: 'เหตุผลการไม่อนุมัติการขอทำสำเนา', menuOrder: 15 },
    { menuName: 'ตั้งค่า SMTP', menuOrder: 16 },
    { menuName: 'ตั้งค่าข้อความ E-mail', menuOrder: 17 },
  ]

  selectionMenu.Group = selectionMenu.Group = [
    { groupName: 'ข้อมูลเจ้าหน้าที่', groupOrder: 2 },
    { groupName: 'ข้อมูลพื้นฐาน', groupOrder: 2 },
    { groupName: 'เกี่ยวกับสมาชิก', groupOrder: 2 },
    { groupName: 'รายการขอทำสำเนา', groupOrder: 1 },
    { groupName: 'ข้อมูลเอกสารจดหมายเหตุ', groupOrder: 1 },
  ]

  selectionMenu.Title = selectionMenu.Title = [
    { titleName: 'ระบบจัดเก็บเอกสารจดหมายเหตุ', titleOrder: 1 },
    { titleName: 'เมนูเกี่ยวกับผู้ดูแลระบบ', titleOrder: 2 },
  ]

  return GeneratedIncludeMenu(selectionMenu) as unknown as GroupedNavMenu
}
// ผู้ใช้งาน / สมาชิก
export function NATMemberMenu() {
    let selectionMenu = new SelectionMenu()
  selectionMenu.Menu = selectionMenu.Menu = [
    { menuName: 'ปฏิทิน', menuOrder: 1 },
    { menuName: 'ข้อมูลเอกสารจดหมายเหตุ(ตัวอย่าง)', menuOrder: 2 },
    { menuName: 'ข้อมูลจดหมายราชการ', menuOrder: 3 },
    { menuName: 'บทความ', menuOrder: 4 },
  ]

  selectionMenu.Group = selectionMenu.Group = [
    { groupName: 'ข้อมูลเอกสารจดหมายเหตุ', groupOrder: 1 },
  ]

  selectionMenu.Title = selectionMenu.Title = [
    { titleName: 'ระบบจัดเก็บเอกสารจดหมายเหตุ', titleOrder: 1 },
  ]
  return GeneratedIncludeMenu(selectionMenu) as unknown as GroupedNavMenu
}
export function NoneMenu() {
  let selectionMenu = new SelectionMenu()
  return GeneratedIncludeMenu(selectionMenu)
}

export class menuItem {
  menuName = '' as string
  menuOrder = 0 as number
}

export class groupItem {
  groupName = '' as string
  groupOrder = 0 as number
}

export class titleItem {
  titleName = '' as string
  titleOrder = 0 as number
}

export class SelectionMenu {
  Menu = [] as menuItem[]
  Group = [] as groupItem[]
  Title = [] as titleItem[]
}
