/**
* @author Jonas Dawson
* @class VIP.view.claims.details.TrackedItems
*
* The view for the tracked items for the selected claim
*/
Ext.define('VIP.view.claims.details.TrackedItems', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.trackeditems',
    title: 'Tracked Items',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Claim Id', dataIndex: 'claimId' },
                { header: 'Recipient', dataIndex: 'recipient' },
                { header: 'Development Actions/Letters', dataIndex: 'shortName' },
                { header: 'Request Date', dataIndex: 'requestDate', xtype: 'datecolumn', format: 'm/d/Y' /**/ },
                { header: 'Suspense Date', dataIndex: 'suspenseDate', xtype: 'datecolumn', format: 'm/d/Y' /**/ },
                { header: 'Receive Date', dataIndex: 'receiveDate', xtype: 'datecolumn', format: 'm/d/Y' /**/ },
                { header: 'Closed Date', dataIndex: 'acceptDate', xtype: 'datecolumn', format: 'm/d/Y'/* */ },
                { header: 'In Error', dataIndex: 'inErrorDate', xtype: 'datecolumn', format: 'm/d/Y/' /**/ },
                { header: 'Follow Up', dataIndex: 'followupDate', xtype: 'datecolumn', format: 'm/d/Y' /**/ },
                { header: '2nd Follow Up', dataIndex: 'secondFollowUpDate', xtype: 'datecolumn', format: 'm/d/Y' /**/ },
                { header: 'Dev Item Tc', dataIndex: 'developmentTypeCode' }
            ]
        };

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'button',
                    text: 'Refresh Tracked Items',
                    iconCls: 'icon-refresh',
                    action: 'refreshtrackeditems'
                },
                { xtype: 'tbseparator' },
                {
                    xtype: 'button',
                    text: 'Open Document',
                    tooltip: '',
                    iconCls: 'icon-docDownload',
                    action: 'opentrackeditemdocument'
                }
            ]
        }];

        me.callParent();
    }
});
