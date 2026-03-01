const openBtn = document.querySelector(".settings-btn");
const menu = document.querySelector(".settings-menu");
const closeBtn = document.querySelector(".close-menu");

openBtn.addEventListener("click", () => menu.classList.add("active"));
closeBtn.addEventListener("click", () => menu.classList.remove("active"));

// Берём гостевой ID из cookie
function getCookie(name) {
  const value = `; ${document.cookie}`;
  const parts = value.split(`; ${name}=`);
  if (parts.length === 2) return parts.pop().split(";").shift();
}

async function ensureGuestId() {
  try {
    console.log("Текущий origin страницы:", window.location.origin);
    console.log("Cookie ДО запроса /guest:", document.cookie);

    const resp = await fetch("/guest", { credentials: "same-origin" });
    console.log("Ответ /guest:", resp.status, resp.statusText, resp.url);

    let guestPayload;
    try {
      guestPayload = await resp.clone().json();
    } catch {
      guestPayload = null;
    }
    console.log("Payload /guest:", guestPayload);

    console.log("Cookie ПОСЛЕ запроса /guest:", document.cookie);
  } catch (err) {
    console.error("Не удалось получить гостя с сервера (/guest):", err);
  }

  const guestId = getCookie("GuestId");
  console.log("Ваш гостевой ID ===> ", guestId);
}

ensureGuestId();

// Можно сразу передать его на сервер через fetch или WebSocket
// fetch('/api/action', { method: 'POST', body: JSON.stringify({ guestId, ... }) });
