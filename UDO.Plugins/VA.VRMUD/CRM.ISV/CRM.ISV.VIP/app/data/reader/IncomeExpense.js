Ext.define('VIP.data.reader.IncomeExpense', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.incomeexpense',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("incomeRecord", "incomeRecords", "incomeSummaryRecords", response);
        VIP.util.Xml.wrap("expenseRecord", "expenseRecords", "incomeSummaryRecords", response);

        return me.callParent([response]);
    }
});



