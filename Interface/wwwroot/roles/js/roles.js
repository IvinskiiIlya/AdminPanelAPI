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

        loadRoles(pagination.pageNumber);
    }

    document.querySelector('.pagination').addEventListener('click', (event) => {
        const target = event.target;
        if (!target.classList.contains('page-btn')) return;

        let newPage = Number(target.dataset.page);
        if (newPage >= 1 && newPage <= pagination.totalPages && newPage !== pagination.pageNumber) {
            pagination.pageNumber = newPage;
            loadRoles(pagination.pageNumber);
        }
    });

    async function loadRoles(pageNumber) {
        const roleListContainer = document.querySelector('.role-list');
        const paginationContainer = document.querySelector('.pagination');

        roleListContainer.innerHTML = '<p>Загрузка ролей...</p>';
        paginationContainer.innerHTML = '';

        try {
            const response = await fetch(`/api/role?pageNumber=${pageNumber}&pageSize=${pagination.pageSize}`, {
                method: 'GET',
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 401) {
                    alert('Сессия истекла, требуется повторная авторизация');
                    window.location.href = '../../auth.html';
                    return;
                }
                throw new Error(`Ошибка загрузки ролей: ${response.status}`);
            }

            const responseData = await response.json();
            const roles = responseData.data;

            paginationContainer.innerHTML = '';
            pagination.totalPages = responseData.totalPages;

            if (roles.length === 0) {
                roleListContainer.innerHTML = '<p>Ролей пока нет.</p>';
                return;
            }

            roleListContainer.innerHTML = '';

            const isAdmin = currentUserInfo.roles && currentUserInfo.roles.includes('Администратор');

            roles.forEach(role => {
                const roleElem = document.createElement('div');
                roleElem.className = 'role-item';

                let buttonsHtml = `<button class="btn-detail" data-id="${role.id}"><i class="fa fa-search"></i></button>`;

                if (isAdmin) {
                    buttonsHtml += `
                        <button class="btn-edit" data-id="${role.id}"><i class="fa fa-pencil"></i></button>
                        <button class="btn-delete" data-id="${role.id}"><i class="fa fa-trash"></i></button>
                    `;
                }

                roleElem.innerHTML = `
                <div class="role-content">
                    <p><strong>Название:</strong> ${escapeHtml(role.name)}</p>
                </div>
                <div class="role-buttons">
                    ${buttonsHtml}
                </div>
                `;

                roleListContainer.appendChild(roleElem);
            });

            buildPagination(paginationContainer, responseData.pageNumber, responseData.totalPages);

        } catch (error) {
            roleListContainer.innerHTML = '<p>Ошибка загрузки ролей. Попробуйте позже.</p>';
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

    const roleListContainer = document.querySelector('.role-list');

    roleListContainer.addEventListener('click', (event) => {
        let target = event.target;
        if (target.tagName !== 'BUTTON') {
            target = target.closest('button');
            if (!target) return;
        }

        const id = target.dataset.id;

        if (target.classList.contains('btn-detail')) {
            window.location.href = `role-details.html?id=${id}`;
        } else if (target.classList.contains('btn-edit')) {
            window.location.href = `role-edit.html?id=${id}`;
        } else if (target.classList.contains('btn-delete')) {
            if (confirm('Вы уверены, что хотите удалить эту роль?')) {
                deleteRole(id);
            }
        }
    });

    async function deleteRole(id) {
        try {
            const response = await fetch(`/api/role/${id}`, {
                method: 'DELETE',
                credentials: 'include'
            });
            if (response.ok) {
                alert('Роль успешно удалена');
                const responseCheck = await fetch(`/api/role?pageNumber=${pagination.pageNumber}&pageSize=${pagination.pageSize}`, {
                    method: 'GET',
                    credentials: 'include'
                });

                if (!responseCheck.ok) {
                    alert('Ошибка при обновлении списка');
                    return;
                }

                const dataCheck = await responseCheck.json();
                const roles = dataCheck.data;

                if (roles.length === 0 && pagination.pageNumber > 1) {
                    pagination.pageNumber--;
                }

                loadRoles(pagination.pageNumber);
            } else {
                alert(`Ошибка удаления роли: ${response.status}`);
            }
        } catch (error) {
            alert('Ошибка при удалении роли');
            console.error(error);
        }
    }

    init();
});
