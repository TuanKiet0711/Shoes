document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('spFilterForm');
    if (!form) return;

    const SCROLL_KEY = 'SP_FILTER_SCROLL';
    const saveScroll = () => {
        sessionStorage.setItem(SCROLL_KEY, String(window.scrollY || 0));
    };

    const restoreScroll = () => {
        const val = sessionStorage.getItem(SCROLL_KEY);
        if (!val) return;
        const y = parseInt(val, 10);
        if (!Number.isNaN(y)) window.scrollTo(0, y);
        sessionStorage.removeItem(SCROLL_KEY);
    };

    restoreScroll();

    const submitForm = () => {
        saveScroll();
        form.submit();
    };

    const inputs = Array.from(form.querySelectorAll('input[type="checkbox"], select'));
    const externalSelects = Array.from(document.querySelectorAll('select[form="spFilterForm"]'));
    const targets = [...new Set([...inputs, ...externalSelects])];

    targets.forEach(el => {
        el.addEventListener('change', submitForm);
    });
});
