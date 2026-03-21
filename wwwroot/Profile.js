// Загрузка данных профиля
export async function loadProfileData(username) {
  try {
    // Если username не передан, получаем текущего пользователя
    if (!username) {
      const currentUser = await getCurrentUser();
      username = currentUser.username || currentUser.name;
    }
    
    const response = await fetch(`/api/Profile/${username}/info`);
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
  
  // Обновляем имя пользователя
  const usernameElement = document.querySelector('h1');
  if (usernameElement) {
    usernameElement.textContent = profileData.username || 'Пользователь';
    console.log('Updated username:', usernameElement.textContent);
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
        // Заглушка, можно добавить likeCount в ProfileDto
        value.textContent = '14';
        break;
      case 'Избранное':
        // Заглушка
        value.textContent = '48';
        break;
    }
  });
}

// Получение username из URL или текущего пользователя
export function getCurrentUsername() {
  // Можно получить из URL параметров или из куки
  const pathParts = window.location.pathname.split('/');
  const usernameIndex = pathParts.indexOf('profile') + 1;
  
  if (usernameIndex > 0 && pathParts[usernameIndex]) {
    return pathParts[usernameIndex];
  }
  
  // Если username в URL нет, используем текущего пользователя
  return null; // Будет определено в loadProfileData
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
  
  // Возвращаем заглушку если не удалось получить пользователя
  return { username: 'Guest_' + Math.random().toString(36).substr(2, 5), name: 'Guest' };
}

// // Обновление информации профиля
// export async function updateProfileInfo(profileData) {
//   try {
//     const response = await fetch('/api/Profile/info', {
//       method: 'PATCH',
//       headers: {
//         'Content-Type': 'application/json',
//       },
//       body: JSON.stringify(profileData)
//     });
    
//     if (!response.ok) {
//       throw new Error('Ошибка обновления профиля');
//     }
    
//     const updatedProfile = await response.json();
//     console.log('Профиль обновлен:', updatedProfile);
    
//     // Обновляем интерфейс с новыми данными
//     updateProfileUI(updatedProfile);
    
//     return updatedProfile;
//   } catch (error) {
//     console.error('Ошибка при обновлении профиля:', error);
//     throw error;
//   }
// }

// // Загрузка постов пользователя
// export async function loadUserPosts(username) {
//   try {
//     // Если username не передан, получаем текущего пользователя
//     if (!username) {
//       const currentUser = await getCurrentUser();
//       username = currentUser.username || currentUser.name;
//     }
    
//     const response = await fetch(`/api/Profile/${username}/posts`);
//     if (!response.ok) {
//       throw new Error('Ошибка загрузки постов пользователя');
//     }
    
//     const userPosts = await response.json();
//     console.log('Посты пользователя:', userPosts);
    
//     // Обновляем интерфейс с постами
//     updatePostsUI(userPosts);
    
//     return userPosts;
//   } catch (error) {
//     console.error('Ошибка при загрузке постов:', error);
//     throw error;
//   }
// }

// // Обновление интерфейса с постами
// function updatePostsUI(posts) {
//   console.log('Updating posts UI with:', posts);
  
//   // Находим контейнер для постов
//   const postsContainer = document.querySelector('.posts-container') || 
//                         document.querySelector('[data-posts-container]');
  
//   if (!postsContainer) {
//     console.warn('Контейнер для постов не найден');
//     return;
//   }
  
//   // Очищаем контейнер
//   postsContainer.innerHTML = '';
  
//   if (!posts || posts.length === 0) {
//     postsContainer.innerHTML = '<p class="text-gray-500 text-center">У пользователя пока нет постов</p>';
//     return;
//   }
  
//   // Создаем HTML для каждого поста
//   posts.forEach(post => {
//     const postElement = createPostElement(post);
//     postsContainer.appendChild(postElement);
//   });
// }

// // Создание элемента поста
// function createPostElement(post) {
//   const postDiv = document.createElement('div');
//   postDiv.className = 'bg-white rounded-lg shadow-md p-6 mb-4 border border-gray-200';
  
//   postDiv.innerHTML = `
//     <div class="flex justify-between items-start mb-4">
//       <div class="flex items-center">
//         <div class="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center text-white font-semibold mr-3">
//           ${post.username ? post.username[0].toUpperCase() : 'U'}
//         </div>
//         <div>
//           <h3 class="font-semibold text-gray-900">${post.username || 'Unknown User'}</h3>
//           <p class="text-sm text-gray-500">@${post.username || 'unknown'}</p>
//         </div>
//       </div>
//       <div class="flex items-center space-x-2">
//         ${post.likedByUser ? 
//           `<button class="text-red-500 hover:text-red-600 transition-colors" onclick="toggleLike('${post.postId}')">
//             <i class="fas fa-heart"></i>
//           </button>` :
//           `<button class="text-gray-400 hover:text-red-500 transition-colors" onclick="toggleLike('${post.postId}')">
//             <i class="far fa-heart"></i>
//           </button>`
//         }
//         ${post.isGuest ? '<span class="text-xs bg-gray-200 text-gray-600 px-2 py-1 rounded">Гость</span>' : ''}
//       </div>
//     </div>
    
//     ${post.title ? `<h2 class="text-xl font-bold text-gray-900 mb-3">${post.title}</h2>` : ''}
    
//     <div class="text-gray-700 whitespace-pre-wrap">${post.text || ''}</div>
    
//     <div class="mt-4 pt-4 border-t border-gray-100">
//       <div class="flex justify-between items-center text-sm text-gray-500">
//         <span>ID: ${post.postId}</span>
//         <span>UserID: ${post.userId}</span>
//       </div>
//     </div>
//   `;
  
//   return postDiv;
// }

// // Функция для переключения лайка (заглушка, нужно реализовать)
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