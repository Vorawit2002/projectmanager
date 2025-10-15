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
  emailOrUsername: '',
  password: '',
  remember: false,
})

const isPasswordVisible = ref(false)
const loading = ref(false)
const errorMessage = ref('')
const formRef = ref()

const authThemeMask = computed(() => {
  return vuetifyTheme.global.name.value === 'light' ? authV1MaskLight : authV1MaskDark
})

// Validation rules
const emailOrUsernameRules = [
  (v: string) => !!v || 'กรุณากรอกอีเมลหรือชื่อผู้ใช้',
  (v: string) => (v && v.length >= 3) || 'อีเมลหรือชื่อผู้ใช้ต้องมีอย่างน้อย 3 ตัวอักษร',
]

const passwordRules = [
  (v: string) => !!v || 'กรุณากรอกรหัสผ่าน',
  (v: string) => (v && v.length >= 6) || 'รหัสผ่านต้องมีอย่างน้อย 6 ตัวอักษร',
]

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
    const success = await authStore.login(form.value.emailOrUsername, form.value.password)
    
    if (success) {
      // Redirect is handled by auth store
      // router.push('/Homepage') is called in auth.ts
    } else {
      errorMessage.value = 'อีเมลหรือรหัสผ่านไม่ถูกต้อง'
    }
  } catch (error: any) {
    console.error('Login error:', error)
    errorMessage.value = error.message || 'เกิดข้อผิดพลาดในการเข้าสู่ระบบ กรุณาลองใหม่อีกครั้ง'
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
        <h4 class="text-h4 mb-1">ยินดีต้อนรับ! 👋🏻</h4>
        <p class="mb-0">กรุณาเข้าสู่ระบบเพื่อเริ่มต้นใช้งาน</p>
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

            <!-- email or username -->
            <VCol cols="12">
              <VTextField
                v-model="form.emailOrUsername"
                label="อีเมลหรือชื่อผู้ใช้"
                placeholder="กรอกอีเมลหรือชื่อผู้ใช้"
                :rules="emailOrUsernameRules"
                :disabled="loading"
              />
            </VCol>

            <!-- password -->
            <VCol cols="12">
              <VTextField
                v-model="form.password"
                label="รหัสผ่าน"
                placeholder="············"
                :type="isPasswordVisible ? 'text' : 'password'"
                autocomplete="current-password"
                :append-inner-icon="isPasswordVisible ? 'ri-eye-off-line' : 'ri-eye-line'"
                :rules="passwordRules"
                :disabled="loading"
                @click:append-inner="isPasswordVisible = !isPasswordVisible"
              />

              <!-- remember me checkbox -->
              <div class="d-flex align-center justify-space-between flex-wrap my-6">
                <VCheckbox
                  v-model="form.remember"
                  label="จดจำฉันไว้"
                  :disabled="loading"
                />
              </div>

              <!-- login button -->
              <VBtn
                block
                type="submit"
                :loading="loading"
                :disabled="loading"
              >
                เข้าสู่ระบบ
              </VBtn>
            </VCol>

            <!-- create account -->
            <VCol
              cols="12"
              class="text-center text-base"
            >
              <span>ยังไม่มีบัญชีผู้ใช้?</span>
              <RouterLink
                class="text-primary ms-2"
                to="/register"
              >
                สมัครสมาชิก
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
@use '@core/scss/template/pages/page-auth';
</style>
