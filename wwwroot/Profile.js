// Загрузка данных профиля
export async function loadProfileData(username) {
  try {
    // Если username не передан, получаем текущего пользователя
    if (!username) {
      const currentUser = await getCurrentUser();
      username = currentUser.username || currentUser.name;
    }
    
    const response = await fetch(`/api/Profile/${username}/content`);
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
        value.textContent = '2.3K';
        break;
      case 'Избранное':
        // Заглушка
        value.textContent = '489';
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