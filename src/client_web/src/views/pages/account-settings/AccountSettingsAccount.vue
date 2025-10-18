<template>
  <VRow>
    <VCol cols="12">
      <VCard title="จัดการข้อมูลส่วนตัว">
        <VCardText class="d-flex">
          <!-- 👉 Avatar -->
          <VAvatar
            :key="currentImageProfile"
            rounded="lg"
            size="200"
            class="me-6 avatar-style"
            :image="imagePreview || currentImageProfile || avatar1"
          />

          <!-- 👉 Upload Photo -->
          <form class="d-flex flex-column justify-center gap-5">
            <div class="d-flex flex-wrap gap-2">
              <VBtn
                color="primary"
                @click="refInputEl?.click()"
                :loading="uploadingImage"
              >
                <VIcon
                  icon="ri-upload-cloud-line"
                  class="d-sm-none"
                />
                <span class="d-none d-sm-block">อัพโหลดรูปใหม่</span>
              </VBtn>

              <input
                ref="refInputEl"
                type="file"
                name="file"
                accept="image/jpeg,image/png,image/jpg,image/gif"
                hidden
                @change="changeAvatar"
              />

              <VBtn
                type="reset"
                color="error"
                variant="outlined"
                @click="resetAvatar"
                :disabled="!currentImageProfile"
              >
                <span class="d-none d-sm-block">รีเซ็ท</span>
                <VIcon
                  icon="ri-refresh-line"
                  class="d-sm-none"
                />
              </VBtn>
            </div>

            <p class="text-body-1 mb-0">รองรับไฟล์ JPG, GIF, PNG ขนาดไม่เกิน 5MB</p>
          </form>
        </VCardText>

        <VDivider />

        <VCardText>
          <!-- 👉 Form -->
          <VForm
            class="mt-6"
            ref="form"
            @submit.prevent="UpdateUser"
          >
            <VRow>
              <VCol
                md="2"
                cols="12"
              >
                <VSelect
                  v-model="accountData.titleName"
                  :items="titleOptions"
                  label="คำนำหน้า"
                  variant="outlined"
                  density="comfortable"
                  color="primary"
                  :rules="[(value:any)=> !!value|| 'กรุณาเลือกคำนำหน้า']"
                  clearable
                />
              </VCol>
              <!-- 👉 First Name -->
              <VCol
                md="5"
                cols="12"
              >
                <VTextField
                  v-model="accountData.firstName"
                  label="ชื่อจริง"
                  :rules="[(value:any)=> !!value|| 'กรุณากรอกชื่อจริง']"
                />
              </VCol>

              <!-- 👉 Last Name -->
              <VCol
                md="5"
                cols="12"
              >
                <VTextField
                  v-model="accountData.lastName"
                  label="นามสกุล"
                  :rules="[(value:any)=> !!value|| 'กรุณากรอกนามสกุล']"
                />
              </VCol>

              <!-- 👉 Department -->
              <VCol
                cols="12"
                md="6"
              >
                <v-autocomplete
                  :items="Departments"
                  item-title="name"
                  item-value="id"
                  v-model="accountData.departmentId"
                  label="แผนก"
                  :rules="[(value:any)=> !!value|| 'กรุณาเลือกแผนก']"
                >
                </v-autocomplete>
              </VCol>
              <!-- 👉 Position -->
              <VCol
                cols="12"
                md="6"
              >
                <VTextField
                  v-model="accountData.position"
                  label="ตำแหน่ง"
                  :rules="[(value:any)=> !!value|| 'กรุณากรอกตำแหน่ง']"
                />
              </VCol>

              <!-- 👉 Email -->
              <VCol
                cols="12"
                md="6"
              >
                <VTextField
                  v-model="accountData.email"
                  label="อีเมล"
                  :rules="[value => !!value || 'กรุณากรอกอีเมล']"
                  type="อีเมล"
                />
              </VCol>

              <!-- 👉 Phone -->
              <VCol
                cols="12"
                md="6"
              >
                <VTextField
                  v-model="accountData.phone"
                  label="เบอร์โทรศัพท์"
                  placeholder="061xxx1122"
                />
              </VCol>

              <!-- 👉 Form Actions -->
              <VCol
                cols="12"
                class="d-flex flex-wrap gap-4"
              >
                <VBtn
                  type="submit"
                  :loading="loading"
                  >บันทึกการเปลี่ยนแปลง</VBtn
                >
                <!-- <VBtn
                  color="error"
                  variant="outlined"
                  type="reset"
                  @click.prevent="resetForm()"
                >
                  รีเซ็ท
                </VBtn> -->
              </VCol>
            </VRow>
          </VForm>
        </VCardText>
      </VCard>
    </VCol>
  </VRow>
</template>

<script lang="ts">
import { Client, UpdateAccountSettingsCommand, AccountSettingsDto } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import avatar1 from '@images/avatars/avatar-1.png'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  data() {
    return {
      accountData: new UpdateAccountSettingsCommand(),
      currentImageProfile: '' as string,
      refInputEl: null as HTMLElement | null,
      isAccountDeactivated: false,
      auth: useAuthStore(),
      avatar1,
      Departments: [] as any,
      sweetAlertStore: useSweetAlertStore(),
      loading: false,
      uploadingImage: false,
      imagePreview: null as string | null,
      selectedFile: null as File | null,
      titleOptions: [
        { title: 'นาย', value: 'นาย' },
        { title: 'นาง', value: 'นาง' },
        { title: 'นางสาว', value: 'นางสาว' },
        { title: 'ดร.', value: 'ดร.' },
        { title: 'ผศ.', value: 'ผศ.' },
        { title: 'รศ.', value: 'รศ.' },
        { title: 'ศ.', value: 'ศ.' },
        { title: 'Mr.', value: 'Mr.' },
        { title: 'Mrs.', value: 'Mrs.' },
        { title: 'Miss', value: 'Miss' },
        { title: 'Ms.', value: 'Ms.' },
        { title: 'Dr.', value: 'Dr.' },
      ],
    }
  },
  async mounted() {
    this.refInputEl = this.$refs.refInputEl as HTMLElement
    await this.initialize()
    await this.getDepartments()
  },
  methods: {
    async initialize() {
      try {
        const response = await client.getAccountSettings()
        if (response) {
          this.accountData.firstName = response.firstName
          this.accountData.lastName = response.lastName
          this.accountData.titleName = response.titleName
          this.accountData.position = response.position
          this.accountData.phone = response.phone
          this.accountData.email = response.email
          this.accountData.departmentId = response.departmentId
          this.currentImageProfile = response.imageProfile || ''
        }
      } catch (error) {
        console.error('Error initializing account settings:', error)
        this.sweetAlertStore.errorDeleted('ไม่สามารถโหลดข้อมูลบัญชีได้')
      }
    },
    async getDepartments() {
      try {
        const response = await client.getDepartmentQuery()
        if (response) {
          this.Departments = response
        }
      } catch (error) {
        console.error('Error fetching departments:', error)
      }
    },
    async UpdateUser() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        this.loading = true
        try {
          await client.updateAccountSettings(this.accountData)
          this.sweetAlertStore.successDeleted('บันทึกการเปลี่ยนแปลงเสร็จสิ้น')
          await this.initialize()
        } catch (error) {
          console.error(error)
          this.sweetAlertStore.errorDeleted('ไม่สามารถบันทึกการเปลี่ยนแปลงได้')
        } finally {
          this.loading = false
        }
      }
    },
    changeAvatar(event: Event) {
      const input = event.target as HTMLInputElement
      const { files } = input

      if (files && files.length) {
        const file = files[0]
        
        // Validate file type
        const validTypes = ['image/jpeg', 'image/png', 'image/jpg', 'image/gif']
        if (!validTypes.includes(file.type)) {
          this.sweetAlertStore.errorDeleted('กรุณาเลือกไฟล์รูปภาพประเภท JPEG, PNG, JPG หรือ GIF')
          input.value = ''
          return
        }

        // Validate file size (max 5MB)
        const maxSize = 5 * 1024 * 1024 // 5MB in bytes
        if (file.size > maxSize) {
          this.sweetAlertStore.errorDeleted('ขนาดไฟล์ต้องไม่เกิน 5MB')
          input.value = ''
          return
        }

        this.selectedFile = file

        // Show preview
        const fileReader = new FileReader()
        fileReader.readAsDataURL(file)
        fileReader.onload = () => {
          if (typeof fileReader.result === 'string') {
            this.imagePreview = fileReader.result
          }
        }

        // Auto upload
        this.uploadProfileImage()
      }
    },
    async uploadProfileImage() {
      if (!this.selectedFile) return

      this.uploadingImage = true
      try {
        // Create FileParameter object for NSwag client
        const fileParameter = {
          data: this.selectedFile,
          fileName: this.selectedFile.name
        }
        
        const response = await client.postApiUsersProfileImage(fileParameter)
        if (response && response.imageUrl) {
          this.currentImageProfile = response.imageUrl
          this.auth.image = response.imageUrl
          
          // Reload account settings to get updated data
          await this.initialize()
          
          this.sweetAlertStore.successDeleted(response.message || 'อัพโหลดรูปโปรไฟล์สำเร็จ')
          this.imagePreview = null
          this.selectedFile = null
        }
      } catch (error) {
        console.error('Error uploading profile image:', error)
        this.sweetAlertStore.errorDeleted('ไม่สามารถอัพโหลดรูปโปรไฟล์ได้')
        this.imagePreview = null
        this.selectedFile = null
      } finally {
        this.uploadingImage = false
      }
    },
    resetAvatar() {
      this.currentImageProfile = ''
      this.auth.image = ''
      this.imagePreview = null
      this.selectedFile = null
      this.sweetAlertStore.successDeleted('รีเซ็ทรูปโปรไฟล์สำเร็จ')
    },
    resetForm() {
      this.accountData.titleName = undefined
      this.accountData.firstName = undefined
      this.accountData.lastName = undefined
      this.accountData.phone = undefined
      this.accountData.departmentId = undefined
      this.accountData.position = undefined
    },
  },
})
</script>

<style scoped>
.avatar-style {
  border: 2px solid #2b3086;
}
</style>
