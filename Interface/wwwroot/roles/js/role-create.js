document.addEventListener('DOMContentLoaded', () => {
    const formMessage = document.getElementById('formMessage');
    const roleForm = document.getElementById('roleForm');

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

    roleForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const name = document.getElementById('name').value.trim();  
        if (name.length < 1) {
            formMessage.textContent = 'Название роли обязательно.';
            return;
        }

        try {
            const response = await fetch('/api/role', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(name)
            });

            if (response.status === 201) {
                window.location.href = 'roles.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                formMessage.textContent = 'Ошибка: ' + (errorData.detail || 'Некорректные данные');
            } else if (response.status === 401) {
                window.location.href = '../../auth.html';
            } else {
                formMessage.textContent = 'Ошибка создания роли.';
            }
        } catch (error) {
            console.error(error);
            formMessage.textContent = 'Ошибка сервера. Попробуйте позже.';
        }
    });
});
