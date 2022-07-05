/**
* @author Josh Oliver
* @class VIP.view.appeals.details.issuesremandreasons.RemandReasons
*
* View for appeals diaries
*/
Ext.define('VIP.view.appeals.details.issuesremandreasons.RemandReasons', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.appeals.details.issuesremandreasons.remandreasons',
    title: 'Remand Reasons',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Cert To BVA Indicator', dataIndex:'remandReasonCertifiedToBVAIndicator' },
			    { header: 'Issue Sequence Number', dataIndex:'remandIssueSequenceNumber' },
			    { header: 'Reason Description', dataIndex:'remandReasonDescription' },
			    { header: 'Modified By Name', dataIndex: 'lastModifiedByName' },
			    { header: 'Modified Date', dataIndex: 'lastModifiedDate', xtype: 'datecolumn', format: 'm/d/Y' }
            ]
        };

        me.callParent();
    }
});