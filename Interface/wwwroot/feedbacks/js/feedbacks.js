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
        categoryId: "",
        statusId: "",
        createdFrom: "",
        createdTo: "",
        sortColumn: "Id",
        sortOrder: "asc"
    };

    let currentUserInfo = null;

    const categoryNameMap = {
        errors: 'Ошибки',
        suggestions: 'Предложения',
        questions: 'Вопросы',
        complaints: 'Жалобы',
        docs: 'Документация',
        security: 'Безопасность',
        performance: 'Производительность',
        ui: 'Интерфейс',
        integration: 'Интеграция',
        other: 'Прочее'
    };

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

        await loadCategories();
        await loadStatuses();

        loadPaginationFromUrlOrStorage();

        loadFeedbacks(pagination.pageNumber);
    }

    async function loadCategories() {
        try {
            const response = await fetch('/api/category', { method: 'GET', credentials: 'include' });
            if (!response.ok) throw new Error('Ошибка загрузки категорий');
            const data = await response.json();
            const categories = data.data || data;

            const categorySelect = document.getElementById('categoryId');
            if (categorySelect && categories) {
                categories.forEach(cat => {
                    const option = document.createElement('option');
                    option.value = cat.id;
                    option.textContent = categoryNameMap[cat.name] || cat.name || `Категория ${cat.id}`;
                    categorySelect.appendChild(option);
                });
            }
        } catch (error) {
            console.error(error);
        }
    }

    async function loadStatuses() {
        try {
            const response = await fetch('/api/status', { method: 'GET', credentials: 'include' });
            if (!response.ok) throw new Error('Ошибка загрузки статусов');
            const data = await response.json();
            const statuses = data.data || data;

            const statusSelect = document.getElementById('statusId');
            if (statusSelect && statuses) {
                statuses.forEach(st => {
                    const option = document.createElement('option');
                    option.value = st.id;
                    option.textContent = st.name || `Статус ${st.id}`;
                    statusSelect.appendChild(option);
                });
            }
        } catch (error) {
            console.error(error);
        }
    }

    function updateFilterUI() {
        document.getElementById('searchTerm').value = pagination.searchTerm;
        document.getElementById('categoryId').value = pagination.categoryId;
        document.getElementById('statusId').value = pagination.statusId;
        document.getElementById('createdFrom').value = pagination.createdFrom;
        document.getElementById('createdTo').value = pagination.createdTo;
        document.getElementById('sortColumn').value = pagination.sortColumn;
        document.getElementById('sortOrder').value = pagination.sortOrder;
    }

    function loadPaginationFromUrlOrStorage() {
        const params = new URLSearchParams(window.location.search);
        const stored = sessionStorage.getItem('paginationFilters');
        if (stored) {
            const storedPagination = JSON.parse(stored);
            Object.assign(pagination, storedPagination);
        } else {
            pagination.pageNumber = Number(params.get('pageNumber')) || 1;
            pagination.searchTerm = params.get('searchTerm') || "";
            pagination.categoryId = params.get('categoryId') || "";
            pagination.statusId = params.get('statusId') || "";
            pagination.createdFrom = params.get('createdFrom') || "";
            pagination.createdTo = params.get('createdTo') || "";
            pagination.sortColumn = params.get('sortColumn') || "Id";
            pagination.sortOrder = params.get('sortOrder') || "asc";

            sessionStorage.setItem('paginationFilters', JSON.stringify(pagination));
        }

        updateFilterUI();
    }

    function updateUrlFromPagination() {
        const params = new URLSearchParams();
        
        params.set('pageNumber', pagination.pageNumber);
        if (pagination.searchTerm) params.set('searchTerm', pagination.searchTerm);
        if (pagination.categoryId) params.set('categoryId', pagination.categoryId);
        if (pagination.statusId) params.set('statusId', pagination.statusId);
        if (pagination.createdFrom) params.set('createdFrom', pagination.createdFrom);
        if (pagination.createdTo) params.set('createdTo', pagination.createdTo);
        params.set('sortColumn', pagination.sortColumn);
        params.set('sortOrder', pagination.sortOrder);
        
        const newUrl = `${window.location.pathname}?${params.toString()}`;
        history.pushState({...pagination}, "", newUrl);
        sessionStorage.setItem('paginationFilters', JSON.stringify(pagination));
    }

    document.querySelector('.pagination').addEventListener('click', (event) => {
        const target = event.target;
        if (!target.classList.contains('page-btn')) return;

        const newPage = Number(target.dataset.page);
        if (newPage >= 1 && newPage <= pagination.totalPages && newPage !== pagination.pageNumber) {
            pagination.pageNumber = newPage;
            updateUrlFromPagination();
            loadFeedbacks(pagination.pageNumber);
        }
    });

    document.getElementById('filterBtn').addEventListener('click', () => {
        pagination.searchTerm = document.getElementById('searchTerm').value.trim();
        pagination.categoryId = document.getElementById('categoryId').value;
        pagination.statusId = document.getElementById('statusId').value;
        pagination.createdFrom = document.getElementById('createdFrom').value;
        pagination.createdTo = document.getElementById('createdTo').value;
        pagination.sortColumn = document.getElementById('sortColumn').value;
        pagination.sortOrder = document.getElementById('sortOrder').value;
        pagination.pageNumber = 1;

        updateUrlFromPagination();
        loadFeedbacks(pagination.pageNumber);
    });

    document.getElementById('resetBtn').addEventListener('click', () => {
        pagination.searchTerm = "";
        pagination.categoryId = "";
        pagination.statusId = "";
        pagination.createdFrom = "";
        pagination.createdTo = "";
        pagination.sortColumn = "Id";
        pagination.sortOrder = "asc";
        pagination.pageNumber = 1;

        updateFilterUI();
        updateUrlFromPagination();
        loadFeedbacks(pagination.pageNumber);
        sessionStorage.removeItem('paginationFilters');
    });

    async function loadFeedbacks(pageNumber) {
        const feedbackListContainer = document.querySelector('.feedback-list');
        const paginationContainer = document.querySelector('.pagination');

        feedbackListContainer.innerHTML = '<p>Загрузка отзывов...</p>';
        paginationContainer.innerHTML = '';

        try {
            const params = new URLSearchParams({
                pageNumber,
                pageSize: pagination.pageSize,
                searchTerm: pagination.searchTerm || "",
                categoryId: pagination.categoryId || "",
                statusId: pagination.statusId || "",
                createdFrom: pagination.createdFrom || "",
                createdTo: pagination.createdTo || "",
                sortColumn: pagination.sortColumn || "Id",
                sortOrder: pagination.sortOrder || "asc"
            });

            const response = await fetch(`/api/feedback?${params.toString()}`, {
                method: 'GET',
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 401) {
                    sessionStorage.setItem('pendingAlert', 'Сессия истекла, требуется повторная авторизация');
                    sessionStorage.removeItem('paginationFilters'); 
                    window.location.href = '../../auth.html';
                    return;
                }
                throw new Error(`Ошибка загрузки отзывов: ${response.status}`);
            }

            const responseData = await response.json();
            const feedbacks = responseData.data;

            pagination.totalPages = responseData.totalPages;

            if (feedbacks.length === 0) {
                feedbackListContainer.innerHTML = '<p>Отзывов пока нет.</p>';
                return;
            }

            feedbackListContainer.innerHTML = '';

            const isAdmin = currentUserInfo.roles && currentUserInfo.roles.includes('Администратор');
            const currentUserId = currentUserInfo.userId || null;

            feedbacks.forEach(fb => {
                const feedbackElem = document.createElement('div');
                feedbackElem.className = 'feedback-item';

                let buttonsHtml = `<button class="btn-detail" data-id="${fb.id}"><i class="fa fa-search"></i></button>`;
                if (isAdmin || fb.userId === currentUserId) {
                    buttonsHtml += `
                    <button class="btn-edit" data-id="${fb.id}"><i class="fa fa-pencil"></i></button>
                    <button class="btn-delete" data-id="${fb.id}"><i class="fa fa-trash"></i></button>`;
                }

                feedbackElem.innerHTML = `
                <div class="feedback-content">
                    <p><strong>Отзыв:</strong> ${escapeHtml(fb.message)}</p>
                    <p><small>Дата: ${new Date(fb.createdAt).toLocaleString()}</small></p>
                </div>
                <div class="feedback-buttons">
                    ${buttonsHtml}
                </div>
                `;

                feedbackListContainer.appendChild(feedbackElem);
            });

            buildPagination(paginationContainer, responseData.pageNumber, responseData.totalPages);
        } catch (error) {
            feedbackListContainer.innerHTML = '<p>Ошибка загрузки отзывов. Попробуйте позже.</p>';
            console.error(error);
        }
    }

    async function getUserInfo() {
        const response = await fetch('/api/auth/userinfo', {
            method: 'GET',
            credentials: 'include'
        });
        if (!response.ok) {
            sessionStorage.removeItem('paginationFilters');
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

    const feedbackListContainer = document.querySelector('.feedback-list');

    feedbackListContainer.addEventListener('click', (event) => {
        let target = event.target;
        if (target.tagName !== 'BUTTON') {
            target = target.closest('button');
            if (!target) return;
        }

        const id = target.dataset.id;
        if (target.classList.contains('btn-detail')) {
            window.location.href = `feedback-details.html?id=${id}`;
        } else if (target.classList.contains('btn-edit')) {
            window.location.href = `feedback-edit.html?id=${id}`;
        } else if (target.classList.contains('btn-delete')) {
            if (confirm('Удалить этот отзыв?')) {
                deleteFeedback(id);
            }
        }
    });

    async function deleteFeedback(id) {
        try {
            const response = await fetch(`/api/feedback/${id}`, {
                method: 'DELETE',
                credentials: 'include'
            });
            if (!response.ok) {
                showCustomAlert('Ошибка при удалении отзыва');
                return;
            }
            showCustomAlert('Отзыв успешно удалён');
            loadFeedbacks(1);
        } catch (error) {
            showCustomAlert('Не удалось удалить отзыв.');
            console.error(error);
        }
    }

    await init();

    window.onpopstate = (event) => {
        if (event.state) {
            Object.assign(pagination, event.state);
            updateFilterUI();
            loadFeedbacks(pagination.pageNumber);
            sessionStorage.setItem('paginationFilters', JSON.stringify(pagination));
        } else {
            loadPaginationFromUrlOrStorage();
            loadFeedbacks(pagination.pageNumber);
        }
    };
});
