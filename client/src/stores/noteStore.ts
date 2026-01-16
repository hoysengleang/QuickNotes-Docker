import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface Note {
    id?: number;
    title: string;
    content: string;
    userId: number;
    createdAt?: string;
    updatedAt?: string;
    color?: string;
    imageUrl?: string;
    isPinned?: boolean;
    isDeleted?: boolean;
    deletedAt?: string;
}

export const useNoteStore = defineStore('noteStore', () => {
    const notes = ref<Note[]>([]);
    const trashNotes = ref<Note[]>([]);
    const loading = ref(false);
    const error = ref<string | null>(null);

    const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

    const getHeaders = () => ({
        'Content-Type': 'application/json'
    });

    // 1. Fetch Active Notes
    const fetchNotes = async (userId: number) => {
        loading.value = true;
        try {
            const response = await fetch(`${apiBaseUrl}/api/notes?userId=${userId}`);
            if (!response.ok) throw new Error('Failed to fetch notes');
            notes.value = await response.json();
        } catch (err: any) {
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    };

    // Fetch Trash Notes
    const fetchTrash = async (userId: number) => {
        loading.value = true;
        try {
            const response = await fetch(`${apiBaseUrl}/api/notes/trash?userId=${userId}`);
            if (!response.ok) throw new Error('Failed to fetch trash');
            trashNotes.value = await response.json();
        } catch (err: any) {
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    };

    // 2. Create Note
    const addNote = async (note: Note) => {
        loading.value = true;
        try {
            const response = await fetch(`${apiBaseUrl}/api/notes`, {
                method: 'POST',
                headers: getHeaders(),
                body: JSON.stringify(note)
            });
            if (!response.ok) throw new Error('Failed to create note');
            const newNote = await response.json();
            // Add to top of list
            notes.value.unshift(newNote);
        } catch (err: any) {
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    };

    // 3. Update Note
    const updateNote = async (note: Note) => {
        if (!note.id) return;
        loading.value = true;
        try {
            const response = await fetch(`${apiBaseUrl}/api/notes/${note.id}`, {
                method: 'PUT',
                headers: getHeaders(),
                body: JSON.stringify(note)
            });
            if (!response.ok) throw new Error('Failed to update note');

            // Update local state
            const index = notes.value.findIndex(n => n.id === note.id);
            if (index !== -1) {
                notes.value[index] = { ...note, updatedAt: new Date().toISOString() };
            }
        } catch (err: any) {
            error.value = err.message;
        } finally {
            loading.value = false;
        }
    };

    // 4. Delete Note (Soft Delete)
    const deleteNote = async (id: number, userId: number) => {
        try {
            const noteToDelete = notes.value.find(n => n.id === id);
            if (noteToDelete) {
                notes.value = notes.value.filter(n => n.id !== id);
            }

            const response = await fetch(`${apiBaseUrl}/api/notes/${id}?userId=${userId}`, {
                method: 'DELETE'
            });

            if (!response.ok) throw new Error('Failed to delete note');
        } catch (err: any) {
            error.value = err.message;
        }
    };

    // Restore Note
    const restoreNote = async (id: number, userId: number) => {
        try {
            const noteToRestore = trashNotes.value.find(n => n.id === id);
            if (noteToRestore) {
                trashNotes.value = trashNotes.value.filter(n => n.id !== id);
                notes.value.unshift({ ...noteToRestore, isDeleted: false });
            }

            await fetch(`${apiBaseUrl}/api/notes/${id}/restore?userId=${userId}`, { method: 'POST' });
        } catch (err: any) {
            error.value = err.message;
        }
    };

    // Permanent Delete
    const deleteForever = async (id: number, userId: number) => {
        try {
            trashNotes.value = trashNotes.value.filter(n => n.id !== id);
            await fetch(`${apiBaseUrl}/api/notes/${id}?userId=${userId}&permanent=true`, { method: 'DELETE' });
        } catch (err: any) {
            error.value = err.message;
        }
    };

    return {
        notes,
        trashNotes,
        loading,
        error,
        fetchNotes,
        fetchTrash,
        addNote,
        updateNote,
        deleteNote,
        restoreNote,
        deleteForever
    };
});
