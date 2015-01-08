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
            alert(xhr.statusText);
        });
    }

    function showVerses(abbreviation) {
        // ':' cannot be transferred in RESTful
        var promise = church.dataModel.bibleModel.getVerses(abbreviation, "");
        promise.done(function (response) {
            var lines = [];
            $.each(response.verses, function () {
                lines.push(response.selectedChapter + ':' + this.id + ' ' + this.text);
            });
            var dig = '<div class="ui-helper-hidden" title="' + abbreviation + '">' + lines.join('<br />') + '</div>';
            $(dig).dialog({
                resizable: false,
                width: "auto",
                height: "auto"
            });
        }).fail(function (xhr) {
            alert(xhr.statusText);
        });
    }

    function nailHymns() {
        var panels = arguments;

        $.each(panels, function (index, panel) {
            $(panel).click(function () {
                var self = this;
                if (null == $(self).next().html()) {
                    var parts = $(self).attr('itemid').split('@');
                    var promise = church.dataModel.hymnsModel.getByNameSource(parts[0], parts[1]);
                    promise.done(function (response) {
                        var lyrics = JSON.parse(response.lyrics);
                            var lines = [];
                            $.each(lyrics, function () {
                                lines.push((this.Type == 1 ? '* ' : '') + this.Text.join('  '));
                            });
                            $(self).after('<span><br />' + lines.join('<br />') + '</span>');
                        })
                        .fail(function (xhr) {
                            // Do nothing
                        });
                } else {
                    $(self).next().toggle();
                }
                return false;
            });
        });
    }

    return {
        searchBibleVerses: searchBibleVerses,
        nailHymns: nailHymns,
    };
})(window, jQuery);
