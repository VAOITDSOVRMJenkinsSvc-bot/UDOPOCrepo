/**
* @author Josh Oliver
* @class VIP.model.analyze.ShareMessage
*
* The model for web service messages
*/
Ext.define('VIP.model.analyze.ShareMessage', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'code',
            mapping: 'returnCode'
        },
        {
            name: 'message',
            mapping: 'returnMessage'
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