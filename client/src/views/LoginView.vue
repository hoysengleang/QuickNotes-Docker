<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();
const username = ref('');
const message = ref('');

const handleLogin = async () => {
  try {
    // 1. Call your .NET API
    const response = await fetch(`http://localhost:5000/api/users?username=${username.value}`);
    
    if (response.ok) {
      // ✅ SUCCESS: Save user to storage so the Router Guard lets us pass
      localStorage.setItem('user', username.value);
      
      // 🚀 Redirect to Home Page
      router.push('/'); 
    } else {
      message.value = "❌ User not found. (Try 'Admin')";
    }
  } catch (error) {
    console.error(error);
    message.value = "❌ API Error. Is Docker running?";
  }
};
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-gray-100">
    <div class="w-full max-w-md bg-white p-8 rounded shadow-md">
      <h2 class="text-2xl font-bold mb-6 text-center text-gray-800">Login</h2>
      <form @submit.prevent="handleLogin" class="space-y-4">
        <div>
          <label class="block text-sm font-medium text-gray-700">Username</label>
          <input v-model="username" type="text" placeholder="Try 'Admin'" class="mt-1 block w-full p-2 border rounded" />
        </div>
        <button type="submit" class="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700">
          Login
        </button>
      </form>
      <p v-if="message" class="mt-4 text-center text-red-600">{{ message }}</p>
    </div>
  </div>
</template>