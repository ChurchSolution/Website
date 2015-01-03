(function ($) {
    ko.bindingHandlers.file = {
        init: function (element, valueAccessor) {
            $(element).change(function () {
                if (this.files.length > 0) {
                    var accessor = valueAccessor();
                    var file = this.files[0];
                    if (ko.isObservable(accessor)) {
                        accessor(file);
                    }
                }
            });
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            var bindings = allBindingsAccessor();
            if (bindings.fileContent && ko.isObservable(bindings.fileContent)) {
                var file = ko.utils.unwrapObservable(valueAccessor());
                if (!file) {
                    bindings.fileContent(null);
                } else {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        bindings.fileContent(e.target.result);
                    }
                    if (file.type.indexOf('text/') == 0) {
                        reader.readAsText(file);
                    }
                    else {
                        reader.readAsDataURL(file);
                    }
                }
            }
        }
    };
})(jQuery);
