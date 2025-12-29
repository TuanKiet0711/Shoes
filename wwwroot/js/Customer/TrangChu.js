//================== SLIDER ================= //
    const slides = document.querySelectorAll('.slide');
    let current = 0;
    setInterval(() => {
        slides[current].classList.remove('active');
        current = (current + 1) % slides.length;
        slides[current].classList.add('active');
    }, 3000);
//================== FAQ ================= //
    document.querySelectorAll('.faq-question').forEach(btn => {
        btn.onclick = () => {
            const ans = btn.nextElementSibling;
            const icon = btn.querySelector('span');

            if (ans.style.maxHeight) {
                ans.style.maxHeight = null;
                icon.textContent = '+';
            } else {
                ans.style.maxHeight = ans.scrollHeight + "px";
                icon.textContent = '−';
            }
        };
    });

