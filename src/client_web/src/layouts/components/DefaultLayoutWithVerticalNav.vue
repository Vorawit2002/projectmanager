<script lang="ts" setup>
import NavItems from '@/layouts/components/NavItems.vue'
// import logo from '@images/logo.svg?raw'
import CRMLogo from '@/assets/images/logos/CRMLogoV2.png'
import NTiLogofotor from '@/assets/images/logos/NTi-Logo-fotor.png'
import notification from '@/layouts/components/Notification.vue'
import VerticalNavLayout from '@layouts/components/VerticalNavLayout.vue'
// Components
import Footer from '@/layouts/components/Footer.vue'
import UserProfile from '@/layouts/components/UserProfile.vue'

// Define components
const components = {
  notification,
}

const data = {
  logo: NTiLogofotor,
  CRMLogo: CRMLogo,
}
</script>

<template>
  <VerticalNavLayout>
    <!-- 👉 navbar -->
    <template #navbar="{ toggleVerticalOverlayNavActive }">
      <div class="d-flex h-100 align-center">
        <!-- 👉 Vertical nav toggle in overlay mode -->
        <IconBtn
          class="ms-n3 d-lg-none"
          @click="toggleVerticalOverlayNavActive(true)"
        >
          <VIcon icon="ri-menu-line" />
        </IconBtn>

        <!-- 👉 Search -->
        <!-- <div class="d-flex align-center cursor-pointer" style="user-select: none;">

          <IconBtn>
            <VIcon icon="ri-search-line" />
          </IconBtn>

          <span class="d-none d-md-flex align-center text-disabled">
            <span class="me-3">Search</span>
            <span class="meta-key">&#8984;K</span>
          </span>
        </div> -->

        <VSpacer />

        <!-- <IconBtn href="https://github.com/themeselection/materio-vuetify-vuejs-admin-template-free" target="_blank"
          rel="noopener noreferrer">
          <VIcon icon="ri-github-fill" />
        </ไอคอน> -->
        <notification />
        <!-- <NavbarThemeSwitcher class="me-2" /> -->

        <UserProfile />
      </div>
    </template>

    <template #vertical-nav-header="{ toggleIsOverlayNavActive }">
      <center>
        <RouterLink
          to="/dashboard"
          class="app-logo app-title-wrapper text-center justify-center"
        >
          <!-- eslint-disable vue/no-v-html -->
          <img
            class="d-flex justify-center"
            :src="CRMLogo"
            width="180"
          />
          <!-- eslint-enable -->
          <!-- <span class="text-menu text-primary-darken-0">CRM</span> -->
        </RouterLink>
      </center>

      <IconBtn
        class="d-block d-lg-none"
        @click="toggleIsOverlayNavActive(false)"
      >
        <VIcon icon="ri-close-line" />
      </IconBtn>
    </template>

    <template #vertical-nav-content>
      <NavItems />
    </template>

    <!-- 👉 Pages -->
    <slot />

    <!-- 👉 Footer -->
    <template #footer>
      <Footer />
    </template>
  </VerticalNavLayout>
</template>

<style lang="scss" scoped>
.meta-key {
  border: thin solid rgba(var(--v-border-color), var(--v-border-opacity));
  border-radius: 6px;
  block-size: 1.5625rem;
  line-height: 1.3125rem;
  padding-block: 0.125rem;
  padding-inline: 0.25rem;
}

.app-logo {
  display: flex;
  align-items: center;
  column-gap: 0.75rem;

  .app-logo-title {
    font-size: 1.25rem;
    font-weight: 500;
    line-height: 1.75rem;
    text-transform: uppercase;
  }
}

.text-menu {
  font-size: 34px;
  font-weight: 800;
  font-style: italic;
}
</style>
