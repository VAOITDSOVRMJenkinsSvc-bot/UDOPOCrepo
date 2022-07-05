/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.separationpay.SeparationPayments
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.separationpay.SeparationPayments', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.separationpay.separationpayments',
    store: 'militaryservice.SeparationPayment',
    title: 'Separation Payments',

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
                { header: 'Participant Id', dataIndex: 'participantId' }
            ]
        };

        me.callParent();
    }
});
