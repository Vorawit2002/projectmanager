# Research Document: Project Frontend Architecture (`client_web`)

เอกสารฉบับนี้วิเคราะห์และสรุปโครงสร้างเชิงลึกของ Project Frontend เพื่อใช้เป็นคู่มือสำหรับ Developer ในการพัฒนาต่อยอดและรักษามาตรฐานของโค้ด (Coding Standards)

---

## 1. Technology Stack Overview
*   **Core Framework**: [Vue.js 3](https://vuejs.org/) (Composition API & Options API)
*   **Language**: [TypeScript](https://www.typescriptlang.org/) (Strict typing encouraged)
*   **Build Tool**: [Vite](https://vitejs.dev/) (Fast HMR & Build)
*   **UI Framework**: [Vuetify 3](https://vuetifyjs.com/) (Material Design Component Library)
*   **State Management**: [Pinia](https://pinia.vuejs.org/)
*   **Routing**: [Vue Router 4](https://router.vuejs.org/)
*   **API Client**: [NSwag](https://github.com/RicoSuter/NSwag) (Auto-generated TypeScript Client)
*   **Icons**: [Iconify](https://iconify.design/) via `@iconify/vue`
*   **Utils**: `dayjs` or `moment` (check package.json), `sweetalert2` for alerts.

---

## 2. Project Directory Structure
โครงสร้างโฟลเดอร์ถูกจัดระเบียบตามหน้าที่ (Feature-based & Layer-based):

```
src/client_web/src/
├── @core/                  # Core System Components & Logic (จาก Template Materio)
│   ├── components/         # Global Components (e.g., Cards, Custom Inputs)
│   ├── scss/               # Global Styles, Variables, Mixins
│   └── utils/              # Core Utilities
├── components/             # Reusable Project Components
├── layouts/                # Vue Layouts (โครงหน้าเว็บ)
│   ├── default.vue         # Layout หลัก (Sidebar + Navbar)
│   └── blank.vue           # Layout เปล่า (Login, Register, Error Pages)
├── pages/                  # Route Views (File-based routing entry points)
├── plugins/                # Plugin Registrations (Vuetify, Router, Pinia, etc.)
├── router/                 # Router Configuration & Guards
├── stores/                 # Pinia Stores (Global State)
├── styles/                 # Project specific styles
├── utils/                  # Helper Functions
│   ├── NavigationGenerator.ts  # Logic สร้างเมนูตาม Role
│   ├── RoleService.ts          # Logic ตรวจสอบสิทธิ์
│   └── ...
├── views/                  # Page Content & Logic (แยก logic หนักๆ ออกจาก pages/)
├── App.vue                 # Root Component
├── client.ts               # ⭐️ API Client (Generated Code - DO NOT EDIT MANUALLY)
└── main.ts                 # Application Entry Point
```

---

## 3. Architecture & Patterns

### 3.1 Authentication & Authorization
ระบบการยืนยันตัวตนจัดการผ่าน `src/stores/auth.ts`:
*   **Token**: ใช้ JWT Token เก็บใน `localStorage` Key: `TOKEN_KEY`
*   **Role Management**: สิทธิ์การใช้งานถูกจัดการผ่าน Role ที่ได้รับจาก Token หรือ API
*   **Guards** (`src/router/guards.ts`):
    *   `roleGuard`: ตรวจสอบสิทธิ์ก่อนเข้าหน้าต่างๆ (กำหนดใน `meta.roles` ของ Route)
    *   `authGuard`: (ใน `router/index.ts`) ตรวจสอบ Token และ Session Expired

### 3.2 Dynamic Navigation
เมนูข้างซ้าย (Sidebar) ไม่ได้ Hardcode แต่ถูกสร้างแบบ Dynamic ผ่าน `src/utils/NavigationGenerator.ts` โดยเช็คจาก Role ของ User ปัจจุบัน

### 3.3 API Communication ⚠️
**กฎเหล็ก**: ห้ามใช้ `axios` หรือ `fetch` ตรงๆ
โปรเจคนี้ใช้ Generated Client ในการคุยกับ Backend เพื่อลดข้อผิดพลาดเรื่อง Type Safety

**วิธีเรียก API:**
1.  Import `Client` จาก `@/client`
2.  Import `BACKEND_API_URL` จาก `@/constants`
3.  สร้าง Instance `new Client(BACKEND_API_URL)`
4.  เรียก Method ที่ต้องการ (Method ถูกตั้งชื่อตาม Controller/Action ของ Backend C#)

```typescript
import { Client } from '@/client'
import { BACKEND_API_URL } from '@/constants'

const client = new Client(BACKEND_API_URL)
const data = await client.getCurrentUser()
```

### 3.4 Coding Style
*   **Component**: มีการผสมระหว่าง Options API และ Composition API แต่แนะนำให้ใช้ **Composition API (`<script setup lang="ts">`)** สำหรับงานใหม่
*   **Styling**: ใช้ SCSS และ Utility Classes ของ Vuetify (e.g., `ma-2`, `text-center`) แทนการเขียน CSS เองถ้าเป็นไปได้

---

## 4. Development Workflow Guide

### ขั้นตอนการสร้างหน้าจอใหม่ (Step-by-Step)

1.  **สร้าง Page Component**:
    *   สร้างไฟล์โฟลเดอร์ `src/pages/` (เช่น `src/pages/employees/index.vue`)
    *   ใช้ `<script setup lang="ts">`

2.  **กำหนด Layout**:
## 4. รูปแบบการเขียน Component (Component Pattern)

จากการวิเคราะห์ `src/views/MasterData/Projects/`, รูปแบบมาตรฐานของโปรเจคนี้คือ **Options API** ที่เน้นความชัดเจนและแยก Logic การโหลดข้อมูลไว้ใน `initialize()`

### ✅ Standard Template (List View)
จงใช้โครงสร้างนี้เป็นต้นแบบในการสร้างหน้าแสดงรายการ (List View) ใหม่ทุกครั้ง:

```vue
<script lang="ts">
import { defineComponent } from 'vue'
// 1. Imports: เรียงลำดับจาก Library -> Local Components -> Client -> Store -> Utils
import { Client, GetProjectWithPaginationQuery } from '@/client' // Import Query Class เสมอ
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'

// 2. Client Constants: สร้าง Client นอก Component เพื่อลดการสร้าง Object ซ้ำซ้อน
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'ProjectListView', // ตั้งชื่อให้สื่อความหมาย (PascalCase)
  components: {
    // ลงทะเบียน Components ย่อยที่นี่
  },
  data() {
    return {
      // 3. State Management
      auth: useAuthStore(),
      sweetAlert: useSweetAlertStore(),
      
      // Data Table State
      isLoading: false,
      data: [] as any[], // หรือระบุ Type ที่ Generate มา เช่น ProjectDto[]
      totalItem: 0,
      
      // Pagination & Filter State
      pageNumber: 1,
      pageSize: 10,
      search: '',
      
      // Request Object (ใช้ Class จาก NSwag)
      request: new GetProjectWithPaginationQuery(), // ใช้ Class ที่ generate มา
      
      // Other UI State
      DialogCreate: false,
      DialogEdit: false,
      selectedId: ''
    }
  },
  async mounted() {
    // 4. Lifecycle: ห้ามเขียน Logic ยาวๆ ใน mounted ให้เรียก initialize()
    await this.initialize()
  },
  methods: {
    // 5. Initialize Method: ศูนย์รวมการโหลดข้อมูลเริ่มต้น
    async initialize() {
      try {
        this.isLoading = true
        
        // Setup Parameters
        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize
        this.request.search = this.search
        
        // Call API
        const response = await client.getProjectWithPagination(this.request)
        
        // Update State
        this.data = response.items || []
        this.totalItem = response.totalCount || 0
        
      } catch (error) {
        console.error('Initialization error:', error)
        // this.sweetAlert.error('โหลดข้อมูลไม่สำเร็จ')
      } finally {
        this.isLoading = false
      }
    },

    // 6. Action Handlers: Event ต่างๆ ให้เรียก initialize() เมื่อต้องการ Refresh
    async handlePageChange(page: number) {
      this.pageNumber = page
      await this.initialize()
    },
    
    async handleSearchChange() {
      this.pageNumber = 1 // Reset ไปหน้าแรกเสมอเมื่อค้นหา
      await this.initialize()
    },

    openCreateDialog() {
      this.DialogCreate = true
    },

    // ตัวอย่างการปิด Dialog และ Reload ข้อมูล
    closeCreateDialog(value: boolean, reload: boolean) {
      this.DialogCreate = value
      if (reload) {
        this.initialize()
      }
    }
  }
})
</script>

<template>
  <!-- Template ใช้ Vuetify ตามปกติ -->
</template>
```

### Key Principles (สรุปกฎเหล็ก):
1.  **Always use `defineComponent`**: เพื่อให้ TypeScript ทำงานได้สมบูรณ์และเป็นมาตรฐานเดียวกัน
2.  **`client` outside class**: แยก `const client = new Client(...)` ไว้นอก `export default`
3.  **`initialize()` is King**: ทุกการโหลดข้อมูลต้องผ่าน function นี้ (หรือแยกย่อยไปแต่อยู่ภายใต้ concept เดียวกัน)
4.  **Strongly Typed Requests**: ใช้ Class ที่ NSwag generate ให้ (เช่น `new Get...Query()`) แทนการใช้ anonymous object `{}` เพื่อความถูกต้องของ parameters


```

---

## 5. Case Study: "Projects" Module Script Pattern
จากการวิเคราะห์โมดูล **Projects** (`src/client_web/src/views/MasterData/Projects/`) พบรูปแบบการเขียนที่เป็นมาตรฐานเดียวกัน ดังนี้:

### 5.1 ProjectListView.vue (หน้าแสดงรายการ)
*   **API Client**: สร้าง `client` นอก Component
*   **State**: ใช้ `data()` เก็บ `search`, `statusList`, `pageNumber`, `pageSize`
*   **Initialization**: `mounted()` เรียก `initialize()` เพื่อโหลด `Dropdown` (หน่วยงาน) และ `Table Data` (รายการโครงการ)
*   **Permission**: มีการเช็ค Role ผ่าน `RoleService` ใน method `canViewProjects()` หรือ `canModifyProjects()`

### 5.2 CreateProject.vue (หน้าสร้างข้อมูล)
*   **Pattern**: ยังคงใช้ `defineComponent` + `initialize()`
*   **Form Logic**:
    *   สร้าง `CreateProjectCommand` ใน `data()`
    *   โหลด Dropdown (OrganizationList) ใน `initialize()`
    *   ใช้ `RulesService` (`projectCodeRules`, `projectNameRules`) ในการ Validate Form
    *   เรียก `client.createProject(...)` เมื่อกดปันทึก

### 5.3 Key Takeaway for Developers
การเขียน Script ของโมดูลใหม่ **ควรยึดตาม Pattern ของ Projects** คือ:
1.  ใช้ **Options API** (`defineComponent`)
2.  แยก Logic การโหลดข้อมูลไว้ใน `async initialize()`
3.  เรียก `initialize()` ใน `mounted()`
4.  ตรวจสอบสิทธิ์ (Permission) ผ่าน `RoleService`

---

## 6. Key File Descriptions

| File / Directory | Description |
| :--- | :--- |
| `src/client.ts` | **(ห้ามแก้)** ไฟล์ API Client หลัก เชื่อมต่อกับ Backend |
| `src/stores/auth.ts` | จัดการ Login, Logout, User Profile, Permission |
| `src/utils/RoleService.ts` | Helper เช็ค Role (e.g. `isAdmin`, `isManager`) |
| `src/@core/components` | UI Components เสริมที่ปรับแต่งมาแล้ว (Theme specific) |
| `src/plugins/vuetify` | ตั้งค่า Theme, Colors, Icons ของ Vuetify |
| `vite.config.ts` | ตั้งค่า Build, Alias (`@/`), Proxy |

---

## 6. ข้อควรระวัง (Do's and Don'ts)
*   ✅ **DO** ใช้ `VRow`, `VCol` ของ Vuetify ในการจัด Layout
*   ✅ **DO** ใช้ `Swal` (SweetAlert2) ผ่าน `useSweetAlertStore` หรือ import โดยตรงสำหรับ Popup แจ้งเตือน
*   ❌ **DON'T** แก้ไขไฟล์ใน `node_modules` หรือ `src/client.ts`
*   ❌ **DON'T** เขียน Inline Style เยอะๆ ให้ใช้ Class Utility ของ Vuetify แทน

---

เอกสารนี้รวบรวมจาก Source Code ปัจจุบัน (ณ วันที่ 8 ธ.ค. 2025)
