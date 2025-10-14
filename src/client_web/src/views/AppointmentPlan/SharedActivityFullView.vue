<template>
  <div class="shared-fullscreen">
    <v-container
      fluid
      class="py-0 px-0"
    >
      <v-row no-gutters>
        <v-col cols="12">
          <div class="header text-center py-4">
            <!-- <v-icon
              icon="ri-file-list-3-line"
              class="mr-2"
            /> -->
            <span class="title">รายละเอียดรายงานสรุปผล</span>
          </div>
        </v-col>
        <v-col cols="12">
          <v-card class="elevation-0 content-card">
            <v-card-text>
              <!-- Loading validation -->
              <div
                v-if="isValidating"
                class="text-center py-10 text-grey"
              >
                <v-progress-circular
                  indeterminate
                  color="primary"
                  class="mb-4"
                />
                <div>กำลังตรวจสอบสิทธิ์การเข้าถึง...</div>
              </div>

              <!-- Validation failed -->
              <div
                v-else-if="validationError"
                class="text-center py-12"
              >
                <VImg
        :src="misc404"
        alt="Page Not Found"
        :max-width="500"
        class="mx-auto mb-5"
      />
                <div class="error-title mb-2">ไม่สามารถเข้าถึงลิงก์ได้</div>
                <div class="error-desc mb-6">{{ validationError }}</div>
                <v-btn
                  color="primary"
                  @click="$router.push('/')"
                >
                  กลับหน้าหลัก
                </v-btn>
              </div>

              <!-- Content -->
              <div v-else-if="activityId && updateCommand.id">
                <GetCustomerAppointmentPlan
                  :ActivityPlan="updateCommand"
                  :PlanNote="planNoteCommand"
                />

                <!-- เอกสารอ้างอิง -->
                <v-card
                  class="document-card px-4 px-md-6 py-4 py-md-6 mb-4 mb-md-6 mt-6"
                  border="grey-500 opacity-50 sm"
                  elevation="0"
                >
                  <v-row class="align-center mb-0">
                    <!-- Title -->
                    <v-col
                      cols="12"
                      md="6"
                      class="d-flex align-center justify-center justify-md-start"
                    >
                      <h2 class="document-title mb-0">เอกสารอ้างอิง</h2>
                    </v-col>
                  </v-row>

                  <v-row
                    class="mt-0 mb-0 px-0 text-form carousel-row"
                    align="center"
                    justify="center"
                  >
                    <v-col
                      cols="12"
                      class="carousel-col"
                    >
                      <CarouselsImage
                        v-if="activityId"
                        :id="String(activityId)"
                      />
                    </v-col>
                  </v-row>
                </v-card>
              </div>

              <!-- Loading content -->
              <div
                v-else
                class="text-center py-10 text-grey"
              >
                <v-progress-circular
                  indeterminate
                  color="primary"
                  class="mb-4"
                />
                <div>กำลังโหลดข้อมูล...</div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script lang="ts">
import {
  ActivityPlanAttachmentDto,
  Client,
  GetActivityPlanAttachmentsByActivityPlanIdQuery,
  UpdateActivityPlanCommand,
  UpdatePlanNoteCommand,
} from '@/client'
import CarouselsImage from '@/components/CarouselsImage.vue'
import { BACKEND_API_URL } from '@/constants'
import { useAuthStore, useSweetAlertStore } from '@/stores'
import { defineComponent } from 'vue'
import GetCustomerAppointmentPlan from './GetCustomerAppointmentPlan.vue'

const client = new Client(BACKEND_API_URL)
import misc404 from '@images/pages/404.png'
export default defineComponent({
  name: 'SharedActivityFullView',
  components: { GetCustomerAppointmentPlan, CarouselsImage },
  data() {
    return {
      activityId: '' as string,
      token: '' as string,
      isValidating: true,
      validationError: '' as string | null,
      updateCommand: new UpdateActivityPlanCommand(),
      planNoteCommand: new UpdatePlanNoteCommand(),
      attachments: [] as ActivityPlanAttachmentDto[],
      sweetAlertStore: useSweetAlertStore(),
      auth: useAuthStore(),
      misc404,
    }
  },
  async mounted() {
    // id จาก route param
    this.activityId = String(this.$route.params.id || '')
    this.token = String(this.$route.query.token || '') // เก็บไว้สำหรับ backward compatibility

    console.log('SharedActivityFullView mounted')
    console.log('Activity ID:', this.activityId)
    console.log('Token:', this.token || 'No token (public shared link)')
    console.log('Route path:', this.$route.path)

    await this.validateAndLoad()
  },
  methods: {
    async validateAndLoad() {
      this.isValidating = true
      this.validationError = null
      try {
        if (!this.activityId) {
          this.validationError = 'ไม่พบรหัสรายการ'
          return
        }

        // For shared reports, no token validation required
        // Just try to fetch the data directly
        console.log('Loading shared report with ID:', this.activityId)
        await this.fetchData()
      } catch (e) {
        console.error(e)
        this.validationError = 'เกิดข้อผิดพลาดระหว่างโหลดข้อมูล หรือไม่มีสิทธิ์เข้าถึง'
      } finally {
        this.isValidating = false
      }
    },
    async validateToken(): Promise<boolean> {
      try {
        const bearer = localStorage.getItem('TOKEN_KEY')
        const url = `${BACKEND_API_URL}/api/ActivityPlanEndpoint/ValidateShare?activityPlanId=${encodeURIComponent(
          this.activityId,
        )}&token=${encodeURIComponent(this.token)}`
        const res = await fetch(url, {
          method: 'GET',
          headers: {
            Accept: 'application/json',
            ...(bearer ? { Authorization: `Bearer ${bearer}` } : {}),
          },
        })
        if (!res.ok) return false
        // backend may return boolean or object
        const data = await res.json().catch(() => true)
        if (typeof data === 'boolean') return data
        if (data && typeof data === 'object') {
          // accept common properties
          if (data.valid === true || data.authorized === true || data.success === true) return true
          if (data.status && String(data.status).toLowerCase() === 'ok') return true
        }
        return true // if no body, assume ok when status 200
      } catch (e) {
        console.error('validateToken error', e)
        return false
      }
    },
    async fetchData() {
      try {
        if (!this.activityId) {
          console.error('No activity ID provided')
          return
        }

        console.log('Fetching activity plan data for ID:', this.activityId)

        const activity = await client.getActivityPlanQueryByID(String(this.activityId))
        if (activity) {
          console.log('Activity plan loaded successfully:', activity.objective || activity.id)
          this.updateCommand = { ...activity } as UpdateActivityPlanCommand
        } else {
          console.warn('No activity plan found for ID:', this.activityId)
        }

        const note = await client.getPlanNoteQueryByActivityPlanId(String(this.activityId))
        if (note) {
          console.log('Plan note loaded successfully')
          this.planNoteCommand = { ...note } as UpdatePlanNoteCommand
        } else {
          console.log('No plan note found for activity ID:', this.activityId)
        }

        // Load attachments/documents
        try {
          console.log('Fetching attachments for activity ID:', this.activityId)
          const attachmentQuery = new GetActivityPlanAttachmentsByActivityPlanIdQuery({
            activityPlanId: this.activityId,
          })
          const attachments = await client.getActivityPlanAttachmentsQueryByActivityPlanId(attachmentQuery)
          if (attachments && attachments.length > 0) {
            console.log('Attachments loaded successfully:', attachments.length, 'files')
            console.log(
              'Attachment details:',
              attachments.map(att => ({
                id: att.id,
                attachmentId: att.attachmentId,
                fileName: att.attachments?.nameFile,
                fileSize: att.attachments?.fileSize,
                fileExtension: att.attachments?.fileExtension,
                pathFile: att.attachments?.pathFile,
              })),
            )
            this.attachments = attachments

            // Store attachments data for the component to use
            // Note: GetCustomerAppointmentPlan should handle displaying these attachments
          } else {
            console.log('No attachments found for activity ID:', this.activityId)
            this.attachments = []
          }
        } catch (attachmentError) {
          console.error('Error loading attachments:', attachmentError)
          this.attachments = []
        }
      } catch (e) {
        console.error('Error fetching shared activity data:', e)
        throw e // Re-throw เพื่อให้ validateAndLoad จัดการ
      }
    },
  },
})
</script>

<style scoped>
.shared-fullscreen {
  min-height: 100vh;
  /* Unify page background color */
  background: #ffffff;
}
.header {
  background: #ffffff;
  /* Remove divider to keep a seamless background */
  border-bottom: none;
}
.title {
  font-size: clamp(18px, 4vw, 28px);
  font-weight: 700;
  color: #2b3086;
}
.content-card {
  background: transparent;
  padding: 8px;
}
.error-title {
  font-size: 20px;
  font-weight: 700;
  color: #b00020;
}
.error-desc {
  color: #666;
}
.document-card {
  border-radius: 12px;
  transition: all 0.3s ease;
}
.document-title {
  font-size: clamp(16px, 3vw, 20px);
  color: #2b3086;
  font-weight: 600;
}
.carousel-row {
  margin-top: 16px;
}
.carousel-col {
  margin-bottom: 0;
}
@media (min-width: 960px) {
  .content-card {
    max-width: 1200px;
    margin: 0 auto;
    padding: 16px 0;
  }
}
</style>
