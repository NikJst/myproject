// Search.js - Модуль для полнотекстового поиска

class SearchModule {
    constructor() {
        this.searchContainer = null;
        this.searchInput = null;
        this.searchResults = null;
        this.isSearchVisible = false;
        this.init();
    }

    init() {
        this.createSearchElements();
        this.bindEvents();
    }

    createSearchElements() {
        // Создаем контейнер для поиска
        this.searchContainer = document.createElement('div');
        this.searchContainer.className = 'search-container';
        this.searchContainer.innerHTML = `
            <div class="search-header">
                <button class="search-toggle-btn" id="searchToggleBtn">
                    <img src="image/message.png" alt="Поиск" />
                </button>
                <div class="search-box" id="searchBox">
                    <input 
                        type="text" 
                        id="searchInput" 
                        placeholder="Поиск по постам..." 
                        autocomplete="off"
                    />
                    <button class="search-close-btn" id="searchCloseBtn">
                        <img src="image/close.png" alt="Закрыть" />
                    </button>
                </div>
            </div>
            <div class="search-results" id="searchResults"></div>
        `;

        // Добавляем контейнер в начало body
        document.body.insertBefore(this.searchContainer, document.body.firstChild);
        
        // Получаем ссылки на элементы
        this.searchInput = document.getElementById('searchInput');
        this.searchResults = document.getElementById('searchResults');
        this.searchToggleBtn = document.getElementById('searchToggleBtn');
        this.searchCloseBtn = document.getElementById('searchCloseBtn');
        this.searchBox = document.getElementById('searchBox');
    }

    bindEvents() {
        // Кнопка открытия поиска
        this.searchToggleBtn.addEventListener('click', () => this.toggleSearch());
        
        // Кнопка закрытия поиска
        this.searchCloseBtn.addEventListener('click', () => this.hideSearch());
        
        // Поиск при вводе текста (с debounce)
        let searchTimeout;
        this.searchInput.addEventListener('input', (e) => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.performSearch(e.target.value);
            }, 300);
        });

        // Закрытие поиска по Escape
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && this.isSearchVisible) {
                this.hideSearch();
            }
        });

        // Закрытие поиска при клике вне контейнера
        document.addEventListener('click', (e) => {
            if (this.isSearchVisible && !this.searchContainer.contains(e.target)) {
                this.hideSearch();
            }
        });
    }

    toggleSearch() {
        if (this.isSearchVisible) {
            this.hideSearch();
        } else {
            this.showSearch();
        }
    }

    showSearch() {
        this.isSearchVisible = true;
        this.searchBox.style.display = 'flex';
        this.searchInput.focus();
        this.searchContainer.classList.add('search-active');
    }

    hideSearch() {
        this.isSearchVisible = false;
        this.searchBox.style.display = 'none';
        this.searchInput.value = '';
        this.searchResults.innerHTML = '';
        this.searchContainer.classList.remove('search-active');
    }

    async performSearch(query) {
        if (!query.trim()) {
            this.searchResults.innerHTML = '';
            return;
        }

        this.searchResults.innerHTML = '<div class="search-loading">Поиск...</div>';

        try {
            const response = await fetch(`/api/GIN?query=${encodeURIComponent(query)}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            this.displaySearchResults(data);
        } catch (error) {
            console.error('Erreur de recherche:', error);
            this.searchResults.innerHTML = '<div class="search-error">Erreur lors de la recherche. Veuillez réessayer.</div>';
        }
    }

    displaySearchResults(posts) {
        if (!posts || posts.length === 0) {
            this.searchResults.innerHTML = '<div class="search-no-results">Ничего не найдено</div>';
            return;
        }

        const resultsHTML = posts.map(post => this.createPostCard(post)).join('');
        this.searchResults.innerHTML = `
            <div class="search-results-header">Найдено постов: ${posts.length}</div>
            <div class="search-results-list">${resultsHTML}</div>
        `;
    }

    createPostCard(post) {
        const date = new Date(post.createdAt).toLocaleDateString('ru-RU');
        const likedClass = post.likedByUser ? 'liked' : '';
        
        return `
            <div class="search-result-card">
                <div class="search-result-content">
                    <div class="search-result-header">
                        <span class="search-result-username">${post.username || 'Аноним'}</span>
                        <span class="search-result-date">${date}</span>
                        ${post.isOnline ? '<span class="online-indicator">●</span>' : ''}
                    </div>
                    <div class="search-result-text">${this.escapeHtml(post.text)}</div>
                </div>
                <div class="search-result-actions">
                    <button class="like-button ${likedClass}" data-post-id="${post.postId}">
                        <img src="image/like.png" alt="Лайк" />
                        <span class="likes-count">${post.likesCount}</span>
                    </button>
                </div>
            </div>
        `;
    }

    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
}

// Создаем экземпляр модуля поиска
const searchModule = new SearchModule();

// Экспортируем для использования в других модулях
export { searchModule };
