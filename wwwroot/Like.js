document.addEventListener("click", async (e) => {
  const likeButton = e.target.closest("button");
  if (!likeButton) return;

  const likeIcon = likeButton.querySelector("img");
  if (
    !likeIcon ||
    (!likeIcon.src.includes("like.png") && !likeIcon.src.includes("like+.png"))
  )
    return;

  // Берем postId из data-атрибута кнопки
  const postId = likeButton.dataset.postId;
  if (!postId) {
    console.log("postId не найден в кнопке");
    return;
  }

  console.log("Нажат лайк для postId:", postId);

  try {
    const response = await fetch("/api/like", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ postId: postId }),
    });

    if (!response.ok) {
      console.error("Ошибка при лайке:", response.status, response.statusText);
      throw new Error("Ошибка при лайке");
    }

    const data = await response.json(); // { LikesCount: ... }

    likeButton.classList.toggle("liked");
    likeIcon.src = likeButton.classList.contains("liked")
      ? "image/like+.png"
      : "image/like.png";

    console.log("Текущее количество лайков:", data.LikesCount);
  } catch (err) {
    console.error("Ошибка при лайке:", err);
  }
});
