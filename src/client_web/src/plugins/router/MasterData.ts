import OrganizationContactListView from '@/views/MasterData/OraganizationContact/OrganizationContactListView.vue';
import OrganizationListView from '@/views/MasterData/Organizations/OrganizationListView.vue';
import ProjectContactListView from '@/views/MasterData/ProjectContact/ProjectContactListView.vue';
import ProjectListView from '@/views/MasterData/Projects/ProjectListView.vue';
import EventTypeListView from '@/views/MasterData/EventType/EventTypeListView.vue';


export default [
  {
    path: '/',
    component: () => import('@/layouts/default.vue'),
    children: [
      {
        path: 'MasterData/OrganizationListView',
        name: 'OrganizationListView',
        component: OrganizationListView,
        meta: {
          requiresAuth: true,
          roles: ['Admin', 'Manager', 'User']
        }
      },
      {
        path: 'MasterData/OrganizationContactListView',
        name: 'OrganizationContactListView',
        component: OrganizationContactListView,
        meta: {
          requiresAuth: true,
          roles: ['Admin', 'Manager', 'User']
        }
      },
      {
        path: 'MasterData/ProjectListView',
        name: 'ProjectListView',
        component: ProjectListView,
        meta: {
          requiresAuth: true,
          roles: ['Admin', 'Manager', 'User']
        }
      },
      {
        path: 'MasterData/ProjectContactListView',
        name: 'ProjectContactListView',
        component: ProjectContactListView,
        meta: {
          requiresAuth: true,
          roles: ['Admin', 'Manager', 'User']
        }
      },
      {
        path: 'MasterData/EventTypeListView',
        name: 'EventTypeListView',
        component: EventTypeListView,
        meta: {
          requiresAuth: true,
          roles: ['Admin', 'Manager']
        }
      },
      {
        path: 'MasterData/UserListView',
        name: 'UserListView',
        component: () => import('@/views/MasterData/Users/UserListView.vue'),
        meta: {
          requiresAuth: true,
          roles: ['Admin']
        }
      },
      {
        path: 'MasterData/DepartmentListView',
        name: 'DepartmentListView',
        component: () => import('@/views/MasterData/Departments/DepartmentListView.vue'),
        meta: {
          requiresAuth: true,
          roles: ['Admin', 'Manager']
        }
      },
      {
        path: 'MasterData/EmployeeListView',
        name: 'EmployeeListView',
        component: () => import('@/views/MasterData/Employees/EmployeeListView.vue'),
        meta: {
          requiresAuth: true,
          roles: ['Admin', 'Manager']
        }
      },
    ],

  }
]
