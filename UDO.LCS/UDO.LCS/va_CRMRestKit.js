///
/// Acando CRM 5.0 CrmRestKit Version 0.0.1
/// Based on 'MSCRM4 Web Service Toolkit for JavaScript v2.1' (http://crmtoolkit.codeplex.com/releases/view/48329)
///
/// @author Daniel Rene Thul, daniel.rene.thul@acando.de
///
/// Dependencies
///		- jquery.1.4.2.js
///    	- JSON2 
///
CrmRestKit = function () {

    var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc";
    var context = Xrm.Page.context || GetGlobalContext();
    var serverUrl = context.getClientUrl();

    var _doRequest = function (options, async, internalCallback) {

        // default settings
        var settings = {

            type: "GET",
            async: async,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            beforeSend: function (request) {

                request.setRequestHeader("Accept", "application/json");
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {

                alert("Error : " + textStatus + ": " + XmlHttpRequest.statusText);
            }
        };

        // merge the default-settings with the options-object
        options = $.extend(settings, options);

        if (!async) {

            var result = $.ajax(options).responseText;
            var jsonResult = (result) ? JSON.parse(result).d : null;

            return !!internalCallback ? internalCallback(jsonResult) : jsonResult;
        }
        else {

            settings.success = function (data, textStatus, XmlHttpRequest) {

                internalCallback(data.d);
            };

            $.ajax(options);
        }
    };

    var _retrieve = function (entityName, id, columns, callback) {

        var async = !!callback;
        var setName = entityName + 'Set';
        var query = serverUrl + ODATA_ENDPOINT + "/" + setName + "(guid'" + id + "')" + "?$select=" + columns.join(',');

        return _doRequest({ url: query }, async, function (result) {

            if (async)
                callback(result);
            else
                return result;
        });
    };


    var _retrieveMultiple = function (entityName, columns, filter, callback, top, orderby) {

        // var async = !!callback;
        var setName = entityName + 'Set';
        var _callback = callback;



        //var query = serverUrl + ODATA_ENDPOINT + "/" + setName + "()" + "?$select=" + columns.join(',') + filter;
        var query = serverUrl + ODATA_ENDPOINT + "/" + setName + "()" + "?$select=" + columns.join(',');

        //if orderby
        if (orderby) query += ('&$orderby=' + orderby);
 
        //if filter
        filter = (filter) ? "&$filter=" + filter : '';
        query += filter;

        //if top
        if (top) query += ('&$top=' + top);

        var performRequest = function (query, fnCallback) {

            var async = !!fnCallback;
            var options = { url: query }

            return _doRequest(options, async, function (data) {

                var next = data.__next || null;
                var results = data.results || data;
                var response = { 'results': results, 'next': next }

                if (next) { // enable eage loading

                    response.LoadNext = function (callback) {

                        return performRequest(next, callback);
                    };
                }

                if (async)
                    fnCallback(response);
                else
                    return response;
            });
        };

        return performRequest(query, callback);
    };

    var _created = function (entityName, entityObject, callback) {

        var async = !!callback;
        var setName = entityName + 'Set';
        var json = window.JSON.stringify(entityObject);
        var query = serverUrl + ODATA_ENDPOINT + "/" + setName;

        var options = { type: "POST", url: query, data: json };

        return _doRequest(options, async, function (result) {

            if (async)
                callback(result);
            else
                return result;

        });
    };

    var _update = function (entityName, id, entityObject, callback) {

        var async = !!callback;
        var setName = entityName + 'Set';
        var json = window.JSON.stringify(entityObject);
        var _id = id;
        var query = serverUrl + ODATA_ENDPOINT + "/" + setName + "(guid'" + _id + "')"

        var options = {
            type: "POST",
            url: query,
            data: json,
            beforeSend: function (request) {

                request.setRequestHeader("Accept", "application/json");
                request.setRequestHeader("X-HTTP-Method", "MERGE");
            }
        };

        // MERGE methode does not return data
        return _doRequest(options, async, function () {

            if (async)
                callback(_id);
            else
                return _id;

        });
    };

    var _delete = function (entityName, id, callback) {

        var async = !!callback;
        var setName = entityName + 'Set';
        var query = serverUrl + ODATA_ENDPOINT + '/' + setName + "(guid'" + id + "')";

        var options = {

            type: "POST",
            url: query,
            beforeSend: function (request) {

                request.setRequestHeader('Accept', 'application/json');
                request.setRequestHeader('X-HTTP-Method', 'DELETE');
            }
        };

        return _doRequest(options, async, function (result) {

            if (async)
                callback(result);
            else
                return result;

        });
    };

    return {

        Retrieve: _retrieve,

        RetrieveMultiple: _retrieveMultiple,

        Create: _created,

        Update: _update,

        Delete: _delete
    };
} ();
