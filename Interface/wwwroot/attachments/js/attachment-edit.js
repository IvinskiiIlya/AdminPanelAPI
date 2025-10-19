function showCustomAlert(message, duration = 3000) {
    let alertContainer = document.getElementById('custom-alert-container');
    if (!alertContainer) {
        alertContainer = document.createElement('div');
        alertContainer.id = 'custom-alert-container';
        Object.assign(alertContainer.style, {
            position: 'fixed',
            top: '1rem',
            right: '1rem',
            zIndex: 10000,
            display: 'flex',
            flexDirection: 'column',
            gap: '0.5rem',
            maxWidth: '300px',
            fontFamily: '"Segoe UI", Tahoma, Geneva, Verdana, sans-serif'
        });
        document.body.appendChild(alertContainer);
    }

    const alert = document.createElement('div');
    alert.textContent = message;
    Object.assign(alert.style, {
        backgroundColor: 'rgba(51, 51, 51, 0.9)',
        color: 'white',
        padding: '0.75rem 1rem',
        borderRadius: '0.625rem',
        boxShadow: '0 0.4rem 0.75rem rgba(51, 51, 51, 0.7)',
        fontSize: '1rem',
        opacity: '1',
        transition: 'opacity 0.5s ease'
    });
    alertContainer.appendChild(alert);

    setTimeout(() => {
        alert.style.opacity = '0';
        setTimeout(() => alert.remove(), 500);
    }, duration);
}

function showPendingAlerts() {
    const pendingAlert = sessionStorage.getItem('pendingAlert');
    if (pendingAlert) {
        showCustomAlert(pendingAlert);
        sessionStorage.removeItem('pendingAlert');
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    showPendingAlerts();

    const feedbackIdInput = document.getElementById('feedbackId');
    const filePathInput = document.getElementById('filePath');
    const fileTypeInput = document.getElementById('fileType');
    const idInput = document.getElementById('id');
    const formMessage = document.getElementById('formMessage');
    const attachmentForm = document.getElementById('attachmentForm');

    const urlParams = new URLSearchParams(window.location.search);
    const attachmentId = urlParams.get('id');
    if (!attachmentId) {
        showCustomAlert('Не указан ID вложения');
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
                    showCustomAlert('Вложение не найдено');
                } else if (response.status === 401) {
                    showCustomAlert('Требуется авторизация');
                } else {
                    showCustomAlert('Ошибка загрузки вложения');
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
            showCustomAlert('Ошибка сервера при загрузке вложения');
            window.location.href = 'attachments.html';
        }
    }

    const userInfo = await getUserInfo();
    if (!userInfo) {
        sessionStorage.setItem('pendingAlert', 'Требуется авторизация, перенаправление на страницу входа');
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
            showCustomAlert('Пожалуйста, заполните все поля корректно.');
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
                sessionStorage.setItem('pendingAlert', 'Вложение успешно обновлено');
                window.location.href = 'attachments.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                showCustomAlert('Ошибка: ' + (errorData.detail || 'Некорректные данные'));
            } else if (response.status === 401) {
                sessionStorage.setItem('pendingAlert', 'Сессия истекла, требуется повторная авторизация');
                window.location.href = '../../auth.html';
            } else if (response.status === 404) {
                showCustomAlert('Вложение не найдено');
                window.location.href = 'attachments.html';
            } else {
                showCustomAlert('Ошибка обновления вложения.');
            }
        } catch (error) {
            console.error(error);
            showCustomAlert('Ошибка сервера. Попробуйте позже.');
        }
    });
});