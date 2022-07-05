function LoadAll() {
    LoadVALinksUCI();
    LoadTypesUCI();
}

function buildQueryFilter(field, value, and) {
    if (and) {
        return " and " + field + " eq '" + value + "'";
    } else {
        return field + " eq '" + value + "'";
    }
}

function buildQueryFilterNonString(field, value, and) {
    if (and) {
        return " and " + field + " eq " + value;
    } else {
        return field + " eq " + value;
    }
}

/* ***** UCI Code for Edge and Chrome  browsers ***** */

function LoadVALinksUCI() {
    var filter = "?$filter="
    filter += buildQueryFilterNonString("va_type", "953850000", false);
    filter += buildQueryFilterNonString("statecode", 0, true);
    filter += "&$orderby=va_description asc"

    Xrm.WebApi.retrieveMultipleRecords("va_systemsettings", filter)
        .then(
            function reply(response) {
                finishLoadVALinksUCI(response.value);
            }
        );
}

function LoadTypesUCI() {
    var filter = "?$filter="
    filter += "udo_order ne null";
    filter += "&$orderby=udo_name asc"

    Xrm.WebApi.retrieveMultipleRecords("udo_requesttype", filter)
        .then(
            function reply(response) {
                finishLoadTypesUCI(response.value);
            }
        );
}

function finishLoadVALinksUCI(links) {
    if (!links || links.length == 0) {
        alert('System Settings lookup data does not contain any External Links settings.');
        return;
    }
    var s = '';
    for (var i = 0; i < links.length; i++) {
        if (!links[i].va_name) continue;
        var desc = links[i].va_description;

        s += '<li><a id="relLink' + i.toString() + '" style="text-decoration: underline; cursor: pointer;" href="' +
            links[i].va_name + '" target="_blank" title="Click to Open ' + desc + '">' + desc + '</a></li>';
    }

    if (s.length == 0) {
        s = 'No VA Links Found';
    } else {
        s = "<ul>" + s + "</ul>";
    }

    var elem3 = document.getElementById('step'); if (elem3) { elem3.innerHTML = s; }
}

function finishLoadTypesUCI(data) {
    if (!data || data.length == 0) {
        return;
    }

    var elem = document.getElementById('callscripts'); if (elem) { elem.innerHTML = '<ul id="callscripts-ul" />'; }
    var ul = $("#callscripts-ul");

    for (var i = 0; i < data.length; i++) {
        ul.append('<li id="type-' + data[i].udo_requesttypeid + '" style="display:none"><a href="javascript:;">' + data[i].udo_name + '</a><ul style="display:none" /></li>');
        var li = $("#type-" + data[i].udo_requesttypeid).click(function () {
            $(this).children("ul:first").toggle();
        });
    }

    LoadSubTypesUCI();
}

function LoadSubTypesUCI() {
    var filter = "?$filter="
    filter += "udo_scriptfilename ne null and udo_order ne null";
    filter += "&$orderby=udo_name asc"

    Xrm.WebApi.retrieveMultipleRecords("udo_requestsubtype", filter)
        .then(
            function reply(response) {
                finishLoadSubTypesUCI(response.value);
            }
        );
}

function finishLoadSubTypesUCI(data) {
    if (!data || data.length == 0) {
        return;
    }

    for (var i = 0; i < data.length; i++) {
        var parentli = $("#type-" + data[i]._udo_type_value);
        if (!parentli) continue;
        if (parentli.children("ul").length == 0) {
            parentli.append("<ul />");
        }
        var ul = parentli.children("ul:first");
        var html = '<li><a id="relsLink_' + data[i].udo_requestsubtypeid + '" style="text-decoration: underline; cursor: pointer;" href="' +
            data[i].udo_scriptfilename + '" target="_blank" title="Click to Open ' + data[i].udo_name + '">' + data[i].udo_name + '</a></li>';
        ul.append(html);
        parentli.show();
    }
}

function toggleDiv(divid) {
    if (document.getElementById(divid).style.display == 'none') {
        document.getElementById(divid).style.display = 'block';
    } else {
        document.getElementById(divid).style.display = 'none';
    }

    var _divs = ['step', 'callscripts'];

    for (var i in _divs) {
        if (_divs[i] != divid) {
            var curDiv = document.getElementById(_divs[i]);
            if (curDiv.style.display != 'none') { curDiv.style.display = 'none'; }
        }
    }
}

/* ***** Code for IE browsers; Needed until such time VA no longer uses Internet Explorer ***** */

function LoadVALinks() {
    var extLink = '953850000';

    var sFetch = '<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false"><entity name="va_systemsettings"><attribute name="va_name"/><attribute name="va_description"/><order attribute="va_description" descending="false"/><filter type="and"><condition attribute="va_type" operator="eq" value="' +
        extLink +
        '"/></filter><filter type="and"><condition attribute="statecode" operator="eq" value="0" /></filter></entity></fetch>';

    var _oService;
    var _sOrgName = "";
    var _sServerUrl = window.Xrm.Page.context.getClientUrl();
    _oService.Fetch(sFetch, finishLoadVALinks);
}

function LoadTypes() {
    var sFetch = '<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false"><entity name="udo_requesttype"><attribute name="udo_requesttypeid" /><attribute name="udo_name"/><attribute name="udo_order"/><filter type="and"><condition attribute="udo_order" operator="not-null" /></filter><order attribute="udo_order" descending="false" /><order attribute="udo_name" descending="false" /></entity></fetch>';
    var _oService;
    var _sOrgName = "";
    var _sServerUrl = window.Xrm.Page.context.getClientUrl();
    _oService.Fetch(sFetch, finishLoadTypes);
}

function finishLoadVALinks(links) {
    if (!links || links.length == 0) {
        alert('System Settings lookup data does not contain any External Links settings.');
        return;
    }
    var s = '';
    for (var i = 0; i < links.length; i++) {
        if (!links[i].attributes["va_name"] || !links[i].attributes["va_name"].value) continue;
        var desc = links[i].attributes["va_description"] && links[i].attributes["va_description"].value ? links[i].attributes["va_description"].value : null;
        if (!desc) desc = links[i].attributes["va_name"].value;

        s += '<li><a id="relLink' + i.toString() + '" style="text-decoration: underline; cursor: pointer;" href="' +
            links[i].attributes["va_name"].value + '" target="_blank" title="Click to Open ' + desc + '">' + desc + '</a></li>';
    }

    if (s.length == 0) {
        s = 'No VA Links Found';
    } else {
        s = "<ul>" + s + "</ul>";
    }

    var elem3 = document.getElementById('step'); if (elem3) { elem3.innerHTML = s; }
}

function finishLoadTypes(data) {
    if (!data || data.length == 0) {
        return;
    }

    var elem = document.getElementById('callscripts'); if (elem) { elem.innerHTML = '<ul id="callscripts-ul" />'; }
    var ul = $("#callscripts-ul");

    for (var i = 0; i < data.length; i++) {
        ul.append('<li id="type-' + data[i].attributes["udo_requesttypeid"].value + '" style="display:none"><a href="javascript:;">' + data[i].attributes["udo_name"].value + '</a><ul style="display:none" /></li>');
        var li = $("#type-" + data[i].attributes["udo_requesttypeid"].value).click(function () {
            $(this).children("ul:first").toggle();
        });
    }

    LoadSubTypes();
}

function LoadSubTypes() {
    var sFetch = '<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false"><entity name="udo_requestsubtype"><attribute name="udo_requestsubtypeid" /><attribute name="udo_name"/><attribute name="udo_type"/><attribute name="udo_scriptfilename"/><filter type="and"><condition attribute="udo_scriptfilename" operator="not-null" />"><condition attribute="udo_order" operator="not-null" /></filter><order attribute="udo_order" descending="false" /><order attribute="udo_name" descending="false" /></entity></fetch>';
    var _oService;
    var _sOrgName = "";
    var _sServerUrl = window.Xrm.Page.context.getClientUrl();
    _oService.Fetch(sFetch, finishLoadSubTypes);
}

function finishLoadSubTypes(data) {
    if (!data || data.length == 0) {
        return;
    }

    for (var i = 0; i < data.length; i++) {
        var parentli = $("#type-" + data[i].attributes["udo_type"].guid);
        if (!parentli) continue;
        if (parentli.children("ul").length == 0) {
            parentli.append("<ul />");
        }
        var ul = parentli.children("ul:first");
        var html = '<li><a id="relsLink_' + data[i].attributes["udo_requestsubtypeid"].value + '" style="text-decoration: underline; cursor: pointer;" href="' +
            data[i].attributes["udo_scriptfilename"].value + '" target="_blank" title="Click to Open ' + data[i].attributes["udo_name"].value + '">' + data[i].attributes["udo_name"].value + '</a></li>';
        ul.append(html);
        parentli.show();
    }
}
