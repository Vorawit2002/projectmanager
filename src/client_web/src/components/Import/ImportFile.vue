<template>
  <section class="justify-center mt-4 mb-8 card-rounded">
    <v-form>
      <v-row class="d-flex justify-center">
        <v-col cols="12">
          <v-card
            class="d-flex flex-column align-center justify-between upload-zone mb-4 rounded-xl bg-white dashed-border"
            variant="outlined"
            @click="triggerFileInput"
          >
            <div
              v-if="loading"
              class="loader_file"
            >
              <label>กำลังอัพโหลดรูปภาพ...</label>
              <div class="loading_file"></div>
            </div>

            <div
              v-else
              class="text-center mt-4"
            >
              <v-avatar
                rounded="0"
                :size="$vuetify.display.xs ? 80 : 100"
                class="mb-2 mb-sm-3 mt-4 mt-sm-5 mx-auto upload-icon"
              >
                <v-img
                  alt="Uploade_IMG"
                  :src="Uploade_IMG"
                  cover
                ></v-img>
              </v-avatar>

              <v-card-title class="text-center text-detail-uploade pa-2">
                <span class="d-none d-sm-block">ลากและวางไฟล์ได้ที่นี่</span>
                <span class="d-sm-none">เลือกไฟล์</span>
              </v-card-title>
              <v-card-text class="text-center text-detail-uploade pa-2">
                <span class="d-none d-sm-block">หรือ เลือกไฟล์ ได้จากในอุปกรณ์ของคุณ</span>
                <span class="d-sm-none">จากอุปกรณ์ของคุณ</span>
              </v-card-text>
            </div>
          </v-card>

          <span class="d-flex justify-start mb-8 text-primary-darken-1">
            รองรับไฟล์รูปภาพ เช่น .jpg, .png, .gif และไฟล์เอกสาร เช่น .pdf, .doc, .docx, .xls, .xlsx, .ppt, .pptx, .zip,
            .rar และ .7z
          </span>

          <!-- Hidden v-file-input -->
          <v-file-input
            ref="fileInput"
            label="อัพโหลดไฟล์"
            hide-input
            class="d-none"
            :model-value="null"
            accept=".jpg,.jpeg,.png,.gif,.pdf,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.zip,.rar,.7z"
            multiple
            @update:modelValue="handleFileChange"
          />
        </v-col>
      </v-row>

      <v-col
        cols="12"
        md="12"
      >
        <v-row class="d-flex justify-center mb-4">
          <v-col
            v-for="(file, index) in nameInput"
            :key="index"
            cols="12"
            md="4"
            class="text-center"
          >
            <div class="image-wrapper">
              <v-avatar
                rounded="lg"
                :size="$vuetify.display.xs ? 100 : $vuetify.display.sm ? 130 : 140"
                class="mx-auto px-1 py-1 preview-avatar"
              >
                <v-img
                  :src="getImageUrl(file)"
                  cover
                  :class="{ 'preview-image': true, loading: loading }"
                  style="border-radius: 12px"
                ></v-img>
              </v-avatar>

              <!-- ปุ่มลบ - Responsive Version -->
              <v-btn
                icon
                size="small"
                class="image-close-btn"
                color="error"
                variant="elevated"
                @click.stop="removeImage(index)"
              >
                <v-icon
                  :size="$vuetify.display.xs ? 15 : 16"
                  icon="ri-close-large-line"
                ></v-icon>
              </v-btn>
            </div>

            <div class="text-caption mt-2">{{ file.name || file.attachments?.nameFile }}</div>
          </v-col>
        </v-row>
      </v-col>

      <v-card-actions class="justify-center">
        <!-- <v-btn
          class="mr-4 mb-0"
          rounded="lg"
          variant="outlined"
          @click="clearFileName"
          color="error-darken-1"
          >ยกเลิก</v-btn
        > -->
        <v-btn
        v-if="!readonly"
          :loading="loading"
          @click="ConfirmUploadFile"
          class="mb-0 bg_BTN text-white"
          rounded="lg"
        >
          อัพโหลด
        </v-btn>
      </v-card-actions>
    </v-form>
  </section>
</template>

<script lang="ts">
import { Client, CreateActivityPlanAttachmentCommand, GetActivityPlanAttachmentsByActivityPlanIdQuery } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { defineComponent } from 'vue'
import '@/assets/styles/ImportFile.css'
import Uploade_IMG from '../../assets/images/logos/generative-image.png'
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

const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'ImportFile',
  props: {
    id: {
      type: String,
      required: true,
    },
    readonly: {
      type: Boolean,
      default: false,
    },
  },
  components: {},
  data() {
    return {
      Uploade_IMG: Uploade_IMG,
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
      nameInput: [] as any,
      loading: false,
      uploadFile: [] as any,
      selectedFileName: '',
      files: [] as File[],
      StepperValue: ['1'] as any,
      CreateFile: new CreateActivityPlanAttachmentCommand(),
      request: new GetActivityPlanAttachmentsByActivityPlanIdQuery(),
    }
  },
  async mounted() {
    await this.initialize()
  },
  methods: {
    async initialize() {
      try {
        if (this.id) {
          // ทำการดึงข้อมูลหรือเตรียมค่าที่จำเป็นสำหรับ id ที่ได้รับ
          this.request.activityPlanId = this.id
          const response = await client.getActivityPlanAttachmentsQueryByActivityPlanId(this.request)
          this.nameInput = response.map((attachment: any) => {
            return {
              ...attachment,
              create: true, // กำหนดว่าไฟล์นี้ถูกสร้างแล้ว
            }
          })
          console.log('Fetched attachments:', this.nameInput)
        }
      } catch (error) {
        console.error('Error initializing component:', error)
      }
    },
    getImageUrl(file: any) {
      const iconMap: { [key: string]: string } = {
        pdf: this.pdf,
        doc: this.doc,
        docx: this.docx,
        xls: this.xls,
        xlsx: this.xlsx,
        ppt: this.ppt,
        pptx: this.pptx,
        zip: this.zip,
        '7z': this.Sevenzip,
        rar: this.rar,
      }

      const isImageExtension = (ext: string) => ['jpg', 'jpeg', 'png', 'gif', 'webp'].includes(ext)

      // ✅ 1. กรณีเป็น File object ที่อัปโหลดใหม่
      if (file instanceof File) {
        const ext = file.name.split('.').pop()?.toLowerCase() || ''
        return file.type.startsWith('image/') ? URL.createObjectURL(file) : iconMap[ext] || this.Uploade_IMG
      }

      // ✅ 2. กรณีเป็นไฟล์จาก API (object ที่มี attachments)
      if (file && file.attachments) {
        const ext = file.attachments.fileExtension?.replace('.', '').toLowerCase() || ''
        const nameFile = file.attachments.nameFile
        const isImage = isImageExtension(ext)

        if (isImage && file.thumbnailBase64) {
          return `data:image/png;base64,${file.thumbnailBase64}`
        }

        return iconMap[ext] || this.Uploade_IMG
      }

      // ✅ 3. กรณี thumbnailBase64 ที่เป็น string ตรง ๆ
      if (typeof file === 'string') {
        return `data:image/png;base64,${file}`
      }

      return this.Uploade_IMG
    },
    //ใช้อันนี้ Uploadfile
    async ConfirmUploadFile() {
      if (!this.nameInput || this.nameInput.length === 0) {
        console.warn('No files selected')
        return
      }
      this.loading = true
      try {
        this.nameInput.forEach((file: any) => {
          if (file.create !== true) {
            new Promise(resolve => {
              const reader = new FileReader()
              reader.onload = (e: any) => {
                resolve(e.target.result)
              }

              reader.readAsDataURL(file)
            }).then(async (data: any) => {
              this.CreateFile.activityPlanId = this.id
              this.CreateFile.fileName = file.name
              this.CreateFile.base64 = data.split(',')[1] // แยกข้อมูล base
              console.log(this.CreateFile)
              const response = await client.createActivityPlanAttachment(this.CreateFile)

              file.create = true
            })
          }
        })
      } catch (err) {
        console.error('Error uploading files:', err)
      } finally {
        // หน่วงเวลาให้ loading แสดงนาน 8 วินาที
        await new Promise(resolve => setTimeout(resolve, 8000))
        this.loading = false
      }
    },
    handleFileChange(files: File | File[]) {
      // รวมไฟล์ใหม่เข้ากับไฟล์เก่า และกรองไม่ให้ซ้ำ
      const fileArray = Array.isArray(files) ? files : [files]
      const existingNames = this.nameInput.map((f: File) => f.name)
      const newFiles = fileArray.filter(file => !existingNames.includes(file.name))
      newFiles.forEach((file: any) => {
        file.create = false // กำหนดค่า create เป็น false สำหรับไฟล์ใหม่
      })
      this.nameInput = [...this.nameInput, ...newFiles]
      console.log('Selected files:', this.nameInput)
    },
    onDrop(event: any) {
      if (event.dataTransfer) {
        const droppedFiles = event.dataTransfer.files
        if (droppedFiles.length) {
          this.files = Array.from(droppedFiles)
          this.nameInput = this.files
        }
      }
    },
    triggerFileInput() {
      // Trigger v-file-input
      if(!this.readonly){
      ;(this.$refs.fileInput as any).$el.querySelector('input').click()
      }
    },
    clearFileName() {
      this.nameInput = [] // ลบค่าชื่อไฟล์
      this.StepperValue = ['1']
    },
    removeImage(index: number) {
      if (this.nameInput[index].create) {
        client.deleteActivityPlanAttachment(this.nameInput[index].id)
      }
      this.nameInput.splice(index, 1)
    },
  },
})
</script>

<style scoped>
</style>
