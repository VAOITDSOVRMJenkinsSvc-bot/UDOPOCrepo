/**
* @author Jonas Dawson
* @class VIP.view.claims.details.Status
*
* The view for the statuses for the selected claim
*/
Ext.define('VIP.view.claims.details.Status', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.status',
    title: 'Status',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Claim Id', dataIndex: 'claimId' },
                { header: 'Status', dataIndex: 'claimLocationStatusTypeName' },
                { header: 'Action Location', dataIndex: 'actionLocationId' },
                { header: 'Change Date', dataIndex: 'changedDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Days in Status', dataIndex: 'daysInStatus' }
            ]
        };

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'checkbox',
                    boxLabel: 'Exclude Closed (CLR, CAN) Claims',
                    submitValue: false
                },
                { xtype: 'tbseparator' },
                {
                    xtype: 'button',
                    text: 'Claim Processing Time for Selected Status',
                    tooltip: '',
                    iconCls: 'icon-clock',
                    action: 'claimprocessingtimes',
                    hidden: true
                },
                { xtype: 'tbseparator', hidden: true }
            ]
        }];

        me.callParent();
    }
});
