<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="12"
    >
      <v-card class="page-container scroll-content dialog-scrollbar">
        <v-card-text>
          <span class="text-sub-title">
            <v-icon
              icon="ri-shield-user-line"
              class="mr-2"
            />
            กำหนด Role
          </span>

          <v-form
            ref="form"
            @submit.prevent="assignRole"
            class="text-black"
          >
            <v-row class="px-1 px-sm-3 px-md-5 mt-3">
              <!-- ข้อมูลผู้ใช้งาน -->
              <v-col cols="12">
                <v-card
                  variant="tonal"
                  color="primary"
                  class="pa-4"
                >
                  <div class="d-flex align-center">
                    <v-avatar
                      size="60"
                      class="mr-4"
                      color="primary"
                    >
                      <v-img
                        v-if="user?.imageProfile"
                        :src="user.imageProfile"
                        :alt="user.firstName + ' ' + user.lastName"
                      />
                      <span
                        v-else
                        class="text-h5 font-weight-bold"
                      >
                        {{ getUserInitials() }}
                      </span>
                    </v-avatar>
                    <div>
                      <div class="text-h6 font-weight-bold">{{ user?.firstName }} {{ user?.lastName }}</div>
                      <div class="text-body-2">{{ user?.email }}</div>
                      <div class="text-caption">{{ user?.department || 'ไม่มีแผนก' }}</div>
                    </div>
                  </div>
                </v-card>
              </v-col>

              <!-- Role ปัจจุบัน -->
              <v-col cols="12">
                <label class="form-label">Role ปัจจุบัน</label>
                <div class="mt-2">
                  <v-chip
                    v-for="role in user?.roles"
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
                  <span
                    v-if="!user?.roles || user.roles.length === 0"
                    class="text-medium-emphasis"
                  >
                    ไม่มี Role
                  </span>
                </div>
              </v-col>

              <!-- เลือก Role ใหม่ -->
              <v-col cols="12">
                <label class="form-label">เลือก Role ใหม่ <span class="text-error">*</span></label>
                <v-select
                  v-model="selectedRole"
                  :items="roleOptions"
                  placeholder="เลือก Role"
                  variant="outlined"
                  density="comfortable"
                  color="primary"
                  :rules="[(v:any) => !!v || 'กรุณาเลือก Role']"
                  clearable
                >
                  <template v-slot:item="{ props, item }">
                    <v-list-item v-bind="props">
                      <template v-slot:prepend>
                        <v-chip
                          :color="getRoleColor(item.value)"
                          size="small"
                          class="mr-2"
                        >
                          {{ item.title }}
                        </v-chip>
                      </template>
                    </v-list-item>
                  </template>
                  <template v-slot:selection="{ item }">
                    <v-chip
                      :color="getRoleColor(item.value)"
                      size="small"
                    >
                      {{ item.title }}
                    </v-chip>
                  </template>
                </v-select>
              </v-col>

              <!-- คำเตือน -->
              <v-col cols="12">
                <v-alert
                  type="warning"
                  variant="tonal"
                  density="compact"
                  class="text-body-2"
                >
                  <v-icon
                    start
                    size="20"
                  >ri-information-line</v-icon>
                  <strong>หมายเหตุ:</strong> การกำหนด Role ใหม่จะแทนที่ Role เดิมทั้งหมด
                </v-alert>
              </v-col>

              <!-- ปุ่มควบคุม -->
              <v-col
                cols="12"
                class="pt-6"
              >
                <div class="button-container">
                  <v-btn
                    class="mobile-btn submit-btn"
                    rounded="lg"
                    color="success-darken-2"
                    type="submit"
                    :loading="isLoading"
                    :disabled="isLoading || !selectedRole"
                    block
                  >
                    <v-icon class="mr-2">ri-save-3-fill</v-icon>
                    บันทึก
                  </v-btn>
                  <v-btn
                    class="mobile-btn cancel-btn"
                    rounded="lg"
                    color="error"
                    @click="closeDrawer"
                    :disabled="isLoading"
                    block
                  >
                    <v-icon
                      icon="ri-close-line"
                      class="mr-2"
                    ></v-icon>
                    ยกเลิก
                  </v-btn>
                </div>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue'
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'AssignRoleDrawer',
  props: {
    user: {
      type: Object as PropType<any>,
      required: true,
    },
  },
  emits: ['close', 'role-assigned'],
  data() {
    return {
      sweetAlert: useSweetAlertStore(),
      selectedRole: null as string | null,
      roleOptions: [
        { title: 'Admin', value: 'Admin' },
        { title: 'Manager', value: 'Manager' },
        { title: 'User', value: 'User' },
        { title: 'Viewer', value: 'Viewer' },
      ],
      isLoading: false,
    }
  },
  mounted() {
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
        this.closeDrawer()
      }
    },
    getUserInitials(): string {
      const firstName = this.user?.firstName || ''
      const lastName = this.user?.lastName || ''
      const userName = this.user?.userName || ''

      if (firstName && lastName) {
        return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase()
      } else if (firstName) {
        return firstName.charAt(0).toUpperCase()
      } else if (userName) {
        return userName.charAt(0).toUpperCase()
      }
      return 'U'
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
    closeDrawer() {
      this.$emit('close')
      this.selectedRole = null
    },
    async assignRole() {
      const form = this.$refs.form as any
      const { valid } = await form.validate()

      if (!valid || !this.selectedRole) {
        return
      }

      try {
        this.isLoading = true

        const command = {
          userId: this.user.userId,
          roleName: this.selectedRole,
        }

        await client.assignRole(command)

        this.sweetAlert.success('กำหนด Role สำเร็จ')
        this.$emit('role-assigned')
        this.closeDrawer()
      } catch (error: any) {
        console.error('Error assigning role:', error)

        let errorMsg = 'ไม่สามารถกำหนด Role ได้'
        if (error.response) {
          try {
            const errorData = await error.response.text()
            errorMsg = errorData || errorMsg
          } catch (e) {
            // Use default error message
          }
        } else if (error.message) {
          errorMsg = error.message
        }

        this.sweetAlert.error(errorMsg)
      } finally {
        this.isLoading = false
      }
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

.button-container {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.mobile-btn {
  flex: 1;
  min-width: 150px;
}

@media (max-width: 600px) {
  .button-container {
    flex-direction: column;
  }

  .mobile-btn {
    width: 100%;
  }
}

.form-label {
  font-size: 0.875rem;
  font-weight: 500;
  color: rgba(0, 0, 0, 0.87);
  margin-bottom: 0.5rem;
  display: block;
}
</style>
