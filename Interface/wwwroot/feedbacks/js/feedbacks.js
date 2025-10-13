document.addEventListener('DOMContentLoaded', async () => {
    const pagination = {
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1
    };

    let currentUserInfo = null;

    async function init() {
        currentUserInfo = await getUserInfo();
        if (!currentUserInfo) {
            alert('Требуется авторизация, перенаправление на страницу входа');
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

        loadFeedbacks(pagination.pageNumber);
    }

    document.querySelector('.pagination').addEventListener('click', (event) => {
        const target = event.target;
        if (!target.classList.contains('page-btn')) return;

        let newPage = Number(target.dataset.page);
        if (newPage >= 1 && newPage <= pagination.totalPages && newPage !== pagination.pageNumber) {
            pagination.pageNumber = newPage;
            loadFeedbacks(pagination.pageNumber);
        }
    });

    async function loadFeedbacks(pageNumber) {
        const feedbackListContainer = document.querySelector('.feedback-list');
        const paginationContainer = document.querySelector('.pagination');

        feedbackListContainer.innerHTML = '<p>Загрузка отзывов...</p>';
        paginationContainer.innerHTML = '';

        try {
            const response = await fetch(`/api/feedback?pageNumber=${pageNumber}&pageSize=${pagination.pageSize}`, {
                method: 'GET',
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 401) {
                    alert('Сессия истекла, требуется повторная авторизация');
                    window.location.href = '../../auth.html';
                    return;
                }
                throw new Error(`Ошибка загрузки отзывов: ${response.status}`);
            }

            const responseData = await response.json();
            const feedbacks = responseData.data;

            paginationContainer.innerHTML = '';
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
                alert('Ошибка при удалении отзыва');
                return;
            }
            alert('Отзыв успешно удалён');
            loadFeedbacks(1);
        } catch (error) {
            alert('Не удалось удалить отзыв.');
            console.error(error);
        }
    }

    const feedbackForm = document.getElementById('feedbackForm');
    if (feedbackForm) {
        feedbackForm.addEventListener('submit', async (event) => {
            event.preventDefault();
            const formMessage = document.getElementById('formMessage');
            formMessage.textContent = '';

            const userInfo = await getUserInfo();
            if (!userInfo) {
                alert('Требуется авторизация, перенаправление на страницу входа');
                window.location.href = '../../auth.html';
                return;
            }

            const categoryId = document.getElementById('category').value;
            const statusId = document.getElementById('status').value;
            const message = document.getElementById('message').value.trim();

            if (!categoryId || !statusId || message.length < 10) {
                alert('Пожалуйста, заполните все поля корректно.');
                return;
            }

            const dto = {
                userId: userInfo.userId,
                categoryId: Number(categoryId),
                statusId: Number(statusId),
                message: message
            };

            try {
                const response = await fetch('/api/feedback', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    credentials: 'include',
                    body: JSON.stringify(dto)
                });

                if (response.status === 201) {
                    alert('Отзыв успешно создан');
                    window.location.href = 'feedbacks.html';
                } else if (response.status === 400) {
                    const errorData = await response.json();
                    alert('Ошибка: ' + (errorData.detail || 'Некорректные данные'));
                } else if (response.status === 401) {
                    alert('Сессия истекла, требуется повторная авторизация');
                    window.location.href = '../../auth.html';
                } else {
                    alert('Ошибка создания отзыва.');
                }
            } catch (error) {
                alert('Ошибка сервера. Попробуйте позже.');
                console.error(error);
            }
        });
    }

    init();
});
