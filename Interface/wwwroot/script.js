document.addEventListener('DOMContentLoaded', () => {
    handleLoginForm();
    handleMainPageRoleControl();

    const logoutBtn = document.getElementById('logout-button');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', async () => {
            await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include'
            });
            window.location.href = 'auth.html';
        });
    }
});

async function handleLoginForm() {
    const loginForm = document.getElementById('login-form');
    if (!loginForm) return;

    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value;

        if (!email || !password) {
            alert('Пожалуйста, заполните все поля');
            return;
        }

        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
                credentials: 'include'
            });

            if (!response.ok) {
                alert('Неверная электронная почта или пароль');
                return;
            }
            
            window.location.href = 'index.html';
            
        } catch (error) {
            alert('Ошибка сервера, попробуйте позже');
            console.error(error);
        }
    });
}

async function getUserInfo() {
    const response = await fetch('/api/auth/userinfo', {
        method: 'GET',
        credentials: 'include'
    });

    if (response.ok) {
        return await response.json();
    }
    return null;
}

async function handleMainPageRoleControl() {
    if (window.location.pathname.endsWith('auth.html')) return;

    const container = document.querySelector('.container-card');
    if (!container) return;

    const userInfo = await getUserInfo();
    if (!userInfo) {
        window.location.href = '/auth.html';
        return;
    }

    const roles = userInfo.roles || [];
    const userName = userInfo.name || '';

    if (userName) {
        const h1 = container.querySelector('h1');
        if (h1) {
            h1.textContent = `Добро пожаловать, ${userName}!`;
        }
    }

    const elementsToCheck = [
        { selector: "button[onclick*='attachment-create.html']", requiredRole: 'Пользователь', hideParent: false },
        { selector: "button[onclick*='category-create.html']", requiredRole: 'Администратор', hideParent: false },
        { selector: "button[onclick*='feedback-create.html']", requiredRole: 'Пользователь', hideParent: false },
        { selector: "button[onclick*='response-create.html']", requiredRole: 'Администратор', hideParent: false },
        { selector: "button[onclick*='role-create.html']", requiredRole: 'Администратор', hideParent: false },
        { selector: "button[onclick*='status-create.html']", requiredRole: 'Администратор', hideParent: false },
        { selector: "button[onclick*='user-create.html']", requiredRole: 'Администратор', hideParent: false },

        { selector: "nav ul li a[href*='attachments.html']", requiredRole: 'Пользователь', hideParent: true },
        { selector: "nav ul li a[href*='categories.html']", requiredRole: 'Администратор', hideParent: true },
        { selector: "nav ul li a[href*='feedbacks.html']", requiredRole: 'Пользователь', hideParent: true },
        { selector: "nav ul li a[href*='responses.html']", requiredRole: 'Администратор', hideParent: true },
        { selector: "nav ul li a[href*='roles.html']", requiredRole: 'Администратор', hideParent: true },
        { selector: "nav ul li a[href*='statuses.html']", requiredRole: 'Администратор', hideParent: true },
        { selector: "nav ul li a[href*='users.html']", requiredRole: 'Администратор', hideParent: true },
    ];

    elementsToCheck.forEach(({ selector, requiredRole, hideParent }) => {
        const element = container.querySelector(selector);
        if (element && !hasRole(requiredRole, roles)) {
            if (hideParent && element.parentElement) {
                element.parentElement.style.display = 'none';
            } else {
                element.style.display = 'none';
            }
        }
    });
}

function hasRole(role, roles) {
    if (!roles) return false;
    if (Array.isArray(roles)) {
        return roles.includes(role);
    }
    return roles === role;
}
