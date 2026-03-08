function createEmptyPostCard(
  title = "Новый пост",
  text = "Введите текст...",
  postId = null,
  likedByUser = false,
) {
  console.log(
    "createEmptyPostCard вызван с postId:",
    postId,
    "likedByUser:",
    likedByUser,
  );

  const container = document.querySelector(".cards-container"); //====> находим контейнер для карточек
  if (!container) return;

  const card = document.createElement("div");
  card.classList.add("card");

  if (postId) card.dataset.postId = postId; //====> сохраняем id поста если он предан

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

    // Если это лайк и пользователь лайкнул пост, используем картинку like+
    if (icon === "like.png" && likedByUser) {
      img.src = "image/like+.png";
    } else {
      img.src = `image/${icon}`;
    }
    img.alt = "";

    btn.appendChild(img);

    //===> лайк-кнопка получает postId через data-атрибут
    if (icon === "like.png" && postId) {
      btn.dataset.postId = postId;
      btn.id = `like-btn-${postId}`; // уникальный id для кнопки LIKE а не только люббой кнопки в картчоке - (like-btn-{postId})

      console.log(
        "Лайк-кнопка получила postId:",
        postId,
        "Liked:",
        likedByUser,
      );

      // Добавляем элемент для отображения количества лайков
      const likesCount = document.createElement("span");
      likesCount.classList.add("likes-count");
      likesCount.style.marginLeft = "5px";
      likesCount.style.fontSize = "14px";
      likesCount.textContent = "0"; // начальное значение
      btn.appendChild(likesCount);
    }
    buttonsBottom.appendChild(btn);
  });

  // Сборка карточки
  card.appendChild(closeBtn);
  card.appendChild(textBlock);
  card.appendChild(buttonsBottom);

  container.prepend(card);
}
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
        post.userId, // вся проблема была в имени поля. теперь используем userId вместо guidId
        post.likedByUser,
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
    // Создаём карточку на странице с правильным postId и userId
    createEmptyPostCard("Новый пост", newPost.text, newPost.userId);
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
      newPost.userId,
      newPost.likedByUser,
    );
  }
}
