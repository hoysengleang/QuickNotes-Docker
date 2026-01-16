<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';

const router = useRouter();
const route = useRoute();

const emit = defineEmits(['filter']);
const user = ref<any>(null);
const activeFilter = ref('all'); 
onMounted(() => {
    const userData = localStorage.getItem('user');
    if (userData) {
        user.value = JSON.parse(userData);
    }
});


const items = computed(() => {
    const baseItems = [
        { name: 'Notes', icon: 'lightbulb', filter: 'all' },
        { name: 'Trash', icon: 'trash', filter: 'trash' },
    ];
    
    if (user.value && (user.value.IsAdmin || user.value.isAdmin)) {
        baseItems.push({ name: 'Admin Logs', icon: 'shield-check', filter: 'admin-logs' });
    }
    
    return baseItems;
});

const selectFilter = (filter: string) => {
    if (filter === 'admin-logs') {
        router.push('/admin/logs');
    } else {
        if (route.path !== '/') {
            router.push('/');
        }
        activeFilter.value = filter;
        emit('filter', filter);

    }
};

watch(() => route.path, (path) => {
    if (path === '/admin/logs') {
        activeFilter.value = 'admin-logs';
    } else {
        if (activeFilter.value === 'admin-logs') {
            activeFilter.value = 'all';
        }
    }
}, { immediate: true });

</script>

<template>
    <aside class="w-16 lg:w-60 hidden sm:flex flex-col h-screen sticky top-0 pt-4 bg-[#252526] border-r border-[#333333] z-20">
        <nav class="flex flex-col mt-2">
            <button 
                v-for="item in items" 
                :key="item.name"    
                @click="selectFilter(item.filter)"
                class="w-full flex items-center gap-3 px-4 py-2 text-gray-400 hover:text-gray-100 hover:bg-[#2a2d2e] transition-colors group relative"
                :class="{ 'text-white bg-[#37373d] border-l-2 border-blue-500': false }"
            >
                <svg v-if="item.icon === 'lightbulb'" xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 transition-colors duration-200" :class="activeFilter === item.filter ? 'text-white' : 'text-zinc-400 group-hover:text-zinc-200'" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3M3.343 15.657l.707-.707m12.728 0l-.707.707M6.343 4.636l-.707.707M15.657 17.657l.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" /></svg>
                
                <svg v-else-if="item.icon === 'trash'" xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 transition-colors duration-200" :class="activeFilter === item.filter ? 'text-white' : 'text-zinc-400 group-hover:text-zinc-200'" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                
                <svg v-else-if="item.icon === 'shield-check'" xmlns="http://www.w3.org/2000/svg" class="w-6 h-6 transition-colors duration-200" :class="activeFilter === item.filter ? 'text-white' : 'text-zinc-400 group-hover:text-zinc-200'" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" /></svg>
                <span class="text-sm font-medium hidden lg:block">{{ item.name }}</span>
            </button>
        </nav>
    </aside>
</template>
