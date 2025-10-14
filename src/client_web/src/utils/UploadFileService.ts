import { BACKEND_API_URL } from '@/const'
import { useSweetAlertStore } from '@/stores/index'

export async function uploadFile(formData: FormData): Promise<any> {
  const SweetAlert = useSweetAlertStore()

  try {
    const response = await fetch(BACKEND_API_URL + '/api/UploadFileEndpoint/UploadFile', {
      method: 'POST',
      body: formData,
    })

    const result = await response.json()

    if (response.ok) {
      SweetAlert.success('อัพโหลดข้อมูลสำเร็จ')
      console.log('Upload success:', result)
      return result
    } else {
      SweetAlert.error('เกิดข้อผิดพลาดในการอัพโหลดเอกสาร')
      console.error('Upload failed:', result)
      throw result
    }
  } catch (error) {
    SweetAlert.error('ไม่สามารถเชื่อมต่อเซิร์ฟเวอร์ได้')
    console.error('Upload error:', error)
    throw error
  }
}
