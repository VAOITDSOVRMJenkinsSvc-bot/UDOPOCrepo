/**
* @author Jonas Dawson
* @class VIP.util.soap.Envelope
*
* Template SOAP envelope, to be used with the SOAP Request Builder.
*/
Ext.define('VIP.util.soap.Envelope', {
    config: {
        request: {
            Envelope: {
                namespace: 'soapenv',
                namespaces: {
                    'soapenv': 'http://schemas.xmlsoap.org/soap/envelope/',

                    'xsd': 'http://www.w3.org/2001/XMLSchema',

                    'xsi': 'http://www.w3.org/2001/XMLSchema-instance'

                },
                Header: {},
                Body: {}
            }
        }
    },

    constructor: function (config) {
        var me = this,
            envelopeTemplate;

        me.initConfig(config);

        envelopeTemplate = me.getRequest();

        return envelopeTemplate;
    }
});