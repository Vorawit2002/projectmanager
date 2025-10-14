<script lang="ts" setup>
import VerticalNavSectionTitle from '@/@layouts/components/VerticalNavSectionTitle.vue'
import VerticalNavLink from '@layouts/components/VerticalNavLink.vue'
</script>

<template>
  <VerticalNavLink
    :item="{
      title: 'Dashboard',
      icon: 'ri-bar-chart-fill',
      to: '/dashboard',
      roles: ['Manager', 'Admin'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'นัดหมาย',
      icon: 'ri-id-card-line',
      to: '/CustomerAppointmentListView',
      roles: ['CRM'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'มุมมองปฏิทิน',
      icon: 'ri-calendar-todo-fill',
      to: '/Calendar',
      roles: ['CRM'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'ลงเวลาเข้า - ออกงาน',
      icon: 'ri-time-line',
      to: '/OnsiteViews',
      roles: ['Manager', 'CRM'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'รายงานผล',
      icon: 'ri-file-chart-line',
      to: '/ReportCustomerAppointmentPlanListView',
      roles: ['Manager', 'Admin'],
    }"
  />

  <!-- 👉 Apps & Pages -->
  <VerticalNavSectionTitle
    :item="{
      heading: 'ตั้งค่าข้อมูล',
    }"
  />
  <VerticalNavLink
    :item="{
      title: 'หน่วยงาน',
      icon: 'ri-community-line',
      to: '/MasterData/OrganizationListView',
      roles: ['CRM'],
    }"
  />
  <VerticalNavLink
    :item="{
      title: 'ผู้ติดต่อหน่วยงาน',
      icon: 'ri-contacts-line',
      to: '/MasterData/OrganizationContactListView',
      roles: ['CRM'],
    }"
  />
  <VerticalNavLink
    :item="{
      title: 'โครงการ',
      icon: 'ri-article-line',
      to: '/MasterData/ProjectListView',
      roles: ['CRM'],
    }"
  />
  <VerticalNavLink
    :item="{
      title: 'ผู้ติดต่อโครงการ',
      icon: 'ri-group-line',
      to: '/MasterData/ProjectContactListView',
      roles: ['CRM'],
    }"
  />
  <VerticalNavLink
    :item="{
      title: 'ประเภทกิจกรรม',
      icon: 'ri-home-office-line',
      to: '/MasterData/EventTypeListView',
      roles: ['Admin'],
    }"
  />
</template>
