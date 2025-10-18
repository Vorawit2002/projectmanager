<template>
  <div class="misc-wrapper">
    <div class="misc-content">
      <!-- Error Icon -->
      <div class="d-flex justify-center mb-6">
        <VIcon
          icon="tabler-lock-access"
          size="100"
          color="error"
        />
      </div>

      <!-- Error Title -->
      <h1 class="text-h4 font-weight-medium mb-2">
        ไม่มีสิทธิ์เข้าถึง 🔒
      </h1>

      <!-- Error Message -->
      <p class="text-body-1 mb-6">
        คุณไม่มีสิทธิ์เข้าถึงหน้านี้ กรุณาติดต่อผู้ดูแลระบบหากคุณคิดว่านี่เป็นข้อผิดพลาด
      </p>

      <!-- Back to Dashboard Button -->
      <VBtn
        color="primary"
        @click="goToDashboard"
      >
        กลับไปหน้าหลัก
      </VBtn>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goToDashboard = () => {
  router.push('/')
}
</script>

<style lang="scss" scoped>
.misc-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  min-block-size: 100vh;
  padding: 1rem;
}

.misc-content {
  text-align: center;
  max-inline-size: 500px;
}
</style>
