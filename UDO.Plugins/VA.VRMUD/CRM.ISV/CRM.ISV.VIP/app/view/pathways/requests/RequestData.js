/**
* @author Ivan Yurisevic
* @class VIP.view.requestsexams.requestsexamstabs.requests.RequestList
*
* The view for the past POA grid
*/
Ext.define('VIP.view.pathways.requests.RequestData', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.pathways.requests.requestdata',
    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'RO', dataIndex: 'roName' },
            { header: 'Patient', dataIndex: 'patientName' },
            { header: 'Request Date', dataIndex: 'requestDate', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Requestor', dataIndex: 'requestorName' },
            { header: 'Priority', dataIndex: 'priorityOfExam' },
            { header: 'Completed', dataIndex: 'dateCompleted' },
            { header: 'Sched Completed Date', dataIndex: 'dateSchedulingCompleted', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Rep to MAS', dataIndex: 'dateReportedToMas', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Approved', dataIndex: 'dateApproved', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Approved By', dataIndex: 'approvedBy' },
            { header: 'Status', dataIndex: 'status' },
            { header: 'Last Exam Add Date', dataIndex: 'lastExamAddDate', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Last To Add', dataIndex: '' },
            { header: 'Cancellation Date', dataIndex: 'cancellationDate', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Cancelled By', dataIndex: 'cancelledByName' },
            { header: 'Remarks', dataIndex: 'remarks' },
            { header: 'Remarks Mod', dataIndex: 'remarksModificationDate' },
            { header: 'Remarks Mod By', dataIndex: 'remarksModifiedByName' },
            { header: 'Approval', dataIndex: 'approvalDateTime', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Last Rating Exam', dataIndex: 'lastRatingExamDate', xtype: 'datecolumn', format: 'm/d/Y' },
            { header: 'Disabilities 1', dataIndex: 'otherDisabilitiesLine1' },
            { header: 'Disabilities 2', dataIndex: 'otherDisabilitiesLine2' },
            { header: 'Disabilities 3', dataIndex: 'otherDisabilitiesLine3' },
            { header: 'Original Request', dataIndex: 'originalRequestPointer' },
            { header: 'Orig 2507 Req', dataIndex: 'original2507RequestIdentity' },
            { header: 'Orig 2507 Processing', dataIndex: 'original2507ProcessingTime' },
            { header: 'Elapsed Time', dataIndex: 'elapsedTime' },
            { header: 'Other Doc Req', dataIndex: 'otherDocumentsRequired' },
            { header: 'Routing Loc', dataIndex: 'routingLocationName' },
            { header: 'Claim Folder Req', dataIndex: 'claimFolderRequired' },
            { header: 'Released', dataIndex: 'dateReleased', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Released By', dataIndex: 'releasedByName' },
            { header: 'Owner Domain', dataIndex: 'ownerDomain' },
            { header: 'Trans To Other Site', dataIndex: 'dateTransferredToOtherSite', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Trans In From Remote Site', dataIndex: 'dateTransferredInFromRemoteSite', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'All Trans Ret', dataIndex: 'dateAllTransfersReturned', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Ret To Owner Site', dataIndex: 'returnedToOwnerSiteByName' },
            { header: 'Printed By RO Date', dataIndex: 'datePrintedByRO', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
            { header: 'Printed By', dataIndex: 'printedByName' }, 
            { header: 'Transcription', dataIndex: 'transcriptionDate' }
        ];

        me.callParent();
    }
});