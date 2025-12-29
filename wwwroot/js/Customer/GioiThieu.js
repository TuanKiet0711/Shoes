//================== BANNER HERO SLIDER JS (GIỮ NGUYÊN STYLE, TỰ CHUYỂN 3 GIÂY) ================= //
    (function () {
        const root = document.getElementById('aboutHero');
        if (!root) return;

        const slides = root.querySelectorAll('.about-slide');
        if (!slides.length) return;

        let current = 0;
        const INTERVAL = 3000; // 3 giây

        function setActive(idx) {
            slides[current].classList.remove('active');
            current = idx;
            slides[current].classList.add('active');

            // reset animation text cho slide hiện tại
            slides[current].querySelectorAll('.about-anim').forEach(el => {
                el.style.animation = 'none';
                void el.offsetHeight; // force reflow
                el.style.animation = '';
            });
        }

        setInterval(() => {
            setActive((current + 1) % slides.length);
        }, INTERVAL);
    })();

