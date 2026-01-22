document.addEventListener('DOMContentLoaded', () => {
    const button = document.querySelector('[data-close-tab]');
    if (!button) return;

    button.addEventListener('click', () => {
        window.close();
    });
});
