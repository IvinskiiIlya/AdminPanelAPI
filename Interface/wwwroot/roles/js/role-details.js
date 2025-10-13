document.addEventListener('DOMContentLoaded', async () => {
    const idElem = document.getElementById('id');
    const nameElem = document.getElementById('name');

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
            window.location.href = 'roles.html';
            return null;
        }
    }

    const urlParams = new URLSearchParams(window.location.search);
    const roleId = urlParams.get('id');
    if (!roleId) {
        alert('Не указан ID роли');
        window.location.href = 'roles.html';
        return;
    }

    const role = await fetchJson(`/api/role/${roleId}`);
    if (!role) return;

    idElem.textContent = role.id || '---';
    nameElem.textContent = role.name || 'Неизвестно';
});
