/**
* @author Hung Tran
* @class VIP.view.awards.details.ClothingAllowances
*
* ClothingAllowances for the selected award line
*/
Ext.define('VIP.view.awards.details.ClothingAllowances', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.clothingallowances',
    store: 'awards.ClothingAllowances',
    title: 'Clothing Allowances',

    initComponent: function () {
        var me = this;

        me.columns = {
            items: [
                { header: 'Eligibility Year', dataIndex: 'eligibilityYear', xtype: 'datecolumn', format: 'm/d/Y', width: 150 },
                { header: 'Net Award', dataIndex: 'netAward', xtype: 'numbercolumn', format: '$0.00', width: 150 },
                { header: 'Gross Award', dataIndex: 'grossAward', xtype: 'numbercolumn', format: '$0.00', width: 150 },
                { header: 'Incarceration Adjustment', dataIndex: 'incarcerationAdjustment', width: 150 }
            ]

        };

        me.callParent();
    }
});