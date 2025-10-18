<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="elevation-10 mb-4 pa-3 card-Dialog dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title">
            <v-icon icon="ri-edit-2-line" class="mr-2" />
            แก้ไขข้อมูลแผนก
          </span>

          <v-form
            ref="form"
            @submit.prevent="save"
            class="text-black"
          >
            <v-row class="px-5 mt-3">
              <v-col cols="12">
                <label class="mb-2">
                  ชื่อแผนก <span class="text-error">*</span>
                </label>
                <v-text-field
                  v-model="updateCommand.name"
                  placeholder="ระบุชื่อแผนก"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="nameRules"
                  :loading="loading"
                  :disabled="loading"
                ></v-text-field>
              </v-col>

              <v-col cols="12">
                <v-checkbox
                  v-model="updateCommand.isActive"
                  label="สถานะการใช้งาน"
                  color="primary"
                  density="comfortable"
                  :disabled="loading"
                ></v-checkbox>
              </v-col>

              <v-col
                cols="12"
                class="d-flex justify-center"
              >
                <v-btn
                  class="mr-4"
                  rounded="lg"
                  color="error"
                  @click="cancel"
                  :disabled="loading"
                >
                  ยกเลิก
                </v-btn>
                <v-btn
                  color="success-darken-2"
                  rounded="lg"
                  type="submit"
                  :loading="loading"
                >
                  บันทึก
                </v-btn>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useSweetAlertStore } from '@/stores'
import { Client, UpdateDepartmentCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'UpdateDepartment',
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
      updateCommand: new UpdateDepartmentCommand(),
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
    this.loadDepartment()
  },
  methods: {
    cancel() {
      this.CloseDialogEdit(false, false)
    },
    async loadDepartment() {
      try {
        this.loading = true
        const response = await client.getDepartmentQueryByID(this.id)
        
        if (response) {
          this.updateCommand.id = response.id!
          this.updateCommand.name = response.name!
          this.updateCommand.isActive = response.isActive!
        }
      } catch (error: any) {
        console.error('Error loading department:', error)
        
        if (error.status === 404) {
          this.sweetAlert.error('ไม่พบข้อมูลแผนกที่ต้องการแก้ไข')
          this.CloseDialogEdit(false, false)
        } else if (error.status === 401) {
          this.sweetAlert.error('คุณไม่มีสิทธิ์ในการเข้าถึงข้อมูลนี้')
          this.CloseDialogEdit(false, false)
        } else {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการโหลดข้อมูล')
        }
      } finally {
        this.loading = false
      }
    },
    async save() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      
      if (valid) {
        try {
          this.loading = true
          const response = await client.updateDepartment(this.updateCommand)
          
          if (response) {
            this.sweetAlert.success('อัพเดทข้อมูลสำเร็จ')
            this.CloseDialogEdit(false, true)
          }
        } catch (error: any) {
          console.error('Error updating department:', error)
          
          if (error.status === 401) {
            this.sweetAlert.error('คุณไม่มีสิทธิ์ในการทำรายการนี้')
          } else if (error.status === 400) {
            this.sweetAlert.error('ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง')
          } else if (error.status === 404) {
            this.sweetAlert.error('ไม่พบข้อมูลแผนกที่ต้องการแก้ไข')
          } else {
            this.sweetAlert.error('เกิดข้อผิดพลาดในการอัพเดทข้อมูล')
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
.card-Dialog {
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  border-radius: 12px;
}

.text-sub-title {
  font-size: 25px;
  font-weight: bold;
  color: #2b3086;
  background-clip: text;
  -webkit-background-clip: text;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
}

.text-black {
  color: #000 !important;
}
</style>
