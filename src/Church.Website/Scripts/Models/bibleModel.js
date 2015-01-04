var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.bibleModel = (function (window, undefined) {
    "use strict";

    function get(book, chapter, id) {
        return $.getJSON(encodeURI("/api/bible/" + book + "/" + chapter + "?id=" + id));
    };

    function getVersePattern(culture) {
        return $.getJSON(encodeURI("/api/bible/versePattern?culture=" + culture));
    };

    function getVerses(abbreviation, id) {
        return $.getJSON(encodeURI("/api/bible/" + abbreviation + "?id=" + id));
    };

    return {
        get: get,
        getVersePattern: getVersePattern,
        getVerses: getVerses,
    };
})(window);