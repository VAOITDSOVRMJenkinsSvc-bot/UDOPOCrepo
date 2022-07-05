/**
* @author Ivan Yurisevic
* @class VIP.view.paymenthistory.payments.paymentdata.Payments
*
* The view for the main payments grid for old payment history
*/
Ext.define('VIP.view.paymenthistory.payments.paymentdata.Payments', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.paymenthistory.payments.paymentdata.payments',
    store: 'paymenthistory.Payments',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
			    { header: 'Type', dataIndex: 'payCheckType' },
			    { header: 'Amount', dataIndex: 'payCheckAmount', format: '$0.00' },
			    { header: 'Pay Date', dataIndex: 'payCheckDate', xtype: 'datecolumn', format: 'm/d/Y' },
			    { header: 'Check Id', dataIndex: 'payCheckID' },
			    { header: 'Paid By', dataIndex: 'payCheckReturnFiche' }
            ]
        };

        me.callParent();
    }
});
