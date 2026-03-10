// Скрипт для создания кнопки перехода на страницу профиля

function createProfileButton() {
  const container = document.querySelector(".action-buttons-container");
  if (!container) {
    console.error("Контейнер .action-buttons-container не найден");
    return null;
  }

  // Создаем кнопку профиля
  const profileButton = document.createElement("button");
  profileButton.className = "profile-btn";
  profileButton.textContent = "Профиль";

  // Обработчик клика для перехода на страницу профиля
  profileButton.addEventListener("click", function () {
    window.location.href = "/Profile.html";
  });

  // Добавляем кнопку в контейнер
  container.appendChild(profileButton);

  return profileButton;
}

// Автоматическое создание кнопки при загрузке страницы
document.addEventListener("DOMContentLoaded", function () {
  createProfileButton();
});
