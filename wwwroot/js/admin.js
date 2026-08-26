(function ($) {
    'use strict';
    $(function () {
        // Confirm links
        $('[data-confirm]').on('click', function (e) {
            if (!confirm($(this).data('confirm'))) e.preventDefault();
        });
    });
})(jQuery);
