/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.RetirementPay
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.RetirementPay', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.retirementpay',
    store: 'militaryservice.RetirementPayment',
    title: 'Retirement Pay',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { width: 130 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Line Item', dataIndex: 'lineItemNumber' },
                { header: 'Pay Type Code', dataIndex: 'retirementPayTypeCode' },
                { header: 'Effective Date', dataIndex: 'effectiveDate' },
                { header: 'Full Waiver', dataIndex: 'fullWaiverIndicator' },
                { header: 'Gross Monthly Amount', dataIndex: 'grossMonthlyAmount' },
                { header: 'Less Federal Tax', dataIndex: 'lessFedTaxAmount' },
                { header: 'Retirement Waived Date', dataIndex: 'retirementWaivedDate', width: 150 },
                { header: 'SBP Overpayment Amount', dataIndex: 'sbpOverpaymentAmount', width: 150 },
                { header: 'Participant Id', dataIndex: 'participantId' }
            ]
        };

        me.callParent();
    }
});
