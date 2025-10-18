<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="elevation-10 mb-4 pa-3 card-Dialog dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title">
            <v-icon icon="ri-add-circle-line" class="mr-2" />
            เพิ่มข้อมูลพนักงาน
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
                  v-model="createCommand.userId"
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
                  v-model="createCommand.titleName"
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
                  v-model="createCommand.firstName"
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
                  v-model="createCommand.lastName"
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
                  v-model="createCommand.email"
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
                  v-model="createCommand.position"
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
                  v-model="createCommand.phone"
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
                  v-model="createCommand.departmentId"
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
                  placeholder="เลือกรูปภาพ"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  prepend-icon=""
                  prepend-inner-icon="ri-image-line"
                  :rules="imageRules"
                  show-size
                  clearable
                ></v-file-input>
                <div v-if="imagePreview" class="mt-2 text-center">
                  <v-img
                    :src="imagePreview"
                    max-width="200"
                    max-height="200"
                    class="mx-auto rounded"
                  ></v-img>
                </div>
                <div v-if="imageError" class="text-error mt-1">
                  {{ imageError }}
                </div>
              </v-col>

              <!-- Roles -->
              <v-col cols="12">
                <label class="mb-2">บทบาท</label>
                <v-text-field
                  v-model="createCommand.roles"
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
                  v-model="createCommand.group"
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
                  v-model="createCommand.isActive"
                  label="สถานะการใช้งาน"
                  color="primary"
                  density="comfortable"
                ></v-checkbox>
              </v-col>

              <v-col cols="12">
                <v-checkbox
                  v-model="createCommand.subscription"
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
import { Client, DepartmentDto } from '@/client'
import { BACKEND_API_URL } from '@/constants'

const client = new Client(BACKEND_API_URL)

interface CreateEmployeeCommand {
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
  name: 'CreateEmployee',
  props: {
    CloseDialogCreate: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      createCommand: {
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
      } as CreateEmployeeCommand,
      loading: false,
      loadingDepartments: false,
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
    this.resetForm()
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
    handleImageUpload() {
      this.imageError = null
      this.imagePreview = null

      if (!this.imageFile || this.imageFile.length === 0) {
        this.createCommand.imageProfile = undefined
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
      this.CloseDialogCreate(false, false)
    },
    resetForm() {
      this.createCommand = {
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
      }
      this.imageFile = null
      this.imagePreview = null
      this.imageError = null
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

          // Handle image upload if file is selected
          if (this.imageFile && this.imageFile.length > 0) {
            try {
              const formData = new FormData()
              formData.append('imageFile', this.imageFile[0])
              
              const uploadResponse = await client.postApiUsersProfileImage({
                data: this.imageFile[0],
                fileName: this.imageFile[0].name,
              })
              
              if (uploadResponse && uploadResponse.imageUrl) {
                this.createCommand.imageProfile = uploadResponse.imageUrl
              }
            } catch (uploadError) {
              console.error('Error uploading image:', uploadError)
              this.sweetAlert.error('เกิดข้อผิดพลาดในการอัพโหลดรูปภาพ')
              this.loading = false
              return
            }
          }

          const response = await client.createEmployee(this.createCommand as any)
          
          if (response) {
            this.sweetAlert.success('บันทึกข้อมูลสำเร็จ')
            this.CloseDialogCreate(false, true, JSON.stringify(this.createCommand))
          }
        } catch (error: any) {
          console.error('Error creating employee:', error)
          
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
