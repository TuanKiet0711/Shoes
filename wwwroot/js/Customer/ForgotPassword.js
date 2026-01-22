document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.auth-form--forgot');
    if (!form) return;

    const toggle = document.querySelector('[data-forgot-toggle]');
    const divider = document.querySelector('[data-forgot-divider]');
    const emailInput = form.querySelector('input[type="email"]');

    if (toggle) {
        toggle.addEventListener('click', (event) => {
            event.preventDefault();
            const isOpen = form.classList.toggle('is-open');
            if (divider) {
                divider.classList.toggle('is-open', isOpen);
            }
            toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
            if (isOpen && emailInput) {
                emailInput.focus();
            }
        });
    }

    const button = form.querySelector('.auth-btn');
    form.addEventListener('submit', () => {
        if (!button) return;
        button.disabled = true;
        button.dataset.loading = '1';
        button.textContent = 'Đang gửi...';
    });
});
