<template>
  <VCard
    class="activity-plan-card scroll-content"
    elevation="0"
  >
    <!-- Header Section -->
    <VCardItem class="header-section">
      <div class="header-content mt-3">
        <VCardTitle class="activity-title">
          {{ objectiveText }}
          <div class="activity-subtitle mt-2">วัตถุประสงค์</div>
        </VCardTitle>
      </div>
    </VCardItem>

    <VCardText class="px-2 px-sm-4 px-md-6 py-3 py-sm-4 py-md-6">
      <!-- Info Grid -->
      <VRow class="info-grid">
        <!-- Project Code -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-hashtag"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-label">รหัสโครงการ</div>
              <div class="info-value">{{ projectDisplayText }}</div>
            </div>
          </div>
        </VCol>

        <!-- Date/Time -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-calendar-schedule-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-label">วันที่ / เวลา</div>
              <div class="info-value">
                {{ formatDateforshow(ActivityPlan.startDate, ActivityPlan.endDate, ActivityPlan.allDay) }}
              </div>
            </div>
          </div>
        </VCol>

        <!-- Responsible Person -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-user-3-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-label">ผู้รับผิดชอบ</div>
              <div class="info-value">
                <template
                  v-if="ActivityPlan.employees && (ActivityPlan.employees.firstName || ActivityPlan.employees.lastName)"
                >
                  {{
                    (ActivityPlan.employees.titleName || '') +
                    (ActivityPlan.employees.firstName || '') +
                    ' ' +
                    (ActivityPlan.employees.lastName || '')
                  }}
                </template>
                <template v-else>
                  {{ employeeName && employeeName.trim() !== '' ? employeeName : 'ไม่ระบุ' }}
                </template>
              </div>
            </div>
          </div>
        </VCol>

        <!-- Organization -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-community-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-label">องค์กร</div>
              <div class="info-value">{{ organizationName }}</div>
            </div>
          </div>
        </VCol>

        <!-- Location -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-map-pin-2-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-label">สถานที่</div>
              <div class="info-value">{{ locationText }}</div>
            </div>
          </div>
        </VCol>

        <!-- Cost -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item cost-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-money-dollar-circle-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="cost-header">
                <div class="info-value">ค่าใช้จ่าย</div>
                <div class="info-label">{{ costText }}</div>
              </div>
              <div class="cost-details mt-2">
                <div class="info-value">รายละเอียดค่าใช้จ่าย</div>
                <div class="info-label">{{ costDetailText }}</div>
              </div>
            </div>
          </div>
        </VCol>

        <!-- Additional Details -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-file-text-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">รายละเอียดเพิ่มเติม</div>
              <div class="info-label mt-1">{{ detailText }}</div>
            </div>
          </div>
        </VCol>

        <!-- สรุปผลการนัดพบ -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-file-list-3-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">สรุปผลการนัดพบ</div>
              <div class="info-label mt-1">{{ summary }}</div>
            </div>
          </div>
        </VCol>

        <!-- สิ่งที่ต้องดำเนินการ -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-todo-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value mt-1">สิ่งที่ต้องดำเนินการ</div>
              <div class="info-label">{{ toDoNext }}</div>
            </div>
          </div>
        </VCol>

        <!-- หมายเหตุ -->
        <VCol
          cols="12"
          md="12"
          class="pa-1 pa-sm-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-sticky-note-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">หมายเหตุ</div>
              <div class="info-label mt-1">{{ remarks }}</div>
            </div>
          </div>
        </VCol>
      </VRow>

      <!-- Map Section -->
      <div class="map-section">
        <div class="section-header">
          <VIcon
            icon="ri-map-2-line"
            class="section-icon"
          />
          <h3 class="section-title">พิกัดหน่วยงาน</h3>
        </div>

        <div class="map-container">
          <template v-if="isValidCoordinates">
            <iframe
              :src="googleMapsUrl"
              class="map-iframe"
              loading="lazy"
              referrerpolicy="no-referrer-when-downgrade"
            ></iframe>
          </template>
          <template v-else>
            <VCard
              class="map-placeholder"
              elevation="0"
            >
              <VCardText class="text-center py-6 py-sm-8 py-md-12">
                <VIcon
                  icon="ri-map-pin-off-line"
                  :size="$vuetify.display.xs ? 40 : $vuetify.display.sm ? 48 : 64"
                  class="mb-2 mb-sm-3 mb-md-4 text-grey-darken-1"
                />
                <p class="text-grey-darken-1 text-caption text-sm-body-2 text-md-body-1">
                  {{ coordinatesMessage }}
                </p>
              </VCardText>
            </VCard>
          </template>
        </div>
      </div>

      <!-- Button Section -->
      <VRow class="mt-4">
        <VCol
          cols="12"
          class="pt-6 action-btn-row"
        >
          <div
            class="button-container"
            :style="{
              justifyContent: 'center',
              width: $vuetify.display.xs ? '100%' : 'auto',
            }"
          >
            <v-btn
              class="mobile-btn cancel-btn"
              rounded="lg"
              color="error"
              @click="closeDialog()"
              :style="{
                width: $vuetify.display.xs ? '100%' : '50%',
                maxWidth: $vuetify.display.xs ? 'none' : '220px',
                margin: $vuetify.display.xs ? '0 0 22% 0' : '0 auto',
              }"
            >
              <v-icon
                icon="ri-close-line"
                class="mr-2"
              ></v-icon>
              ปิด
            </v-btn>
          </div>
        </VCol>
      </VRow>
    </VCardText>
  </VCard>
</template>

<script lang="ts">
import { Client } from '@/client'
import moment from 'moment'
import { computed, defineComponent, onMounted, ref, watch } from 'vue'

const client = new Client()

export default defineComponent({
  name: 'GetActivityById',
  emits: ['close'],
  props: {
    ActivityPlan: {
      type: Object as any,
      required: true,
    },
    PlanNote: {
      type: Object as any,
      required: false,
      default: null,
    },
    CloseDialogDetail: {
      type: Function,
      required: true,
    },
  },
  setup(props) {
    const employeeName = ref<string>('')

    const getEmployeeFullName = (emp: any) => {
      if (!emp) return ''
      const title = emp.titleName || ''
      const first = emp.firstName || ''
      const last = emp.lastName || ''
      return [title, first, last].filter(Boolean).join(' ')
    }
    const organizationName = ref<string>('ไม่ระบุ')
    const detailText = ref<string>('ไม่ระบุ')
    const objectiveText = ref<string>('ไม่ระบุ')
    const objectiveDetailText = ref<string>('ไม่ระบุ')
    const costText = ref<string>('ไม่ระบุ')
    const costDetailText = ref<string>('ไม่ระบุ')
    const locationText = ref<string>('ไม่ระบุ')
    const summary = ref<string>('ไม่ระบุ')
    const toDoNext = ref<string>('ไม่ระบุ')
    const remarks = ref<string>('ไม่ระบุ')

    // ฟังก์ชันดึงชื่อพนักงาน
    const fetchEmployeeName = async (id: string | undefined) => {
      if (!id) {
        employeeName.value = ''
        return
      }
      try {
        const emp = await client.getEmployeeQueryByID(id)
        if (emp) {
          employeeName.value = getEmployeeFullName(emp)
        } else {
          employeeName.value = ''
        }
      } catch (e) {
        employeeName.value = ''
      }
    }

    // ===== Map Logic (แบบ Composition API) =====
    const coordinatesString = computed(() => {
      return props.ActivityPlan.organizations?.coordinates || props.ActivityPlan.location || ''
    })
    const parsedCoordinates = computed(() => {
      const coordinates = coordinatesString.value
      if (!coordinates) return null
      const cleanCoords = coordinates.trim()
      const patterns = [
        /^(-?\d+\.?\d*),\s*(-?\d+\.?\d*)$/, // lat,lng หรือ lat, lng
        /^(-?\d+\.?\d*)\s+(-?\d+\.?\d*)$/, // lat lng
      ]
      for (const pattern of patterns) {
        const match = cleanCoords.match(pattern)
        if (match) {
          const lat = parseFloat(match[1])
          const lng = parseFloat(match[2])
          if (isFinite(lat) && isFinite(lng) && lat >= -90 && lat <= 90 && lng >= -180 && lng <= 180) {
            return { lat, lng }
          }
        }
      }
      return null
    })
    const isValidCoordinates = computed(() => {
      return !!parsedCoordinates.value
    })
    const googleMapsUrl = computed(() => {
      if (!parsedCoordinates.value) return ''
      const coords = parsedCoordinates.value
      return `https://maps.google.com/maps?q=${coords.lat},${coords.lng}&hl=th&z=15&output=embed`
    })
    const coordinatesMessage = computed(() => {
      const coords = coordinatesString.value
      if (!coords) {
        return 'กรุณาระบุพิกัดในรูปแบบตัวเลขเพื่อแสดงแผนที่'
      } else if (coords.match(/[ก-ฮ]/)) {
        return 'กรุณาระบุพิกัดเป็นตัวเลข\nตัวอย่าง: 13.7563,100.5018'
      } else {
        return 'รูปแบบพิกัดไม่ถูกต้อง\nตัวอย่าง: 13.7563,100.5018 (ละเว้นช่องว่าง)'
      }
    })

    // ฟังก์ชันตรวจสอบว่าเป็น placeholder text หรือไม่
    const isPlaceholderText = (text: string) => {
      if (!text || text.trim() === '') return true

      const placeholderTexts = [
        // สำหรับ summary
        'ระบุรายละเอียดสรุปผลการนัดพบ',
        '*รายละเอียดสรุปผลการนัดพบ',
        'รายละเอียดสรุปผลการนัดพบ',

        // สำหรับ toDoNext
        'ระบุสิ่งที่ต้องดำเนินการ',
        '*ระบุสิ่งที่ต้องดำเนินการ',
        'สิ่งที่ต้องดำเนินการ',
        'ระบุรายการที่ต้องทำ',
        '*รายการที่ต้องทำ',
        'รายการที่ต้องทำ',

        // สำหรับ remarks
        'ระบุหมายเหตุ',
        '*ระบุหมายเหตุ',
        'หมายเหตุ',
        'ระบุข้อมูลเพิ่มเติม',
        '*ข้อมูลเพิ่มเติม',

        // ทั่วไป
        'ระบุรายละเอียด',
        'placeholder',
        'กรุณาระบุ',
        'โปรดระบุ',
        'enter',
        'input',
      ]

      const lowerText = text.toLowerCase().trim()
      return placeholderTexts.some(
        placeholder =>
          lowerText.includes(placeholder.toLowerCase()) ||
          (text.startsWith('*') &&
            (text.includes('รายละเอียด') || text.includes('ดำเนินการ') || text.includes('หมายเหตุ'))),
      )
    }

    // ฟังก์ชันแปลงชื่อองค์กร
    const extractOrganizationName = (org: any) => {
      if (!org) return 'ไม่ระบุ'
      return org.name || org.orgName || org.organizationName || 'ไม่ระบุ'
    }

    // อัปเดตค่า fields สำหรับแสดงผล
    const updateDisplayFields = () => {
      organizationName.value = extractOrganizationName(props.ActivityPlan.organizations)
      detailText.value = props.ActivityPlan.detail || 'ไม่ระบุ'
      objectiveText.value = props.ActivityPlan.objective || 'ไม่ระบุ'
      objectiveDetailText.value = props.ActivityPlan.objectiveDetail || 'ไม่ระบุ'
      costText.value =
        props.ActivityPlan.cost !== null && props.ActivityPlan.cost !== undefined ? props.ActivityPlan.cost : 'ไม่ระบุ'
      costDetailText.value = props.ActivityPlan.costDetail || 'ไม่ระบุ'
      locationText.value = props.ActivityPlan.location || 'ไม่ระบุ'
      summary.value = props.PlanNote?.summary || 'ไม่ระบุ'
      toDoNext.value = props.PlanNote?.toDoNext || 'ไม่ระบุ'
      remarks.value = props.PlanNote?.remarks || 'ไม่ระบุ'
    }

    onMounted(() => {
      console.log('🟢 ActivityPlan.employees:', props.ActivityPlan.employees)
      fetchEmployeeName(props.ActivityPlan.employeeId)
      updateDisplayFields()
    })

    // Watch for changes in PlanNote prop
    watch(
      () => props.PlanNote,
      () => {
        updateDisplayFields()
      },
      { deep: true },
    )

    const projectCodeDisplay = computed(() => {
      const projects = props.ActivityPlan.projects
      if (Array.isArray(projects)) {
        return projects[0]?.projectCode || 'ไม่ระบุ'
      }
      return projects?.projectCode || 'ไม่ระบุ'
    })

    const projectDisplayText = computed(() => {
      const projects = props.ActivityPlan.projects
      let projectCode = ''
      let projectName = ''

      if (Array.isArray(projects)) {
        projectCode = projects[0]?.projectCode || ''
        projectName = projects[0]?.projectName || ''
      } else {
        projectCode = projects?.projectCode || ''
        projectName = projects?.projectName || ''
      }

      // ถ้าไม่มีทั้งรหัสและชื่อโครงการ
      if (!projectCode && !projectName) {
        return 'ไม่ระบุ'
      }

      // ถ้ามีเฉพาะรหัสโครงการ
      if (projectCode && !projectName) {
        return projectCode
      }

      // ถ้ามีเฉพาะชื่อโครงการ
      if (!projectCode && projectName) {
        return projectName
      }

      // ถ้ามีทั้งรหัสและชื่อโครงการ
      return `${projectCode} ${projectName}`
    })

    const showObjectiveSection = computed(() => {
      return objectiveText.value !== 'ไม่ระบุ' || (detailText.value && detailText.value !== 'ไม่ระบุ')
    })

    return {
      employeeName,
      projectCodeDisplay,
      projectDisplayText,
      organizationName,
      detailText,
      objectiveText,
      objectiveDetailText,
      costText,
      costDetailText,
      locationText,
      summary,
      toDoNext,
      remarks,
      isValidCoordinates,
      googleMapsUrl,
      coordinatesMessage,
      showObjectiveSection,
    }
  },

  methods: {
    closeDialog(reload: boolean = false) {
      console.log('🔴 กดปุ่มปิด - เรียก CloseDialogDetail')
      try {
        if (this.CloseDialogDetail && typeof this.CloseDialogDetail === 'function') {
          this.CloseDialogDetail(true, reload) // ปิด dialog (ส่ง true เพื่อปิด)
        } else {
          console.warn('⚠️ CloseDialogDetail prop is not a function, using emit instead')
          this.$emit('close')
        }
      } catch (error) {
        console.error('❌ Error calling CloseDialogDetail:', error)
        // ถ้า CloseDialogDetail ไม่ทำงาน ลองใช้ emit
        this.$emit('close')
      }
    },
    formatDateforshow(startdate: any, enddate: any, allday: any) {
      if (
        !startdate ||
        !enddate ||
        startdate === '0001-01-01T00:00:00' ||
        enddate === '0001-01-01T00:00:00' ||
        new Date(startdate).getFullYear() === 1 ||
        new Date(enddate).getFullYear() === 1
      ) {
        return ''
      }
      if (startdate && enddate) {
        let start = moment(startdate)
          .locale('th')
          .add(543, 'year')
          .format('D MMMM YYYY ' + (allday !== true ? 'เวลา HH:mm น.' : ''))
          .replace(/เดือน/g, '')
        let end = moment(enddate)
          .locale('th')
          .add(543, 'year')
          .format('D MMMM YYYY ' + (allday !== true ? 'เวลา HH:mm น.' : ''))
          .replace(/เดือน/g, '')
        return start + ' ถึง ' + end
      }
    },
  },
})
</script>

<style scoped>
/* Base Card Styles */
.activity-plan-card {
  background: #ffffff;
  overflow: hidden;
  width: 100% !important;
  max-width: 100% !important;
  box-sizing: border-box;
}

/* Header Section */
.header-section {
  display: flex;
  align-items: center;
  gap: 16px;
  background: #ffffff;
  border-bottom: 1px solid #f3f4f6;
  padding: 16px 20px !important;
}

.header-content {
  flex: 1;
}

.activity-title {
  font-size: 1.125rem;
  font-weight: 600;
  color: #111827;
  line-height: 1.3;
  margin: 0;
  padding: 0;
}

/* Scrollable area with appropriate height */
.scroll-content {
  overflow-y: visible;
  overflow-x: hidden;
  width: 100% !important;
  max-width: 100% !important;
  height: auto;
  scrollbar-width: thin;
  scrollbar-color: #667eea #f8f9fa;
  -webkit-overflow-scrolling: touch;
  overscroll-behavior: contain;
}

/* Enhanced scrollbar */
.scroll-content::-webkit-scrollbar {
  width: 8px;
}

.scroll-content::-webkit-scrollbar-track {
  background: rgba(0, 0, 0, 0.05);
  border-radius: 4px;
}

.scroll-content::-webkit-scrollbar-thumb {
  background: linear-gradient(135deg, #667eea, #764ba2);
  border-radius: 4px;
}

.activity-subtitle {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 4px;
}

/* Info Grid */
.info-grid {
  margin: 0 -4px;
  width: 100% !important;
  max-width: 100% !important;
}

.info-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 14px 16px;
  background: #ffffff;
  border: 1px solid rgb(210, 210, 211);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border-radius: 12px;
  transition: all 0.2s ease;
  height: 100%;
  min-height: 80px;
}

.info-item:hover {
  border-color: #d1d5db;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.cost-item {
  min-height: 100px;
}

.cost-header,
.cost-details {
  width: 100%;
}

.info-icon-wrapper {
  flex-shrink: 0;
  width: 36px;
  height: 36px;
  background: #e0e7ff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.info-icon {
  color: #6366f1;
  font-size: 18px;
}

.info-content {
  flex: 1;
  min-width: 0;
}

.info-label {
  font-size: 0.75rem;
  font-weight: 500;
  color: #9ca3af;
  margin-bottom: 2px;
  line-height: 1.3;
}

.info-value {
  font-size: 0.8125rem;
  font-weight: 600;
  color: #111827;
  line-height: 1.4;
  word-break: break-word;
}

/* Map Section */
.map-section {
  margin-top: 24px;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
  padding: 14px 16px;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
}

.section-icon {
  color: #6366f1;
  font-size: 18px;
}

.section-title {
  font-size: 0.9375rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.map-container {
  background: #ffffff;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid #e5e7eb;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
}

.map-iframe {
  width: 100%;
  height: 300px;
  border: none;
  background: #f8fafc;
}

.map-placeholder {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 12px;
  min-height: 200px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}

/* Responsive Breakpoints */

/* Extra Small (xs) - Mobile Portrait: 0-599px */
@media (max-width: 599px) {
  .scroll-content {
    max-height: 75vh;
    min-height: 300px;
  }

  .header-section {
    padding: 12px 16px !important;
    gap: 10px;
  }

  .activity-title {
    font-size: 1rem;
    line-height: 1.2;
  }

  .activity-subtitle {
    font-size: 0.8125rem;
    margin-top: 2px;
  }

  .action-btn-row {
    margin-bottom: 15% !important;
  }

  .info-grid {
    margin: 0 -2px;
  }

  .info-item {
    padding: 12px 14px;
    gap: 10px;
    min-height: 70px;
  }

  .cost-item {
    min-height: 85px;
  }

  .info-icon-wrapper {
    width: 32px;
    height: 32px;
  }

  .info-icon {
    font-size: 16px;
  }

  .info-label {
    font-size: 0.6875rem;
  }

  .info-value {
    font-size: 0.75rem;
  }

  .section-header {
    padding: 12px 14px;
    gap: 8px;
  }

  .section-icon {
    font-size: 16px;
  }

  .section-title {
    font-size: 0.875rem;
  }

  .map-iframe {
    height: 220px;
  }

  .map-placeholder {
    min-height: 180px;
  }

  .map-section {
    margin-top: 20px;
  }
}

/* Small (sm) - Mobile Landscape & Small Tablets: 600-959px */
@media (min-width: 600px) and (max-width: 959px) {
  .scroll-content {
    max-height: 80vh;
    min-height: 350px;
  }

  .header-section {
    padding: 16px 20px !important;
  }

  .activity-title {
    font-size: 1.125rem;
  }

  .info-item {
    padding: 14px 16px;
    min-height: 75px;
  }

  .cost-item {
    min-height: 95px;
  }

  .info-icon-wrapper {
    width: 36px;
    height: 36px;
  }

  .info-icon {
    font-size: 17px;
  }

  .info-label {
    font-size: 0.75rem;
  }

  .info-value {
    font-size: 0.8125rem;
  }

  .map-iframe {
    height: 280px;
  }

  .map-placeholder {
    min-height: 220px;
  }
}

/* Medium (md) - Tablets: 960-1263px */
@media (min-width: 960px) and (max-width: 1263px) {
  .scroll-content {
    max-height: 82vh;
    min-height: 400px;
  }

  .header-section {
    padding: 18px 24px !important;
  }

  .activity-title {
    font-size: 1.1875rem;
  }

  .info-item {
    padding: 15px 18px;
    min-height: 80px;
  }

  .cost-item {
    min-height: 100px;
  }

  .info-icon-wrapper {
    width: 38px;
    height: 38px;
  }

  .info-icon {
    font-size: 18px;
  }

  .map-iframe {
    height: 320px;
  }

  .map-placeholder {
    min-height: 250px;
  }
}

/* Large (lg) - Small Desktops: 1264-1903px */
@media (min-width: 1264px) and (max-width: 1903px) {
  .scroll-content {
    max-height: 85vh;
    min-height: 450px;
  }

  .header-section {
    padding: 20px 24px !important;
  }

  .activity-title {
    font-size: 1.25rem;
  }

  .info-item {
    padding: 16px 20px;
    min-height: 85px;
  }

  .cost-item {
    min-height: 105px;
  }

  .info-icon-wrapper {
    width: 40px;
    height: 40px;
  }

  .info-icon {
    font-size: 19px;
  }

  .map-iframe {
    height: 360px;
  }

  .map-placeholder {
    min-height: 280px;
  }
}

/* Extra Large (xl) - Large Desktops: 1904px+ */
@media (min-width: 1904px) {
  .scroll-content {
    max-height: 88vh;
    min-height: 500px;
  }

  .header-section {
    padding: 24px 32px !important;
  }

  .activity-title {
    font-size: 1.375rem;
  }

  .info-item {
    padding: 18px 22px;
    min-height: 90px;
  }

  .cost-item {
    min-height: 110px;
  }

  .info-icon-wrapper {
    width: 42px;
    height: 42px;
  }

  .info-icon {
    font-size: 20px;
  }

  .map-iframe {
    height: 400px;
  }

  .map-placeholder {
    min-height: 300px;
  }

  .map-section {
    margin-top: 32px;
  }
}

/* Special handling for very small screens */
@media (max-width: 479px) {
  .header-section {
    padding: 10px 12px !important;
  }

  .activity-title {
    font-size: 0.9375rem;
  }

  .activity-subtitle {
    font-size: 0.75rem;
  }

  .info-item {
    padding: 10px 12px;
    min-height: 65px;
  }

  .cost-item {
    min-height: 80px;
  }

  .info-icon-wrapper {
    width: 30px;
    height: 30px;
  }

  .info-icon {
    font-size: 14px;
  }

  .info-label {
    font-size: 0.625rem;
  }

  .info-value {
    font-size: 0.6875rem;
  }

  .section-header {
    padding: 10px 12px;
  }

  .section-title {
    font-size: 0.8125rem;
  }

  .map-iframe {
    height: 200px;
  }

  .map-placeholder {
    min-height: 160px;
  }
}

/* Animation and Accessibility */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.info-item {
  animation: fadeIn 0.3s ease-out;
}

/* Focus states for accessibility */
.info-item:focus-within {
  outline: 2px solid #6366f1;
  outline-offset: 2px;
}

/* Reduced motion support */
@media (prefers-reduced-motion: reduce) {
  .info-item {
    transition: none;
    animation: none;
  }
}

/* High contrast mode support */
@media (prefers-contrast: high) {
  .info-item {
    border-color: #000;
  }

  .info-label,
  .info-value {
    color: #000;
  }
}

/* Button Styles (เหมือนกับ DetailProjectContact) */
.button-container {
  display: flex;
  gap: 16px;
  justify-content: center;
  flex-wrap: wrap;
}

.mobile-btn {
  min-width: 120px;
  height: 44px;
  font-size: 16px;
  font-weight: 500;
  border: none !important;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.mobile-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.2);
}

/* Mobile Button Responsive */
@media (max-width: 599px) {
  .button-container {
    flex-direction: column;
    gap: 12px;
  }

  .mobile-btn {
    width: 100%;
    min-height: 48px;
    font-size: 16px;
  }
}

/* Tablet Button Responsive */
@media (min-width: 600px) and (max-width: 959px) {
  .mobile-btn {
    min-width: 140px;
  }
}

/* Desktop Button */
@media (min-width: 960px) {
  .button-container {
    max-width: 400px;
    margin: 0 auto;
  }

  .mobile-btn {
    min-width: 150px;
  }
}
</style>
