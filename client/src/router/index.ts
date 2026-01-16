import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '../views/HomeView.vue';
import LoginView from '../views/LoginView.vue';
import RegisterView from '../views/RegisterView.vue';
import AdminView from '../views/AdminView.vue';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomeView, meta: { requiresAuth: true } },
    { path: '/login', component: LoginView },
    { path: '/register', component: RegisterView },
    { path: '/admin', component: AdminView, meta: { requiresAuth: true } }
  ]
});

// 🔒 Global Guard: Check if user is logged in
router.beforeEach((to, _from, next) => {
  const loggedIn = localStorage.getItem('user');

  if (to.meta.requiresAuth && !loggedIn) {
    next('/login'); // Redirect to Login
  } else {
    next(); // Allow access
  }
});

export default router;