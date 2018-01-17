
(function ($) {
    $.fn.renderTemplate = function (templateName, data) {
        return this.each(function (index, Element) {
            var elem = $(this);
            var instruction = 'ich.' + templateName + '({"data" : data}, true);';
            var tmpl = eval(instruction);
            elem.html(tmpl);
        });
    };
})(jQuery);


(function ($) {
    $.fn.reset = function () {
        return this.each(function () {
            this.reset();
        });
    };
})(jQuery);



(function ($) {
    $.fn.loadView = function (url) {
        return this.each(function (index, Element) {
            $(this).html(http.get(url));
        });
    };
})(jQuery);


