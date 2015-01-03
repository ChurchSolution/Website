var church = church || {};

church.viewModel = (function (window, undefined) {
    "use strict";

    function bulletinViewModel(data) {
        var self = this;

        this.selectedDate = ko.observable();
        this.calendar = ko.observable();
        this.searchBulletin = function () {
            church.dataModel.bulletinsModel.getByDate(self.selectedDate()).done(function (data) {
                self.date(data.bulletin.date);
                self.bulletinUrl(data.bulletin.fileUrl);
                self.url("/Home/Bulletin?date=" + data.bulletin.date);
                self.speaker(data.bulletin.speaker);
                self.title(data.bulletin.messageTitle);
                self.sermonUrl(data.sermon == null ? undefined : data.sermon.fileUrl);
            }).fail(function (xhr) {
                alert(xhr.responseJSON.ExceptionMessage);
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
        this.bulletinUrl = ko.observable(data.fileUri);
        this.url = ko.observable("/Home/Bulletin?date=" + data.date);
        this.speaker = ko.observable(data.properties.speaker);
        this.title = ko.observable(data.properties.messageTitle);
        this.sermonUrl = ko.observable(data.sermon == null ? undefined : data.sermon.fileUrl);
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
                alert(xhr.responseJSON.ExceptionMessage);
            }).always(function () {
                self.closeDialog();
            });
        };
        this.closeDialog = function () {
            $('#uploadBulletinPanel').dialog('close');
        };
    };

    function _arrayBufferToBase64(buffer) {
        var binary = ''
        var bytes = new Uint8Array(buffer)
        var len = bytes.byteLength;
        for (var i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i])
        }
        return window.btoa(binary);
    }

    function sermonsViewModel(data) {
        var sermonArray = [];
        data.forEach(function (element, index, array) {
            var item = [element.type, new Date(element.date).toLocaleDateString(), element.speaker, "<a href='" + element.fileUrl + "'>" + element.title + "</a>"];
            sermonArray.push(item);
        });
        this.sermons = ko.observableArray(sermonArray);
    };

    function materialsViewModel(data) {
        var materialArray = [];
        data.forEach(function (element, index, array) {
            var item = [element.type, new Date(element.date).toLocaleDateString(), element.authors, "<a href='" + element.fileUrl + "'>" + element.title + "</a>"];
            materialArray.push(item);
        });
        this.materials = ko.observableArray(materialArray);
    };

    function bibleViewModel(data) {
        var self = this;
        var getBible = function (version, book, chapter) {
            var promise = church.dataModel.bibleModel.get(version, book, chapter);
            promise.done(function (data) {
                ko.mapping.fromJS(data, self);
            }).fail(function (xhr) {
                alert(xhr.responseJSON.ExceptionMessage);
            });
        };

        ko.mapping.fromJS(data, {}, self);
        self.selectedVersion.subscribe(function (item) {
            getBible(item, self.selectedBook(), self.selectedChapter());
        });
        self.selectedBook.subscribe(function (item) {
            getBible(self.selectedVersion(), item, 1);
        });
        self.selectedChapter.subscribe(function (item) {
            getBible(self.selectedVersion(), self.selectedBook(), item);
        })
    }

    return {
        BulletinViewModel: bulletinViewModel,
        MessageViewModel: messageViewModel,
        UploadBulletinViewModel: uploadBulletinViewModel,
        SermonsViewModel: sermonsViewModel,
        MaterialsViewModel: materialsViewModel,
        BibleViewModel: bibleViewModel,
    };
})(window);
