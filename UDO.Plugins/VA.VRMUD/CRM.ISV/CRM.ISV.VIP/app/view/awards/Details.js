/**
* @author Jonas Dawson
* @class VIP.view.awards.Details
*
* Tab panel for award line details
*/
Ext.define('VIP.view.awards.Details', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.awards.details',
    tabPosition: 'top',
    requires: [
        'VIP.view.awards.details.PayeeFiduciary',
        'VIP.view.awards.details.AwardDetails',
        'VIP.view.awards.details.Receivables',
        'VIP.view.awards.details.Deductions',
        'VIP.view.awards.details.Proceeds',
        'VIP.view.awards.details.AwardLines',
        'VIP.view.awards.details.IncomeSummary',
        'VIP.view.awards.details.IncomeExpenseRecords',
        'VIP.view.awards.details.Diaries',
        'VIP.view.awards.details.Evrs',
        'VIP.view.awards.details.ClothingAllowances'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'awards.details.payeefiduciary' },
            { xtype: 'awards.details.awarddetails' },
            { xtype: 'awards.details.receivables' },
            { xtype: 'awards.details.deductions' },
            { xtype: 'awards.details.proceeds' },
            { xtype: 'awards.details.awardlines' },
            { xtype: 'awards.details.incomesummary' },
            { xtype: 'awards.details.incomeexpenserecords' },
            { xtype: 'awards.details.diaries' },
            { xtype: 'awards.details.evrs' },
            { xtype: 'awards.details.clothingallowances' }

        ];

        me.callParent();
    }
});