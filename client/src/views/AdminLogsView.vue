<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import Sidebar from '../components/Sidebar.vue';
import Header from '../components/Header.vue';


const router = useRouter();
const logs = ref<any[]>([]);
const loading = ref(true);
const error = ref<string | null>(null);
const user = ref<any>(null);
const searchQuery = ref('');

const fetchLogs = async () => {
    try {
        const userData = localStorage.getItem('user');
        if (!userData) throw new Error("Not authenticated");
        user.value = JSON.parse(userData);

        if (!user.value.IsAdmin && !user.value.isAdmin) {
             router.push('/');
             return;
        }

        const url = `${import.meta.env.VITE_API_BASE_URL}/api/logs?userId=${user.value.Id || user.value.id}`;
        
        const response = await fetch(url);
        if (!response.ok) throw new Error(`Failed to fetch logs: ${response.statusText}`);
        logs.value = await response.json();
    } catch (err: any) {
        error.value = err.message;
    } finally {
        loading.value = false;
    }
};

const filteredLogs = computed(() => {
    if (!searchQuery.value) return logs.value;
    const lower = searchQuery.value.toLowerCase();
    return logs.value.filter(log => 
        (log.message || log.Message || '').toLowerCase().includes(lower) ||
        (log.source || log.Source || '').toLowerCase().includes(lower) ||
        (log.id || log.Id || '').toString().includes(lower)
    );
});

const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString();
};

const handleLogout = () => {
    localStorage.removeItem('user');
    router.push('/login');
};

onMounted(() => {
    fetchLogs();
});
</script>

<template>
    <div class="h-screen bg-[#202124] text-zinc-300 font-sans flex flex-col overflow-hidden">
        <Header 
            :user="user" 
            :search-query="searchQuery" 
            @update:searchQuery="q => searchQuery = q"
            @logout="handleLogout" 
            @view-profile="$router.push('/profile')" 
            show-search
        />

        <div class="flex flex-1 overflow-hidden relative">
            <Sidebar class="border-r border-zinc-800/30" />

            <main class="flex-1 overflow-y-auto relative p-8">
                <div class="max-w-6xl mx-auto">
                    <header class="mb-8 flex items-center justify-between">
                        <div>
                            <h1 class="text-2xl font-bold text-white mb-2">System Error Logs</h1>
                            <p class="text-zinc-500 text-sm">Monitor application errors and backend exceptions.</p>
                        </div>
                        <button @click="fetchLogs" class="p-2 bg-zinc-800 rounded-md hover:bg-zinc-700 text-zinc-400 hover:text-white transition-colors" title="Refresh">
                            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" /></svg>
                        </button>
                    </header>

                    <div v-if="loading" class="flex justify-center py-20">
                        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-white"></div>
                    </div>

                    <div v-else-if="error" class="bg-red-900/20 text-red-400 p-4 rounded border border-red-900/50">
                        {{ error }}
                    </div>

                    <div v-else class="bg-[#252526] rounded-lg border border-zinc-700/50 overflow-hidden shadow-xl">
                        <table class="w-full text-left border-collapse">
                            <thead>
                                <tr class="bg-[#2d2d30] border-b border-zinc-700">
                                    <th class="py-3 px-4 font-medium text-xs text-zinc-400 uppercase tracking-wider w-24">ID</th>
                                    <th class="py-3 px-4 font-medium text-xs text-zinc-400 uppercase tracking-wider w-48">Timestamp</th>
                                    <th class="py-3 px-4 font-medium text-xs text-zinc-400 uppercase tracking-wider w-24">Source</th> 
                                    <th class="py-3 px-4 font-medium text-xs text-zinc-400 uppercase tracking-wider">Message</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-zinc-700/50">
                                <tr v-for="log in filteredLogs" :key="log.id || log.Id" class="hover:bg-zinc-800/50 transition-colors">
                                    <td class="py-3 px-4 text-sm font-mono text-zinc-500">#{{ log.id || log.Id }}</td>
                                    <td class="py-3 px-4 text-sm text-zinc-400">{{ formatDate(log.createdAt || log.CreatedAt) }}</td>
                                    <td class="py-3 px-4">
                                        <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium"
                                              :class="log.source === 'Backend' ? 'bg-purple-900/30 text-purple-400 border border-purple-800/50' : 'bg-blue-900/30 text-blue-400 border border-blue-800/50'">
                                            {{ log.source || log.Source }}
                                        </span>
                                    </td>
                                    <td class="py-3 px-4 text-sm text-zinc-200 font-medium">
                                        {{ log.message || log.Message }}
                                        <div v-if="log.stackTrace || log.StackTrace" class="mt-1 text-xs text-zinc-500 font-mono overflow-x-auto whitespace-pre p-2 bg-black/20 rounded">
                                            {{ (log.stackTrace || log.StackTrace).substring(0, 200) }}...
                                        </div>
                                    </td>
                                </tr>
                                <tr v-if="filteredLogs.length === 0">
                                    <td colspan="4" class="py-12 text-center text-zinc-500">No logs found.</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </main>
        </div>
    </div>
</template>
