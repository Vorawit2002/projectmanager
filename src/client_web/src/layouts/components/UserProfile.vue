<script setup lang="ts">
import { BACKEND_API_URL } from '@/constants'
import avatar1 from '@images/avatars/avatar-1.png'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores'

const router = useRouter()
const auth = useAuthStore()

async function toLogout() {
  // ล้างข้อมูล notification session ก่อน logout
  clearLoginNotificationData()
  // ใช้ auth store logout method
  await auth.logout()
  router.push('/login')
}

function clearLoginNotificationData() {
  // ล้าง session ทั้งหมดที่เกี่ยวข้องกับ login notification
  Object.keys(sessionStorage).forEach(key => {
    if (key.startsWith('login_session_')) {
      sessionStorage.removeItem(key)
    }
  })
}

function goToProfile() {
  router.push('/account-settings')
}
</script>

<template>
  <VBadge
    dot
    location="bottom right"
    offset-x="3"
    offset-y="3"
    color="success"
    bordered
  >
    <VAvatar
      class="cursor-pointer"
      color="primary"
      variant="tonal"
    >
      <VImg :src="auth.image" />

      <!-- SECTION Menu -->
      <VMenu
        activator="parent"
        width="auto"
        location="bottom end"
        offset="14px"
      >
        <VList
        style="border-radius: 15px;">
          <!-- 👉 User Avatar & Name -->
          <VListItem>
            <template #prepend>
              <VListItemAction start>
                <VBadge
                  dot
                  location="bottom right"
                  offset-x="3"
                  offset-y="3"
                  color="success"
                >
                  <VAvatar
                    color="primary"
                    variant="tonal"
                  >
                    <VImg :src="auth.image" />
                  </VAvatar>
                </VBadge>
              </VListItemAction>
            </template>

            <VListItemTitle class="font-weight-semibold"> {{ auth.username }} </VListItemTitle>
            <span
              class="text-grey-400"
              v-for="(role, index) in auth.roles"
            >
              {{ role }}{{ index + 1 === auth.roles.length ? '' : ',' }}
            </span>
          </VListItem>
          <VDivider class="my-2" />

          <!-- 👉 Profile -->
          <VListItem
            link
            @click="goToProfile"
          >
            <template #prepend>
              <VIcon
                class="me-2"
                icon="ri-user-line"
                size="22"
              />
            </template>

            <VListItemTitle>Profile</VListItemTitle>
          </VListItem>

          <!-- 👉 Settings -->
          <!-- <VListItem link>
            <template #prepend>
              <VIcon
                class="me-2"
                icon="ri-settings-4-line"
                size="22"
              />
            </template>

            <VListItemTitle>Settings</VListItemTitle>
          </VListItem> -->

          <!-- 👉 Pricing -->
          <!-- <VListItem link>
            <template #prepend>
              <VIcon
                class="me-2"
                icon="ri-money-dollar-circle-line"
                size="22"
              />
            </template>

            <VListItemTitle>Pricing</VListItemTitle>
          </VListItem> -->

          <!-- 👉 FAQ -->
          <!-- <VListItem link>
            <template #prepend>
              <VIcon
                class="me-2"
                icon="ri-question-line"
                size="22"
              />
            </template>

            <VListItemTitle>FAQ</VListItemTitle>
          </VListItem> -->

          <!-- Divider -->
          <VDivider class="my-2" />

          <!-- 👉 Logout -->
          <VListItem
            link
            @click="toLogout"
          >
            <template #prepend>
              <VIcon
                class="me-2"
                icon="ri-logout-box-r-line"
                size="22"
              />
            </template>

            <VListItemTitle>Logout</VListItemTitle>
          </VListItem>
        </VList>
      </VMenu>
      <!-- !SECTION -->
    </VAvatar>
  </VBadge>
</template>
