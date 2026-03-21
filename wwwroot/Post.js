import { createEmptyPostCard } from "./ModelCard.js";
import { togglePostForm } from "./CardForm.js";

// Функция для получения всех постов с сервера
async function loadAllPosts() {
  console.log("loadAllPosts: Starting to load posts...");
  try {
    // Используем относительный URL вместо абсолютного
    const response = await fetch("/api/Post");
    console.log("loadAllPosts: Response status:", response.status);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error("loadAllPosts: Error response:", errorText);
      throw new Error(`Ошибка при получении постов: ${response.status}`);
    }

    const posts = await response.json();
    console.log("loadAllPosts: Received posts:", posts);

    // Для каждого поста создаем карточку
    posts.forEach((post) => {
      console.log("loadAllPosts: Creating card for post:", post);
      createEmptyPostCard(
        post.title,
        post.text,
        post.postId,//==
        post.likedByUser,
        post.userId,
        post.likesCount, // Добавляем количество лайков
        post.username // Передаем имя автора
      );
    });
    
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
        // userId не указываем, сервер сам подставит текущего гостя из куков
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
      0, // Новый пост всегда имеет 0 лайков
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


// Экспортируем функции для использования в других модулях
export { loadAllPosts, createPost, createEmptyPostCard };


