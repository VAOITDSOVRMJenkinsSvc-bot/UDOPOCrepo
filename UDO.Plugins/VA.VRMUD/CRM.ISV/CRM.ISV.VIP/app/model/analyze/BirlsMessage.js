/**
* @author Josh Oliver
* @class VIP.model.analyze.BirlsMessage
*
* The model for web service messages
*/
Ext.define('VIP.model.analyze.BirlsMessage', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'code',
            mapping: 'RETURN_CODE'
        },
        {
            name: 'message',
            mapping: 'RETURN_MESSAGE'
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