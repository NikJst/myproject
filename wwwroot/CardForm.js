// Функция для создания формы добавления поста
export function createPostForm() {
  // Ищем контейнер для карточек или постов
  let container = document.querySelector(".posts-container");
  if (!container) {
    container = document.querySelector(".cards-container");
  }
  if (!container) return;

  // Создаем модальное окно для формы
  const modal = document.createElement("div");
  modal.classList.add("post-form-modal");
  modal.style.display = "none";

  // Создаем саму форму
  const form = document.createElement("form");
  form.classList.add("post-form");

  // Заголовок формы
  const title = document.createElement("h3");
  title.textContent = "Создать новый пост";

  // Поле для заголовка
  const titleGroup = document.createElement("div");
  titleGroup.classList.add("form-group");

  const titleLabel = document.createElement("label");
  titleLabel.textContent = "Заголовок:";
  titleLabel.setAttribute("for", "post-title"); //

  const titleInput = document.createElement("input");
  titleInput.type = "text";
  titleInput.id = "post-title";
  titleInput.name = "title"; //
  titleInput.placeholder = "<Введите заголовок, если хотите>";
  titleInput.value = null; // значение по умолчанию

  // Надписи исчезают при клике на поле
  titleInput.addEventListener("focus", function () {
    this.placeholder = "";
  });

  titleInput.addEventListener("blur", function () {
    this.placeholder = "<Введите заголовок, если хотите>";
  });

  titleGroup.appendChild(titleLabel);
  titleGroup.appendChild(titleInput);

  // Поле для текста
  const textGroup = document.createElement("div");
  textGroup.classList.add("form-group");

  const textLabel = document.createElement("label");
  textLabel.textContent = "Текст поста:";
  textLabel.setAttribute("for", "post-text");

  const textArea = document.createElement("textarea");
  textArea.id = "post-text";
  textArea.name = "text";
  textArea.placeholder = "Введите текст поста...";
  textArea.rows = 4;
  textArea.required = true;

  // Надписи исчезают при клике на поле
  textArea.addEventListener("focus", function () {
    this.placeholder = "Введите текст поста...";
  });

  textArea.addEventListener("blur", function () {
    this.placeholder = "Введите текст поста...";
  });

  textGroup.appendChild(textLabel);
  textGroup.appendChild(textArea);

  // Кнопки формы
  const buttonGroup = document.createElement("div");
  buttonGroup.classList.add("form-buttons");

  const submitBtn = document.createElement("button");
  submitBtn.type = "submit";
  submitBtn.textContent = "Создать пост";
  submitBtn.classList.add("submit-btn");

  const cancelBtn = document.createElement("button");
  cancelBtn.type = "button";
  cancelBtn.textContent = "Отмена";
  cancelBtn.classList.add("cancel-btn");

  buttonGroup.appendChild(submitBtn);
  buttonGroup.appendChild(cancelBtn);

  // Сборка формы
  form.appendChild(title);
  form.appendChild(titleGroup);
  form.appendChild(textGroup);
  form.appendChild(buttonGroup);

  // Добавляем форму в модальное окно
  modal.appendChild(form);

  // Добавляем модальное окно в body
  document.body.appendChild(modal);

  // Обработчик отправки формы
  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    const formData = new FormData(form);
    const postData = {
      title: formData.get("title") || null,
      text: formData.get("text"),
    };

    // Вызываем функцию создания поста с данными из формы
    await createPostWithFormData(postData);

    // Закрываем форму и очищаем
    modal.style.display = "none";
    form.reset();
  });

  // Обработчик кнопки отмены
  cancelBtn.addEventListener("click", () => {
    modal.style.display = "none";
    form.reset();
  });

  // Закрытие по клику вне формы
  modal.addEventListener("click", (e) => {
    if (e.target === modal) {
      modal.style.display = "none";
      form.reset();
    }
  });

  return modal;
}

// Функция для создания поста с данными из формы
async function createPostWithFormData(postData) {
  try {
    const response = await fetch("http://192.168.1.35:3000/api/Post", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        title: postData.title,
        text: postData.text,
        // userId не указываем, сервер сам подставит текущего гостя из куков
      }),
    });

    if (!response.ok) throw new Error("Ошибка при создании поста");

    const newPost = await response.json(); //newpost это ортвет
    console.log("Пост создан:", newPost);

    // Импортируем createEmptyPostCard и создаем карточку
    const { createEmptyPostCard } = await import("./ModelCard.js");
    createEmptyPostCard(
      postData.title,
      postData.text,
      newPost.guidId,
      newPost.likedByUser,
      newPost.userId,
    );
  } catch (error) {
    console.error("Ошибка при создании карточки:", error);
  }
}

// Функция для показа/скрытия формы
export function togglePostForm() {
  let modal = document.querySelector(".post-form-modal");

  // Если форма еще не создана, создаем ее
  if (!modal) {
    modal = createPostForm();
  }

  // Показываем или скрываем форму
  modal.style.display = modal.style.display === "none" ? "flex" : "none";

  // Устанавливаем фокус на поле текста
  if (modal.style.display === "flex") {
    document.getElementById("post-text").focus();
  }
}

// Функция для создания кнопки добавления поста
export function createAddPostButton() {
  const container = document.querySelector(".action-buttons-container");
  if (!container) {
    console.error("Контейнер .action-buttons-container не найден");
    return null;
  }

  // Создаем кнопку добавления поста
  const addPostButton = document.createElement("button");
  addPostButton.className = "add-post-btn";
  addPostButton.textContent = "+ Пост";

  // Обработчик клика для открытия формы создания поста
  addPostButton.addEventListener("click", function () {
    togglePostForm();
  });

  // Добавляем кнопку в контейнер
  container.appendChild(addPostButton);

  return addPostButton;
}

// Функция для создания кнопки "На главную"
export function createMainPageButton() {
  const container = document.querySelector(".action-buttons-container");
  if (!container) {
    console.error("Контейнер .action-buttons-container не найден");
    return null;
  }

  // Создаем кнопку "На главную"
  const mainPageButton = document.createElement("button");
  mainPageButton.className = "main-page-btn";
  mainPageButton.textContent = "На главную";

  // Обработчик клика для перехода на главную страницу
  mainPageButton.addEventListener("click", function () {
    window.location.href = "/index.html";
  });

  // Добавляем кнопку в контейнер
  container.appendChild(mainPageButton);

  return mainPageButton;
}
