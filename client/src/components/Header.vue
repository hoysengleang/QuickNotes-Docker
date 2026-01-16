<script setup lang="ts">
import UserMenu from './UserMenu.vue';

const props = defineProps<{
    user: any;
    showSearch?: boolean;
    searchQuery?: string;
}>();

const emit = defineEmits(['update:searchQuery', 'logout', 'view-profile']);

const onSearchInput = (event: Event) => {
    emit('update:searchQuery', (event.target as HTMLInputElement).value);
};
</script>

<template>
    <header class="sticky top-0 z-50 bg-[#202124] border-b border-zinc-800/50 h-16 flex items-center px-4 shrink-0 justify-between shadow-sm">
        <div class="flex items-center gap-6 w-auto">
             <!-- Logo -->
            <div class="flex items-center gap-3 w-60 cursor-pointer" @click="$router.push('/')">
                <div class="w-9 h-9 bg-blue-600 rounded-xl flex items-center justify-center shadow-lg shadow-blue-900/20">
                    <span class="text-white font-bold text-xl leading-none">Q</span>
                </div>
                <span class="text-lg font-medium text-zinc-100 tracking-tight">QuickNotes</span>
            </div>

             <!-- Search -->
            <div v-if="showSearch" class="w-full max-w-xl hidden sm:block">
                <div class="relative group">
                    <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 text-zinc-500 group-focus-within:text-zinc-300 transition-colors" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" /></svg>
                    </div>
                    <input 
                        :value="searchQuery"
                        @input="onSearchInput"
                        type="text" 
                        placeholder="Search" 
                        class="w-full bg-zinc-800/80 text-zinc-100 rounded-lg py-2.5 pl-12 pr-4 text-base focus:bg-white focus:text-black focus:placeholder-zinc-600 outline-none transition-all placeholder-zinc-500 shadow-inner"
                    />
                </div>
            </div>
        </div>

        <div class="flex items-center gap-4 justify-end">
            <UserMenu :user="user" @logout="emit('logout')" @view-profile="emit('view-profile')" />
        </div>
    </header>
</template>
