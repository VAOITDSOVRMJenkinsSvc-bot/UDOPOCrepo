/**
* @author Josh Oliver
* @class VIP.view.paymentinformation.details.AwardAdjustments
*
* View for paymentinformation award adjustments
*/
Ext.define('VIP.view.paymentinformation.details.AwardAdjustments', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.paymentinformation.details.awardadjustments',
    store: 'paymentinformation.AwardAdjustmentVo',
    title: 'Award Adjustments',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
			{ header: 'Adjustment Amount', dataIndex: 'adjustmentAmount', xtype: 'numbercolumn', format: '$0.00' },
			{ header: 'Adjustment Eff Date', dataIndex: 'adjustmentEffectiveDate', xtype: 'datecolumn', format: 'm/d/Y' },
			{ header: 'Adjustment Operation', dataIndex: 'adjustmentOperation'},
			{ header: 'Adjustment Type', dataIndex: 'adjustmentType'},
			{ header: 'Allotee Relationship', dataIndex: 'alloteeRelationship'},
			{ header: 'Allotment Recipient', dataIndex: 'allotmentRecipientName'},
			{ header: 'Allotment Type', dataIndex: 'allotmentType'}
            ]
        };

        me.callParent();
    }
});