/**
* @author Ivan Yurisevic
* @class VIP.model.WebServiceHtml
*
* The model for web service faults
*/
Ext.define('VIP.model.WebServiceHtml', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'title',
        type: 'string',
        mapping: 'TITLE'
    }, {
        name: 'header2',
        type: 'string',
        mapping: 'H2'
    },{
        name: 'body',
        type: 'string',
        mapping: 'BODY'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'HTML'
        }
    }
});