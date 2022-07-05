/**
* @author Josh Oliver
* @class VIP.model.pathways.Exam
*
* The model for appointments
*/
Ext.define('VIP.model.pathways.Exam', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'identity',
            type: 'string',
            mapping: 'recordIdentifier/identity'
        },
        {
            name: 'namespaceId',
            type: 'string',
            mapping: 'recordIdentifier/namespaceId'
        },
        {
            name: 'universalId',
            type: 'string',
            mapping: 'recordIdentifier/universalId'
        },
        {
            name: 'universalIdType',
            type: 'string',
            mapping: 'recordIdentifier/universalIdType'
        },
        {
            name: 'examReferenceNumber',
            type: 'string'
        },
        {
            name: 'patientIdentity',
            type: 'string',
            mapping: 'patient/identifier/identity'
        },
        {
            name: 'patientAssigningFacility',
            type: 'string',
            mapping: 'patient/identifier/assigningFacility'
        },
        {
            name: 'patientAssigningAuthority',
            type: 'string',
            mapping: 'patient/identifier/assigningAuthority'
        },
        {
            name: 'patientPrefix',
            type: 'string',
            mapping: 'patient/name/prefix'
        },
        {
            name: 'patientGiven',
            type: 'string',
            mapping: 'patient/name/given'
        },
        {
            name: 'patientMiddle',
            type: 'string',
            mapping: 'patient/name/middle'
        },
        {
            name: 'patientFamily',
            type: 'string',
            mapping: 'patient/name/family'
        },
        {
            name: 'patientSuffix',
            type: 'string',
            mapping: 'patient/name/suffix'
        },
        {
            name: 'patientTitle',
            type: 'string',
            mapping: 'patient/name/title'
        },
        {
            name: 'patientName',
            type: 'string',
            mapping: 'patient/name/displayName'
        },
        {
            name: 'request2507Identity',
            type: 'string',
            mapping: 'request2507/identity'
        },
        {
            name: 'request2507NamespaceId',
            type: 'string',
            mapping: 'request2507/namespaceId'
        },
        {
            name: 'request2507UniversalId',
            type: 'string',
            mapping: 'request2507/universalId'
        },
        {
            name: 'request2507UniversalIdType',
            type: 'string',
            mapping: 'request2507/universalIdType'
        },
        {
            name: 'examTypeCode',
            type: 'string',
            mapping: 'examType/code'
        },
        {
            name: 'examTypeText',
            type: 'string',
            mapping: 'examType/displayText'
        },
        {
            name: 'examTypeCodingSystem',
            type: 'string',
            mapping: 'examType/codingSystem'
        },
        {
            name: 'examType',
            type: 'string',
            mapping: 'examType/displayText'
        },
        {
            name: 'printName',
            type: 'string'
        },
        {
            name: 'statusCode',
            type: 'string',
            mapping: 'status/code'
        },
        {
            name: 'statusText',
            type: 'string',
            mapping: 'status/displayText'
        },
        {
            name: 'statusCodingSystem',
            type: 'string',
            mapping: 'status/codingSystem'
        },
        {
            name: 'status',
            type: 'string',
            mapping: 'status/displayText'
        },
        {
            name: 'workSheetPrintedCode',
            type: 'string',
            mapping: 'workSheetPrinted/code'
        },
        {
            name: 'workSheetPrintedText',
            type: 'string',
            mapping: 'workSheetPrinted/displayText'
        },
        {
            name: 'workSheetPrintedCodingSystem',
            type: 'string',
            mapping: 'workSheetPrinted/codingSystem'
        },
        {
            name: 'workSheetPrinted',
            type: 'string',
            mapping: 'workSheetPrinted/displayText'
        },
        {
            name: 'dateOfExam',
            type: 'date',
            mapping: 'dateOfExam/literal',
            dateFormat: 'Ymd'
        },
        {
            name: 'dateOfExam_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('dateOfExam'))) {
                    return Ext.Date.format(record.get('dateOfExam'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'examiningPhysicianIdentity',
            type: 'string',
            mapping: 'examiningPhysician/identifier/identity'
        },
        {
            name: 'examiningPhysicianAssigningFacility',
            type: 'string',
            mapping: 'examiningPhysician/identifier/assigningFacility'
        },
        {
            name: 'examiningPhysicianAssigningAuthority',
            type: 'string',
            mapping: 'examiningPhysician/identifier/assigningAuthority'
        },
        {
            name: 'examiningPhysicianIdSourceTable',
            type: 'string',
            mapping: 'examiningPhysician/idSourceTable'
        },
        {
            name: 'examiningPhysicianPrefix',
            type: 'string',
            mapping: 'examiningPhysician/name/prefix'
        },
        {
            name: 'examiningPhysicianGiven',
            type: 'string',
            mapping: 'examiningPhysician/name/given'
        },
        {
            name: 'examiningPhysicianMiddle',
            type: 'string',
            mapping: 'examiningPhysician/name/middle'
        },
        {
            name: 'examiningPhysicianFamily',
            type: 'string',
            mapping: 'examiningPhysician/name/family'
        },
        {
            name: 'examiningPhysicianSuffix',
            type: 'string',
            mapping: 'examiningPhysician/name/suffix'
        },
        {
            name: 'examiningPhysicianTitle',
            type: 'string',
            mapping: 'examiningPhysician/name/title'
        },
        {
            name: 'examiningPhysicianName',
            type: 'string',
            mapping: 'examiningPhysician'
        },
        {
            name: 'feeExamCode',
            type: 'string',
            mapping: 'feeExam/code'
        },
        {
            name: 'feeExamText',
            type: 'string',
            mapping: 'feeExam/displayText'
        },
        {
            name: 'feeExamCodingSystem',
            type: 'string',
            mapping: 'feeExam/codingSystem'
        },
        {
            name: 'feeExam',
            type: 'string',
            mapping: 'feeExam/displayText'
        },
        {
            name: 'examPlaceCode',
            type: 'string',
            mapping: 'examPlace/code'
        },
        {
            name: 'examPlaceText',
            type: 'string',
            mapping: 'examPlace/displayText'
        },
        {
            name: 'examPlaceCodingSystem',
            type: 'string',
            mapping: 'examPlace/codingSystem'
        },
        {
            name: 'examPlace',
            type: 'string',
            mapping: 'examPlace/displayText'
        },
        {
            name: 'insufficientReasonCode',
            type: 'string',
            mapping: 'insufficientReason/code'
        },
        {
            name: 'insufficientReasonText',
            type: 'string',
            mapping: 'insufficientReason/displayText'
        },
        {
            name: 'insufficientReasonCodingSystem',
            type: 'string',
            mapping: 'insufficientReason/codingSystem'
        },
        {
            name: 'insufficientReason',
            type: 'string',
            mapping: 'insufficientReason/displayText'
        },
        {
            name: 'originalProviderName',
            type: 'string',
            mapping: 'originalProvider'
        },
        {
            name: 'cancellationDateTime',
            type: 'date',
            mapping: 'cancellationDateTime/literal'
        },
        {
            name: 'cancelledByIdentity',
            type: 'string',
            mapping: 'cancelledBy/identifier/identity'
        },
        {
            name: 'cancelledByAssigningFacility',
            type: 'string',
            mapping: 'cancelledBy/identifier/assigningFacility'
        },
        {
            name: 'cancelledByAssigningAuthority',
            type: 'string',
            mapping: 'cancelledBy/identifier/assigningAuthority'
        },
        {
            name: 'cancelledByPrefix',
            type: 'string',
            mapping: 'cancelledBy/name/prefix'
        },
        {
            name: 'cancelledByGiven',
            type: 'string',
            mapping: 'cancelledBy/name/given'
        },
        {
            name: 'cancelledByMiddle',
            type: 'string',
            mapping: 'cancelledBy/name/middle'
        },
        {
            name: 'cancelledByFamily',
            type: 'string',
            mapping: 'cancelledBy/name/family'
        },
        {
            name: 'cancelledBySuffix',
            type: 'string',
            mapping: 'cancelledBy/name/suffix'
        },
        {
            name: 'cancelledByTitle',
            type: 'string',
            mapping: 'cancelledBy/name/title'
        },
        {
            name: 'cancelledByName',
            type: 'string',
            mapping: 'cancelledBy/name/displayName'
        },
        {
            name: 'cancellationReasonCode',
            type: 'string',
            mapping: 'cancellationReason/code'
        },
        {
            name: 'cancellationReasonText',
            type: 'string',
            mapping: 'cancellationReason/displayText'
        },
        {
            name: 'cancellationReasonCodingSystem',
            type: 'string',
            mapping: 'cancellationReason/codingSystem'
        },
        {
            name: 'cancellationReason',
            type: 'string',
            mapping: 'cancellationReason/displayText'
        },
        {
            name: 'dateTransferredOut',
            type: 'string',
            mapping: 'dateTransferredOut/literal'
        },
        {
            name: 'transferredOutByName',
            type: 'string',
            mapping: 'transferredOutBy'
        },
        {
            name: 'transferredOutToIdentity',
            type: 'string',
            mapping: 'transferredOutTo/identity'
        },
        {
            name: 'transferredOutToName',
            type: 'string',
            mapping: 'transferredOutTo/name'
        },
        {
            name: 'transferredOutToAssigningAuthority',
            type: 'string',
            mapping: 'transferredOutTo/assigningAuthority'
        },
        {
            name: 'dateTransferredIn',
            type: 'string',
            mapping: 'dateTransferredIn/literal'
        },
        {
            name: 'dateReturnedToOwnerSite',
            type: 'string',
            mapping: 'dateReturnedToOwnerSite/literal'
        },
        {
            name: 'insufficientRemarks',
            type: 'string'
        },
        {
            name: 'dateTranscriptionComplete',
            type: 'string',
            mapping: 'dateTranscriptionComplete/literal'
        },
        {
            name: 'recordSourceNamespace',
            type: 'string',
            mapping: 'recordSource/namespaceId'
        },
        {
            name: 'recordSourceUniversalId',
            type: 'string',
            mapping: 'recordSource/universalId'
        },
        {
            name: 'recordSourceUniversalIdType',
            type: 'string',
            mapping: 'recordSource/universalIdType'
        },
        {
            name: 'recordVersion',
            type: 'int'
        },
        {
            name: 'recordUpdateTime',
            type: 'string',
            mapping: 'recordUpdateTime/literal'
        }
    ],
    belongsTo: 'VIP.model.pathways.Patient',
    proxy: {
        type: 'memory',        
        reader: {
            type: 'xml',
            record: 'exam'
        }
    }
});