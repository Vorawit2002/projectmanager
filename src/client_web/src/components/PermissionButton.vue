<template>
  <v-tooltip
    v-if="!hasPermission"
    location="top"
  >
    <template v-slot:activator="{ props }">
      <v-btn
        v-bind="{ ...props, ...$attrs }"
        :disabled="true"
        :color="color"
        :variant="variant"
        :size="size"
        :icon="icon"
      >
        <slot></slot>
      </v-btn>
    </template>
    <span>{{ tooltipMessage }}</span>
  </v-tooltip>
  
  <v-btn
    v-else
    v-bind="$attrs"
    :color="color"
    :variant="variant"
    :size="size"
    :icon="icon"
    @click="handleClick"
  >
    <slot></slot>
  </v-btn>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { usePermissions, type PermissionOptions } from '@/composables/usePermissions'
import { useSweetAlertStore } from '@/stores'

interface Props {
  action: 'create' | 'edit' | 'delete' | 'view'
  permissionOptions?: PermissionOptions
  color?: string
  variant?: string
  size?: string
  icon?: boolean
  showAlert?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  color: 'primary',
  variant: 'elevated',
  size: 'default',
  icon: false,
  showAlert: true,
})

const emit = defineEmits<{
  (e: 'click', event: MouseEvent): void
}>()

const permissions = usePermissions()
const sweetAlert = useSweetAlertStore()

const hasPermission = computed(() => {
  switch (props.action) {
    case 'create':
      return permissions.canCreate.value
    case 'edit':
      return permissions.canEdit(props.permissionOptions)
    case 'delete':
      return permissions.canDelete(props.permissionOptions)
    case 'view':
      return permissions.canView(props.permissionOptions)
    default:
      return false
  }
})

const tooltipMessage = computed(() => {
  return permissions.getPermissionErrorMessage()
})

const handleClick = (event: MouseEvent) => {
  if (!hasPermission.value) {
    if (props.showAlert) {
      sweetAlert.warning(tooltipMessage.value)
    }
    return
  }
  emit('click', event)
}
</script>
