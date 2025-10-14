<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon icon="ri-article-line" />
        โครงการ</span
      >

      <v-btn @click="OpenDialogCreate"
        ><v-icon
          class="mr-2"
          icon="ri-add-circle-line"
        />
        เพิ่มโครงการ</v-btn
      >
    </v-col>
  </VRow>

  <VRow
    class="mb-2"
    justify="end"
  >
    <VCol
      cols="12"
      md="3"
    >
      <v-select
        :items="statusList"
        v-model="selectedProjectTypeObj"
        return-object
        label="ประเภทโครงการ"
        color="primary"
        class="mr-4 form-field"
        dense
        item-title="title"
        item-value="value"
        @update:model-value="handleProjectTypeChange"
      ></v-select>
    </VCol>
    <VCol
      cols="12"
      md="4"
    >
      <VTextField
        v-model="search"
        @input="handleSearchChange"
        label="ค้นหาโครงการ"
        color="primary"
        class="mr-4 form-field"
        append-inner-icon="ri-search-line"
        clearable
        dense
      ></VTextField>
    </VCol>
  </VRow>

  <VCard class="card-table custom-scrollbar">
    <v-data-table-server
      v-model:page="pageNumber"
      v-model:items-per-page="pageSize"
      :items-per-page-options="pageSizeOptions"
      :loading="isLoading"
      :headers="header"
      :items="data"
      :items-length="totalItem"
      @update:page="handlePageChange"
      @update:items-per-page="handlePageSizeChange"
      class="text-no-wrap"
    >
      <template v-slot:item.contractSignedDate="{ item }: { item: any }">
        {{ formatThaiDate(item.contractSignedDate) }}
      </template>

      <template v-slot:item.warrantyEndDate="{ item }: { item: any }">
        {{ formatThaiDate(item.warrantyEndDate) }}
      </template>

      <template v-slot:item.projectCost="{ item }: { item: any }">
        {{ formatProjectCost(item.projectCost) }}
      </template>

      <template v-slot:item.organizationId="{ item }: { item: any }">
        {{ item.organizationName || '-' }}
      </template>

      <template v-slot:item="{ item, index }: any">
        <tr :class="{ 'selected-row': item.id === id }">
          <td class="text-center">
            <v-btn
              class="text-white mr-2"
              density="compact"
              icon
              color="info"
              rounded="lg"
              v-tooltip="{ text: 'ดูรายละเอียด', contentClass: 'bg-info text-white', location: 'top' }"
              @click.stop="OpenDialogDetail(item.id)"
            >
              <v-icon size="18">ri-article-line</v-icon>
            </v-btn>
            <v-btn
              class="text-white mr-2"
              density="compact"
              color="warning"
              icon
              rounded="lg"
              v-tooltip="{ text: 'แก้ไขข้อมูล', contentClass: 'bg-warning text-white', location: 'top' }"
              @click.stop="OpenDialogEdit(item.id)"
            >
              <v-icon size="18">ri-edit-2-line</v-icon>
            </v-btn>
            <v-btn
              class="text-white mr-2"
              density="compact"
              color="error-darken-1"
              icon
              rounded="lg"
              v-tooltip="{ text: 'ลบข้อมูล', contentClass: 'bg-error-darken-1 text-white', location: 'top' }"
              @click.stop="OpenDelete(item.id)"
            >
              <v-icon size="18">ri-delete-bin-6-line</v-icon>
            </v-btn>
          </td>
          <td>{{ item.projectCode }}</td>
          <td>{{ item.contractNumber || '-' }}</td>
          <td>{{ item.projectName || '-' }}</td>
          <td>{{ ProjectTypeEnum.find(x => item.projectType === x.value)?.title || '-' }}</td>
          <td>{{ item.organizationName || '-' }}</td>
          <td class="text-right">
            {{ formatProjectCost(item.projectCost) }}
          </td>
          <td>{{ formatThaiDate(item.contractSignedDate) }}</td>
        </tr>
      </template>
    </v-data-table-server>
  </VCard>

  <v-navigation-drawer
    v-model="DialogCreate"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <CreateProject
      v-if="DialogCreate"
      :CloseDialogCreate="CloseDialogCreate"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogEdit"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <UpdateProject
      v-if="DialogEdit"
      :id="id"
      :CloseDialogEdit="CloseDialogEdit"
    />
  </v-navigation-drawer>

  <v-navigation-drawer
    v-model="DialogDetail"
    :width="$vuetify.display.xs ? '100vw' : '550'"
    class="z-indexDialog create-activity-drawer"
    close-on-back
    temporary
    transition="dialog-right-transition"
    location="right"
    scrollable
  >
    <ProjectDetail
      v-if="DialogDetail"
      :id="id"
      :CloseDialogDetail="CloseDialogDetail"
    />
  </v-navigation-drawer>
</template>

<script lang="ts">
import { ProjectTypeEnum, TypeOrganizationEnum } from '@/@layouts/enums'
import { Client, GetProjectWithPaginationQuery, ProjectType } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import moment from 'moment'
import { defineComponent } from 'vue'
import CreateProject from './CreateProject.vue'
import ProjectDetail from './ProjectDetail.vue'
import UpdateProject from './UpdateProject.vue'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'OrganizationListView',
  components: {
    CreateProject,
    UpdateProject,
    ProjectDetail,
  },
  data() {
    return {
      sweetAlert: useSweetAlertStore(),
      selectedProjectTypeObj: { title: 'ทั้งหมด', value: -1 },
      header: [
        { title: 'จัดการ', value: 'actions', align: 'center' },
        { title: 'รหัสโครงการ', value: 'projectCode' },
        { title: 'หมายเลขสัญญา', value: 'contractNumber' },
        { title: 'ชื่อโครงการ', value: 'projectName' },
        { title: 'ประเภทโครงการ', value: 'projectType' },
        { title: 'หน่วยงาน', value: 'organizationId' },
        { title: 'มูลค่าโครงการ', value: 'projectCost' },
        { title: 'วันที่ลงนามสัญญา', value: 'contractSignedDate' },
      ] as any,
      search: '',
      data: [] as any,
      statusList: [
        { title: 'ทั้งหมด', value: -1 },
        { title: 'งานฮาร์ดแวร์', value: ProjectType.Hardware },
        { title: 'งานพัฒนาระบบ', value: ProjectType.Developer },
        { title: 'งานดูแลระบบ', value: ProjectType.Maintenance },
        { title: 'งานอื่นๆ', value: ProjectType.Other },
      ] as any,
      request: new GetProjectWithPaginationQuery(),
      pageNumber: 1,
      pageSize: 10,
      totalItem: 0 as any,
      ProjectTypeEnum,
      id: '',
      DialogCreate: false,
      DialogEdit: false,
      DialogDetail: false,
      isLoading: false,
      OrganizationList: [] as Array<{ id: string; name: string }>,
      pageSizeOptions: [10, 25, 50, 100, -1],
    }
  },
  async mounted() {
    this.forceCloseAllDrawers() // บังคับปิด Drawer ทั้งหมดเมื่อ Component ถูก Mount
    this.request.projectType = -1 // Set default project type to "ทั้งหมด"
    await this.initialize()

    // Add fallback: ESC key closes drawer if open
    window.addEventListener('keydown', this.handleEscCloseDrawer)
    window.addEventListener('popstate', this.handleBackButton) // เพิ่ม listener สำหรับ back button บนมือถือ
  },
  beforeUnmount() {
    window.removeEventListener('keydown', this.handleEscCloseDrawer)
    window.removeEventListener('popstate', this.handleBackButton) // ลบ listener
  },
  methods: {
    forceCloseAllDrawers() {
      this.DialogCreate = false
      this.DialogEdit = false
      this.DialogDetail = false
    },
    handleBackButton() {
      // ตรวจสอบว่ามี drawer ตัวไหนเปิดอยู่หรือไม่
      if (this.DialogCreate || this.DialogEdit || this.DialogDetail) {
        this.forceCloseAllDrawers()
        // ไม่ต้อง history.pushState(null, '', location.href); เพราะ close-on-back จะจัดการให้
      }
    },
    handleEscCloseDrawer(e: KeyboardEvent) {
      if (e.key === 'Escape') {
        if (this.DialogCreate) {
          this.CloseDialogCreate(false, false)
        } else if (this.DialogEdit) {
          this.CloseDialogEdit(false, false)
        } else if (this.DialogDetail) {
          this.CloseDialogDetail(false, false)
        }
      }
    },
    async fetchOrganizations() {
      const orgListRaw = await client.getOrganizationQuery()
      return (orgListRaw || []).map((org: any) => ({
        id: org.id || '',
        name: org.name || '-',
      }))
    },
    mapOrganizationNames(items: any[], orgMap: Record<string, string>) {
      return items.map((item: any) => ({
        ...item,
        organizationName: orgMap[item.organizationId] || '-',
      }))
    },
    async fetchAllProjects(request: any) {
      let allItems: any[] = []
      let page = 1
      const pageSize = 100
      let finished = false
      let total = 0

      while (!finished) {
        request.pageNumber = page
        request.pageSize = pageSize
        const response = await client.getProjectWithPagination(request)
        if (response.items && response.items.length > 0) {
          allItems = allItems.concat(response.items)
          total = response.totalCount || allItems.length
          if (allItems.length >= total) {
            finished = true
          } else {
            page++
          }
        } else {
          finished = true
        }
      }
      return allItems
    },
    async initialize() {
      try {
        this.isLoading = true

        // Fetch organizations
        const orgList = await this.fetchOrganizations()
        this.OrganizationList = orgList
        const orgMap = Object.fromEntries(orgList.map((org: any) => [org.id, org.name]))

        // // Handle "all" page size
        if (this.pageSize === -1) {
          this.pageNumber = 1
          this.request.pageNumber = 1
          this.request.pageSize = -1
          let items = await this.fetchAllProjects(this.request)
          if (this.request.projectType !== undefined && this.request.projectType !== -1) {
            items = items.filter((item: any) => item.projectType === this.request.projectType)
          }
          this.data = this.mapOrganizationNames(items, orgMap)
          this.totalItem = items.length
          return
        }

        // Set request params for normal pagination
        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize

        // Fetch paginated projects
        const response = await client.getProjectWithPagination(this.request)
        let items = response.items || []
        if (this.request.projectType !== undefined && this.request.projectType !== -1) {
          items = items.filter((item: any) => item.projectType === this.request.projectType)
        }
        this.data = this.mapOrganizationNames(items, orgMap)
        this.totalItem = response.totalCount
      } catch (error) {
        // Error handling can be improved as needed
      } finally {
        this.isLoading = false
      }
    },

    async handleProjectTypeChange(newObj: any) {
      this.selectedProjectTypeObj = newObj
      this.request.projectType = newObj.value === -1 ? undefined : newObj.value
      this.pageNumber = 1
      await this.initialize()
    },

    async handleSearchChange() {
      this.request.search = this.search
      this.pageNumber = 1
      await this.initialize()
    },

    async handlePageChange(page: number) {
      if (this.pageSize === -1) {
        this.pageNumber = 1
        return
      }
      this.pageNumber = page
      await this.initialize()
    },
    async handlePageSizeChange(size: number) {
      this.pageSize = size
      this.pageNumber = 1
      await this.initialize()
    },
    OpenDialogCreate() {
      this.DialogCreate = true
    },
    CloseDialogCreate(value: boolean, reload: boolean, searchData?: string) {
      this.DialogCreate = value
      if (reload === true) {
        this.initialize().then(() => {
          if (searchData) {
            try {
              const search = JSON.parse(searchData)
              const matchedItem = this.data.find(
                (item: any) =>
                  item.projectCode === search.projectCode &&
                  item.projectName === search.projectName &&
                  item.contractNumber === search.contractNumber &&
                  item.organizationId === search.organizationId,
              )
              if (matchedItem) {
                this.id = matchedItem.id
              } else {
                if (this.data.length > 0) {
                  this.id = this.data[0].id
                }
              }
            } catch (error) {
              // Error parsing project search data
            }
          }
        })
      }
    },
    OpenDialogEdit(id: any) {
      if (id) {
        this.DialogEdit = true
        this.id = id
      }
    },
    CloseDialogEdit(value: boolean, reload: boolean) {
      this.DialogEdit = value
      if (reload === true) {
        this.initialize()
      }
    },
    OpenDialogDetail(id: any) {
      if (id) {
        this.DialogDetail = true
        this.id = id
      }
    },
    CloseDialogDetail(value: boolean, reload: boolean) {
      this.DialogDetail = value
      if (reload === true) {
        this.initialize()
      }
    },
    async OpenDelete(id: any) {
      this.id = id
      const confirmDelete = await this.sweetAlert.showAlert({
        title: 'ยืนยันการลบข้อมูล',
        text: `คุณต้องการลบข้อมูลนี้หรือไม่ ?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'ลบข้อมูล',
        cancelButtonText: 'ยกเลิก',
        customClass: {
          popup: 'bg-white z-index-5000 rounded-popup',
        },
      })

      if (confirmDelete.isConfirmed) {
        try {
          const del = await client.deleteProject(id)
          if (del) {
            this.sweetAlert.successDeleted('ลบข้อมูลสำเร็จ')
            this.initialize()
          }
        } catch (error) {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
        }
      }
    },
    formatThaiDate(date: string | Date): string {
      if (!date || date === '0001-01-01T00:00:00' || new Date(date).getFullYear() === 1) {
        return ''
      }
      const monthShortThai = [
        '',
        'ม.ค.',
        'ก.พ.',
        'มี.ค.',
        'เม.ย.',
        'พ.ค.',
        'มิ.ย.',
        'ก.ค.',
        'ส.ค.',
        'ก.ย.',
        'ต.ค.',
        'พ.ย.',
        'ธ.ค.',
      ]
      const m = moment(date)
      const day = m.date()
      const month = monthShortThai[m.month() + 1]
      const year = m.year() + 543
      return `${day} ${month} ${year}`
    },
    formatProjectCost(cost: number | string): string {
      if (cost == null || cost === '') return ''
      const number = typeof cost === 'string' ? parseFloat(cost) : cost
      return isNaN(number) ? '' : number.toLocaleString('en-US')
    },
  },
})
</script>

<style scoped>
@media (max-width: 767.98px) {
  .create-activity-drawer:not(.v-navigation-drawer--active) {
    transform: translateX(100%) !important;
    visibility: hidden !important;
  }
}
</style>
