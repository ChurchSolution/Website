var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.sermonsModel = (function (window, undefined) {
    "use strict";
    var url = "/api/sermons";

    function get() {
        return $.getJSON(url);
    };

    function getBySpeakerDateTitle(speaker, date, title) {
        return $.getJSON(encodeURI(url + "/0?speaker=" + speaker + "&date=" + date + "&title=" + title));
    };

    return {
        get: get,
        getBySpeakerDateTitle: getBySpeakerDateTitle,
    };
})(window);
