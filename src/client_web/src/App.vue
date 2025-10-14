<script setup lang="ts">
// import UpgradeToPro from '@/components/UpgradeToPro.vue'
</script>

<template>
  <VApp class="BG-Content">
    <RouterView />
    <!-- <UpgradeToPro /> -->
  </VApp>
</template>

<style>
@import url('https://fonts.googleapis.com/css2?family=Sarabun:wght@100;200;300;400;500;600;700&display=swap');

* {
  font-family: 'Sarabun';
}

.card-table {
  box-shadow: 2px 2px 8px 4px rgba(0, 0, 0, 0.1) !important;
}

.rounded-popup {
  border-radius: 10px !important;
}

.btn-reset {
  color: #ffff !important;
  background-color: #49c628;
  background-image: linear-gradient(135deg, #66f775 10%, #49c628 100%);
}

.btn-delate {
  color: #ffff !important;
  background-color: #e80505;
  background-image: linear-gradient(135deg, #fd8019 10%, #e80505 100%);
}

.text-form {
  color: black !important;
}

/* .BG-Content {
  background-image: linear-gradient(180deg, rgb(226, 239, 252) 0%, rgba(196, 217, 255, 0.855) 100%) !important;
} */
body {
  background-color: #F4F5FA !important;
}
</style>
