<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';

// Define strict interfaces
interface User {
    id: number;
    username: string;
    isAdmin: boolean;
}

interface RegisterPayload {
    username: string;
}

const router = useRouter();
const username = ref<string>('');
const email = ref<string>(''); // Visual only
const password = ref<string>(''); // Visual only
const confirmPassword = ref<string>(''); // Visual only
const message = ref<string>('');
const loading = ref<boolean>(false);

const handleRegister = async (): Promise<void> => {
    loading.value = true;
    message.value = '';
    
    // Simulate delay
    await new Promise<void>(r => setTimeout(r, 800));

    try {
        const payload: RegisterPayload = { username: username.value };

        // API Call (Auto-provisioning handled by Login endpoint)
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
               message.value = "Account created but invalid response received.";
           }
        } else {
            message.value = "Unable to create account. Please try again.";
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
                Start <br/><span class="text-blue-500 italic">Something</span> New.
            </h1>
            <p class="text-xl text-gray-400 font-light max-w-sm leading-relaxed">
                Join our community of thinkers, creators, and doers.
            </p>
        </div>

        <!-- Feature Stack -->
        <div class="relative z-10 grid grid-cols-1 gap-4 mt-12 max-w-sm">
             <!-- Card 1 -->
             <div class="bg-white/5 backdrop-blur-lg border border-white/10 p-4 rounded-xl flex items-center gap-4 transition-transform hover:scale-105 duration-300">
                <div class="w-10 h-10 rounded-lg bg-indigo-500/20 flex items-center justify-center text-indigo-400">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 15a4 4 0 004 4h9a5 5 0 10-.1-9.999 5.002 5.002 0 10-9.78 2.096A4.001 4.001 0 003 15z" /></svg>
                </div>
                <div>
                    <h4 class="text-white font-medium text-sm">Cloud Sync</h4>
                    <p class="text-gray-400 text-xs mt-0.5">Keep your notes safe and accessible anywhere.</p>
                </div>
             </div>

             <!-- Card 2 -->
             <div class="bg-white/5 backdrop-blur-lg border border-white/10 p-4 rounded-xl flex items-center gap-4 ml-6 transition-transform hover:scale-105 duration-300 delay-75">
                 <div class="w-10 h-10 rounded-lg bg-rose-500/20 flex items-center justify-center text-rose-400">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" /></svg>
                 </div>
                 <div>
                     <h4 class="text-white font-medium text-sm">Markdown Support</h4>
                     <p class="text-gray-400 text-xs mt-0.5">Write in a clean, standard format.</p>
                 </div>
             </div>
        </div>

        <div class="relative z-10 text-xs text-gray-600 uppercase tracking-widest mt-12">
            © 2026 QuickNotes Inc.
        </div>
    </div>

    <!-- Right: Register Form -->
    <div class="w-full lg:w-1/2 flex items-center justify-center p-8 lg:p-16 pb-32 relative">
        <div class="w-full max-w-md space-y-8">
            
            <div class="text-center lg:text-left">
                <h2 class="font-serif text-4xl font-bold text-gray-900 mb-2">Create Account</h2>
                <p class="text-gray-500">Enter your details to get started.</p>
            </div>

            <form @submit.prevent="handleRegister" class="space-y-5">
                
                <div class="space-y-1.5">
                    <label class="block text-sm font-medium text-gray-700">Username</label>
                    <input 
                        v-model="username" 
                        type="text" 
                        required
                        placeholder="Choose a unique handle"
                        class="w-full px-4 py-3 bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all placeholder-gray-300" 
                    />
                </div>

                <div class="space-y-1.5">
                    <label class="block text-sm font-medium text-gray-700">Email Address <span class="text-gray-400 font-normal">(Optional)</span></label>
                    <input 
                        v-model="email" 
                        type="email" 
                        placeholder="you@example.com"
                        class="w-full px-4 py-3 bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all placeholder-gray-300" 
                    />
                </div>

                <div class="grid grid-cols-2 gap-4">
                    <div class="space-y-1.5">
                        <label class="block text-sm font-medium text-gray-700">Password</label>
                        <input 
                            v-model="password" 
                            type="password" 
                            placeholder="••••••••"
                            class="w-full px-4 py-3 bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all placeholder-gray-300" 
                        />
                    </div>
                    <div class="space-y-1.5">
                        <label class="block text-sm font-medium text-gray-700">Confirm</label>
                        <input 
                            v-model="confirmPassword" 
                            type="password" 
                            placeholder="••••••••"
                            class="w-full px-4 py-3 bg-white border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500/20 focus:border-blue-500 transition-all placeholder-gray-300" 
                        />
                    </div>
                </div>

                <div class="flex items-start gap-2 pt-2">
                     <input type="checkbox" id="terms" class="mt-1 rounded border-gray-300 text-blue-600 focus:ring-blue-500">
                     <label for="terms" class="text-sm text-gray-600 select-none cursor-pointer">
                        I agree to the <a href="#" class="text-blue-600 hover:underline">Terms of Service</a> and <a href="#" class="text-blue-600 hover:underline">Privacy Policy</a>.
                     </label>
                </div>

                <button 
                    type="submit" 
                    :disabled="loading"
                    class="w-full bg-black text-white py-3.5 rounded-lg font-medium hover:bg-gray-800 focus:ring-4 focus:ring-gray-200 transition-all disabled:opacity-70 disabled:cursor-not-allowed flex items-center justify-center gap-2"
                >
                    <span v-if="loading" class="w-5 h-5 border-2 border-white/30 border-t-white rounded-full animate-spin"></span>
                    {{ loading ? 'Creating Account...' : 'Create Account' }}
                </button>
            </form>

            <div v-if="message" class="p-3 bg-red-50 text-red-600 text-sm rounded-md border border-red-100 flex items-center gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor"><path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z" clip-rule="evenodd" /></svg>
                {{ message }}
            </div>

            <p class="text-center text-sm text-gray-500">
                Already have an account? 
                <a href="/login" @click.prevent="router.push('/login')" class="font-semibold text-black hover:underline">Sign in</a>
            </p>
        </div>
    </div>
  </div>
</template>
