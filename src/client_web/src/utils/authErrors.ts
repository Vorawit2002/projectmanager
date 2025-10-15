/**
 * Authentication Error Codes
 */
export const AuthErrorCodes = {
  INVALID_CREDENTIALS: 'AUTH001',
  USER_NOT_FOUND: 'AUTH002',
  EMAIL_ALREADY_EXISTS: 'AUTH003',
  WEAK_PASSWORD: 'AUTH004',
  TOKEN_EXPIRED: 'AUTH005',
  INVALID_TOKEN: 'AUTH006',
  PASSWORD_MISMATCH: 'AUTH007',
  INVALID_EMAIL: 'AUTH008',
  INVALID_USERNAME: 'AUTH009',
  REGISTRATION_FAILED: 'AUTH010',
  UNKNOWN_ERROR: 'AUTH999',
} as const

export type AuthErrorCode = typeof AuthErrorCodes[keyof typeof AuthErrorCodes]

/**
 * Thai error messages for authentication errors
 */
export const AUTH_ERROR_MESSAGES: Record<AuthErrorCode, string> = {
  [AuthErrorCodes.INVALID_CREDENTIALS]: 'อีเมลหรือรหัสผ่านไม่ถูกต้อง',
  [AuthErrorCodes.USER_NOT_FOUND]: 'ไม่พบผู้ใช้งานในระบบ',
  [AuthErrorCodes.EMAIL_ALREADY_EXISTS]: 'อีเมลนี้ถูกใช้งานแล้ว',
  [AuthErrorCodes.WEAK_PASSWORD]: 'รหัสผ่านไม่ปลอดภัยเพียงพอ รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร',
  [AuthErrorCodes.TOKEN_EXPIRED]: 'เซสชันหมดอายุ กรุณาเข้าสู่ระบบใหม่',
  [AuthErrorCodes.INVALID_TOKEN]: 'ข้อมูลการเข้าสู่ระบบไม่ถูกต้อง',
  [AuthErrorCodes.PASSWORD_MISMATCH]: 'รหัสผ่านและยืนยันรหัสผ่านไม่ตรงกัน',
  [AuthErrorCodes.INVALID_EMAIL]: 'รูปแบบอีเมลไม่ถูกต้อง',
  [AuthErrorCodes.INVALID_USERNAME]: 'ชื่อผู้ใช้ไม่ถูกต้อง',
  [AuthErrorCodes.REGISTRATION_FAILED]: 'ลงทะเบียนไม่สำเร็จ',
  [AuthErrorCodes.UNKNOWN_ERROR]: 'เกิดข้อผิดพลาดที่ไม่ทราบสาเหตุ',
}

/**
 * Interface for API error response
 */
export interface ApiErrorResponse {
  message?: string
  errors?: Record<string, string[]> | string[]
  title?: string
  status?: number
  detail?: string
}

/**
 * Parse error response from API and return Thai error message
 */
export function parseAuthError(error: any): string {
  // If it's already a string, return it
  if (typeof error === 'string') {
    return error
  }

  // If it's an ApiException from the client
  if (error?.response) {
    try {
      const errorData: ApiErrorResponse = JSON.parse(error.response)
      return extractErrorMessage(errorData)
    } catch {
      return error.message || AUTH_ERROR_MESSAGES[AuthErrorCodes.UNKNOWN_ERROR]
    }
  }

  // If it's a direct error object
  if (error?.message) {
    return error.message
  }

  return AUTH_ERROR_MESSAGES[AuthErrorCodes.UNKNOWN_ERROR]
}

/**
 * Extract error message from API error response
 */
function extractErrorMessage(errorData: ApiErrorResponse): string {
  // Check for validation errors
  if (errorData.errors) {
    if (Array.isArray(errorData.errors)) {
      return errorData.errors.join(', ')
    }
    
    if (typeof errorData.errors === 'object') {
      const errorMessages: string[] = []
      for (const key in errorData.errors) {
        const messages = errorData.errors[key]
        if (Array.isArray(messages)) {
          errorMessages.push(...messages)
        }
      }
      if (errorMessages.length > 0) {
        return mapValidationErrors(errorMessages)
      }
    }
  }

  // Check for direct message
  if (errorData.message) {
    return mapErrorMessage(errorData.message)
  }

  // Check for title
  if (errorData.title) {
    return mapErrorMessage(errorData.title)
  }

  // Check for detail
  if (errorData.detail) {
    return mapErrorMessage(errorData.detail)
  }

  return AUTH_ERROR_MESSAGES[AuthErrorCodes.UNKNOWN_ERROR]
}

/**
 * Map validation error messages to Thai
 */
function mapValidationErrors(errors: string[]): string {
  const mappedErrors = errors.map(error => {
    const lowerError = error.toLowerCase()
    
    if (lowerError.includes('email') && lowerError.includes('already')) {
      return AUTH_ERROR_MESSAGES[AuthErrorCodes.EMAIL_ALREADY_EXISTS]
    }
    if (lowerError.includes('password') && (lowerError.includes('short') || lowerError.includes('length'))) {
      return AUTH_ERROR_MESSAGES[AuthErrorCodes.WEAK_PASSWORD]
    }
    if (lowerError.includes('password') && lowerError.includes('match')) {
      return AUTH_ERROR_MESSAGES[AuthErrorCodes.PASSWORD_MISMATCH]
    }
    if (lowerError.includes('email') && lowerError.includes('invalid')) {
      return AUTH_ERROR_MESSAGES[AuthErrorCodes.INVALID_EMAIL]
    }
    if (lowerError.includes('username') && lowerError.includes('invalid')) {
      return AUTH_ERROR_MESSAGES[AuthErrorCodes.INVALID_USERNAME]
    }
    
    return error
  })
  
  return mappedErrors.join(', ')
}

/**
 * Map error message to Thai
 */
function mapErrorMessage(message: string): string {
  const lowerMessage = message.toLowerCase()
  
  if (lowerMessage.includes('invalid') && (lowerMessage.includes('credential') || lowerMessage.includes('password') || lowerMessage.includes('email'))) {
    return AUTH_ERROR_MESSAGES[AuthErrorCodes.INVALID_CREDENTIALS]
  }
  if (lowerMessage.includes('user') && lowerMessage.includes('not found')) {
    return AUTH_ERROR_MESSAGES[AuthErrorCodes.USER_NOT_FOUND]
  }
  if (lowerMessage.includes('email') && lowerMessage.includes('already')) {
    return AUTH_ERROR_MESSAGES[AuthErrorCodes.EMAIL_ALREADY_EXISTS]
  }
  if (lowerMessage.includes('password') && (lowerMessage.includes('weak') || lowerMessage.includes('short'))) {
    return AUTH_ERROR_MESSAGES[AuthErrorCodes.WEAK_PASSWORD]
  }
  if (lowerMessage.includes('token') && lowerMessage.includes('expired')) {
    return AUTH_ERROR_MESSAGES[AuthErrorCodes.TOKEN_EXPIRED]
  }
  if (lowerMessage.includes('token') && lowerMessage.includes('invalid')) {
    return AUTH_ERROR_MESSAGES[AuthErrorCodes.INVALID_TOKEN]
  }
  if (lowerMessage.includes('unauthorized')) {
    return AUTH_ERROR_MESSAGES[AuthErrorCodes.INVALID_CREDENTIALS]
  }
  
  return message
}

/**
 * Get error message by error code
 */
export function getAuthErrorMessage(code: AuthErrorCode): string {
  return AUTH_ERROR_MESSAGES[code] || AUTH_ERROR_MESSAGES[AuthErrorCodes.UNKNOWN_ERROR]
}

/**
 * Check if error is authentication related
 */
export function isAuthError(error: any): boolean {
  if (!error) return false
  
  const status = error.status || error.response?.status
  return status === 401 || status === 403
}

/**
 * Check if error is validation related
 */
export function isValidationError(error: any): boolean {
  if (!error) return false
  
  const status = error.status || error.response?.status
  return status === 400
}
