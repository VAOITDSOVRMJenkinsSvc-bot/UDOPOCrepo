/**
* @author Jonas Dawson
* @class VIP.view.claims.details.Letters
*
* The view for the letters for the selected claim
*/
Ext.define('VIP.view.claims.details.Letters', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.letters',
    title: 'Letters',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Claim Id', dataIndex: 'claimId' },
                { header: 'Date Issued', dataIndex: 'documentDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Type of Letter', dataIndex: 'name' },
                { header: 'Type Code', dataIndex: 'developmentTypeCode' },
                { header: 'Participant ID', dataIndex: 'participantId' }
            ]
        };

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'button',
                    text: 'Refresh Letters',
                    iconCls: 'icon-refresh',
                    action: 'refreshletters'
                },
                {
                    xtype: 'button',
                    text: 'Open Document',
                    tooltip: '',
                    iconCls: 'icon-docDownload',
                    action: 'openletterdocument'
                }
            ]
        }];

        me.callParent();
    }
});
