/*globales Xrm, $ */

///
/// AlfaPeople CRM 2011 CrmRestKit
///
/// Based on 'MSCRM4 Web Service Toolkit for JavaScript v2.1' (http://crmtoolkit.codeplex.com/releases/view/48329)
/// and XrmSvcTookit 'http://xrmsvctoolkit.codeplex.com/
///
/// Credits:
///     Daniel Cai (getServerUrl, associate, disassociate)
///     Matt (https://www.codeplex.com/site/users/view/MattMatt)
///
/// @author:
///     Daniel Rene Thul, drt@alfapeople.com
/// @modified
///     to include OrderBy RetrieveMultiple
///
/// @version:
///     2.6.1
///
/// requires (jquery.1.7.2.js, JSON2.js)
///
var CrmRestKit2011 = (function (window, document, undefined) {
    'use strict';

    ///
    /// Private members
    ///
    var ODATA_ENDPOINT = "/XRMServices/2011/OrganizationData.svc",
        version = '2.6.0',
        serverUrl = Xrm.Page.context.getClientUrl();;



    ///
    /// Private function to return the server URL from the context
    ///
    function getServerUrl() {
        return Xrm.Page.context.getClientUrl();

    }
    ///
    /// Private function to return the path to the REST endpoint.
    ///
    function getODataPath() {

        return getServerUrl() + ODATA_ENDPOINT;
    }

    ///
    /// Returns an object that reprensts a entity-reference 
    ///
    function entityReferenceFactory(id, opt_logicalName) {

        var reference = null;

        if (id !== undefined && id !== null) {

            reference = {
                __metadata: { type: "Microsoft.Crm.Sdk.Data.Services.EntityReference" },
                Id: id
            };

            if (opt_logicalName !== undefined && opt_logicalName !== null) {

                reference.LogicalName = opt_logicalName;
            }
        }

        return reference;
    }

    ///
    /// Returns an object that reprensts a option-set-value 
    ///
    function optionSetValueFactory(option_value) {

        return {
            __metadata: { type: 'Microsoft.Crm.Sdk.Data.Services.OptionSetValue' },
            Value: option_value
        };
    }

    ///
    /// Returns an object that represents an money value
    ///
    function moneyValueFactory(value) {

        return {
            __metadata: { type: 'Microsoft.Crm.Sdk.Data.Services.Money' },
            Value: value
        };
    }

    ///
    /// Parses the ODATA date-string into a date-object
    /// All queries return a date in the format "/Date(1368688809000)/"
    /// 
    function parseODataDate(value) {

        return new Date(parseInt(value.replace('/Date(', '').replace(')/', ''), 10));
    }

    ///
    /// Generics ajax-call funciton. Returns a promise object
    ///
    function doRequest(options, asyn) {

        // default values for the ajax queries
        var ajaxDefaults = {
            type: "GET",
            async: true,
            contentType: "application/json; charset=utf-8",
            datatype: "json",
            beforeSend: function (request) {

                request.setRequestHeader("Accept", "application/json");
            }
        };

        // merge the default-settings with the options-object
        options = $.extend(ajaxDefaults, options);

        // request could be executed in sync or asyn mode
        options.async = (asyn === undefined) ? true : asyn;

        return $.ajax(options);
    }

    ///
    /// Creates a link between records 
    ///
    function associate(entity1Id, entity1Name, entity2Id, entity2Name, relationshipName, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn,
            odatapath = getODataPath(),
            request = {
                url: odatapath + "/" + entity1Name + "Set(guid'" + entity1Id + "')/$links/" + relationshipName,
                type: "POST",
                data: window.JSON.stringify({
                    uri: odatapath + "/" + entity2Name + "Set(guid'" + entity2Id + "')"
                })
            };

        return doRequest(request, asyn);
    }

    ///
    /// Removes a link between records 
    ///
    function disassociate(entity1Id, entity1Name, entity2Id, relationshipName, opt_asyn) {

        var asyn = (opt_asyn === undefined) ? true : opt_asyn,
            odatapath = getODataPath(),
            request = {
                url: odatapath + "/" + entity1Name + "Set(guid'" + entity1Id + "')/$links/" + relationshipName + "(guid'" + entity2Id + "')",
                type: "POST",
                // method: "DELETE",
                beforeSend: function (request) {
                    request.setRequestHeader('Accept', 'application/json');
                    request.setRequestHeader('X-HTTP-Method', 'DELETE');
                }
            };

        return doRequest(request, asyn);
    }

    ///
    /// Retrieves a single record 
    ///
    function retrieve(entityName, id, columns, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn,
            setName = entityName + 'Set',
            query = getODataPath() + "/" + setName + "(guid'" + id + "')" + "?$select=" + columns.join(',');

        // returns a promise instance
        return doRequest({ url: query }, asyn);
    }

    ///
    /// Used in the context of lazy-loading (more than 50 records found in the retrieveMultiple request)
    /// Query (url) needs to define the entity, columns and filter
    ///
    function byQueryUrl(queryUrl, opt_asyn) {

        return doRequest({ url: queryUrl }, opt_asyn);
    }

    ///
    /// Used for joins
    ///
    function byExpandQuery(entityName, columns, expand, filter, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn;

        // in case filter is empty 
        filter = (filter) ? "&$filter=" + encodeURIComponent(filter) : '';

        // create defered object
        var setName = entityName + 'Set',
            query = getODataPath() + "/" + setName + "?$select=" + columns.join(',') + '&$expand=' + expand + filter;

        return doRequest({ url: query }, asyn);
    }

    ///
    /// Retrievs multiuple records based on filter
    /// The max number of records returned by Odata is limited to 50, the result object contains the property 
    /// 'next' and the fn loadNext that could be used to load the addional records 
    ///
    function byQuery(entityName, columns, filter, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn;

        // in case filter is empty 
        filter = (filter) ? "&$filter=" + encodeURIComponent(filter) : '';

        // create defered object
        var setName = entityName + 'Set',
            query = getODataPath() + "/" + setName + "?$select=" + columns.join(',') + filter;

        return doRequest({ url: query }, asyn);
    }

    ///
    /// Per default a REST query returns only 50 record. This function will load all records
    ///
    function byQueryAll(entityName, columns, filter, opt_asyn) {

        var dfdAll = new $.Deferred(),
            allRecords = [];

        byQuery(entityName, columns, filter, opt_asyn).then(function byQueryAllSuccess(result) {

            // add the elements to the collection
            allRecords = allRecords.concat(result.d.results);

            if (result.d.__next) {

                // the success-handler will be this function
                byQueryUrl(result.d.__next, opt_asyn).then(byQueryAllSuccess, dfdAll.reject);

                // call the progressCallbacks of the promise
                dfdAll.notify(result);
            }
            else {
                dfdAll.resolve(allRecords);
            }

        }, dfdAll.reject);

        return dfdAll.promise();
    }

    ///
    /// Create a single reocrd
    ///
    function created(entityName, entityObject, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn,
            setName = entityName + 'Set',
            json = window.JSON.stringify(entityObject),
            query = getODataPath() + "/" + setName;

        // returns a promise object
        return doRequest({ type: "POST", url: query, data: json }, asyn);
    }

    ///
    /// Updates the record with the stated intance.
    /// MERGE methode does not return data
    ///
    /// Sample:
    ///     CrmRestKit.Update('Account', id, { 'Address1_City': 'sample', 'Name': 'sample' }).done(...).fail(..)
    ///
    function update(entityName, id, entityObject, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn,
            setName = entityName + 'Set',
            json = window.JSON.stringify(entityObject),
            query = getODataPath() + "/" + setName + "(guid'" + id + "')",
            // ajax-call-options
            options = {
                type: "POST",
                url: query,
                data: json,
                beforeSend: function (request) {
                    request.setRequestHeader("Accept", "application/json");
                    request.setRequestHeader("X-HTTP-Method", "MERGE");
                }
            };

        // MERGE methode does not return data
        return doRequest(options, asyn);
    }

    ///
    /// Deletes as single record identified by the id
    /// Sample:
    ///         CrmRestKit.Delete('Account', id).done(...).fail(..);
    ///
    function deleteRecord(entityName, id, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn,
            setName = entityName + 'Set',
            query = getODataPath() + '/' + setName + "(guid'" + id + "')",
            options = {
                type: "POST",
                url: query,
                beforeSend: function (request) {
                    request.setRequestHeader('Accept', 'application/json');
                    request.setRequestHeader('X-HTTP-Method', 'DELETE');
                }
            };

        return doRequest(options, asyn);
    }

    ///
    /// Added 10-22-2015 to support Ordered Queries
    ///
    ///
    /// Retrievs multiuple records based on filter
    /// The max number of records returned by Odata is limited to 50, the result object contains the property 
    /// 'next' and the fn loadNext that could be used to load the addional records 
    ///
    /// The orderby field was added and deviates from the publically availble restkit.  It is added at the end
    /// of the procedure and checked for undefined values to prevent altering standard calls to byQueryAll and
    /// byQuery
    ///
    function byQueryOrdered(entityName, columns, filter, orderby, opt_asyn) {

        // default is 'true'
        var asyn = (opt_asyn === undefined) ? true : opt_asyn;

        var orderBy = (orderby === undefined) ? '' : "&$orderby=" + encodeURIComponent(orderby);

        // in case filter is empty 
        filter = (filter) ? "&$filter=" + encodeURIComponent(filter) : '';

        // create defered object
        var setName = entityName + 'Set',
            query = getODataPath() + "/" + setName + "?$select=" + columns.join(',') + orderBy + filter;

        return doRequest({ url: query }, asyn);
    }

    ///
    /// Per default a REST query returns only 50 record. This function will load all records
    ///
    function byQueryAllOrdered(entityName, columns, filter, orderby, opt_asyn) {

        var dfdAll = new $.Deferred(),
            allRecords = [];

        byQueryOrdered(entityName, columns, filter, orderby, opt_asyn).then(function byQueryAllSuccess(result) {

            // add the elements to the collection
            allRecords = allRecords.concat(result.d.results);

            if (result.d.__next) {

                // the success-handler will be this function
                byQueryUrl(result.d.__next, opt_asyn).then(byQueryAllSuccess, dfdAll.reject);

                // call the progressCallbacks of the promise
                dfdAll.notify(result);
            }
            else {
                dfdAll.resolve(allRecords);
            }

        }, dfdAll.reject);

        return dfdAll.promise();
    }

    ///
    /// Public API
    ///
    return {
        Version: version,
        /* Read /retrieve methods*/
        Retrieve: retrieve,
        ByQuery: byQuery,
        ByQueryUrl: byQueryUrl,
        ByExpandQuery: byExpandQuery,
        ByQueryAll: byQueryAll,
        /* C U D */
        Create: created,
        Update: update,
        Delete: deleteRecord,
        /* N:M relationship operations */
        Associate: associate,
        Disassociate: disassociate,
        /* Factory methods */
        EntityReferenceFactory: entityReferenceFactory,
        OptionSetValueFactory: optionSetValueFactory,
        MoneyValueFactory: moneyValueFactory,
        /* util methods */
        ParseODataDate: parseODataDate,
        /* added 10-22-2015 */
        ByQueryOrdered: byQueryOrdered,
        ByQueryAllOrdered: byQueryAllOrdered
    };
}(window, document));