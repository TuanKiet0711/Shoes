document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.auth-form--login');
    if (!form) return;

    const button = form.querySelector('.auth-btn');
    form.addEventListener('submit', () => {
        if (!button) return;
        button.disabled = true;
        button.dataset.loading = '1';
        button.textContent = 'Đang xử lý...';
    });
});
