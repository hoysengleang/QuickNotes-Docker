<script setup lang="ts">
import { ref, watch } from 'vue';
import type { Note } from '../stores/noteStore';

const props = defineProps<{
    modelValue: boolean;
    noteToEdit?: Note | null;
}>();

const emit = defineEmits(['update:modelValue', 'save', 'delete']);

const title = ref('');
const content = ref('');
const selectedColor = ref('default');
const imageUrl = ref('');
const isEditing = ref(false);
const showColorPicker = ref(false);
const fileInputRef = ref<HTMLInputElement | null>(null);

const colors = [
    { name: 'default', class: 'bg-[#202124] border-zinc-700/50', hex: '#202124' },
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

// Sync with noteToEdit when modal opens
watch(() => props.modelValue, (isOpen) => {
    if (isOpen) {
        if (props.noteToEdit) {
            title.value = props.noteToEdit.title;
            content.value = props.noteToEdit.content;
            selectedColor.value = props.noteToEdit.color || 'default';
            imageUrl.value = props.noteToEdit.imageUrl || '';
            isEditing.value = true;
        } else {
            title.value = '';
            content.value = '';
            selectedColor.value = 'default';
            imageUrl.value = '';
            isEditing.value = false;
        }
    }
});

const close = () => {
    emit('update:modelValue', false);
    showColorPicker.value = false;
};

const save = () => {
    emit('save', {
        id: props.noteToEdit?.id,
        title: title.value,
        content: content.value,
        color: selectedColor.value,
        imageUrl: imageUrl.value
    });
    close();
};

const triggerImageUpload = () => fileInputRef.value?.click();

const handleImageUpload = (event: Event) => {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = (e) => {
            imageUrl.value = e.target?.result as string;
        };
        reader.readAsDataURL(file);
    }
};
</script>

<template>
    <div v-if="modelValue" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <!-- Backdrop -->
        <div class="absolute inset-0 bg-black/60 backdrop-blur-sm" @click="close"></div>

        <!-- Modal -->
        <div 
            class="relative w-full max-w-[600px] border rounded-lg shadow-2xl overflow-hidden flex flex-col max-h-[90vh] animate-in fade-in zoom-in duration-200 transition-colors"
            :class="getColorClass(selectedColor)"
            :style="getStyle(selectedColor)"
        >
             <!-- Image Preview -->
            <div v-if="imageUrl" class="relative">
                <img :src="imageUrl" class="w-full max-h-96 object-cover" />
                <button @click="imageUrl = ''" class="absolute bottom-2 right-2 p-1 bg-black/50 hover:bg-black/70 rounded text-white transform hover:scale-105 transition-all">
                    <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                </button>
            </div>

            <!-- Body -->
            <div class="p-4 flex flex-col gap-2 overflow-y-auto">
                <input 
                    v-model="title" 
                    placeholder="Title" 
                    class="bg-transparent text-white pt-2 pb-2 text-base font-semibold placeholder-gray-300 focus:outline-none w-full placeholder-opacity-70"
                />
                
                <textarea 
                    v-model="content" 
                    placeholder="Type details..." 
                    class="w-full h-64 bg-transparent text-gray-200 py-2 resize-none focus:outline-none text-sm font-normal placeholder-gray-400 placeholder-opacity-70"
                ></textarea>
            </div>

            <!-- Action Bar -->
            <div class="flex items-center justify-between px-4 py-2 flex-row-reverse border-t border-white/10" :class="selectedColor !== 'default' ? 'bg-black/10' : 'bg-[#202124]'">
                 <!-- Save/Close Actions -->
                 <div class="flex items-center gap-3">
                    <button 
                        @click="close" 
                        class="px-4 py-1.5 text-zinc-400 hover:text-white hover:bg-zinc-800 rounded-sm transition-colors text-sm font-medium"
                    >
                        Close
                    </button>
                    <button 
                        @click="save" 
                        class="text-zinc-900 bg-white hover:bg-zinc-200 px-6 py-1.5 rounded-sm transition-colors text-sm font-medium"
                    >
                        Save
                    </button>
                </div>
                
                <!-- Left Actions (Color, Image, Delete) -->
                <div class="flex items-center gap-1 relative">
                    <!-- Delete -->
                    <button 
                        v-if="isEditing"
                        @click="$emit('delete', props.noteToEdit?.id)"
                        class="p-2 text-zinc-400 hover:text-red-400 hover:bg-red-900/20 rounded-full transition-all duration-200 group"
                        title="Delete note"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5 group-hover:scale-110 transition-transform" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                    </button>

                    <!-- Color Picker Toggle -->
                    <button 
                        @click.stop="showColorPicker = !showColorPicker"
                        class="p-2 rounded-full text-zinc-400 hover:text-white hover:bg-zinc-700/50 transition-colors"
                        title="Background options"
                    >
                         <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01" /></svg>
                    </button>

                     <!-- Color Palette Popover -->
                        <div v-if="showColorPicker" class="absolute bottom-full left-0 mb-2 p-2 bg-[#2d2d2d] border border-zinc-700 rounded-lg shadow-xl flex gap-1 z-30 items-center">
                            <button 
                                v-for="color in colors" 
                                :key="color.name"
                                @click="selectedColor = color.name; showColorPicker = false"
                                class="w-6 h-6 rounded-full border border-zinc-600 hover:scale-110 transition-transform"
                                :class="{ 'ring-2 ring-white': selectedColor === color.name }"
                                :style="{ backgroundColor: color.hex }"
                                :title="color.name"
                            ></button>
                            <!-- Custom Color Picker -->
                            <div class="relative w-6 h-6 rounded-full border border-zinc-600 hover:scale-110 transition-transform overflow-hidden ml-1" title="Custom Color">
                                <input 
                                    type="color" 
                                    class="absolute -top-2 -left-2 w-10 h-10 cursor-pointer p-0 border-0"
                                    @input="(e) => selectedColor = (e.target as HTMLInputElement).value"
                                />
                                <div class="absolute inset-0 pointer-events-none bg-gradient-to-br from-red-500 via-green-500 to-blue-500"></div>
                            </div>
                        </div>

                    <!-- Image Upload -->
                    <button 
                        @click.stop="triggerImageUpload"
                        class="p-2 rounded-full text-zinc-400 hover:text-white hover:bg-zinc-700/50 transition-colors"
                        title="Add Image"
                    >
                        <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg>
                    </button>
                    <input type="file" ref="fileInputRef" @change="handleImageUpload" accept="image/*" class="hidden" />
                </div>
            </div>
        </div>
    </div>
</template>
