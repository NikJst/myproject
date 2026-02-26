document.addEventListener('click', async (e) => {
    // Находим кнопку, которая содержит изображение лайка
    const likeButton = e.target.closest('button');
    if (!likeButton) return;

    const likeIcon = likeButton.querySelector('img');
    if (!likeIcon || (!likeIcon.src.includes('like.png') && !likeIcon.src.includes('like+.png'))) return;

    try {
        const response = await fetch('/api/like', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
      body: JSON.stringify({ postId: postId }),
        });

        if (!response.ok) {
            console.error('Ошибка при лайке:', response.status, response.statusText);
            throw new Error('Ошибка при лайке');
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