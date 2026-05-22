// Search.js - Модуль для полнотекстового поиска
import { createEmptyPostCard } from './ModelCard.js';

class SearchModule {
    constructor() {
        this.searchInput = null;
        this.isSearchVisible = false;
        this.init();
    }

    init() {
        this.createSearchElements();
        this.bindEvents();
    }

    createSearchElements() {
        // Находим существующее поле ввода для поиска
        this.searchInput = document.getElementById('searchInput');
        
        // Если поле поиска не существует, создаем простое поле ввода
        if (!this.searchInput) {
            this.searchInput = document.createElement('input');
            this.searchInput.type = 'text';
            this.searchInput.id = 'searchInput';
            this.searchInput.placeholder = 'Поиск по постам...';
            this.searchInput.style.cssText = `
                padding: 10px;
                border: 1px solid #ccc;
                border-radius: 5px;
                margin: 10px;
                width: 300px;
            `;
            
            // Добавляем в начало body или в существующий контейнер
            const existingContainer = document.querySelector('.search-container') || document.body;
            existingContainer.insertBefore(this.searchInput, existingContainer.firstChild);
        }
    }

    bindEvents() {
        // Поиск при вводе текста (с debounce)
        let searchTimeout;
        this.searchInput.addEventListener('input', (e) => {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                this.performSearch(e.target.value);
            }, 300);
        });
    }


    async performSearch(query) {
        if (!query.trim()) {
            // Очищаем контейнер с карточками при пустом запросе
            this.clearSearchResults();
            return;
        }

        try {
            const response = await fetch(`/api/GIN?query=${encodeURIComponent(query)}`, {
                method: 'GET',
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
            console.error('Ошибка поиска:', error);
            this.clearSearchResults();
        }
    }

    displaySearchResults(posts) {
        // Сначала очищаем предыдущие результаты поиска
        this.clearSearchResults();
        
        if (!posts || posts.length === 0) {
            console.log('Ничего не найдено');
            return;
        }

        // Создаем карточки для каждого найденного поста
        posts.forEach(post => {
            createEmptyPostCard(
                '', // заголовок (пустой, так как в постах нет заголовка)
                post.text,
                post.postId,
                post.likedByUser,
                post.userId,
                post.likesCount,
                post.username,
                post.isOnline,
                post.createdAt,
                '.cards-container' // указываем контейнер для карточек
            );
        });
    }

    clearSearchResults() {
        // Находим и удаляем все карточки, которые были добавлены в результате поиска
        const cardsContainer = document.querySelector('.cards-container');
        if (cardsContainer) {
            // Можно добавить специальный класс для карточек поиска, чтобы удалять только их
            // А пока просто очищаем весь контейнер
            cardsContainer.innerHTML = '';
        }
    }

}

// Создаем экземпляр модуля поиска
const searchModule = new SearchModule();

// Экспортируем для использования в других модулях
export { searchModule };
