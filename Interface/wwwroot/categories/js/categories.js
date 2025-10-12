document.addEventListener('DOMContentLoaded', () => {
    const pagination = {
        pageNumber: 1,
        pageSize: 10,
        totalPages: 1
    };
    loadCategories(pagination.pageNumber);

    document.querySelector('.pagination').addEventListener('click', (event) => {
        const target = event.target;
        if (!target.classList.contains('page-btn')) return;

        let newPage = Number(target.dataset.page);
        if (newPage >= 1 && newPage <= pagination.totalPages && newPage !== pagination.pageNumber) {
            pagination.pageNumber = newPage;
            loadCategories(pagination.pageNumber);
        }
    });

    async function loadCategories(pageNumber) {
        const categoryListContainer = document.querySelector('.category-list');
        const paginationContainer = document.querySelector('.pagination');

        categoryListContainer.innerHTML = '<p>Загрузка категорий...</p>';
        paginationContainer.innerHTML = '';

        try {
            const response = await fetch(`/api/category?pageNumber=${pageNumber}&pageSize=${pagination.pageSize}`, {
                method: 'GET',
                credentials: 'include'
            });

            if (!response.ok) {
                if (response.status === 401) {
                    window.location.href = '../../auth.html';
                    return;
                }
                throw new Error(`Ошибка загрузки категорий: ${response.status}`);
            }

            const responseData = await response.json();
            const categories = responseData.data;

            paginationContainer.innerHTML = '';
            pagination.totalPages = responseData.totalPages;

            if (categories.length === 0) {
                categoryListContainer.innerHTML = '<p>Категорий пока нет.</p>';
                return;
            }

            categoryListContainer.innerHTML = '';

            categories.forEach(cat => {
                const categoryElem = document.createElement('div');
                categoryElem.className = 'category-item';

                let buttonsHtml = `
                    <button class="btn-detail" data-id="${cat.id}"><i class="fa fa-search"></i></button>
                    <button class="btn-edit" data-id="${cat.id}"><i class="fa fa-pencil"></i></button>
                    <button class="btn-delete" data-id="${cat.id}"><i class="fa fa-trash"></i></button>
                `;

                categoryElem.innerHTML = `
                <div class="category-content">
                    <p><strong>Название:</strong> ${escapeHtml(cat.name)}</p>
                    <p><small>${cat.description ? escapeHtml(cat.description) : ''}</small></p>
                </div>
                <div class="category-buttons">
                    ${buttonsHtml}
                </div>
                `;

                categoryListContainer.appendChild(categoryElem);
            });

            buildPagination(paginationContainer, responseData.pageNumber, responseData.totalPages);

        } catch (error) {
            categoryListContainer.innerHTML = '<p>Ошибка загрузки категорий. Попробуйте позже.</p>';
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

    const categoryListContainer = document.querySelector('.category-list');

    categoryListContainer.addEventListener('click', (event) => {
        let target = event.target;
        if (target.tagName !== 'BUTTON') {
            target = target.closest('button');
            if (!target) return;
        }

        const id = target.dataset.id;

        if (target.classList.contains('btn-detail')) {
            window.location.href = `category-details.html?id=${id}`;
        } else if (target.classList.contains('btn-edit')) {
            window.location.href = `category-edit.html?id=${id}`; 
        } else if (target.classList.contains('btn-delete')) {
            if (confirm('Вы уверены, что хотите удалить эту категорию?')) {
                deleteCategory(id);
            }
        }
    });

    async function deleteCategory(id) {
        try {
            const response = await fetch(`/api/category/${id}`, {
                method: 'DELETE',
                credentials: 'include'
            });
            if (response.ok) {
                const responseCheck = await fetch(`/api/category?pageNumber=${pagination.pageNumber}&pageSize=${pagination.pageSize}`, {
                    method: 'GET',
                    credentials: 'include'
                });

                if (!responseCheck.ok) {
                    alert('Ошибка при обновлении списка');
                    return;
                }

                const dataCheck = await responseCheck.json();
                const categories = dataCheck.data;

                if (categories.length === 0 && pagination.pageNumber > 1) {
                    pagination.pageNumber--;
                }

                loadCategories(pagination.pageNumber);
            } else {
                alert(`Ошибка удаления категории: ${response.status}`);
            }
        } catch (error) {
            alert('Ошибка при удалении категории');
            console.error(error);
        }
    }
});
