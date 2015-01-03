if (window.Type) {
    Type.registerNamespace('Church');
}

if (!window.Church) {
    window.Church = {};
}

if (!window.Church.Website) {
    window.Church.Website = {};
}

var Website = window.Church.Website;

Website.S4 = function () {
    return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
}

Website.fixPaging = function (panel, url, params) {
    $.each($(panel + ' table tfoot tr td a'), function () {
        var link = $(this).attr('href');
        var tmp = [];
        var para = link.substring(link.indexOf('?'));
        tmp.push(url + para);
        if (para.indexOf('&_=') < 0) {
            if ('' != params) {
                tmp.push(params);
            }
            tmp.push('_=' + Website.S4() + Website.S4());
        }
        var str = tmp.join('&');
        $(this).click(function (e) {
            e.preventDefault();
            $(panel).load(str, function (response, status, xhr) {
                if ('error' == status) {
                    alert(response);
                }
                //                Website.fixPaging(panel, url, params);
            });
            return false;
        });
    });
}

Website.getParameters = function (keys) {
    var tmp = [];
    $.each(keys, function () {
        var value = escape($('#' + this).val());
        if (value != undefined && '' != value) {
            tmp.push(this + '=' + value);
        }
    });
    return tmp.join('&');
}

Website.SearchBibleVerses = function () {
    var panels = arguments;
    $.getJSON("/Bible/VersePattern")
        .success(function (response) {
            if (!response.Pattern || 0 === response.Pattern.length) {
                return;
            }
            var re = new RegExp("(" + response.Pattern + ")", "g");
            for (var loop = 0; loop < panels.length; loop++) {
                //TODO Current panel could not hold javascript code, need to update this
                var updated = $(panels[loop]).html().replace(re, '<a class="VerseLink" href="#">$1</a>');
                $(panels[loop]).html(updated);
            }
            links = $(".VerseLink");
            links.click(function () {
                var thisVerse = $(this).text();
                // : cannot be transferred in RESTful
                $.getJSON(encodeURI("/Bible/Verses/" + thisVerse.replace(/:/g, '|')))
                    .success(function (response) {
                        var lines = [];
                        $.each(response.Verses, function () {
                            lines.push(this.Chapter + ':' + this.Order + ' ' + this.Text);
                        });
                        var dig = '<div class="ui-helper-hidden" title="' + thisVerse + '">' + lines.join('<br />') + '</div>';
                        $(dig).dialog({
                            resizable: false,
                            width: "auto",
                            height: "auto"
                        });
                    })
                    .error(function (e) {
                        //TODO Message should be nice
                        alert(e.responseText);
                    });
            });
        })
        .error(function (e) {
            //TODO Message should be nice
            alert(e.responseText);
    });
}

Website.NailHymns = function () {
    var panels = arguments;
    $.each(panels, function (index, panel) {
        $(panel).click(function () {
            var _this = this;
            if (null == $(_this).next().html()) {
                var url = "/Library/Hymns/" + $(_this).attr('itemid');
                var parts = url.split('@');
                $.getJSON(encodeURI(parts[0] + '?_=' + Website.S4() + Website.S4()), { source: parts[1] })
                    .success(function (hymn) {
                        var lyrics = JSON.parse(hymn.Lyrics);
                        var lines = [];
                        $.each(lyrics, function () {
                            lines.push((this.Type == 1 ? '* ' : '') + this.Text.join('  '));
                        });
                        $(_this).after('<span><br />' + lines.join('<br />') + '</span>');
                    })
                    .error(function (e) {
                        //TODO Message should be nice
                        alert(e.responseText);
                    });
            } else {
                $(_this).next().toggle();
            }
            return false;
        });
    });
}

Website.loadBible = function (selectedBook, selectedChapter, callback) {
    $.getJSON(encodeURI('~/../../../' + selectedBook + '/Chapters/' + selectedChapter + '?' + $('#selectedVersionParam').val() + '&_=' + Website.S4() + Website.S4()))
        .success(callback)
        .error(function (e) {
            //TODO Message should be nice
            alert(e.responseText);
        });
}

Website.updateBooks = function (books, panel) {
    var lines = [];
    $.each(books, function (index, book) {
        lines.push(book.Order + '">' + book.Name);
    });

    var index = $(panel).prop("selectedIndex");
    $(panel).empty()
        .append('<option value="' + lines.join('</option><option value="') + '</option>');
    $(panel).prop("selectedIndex", index);
}

Website.updateChapters = function (chapters, panel) {
    var lines = [];
    $.each(chapters, function (index, chapter) {
        lines.push(chapter.toString());
    });
    $(panel).empty()
        .append('<option>' + lines.join('</option><option>') + '</option>');
}

Website.updateVerses = function (verses, panel) {
    var lines = [];
    $.each(verses, function (index, verse) {
        lines.push((index + 1).toString() + ' ' + verse);
    });
    $(panel).empty()
        .append('<div>' + lines.join('</div><div>') + '</div>');
}

Website.loadVerses = function (e, abbreviation, versesPattern, bookPanel, chapterPanel, panel) {
    if (event.which == 13 || event.keyCode == 13) {
        var regexp = new RegExp("^(" + versesPattern + ")$", "g");
        if (null == abbreviation.trim().match(regexp)) {
            alert("The string is incorrect, please check.");
            return true;
        }
        // Workaround for ':'
        var updatedAbbr = abbreviation.trim().replace(':', '|');
        $.getJSON(encodeURI('~/../../../../Verses/' + updatedAbbr + '?_=' + Website.S4() + Website.S4()))
            .success(function (response) {
                $(bookPanel + " option").filter(function () {
                    //may want to use $.trim in here
                    return $(this).text() == response.Book;
                }).prop('selected', true);
                Website.updateChapters(response.Chapters, chapterPanel);
                $(chapterPanel + " option").filter(function () {
                    //may want to use $.trim in here
                    return $(this).text() == response.Verses[0].Chapter.toString();
                }).prop('selected', true);
                var lines = [];
                $.each(response.Verses, function (index, verse) {
                    lines.push(verse.Order.toString() + ' ' + verse.Text);
                });
                $(panel).empty()
                    .append('<div>' + lines.join('</div><div>') + '</div>');
            })
            .error(function (e) {
                //TODO Message should be nice
                alert(e.responseText);
            });
        return false;
    }
    return true;
}
