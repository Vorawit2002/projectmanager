<template>
  <div class="user-detail-view">
    <!-- Header -->
    <div class="drawer-header bg-primary pa-4">
      <div class="d-flex align-center justify-space-between">
        <span class="text-h6 text-white">
          <v-icon
            icon="ri-user-line"
            class="mr-2"
          />
          รายละเอียดผู้ใช้งาน
        </span>
        <v-btn
          icon
          variant="text"
          size="small"
          @click="$emit('close')"
        >
          <v-icon
            color="white"
            icon="ri-close-line"
          />
        </v-btn>
      </div>
    </div>

    <!-- Loading State -->
    <div
      v-if="isLoading"
      class="pa-8 text-center"
    >
      <v-progress-circular
        indeterminate
        color="primary"
        size="64"
      />
      <div class="mt-4 text-medium-emphasis">กำลังโหลดข้อมูล...</div>
    </div>

    <!-- Content -->
    <div
      v-else-if="user"
      class="pa-6"
    >
      <!-- Profile Section -->
      <v-card
        class="mb-4"
        variant="outlined"
      >
        <v-card-text class="text-center pa-6">
          <v-avatar
            size="120"
            class="mb-4"
          >
            <v-img
              v-if="user.imageProfile"
              :src="user.imageProfile"
              :alt="user.firstName + ' ' + user.lastName"
            />
            <v-icon
              v-else
              icon="ri-user-line"
              size="64"
            />
          </v-avatar>
          <h3 class="text-h5 mb-2">{{ user.firstName }} {{ user.lastName }}</h3>
          <div class="text-medium-emphasis mb-3">{{ user.email }}</div>
          <v-chip
            :color="user.isActive ? 'success' : 'error'"
            size="small"
          >
            {{ user.isActive ? 'Active' : 'Inactive' }}
          </v-chip>
        </v-card-text>
      </v-card>

      <!-- User Information -->
      <v-card
        class="mb-4"
        variant="outlined"
      >
        <v-card-title class="bg-grey-lighten-4 pa-4">
          <v-icon
            icon="ri-information-line"
            class="mr-2"
          />
          ข้อมูลผู้ใช้งาน
        </v-card-title>
        <v-card-text class="pa-4">
          <v-row dense>
            <v-col
              cols="12"
              class="py-2"
            >
              <div class="text-caption text-medium-emphasis">ชื่อผู้ใช้</div>
              <div class="font-weight-medium">{{ user.username || '-' }}</div>
            </v-col>
            <v-col
              cols="12"
              class="py-2"
            >
              <div class="text-caption text-medium-emphasis">อีเมล</div>
              <div class="font-weight-medium">{{ user.email || '-' }}</div>
            </v-col>
            <v-col
              cols="12"
              class="py-2"
            >
              <div class="text-caption text-medium-emphasis">ชื่อ</div>
              <div class="font-weight-medium">{{ user.firstName || '-' }}</div>
            </v-col>
            <v-col
              cols="12"
              class="py-2"
            >
              <div class="text-caption text-medium-emphasis">นามสกุล</div>
              <div class="font-weight-medium">{{ user.lastName || '-' }}</div>
            </v-col>
            <v-col
              cols="12"
              class="py-2"
            >
              <div class="text-caption text-medium-emphasis">แผนก</div>
              <div class="font-weight-medium">{{ user.department || '-' }}</div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!-- Roles Section -->
      <v-card
        class="mb-4"
        variant="outlined"
      >
        <v-card-title class="bg-grey-lighten-4 pa-4">
          <v-icon
            icon="ri-shield-user-line"
            class="mr-2"
          />
          บทบาท (Roles)
        </v-card-title>
        <v-card-text class="pa-4">
          <div v-if="user.roles && user.roles.length > 0">
            <v-chip
              v-for="role in user.roles"
              :key="role"
              :color="getRoleColor(role)"
              size="default"
              class="mr-2 mb-2"
            >
              <v-icon
                icon="ri-shield-check-line"
                class="mr-1"
                size="18"
              />
              {{ role }}
            </v-chip>
          </div>
          <div
            v-else
            class="text-medium-emphasis"
          >
            ไม่มี Role
          </div>
        </v-card-text>
      </v-card>

      <!-- Activity Section -->
      <v-card
        class="mb-4"
        variant="outlined"
      >
        <v-card-title class="bg-grey-lighten-4 pa-4">
          <v-icon
            icon="ri-time-line"
            class="mr-2"
          />
          กิจกรรม
        </v-card-title>
        <v-card-text class="pa-4">
          <v-row dense>
            <v-col
              cols="12"
              class="py-2"
            >
              <div class="text-caption text-medium-emphasis">เข้าสู่ระบบล่าสุด</div>
              <div class="font-weight-medium">
                {{ user.lastLoginDate ? formatDate(user.lastLoginDate) : 'ไม่มีข้อมูล' }}
              </div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!-- Role History Section (if available) -->
      <v-card
        v-if="roleHistory && roleHistory.length > 0"
        variant="outlined"
      >
        <v-card-title class="bg-grey-lighten-4 pa-4">
          <v-icon
            icon="ri-history-line"
            class="mr-2"
          />
          ประวัติการเปลี่ยน Role
        </v-card-title>
        <v-card-text class="pa-4">
          <v-timeline
            side="end"
            density="compact"
            align="start"
          >
            <v-timeline-item
              v-for="(history, index) in roleHistory"
              :key="index"
              dot-color="primary"
              size="small"
            >
              <div class="mb-2">
                <div class="font-weight-medium">{{ history.roleName }}</div>
                <div class="text-caption text-medium-emphasis">
                  {{ formatDate(history.assignedDate) }}
                </div>
              </div>
            </v-timeline-item>
          </v-timeline>
        </v-card-text>
      </v-card>
    </div>

    <!-- Error State -->
    <div
      v-else
      class="pa-8 text-center"
    >
      <v-icon
        icon="ri-error-warning-line"
        size="64"
        color="error"
        class="mb-4"
      />
      <div class="text-h6 mb-2">ไม่พบข้อมูลผู้ใช้งาน</div>
      <v-btn
        color="primary"
        @click="$emit('close')"
      >
        ปิด
      </v-btn>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'UserDetailView',
  props: {
    userId: {
      type: String,
      required: true,
    },
  },
  emits: ['close'],
  data() {
    return {
      sweetAlert: useSweetAlertStore(),
      user: null as any,
      roleHistory: [] as any[],
      isLoading: false,
    }
  },
  async mounted() {
    await this.loadUserDetails()
  },
  methods: {
    async loadUserDetails() {
      try {
        this.isLoading = true
        this.user = await client.getUserById(this.userId)
        
        // Role history would be loaded here if the backend provides it
        // For now, we'll leave it empty as it's not in the current implementation
        this.roleHistory = []
      } catch (error: any) {
        console.error('Error loading user details:', error)
        this.sweetAlert.error('ไม่สามารถโหลดข้อมูลผู้ใช้งานได้')
      } finally {
        this.isLoading = false
      }
    },
    getRoleColor(role: string): string {
      const colors: Record<string, string> = {
        Admin: 'error',
        Manager: 'warning',
        User: 'primary',
        Viewer: 'secondary',
      }
      return colors[role] || 'default'
    },
    formatDate(date: string | Date): string {
      if (!date) return '-'
      const d = new Date(date)
      return d.toLocaleDateString('th-TH', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      })
    },
  },
})
</script>

<style scoped>
.user-detail-view {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.drawer-header {
  position: sticky;
  top: 0;
  z-index: 1;
}
</style>
