/**
* @author Jonas Dawson
* @class VIP.view.awards.details.IncomeSummary
*
* Income Summary for the selected award line
*/
Ext.define('VIP.view.awards.details.IncomeSummary', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.incomesummary',
    store: 'awards.IncomeExpenseInfo',
    title: 'Income Summary',

    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'Award Type', dataIndex: 'awardTypeCode' },
            { header: 'Effective Date', dataIndex: 'effectiveDate', xtype: 'datecolumn', format: 'm/d/Y' },
            { header: 'Family Income', dataIndex: 'familyIncomeInd' },
            { header: 'Family Net Worth', dataIndex: 'familyNetWorthAmount' },
            { header: 'IVAP', dataIndex: 'ivap' },
            { header: 'IVM Adjustment', dataIndex: 'ivmAdjustmentInd' },
            { header: 'Net Worth', dataIndex: 'netWorthInd' },
            { header: 'Potential Fraud', dataIndex: 'potentialFraudInd' },
            { header: '# of Expense Records', dataIndex: 'numberOfExpenseRecords' },
            { header: '# of Income Records', dataIndex: 'numberOfIncomeRecords' },
            { header: 'Bene Ptcpnt Id', dataIndex: 'participantBeneId' },
            { header: 'Vet Ptcpnt Id', dataIndex: 'participantVetId' }
        ];

        me.callParent();
    }
});
