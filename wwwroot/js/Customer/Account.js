document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.account-form');
    if (!form) return;

    const phoneInput = form.querySelector('#Sdt');
    const dateInput = form.querySelector('[data-date-input]');
    const datePicker = form.querySelector('#AccountNgaySinhPicker');
    const dateButton = form.querySelector('[data-date-picker]');
    const toggles = form.querySelectorAll('[data-toggle-password]');

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

    if (phoneInput) {
        phoneInput.addEventListener('input', () => {
            const digits = phoneInput.value.replace(/\D/g, '').slice(0, 10);
            if (phoneInput.value !== digits) phoneInput.value = digits;
        });
    }

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

    form.addEventListener('submit', (event) => {
        const birthDateOk = validateBirthDate();
        if (!birthDateOk) {
            event.preventDefault();
            form.reportValidity();
        }
    });

    const setupToggle = (button) => {
        const target = form.querySelector(button.dataset.togglePassword || '');
        if (!target) return;
        button.addEventListener('click', () => {
            const isPassword = target.type === 'password';
            target.type = isPassword ? 'text' : 'password';
            button.innerHTML = isPassword
                ? '<i class="fa-regular fa-eye-slash"></i>'
                : '<i class="fa-regular fa-eye"></i>';
        });
    };

    toggles.forEach(setupToggle);
});
