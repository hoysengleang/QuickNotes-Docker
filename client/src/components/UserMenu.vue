<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';

const props = defineProps<{ user: any }>();
const emit = defineEmits(['logout', 'view-profile']);

const isOpen = ref(false);
const dropdownRef = ref<HTMLElement | null>(null);

const toggle = () => isOpen.value = !isOpen.value;

const close = (e: MouseEvent) => {
    if (dropdownRef.value && !dropdownRef.value.contains(e.target as Node)) {
        isOpen.value = false;
    }
};

onMounted(() => document.addEventListener('click', close));
onUnmounted(() => document.removeEventListener('click', close));
</script>

<template>
    <div class="relative" ref="dropdownRef">
        <!-- Avatar Button -->
        <button 
            @click.stop="toggle"
            class="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-500 to-purple-600 flex items-center justify-center text-white font-bold shadow-lg border border-white/10 hover:ring-2 hover:ring-indigo-400 transition-all"
        >
            {{ user?.Username?.charAt(0).toUpperCase() || 'U' }}
        </button>

        <!-- Dropdown Menu -->
        <transition
            enter-active-class="transition ease-out duration-200"
            enter-from-class="transform opacity-0 scale-95"
            enter-to-class="transform opacity-100 scale-100"
            leave-active-class="transition ease-in duration-75"
            leave-from-class="transform opacity-100 scale-100"
            leave-to-class="transform opacity-0 scale-95"
        >
            <div 
                v-if="isOpen"
                class="absolute right-0 mt-3 w-64 bg-[#1A1B1E]/90 backdrop-blur-xl border border-white/10 rounded-2xl shadow-2xl py-2 z-50 overflow-hidden"
            >
                <!-- User Info Header -->
                <div class="px-5 py-4 border-b border-white/5 bg-white/5">
                    <p class="text-white font-medium truncate">{{ user?.Username }}</p>
                    <div class="flex items-center gap-2 mt-1">
                        <span class="w-2 h-2 rounded-full bg-green-400 animate-pulse"></span>
                        <span class="text-xs text-green-400 font-medium">Active Now</span>
                    </div>
                </div>

                <!-- Menu Items -->
                <div class="py-2">
                    <button 
                        @click="emit('view-profile'); isOpen = false"
                        class="w-full text-left px-5 py-2.5 text-sm text-gray-300 hover:bg-white/10 hover:text-white transition-colors flex items-center gap-3"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
                        View Profile
                    </button>
                </div>

                <div class="border-t border-white/5 py-2">
                    <button 
                        @click="emit('logout')"
                        class="w-full text-left px-5 py-2.5 text-sm text-rose-400 hover:bg-rose-500/10 transition-colors flex items-center gap-3"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" /></svg>
                        Sign Out
                    </button>
                </div>
            </div>
        </transition>
    </div>
</template>
