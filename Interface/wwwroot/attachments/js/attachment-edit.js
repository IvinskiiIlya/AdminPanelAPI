document.addEventListener('DOMContentLoaded', async () => {
    const feedbackIdInput = document.getElementById('feedbackId');
    const filePathInput = document.getElementById('filePath');
    const fileTypeInput = document.getElementById('fileType');
    const idInput = document.getElementById('id');
    const formMessage = document.getElementById('formMessage');
    const attachmentForm = document.getElementById('attachmentForm');

    const urlParams = new URLSearchParams(window.location.search);
    const attachmentId = urlParams.get('id');
    if (!attachmentId) {
        alert('Не указан ID вложения');
        window.location.href = 'attachments.html';
        return;
    }

    async function getUserInfo() {
        const response = await fetch('/api/auth/userinfo', {
            method: 'GET',
            credentials: 'include'
        });
        if (!response.ok) return null;
        const data = await response.json();
        return { userId: data.id || data.userId || null, name: data.name, roles: data.roles };
    }

    async function loadAttachment() {
        try {
            const response = await fetch(`/api/attachment/${attachmentId}`, {
                credentials: 'include'
            });
            if (!response.ok) {
                if (response.status === 404) {
                    alert('Вложение не найдено');
                } else if (response.status === 401) {
                    alert('Требуется авторизация');
                } else {
                    alert('Ошибка загрузки вложения');
                }
                window.location.href = 'attachments.html';
                return;
            }
            const att = await response.json();
            idInput.value = att.id;
            feedbackIdInput.value = att.feedbackId;
            filePathInput.value = att.filePath;
            fileTypeInput.value = att.fileType;
        } catch (error) {
            console.error(error);
            alert('Ошибка сервера при загрузке вложения');
            window.location.href = 'attachments.html';
        }
    }

    const userInfo = await getUserInfo();
    if (!userInfo) {
        window.location.href = '../../auth.html';
        return;
    }

    await loadAttachment();

    attachmentForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const dto = {
            id: Number(idInput.value),
            feedbackId: Number(feedbackIdInput.value),
            filePath: filePathInput.value.trim(),
            fileType: fileTypeInput.value.trim()
        };

        if (!dto.feedbackId || !dto.filePath || !dto.fileType) {
            formMessage.textContent = 'Пожалуйста, заполните все поля корректно.';
            return;
        }

        try {
            const response = await fetch(`/api/attachment/${attachmentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                credentials: 'include',
                body: JSON.stringify(dto)
            });

            if (response.status === 204) {
                window.location.href = 'attachments.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                formMessage.textContent = 'Ошибка: ' + (errorData.detail || 'Некорректные данные');
            } else if (response.status === 401) {
                window.location.href = '../../auth.html';
            } else if (response.status === 404) {
                alert('Вложение не найдено');
                window.location.href = 'attachments.html';
            } else {
                formMessage.textContent = 'Ошибка обновления вложения.';
            }
        } catch (error) {
            console.error(error);
            formMessage.textContent = 'Ошибка сервера. Попробуйте позже.';
        }
    });
});
