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

document.addEventListener('DOMContentLoaded', () => {
    showPendingAlerts();

    const feedbackListDiv = document.getElementById('feedbackList');
    const attachmentForm = document.getElementById('attachmentForm');
    const formMessage = document.getElementById('formMessage');
    const fileInput = document.getElementById('fileInput');
    const fileNameSpan = document.getElementById('fileName');
    const fileSelectBtn = document.getElementById('fileSelectBtn');

    fileSelectBtn.addEventListener('click', () => {
        fileInput.click();
    });

    fileInput.addEventListener('change', () => {
        if (fileInput.files.length > 0) {
            fileNameSpan.textContent = fileInput.files[0].name;
        } else {
            fileNameSpan.textContent = 'Файл не выбран';
        }
    });

    async function getUserInfo() {
        const response = await fetch('/api/auth/userinfo', {
            method: 'GET',
            credentials: 'include'
        });
        if (!response.ok) return null;
        const data = await response.json();
        return { userId: data.id || data.userId || null, name: data.name, roles: data.roles };
    }

    async function loadAllFeedbacks() {
        const response = await fetch('/api/feedback?pageNumber=1&pageSize=1000', {
            method: 'GET',
            credentials: 'include'
        });
        if (!response.ok) {
            throw new Error(`Ошибка загрузки отзывов: ${response.status}`);
        }
        const data = await response.json();
        return data.data || [];
    }

    async function renderFeedbacks() {
        feedbackListDiv.innerHTML = '<p>Загрузка отзывов...</p>';
        const userInfo = await getUserInfo();
        if (!userInfo || !userInfo.userId) {
            sessionStorage.setItem('pendingAlert', 'Требуется авторизация, перенаправление на страницу входа');
            window.location.href = '../../auth.html';
            return;
        }

        try {
            const allFeedbacks = await loadAllFeedbacks();
            const userFeedbacks = allFeedbacks.filter(fb => fb.userId === userInfo.userId);

            const feedbacksToShow = userFeedbacks.slice(0, 100);

            if (feedbacksToShow.length === 0) {
                feedbackListDiv.innerHTML = '<p>У вас пока нет отзывов.</p>';
                return;
            }

            feedbackListDiv.innerHTML = '';
            feedbacksToShow.forEach(fb => {
                const messagePreview = fb.message.length > 100 ? fb.message.substring(0, 100) + '...' : fb.message;

                const label = document.createElement('label');
                label.style.display = 'block';
                label.style.marginBottom = '5px';

                const checkbox = document.createElement('input');
                checkbox.type = 'radio';
                checkbox.name = 'feedbackCheckbox';
                checkbox.value = fb.id;

                label.appendChild(checkbox);
                label.appendChild(document.createTextNode(messagePreview));

                feedbackListDiv.appendChild(label);
            });
        } catch (error) {
            console.error(error);
            feedbackListDiv.innerHTML = '<p>Ошибка загрузки отзывов. Попробуйте позже.</p>';
        }
    }

    attachmentForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        formMessage.textContent = '';

        const userInfo = await getUserInfo();
        if (!userInfo) {
            sessionStorage.setItem('pendingAlert', 'Требуется авторизация, перенаправление на страницу входа');
            window.location.href = '../../auth.html';
            return;
        }

        const selectedFeedback = document.querySelector('input[name="feedbackCheckbox"]:checked');
        if (!selectedFeedback) {
            showCustomAlert('Пожалуйста, выберите отзыв.');
            return;
        }

        const file = fileInput.files[0];
        if (!file) {
            formMessage.textContent = 'Пожалуйста, выберите файл.';
            return;
        }

        const fileType = file.type || 'application/octet-stream';

        const formData = new FormData();
        formData.append('File', file);
        formData.append('UserId', String(userInfo.userId));
        formData.append('FeedbackId', String(Number(selectedFeedback.value)));
        formData.append('FileType', fileType);
        formData.append('FilePath', 'temp');

        try {
            const response = await fetch('/api/attachment', {
                method: 'POST',
                credentials: 'include',
                body: formData
            });

            if (response.status === 201) {
                sessionStorage.setItem('pendingAlert', 'Вложение успешно добавлено');
                window.location.href = 'attachments.html';
            } else if (response.status === 400) {
                const errorData = await response.json();
                if (errorData.errors) {
                    showCustomAlert('Ошибка: ' + Object.values(errorData.errors).flat().join(', '));
                } else {
                    showCustomAlert('Ошибка: ' + (errorData.detail || 'Некорректные данные'));
                }
            } else if (response.status === 401) {
                sessionStorage.setItem('pendingAlert', 'Сессия истекла, требуется повторная авторизация');
                window.location.href = '../../auth.html';
            } else {
                showCustomAlert('Ошибка при добавлении вложения.');
            }
        } catch (error) {
            console.error(error);
            showCustomAlert('Ошибка сервера. Попробуйте позже.');
        }
    });

    renderFeedbacks();
});