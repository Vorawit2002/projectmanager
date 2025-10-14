import { ProjectType, TypeOrganization } from "@/client"

export const ContentWidth = {
  Fluid: 'fluid',
  Boxed: 'boxed',
} as const

export const NavbarType = {
  Sticky: 'sticky',
  Static: 'static',
  Hidden: 'hidden',
} as const

export const FooterType = {
  Sticky: 'sticky',
  Static: 'static',
  Hidden: 'hidden',
} as const

export const AppContentLayoutNav = {
  Vertical: 'vertical',
  Horizontal: 'horizontal',
} as const

export const HorizontalNavType = {
  Sticky: 'sticky',
  Static: 'static',
  Hidden: 'hidden',
} as const

export const TypeOrganizationEnum = [
  { title: 'รัฐบาล', value: TypeOrganization.Government },
  { title: 'เอกชน', value: TypeOrganization.Private },
  { title: 'รัฐวิสาหกิจ', value: TypeOrganization.StateEnterprise },
  { title: 'อื่นๆ', value: TypeOrganization.Other },

]

export const ProjectTypeEnum = [
  { title: 'งานฮาร์ดแวร์', value: ProjectType.Hardware },
  { title: 'งานพัฒนาระบบ', value: ProjectType.Developer },
  { title: 'งานดูแลระบบ', value: ProjectType.Maintenance },
  { title: 'งานอื่นๆ', value: ProjectType.Other },
]
