/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.SeparationBalance
*
* The model service separation balances
*/
Ext.define('VIP.model.militaryservice.SeparationBalance', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'currentBalance',
        type: 'string',
        mapping: 'currentBalance'
    }, {
        name: 'dateOfZeroBalance',
        type: 'string',
        mapping: 'dateOfZeroBalance'
    }, {
        name: 'originalBalance',
        type: 'string',
        mapping: 'originalBalance'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militarySeperationBalances'
        }
    }
});