Ext.define('VIP.soap.envelopes.VARequestBuilder', {
    extend: 'VIP.util.soap.AbstractRequestBuilder',
    requires: [
		'VIP.soap.envelopes.components.SecurityHeader',
		'VIP.soap.envelopes.components.Dac',
        'VIP.services.CrmEnvironment',
        'VIP.util.xml.Serializer',
        'VIP.model.WebServiceMessage'
	],

    config: {
        namespaces: {
            'q0': 'http://services.share.benefits.vba.va.gov/',
            'ser': 'http://services.share.benefits.vba.va.gov/',
            'ws': 'http://ws.vrm.benefits.vba.va.gov/',
            'pay': 'http://paymenthistory.services.ebenefits.vba.va.gov/',
            'com': 'http://ddeft.service.vetsnet.vba.va.gov/',
            'ebs': 'http://ddeft.service.vetsnet.vba.va.gov/'
},
        //The configs below are set in the SOAP templates
        isDacRequest: true,
        baseUrl: '',
        serviceUrl: '',
        //The configs below are set in the constructor
        dacUrl: '',
        isPROD: '',
        securityContext: {
            username: '',
            password: '',
            clientMachine: '',
            stationId: '',
            applicationName: '',
            loginName: ''
        }
    },

    //This function is used in Soap.js
    getRequestUrl: function () {
        var me = this;
        if (!me.getIsDacRequest()) {
            return me.getBaseUrl() + me.getServiceUrl();
        }

        return me.getDacUrl(); //return config item
    },

    getEnvironment: function () {
        var app = _extApp;
        if (Ext.isEmpty(app)) {
            app = this.application;
        }
        if (!Ext.isEmpty(app) && !Ext.isEmpty(app.crmEnvironment)) {
            return app.crmEnvironment;
        }
        return null;
    },

    constructor: function (config) {
        var me = this,
			securityHeader,
            env = me.getEnvironment();

        me.initConfig(config);
        me.callParent();

        //Set configs
        me.setDacUrl(env.get('globalDAC'));
        me.setIsPROD(env.get('isPROD'));

        me.addExcludedParameter([
			'namespaces',
			'isDacRequest',
			'baseUrl',
			'serviceUrl',
			'requestUrl',
		    'securityContext',
			'username',
			'password',
			'clientMachine',
			'stationId'
		]);
        // TODO: better integration
        var crmEnvironment = Ext.create('VIP.services.CrmEnvironment');
        var user = crmEnvironment.GetCurrentCrmUser();
        if ((!Ext.isEmpty(user)) && (!Ext.isEmpty(user.data))) {
            user = user.data; 
        }

        securityHeader = Ext.create('VIP.soap.envelopes.components.SecurityHeader', {
            username: user ? user.userName : me.securityContext.username,
            password: '', // not used me.securityContext.password,
            clientMachine: user ? user.clientMachine : me.securityContext.clientMachine,
            stationId: user ? user.stationId : me.securityContext.stationId,
            applicationName: user ? user.applicationName : me.securityContext.applicationName,
            externalUid: user ? user.loginName : me.securityContext.loginName,
            externalKey: user ? user.loginName : me.securityContext.loginName,
            useExternalUid: Ext.isEmpty(me.getUseExternalUid) || Ext.isEmpty(me.getUseExternalUid()) ? false : me.getUseExternalUid()
        });
        
        //Sets the config object with same info
        me.setSecurityContext({
            username: user.userName,
            password: '',
            clientMachine: user.clientMachine,
            stationId: user.stationId,
            applicationName: user.applicationName,
            loginName: user.loginName
        });

        me.addHeader(securityHeader.elementName, securityHeader.data);

        for (var namespace in me.getNamespaces()) {
            me.addNamespace(namespace, me.getNamespaces()[namespace]);
        }
    },

    useDac: function (address, payload) {
        return Ext.create('VIP.soap.envelopes.components.Dac', {
            address: address,
            payload: payload
        });
    },

    applyRequestUrl: function (requestUrl) {
        var me = this,
			useDac = me.getIsDacRequest();

        if (useDac && Ext.isEmpty(requestUrl)) {
            Ext.Error.raise({
                msg: 'Request URL must be defined in the configuration when useDac is true',
                requestUrl: requestUrl
            });
        }

        return requestUrl;
    },

    applySecurityHeaderUserName: function (userName) {
        var me = this,
            request = me.getRequest();

        request.Envelope.Header.Security.UsernameToken.Username = userName;
    },

    applySecurityHeaderPassword: function (password) {
        var me = this,
            request = me.getRequest();

        request.Envelope.Header.Security.UsernameToken.Password.value = password;

    },

    getMethodName: function () {
        var me = this,
			possibleMethodName;

        for (var bodyKey in me.request.Envelope.Body) {
            possibleMethodName = bodyKey;
            break;
        }

        return possibleMethodName;
    },

    toString: function () {
        var me = this,
			rootNodeName = 'Envelope',
			xmlSerializer = Ext.create('VIP.util.xml.Serializer'),
			request = me.getRequest(),
			baseUrl = me.getBaseUrl(),
			serviceUrl = me.getServiceUrl(),
			isDacRequest = me.getIsDacRequest(),
			serializedRequest,
			addressUrl;


        if (!isDacRequest) return me.callParent();

        if (!Ext.isEmpty(baseUrl) && !Ext.isEmpty(serviceUrl)) {
            addressUrl = baseUrl + serviceUrl;
        }

        serializedRequest = xmlSerializer.serialize(request, rootNodeName);

        var dacEnvelope = me.useDac(addressUrl, serializedRequest);

        if (!Ext.isEmpty(dacEnvelope)) {
            return xmlSerializer.serialize(dacEnvelope, rootNodeName);
        }

        return me.callParent();
    }
});