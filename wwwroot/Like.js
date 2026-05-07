document.addEventListener("click", async (e) => {
  const likeButton = e.target.closest("button");
  if (!likeButton) return; //выход если клик не на кнопку

  const likeIcon = likeButton.querySelector("img");
  if (
    !likeIcon ||
    (!likeIcon.src.includes("like.png") && !likeIcon.src.includes("like+.png"))
  )
    return; //выход если иконка лайка не найдена

  // Берем postId из data-атрибута кнопки
  const postId = likeButton.dataset.postId;
  if (!postId) {
    console.error("postId не найден в кнопке");
    return; //выход если postId не найден
  }
  console.log("Нажат лайк для postId:", postId);

  try {
    const response = await fetch("/api/like", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ postId: postId }), // в теле запроса отправляем postId как объект { postId: "..." }
    });

    if (!response.ok) {
      console.error(
        "Ошибка при постановке Like:",
        response.status,
        response.statusText,
      );
      throw new Error("Ошибка при постановке Like");
    }

    const data = await response.json(); // { likesCount: ..., likedByUser: ... }
    
    // Проверяем структуру ответа
    if (!data || typeof data.likesCount === 'undefined' || typeof data.likedByUser === 'undefined') {
      console.error("Некорректный ответ сервера:", data);
      throw new Error("Некорректный ответ сервера");
    }

    // Обновляем состояние на основе ответа от сервера
    if (data.likedByUser) {
      likeButton.classList.add("liked");
      likeIcon.src = "image/like+.png";
    } else {
      likeButton.classList.remove("liked");
      likeIcon.src = "image/like.png";
    }
    
    // Обновляем счетчик лайков
    const likeCount = document.getElementById(`like-count-${postId}`);
    if (likeCount) {
      likeCount.textContent = data.likesCount;
    }
  } catch (err) {
    console.error("Ошибка при постановке Like:", err);
  } finally {
    console.log("Постановка Like завершена");
  }
});

// Настройка наблюдателей для динамически создаваемых кнопок лайков
export function setupLikeObservers() {
  // Наблюдаем за изменениями в контейнере постов
  const postsContainer = document.querySelector(".posts-container");
  if (postsContainer) {
    const observer = new MutationObserver(() => {
      console.log("Posts container changed, checking for new like buttons");
    });
    
    observer.observe(postsContainer, {
      childList: true,
      subtree: true
    });
  }
  
  // Наблюдаем за изменениями в cards-container
  const cardsContainer = document.querySelector(".cards-container");
  if (cardsContainer) {
    const observer = new MutationObserver(() => {
      console.log("Cards container changed, checking for new like buttons");
    });
    
    observer.observe(cardsContainer, {
      childList: true,
      subtree: true
    });
  }
}

// Функция для отправки запроса на сервер (для совместимости)
export async function toggleLikeOnServer(postId, buttonElement) {
  try {
    const response = await fetch("/api/Like", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ postId: postId })
    });
    
    if (response.ok) {
      const result = await response.json();
      // Обновляем интерфейс на основе ответа от сервера
      updateLikeButton(buttonElement, result.likedByUser, result.likesCount);
      return result;
    } else {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error toggling like:", error);
    throw error;
  }
}

// Функция для обновления кнопки лайка на основе ответа сервера
export function updateLikeButton(buttonElement, isLiked, likesCount) {
  // Для Font Awesome иконок (posts-container)
  const icon = buttonElement.querySelector("i");
  if (icon) {
    if (isLiked) {
      icon.classList.remove("far");
      icon.classList.add("fas", "text-red-600");
    } else {
      icon.classList.remove("fas", "text-red-600");
      icon.classList.add("far");
    }
    buttonElement.innerHTML = `<i class="${icon.className} mr-1"></i>${likesCount}`;
  }
  
  // Для img иконок (cards-container)
  const img = buttonElement.querySelector('img');
  if (img) {
    img.src = isLiked ? 'image/like+.png' : 'image/like.png';
    // Обновляем счетчик лайков (теперь он находится вне кнопки)
    const postId = buttonElement.dataset.postId;
    if (postId) {
      const likeCount = document.getElementById(`like-count-${postId}`);
      if (likeCount) {
        likeCount.textContent = likesCount;
      }
    }
  }
}
