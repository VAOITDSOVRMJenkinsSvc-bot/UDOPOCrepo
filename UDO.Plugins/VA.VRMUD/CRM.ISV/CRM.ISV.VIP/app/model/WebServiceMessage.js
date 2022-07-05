/**
* @author Josh Oliver
* @class VIP.model.WebServiceMessage
*
* The model for web service messages.  Modeled after sensitive file errors/Access Violation.
*/
Ext.define('VIP.model.WebServiceMessage', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'birlsReturnCode',
        type: 'string',
        mapping: 'RETURN_CODE'
    }, {
        name: 'birlsReturnMessage',
        type: 'string',
        mapping: 'RETURN_MESSAGE'
    }, {
        name: 'title',
        type: 'string',
        mapping: 'title'
    }, {
        name: 'corpReturnCode',
        type: 'string',
        mapping: 'returnCode'
    }, {
        name: 'corpReturnMessage',
        type: 'string',
        mapping: 'returnMessage'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'return'
        }
    }
});