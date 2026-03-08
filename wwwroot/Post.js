function createEmptyPostCard(
  title = "Новый пост",
  text = "Введите текст...",
  postId = null,
  LikedByUser = false,
) {
  const container = document.querySelector(".cards-container"); //====> находим контейнер для карточек
  if (!container) return;

  const card = document.createElement("div");
  card.classList.add("card");

  if (postId) card.dataset.postId = postId; //====> сохраняем id поста если он передан

  // Кнопка закрытия
  const closeBtn = document.createElement("button");
  closeBtn.classList.add("close-btn");

  const closeImg = document.createElement("img");
  closeImg.src = "image/close.png";
  closeImg.alt = "";
  closeBtn.appendChild(closeImg);

  // Блок текста
  const textBlock = document.createElement("div");
  textBlock.classList.add("text-block");

  // Заголовок
  const h2 = document.createElement("h2");
  h2.textContent = title;

  // Текст
  const p = document.createElement("p");
  p.textContent = text;

  textBlock.appendChild(h2);
  textBlock.appendChild(p);

  // кнопки
  const buttonsBottom = document.createElement("div");
  buttonsBottom.classList.add("buttons-bottom");
  const icons = ["repost.png", "star.png", "message.png", "like.png"];

  icons.forEach((icon) => {
    const btn = document.createElement("button");
    const img = document.createElement("img");

    // Если это лайк и пользователь лайкнул пост (LikedByUser == true), используем картинку like+
    if (LikedByUser === true) {
      img.src = "image/like+.png";
    } else {
      img.src = `image/${icon}`;
    }
    img.alt = "";

    btn.appendChild(img);

    //===> лайк-кнопка получает postId через data-атрибут
    if (icon === "like.png" && postId) {
      btn.dataset.postId = postId;
      console.log(
        "Лайк-кнопка получила postId:",
        postId,
        "Liked:",
        LikedByUser,
      );

      // Добавляем элемент для отображения количества лайков
      // const likesCount = document.createElement("span");
      // likesCount.classList.add("likes-count");
      // likesCount.style.marginLeft = "5px";
      // likesCount.style.fontSize = "14px";
      // likesCount.textContent = "0"; // начальное значение
      // btn.appendChild(likesCount);
    }
    buttonsBottom.appendChild(btn);
  });

  // Сборка карточки
  card.appendChild(closeBtn);
  card.appendChild(textBlock);
  // card.appendChild(buttonsBottom);

  // Добавляем лайк-кнопку вместе с карточкой, отдельно
  const likeButton = document.createElement("button");
  const likeImg = document.createElement("img");
  likeImg.src = LikedByUser ? "image/like+.png" : "image/like.png";
  likeImg.alt = "";
  likeButton.appendChild(likeImg);
  likeButton.classList.add("like-button");
  likeButton.dataset.postId = postId; // <-- важно

  // навешиваем обработчик лайка
  likeButton.addEventListener("click", () =>
    handleLikeClick(postId, likeButton),
  );

  // добавляем кнопку в карточку
  card.appendChild(likeButton);

  // добавляем карточку в контейнер
  postsContainer.appendChild(card);
}

// ======> Функция для получения всех постов с сервера
async function loadAllPosts() {
  try {
    const response = await fetch("http://192.168.1.35:3000/api/Post");
    if (!response.ok) throw new Error("Ошибка при получении постов");

    const posts = await response.json();

    // Для каждого поста создаем карточку
    posts.forEach((post) => {
      console.log("Post data:", post); // для отладки
      createEmptyPostCard(
        "Новый пост",
        post.text,
        post.guidId,
        post.LikedByUser,
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
        // userId не указываем, сервер сам подставит текущего гостя из куки
      }),
    });

    if (!response.ok) throw new Error("Ошибка при создании поста");

    const newPost = await response.json();
    console.log("Пост создан:", newPost);

    // Создаём карточку на странице с правильным postId и userId
    createEmptyPostCard("Новый пост", newPost.text, newPost.guidId);
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

// Вызовем сразу при загрузке страницы
loadAllPosts();
async function handleCreatePost(text) {
  const newPost = await createPostOnServer(text);
  if (newPost) {
    createEmptyPostCard(
      "Новый пост",
      newPost.text,
      newPost.guidId,
      newPost.LikedByUser,
    );
  }
}
