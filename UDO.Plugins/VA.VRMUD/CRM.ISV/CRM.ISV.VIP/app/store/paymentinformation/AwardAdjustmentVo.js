/**
* @author Josh Oliver
* @class VIP.store.paymentinformation.AwardAdjustmentVo
*
* Store associated with paymentinformation award adjustment vos
*/
Ext.define('VIP.store.paymentinformation.AwardAdjustmentVo', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.paymentinformation.AwardAdjustmentVo'
    ],
    model: 'VIP.model.paymentinformation.AwardAdjustmentVo'
});