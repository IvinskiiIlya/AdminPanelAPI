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

    const feedbackMessageElem = document.getElementById('feedbackMessagePreview');
    const filePathElem = document.getElementById('filePath');
    const fileTypeElem = document.getElementById('fileType');
    const createdAtElem = document.getElementById('createdAt');
    const fileLinkElem = document.getElementById('fileLink');

    const urlParams = new URLSearchParams(window.location.search);
    const attachmentId = urlParams.get('id');
    if (!attachmentId) {
        showCustomAlert('Не указан ID вложения');
        window.location.href = 'attachments.html';
        return;
    }

    async function fetchJson(url) {
        try {
            const resp = await fetch(url, { credentials: 'include' });
            if (!resp.ok) {
                showCustomAlert(`Ошибка загрузки ${url}: ${resp.status}`);
                throw new Error(`Ошибка загрузки ${url}: ${resp.status}`);
            }
            return resp.json();
        } catch (error) {
            console.error(error);
            showCustomAlert('Ошибка загрузки данных. Попробуйте позже.');
            throw error;
        }
    }

    let attachment;
    try {
        attachment = await fetchJson(`/api/attachment/${attachmentId}`);
    } catch {
        showCustomAlert('Не удалось загрузить вложение');
        window.location.href = 'attachments.html';
        return;
    }

    let feedbackMessage = '';
    if (attachment.feedbackId) {
        try {
            const feedback = await fetchJson(`/api/feedback/${attachment.feedbackId}`);
            if (feedback.message) {
                feedbackMessage = feedback.message.length > 100 ? feedback.message.substring(0, 100) + '...' : feedback.message;
            }
        } catch {
            console.warn('Не удалось загрузить отзыв по ID');
        }
    }

    feedbackMessageElem.textContent = feedbackMessage || '(нет сообщения)';

    filePathElem.textContent = attachment.filePath;
    fileTypeElem.textContent = attachment.fileType;
    createdAtElem.textContent = new Date(attachment.createdAt).toLocaleString();

    fileLinkElem.href = attachment.filePath;
    fileLinkElem.textContent = attachment.filePath.split('/').pop();
});