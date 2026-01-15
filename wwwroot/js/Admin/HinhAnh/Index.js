function normalizeVN(str) {
    return (str || '')
        .toLowerCase()
        .normalize('NFD')
        .replace(/[\u0300-\u036f]/g, '')
        .replace(/đ/g, 'd')
        .replace(/[^a-z0-9\s]/g, '')
        .trim();
}

document.addEventListener('DOMContentLoaded', () => {
    function getHidden(wrapper, ddKey) {
        return wrapper.querySelector(`input[type="hidden"][data-dd-hidden="${ddKey}"]`);
    }

    function setButtonLabel(wrapper, label) {
        const el = wrapper.querySelector('.dd-label');
        if (el) el.textContent = label || '-- Tất cả --';
    }

    function getButton(wrapper) {
        return wrapper.querySelector('button[data-dd]');
    }

    function clearActive(wrapper) {
        wrapper.querySelectorAll('.dd-item').forEach(x => x.classList.remove('active'));
    }

    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        const btn = getButton(wrapper);
        const ddKey = btn?.dataset.dd;
        if (!ddKey) return;

        const hidden = getHidden(wrapper, ddKey);
        const value = (hidden?.value ?? '').toString().trim();

        const match = wrapper.querySelector(`.dd-item[data-value="${CSS.escape(value)}"]`);
        if (match) {
            setButtonLabel(wrapper, match.dataset.label || match.textContent.trim());
            clearActive(wrapper);
            match.classList.add('active');
        }
    });

    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.addEventListener('shown.bs.dropdown', () => {
            const search = wrapper.querySelector('.dd-search');
            if (search) {
                search.value = '';
                setTimeout(() => search.focus(), 0);
            }

            wrapper.querySelectorAll('.dd-item').forEach(x => x.classList.remove('d-none'));
        });
    });

    document.querySelectorAll('.dd-search').forEach(inp => {
        inp.addEventListener('input', () => {
            const wrapper = inp.closest('.dd-wrap');
            if (!wrapper) return;

            const q = normalizeVN(inp.value);
            wrapper.querySelectorAll('.dd-item').forEach(it => {
                const text = normalizeVN(it.textContent);
                it.classList.toggle('d-none', !!q && !text.includes(q));
            });
        });
    });

    document.querySelectorAll('.dd-item').forEach(item => {
        item.addEventListener('click', () => {
            const wrapper = item.closest('.dd-wrap');
            if (!wrapper) return;

            const btn = getButton(wrapper);
            const ddKey = btn?.dataset.dd;
            if (!ddKey) return;

            const hidden = getHidden(wrapper, ddKey);
            if (hidden) hidden.value = (item.dataset.value ?? '').toString().trim();

            setButtonLabel(wrapper, item.dataset.label || item.textContent.trim());
            clearActive(wrapper);
            item.classList.add('active');

            btn?.click();
        });
    });
});
