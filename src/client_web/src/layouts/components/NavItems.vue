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
      roles: ['Admin', 'Manager', 'User', 'Viewer'],
    }"
  />

  <!-- 👉 Activity Plans -->
  <VerticalNavLink
    :item="{
      title: 'นัดหมาย',
      icon: 'ri-id-card-line',
      to: '/CustomerAppointmentListView',
      roles: ['Admin', 'Manager', 'User', 'Viewer'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'มุมมองปฏิทิน',
      icon: 'ri-calendar-todo-fill',
      to: '/Calendar',
      roles: ['Admin', 'Manager', 'User', 'Viewer'],
    }"
  />

  

  <!-- 👉 Check-in/Check-out -->
  <VerticalNavLink
    :item="{
      title: 'ลงเวลาเข้า - ออกงาน',
      icon: 'ri-time-line',
      to: '/OnsiteViews',
      roles: ['Admin', 'Manager', 'User', 'Viewer'],
    }"
  />

  <!-- 👉 Reports -->
  <VerticalNavLink
    :item="{
      title: 'รายงานผล',
      icon: 'ri-file-chart-line',
      to: '/ReportCustomerAppointmentPlanListView',
      roles: ['Admin', 'Manager', 'User', 'Viewer'],
    }"
  />

  <!-- 👉 Master Data Section -->
  <VerticalNavSectionTitle
    :item="{
      heading: 'ตั้งค่าข้อมูล',
    }"
  />

  <!-- 👉 Departments -->
  <VerticalNavLink
    :item="{
      title: 'แผนก',
      icon: 'ri-building-line',
      to: '/MasterData/DepartmentListView',
      roles: ['Admin', 'Manager'],
    }"
  />

  <!-- 👉 Organizations -->
  <VerticalNavLink
    :item="{
      title: 'หน่วยงาน',
      icon: 'ri-community-line',
      to: '/MasterData/OrganizationListView',
      roles: ['Admin', 'Manager'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'โครงการ',
      icon: 'ri-article-line',
      to: '/MasterData/ProjectListView',
      roles: ['Admin', 'Manager', 'User', 'Viewer'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'ผู้ติดต่อโครงการ',
      icon: 'ri-group-line',
      to: '/MasterData/ProjectContactListView',
      roles: ['Admin', 'Manager', 'User', 'Viewer'],
    }"
  />

  <VerticalNavLink
    :item="{
      title: 'ผู้ติดต่อหน่วยงาน',
      icon: 'ri-contacts-line',
      to: '/MasterData/OrganizationContactListView',
      roles: ['Admin', 'Manager'],
    }"
  />

  <!-- 👉 Event Types -->
  <VerticalNavLink
    :item="{
      title: 'ประเภทกิจกรรม',
      icon: 'ri-home-office-line',
      to: '/MasterData/EventTypeListView',
      roles: ['Admin', 'Manager'],
    }"
  />

  <!-- 👉 Master Data Section -->
  <VerticalNavSectionTitle
    :item="{
      heading: 'ตั้งค่าระบบ',
    }"
  />

  <!-- 👉 User Management (Admin only) -->
  <VerticalNavLink
    :item="{
      title: 'จัดการผู้ใช้งาน',
      icon: 'ri-user-settings-line',
      to: '/MasterData/UserListView',
      roles: ['Admin'],
    }"
  />

  <!-- 👉 Employees -->
  <VerticalNavLink
    :item="{
      title: 'ผู้ใช้งานระบบ',
      icon: 'ri-user-line',
      to: '/MasterData/EmployeeListView',
      roles: ['Admin', 'Manager'],
    }"
  />
</template>
