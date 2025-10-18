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
            รายละเอียดพนักงาน
          </span>

          <v-row class="px-5 mt-3">
            <!-- Profile Image -->
            <v-col cols="12" class="text-center">
              <v-avatar
                v-if="employee.imageProfile"
                size="120"
                class="profile-avatar-large mb-3"
              >
                <v-img
                  :src="employee.imageProfile"
                  :alt="getFullName()"
                ></v-img>
              </v-avatar>
              <v-avatar
                v-else
                size="120"
                color="primary"
                class="profile-avatar-large mb-3"
              >
                <span class="text-h3 text-white">{{ getInitials() }}</span>
              </v-avatar>
            </v-col>

            <!-- User ID -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">User ID</label>
              <v-text-field
                v-model="employee.userId"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Full Name -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">ชื่อ-นามสกุล</label>
              <v-text-field
                :model-value="getFullName()"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Email -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">อีเมล</label>
              <v-text-field
                v-model="employee.email"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Position -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">ตำแหน่ง</label>
              <v-text-field
                :model-value="employee.position || '-'"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Phone -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">เบอร์โทร</label>
              <v-text-field
                :model-value="employee.phone || '-'"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Department -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">แผนก</label>
              <v-text-field
                :model-value="getDepartmentName()"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Active Status -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">สถานะการใช้งาน</label>
              <div>
                <v-chip
                  :color="employee.isActive ? 'success' : 'error'"
                  variant="flat"
                  size="small"
                >
                  {{ employee.isActive ? 'ใช้งาน' : 'ไม่ใช้งาน' }}
                </v-chip>
              </div>
            </v-col>

            <!-- Subscription -->
            <v-col cols="12">
              <label class="mb-2 font-weight-bold">การแจ้งเตือน</label>
              <div>
                <v-chip
                  :color="employee.subscription ? 'success' : 'default'"
                  variant="flat"
                  size="small"
                >
                  {{ employee.subscription ? 'เปิดใช้งาน' : 'ปิดใช้งาน' }}
                </v-chip>
              </div>
            </v-col>

            <!-- Roles -->
            <v-col cols="12" v-if="employee.roles">
              <label class="mb-2 font-weight-bold">บทบาท</label>
              <v-text-field
                v-model="employee.roles"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Group -->
            <v-col cols="12" v-if="employee.group">
              <label class="mb-2 font-weight-bold">กลุ่ม</label>
              <v-text-field
                v-model="employee.group"
                variant="outlined"
                density="comfortable"
                readonly
                :loading="loading"
              ></v-text-field>
            </v-col>

            <!-- Created Date -->
            <v-col cols="12" v-if="employee.created">
              <label class="mb-2 font-weight-bold">วันที่สร้าง</label>
              <v-text-field
                :model-value="formatDate(employee.created)"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <!-- Created By -->
            <v-col cols="12" v-if="employee.createdBy">
              <label class="mb-2 font-weight-bold">ผู้สร้าง</label>
              <v-text-field
                v-model="employee.createdBy"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <!-- Last Modified Date -->
            <v-col cols="12" v-if="employee.lastModified">
              <label class="mb-2 font-weight-bold">วันที่แก้ไขล่าสุด</label>
              <v-text-field
                :model-value="formatDate(employee.lastModified)"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <!-- Last Modified By -->
            <v-col cols="12" v-if="employee.lastModifiedBy">
              <label class="mb-2 font-weight-bold">ผู้แก้ไขล่าสุด</label>
              <v-text-field
                v-model="employee.lastModifiedBy"
                variant="outlined"
                density="comfortable"
                readonly
              ></v-text-field>
            </v-col>

            <!-- Close Button -->
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
import { Client, EmployeeDto, DepartmentDto } from '@/client'
import { BACKEND_API_URL } from '@/constants'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'EmployeeDetail',
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
      employee: new EmployeeDto(),
      departments: [] as DepartmentDto[],
      loading: false,
      sweetAlert: useSweetAlertStore(),
    }
  },
  mounted() {
    this.loadEmployee()
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
    getFullName(): string {
      const parts = []
      if (this.employee.titleName) parts.push(this.employee.titleName)
      if (this.employee.firstName) parts.push(this.employee.firstName)
      if (this.employee.lastName) parts.push(this.employee.lastName)
      return parts.length > 0 ? parts.join(' ') : '-'
    },
    getInitials(): string {
      const firstName = this.employee.firstName || ''
      const lastName = this.employee.lastName || ''
      return (firstName.charAt(0) + lastName.charAt(0)).toUpperCase() || '?'
    },
    getDepartmentName(): string {
      if (!this.employee.departmentId) return '-'
      const department = this.departments.find(
        (dept: DepartmentDto) => dept.id === this.employee.departmentId
      )
      return department?.name || '-'
    },
    async loadEmployee() {
      try {
        this.loading = true
        
        // Load departments first
        const departmentsResponse = await client.getDepartmentQuery()
        this.departments = departmentsResponse
        
        // Load employee data
        const response = await client.getEmployeeQueryByID(this.id)
        
        if (response) {
          this.employee = response
        }
      } catch (error: any) {
        console.error('Error loading employee:', error)
        
        if (error.status === 404) {
          this.sweetAlert.error('ไม่พบข้อมูลพนักงานที่ต้องการดู')
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

.profile-avatar-large {
  border: 3px solid rgba(var(--v-theme-primary), 0.3);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}
</style>
