class OnlineStatusManager {
    constructor() {
        this.intervalId = null;
        this.isRunning = false;
    }

    // Start sending ping requests every 2 seconds
    start() {
        if (this.isRunning) {
            console.log('Online status tracking is already running');
            return;
        }

        this.isRunning = true;
        console.log('Starting online status tracking...');

        // Send immediate ping
        this.sendPing();

        // Set up interval for subsequent pings
        this.intervalId = setInterval(() => {
            this.sendPing();
        }, 2000);
    }

    // Stop the ping requests
    stop() {
        if (!this.isRunning) {
            console.log('Online status tracking is not running');
            return;
        }

        this.isRunning = false;
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
        console.log('Online status tracking stopped');
    }

    // Send ping request to the server
    async sendPing() {
        try {
            const response = await fetch('/api/online/ping', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include' // Include cookies for authentication
            });

            if (response.ok) {
                const result = await response.text();
                console.log('Ping successful:', result);
            } else {
                console.error('Ping failed with status:', response.status);
            }
        } catch (error) {
            console.error('Error sending ping:', error);
        }
    }

    // Get current status
    getStatus() {
        return {
            isRunning: this.isRunning,
            intervalId: this.intervalId
        };
    }
}

// Create global instance
const onlineStatus = new OnlineStatusManager();

// Auto-start when page loads if needed
// Uncomment the following line to automatically start tracking
// onlineStatus.start();

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = onlineStatus;
} else {
    window.onlineStatus = onlineStatus;
}
