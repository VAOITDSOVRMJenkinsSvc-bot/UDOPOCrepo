/**
* @author Josh Oliver
* @class VIP.view.appeals.details.Diaries
*
* View for appeals diaries
*/
Ext.define('VIP.view.appeals.details.Diaries', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.appeals.details.diaries',
    title: 'Diaries',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                //                flex: 1
                labelStyle: 'font-weight:bold;font-size:11px;',
                fieldStyle: 'font-size:11px;',
                labelWidth: 120,
                width: 150
            },
            items: [
                { header: 'Req Activity Description', dataIndex: 'requestedActivityDescription' },
                { header: 'Response Notes Description', dataIndex: 'responseNotesDescription' },
                { header: 'Diary Description', dataIndex: 'diaryDescription' },
			    { header: 'Assigned To', dataIndex: 'assignedStaffMemberName' },
			    { header: 'Assigned Date', dataIndex: 'assignedToStaffMemberDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Days To Complete', dataIndex: 'daysToCompleteDiaryItemQuantity' },
			    { header: 'Due Date', dataIndex: 'diarySuspenseDueDate', xtype: 'datecolumn', format: 'm/d/Y' },
			    { header: 'Closed Date', dataIndex: 'diaryClosedDate', xtype: 'datecolumn', format: 'm/d/Y' },
			    { header: 'Status', dataIndex: 'diaryStatusDescription' },
			    { header: 'BVA/RO', dataIndex: 'bvaOrRODiaryIndicatorText' }
            ]
        };

        me.callParent();
    }
});