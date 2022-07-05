/**
* @author Ivan Yurisevic
* @class VIP.view.requestsexams.requestsexamstabs.exams.ExamList
*
* The view for the past POA grid
*/
Ext.define('VIP.view.pathways.exams.ExamData', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.pathways.exams.examdata',
    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'Ref #', dataIndex: 'examReferenceNumber'},
			{ header: 'Exam', dataIndex: 'examType' },
			{ header: 'Exam Desc', dataIndex: 'printName' },
			{ header: 'Date of Exam', dataIndex:'dateOfExam', xtype: 'datecolumn', format: 'm/d/Y' },
			{ header: 'Exam Place', dataIndex: 'examPlace'},
			{ header: 'Status', dataIndex: 'status'},
			{ header: 'Patient', dataIndex: 'patientName'},
			{ header: 'Request 2507', dataIndex: 'request2507NamespaceId' },
			{ header: 'Request 2507 Id', dataIndex: 'request2507Identity' },
			{ header: 'Code', dataIndex: 'examTypeCode'},
			{ header: 'Fee Exam', dataIndex:'feeExamText' },
			{ header: 'Original Provider', dataIndex: 'originalProviderName'},
			{ header: 'Cancellation', dataIndex:'cancellationDateTime' },
			{ header: 'Cancelled By', dataIndex: 'cancelledByName'},
			{ header: 'Cancellation Reason', dataIndex:'cancellationReasonText' },
			{ header: 'Exam Phys', dataIndex:'examiningPhysicianName' },
			{ header: 'Trans Out', dataIndex:'dateTransferredOut'},
			{ header: 'Trans Out By', dataIndex: 'transferredOutByName'},
			{ header: 'Trans Out To', dataIndex: 'transferredOutToName' },
			{ header: 'Trans In', dataIndex:'dateTransferredIn'},
			{ header: 'Returned to Owner Site', dataIndex:'dateReturnedToOwnerSite'},
			{ header: 'Insufficient Reason',  dataIndex:'insufficientReasonText' },
			{ header: 'Insufficient Remarks', dataIndex:'insufficientRemarks' },
            { header: 'Transcript Complete', dataIndex:'dateTranscriptionComplete'},
            { header: 'Work Sheet Printed', dataIndex:'workSheetPrinted' },
            { header: 'Patient Id', dataIndex: 'patientIdentity' }
        ];

        me.callParent();
    }
});
