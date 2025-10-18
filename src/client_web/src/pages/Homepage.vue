<template>
  <v-content>
    <v-row
      class="mb-8 align-center justify-center px-5 py-5 elevation-5 scroll-reveal"
      ref="mainSection"
    >
      <!-- ส่วนที่แสดงการ์ด 2 (แสดงในหน้าจอโทรศัพท์) -->
      <v-col
        cols="12"
        class="d-md-none scroll-reveal-item"
        data-delay="0.1s"
      >
        <div class="card floating-card">
          <v-img
            :src="pexels_fauxels3"
            alt=""
            class="mobile-image"
            style="border-radius: 0px 30px 0px 30px; filter: blur(1px)"
            height="300"
            cover
          ></v-img>
        </div>
      </v-col>

      <!-- ส่วนที่แสดงข้อความ -->
      <v-col
        cols="12"
        md="5"
        class="mt-md-16 mb-md-16 px-2 scroll-reveal-item"
        data-delay="0.2s"
      >
        <span class="text-Potal typing-text">Customer Relationship Management</span>

        <p class="mb-6 mt-md-6 font-sub fade-in-text">
          CRM มุ่งเน้นการจัดการข้อมูลและความสัมพันธ์กับลูกค้า เพื่อสร้างและรักษาความสัมพันธ์ที่ดี
          และช่วยให้ธุรกิจเข้าถึงความต้องการของลูกค้าอย่างลึกซึ้งมากขึ้น.
        </p>

        <div class="d-flex flex-wrap gap-4 mt-md-8">
          <v-btn
            @click="GoLogin"
            class="btn-glow magnetic-button"
            size="large"
          >
            <i
              class="ri-login-box-line mr-2"
              size="30"
            ></i>
            เข้าสู่ระบบ
          </v-btn>

          <v-btn
            @click="GoRegister"
            class="btn-outline magnetic-button"
            size="large"
            variant="outlined"
          >
            <i
              class="ri-user-add-line mr-2"
              size="30"
            ></i>
            สมัครสมาชิก
          </v-btn>
        </div>
      </v-col>

      <!-- ส่วนที่แสดงการ์ด (แสดงในหน้าจอคอม) -->
      <v-col
        cols="12"
        md="5"
        class="d-none d-md-block mt-10 mb-10 text-end scroll-reveal-item"
        data-delay="0.3s"
      >
        <div class="morphing-card">
          <v-img
            :src="pexels_fauxels3"
            alt=""
            style="min-width: 100%; border-radius: 0px 85px 0px 85px; filter: blur(1px)"
          ></v-img>
        </div>
      </v-col>
    </v-row>

    <v-row
      class="mb-5 align-center justify-center px-5 py-5 elevation-0 scroll-reveal"
      ref="featureSection"
    >
      <VCol
        cols="12"
        md="10"
        class="scroll-reveal-item"
        data-delay="0.4s"
      >
        <!-- VCard CRMUltraFeature -->
        <CRMUltraFeature />
      </VCol>
    </v-row>
  </v-content>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import undraw_realtime from '@images/pages/undraw_real-time.png'
import pexels_fauxels3 from '@images/pages/pexels-fauxels-3182773.jpg'
import AnalyticsTotalProfitLineCharts from '@/views/dashboard/AnalyticsTotalProfitLineCharts.vue'
import CardStatisticsVertical from '@core/components/cards/CardStatisticsVertical.vue'
import AnalyticsBarCharts from '@/views/dashboard/AnalyticsBarCharts.vue'
import CRMUltraFeature from '@/components/CRMUltraFeature.vue'
import { useAuthStore } from '@/stores'

export default defineComponent({
  name: 'CustomerAppointmentPlanListView',
  components: { AnalyticsTotalProfitLineCharts, CardStatisticsVertical, AnalyticsBarCharts, CRMUltraFeature },
  data() {
    return {
      undraw_realtime: undraw_realtime,
      pexels_fauxels3: pexels_fauxels3,
      totalProfit: {
        title: 'Total Profit',
        color: 'secondary',
        icon: 'ri-pie-chart-2-line',
        stats: '$25.6k',
        change: 42,
        subtitle: 'Weekly Project',
      },
      newProject: {
        title: 'New Project',
        color: 'primary',
        icon: 'ri-file-word-2-line',
        stats: '862',
        change: -18,
        subtitle: 'Yearly Project',
      },
      auth: useAuthStore(),
      observer: null as IntersectionObserver | null,
    }
  },
  async mounted() {
    let logout = this.$route.query.logout
    if (logout === 'true') {
      await this.auth.logout()
      this.$router.replace({ query: {} }) // ลบ ?logout=true
    }
    let portal = this.$route.query.portal
    if (portal === 'true') {
      this.$router.push('/login')
      return
    }
    const isIndexPage = this.$route.path === '/'
    if (isIndexPage && this.auth.token && this.auth.roles?.includes('CRM')) {
      this.$router.push('/dashboard')
    }

    // Initialize scroll animations
    this.initScrollAnimations()
  },
  beforeUnmount() {
    if (this.observer) {
      this.observer.disconnect()
    }
  },
  methods: {
    GoLogin() {
      this.$router.push('/login')
    },
    GoRegister() {
      this.$router.push('/register')
    },
    initScrollAnimations() {
      // Create intersection observer
      this.observer = new IntersectionObserver(
        entries => {
          entries.forEach(entry => {
            if (entry.isIntersecting) {
              const element = entry.target as HTMLElement
              const delay = element.getAttribute('data-delay') || '0s'

              // Add active class with delay
              setTimeout(() => {
                element.classList.add('active')
              }, parseFloat(delay) * 1000)

              // Stop observing this element
              this.observer?.unobserve(element)
            }
          })
        },
        {
          threshold: 0.1,
          rootMargin: '50px 0px -50px 0px',
        },
      )

      // Observe all scroll reveal elements
      const scrollRevealElements = document.querySelectorAll('.scroll-reveal-item')
      scrollRevealElements.forEach(element => {
        this.observer?.observe(element)
      })

      // Add parallax effect on scroll
      this.initParallaxEffect()
    },

    initParallaxEffect() {
      let ticking = false

      const updateParallax = () => {
        const scrolled = window.pageYOffset
        const parallaxElements = document.querySelectorAll('.morphing-card, .floating-card')

        parallaxElements.forEach(element => {
          if (element instanceof HTMLElement) {
            const speed = 0.3
            const yPos = -(scrolled * speed)
            element.style.transform = `translate3d(0, ${yPos}px, 0)`
          }
        })

        ticking = false
      }

      const requestTick = () => {
        if (!ticking) {
          requestAnimationFrame(updateParallax)
          ticking = true
        }
      }

      window.addEventListener('scroll', requestTick)
    },
  },
})
</script>

<style scoped>
/* Scroll Reveal Base Styles */
.scroll-reveal-item {
  opacity: 0;
  transform: translateY(50px);
  transition: all 0.8s cubic-bezier(0.25, 0.46, 0.45, 0.94);
}

.scroll-reveal-item.active {
  opacity: 1;
  transform: translateY(0);
}

/* Modern Text Animations */
.text-Potal {
  background: url('../assets/images/pages/black-BG-Text.jpg') center;
  background-size: cover;
  background-clip: text;
  -webkit-background-clip: text;
  color: transparent;
  font-size: clamp(24px, 6vw, 65px);
  font-weight: bolder;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.6);
  position: relative;
  overflow: hidden;
}

.scroll-reveal-item.active .text-Potal {
  animation: textReveal 1.2s ease-out forwards;
}

@keyframes textReveal {
  0% {
    transform: translateY(100px);
    opacity: 0;
  }
  50% {
    transform: translateY(-10px);
    opacity: 0.7;
  }
  100% {
    transform: translateY(0);
    opacity: 1;
  }
}

/* Typing Effect */
.typing-text::after {
  position: absolute;
  right: -10px;
  top: 0;
  bottom: 0;
  width: 3px;
  background: linear-gradient(135deg, #0c359e, #2b3086);
  animation: blink 1s infinite;
}

@keyframes blink {
  0%,
  50% {
    opacity: 1;
  }
  51%,
  100% {
    opacity: 0;
  }
}

/* Fade In Text */
.fade-in-text {
  opacity: 0;
  transition: opacity 1s ease-in-out 0.3s;
}

.scroll-reveal-item.active .fade-in-text {
  opacity: 1;
}

/* Modern Card Animations */
.floating-card {
  transition: all 0.6s cubic-bezier(0.23, 1, 0.32, 1);
}

.scroll-reveal-item.active .floating-card {
  animation: floatIn 1.5s ease-out forwards;
}

@keyframes floatIn {
  0% {
    transform: translateY(100px) scale(0.8);
    opacity: 0;
  }
  60% {
    transform: translateY(-20px) scale(1.05);
    opacity: 0.8;
  }
  100% {
    transform: translateY(0) scale(1);
    opacity: 1;
  }
}

.morphing-card {
  transition: all 0.8s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  transform-style: preserve-3d;
}

.scroll-reveal-item.active .morphing-card {
  animation: morphIn 1.8s ease-out forwards;
}

@keyframes morphIn {
  0% {
    transform: perspective(1000px) rotateX(45deg) translateY(100px);
    opacity: 0;
  }
  50% {
    transform: perspective(1000px) rotateX(-5deg) translateY(-20px);
    opacity: 0.7;
  }
  100% {
    transform: perspective(1000px) rotateX(0deg) translateY(0);
    opacity: 1;
  }
}

/* Mobile Image Styling */
.mobile-image {
  width: 100%;
  object-fit: cover;
  aspect-ratio: 16/9;
  border-radius: 0px 60px 0px 60px;
}

@media (max-width: 768px) {
  .mobile-image {
    aspect-ratio: 4/3;
    height: 250px;
  }
}

@media (max-width: 480px) {
  .mobile-image {
    aspect-ratio: 3/2;
    height: 200px;
  }
}
.magnetic-button {
  position: relative;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
}

.magnetic-button::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
  transition: left 0.5s;
}

.magnetic-button:hover::before {
  left: 100%;
}

.magnetic-button:hover {
  transform: translateY(-3px) scale(1.05);
  box-shadow: 0 10px 30px rgba(102, 126, 234, 0.6);
}

/* Enhanced Primary Button */
.btn-glow {
  background: linear-gradient(135deg, #0c359e 0%, #2b3086 100%);
  color: white;
  border: none;
  padding: 14px 28px;
  font-size: 1rem;
  font-weight: 500;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: inline-flex;
  align-items: center;
  gap: 10px;
  font-family: 'Inter', sans-serif;
  min-width: 200px;
  justify-content: center;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 14px rgba(59, 130, 246, 0.3);
}

/* Outline Button */
.btn-outline {
  background: transparent;
  color: #0c359e;
  border: 2px solid #0c359e;
  padding: 14px 28px;
  font-size: 1rem;
  font-weight: 500;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: inline-flex;
  align-items: center;
  gap: 10px;
  font-family: 'Inter', sans-serif;
  min-width: 200px;
  justify-content: center;
  position: relative;
  overflow: hidden;
}

.btn-outline::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(135deg, #0c359e 0%, #2b3086 100%);
  transition: left 0.5s;
  z-index: -1;
}

.btn-outline:hover {
  color: white;
  border-color: #2b3086;
  transform: translateY(-3px) scale(1.05);
  box-shadow: 0 10px 30px rgba(43, 48, 134, 0.4);
}

.btn-outline:hover::before {
  left: 0;
}

.scroll-reveal-item.active .btn-glow {
  animation: buttonSlideIn 1s ease-out 0.6s forwards;
}

.scroll-reveal-item.active .btn-outline {
  animation: buttonSlideIn 1s ease-out 0.7s forwards;
}

@keyframes buttonSlideIn {
  0% {
    transform: translateX(-100px);
    opacity: 0;
  }
  100% {
    transform: translateX(0);
    opacity: 1;
  }
}

/* Button Container */
.gap-4 {
  gap: 1rem;
}

@media (max-width: 600px) {
  .gap-4 {
    gap: 0.75rem;
  }
  
  .btn-glow,
  .btn-outline {
    min-width: 160px;
    padding: 12px 20px;
    font-size: 0.9rem;
  }
}

/* Font Styles */
.font-sub {
  font-size: 18px;
  line-height: 1.6;
}

/* Responsive Font Sizes */
@media (max-width: 1200px) {
  .text-Potal {
    font-size: 48px;
  }
}

@media (max-width: 768px) {
  .text-Potal {
    font-size: 32px;
  }
}

@media (max-width: 480px) {
  .text-Potal {
    font-size: 24px;
  }
}

/* Smooth Scroll */
html {
  scroll-behavior: smooth;
}

/* Performance Optimization */
.scroll-reveal-item {
  will-change: transform, opacity;
}

.morphing-card,
.floating-card {
  will-change: transform;
}

/* Modern Glassmorphism Effect */
.scroll-reveal-item.active .v-card {
  backdrop-filter: blur(10px);
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

/* Pulse Animation for Loading State */
@keyframes pulse {
  0% {
    transform: scale(1);
    opacity: 1;
  }
  50% {
    transform: scale(1.05);
    opacity: 0.8;
  }
  100% {
    transform: scale(1);
    opacity: 1;
  }
}

/* Hover Effects */
.scroll-reveal-item:hover {
  transform: translateY(-5px);
  transition: transform 0.3s ease;
}

/* Stagger Animation for Multiple Elements */
.scroll-reveal-item:nth-child(1) {
  transition-delay: 0.1s;
}
.scroll-reveal-item:nth-child(2) {
  transition-delay: 0.2s;
}
.scroll-reveal-item:nth-child(3) {
  transition-delay: 0.3s;
}
.scroll-reveal-item:nth-child(4) {
  transition-delay: 0.4s;
}
</style>
