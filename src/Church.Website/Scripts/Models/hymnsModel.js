var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.hymnsModel = (function (window, undefined) {
    "use strict";
    var url = "/api/hymns";

    function get() {
        return $.getJSON(url);
    };

    function getByNameSource(name, source) {
        return $.getJSON(encodeURI(url + "/0?name=" + name + "&source=" + source));
    };

    return {
        get: get,
        getByNameSource: getByNameSource,
    };
})(window);
