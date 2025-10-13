document.addEventListener('DOMContentLoaded', async () => {
    const idElem = document.getElementById('id');
    const nameElem = document.getElementById('name');
    const descriptionElem = document.getElementById('description');

    async function fetchJson(url) {
        try {
            const resp = await fetch(url, { credentials: 'include' });
            if (!resp.ok) {
                alert(`Ошибка загрузки данных: ${resp.status}`);
                throw new Error(`Ошибка загрузки ${url}: ${resp.status}`);
            }
            return resp.json();
        } catch (error) {
            console.error(error);
            alert('Ошибка загрузки данных');
            window.location.href = 'categories.html';
            return null;
        }
    }

    const urlParams = new URLSearchParams(window.location.search);
    const categoryId = urlParams.get('id');
    if (!categoryId) {
        alert('Не указан ID категории');
        window.location.href = 'categories.html';
        return;
    }

    const category = await fetchJson(`/api/category/${categoryId}`);
    if (!category) return;

    idElem.textContent = category.id || '---';
    nameElem.textContent = category.name || 'Неизвестно';
    descriptionElem.textContent = category.description || 'Отсутствует описание';
});
