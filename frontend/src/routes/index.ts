import { createRouter, createWebHistory } from 'vue-router'

const routes = [
    {
        path: '/', name: 'Home',
        component: () => import('@/views/Home.vue')
    },
    {
        path: '/login', name: 'Login',
        component: () => import('@/views/Login.vue')
    },
    {
        path: '/register', name: 'Register',
        component: () => import('@/views/Register.vue')
    },
    {
        path: '/post/:id', name: 'PostDetail',
        component: () => import('@/views/PostDetail.vue')
    },
    {
        path: '/post/create', name: 'CreatePost',
        component: () => import('@/views/CreatePost.vue')
    },
    {
        path: '/post/edit/:id', name: 'EditPost',
        component: () => import('@/views/EditPost.vue')
    },
    {
        path: '/profile', name: 'Profile',
        component: () => import('@/views/Profile.vue')
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
})

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token')
  if (['CreatePost', 'EditPost', 'Profile'].includes(to.name as string) && !token) {
    next('/login')
  } else {
    next()
  }
})

export default router