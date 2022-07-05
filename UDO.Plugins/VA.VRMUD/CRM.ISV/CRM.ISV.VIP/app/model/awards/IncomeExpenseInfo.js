/**
* @author Ivan Yurisevic
* @class VIP.model.awards.IncomeExpenseInfo
*
* The model for Awards IncomeExpenseInfo record details
*/
Ext.define('VIP.model.awards.IncomeExpenseInfo', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.claimant.FindIncomeExpense', //SOAP
        'VIP.data.reader.IncomeExpense', //Reader
        'VIP.model.awards.incomeexpense.Expense', 
        'VIP.model.awards.incomeexpense.Income'
    ],
    fields: [{
        name: 'awardTypeCode',
        mapping: 'awardTypeCd',
        type: 'string'
    }, {
        name: 'effectiveDate',
        mapping: 'effectiveDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'familyIncomeInd',
        mapping: 'familyIncomeInd',
        type: 'string'
    }, {
        name: 'familyNetWorthAmount',
        mapping: 'incomeSummaryRecords/familyNetWorthAmount',
        type: 'string'
    }, {
        name: 'ivap',
        mapping: 'ivap',
        type: 'string'
    }, {
        name: 'ivmAdjustmentInd',
        mapping: 'ivmAdjustmentInd',
        type: 'string'
    }, {
        name: 'netWorthInd',
        mapping: 'netWorthInd',
        type: 'string'
    }, {
        name: 'numberOfExpenseRecords',
        mapping: 'numberOfExpenseRecords',
        type: 'string'
    }, {
        name: 'numberOfIncomeRecords',
        mapping: 'numberOfIncomeRecords',
        type: 'string'
    }, {
        name: 'potentialFraudInd',
        mapping: 'potentialFraudInd',
        type: 'string'
    }, {
        name: 'participantBeneId',
        mapping: 'ptcpntBeneId',
        type: 'string'
    }, {
        name: 'participantVetId',
        mapping: 'ptcpntVetId',
        type: 'string'
    }, {
        name: 'returnCode',
        mapping: 'returnCode',
        type: 'string'
    }, {
        name: 'returnMessage',
        mapping: 'returnMessage',
        type: 'string'
    }],

    //Start Associations
    hasMany: [{
        model: 'VIP.model.awards.incomeexpense.Expense',
        name: 'expenses',
        associationKey: 'expenseRecord'
    }, {
        model: 'VIP.model.awards.incomeexpense.Income',
        name: 'incomes',
        associationKey: 'incomeRecord'
    }],

    //Start SOAP Proxy
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'incomeexpense',
            record: 'incomeSummaryRecords'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.claimant.FindIncomeExpense',
            update: '',
            destroy: ''
        }
    }
});