document.addEventListener('DOMContentLoaded', async () => {
    const idInput = document.getElementById('id');
    const nameInput = document.getElementById('name');
    const formMessage = document.getElementById('formMessage');
    const statusForm = document.getElementById('statusForm');

    let loadedStatus = null;

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

    const urlParams = new URLSearchParams(window.location.search);
    const statusId = urlParams.get('id');
    if (!statusId) {
        alert('Не указан ID статуса');
        window.location.href = 'statuses.html';
        return;
    }

    async function loadStatus() {
        try {
            const response = await fetch(`/api/status/${statusId}`, { credentials: 'include' });
            if (!response.ok) {
                if (response.status === 404) {
                    alert('Статус не найден');
                } else if (response.status === 401) {
                    alert('Сессия истекла, требуется повторная авторизация');
                } else {
                    alert('Ошибка загрузки статуса');
                }
                window.location.href = 'statuses.html';
                return;
            }
            loadedStatus = await response.json();
            idInput.value = loadedStatus.id;
            nameInput.value = loadedStatus.name;
        } catch (error) {
            console.error(error);
            alert('Ошибка сервера при загрузке статуса');
            window.location.href = 'statuses.html';
        }
    }

    await loadStatus();

    statusForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const dto = {
            id: Number(idInput.value),
            name: nameInput.value.trim()
        };

        if (dto.name.length < 1) {
            alert('Название статуса обязательно.');
            return;
        }

        try {
            const response = await fetch(`/api/status/${dto.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(dto)
            });

            if (response.status === 204) {
                alert('Статус успешно обновлен');
                window.location.href = 'statuses.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                alert('Ошибка: ' + (errorData.detail || 'Некорректные данные'));
            } else if (response.status === 401) {
                alert('Сессия истекла, требуется повторная авторизация');
                window.location.href = '../../auth.html';
            } else if (response.status === 404) {
                alert('Статус не найден');
                window.location.href = 'statuses.html';
            } else {
                alert('Ошибка обновления статуса.');
            }
        } catch (error) {
            console.error(error);
            alert('Ошибка сервера. Попробуйте позже.');
        }
    });
});
