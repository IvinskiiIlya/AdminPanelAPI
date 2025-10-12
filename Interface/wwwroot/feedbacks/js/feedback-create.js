document.addEventListener('DOMContentLoaded', async () => {
    const categorySelect = document.getElementById('category');
    const formMessage = document.getElementById('formMessage');
    const feedbackForm = document.getElementById('feedbackForm');

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
            formMessage.textContent = 'Ошибка загрузки данных.';
            return [];
        }
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
            formMessage.textContent = 'Пожалуйста, заполните все поля корректно.';
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
                window.location.href = 'feedbacks.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                formMessage.textContent = 'Ошибка: ' + (errorData.detail || 'Некорректные данные');
            } else if (response.status === 401) {
                window.location.href = '../../auth.html';
            } else {
                formMessage.textContent = 'Ошибка создания отзыва.';
            }
        } catch (error) {
            console.error(error);
            formMessage.textContent = 'Ошибка сервера. Попробуйте позже.';
        }
    });
});
