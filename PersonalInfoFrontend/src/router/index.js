import { createRouter, createWebHistory } from 'vue-router'
import PersonList from '../views/PersonList.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: PersonList
    }
  ]
})

export default router