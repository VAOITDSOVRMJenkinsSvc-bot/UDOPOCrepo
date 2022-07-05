/**
* @author Josh Oliver
* @class VIP.model.analyze.FaultMessage
*
* The model for web service messages
*/
Ext.define('VIP.model.analyze.FaultMessage', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'code',
            mapping: 'faultCode'
        },
        {
            name: 'message',
            mapping: 'exception/message'
        },
        {
            name: 'shareMessage',
            mapping: 'ShareException/message'
        },
        {
            name: 'faultMessage',
            mapping: 'faultstring'
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Fault'
        }
    }
});