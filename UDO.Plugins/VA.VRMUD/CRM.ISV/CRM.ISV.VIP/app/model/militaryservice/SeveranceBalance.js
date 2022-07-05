/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.SeveranceBalance
*
* The model service severance balances
*/
Ext.define('VIP.model.militaryservice.SeveranceBalance', {
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
            record: 'militarySeveranceBalances'
        }
    }
});