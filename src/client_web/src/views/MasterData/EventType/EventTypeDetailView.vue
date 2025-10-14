<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="page-container scroll-content dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title"> รายละเอียดประเภทกิจกรรม</span>

          <v-form
            ref="form"
            @submit.prevent="UpdateEventType"
            class="text-black"
          >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">ชื่อ</label>
                <v-text-field
                  v-model="UpdateCommand.name"
                  placeholder="ระบุLineID"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  readonly
                  dense
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="12"
              ></v-col>

              <v-col
                cols="12"
                class="pt-6"
              >
                <div class="button-container">
                  <v-btn
                    class="mobile-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="closeDialog"
                    block
                  >
                    <v-icon
                      icon="ri-close-line"
                      class="mr-2"
                    ></v-icon>
                    ปิด
                  </v-btn>
                </div>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { Client, UpdateEventTypeCommand } from '@/client'
import { defineComponent } from 'vue'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'UpdateEventTypeDetailView',
  props: {
    id: {
      type: String,
      required: true,
    },
    CloseDialogEdit: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      UpdateCommand: new UpdateEventTypeCommand() as any,
      loading: false,
      sweetAlertStore: useSweetAlertStore(),
      OrganizationList: [] as any,
      files: [] as any, // สำหรับเก็บไฟล์ที่อัปโหลด
    }
  },
  mounted() {
    this.initialize()
  },
  watch: {
    id() {
      this.initialize()
    },
  },

  methods: {
    async initialize() {
      try {
        // สมมุติว่ามี client.getEventTypeById (ถ้าไม่มีต้องสร้างใน client)
        const response = await client.getEventTypeQueryByID(this.id)
        if (response) {
          this.UpdateCommand = response as UpdateEventTypeCommand
          if (this.UpdateCommand.attachmentId) {
            const result = await client.getAttachmentQueryById(this.UpdateCommand.attachmentId)
            this.files.name = result.nameFile
          }
        }
      } catch (error) {
        console.error(error)
      }
    },
    closeDialog(reload: boolean = false) {
      this.CloseDialogEdit(false, reload) // ปิด dialog
    },
    async UpdateEventType() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (!valid) return
      this.loading = true
      try {
        const response = await client.updateEventType(this.UpdateCommand)
        if (response) {
          this.sweetAlertStore.success('แก้ไขข้อมูลสำเร็จ')
          this.closeDialog(true)
        }
      } catch (error) {
        console.error(error)
        this.sweetAlertStore.error('เกิดข้อผิดพลาดในการแก้ไขข้อมูล ล้มเหลว!')
        this.closeDialog()
      } finally {
        this.loading = false
      }
    },
    triggerFileInput() {
      const fileInput = this.$refs.fileInput as HTMLInputElement
      if (fileInput) {
        fileInput.click() // เปิด dialog สำหรับเลือกไฟล์
      }
    },
    UploadFile(event: any) {
      const file = event.target.files[0]
      this.files = file // เก็บไฟล์ที่เลือกไว้ในตัวแปร files
      if (file) {
        const reader = new FileReader()
        reader.onload = (e: any) => {
          this.UpdateCommand.fileName = file.name // เก็บข้อมูล Base64 ของรูปภาพ
          this.UpdateCommand.base64 = e.target.result.split(',')[1] // เก็บชื่อไฟล์
        }
        reader.readAsDataURL(file) // อ่านไฟล์เป็น Base64
      }
    },
    onClearFile() {
      this.files = []
      this.UpdateCommand.fileName = ''
      this.UpdateCommand.base64 = ''
    },
  },
})
</script>

<style scoped>
/* Enhanced Image Card */
.imgCard {
  border-radius: 20px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12), 0 2px 8px rgba(0, 0, 0, 0.08);
  border: 3px solid rgba(255, 255, 255, 0.2);
  overflow: hidden;
}

.page-container {
  width: 100%;
  min-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 16px 0 32px 0;
  scroll-behavior: smooth;
  position: relative;
}



/* Enhanced Typography */
.text-title {
  font-size: 25px;
  font-weight: 700;
  color: #2b3086;
  text-align: center;
  margin-bottom: 16px;
}

.text-sub-title {
  font-size: clamp(20px, 4vw, 28px);
  font-weight: 700;
  color: #2b3086;
  background: linear-gradient(135deg, #2b3086 0%, #667eea 100%);
  background-clip: text;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  text-align: center;
  display: block;
  margin: 0 0 24px 0;
  padding: 0;
  line-height: 1.5;
  letter-spacing: -0.02em;
}

.text-black {
  color: #000 !important;
}

/* Enhanced Labels */
label {
  font-size: 15px;
  font-weight: 500;
  color: #333;
  margin-bottom: 8px;
  display: block;
  letter-spacing: 0.025em;
}

.textarea-field :deep(.v-field) {
  border-radius: 20px;
}

.textarea-field :deep(.v-field__input) {
  padding: 20px;
}

.checkbox-field :deep(.v-selection-control) {
  min-height: 32px;
}

.checkbox-field :deep(.v-selection-control__wrapper) {
  border-radius: 8px;
}

.add-btn {
  opacity: 0.7;
}

/* Enhanced Upload Container */
.upload-container {
  background: rgba(255, 255, 255, 0.9);
  border: 2px solid #e8eaed;
  border-radius: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.04);
  backdrop-filter: blur(5px);
}

/* Enhanced Button Container */
.button-container {
  display: flex;
  gap: 20px;
  justify-content: center;
  flex-wrap: wrap;
  max-width: 500px;
  margin: 0 auto;
}

.mobile-btn {
  min-width: 300px;
  min-height: 52px;
  font-size: 16px;
  font-weight: 600;
  border: none !important;
  border-radius: 16px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1), 0 1px 4px rgba(0, 0, 0, 0.05);
  text-transform: none;
  letter-spacing: 0.025em;
  position: relative;
  overflow: hidden;
}

.mobile-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
}

.mobile-btn .v-icon {
  font-size: 20px;
}

@media (max-width: 600px) {
  .text-sub-title {
    font-size: 30px;
    margin-bottom: 20px;
  }
}

@media (min-width: 600px) {
  .v-col[cols='0'][md='6'] {
    display: block;
    flex-basis: 50%;
    max-width: 50%;
  }
}
</style>
