<script setup lang="ts">
import { ref, nextTick, onMounted, onUnmounted } from 'vue';

const emit = defineEmits(['create']);

const isExpanded = ref(false);
const title = ref('');
const content = ref('');
const selectedColor = ref('default');
const imageUrl = ref('');
const textareaRef = ref<HTMLTextAreaElement | null>(null);
const containerRef = ref<HTMLElement | null>(null);
const fileInputRef = ref<HTMLInputElement | null>(null);
const showColorPicker = ref(false);

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

const expandForm = async () => {
    isExpanded.value = true;
    await nextTick();
    textareaRef.value?.focus();
};

const closeForm = () => {
    // Only save if content exists
    if (title.value.trim() || content.value.trim() || imageUrl.value) {
        emit('create', {
            title: title.value,
            content: content.value,
            isPinned: false,
            color: selectedColor.value,
            imageUrl: imageUrl.value
        });
    }
    
    // Reset
    title.value = '';
    content.value = '';
    selectedColor.value = 'default';
    imageUrl.value = '';
    isExpanded.value = false;
    showColorPicker.value = false;
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

const handleClickOutside = (event: MouseEvent) => {
    if (containerRef.value && !containerRef.value.contains(event.target as Node) && isExpanded.value) {
        // Explicitly close without saving if user prefers? 
        // Standard "Keep" behavior is auto-save.
        // But user said "I need untill click save".
        // To support that, we should NOT emit 'create' here?
        // But then how do they save? There's a Save button.
        // User asked "why when change color its getting save".
        // Let's keep auto-save for now but ensure NoteModal doesn't auto-save.
        closeForm();
    }
};

onMounted(() => document.addEventListener('click', handleClickOutside));
onUnmounted(() => document.removeEventListener('click', handleClickOutside));

const autoResize = (e: Event) => {
    const target = e.target as HTMLTextAreaElement;
    target.style.height = 'auto';
    target.style.height = target.scrollHeight + 'px';
};
</script>

<template>
  <div ref="containerRef" class="w-full max-w-[600px] mx-auto transition-all duration-200 ease-in-out">
    
    <div 
        class="border rounded-lg shadow-lg shadow-black/30 overflow-hidden relative group transition-colors duration-200"
        :class="[getColorClass(selectedColor), { 'ring-1 ring-zinc-500 shadow-2xl': isExpanded }]"
        :style="getStyle(selectedColor)"
    >   
        <!-- Image Preview -->
        <div v-if="imageUrl" class="relative">
            <img :src="imageUrl" class="w-full max-h-96 object-cover" />
            <button @click="imageUrl = ''" class="absolute bottom-2 right-2 p-1 bg-black/50 hover:bg-black/70 rounded text-white transform hover:scale-105 transition-all">
                <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
            </button>
        </div>

        <div 
            v-if="!isExpanded" 
            @click.stop="expandForm"
            class="px-4 py-3 text-zinc-400 font-medium text-sm cursor-text flex items-center justify-between"
        >
                <span>Take a note...</span>
                <div class="flex gap-4">
                     <button @click.stop="expandForm" class="hover:text-white"><svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg></button>
                </div>
            </div>

            <!-- Expanded View -->
            <div v-else class="flex flex-col">
                <input 
                    v-model="title" 
                    type="text" 
                    placeholder="Title" 
                    class="bg-transparent text-white px-4 pt-3 pb-2 text-base font-semibold placeholder-gray-300 focus:outline-none placeholder-opacity-70"
                />
                <textarea 
                    ref="textareaRef"
                    v-model="content" 
                    @input="autoResize"
                    placeholder="Type details..." 
                    rows="4"
                    class="bg-transparent text-gray-200 px-4 py-2 pb-4 resize-none focus:outline-none text-sm font-normal placeholder-gray-400 placeholder-opacity-70"
                ></textarea>
                
                <!-- Toolbar -->
                <div class="flex items-center justify-between px-3 py-2 border-t border-white/10" :class="selectedColor !== 'default' ? 'bg-black/10' : 'bg-[#2d2d2d]'">
                    <div class="flex items-center gap-1 relative">
                        <!-- Color Picker Toggle -->
                        <button 
                            @click.stop="showColorPicker = !showColorPicker"
                            class="p-2 rounded-full text-zinc-400 hover:text-white hover:bg-zinc-700/50 transition-colors"
                            title="Background options"
                        >
                             <svg xmlns="http://www.w3.org/2000/svg" class="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21a4 4 0 01-4-4V5a2 2 0 012-2h4a2 2 0 012 2v12a4 4 0 01-4 4zm0 0h12a2 2 0 002-2v-4a2 2 0 00-2-2h-2.343M11 7.343l1.657-1.657a2 2 0 012.828 0l2.829 2.829a2 2 0 010 2.828l-8.486 8.485M7 17h.01" /></svg>
                        </button>

                         <!-- Color Palette Popover -->
                        <div v-if="showColorPicker" class="absolute bottom-full left-0 mb-2 p-2 bg-[#2d2d2d] border border-zinc-700 rounded-lg shadow-xl flex gap-1 z-20 items-center">
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

                    <button 
                        @click="closeForm"
                        class="text-zinc-900 bg-white hover:bg-zinc-200 px-6 py-1.5 rounded-sm transition-colors text-sm font-medium"
                    >
                        Save
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
