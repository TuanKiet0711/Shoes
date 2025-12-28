
    function toggleMenu(el) {
        el.parentElement.classList.toggle("open");
    }
    function toggleUserMenu(el) {
        el.parentElement.classList.toggle("open");
    }
    window.onclick = function (e) {
        if (!e.target.closest('.user-dropdown')) {
            document.querySelector('.user-dropdown')?.classList.remove('open');
        }
    }
