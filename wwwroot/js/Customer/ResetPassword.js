document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.auth-form--reset');
    if (!form) return;

    const setupToggle = (button) => {
        const target = document.querySelector(button.dataset.togglePassword || '');
        if (!target) return;
        button.addEventListener('click', () => {
            const isPassword = target.type === 'password';
            target.type = isPassword ? 'text' : 'password';
            button.innerHTML = isPassword
                ? '<i class="fa-regular fa-eye-slash"></i>'
                : '<i class="fa-regular fa-eye"></i>';
        });
    };

    form.querySelectorAll('[data-toggle-password]').forEach(setupToggle);

    const passwordInput = form.querySelector('#AuthResetPassword');
    const confirmInput = form.querySelector('#AuthResetConfirm');

    const validatePasswords = () => {
        if (!passwordInput || !confirmInput) return true;
        const lengthOk = passwordInput.value.length >= 6;
        passwordInput.setCustomValidity(lengthOk ? '' : 'Mật khẩu tối thiểu 6 ký tự.');

        const matchOk = confirmInput.value === passwordInput.value;
        confirmInput.setCustomValidity(matchOk ? '' : 'Xác nhận mật khẩu không khớp.');
        return lengthOk && matchOk;
    };

    if (passwordInput) passwordInput.addEventListener('input', validatePasswords);
    if (confirmInput) confirmInput.addEventListener('input', validatePasswords);

    const button = form.querySelector('.auth-btn');
    form.addEventListener('submit', (event) => {
        const ok = validatePasswords();
        if (!ok) {
            event.preventDefault();
            form.reportValidity();
            return;
        }
        if (!button) return;
        button.disabled = true;
        button.dataset.loading = '1';
        button.textContent = 'Đang xử lý...';
    });
});
