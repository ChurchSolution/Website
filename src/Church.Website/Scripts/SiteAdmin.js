window.Church.Website.Incident = {};

var Incident = window.Church.Website.Incident;

Incident.initialize = function () {
    $('#createIncidentButton').click(function () {
        $('#CreateIncidentPanel').dialog({
            modal: true,
            resizable: false,
            width: 'auto',
            height: 'auto'
        });
    });
    $('#createIncidentCloseButton').click(function (e) {
        e.preventDefault();
        $('#CreateIncidentPanel').dialog('close');
    });
}

Incident.onCreateComplete = function () {
    location.reload();
    //$('#CreateIncidentPanel').dialog('close');
}

Incident.onRemoveComplete = function () {
}

Incident.reload = function () {
    $.ajax(
        { type: "GET",
            url: '/Member/ReloadIncidents/',
            data: "{}",
            cache: false,
            dataType: "html",
            success: function (data)
            { $().html(data); }
        });
}

window.Church.Website.Bulletin = {};

var Bulletin = window.Church.Website.Bulletin;

Bulletin.initialize = function () {
    $('#uploadBulletinButton').click(function () {
        $('#uploadBulletinPanel').dialog({
            modal: true,
            resizable: false,
            width: 'auto',
            height: 'auto'
        });
    });
    $("#uploadBulletinPanel #Date").datepicker();
    $('#uploadBulletinDialogCloseButton').click(function () {
        $('#uploadBulletinPanel').dialog('close');
    });
}

Bulletin.onRemoveComplete = function () {
}

window.Church.Website.Sermon = {};

var Sermon = window.Church.Website.Sermon;

Sermon.initialize = function () {
    $('#uploadSermonButton').click(function () {
        Sermon.uploadButton('', '', '');
        $('#uploadSermonPanel').dialog({
            modal: true,
            resizable: false,
            width: 'auto',
            height: 'auto'
        });
    });
    $("#SermonDate").datepicker();
    $('#uploadSermonDialogCloseButton').click(function () {
        $('#uploadSermonPanel').dialog('close');
    });
}

Sermon.uploadButton = function (date, title, speaker) {
    $('#uploadSermonPanel #SermonDate').val(date);
    $('#uploadSermonPanel #Title').val(title);
    $('#uploadSermonPanel #Speaker').val(speaker);
    $('#uploadSermonPanel').dialog({
        modal: true,
        resizable: false,
        width: 'auto',
        height: 'auto'
    });
}

Sermon.onRemoveComplete = function () {
}

window.Church.Website.Material = {};

var Material = window.Church.Website.Material;

Material.initialize = function () {
    $('#addMaterialButton').click(function () {
        $('#ManageMaterialPanel').dialog({
            modal: true,
            resizable: false,
            width: 'auto',
            height: 'auto'
        });
    });
    $("#MaterialDate").datepicker();
    $('#ManageMaterialCloseButton').click(function (e) {
        e.preventDefault();
        $('#ManageMaterialPanel').dialog('close');
    });
}

Material.onRemoveComplete = function () {
}
