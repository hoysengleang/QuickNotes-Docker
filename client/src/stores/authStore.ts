import { defineStore } from 'pinia';
import axios from 'axios';

// ⚠️ IMPORTANT: Docker uses localhost:5000
const API_URL = "http://localhost:5000/api";

interface User {
  id: number;
  username: string;
  isAdmin: boolean;
}

export const useAuthStore = defineStore('auth', {
    state: () => ({
        // Restore user from localStorage if they refresh the page
        user: JSON.parse(localStorage.getItem('user') || 'null') as User | null
    }),
    
    actions: {
        async login(username: string) {
            // Call your AuthController
            const res = await axios.post(`${API_URL}/auth/login`, { username });
            
            // Save to State & LocalStorage
            this.user = res.data;
            localStorage.setItem('user', JSON.stringify(res.data));
        },

        logout() {
            this.user = null;
            localStorage.removeItem('user');
        }
    }
});