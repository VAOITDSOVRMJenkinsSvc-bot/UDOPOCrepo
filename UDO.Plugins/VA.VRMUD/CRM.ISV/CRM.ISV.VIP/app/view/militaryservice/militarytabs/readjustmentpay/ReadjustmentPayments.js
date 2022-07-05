/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.readjustmentpay.ReadjustmentPayments
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.readjustmentpay.ReadjustmentPayments', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.readjustmentpay.readjustmentpayments',
    store: 'militaryservice.ReadjustmentPayment',
    title: 'Readjustment Payments',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Line Item', dataIndex: 'lineItemNumber' },
                { header: 'Gross Amount', dataIndex: 'grossAmount' },
                { header: 'Less Federal Tax', dataIndex: 'lessFedTaxAmount' },
                { header: 'Receipt Date', dataIndex: 'receiptDate' },
                { header: 'Participant Id', dataIndex: 'participantId' },
                { header: 'Reason', dataIndex: 'reasonText' },
                { header: 'Code Reason', dataIndex: 'useCodeReasonText' }
            ]
        };

        me.callParent();
    }
});
