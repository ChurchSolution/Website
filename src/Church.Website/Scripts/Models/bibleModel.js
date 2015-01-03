var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.bibleModel = (function (window, undefined) {
    "use strict";

    function getByCulture(culture) {
        return $.getJSON("/api/bible?culture=" + culture);
    };

    function get(id, book, chapter) {
        return $.getJSON("/api/bible/" + id + "?bookOrder=" + book + "&chapterOrder=" + chapter);
    };

    return {
        getByCulture: getByCulture,
        get: get,
    };
})(window);