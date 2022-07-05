Ext.define('VIP.store.paymentinformation.Comerica', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.paymentinformation.Comerica'
    ],
    pageSize: 5,

    model: 'VIP.model.paymentinformation.Comerica'
});