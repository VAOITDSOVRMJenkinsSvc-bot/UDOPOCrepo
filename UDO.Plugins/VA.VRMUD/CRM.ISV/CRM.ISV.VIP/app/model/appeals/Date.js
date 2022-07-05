/**
* @author Josh Oliver
* @class VIP.model.appeals.Date
*
* The model for appeal dates
*/
Ext.define('VIP.model.appeals.Date', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'dateTypeCode',
            type: 'string',
            mapping: 'DateTypeCode'
        },
        {
            name: 'dateTypeDescription',
            type: 'string',
            mapping: 'DateTypeDescription'
        },
        {
            name: 'date',
            type: 'date',
            mapping: 'Date',
            dateFormat: 'Y-m-d'
        },
        {
        	name: 'date_s',
        	type: 'string',
        	mapping: 'Date'
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'AppealDate'
        }
    }
});