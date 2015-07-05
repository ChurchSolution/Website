var church = church || {};

church.viewModel = (function (window, undefined) {
    "use strict";

    function bulletinViewModel(data) {
        var self = this;

        this.selectedDate = ko.observable();
        this.calendar = ko.observable();
        this.searchBulletin = function () {
            church.dataModel.bulletinsModel.getByDate(self.selectedDate()).done(function (model) {
                self.date(model.bulletin.date);
                self.dateString(model.bulletin.properties.dateString);
                self.bulletinUrl(model.bulletin.fileUrl);
                self.url("/Home/Bulletin?date=" + model.bulletin.date);
                self.speaker(model.bulletin.speaker);
                self.title(model.bulletin.messageTitle);
                self.sermonUrl(model.sermon == null ? undefined : model.sermon.fileUrl);
            }).fail(function (xhr) {
                alert(xhr.statusText);
            })
        };
        this.uploadBulletin = function () {
            $('#uploadBulletinPanel').dialog({
                modal: true,
                resizable: false,
                width: 'auto',
                height: 'auto',
            });
        }

        this.date = ko.observable(data.date);
        this.dateString = ko.observable(data.properties.dateString);
        this.bulletinUrl = ko.observable(data.fileUri);
        this.url = ko.observable("/Home/Bulletin?date=" + data.date);
        this.speaker = ko.observable(data.properties.speaker);
        this.title = ko.observable(data.properties.messageTitle);
        this.sermonUrl = ko.observable(data.sermon == null ? undefined : data.sermon.fileUrl);
    }

    function sermonViewModel(dataModel) {
        var self = this;
        var promise = church.dataModel.sermonsModel.getBySpeakerDateTitle(dataModel.speaker, dataModel.date, dataModel.title);
        promise.done(function(data) {
            ko.mapping.fromJS(data, self);
        }).fail(function(xhr) {
            // Do nothing
        });

        ko.mapping.fromJS(dataModel, {}, self);
    }

    function messageViewModel() {
        var self = this;

        this.messageStyle = ko.observable('message-normal');
        this.message = ko.observable();
    }

    function uploadBulletinViewModel(messageViewModel) {
        var self = this;

        this.bulletinDate = ko.observable();
        this.calendar = ko.observable();
        this.textFile = ko.observable();
        this.textFileContent = ko.observable();
        this.printFile = ko.observable();
        this.printFileContent = ko.observable();
        this.uploadBulletin = function () {
            var date = self.bulletinDate();
            var textContent = self.textFileContent();
            var printContent = self.printFileContent();
            var promise = church.dataModel.bulletinsModel.post(date, textContent, printContent);
            promise.done(function (data) {
                messageViewModel.messageStyle('message-normal');
                messageViewModel.message('successful');
            }).fail(function (xhr) {
                messageViewModel.messageStyle('message-error');
                alert(xhr.statusText);
            }).always(function () {
                self.closeDialog();
            });
        };
        this.closeDialog = function () {
            $('#uploadBulletinPanel').dialog('close');
        };
    };

    function sermonsViewModel(data) {
        var sermonArray = [];
        data.value.forEach(function (element, index, array) {
            var item = [element.type, new Date(element.date).toLocaleDateString(), element.speaker, "<a href='" + element.fileUrl + "'>" + element.title + "</a>"];
            sermonArray.push(item);
        });
        this.sermons = ko.observableArray(sermonArray);
    };

    function materialsViewModel(data) {
        var materialArray = [];
        data.value.forEach(function (element, index, array) {
            var item = [element.type, new Date(element.date).toLocaleDateString(), element.authors, "<a href='" + element.fileUrl + "'>" + element.title + "</a>"];
            materialArray.push(item);
        });
        this.materials = ko.observableArray(materialArray);
    };

    function bibleViewModel(data) {
        var self = this;
        ko.mapping.fromJS(data, {}, self);

        var loadChapter = function (bibleId, bookOrder, chapterOrder) {
            self.errorMessage("");
            church.dataModel.bibleModel.getChapter(bibleId, bookOrder, chapterOrder).done(function (model) {
                ko.mapping.fromJS(model, self);
            }).fail(function (xhr) {
                self.errorMessage(xhr.responseText);
            });
        }
        var loadAbbreviation = function(bibleId, abbreviation) {
            self.errorMessage("");
            church.dataModel.bibleModel.getVerses(bibleId, abbreviation).done(function (model) {
                self.verses(model);
            }).fail(function (xhr) {
                self.errorMessage(xhr.responseText);
            });
        }
        var loadVerses = function (version, abbreviation) {
            abbreviation = abbreviation.replace(/^\s\s*/, "").replace(/\s\s*$/, "");
            if (abbreviation === "") {
                loadChapter(version, self.selectedBook(), self.selectedChapter());
            } else {
                loadAbbreviation(version, abbreviation);
            }
        }

        self.selectedVersion.subscribe(function (item) {
            loadVerses(item, self.abbreviation());
        });
        self.selectedBook.subscribe(function(item) {
            loadChapter(self.selectedVersion(), item, 1);
        });
        self.selectedChapter.subscribe(function(item) {
            loadChapter(self.selectedVersion(), self.selectedBook(), item);
        });
        self.abbreviation.subscribe(function (item) {
            loadVerses(self.selectedVersion(), item);
        });
        self.getVerses = function () {
            // Do nothing
        };
    }

    return {
        BulletinViewModel: bulletinViewModel,
        SermonViewModel: sermonViewModel,
        MessageViewModel: messageViewModel,
        UploadBulletinViewModel: uploadBulletinViewModel,
        SermonsViewModel: sermonsViewModel,
        MaterialsViewModel: materialsViewModel,
        BibleViewModel: bibleViewModel,
    };
})(window);
