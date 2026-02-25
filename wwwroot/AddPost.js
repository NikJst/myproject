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

    // Заголовок
    const h2 = document.createElement("h2");
    h2.textContent = title;

    // Текст
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

    container.prepend(card);
}
// Функция для получения всех постов с сервера
async function loadAllPosts() {
    try {
        const response = await fetch("http://192.168.1.35:3000/api/Post");
        if (!response.ok) throw new Error("Ошибка при получении постов");

        const posts = await response.json();

        // Для каждого поста создаем карточку
        posts.forEach(post => {
            createEmptyPostCard("Новый пост", post.text);
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
                userId: "3fa85f64-5717-4562-b3fc-2c963f66afa6"
            })
        });

        if (!response.ok) throw new Error("Ошибка при создании поста");

        const newPost = await response.json();
        console.log("Пост создан:", newPost);

        // Создаём карточку на странице
        createEmptyPostCard("Новый пост", newPost.text);
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
        createEmptyPostCard("Новый пост", newPost.text);
    }
}

