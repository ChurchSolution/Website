var church = church || {};
church.dataModel = church.dataModel || {};

church.dataModel.bulletinsModel = (function (window, undefined) {
    "use strict";
    var url = "/api/bulletins";

    function getById(id) {
        return $.getJSON(url + "/" + id);
    };

    function getByDate(date) {
        return $.getJSON(url + "?date=" + encodeURI(date));
    }

    function post(date, textBinary, printBinary) {
        return $.ajax({
            url: encodeURI(url),
            type: "POST",
            data: JSON.stringify({ date: date, textFileContent: textBinary, printFileContent: printBinary }),
            contentType: "application/json",
            dataType: "json",
        });
    }

    return {
        getById: getById,
        getByDate: getByDate,
        post: post,
    };
})(window);