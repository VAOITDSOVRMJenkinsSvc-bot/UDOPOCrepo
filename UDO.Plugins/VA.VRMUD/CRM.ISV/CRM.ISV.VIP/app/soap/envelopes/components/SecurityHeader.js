/**
* @author Jonas Dawson
* @class VIP.soap.envelopes.components.SecurityHeader
*
* VA web service requests require a security header. This class
* encapsulates the logic for manipulating this security header.
*/
Ext.define('VIP.soap.envelopes.components.SecurityHeader', {
    configs: {
        username: '',
        password: '',
        clientMachine: '',
        stationId: '',
        applicationName: '',
        externalUid: '',
        externalKey: '',
        useExternalUid: false
    },

    constructor: function (config) {
        var me = this,
            vaServiceHeaders,
            securityHeader;

        me.initConfig(config);

        //You may have to add vaServiceHeaders as a string to the security header but I doubt it.
        vaServiceHeaders = {
            namespaces: { vaws: 'http://vbawebservices.vba.va.gov/vawss' },
            namespace: 'vaws',
            CLIENT_MACHINE: config.clientMachine,
            STN_ID: config.stationId,
            applicationName: config.applicationName
        };

//        if (config.useExternalUid) {
//            vaServiceHeaders.ExternalUid = config.externalUid ? config.externalUid.replace('\\', '') : '';
//            vaServiceHeaders.ExternalKey = config.externalKey ? config.externalKey.replace('\\', '') : '';
//        }

        securityHeader = {
            elementName: 'Security',
            data: {
                namespace: 'wsse',
                namespaces: { 'wsse': 'http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd' },
                UsernameToken: {
                    //namespace: 'wsse',
                    namespaces: { 'wsu': 'http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd' },
                    Username: config.username,
                    Password: {
                        attributes: {
                            'Type': 'http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText'
                        },
                        value: config.password
                    }
                },
                VaServiceHeaders: vaServiceHeaders
            }
        };

        return securityHeader;
    },

    applyUsername: function (username) {
        if (Ext.isEmpty(username)) {
            Ext.Error.raise({
                msg: 'username must be defined in the configuration'
            });
        }

        return username;
    },

    applyPassword: function (password) {
        if (Ext.isEmpty(username)) {
            Ext.Error.raise({
                msg: 'username must be defined in the configuration'
            });
        }

        return password;
    },

    applyClientMachine: function (clientMachine) {
        if (Ext.isEmpty(username)) {
            Ext.Error.raise({
                msg: 'username must be defined in the configuration'
            });
        }

        return clientMachine;
    },

    applyStationId: function (stationId) {
        if (Ext.isEmpty(username)) {
            Ext.Error.raise({
                msg: 'username must be defined in the configuration'
            });
        }

        return stationId;
    }
});