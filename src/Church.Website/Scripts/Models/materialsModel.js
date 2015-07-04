var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.materialsModel = (function (window, undefined) {
    "use strict";

    function get() {
        return $.getJSON("/OData/Materials");
    };

    return {
        get: get,
    };
})(window);
