/**
* @author Jonas Dawson
* @class VIP.view.claims.details.Suspenses
*
* The view for the Suspenses for the selected claim
*/
Ext.define('VIP.view.claims.details.Suspenses', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.suspenses',
    title: 'Suspenses',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Suspense Date', dataIndex: 'suspenseDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Suspense Reason', dataIndex: 'suspenseReasonText' },
                { header: 'Action Completed On', dataIndex: 'suspenseActionDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Updated By', dataIndex: 'updatedBy' }
            ]
        };

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'checkbox',
                    boxLabel: 'Exclude Closed (CLR, CAN) Claims',
                    submitValue: false
                }
            ]
        }];

        me.callParent();
    }
});
