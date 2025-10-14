<template>
  <div class="footer-container h-100 d-flex align-center justify-space-between px-4 py-2">
    <!-- Mobile: รวมทุกอย่าง -->
    <div class="footer-content-mobile">
      <VIcon
        icon="ri-heart-3-line"
        color="error"
        size="1.2rem"
        class="mr-2 heart-icon"
      />
      <span class="version-text">Version 1.5.0</span>
      <span class="divider-mobile">|</span>
      <VIcon
        icon="ri-copyright-line"
        color="primary"
        size="1.2rem"
        class="mr-2 copyright-icon"
      />
      <span class="copyright-text">By Development Team</span>
    </div>
    <!-- Desktop: ซ้าย-ขวา -->
    <div class="footer-left d-flex align-center">
      <VIcon
        icon="ri-heart-3-line"
        color="error"
        size="1.2rem"
        class="mr-2 heart-icon"
      />
      <span class="version-text">Version 1.5.1</span>
    </div>
    <div class="footer-right d-flex align-center">
      <div class="align-center copyright-section">
        <VIcon
          icon="ri-copyright-line"
          color="primary"
          size="1.2rem"
          class="mr-2 copyright-icon"
        />
        <span class="copyright-text">By Development Team</span>
      </div>
    </div>
  </div>
</template>

<script setup>
// Footer component logic can be added here if needed
</script>

<style scoped>
.footer-container {
  border-top: 1px solid #dee2e6;
  min-height: 60px;
}

.version-badge {
  background: rgba(255, 255, 255, 0.8);
  padding: 6px 12px;
  border-radius: 20px;
  border: 1px solid rgba(var(--v-theme-primary), 0.2);
  transition: all 0.3s ease;
}

.version-badge:hover {
  background: rgba(255, 255, 255, 1);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.version-text {
  font-size: 0.875rem;
  font-weight: 500;
  color: #495057;
  letter-spacing: 0.5px;
}

.copyright-section {
  background: rgba(var(--v-theme-primary), 0.1);
  padding: 6px 12px;
  border-radius: 15px;
  transition: all 1s ease;
}

.copyright-section:hover {
  background: rgba(var(--v-theme-primary), 0.15);
  transform: translateY(-1px);
}

.copyright-text {
  font-size: 0.875rem;
  font-weight: 500;
  color: rgb(var(--v-theme-primary));
  letter-spacing: 0.3px;
}

.heart-icon {
  animation: heartbeat 1s ease-in-out infinite;
}

.copyright-icon {
  animation: swing 3s ease-in-out infinite;
}

.copyright-icon-mobile {
  opacity: 0.7;
}

/* Animations */
@keyframes heartbeat {
  0%,
  50%,
  100% {
    transform: scale(1);
  }
  25% {
    transform: scale(1.1);
  }
}

@keyframes swing {
  0%,
  100% {
    transform: rotate(0deg);
  }
  25% {
    transform: rotate(5deg);
  }
  75% {
    transform: rotate(-5deg);
  }
}

/* Responsive Design */
@media (max-width: 768px) {
  .footer-container {
    padding: 8px 16px;
    min-height: 50px;
  }

  .version-text {
    font-size: 0.8rem;
  }

  .version-badge {
    padding: 4px 8px;
  }
}
.footer-content-mobile {
  display: none;
}
.divider-mobile {
  margin: 0 8px;
  color: #ccc;
}

@media (max-width: 480px) {
  .footer-content-mobile {
    display: flex !important;
    flex-direction: row;
    align-items: center;
    justify-content: center;
    width: 100%;
    gap: 4px;
    padding: 12px 8px;
    min-height: auto;
    text-align: center;
  }
  .footer-left,
  .footer-right {
    display: none !important;
  }
}
@media (min-width: 481px) {
  .footer-content-mobile {
    display: none !important;
  }
  .footer-left,
  .footer-right {
    display: flex !important;
  }
}

</style>
