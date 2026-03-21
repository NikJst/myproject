// Импортируем необходимые функции
import { createEmptyPostCard } from "./Post.js";
import { loadProfileData, getCurrentUsername, loadUserPosts } from "./Profile.js";
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
  
  // Делаем функцию доступной глобально для других скриптов
  window.createEmptyPostCard = createEmptyPostCard;
}

// Автоматическая инициализация при загрузке DOM
document.addEventListener("DOMContentLoaded", initializeProfile);
