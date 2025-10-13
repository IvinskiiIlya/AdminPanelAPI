document.addEventListener('DOMContentLoaded', async () => {
    const userNameElem = document.getElementById('userName');
    const userEmailElem = document.getElementById('userEmail');
    const categoryNameElem = document.getElementById('categoryName');
    const statusNameElem = document.getElementById('statusName');
    const createdAtElem = document.getElementById('createdAt');
    const messageElem = document.getElementById('message');
    const responsesListElem = document.getElementById('responsesList');
    const attachmentsListElem = document.getElementById('attachmentsList');

    async function fetchJson(url) {
        try {
            const resp = await fetch(url, {
                credentials: 'include'
            });
            if (!resp.ok) {
                alert(`Ошибка загрузки данных: ${resp.status}`);
                throw new Error(`Ошибка загрузки ${url}: ${resp.status}`);
            }
            return resp.json();
        } catch (error) {
            console.error(error);
            alert('Ошибка загрузки данных. Попробуйте позже.');
            throw error;
        }
    }

    async function fetchUserInfo(userId) {
        try {
            const user = await fetchJson(`/api/user/${userId}`);
            return user;
        } catch {
            return null;
        }
    }

    const urlParams = new URLSearchParams(window.location.search);
    const feedbackId = urlParams.get('id');
    if (!feedbackId) {
        alert('Не указан ID отзыва');
        window.location.href = 'feedbacks.html';
        return;
    }

    let feedback;
    try {
        feedback = await fetchJson(`/api/feedback/${feedbackId}`);
    } catch {
        alert('Не удалось загрузить отзыв');
        window.location.href = 'feedbacks.html';
        return;
    }

    let user = null;
    try {
        user = await fetchUserInfo(feedback.userId);
    } catch {
        userNameElem.textContent = 'Неизвестно';
        userEmailElem.textContent = '';
    }

    const [categoriesResp, statusesResp] = await Promise.all([
        fetchJson('/api/category?pageNumber=1&pageSize=100'),
        fetchJson('/api/status?pageNumber=1&pageSize=100')
    ]);
    const categories = categoriesResp.data || [];
    const statuses = statusesResp.data || [];

    const category = categories.find(c => c.id === feedback.categoryId);
    const status = statuses.find(s => s.id === feedback.statusId);

    userNameElem.textContent = user ? user.userName : 'Неизвестно';
    userEmailElem.textContent = user ? user.email : '';

    categoryNameElem.textContent = category ? category.name : 'Неизвестно';
    statusNameElem.textContent = status ? status.name : 'Неизвестно';
    createdAtElem.textContent = new Date(feedback.createdAt).toLocaleString();
    messageElem.textContent = feedback.message;

    let responses = [];
    try {
        responses = await fetchJson(`/api/response/by-feedback/${feedbackId}`);
    } catch {
        responsesListElem.innerHTML = '<li>Ошибка загрузки ответов</li>';
    }

    responsesListElem.innerHTML = '';
    if (responses.length === 0) {
        responsesListElem.innerHTML = '<li>Ответов нет</li>';
    } else {
        for (const resp of responses) {
            const li = document.createElement('li');
            const respUser = await fetchUserInfo(resp.userId);
            const userDisplay = respUser ? `${respUser.userName} (${respUser.email})` : resp.userId;
            li.textContent = `(${new Date(resp.createdAt).toLocaleString()}) ${resp.message} — от администратора ${userDisplay}`;
            responsesListElem.appendChild(li);
        }
    }

    let attachments = [];
    try {
        attachments = await fetchJson(`/api/attachment/feedback/${feedbackId}`);
    } catch {
        attachmentsListElem.innerHTML = '<li>Ошибка загрузки вложений</li>';
    }

    attachmentsListElem.innerHTML = '';
    if (attachments.length === 0) {
        attachmentsListElem.innerHTML = '<li>Вложений нет</li>';
    } else {
        for (const att of attachments) {
            const li = document.createElement('li');
            const link = document.createElement('a');
            link.href = att.filePath;
            link.target = '_blank';
            link.textContent = att.filePath.split('/').pop();
            li.appendChild(link);
            li.append(` (${att.fileType}, загружено: ${new Date(att.createdAt).toLocaleString()})`);
            attachmentsListElem.appendChild(li);
        }
    }
});
