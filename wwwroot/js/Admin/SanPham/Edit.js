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
        if (el) el.textContent = label || '-- Chọn --';
    }

    function getVisibleItems(wrapper) {
        return Array.from(wrapper.querySelectorAll('.dd-item'))
            .filter(x => !x.classList.contains('d-none'));
    }

    function clearActive(wrapper) {
        wrapper.querySelectorAll('.dd-item').forEach(x => x.classList.remove('active'));
    }

    function setActiveItem(wrapper, idx) {
        const items = getVisibleItems(wrapper);
        clearActive(wrapper);
        if (!items.length) return -1;
        const i = Math.max(0, Math.min(idx, items.length - 1));
        items[i].classList.add('active');
        items[i].scrollIntoView({ block: 'nearest' });
        return i;
    }

    function getActiveIndex(wrapper) {
        const items = getVisibleItems(wrapper);
        return items.findIndex(x => x.classList.contains('active'));
    }

    // ===== INIT label theo hidden value (giữ khi validation fail / reload) =====
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        const btn = wrapper.querySelector('button[data-dd]');
        const ddKey = btn?.dataset.dd;
        if (!ddKey) return;

        const hidden = getHidden(wrapper, ddKey);
        const value = (hidden?.value ?? '').toString().trim();

        if (!value) return;

        let match = wrapper.querySelector(`.dd-item[data-value="${CSS.escape(value)}"]`);

        // fallback: nếu DB lưu khác dấu/format
        if (!match) {
            const target = normalizeVN(value);
            match = Array.from(wrapper.querySelectorAll('.dd-item')).find(it => {
                const v = (it.dataset.value ?? '').toString().trim();
                return normalizeVN(v) === target || normalizeVN(it.textContent) === target;
            });
        }

        if (match) {
            setButtonLabel(wrapper, match.dataset.label || match.textContent.trim());
            clearActive(wrapper);
            match.classList.add('active');
        }
    });

    // ===== open dropdown: focus search, giữ active hiện tại =====
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.addEventListener('shown.bs.dropdown', () => {
            const search = wrapper.querySelector('.dd-search');
            if (!search) return;

            wrapper.querySelectorAll('.dd-item').forEach(it => it.classList.remove('d-none'));
            search.value = '';
            search.focus();

            const visible = getVisibleItems(wrapper);
            const idx = visible.findIndex(x => x.classList.contains('active'));
            setActiveItem(wrapper, idx >= 0 ? idx : 0);
        });
    });

    // ===== search + keyboard =====
    document.querySelectorAll('.dd-search').forEach(inp => {
        inp.addEventListener('input', () => {
            const wrapper = inp.closest('.dd-wrap');
            if (!wrapper) return;

            const q = normalizeVN(inp.value);
            wrapper.querySelectorAll('.dd-item').forEach(item => {
                const text = normalizeVN(item.textContent);
                item.classList.toggle('d-none', q && !text.includes(q));
            });

            setActiveItem(wrapper, 0);
        });

        inp.addEventListener('keydown', (e) => {
            const wrapper = inp.closest('.dd-wrap');
            if (!wrapper) return;

            const items = getVisibleItems(wrapper);
            const btn = wrapper.querySelector('button[data-dd]');

            if (e.key === 'ArrowDown') {
                e.preventDefault();
                if (!items.length) return;
                setActiveItem(wrapper, Math.min(getActiveIndex(wrapper) + 1, items.length - 1));
            }

            if (e.key === 'ArrowUp') {
                e.preventDefault();
                if (!items.length) return;
                setActiveItem(wrapper, Math.max(getActiveIndex(wrapper) - 1, 0));
            }

            if (e.key === 'Enter') {
                e.preventDefault();
                if (!items.length) return;
                const idx = getActiveIndex(wrapper);
                (items[idx >= 0 ? idx : 0])?.click();
            }

            if (e.key === 'Escape') {
                e.preventDefault();
                btn?.click();
            }
        });
    });

    // ===== click item =====
    document.querySelectorAll('.dd-item').forEach(item => {
        item.addEventListener('mouseenter', () => {
            const wrapper = item.closest('.dd-wrap');
            if (!wrapper) return;
            clearActive(wrapper);
            item.classList.add('active');
        });

        item.addEventListener('click', () => {
            const wrapper = item.closest('.dd-wrap');
            if (!wrapper) return;

            const btn = wrapper.querySelector('button[data-dd]');
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
