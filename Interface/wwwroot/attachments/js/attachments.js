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

    const pagination = {
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1,
        searchTerm: "",
        sortColumn: "Id",
        sortOrder: "asc"
    };

    let currentUserInfo = null;

    function updateFilterUI() {
        document.getElementById('searchTerm').value = pagination.searchTerm;
        document.getElementById('sortColumn').value = pagination.sortColumn;
        document.getElementById('sortOrder').value = pagination.sortOrder;
    }

    function loadPaginationFromUrlOrStorage() {
        const params = new URLSearchParams(window.location.search);
        const stored = sessionStorage.getItem('attachmentPaginationFilters');
        if (stored) {
            const storedPagination = JSON.parse(stored);
            Object.assign(pagination, storedPagination);
        } else {
            pagination.pageNumber = Number(params.get('page')) || 1;
            pagination.searchTerm = params.get('search') || "";
            pagination.sortColumn = params.get('sortColumn') || "Id";
            pagination.sortOrder = params.get('sortOrder') || "asc";

            sessionStorage.setItem('attachmentPaginationFilters', JSON.stringify(pagination));
        }

        updateFilterUI();
    }

    function updateUrlFromPagination() {
        const params = new URLSearchParams();

        params.set('page', pagination.pageNumber);
        if (pagination.searchTerm) params.set('search', pagination.searchTerm);
        params.set('sortColumn', pagination.sortColumn);
        params.set('sortOrder', pagination.sortOrder);

        const newUrl = `${window.location.pathname}?${params.toString()}`;
        history.pushState({...pagination}, '', newUrl);
        sessionStorage.setItem('attachmentPaginationFilters', JSON.stringify(pagination));
    }

    async function init() {
        currentUserInfo = await getUserInfo();
        if (!currentUserInfo) {
            sessionStorage.setItem('pendingAlert', 'Требуется авторизация, перенаправление на страницу входа');
            window.location.href = '../../auth.html';
            return;
        }

        const isAdmin = currentUserInfo.roles && currentUserInfo.roles.includes('Администратор');

        const menu = document.querySelector('nav ul');
        const links = menu.querySelectorAll('li');

        if (!isAdmin) {
            links.forEach(li => {
                const a = li.querySelector('a');
                if (a) {
                    const text = a.textContent.trim();
                    if (text !== 'Вложения' && text !== 'Отзывы') {
                        li.style.display = 'none';
                    }
                }
            });
        }

        const currentPath = window.location.pathname.split('/').pop();
        links.forEach(li => {
            const a = li.querySelector('a');
            if (a) {
                const href = a.getAttribute('href');
                if (href === currentPath || href.endsWith(currentPath)) {
                    a.classList.add('active');
                } else {
                    a.classList.remove('active');
                }
            }
        });

        loadPaginationFromUrlOrStorage();

        loadAttachments(pagination.pageNumber);
    }

    document.querySelector('.pagination').addEventListener('click', (event) => {
        const target = event.target;
        if (!target.classList.contains('page-btn')) return;

        const newPage = Number(target.dataset.page);
        if (newPage >= 1 && newPage <= pagination.totalPages && newPage !== pagination.pageNumber) {
            pagination.pageNumber = newPage;
            updateUrlFromPagination();
            loadAttachments(pagination.pageNumber);
        }
    });

    document.getElementById('filterBtn').addEventListener('click', () => {
        pagination.searchTerm = document.getElementById('searchTerm').value.trim();
        pagination.sortColumn = document.getElementById('sortColumn').value;
        pagination.sortOrder = document.getElementById('sortOrder').value;
        pagination.pageNumber = 1;

        updateUrlFromPagination();
        loadAttachments(pagination.pageNumber);
    });

    document.getElementById('resetBtn').addEventListener('click', () => {
        pagination.searchTerm = "";
        pagination.sortColumn = "Id";
        pagination.sortOrder = "asc";
        pagination.pageNumber = 1;

        updateFilterUI();
        updateUrlFromPagination();
        loadAttachments(pagination.pageNumber);
        sessionStorage.removeItem('attachmentPaginationFilters');
    });

    async function loadAttachments(pageNumber) {
        const attachmentListContainer = document.querySelector('.attachment-list');
        const paginationContainer = document.querySelector('.pagination');

        attachmentListContainer.innerHTML = '<p>Загрузка вложений...</p>';
        paginationContainer.innerHTML = '';

        try {
            const params = new URLSearchParams({
                pageNumber,
                pageSize: pagination.pageSize,
                searchTerm: pagination.searchTerm || "",
                sortColumn: pagination.sortColumn || "Id",
                sortOrder: pagination.sortOrder || "asc"
            });

            const response = await fetch(`/api/attachment?${params.toString()}`, {
                method: 'GET',
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 401) {
                    sessionStorage.setItem('pendingAlert', 'Сессия истекла, требуется повторная авторизация');
                    sessionStorage.removeItem('attachmentPaginationFilters');
                    window.location.href = '../../auth.html';
                    return;
                }
                throw new Error(`Ошибка загрузки вложений: ${response.status}`);
            }

            const responseData = await response.json();
            const attachments = responseData.data;

            pagination.totalPages = responseData.totalPages;

            if (attachments.length === 0) {
                attachmentListContainer.innerHTML = '<p>Вложений пока нет.</p>';
                return;
            }

            attachmentListContainer.innerHTML = '';

            const isAdmin = currentUserInfo.roles && currentUserInfo.roles.includes('Администратор');
            const currentUserId = currentUserInfo.userId || null;

            async function getFeedbackPreview(feedbackId) {
                if (!feedbackId) return '';
                try {
                    const resp = await fetch(`/api/feedback/${feedbackId}`, { credentials: 'include' });
                    if (!resp.ok) throw new Error(`Ошибка загрузки отзыва: ${resp.status}`);
                    const feedback = await resp.json();
                    if (feedback.message) {
                        return feedback.message.length > 100 ? feedback.message.substring(0, 100) + '...' : feedback.message;
                    } else {
                        return '';
                    }
                } catch {
                    return '';
                }
            }

            for (const att of attachments) {
                const feedbackPreview = await getFeedbackPreview(att.feedbackId);

                const attachmentElem = document.createElement('div');
                attachmentElem.className = 'attachment-item';

                let buttonsHtml = `<button class="btn-detail" data-id="${att.id}"><i class="fa fa-search"></i></button>`;
                if (isAdmin || att.userId === currentUserId) {
                    buttonsHtml += `
                        <button class="btn-edit" data-id="${att.id}"><i class="fa fa-pencil"></i></button>
                        <button class="btn-delete" data-id="${att.id}"><i class="fa fa-trash"></i></button>
                    `;
                }

                attachmentElem.innerHTML = `
                <div class="attachment-content">
                    <p><strong>Файл:</strong> <a href="${escapeHtml(att.filePath)}" target="_blank" class="file-link" data-filepath="${escapeHtml(att.filePath)}">${escapeHtml(att.filePath.split('/').pop())}</a></p>
                    <p><small>Тип: ${escapeHtml(att.fileType)}</small></p>
                    <p><small>Дата: ${new Date(att.createdAt).toLocaleString()}</small></p>
                    <p class="feedback-preview">${escapeHtml(feedbackPreview) || '(нет)'}</p>
                </div>
                <div class="attachment-buttons">${buttonsHtml}</div>
                `;

                attachmentListContainer.appendChild(attachmentElem);
            }

            buildPagination(paginationContainer, responseData.pageNumber, responseData.totalPages);

        } catch (error) {
            attachmentListContainer.innerHTML = '<p>Ошибка загрузки вложений. Попробуйте позже.</p>';
            console.error(error);
        }
    }

    async function getUserInfo() {
        const response = await fetch('/api/auth/userinfo', {
            method: 'GET',
            credentials: 'include'
        });
        if (!response.ok) {
            sessionStorage.removeItem('attachmentPaginationFilters');
            return null;
        }

        const data = await response.json();
        return { userId: data.id || data.userId || null, name: data.name, roles: data.roles };
    }

    function buildPagination(container, currentPage, totalPages) {
        container.innerHTML = '';

        const prevBtn = document.createElement('button');
        prevBtn.textContent = 'Назад';
        prevBtn.className = 'page-btn';
        prevBtn.disabled = currentPage === 1;
        prevBtn.dataset.page = currentPage - 1;
        container.appendChild(prevBtn);

        let startPage = Math.max(1, currentPage - 2);
        let endPage = Math.min(totalPages, startPage + 4);
        if (endPage - startPage < 4) {
            startPage = Math.max(1, endPage - 4);
        }
        for (let i = startPage; i <= endPage; i++) {
            const pageBtn = document.createElement('button');
            pageBtn.textContent = i;
            pageBtn.className = 'page-btn';
            if (i === currentPage) {
                pageBtn.disabled = true;
                pageBtn.style.fontWeight = 'bold';
            }
            pageBtn.dataset.page = i;
            container.appendChild(pageBtn);
        }

        const nextBtn = document.createElement('button');
        nextBtn.textContent = 'Вперед';
        nextBtn.className = 'page-btn';
        nextBtn.disabled = currentPage === totalPages;
        nextBtn.dataset.page = currentPage + 1;
        container.appendChild(nextBtn);
    }

    function escapeHtml(text) {
        if (!text) return '';
        return text.replace(/[&<>"']/g, function (m) {
            switch (m) {
                case '&': return '&amp;';
                case '<': return '&lt;';
                case '>': return '&gt;';
                case '"': return '&quot;';
                case "'": return '&#39;';
                default: return m;
            }
        });
    }

    const attachmentListContainer = document.querySelector('.attachment-list');

    attachmentListContainer.addEventListener('click', async (event) => {
        let target = event.target;

        if (target.classList.contains('file-link')) {
            event.preventDefault();
            const filePath = target.dataset.filepath;
            window.open(filePath, '_blank');
            return;
        }

        if (target.tagName !== 'BUTTON') {
            target = target.closest('button');
            if (!target) return;
        }

        const id = target.dataset.id;
        if (target.classList.contains('btn-detail')) {
            window.location.href = `attachment-details.html?id=${id}`;
        } else if (target.classList.contains('btn-edit')) {
            window.location.href = `attachment-edit.html?id=${id}`;
        } else if (target.classList.contains('btn-delete')) {
            if (confirm('Вы уверены, что хотите удалить вложение?')) {
                await deleteAttachment(id);
            }
        }
    });

    async function deleteAttachment(id) {
        try {
            const response = await fetch(`/api/attachment/${id}`, {
                method: 'DELETE',
                credentials: 'include'
            });
            if (!response.ok) {
                showCustomAlert('Ошибка при удалении вложения');
                return;
            }
            showCustomAlert('Вложение успешно удалено');
            loadAttachments(pagination.pageNumber);
        } catch (error) {
            console.error(error);
            showCustomAlert('Ошибка при удалении вложения. Попробуйте позже.');
        }
    }

    await init();

    window.onpopstate = (event) => {
        if (event.state) {
            Object.assign(pagination, event.state);
            updateFilterUI();
            loadAttachments(pagination.pageNumber);
            sessionStorage.setItem('attachmentPaginationFilters', JSON.stringify(pagination));
        } else {
            loadPaginationFromUrlOrStorage();
            loadAttachments(pagination.pageNumber);
        }
    };
});