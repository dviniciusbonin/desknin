(function () {
    const SIDEBAR_KEY = 'desknin-sidebar-hidden';
    const SIDEBAR_ID = 'desknin-sidebar';
    const TOGGLE_ID = 'desknin-sidebar-toggle';
    const ICON_ID = 'desknin-sidebar-toggle-icon';
    const BACKDROP_ID = 'desknin-sidebar-backdrop';
    const MOBILE_BREAKPOINT = 768;

    const HAMBURGER_ICON = '<line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="18" x2="21" y2="18"/>';
    const ARROW_LEFT_ICON = '<path d="M9 18l-6-6 6-6"/><line x1="3" y1="12" x2="21" y2="12"/>';
    const CLOSE_ICON = '<line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>';

    function getSidebar() {
        return document.getElementById(SIDEBAR_ID);
    }

    function getBackdrop() {
        return document.getElementById(BACKDROP_ID);
    }

    function isMobile() {
        return window.matchMedia('(max-width: ' + (MOBILE_BREAKPOINT - 0.02) + 'px)').matches;
    }

    function isCollapsed() {
        return getSidebar().classList.contains('desknin-sidebar-hidden');
    }

    function isOverlayOpen() {
        return getSidebar().classList.contains('desknin-sidebar-open');
    }

    function setSidebarCollapsed(collapsed) {
        var sidebar = getSidebar();
        if (collapsed) {
            sidebar.classList.add('desknin-sidebar-hidden');
            localStorage.setItem(SIDEBAR_KEY, '1');
        } else {
            sidebar.classList.remove('desknin-sidebar-hidden');
            localStorage.removeItem(SIDEBAR_KEY);
        }
    }

    function setOverlayOpen(open) {
        var sidebar = getSidebar();
        var backdrop = getBackdrop();
        if (!backdrop) return;
        if (open) {
            sidebar.classList.add('desknin-sidebar-open');
            backdrop.classList.add('show');
            backdrop.setAttribute('aria-hidden', 'false');
        } else {
            sidebar.classList.remove('desknin-sidebar-open');
            backdrop.classList.remove('show');
            backdrop.setAttribute('aria-hidden', 'true');
        }
    }

    function updateToggleIcon() {
        var icon = document.getElementById(ICON_ID);
        var btn = document.getElementById(TOGGLE_ID);
        if (!icon || !btn) return;

        if (isMobile()) {
            var open = isOverlayOpen();
            icon.innerHTML = open ? CLOSE_ICON : HAMBURGER_ICON;
            btn.setAttribute('title', open ? 'Close menu' : 'Open menu');
            btn.setAttribute('aria-label', open ? 'Close menu' : 'Open menu');
        } else {
            var collapsed = isCollapsed();
            icon.innerHTML = collapsed ? HAMBURGER_ICON : ARROW_LEFT_ICON;
            btn.setAttribute('title', collapsed ? 'Show menu' : 'Hide menu');
            btn.setAttribute('aria-label', collapsed ? 'Show menu' : 'Hide menu');
        }
    }

    function init() {
        var sidebar = getSidebar();
        var btn = document.getElementById(TOGGLE_ID);
        var backdrop = getBackdrop();
        if (!sidebar || !btn) return;

        if (isMobile()) {
            setOverlayOpen(false);
        } else {
            if (localStorage.getItem(SIDEBAR_KEY) === '1') {
                setSidebarCollapsed(true);
            }
        }
        updateToggleIcon();

        btn.addEventListener('click', function () {
            if (isMobile()) {
                setOverlayOpen(!isOverlayOpen());
            } else {
                setSidebarCollapsed(!isCollapsed());
            }
            updateToggleIcon();
        });

        if (backdrop) {
            backdrop.addEventListener('click', function () {
                if (isMobile() && isOverlayOpen()) {
                    setOverlayOpen(false);
                    updateToggleIcon();
                }
            });
        }

        sidebar.addEventListener('click', function (e) {
            if (!isMobile()) return;
            var link = e.target.closest('a.nav-link');
            if (link) {
                setOverlayOpen(false);
                updateToggleIcon();
            }
        });

        window.addEventListener('resize', function () {
            if (!isMobile()) {
                setOverlayOpen(false);
            }
            updateToggleIcon();
        });
    }

    document.addEventListener('DOMContentLoaded', init);
})();
