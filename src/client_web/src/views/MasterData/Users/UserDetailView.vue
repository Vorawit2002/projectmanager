<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="page-container scroll-content dialog-scrollbar">
        <v-card-text>
          <div class="d-flex align-center justify-space-between mb-4">
            <span class="text-sub-title">
              <v-icon
                icon="ri-user-line"
                class="mr-2"
              />
              รายละเอียดผู้ใช้งาน
            </span>
            <!-- <v-btn
              icon
              variant="text"
              size="small"
              @click="$emit('close')"
            >
              <v-icon icon="ri-close-line" />
            </v-btn> -->
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
          <v-row
            v-else-if="user"
            class="px-1 px-sm-3 px-md-5 mt-3"
          >
            <!-- Profile Section -->
            <v-col cols="12">
              <v-card
                variant="tonal"
                color="primary"
                class="pa-4"
              >
                <div class="d-flex align-center">
                  <v-avatar
                    size="80"
                    class="mr-4"
                    color="primary"
                  >
                    <v-img
                      v-if="user.imageProfile"
                      :src="user.imageProfile"
                      :alt="user.firstName + ' ' + user.lastName"
                    />
                    <span
                      v-else
                      class="text-h4 font-weight-bold"
                    >
                      {{ getUserInitials() }}
                    </span>
                  </v-avatar>
                  <div class="flex-grow-1">
                    <div class="text-h5 font-weight-bold mb-1">{{ user.firstName }} {{ user.lastName }}</div>
                    <div class="text-body-2 mb-2">{{ user.email }}</div>
                    <v-chip
                      :color="user.isActive ? 'success' : 'error'"
                      size="small"
                    >
                      <v-icon
                        start
                        size="16"
                      >{{ user.isActive ? 'ri-checkbox-circle-line' : 'ri-close-circle-line' }}</v-icon>
                      {{ user.isActive ? 'Active' : 'Inactive' }}
                    </v-chip>
                  </div>
                </div>
              </v-card>
            </v-col>

            <!-- User Information -->
            <v-col cols="12">
              <label class="form-label">
                <v-icon
                  icon="ri-information-line"
                  size="18"
                  class="mr-1"
                />
                ข้อมูลผู้ใช้งาน
              </label>
              <v-card
                variant="outlined"
                class="mt-2"
              >
                <v-card-text class="pa-4">
                  <v-row dense>
                    <v-col
                      cols="12"
                      md="6"
                      class="py-2"
                    >
                      <div class="text-caption text-medium-emphasis mb-1">ชื่อผู้ใช้</div>
                      <div class="font-weight-medium">{{ user.username || '-' }}</div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      class="py-2"
                    >
                      <div class="text-caption text-medium-emphasis mb-1">อีเมล</div>
                      <div class="font-weight-medium">{{ user.email || '-' }}</div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      class="py-2"
                    >
                      <div class="text-caption text-medium-emphasis mb-1">ชื่อ</div>
                      <div class="font-weight-medium">{{ user.firstName || '-' }}</div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      class="py-2"
                    >
                      <div class="text-caption text-medium-emphasis mb-1">นามสกุล</div>
                      <div class="font-weight-medium">{{ user.lastName || '-' }}</div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      class="py-2"
                    >
                      <div class="text-caption text-medium-emphasis mb-1">แผนก</div>
                      <div class="font-weight-medium">{{ user.department || '-' }}</div>
                    </v-col>
                    <v-col
                      cols="12"
                      md="6"
                      class="py-2"
                    >
                      <div class="text-caption text-medium-emphasis mb-1">ตำแหน่ง</div>
                      <div class="font-weight-medium">{{ user.position || '-' }}</div>
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-col>

            <!-- Roles Section -->
            <v-col cols="12">
              <label class="form-label">
                <v-icon
                  icon="ri-shield-user-line"
                  size="18"
                  class="mr-1"
                />
                บทบาท (Roles)
              </label>
              <v-card
                variant="outlined"
                class="mt-2"
              >
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
                        start
                        size="18"
                      >ri-shield-check-line</v-icon>
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
            </v-col>

            <!-- Activity Section -->
            <v-col
              cols="12"
              v-if="user.lastLoginDate"
            >
              <label class="form-label">
                <v-icon
                  icon="ri-time-line"
                  size="18"
                  class="mr-1"
                />
                กิจกรรม
              </label>
              <v-card
                variant="outlined"
                class="mt-2"
              >
                <v-card-text class="pa-4">
                  <div class="text-caption text-medium-emphasis mb-1">เข้าสู่ระบบล่าสุด</div>
                  <div class="font-weight-medium">
                    {{ formatDate(user.lastLoginDate) }}
                  </div>
                </v-card-text>
              </v-card>
            </v-col>

            <!-- Role History Section (if available) -->
            <v-col
              cols="12"
              v-if="roleHistory && roleHistory.length > 0"
            >
              <label class="form-label">
                <v-icon
                  icon="ri-history-line"
                  size="18"
                  class="mr-1"
                />
                ประวัติการเปลี่ยน Role
              </label>
              <v-card
                variant="outlined"
                class="mt-2"
              >
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
            </v-col>

            <!-- Close Button -->
            <v-col
              cols="12"
              class="pt-6"
            >
              <v-btn
                rounded="lg"
                color="primary"
                @click="$emit('close')"
                block
              >
                <v-icon
                  icon="ri-close-line"
                  class="mr-2"
                ></v-icon>
                ปิด
              </v-btn>
            </v-col>
          </v-row>

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
              rounded="lg"
              @click="$emit('close')"
            >
              <v-icon
                icon="ri-close-line"
                class="mr-2"
              ></v-icon>
              ปิด
            </v-btn>
          </div>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
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
    // Add ESC key listener
    window.addEventListener('keydown', this.handleEscKey)
  },
  beforeUnmount() {
    // Remove ESC key listener
    window.removeEventListener('keydown', this.handleEscKey)
  },
  methods: {
    handleEscKey(event: KeyboardEvent) {
      if (event.key === 'Escape' && !this.isLoading) {
        this.$emit('close')
      }
    },
    getUserInitials(): string {
      if (!this.user) return 'U'
      
      const firstName = this.user.firstName || ''
      const lastName = this.user.lastName || ''
      const userName = this.user.username || ''

      if (firstName && lastName) {
        return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase()
      } else if (firstName) {
        return firstName.charAt(0).toUpperCase()
      } else if (userName) {
        return userName.charAt(0).toUpperCase()
      }
      return 'U'
    },
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
.page-container {
  min-height: 100vh;
  border-radius: 0;
  overflow-y: auto;
  max-height: 100vh;
}

.scroll-content {
  overflow-y: auto;
}

.dialog-scrollbar::-webkit-scrollbar {
  width: 8px;
}

.dialog-scrollbar::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

.dialog-scrollbar::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 10px;
}

.dialog-scrollbar::-webkit-scrollbar-thumb:hover {
  background: #555;
}

.form-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: rgba(0, 0, 0, 0.87);
  margin-bottom: 0.5rem;
  display: block;
}
</style>
