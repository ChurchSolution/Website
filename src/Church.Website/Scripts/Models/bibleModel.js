var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.bibleModel = (function (window, undefined) {
    "use strict";

    function getVersion(id) {
        return $.getJSON(encodeURI("/bible/" + id));
    };

    function getBook(id, book) {
        return $.getJSON(encodeURI("/bible/" + id + "/book/" + book));
    };

    function getChapter(id, book, chapter) {
        return $.getJSON(encodeURI("/bible/" + id + "/book/" + book + "/chapter/" + chapter));
    };

    function getAbbreviations(id) {
        return $.getJSON(encodeURI("/bible/" + id + "/abbreviations"));
    };

    function getVerses(id, abbreviation) {
        return $.getJSON(encodeURI("/bible/" + id + "/abbreviations/" + abbreviation));
    };

    return {
        getVersion: getVersion,
        getBook: getBook,
        getChapter: getChapter,
        getAbbreviations: getAbbreviations,
        getVerses: getVerses
    };
})(window);