// Импортируем необходимые функции
import { createEmptyPostCard } from "./Post.js";
import { loadProfileData, getCurrentUsername, loadUserPosts, loadUserLikes, loadUserFavorites, loadUserDrafts } from "./Profile.js";
import { setupScrollEffect } from "./ScrollUtils.js";
import { setupLikeObservers, toggleLikeOnServer, updateLikeButton } from "./Like.js";

// Основная функция инициализации страницы профиля
export function initializeProfile() {
  console.log("Profile page loaded");
  
  // Загружаем данные профиля
  const username = getCurrentUsername();
  loadProfileData(username).then((profileData) => {
    console.log("Profile data loaded");
    // После загрузки данных профиля загружаем посты пользователя
    return loadUserPosts(profileData.username);
  }).then(() => {
    console.log("User posts loaded");
  }).catch(error => {
    console.error("Failed to load profile data or posts:", error);
  });
  
  // Настраиваем эффект прокрутки
  setupScrollEffect();
  
  // Настраиваем обработчики лайков
  setupLikeObservers();
  
  // Настраиваем переключение вкладок
  setupTabSwitching();
  
  // Делаем функцию доступной глобально для других скриптов
  window.createEmptyPostCard = createEmptyPostCard;
}

// Функция для настройки переключения вкладок
function setupTabSwitching() {
  const tabButtons = document.querySelectorAll('.tab-button');
  const tabContents = document.querySelectorAll('.tab-content');
  
  console.log('setupTabSwitching: Found tab buttons:', tabButtons.length);
  
  tabButtons.forEach((button, index) => {
    console.log(`Setting up tab button ${index}:`, button.textContent, button.getAttribute('data-tab'));
    
    button.addEventListener('click', async (event) => {
      event.preventDefault();
      const targetTab = button.getAttribute('data-tab');
      console.log(`Tab button clicked: ${targetTab}`);
      
      // Получаем username для загрузки данных
      const username = getCurrentUsername();
      console.log('Using username for tab:', username);
      
      // Удаляем активные классы у всех кнопок
      tabButtons.forEach(btn => {
        btn.classList.remove('text-blue-600', 'border-b-2', 'border-blue-600');
        btn.classList.add('text-gray-600');
      });
      
      // Добавляем активные классы к нажатой кнопке
      button.classList.remove('text-gray-600');
      button.classList.add('text-blue-600', 'border-b-2', 'border-blue-600');
      
      // Скрываем все контейнеры
      tabContents.forEach(content => {
        content.classList.add('hidden');
      });
      
      // Показываем выбранный контейнер
      const targetContent = document.getElementById(targetTab + '-content');
      if (targetContent) {
        targetContent.classList.remove('hidden');
        console.log(`Showing content for: ${targetTab}`);
      } else {
        console.error(`Content container not found: ${targetTab}-content`);
        return;
      }
      
      // Загружаем данные для соответствующей вкладки
      if (username) {
        await loadTabData(targetTab, username);
      } else {
        console.error('No username available for loading tab data');
      }
    });
  });
}

// Функция для загрузки данных вкладки
async function loadTabData(targetTab, username) {
  try {
    console.log(`Loading data for tab: ${targetTab}`);
    switch (targetTab) {
      case 'likes':
        await loadUserLikes(username);
        break;
      case 'favorites':
        await loadUserFavorites(username);
        break;
      case 'drafts':
        await loadUserDrafts(username);
        break;
      case 'posts':
      default:
        // Посты уже загружены при инициализации, но можно перезагрузить если нужно
        console.log('Posts tab selected - posts should already be loaded');
        break;
    }
    console.log(`Successfully loaded data for tab: ${targetTab}`);
  } catch (error) {
    console.error(`Failed to load ${targetTab}:`, error);
  }
}

// Автоматическая инициализация при загрузке DOM
document.addEventListener("DOMContentLoaded", initializeProfile);
