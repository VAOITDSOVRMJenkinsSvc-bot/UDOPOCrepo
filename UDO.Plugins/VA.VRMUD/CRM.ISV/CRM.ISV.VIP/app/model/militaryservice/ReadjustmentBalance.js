/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.ReadjustmentBalance
*
* The model service readjustment balances
*/
Ext.define('VIP.model.militaryservice.ReadjustmentBalance', {
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
            record: 'militaryReadjustmentBalances'
        }
    }
});