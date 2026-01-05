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
    const PAGE_SIZE = 15;

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

    function isOpen(wrapper) {
        return wrapper.classList.contains('show') || wrapper.querySelector('.dropdown-menu.show');
    }

    function clearActive(wrapper) {
        wrapper.querySelectorAll('.dd-item').forEach(x => x.classList.remove('active'));
    }

    function getAllItems(wrapper) {
        return Array.from(wrapper.querySelectorAll('.dd-item'));
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
        items[i].scrollIntoView({ block: 'nearest' });
    }

    function setActiveByDelta(wrapper, delta) {
        const items = getVisibleItems(wrapper);
        if (!items.length) return;

        let idx = items.findIndex(x => x.classList.contains('active'));
        if (idx < 0) idx = 0;
        setActiveByIndex(wrapper, idx + delta);
    }

    function state(wrapper) {
        if (!wrapper.__dd) wrapper.__dd = { page: 1, totalPages: 1, query: "" };
        return wrapper.__dd;
    }

    function updatePagerUI(wrapper, filteredCount) {
        const st = state(wrapper);

        const pager = wrapper.querySelector('.dd-pager');
        const prev = wrapper.querySelector('.dd-prev');
        const next = wrapper.querySelector('.dd-next');

        const pageEl = wrapper.querySelector('.dd-page');
        const totalEl = wrapper.querySelector('.dd-total');
        const countEl = wrapper.querySelector('.dd-count');

        if (countEl) countEl.textContent = filteredCount.toString();
        if (pageEl) pageEl.textContent = st.page.toString();
        if (totalEl) totalEl.textContent = st.totalPages.toString();

        // chỉ hiện pager khi > 15 sản phẩm
        if (pager) pager.style.display = (filteredCount <= PAGE_SIZE) ? 'none' : '';

        if (prev) prev.disabled = st.page <= 1;
        if (next) next.disabled = st.page >= st.totalPages;
    }

    function applyFilterAndPaging(wrapper) {
        const st = state(wrapper);

        const all = getAllItems(wrapper);
        const q = normalizeVN(st.query);

        const filtered = all.filter(it => {
            const text = normalizeVN(it.textContent);
            return !q || text.includes(q);
        });

        if (filtered.length <= PAGE_SIZE) {
            st.totalPages = 1;
            st.page = 1;
        } else {
            st.totalPages = Math.ceil(filtered.length / PAGE_SIZE);
            st.page = Math.max(1, Math.min(st.page, st.totalPages));
        }

        all.forEach(it => it.classList.add('d-none'));

        const start = (st.page - 1) * PAGE_SIZE;
        const end = start + PAGE_SIZE;
        filtered.slice(start, end).forEach(it => it.classList.remove('d-none'));

        updatePagerUI(wrapper, filtered.length);

        // active item đầu tiên đang hiển thị
        setActiveByIndex(wrapper, 0);
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

    // open dropdown
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.addEventListener('shown.bs.dropdown', () => {
            const st = state(wrapper);
            st.page = 1;
            st.query = "";

            const search = wrapper.querySelector('.dd-search');
            if (search) {
                search.value = "";
                setTimeout(() => search.focus(), 0);
            }

            applyFilterAndPaging(wrapper);
        });
    });

    // search input
    document.querySelectorAll('.dd-search').forEach(inp => {
        inp.addEventListener('input', () => {
            const wrapper = inp.closest('.dd-wrap');
            if (!wrapper) return;

            const st = state(wrapper);
            st.query = inp.value || "";
            st.page = 1;

            applyFilterAndPaging(wrapper);
        });
    });

    // pager buttons
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.querySelector('.dd-prev')?.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();

            const st = state(wrapper);
            st.page = Math.max(1, st.page - 1);
            applyFilterAndPaging(wrapper);

            wrapper.querySelector('.dd-search')?.focus();
        });

        wrapper.querySelector('.dd-next')?.addEventListener('click', (e) => {
            e.preventDefault();
            e.stopPropagation();

            const st = state(wrapper);
            st.page = Math.min(st.totalPages, st.page + 1);
            applyFilterAndPaging(wrapper);

            wrapper.querySelector('.dd-search')?.focus();
        });
    });

    // keyboard nav
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.addEventListener('keydown', (e) => {
            if (!isOpen(wrapper)) return;

            if (e.key === 'ArrowDown') {
                e.preventDefault();
                setActiveByDelta(wrapper, +1);
            }

            if (e.key === 'ArrowUp') {
                e.preventDefault();
                setActiveByDelta(wrapper, -1);
            }

            if (e.key === 'Enter') {
                e.preventDefault();
                const active = getVisibleItems(wrapper).find(x => x.classList.contains('active'));
                active?.click();
            }

            if (e.key === 'Escape') {
                e.preventDefault();
                getButton(wrapper)?.click();
            }

            if (e.key === 'PageDown') {
                e.preventDefault();
                const st = state(wrapper);
                st.page = Math.min(st.totalPages, st.page + 1);
                applyFilterAndPaging(wrapper);
            }

            if (e.key === 'PageUp') {
                e.preventDefault();
                const st = state(wrapper);
                st.page = Math.max(1, st.page - 1);
                applyFilterAndPaging(wrapper);
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

    // click item
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
