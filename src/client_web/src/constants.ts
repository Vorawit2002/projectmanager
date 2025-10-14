import moment from 'moment'

//local
export const BACKEND_API_URL = 'https://localhost:5001'
// export const BACKEND_API_URL = 'http://localhost:8800'
// export const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API 
//nti
// export const BACKEND_API_URL = 'https://crm.nti.co.th' // nti

//open id connect
export const ClientId = 'OPNNricj2qWgVQqo3x4JjHpaoDy6Z0'
export const ClientSecret = 'T4Q3ENQorJKnC7IALb6d49dmA7sgWMZf'

// localhost
export const RedirectUris = 'http://localhost:5173/login-callback'
export const Logout = 'http://localhost:5173/?logout=true'

// NTi
// export const RedirectUris = 'https://crm.nti.co.th/login-callback'
// export const Logout = 'https://crm.nti.co.th/?logout=true'

export const MediafileUrl = `https://media.nti.co.th/crmfile/`
export const PortalOpenId = 'https://ntiportal.nti.co.th/connect'
export const scope = 'openid profile email roles phone profile_image'
export const code_challenge = 'gEKX6x8KW3Pxfna9viyf6ZHhTZlleNA15rxji0jWlvM'
export const code_verifier = 'Z6K7_cL80limbPAXmaS7zeNWvXU5y2eMS4hc0LQcfxo'

export function formattdate(date: string | Date): string {
  return moment(date).locale('th').add(543, 'year').format('D MMMM YYYY').replace(/เดือน/g, '')
}

export function checkFileExtensionPDF(fileName: string): boolean {
  const allowedExtensions = ['pdf']
  const fileExtension = fileName.split('.').pop()?.toLowerCase()
  return allowedExtensions.includes(fileExtension || '')
}

export function checkFileExtensionimg(fileName: string): boolean {
  const allowedExtensions = ['jpg', 'png', 'jpeg']
  const fileExtension = fileName.split('.').pop()?.toLowerCase()
  return allowedExtensions.includes(fileExtension || '')
}
export const maxSizeimg = 2
export const maxSizepdf = 25
export const SizeimgMB = maxSizeimg * 1024 * 1024
export const SizepdfMB = maxSizepdf * 1024 * 1024

// Returns the full Thai month name for 1–12. Returns '' for invalid input.
export function getThaiFullMonth(n: number): string {
  const months = [
    'มกราคม',
    'กุมภาพันธ์',
    'มีนาคม',
    'เมษายน',
    'พฤษภาคม',
    'มิถุนายน',
    'กรกฎาคม',
    'สิงหาคม',
    'กันยายน',
    'ตุลาคม',
    'พฤศจิกายน',
    'ธันวาคม',
  ]
  if (n >= 1 && n <= 12) {
    return months[n - 1]
  }
  return ''
}

export function GoLogin() {
  window.location.href = `${PortalOpenId}/authorize?response_type=code&client_id=${ClientId}&redirect_uri=${RedirectUris}&scope=${scope}&code_challenge=${code_challenge}&code_challenge_method=S256&state=authencrm`
}

