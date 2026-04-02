async function pingServer() {
    try {
        console.log('Sending ping to server...');
        const response = await fetch('/api/online', {
            method: 'POST'
        });
        
        if (response.ok) {
            console.log('Ping sent successfully, response:', await response.text());
        } else {
            console.error('Ping failed with status:', response.status);
        }
    } catch (error) {
        console.error('Error pinging server:', error);
    }
}

async function updateStatuses() {
    // 1. Обновляем онлайн статус для постов
    const userIds = Array.from(document.querySelectorAll('.card'))
                         .filter(el => el.dataset.userId)
                         .map(el => el.dataset.userId);

    console.log('updateStatuses: Found user IDs:', userIds);

    if (userIds.length > 0) {
        // 2. Спрашиваем у быстрого эндпоинта (который смотрит только в кэш)
        const response = await fetch('/api/online/users?userIds=' + userIds.join(','), {
            method: 'GET'
        });
        const statuses = await response.json(); // Придет { "userId": true, "userId2": false }
        console.log('updateStatuses: Received statuses:', statuses);

        // 3. Обновляем классы в HTML
        for (const [id, isActive] of Object.entries(statuses)) {
            const indicator = document.querySelector(`#status-dot-${id}`);
            console.log(`Processing indicator for ID ${id}:`, indicator);
            if (indicator) {
                if (isActive) {
                    indicator.classList.add('online');
                    indicator.classList.remove('offline');
                    console.log(`Set user ${id} to ONLINE`);
                } else {
                    indicator.classList.remove('online');
                    indicator.classList.add('offline');
                    console.log(`Set user ${id} to OFFLINE`);
                }
            } else {
                console.log(`No indicator found for ID ${id}`);
            }
        }
    }
    
    // 4. Обновляем онлайн статус для профиля
    updateProfileOnlineStatus();
}

// Функция для обновления онлайн статуса на странице профиля
async function updateProfileOnlineStatus() {
    const profileIndicator = document.getElementById('online-indicator');
    if (!profileIndicator) {
        console.log('No profile indicator found, not on profile page');
        return; // Не на странице профиля
    }
    
    // Получаем ID пользователя профиля из data-атрибута или URL
    const profileUserId = document.body.dataset.profileUserId || getCurrentProfileUserId();
    if (!profileUserId) {
        console.error('No profile user ID found');
        return;
    }
    
    console.log('Updating online status for profile user:', profileUserId);
    
    try {
        const response = await fetch(`/api/online/users?userIds=${profileUserId}`, {
            method: 'GET'
        });
        
        if (response.ok) {
            const statuses = await response.json();
            console.log('Received online statuses:', statuses);
            
            const isOnline = statuses[profileUserId] || false;
            console.log('Profile user isOnline:', isOnline);
            
            if (isOnline) {
                profileIndicator.classList.remove('offline');
                profileIndicator.classList.add('online');
                profileIndicator.title = 'В сети';
            } else {
                profileIndicator.classList.remove('online');
                profileIndicator.classList.add('offline');
                profileIndicator.title = 'Не в сети';
            }
        } else {
            console.error('Failed to get online status:', response.status);
        }
    } catch (error) {
        console.error('Error updating profile online status:', error);
    }
}

// Функция для получения ID пользователя профиля из URL
function getCurrentProfileUserId() {
    // Пробуем получить из параметра user в URL
    const urlParams = new URLSearchParams(window.location.search);
    const userParam = urlParams.get('user');
    
    if (userParam) {
        return userParam;
    }
    
    // Пробуем получить из пути
    const pathParts = window.location.pathname.split('/');
    const profileIndex = pathParts.indexOf('profile') + 1;
    
    if (profileIndex > 0 && pathParts[profileIndex]) {
        return pathParts[profileIndex];
    }
    
    return null;
}

async function updateStatusesWithPing() {
    await pingServer();
    await updateStatuses();
}

// Глобальный объект для управления онлайн статусом
const onlineStatus = {
    intervalId: null,
    
    // Запуск отслеживания
    start() {
        // Отправляем пинг при старте
        pingServer();
        
        // Запускаем обновление каждые 5 секунд
        if (this.intervalId) {
            clearInterval(this.intervalId);
        }
        this.intervalId = setInterval(updateStatusesWithPing, 5000);
        console.log('Online status tracking started');
    },
    
    // Остановка отслеживания
    stop() {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
            console.log('Online status tracking stopped');
        }
    }
};

// Экспортируем для использования в других модулях
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { onlineStatus, pingServer, updateStatuses };
}