/**
* @author Jonas Dawson
* @class VIP.view.claims.details.Contentions
*
* The view for the evidences for the selected claim
*/
Ext.define('VIP.view.claims.details.Evidence', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.evidence',
    title: 'Evidence',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Evidence Name', dataIndex: 'descriptionText' },
                { header: 'Date Received', dataIndex: 'receivedDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Journal Date', dataIndex: 'journalDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Last Updated By', dataIndex: 'userName' }
            ]
        };

        me.callParent();
    }
});
