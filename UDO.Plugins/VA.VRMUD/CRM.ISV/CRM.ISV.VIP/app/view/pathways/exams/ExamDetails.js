/**
* @author Ivan Yurisevic
* @class VIP.view.requestsexams.requestsexamstabs.exams.ExamDetails
*
* The view for the past POA grid
*/
Ext.define('VIP.view.pathways.exams.ExamDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.pathways.exams.examdetails',
    store: 'pathways.Exam',
    title: 'Exam Information',
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
            fieldLabel: 'Ref #',
            name: 'examReferenceNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Status',
            name: 'status'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Cancellation Date',
            name: 'cancellationDateTime'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date Trans Out',
            name: 'dateTransferredOut'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Exam',
            name: 'examTypeText'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Exam Type',
            name: 'examType'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Cancelled By',
            name: 'cancelledByName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Trans Out By',
            name: 'transferredOutByName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date of Exam',
            name: 'dateOfExam_f'

        }, {
            xtype: 'displayfield',
            fieldLabel: 'Fee Exam',
            name: 'feeExamText'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Cancellation Reason',
            name: 'cancellationReasonText'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Trans Out To',
            name: 'transferredOutToName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Exam Place',
            name: 'examPlace'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Orig Provider',
            name: 'originalProviderName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Insufficient Reason',
            name: 'insufficientReasonText'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date Trans In',
            name: 'dateTransferredIn'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Patient',
            name: 'patientName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Exam Phys',
            name: 'examiningPhysicianName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Insufficient Remarks',
            name: 'insufficientRemarks'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date Returned To Owner Site',
            name: 'dateReturnedToOwnerSite'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Request 2507',
            name: 'request2507NamespaceId'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Date Transcript Complete',
            name: 'dateTranscriptionComplete',
            colspan: 2
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Work Sheet Printed',
            name: 'workSheetPrinted'
        }];

        me.callParent();
    }
});