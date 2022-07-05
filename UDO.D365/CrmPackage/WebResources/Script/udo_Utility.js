"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Utility = Va.Udo.Crm.Scripts.Utility || {};

Va.Udo.Crm.Scripts.Utility.getPageCache = 0;
Va.Udo.Crm.Scripts.Utility.getPage = function () {
    if (Va.Udo.Crm.Scripts.Utility.getPageCache === 0) {
        var search = location.search;
        var url = location.href;
        var page = "";
        if (search === "") {
            var pos = url.toLowerCase().indexOf("%3f");
            if (pos > -1) {
                search = "?" + decodeURIComponent(url.substring(pos + 3));
                page = url.substring(0, pos);
                url = page + search;
            }
        } else {
            page = url.substring(0, url.indexOf("?"));
        }
        Va.Udo.Crm.Scripts.Utility.getPageCache = {};
        Va.Udo.Crm.Scripts.Utility.getPageCache.page = page;
        Va.Udo.Crm.Scripts.Utility.getPageCache.search = search;
        Va.Udo.Crm.Scripts.Utility.getPageCache.url = url;
    }
    return Va.Udo.Crm.Scripts.Utility.getPageCache;
}

Va.Udo.Crm.Scripts.Utility.buildURL = function (baseURL, querystring) {
    var url = baseURL;
    if (url === "") {
        url = Va.Udo.Crm.Scripts.Utility.getPage().url;
    }
    if (querystring.length > 0) {
        if (querystring[0] !== "?") {
            url = url + "?";
        }
        url = url + querystring;
    }
    return url;
}

Va.Udo.Crm.Scripts.Utility.getUrlParamsCache = 0;
Va.Udo.Crm.Scripts.Utility.getUrlParams = function () {
    if (Va.Udo.Crm.Scripts.Utility.getUrlParamsCache === 0) {
        var result = {};
        var search = Va.Udo.Crm.Scripts.Utility.getPage().search;
        var pArr = search.substr(1).split("&");

        for (var i = 0; i < pArr.length; i++) {
            var p = pArr[i].split('=');
            if (p[0].toLowerCase() !== "data") {
                result[p[0].toLowerCase()] = decodeURIComponent(p[1]);
            } else {
                var dResult = {};
                var data = p[1];
                if (p.length > 2) {
                    data = "";
                    for (var n = 1; n < p.length; n = n + 2) {
                        data = data + p[n] + "=" + p[n + 1] + "&";
                    }
                    data = data.substring(0, data.length - 1);
                }
                var dArr = decodeURIComponent(data).split("&");
                for (var k = 0; k < dArr.length; k++) {
                    var d = dArr[k].split('=');
                    dResult[d[0].toLowerCase()] = d[1];
                }
                result["data"] = dResult;
            }
        }
        Va.Udo.Crm.Scripts.Utility.getUrlParamsCache = result;
    }
    return Va.Udo.Crm.Scripts.Utility.getUrlParamsCache;
}
Va.Udo.Crm.Scripts.Utility.getUrlParams2 = function () {
    if (Va.Udo.Crm.Scripts.Utility.getUrlParamsCache === 0) {
        var result = {};
        var search = Va.Udo.Crm.Scripts.Utility.getPage().search;
        var pArr = decodeURIComponent(search.substr(1)).split("&");

        for (var i = 0; i < pArr.length; i++) {
            var p = pArr[i].split('=');
            if (p[0].toLowerCase() !== "data") {
                result[p[0].toLowerCase()] = decodeURIComponent(p[1]);
            } else {
                var dResult = {};
                var data = p[1];
                if (p.length > 2) {
                    data = "";
                    for (var n = 1; n < p.length; n = n + 2) {
                        data = data + p[n] + "=" + p[n + 1] + "&";
                    }
                    data = data.substring(0, data.length - 1);
                }
                var dArr = decodeURIComponent(data).split("&");
                for (var k = 0; k < dArr.length; k++) {
                    var d = dArr[k].split('=');
                    dResult[d[0].toLowerCase()] = d[1];
                }
                result["data"] = dResult;
            }
        }
        Va.Udo.Crm.Scripts.Utility.getUrlParamsCache = result;
    }
    return Va.Udo.Crm.Scripts.Utility.getUrlParamsCache;
}

Va.Udo.Crm.Scripts.Utility.getQueryParameter = function (name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(Va.Udo.Crm.Scripts.Utility.getPage().search);  //was location.search
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

Va.Udo.Crm.Scripts.Utility.getDataParameter = function (dataValue) {
    var dataParameter = null;
    if (dataValue !== "") {
        dataParameter = {};
        var vals = new Array();
        vals = decodeURIComponent(dataValue).split("&");
        for (var i in vals) {
            vals[i] = vals[i].replace(/\+/g, " ").split("=");
        }
        for (var i in vals) {
            dataParameter[vals[i][0]] = vals[i][1];
        }
    }
    return dataParameter;
}

Va.Udo.Crm.Scripts.Utility.dateFilter = function (date) {
    var monthString;
    var rawMonth = (date.getMonth() + 1).toString();
    if (rawMonth.length === 1) {
        monthString = "0" + rawMonth;
    }
    else { monthString = rawMonth; }

    var dateString;
    var rawDate = date.getDate().toString();
    if (rawDate.length === 1) {
        dateString = "0" + rawDate;
    }
    else { dateString = rawDate; }


    var DateFilter = "datetime\'";
    DateFilter += date.getFullYear() + "-";
    DateFilter += monthString + "-";
    DateFilter += dateString;
    DateFilter += "T00:00:00Z\'";
    return DateFilter;
}

Va.Udo.Crm.Scripts.Utility.dateTimeFilter = function (date) {
    var monthString;
    var rawMonth = (date.getMonth() + 1).toString();
    if (rawMonth.length === 1) {
        monthString = "0" + rawMonth;
    }
    else { monthString = rawMonth; }

    var dateString;
    var rawDate = date.getDate().toString();
    if (rawDate.length === 1) {
        dateString = "0" + rawDate;
    }
    else { dateString = rawDate; }


    var DateFilter = "datetime\'";
    DateFilter += date.getFullYear() + "-";
    DateFilter += monthString + "-";
    DateFilter += dateString;
    DateFilter += "T" + date.getHours() + ":";
    DateFilter += date.getMinutes() + ":";
    DateFilter += date.getSeconds();
    DateFilter += "Z\'";
    return DateFilter;
}

Va.Udo.Crm.Scripts.Utility.getEntityTypeCode = function (entityName) {
    try {
        var command = new RemoteCommand("LookupService", "RetrieveTypeCode");
        command.SetParameter("entityName", entityName);
        var result = command.Execute();
        if (result.Success && typeof result.ReturnValue === "number") {
            return result.ReturnValue;
        }
        else {
            return null;
        }
    }
    catch (ex) {
        return null;
    }
}

Va.Udo.Crm.Scripts.Utility.formatTelephone = function (phone) {
    var Phone = phone;
    var ext = '';
    var result;
    if (Phone !== null) {
        if (0 !== Phone.indexOf('+')) {
            if (1 < Phone.lastIndexOf('x')) {
                ext = Phone.slice(Phone.lastIndexOf('x'));
                Phone = Phone.slice(0, Phone.lastIndexOf('x'));
            }
            Phone = Phone.replace(/[^\d]/gi, '');
            result = Phone;
            if (7 === Phone.length) {
                result = Phone.slice(0, 3) + '-' + Phone.slice(3)
            }
            if (10 === Phone.length) {
                result = '(' + Phone.slice(0, 3) + ') ' + Phone.slice(3, 6) + '-' + Phone.slice(6);
            }
            if (11 === Phone.length) {
                result = Phone.slice(0, 1) + ' (' + Phone.slice(1, 4) + ') ' + Phone.slice(4, 7) + '-' + Phone.slice(7);
            }
            if (0 < ext.length) {
                result = result + ' ' + ext;
            }
            return result;
        }
    }
    return "";
}

Va.Udo.Crm.Scripts.Utility.getAppId = function () {
    name = "appid";
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
    results = regex.exec(Va.Udo.Crm.Scripts.Utility.getPage().url);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
