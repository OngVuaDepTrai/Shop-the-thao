/* ==========================================================================
   SportHub - Scripts/site.js (Tâm phụ trách)
   Xử lý thư viện ảnh, chọn size/màu, số lượng, đánh giá sao, bộ lọc sản phẩm
   ========================================================================== */

// 1. Đổi ảnh chính trong trang Chi tiết sản phẩm (ProductImage.Gallery)
function switchProductImage(thumbBtn, imgUrl) {
    var mainImg = document.getElementById('mainProductImage');
    if (mainImg && imgUrl) {
        mainImg.style.opacity = '0.35';
        setTimeout(function () {
            mainImg.src = imgUrl;
            mainImg.style.opacity = '1';
        }, 120);
    }
    var thumbs = document.querySelectorAll('.gallery-thumbs .thumb-item');
    for (var i = 0; i < thumbs.length; i++) {
        thumbs[i].classList.remove('active');
    }
    if (thumbBtn) {
        thumbBtn.classList.add('active');
    }
}

// 2. Chọn biến thể Màu sắc & Kích cỡ (Size)
function selectVariantColor(btn, color) {
    var btns = document.querySelectorAll('#colorOptionsGroup .color-btn');
    for (var i = 0; i < btns.length; i++) {
        btns[i].classList.remove('active');
    }
    if (btn) btn.classList.add('active');

    var colorInput = document.getElementById('selectedColorInput');
    if (colorInput) colorInput.value = color;

    var colorLabel = document.getElementById('selectedColorLabel');
    if (colorLabel) colorLabel.textContent = color;

    syncSelectedVariant();
}

function selectVariantSize(btn, size) {
    var btns = document.querySelectorAll('#sizeOptionsGroup .size-btn');
    for (var i = 0; i < btns.length; i++) {
        btns[i].classList.remove('active');
    }
    if (btn) btn.classList.add('active');

    var sizeInput = document.getElementById('selectedSizeInput');
    if (sizeInput) sizeInput.value = size;

    var sizeLabel = document.getElementById('selectedSizeLabel');
    if (sizeLabel) sizeLabel.textContent = size;

    syncSelectedVariant();
}

function syncSelectedVariant() {
    var variants = window.productVariantsData || [];
    if (!variants.length) return;

    var sizeInput = document.getElementById('selectedSizeInput');
    var colorInput = document.getElementById('selectedColorInput');
    var currentSize = sizeInput ? sizeInput.value : '';
    var currentColor = colorInput ? colorInput.value : '';

    var matched = null;
    for (var i = 0; i < variants.length; i++) {
        var v = variants[i];
        var sizeMatch = !currentSize || v.size === currentSize;
        var colorMatch = !currentColor || v.color === currentColor;
        if (sizeMatch && colorMatch) {
            matched = v;
            break;
        }
    }

    // Nếu tổ hợp hiện tại không tồn tại, ưu tiên biến thể cùng size hoặc cùng màu còn hàng
    if (!matched) {
        for (var j = 0; j < variants.length; j++) {
            if (currentSize && variants[j].size === currentSize) {
                matched = variants[j];
                break;
            }
        }
    }

    var varInput1 = document.getElementById('selectedVariantId');
    var varInput2 = document.getElementById('selectedProductVariantId');
    var addBtn = document.getElementById('btnAddToCart');
    var stockBadge = document.getElementById('stockStatusBadge');
    var qtyInput = document.getElementById('detailQuantityInput');

    if (matched && matched.stock > 0) {
        if (varInput1) varInput1.value = matched.id;
        if (varInput2) varInput2.value = matched.id;
        if (addBtn) {
            addBtn.disabled = false;
            addBtn.setAttribute('data-variant-id', matched.id);
            var span = addBtn.querySelector('span');
            if (span) span.textContent = 'Thêm vào giỏ hàng';
        }
        if (stockBadge) {
            stockBadge.className = 'stock-badge in-stock';
            stockBadge.innerHTML = '<i class="bi bi-check-circle-fill me-1"></i><span>Còn hàng (' + matched.stock + ' sản phẩm)</span>';
        }
        if (qtyInput) {
            qtyInput.max = matched.stock;
            if (parseInt(qtyInput.value, 10) > matched.stock) {
                qtyInput.value = matched.stock;
            }
        }
    } else {
        if (addBtn) {
            addBtn.disabled = true;
            var btnSpan = addBtn.querySelector('span');
            if (btnSpan) btnSpan.textContent = 'Hết hàng size/màu này';
        }
        if (stockBadge) {
            stockBadge.className = 'stock-badge out-stock';
            stockBadge.innerHTML = '<i class="bi bi-x-circle-fill me-1"></i><span>Tạm hết hàng phân loại này</span>';
        }
    }
}

// 3. Tăng / giảm số lượng sản phẩm
function changeDetailQty(delta) {
    var input = document.getElementById('detailQuantityInput');
    if (!input) return;
    var current = parseInt(input.value, 10) || 1;
    var max = parseInt(input.max, 10) || 99;
    var next = current + delta;
    if (next < 1) next = 1;
    if (next > max) next = max;
    input.value = next;
}

// 4. Chọn số sao đánh giá sản phẩm (1..5 sao)
function pickReviewStar(starCount) {
    var ratingInput = document.getElementById('reviewRatingInput');
    if (ratingInput) ratingInput.value = starCount;

    var starBtns = document.querySelectorAll('#starRatingPicker .star-pick-btn');
    for (var i = 0; i < starBtns.length; i++) {
        var val = parseInt(starBtns[i].getAttribute('data-value'), 10);
        var icon = starBtns[i].querySelector('i');
        if (val <= starCount) {
            starBtns[i].classList.add('active');
            if (icon) icon.className = 'bi bi-star-fill';
        } else {
            starBtns[i].classList.remove('active');
            if (icon) icon.className = 'bi bi-star';
        }
    }

    var labels = {
        1: 'Rất tệ (1/5)',
        2: 'Chưa hài lòng (2/5)',
        3: 'Bình thường (3/5)',
        4: 'Hài lòng (4/5)',
        5: 'Tuyệt vời (5/5)'
    };
    var labelEl = document.getElementById('ratingTextLabel');
    if (labelEl && labels[starCount]) {
        labelEl.textContent = labels[starCount];
    }
}

// 5. Bộ lọc nhanh khoảng giá & sắp xếp tại trang Danh sách sản phẩm
function setPriceRange(minVal, maxVal) {
    var minInput = document.getElementById('minPriceInput');
    var maxInput = document.getElementById('maxPriceInput');
    if (minInput) minInput.value = minVal !== '' ? minVal : '';
    if (maxInput) maxInput.value = maxVal !== '' ? maxVal : '';

    var form = document.getElementById('productFilterForm');
    if (form) form.submit();
}

function applySortOrder(sortVal) {
    var hiddenSort = document.getElementById('hiddenSortBy');
    var form = document.getElementById('productFilterForm');
    if (hiddenSort && form) {
        hiddenSort.value = sortVal;
        form.submit();
    }
}

// 6. Khởi tạo khi trang tải xong
document.addEventListener('DOMContentLoaded', function () {
    if (window.productVariantsData && window.productVariantsData.length > 0) {
        syncSelectedVariant();
    }

    // Tự động ẩn thông báo sau 5 giây
    var alerts = document.querySelectorAll('.alert-dismissible');
    if (alerts.length > 0) {
        setTimeout(function () {
            for (var i = 0; i < alerts.length; i++) {
                try {
                    var bsAlert = bootstrap.Alert.getOrCreateInstance(alerts[i]);
                    if (bsAlert) bsAlert.close();
                } catch (e) { }
            }
        }, 5000);
    }
});

