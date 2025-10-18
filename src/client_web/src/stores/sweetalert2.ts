import { defineStore } from 'pinia'
import Swal from 'sweetalert2'

type AlertIcon = 'success' | 'error' | 'warning' | 'info' | 'question'

export const useSweetAlertStore = defineStore('sweetAlert', {
  state: () => ({}),

  actions: {
    async showAlert(options: {
      title?: string
      text?: string
      icon?: AlertIcon
      confirmButtonText?: string
      showCancelButton?: boolean
      cancelButtonText?: string
      showDenyButton?: boolean
      denyButtonText?: string
      footer?: string
      timer?: number
      confirmButtonClass?: string
      cancelButtonClass?: string
      denyButtonClass?: string
      reverseButtons?: boolean
      showConfirmButton?: boolean
      scrollbarPadding?: boolean
      input?: 'text' | 'textarea'
      inputPlaceholder?: string
      inputValidator?: (value: any) => string | null
      customClass?: {
        popup?: string
        title?: string
        confirmButton?: string
        cancelButton?: string
        denyButton?: string
      }
    }) {
      const classMapping: Record<AlertIcon, { title: string; buttonClass: string }> = {
        success: {
          title: 'text-green',
          buttonClass: 'bg-success text-white hover:bg-green-200 hover:text-black',
        },
        error: {
          title: 'text-red',
          buttonClass: 'bg-error text-white hover:bg-red-200',
        },
        warning: {
          title: 'text-orange',
          buttonClass: 'bg-warning text-white hover:bg-yellow-200',
        },
        info: {
          title: 'infoToAlert',
          buttonClass: 'bg-info text-white hover:bg-blue-200',
        },
        question: {
          title: 'text-primary',
          buttonClass: 'bg-primary text-white hover:bg-primary',
        },
      }

      const selectedClasses = classMapping[options.icon || 'info']

      const result = await Swal.fire({
        title: options.title || 'Default Title',
        text: options.text || 'Default Message',
        icon: options.icon || 'info',
        confirmButtonText: options.confirmButtonText || 'ตกลง',
        showCancelButton: options.showCancelButton || false,
        cancelButtonText: options.cancelButtonText || 'ยกเลิก',
        showDenyButton: options.showDenyButton || false,
        denyButtonText: options.denyButtonText || 'Deny',
        reverseButtons: options.reverseButtons ?? true,
        showConfirmButton: options.showConfirmButton ?? true,
        scrollbarPadding: options.scrollbarPadding ?? true,
        footer: options.footer || '',
        timer: options.timer !== undefined ? options.timer : undefined,
        input: options.input,
        inputPlaceholder: options.inputPlaceholder,
        inputValidator: options.inputValidator,
        customClass: {
          confirmButton: options.confirmButtonClass || 'btn-reset',
          cancelButton: options.cancelButtonClass || 'btn-delate',
          denyButton: options.denyButtonClass || 'bg-primary text-white',
          popup: options.customClass?.popup || 'bg-white rounded-popup',
          title: options.customClass?.title || selectedClasses.title,
        },
        didOpen: () => {
          // ตั้ง z-index สูงที่สุด
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
            popup.style.position = 'relative'
            popup.style.borderRadius = '20px'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
        },
      })

      return result
    }
    ,

    async success(message: string, title?: string) {
      await this.showAlert({
        title: title || 'สำเร็จ!',
        text: message,
        icon: 'success',
      })
    },
    async successDeleted(message: string, timer?: number) {
      await Swal.fire({
        title: 'สำเร็จ!',
        text: message,
        icon: 'success',
        showConfirmButton: false,
        timer: timer || 1000,
        timerProgressBar: true,
        customClass: {
          popup: 'bg-white rounded-popup',
          title: 'text-success fw-bold',
          timerProgressBar: 'bg-success',
        },
        didOpen: () => {
          // ตั้ง z-index สูงที่สุด
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
            popup.style.borderRadius = '20px'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
        },
        didRender: () => {
          const textElement = document.querySelector('.swal2-html-container') as HTMLElement;
          if (textElement) {
            textElement.style.color = 'black';
          }
        },
      });
    },
    async questionDeleted(message: string) {
      await Swal.fire({
        title: 'สำเร็จ!',
        text: message,
        icon: 'question',
        showConfirmButton: false,
        timer: 2000,
        timerProgressBar: true,
        customClass: {
          popup: 'bg-primaryfill',
          title: 'text-primaryEEF',
          timerProgressBar: 'bg-primary',
        },
        didOpen: () => {
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
        },
        didRender: () => {
          const textElement = document.querySelector('.swal2-html-container') as HTMLElement
          if (textElement) {
            textElement.style.color = 'black'
          }
        },
      })
      Swal.fire({
        title: 'กำลังดำเนินการ...',
        text: 'โปรดรอสักครู่',
        allowOutsideClick: false,
        didOpen: () => {
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
          Swal.showLoading()
        },
      })
    },
    async successLogin(message: string) {
      await Swal.fire({
        title: 'เข้าสู่ระบบสำเร็จ',
        text: message,
        icon: 'success',
        showConfirmButton: false,
        timer: 1000,
        timerProgressBar: true,
        customClass: {
          popup: 'bg-primaryfill',
          title: 'text-primaryEEF',
          timerProgressBar: 'bg-success',
        },
        didOpen: () => {
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
        },
        didRender: () => {
          const textElement = document.querySelector('.swal2-html-container') as HTMLElement
          if (textElement) {
            textElement.style.color = 'black'
          }
        },
      })
    },
    async error(message: string, title?: string) {
      await this.showAlert({
        title: title || 'ข้อผิดพลาด!',
        text: message,
        icon: 'error',
      })
    },
    async errorDeleted(message: string, timer?: number) {
      await Swal.fire({
        title: 'ข้อผิดพลาด!',
        text: message,
        icon: 'error',
        showConfirmButton: false,
        timer: timer || 2000,
        timerProgressBar: true,
        customClass: {
          popup: 'bg-white rounded-popup',
          title: 'text-error fw-bold',
          timerProgressBar: 'bg-error',
        },
        didOpen: () => {
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
            popup.style.borderRadius = '20px'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
        },
        didRender: () => {
          const textElement = document.querySelector('.swal2-html-container') as HTMLElement
          if (textElement) {
            textElement.style.color = 'black'
          }
        },
      })
    },

    async warning(message: string, title?: string) {
      return await this.showAlert({
        title: title || 'คำเตือน!',
        text: message,
        icon: 'warning',
      })
    },

    async info(message: string, title?: string) {
      await this.showAlert({
        title: title || 'โปรดทราบ',
        text: message,
        icon: 'info',
      })
    },
    async question(
      message: string,
      title?: string,
      confirmButtonClass?: string,
      cancelButtonClass?: string,
    ) {
      await Swal.fire({
        title: title || 'Are you sure?',
        text: message || 'Do you want to proceed?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Yes, proceed!',
        cancelButtonText: 'Cancel',
        customClass: {
          popup: 'bg-black',
          title: 'text-primary',
          confirmButton:
            confirmButtonClass || 'bg-primary text-white hover:bg-green-200 hover:text-black',
          cancelButton: cancelButtonClass || 'bg-error text-white hover:bg-red-200',
        },
        reverseButtons: true,
        didOpen: () => {
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
        },
      })
    },
    async showtextarea(
      message: string,
      title?: string,
      confirmButtonClass?: string,
      cancelButtonClass?: string,
    ) {
      const { value } = await Swal.fire({
        title: title || 'กรอกข้อความ',
        input: 'textarea',
        inputLabel: message,
        inputPlaceholder: 'กรุณาระบุรายละเอียด...',
        showCancelButton: true,
        confirmButtonText: 'ส่งคำขอ',
        cancelButtonText: 'ยกเลิก',
        icon: 'question',
        customClass: {
          popup: 'bg-white',
          title: 'text-primary',
          confirmButton: confirmButtonClass || 'bg-successEEF',
          cancelButton: cancelButtonClass || 'bg-redEEF',
        },
        reverseButtons: true,
        didOpen: () => {
          const popup = Swal.getPopup()
          const container = Swal.getContainer()
          if (popup) {
            popup.style.zIndex = '999999'
          }
          if (container) {
            container.style.zIndex = '999999'
          }
        },
        preConfirm: async (inputValue) => {
          if (!inputValue) {
            Swal.showValidationMessage('กรุณากรอกข้อความก่อนส่ง!')
            return false
          }

          Swal.fire({
            title: 'กำลังดำเนินการ...',
            text: 'โปรดรอสักครู่',
            allowOutsideClick: false,
            didOpen: () => {
              const popup = Swal.getPopup()
              const container = Swal.getContainer()
              if (popup) {
                popup.style.zIndex = '999999'
              }
              if (container) {
                container.style.zIndex = '999999'
              }
              Swal.showLoading()
            },
          })
        },
      })

      if (value) {
        return value // คืนค่าข้อความที่ผู้ใช้กรอก
      }
      return null
    },
  },
})
