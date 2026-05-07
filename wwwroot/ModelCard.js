export function createEmptyPostCard(
  title,
  text,
  postId,
  likedByUser,
  userId,
  likesCount = 0, // Добавляем параметр для счетчика лайков
  username = "Автор", // Добавляем параметр для имени автора
  isOnline = false, // Добавляем параметр для онлайн-статуса
  createdAt = null, // Добавляем параметр для даты создания
  containerSelector = null // Добавляем параметр для выбора контейнера
) {
  console.log(
    "createEmptyPostCard вызван с postId:",
    postId,
    "likedByUser:",
    likedByUser,
    "userId:",
    userId,
    "isOnline:",
    isOnline,
    "createdAt:",
    createdAt,
    "containerSelector:",
    containerSelector
  );

  // Определяем, в какой контейнер добавлять карточку
  let targetContainer;
  if (containerSelector) {
    targetContainer = document.querySelector(containerSelector);
  } else {
    // Если контейнер не указан, используем старую логику
    const cardsContainer = document.querySelector(".cards-container");
    const postsContainer = document.querySelector(".posts-container");
    targetContainer = postsContainer || cardsContainer;
  }
  
  console.log("createEmptyPostCard: targetContainer found:", !!targetContainer);
  
  if (!targetContainer) {
    console.error("createEmptyPostCard: No target container found!");
    return;
  }

  const card = document.createElement("div");
  card.classList.add("card");
  
  // Add hover effect class
  card.classList.add("card-hover");

  if (postId) card.dataset.postId = postId; //====> сохраняем id поста если он предан
  if (userId) card.dataset.userId = userId; //====> сохраняем id пользователя если он передан

  // // Кнопка закрытия
  // const closeBtn = document.createElement("button");
  // closeBtn.classList.add("close-btn");

  // const closeImg = document.createElement("img");
  // closeImg.src = "image/close.png";
  // closeImg.alt = "";
  // closeBtn.appendChild(closeImg);

  // Блок текста
  const textBlock = document.createElement("div");
  textBlock.classList.add("text-block");
  // textBlock.style.color = "#666";


  // Заголовок
  const h2 = document.createElement("h2");
  h2.textContent = title;
  h2.style.color = "#2e2e2e";


  // Текст
  const p = document.createElement("p");
  p.textContent = text;
  p.style.color = "#4a4a4a";

  textBlock.appendChild(h2);
  textBlock.appendChild(p);

  // кнопки
  const buttonsBottom = document.createElement("div");
  buttonsBottom.classList.add("buttons-bottom");
  const icons = ["repost.png", "star.png", "message.png", "like.png"];

  // ====> перебираем и создаем кнопки
  icons.forEach((icon) => {
    const btn = document.createElement("button");
    const img = document.createElement("img");
    if (icon === "like.png" && likedByUser) {
      img.src = "image/like+.png";
    } else {
      img.src = `image/${icon}`;
    }
    img.alt = "";

    btn.appendChild(img); //====> добавляем картинку в кнопку

    //===> лайк-кнопка получает postId через data-атрибут
    if (icon === "like.png" && postId) {
      btn.dataset.postId = postId;
      btn.id = `like-btn-${postId}`; // уникальный id для кнопки LIKE а не только люббой кнопки в картчоке - (like-btn-{postId})

      // Создаем счетчик лайков
      const likeCount = document.createElement("span");
      likeCount.classList.add("like-count");
      likeCount.textContent = likesCount;
      likeCount.id = `like-count-${postId}`;
      
      // Добавляем счетчик после кнопки
      btn.appendChild(likeCount);

      console.log(
        "Лайк-кнопка получила postId:",
        postId,
        "Liked:",
        likedByUser,
        "Likes count:",
        likesCount,
      );
    }
    buttonsBottom.appendChild(btn);
  });

  // Сборка карточки
  // card.appendChild(closeBtn);
  card.appendChild(textBlock);
  card.appendChild(buttonsBottom);
  
  // Добавляем имя автора, онлайн-статус и дату создания внизу карточки
  if (username && username !== "Автор") {
    const authorDiv = document.createElement("div");
    authorDiv.classList.add("author-info");
    
    // Создаем индикатор онлайн-статуса
    const onlineIndicator = document.createElement("span");
    onlineIndicator.classList.add("online-indicator");
    onlineIndicator.id = `status-dot-${userId}`; // Добавляем ID для обновления через JavaScript
    if (isOnline) {
      onlineIndicator.classList.add("online");
      onlineIndicator.title = "В сети";
    } else {
      onlineIndicator.classList.add("offline");
      onlineIndicator.title = "Не в сети";
    }
    
    authorDiv.appendChild(onlineIndicator);
    
    // Добавляем ссылку на профиль автора
    const authorLink = document.createElement("a");
    authorLink.href = `/Profile.html?user=${username}`;
    authorLink.textContent = username;
    
    authorDiv.appendChild(authorLink);
    
    // Добавляем дату создания, если она есть
    if (createdAt) {
      const dateSpan = document.createElement("span");
      dateSpan.classList.add("post-date");
      
      // Форматируем дату
      const date = new Date(createdAt);
      const now = new Date();
      const diffTime = Math.abs(now - date);
      const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
      
      if (diffDays === 0) {
        // Сегодня
        dateSpan.textContent = `сегодня в ${date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })}`;
      } else if (diffDays === 1) {
        // Вчера
        dateSpan.textContent = `вчера в ${date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })}`;
      } else if (diffDays < 7) {
        // На этой неделе
        dateSpan.textContent = `${diffDays} дней назад`;
      } else {
        // Старые посты
        dateSpan.textContent = date.toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit', year: '2-digit' });
      }
      
      authorDiv.appendChild(dateSpan);
    }
    
    card.appendChild(authorDiv);
  }
  // Добавляем карточку в целевой контейнер
  targetContainer.prepend(card); // Используем prepend для обратного порядка
  // targetContainer.appendChild(card); // Используем appendChild для добавления в конец
  console.log(`Карточка добавлена в контейнер: ${containerSelector || 'default'}`);
}
