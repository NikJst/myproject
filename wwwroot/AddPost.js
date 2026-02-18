function createEmptyPostCard(title = "Новый пост", text = "Введите текст...") {
    const container = document.querySelector(".cards-container");
    if (!container) return;

    const card = document.createElement("div");
    card.classList.add("card");

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

    const h2 = document.createElement("h2");
    h2.textContent = title;

    const p = document.createElement("p");
    p.textContent = text;

    textBlock.appendChild(h2);
    textBlock.appendChild(p);

    // Нижние кнопки
    const buttonsBottom = document.createElement("div");
    buttonsBottom.classList.add("buttons-bottom");

    const icons = ["repost.png", "star.png", "message.png", "like.png"];

    icons.forEach(icon => {
        const btn = document.createElement("button");
        const img = document.createElement("img");
        img.src = `image/${icon}`;
        img.alt = "";
        btn.appendChild(img);
        buttonsBottom.appendChild(btn);
    });

    // Сборка карточки
    card.appendChild(closeBtn);
    card.appendChild(textBlock);
    card.appendChild(buttonsBottom);

    // ВСТАВКА В НАЧАЛО
    container.prepend(card);
}