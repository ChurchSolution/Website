var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.materialsModel = (function (window, undefined) {
    "use strict";

    function get() {
        return $.getJSON("/api/materials");
    };

    return {
        get: get,
    };
})(window);
