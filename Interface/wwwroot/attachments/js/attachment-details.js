document.addEventListener('DOMContentLoaded', async () => {
    const feedbackMessageElem = document.getElementById('feedbackMessagePreview');
    const filePathElem = document.getElementById('filePath');
    const fileTypeElem = document.getElementById('fileType');
    const createdAtElem = document.getElementById('createdAt');
    const fileLinkElem = document.getElementById('fileLink');

    const urlParams = new URLSearchParams(window.location.search);
    const attachmentId = urlParams.get('id');
    if (!attachmentId) {
        alert('Не указан ID вложения');
        window.location.href = 'attachments.html';
        return;
    }

    async function fetchJson(url) {
        try {
            const resp = await fetch(url, { credentials: 'include' });
            if (!resp.ok) {
                alert(`Ошибка загрузки ${url}: ${resp.status}`);
                throw new Error(`Ошибка загрузки ${url}: ${resp.status}`);
            }
            return resp.json();
        } catch (error) {
            console.error(error);
            alert('Ошибка загрузки данных. Попробуйте позже.');
            throw error;
        }
    }

    let attachment;
    try {
        attachment = await fetchJson(`/api/attachment/${attachmentId}`);
    } catch {
        alert('Не удалось загрузить вложение');
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
