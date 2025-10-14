import LoginOpenID from '@/views/pages/OpenIds/LoginOpenId.vue';
import NationalIdCardReader from '@/components/NationalIdCardReader.vue'

export default [
  {
    path: '/',
    component: () => import('@/layouts/blank.vue'),
    children: [
      {
        path: 'login-callback',
        name: 'LoginOpenID',
        component: LoginOpenID,
      },
      {
        path: 'NationalIdCardReader',
        name: 'NationalIdCardReader',
        component: NationalIdCardReader,
      },
    ]
  }
]
