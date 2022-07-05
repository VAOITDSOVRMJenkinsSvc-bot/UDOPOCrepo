/**
* @author Josh Oliver
* @class VIP.model.WebServiceFault
*
* The model for web service faults
*/
Ext.define('VIP.model.WebServiceFault', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'faultCode',
        type: 'string',
        mapping: 'faultcode'
    }, {
        name: 'faultMessage',
        type: 'string',
        mapping: 'faultstring'
    }, {
        name: 'exceptionMessage',
        type: 'string',
        mapping: 'exception/message'
    }, {
        name: 'shareMessage',
        mapping: 'ShareException/message'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Fault'
        }
    }
});