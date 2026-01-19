(() => {
    const overlay = document.querySelector('[data-auth-overlay]');
    if (!overlay) return;

    const openTriggers = document.querySelectorAll('[data-auth-trigger]');
    const closeTriggers = overlay.querySelectorAll('[data-auth-close]');

    const focusTarget = (mode) => {
        const selector = mode === 'register' ? '[data-auth-focus="register"]' : '[data-auth-focus="login"]';
        const el = overlay.querySelector(selector);
        if (el) el.focus();
    };

    const openModal = (mode) => {
        overlay.classList.add('is-open');
        overlay.dataset.authMode = mode || '';
        document.body.classList.add('auth-modal-open');
        focusTarget(mode);
    };

    const closeModal = () => {
        overlay.classList.remove('is-open');
        overlay.dataset.authMode = '';
        document.body.classList.remove('auth-modal-open');
    };

    openTriggers.forEach(trigger => {
        trigger.addEventListener('click', (event) => {
            event.preventDefault();
            openModal(trigger.dataset.authTrigger);
        });
    });

    closeTriggers.forEach(trigger => {
        trigger.addEventListener('click', closeModal);
    });

    document.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') closeModal();
    });

    const initial = overlay.dataset.authInitial;
    if (initial) {
        openModal(initial);
    }
})();
