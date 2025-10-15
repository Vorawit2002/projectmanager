<template>
  <VRow>
    <VCol cols="12">
      <VCard title="จัดการข้อมูลส่วนตัว">
        <VCardText class="d-flex">
          <!-- 👉 Avatar -->
          <VAvatar
            rounded="lg"
            size="200"
            class="me-6 avatar-style"
            :image="auth.image || avatar1"
          />

          <!-- 👉 Upload Photo -->
          <form class="d-flex flex-column justify-center gap-5">
            <div class="d-flex flex-wrap gap-2">
              <!-- <VBtn
                color="primary"
                @click="refInputEl?.click()"
              >
                <VIcon
                  icon="ri-upload-cloud-line"
                  class="d-sm-none"
                />
                <span class="d-none d-sm-block">Upload new photo</span>
              </VBtn> -->

              <input
                ref="refInputEl"
                type="file"
                name="file"
                accept=".jpeg,.png,.jpg,GIF"
                hidden
                @input="changeAvatar"
              />

              <!-- <VBtn
                type="reset"
                color="error"
                variant="outlined"
                @click="resetAvatar"
              >
                <span class="d-none d-sm-block">Reset</span>
                <VIcon
                  icon="ri-refresh-line"
                  class="d-sm-none"
                />
              </VBtn> -->
            </div>

            <!-- <p class="text-body-1 mb-0">Allowed JPG, GIF or PNG. Max size of 800K</p> -->
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
                <VTextField
                  v-model="accountData.titleName"
                  label="คำนำหน้า"
                  :rules="[(value:any)=> !!value|| 'กรุณากรอกคำนำหน้า']"
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
import { Client, UpdateEmployeeCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import avatar1 from '@images/avatars/avatar-1.png'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  data() {
    return {
      accountData: new UpdateEmployeeCommand(),
      refInputEl: null as HTMLElement | null,
      isAccountDeactivated: false,
      auth: useAuthStore(),
      avatar1,
      Departments: [] as any,
      sweetAlertStore: useSweetAlertStore(),
      loading: false,
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
        if (this.auth.userId) {
          const response = await client.getEmployeeQueryByUserID(this.auth.userId)
          if (response) {
            this.accountData = { ...response } as UpdateEmployeeCommand
            if (!this.accountData.phone && this.auth.phone) {
              this.accountData.phone = this.auth.phone
            }
          }
        } else {
          return
        }
      } catch (error) {
        console.error('Error initializing account settings:', error)
      }
    },
    async getDepartments() {
      try {
        const response = await client.getDepartmentQuery()
        if (response) {
          this.Departments = response
          console.log(response)
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
          const response = await client.updateEmployee(this.accountData)
          if (response) {
            this.sweetAlertStore.successDeleted('บันทึกการเปลี่ยนแปลงเสร็จสิ้น')
            this.loading = false
          }
        } catch (error) {
          console.error(error)
          this.loading = false
        }
      }
    },
    changeAvatar(file: Event) {
      const fileReader = new FileReader()
      const { files } = file.target as HTMLInputElement

      if (files && files.length) {
        fileReader.readAsDataURL(files[0])
        fileReader.onload = () => {
          // if (typeof fileReader.result === 'string') this.accountDataLocal.avatarImg = fileReader.result
        }
      }
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
