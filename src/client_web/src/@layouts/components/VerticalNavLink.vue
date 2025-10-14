<script lang="ts" setup>
import type { NavLink } from '@layouts/types'
import { useAuthStore } from '@/stores'
const auth = useAuthStore()
defineProps<{
  item: NavLink
}>()
</script>

<template>
  <li
    class="nav-link"
    :class="{ disabled: item.disable }"
    v-if="auth.roles.some(role => item.roles.includes(role))"
  >
    <Component
      :is="item.to ? 'RouterLink' : 'a'"
      :to="item.to"
      :href="item.href"
      :target="item.target"
    >
      <VIcon
        :icon="item.icon || 'ri-checkbox-blank-circle-line'"
        class="nav-item-icon"
      />
      <!-- 👉 Title -->
      <span class="nav-item-title">
        {{ item.title }}
      </span>
      <!-- <span
        class="nav-item-badge"
        :class="item.badgeClass"
      >
        {{ item.badgeContent }}
      </span> -->
    </Component>
  </li>
</template>

<style lang="scss">
.layout-vertical-nav {
  .nav-link a {
    display: flex;
    align-items: center;
    cursor: pointer;
  }
}
.nav-link{
   color: rgb(var(--v-theme-primary));
}
</style>
