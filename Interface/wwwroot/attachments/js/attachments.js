document.addEventListener('DOMContentLoaded', async () => {
    const pagination = {
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1,
        searchTerm: "",
        sortColumn: "Id",
        sortOrder: "asc"
    };

    const userInfo = await getUserInfo();
    if (!userInfo) {
        alert('Требуется авторизация, перенаправление на страницу входа');
        window.location.href = '../../auth.html';
        return;
    }

    const isAdmin = userInfo.roles && userInfo.roles.includes('Администратор');

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

    loadAttachments(pagination.pageNumber);

    document.querySelector('.pagination').addEventListener('click', (event) => {
        const target = event.target;
        if (!target.classList.contains('page-btn')) return;

        let newPage = Number(target.dataset.page);
        if (newPage >= 1 && newPage <= pagination.totalPages && newPage !== pagination.pageNumber) {
            pagination.pageNumber = newPage;
            loadAttachments(pagination.pageNumber);
        }
    });

    document.getElementById('filterBtn').addEventListener('click', () => {
        const searchTerm = document.getElementById('searchTerm').value.trim();
        const sortColumn = document.getElementById('sortColumn').value;
        const sortOrder = document.getElementById('sortOrder').value;

        pagination.searchTerm = searchTerm;
        pagination.sortColumn = sortColumn;
        pagination.sortOrder = sortOrder;
        pagination.pageNumber = 1;

        loadAttachments(pagination.pageNumber);
    });

    async function loadAttachments(pageNumber) {
        const attachmentListContainer = document.querySelector('.attachment-list');
        const paginationContainer = document.querySelector('.pagination');

        const currentUserId = userInfo.userId || null;

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
                    alert('Сессия истекла, требуется повторная авторизация');
                    window.location.href = '../../auth.html';
                    return;
                }
                alert(`Ошибка загрузки вложений: ${response.status}`);
                throw new Error(`Ошибка загрузки вложений: ${response.status}`);
            }

            const responseData = await response.json();
            const attachments = responseData.data;

            if (attachments.length === 0 && pageNumber > 1) {
                pagination.pageNumber = pageNumber - 1;
                return loadAttachments(pagination.pageNumber);
            }

            paginationContainer.innerHTML = '';
            pagination.totalPages = responseData.totalPages;

            if (attachments.length === 0) {
                attachmentListContainer.innerHTML = '<p>Вложений пока нет.</p>';
                return;
            }

            attachmentListContainer.innerHTML = '';

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

                const isAdmin = userInfo.roles && userInfo.roles.includes('Администратор');
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
        if (!response.ok) return null;

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
                alert('Ошибка при удалении вложения');
                return;
            }
            alert('Вложение успешно удалено');
            loadAttachments(pagination.pageNumber);
        } catch (error) {
            console.error(error);
            alert('Ошибка при удалении вложения. Попробуйте позже.');
        }
    }
});
