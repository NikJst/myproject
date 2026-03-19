// Функция для создания кнопки "На главную"
export function createIndexButton() {
  const container = document.querySelector(".action-buttons-container");
  if (!container) {
    console.error("Контейнер .action-buttons-container не найден");
    return null;
  }

  // Создаем кнопку перехода на главную
  const indexButton = document.createElement("button");
  indexButton.className = "index-btn";
  indexButton.textContent = "На главную";

  // Обработчик клика для перехода на главную страницу
  indexButton.addEventListener("click", function () {
    window.location.href = "/index.html";
  });

  // Добавляем кнопку в контейнер
  container.appendChild(indexButton);

  return indexButton;
}

// Функция для создания кнопки профиля
export function createProfileButton() {
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

// Функция для создания всех кнопок навигации
export function createNavigationButtons() {
  createIndexButton();
  createProfileButton();
}
