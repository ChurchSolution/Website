var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.sermonsModel = (function (window, undefined) {
    "use strict";

    function get() {
        return $.getJSON("/api/sermons");
    };

    return {
        get: get,
    };
})(window);
