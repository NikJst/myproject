import { createEmptyPostCard } from "./ModelCard.js";

// Функция для получения всех постов с сервера
async function loadAllPosts() {
  try {
    const response = await fetch("http://192.168.1.35:3000/api/Post");
    if (!response.ok) throw new Error("Ошибка при получении постов");

    const posts = await response.json();
    console.log("Полученные посты с сервера:", posts);

    // Для каждого поста создаем карточку
    posts.forEach((post) => {
      console.log("Обработка поста:", post);
      createEmptyPostCard(
        "Новый пост",
        post.text,
        post.guidId,
        post.likedByUser,
        post.userId,
      );
    });
  } catch (error) {
    console.error("Ошибка loadAllPosts:", error);
  }
}

// Функция создания поста на сервере
async function createPost() {
  try {
    const response = await fetch("http://192.168.1.35:3000/api/Post", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        text: "Пример текста поста",
        // userId не указываем, сервер сам подставит текущего гостя из куков
      }),
    });

    if (!response.ok) throw new Error("Ошибка при создании поста");

    const newPost = await response.json();
    console.log("Пост создан:", newPost);
    // Создаём карточку на странице с правильным guidId и userId
    createEmptyPostCard(
      "Новый пост",
      newPost.text,
      newPost.guidId,
      newPost.likedByUser,
      newPost.userId,
    );
  } catch (error) {
    console.error(error);
  }
}

// Привязываем кнопку к функции при загрузке страницы
document.addEventListener("DOMContentLoaded", () => {
  const addPostBtn = document.querySelector(".add-post-btn"); // ищем кнопку по классу
  if (addPostBtn) {
    addPostBtn.addEventListener("click", createPost); // просто передаем функцию
  }
});

// Вызовем при загрузке страницы
loadAllPosts();
async function handleCreatePost(text) {
  const newPost = await createPostOnServer(text);
  if (newPost) {
    createEmptyPostCard(
      "Новый пост",
      newPost.text,
      newPost.guidId,
      newPost.likedByUser,
      newPost.userId,
    );
  }
}
