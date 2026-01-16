<script setup lang="ts">
const props = defineProps<{
    modelValue: boolean;
    user?: any;
    noteCount?: number;
}>();

const emit = defineEmits(['update:modelValue']);

import { watch } from 'vue';
watch(() => props.modelValue, (newVal) => {
    if (newVal) {
        console.log('ProfileModal opened. User prop:', props.user);
    }
});

const close = () => {
    emit('update:modelValue', false);
};
</script>

<template>
    <transition name="modal">
        <div v-if="modelValue" class="fixed inset-0 z-[60] flex items-center justify-center p-4">
            <!-- Backdrop -->
            <div class="fixed inset-0 bg-black/60 backdrop-blur-sm transition-opacity" @click="close"></div>
            
            <!-- Modal Window -->
            <div class="relative bg-[#202124] w-full max-w-sm rounded-2xl shadow-2xl border border-zinc-700/50 transform transition-all flex flex-col overflow-hidden font-sans">
                
                <!-- Header with Close Button -->
                <div class="px-5 py-4 flex justify-end items-center absolute top-0 right-0 z-10">
                    <button @click="close" class="p-2 rounded-full text-zinc-400 hover:text-zinc-100 hover:bg-zinc-700/50 transition-colors">
                        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
                    </button>
                </div>

                <!-- Profile Content -->
                <div class="p-8 flex flex-col items-center text-center">
                    <!-- Avatar -->
                    <div class="w-24 h-24 rounded-full bg-gradient-to-br from-blue-600 to-indigo-700 flex items-center justify-center text-4xl font-bold text-white shadow-lg mb-5 border-4 border-[#202124] ring-2 ring-zinc-700/50">
                        {{ (user?.username || user?.Username || 'U').charAt(0).toUpperCase() }}
                    </div>
                    
                    <!-- Username -->
                    <h2 class="text-2xl font-bold text-white mb-1">{{ user?.username || user?.Username || 'Unknown User' }}</h2>
                    
                    <!-- Role Badge -->
                    <div v-if="user?.isAdmin || user?.IsAdmin" class="mt-2 text-xs font-bold px-2 py-0.5 rounded bg-purple-900/50 text-purple-300 border border-purple-700/50 inline-block uppercase tracking-wider">
                        Administrator
                    </div>
                     <div v-else class="mt-2 text-xs font-bold px-2 py-0.5 rounded bg-blue-900/30 text-blue-300 border border-blue-800/50 inline-block uppercase tracking-wider">
                        Standard User
                    </div>


                    <!-- Stats Grid -->
                    <div class="flex justify-center w-full mt-8">
                        <div class="bg-[#2a2b2e] p-4 rounded-xl border border-zinc-700/30 flex flex-col items-center w-full max-w-[150px]">
                            <span class="text-3xl font-bold text-white mb-1">{{ noteCount || 0 }}</span>
                            <span class="text-xs text-zinc-400 uppercase tracking-wide font-medium">Total Notes</span>
                        </div>
                    </div>
                </div>

                <!-- Footer INFO -->
                <div class="px-6 py-4 bg-[#252526] border-t border-zinc-800 flex justify-between items-center text-xs text-zinc-500">
                    <span>Member since 2026</span>
                    <button @click="close" class="text-zinc-400 hover:text-white transition-colors font-medium">Done</button>
                </div>
            </div>
        </div>
    </transition>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: all 0.2s cubic-bezier(0.16, 1, 0.3, 1);
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
  transform: scale(0.95);
}
</style>
