$(document).ready(function () {
    jQuery(function ($) {
        $.validator.addMethod('date',
        function (value, element) {
            if (this.optional(element)) {
                return true;
            }

            var ok = true;
            try {
                $.datepicker.parseDate('dd/mm/yy', value);
            }
            catch (err) {
                ok = false;
            }
            return ok;
        });
    });

    $('.datefield').datepicker({
        dateFormat: "dd/mm/yy"
    });
});