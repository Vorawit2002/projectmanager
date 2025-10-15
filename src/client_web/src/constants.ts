import moment from 'moment'

//local
export const BACKEND_API_URL = 'https://localhost:5001'
// export const BACKEND_API_URL = 'http://localhost:8800'
// export const BACKEND_API_URL = import.meta.env.VITE_BACKEND_API 
//nti
// export const BACKEND_API_URL = 'https://crm.nti.co.th' // nti

// Authentication endpoints
export const AUTH_ENDPOINTS = {
  LOGIN: '/api/auth/login',
  REGISTER: '/api/auth/register',
  REFRESH: '/api/auth/refresh-token',
  ME: '/api/auth/me'
}

export const MediafileUrl = `https://media.nti.co.th/crmfile/`

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



