<script setup lang="ts">
import { useTheme } from 'vuetify'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import NTiLogo from '@/assets/images/logos/NTiLogo.png'
import authV1MaskDark from '@images/pages/auth-v1-mask-dark.png'
import authV1MaskLight from '@images/pages/auth-v1-mask-light.png'
import authV1Tree2 from '@images/pages/auth-v1-tree-2.png'
import authV1Tree from '@images/pages/auth-v1-tree.png'

const router = useRouter()
const authStore = useAuthStore()
const vuetifyTheme = useTheme()

const form = ref({
  email: '',
  username: '',
  password: '',
  confirmPassword: '',
  firstName: '',
  lastName: '',
})

const isPasswordVisible = ref(false)
const isConfirmPasswordVisible = ref(false)
const loading = ref(false)
const errorMessage = ref('')
const formRef = ref()

const authThemeMask = computed(() => {
  return vuetifyTheme.global.name.value === 'light' ? authV1MaskLight : authV1MaskDark
})

// Validation rules
const emailRules = [
  (v: string) => !!v || 'กรุณากรอกอีเมล',
  (v: string) => /.+@.+\..+/.test(v) || 'รูปแบบอีเมลไม่ถูกต้อง',
]

const usernameRules = [
  (v: string) => !!v || 'กรุณากรอกชื่อผู้ใช้',
  (v: string) => (v && v.length >= 3) || 'ชื่อผู้ใช้ต้องมีอย่างน้อย 3 ตัวอักษร',
  (v: string) => (v && v.length <= 50) || 'ชื่อผู้ใช้ต้องมีความยาวไม่เกิน 50 ตัวอักษร',
]

const firstNameRules = [
  (v: string) => !!v || 'กรุณากรอกชื่อ',
]

const lastNameRules = [
  (v: string) => !!v || 'กรุณากรอกนามสกุล',
]

const passwordRules = [
  (v: string) => !!v || 'กรุณากรอกรหัสผ่าน',
  (v: string) => (v && v.length >= 6) || 'รหัสผ่านต้องมีอย่างน้อย 6 ตัวอักษร',
]

const confirmPasswordRules = [
  (v: string) => !!v || 'กรุณายืนยันรหัสผ่าน',
  (v: string) => v === form.value.password || 'รหัสผ่านไม่ตรงกัน',
]

// Password strength indicator
const passwordStrength = computed(() => {
  const password = form.value.password
  if (!password) return { level: 0, text: '', color: '' }
  
  let strength = 0
  if (password.length >= 6) strength++
  if (password.length >= 8) strength++
  if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++
  if (/\d/.test(password)) strength++
  if (/[^a-zA-Z0-9]/.test(password)) strength++
  
  if (strength <= 1) return { level: 1, text: 'อ่อนแอ', color: 'error' }
  if (strength <= 3) return { level: 2, text: 'ปานกลาง', color: 'warning' }
  return { level: 3, text: 'แข็งแรง', color: 'success' }
})

const onSubmit = async () => {
  errorMessage.value = ''
  
  // Validate form using Vuetify validation
  const { valid } = await formRef.value.validate()
  
  if (!valid) {
    errorMessage.value = 'กรุณากรอกข้อมูลให้ถูกต้องและครบถ้วน'
    return
  }
  
  loading.value = true
  
  try {
    const result = await authStore.register({
      email: form.value.email,
      username: form.value.username,
      password: form.value.password,
      confirmPassword: form.value.confirmPassword,
      firstName: form.value.firstName,
      lastName: form.value.lastName,
    })
    
    if (result.success) {
      // Redirect to login page after successful registration
      router.push('/login')
    } else {
      errorMessage.value = result.message || 'เกิดข้อผิดพลาดในการสมัครสมาชิก'
    }
  } catch (error: any) {
    console.error('Register error:', error)
    errorMessage.value = error.message || 'เกิดข้อผิดพลาดในการสมัครสมาชิก กรุณาลองใหม่อีกครั้ง'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div
    class="auth-wrapper d-flex align-center justify-center pa-4"
    style="background: #c4d9ff !important"
  >
    <VCard
      class="auth-card pa-4 pt-7"
      max-width="448"
    >
      <VCardItem class="justify-center">
        <div class="d-flex align-center gap-3">
          <img
            class="d-flex"
            :src="NTiLogo"
            width="120"
          />
        </div>
      </VCardItem>

      <VCardText class="pt-2">
        <h4 class="text-h4 mb-1">สมัครสมาชิก 🚀</h4>
        <p class="mb-0">กรอกข้อมูลเพื่อสร้างบัญชีผู้ใช้ใหม่</p>
      </VCardText>

      <VCardText>
        <VForm 
          ref="formRef"
          @submit.prevent="onSubmit"
        >
          <VRow>
            <!-- Error message -->
            <VCol
              v-if="errorMessage"
              cols="12"
            >
              <VAlert
                type="error"
                variant="tonal"
                closable
                @click:close="errorMessage = ''"
              >
                {{ errorMessage }}
              </VAlert>
            </VCol>

            <!-- Email -->
            <VCol cols="12">
              <VTextField
                v-model="form.email"
                label="อีเมล"
                placeholder="example@email.com"
                type="email"
                :rules="emailRules"
                :disabled="loading"
              />
            </VCol>

            <!-- Username -->
            <VCol cols="12">
              <VTextField
                v-model="form.username"
                label="ชื่อผู้ใช้"
                placeholder="กรอกชื่อผู้ใช้"
                :rules="usernameRules"
                :disabled="loading"
              />
            </VCol>

            <!-- First Name -->
            <VCol cols="12">
              <VTextField
                v-model="form.firstName"
                label="ชื่อ"
                placeholder="กรอกชื่อ"
                :rules="firstNameRules"
                :disabled="loading"
              />
            </VCol>

            <!-- Last Name -->
            <VCol cols="12">
              <VTextField
                v-model="form.lastName"
                label="นามสกุล"
                placeholder="กรอกนามสกุล"
                :rules="lastNameRules"
                :disabled="loading"
              />
            </VCol>

            <!-- Password -->
            <VCol cols="12">
              <VTextField
                v-model="form.password"
                label="รหัสผ่าน"
                placeholder="············"
                :type="isPasswordVisible ? 'text' : 'password'"
                autocomplete="new-password"
                :append-inner-icon="isPasswordVisible ? 'ri-eye-off-line' : 'ri-eye-line'"
                :rules="passwordRules"
                :disabled="loading"
                @click:append-inner="isPasswordVisible = !isPasswordVisible"
              />
              <!-- Password strength indicator -->
              <div
                v-if="form.password"
                class="mt-2"
              >
                <div class="d-flex align-center gap-2">
                  <VProgressLinear
                    :model-value="passwordStrength.level * 33.33"
                    :color="passwordStrength.color"
                    height="4"
                  />
                  <span
                    class="text-caption"
                    :class="`text-${passwordStrength.color}`"
                  >
                    {{ passwordStrength.text }}
                  </span>
                </div>
              </div>
            </VCol>

            <!-- Confirm Password -->
            <VCol cols="12">
              <VTextField
                v-model="form.confirmPassword"
                label="ยืนยันรหัสผ่าน"
                placeholder="············"
                :type="isConfirmPasswordVisible ? 'text' : 'password'"
                autocomplete="new-password"
                :append-inner-icon="isConfirmPasswordVisible ? 'ri-eye-off-line' : 'ri-eye-line'"
                :rules="confirmPasswordRules"
                :disabled="loading"
                @click:append-inner="isConfirmPasswordVisible = !isConfirmPasswordVisible"
              />
            </VCol>

            <!-- Register button -->
            <VCol cols="12">
              <VBtn
                block
                type="submit"
                :loading="loading"
                :disabled="loading"
              >
                สมัครสมาชิก
              </VBtn>
            </VCol>

            <!-- Login link -->
            <VCol
              cols="12"
              class="text-center text-base"
            >
              <span>มีบัญชีผู้ใช้แล้ว?</span>
              <RouterLink
                class="text-primary ms-2"
                to="/login"
              >
                เข้าสู่ระบบ
              </RouterLink>
            </VCol>
          </VRow>
        </VForm>
      </VCardText>
    </VCard>

    <VImg
      class="auth-footer-start-tree d-none d-md-block"
      :src="authV1Tree"
      :width="250"
    />

    <VImg
      :src="authV1Tree2"
      class="auth-footer-end-tree d-none d-md-block"
      :width="350"
    />

    <!-- bg img -->
    <VImg
      class="auth-footer-mask d-none d-md-block"
      :src="authThemeMask"
    />
  </div>
</template>

<style lang="scss">
@use "@core/scss/template/pages/page-auth";
</style>
