<template>
  <VCard
    class="activity-plan-card"
    elevation="0"
    rounded="xl"
  >
    <!-- Header Section -->
    <VCardItem class="header-section">
      <!-- <div class="header-icon">
        <VIcon
          icon="ri-calendar-event-line"
          class="header-icon-svg"
        />
      </div> -->
      <div class="header-content mt-3">
        <VCardTitle class="activity-title">
          {{ objectiveText }}
        </VCardTitle>
        <div class="activity-subtitle">วัตถุประสงค์</div>
      </div>
    </VCardItem>

    <VCardText class="px-4 px-sm-6 py-4 py-sm-6">
      <!-- Info Grid -->
      <VRow class="info-grid">
        <!-- Project Code -->
        <VCol
          cols="12"
          md="12"
          class="pa-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-hashtag"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">รหัสโครงการ</div>
              <div class="info-label">
                <template
                  v-if="
                    (projectCodeDisplay && projectCodeDisplay !== 'ไม่ระบุ') ||
                    (ActivityPlan.projects?.projectName &&
                      ActivityPlan.projects?.projectName !== 'ไม่ระบุ' &&
                      ActivityPlan.projects?.projectName !== 'undefined')
                  "
                >
                  {{
                    (projectCodeDisplay !== 'ไม่ระบุ' ? projectCodeDisplay : '') +
                    (projectCodeDisplay !== 'ไม่ระบุ' &&
                    ActivityPlan.projects?.projectName &&
                    ActivityPlan.projects?.projectName !== 'ไม่ระบุ' &&
                    ActivityPlan.projects?.projectName !== 'undefined'
                      ? ' '
                      : '') +
                    (ActivityPlan.projects?.projectName &&
                    ActivityPlan.projects?.projectName !== 'ไม่ระบุ' &&
                    ActivityPlan.projects?.projectName !== 'undefined'
                      ? ActivityPlan.projects?.projectName
                      : '')
                  }}
                </template>
                <template v-else> ไม่ระบุ </template>
              </div>
            </div>
          </div>
        </VCol>

        <!-- Date/Time -->
        <VCol
          cols="12"
          md="12"
          class="pa-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-calendar-schedule-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">วันที่ / เวลา</div>
              <div class="info-label">
                {{ formatDateforshow(ActivityPlan.startDate, ActivityPlan.endDate, ActivityPlan.allDay) }}
              </div>
            </div>
          </div>
        </VCol>

        <!-- Responsible Person -->
        <VCol
          cols="12"
          md="12"
          class="pa-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-user-3-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">ผู้รับผิดชอบ</div>
              <div class="info-label">
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
          class="pa-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-community-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">องค์กร</div>
              <div class="info-label">{{ organizationName }}</div>
            </div>
          </div>
        </VCol>

        <!-- Location -->
        <VCol
          cols="12"
          md="12"
          class="pa-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-map-pin-2-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">สถานที่</div>
              <div class="info-label">{{ locationText }}</div>
            </div>
          </div>
        </VCol>

        <VCol
          cols="12"
          sm="6"
          lg="6"
          class="pa-2"
        ></VCol>

        <!-- Cost -->
        <VCol
          cols="12"
          sm="12"
          lg="12"
          class="pa-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-money-dollar-circle-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">ค่าใช้จ่าย</div>
              <div class="info-label">{{ costText }}</div>
              <div class="info-value mt-1 mb-1">รายละเอียดค่าใช้จ่าย</div>
              <div class="info-label">{{ costDetailText }}</div>
            </div>
          </div>
        </VCol>
        <!-- รายละเอียดเพิ่มเติม -->
        <VCol
          cols="12"
          sm="12"
          lg="12"
          class="pa-2"
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
              <div class="info-label">{{ detailText }}</div>
            </div>
          </div>
        </VCol>

        <!-- สรุปผลการนัดพบ -->
        <VCol
          cols="12"
          sm="12"
          lg="12"
          class="pa-2"
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
              <div class="info-label">{{ summary }}</div>
            </div>
          </div>
        </VCol>

        <!-- สิ่งที่ต้องดำเนินการ -->
        <VCol
          cols="12"
          sm="12"
          lg="12"
          class="pa-2"
        >
          <div class="info-item">
            <div class="info-icon-wrapper">
              <VIcon
                icon="ri-todo-line"
                class="info-icon"
              />
            </div>
            <div class="info-content">
              <div class="info-value">สิ่งที่ต้องดำเนินการ</div>
              <div class="info-label">{{ toDoNext }}</div>
            </div>
          </div>
        </VCol>

        <!-- หมายเหตุ -->
        <VCol
          cols="12"
          sm="12"
          lg="12"
          class="pa-2"
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
              <div class="info-label">{{ remarks }}</div>
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
              <VCardText class="text-center py-8 py-sm-12">
                <VIcon
                  icon="ri-map-pin-off-line"
                  :size="$vuetify.display.xs ? 48 : 64"
                  class="mb-3 mb-sm-4 text-grey-darken-1"
                />
                <p class="text-grey-darken-1 text-body-2 text-sm-body-1">
                  {{ coordinatesMessage }}
                </p>
              </VCardText>
            </VCard>
          </template>
        </div>
      </div>
    </VCardText>
  </VCard>
</template>

<script lang="ts">
import { Client } from '@/client'
import moment from 'moment'
import { computed, defineComponent, onMounted, ref, watch } from 'vue'

const client = new Client()

export default defineComponent({
  name: 'GetCustomerAppointmentPlan',
  props: {
    ActivityPlan: {
      type: Object as any,
      required: true,
    },
    PlanNote: {
      type: Object as any,
      required: false,
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

      // console.log('🔍 Checking if placeholder:', text)

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
        '*รายละเอียดสิ่งที่ต้องดำเนินการ',

        // สำหรับ remarks
        'ระบุหมายเหตุ',
        '*ระบุหมายเหตุ',
        'หมายเหตุ',
        'ระบุข้อมูลเพิ่มเติม',
        '*ข้อมูลเพิ่มเติม',
        '*รายละเอียดหมายเหตุ',

        // ทั่วไป
        'ระบุรายละเอียด',
        'placeholder',
        'กรุณาระบุ',
        'โปรดระบุ',
        'enter',
        'input',
      ]

      const lowerText = text.toLowerCase().trim()
      const isPlaceholder = placeholderTexts.some(
        placeholder =>
          lowerText.includes(placeholder.toLowerCase()) ||
          text.trim() === placeholder ||
          (text.startsWith('*') &&
            (text.includes('รายละเอียด') || text.includes('ดำเนินการ') || text.includes('หมายเหตุ'))),
      )

      // console.log('🎯 Is placeholder result:', isPlaceholder)
      return isPlaceholder
    }

    // ฟังก์ชันแปลงชื่อองค์กร
    const extractOrganizationName = (org: any) => {
      if (!org) return 'ไม่ระบุ'
      return org.name || org.orgName || org.organizationName || 'ไม่ระบุ'
    }

    // ฟังก์ชัน format number แบบเดียวกับ UpdateAppointmentOutcome
    const formatNumber = (value: any) => {
      if (value == null || value === '') return ''
      const number = parseFloat(value)
      return isNaN(number) ? '' : number.toLocaleString('en-US')
    }

    // อัปเดตค่า fields สำหรับแสดงผล
    const updateDisplayFields = () => {
      organizationName.value = extractOrganizationName(props.ActivityPlan.organizations)
      detailText.value = props.ActivityPlan.detail || 'ไม่ระบุ'
      objectiveText.value = props.ActivityPlan.objective || 'ไม่ระบุ'
      objectiveDetailText.value = props.ActivityPlan.objectiveDetail || 'ไม่ระบุ'
      // ฟอร์แมต costText แบบมี comma และ "บาท"
      if (props.ActivityPlan.cost !== null && props.ActivityPlan.cost !== undefined && props.ActivityPlan.cost !== '') {
        costText.value = formatNumber(props.ActivityPlan.cost) + ' บาท'
      } else {
        costText.value = 'ไม่ระบุ'
      }
      costDetailText.value = props.ActivityPlan.costDetail || 'ไม่ระบุ'
      locationText.value = props.ActivityPlan.location || 'ไม่ระบุ'

      // จัดการ summary
      // console.log('🔍 Checking summary - PlanNote.summary:', props.PlanNote?.summary)
      // console.log('🔍 Is placeholder?', props.PlanNote?.summary ? isPlaceholderText(props.PlanNote.summary) : 'N/A')

      if (props.PlanNote?.summary) {
        if (isPlaceholderText(props.PlanNote.summary)) {
          // console.log('📝 Found placeholder text in PlanNote.summary:', props.PlanNote.summary)
          summary.value = 'ไม่ระบุ'
        } else {
          summary.value = props.PlanNote.summary
          // console.log('✅ Found valid summary in PlanNote:', summary.value)
        }
      } else {
        summary.value = 'ไม่ระบุ'
      }

      // จัดการ toDoNext
      // console.log('🔍 Checking toDoNext - PlanNote.toDoNext:', props.PlanNote?.toDoNext)
      // console.log('🔍 Is placeholder?', props.PlanNote?.toDoNext ? isPlaceholderText(props.PlanNote.toDoNext) : 'N/A')

      if (props.PlanNote?.toDoNext) {
        if (isPlaceholderText(props.PlanNote.toDoNext)) {
          // console.log('📋 Found placeholder text in PlanNote.toDoNext:', props.PlanNote.toDoNext)
          toDoNext.value = 'ไม่ระบุ'
        } else {
          toDoNext.value = props.PlanNote.toDoNext
          // console.log('✅ Found valid toDoNext in PlanNote:', toDoNext.value)
        }
      } else {
        toDoNext.value = 'ไม่ระบุ'
      }

      // จัดการ remarks
      // console.log('🔍 Checking remarks - PlanNote.remarks:', props.PlanNote?.remarks)
      // console.log('🔍 Is placeholder?', props.PlanNote?.remarks ? isPlaceholderText(props.PlanNote.remarks) : 'N/A')

      if (props.PlanNote?.remarks) {
        if (isPlaceholderText(props.PlanNote.remarks)) {
          // console.log('📝 Found placeholder text in PlanNote.remarks:', props.PlanNote.remarks)
          remarks.value = 'ไม่ระบุ'
        } else {
          remarks.value = props.PlanNote.remarks
          // console.log('✅ Found valid remarks in PlanNote:', remarks.value)
        }
      } else {
        remarks.value = 'ไม่ระบุ'
      }

      // Debug: log ActivityPlan and PlanNote structure to find summary field
      // console.log('🎯 ActivityPlan data for summary:', props.ActivityPlan)
      // console.log('📋 ActivityPlan available fields:', Object.keys(props.ActivityPlan))
      // console.log('📝 PlanNote data:', props.PlanNote)
      // console.log('✅ Final summary value:', summary.value)
      // console.log('✅ Final toDoNext value:', toDoNext.value)
      // console.log('✅ Final remarks value:', remarks.value)
    }

    onMounted(() => {
      // Debug: log employees object structure
      if (props.ActivityPlan.employees) {
        // console.log('🧑‍💼 employees object:', props.ActivityPlan.employees)
        // console.log('📋 Available fields:', Object.keys(props.ActivityPlan))
        // console.log('🎯 ActivityPlan data:', props.ActivityPlan) // Debug log
      }
      fetchEmployeeName(props.ActivityPlan.employeeId)
      updateDisplayFields()
    })

    // Watch for changes in PlanNote prop
    watch(
      () => props.PlanNote,
      newPlanNote => {
        // console.log('🔄 PlanNote changed:', newPlanNote)
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
    const showObjectiveSection = computed(() => {
      return objectiveText.value !== 'ไม่ระบุ' || (detailText.value && detailText.value !== 'ไม่ระบุ')
    })

    return {
      employeeName,
      projectCodeDisplay,
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
/* Base Styles */
.header-section {
  display: flex;
  align-items: center;
  gap: 16px;
  background: #ffffff;
  border-bottom: 1px solid #f3f4f6;
  padding: 20px 24px !important;
}

.header-icon {
  width: 48px;
  height: 48px;
  background: #6366f1;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.header-icon-svg {
  color: white;
  font-size: 24px;
}

.header-content {
  flex: 1;
}

.activity-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #111827;
  line-height: 1.3;
  margin: 0;
  padding: 0;
}

.activity-subtitle {
  font-size: 0.875rem;
  color: #6b7280;
  margin-top: 3px;
}

/* Info Grid */
.info-grid {
  margin: 0 -8px;
}

.info-grid .v-col {
  padding: 8px;
}

.info-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  padding: 16px;
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

.info-icon-wrapper {
  flex-shrink: 0;
  width: 40px;
  height: 40px;
  background: #e0e7ff;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.info-icon {
  color: #6366f1;
  font-size: 20px;
}

.info-content {
  flex: 1;
  min-width: 0;
}

.info-label {
  font-size: 0.8rem;
  font-weight: 500;
  color: #5c5f64;
  margin-bottom: 4px;
  margin-top: 2px;
}

.info-value {
  font-size: 0.9rem;
  font-weight: 700;
  color: #111827;
  line-height: 1.4;
  word-break: break-word;
}

/* Section Styles */
.objective-section,
.map-section {
  margin-top: 32px;
}

.section-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
  padding: 16px;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
}

.section-icon {
  color: #6366f1;
  font-size: 20px;
}

.section-title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.section-content {
  background: #ffffff;
  padding: 20px;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
}

.unified-section {
  margin-top: 32px;
  background: #fff;
  border-radius: 12px;
}

.unified-section .section-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 16px;
  padding: 16px;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
}

.unified-section .section-icon {
  color: #6366f1;
  font-size: 20px;
}

.unified-section .section-title {
  font-size: 1rem;
  font-weight: 600;
  color: #111827;
  margin: 0;
}

.unified-section .section-content {
  background: #ffffff;
  padding: 20px;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
}

.unified-content {
  padding: 0;
}

.content-subtitle {
  font-size: 0.875rem;
  font-weight: 600;
  color: #4b5563;
  margin: 0 0 8px 0;
}

.content-text {
  font-size: 0.875rem;
  line-height: 1.6;
  color: #374151;
  margin: 0 0 16px 0;
}

.content-text:last-child {
  margin-bottom: 0;
}

/* Map Section */
.map-section {
  margin-top: 32px;
}

.map-container {
  background: #ffffff;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid #e5e7eb;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
}

.map-container:hover {
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.08);
  border-color: #d1d5db;
}

.map-iframe {
  width: 100%;
  height: 400px;
  border: none;
  background: #f8fafc;
  transition: all 0.3s ease;
  filter: contrast(1.02) saturate(1.02);
}

.map-placeholder {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 12px;
  padding: 2rem;
  text-align: center;
  min-height: 300px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
}

/* ===== RESPONSIVE BREAKPOINTS ===== */

/* Extra Small - Mobile Portrait (max-width: 479px) */
@media (max-width: 479px) {
  .header-section {
    padding: 12px 16px !important;
    gap: 12px;
  }

  .header-icon {
    width: 36px;
    height: 36px;
  }

  .header-icon-svg {
    font-size: 16px;
  }

  .activity-title {
    font-size: 0.875rem;
    line-height: 1.3;
  }

  .activity-subtitle {
    font-size: 0.75rem;
  }

  .info-item {
    padding: 10px 12px;
    gap: 10px;
    min-height: 60px;
  }

  .info-icon-wrapper {
    width: 32px;
    height: 32px;
  }

  .info-icon {
    font-size: 14px;
  }

  .info-label {
    font-size: 0.6875rem;
  }

  .info-value {
    font-size: 0.75rem;
  }

  .section-header {
    padding: 10px 12px;
    gap: 8px;
    margin-bottom: 12px;
  }

  .section-icon {
    font-size: 16px;
  }

  .section-title {
    font-size: 0.8125rem;
  }

  .section-content {
    padding: 12px;
  }

  .map-iframe {
    height: 200px;
  }

  .map-placeholder {
    min-height: 180px;
    padding: 1rem;
  }

  .objective-section,
  .map-section {
    margin-top: 16px;
  }
}

/* Small - Mobile Landscape & Small Tablets (min-width: 480px and max-width: 599px) */
@media (min-width: 480px) and (max-width: 599px) {
  .header-section {
    padding: 14px 18px !important;
    gap: 14px;
  }

  .header-icon {
    width: 40px;
    height: 40px;
  }

  .header-icon-svg {
    font-size: 18px;
  }

  .activity-title {
    font-size: 1rem;
    line-height: 1.3;
  }

  .activity-subtitle {
    font-size: 0.8125rem;
  }

  .info-item {
    padding: 12px 14px;
    gap: 10px;
    min-height: 65px;
  }

  .info-icon-wrapper {
    width: 36px;
    height: 36px;
  }

  .info-icon {
    font-size: 16px;
  }

  .info-label {
    font-size: 0.75rem;
  }

  .info-value {
    font-size: 0.8125rem;
  }

  .section-header {
    padding: 12px 14px;
    gap: 10px;
    margin-bottom: 14px;
  }

  .section-icon {
    font-size: 18px;
  }

  .section-title {
    font-size: 0.875rem;
  }

  .section-content {
    padding: 14px;
  }

  .map-iframe {
    height: 220px;
  }

  .map-placeholder {
    min-height: 200px;
    padding: 1.25rem;
  }

  .objective-section,
  .map-section {
    margin-top: 18px;
  }
}

/* Medium - Tablets Portrait (min-width: 600px and max-width: 959px) */
@media (min-width: 600px) and (max-width: 959px) {
  .header-section {
    padding: 16px 20px !important;
    gap: 16px;
  }

  .header-icon {
    width: 44px;
    height: 44px;
  }

  .header-icon-svg {
    font-size: 20px;
  }

  .activity-title {
    font-size: 1.125rem;
  }

  .activity-subtitle {
    font-size: 0.875rem;
  }

  .info-item {
    padding: 14px 16px;
    min-height: 70px;
  }

  .info-icon-wrapper {
    width: 38px;
    height: 38px;
  }

  .info-icon {
    font-size: 18px;
  }

  .info-label {
    font-size: 0.75rem;
  }

  .info-value {
    font-size: 0.875rem;
  }

  .section-header {
    padding: 14px 16px;
    margin-bottom: 16px;
  }

  .section-content {
    padding: 16px;
  }

  .map-iframe {
    height: 300px;
  }

  .map-placeholder {
    min-height: 260px;
    padding: 1.5rem;
  }

  .objective-section,
  .map-section {
    margin-top: 24px;
  }
}

/* Large - Tablets Landscape & Small Desktops (min-width: 960px and max-width: 1263px) */
@media (min-width: 960px) and (max-width: 1263px) {
  .header-section {
    padding: 18px 22px !important;
  }

  .activity-title {
    font-size: 1.1875rem;
  }

  .info-item {
    padding: 15px;
    min-height: 75px;
  }

  .map-iframe {
    height: 350px;
  }

  .map-placeholder {
    min-height: 280px;
  }

  .objective-section,
  .map-section {
    margin-top: 28px;
  }
}

/* Extra Large - Desktop (min-width: 1264px and max-width: 1903px) */
@media (min-width: 1264px) and (max-width: 1903px) {
  .header-section {
    padding: 20px 24px !important;
  }

  .activity-title {
    font-size: 1.25rem;
  }

  .info-item {
    padding: 16px;
    min-height: 80px;
  }

  .map-iframe {
    height: 380px;
  }

  .objective-section,
  .map-section {
    margin-top: 30px;
  }
}

/* Extra Extra Large - Large Desktop (min-width: 1904px) */
@media (min-width: 1904px) {
  .header-section {
    padding: 24px 28px !important;
  }

  .activity-title {
    font-size: 1.375rem;
  }

  .activity-subtitle {
    font-size: 0.9375rem;
  }

  .info-item {
    padding: 18px;
    min-height: 85px;
  }

  .info-icon-wrapper {
    width: 44px;
    height: 44px;
  }

  .info-icon {
    font-size: 22px;
  }

  .info-label {
    font-size: 0.8125rem;
  }

  .info-value {
    font-size: 0.9375rem;
  }

  .section-header {
    padding: 18px;
  }

  .section-icon {
    font-size: 22px;
  }

  .section-title {
    font-size: 1.0625rem;
  }

  .section-content {
    padding: 22px;
  }

  .map-iframe {
    height: 420px;
  }

  .map-placeholder {
    min-height: 320px;
    padding: 2.5rem;
  }

  .objective-section,
  .map-section {
    margin-top: 36px;
  }
}

/* Animation for loading states */
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

.info-item,
.section-content,
.map-container {
  animation: fadeIn 0.5s ease-out;
}

/* Focus states for accessibility */
.info-item:focus-within,
.map-iframe:focus {
  outline: 2px solid #6366f1;
  outline-offset: 2px;
}

/* High contrast mode support */
@media (prefers-contrast: high) {
  .info-item {
    border-color: #000;
  }

  .info-label {
    color: #000;
  }

  .info-value {
    color: #000;
  }
}

/* Reduced motion support */
@media (prefers-reduced-motion: reduce) {
  .activity-plan-card,
  .info-item,
  .map-iframe {
    transition: none;
  }

  .info-item,
  .section-content,
  .map-container {
    animation: none;
  }
}
</style>
