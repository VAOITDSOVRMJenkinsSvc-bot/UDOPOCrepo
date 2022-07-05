/**
* @author Hung Tran
* @class VIP.store.awards.ClothingAllowances
*
* Store associated with ClothingAllowances model
*/
Ext.define('VIP.store.awards.ClothingAllowances', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.awards.awardinfo.ClothingAllowances'
    ],
    model: 'VIP.model.awards.awardinfo.ClothingAllowances'
});