/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.severancepay.SeverancePayments
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.severancepay.SeverancePayments', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.severancepay.severancepayments',
    store: 'militaryservice.SeverancePayment',
    title: 'Severance Payments',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Line Item', dataIndex: 'lineItemNumber' },
                { header: 'Gross Amount', dataIndex: 'grossAmount', xtype: 'numbercolumn', format: '$0,000.00' },
                { header: 'Less Federal Tax', dataIndex: 'lessFedTaxAmount', xtype: 'numbercolumn', format: '$0,000.00' },
                { header: 'Disability', dataIndex: 'disabilityText' },
                { header: 'Participant Id', dataIndex: 'participantId' }
            ]
        };

        me.callParent();
    }
});
