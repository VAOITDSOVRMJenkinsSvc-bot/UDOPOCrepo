Ext.define('VIP.data.proxy.Soap', {
    requires: ['Ext.util.MixedCollection', 'Ext.Ajax'],
    extend: 'Ext.data.proxy.Server',
    alias: 'proxy.soap',
    requires: [
        'VIP.util.xml.Parser'
    ],
    pageParam: undefined,

    startParam: undefined,

    limitParam: undefined,

    groupParam: undefined,

    sortParam: undefined,

    filterParam: undefined,

    /**
    * @property actionMethods
    * Mapping of action name to HTTP request method. In the basic AjaxProxy these are set to 'GET' for 'read' actions and 'POST' 
    * for 'create', 'update' and 'destroy' actions. The {@link Ext.data.proxy.Rest} maps these to the correct RESTful methods.
    */
    actionMethods: {
        create: 'POST',
        read: 'POST',
        update: 'POST',
        destroy: 'POST'
    },

    buildRequest: function (operation) {
        var params = Ext.applyIf(operation.params || {}, this.extraParams || {}),
                request;

        //copy any sorters, filters etc into the params so they can be sent over the wire
        params = Ext.applyIf(params, this.getParams(operation));

        if (operation.id && !params.id) {
            params.id = operation.id;
        }

        request = Ext.create('Ext.data.Request', {
            params: params,
            action: operation.action,
            records: operation.records,
            operation: operation,
            url: operation.url,
            method: 'POST'
        });
        var opScopeId = null;
        if ((operation) && (operation.scope) && (operation.scope.id)) {
            opScopeId = operation.scope.id;
        }
        request.url = this.setAutoResponderFlag(this.buildUrl(request), opScopeId);

        /*
        * Save the request on the Operation. Operations don't usually care about Request and Response data, but in the
        * ServerProxy and any of its subclasses we add both request and response as they may be useful for further processing
        */
        operation.request = request;

        return request;
    },

    /* 
    * Used with fiddler to setup an auto responder response. This is a flag set on the WS url
    * to setup a rul for the fiddler auto responder to match the URL
    */
    setAutoResponderFlag: function (url, operationId) {
        var i, wsFlag, wsFlags;

        if (window.location.hostname !== 'localhost') return url;

        var query = function (name) {
            var regex;
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        wsFlag = query('ws');

        if (!wsFlag) return url;

        wsFlags = wsFlag.split(',');

        for (i = 0; i < wsFlags.length; i++) {
            if (operationId === wsFlags[i])
                return url + '&_' + operationId;
        }

        return url;
    },

    constructor: function (config) {
        var me = this;

        config = config || {};
        this.addEvents(
        /**
        * @event exception
        * Fires when the server returns an exception
        * @param {Ext.data.proxy.Proxy} this
        * @param {Object} response The response from the AJAX request
        * @param {Ext.data.Operation} operation The operation that triggered request
        */
							'exception'
						);
        me.callParent([config]);

        /**
        * @cfg {Object} extraParams Extra parameters that will be included on every request. Individual requests with params
        * of the same name will override these params when they are in conflict.
        */
        me.extraParams = config.extraParams || {};

        me.api = config.api || {};

        //backwards compatibility, will be deprecated in 5.0
        me.nocache = me.noCache;

        me.envelopes = config.envelopes || {};


    },

    /**
    * @cfg {Object} headers Any headers to add to the Ajax request. Defaults to <tt>undefined</tt>.
    */

    filters: Ext.create('Ext.util.MixedCollection'),

    extractResponseData: function (response) {
        var me = this,
            xmlParser = Ext.create('VIP.util.xml.Parser', {
                xmlText: response.responseXML.text
            }),
            extractedResponse = xmlParser.parse();

        return extractedResponse;
    },

    createRemotelyFilteredEnvelope: function (envelopeType) {
        var me = this,
            templateEnvelope = Ext.create(envelopeType),
            populatedEnvelope,
            envelopeConfig = templateEnvelope.config,
            filterEnvelopeConfig = {};

        me.filters.each(function (filter) {
            if (!Ext.isFunction(filter)) {
                var property = filter.property,
                        value = filter.value,
                        field,
                        fieldMapping;

                if (templateEnvelope.canFilterBy(property)) {
                    field = me.model.prototype.fields.get(property);
                    if (Ext.isEmpty(field)) {
                        filterEnvelopeConfig[property] = value;
                    }
                    else {
                        if (Ext.isEmpty(field.ignoreMappingOnRequest)) {
                            fieldMapping = field.mapping;
                            if (fieldMapping.indexOf('/') != -1) {
                                var scrubbedFieldMapping = fieldMapping.substr(fieldMapping.indexOf('/') + 1);

                                fieldMapping = scrubbedFieldMapping;
                            }
                        }
                        else {
                            fieldMapping = property;
                        }
                        filterEnvelopeConfig[fieldMapping] = value;
                    }
                    me.filters.remove(filter);
                }
            }
        });

        populatedEnvelope = Ext.create(envelopeType, Ext.apply(envelopeConfig, filterEnvelopeConfig));

        return populatedEnvelope;
    },

    create: function () {
        var newCallback = function (operation) {
            if (operation.wasSuccessful()) {
                /*record = operation.getRecords()[0];    Jonas 4/8/2012 not needed for SOAP proxy, since no data is returned on creation
                    

                me.set(record.data);
                record.dirty = false;
                */
                Ext.callback(operation.success, operation.scope, [null, operation]);
            } else {
                Ext.callback(operation.failure, operation.scope, [null, operation]);
            }

            Ext.callback(operation.callback, operation.scope, [null, operation]);
        };

        arguments[1] = newCallback; //arguments[1] should always be the callback

        return this.doRequest.apply(this, arguments);
    },
    /*
    read: function () {
    return this.doRequest.apply(this, arguments);
    },
    */
    update: function () {
        //debugger;
        var newCallback = function (operation) {
            if (operation.wasSuccessful()) {
                /*record = operation.getRecords()[0];    Jonas 4/8/2012 not needed for SOAP proxy, since no data is returned on creation
                    

                me.set(record.data);
                record.dirty = false;
                */
                Ext.callback(operation.success, operation.scope, [null, operation]);
            } else {
                Ext.callback(operation.failure, operation.scope, [null, operation]);
            }

            Ext.callback(operation.callback, operation.scope, [null, operation]);
        };

        arguments[1] = newCallback; //arguments[1] should always be the callback

        return this.doRequest.apply(this, arguments);
    },

    destroy: function () {
        return this.doRequest.apply(this, arguments);
    },

    /**
    * @ignore
    */
    doRequest: function (operation, callback, scope) {
        var me = this,
            writer = me.getWriter(),
			request,
            envelopeType = me.envelopes[operation.action],
            envelope;

        for (var i in operation.filters) {
            var filter = new Ext.util.Filter(operation.filters[i]);
            me.filters.add(filter);
        }

        if (!Ext.isEmpty(operation.id)) {
            var filter = new Ext.util.Filter({
                property: me.model.idProperty,
                value: operation.id
            });
            operation.filters.add(filter);
        }

        if (me.filters.getCount()) {
            envelope = me.createRemotelyFilteredEnvelope(envelopeType);
        }
        else {
            envelope = Ext.create(envelopeType);
        }

        if (Ext.isEmpty(envelope.getRequestUrl())) {
            Ext.Error.raise('No requestUrl was defined on the envelope.');
            //debugger;
            return;
        }

        if (!Ext.isEmpty(envelope.postCreate) && Ext.isFunction(envelope.postCreate)) {
            envelope.postCreate();
        }

        me.url = envelope.getRequestUrl();

        request = me.buildRequest(operation, callback, scope);
        /* Jonas 4/7/2012 Not needed at all
        if (operation.allowWrite()) {
        request = writer.write(request);
        }

        //stan 4/6: Ugly hack to make the createNote code work
        if (request.operation.request.url.indexOf("DevelopmentNotesService") != -1) {
        request.jsonData = null;
        }
        */
        var timeout = 600000;

        Ext.apply(request, {
            headers: me.headers,
            timeout: timeout, //me.timeout,
            scope: me,
            callback: me.createRequestCallback(request, operation, callback, scope),
            method: me.getMethod(request),
            disableCaching: false, // explicitly set it to false, ServerProxy handles caching
            xmlData: envelope.toString(),
            envelope: envelope
        });

        if (request.xmlData.indexOf('findBenefitClaimDetailsByBnftClaimId') != -1) {
            //request.xmlData = request.xmlData.replace('<wsse:Username>VACOGROSSJ</wsse:Username>', '<wsse:Username>VAEBENEFITS</wsse:Username>');
            //request.xmlData = request.xmlData.replace('<vaws:STN_ID>317</vaws:STN_ID>', '<vaws:STN_ID>281</vaws:STN_ID>');
            //request.xmlData = request.xmlData.replace('<wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"></wsse:Password>', '<wsse:Password Type="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText">Buda110!</wsse:Password>');
            //request.xmlData = request.xmlData.replace('<vaws:applicationName>CRMUD</vaws:applicationName>', '<vaws:applicationName>EBENEFITS</vaws:applicationName>');

            request.xmlData = request.xmlData.replace('<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"', '<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"  xmlns:q0="http://claimstatus.services.ebenefits.vba.va.gov/" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"');
            request.xmlData = request.xmlData.replace('<ebs:findBenefitClaimDetailsByBnftClaimId', '<q0:findBenefitClaimDetailsByBnftClaimId');
            request.xmlData = request.xmlData.replace('</ebs:findBenefitClaimDetailsByBnftClaimId', '</q0:findBenefitClaimDetailsByBnftClaimId');
            request.xmlData = request.xmlData.replace('<ebs:bnftClaimId>', '<bnftClaimId>');
            request.xmlData = request.xmlData.replace('</ebs:bnftClaimId>', '</bnftClaimId>');
            request.xmlData = request.xmlData.replace(' xmlns:ebs="http://ddeft.service.vetsnet.vba.va.gov/"', '');
        }
        if (request.xmlData.indexOf('findComericaRoutngTrnsitNbr') != -1) {
            request.xmlData = request.xmlData.replace('<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"', '<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:q0="http://ddeft.services.vetsnet.vba.va.gov/"');
            request.xmlData = request.xmlData.replace('<com:findComericaRoutngTrnsitNbr', '<q0:findComericaRoutngTrnsitNbr');
            request.xmlData = request.xmlData.replace('</com:findComericaRoutngTrnsitNbr', '</q0:findComericaRoutngTrnsitNbr');
            request.xmlData = request.xmlData.replace(' xmlns:com="http://ddeft.service.vetsnet.vba.va.gov/"', '');
        }

        Ext.Ajax.request(request);

        return request;
    },

    /**
    * Returns the HTTP method name for a given request. By default this returns based on a lookup on {@link #actionMethods}.
    * @param {Ext.data.Request} request The request object
    * @return {String} The HTTP method to use (should be one of 'GET', 'POST', 'PUT' or 'DELETE')
    */
    getMethod: function (request) {
        return this.actionMethods[request.action];
    },

    /**
    * @private
    * TODO: This is currently identical to the JsonPProxy version except for the return function's signature. There is a lot
    * of code duplication inside the returned function so we need to find a way to DRY this up.
    * @param {Ext.data.Request} request The Request object
    * @param {Ext.data.Operation} operation The Operation being executed
    * @param {Function} callback The callback function to be called when the request completes. This is usually the callback
    * passed to doRequest
    * @param {Object} scope The scope in which to execute the callback function
    * @return {Function} The callback function
    */
    createRequestCallback: function (request, operation, callback, scope) {
        var me = this;

        return function (options, success, response) {
            me.processResponse(success, operation, request, response, callback, scope);
        };
    },
    getParams: function (operation) {
        var me = this,
			params = {},
			isDef = Ext.isDefined,
			groupers = operation.groupers,
			sorters = operation.sorters,
			filters = operation.filters,
			page = operation.page,
			start = operation.start,
			limit = operation.limit,

                     simpleSortMode = me.simpleSortMode,

                     pageParam = me.pageParam,
                     startParam = me.startParam,
                     limitParam = me.limitParam,
                     groupParam = me.groupParam,
                     sortParam = me.sortParam,
                     filterParam = me.filterParam,
                     directionParam = me.directionParam;

        if (pageParam && isDef(page)) {
            params[pageParam] = page;
        }

        if (startParam && isDef(start)) {
            params[startParam] = start;
        }

        if (limitParam && isDef(limit)) {
            params[limitParam] = limit;
        }

        if (groupParam && groupers && groupers.length > 0) {
            // Grouper is a subclass of sorter, so we can just use the sorter method
            params[groupParam] = me.encodeSorters(groupers);
        }

        if (sortParam && sorters && sorters.length > 0) {
            if (simpleSortMode) {
                params[sortParam] = sorters[0].property;
                params[directionParam] = sorters[0].direction;
            } else {
                params[sortParam] = me.encodeSorters(sorters);
            }

        }

        if (filterParam && filters && filters.length > 0) {
            params[filterParam] = me.encodeFilters(filters);
        }

        return params;
    },

    processResponse: function (success, operation, request, response, callback, scope) {
        var me = this,
            reader,
            result,
            filteredResults = Ext.create('Ext.util.MixedCollection'),
            message = null;

        if (success === true) {
            reader = me.getReader();

            if (!Ext.isEmpty(response.responseText) && response.responseText.indexOf('findComericaRoutngTrnsitNbrResponse') != -1) {
                result = reader.read($.parseXML(response.responseText));
            }
            else {
                if (!Ext.isEmpty(response.responseText) && response.responseText.indexOf('findBenefitClaimDetailsByBnftClaimIdResponse') != -1) {
                    var a = 'a';
                }
                if (request.envelope.isDacRequest) {
                    response = me.extractResponseData(response);
                }

                if (!Ext.isEmpty(request.envelope.analyzeResponse)) {
                    if (Ext.isFunction(request.envelope.analyzeResponse)) {
                        result = request.envelope.analyzeResponse(response, reader);
                    }
                    else {
                        result = request.envelope.analyzeResponse.analyze(response, reader);
                    }
                }
                else {
                    result = reader.read(response);
                }
            }

            if (Ext.isEmpty(result)) {
                return;
            }
            if (result.success !== false) {
                if (me.filters.getCount()) {

                    filteredResults.addAll(result.records);

                    filteredResults.filter(me.filters.items);

                    result.records = filteredResults.items;
                }

                //see comment in buildRequest for why we include the response object here
                Ext.apply(operation, {
                    response: response,
                    resultSet: result
                });

                operation.setCompleted();
                operation.setSuccessful();
            }
            else {
                me.setException(operation, response);
                me.fireEvent('exception', me, response, operation, result);
                //operation.setCompleted();
            }
        }
        else {
            me.setException(operation, response);
            me.fireEvent('exception', this, response, operation);
        }

        //this callback is the one that was passed to the 'read' or 'write' function above
        if (typeof callback == 'function') {
            callback.call(scope || me, operation);
        }

        me.afterRequest(request, success);
    }

});
