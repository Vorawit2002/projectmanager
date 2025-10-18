<script lang="ts" setup>
import type { NavSectionTitle } from '@layouts/types'
import { useAuthStore } from '@/stores/auth'
import { computed } from 'vue'

const props = defineProps<{
  item: NavSectionTitle
}>()

const authStore = useAuthStore()

// Check if user has required roles
const hasAccess = computed(() => {
  if (!props.item.roles || props.item.roles.length === 0) {
    return true // No roles required, show to everyone
  }
  
  return props.item.roles.some(role => authStore.roles.includes(role))
})
</script>

<template>
  <li
    v-if="hasAccess"
    class="nav-section-title"
  >
    <div class="title-wrapper">
      <!-- eslint-disable vue/no-v-text-v-html-on-component -->
      <span
        class="title-text"
        v-text="item.heading"
      />
      <!-- eslint-enable vue/no-v-text-v-html-on-component -->
    </div>
  </li>
</template>

<style scoped>
.nav-section-title {
  color: rgb(var(--v-theme-primary)) !important; 
}
</style>
