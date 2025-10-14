<template>
  <v-row class="d-flex align-center justify-center">
    <v-col
      cols="12"
      md="7"
    >
      <v-card class="elevation-10 mb-4 pa-3 card-Dialog dialog-scrollbar">
        <!-- Close Dialog -->
        <!-- <div class="d-flex justify-end">
          <v-btn
            color="error"
            variant="text"
            density="comfortable"
            icon="ri-close-circle-line"
            @click="closeDialog"
          ></v-btn>
        </div> -->

        <v-card-text>
          <span class="text-sub-title"> เพิ่มข้อมูลหน่วยงาน</span>

          <v-form
            ref="form"
            @submit.prevent="CreateOrganization"
            class="text-black"
          >
            <v-row class="px-5 mt-3">
              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">ชื่อหน่วยงาน <span class="text-error">*</span></label>
                <v-text-field
                  v-model="createCommand.name"
                  placeholder="ระบุชื่อหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="[
                    (v: string) => !!v || 'กรุณาระบุชื่อหน่วยงาน',
                  ]"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">ประเภทหน่วยงาน <span class="text-error">*</span></label>
                <v-autocomplete
                  v-model="createCommand.typeOrganization"
                  :items="TypeOrganizationEnum"
                  item-title="title"
                  item-value="value"
                  placeholder="เลือกประเภทหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="[
                    (v: number | null | undefined) => v !== null && v !== undefined || 'กรุณาระบุประเภทหน่วยงาน'
                  ]"
                />
              </v-col>

              <v-col
                cols="12"
                md="12"
              >
                <label class="mb-2">ที่อยู่ <span class="text-error">*</span></label>
                <v-text-field
                  v-model="createCommand.address"
                  placeholder="ระบุที่อยู่หน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="[
                    (v: string) => !!v || 'กรุณาระบุที่อยู่ของหน่วยงาน',
                  ]"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">พิกัด <span class="text-error">*</span></label>
                <v-text-field
                  v-model="createCommand.coordinates"
                  placeholder="ระบุพิกัดหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="[
                    (v: string) => !!v || 'กรุณาระบุพิกัดของหน่วยงาน',
                  ]"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">เว็บไซต์ <span class="text-error">*</span></label>
                <v-text-field
                  v-model="createCommand.webSite"
                  placeholder="ระบุเว็บไซต์หน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="[
                    (v: string) => !!v || 'กรุณาระบุเว็บไซต์ของหน่วยงาน',
                  ]"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">เบอร์โทร <span class="text-error">*</span></label>
                <v-text-field
                  v-model="createCommand.phone"
                  placeholder="ระบุเบอร์โทรหน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="[
                    (v: string) => !!v || 'กรุณาระบุเบอร์โทร',
                  ]"
                ></v-text-field>
              </v-col>

              <v-col
                cols="12"
                md="6"
              >
                <label class="mb-2">แฟกซ์ <span class="text-error">*</span></label>
                <v-text-field
                  v-model="createCommand.fax"
                  placeholder="ระบุแฟกซ์หน่วยงาน"
                  variant="outlined"
                  density="comfortable"
                  color="primary-darken-0"
                  dense
                  :rules="[
                    (v: string) => !!v || 'กรุณาระบุแฟกซ์',
                  ]"
                ></v-text-field>
              </v-col>

              <v-col
                v-col
                cols="12"
                class="d-flex justify-center"
              >
                <v-col
                  cols="12"
                  class="d-flex justify-center"
                >
                  <v-btn
                    class="mr-4"
                    rounded="lg"
                    color="error"
                    @click="closeDialog"
                    >ยกเลิก</v-btn
                  >
                  <v-btn
                    color="success-darken-2"
                    rounded="lg"
                    type="submit"
                  >
                    ตกลง
                  </v-btn>
                </v-col>
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
      </v-card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useSweetAlertStore } from '@/stores'
import { Client, CreateOrganizationCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { TypeOrganizationEnum } from '@/@layouts/enums'
const client = new Client(BACKEND_API_URL)

export default defineComponent({
  name: 'CreateOrganization',
  props: {
    CloseDialogCreate: {
      type: Function,
      required: true,
    },
  },
  data() {
    return {
      createCommand: new CreateOrganizationCommand(),
      loading: false,
      sweetAlertStore: useSweetAlertStore(),
      TypeOrganizationEnum,
    }
  },
  mounted() {},
  methods: {
    closeDialog(reload: boolean = false) {
      this.CloseDialogCreate(false, reload) // ปิด dialog
    },
    async CreateOrganization() {
      console.log('CreateOrganization ทำงาน')
      const form = this.$refs.form as any
      const { valid } = await form.validate()
      if (valid) {
        try {
          this.loading = true
          const response = await client.createOrganization(this.createCommand)
          if (response) {
            setTimeout(() => {
              this.loading = false
              this.sweetAlertStore.success('เพิ่มข้อมูลหน่วยงานสำเร็จ!')
              this.closeDialog(true)
            }, 600)
          }
        } catch (error) {
          console.error(error)
          this.closeDialog(true)
          setTimeout(() => {
            this.loading = false
            this.sweetAlertStore.error('เกิดข้อผิดพลาดในการเพิ่มข้อมูล ล้มเหลว!')
          }, 600)
        }
      }
    },
  },
})
</script>

<style scoped>
.card-Dialog {
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  border-radius: 12px;
}

.text-title {
  font-size: 25px;
  font-weight: 700;
  color: #000;
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

.text-black {
  color: #000 !important;
}
</style>
