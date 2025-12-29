function toggleMenu(el) {
    const li = el.closest(".has-sub");
    if (!li) return;
    li.classList.toggle("open");
}

function toggleUserMenu(el) {
    el.parentElement.classList.toggle("open");
}

window.addEventListener("click", function (e) {
    if (!e.target.closest(".user-dropdown")) {
        document.querySelector(".user-dropdown")?.classList.remove("open");
    }
});

// Auto open submenu if contains active child
document.addEventListener("DOMContentLoaded", () => {
    const toastEl = document.getElementById("appToast");
    if (toastEl) {
        const toast = new bootstrap.Toast(toastEl);
        toast.show();
    }
});

