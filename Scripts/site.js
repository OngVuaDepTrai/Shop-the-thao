// SportHub - Scripts/site.js
// Client-side interactions & utilities for Nguyễn Minh Tâm's customer views

document.addEventListener('DOMContentLoaded', function () {
    // 1. Auto dismiss flash alerts after 5 seconds (static inline notices stay)
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            if (alert && alert.parentElement) {
                alert.style.transition = 'opacity 0.5s ease';
                alert.style.opacity = '0';
                setTimeout(() => alert.remove(), 500);
            }
        }, 5000);
    });

    // 2. Quantity counter (+ / -) in Product Details & Cart
    initQuantityControls();

    // 3. Product Gallery Image Switcher
    initGallerySwitcher();

    // 4. Variant selectors (Size & Color)
    initVariantSelectors();
});

// Quantity controls
function initQuantityControls() {
    document.querySelectorAll('.qty-control').forEach(function (control) {
        const input = control.querySelector('.qty-input');
        const btnMinus = control.querySelector('.qty-minus');
        const btnPlus = control.querySelector('.qty-plus');

        if (!input) return;

        if (btnMinus) {
            btnMinus.addEventListener('click', function (e) {
                e.preventDefault();
                let current = parseInt(input.value) || 1;
                const min = parseInt(input.getAttribute('min')) || 1;
                if (current > min) {
                    input.value = current - 1;
                    input.dispatchEvent(new Event('change'));
                }
            });
        }

        if (btnPlus) {
            btnPlus.addEventListener('click', function (e) {
                e.preventDefault();
                let current = parseInt(input.value) || 1;
                const max = parseInt(input.getAttribute('max')) || 999;
                if (current < max) {
                    input.value = current + 1;
                    input.dispatchEvent(new Event('change'));
                }
            });
        }
    });
}

// Product Details Gallery Switcher
function initGallerySwitcher() {
    const mainImg = document.getElementById('mainProductImage');
    const thumbs = document.querySelectorAll('.gallery-thumbs .thumb-item');

    if (!mainImg || thumbs.length === 0) return;

    thumbs.forEach(function (thumb) {
        thumb.addEventListener('click', function () {
            thumbs.forEach(t => t.classList.remove('active'));
            this.classList.add('active');

            const newSrc = this.getAttribute('data-img-src') || this.querySelector('img')?.src;
            if (newSrc && mainImg.src !== newSrc) {
                mainImg.style.opacity = '0.3';
                setTimeout(function () {
                    mainImg.src = newSrc;
                    mainImg.style.opacity = '1';
                }, 150);
            }
        });
    });
}

// Product Variant Selectors (Size & Color)
function initVariantSelectors() {
    // Size pills
    const sizePills = document.querySelectorAll('.size-picker .size-pill:not(.disabled)');
    const hiddenSizeInput = document.getElementById('selectedSize');

    sizePills.forEach(function (pill) {
        pill.addEventListener('click', function () {
            sizePills.forEach(p => p.classList.remove('active'));
            this.classList.add('active');
            const sizeVal = this.getAttribute('data-size') || this.innerText.trim();
            if (hiddenSizeInput) {
                hiddenSizeInput.value = sizeVal;
                hiddenSizeInput.dispatchEvent(new Event('change'));
            }
        });
    });

    // Color pills / chips
    const colorPills = document.querySelectorAll('.color-picker .color-pill:not(.disabled)');
    const hiddenColorInput = document.getElementById('selectedColor');

    colorPills.forEach(function (pill) {
        pill.addEventListener('click', function () {
            colorPills.forEach(p => p.classList.remove('active'));
            this.classList.add('active');
            const colorVal = this.getAttribute('data-color') || this.innerText.trim();
            if (hiddenColorInput) {
                hiddenColorInput.value = colorVal;
                hiddenColorInput.dispatchEvent(new Event('change'));
            }
        });
    });
}

