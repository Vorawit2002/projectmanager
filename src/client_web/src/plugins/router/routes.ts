export const routes = [
  // { path: '/', redirect: '/Homepage' },
  {
    path: '/',
    component: () => import('@/pages/Homepage.vue'),
  },
  // Component UI Router
  {
    path: '/',
    component: () => import('@/layouts/default.vue'),
    children: [
      {
        path: 'Homepage',
        component: () => import('@/pages/dashboard.vue'),
      },
      {
        path: 'dashboard',
        component: () => import('@/pages/dashboard.vue'),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: 'Calendar',
        component: () => import('@/views/pages/Calendar/CardVueCalendar.vue'),
        meta: {
          requiresAuth: true,
        },
      },
      {
        path: 'account-settings',
        component: () => import('@/pages/account-settings.vue'),
      },
      {
        path: 'typography',
        component: () => import('@/pages/typography.vue'),
      },
      {
        path: 'icons',
        component: () => import('@/pages/icons.vue'),
      },
      {
        path: 'cards',
        component: () => import('@/pages/cards.vue'),
      },
      {
        path: 'tables',
        component: () => import('@/pages/tables.vue'),
      },
      {
        path: 'form-layouts',
        component: () => import('@/pages/form-layouts.vue'),
      },
    ],
    meta: {
      requiresAuth: true,
    },
  },
  // shared activity route (fullscreen, no auth required)
  // {
  //   path: '/activity/:id/shared',
  //   component: () => import('@/views/AppointmentPlan/SharedActivityFullView.vue'),
  // },
  // shared report route (alternative path for shared activity)
  {
    path: '/report/:id',
    component: () => import('@/views/AppointmentPlan/SharedActivityFullView.vue'),
  },
  // authentication routes
  {
    path: '/',
    component: () => import('@/layouts/blank.vue'),
    children: [
      {
        path: 'login',
        component: () => import('@/pages/login.vue'),
      },
      {
        path: 'register',
        component: () => import('@/pages/register.vue'),
      },
      {
        path: 'not-authorized',
        component: () => import('@/pages/not-authorized.vue'),
      },
      {
        path: '/:pathMatch(.*)*',
        component: () => import('@/pages/[...error].vue'),
      },
    ],
  },
  // Customer CardCalendar
  {
    path: '/',
    component: () => import('@/layouts/default.vue'),
    children: [
      {
        path: 'Calendar',
        component: () => import('@/views/pages/Calendar/CardCalendar.vue'),
        meta: {
          requiresAuth: true,
        },
      },
    ],
  },
]
