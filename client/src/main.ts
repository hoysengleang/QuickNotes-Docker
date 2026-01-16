import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import App from './App.vue'
import router from './router'
import { setupGlobalErrorHandling } from './errorHandler'

const app = createApp(App)

setupGlobalErrorHandling()

app.use(createPinia())
app.use(router)
app.mount('#app')