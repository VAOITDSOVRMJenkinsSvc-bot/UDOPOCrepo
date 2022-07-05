/**
* @author Jonas Dawson
* @class VIP.util.soap.RequestBuilder
*
* Provides useful methods for building SOAP envelopes with the XmlSerializer
*/
Ext.define('VIP.util.soap.AbstractRequestBuilder', {
    requires: [
        'VIP.util.soap.Envelope'
    ],
    config: {
        request: Ext.create('VIP.util.soap.Envelope')
    },

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        return me;
    },

    addHeader: function (name, object) {
        var me = this,
            request = me.getRequest();

        request.Envelope.Header[name] = object;
    },

    addNamespace: function (name, url) {
        var me = this,
            request = me.getRequest();

        request.Envelope.namespaces[name] = url;
    },

    setBody: function (name, object) {
        var me = this,
            request = me.getRequest();

        request.Envelope.Body[name] = object;
    },

    toString: function () {
        var me = this,
            rootNodeName = 'Envelope',
            xmlSerializer = Ext.create('VIP.util.xml.Serializer'),
            request = me.getRequest();

        return xmlSerializer.serialize(request, rootNodeName);
    },

    excludedParameters: [
        'request'
    ],

    addExcludedParameter: function (parameterName) {
        var me = this;

        if (Ext.isString(parameterName)) {
            Ext.Array.include(me.excludedParameters, parameterName);
        }
        else if (Ext.isArray(parameterName)) {
            Ext.Array.each(parameterName, function (parameter) {
                if (Ext.isString(parameter)) {
                    Ext.Array.include(me.excludedParameters, parameter);
                }
            });
        }
    },

    removeExcludedParameter: function (parameterName) {
        var me = this;

        if (Ext.isString(parameterName)) {
            Ext.Array.remove(me.excludedParameters, parameterName);
        }
        else if (Ext.isArray(parameterName)) {
            Ext.Array.each(parameterName, function (parameter) {
                if (Ext.isString(parameter)) {
                    Ext.Array.remove(me.excludedParameters, parameter);
                }
            });
        }
    },

    canFilterBy: function (propertyName) {
        var me = this,
            possibleParameters = Ext.Object.getKeys(me.config);

        if (Ext.Array.contains(possibleParameters, propertyName) && !Ext.Array.contains(me.excludedParameters, propertyName)) {
            return true;
        }

        return false;
    },

    getMethodName: Ext.emptyFn(),

    postCreate: Ext.emptyFn()
});