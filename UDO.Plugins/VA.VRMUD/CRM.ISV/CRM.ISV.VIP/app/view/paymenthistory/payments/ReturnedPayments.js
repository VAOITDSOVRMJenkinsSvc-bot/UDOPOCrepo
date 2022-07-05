/**
* @author Ivan Yurisevic
* @class VIP.view.paymenthistory.payments.ReturnedPayments
*
* The view for the returned payments grid for old payment history
*/
Ext.define('VIP.view.paymenthistory.payments.ReturnedPayments', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.paymenthistory.payments.returnedpayments',
    store: 'paymenthistory.ReturnedPayments',
    title: 'Returned Payment Data',
    layout: {
        type: 'fit'
    },
    initComponent: function () {
        var me = this;

        me.columns = [
            { xtype: 'rownumberer' },
			{ header: 'Type', dataIndex: 'checkType', flex: 1 },
			{ header: 'Amount', dataIndex: 'checkAmount', format: '$0.00' },
			{ header: 'Issue Date', dataIndex: 'checkIssueDate' },
			{ header: 'Cancel Date', dataIndex: 'checkCancelDate' },
			{ header: 'Check Num', dataIndex: 'checkNum' },
			{ header: 'Reason', dataIndex: 'checkReason' },
			{ header: 'RO', dataIndex: 'checkRO' },
			{ header: 'Paid By', dataIndex: 'checkReturnFiche' }
        ];

        me.callParent();
    }
});
