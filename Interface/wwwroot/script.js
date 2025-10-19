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

document.addEventListener('DOMContentLoaded', () => {
    showPendingAlerts();
    handleLoginForm();
    handleMainPageRoleControl();

    const logoutBtn = document.getElementById('logout-button');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', async () => {
            await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include'
            });
            sessionStorage.setItem('pendingAlert', 'Вы успешно вышли из системы');
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
            showCustomAlert('Пожалуйста, заполните все поля');
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
                showCustomAlert('Неверная электронная почта или пароль');
                return;
            }

            sessionStorage.setItem('pendingAlert', 'Вход выполнен успешно');
            window.location.href = 'index.html';

        } catch (error) {
            showCustomAlert('Ошибка сервера, попробуйте позже');
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
        sessionStorage.setItem('pendingAlert', 'Требуется авторизация, перенаправление на страницу входа');
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

    if (!hasRole('Пользователь', roles)) {
        return;
    }

    const elementsToCheck = [
        { selector: "button[onclick*='category-create.html']", hideParent: false },
        { selector: "button[onclick*='response-create.html']", hideParent: false },
        { selector: "button[onclick*='role-create.html']", hideParent: false },
        { selector: "button[onclick*='status-create.html']", hideParent: false },
        { selector: "button[onclick*='user-create.html']", hideParent: false },

        { selector: "nav ul li a[href*='categories.html']", hideParent: true },
        { selector: "nav ul li a[href*='responses.html']", hideParent: true },
        { selector: "nav ul li a[href*='roles.html']", hideParent: true },
        { selector: "nav ul li a[href*='statuses.html']", hideParent: true },
        { selector: "nav ul li a[href*='users.html']", hideParent: true },
    ];

    elementsToCheck.forEach(({ selector, hideParent }) => {
        const element = container.querySelector(selector);
        if (element) {
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