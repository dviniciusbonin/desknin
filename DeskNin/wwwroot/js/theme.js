(function () {
    const THEME_KEY = 'desknin-theme';
    const ROOT = document.documentElement;

    function getStoredTheme() {
        return localStorage.getItem(THEME_KEY) || 'light';
    }

    function setTheme(theme) {
        ROOT.setAttribute('data-bs-theme', theme);
        localStorage.setItem(THEME_KEY, theme);
        updateToggleIcon(theme);
    }

    function updateToggleIcon(theme) {
        document.querySelectorAll('.theme-toggle-btn').forEach(function (btn) {
            const icon = btn.querySelector('svg');
            const label = btn.querySelector('.theme-label');
            if (theme === 'dark') {
                if (icon) icon.innerHTML = '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"/>';
                if (label) label.textContent = 'Light theme';
            } else {
                if (icon) icon.innerHTML = '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"/>';
                if (label) label.textContent = 'Dark theme';
            }
        });
    }

    function initTheme() {
        const theme = getStoredTheme();
        setTheme(theme);
    }

    function toggleTheme() {
        const current = getStoredTheme();
        setTheme(current === 'dark' ? 'light' : 'dark');
    }

    document.addEventListener('DOMContentLoaded', function () {
        initTheme();

        document.querySelectorAll('.theme-toggle-btn').forEach(function (btn) {
            btn.addEventListener('click', toggleTheme);
        });
    });

    window.DeskNinTheme = { setTheme, toggleTheme, getStoredTheme };
})();
