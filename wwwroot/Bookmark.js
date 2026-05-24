document.addEventListener("click", async (e) => {
  const bookmarkButton = e.target.closest("button");
  if (!bookmarkButton) return; //выход если клик не на кнопку

  const bookmarkIcon = bookmarkButton.querySelector("img");
  if (
    !bookmarkIcon ||
    (!bookmarkIcon.src.includes("bookmark.png") && !bookmarkIcon.src.includes("bookmark+.png"))
  )
    return; //выход если иконка закладки не найдена

  // Берем postId из data-атрибута кнопки
  const postId = bookmarkButton.dataset.postId;
  if (!postId) {
    console.error("postId не найден в кнопке");
    return; //выход если postId не найден
  }
  console.log("Нажата закладка для postId:", postId);

  try {
    const response = await fetch("/api/BookMark", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ postId: postId }), // в теле запроса отправляем postId как объект { postId: "..." }
    });

    if (!response.ok) {
      console.error(
        "Ошибка при установке Bookmark:",
        response.status,
        response.statusText,
      );
      throw new Error("Ошибка при установке Bookmark");
    }

    const data = await response.json();
    
    // Проверяем структуру ответа
    if (!data || typeof data.bookmarkedByUser === 'undefined') {
      console.error("Некорректный ответ сервера:", data);
      throw new Error("Некорректный ответ сервера");
    }

    // Обновляем состояние на основе ответа от сервера
    if (data.bookmarkedByUser) {
      bookmarkButton.classList.add("bookmarked");
      bookmarkIcon.src = "image/bookmark+.png";
    } else {
      bookmarkButton.classList.remove("bookmarked");
      bookmarkIcon.src = "image/bookmark.png";
    }
  } catch (err) {
    console.error("Ошибка при установке Bookmark:", err);
  } finally {
    console.log("Установка Bookmark завершена");
  }
});

// Настройка наблюдателей для динамически создаваемых кнопок закладок
export function setupBookmarkObservers() {
  // Наблюдаем за изменениями в контейнере постов
  const postsContainer = document.querySelector(".posts-container");
  if (postsContainer) {
    const observer = new MutationObserver(() => {
      console.log("Posts container changed, checking for new bookmark buttons");
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
      console.log("Cards container changed, checking for new bookmark buttons");
    });
    
    observer.observe(cardsContainer, {
      childList: true,
      subtree: true
    });
  }
}

// Функция для отправки запроса на сервер (для совместимости)
export async function toggleBookmarkOnServer(postId, buttonElement) {
  try {
    const response = await fetch("/api/BookMark", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ postId: postId })
    });
    
    if (response.ok) {
      const result = await response.json();
      // Обновляем интерфейс на основе ответа от сервера
      updateBookmarkButton(buttonElement, result.bookmarkedByUser);
      return result;
    } else {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
  } catch (error) {
    console.error("Error toggling bookmark:", error);
    throw error;
  }
}

// Функция для обновления кнопки закладки на основе ответа сервера
export function updateBookmarkButton(buttonElement, isBookmarked) {
  // Для Font Awesome иконок (posts-container)
  const icon = buttonElement.querySelector("i");
  if (icon) {
    if (isBookmarked) {
      icon.classList.remove("far");
      icon.classList.add("fas", "text-yellow-600");
    } else {
      icon.classList.remove("fas", "text-yellow-600");
      icon.classList.add("far");
    }
  }
  
  // Для img иконок (cards-container)
  const img = buttonElement.querySelector('img');
  if (img) {
    img.src = isBookmarked ? 'image/bookmark+.png' : 'image/bookmark.png';
  }
}
