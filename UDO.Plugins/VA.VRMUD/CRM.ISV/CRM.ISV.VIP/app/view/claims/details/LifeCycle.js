/**
* @author Jonas Dawson
* @class VIP.view.claims.details.Contentions
*
* The view for the life cycles for the selected claim
*/
Ext.define('VIP.view.claims.details.LifeCycle', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.lifecycle',
    title: 'Life Cycle',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Claim Id', dataIndex: 'claimId' },
                { header: 'Life Cycle Status', dataIndex: 'lifeCycleStatusTypeName' },
                { header: 'Change Date', dataIndex: 'changedDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'PCAN/PCLR Reason', dataIndex: 'statusReasonTypeName' },
                { header: 'Explanation', dataIndex: 'reasonText' },
                { header: 'Claim Stn', dataIndex: 'stationOfJurisdiction' },
                { header: 'Action Stn', dataIndex: 'actionStationNumber' },
                { header: 'Action Person', dataIndex: 'actionPerson' }
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
                    text: 'Claim Processing Times',
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
