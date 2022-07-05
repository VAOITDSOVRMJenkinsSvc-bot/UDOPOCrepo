/**
* @author Ivan Yurisevic
* @class VIP.model.awards.awardinfo.AccountBalances
*
* Submodel for the award info response. Required with the association.
*/
Ext.define('VIP.model.awards.awardinfo.AccountBalances', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'balance',
        mapping: 'balance',
        type: 'float'
    }, {
        name: 'code',
        mapping: 'code',
        type: 'string'
    }, {
        name: 'name',
        mapping: 'name',
        type: 'string'
    }],

    //Start Memory proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'accountBalances'
        }
    }
});