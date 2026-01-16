import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '../views/HomeView.vue';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
// import AdminView from '../views/AdminView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomeView, meta: { requiresAuth: true } },
    { path: '/login', component: LoginView, meta: { guestOnly: true } },
    { path: '/register', component: RegisterView, meta: { guestOnly: true } },
    // { path: '/admin', component: AdminView, meta: { requiresAuth: true } },
    {
      path: '/admin/logs',
      name: 'admin-logs',
      component: () => import('../views/AdminLogsView.vue'),
      meta: { requiresAuth: true }
    }
  ]
});

// Global Guard
router.beforeEach((to, _from, next) => {
  const loggedIn = localStorage.getItem('user');

  if (to.meta.requiresAuth && !loggedIn) {
    return next('/login');
  }

  if (to.meta.guestOnly && loggedIn) {
    return next('/');
  }
  if (to.meta.guestOnly && loggedIn) {
    return next('/');
  }

  next();
});

export default router;