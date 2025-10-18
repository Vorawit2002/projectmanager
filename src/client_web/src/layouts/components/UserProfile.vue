<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { BACKEND_API_URL } from '@/constants'
import { Client } from '@/client'
import avatar1 from '@images/avatars/avatar-1.png'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores'

const router = useRouter()
const auth = useAuthStore()
const client = new Client(BACKEND_API_URL)

// Store user profile data
const userProfile = ref<any>(null)

// Function to fetch user profile
const fetchUserProfile = async () => {
  try {
    userProfile.value = await client.getCurrentUser()
    console.log('UserProfile - Fetched user data:', userProfile.value)
  } catch (error) {
    console.error('UserProfile - Error fetching user data:', error)
  }
}

// Fetch user profile on mount
onMounted(async () => {
  await fetchUserProfile()
})

// Watch auth.image for changes (when profile image is updated)
watch(() => auth.image, async (newImage, oldImage) => {
  if (newImage !== oldImage) {
    console.log('UserProfile - auth.image changed, reloading profile')
    await fetchUserProfile()
  }
})

// Watch auth.isLogged to reload profile when user logs in
watch(() => auth.isLogged, async (isLogged) => {
  if (isLogged) {
    console.log('UserProfile - User logged in, fetching profile')
    await fetchUserProfile()
  }
})

// Use imageProfile from fetched data with proper URL handling
const userImage = computed(() => {
  const imageProfile = userProfile.value?.imageProfile || auth.image || null
  
  if (!imageProfile) return null
  
  // If it's already a full URL (http/https) or data URL (data:), use as is
  if (imageProfile.startsWith('http') || imageProfile.startsWith('data:')) {
    return imageProfile
  }
  
  // If it's a relative path, prepend BACKEND_API_URL
  return `${BACKEND_API_URL}${imageProfile.startsWith('/') ? '' : '/'}${imageProfile}`
})

// Get user initials for avatar
const userInitials = computed(() => {
  const firstName = userProfile.value?.firstName || auth.firstName || ''
  const lastName = userProfile.value?.lastName || auth.lastName || ''
  const userName = userProfile.value?.userName || auth.username || ''
  
  if (firstName && lastName) {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase()
  } else if (firstName) {
    return firstName.charAt(0).toUpperCase()
  } else if (userName) {
    return userName.charAt(0).toUpperCase()
  }
  return 'U'
})

async function toLogout() {
  // ล้างข้อมูล notification session ก่อน logout
  clearLoginNotificationData()
  // ใช้ auth store logout method
  await auth.logout()
  router.push('/')
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
      :key="userImage || userInitials"
      class="cursor-pointer"
      variant="tonal"
    >
      <VImg 
        v-if="userImage"
        :src="userImage"
        @error="(e) => console.error('UserProfile main - Image load error:', e)"
      />
      <span v-else class="text-h6">{{ userInitials }}</span>

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
                    :key="userImage || userInitials"
                    color="primary"
                    variant="tonal"
                  >
                    <VImg 
                      v-if="userImage"
                      :src="userImage"
                      @error="(e) => console.error('UserProfile menu - Image load error:', e)"
                    />
                    <span v-else class="text-h6">{{ userInitials }}</span>
                  </VAvatar>
                </VBadge>
              </VListItemAction>
            </template>

            <VListItemTitle class="font-weight-semibold"> 
              {{ userProfile?.userName || auth.username }} 
            </VListItemTitle>
            <span
              class="text-grey-400"
              v-for="(role, index) in (userProfile?.roles || userProfile?.roleNames || auth.roles)"
              :key="index"
            >
              {{ role }}{{ index + 1 === (userProfile?.roles || userProfile?.roleNames || auth.roles).length ? '' : ',' }}
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
