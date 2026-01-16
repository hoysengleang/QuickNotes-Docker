<script setup lang="ts">
import { computed } from 'vue';

const props = defineProps<{
    modelValue: boolean;
    type: 'terms' | 'privacy';
}>();

const emit = defineEmits(['update:modelValue']);

const close = () => {
    emit('update:modelValue', false);
};

const title = computed(() => props.type === 'terms' ? 'Terms of Service' : 'Privacy Policy');
</script>

<template>
    <transition name="modal">
        <div v-if="modelValue" class="fixed inset-0 z-[60] flex items-center justify-center p-4">
            <!-- Backdrop -->
            <div class="fixed inset-0 bg-black/60 backdrop-blur-sm transition-opacity" @click="close"></div>
            
            <!-- Modal Window -->
            <div class="relative bg-white w-full max-w-2xl rounded-2xl shadow-2xl overflow-hidden font-sans flex flex-col max-h-[85vh]">
                
                <!-- Header -->
                <div class="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
                    <h2 class="text-xl font-bold text-gray-900">{{ title }}</h2>
                    <button @click="close" class="p-2 rounded-full text-gray-400 hover:text-gray-900 hover:bg-gray-200/50 transition-colors">
                        <svg xmlns="http://www.w3.org/2000/svg" class="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
                    </button>
                </div>

                <!-- Content -->
                <div class="p-8 overflow-y-auto text-gray-600 leading-relaxed space-y-4 text-sm">
                    
                    <!-- Terms Content -->
                    <template v-if="type === 'terms'">
                        <p><strong>1. Introduction</strong><br>Welcome to QuickNotes. By creating an account, you agree to these terms.</p>
                        <p><strong>2. Usage Policy</strong><br>You are responsible for the content of your notes. Do not store illegal or harmful content.</p>
                        <p><strong>3. Privacy</strong><br>We respect your privacy. Your notes are stored securely and are not shared with third parties without consent, except as required by law.</p>
                        <p><strong>4. Account Security</strong><br>You are responsible for maintaining the confidentiality of your account credentials.</p>
                        <p><strong>5. Data Retention</strong><br>Deleted notes are moved to Trash and permanently removed after 7 days.</p>
                        <p><strong>6. Termination</strong><br>We reserve the right to terminate accounts that violate these terms.</p>
                        <p><strong>7. Changes to Terms</strong><br>We may update these terms at any time. Continued use implies acceptance.</p>
                    </template>

                    <!-- Privacy Content -->
                    <template v-else>
                        <p><strong>1. Information Collection</strong><br>We collect your username, password (encrypted), and the notes you create. We do not collect personal identifiers like phone numbers or physical addresses.</p>
                        <p><strong>2. Usage of Data</strong><br>Your data is used solely to provide the QuickNotes service, including syncing notes across devices and maintaining your account.</p>
                        <p><strong>3. Data Protection</strong><br>We implement security measures to maintain the safety of your personal information. Your passwords are hashed and salted.</p>
                        <p><strong>4. Third-Party Disclosure</strong><br>We do not sell, trade, or otherwise transfer your Personally Identifiable Information to outside parties.</p>
                        <p><strong>5. Cookies & Local Storage</strong><br>We use local storage to maintain your login session. No tracking cookies are used.</p>
                        <p><strong>6. Your Rights</strong><br>You have the right to access, correct, or delete your data at any time through your profile settings.</p>
                    </template>

                    <div class="h-8"></div> <!-- Spacer -->
                </div>

                <!-- Footer -->
                <div class="px-6 py-4 border-t border-gray-100 bg-gray-50 flex justify-end">
                    <button @click="close" class="px-6 py-2.5 bg-black text-white rounded-lg font-medium hover:bg-gray-800 transition-colors">
                        {{ type === 'terms' ? 'I Understand' : 'Close' }}
                    </button>
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
  transform: scale(0.98);
}
</style>
