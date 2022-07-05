/**
* @author Josh Oliver
* @class VIP.model.analyze.MapDMessage
*
* The model for web service messages
*/
Ext.define('VIP.model.analyze.MapDMessage', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'code',
            mapping: 'errorCode'
        },
        {
            name: 'message',
            mapping: 'displayMessage'
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'return'
        }
    }
});