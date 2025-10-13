document.addEventListener('DOMContentLoaded', async () => {
    const nameInput = document.getElementById('name');
    const formMessage = document.getElementById('formMessage');
    const roleForm = document.getElementById('roleForm');

    async function getUserInfo() {
        const response = await fetch('/api/auth/userinfo', { method: 'GET', credentials: 'include' });
        if (!response.ok) return null;
        const data = await response.json();
        return { userId: data.id || data.userId || null, name: data.name, roles: data.roles };
    }

    const userInfo = await getUserInfo();
    if (!userInfo) {
        window.location.href = '../../auth.html';
        return;
    }

    const urlParams = new URLSearchParams(window.location.search);
    const roleId = urlParams.get('id');
    if (!roleId) {
        alert('Не указан ID роли');
        window.location.href = 'roles.html';
        return;
    }

    async function loadRole() {
        try {
            const response = await fetch(`/api/role/${encodeURIComponent(roleId)}`, { credentials: 'include' });
            if (!response.ok) {
                if (response.status === 404) {
                    alert('Роль не найдена');
                } else if (response.status === 401) {
                    alert('Требуется авторизация');
                } else {
                    alert('Ошибка загрузки роли');
                }
                window.location.href = 'roles.html';
                return;
            }
            const loadedRole = await response.json();
            nameInput.value = loadedRole.name;
        } catch (error) {
            console.error(error);
            alert('Ошибка сервера при загрузке роли');
            window.location.href = 'roles.html';
        }
    }

    await loadRole();

    roleForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        if (nameInput.value.trim().length < 1) {
            formMessage.textContent = 'Название роли обязательно.';
            return;
        }

        const dto = {
            id: roleId,
            name: nameInput.value.trim()
        };

        try {
            const response = await fetch(`/api/role/${encodeURIComponent(roleId)}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(dto)
            });

            if (response.status === 204) {
                window.location.href = 'roles.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                formMessage.textContent = 'Ошибка: ' + (errorData.detail || 'Некорректные данные');
            } else if (response.status === 401) {
                window.location.href = '../../auth.html';
            } else if (response.status === 404) {
                alert('Роль не найдена');
                window.location.href = 'roles.html';
            } else {
                formMessage.textContent = 'Ошибка обновления роли.';
            }
        } catch (error) {
            console.error(error);
            formMessage.textContent = 'Ошибка сервера. Попробуйте позже.';
        }
    });
});
