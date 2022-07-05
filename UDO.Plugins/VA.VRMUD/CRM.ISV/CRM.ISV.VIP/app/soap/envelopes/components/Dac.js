/**
* @author Jonas Dawson
* @class VIP.soap.envelopes.components.Dac
*
* Data Access Component wrapper
*/
Ext.define('VIP.soap.envelopes.components.Dac', {
    config: {
        template: {
            Envelope: {
                namespace: 'soap',
                namespaces: {
                    'soap': 'http://schemas.xmlsoap.org/soap/envelope/',

                    'xsd': 'http://www.w3.org/2001/XMLSchema',

                    'xsi': 'http://www.w3.org/2001/XMLSchema-instance'

                },
                Body: {
                    Execute: {
                        namespace: '',
                        attributes: {
                            xmlns: 'http://tempuri.org/'
                        },
                        address: {
                            namespace: ''
                        },
                        '@value': {
                            namespace: '',
                            cdata: {}
                        }
                    }
                }
            }
        },
        address: '',
        payload: {}
    },

    constructor: function (config) {
        var me = this;

        me.initConfig(config);

        return me.getTemplate();
    },

    applyAddress: function (address) {
        var me = this,
            envelope = me.getTemplate().Envelope;

        if (Ext.isEmpty(address)) {
            Ext.Error.raise({
                msg: 'Address must be defined in the configuration when creating a DAC envelope',
                address: address
            });
        }

        envelope.Body.Execute.address.value = address;

        return address;
    },

    applyPayload: function (payload) {
        var me = this,
            envelope = me.getTemplate().Envelope;

        if (Ext.isEmpty(payload)) {
            Ext.Error.raise({
                msg: 'Payload must be defined in the configuration when creating a DAC envelope',
                payload: payload
            });
        }

        envelope.Body.Execute['@value'].cdata = payload;

        return payload;
    }
});