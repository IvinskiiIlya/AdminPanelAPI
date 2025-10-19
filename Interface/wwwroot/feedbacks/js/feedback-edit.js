function showCustomAlert(message, duration = 3000) {
    let alertContainer = document.getElementById('custom-alert-container');
    if (!alertContainer) {
        alertContainer = document.createElement('div');
        alertContainer.id = 'custom-alert-container';
        Object.assign(alertContainer.style, {
            position: 'fixed',
            top: '1rem',
            right: '1rem',
            zIndex: 10000,
            display: 'flex',
            flexDirection: 'column',
            gap: '0.5rem',
            maxWidth: '300px',
            fontFamily: '"Segoe UI", Tahoma, Geneva, Verdana, sans-serif'
        });
        document.body.appendChild(alertContainer);
    }

    const alert = document.createElement('div');
    alert.textContent = message;
    Object.assign(alert.style, {
        backgroundColor: 'rgba(51, 51, 51, 0.9)',
        color: 'white',
        padding: '0.75rem 1rem',
        borderRadius: '0.625rem',
        boxShadow: '0 0.4rem 0.75rem rgba(51, 51, 51, 0.7)',
        fontSize: '1rem',
        opacity: '1',
        transition: 'opacity 0.5s ease'
    });
    alertContainer.appendChild(alert);

    setTimeout(() => {
        alert.style.opacity = '0';
        setTimeout(() => alert.remove(), 500);
    }, duration);
}

function showPendingAlerts() {
    const pendingAlert = sessionStorage.getItem('pendingAlert');
    if (pendingAlert) {
        showCustomAlert(pendingAlert);
        sessionStorage.removeItem('pendingAlert');
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    showPendingAlerts();

    const categorySelect = document.getElementById('category');
    const messageInput = document.getElementById('message');
    const formMessage = document.getElementById('formMessage');
    const feedbackForm = document.getElementById('feedbackForm');

    const categoryNameMap = {
        errors: 'Ошибки',
        suggestions: 'Предложения',
        questions: 'Вопросы',
        complaints: 'Жалобы',
        docs: 'Документация',
        security: 'Безопасность',
        performance: 'Производительность',
        ui: 'Интерфейс',
        integration: 'Интеграция',
        other: 'Прочее'
    };

    let loadedFeedback = null;

    async function getUserInfo() {
        const response = await fetch('/api/auth/userinfo', {
            method: 'GET',
            credentials: 'include'
        });
        if (!response.ok) return null;
        const data = await response.json();
        return { userId: data.id || data.userId || null, name: data.name, roles: data.roles };
    }

    const userInfo = await getUserInfo();
    if (!userInfo) {
        sessionStorage.setItem('pendingAlert', 'Требуется авторизация, перенаправление на страницу входа');
        window.location.href = '../../auth.html';
        return;
    }

    async function loadPagedData(url) {
        try {
            const response = await fetch(url, {
                credentials: 'include'
            });
            if (!response.ok) throw new Error(`Ошибка загрузки данных: ${response.status}`);
            const respJson = await response.json();
            return respJson.data || [];
        } catch (err) {
            console.error(err);
            showCustomAlert('Ошибка загрузки данных.');
            return [];
        }
    }

    const urlParams = new URLSearchParams(window.location.search);
    const feedbackId = urlParams.get('id');
    if (!feedbackId) {
        showCustomAlert('Не указан ID отзыва');
        window.location.href = 'feedbacks.html';
        return;
    }

    async function loadCategories() {
        const categories = await loadPagedData('/api/category?pageNumber=1&pageSize=100');
        categories.forEach(cat => {
            const option = document.createElement('option');
            option.value = cat.id;
            option.textContent = categoryNameMap[cat.name] || cat.name;
            categorySelect.appendChild(option);
        });
    }

    async function loadFeedback() {
        try {
            const response = await fetch(`/api/feedback/${feedbackId}`, {
                credentials: 'include'
            });
            if (!response.ok) {
                if (response.status === 404) {
                    showCustomAlert('Отзыв не найден');
                } else if (response.status === 401) {
                    sessionStorage.setItem('pendingAlert', 'Сессия истекла, требуется повторная авторизация');
                } else {
                    showCustomAlert('Ошибка загрузки отзыва');
                }
                window.location.href = 'feedbacks.html';
                return;
            }
            loadedFeedback = await response.json();
            categorySelect.value = loadedFeedback.categoryId;
            messageInput.value = loadedFeedback.message;
        } catch (error) {
            console.error(error);
            showCustomAlert('Ошибка сервера при загрузке отзыва');
            window.location.href = 'feedbacks.html';
        }
    }

    await loadCategories();
    await loadFeedback();

    feedbackForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const currentUserId = userInfo.userId;
        if (!currentUserId) {
            sessionStorage.setItem('pendingAlert', 'Требуется авторизация, перенаправление на страницу входа');
            window.location.href = '../../auth.html';
            return;
        }

        const dto = {
            id: Number(feedbackId),
            userId: currentUserId,
            categoryId: Number(categorySelect.value),
            statusId: loadedFeedback.statusId,
            message: messageInput.value.trim()
        };

        if (!dto.categoryId || dto.message.length < 10) {
            showCustomAlert('Пожалуйста, заполните все поля корректно.');
            return;
        }

        try {
            const response = await fetch(`/api/feedback/${feedbackId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(dto)
            });

            if (response.status === 204) {
                sessionStorage.setItem('pendingAlert', 'Отзыв успешно обновлен.');
                window.location.href = 'feedbacks.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                showCustomAlert('Ошибка: ' + (errorData.detail || 'Некорректные данные'));
            } else if (response.status === 401) {
                sessionStorage.setItem('pendingAlert', 'Сессия истекла, требуется повторная авторизация');
                window.location.href = '../../auth.html';
            } else if (response.status === 404) {
                showCustomAlert('Отзыв не найден');
                window.location.href = 'feedbacks.html';
            } else {
                showCustomAlert('Ошибка обновления отзыва.');
            }
        } catch (error) {
            console.error(error);
            showCustomAlert('Ошибка сервера. Попробуйте позже.');
        }
    });
});