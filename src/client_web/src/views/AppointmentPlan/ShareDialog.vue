<template>
  <v-dialog
    v-model="isOpen"
    max-width="600px"
    @keydown.esc.stop="handleEscKey"
  >
    <v-card>
      <v-card-title class="text-sub-title text-h5 text-center py-4">
        <v-icon
          icon="ri-share-line"
          class="mr-2"
        ></v-icon>
        แชร์รายงาน
      </v-card-title>

      <v-card-text class="px-6">
        <v-form ref="shareForm">
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="shareData.subject"
                label="หัวข้อ"
                variant="outlined"
                dense
                prepend-inner-icon="ri-text"
              />
            </v-col>

            <v-col cols="12">
              <v-combobox
                v-model="shareData.emails"
                :items="employeeEmails"
                label="อีเมลผู้รับ"
                variant="outlined"
                multiple
                chips
                closable-chips
                :hint="`พิมพ์เพื่อค้นหาและเลือกอีเมล (${emailCount} รายการ)`"
                persistent-hint
                prepend-inner-icon="ri-mail-line"
                :rules="emailRules"
                :loading="employeeListLoading"
                clearable
                no-data-text="ไม่พบอีเมลที่ตรงกับการค้นหา"
                :menu-props="{ maxHeight: '200px' }"
                auto-select-first
              >
                <template #item="{ props, item }">
                  <v-list-item
                    v-bind="props"
                    :title="typeof item === 'string' ? item : item.title || ''"
                  />
                </template>
              </v-combobox>
            </v-col>

            <v-col cols="12">
              <v-textarea
                v-model="shareData.message"
                label="ข้อความเพิ่มเติม (ถ้ามี)"
                variant="outlined"
                rows="3"
                prepend-inner-icon="ri-message-2-line"
              />
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <v-card-actions class="px-6 pb-4">
        <v-spacer></v-spacer>
        <v-btn
          color="grey"
          variant="outlined"
          @click="closeDialog"
          class="mr-2 equal-btn"
        >
          <v-icon
            icon="ri-close-line"
            class="mr-1"
          ></v-icon>
          ยกเลิก
        </v-btn>
        <v-btn
          color="primary"
          variant="outlined"
          @click="sendShare"
          :loading="shareLoading"
          class="equal-btn"
        >
          <v-icon
            icon="ri-send-plane-line"
            class="mr-1"
          ></v-icon>
          ส่ง
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts">
import { Client, ShareActivityPlanCommand } from '@/client'
import { BACKEND_API_URL } from '@/constants'
import { useSweetAlertStore } from '@/stores'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'ShareDialog',
  props: {
    open: {
      type: Boolean,
      default: false,
    },
    reportTitle: {
      type: String,
      default: '',
    },
    reportId: {
      type: [String, Number],
      default: '',
    },
  },
  emits: ['close', 'share'],
  data() {
    return {
      shareLoading: false,
      employeeListLoading: false,
      searchInput: '',
      shareData: {
        subject: '',
        emails: [] as string[],
        message: '',
      },
      employeeEmails: [] as string[],
      sweetAlertStore: useSweetAlertStore(),
      client: new Client(BACKEND_API_URL),
      emailRules: [
        (value: string[]) => {
          if (!value || value.length === 0) return 'กรุณาใส่อีเมลอย่างน้อย 1 อีเมล'
          const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
          for (const email of value) {
            if (!emailPattern.test(email)) {
              return `อีเมล "${email}" ไม่ถูกต้อง`
            }
          }
          return true
        },
      ],
    }
  },
  computed: {
    isOpen: {
      get(): boolean {
        return this.open
      },
      set(value: boolean) {
        if (!value) {
          this.closeDialog()
        }
      },
    },
    emailCount(): number {
      return this.employeeEmails.length
    },
  },
  watch: {
    open(newVal) {
      if (newVal) {
        // console.log('=== Share Dialog - Opening ===')
        // console.log('Report ID:', this.reportId)
        // console.log('Report Title:', this.reportTitle)
        // console.log('Current URL:', window.location.href)
        // console.log('=============================')

        this.initializeDialog()
        this.fetchEmployeeEmails()
      }
    },
  },
  methods: {
    initializeDialog() {
      this.shareData.subject = `รายงานสรุปผล - ${this.reportTitle || 'รายงาน'}`
      // console.log('Share Dialog - Initialized subject:', this.shareData.subject)
    },
    closeDialog() {
      this.$emit('close')
      this.resetForm()
    },
    resetForm() {
      this.shareData = {
        subject: '',
        emails: [],
        message: '',
      }
      this.shareLoading = false
    },
    async sendShare() {
      const shareForm = this.$refs.shareForm as any
      const { valid } = await shareForm.validate()

      if (valid) {
        this.shareLoading = true
        try {
          // Log sharing data before sending
          // console.log('=== Share Dialog - Sharing Data ===')
          // console.log('Report ID:', this.reportId)
          // console.log('Report Title:', this.reportTitle)
          // console.log('Share Subject:', this.shareData.subject)
          // console.log('Recipients:', this.shareData.emails)
          // console.log('Message:', this.shareData.message)
          // console.log('==================================')

          // Emit share event with data to parent component
          this.$emit('share', {
            ...this.shareData,
            reportId: this.reportId,
          })

          // เรียก API เพื่อส่งอีเมล
          const command = new ShareActivityPlanCommand({
            activityPlanId: this.reportId?.toString(),
            title: this.shareData.subject,
            emails: this.shareData.emails,
            remarks: this.shareData.message,
          })

          console.log('Share Command to API:', command)
          const result = await this.client.shareActivityPlan(command)
          console.log('Share API result:', result)
          // console.log('Share completed successfully for Report ID:', this.reportId)

          // The backend sends OpenID authorize URL, but we'll show the direct shared link format
          const directShareLink = `${window.location.origin}/activity/${this.reportId}/shared`
          const openIdShareLink = `https://ntiportal.nti.co.th/connect/authorize?response_type=code&client_id=OPNNricj2qWgVQqo3x4JjHpaoDy6Z0&redirect_uri=https://crm.nti.co.th/login-callback?id=${this.reportId}&scope=openid profile email roles phone profile_image&code_challenge=gEKX6x8KW3Pxfna9viyf6ZHhTZlleNA15rxji0jWlvM&code_challenge_method=S256&state=authencrm`

          console.log('🔗 Direct Share Link:', directShareLink)
          // console.log('🔗 OpenID Share Link (sent in email):', openIdShareLink)

          // Create clickable link for testing
          console.log(
            `%cClick to test: ${directShareLink}`,
            'color: blue; text-decoration: underline; cursor: pointer;',
          )

          // Also show as alert for easy clicking
          this.sweetAlertStore.showAlert({
            title: 'ลิงก์สำหรับแชร์',
            text: `ลิงก์ทดสอบ: ${directShareLink}\n\nคลิกลิงก์นี้เพื่อทดสอบการเข้าหน้า SharedActivityFullView`,
            icon: 'success',
            confirmButtonText: 'ตกลง',
          })

          this.sweetAlertStore.successDeleted('ส่งอีเมลสำเร็จ')
          this.closeDialog()
        } catch (error) {
          console.error('Error sending email:', error)
          this.sweetAlertStore.error('เกิดข้อผิดพลาดในการส่งอีเมล')
        } finally {
          this.shareLoading = false
        }
      }
    },
    handleEscKey(event: KeyboardEvent) {
      // ป้องกัน event จาก propagate ไปยัง parent
      event.stopPropagation()
      event.preventDefault()
      this.closeDialog()
    },
    async fetchEmployeeEmails() {
      if (this.employeeEmails.length > 0) return // ถ้ามีข้อมูลแล้วไม่ต้อง fetch ใหม่

      this.employeeListLoading = true
      try {
        const response = await fetch('https://hr.nti.co.th/api/EmployeeEndpoint/ObjEmployeeLists')
        if (!response.ok) {
          throw new Error('Failed to fetch employee list')
        }

        const employees = (await response.json()) as any[]
        if (!Array.isArray(employees) || employees.length === 0) {
          this.employeeEmails = []
          return
        }

        // console.log('Employee data:', employees)
        // console.log('Sample employee:', employees[0])
        // console.log('Employee fields:', Object.keys(employees[0]))

        // ดู fields ที่มี email ในชื่อ
        const emailFields = Object.keys(employees[0]).filter(
          key => key.toLowerCase().includes('email') || key.toLowerCase().includes('mail'),
        )
        console.log('Email related fields:', emailFields)

        // ลองหา field ที่เป็น email (อาจเป็น Email หรือ emailAddress หรืออื่นๆ)
        const detectedField =
          emailFields.find(field => employees[0][field] && typeof employees[0][field] === 'string') ||
          Object.keys(employees[0]).find(key => {
            const value = employees[0][key]
            return value && typeof value === 'string' && value.includes('@')
          })

        const field: string | undefined = detectedField
        console.log('Using email field:', field)

        if (!field) {
          this.employeeEmails = []
          return
        }

        // แปลงข้อมูลเป็น array ของ email (กรอง email ที่ไม่ว่าง)
        this.employeeEmails = employees
          .map((emp: any) => {
            const v = emp[field]
            return typeof v === 'string' ? v.trim() : ''
          })
          .filter((email: string) => email !== '')
          .filter((email: string, index: number, self: string[]) => self.indexOf(email) === index) // กรอง email ซ้ำ

        // console.log('Filtered emails:', this.employeeEmails)
        console.log('Total emails found:', this.employeeEmails.length)
      } catch (error) {
        console.error('Error fetching employee emails:', error)
        this.sweetAlertStore.error('ไม่สามารถโหลดรายชื่ออีเมลได้')
      } finally {
        this.employeeListLoading = false
      }
    },
    selectEmail(email: string) {
      if (!this.shareData.emails.includes(email)) {
        this.shareData.emails.push(email)
      }
      this.searchInput = ''
    },
  },
})
</script>

<style scoped>
.equal-btn {
  min-width: 140px;
  width: 140px;
}

.text-sub-title {
  font-size: clamp(20px, 5vw, 35px);
  font-weight: bold;
  color: #2b3086;
  background-clip: text;
  text-shadow: 5px 5px 8px rgba(65, 69, 92, 0.447);
  line-height: 1.2;
}

.v-card-title {
  color: #2b3086;
  border-radius: 4px 4px 0 0;
}

.v-card-title .v-icon {
  color: #2b3086;
}

/* Mobile: <= 599px */
@media (max-width: 599px) {
  .v-dialog {
    margin: 12px;
  }

  .v-card {
    border-radius: 12px;
  }

  .v-card-title {
    font-size: 18px !important;
    padding: 16px !important;
  }

  .v-card-text {
    padding: 16px !important;
  }

  .v-card-actions {
    padding: 16px !important;
  }

  .text-sub-title {
    font-size: 30px;
    text-align: center;
  }
}

/* Tablet: 600px - 959px */
@media (min-width: 600px) and (max-width: 959px) {
  .v-card-title {
    font-size: 20px;
  }

  .text-sub-title {
    font-size: 28px;
    padding: 6px 10px;
    text-align: center;
  }
}

/* Desktop: >= 960px */
@media (min-width: 960px) {
  .v-card-title {
    font-size: 22px;
  }

  .text-sub-title {
    font-size: 35px;
    padding: 0;
    text-align: left;
  }
}

/* Extra small devices adjustments */
@media (max-width: 375px) {
  .text-sub-title {
    font-size: 18px;
  }
}

/* Landscape mobile orientation */
@media (max-width: 959px) and (orientation: landscape) {
  .text-sub-title {
    font-size: clamp(16px, 4vw, 24px);
  }
}
</style>
