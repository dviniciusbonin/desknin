(function () {
    const SIDEBAR_KEY = 'desknin-sidebar-hidden';
    const SIDEBAR_ID = 'desknin-sidebar';
    const TOGGLE_ID = 'desknin-sidebar-toggle';
    const ICON_ID = 'desknin-sidebar-toggle-icon';

    function getSidebar() {
        return document.getElementById(SIDEBAR_ID);
    }

    function isHidden() {
        return getSidebar().classList.contains('desknin-sidebar-hidden');
    }

    function setSidebarHidden(hidden) {
        var sidebar = getSidebar();
        if (hidden) {
            sidebar.classList.add('desknin-sidebar-hidden');
            localStorage.setItem(SIDEBAR_KEY, '1');
        } else {
            sidebar.classList.remove('desknin-sidebar-hidden');
            localStorage.removeItem(SIDEBAR_KEY);
        }
        updateToggleIcon(hidden);
    }

    function updateToggleIcon(hidden) {
        var icon = document.getElementById(ICON_ID);
        var btn = document.getElementById(TOGGLE_ID);
        if (!icon || !btn) return;
        if (hidden) {
            icon.innerHTML = '<line x1="3" y1="12" x2="21" y2="12"/><line x1="3" y1="6" x2="21" y2="6"/><line x1="3" y1="18" x2="21" y2="18"/>';
            btn.setAttribute('title', 'Show menu');
            btn.setAttribute('aria-label', 'Show menu');
        } else {
            icon.innerHTML = '<path d="M9 18l-6-6 6-6"/><line x1="3" y1="12" x2="21" y2="12"/>';
            btn.setAttribute('title', 'Hide menu');
            btn.setAttribute('aria-label', 'Hide menu');
        }
    }

    function init() {
        var sidebar = getSidebar();
        var btn = document.getElementById(TOGGLE_ID);
        if (!sidebar || !btn) return;

        if (localStorage.getItem(SIDEBAR_KEY) === '1') {
            setSidebarHidden(true);
        } else {
            updateToggleIcon(false);
        }

        btn.addEventListener('click', function () {
            setSidebarHidden(!isHidden());
        });
    }

    document.addEventListener('DOMContentLoaded', init);
})();
