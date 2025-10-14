<template>
  <!-- Loading State -->
  <div
    v-if="loading"
    class="text-center py-8"
  >
    <v-progress-circular
      indeterminate
      color="primary"
      size="64"
    ></v-progress-circular>
    <p class="mt-4">Loading images...</p>
  </div>

  <!-- Empty State -->
  <div
    v-else-if="files.length === 0"
    class="text-center py-8"
  >
    <v-icon
      size="64"
      color="grey"
      >mdi-image-off</v-icon
    >
    <p class="mt-4 text-grey">No images available</p>
  </div>

  <v-container>
    <!-- Download All Images Button -->
    <v-row>
      <v-col
        cols="12"
        class="text-right mb-0"
        v-if="!loading && files.length > 0"
      >
        <v-btn
          color="primary"
          class="elevation-5"
          @click="downloadAll"
          prepend-icon="ri-folder-zip-line"
        >
          ดาวน์โหลดเอกสารทั้งหมด
        </v-btn>
      </v-col>
    </v-row>
    <v-row class="my-4">
      <v-col
        cols="12"
        md="12"
      >
        <v-carousel
          v-model="currentSlide"
          :continuous="true"
          :show-arrows="true"
          height="400"
          hide-delimiters
          delimiter-icon="mdi-minus"
          v-if="!loading && groupedSlides.length > 0"
        >
          <v-carousel-item
            v-for="(slide, index) in groupedSlides"
            :key="index"
          >
            <v-row
              no-gutters
              class="fill-height justify-center align-center"
            >
              <v-col
                v-for="(image, imgIndex) in slide.images"
                :key="imgIndex"
                v-bind="getResponsiveColProps(slide.images.length)"
                class="pa-1 d-flex align-center"
              >
                <v-avatar
                  rounded="lg"
                  :size="250"
                  class="mx-auto px-1 py-1 preview-avatar"
                >
                  <template v-if="image.isImage">
                    <v-img
                      v-if="image.src"
                      :src="`${image.src}`"
                      cover
                      height="380"
                      width="500"
                      class="rounded border-IMG"
                      style="cursor: pointer"
                      @click="showImage(image)"
                      @error="image.broken = true"
                    >
                      <template v-slot:placeholder>
                        <v-row
                          class="fill-height ma-0"
                          align="center"
                          justify="center"
                        >
                          <v-progress-circular
                            indeterminate
                            color="grey lighten-5"
                          ></v-progress-circular>
                        </v-row>
                      </template>
                    </v-img>
                    <div
                      v-else
                      class="d-flex flex-column align-center justify-center"
                      style="height: 380px; width: 500px"
                    >
                      <v-icon
                        color="error"
                        size="64"
                        >mdi-image-broken-variant</v-icon
                      >
                      <span class="mt-2 text-error">Image not available</span>
                    </div>
                  </template>

                  <v-img
                    v-else
                    :src="image.src"
                    cover
                    height="380"
                    width="500"
                    class="rounded border-IMG"
                    style="cursor: pointer"
                    @click="downloadFile(image, image.title)"
                  >
                  </v-img>

                  <div class="image-overlay">
                    <!-- Description Chip (Bottom) -->
                    <v-chip
                      v-if="image.description"
                      color="primary-darken-0"
                      class="chip-description text-primary"
                      small
                      @click="downloadFile(image, getImageNumber(index, imgIndex))"
                    >
                      <v-icon
                        size="18"
                        class="mr-1"
                        >ri-download-2-line</v-icon
                      >{{ getImageNumber(index, imgIndex) }}
                    </v-chip>
                  </div>
                </v-avatar>
                <!-- </v-card> -->
              </v-col>
            </v-row>
          </v-carousel-item>
        </v-carousel>

        <!-- Navigation Info -->
        <div
          class="text-center mt-4"
          v-if="!loading && groupedSlides.length > 0"
        >
          <v-progress-linear
            :max="groupedSlides.length - 1"
            v-model="currentSlide"
          ></v-progress-linear>
        </div>
      </v-col>
    </v-row>
  </v-container>

  <!-- Image Preview Dialog -->
  <v-dialog
    v-model="dialog"
    :max-width="$vuetify.display.xs ? '95vw' : $vuetify.display.sm ? '80vw' : '900'"
    :fullscreen="$vuetify.display.xs"
    class="image-preview-dialog"
  >
    <v-card
      class="d-flex flex-column align-center justify-center dialog-card"
      :class="{
        'mobile-card': $vuetify.display.xs,
        'tablet-card': $vuetify.display.sm,
        'desktop-card': $vuetify.display.mdAndUp,
      }"
    >
      <!-- Close Button -->
      <v-btn
        icon
        color="error"
        class="close-btn"
        @click="dialog = false"
        :size="$vuetify.display.xs ? 'default' : 'small'"
      >
        <v-icon :size="$vuetify.display.xs ? 20 : 16">ri-close-large-fill</v-icon>
      </v-btn>

      <!-- Image Container -->
      <div class="image-container mt-md-9">
        <img
          v-if="selectedImage"
          :src="getFullImageSrc(selectedImage)"
          class="preview-image"
          alt="Preview image"
        />
      </div>

      <!-- Description -->
      <!-- <div
        v-if="selectedImage && selectedImage.description && selectedImage.description !== 'No description available'"
        class="description-text"
      >
        {{ selectedImage.description }}
      </div> -->

      <!-- Download Button -->
      <div class="download-section">
        <v-btn
          color="primary"
          class="elevation-5 download-btn"
          :size="$vuetify.display.xs ? 'default' : 'small'"
          :prepend-icon="$vuetify.display.xs ? '' : 'ri-download-line'"
          @click="downloadImageByone"
        >
          <v-icon
            v-if="$vuetify.display.xs"
            class="mr-2"
            size="18"
          >
            ri-download-line
          </v-icon>
          {{ $vuetify.display.xs ? 'ดาวน์โหลด' : 'ดาวน์โหลดรูปภาพ' }}
        </v-btn>
      </div>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { BACKEND_API_URL, MediafileUrl } from '@/constants'
import { Client, GetActivityPlanAttachmentsByActivityPlanIdQuery } from '@/client'
import '@/assets/styles/CarouselsImage.css'
import pdf from '@/assets/images/logos/pdf.png'
import doc from '@/assets/images/logos/doc.png'
import docx from '@/assets/images/logos/file.png'
import xls from '@/assets/images/logos/xls.png'
import xlsx from '@/assets/images/logos/xlsx.png'
import ppt from '@/assets/images/logos/slide.png'
import pptx from '@/assets/images/logos/pptx-file.png'
import zip from '@/assets/images/logos/zip.png'
import Sevenzip from '@/assets/images/logos/7z.png'
import rar from '@/assets/images/logos/rar-file-format.png'
import JSZip from 'jszip'

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'CarouselsImage',
  props: {
    id: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      pdf: pdf,
      doc: doc,
      docx: docx,
      xls: xls,
      xlsx: xlsx,
      ppt: ppt,
      pptx: pptx,
      zip: zip,
      Sevenzip: Sevenzip,
      rar: rar,
      files: [] as any,
      rawFiles: [] as any, // เก็บข้อมูลดิบที่ได้จาก API
      request: new GetActivityPlanAttachmentsByActivityPlanIdQuery(),
      currentSlide: 0 as any,
      slides: [] as any, // รูปที่แบ่งกลุมแล้ว
      loading: false as boolean,
      imagesPerSlide: 4 as number, // จำนวนรูปต่อ slide
      dialog: false as boolean,
      selectedImage: null as any,
    }
  },
  async mounted() {
    await this.initialize()
  },
  computed: {
    // แบ่งรูปออกเป็นกลุม ๆ ละ 3 รูป
    groupedSlides(): any {
      const groups: any = []
      for (let i = 0; i < this.files.length; i += this.imagesPerSlide) {
        groups.push({
          images: this.files.slice(i, i + this.imagesPerSlide),
        })
      }
      return groups
    },
  },
  methods: {
    async initialize() {
      try {
        if (this.id) {
          // ทำการดึงข้อมูลหรือเตรียมค่าที่จำเป็นสำหรับ id ที่ได้รับ
          this.request.activityPlanId = this.id
          const response = await client.getActivityPlanAttachmentsQueryByActivityPlanId(this.request)
          this.rawFiles = response // เก็บข้อมูลดิบไว้
          console.log('Response from API:', this.rawFiles)
          this.files = response.map((attachment: any) => {
            return {
              ...attachment,
              create: true, // กำหนดว่าไฟล์นี้ถูกสร้างแล้ว
            }
          })
          this.files = this.transformApiData(this.files)
          console.log('Files loaded:', this.files)
        }
      } catch (error) {
        console.error('Error initializing component:', error)
      }
    },
    isImageFile(extension: string): boolean {
      return ['.jpg', '.jpeg', '.png', '.gif', '.bmp', '.webp'].includes(extension.toLowerCase())
    },

    getFileIcon(extension: string): string {
      const ext = extension.toLowerCase()
      const icons: Record<string, string> = {
        '.pdf': this.pdf,
        '.doc': this.doc,
        '.docx': this.docx,
        '.xls': this.xls,
        '.xlsx': this.xlsx,
        '.ppt': this.ppt,
        '.pptx': this.pptx,
        '.zip': this.zip,
        '.7z': this.Sevenzip,
        '.rar': this.rar,
      }
      return icons[ext] || this.docx // fallback เป็น icon ทั่วไป
    },

    transformApiData(apiData: any[]): any {
      return apiData.map((item, index) => {
        const ext = item.attachments?.fileExtension?.toLowerCase() || ''
        const isImage = this.isImageFile(ext)
        const base64 = item.thumbnailBase64 || ''
        const pathFile = item.attachments?.pathFile || ''
        const nameFile = item.attachments?.nameFile || `เอกสารอ้างอิง ${index + 1}`

        let src = null
        let broken = false
        let downloadUrl = null

        if (isImage) {
          if (base64) {
            src = `data:image/png;base64,${base64}`
          } else if (pathFile) {
            if (pathFile.startsWith('http') || pathFile.startsWith('/')) {
              src = pathFile
            } else {
              src = `${BACKEND_API_URL}/${pathFile}`
            }
          } else {
            broken = true
          }
        } else {
          // สำหรับไฟล์เอกสาร: src คือไอคอน, downloadUrl คือไฟล์จริง
          src = this.getFileIcon(ext)
        }

        // ✅ กำหนด downloadUrl (เฉพาะ pathFile ที่มีข้อมูล)
        if (pathFile) {
          downloadUrl =
            pathFile.startsWith('http') || pathFile.startsWith('/') ? pathFile : `${MediafileUrl}/${pathFile}`
        }

        return {
          src,
          alt: nameFile,
          title: nameFile,
          description: `ไฟล์ประเภท ${ext}`,
          isImage,
          broken,
          downloadUrl, // ✅ ใส่ไว้สำหรับโหลดไฟล์จริง
        }
      })
    },

    nextSlide(): void {
      this.currentSlide = (this.currentSlide + 1) % this.groupedSlides.length
    },

    prevSlide(): void {
      this.currentSlide = this.currentSlide === 0 ? this.groupedSlides.length - 1 : this.currentSlide - 1
    },
    goToSlide(index: number): void {
      this.currentSlide = index
    },
    // แก้ไขการคำนวณเลขให้ถูกต้อง - ให้เลขต่อเนื่อง
    getImageNumber(slideIndex: number, imgIndex: number): string {
      const imageNumber = slideIndex * this.imagesPerSlide + imgIndex + 1
      return `เอกสารอ้างอิง ${imageNumber}`
    },
    // Responsive column props for Vuetify grid
    getResponsiveColProps(imageCount: number) {
      // 1 image: full width, 2: half, 3: third, 4+: quarter
      if (imageCount === 1) {
        return { cols: 12, sm: 12, md: 12, lg: 12 }
      } else if (imageCount === 2) {
        return { cols: 12, sm: 6, md: 6, lg: 6 }
      } else if (imageCount === 3) {
        return { cols: 12, sm: 6, md: 4, lg: 4 }
      } else if (imageCount === 4) {
        return { cols: 12, sm: 6, md: 3, lg: 3 }
      } else {
        // 5+ images: stack on mobile, 2 per row on sm, 3 on md, 4 on lg
        return { cols: 12, sm: 6, md: 4, lg: 3 }
      }
    },
    /**
     * Download the image as a PNG file.
     * @param image The image object (with .src as base64)
     * @param name The filename (string)
     */
    async downloadFile(image: any, name: string) {
      console.log('Downloading file:', image, 'with name:', name)
      const ext = image.title?.slice(image.title.lastIndexOf('.')).toLowerCase() || ''
      const fileName = name.endsWith(ext) ? name : `${name}${ext}`

      const mimeTypes: Record<string, string> = {
        '.pdf': 'application/pdf',
        '.doc': 'application/msword',
        '.docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
        '.xls': 'application/vnd.ms-excel',
        '.xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
        '.ppt': 'application/vnd.ms-powerpoint',
        '.pptx': 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
        '.zip': 'application/zip',
        '.rar': 'application/vnd.rar',
        '.7z': 'application/x-7z-compressed',
        '.jpg': 'image/jpeg',
        '.jpeg': 'image/jpeg',
        '.png': 'image/png',
        '.gif': 'image/gif',
      }
      const mimeType = mimeTypes[ext] || 'application/octet-stream'

      try {
        const response = await fetch(image.downloadUrl)
        if (!response.ok) throw new Error('Download failed')
        let blob = await response.blob()

        if (blob.type === '' || blob.type === 'application/octet-stream') {
          blob = new Blob([await blob.arrayBuffer()], { type: mimeType })
        }

        const url = URL.createObjectURL(blob)
        const a = document.createElement('a')
        a.href = url
        a.download = fileName
        document.body.appendChild(a)
        a.click()
        // setTimeout(() => {
        //   document.body.removeChild(a)
        //   URL.revokeObjectURL(url)
        // }, 100)
      } catch (err) {
        console.error('Download error:', err)
        alert('ไม่สามารถดาวน์โหลดไฟล์ได้')
      }
    },
    /**
     * Download all images fetched from initialize (rawFiles).
     */
    async downloadAll() {
      if (!this.rawFiles || this.rawFiles.length === 0) return

      const zip = new JSZip()

      const addFileToZip = async (item: any, idx: number) => {
        const attachment = item.attachments
        const fileName = attachment?.nameFile || `file_${idx + 1}`
        const fileExt = attachment?.fileExtension?.toLowerCase() || ''
        const fullFileName = fileName.endsWith(fileExt) ? fileName : `${fileName}${fileExt}`
        let src = item.thumbnailBase64 || item.image_url || item.src

        try {
          if (typeof src === 'string') {
            if (src.startsWith('http') || src.startsWith('/')) {
              // ✅ Fetch as Blob with fallback MIME
              const response = await fetch(src)
              let blob = await response.blob()
              if (blob.type === '' || blob.type === 'application/octet-stream') {
                // ใช้ MIME type ตามนามสกุล
                const mimeTypes: Record<string, string> = {
                  '.jpg': 'image/jpeg',
                  '.jpeg': 'image/jpeg',
                  '.png': 'image/png',
                  '.gif': 'image/gif',
                  '.pdf': 'application/pdf',
                  '.doc': 'application/msword',
                  '.docx': 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
                  '.xls': 'application/vnd.ms-excel',
                  '.xlsx': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
                  '.ppt': 'application/vnd.ms-powerpoint',
                  '.pptx': 'application/vnd.openxmlformats-officedocument.presentationml.presentation',
                  '.zip': 'application/zip',
                  '.rar': 'application/vnd.rar',
                  '.7z': 'application/x-7z-compressed',
                }
                const mimeType = mimeTypes[fileExt] || 'application/octet-stream'
                blob = new Blob([await blob.arrayBuffer()], { type: mimeType })
              }
              zip.file(fullFileName, blob)
            } else if (src.startsWith('data:')) {
              // ✅ Base64 with prefix
              const base64 = src.split(',')[1]
              zip.file(fullFileName, base64, { base64: true })
            } else if (/^[A-Za-z0-9+/=]+$/.test(src)) {
              // ✅ Raw base64
              zip.file(fullFileName, src, { base64: true })
            } else {
              console.warn(`⚠️ ไม่สามารถประมวลผลไฟล์: ${fullFileName}`)
            }
          }
        } catch (e) {
          console.error(`❌ Error loading file ${fullFileName}`, e)
        }
      }

      await Promise.all(this.rawFiles.map((item: any, idx: number) => addFileToZip(item, idx)))

      const content = await zip.generateAsync({ type: 'blob' })
      const url = URL.createObjectURL(content)
      const a = document.createElement('a')
      a.href = url
      a.download = 'attachments.zip' // ✅ ตั้งชื่อ zip เป็นภาษาอังกฤษ
      document.body.appendChild(a)
      a.click()
      setTimeout(() => {
        document.body.removeChild(a)
        URL.revokeObjectURL(url)
      }, 100)
    },
    /**
     * Show image in popup dialog.
     */
    showImage(image: any) {
      this.selectedImage = { ...image }
      this.dialog = true
      // Debug: log selectedImage
      console.log('Selected image for dialog:', this.selectedImage)
    },
    /**
     * Get full image src for dialog (handles base64 or URL).
     */
    getFullImageSrc(image: any) {
      if (!image || !image.src) return ''
      if (image.src.startsWith('http')) return image.src
      // If already has data:image prefix, return as is
      if (image.src.startsWith('data:image')) return image.src
      // Otherwise, assume base64 and add prefix
      return `data:image/png;base64,${image.src}`
    },
    /**
     * Download the currently previewed image in the dialog.
     */
    downloadImageByone() {
      if (!this.selectedImage) return
      // Use title, alt, or fallback name
      const name = this.selectedImage.title || this.selectedImage.alt || 'downloaded-image'
      this.downloadFile(this.selectedImage, name)
    },
  },
})
</script>

<style scoped></style>
