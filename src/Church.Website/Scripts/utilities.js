var church = church || {};

church.utilities = (function (window, $, undefined) {
    "use strict";

    function searchBibleVerses() {
        var panels = arguments;

        var promise = church.dataModel.bibleModel.getVersePattern("");
        promise.done(function (response) {
            var re = new RegExp("(" + response + ")", "g");
            for (var loop = 0; loop < panels.length; loop++) {
                //TODO Current panel could not hold javascript code, need to update this
                var updated = $(panels[loop]).html().replace(re, '<a class="verseLink" href="#">$1</a>');
                $(panels[loop]).html(updated);
            }
            var links = $(".verseLink");
            links.click(function () {
                var abbreviation = $(this).text();
                showVerses(abbreviation);
            });
        }).fail(function (xhr) {
            alert(xhr.responseJSON.exceptionMessage);
        });
    }

    function showVerses(abbreviation) {
        // : cannot be transferred in RESTful
        var promise = church.dataModel.bibleModel.getVerses(abbreviation, "");
        promise.done(function (response) {
            var lines = [];
            $.each(response.verses, function () {
                lines.push(response.selectedChapter + ':' + this.order + ' ' + this.value);
            });
            var dig = '<div class="ui-helper-hidden" title="' + abbreviation + '">' + lines.join('<br />') + '</div>';
            $(dig).dialog({
                resizable: false,
                width: "auto",
                height: "auto"
            });
        }).fail(function (xhr) {
            alert(xhr.responseJSON.exceptionMessage);
        });
    }

    //Website.NailHymns = function () {
    //    var panels = arguments;
    //    $.each(panels, function (index, panel) {
    //        $(panel).click(function () {
    //            var _this = this;
    //            if (null == $(_this).next().html()) {
    //                var url = "/Library/Hymns/" + $(_this).attr('itemid');
    //                var parts = url.split('@');
    //                $.getJSON(encodeURI(parts[0] + '?_=' + Website.S4() + Website.S4()), { source: parts[1] })
    //                    .success(function (hymn) {
    //                        var lyrics = JSON.parse(hymn.Lyrics);
    //                        var lines = [];
    //                        $.each(lyrics, function () {
    //                            lines.push((this.Type == 1 ? '* ' : '') + this.Text.join('  '));
    //                        });
    //                        $(_this).after('<span><br />' + lines.join('<br />') + '</span>');
    //                    })
    //                    .error(function (e) {
    //                        //TODO Message should be nice
    //                        alert(e.responseText);
    //                    });
    //            } else {
    //                $(_this).next().toggle();
    //            }
    //            return false;
    //        });
    //    });
    //}

    return {
        searchBibleVerses: searchBibleVerses,
    };
})(window, jQuery);
