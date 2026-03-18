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

    const data = await response.json(); // { LikesCount: ... }
    
    // Проверяем структуру ответа
    if (!data || typeof data.likesCount === 'undefined') {
      console.error("Некорректный ответ сервера:", data);
      throw new Error("Некорректный ответ сервера");
    }

    likeButton.classList.toggle("liked");
    likeIcon.src = likeButton.classList.contains("liked")
      ? "image/like+.png"
      : "image/like.png";
    
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
