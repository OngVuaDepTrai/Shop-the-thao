
document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.getElementById("adminSidebar");
    const overlay = document.getElementById("sidebarOverlay");

    const toggleButton = document.getElementById("sidebarToggle");
    const closeButton = document.getElementById("sidebarClose");

    function openSidebar() {
        sidebar.classList.add("show");
        overlay.classList.add("show");
        document.body.style.overflow = "hidden";
    }

    function closeSidebar() {
        sidebar.classList.remove("show");
        overlay.classList.remove("show");
        document.body.style.overflow = "";
    }

    if (toggleButton) {
        toggleButton.addEventListener("click", function () {
            if (sidebar.classList.contains("show")) {
                closeSidebar();
            } else {
                openSidebar();
            }
        });
    }

    if (closeButton) {
        closeButton.addEventListener("click", closeSidebar);
    }

    if (overlay) {
        overlay.addEventListener("click", closeSidebar);
    }

    // Đóng sidebar khi chọn menu trên mobile
    document.querySelectorAll(".sidebar-link").forEach(function (link) {
        link.addEventListener("click", function () {
            if (window.innerWidth < 992) {
                closeSidebar();
            }
        });
    });

    // Nếu chuyển sang desktop, loại bỏ trạng thái mobile
    window.addEventListener("resize", function () {
        if (window.innerWidth >= 992) {
            closeSidebar();
        }
    });

});
