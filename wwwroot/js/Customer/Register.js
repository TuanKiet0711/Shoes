document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('.auth-form--register');
    if (!form) return;

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

    if (dateInput) {
        dateInput.addEventListener('input', () => {
            dateInput.value = formatDate(dateInput.value);
            dateInput.setCustomValidity('');
        });

        dateInput.addEventListener('blur', () => {
            if (!dateInput.value) return;
            dateInput.value = normalizeDate(dateInput.value);
            if (!datePicker) return;
            const match = dateInput.value.match(/^(\d{2})\/(\d{2})\/(\d{4})$/);
            if (!match) return;
            const [, d, m, y] = match;
            datePicker.value = `${y}-${m}-${d}`;
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
        });
    }

    const button = form.querySelector('.auth-btn');
    form.addEventListener('submit', () => {
        if (!button) return;
        button.disabled = true;
        button.dataset.loading = '1';
        button.textContent = 'Đang xử lý...';
    });
});
