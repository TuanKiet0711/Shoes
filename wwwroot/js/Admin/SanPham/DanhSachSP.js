
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
        return wrapper.querySelector(`input[type="hidden"][name="${ddKey}"]`);
    }

    function setButtonLabel(wrapper, label) {
        const el = wrapper.querySelector('.dd-label');
        if (el) el.textContent = label || '-- Tất cả --';
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

    // ===================== 1) INIT GIỮ LỌC SAU RELOAD =====================
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        const btn = wrapper.querySelector('button[data-dd]');
        const ddKey = btn?.dataset.dd;
        if (!ddKey) return;

        const hidden = getHidden(wrapper, ddKey);
        const value = (hidden?.value ?? '').toString();

        // chọn item theo hidden value
        const match = wrapper.querySelector(`.dd-item[data-value="${CSS.escape(value)}"]`);
        if (match) {
            setButtonLabel(wrapper, match.dataset.label || match.textContent.trim());
            clearActive(wrapper);
            match.classList.add('active');
        } else {
            // fallback: item "tất cả" (data-value="")
            const allItem = wrapper.querySelector(`.dd-item[data-value=""]`);
            setButtonLabel(wrapper, allItem?.dataset.label || allItem?.textContent.trim() || '-- Tất cả --');
            clearActive(wrapper);
            if (allItem) allItem.classList.add('active');
        }
    });

    // ===================== 2) KHI MỞ DROPDOWN: focus + giữ active hiện tại =====================
    document.querySelectorAll('.dd-wrap').forEach(wrapper => {
        wrapper.addEventListener('shown.bs.dropdown', () => {
            const search = wrapper.querySelector('.dd-search');
            if (!search) return;

            // reset filter hiển thị (KHÔNG reset lựa chọn)
            wrapper.querySelectorAll('.dd-item').forEach(it => it.classList.remove('d-none'));

            search.value = '';
            search.focus();

            // đặt hover vào item đang active, nếu không có thì item đầu
            const active = getVisibleItems(wrapper).findIndex(x => x.classList.contains('active'));
            setActiveItem(wrapper, active >= 0 ? active : 0);
        });
    });

    // ===================== 3) SEARCH + KEYBOARD (không dấu) =====================
    document.querySelectorAll('.dd-search').forEach(inp => {

        inp.addEventListener('input', () => {
            const wrapper = inp.closest('.dd-wrap');
            if (!wrapper) return;

            const q = normalizeVN(inp.value);

            wrapper.querySelectorAll('.dd-item').forEach(item => {
                const text = normalizeVN(item.textContent);
                const show = !q || text.includes(q);
                item.classList.toggle('d-none', !show);
            });

            // khi lọc xong, hover item đầu tiên đang hiển thị
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
                const next = Math.min(getActiveIndex(wrapper) + 1, items.length - 1);
                setActiveItem(wrapper, next);
            }

            if (e.key === 'ArrowUp') {
                e.preventDefault();
                if (!items.length) return;
                const prev = Math.max(getActiveIndex(wrapper) - 1, 0);
                setActiveItem(wrapper, prev);
            }

            if (e.key === 'Enter') {
                e.preventDefault();
                if (!items.length) return;
                const idx = getActiveIndex(wrapper);
                const pick = items[idx >= 0 ? idx : 0];
                if (pick) pick.click();
            }

            if (e.key === 'Escape') {
                e.preventDefault();
                if (btn) btn.click();
            }
        });
    });

    // ===================== 4) HOVER + CLICK CHỌN (set hidden, set label, đóng) =====================
    document.querySelectorAll('.dd-item').forEach(item => {

        item.addEventListener('mouseenter', () => {
            const wrapper = item.closest('.dd-wrap');
            if (!wrapper) return;

            // hover chuột = active
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
            if (hidden) hidden.value = item.dataset.value ?? '';

            setButtonLabel(wrapper, item.dataset.label || item.textContent.trim());

            clearActive(wrapper);
            item.classList.add('active');

            // đóng dropdown
            if (btn) btn.click();

            // Nếu muốn chọn xong tự submit form thì bỏ comment:
            // wrapper.closest('form')?.submit();
        });

    });

});

