document.addEventListener('DOMContentLoaded', () => {
    const pagination = {
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1
    };
    loadStatuses(pagination.pageNumber);

    document.querySelector('.pagination').addEventListener('click', (event) => {
        const target = event.target;
        if (!target.classList.contains('page-btn')) return;

        let newPage = Number(target.dataset.page);
        if (newPage >= 1 && newPage <= pagination.totalPages && newPage !== pagination.pageNumber) {
            pagination.pageNumber = newPage;
            loadStatuses(pagination.pageNumber);
        }
    });

    async function loadStatuses(pageNumber) {
        const statusListContainer = document.querySelector('.status-list');
        const paginationContainer = document.querySelector('.pagination');

        statusListContainer.innerHTML = '<p>Загрузка статусов...</p>';
        paginationContainer.innerHTML = '';

        try {
            const response = await fetch(`/api/status?pageNumber=${pageNumber}&pageSize=${pagination.pageSize}`, {
                method: 'GET',
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 401) {
                    window.location.href = '../../auth.html';
                    return;
                }
                throw new Error(`Ошибка загрузки статусов: ${response.status}`);
            }

            const responseData = await response.json();
            const statuses = responseData.data;

            paginationContainer.innerHTML = '';
            pagination.totalPages = responseData.totalPages;

            if (statuses.length === 0) {
                statusListContainer.innerHTML = '<p>Статусов пока нет.</p>';
                return;
            }

            statusListContainer.innerHTML = '';

            statuses.forEach(st => {
                const statusElem = document.createElement('div');
                statusElem.className = 'status-item';

                let buttonsHtml = `
                    <button class="btn-detail" data-id="${st.id}"><i class="fa fa-search"></i></button>
                    <button class="btn-edit" data-id="${st.id}"><i class="fa fa-pencil"></i></button>
                    <button class="btn-delete" data-id="${st.id}"><i class="fa fa-trash"></i></button>
                `;

                statusElem.innerHTML = `
                <div class="status-content">
                    <p><strong>Название:</strong> ${escapeHtml(st.name)}</p>
                </div>
                <div class="status-buttons">
                    ${buttonsHtml}
                </div>
                `;

                statusListContainer.appendChild(statusElem);
            });

            buildPagination(paginationContainer, responseData.pageNumber, responseData.totalPages);

        } catch (error) {
            statusListContainer.innerHTML = '<p>Ошибка загрузки статусов. Попробуйте позже.</p>';
            console.error(error);
        }
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

    const statusListContainer = document.querySelector('.status-list');

    statusListContainer.addEventListener('click', (event) => {
        let target = event.target;
        if (target.tagName !== 'BUTTON') {
            target = target.closest('button');
            if (!target) return;
        }

        const id = target.dataset.id;

        if (target.classList.contains('btn-detail')) {
            window.location.href = `status-details.html?id=${id}`;
        } else if (target.classList.contains('btn-edit')) {
            window.location.href = `status-edit.html?id=${id}`;
        } else if (target.classList.contains('btn-delete')) {
            if (confirm('Вы уверены, что хотите удалить этот статус?')) {
                deleteStatus(id);
            }
        }
    });

    async function deleteStatus(id) {
        try {
            const response = await fetch(`/api/status/${id}`, {
                method: 'DELETE',
                credentials: 'include'
            });
            if (response.ok) {
                const responseCheck = await fetch(`/api/status?pageNumber=${pagination.pageNumber}&pageSize=${pagination.pageSize}`, {
                    method: 'GET',
                    credentials: 'include'
                });

                if (!responseCheck.ok) {
                    alert('Ошибка при обновлении списка');
                    return;
                }

                const dataCheck = await responseCheck.json();
                const statuses = dataCheck.data;

                if (statuses.length === 0 && pagination.pageNumber > 1) {
                    pagination.pageNumber--;
                }

                loadStatuses(pagination.pageNumber);
            } else {
                alert(`Ошибка удаления статуса: ${response.status}`);
            }
        } catch (error) {
            alert('Ошибка при удалении статуса');
            console.error(error);
        }
    }
});
