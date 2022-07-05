/**
* @author Jonas Dawson
* @class VIP.view.claims.details.Contentions
*
* The view for the contentions for the selected claim
*/
Ext.define('VIP.view.claims.details.Contentions', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.claims.details.contentions',
    title: 'Contentions',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Claim Id', dataIndex: 'claimId' },
                { header: 'Contention / Classification', dataIndex: 'contentclass' },
                { header: 'Claim Received', dataIndex: 'claimReceivedDate', xtype: 'datecolumn', format: 'M/d/Y' },
                { header: 'Medical', dataIndex: 'medIndicator_f' },
                { header: 'Code Sheet Diagnosis', dataIndex: 'diagnosticTypeName' },
                { header: 'Diagnostic Code', dataIndex: 'diagnosticTypeCode' },
                { header: 'Special Issues', dataIndex: '' },
                { header: 'Status', dataIndex: 'contentionStatusTypeCode' }
            ]
        };

        me.callParent();
    }
});