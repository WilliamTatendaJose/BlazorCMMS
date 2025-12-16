// Theme Management - Early initialization before Blazor loads
(function() {
    'use strict';

    window.themeManager = {
        // Initialize theme on page load
        init: function() {
            const savedTheme = localStorage.getItem('rbm-theme') || 'light';
            this.applyTheme(savedTheme);
        },

        // Apply theme by adding/removing dark-mode class
        applyTheme: function(theme) {
            const html = document.documentElement;
            
            if (theme === 'dark') {
                html.classList.add('dark-mode');
                html.setAttribute('data-theme', 'dark');
                localStorage.setItem('rbm-theme', 'dark');
            } else {
                html.classList.remove('dark-mode');
                html.setAttribute('data-theme', 'light');
                localStorage.setItem('rbm-theme', 'light');
            }
            
            console.log('Theme applied:', theme);
        },

        // Toggle between light and dark mode
        toggleTheme: function() {
            const html = document.documentElement;
            const isDark = html.classList.contains('dark-mode');
            this.applyTheme(isDark ? 'light' : 'dark');
            return !isDark ? 'dark' : 'light';
        },

        // Get current theme
        getCurrentTheme: function() {
            return document.documentElement.classList.contains('dark-mode') ? 'dark' : 'light';
        }
    };

    // Initialize immediately on script load
    window.themeManager.init();

    // Also initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            window.themeManager.init();
        });
    }

    // Listen for storage changes (for syncing across tabs)
    window.addEventListener('storage', function(e) {
        if (e.key === 'rbm-theme' && e.newValue) {
            window.themeManager.applyTheme(e.newValue);
        }
    });
})();
