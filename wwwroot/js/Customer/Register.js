document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.auth-form--register');
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

    const dateInput = form.querySelector('#AuthRegisterNgaySinh') || form.querySelector('#NgaySinh');
    const datePicker = form.querySelector('#NgaySinhPicker');
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

    const getError = (name) => form.querySelector(`[data-error-for="${name}"]`);
    const setError = (input, message) => {
        if (!input) return;
        const error = getError(input.name);
        if (error) error.textContent = message || '';
        input.setCustomValidity(message || '');
    };

    const validateBirthDate = () => {
        if (!dateInput) return true;
        if (!dateInput.value) {
            setError(dateInput, '');
            return true;
        }
        const match = dateInput.value.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
        if (!match) {
            setError(dateInput, '');
            return true;
        }
        const [, d, m, y] = match;
        const birthDate = new Date(Number(y), Number(m) - 1, Number(d));
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        const isValid = birthDate <= today;
        setError(dateInput, isValid ? '' : 'Ngày sinh không được lớn hơn ngày hiện tại.');
        return isValid;
    };

    if (dateInput) {
        dateInput.addEventListener('input', () => {
            dateInput.value = formatDate(dateInput.value);
            dateInput.setCustomValidity('');
        });

        dateInput.addEventListener('blur', () => {
            if (!dateInput.value) {
                setError(dateInput, '');
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
    const passwordInput = form.querySelector('#AuthRegisterMatKhau');
    const confirmInput = form.querySelector('#AuthRegisterXacNhan');
    const phoneInput = form.querySelector('#AuthRegisterSdt');

    const validatePhone = (strict = false) => {
        if (!phoneInput) return true;
        const digits = phoneInput.value.replace(/\D/g, '').slice(0, 10);
        if (phoneInput.value !== digits) phoneInput.value = digits;
        const hasValue = digits.length > 0;
        if (!strict && !hasValue) {
            setError(phoneInput, '');
            return true;
        }
        if (strict && !hasValue) {
            setError(phoneInput, '');
            return false;
        }
        const isValid = digits.length === 10;
        setError(phoneInput, isValid ? '' : 'Số điện thoại phải đủ 10 số.');
        return isValid;
    };

    const validatePassword = (strict = false) => {
        if (!passwordInput) return true;
        const length = passwordInput.value.length;
        if (!strict && length === 0) {
            setError(passwordInput, '');
            return true;
        }
        if (strict && length === 0) {
            setError(passwordInput, '');
            return false;
        }
        const isValid = length >= 6;
        setError(passwordInput, isValid ? '' : 'Mật khẩu tối thiểu 6 ký tự.');
        return isValid;
    };

    const validateConfirm = (strict = false) => {
        if (!confirmInput || !passwordInput) return true;
        const confirmValue = confirmInput.value;
        const passwordValue = passwordInput.value;
        if (!confirmValue || !passwordValue) {
            setError(confirmInput, '');
            return !strict;
        }
        const isValid = confirmValue === passwordValue;
        setError(confirmInput, isValid ? '' : 'Xác nhận mật khẩu không khớp.');
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
            validatePassword(false);
            validateConfirm(false);
            updateStrength(passwordInput.value);
        });
    }

    const button = form.querySelector('.auth-btn');
    if (phoneInput) {
        phoneInput.addEventListener('input', () => validatePhone(false));
        phoneInput.addEventListener('blur', () => validatePhone(false));
    }

    if (confirmInput) {
        confirmInput.addEventListener('input', () => validateConfirm(false));
        confirmInput.addEventListener('blur', () => validateConfirm(false));
    }

    form.addEventListener('submit', (event) => {
        const birthDateOk = validateBirthDate();
        const phoneOk = validatePhone(true);
        const passwordOk = validatePassword(true);
        const confirmOk = validateConfirm(true);
        if (!birthDateOk || !phoneOk || !passwordOk || !confirmOk) {
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
