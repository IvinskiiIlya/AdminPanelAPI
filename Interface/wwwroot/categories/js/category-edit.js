document.addEventListener('DOMContentLoaded', async () => {
    const idInput = document.getElementById('id');             
    const nameInput = document.getElementById('name');
    const descriptionInput = document.getElementById('description');
    const formMessage = document.getElementById('formMessage');
    const categoryForm = document.getElementById('categoryForm');

    let loadedCategory = null;

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

    const urlParams = new URLSearchParams(window.location.search);
    const categoryId = urlParams.get('id');
    if (!categoryId) {
        alert('Не указан ID категории');
        window.location.href = 'categories.html';
        return;
    }

    async function loadCategory() {
        try {
            const response = await fetch(`/api/category/${categoryId}`, { credentials: 'include' });
            if (!response.ok) {
                if (response.status === 404) {
                    alert('Категория не найдена');
                } else if (response.status === 401) {
                    alert('Требуется авторизация');
                } else {
                    alert('Ошибка загрузки категории');
                }
                window.location.href = 'categories.html';
                return;
            }
            loadedCategory = await response.json();
            idInput.value = loadedCategory.id;              
            nameInput.value = loadedCategory.name;
            descriptionInput.value = loadedCategory.description || '';
        } catch (error) {
            console.error(error);
            alert('Ошибка сервера при загрузке категории');
            window.location.href = 'categories.html';
        }
    }

    await loadCategory();

    categoryForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const dto = {
            id: Number(idInput.value), 
            name: nameInput.value.trim(),
            description: descriptionInput.value.trim() || null
        };

        if (dto.name.length < 1) {
            formMessage.textContent = 'Название категории обязательно.';
            return;
        }

        try {
            const response = await fetch(`/api/category/${dto.id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                credentials: 'include',
                body: JSON.stringify(dto)
            });

            if (response.status === 204) {
                window.location.href = 'categories.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                formMessage.textContent = 'Ошибка: ' + (errorData.detail || 'Некорректные данные');
            } else if (response.status === 401) {
                window.location.href = '../../auth.html';
            } else if (response.status === 404) {
                alert('Категория не найдена');
                window.location.href = 'categories.html';
            } else {
                formMessage.textContent = 'Ошибка обновления категории.';
            }
        } catch (error) {
            console.error(error);
            formMessage.textContent = 'Ошибка сервера. Попробуйте позже.';
        }
    });
});
