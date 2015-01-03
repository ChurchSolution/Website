/**
 * knockout.dataTables.js v1.0
 *
 * Copyright (c) 2011, Josh Buckley (joshbuckley.co.uk).
 * License: MIT (http://www.opensource.org/licenses/mit-license.php)
 * 
 * Example Usage:
 *
 * Using only a data souce. See http://jsfiddle.net/vB3Aj/ for demo
 *     <table data-bind="dataTable: myData">...
 *
 * Using object syntax to apply options to DataTables. 
 * See http://jsfiddle.net/tdppH/1/ for demo
 *     <table data-bind="dataTable: {data: myData, options: { key: val } }">
 */
(function ($) {
    ko.bindingHandlers.dataTable = {
        init: function (element, valueAccessor) {
            var binding = ko.utils.unwrapObservable(valueAccessor());

            // If the binding is an object with an options field,
            // initialise the dataTable with those options. 
            if (binding.options) {
                $(element).dataTable(binding.options);
            }
        },
        update: function (element, valueAccessor) {
            var binding = ko.utils.unwrapObservable(valueAccessor());

            // If the binding isn't an object, turn it into one. 
            if (!binding.data) {
                binding = { data: valueAccessor() }
            }

            // Clear table
            $(element).dataTable().fnClearTable();

            // Rebuild table from data source specified in binding
            $(element).dataTable().fnAddData(binding.data());
        }
    };
})(jQuery);