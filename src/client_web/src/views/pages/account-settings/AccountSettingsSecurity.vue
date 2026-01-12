<script lang="ts" setup>
import { ref, nextTick } from 'vue'
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores/sweetalert2'

const client = new Client(BACKEND_API_URL)
const SweetAlert = useSweetAlertStore()

const isCurrentPasswordVisible = ref(false)
const isNewPasswordVisible = ref(false)
const isConfirmPasswordVisible = ref(false)
const currentPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const isLoading = ref(false)

// Form reference for validation
const formRef = ref()

// Validation rules
const currentPasswordRules = [
  (v: string) => !!v || 'กรุณากรอกรหัสผ่านปัจจุบัน',
]

const newPasswordRules = [
  (v: string) => !!v || 'กรุณากรอกรหัสผ่านใหม่',
  (v: string) => v.length >= 6 || 'รหัสผ่านใหม่ต้องมีความยาวอย่างน้อย 6 ตัวอักษร',
  (v: string) => v !== currentPassword.value || 'รหัสผ่านใหม่ต้องไม่เหมือนกับรหัสผ่านปัจจุบัน',
]

const confirmPasswordRules = [
  (v: string) => !!v || 'กรุณายืนยันรหัสผ่านใหม่',
  (v: string) => v === newPassword.value || 'รหัสผ่านใหม่ไม่ตรงกัน',
]

const passwordRequirements = [
  'รหัสผ่านต้องมีความยาวอย่างน้อย 6 ตัวอักษร',
  'รหัสผ่านใหม่ต้องไม่เหมือนกับรหัสผ่านปัจจุบัน',
  'รหัสผ่านใหม่และยืนยันรหัสผ่านต้องตรงกัน',
]

const handleChangePassword = async () => {
  // Validate form
  const { valid } = await formRef.value?.validate()
  if (!valid) {
    return
  }

  isLoading.value = true

  try {
    const command = {
      currentPassword: currentPassword.value,
      newPassword: newPassword.value,
      confirmNewPassword: confirmPassword.value,
    }

    await client.changePassword(command)
    
    SweetAlert.success('เปลี่ยนรหัสผ่านสำเร็จ')
    
    // Reset form - use nextTick to ensure validation resets properly
    formRef.value?.reset()
    nextTick(() => {
      currentPassword.value = ''
      newPassword.value = ''
      confirmPassword.value = ''
      formRef.value?.resetValidation()
    })
  } catch (error: any) {
    console.error('Change password error:', error)
    
    // Parse error message
    let errorMessage = 'เกิดข้อผิดพลาดในการเปลี่ยนรหัสผ่าน'
    
    if (error?.result?.errors) {
      errorMessage = error.result.errors.join(', ')
    } else if (error?.response) {
      try {
        const responseData = JSON.parse(error.response)
        if (responseData.errors) {
          errorMessage = Array.isArray(responseData.errors) 
            ? responseData.errors.join(', ') 
            : responseData.errors
        }
      } catch {
        // ignore parse error
      }
    } else if (error?.message) {
      errorMessage = error.message
    }
    
    SweetAlert.error(errorMessage)
  } finally {
    isLoading.value = false
  }
}

const handleReset = () => {
  currentPassword.value = ''
  newPassword.value = ''
  confirmPassword.value = ''
  formRef.value?.resetValidation()
}
</script>

<template>
  <VRow>
    <!-- SECTION: Change Password -->
    <VCol cols="12">
      <VCard title="เปลี่ยนรหัสผ่าน">
        <VForm ref="formRef" @submit.prevent="handleChangePassword">
          <VCardText>
            <!-- 👉 Current Password -->
            <VRow class="mb-3">
              <VCol
                cols="12"
                md="6"
              >
                <!-- 👉 current password -->
                <VTextField
                  v-model="currentPassword"
                  :type="isCurrentPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isCurrentPasswordVisible ? 'ri-eye-off-line' : 'ri-eye-line'"
                  :rules="currentPasswordRules"
                  autocomplete="current-password"
                  label="รหัสผ่านปัจจุบัน"
                  placeholder="กรอกรหัสผ่านปัจจุบัน"
                  @click:append-inner="isCurrentPasswordVisible = !isCurrentPasswordVisible"
                />
              </VCol>
            </VRow>

            <!-- 👉 New Password -->
            <VRow>
              <VCol
                cols="12"
                md="6"
              >
                <!-- 👉 new password -->
                <VTextField
                  v-model="newPassword"
                  :type="isNewPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isNewPasswordVisible ? 'ri-eye-off-line' : 'ri-eye-line'"
                  :rules="newPasswordRules"
                  label="รหัสผ่านใหม่"
                  autocomplete="new-password"
                  placeholder="กรอกรหัสผ่านใหม่"
                  @click:append-inner="isNewPasswordVisible = !isNewPasswordVisible"
                />
              </VCol>

              <VCol
                cols="12"
                md="6"
              >
                <!-- 👉 confirm password -->
                <VTextField
                  v-model="confirmPassword"
                  :type="isConfirmPasswordVisible ? 'text' : 'password'"
                  :append-inner-icon="isConfirmPasswordVisible ? 'ri-eye-off-line' : 'ri-eye-line'"
                  :rules="confirmPasswordRules"
                  autocomplete="new-password"
                  label="ยืนยันรหัสผ่านใหม่"
                  placeholder="กรอกรหัสผ่านใหม่อีกครั้ง"
                  @click:append-inner="isConfirmPasswordVisible = !isConfirmPasswordVisible"
                />
              </VCol>
            </VRow>
          </VCardText>

          <!-- 👉 Password Requirements -->
          <VCardText>
            <p class="text-base font-weight-medium mt-2">
              ข้อกำหนดรหัสผ่าน:
            </p>

            <ul class="d-flex flex-column gap-y-3">
              <li
                v-for="item in passwordRequirements"
                :key="item"
                class="d-flex"
              >
                <div>
                  <VIcon
                    size="7"
                    icon="ri-checkbox-blank-circle-fill"
                    class="me-3"
                  />
                </div>
                <span class="font-weight-medium">{{ item }}</span>
              </li>
            </ul>
          </VCardText>

          <!-- 👉 Action Buttons -->
          <VCardText class="d-flex flex-wrap gap-4">
            <VBtn
              type="submit"
              :loading="isLoading"
              :disabled="isLoading"
            >
              บันทึกการเปลี่ยนแปลง
            </VBtn>

            <VBtn
              type="button"
              color="secondary"
              variant="outlined"
              :disabled="isLoading"
              @click="handleReset"
            >
              รีเซ็ต
            </VBtn>
          </VCardText>
        </VForm>
      </VCard>
    </VCol>
    <!-- !SECTION -->
  </VRow>
</template>
