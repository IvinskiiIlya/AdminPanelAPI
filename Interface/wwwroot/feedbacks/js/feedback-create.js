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

    async function loadCategories() {
        const categories = await loadPagedData('/api/category?pageNumber=1&pageSize=100');
        categories.forEach(cat => {
            const option = document.createElement('option');
            option.value = cat.id;
            option.textContent = categoryNameMap[cat.name] || cat.name;
            categorySelect.appendChild(option);
        });
    }

    await loadCategories();

    feedbackForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const NEW_STATUS_ID = 1;

        const dto = {
            userId: userInfo.userId,
            categoryId: Number(categorySelect.value),
            statusId: NEW_STATUS_ID,
            message: document.getElementById('message').value.trim()
        };

        if (!dto.categoryId || dto.message.length < 10) {
            showCustomAlert('Пожалуйста, заполните все поля корректно.');
            return;
        }

        try {
            const response = await fetch('/api/feedback', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(dto)
            });

            if (response.status === 201) {
                sessionStorage.setItem('pendingAlert', 'Отзыв успешно создан');
                window.location.href = 'feedbacks.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                showCustomAlert('Ошибка: ' + (errorData.detail || 'Некорректные данные'));
            } else if (response.status === 401) {
                sessionStorage.setItem('pendingAlert', 'Сессия истекла, требуется повторная авторизация');
                window.location.href = '../../auth.html';
            } else {
                showCustomAlert('Ошибка создания отзыва.');
            }
        } catch (error) {
            console.error(error);
            showCustomAlert('Ошибка сервера. Попробуйте позже.');
        }
    });
});