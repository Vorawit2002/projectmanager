import CreateCustomerAppointmentPlan from '@/views/AppointmentPlan/CreateCustomerAppointmentPlan.vue'
import CustomerAppointmentByIdDetailView from '@/views/AppointmentPlan/CustomerAppointmentByIdDetailView.vue'
import CustomerAppointmentPlanListView from '@/views/AppointmentPlan/CustomerAppointmentPlanListView.vue'
import AppointmentOutcome from '@/views/AppointmentPlan/ManageActivityDetail.vue'
import OnsiteDetailViews from '@/views/AppointmentPlan/OnsiteDetailViews.vue'
import OnsiteForm from '@/views/AppointmentPlan/OnsiteForm.vue'
import OnsiteViews from '@/views/AppointmentPlan/OnsiteViews.vue'
import ReportCustomerAppointmentPlanListView from '@/views/AppointmentPlan/ReportCustomerAppointmentPlanListView.vue'
import SharedActivityFullView from '@/views/AppointmentPlan/SharedActivityFullView.vue'
import UpdateAppointmentOutcome from '@/views/AppointmentPlan/UpdateAppointmentOutcome.vue'
import UpdateCustomerAppointmentPlan from '@/views/AppointmentPlan/UpdateCustomerAppointmentPlan.vue'
export default [
  {
    path: '/',
    component: () => import('@/layouts/default.vue'),
    children: [
      {
        path: 'CustomerAppointmentListView',
        name: 'CustomerAppointmentListView',
        component: CustomerAppointmentPlanListView,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'CreateCustomerAppointmentPlanDetailview',
        name: 'CreateCustomerAppointmentPlanDetailview',
        component: CreateCustomerAppointmentPlan,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'CustomerAppointmentByIdDetailView/:id',
        name: 'CustomerAppointmentByIdDetailView',
        component: CustomerAppointmentByIdDetailView,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'UpdateCustomerAppointmentPlanDetailview/:id',
        name: 'UpdateCustomerAppointmentPlanDetailview',
        component: UpdateCustomerAppointmentPlan,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'AppointmentOutcomeDetailview/:id',
        name: 'AppointmentOutcomeDetailview',
        component: AppointmentOutcome,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'UpdateAppointmentOutcomeDetailview/:id',
        name: 'UpdateAppointmentOutcomeDetailview',
        component: UpdateAppointmentOutcome,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'ReportCustomerAppointmentPlanListView',
        name: 'ReportCustomerAppointmentPlanListView',
        component: ReportCustomerAppointmentPlanListView,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'OnsiteViews',
        name: 'OnsiteViews',
        component: OnsiteViews,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'OnsiteForm/:mode?',
        name: 'OnsiteForm',
        component: OnsiteForm,
        props: true,
        meta: {
          requiresAuth: true
        }
      },
      {
        path: 'OnsiteDetailViews',
        name: 'OnsiteDetailViews',
        component: OnsiteDetailViews,
        meta: {
          requiresAuth: true
        }
      },
    ],
    meta: {
      requiresAuth: true,
    },
  },
  {
    path: '/',
    component: () => import('@/layouts/blank.vue'),
    children: [
      {
        path: 'activity/:id/shared',
        name: 'SharedActivityFullView',
        component: SharedActivityFullView,
        meta: {
          requiresAuth: true
        }
      },
    ]
  },
]
