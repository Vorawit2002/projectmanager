import { defineStore } from 'pinia'

export const usePhotoCaptureStore = defineStore('photoCapture', {
  state: () => ({
    checkinImage: null as string | null,
    checkoutImage: null as string | null,
  }),
  actions: {
    setCapturedPhoto(imageData: string, imageType: 'checkin' | 'checkout') {
      if (imageType === 'checkin') {
        this.checkinImage = imageData
      } else if (imageType === 'checkout') {
        this.checkoutImage = imageData
      }
    },
    clearCapturedPhoto() {
      this.checkinImage = null
      this.checkoutImage = null
    },
  },
})
