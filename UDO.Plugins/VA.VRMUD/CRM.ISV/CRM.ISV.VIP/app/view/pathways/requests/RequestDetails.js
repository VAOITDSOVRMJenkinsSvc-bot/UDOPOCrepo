/**
* @author Ivan Yurisevic
* @class VIP.view.requestsexams.requestsexamstabs.exams.ExamDetails
*
* The view for the past POA grid
*/
Ext.define('VIP.view.pathways.requests.RequestDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.pathways.requests.requestdetails',
    store: 'pathways.ExamRequest',
    title: 'Exam Request Information',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'RO',
            name: 'roName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Status',
            name: 'status'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Disabilities Line 1',
            name: 'otherDisabilitiesLine1'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date Released',
            name: 'dateReleased'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Approval',
            name: 'approvalDateTime'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Patient',
            name: 'patientName'

        }, {
            xtype: 'displayfield',
            fieldLabel: 'Last Exam Add Date',
            name: 'lastExamAddDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Disabilities Line 2',
            name: 'otherDisabilitiesLine2'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Released By',
            name: 'releasedByName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date Last Rating Exam',
            name: 'lastRatingExamDate_f'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Request Date',
            name: 'requestDate_f'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Last Person To Add',
            name: 'lastPersonToAddExamName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Disabilities Line 3',
            name: 'otherDisabilitiesLine3'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Owner Domain',
            name: 'ownerDomain'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Other Docs Req',
            name: 'otherDocumentsRequired'
        },
        //            {
        //                xtype: 'displayfield',
        //                fieldLabel: 'Requestor'
        //            },
        {
        xtype: 'displayfield',
        fieldLabel: 'Cancellation Date',
        name: 'cancellationDate'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Orig Request Pointer',
        name: 'originalRequestPointer'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date Trans To Other Site',
        name: 'dateTransferredToOtherSite'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Routing Loc',
        name: 'routingLocationName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Priority of Exam',
        name: 'priorityOfExam'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Cancelled By',
        name: 'cancelledByName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Orig 2507 Request',
        name: 'original2507RequestIdentity'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date Trans In From Remote Site',
        name: 'dateTransferredInFromRemoteSite'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Claim Folder Req',
        name: 'claimFolderRequired'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date Completed',
        name: 'dateCompleted'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Remarks',
        name: 'remarks'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Orig 2507 Processing',
        name: 'original2507ProcessingTime'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date All Trans Returned',
        name: 'dateAllTransfersReturned'
    },
    //            {
    //                xtype: 'displayfield',
    //                fieldLabel: 'Transcription Date',
    //                name: 
    //            },
        {
        xtype: 'displayfield',
        fieldLabel: 'Date Sched Completed',
        name: 'dateSchedulingCompleted_f'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date Remarks Mod',
        name: 'remarksModificationDate'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Elapsed Time',
        name: 'elapsedTime'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Return To Owner Site By',
        name: 'returnedToOwnerSiteByName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date Printed by RO',
        name: 'datePrintedByRO'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date Reported to MAS',
        name: 'dateReportedToMas'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Remarks Mod By',
        name: 'remarksModifiedByName',
        colspan: 3
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Date Approved',
        name: 'dateApproved'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Printed By',
        name: 'printedByName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Approved By',
        name: 'approvedBy'
    }];

    me.callParent();
}
});