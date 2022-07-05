/**
* @author Josh Oliver
* @class VIP.view.paymentinformation.details.PaymentAdjustments
*
* View for paymentinformation payment adjustments
*/
Ext.define('VIP.view.paymentinformation.details.PaymentAdjustments', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.paymentinformation.details.paymentadjustments',
    store: 'paymentinformation.PaymentAdjustmentVo',
    title: 'Payment Adjustments',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [

			{ header: 'Adjustment Type', dataIndex: 'adjustmentType'},
			{ header: 'Adjustment Category', dataIndex: 'adjustmentCategory'},
			{ header: 'Adjustment Operation', dataIndex: 'adjustmentOperation'},
			{ header: 'Adjustment Amount', dataIndex: 'adjustmentAmount', xtype: 'numbercolumn', format: '$0.00' },
			{ header: 'Adjustment Reason', dataIndex: 'adjustmentReason'},
			{ header: 'Adjustment Source', dataIndex: 'adjustmentSource' } 
           ]
        };

        me.callParent();
    }
});