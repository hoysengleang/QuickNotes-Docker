<script setup lang="ts">
import type { Note } from '../stores/noteStore';

const props = defineProps<{
    note: Note;
    viewMode?: string;
    isTrash?: boolean;
}>();

const emit = defineEmits(['edit', 'delete', 'update', 'restore']);

const colors = [
    { name: 'default', class: 'bg-[#202124] border-[#3e3e42]', hex: '#202124' },
    { name: 'red', class: 'bg-[#5c2b29] border-[#5c2b29]', hex: '#5c2b29' },
    { name: 'green', class: 'bg-[#345920] border-[#345920]', hex: '#345920' },
    { name: 'blue', class: 'bg-[#16504b] border-[#16504b]', hex: '#16504b' },
    { name: 'purple', class: 'bg-[#42275e] border-[#42275e]', hex: '#42275e' },
    { name: 'brown', class: 'bg-[#614a19] border-[#614a19]', hex: '#614a19' },
    { name: 'gray', class: 'bg-[#3c3f43] border-[#3c3f43]', hex: '#3c3f43' },
];

const getColorClass = (name: string) => {
    if (name.startsWith('#')) return 'border-zinc-700/50';
    return colors.find(c => c.name === name)?.class ?? colors[0]!.class;
};

const getStyle = (name: string) => {
    if (name.startsWith('#')) return { backgroundColor: name };
    return {};
};

const togglePin = () => {
    emit('update', { ...props.note, isPinned: !props.note.isPinned });
};

const handleEdit = () => {
    emit('edit', props.note);
};

const handleDelete = () => {
    emit('delete', props.note.id);
};
</script>

<template>
    <div 
        class="group relative border rounded-lg hover:shadow-md transition-all duration-200 flex flex-col h-auto min-h-[100px] overflow-hidden"
        :class="[getColorClass(note.color || 'default'), viewMode === 'list' ? 'flex-row items-center p-2' : 'p-4']"
        :style="getStyle(note.color || 'default')"
    >
        <!-- Image Preview -->
        <div v-if="note.imageUrl" :class="viewMode === 'list' ? 'w-16 h-16 mr-4 shrink-0' : '-mx-4 -mt-4 mb-3'">
             <img :src="note.imageUrl" class="object-cover w-full h-full" :class="viewMode === 'list' ? 'rounded' : ''" />
        </div>
        <!-- Select/Checkmark (Future feature) -->
        <div class="absolute top-2 left-2 opacity-0 group-hover:opacity-100 transition-opacity z-10">
             <div class="w-5 h-5 bg-zinc-800 rounded-full border border-zinc-600 flex items-center justify-center cursor-pointer hover:bg-zinc-700 hover:border-zinc-500">
                 <svg xmlns="http://www.w3.org/2000/svg" class="w-3 h-3 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" /></svg>
             </div>
        </div>

        <!-- Pin Button -->
        <button 
            @click.stop="togglePin"
            class="absolute top-2 right-2 p-1.5 rounded-full text-zinc-500 hover:text-zinc-200 hover:bg-zinc-800 opacity-0 group-hover:opacity-100 transition-all z-10"
            :class="{ 'opacity-100 text-white': note.isPinned }"
            title="Pin note"
        >
            <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" :fill="note.isPinned ? 'currentColor' : 'none'" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" /></svg>
        </button>

        <!-- Content Area (Click to Edit) -->
        <div @click="handleEdit" class="cursor-default">
            <h3 v-if="note.title" class="font-medium text-zinc-200 mb-2 truncate text-base leading-tight">{{ note.title }}</h3>
            <p v-if="note.content" class="text-zinc-400 text-sm whitespace-pre-wrap leading-relaxed line-clamp-[8]">{{ note.content }}</p>
            <div v-if="!note.title && !note.content" class="text-zinc-600 italic text-sm">Empty note</div>
        </div>

        <!-- Footer Actions (Hover) -->
        <div 
            class="flex items-center justify-between opacity-0 group-hover:opacity-100 transition-opacity"
            :class="viewMode === 'list' ? 'ml-4 shrink-0' : 'mt-4 pt-2 w-full'"
        >
            <div class="flex items-center gap-1">
                 <button 
                    v-if="!isTrash"
                    @click.stop="handleEdit"
                    class="p-2 rounded-full text-zinc-500 hover:text-zinc-200 hover:bg-zinc-800 transition-colors" 
                    title="Edit"
                >
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" /></svg>
                </button>
                <button 
                    v-if="isTrash"
                    @click.stop="emit('restore', note.id)"
                    class="p-2 rounded-full text-zinc-500 hover:text-zinc-200 hover:bg-zinc-800 transition-colors" 
                    title="Restore"
                >
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" /></svg>
                </button>
            </div>
            
            <button 
                @click.stop="handleDelete"
                class="p-2 rounded-full text-zinc-500 hover:text-red-400 hover:bg-red-900/30 transition-colors" 
                :title="isTrash ? 'Delete Forever' : 'Delete'"
            >
                <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
            </button>
        </div>
    </div>
</template>