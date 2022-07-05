/**
* @author Josh Oliver
* @class VIP.model.appeals.SpecialContentions
*
* The model for appeal dates
*/
Ext.define('VIP.model.appeals.SpecialContentions', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'contentionCode',
            type: 'string',
            mapping: 'ContentionCode'
        },
        {
            name: 'contentionDescription',
            type: 'string',
            mapping: 'ContentionDescription'
        },
        {
            name: 'contentionIndicator',
            type: 'string',
            mapping: 'ContentionIndicator'
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'SpecialContentions'
        }
    }
});