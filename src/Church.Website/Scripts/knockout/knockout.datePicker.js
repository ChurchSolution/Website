(function ($) {
    ko.bindingHandlers.datePicker = {
        init: function (element, valueAccessor) {
            var binding = ko.utils.unwrapObservable(valueAccessor());

            // If the binding is an object with an options field,
            // initialise the dataTable with those options. 
            //if (binding.options) {
            //    $(element).datePicker(binding.options);
            //}
        },
        update: function (element, valueAccessor) {
            var binding = ko.utils.unwrapObservable(valueAccessor());

            //// If the binding isn't an object, turn it into one. 
            //if (!binding.data) {
            //    binding = { data: valueAccessor() }
            //}

            $(element).datepicker();
        }
    };
})(jQuery);
