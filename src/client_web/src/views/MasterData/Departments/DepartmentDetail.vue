<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="elevation-10 mb-4 pa-3 card-Dialog dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title">
            <v-icon icon="ri-article-line" class="mr-2" />
            รายละเอียดแผนก
          </span>

          <v-row class="px-5 mt-3">
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">ชื่อแผนก</label>
              <v-text-field
                v-model="department.name"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <v-col cols="12">
              <label class="mb-2 font-weight-bold">สถานะการใช้งาน</label>
              <v-chip
                :color="department.isActive ? 'success' : 'error'"
                variant="flat"
                size="small"
              >
                {{ department.isActive ? 'ใช้งาน' : 'ไม่ใช้งาน' }}
              </v-chip>
            </v-col>

            <v-col cols="12" v-if="department.created">
              <label class="mb-2 font-weight-bold">วันที่สร้าง</label>
              <v-text-field
                :model-value="formatDate(department.created)"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <v-col cols="12" v-if="department.createdBy">
              <label class="mb-2 font-weight-bold">ผู้สร้าง</label>
              <v-text-field
                v-model="department.createdBy"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <v-col cols="12" v-if="department.lastModified">
              <label class="mb-2 font-weight-bold">วันที่แก้ไขล่าสุด</label>
              <v-text-field
                :model-value="formatDate(department.lastModified)"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <v-col cols="12" v-if="department.lastModifiedBy">
              <label class="mb-2 font-weight-bold">ผู้แก้ไขล่าสุด</label>
              <v-text-field
                v-model="department.lastModifiedBy"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <v-col
              cols="12"
              class="d-flex justify-center"
            >
              <v-btn
                rounded="lg"
                color="primary"
                @click="close"
                :disabled="loading"
              >
                ปิด
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useSweetAlertStore } from '@/stores'
import { Client, DepartmentDto } from '@/client'
import { BACKEND_API_URL } from '@/constants'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'DepartmentDetail',
  props: {
    id: {
      type: String,
      required: true,
    },
    CloseDialogDetail: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      department: new DepartmentDto(),
      loading: false,
      sweetAlert: useSweetAlertStore(),
    }
  },
  mounted() {
    this.loadDepartment()
  },
  methods: {
    close() {
      this.CloseDialogDetail(false, false)
    },
    formatDate(date: Date | undefined): string {
      if (!date) return ''
      
      const d = new Date(date)
      const day = String(d.getDate()).padStart(2, '0')
      const month = String(d.getMonth() + 1).padStart(2, '0')
      const year = d.getFullYear() + 543 // Convert to Buddhist Era
      const hours = String(d.getHours()).padStart(2, '0')
      const minutes = String(d.getMinutes()).padStart(2, '0')
      
      return `${day}/${month}/${year} ${hours}:${minutes}`
    },
    async loadDepartment() {
      try {
        this.loading = true
        const response = await client.getDepartmentQueryByID(this.id)
        
        if (response) {
          this.department = response
        }
      } catch (error: any) {
        console.error('Error loading department:', error)
        
        if (error.status === 404) {
          this.sweetAlert.error('ไม่พบข้อมูลแผนกที่ต้องการดู')
          this.CloseDialogDetail(false, false)
        } else if (error.status === 401) {
          this.sweetAlert.error('คุณไม่มีสิทธิ์ในการเข้าถึงข้อมูลนี้')
          this.CloseDialogDetail(false, false)
        } else {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการโหลดข้อมูล')
        }
      } finally {
        this.loading = false
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
</style>
