document.addEventListener('DOMContentLoaded', () => {
    const formMessage = document.getElementById('formMessage');
    const statusForm = document.getElementById('statusForm');

    async function getUserInfo() {
        const response = await fetch('/api/auth/userinfo', {
            method: 'GET',
            credentials: 'include'
        });
        if (!response.ok) return null;

        const data = await response.json();
        return { userId: data.id || data.userId || null, name: data.name, roles: data.roles };
    }

    getUserInfo().then(userInfo => {
        if (!userInfo) {
            window.location.href = '../../auth.html';
            return;
        }
    });

    statusForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const id = document.getElementById('id').value.trim();
        const name = document.getElementById('name').value.trim();

        if (name.length < 1) {
            formMessage.textContent = 'Название статуса обязательно.';
            return;
        }

        const dto = { id: parseInt(id, 10), name };
        try {
            const response = await fetch('/api/status', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(dto)
            });

            if (response.status === 201) {
                window.location.href = 'statuses.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                formMessage.textContent = 'Ошибка: ' + (errorData.detail || 'Некорректные данные');
            } else if (response.status === 401) {
                window.location.href = '../../auth.html';
            } else {
                formMessage.textContent = 'Ошибка создания статуса.';
            }
        } catch (error) {
            console.error(error);
            formMessage.textContent = 'Ошибка сервера. Попробуйте позже.';
        }
    });
});
