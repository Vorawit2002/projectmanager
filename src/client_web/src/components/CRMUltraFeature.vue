<template>
  <div class="text-center text-Crm mb-5">CRM Feature</div>
  <v-row class="d-flex justify-start px-0 equal-height-row">
    <v-col
      v-for="(card, index) in cardCRMU"
      :key="index"
      cols="12"
      md="3"
      class="px-5 py-5 px-md-8 d-flex align-stretch"
    >
      <v-card
        width="100%"
        class="mx-auto mb-8 Card-Radius d-flex flex-column equal-height-card hover-effect animate__animated animate__bounceInUp"
      >
        <v-img
          class="align-end mt-8 text-white cursor-pointer IMGLogo flex-shrink-0 z-10"
          height="100"
          :src="card.Image"
        />
        <template v-slot:loader>
          <v-progress-linear model-value="100"></v-progress-linear>
        </template>
        <v-card-text class="text-center mt-5 text-h5 font-weight-bold flex-shrink-0 z-10">
          {{ card.title }}
        </v-card-text>

        <v-card-title class="text-center flex-grow-1 d-flex justify-center card-detail-full z-10">
          <div class="text-center">{{ card.detail }}</div>
        </v-card-title>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import management from '@/assets/images/logos/management.png'
import schedule from '@/assets/images/logos/schedule.png'
import Graph from '@/assets/images/logos/Graph.png'
import campaign from '@/assets/images/logos/campaign.png'
import support from '@/assets/images/logos/support.png'
import headphone from '@/assets/images/logos/headphone.png'
import document from '@/assets/images/logos/document.png'
import reporting from '@/assets/images/logos/reporting.png'
import WebAnalysis from '@/assets/images/logos/web-analysis.png'

export default defineComponent({
  name: 'CRMUltraFeature',
  data() {
    return {
      cardCRMU: [
        {
          Image: management,
          title: 'Customer Management',
          detail: 'บริหารจัดการข้อมูลลูกค้าอย่างเป็นระบบ และสร้างความสัมพันธ์กับลูกค้า นำไปสู่ยอดขายที่เพิ่มขึ้น',
        },
        {
          Image: schedule,
          title: 'Activity Management',
          detail: 'วางแผนการติดต่อลูกค้าได้อย่างมีประสิทธิภาพไม่พลาดนัดด้วยระบบแจ้งเตือน',
        },
        {
          Image: Graph,
          title: 'Sales Force Automation',
          detail: 'จัดการกระบวนการขายให้เป็นมาตรฐานทำใบเสนอราคา ใบสั่งขาย ได้ทันทีเพื่อสร้างทีมขายมืออาชีพ',
        },
        {
          Image: campaign,
          title: 'Marketing Management',
          detail: 'วางแผนการตลาดให้เหมาะสมกับกลุ่มเป้าหมาย และมีประสิทธิภาพ',
        },
        {
          Image: headphone,
          title: 'Service Management',
          detail: 'บริหารงานหลังการขายเพื่อเพิ่มประสิทธิภาพการบริการ และความพึงพอใจลูกค้า',
        },
        {
          Image: document,
          title: 'Approve Center',
          detail: 'ตรวจสอบและสรุปเอกสารออนไลน์',
        },
        {
          Image: reporting,
          title: 'Reports & Dashboards',
          detail: 'แสดงข้อมูลในรูปแบบแผนภูมิกราฟช่วยให้การวิเคราะห์ข้อมูลขององค์กรเป็นเรื่องง่าย',
        },
        {
          Image: WebAnalysis,
          title: 'Reports Analysis',
          detail: 'สามารถดูรายงานการขายได้ทุกที่ ทุกเวลา ไม่ต้องรอสรุปข้อมูลประจำปี',
        },
      ],
    }
  },
})
</script>

<style scoped>
.Card-Radius {
  border-radius: 12px;
  /* --font-color: #323232; */
  --font-color-sub: #666;
  --bg-color: #fff;
  --main-color: #222566;
  --main-focus: #2d8cf0;
  background: var(--bg-color);
  border: 2px solid var(--main-color);
  box-shadow: 4px 4px var(--main-color);
}

.text-Crm {
  font-size: 40px;
  font-weight: bolder;
  color: #222566;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.3);
}

.card-detail-full {
  white-space: normal !important;
  font-size: 16px !important;
  /* font-weight: normal !important; */
}

.hover-effect {
  position: relative;
  overflow: hidden;
  color: #fff !important;
}

.hover-effect::before {
  content: '';
  position: absolute;
  top: -100%; /* เริ่มต้นที่ด้านบน (ซ่อนอยู่) */
  left: 0;
  width: 100%;
  height: 100%;
  background: rgb(var(--v-theme-primary));
  transition: top 0.3s ease; /* animation สำหรับการไหลลงมา */
  z-index: 1;
  color: #fff !important;
}
/* เมื่อ hover ให้ pseudo-element ไหลลงมา */
.hover-effect:hover::before {
  top: 0; /* เลื่อนลงมาให้เต็มพื้นที่ */
  color: #fff !important;
}

/* ให้ข้อความด้านในเป็นสีขาวเมื่อ hover */
.hover-effect:hover,
.hover-effect:hover .text-Crm,
.hover-effect:hover .card-detail-full,
.hover-effect:hover .v-card-title,
.hover-effect:hover .v-card-text,
.hover-effect:hover .IMGLogo {
  color: #fff !important;
  z-index: 10;
}

/* responsive สำหรับ mobile */
@media (max-width: 960px) {
  .equal-height-card {
    min-height: auto;
  }
}
.z-10 {
  z-index: 10;
}
</style>
