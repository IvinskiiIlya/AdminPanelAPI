document.addEventListener('DOMContentLoaded', async () => {
    const categorySelect = document.getElementById('category');
    const messageInput = document.getElementById('message');
    const formMessage = document.getElementById('formMessage');
    const feedbackForm = document.getElementById('feedbackForm');

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
        alert('Требуется авторизация, перенаправление на страницу входа');
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
            alert('Ошибка загрузки данных.');
            return [];
        }
    }

    const urlParams = new URLSearchParams(window.location.search);
    const feedbackId = urlParams.get('id');
    if (!feedbackId) {
        alert('Не указан ID отзыва');
        window.location.href = 'feedbacks.html';
        return;
    }

    async function loadCategories() {
        const categories = await loadPagedData('/api/category?pageNumber=1&pageSize=100');
        categories.forEach(cat => {
            const option = document.createElement('option');
            option.value = cat.id;
            option.textContent = cat.name;
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
                    alert('Отзыв не найден');
                } else if (response.status === 401) {
                    alert('Сессия истекла, требуется повторная авторизация');
                } else {
                    alert('Ошибка загрузки отзыва');
                }
                window.location.href = 'feedbacks.html';
                return;
            }
            loadedFeedback = await response.json();
            categorySelect.value = loadedFeedback.categoryId;
            messageInput.value = loadedFeedback.message;
        } catch (error) {
            console.error(error);
            alert('Ошибка сервера при загрузке отзыва');
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
            alert('Требуется авторизация, перенаправление на страницу входа');
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
            alert('Пожалуйста, заполните все поля корректно.');
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
                alert('Отзыв успешно обновлен.');
                window.location.href = 'feedbacks.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                alert('Ошибка: ' + (errorData.detail || 'Некорректные данные'));
            } else if (response.status === 401) {
                alert('Сессия истекла, требуется повторная авторизация');
                window.location.href = '../../auth.html';
            } else if (response.status === 404) {
                alert('Отзыв не найден');
                window.location.href = 'feedbacks.html';
            } else {
                alert('Ошибка обновления отзыва.');
            }
        } catch (error) {
            console.error(error);
            alert('Ошибка сервера. Попробуйте позже.');
        }
    });
});
