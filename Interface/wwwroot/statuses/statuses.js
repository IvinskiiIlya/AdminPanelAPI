document.getElementById('save-button').addEventListener('click', async () => {
    const name = document.getElementById('name').value;

    if (!name) {
        alert('Введите название статуса');
        return;
    }

    const dto = { name: name }; // Должно соответствовать CreateStatusDto на сервере

    try {
        const response = await fetch('/api/status', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                // Добавьте заголовок авторизации, если требуется токен, например:
            },
            body: JSON.stringify(dto)
        });

        if (response.ok) {
            const result = await response.json();
            alert('Статус успешно создан: ' + result.name);
            // Можно очистить форму или перейти куда-то
            document.getElementById('status-form').reset();
        } else {
            const errorText = await response.text();
            alert('Ошибка при создании статуса: ' + errorText);
        }
    } catch (error) {
        alert('Произошла ошибка соединения: ' + error.message);
    }
});
