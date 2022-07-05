/**
* @author Josh Oliver
* @class VIP.view.appeals.details.issuesremandreasons.Issues
*
* View for appeals diaries
*/
Ext.define('VIP.view.appeals.details.issuesremandreasons.Issues', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.appeals.details.issuesremandreasons.issues',
    title: 'Issues',
    padding: '0 5 0 0',
    initComponent: function () {
        var me = this;
        me.setAutoScroll(true);
        me.columns = {
            defaults: {
                width: 120
            },
            items: [
                { header: 'Sequence Number', dataIndex: 'issueSequenceNumber', width: 100 },
                { header: 'Level 2', dataIndex: 'issueLevel2', width: 260 },
			    { header: 'Issue Code Description', dataIndex: 'issueCodeDescription' },
			    { header: 'Issue Description', dataIndex: 'issueDescription' },
			    { header: 'Program Description', dataIndex: 'issueProgramDescription' },
			    { header: 'Disposition Description', dataIndex: 'issueDispositionDescription' },
			    { header: 'Disposition Date', dataIndex: 'issueDispositionDate', xtype: 'datecolumn', format: 'm/d/Y' },
			    { header: 'Level 1 Description', dataIndex: 'issueLevel1Description' },
			    { header: 'Level 3 Description', dataIndex: 'issueLevel3Description' }
            ]
        };

        me.callParent();
    }
});