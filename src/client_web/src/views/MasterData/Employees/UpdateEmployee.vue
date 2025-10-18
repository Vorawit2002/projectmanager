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
            แก้ไขข้อมูลพนักงาน
          </span>

          <v-form
            ref="form"
            @submit.prevent="save"
            class="text-black"
          >
            <v-row class="px-5 mt-3">
              <!-- User ID -->
              <v-col cols="12">
                <label class="mb-2">
                  User ID <span class="text-error">*</span>
                </label>
                <v-text-field
                  v-model="updateCommand.userId"
                  placeholder="ระบุ User ID"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="userIdRules"
                ></v-text-field>
              </v-col>

              <!-- Title Name -->
              <v-col cols="12" md="4">
                <label class="mb-2">คำนำหน้าชื่อ</label>
                <v-select
                  v-model="updateCommand.titleName"
                  :items="titleOptions"
                  placeholder="เลือกคำนำหน้า"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  clearable
                ></v-select>
              </v-col>

              <!-- First Name -->
              <v-col cols="12" md="4">
                <label class="mb-2">
                  ชื่อ <span class="text-error">*</span>
                </label>
                <v-text-field
                  v-model="updateCommand.firstName"
                  placeholder="ระบุชื่อ"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="firstNameRules"
                ></v-text-field>
              </v-col>

              <!-- Last Name -->
              <v-col cols="12" md="4">
                <label class="mb-2">
                  นามสกุล <span class="text-error">*</span>
                </label>
                <v-text-field
                  v-model="updateCommand.lastName"
                  placeholder="ระบุนามสกุล"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="lastNameRules"
                ></v-text-field>
              </v-col>

              <!-- Email -->
              <v-col cols="12">
                <label class="mb-2">
                  อีเมล <span class="text-error">*</span>
                </label>
                <v-text-field
                  v-model="updateCommand.email"
                  placeholder="ระบุอีเมล"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="emailRules"
                ></v-text-field>
              </v-col>

              <!-- Position -->
              <v-col cols="12">
                <label class="mb-2">ตำแหน่ง</label>
                <v-text-field
                  v-model="updateCommand.position"
                  placeholder="ระบุตำแหน่ง"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                ></v-text-field>
              </v-col>

              <!-- Phone -->
              <v-col cols="12">
                <label class="mb-2">เบอร์โทร</label>
                <v-text-field
                  v-model="updateCommand.phone"
                  placeholder="ระบุเบอร์โทร (10 หลัก)"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="phoneRules"
                ></v-text-field>
              </v-col>

              <!-- Department -->
              <v-col cols="12">
                <label class="mb-2">แผนก</label>
                <v-select
                  v-model="updateCommand.departmentId"
                  :items="departments"
                  item-title="name"
                  item-value="id"
                  placeholder="เลือกแผนก"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  clearable
                  :loading="loadingDepartments"
                  :no-data-text="departments.length === 0 ? 'กรุณาสร้างแผนกก่อน' : 'ไม่พบข้อมูล'"
                ></v-select>
              </v-col>

              <!-- Image Profile -->
              <v-col cols="12">
                <label class="mb-2">รูปโปรไฟล์</label>
                <v-file-input
                  v-model="imageFile"
                  @change="handleImageUpload"
                  accept="image/jpeg,image/png,image/gif"
                  placeholder="เลือกรูปภาพใหม่"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  prepend-icon=""
                  prepend-inner-icon="ri-image-line"
                  :rules="imageRules"
                  show-size
                  clearable
                ></v-file-input>
                <div v-if="imagePreview || updateCommand.imageProfile" class="mt-2 text-center">
                  <v-img
                    :src="imagePreview || updateCommand.imageProfile"
                    max-width="200"
                    max-height="200"
                    class="mx-auto rounded"
                  ></v-img>
                  <div v-if="!imagePreview && updateCommand.imageProfile" class="text-caption mt-1">
                    รูปภาพปัจจุบัน
                  </div>
                </div>
                <div v-if="imageError" class="text-error mt-1">
                  {{ imageError }}
                </div>
              </v-col>

              <!-- Roles -->
              <v-col cols="12">
                <label class="mb-2">บทบาท</label>
                <v-text-field
                  v-model="updateCommand.roles"
                  placeholder="ระบุบทบาท"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                ></v-text-field>
              </v-col>

              <!-- Group -->
              <v-col cols="12">
                <label class="mb-2">กลุ่ม</label>
                <v-text-field
                  v-model="updateCommand.group"
                  placeholder="ระบุกลุ่ม"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                ></v-text-field>
              </v-col>

              <!-- Checkboxes -->
              <v-col cols="12">
                <v-checkbox
                  v-model="updateCommand.isActive"
                  label="สถานะการใช้งาน"
                  color="primary"
                  density="comfortable"
                ></v-checkbox>
              </v-col>

              <v-col cols="12">
                <v-checkbox
                  v-model="updateCommand.subscription"
                  label="การแจ้งเตือน"
                  color="primary"
                  density="comfortable"
                ></v-checkbox>
              </v-col>

              <!-- Action Buttons -->
              <v-col
                cols="12"
                class="d-flex justify-center"
              >
                <v-btn
                  class="mr-4"
                  rounded="lg"
                  color="error"
                  @click="cancel"
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
import { Client, DepartmentDto, EmployeeDto } from '@/client'
import { BACKEND_API_URL } from '@/constants'

const client = new Client(BACKEND_API_URL)

interface UpdateEmployeeCommand {
  id: string
  userId: string
  titleName?: string
  firstName: string
  lastName: string
  email: string
  position?: string
  phone?: string
  imageProfile?: string
  isActive: boolean
  departmentId?: string
  subscription: boolean
  roles?: string
  group?: string
}

export default defineComponent({
  name: 'UpdateEmployee',
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
      updateCommand: {
        id: '',
        userId: '',
        titleName: undefined,
        firstName: '',
        lastName: '',
        email: '',
        position: undefined,
        phone: undefined,
        imageProfile: undefined,
        isActive: true,
        departmentId: undefined,
        subscription: false,
        roles: undefined,
        group: undefined,
      } as UpdateEmployeeCommand,
      loading: false,
      loadingDepartments: false,
      loadingEmployee: false,
      sweetAlert: useSweetAlertStore(),
      departments: [] as DepartmentDto[],
      imageFile: null as File[] | null,
      imagePreview: null as string | null,
      imageError: null as string | null,
      titleOptions: ['นาย', 'นาง', 'นางสาว', 'ดร.', 'ผศ.', 'รศ.', 'ศ.'],
      userIdRules: [
        (v: string) => !!v || 'กรุณาระบุ User ID',
      ],
      firstNameRules: [
        (v: string) => !!v || 'กรุณาระบุชื่อ',
        (v: string) => (v && v.length >= 2) || 'ชื่อต้องมีอย่างน้อย 2 ตัวอักษร',
      ],
      lastNameRules: [
        (v: string) => !!v || 'กรุณาระบุนามสกุล',
        (v: string) => (v && v.length >= 2) || 'นามสกุลต้องมีอย่างน้อย 2 ตัวอักษร',
      ],
      emailRules: [
        (v: string) => !!v || 'กรุณาระบุอีเมล',
        (v: string) => /.+@.+\..+/.test(v) || 'รูปแบบอีเมลไม่ถูกต้อง',
      ],
      phoneRules: [
        (v: string) => !v || v.length === 0 || /^\d{10}$/.test(v) || 'เบอร์โทรต้องเป็นตัวเลข 10 หลัก',
      ],
      imageRules: [
        (files: File[]) => {
          if (!files || files.length === 0) return true
          const file = files[0]
          const validTypes = ['image/jpeg', 'image/png', 'image/gif']
          if (!validTypes.includes(file.type)) {
            return 'ประเภทไฟล์ต้องเป็น jpg, png หรือ gif เท่านั้น'
          }
          const maxSize = 5 * 1024 * 1024 // 5MB
          if (file.size > maxSize) {
            return 'ขนาดไฟล์ต้องไม่เกิน 5MB'
          }
          return true
        },
      ],
    }
  },
  async mounted() {
    await this.loadDepartments()
    await this.loadEmployee()
  },
  methods: {
    async loadDepartments() {
      try {
        this.loadingDepartments = true
        const response = await client.getDepartmentQuery()
        this.departments = response
          .filter((dept: DepartmentDto) => dept.isActive)
          .sort((a: DepartmentDto, b: DepartmentDto) => 
            (a.name || '').localeCompare(b.name || '', 'th')
          )
      } catch (error) {
        console.error('Error loading departments:', error)
        this.sweetAlert.error('เกิดข้อผิดพลาดในการโหลดข้อมูลแผนก')
      } finally {
        this.loadingDepartments = false
      }
    },
    async loadEmployee() {
      try {
        this.loadingEmployee = true
        const response = await client.getEmployeeQueryByID(this.id)
        
        if (response) {
          this.updateCommand = {
            id: response.id || '',
            userId: response.userId || '',
            titleName: response.titleName,
            firstName: response.firstName || '',
            lastName: response.lastName || '',
            email: response.email || '',
            position: response.position,
            phone: response.phone,
            imageProfile: response.imageProfile,
            isActive: response.isActive ?? true,
            departmentId: response.departmentId,
            subscription: response.subscription ?? false,
            roles: response.roles,
            group: response.group,
          }
        }
      } catch (error: any) {
        console.error('Error loading employee:', error)
        
        if (error.status === 404) {
          this.sweetAlert.error('ไม่พบข้อมูลพนักงาน')
        } else {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการโหลดข้อมูลพนักงาน')
        }
        
        this.CloseDialogEdit(false, false)
      } finally {
        this.loadingEmployee = false
      }
    },
    handleImageUpload() {
      this.imageError = null
      this.imagePreview = null

      if (!this.imageFile || this.imageFile.length === 0) {
        return
      }

      const file = this.imageFile[0]
      
      // Validate file type
      const validTypes = ['image/jpeg', 'image/png', 'image/gif']
      if (!validTypes.includes(file.type)) {
        this.imageError = 'ประเภทไฟล์ต้องเป็น jpg, png หรือ gif เท่านั้น'
        this.imageFile = null
        return
      }

      // Validate file size (5MB)
      const maxSize = 5 * 1024 * 1024
      if (file.size > maxSize) {
        this.imageError = 'ขนาดไฟล์ต้องไม่เกิน 5MB'
        this.imageFile = null
        return
      }

      // Preview image
      this.previewImage(file)
    },
    previewImage(file: File) {
      const reader = new FileReader()
      reader.onload = (e) => {
        this.imagePreview = e.target?.result as string
      }
      reader.readAsDataURL(file)
    },
    cancel() {
      this.CloseDialogEdit(false, false)
    },
    async save() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      
      if (valid) {
        try {
          this.loading = true

          // Handle image upload if new file is selected
          if (this.imageFile && this.imageFile.length > 0) {
            try {
              const uploadResponse = await client.postApiUsersProfileImage({
                data: this.imageFile[0],
                fileName: this.imageFile[0].name,
              })
              
              if (uploadResponse && uploadResponse.imageUrl) {
                this.updateCommand.imageProfile = uploadResponse.imageUrl
              }
            } catch (uploadError) {
              console.error('Error uploading image:', uploadError)
              this.sweetAlert.error('เกิดข้อผิดพลาดในการอัพโหลดรูปภาพ')
              this.loading = false
              return
            }
          }

          const response = await client.updateEmployee(this.updateCommand as any)
          
          if (response) {
            this.sweetAlert.success('บันทึกข้อมูลสำเร็จ')
            this.CloseDialogEdit(false, true)
          }
        } catch (error: any) {
          console.error('Error updating employee:', error)
          
          if (error.status === 401) {
            this.sweetAlert.error('คุณไม่มีสิทธิ์ในการทำรายการนี้')
          } else if (error.status === 400) {
            this.sweetAlert.error('ข้อมูลไม่ถูกต้อง กรุณาตรวจสอบอีกครั้ง')
          } else if (error.status === 404) {
            this.sweetAlert.error('ไม่พบข้อมูลพนักงาน')
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
