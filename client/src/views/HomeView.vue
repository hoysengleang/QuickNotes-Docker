<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useNoteStore, type Note } from '../stores/noteStore';
import Sidebar from '../components/Sidebar.vue';
import CreateNote from '../components/CreateNote.vue';
import Header from '../components/Header.vue';
import NoteCard from '../components/NoteCard.vue';
import NoteModal from '../components/NoteModal.vue';
import ProfileModal from '../components/ProfileModal.vue';

const router = useRouter();
const noteStore = useNoteStore();
const user = ref<any>(null);

const isModalOpen = ref(false);
const isProfileOpen = ref(false);
const noteToEdit = ref<Note | null>(null);
const searchQuery = ref('');
const activeFilter = ref('all');
const sortBy = ref('dateDesc'); 
const viewMode = ref('grid'); 

onMounted(async () => {
    const userData = localStorage.getItem('user');
    if (userData) {
        try {
            const parsedUser = JSON.parse(userData);
            if (typeof parsedUser === 'object' && (parsedUser.id || parsedUser.Id)) {
                user.value = parsedUser;
                await noteStore.fetchNotes(user.value.Id || user.value.id);
            } else {
                throw new Error('Invalid user data');
            }
        } catch (e) {
            console.error('Failed to parse user data:', e);
            localStorage.removeItem('user');
            router.push('/login');
        }
    } else {
        router.push('/login');
    }
});

const filteredNotes = computed(() => {
    if (activeFilter.value === 'trash') {
        const trash = [...noteStore.trashNotes];
        if (searchQuery.value) {
            const lower = searchQuery.value.toLowerCase();
            return trash.filter(n => n.title.toLowerCase().includes(lower) || n.content.toLowerCase().includes(lower))
                .sort((a, b) => {
                    const aTitle = a.title.toLowerCase().includes(lower);
                    const bTitle = b.title.toLowerCase().includes(lower);
                    if (aTitle && !bTitle) return -1;
                    if (!aTitle && bTitle) return 1;
                    return 0; 
                });
        }
        return trash;
    }

    let notes = [...noteStore.notes]; 
    
    if (searchQuery.value) {
        const lower = searchQuery.value.toLowerCase();
        notes = notes.filter(n => n.title.toLowerCase().includes(lower) || n.content.toLowerCase().includes(lower));
        
        // Second step: Sort by priority (Title match > Content match)
        notes.sort((a, b) => {
            const aTitle = a.title.toLowerCase().includes(lower);
            const bTitle = b.title.toLowerCase().includes(lower);
            
            // If A has title match and B does not, A comes first
            if (aTitle && !bTitle) return -1;
            // If B has title match and A does not, B comes first
            if (!aTitle && bTitle) return 1;
            
            return 0; // Keep existing order
        });
    }
    
    // Apply user selected sort
    notes.sort((a, b) => {
        if (searchQuery.value) {
           const lower = searchQuery.value.toLowerCase();
           const aTitle = a.title.toLowerCase().includes(lower);
           const bTitle = b.title.toLowerCase().includes(lower);
           
           if (aTitle !== bTitle) {
               return aTitle ? -1 : 1;
           }
        }
        
        switch (sortBy.value) {
            case 'dateDesc': return new Date(b.createdAt || '').getTime() - new Date(a.createdAt || '').getTime();
            case 'dateAsc': return new Date(a.createdAt || '').getTime() - new Date(b.createdAt || '').getTime();
            case 'titleAsc': return (a.title || '').localeCompare(b.title || '');
            case 'titleDesc': return (b.title || '').localeCompare(a.title || '');
            default: return 0;
        }
    });

    return notes;
});

// Watch filter changes to fetch trash
import { watch } from 'vue';
watch(activeFilter, (newFilter) => {
    if (newFilter === 'trash' && user.value) {
        noteStore.fetchTrash(user.value.Id || user.value.id);
    }
});

const pinnedNotes = computed(() => activeFilter.value === 'trash' ? [] : filteredNotes.value.filter(n => n.isPinned));
const otherNotes = computed(() => activeFilter.value === 'trash' ? filteredNotes.value : filteredNotes.value.filter(n => !n.isPinned));

const handleLogout = () => {
    localStorage.removeItem('user');
    router.push('/login');
};

const openEditModal = (note: Note) => {   
    noteToEdit.value = note;
    isModalOpen.value = true;
};

const handleCreate = async (noteData: any) => {
    await noteStore.addNote({ ...noteData, userId: user.value.Id || user.value.id });
};

const handleSave = async (noteData: any) => {
    if (!user.value) return;
    // console.log('Saving note data:', noteData);

    if (noteData.id) {
        const existingNote = noteToEdit.value || {}; 
        await noteStore.updateNote({ 
            ...existingNote, 
            ...noteData, 
            userId: user.value.Id || user.value.id 
        });
    } else {
        await noteStore.addNote({ ...noteData, userId: user.value.Id || user.value.id });
    }
};

const handleUpdate = async (note: Note) => {
     await noteStore.updateNote({ ...note, userId: user.value.Id || user.value.id });
};

const handleDelete = async (id: number) => {
    if (!user.value) return; 
    
    if (activeFilter.value === 'trash') {
        if (confirm("Delete this note forever? This cannot be undone.")) {
            await noteStore.deleteForever(id, user.value.Id || user.value.id);
        }
    } else {
        // Move to trash
        if (confirm("Move this note to trash?")) {
            await noteStore.deleteNote(id, user.value.Id || user.value.id);
        }
    }
};

const handleRestore = async (id: number) => {
    if (!user.value) return;
    await noteStore.restoreNote(id, user.value.Id || user.value.id);
};
</script>

<template>
  <div class="h-screen bg-[#202124] text-zinc-300 font-sans flex flex-col overflow-hidden">
    
    <Header 
        :user="user" 
        :search-query="searchQuery" 
        @update:searchQuery="q => searchQuery = q"
        @logout="handleLogout" 
        @view-profile="isProfileOpen = true" 
        show-search
    />

    <div class="flex flex-1 overflow-hidden relative">
        <Sidebar @filter="f => activeFilter = f" class="border-r border-zinc-800/30" />

        <main class="flex-1 overflow-y-auto relative scroll-smooth bg-[#202124]">
            <div class="max-w-[1200px] mx-auto px-4 sm:px-6 py-8">
                
                <!-- Controls Row: Sort + View Toggle -->
                <div class="flex justify-end mb-6 gap-4">
                    <!-- View Toggle Removed -->

                    <div class="flex items-center gap-2">
                        <span class="text-xs font-medium text-zinc-500 uppercase tracking-wider">Sort by</span>
                        <div class="relative">
                            <select 
                                v-model="sortBy"
                                class="appearance-none bg-[#2d2d2d] text-zinc-300 text-sm font-medium py-1.5 pl-3 pr-8 rounded-sm border border-[#3e3e42] focus:outline-none focus:border-zinc-500 cursor-pointer hover:bg-[#363636] transition-colors"
                            >
                                <option value="dateDesc">Newest First</option>
                                <option value="dateAsc">Oldest First</option>
                                <option value="titleAsc">Title (A-Z)</option>
                                <option value="titleDesc">Title (Z-A)</option>
                            </select>
                            <div class="absolute inset-y-0 right-0 flex items-center px-2 pointer-events-none">
                                <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4 text-zinc-500" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" /></svg>
                            </div>
                        </div>
                    </div>
                </div>


                <div v-if="activeFilter === 'all'" class="flex justify-center mb-10 mt-2">
                    <div class="w-full max-w-xl bg-transparent">
                        <CreateNote @create="handleCreate" />
                    </div>
                </div>

                <div v-if="activeFilter === 'trash'" class="mb-6 flex justify-center">
                    <div class="bg-amber-900/30 text-amber-200 px-4 py-2 rounded-lg text-sm border border-amber-800/50 flex items-center gap-2">
                         <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" /></svg>
                         Notes in Trash are deleted after 7 days.
                    </div>
                </div>

                <transition-group name="list" tag="div" class="space-y-10">
                    <div v-if="pinnedNotes.length > 0 && activeFilter === 'all'" key="pinned-section">
                        <h4 class="text-xs font-bold text-zinc-500 uppercase tracking-widest mb-3 pl-1">Pinned</h4>
                        <div 
                            :class="viewMode === 'grid' 
                                ? 'grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 items-start' 
                                : 'flex flex-col gap-3'"
                        >
                            <NoteCard 
                                v-for="note in pinnedNotes" 
                                :key="note.id" 
                                :note="note" 
                                :view-mode="viewMode"
                                @edit="openEditModal" 
                                @delete="handleDelete" 
                                @update="handleUpdate"
                            />
                        </div>
                    </div>

                    <div v-if="filteredNotes.length > 0" key="notes-section">
                        <h4 v-if="pinnedNotes.length > 0 && activeFilter === 'all'" class="text-xs font-bold text-zinc-500 uppercase tracking-widest mb-3 mt-8 pl-1">Others</h4>
                        <div 
                            :class="viewMode === 'grid' 
                                ? 'grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 items-start pb-20' 
                                : 'flex flex-col gap-3 pb-20'"
                        >
                            <NoteCard 
                                v-for="note in otherNotes" 
                                :key="note.id" 
                                :note="note" 
                                :view-mode="viewMode"
                                :is-trash="activeFilter === 'trash'"
                                @edit="openEditModal" 
                                @delete="handleDelete" 
                                @update="handleUpdate"
                                @restore="handleRestore"
                            />
                        </div>
                    </div>
                </transition-group>

                <div v-if="filteredNotes.length === 0" class="flex flex-col items-center justify-center py-32 opacity-50">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-16 h-16 text-zinc-600 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" /></svg>
                    <p class="text-lg text-zinc-400">No notes here yet.</p>
                </div>
            </div>
        </main>
    </div>

    <NoteModal v-model="isModalOpen" :note-to-edit="noteToEdit" @save="handleSave" @delete="(id) => { handleDelete(id); isModalOpen = false; }" />
    <ProfileModal v-model="isProfileOpen" :user="user" :note-count="noteStore?.notes?.length || 0" />
  </div>
</template>

<style scoped>
.list-move, .list-enter-active, .list-leave-active { transition: all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1); }
.list-enter-from, .list-leave-to { opacity: 0; transform: scale(0.95); }
.list-leave-active { position: absolute; }
</style>