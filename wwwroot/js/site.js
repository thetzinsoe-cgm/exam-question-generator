(function () {
    // Global site.js
    'use strict';
    document.addEventListener('DOMContentLoaded', function () {
        // Auto-render math for any KaTeX delimiters ($...$) outside Quill editors
        if (window.renderMathInElement) {
            document.querySelectorAll('.math-content').forEach(function (el) {
                window.renderMathInElement(el, {
                    delimiters: [
                        { left: '$$', right: '$$', display: true },
                        { left: '$', right: '$', display: false },
                        { left: '\\(', right: '\\)', display: false },
                        { left: '\\[', right: '\\]', display: true }
                    ],
                    throwOnError: false
                });
            });
        }
    });
})();
