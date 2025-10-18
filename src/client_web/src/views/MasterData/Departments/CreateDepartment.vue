<template>
  <div class="page-container scroll-content dialog-scrollbar">
    <div class="pa-4">
      <span class="text-sub-title">เพิ่มข้อมูลแผนก</span>
      <v-form
        ref="form"
        @submit.prevent="save"
        class="text-black"
      >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <!-- ชื่อแผนก -->
              <v-col
                cols="12"
                md="12"
              >
                <label class="form-label">ชื่อแผนก <span class="text-error">*</span></label>
                <v-text-field
                  v-model="createCommand.name"
                  placeholder="ระบุชื่อแผนก"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  :rules="nameRules"
                ></v-text-field>
              </v-col>

              <!-- สถานะการใช้งาน -->
              <v-col
                cols="12"
                md="12"
              >
                <label class="form-label">สถานะ</label>
                <v-checkbox
                  v-model="createCommand.isActive"
                  label="สถานะการใช้งาน"
                  color="primary"
                  density="comfortable"
                  hide-details
                ></v-checkbox>
              </v-col>

              <!-- ปุ่มควบคุม -->
              <v-col
                cols="12"
                class="pt-6"
              >
                <div class="button-container">
                  <v-btn
                    class="mobile-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="cancel"
                    :disabled="loading"
                    block
                  >
                    <v-icon
                      icon="ri-close-line"
                      class="mr-2"
                    ></v-icon>
                    ยกเลิก
                  </v-btn>
                  <v-btn
                    class="mobile-btn submit-btn"
                    rounded="lg"
                    color="success-darken-2"
                    type="submit"
                    :loading="loading"
                    :disabled="loading"
                    block
                  >
                    <v-icon class="mr-2">ri-save-3-fill</v-icon>
                    บันทึก
                  </v-btn>
                </div>
              </v-col>
            </v-row>
          </v-form>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useSweetAlertStore } from '@/stores'
import { Client, CreateDepartmentCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CreateDepartment',
  props: {
    CloseDialogCreate: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      createCommand: new CreateDepartmentCommand(),
      loading: false,
      sweetAlert: useSweetAlertStore(),
      nameRules: [
        (v: string) => !!v || 'กรุณาระบุชื่อแผนก',
        (v: string) => (v && v.length >= 2) || 'ชื่อแผนกต้องมีอย่างน้อย 2 ตัวอักษร',
        (v: string) => (v && v.length <= 100) || 'ชื่อแผนกต้องไม่เกิน 100 ตัวอักษร',
      ],
    }
  },
  mounted() {
    this.resetForm()
  },
  methods: {
    cancel() {
      this.CloseDialogCreate(false, false)
    },
    resetForm() {
      this.createCommand = new CreateDepartmentCommand()
      this.createCommand.isActive = true
      const form = this.$refs.form as any
      if (form) {
        form.resetValidation()
      }
    },
    async save() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      
      if (valid) {
        try {
          this.loading = true
          const response = await client.createDepartment(this.createCommand)
          
          if (response) {
            this.sweetAlert.success('บันทึกข้อมูลสำเร็จ')
            this.CloseDialogCreate(false, true, JSON.stringify(this.createCommand))
          }
        } catch (error: any) {
          console.error('Error creating department:', error)
          
          if (error.status === 401) {
            this.sweetAlert.error('คุณไม่มีสิทธิ์ในการทำรายการนี้')
          } else if (error.status === 400) {
            this.sweetAlert.error('ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง')
          } else {
            this.sweetAlert.error('เกิดข้อผิดพลาดในการบันทึกข้อมูล')
          }
        } finally {
          this.loading = false
        }
      }
    },
  },
})
</script>

<style scoped>
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

.page-container {
  width: 100%;
  min-height: 100vh;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 16px 0 32px 0;
  scroll-behavior: smooth;
  position: relative;
}

.text-black {
  color: #000 !important;
}

.form-label {
  display: block;
  margin-bottom: 8px;
  font-weight: 500;
  color: #2c3e50;
}

.button-container {
  display: flex;
  gap: 20px;
  justify-content: center;
  flex-wrap: wrap;
  max-width: 500px;
  margin: 0 auto;
}

.mobile-btn {
  min-width: 200px;
  min-height: 52px;
  font-size: 16px;
  font-weight: 600;
  border: none !important;
  border-radius: 16px;
  box-shadow: 
    0 4px 16px rgba(0, 0, 0, 0.1),
    0 1px 4px rgba(0, 0, 0, 0.05);
  text-transform: none;
  letter-spacing: 0.025em;
}

.mobile-btn .v-icon {
  font-size: 20px;
}

.mobile-btn:disabled {
  opacity: 0.5 !important;
  pointer-events: none !important;
}
</style>
