export function createEmptyPostCard(
  title = null,
  text = null,
  guidId = null,
  likedByUser = false,
  userId = null,
) {
  console.log(
    "createEmptyPostCard вызван с guidId:",
    guidId,
    "likedByUser:",
    likedByUser,
    "userId:",
    userId,
  );

  const container = document.querySelector(".cards-container"); //====> находим контейнер для карточек
  if (!container) return;

  const card = document.createElement("div");
  card.classList.add("card");

  if (guidId) card.dataset.postId = guidId; //====> сохраняем id поста если он предан

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

    //===> лайк-кнопка получает guidId через data-атрибут
    if (icon === "like.png" && guidId) {
      btn.dataset.postId = guidId;
      btn.id = `like-btn-${guidId}`; // уникальный id для кнопки LIKE а не только люббой кнопки в картчоке - (like-btn-{guidId})

      console.log(
        "Лайк-кнопка получила guidId:",
        guidId,
        "Liked:",
        likedByUser,
      );
    }
    buttonsBottom.appendChild(btn);
  });

  // Сборка карточки
  card.appendChild(closeBtn);
  card.appendChild(textBlock);
  card.appendChild(buttonsBottom);

  container.prepend(card);
}
