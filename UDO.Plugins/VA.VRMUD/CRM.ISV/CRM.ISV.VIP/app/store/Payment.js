/**
* @author Josh Oliver
* @class VIP.store.paymentinformation.Payment
*
* Store associated with paymentinformation payments
*/
Ext.define('VIP.store.Payment', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.Payment'
    ],
    pageSize: 10,
    model: 'VIP.model.Payment',
    sorters: { property: 'paymentSortDate', direction: 'DESC' },
    filters: [
        {
            filterFn: function (item) {
                if (item.get('bdnRecordType') === 'BDN Payment') {
                    var pattern = /^((Compensation & Pension)|(Compensation and Pension))/i;
                    return !pattern.test(item.get('paymentType'));
                }

                return true;
            }
        }
    ]
});