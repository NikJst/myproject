import { createEmptyPostCard } from './ModelCard.js';

// Загрузка данных профиля
export async function loadProfileData(username) {
  try {
    // Если username не передан, получаем текущего пользователя
    if (!username) {
      const currentUser = await getCurrentUser();
      username = currentUser.username || currentUser.name;
    }
    
    const response = await fetch(`/api/Profile/${username}`);
    if (!response.ok) {
      throw new Error('Ошибка загрузки профиля');
    }
    
    const profileData = await response.json();
    console.log('Данные профиля:', profileData);
    
    // Обновляем HTML на основе полученных данных
    updateProfileUI(profileData);
    
    return profileData;
  } catch (error) {
    console.error('Ошибка при загрузке профиля:', error);
    throw error;
  }
}

// Обновление интерфейса профиля
function updateProfileUI(profileData) {
  console.log('Updating profile UI with data:', profileData);
  
  // Сохраняем ID пользователя в data-атрибуте для онлайн статуса
  if (profileData.userId) {
    document.body.dataset.profileUserId = profileData.userId;
  }
  
  // Обновляем имя пользователя
  const usernameElement = document.querySelector('h1');
  if (usernameElement) {
    usernameElement.textContent = profileData.username || 'Пользователь';
    console.log('Updated username:', usernameElement.textContent);
  }
  
  // Обновляем онлайн-статус
  const onlineIndicator = document.getElementById('online-indicator');
  if (onlineIndicator) {
    if (profileData.isOnline) {
      onlineIndicator.classList.remove('offline');
      onlineIndicator.classList.add('online');
      onlineIndicator.title = 'В сети';
    } else {
      onlineIndicator.classList.remove('online');
      onlineIndicator.classList.add('offline');
      onlineIndicator.title = 'Не в сети';
    }
    console.log('Updated online status:', profileData.isOnline);
  }
  
  // Обновляем username (с @)
  const handleElement = document.querySelector('.text-gray-600.mb-2');
  if (handleElement) {
    handleElement.textContent = profileData.username ? `@${profileData.username}` : '@user';
    console.log('Updated handle:', handleElement.textContent);
  }
  
  // Обновляем описание/био
  const bioElement = document.querySelector('.text-gray-700.text-sm.max-w-md');
  if (bioElement) {
    bioElement.textContent = profileData.description || 'Описание отсутствует';
    console.log('Updated bio:', bioElement.textContent);
  }
  
  // Обновляем заголовок профиля
  const headerElement = document.querySelector('title');
  if (headerElement) {
    headerElement.textContent = `Профиль ${profileData.username || 'пользователя'}`;
  }
  
  // Показываем/скрываем кнопку редактирования в зависимости от IsMine
  const editButton = document.querySelector('button:has(.fa-edit)');
  if (editButton) {
    editButton.style.display = profileData.isMine ? 'block' : 'none';
    console.log('Edit button visibility:', profileData.isMine);
  }
  
  // Обновляем статистику
  updateProfileStats(profileData);
}



// Обновление статистики профиля
function updateProfileStats(profileData) {
  console.log('Updating profile stats with postCount:', profileData.postCount);
  console.log('Updating profile stats with likesCount:', profileData.likesCount);
  
  // Находим элементы по тексту в дочерних элементах для большей надежности
  const statsContainers = document.querySelectorAll('.text-center');
  console.log('Found stats containers:', statsContainers.length);
  
  statsContainers.forEach((container, index) => {
    const label = container.querySelector('.text-gray-600.text-sm');
    const value = container.querySelector('.text-2xl');
    
    console.log(`Container ${index}: label="${label?.textContent}", value="${value?.textContent}"`);
    
    if (!label || !value) return;
    
    switch (label.textContent) {
      case 'Постов':
        value.textContent = profileData.postCount || '0';
        console.log('Updated posts count to:', value.textContent);
        break;
      case 'Понравившиеся':
        value.textContent = profileData.likesCount || '0';
        console.log('Updated likes count to:', value.textContent);
        break;
      case 'Избранное':
        value.textContent = profileData.bookmarksCount || '0';
        console.log('Updated bookmarks count to:', value.textContent);
        break;
    }
  });
}

// Получение username из URL параметра user
export function getCurrentUsername() {
  console.log('Текущий URL:', window.location.href);
  console.log('Pathname:', window.location.pathname);
  console.log('Search:', window.location.search);
  
  // Получаем параметры из URL
  const urlParams = new URLSearchParams(window.location.search);
  const userParam = urlParams.get('user');
  
  console.log('Параметр user из URL:', userParam);
  
  if (userParam) {
    console.log('Найден параметр user в URL:', userParam);
    return userParam;
  }
  
  // Если параметра user нет, пробуем старый способ
  const pathParts = window.location.pathname.split('/');
  const usernameIndex = pathParts.indexOf('profile') + 1;
  
  if (usernameIndex > 0 && pathParts[usernameIndex]) {
    console.log('Найден username в пути:', pathParts[usernameIndex]);
    return pathParts[usernameIndex];
  }
  
  console.log('Username не найден в URL параметрах или пути');
  return null;
}

// Получение данных текущего пользователя
async function getCurrentUser() {
  try {
    const response = await fetch('/User');
    if (response.ok) {
      const user = await response.json();
      return user;
    }
  } catch (error) {
    console.error('Ошибка получения текущего пользователя:', error);
  }
}

// Обновление информации профиля
export async function updateProfileInfo(profileData, username) {
  try {
    const response = await fetch(`/api/Profile/${username}/edit`, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(profileData)
    });

    if (!response.ok) {
      throw new Error('Ошибка обновления профиля');
    }

    const updatedProfile = await response.json();
    console.log('Профиль обновлен:', updatedProfile);

    // Обновляем интерфейс с новыми данными
    updateProfileUI(updatedProfile);

    return updatedProfile;
  } catch (error) {
    console.error('Ошибка при обновлении профиля:', error);
    throw error;
  }
}


// Создание элемента поста
function createPostElement(post, containerSelector = '.posts-container') {
  // Используем существующую функцию создания карточки
  createEmptyPostCard(
    post.title || '',
    post.text || '',
    post.postId,
    post.myLike || false,
    post.userId,
    post.likesCount || 0,
    post.username || 'Автор',
    post.isOnline || false,
    post.createdAt,
    containerSelector,
    post.myBookmark || false
  );
  
  // Возвращаем null, так как createEmptyPostCard сама добавляет карточку в DOM
  return null;
}

// Загрузка данных вкладки через единый эндпоинт с параметром i
export async function loadProfileTabData(username, tabType) {
  try {
    console.log('loadProfileTabData: Загрузка данных для вкладки:', tabType, 'username:', username);
    
    // Всегда получаем username профиля из URL, игнорируя переданный параметр
    const profileUsername = getCurrentUsername();
    if (!profileUsername) {
      throw new Error('Username профиля не найден в URL');
    }
    console.log('Используем username профиля из URL:', profileUsername);

    // Маппинг типа вкладки на параметр i
    const tabToParam = {
      'posts': '1',
      'favorites': '2',
      'likes': '3'
    };

    const iParam = tabToParam[tabType];
    if (!iParam) {
      throw new Error(`Неизвестный тип вкладки: ${tabType}`);
    }

    // Делаем запрос к единому эндпоинту с параметром i, используя username профиля из URL
    const response = await fetch(`/api/Profile/geatusposts?username=${profileUsername}&i=${iParam}`, {
      method: 'POST'
    });
    console.log('Загрузка данных вкладки:', response.status);
    
    if (!response.ok) {
      console.error('Ошибка загрузки данных вкладки:', response.status, response.statusText);
      throw new Error(`Ошибка загрузки данных вкладки: ${response.status}`);
    }
    
    const posts = await response.json();
    console.log('Данные вкладки:', posts);
    
    // Определяем контейнер для обновления
    const containerSelector = {
      'posts': '.posts-container',
      'favorites': '.favorites-container',
      'likes': '.likes-container'
    }[tabType];

    // Очищаем контейнер
    const container = document.querySelector(containerSelector);
    if (container) {
      container.innerHTML = '';
      
      if (!posts || posts.length === 0) {
        const emptyMessages = {
          'posts': 'У пользователя пока нет постов',
          'favorites': 'Избранных постов пока нет',
          'likes': 'Понравившихся постов пока нет'
        };
        container.innerHTML = `<p class="text-gray-500 text-center">${emptyMessages[tabType]}</p>`;
        return;
      }
      
      // Создаем карточки для каждого поста
      posts.forEach(post => {
        createPostElement(post, containerSelector);
      });
    }
    
    return posts;
  } catch (error) {
    console.error('Ошибка при загрузке данных вкладки:', error);
    // Показываем сообщение об ошибке в интерфейсе
    const containerSelector = {
      'posts': '.posts-container',
      'favorites': '.favorites-container',
      'likes': '.likes-container'
    }[tabType];
    
    const container = document.querySelector(containerSelector);
    if (container) {
      container.innerHTML = `<p class="text-red-500 text-center">Ошибка загрузки: ${error.message}</p>`;
    }
    throw error;
  }
}
// =======================><==============================

// Функция для переключения лайка (заглушка, нужно реализовать)
// async function toggleLike(postId) {
//   try {
//     const response = await fetch('/api/Posts/like', {
//       method: 'POST',
//       headers: {
//         'Content-Type': 'application/json',
//       },
//       body: JSON.stringify({ postId: postId })
//     });
    
//     if (!response.ok) {
//       throw new Error('Ошибка переключения лайка');
//     }
    
//     // Перезагружаем посты для обновления состояния лайков
//     const currentUsername = getCurrentUsername();
//     await loadUserPosts(currentUsername);
    
//   } catch (error) {
//     console.error('Ошибка при переключении лайка:', error);
//   }
// }





/*
// Загрузка постов пользователя
export async function loadUserPosts(username) {
  try {
    console.log('loadUserPosts: Использую username из профиля:', username);
    
    if (!username) {
      throw new Error('Username не указан');
    }
    
    const response = await fetch(`/api/Profile/${username}/posts`);
    console.log('получаем посты пользователя:', response.status);
    
    if (!response.ok) {
      console.error('Ошибка загрузки постов пользователя:', response.status, response.statusText);
      throw new Error(`Ошибка загрузки постов пользователя: ${response.status}`);
    }
    
    const userPosts = await response.json();
    console.log('Посты пользователя:', userPosts);
    
    // Обновляем интерфейс с постами
    updatePostsUI(userPosts);
    
    return userPosts;
  } catch (error) {
    console.error('Ошибка при загрузке постов:', error);
    // Показываем сообщение об ошибке в интерфейсе
    const postsContainer = document.querySelector('.posts-container');
    if (postsContainer) {
      postsContainer.innerHTML = `<p class="text-red-500 text-center">Ошибка загрузки постов: ${error.message}</p>`;
    }
    throw error;
  }
}

// Обновление интерфейса с постами
function updatePostsUI(postsResponse) {
  console.log('Updating posts UI with:', postsResponse);
  
  // Находим контейнер для постов
  const postsContainer = document.querySelector('.posts-container') || 
                        document.querySelector('[data-posts-container]');
  
  if (!postsContainer) {
    console.warn('Контейнер для постов не найден');
    return;
  }
  
  // Очищаем контейнер
  postsContainer.innerHTML = '';
  
  // Получаем массив постов из PagedResponse
  const posts = postsResponse.items || postsResponse;
  
  if (!posts || posts.length === 0) {
    postsContainer.innerHTML = '<p class="text-gray-500 text-center">У пользователя пока нет постов</p>';
    return;
  }
  
  // Создаем карточки для каждого поста
  // createPostElement() сама добавляет карточки в DOM через createEmptyPostCard()
  posts.forEach(post => {
    createPostElement(post);
  });
}
*/

/*
// Загрузка понравившихся постов
export async function loadUserLikes(username) {
  try {
    console.log('loadUserLikes: Использую username из профиля:', username);
    
    if (!username) {
      throw new Error('Username не указан');
    }
    
    const response = await fetch(`/api/Profile/${username}/likes`);
    console.log('получаем понравившиеся посты:', response.status);
    
    if (!response.ok) {
      console.error('Ошибка загрузки понравившихся постов:', response.status, response.statusText);
      throw new Error(`Ошибка загрузки понравившихся постов: ${response.status}`);
    }
    
    const likedPosts = await response.json();
    console.log('Понравившиеся посты:', likedPosts);
    
    // Обновляем интерфейс с понравившимися постами
    updateLikesUI(likedPosts);
    
    return likedPosts;
  } catch (error) {
    console.error('Ошибка при загрузке понравившихся постов:', error);
    // Показываем сообщение об ошибке в интерфейсе
    const likesContainer = document.querySelector('.likes-container');
    if (likesContainer) {
      likesContainer.innerHTML = `<p class="text-red-500 text-center">Ошибка загрузки понравившихся постов: ${error.message}</p>`;
    }
    throw error;
  }
}
*/

/*
// Загрузка избранных постов
export async function loadUserFavorites(username) {
  try {
    console.log('loadUserFavorites: Использую username из профиля:', username);
    
    if (!username) {
      throw new Error('Username не указан');
    }
    
    const response = await fetch(`/api/Profile/${username}/bookmarks`);
    console.log('получаем избранные посты:', response.status);
    
    if (!response.ok) {
      console.error('Ошибка загрузки избранных постов:', response.status, response.statusText);
      throw new Error(`Ошибка загрузки избранных постов: ${response.status}`);
    }
    
    const favoritePosts = await response.json();
    console.log('Избранные посты:', favoritePosts);
    
    // Обновляем интерфейс с избранными постами
    updateFavoritesUI(favoritePosts);
    
    return favoritePosts;
  } catch (error) {
    console.error('Ошибка при загрузке избранных постов:', error);
    // Показываем сообщение об ошибке в интерфейсе
    const favoritesContainer = document.querySelector('.favorites-container');
    if (favoritesContainer) {
      favoritesContainer.innerHTML = `<p class="text-red-500 text-center">Ошибка загрузки избранных постов: ${error.message}</p>`;
    }
    throw error;
  }
}
*/

/*
// Загрузка черновиков
export async function loadUserDrafts(username) {
  try {
    console.log('loadUserDrafts: Использую username из профиля:', username);
    
    if (!username) {
      throw new Error('Username не указан');
    }
    
    const response = await fetch(`/api/Profile/${username}/drafts`);
    console.log('получаем черновики:', response.status);
    
    if (!response.ok) {
      console.error('Ошибка загрузки черновиков:', response.status, response.statusText);
      throw new Error(`Ошибка загрузки черновиков: ${response.status}`);
    }
    
    const draftPosts = await response.json();
    console.log('Черновики:', draftPosts);
    
    // Обновляем интерфейс с черновиками
    updateDraftsUI(draftPosts);
    
    return draftPosts;
  } catch (error) {
    console.error('Ошибка при загрузке черновиков:', error);
    // Показываем сообщение об ошибке в интерфейсе
    const draftsContainer = document.querySelector('.drafts-container');
    if (draftsContainer) {
      draftsContainer.innerHTML = `<p class="text-red-500 text-center">Ошибка загрузки черновиков: ${error.message}</p>`;
    }
    throw error;
  }
}
*/

/*
// Обновление интерфейса с понравившимися постами
function updateLikesUI(posts) {
  console.log('Updating likes UI with:', posts);
  
  // Находим контейнер для понравившихся постов
  const likesContainer = document.querySelector('.likes-container');
  
  if (!likesContainer) {
    console.warn('Контейнер для понравившихся постов не найден');
    return;
  }
  
  // Очищаем контейнер
  likesContainer.innerHTML = '';
  
  if (!posts || posts.length === 0) {
    likesContainer.innerHTML = '<p class="text-gray-500 text-center">Понравившихся постов пока нет</p>';
    return;
  }
  
  // Создаем карточки для каждого поста
  posts.forEach(post => {
    createPostElement(post, '.likes-container');
  });
}

// Обновление интерфейса с избранными постами
function updateFavoritesUI(posts) {
  console.log('Updating favorites UI with:', posts);
  
  // Находим контейнер для избранных постов
  const favoritesContainer = document.querySelector('.favorites-container');
  
  if (!favoritesContainer) {
    console.warn('Контейнер для избранных постов не найден');
    return;
  }
  
  // Очищаем контейнер
  favoritesContainer.innerHTML = '';
  
  if (!posts || posts.length === 0) {
    favoritesContainer.innerHTML = '<p class="text-gray-500 text-center">Избранных постов пока нет</p>';
    return;
  }
  
  // Создаем карточки для каждого поста
  posts.forEach(post => {
    createPostElement(post, '.favorites-container');
  });
}

// Обновление интерфейса с черновиками
function updateDraftsUI(posts) {
  console.log('Updating drafts UI with:', posts);
  
  // Находим контейнер для черновиков
  const draftsContainer = document.querySelector('.drafts-container');
  
  if (!draftsContainer) {
    console.warn('Контейнер для черновиков не найден');
    return;
  }
  
  // Очищаем контейнер
  draftsContainer.innerHTML = '';
  
  if (!posts || posts.length === 0) {
    draftsContainer.innerHTML = '<p class="text-gray-500 text-center">Черновиков пока нет</p>';
    return;
  }
  
  // Создаем карточки для каждого поста
  posts.forEach(post => {
    createPostElement(post, '.drafts-container');
  });
}
*/
