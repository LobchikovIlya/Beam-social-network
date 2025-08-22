window.scrollToBottom = (element) => {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }

};

window.onload = function () {
    checkToken(); // Проверяем токен при загрузке

    // Отслеживаем изменение токена в localStorage
    window.addEventListener("storage", function (event) {
        if (event.key === "token") {
            checkToken();
        }
    });

    // Запускаем таймер для периодической проверки токена (на случай, если storage-событие не сработает)
    const interval = setInterval(() => {
        if (localStorage.getItem('token')) {
            checkToken();
            clearInterval(interval); // Останавливаем проверку после обнаружения токена
        }
    }, 1000); // Проверять каждую секунду
};

function checkToken() {
    const token = localStorage.getItem('token');

    if (!token) {
        console.warn("🔒 Пользователь не авторизован, запросы не отправляются.");
        return;
    }

    console.log("✅ Токен найден! Начинаем отслеживание активности.");
    document.addEventListener("mousemove", sendActivity);

}

const apiBaseUrl = window['apiBaseUrl'] || 'http://localhost:5001';

function sendActivity() {
    fetch(`${apiBaseUrl}/api/users/update-activity`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
    }).catch(error => console.error("❌ Ошибка сети:", error));
}



