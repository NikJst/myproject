import { createEmptyPostCard } from "./ModelCard.js";
import { togglePostForm } from "./CardForm.js";

// Глобальные переменные для пагинации
let currentPage = 1;
let pageSize = 10;
let totalPages = 1;

// Функция для получения всех постов с сервера с пагинацией
async function loadAllPosts(pageNumber = 1, pageSizeParam = 10) {
  console.log("loadAllPosts: Starting to load posts...");
  try {
    // Используем query параметры для пагинации
    const response = await fetch(`/api/Post?PageNumber=${pageNumber}&PageSize=${pageSizeParam}`);
    console.log("loadAllPosts: Response status:", response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error("loadAllPosts: Error response:", errorText);
      throw new Error(`Ошибка при получении постов: ${response.status}`);
    }

    const pagedResponse = await response.json();
    console.log("loadAllPosts: Received paged response:", pagedResponse);

    // Обновляем глобальные переменные пагинации
    currentPage = pagedResponse.meta.pageNumber;
    totalPages = Math.ceil(pagedResponse.meta.totalCount / pagedResponse.meta.pageSize);
    
    // Очищаем контейнер перед загрузкой новых постов
    const cardsContainer = document.querySelector(".cards-container");
    if (cardsContainer) {
      cardsContainer.innerHTML = '';
    }

    // Для каждого поста создаем карточку
    pagedResponse.items.forEach((post) => {
      console.log("loadAllPosts: Creating card for post:", post);
      createEmptyPostCard(
        post.title,
        post.text,
        post.postId,
        post.likedByUser,
        post.userId,
        post.likesCount,
        post.username
      );
    });
    
    // Обновляем кнопки пагинации
    updatePaginationControls();
    
    console.log("loadAllPosts: All cards created successfully");
  } catch (error) {
    console.error("Ошибка loadAllPosts:", error);
  }
}

// Функция создания поста на сервере
async function createPost() {
  try {
    const response = await fetch("/api/Post", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        title: "Новый пост",
        text: "Пример текста поста",
        userId: null, // Будет заполнено на сервере
        username: null, // Будет заполнено на сервере
        // likedByUser и likesCount не нужны при создании
      }),
    });

    if (!response.ok) throw new Error("Ошибка при создании карточки");

    const newPost = await response.json();
    console.log("Пост создан:", newPost);
    createEmptyPostCard(
      newPost.title,
      newPost.text,
      newPost.postId,//==
      newPost.likedByUser,
      newPost.userId,
      newPost.likesCount, // Используем значение из ответа сервера
      newPost.username // Передаем имя автора
    );
  } catch (error) {
    console.error(error);
  }
}

// Привязываем кнопку к функции при загрузке страницы
document.addEventListener("DOMContentLoaded", () => {
  const addPostBtn = document.querySelector(".add-post-btn"); // ищем кнопку по классу
  if (addPostBtn) {
    addPostBtn.addEventListener("click", togglePostForm); // используем новую функцию формы
  }
});
  // const newPost = await createPostOnServer(text);
  // if (newPost) {
  //   createEmptyPostCard(
  //     newPost.title,
  //     newPost.text,
  //     newPost.postId,//==
  //     newPost.likedByUser,
  //     newPost.userId,
  //   );
  // }


// Функция для обновления кнопок пагинации
function updatePaginationControls() {
  const paginationContainer = document.querySelector(".pagination-container");
  if (!paginationContainer) return;

  paginationContainer.innerHTML = '';

  // Создаем внутренний контейнер для кнопок пагинации
  const innerContainer = document.createElement("div");
  innerContainer.className = "pagination-controls-inner";

  // Кнопка "Предыдущая"
  const prevButton = document.createElement("button");
  prevButton.textContent = "<=";
  prevButton.className = "pagination-btn";
  prevButton.disabled = currentPage <= 1;
  prevButton.addEventListener("click", () => {
    if (currentPage > 1) {
      loadAllPosts(currentPage - 1, pageSize);
    }
  });

  // Информация о странице
  const pageInfo = document.createElement("span");
  pageInfo.className = "pagination-info";
  pageInfo.textContent = `${currentPage} из ${totalPages}`;

  // Кнопка "Следующая"
  const nextButton = document.createElement("button");
  nextButton.textContent = "=>";
  nextButton.className = "pagination-btn";
  nextButton.disabled = currentPage >= totalPages;
  nextButton.addEventListener("click", () => {
    if (currentPage < totalPages) {
      loadAllPosts(currentPage + 1, pageSize);
    }
  });

  // Добавляем кнопки во внутренний контейнер
  innerContainer.appendChild(prevButton);
  innerContainer.appendChild(pageInfo);
  innerContainer.appendChild(nextButton);

  // Добавляем внутренний контейнер в основной контейнер пагинации
  paginationContainer.appendChild(innerContainer);
}

// Функция для создания контейнера пагинации
function createPaginationContainer() {
  const cardsContainer = document.querySelector(".cards-container");
  if (!cardsContainer) return;

  // Проверяем, существует ли уже контейнер пагинации
  let paginationContainer = document.querySelector(".pagination-container");
  if (!paginationContainer) {
    paginationContainer = document.createElement("div");
    paginationContainer.className = "pagination-container";
    cardsContainer.parentNode.insertBefore(paginationContainer, cardsContainer.nextSibling);
  }
}

// Экспортируем функции для использования в других модулях
export { loadAllPosts, createPost, createEmptyPostCard, updatePaginationControls, createPaginationContainer };


