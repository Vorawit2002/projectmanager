<template>
  <v-dialog
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    max-width="500"
    persistent
  >
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between bg-primary pa-4">
        <span class="text-h6 text-white">
          <v-icon
            icon="ri-shield-user-line"
            class="mr-2"
          />
          กำหนด Role
        </span>
        <v-btn
          icon
          variant="text"
          size="small"
          @click="closeDialog"
        >
          <v-icon
            color="white"
            icon="ri-close-line"
          />
        </v-btn>
      </v-card-title>

      <v-card-text class="pt-6">
        <v-row>
          <v-col cols="12">
            <div class="mb-4">
              <div class="text-subtitle-2 text-medium-emphasis mb-1">ผู้ใช้งาน</div>
              <div class="d-flex align-center">
                <v-avatar
                  size="40"
                  class="mr-3"
                >
                  <v-img
                    v-if="user?.imageProfile"
                    :src="user.imageProfile"
                    :alt="user.firstName + ' ' + user.lastName"
                  />
                  <v-icon
                    v-else
                    icon="ri-user-line"
                    size="24"
                  />
                </v-avatar>
                <div>
                  <div class="font-weight-medium">{{ user?.firstName }} {{ user?.lastName }}</div>
                  <div class="text-caption text-medium-emphasis">{{ user?.email }}</div>
                </div>
              </div>
            </div>
          </v-col>

          <v-col cols="12">
            <div class="mb-2">
              <div class="text-subtitle-2 text-medium-emphasis mb-1">Role ปัจจุบัน</div>
              <div>
                <v-chip
                  v-for="role in user?.roles"
                  :key="role"
                  :color="getRoleColor(role)"
                  size="small"
                  class="mr-1"
                >
                  {{ role }}
                </v-chip>
                <span
                  v-if="!user?.roles || user.roles.length === 0"
                  class="text-medium-emphasis"
                >
                  ไม่มี Role
                </span>
              </div>
            </div>
          </v-col>

          <v-col cols="12">
            <v-select
              v-model="selectedRole"
              :items="roleOptions"
              label="เลือก Role ใหม่"
              color="primary"
              variant="outlined"
              :error-messages="errorMessage"
              required
            >
              <template v-slot:item="{ props, item }">
                <v-list-item
                  v-bind="props"
                  :title="item.title"
                >
                  <template v-slot:prepend>
                    <v-chip
                      :color="getRoleColor(item.value)"
                      size="small"
                    >
                      {{ item.title }}
                    </v-chip>
                  </template>
                </v-list-item>
              </template>
            </v-select>
          </v-col>

          <v-col cols="12">
            <v-alert
              type="info"
              variant="tonal"
              density="compact"
              class="text-caption"
            >
              <strong>หมายเหตุ:</strong> การกำหนด Role ใหม่จะแทนที่ Role เดิมทั้งหมด
            </v-alert>
          </v-col>
        </v-row>
      </v-card-text>

      <v-card-actions class="pa-4">
        <v-spacer />
        <v-btn
          variant="outlined"
          @click="closeDialog"
          :disabled="isLoading"
        >
          ยกเลิก
        </v-btn>
        <v-btn
          color="primary"
          @click="assignRole"
          :loading="isLoading"
          :disabled="!selectedRole"
        >
          <v-icon
            icon="ri-save-line"
            class="mr-2"
          />
          บันทึก
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { defineComponent, PropType } from 'vue'
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'

const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'AssignRoleDialog',
  props: {
    modelValue: {
      type: Boolean,
      required: true,
    },
    user: {
      type: Object as PropType<any>,
      required: true,
    },
  },
  emits: ['update:modelValue', 'role-assigned'],
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
      errorMessage: '',
    }
  },
  methods: {
    getRoleColor(role: string): string {
      const colors: Record<string, string> = {
        Admin: 'error',
        Manager: 'warning',
        User: 'primary',
        Viewer: 'secondary',
      }
      return colors[role] || 'default'
    },
    closeDialog() {
      this.$emit('update:modelValue', false)
      this.selectedRole = null
      this.errorMessage = ''
    },
    async assignRole() {
      if (!this.selectedRole) {
        this.errorMessage = 'กรุณาเลือก Role'
        return
      }

      try {
        this.isLoading = true
        this.errorMessage = ''

        const command = {
          userId: this.user.userId,
          roleName: this.selectedRole,
        }

        await client.assignRole(command)

        this.sweetAlert.success('กำหนด Role สำเร็จ')
        this.$emit('role-assigned')
        this.closeDialog()
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
.v-card-title {
  position: sticky;
  top: 0;
  z-index: 1;
}
</style>
