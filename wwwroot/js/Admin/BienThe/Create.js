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
        if (el) el.textContent = label || '-- Chọn sản phẩm --';
    }

    function getButton(wrapper) {
        return wrapper.querySelector('button[data-dd]');
    }

    function isOpen(wrapper) {
        return !!wrapper.querySelector('.dropdown-menu.show');
    }

    function clearActive(wrapper) {
        wrapper.querySelectorAll('.dd-item').forEach(x => x.classList.remove('active'));
    }

    function getVisibleItems(wrapper) {
        return Array.from(wrapper.querySelectorAll('.dd-item'))
            .filter(x => !x.classList.contains('d-none'));
    }

    function setActiveByIndex(wrapper, idx) {
        const items = getVisibleItems(wrapper);
        if (!items.length) return;

        const i = Math.max(0, Math.min(idx, items.length - 1));
        clearActive(wrapper);
        items[i].classList.add('active');

        // scroll trong dd-list (đúng ý bạn)
        items[i].scrollIntoView({ block: 'nearest' });
    }

    function setActiveByDelta(wrapper, delta) {
        const items = getVisibleItems(wrapper);
        if (!items.length) return;

        let idx = items.findIndex(x => x.classList.contains('active'));
        if (idx < 0) idx = 0;
        setActiveByIndex(wrapper, idx + delta);
    }

    // init label theo hidden value
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

    // mở dropdown -> focus search + hiện caret + reset filter + đảm bảo scroll list đúng
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.addEventListener('shown.bs.dropdown', () => {
            const search = wrapper.querySelector('.dd-search');
            const list = wrapper.querySelector('.dd-list');

            // reset filter
            wrapper.querySelectorAll('.dd-item').forEach(x => x.classList.remove('d-none'));

            // reset scroll list về top
            if (list) list.scrollTop = 0;

            // focus input (đảm bảo có con trỏ)
            if (search) {
                search.value = "";
                setTimeout(() => {
                    search.focus();
                    // đặt caret về cuối/đầu đều được, ở đây đặt cuối cho chắc thấy con trỏ
                    const v = search.value;
                    search.setSelectionRange(v.length, v.length);
                }, 0);
            }

            setActiveByIndex(wrapper, 0);
        });
    });

    // gõ search -> filter list
    document.querySelectorAll('.dd-search').forEach(inp => {
        inp.addEventListener('input', () => {
            const wrapper = inp.closest('.dd-wrap');
            if (!wrapper) return;

            const q = normalizeVN(inp.value);
            wrapper.querySelectorAll('.dd-item').forEach(it => {
                const text = normalizeVN(it.textContent);
                it.classList.toggle('d-none', !!q && !text.includes(q));
            });

            // sau khi lọc, đưa active về item đầu tiên
            const list = wrapper.querySelector('.dd-list');
            if (list) list.scrollTop = 0;

            setActiveByIndex(wrapper, 0);
        });
    });

    // keyboard nav (khi dropdown mở)
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.addEventListener('keydown', (e) => {
            if (!isOpen(wrapper)) return;

            if (e.key === 'ArrowDown') { e.preventDefault(); setActiveByDelta(wrapper, +1); }
            if (e.key === 'ArrowUp')   { e.preventDefault(); setActiveByDelta(wrapper, -1); }

            if (e.key === 'Enter') {
                e.preventDefault();
                const active = getVisibleItems(wrapper).find(x => x.classList.contains('active'));
                active?.click();
            }

            if (e.key === 'Escape') {
                e.preventDefault();
                getButton(wrapper)?.click();
            }
        });
    });

    // hover -> active
    document.querySelectorAll('.dd-item').forEach(item => {
        item.addEventListener('mouseenter', () => {
            const wrapper = item.closest('.dd-wrap');
            if (!wrapper) return;
            clearActive(wrapper);
            item.classList.add('active');
        });
    });

    // click item -> set hidden + label + close
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

            btn?.click(); // đóng dropdown
        });
    });
});
