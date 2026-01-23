document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('form');
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

    const dateInput = form.querySelector('[data-date-input]');
    const datePicker = form.querySelector('#TaiKhoanNgaySinhPicker');
    const dateButton = form.querySelector('[data-date-picker]');

    const clamp = (value, min, max) => Math.min(Math.max(value, min), max);

    const formatDate = (value) => {
        const digits = value.replace(/\D/g, '').slice(0, 8);
        const day = digits.slice(0, 2);
        const month = digits.slice(2, 4);
        const year = digits.slice(4);
        let formatted = day;
        if (month) formatted += `/${month}`;
        if (year) formatted += `/${year}`;
        return formatted;
    };

    const normalizeDate = (value) => {
        const digits = value.replace(/\D/g, '').slice(0, 8);
        if (digits.length < 4) return formatDate(value);

        let day = Number(digits.slice(0, 2));
        let month = Number(digits.slice(2, 4));
        const year = Number(digits.slice(4, 8));

        month = clamp(month, 1, 12);
        if (digits.length >= 8) {
            const maxDay = new Date(year || 2000, month, 0).getDate();
            day = clamp(day, 1, maxDay);
        }

        const dd = String(day).padStart(2, '0');
        const mm = String(month).padStart(2, '0');
        const yyyy = digits.length >= 8 ? String(year).padStart(4, '0') : '';

        return yyyy ? `${dd}/${mm}/${yyyy}` : `${dd}/${mm}`;
    };

    const validateBirthDate = () => {
        if (!dateInput) return true;
        if (!dateInput.value) {
            dateInput.setCustomValidity('');
            return true;
        }
        const match = dateInput.value.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
        if (!match) {
            dateInput.setCustomValidity('');
            return true;
        }
        const [, d, m, y] = match;
        const birthDate = new Date(Number(y), Number(m) - 1, Number(d));
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const isValid = birthDate <= today;
        dateInput.setCustomValidity(isValid ? '' : 'Ngày sinh không được lớn hơn ngày hiện tại.');
        return isValid;
    };

    if (dateInput) {
        dateInput.addEventListener('input', () => {
            dateInput.value = formatDate(dateInput.value);
            dateInput.setCustomValidity('');
        });

        dateInput.addEventListener('blur', () => {
            if (!dateInput.value) {
                dateInput.setCustomValidity('');
                return;
            }
            dateInput.value = normalizeDate(dateInput.value);
            const match = dateInput.value.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
            if (match && datePicker) {
                const [, d, m, y] = match;
                datePicker.value = `${y}-${m}-${d}`;
            }
            validateBirthDate();
        });
    }

    if (dateButton && datePicker) {
        dateButton.addEventListener('click', () => {
            if (typeof datePicker.showPicker === 'function') {
                datePicker.showPicker();
            } else {
                datePicker.focus();
                datePicker.click();
            }
        });

        datePicker.addEventListener('change', () => {
            if (!dateInput || !datePicker.value) return;
            const [y, m, d] = datePicker.value.split('-');
            dateInput.value = `${d}/${m}/${y}`;
            validateBirthDate();
        });
    }

    const strengthWrapper = form.querySelector('[data-strength]');
    const strengthBar = form.querySelector('[data-strength-bar]');
    const strengthText = form.querySelector('[data-strength-text]');
    const passwordInput = form.querySelector('#TaiKhoanMatKhau');
    const confirmInput = form.querySelector('#TaiKhoanXacNhan');

    const validateConfirm = () => {
        if (!confirmInput || !passwordInput) return true;
        if (!confirmInput.value && !passwordInput.value) {
            confirmInput.setCustomValidity('');
            return true;
        }
        const isValid = confirmInput.value === passwordInput.value;
        confirmInput.setCustomValidity(isValid ? '' : 'Xác nhận mật khẩu không khớp.');
        return isValid;
    };

    const updateStrength = (value) => {
        if (!strengthWrapper || !strengthBar || !strengthText) return;

        let score = 0;
        if (value.length >= 6) score += 1;
        if (/[A-Z]/.test(value)) score += 1;
        if (/[a-z]/.test(value)) score += 1;
        if (/[0-9]/.test(value)) score += 1;
        if (/[^A-Za-z0-9]/.test(value)) score += 1;

        let label = 'Yếu';
        let width = '20%';
        let color = '#ef4444';

        if (score >= 4) {
            label = 'Mạnh';
            width = '100%';
            color = '#22c55e';
        } else if (score >= 3) {
            label = 'Khá';
            width = '70%';
            color = '#f59e0b';
        } else if (score >= 2) {
            label = 'Trung bình';
            width = '45%';
            color = '#f97316';
        }

        strengthBar.style.setProperty('--strength-width', width);
        strengthBar.style.setProperty('--strength-color', color);
        strengthText.textContent = `Độ mạnh: ${label}`;
    };

    if (passwordInput) {
        passwordInput.addEventListener('input', () => {
            updateStrength(passwordInput.value);
            validateConfirm();
        });
    }

    if (confirmInput) {
        confirmInput.addEventListener('input', validateConfirm);
    }

    form.addEventListener('submit', (event) => {
        const birthDateOk = validateBirthDate();
        const confirmOk = validateConfirm();
        if (!birthDateOk || !confirmOk) {
            event.preventDefault();
            form.reportValidity();
        }
    });
});
