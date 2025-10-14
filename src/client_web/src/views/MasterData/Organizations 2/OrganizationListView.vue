<template>
  <VRow class="match-height px-0 mb-1">
    <v-col
      cols="12"
      class="d-flex justify-space-between"
    >
      <span class="text-sub-title">
        <v-icon icon="ri-community-line" />
        หน่วยงาน</span
      >

      <v-btn @click="OpenDialogCreate"
        ><v-icon
          class="mr-2"
          icon="ri-add-circle-line"
        />
        เพิ่มหน่วยงาน</v-btn
      >
    </v-col>
  </VRow>

  <VCard class="card-table custom-scrollbar">
    <VDataTable
      :headers="header"
      :items="data"
      :itemlength="totalItem"
      :items-per-page="pageSize"
      class="text-no-wrap"
    >
      <template v-slot:item.typeOrganization="{ item }: any">
        {{ TypeOrganizationEnum.find(x => item.typeOrganization === x.value)?.title || '-' }}
      </template>

      <template v-slot:item.actions="{ item }: any">
        <v-col
          cols="auto"
          class="d-flex justify-center align-center"
        >
          <!-- ปุ่มต่าง ๆ สำหรับดำเนินการ -->
          <v-btn
            class="text-white mr-2"
            density="compact"
            icon
            color="info"
            rounded="lg"
            v-tooltip="{
              text: 'ดูรายละเอียด',
              contentClass: 'bg-info text-white ',
              location: 'top',
            }"
            @click="OpenDialogDetail(item.id)"
          >
            <v-icon size="18">ri-article-line</v-icon>
          </v-btn>

          <v-btn
            class="text-white mr-2"
            density="compact"
            color="warning"
            icon
            rounded="lg"
            v-tooltip="{
              text: 'แก้ไขข้อมูล',
              contentClass: 'bg-warning text-white ',
              location: 'top',
            }"
            @click="OpenDialogEdit(item.id)"
          >
            <v-icon size="18">ri-edit-2-line</v-icon>
          </v-btn>

          <v-btn
            class="text-white mr-2"
            density="compact"
            color="error-darken-1"
            icon
            rounded="lg"
            v-tooltip="{
              text: 'ลบข้อมูล',
              contentClass: 'bg-error-darken-1 text-white ',
              location: 'top',
            }"
            @click="OpenDelete(item.id)"
          >
            <v-icon size="18">ri-delete-bin-6-line</v-icon>
          </v-btn>
        </v-col>
      </template>
    </VDataTable>
  </VCard>

  <v-dialog
    v-model="DialogCreate"
    transition="dialog-top-transition"
    class="z-indexDialog"
  >
    <CreateOrganization :CloseDialogCreate="CloseDialogCreate" />
  </v-dialog>

  <v-dialog
    v-model="DialogEdit"
    transition="dialog-top-transition"
  >
    <UpdateOrganization
      :id="id"
      :CloseDialogEdit="CloseDialogEdit"
    />
  </v-dialog>

  <v-dialog
    v-model="DialogDetail"
    transition="dialog-top-transition"
  >
    <OrganizationDetail
      :id="id"
      :CloseDialogDetail="CloseDialogDetail"
    />
  </v-dialog>
</template>

<script lang="ts">
import { TypeOrganizationEnum } from '@/@layouts/enums'
import { Client, GetOrganizationWithPaginationQuery } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { defineComponent } from 'vue'
import CreateOrganization from './CreateOrganization.vue'
import UpdateOrganization from './UpdateOrganization.vue'
import OrganizationDetail from './OrganizationDetail.vue'
import { useSweetAlertStore } from '@/stores'
const client = new Client(BACKEND_API_URL)
export default defineComponent({
  name: 'OrganizationListView',
  components: {
    CreateOrganization,
    UpdateOrganization,
    OrganizationDetail,
  },
  data() {
    return {
      sweetAlert: useSweetAlertStore(),
      header: [
        { title: 'ชื่อหน่วยงาน', value: 'name' },
        { title: 'ประเภทหน่วยงาน', value: 'typeOrganization' },
        { title: 'ที่อยู่', value: 'address' },
        { title: 'เว็บไซต์', value: 'webSite' },
        { title: 'เบอร์โทร', value: 'phone' },
        { title: 'จัดการ', value: 'actions' },
      ] as any,
      data: [] as any,
      request: new GetOrganizationWithPaginationQuery(),
      pageNumber: 1,
      pageSize: 10,
      totalItem: 0 as any,
      TypeOrganizationEnum,
      id: '',
      DialogCreate: false,
      DialogEdit: false,
      DialogDetail: false,
    }
  },
  async mounted() {
    await this.initialize()
  },
  methods: {
    async initialize() {
      try {
        this.request.pageNumber = this.pageNumber
        this.request.pageSize = this.pageSize
        const response = await client.getOrganizationWithPagination(this.request)
        this.data = response.items
        this.totalItem = response.totalCount
      } catch (error) {
        console.error(error)
      }
    },
    OpenDialogCreate() {
      this.DialogCreate = true
    },
    CloseDialogCreate(value: boolean, reload: boolean) {
      this.DialogCreate = value
      if (reload === true) {
        this.initialize()
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
          const del = await client.deleteOrganization(id)
          if (del) {
            this.sweetAlert.successDeleted('ลบข้อมูลสำเร็จ')
            this.initialize()
          }
        } catch (error) {
          this.sweetAlert.error('เกิดข้อผิดพลาดในการลบข้อมูล !')
          console.error(error)
        }
      }
    },
  },
})
</script>

<style scoped>
.card-form {
  background: #ffffff;
  border-radius: 15px;
  /* box-shadow: 2px 2px 6px 6px rgba(43, 48, 134, 0.15); */
}
.text-sub-title {
  font-size: 25px;
  font-weight: bold;
  color: #2b3086;
  /* background-image: linear-gradient( 135deg, #7e4ee6b6 10%, #8C57FF 100%); */
  background-clip: text;
  -webkit-background-clip: text; /* สำหรับเว็บเบราว์เซอร์ที่รองรับ */
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
}
</style>
