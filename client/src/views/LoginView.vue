<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

// Define strict interfaces
interface User {
    id: number;
    username: string;
    isAdmin: boolean;
}

interface LoginPayload {
    username: string;
}

const router = useRouter();
const username = ref<string>('');
const password = ref<string>(''); // Visual only
const message = ref<string>('');
const loading = ref<boolean>(false);

const handleLogin = async (): Promise<void> => {
    loading.value = true;
    message.value = '';
    await new Promise<void>(r => setTimeout(r, 800));

    try {
        const payload: LoginPayload = { username: username.value };

        // API Call
        const response = await fetch('http://localhost:5000/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });
    
        if (response.ok) {
            const userData: User = await response.json();
            
            if (userData && userData.username) {
                localStorage.setItem('user', userData.username);
                router.push('/'); 
            } else {
                message.value = "Invalid response from server.";
            }
        } else {
             message.value = "Login failed. Please check your username.";
        }
    } catch (error: unknown) {
        console.error(error);
         if (error instanceof Error) {
             message.value = `Connection error: ${error.message}`;
        } else {
             message.value = "An unexpected error occurred.";
        }
    } finally {
        loading.value = false;
    }
};
</script>

<template>
  <div class="min-h-screen flex w-full bg-[#fcfbf9]">
    
    <!-- Left: Artistic Brand Section -->
    <div class="hidden lg:flex w-1/2 bg-[#1c1c1c] text-[#fcfbf9] relative flex-col justify-between p-16 overflow-hidden">
        <!-- Abstract Background Pattern -->
        <div class="absolute inset-0 opacity-20" 
             style="background-image: radial-gradient(#4b5563 1px, transparent 1px); background-size: 30px 30px;">
        </div>
        
        <!-- Decorative Circle -->
        <div class="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-96 h-96 bg-blue-500/10 rounded-full blur-3xl"></div>

        <div class="relative z-10">
            <h1 class="font-serif text-6xl font-bold leading-none tracking-tight mb-6">
                Quick<span class="text-blue-500">Notes</span>.
            </h1>
            <p class="text-xl text-gray-400 font-light max-w-sm leading-relaxed">
                Capture the essence of your thoughts. <br> 
                Organize without boundaries.
            </p>
        </div>

        <!-- Feature Stack -->
        <div class="relative z-10 mt-12 space-y-4 max-w-sm">
            <!-- Card 1 -->
            <div class="bg-white/5 backdrop-blur-lg border border-white/10 p-4 rounded-xl flex items-center gap-4 transform transition-transform hover:scale-105 duration-300">
                <div class="w-10 h-10 rounded-lg bg-blue-500/20 flex items-center justify-center text-blue-400">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19.428 15.428a2 2 0 00-1.022-.547l-2.384-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z" /></svg>
                </div>
                <div>
                    <h4 class="text-white font-medium text-sm">Smart Organization</h4>
                    <p class="text-gray-400 text-xs mt-0.5">Auto-tagging and intelligent folders.</p>
                </div>
            </div>

            <!-- Card 2 -->
            <div class="bg-white/5 backdrop-blur-lg border border-white/10 p-4 rounded-xl flex items-center gap-4 transform translate-x-4 transition-transform hover:translate-x-6 duration-300">
                <div class="w-10 h-10 rounded-lg bg-purple-500/20 flex items-center justify-center text-purple-400">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" /></svg>
                </div>
                <div>
                    <h4 class="text-white font-medium text-sm">Real-time Sync</h4>
                    <p class="text-gray-400 text-xs mt-0.5">Access your notes on any device.</p>
                </div>
            </div>

            <!-- Card 3 -->
            <div class="bg-white/5 backdrop-blur-lg border border-white/10 p-4 rounded-xl flex items-center gap-4 transform translate-x-8 transition-transform hover:translate-x-10 duration-300">
                <div class="w-10 h-10 rounded-lg bg-green-500/20 flex items-center justify-center text-green-400">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                </div>
                <div>
                    <h4 class="text-white font-medium text-sm">Tasks & Goals</h4>
                    <p class="text-gray-400 text-xs mt-0.5">Integrated workspace for doers.</p>
                </div>
            </div>
        </div>

        <div class="relative z-10 text-xs text-gray-600 uppercase tracking-widest">
            © 2026 QuickNotes Inc.
        </div>
    </div>
    
    <!-- Right: Login Form -->
    <div class="w-full lg:w-1/2 flex items-center justify-center p-8 lg:p-16 pb-32 relative">
        <div class="w-full max-w-md space-y-10">
            
            <div class="text-center lg:text-left">
                <h2 class="font-serif text-4xl font-bold text-gray-900 mb-2">Welcome back</h2>
                <p class="text-gray-500">Please enter your details to sign in.</p>
            </div>

            <!-- Social Login Placeholders -->
             <div class="grid grid-cols-2 gap-4">
                <button class="flex items-center justify-center gap-2 py-2.5 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
                     <img src="https://www.svgrepo.com/show/475656/google-color.svg" class="w-5 h-5" alt="Google">
                     <span class="text-sm font-medium text-gray-700">Google</span>
                </button>
                 <button class="flex items-center justify-center gap-2 py-2.5 border border-gray-200 rounded-lg hover:bg-gray-50 transition-colors">
                     <img src="https://www.svgrepo.com/show/512317/github-142.svg" class="w-5 h-5" alt="GitHub">
                     <span class="text-sm font-medium text-gray-700">GitHub</span>
                </button>
             </div>

             <div class="relative flex items-center gap-4">
                 <div class="h-px bg-gray-200 w-full"></div>
                 <span class="text-xs text-gray-400 uppercase tracking-wider bg-[#fcfbf9] px-2 whitespace-nowrap">Or sign in with email</span>
                 <div class="h-px bg-gray-200 w-full"></div>
             </div>

            <form @submit.prevent="handleLogin" class="space-y-6">
                
                <div class="space-y-1.5">
                    <label class="block text-sm font-medium text-gray-700">Username</label>
                    <input 
                        v-model="username" 
                        type="text" 
                        required
                        placeholder="e.g. johndoe"
                        class="w-full px-4 py-3 bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all placeholder-gray-300" 
                    />
                </div>

                <div class="space-y-1.5">
                    <label class="block text-sm font-medium text-gray-700">Password</label>
                    <input 
                        v-model="password" 
                        type="password" 
                        placeholder="••••••••"
                        class="w-full px-4 py-3 bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all placeholder-gray-300" 
                    />
                </div>

                <div class="flex items-center justify-between">
                    <div class="flex items-center gap-2">
                         <input type="checkbox" id="remember" class="rounded border-gray-300 text-blue-600 focus:ring-blue-500">
                         <label for="remember" class="text-sm text-gray-600 select-none cursor-pointer">Remember me</label>
                    </div>
                    <a href="#" class="text-sm font-medium text-blue-600 hover:text-blue-700">Forgot password?</a>
                </div>

                <button 
                    type="submit" 
                    :disabled="loading"
                    class="w-full bg-black text-white py-3.5 rounded-lg font-medium hover:bg-gray-800 focus:ring-4 focus:ring-gray-200 transition-all disabled:opacity-70 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                >
                    <span v-if="loading" class="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                    {{ loading ? 'Signing in...' : 'Sign in' }}
                </button>
            </form>

            <div v-if="message" class="p-3 bg-red-50 text-red-600 text-sm rounded-md border border-red-100 flex items-center gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor"><path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" /></svg>
                {{ message }}
            </div>

            <p class="text-center text-sm text-gray-500">
                Don't have an account? 
                <a href="/register" @click.prevent="router.push('/register')" class="font-semibold text-black hover:underline">Sign up</a>
            </p>
        </div>
    </div>
  </div>
</template>