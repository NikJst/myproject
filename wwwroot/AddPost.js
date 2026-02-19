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

// Функция для создания поста на сервере
async function createPostOnServer(text, userId = "3fa85f64-5717-4562-b3fc-2c963f66afa6") {
    try {
        const response = await fetch("http://localhost:3000/api/Post", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ text, userId })
        });

        if (!response.ok) throw new Error("Ошибка при создании поста");

        const newPost = await response.json();
        return newPost;
    } catch (error) {
        console.error("Ошибка createPostOnServer:", error);
        return null;
    }
}

// Функция для создания поста и добавления карточки на страницу
async function handleCreatePost(text = "Введите текст...") {
    const newPost = await createPostOnServer(text); // userId подставляется по умолчанию
    if (newPost) {
        // Можно использовать текст из нового поста
        createEmptyPostCard("Новый пост", newPost.text);
    }
}

// Пример: создание поста при загрузке или по кнопке
document.querySelector("#createPostBtn")?.addEventListener("click", () => {
    const inputText = document.querySelector("#postInput")?.value || "Введите текст...";
    handleCreatePost(inputText);
});