export function setupGlobalErrorHandling() {
    window.onerror = async (message, source, lineno, colno, error) => {
        try {
            await logErrorToBackend({
                message: message.toString(),
                source: 'Frontend',
                stackTrace: error?.stack || `Line: ${lineno}, Col: ${colno} in ${source}`
            });
        } catch (e) {
            console.error('Failed to report error:', e);
        }
    };

    window.addEventListener('unhandledrejection', async (event: PromiseRejectionEvent) => {
        try {
            await logErrorToBackend({
                message: event.reason && event.reason.message ? event.reason.message : 'Unhandled Promise Rejection',
                source: 'Frontend',
                stackTrace: event.reason ? event.reason.stack : undefined
            });
        } catch (e) {
            console.error('Failed to report rejection:', e);
        }
    });

}

async function logErrorToBackend(errorLog: { message: string; source: string; stackTrace?: string }) {
    const userData = localStorage.getItem('user');
    const user = userData ? JSON.parse(userData) : null;

    const payload = {
        ...errorLog,
        userId: user ? (user.Id || user.id) : null
    };

    try {
        await fetch(`${import.meta.env.VITE_API_BASE_URL}/api/logs`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });
    } catch (apiError) {
        console.error('API Error Log Failed', apiError);
    }
}
