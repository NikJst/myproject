document.addEventListener('click', async (e) => {
    const likeButton = e.target.closest('.like-btn');
    if (!likeButton) return;

    const likeIcon = likeButton.querySelector('.like-icon');

    try {
        const response = await fetch('/api/like', { method: 'POST' });

        if (!response.ok) {
            console.error('Ошибка при лайке:', response.status, response.statusText);
            return;
        }

        const data = await response.json(); // { LikesCount: ... }

        // Меняем класс и иконку кнопки
        likeButton.classList.toggle('liked');
        if (likeIcon) {
            likeIcon.src = likeButton.classList.contains('liked') ? 'image/like+.png' : 'image/like.png';
        }

        console.log('Текущее количество лайков:', data.LikesCount);
    } catch (err) {
        console.error('Ошибка при лайке:', err);
    }
});