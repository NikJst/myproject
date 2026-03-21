// Утилиты для прокрутки

// Плавная прокрутка для контейнера постов
export function setupScrollEffect() {
  const postsContainerScroll = document.querySelector(".posts-container");
  if (postsContainerScroll) {
    postsContainerScroll.addEventListener("scroll", function () {
      if (this.scrollTop > 50) {
        this.classList.add("scrolled");
      } else {
        this.classList.remove("scrolled");
      }
    });
  }
}
