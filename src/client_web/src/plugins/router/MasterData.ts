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
          meta:{
          requiresAuth:true
        }
      },
      {
        path: 'MasterData/OrganizationContactListView',
        name: 'OrganizationContactListView',
        component: OrganizationContactListView,
          meta:{
          requiresAuth:true
        }
      },
      {
        path: 'MasterData/ProjectListView',
        name: 'ProjectListView',
        component: ProjectListView,
          meta:{
          requiresAuth:true
        }
      },
      {
        path: 'MasterData/ProjectContactListView',
        name: 'ProjectContactListView',
        component: ProjectContactListView,
          meta:{
          requiresAuth:true
        }
      },
      {
        path: 'MasterData/EventTypeListView',
        name: 'EventTypeListView',
        component: EventTypeListView,
          meta:{
          requiresAuth:true
        }
      },
    ],

  }
]
