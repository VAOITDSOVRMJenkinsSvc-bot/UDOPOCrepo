/**
* @author Josh Oliver
* @class VIP.model.pathways.ExamRequest
*
* The model for appointments
*/
Ext.define('VIP.model.pathways.ExamRequest', {
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
            name: 'requestDate',
            type: 'date',
            mapping: 'requestDate/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'requestDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('requestDate'))) {
                    return Ext.Date.format(record.get('requestDate'), 'm/d/Y h:i a');
                } else { return ''; }
            }
        },
        {
            name: 'roIdentity',
            type: 'string',
            mapping: 'regionalOfficeNumber/identifier/identity'
        },
        {
            name: 'roAssigningAuthority',
            type: 'string',
            mapping: 'regionalOfficeNumber/identifier/assigningAuthority'
        },
        {
            name: 'roName',
            type: 'string',
            mapping: 'regionalOfficeNumber/identifier/name'
        },
        {
            name: 'roShortName',
            type: 'string',
            mapping: 'regionalOfficeNumber/shortName'
        },
        {
            name: 'roStationNumber',
            type: 'string',
            mapping: 'regionalOfficeNumber/stationNumber'
        },
        {
            name: 'roOfficialVAName',
            type: 'string',
            mapping: 'regionalOfficeNumber/officialVAName'
        },
        {
            name: 'requestorIdentity',
            type: 'string',
            mapping: 'requestor/identifier/identity'
        },
        {
            name: 'requestorAssigningFacility',
            type: 'string',
            mapping: 'requestor/identifier/assigningFacility'
        },
        {
            name: 'requestorAssigningAuthority',
            type: 'string',
            mapping: 'requestor/identifier/assigningAuthority'
        },
        {
            name: 'requestorIdSourceTable',
            type: 'string',
            mapping: 'requestor/idSourceTable'
        },
        {
            name: 'requestorPrefix',
            type: 'string',
            mapping: 'requestor/name/prefix'
        },
        {
            name: 'requestorGiven',
            type: 'string',
            mapping: 'requestor/name/given'
        },
        {
            name: 'requestorMiddle',
            type: 'string',
            mapping: 'requestor/name/middle'
        },
        {
            name: 'requestorFamily',
            type: 'string',
            mapping: 'requestor/name/family'
        },
        {
            name: 'requestorSuffix',
            type: 'string',
            mapping: 'requestor/name/suffix'
        },
        {
            name: 'requestorTitle',
            type: 'string',
            mapping: 'requestor/name/title'
        },
        {
            name: 'requestorName',
            type: 'string',
            mapping: 'requestor/name/displayName'
        },
        {
            name: 'dateReportedToMas',
            type: 'date',
            mapping: 'dateReportedToMas/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'dateSchedulingCompleted',
            type: 'date',
            mapping: 'dateSchedulingCompleted/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'dateSchedulingCompleted_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('dateSchedulingCompleted'))) {
                    return Ext.Date.format(record.get('dateSchedulingCompleted'), 'm/d/Y h:i a');
                } else { return ''; }
            }
        },
        {
            name: 'dateCompleted',
            type: 'date',
            mapping: 'dateCompleted/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'priorityOfExamCode',
            type: 'string',
            mapping: 'priorityOfExam/code'
        },
        {
            name: 'priorityOfExamText',
            type: 'string',
            mapping: 'priorityOfExam/displayText'
        },
        {
            name: 'priorityOfExamCodingSystem',
            type: 'string',
            mapping: 'priorityOfExam/codingSystem'
        },
        {
            name: 'priorityOfExam',
            type: 'string',
            mapping: 'priorityOfExam/displayText'
        },
        {
            name: 'otherDisabilitiesLine1',
            type: 'string'
        },
        {
            name: 'otherDisabilitiesLine2',
            type: 'string'
        },
        {
            name: 'otherDisabilitiesLine3',
            type: 'string'
        },
        {
            name: 'transcriptionDate',
            type: 'date',
            mapping: 'transcriptionDate/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'dateApproved',
            type: 'date',
            mapping: 'dateApproved/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'approvedBy',
            type: 'string',
            mapping: 'approvedBy'
        },
        {
            name: 'dateReleased',
            type: 'date',
            mapping: 'dateReleased/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'releasedByIdentity',
            type: 'string',
            mapping: 'releasedBy/identifier/identity'
        },
        {
            name: 'releasedByAssigningFacility',
            type: 'string',
            mapping: 'releasedBy/identifier/assigningFacility'
        },
        {
            name: 'releasedByAssigningAuthority',
            type: 'string',
            mapping: 'releasedBy/identifier/assigningAuthority'
        },
        {
            name: 'releasedByIdSourceTable',
            type: 'string',
            mapping: 'releasedBy/idSourceTable'
        },
        {
            name: 'releasedByName',
            type: 'string',
            mapping: 'releasedBy/name/displayName'
        },
        {
            name: 'datePrintedByRO',
            type: 'date',
            mapping: 'datePrintedByRO/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'printedByIdentity',
            type: 'string',
            mapping: 'printedBy/identifier/identity'
        },
        {
            name: 'printedByAssigningFacility',
            type: 'string',
            mapping: 'printedBy/identifier/assigningFacility'
        },
        {
            name: 'printedByAssigningAuthority',
            type: 'string',
            mapping: 'printedBy/identifier/assigningAuthority'
        },
        {
            name: 'printedByIdSourceTable',
            type: 'string',
            mapping: 'printedBy/idSourceTable'
        },
        {
            name: 'printedByPrefix',
            type: 'string',
            mapping: 'printedBy/name/prefix'
        },
        {
            name: 'printedByGiven',
            type: 'string',
            mapping: 'printedBy/name/given'
        },
        {
            name: 'printedByMiddle',
            type: 'string',
            mapping: 'printedBy/name/middle'
        },
        {
            name: 'printedByFamily',
            type: 'string',
            mapping: 'printedBy/name/family'
        },
        {
            name: 'printedBySuffix',
            type: 'string',
            mapping: 'printedBy/name/suffix'
        },
        {
            name: 'printedByTitle',
            type: 'string',
            mapping: 'printedBy/name/title'
        },
        {
            name: 'printedByName',
            type: 'string',
            mapping: 'printedBy/name/displayName'
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
            name: 'elapsedTime',
            type: 'string'
        },
        {
            name: 'cancellationDate',
            type: 'date',
            mapping: 'cancellationDate/literal',
            dateFormat: 'YmdHis'
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
            mapping: 'cancelledBy/prefix'
        },
        {
            name: 'cancelledByGiven',
            type: 'string',
            mapping: 'cancelledBy/given'
        },
        {
            name: 'cancelledByMiddle',
            type: 'string',
            mapping: 'cancelledBy/middle'
        },
        {
            name: 'cancelledByFamily',
            type: 'string',
            mapping: 'cancelledBy/family'
        },
        {
            name: 'cancelledBySuffix',
            type: 'string',
            mapping: 'cancelledBy/suffix'
        },
        {
            name: 'cancelledByTitle',
            type: 'string',
            mapping: 'cancelledBy/title'
        },
        {
            name: 'cancelledByName',
            type: 'string',
            mapping: 'cancelledBy/name/displayName'
        },
        {
            name: 'claimFolderRequiredCode',
            type: 'string',
            mapping: 'claimFolderRequired/code'
        },
        {
            name: 'claimFolderRequiredText',
            type: 'string',
            mapping: 'claimFolderRequired/displayText'
        },
        {
            name: 'claimFolderRequiredCodingSystem',
            type: 'string',
            mapping: 'claimFolderRequired/codingSystem'
        },
        {
            name: 'claimFolderRequired',
            type: 'string',
            mapping: 'claimFolderRequired/displayText'
        },
        {
            name: 'otherDocumentsRequiredCode',
            type: 'string',
            mapping: 'otherDocumentsRequired/code'
        },
        {
            name: 'otherDocumentsRequiredText',
            type: 'string',
            mapping: 'otherDocumentsRequired/displayText'
        },
        {
            name: 'otherDocumentsRequiredCodingSystem',
            type: 'string',
            mapping: 'otherDocumentsRequired/codingSystem'
        },
        {
            name: 'otherDocumentsRequired',
            type: 'string',
            mapping: 'otherDocumentsRequired/displayText'
        },
        {
            name: 'remarks',
            type: 'string'
        },
        {
            name: 'lastExamAddDate',
            type: 'date',
            mapping: 'lastExamAddDate/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'lastPersonToAddExamIdentity',
            type: 'string',
            mapping: 'lastPersonToAddExam/identifier/identity'
        },
        {
            name: 'lastPersonToAddExamAssigningFacility',
            type: 'string',
            mapping: 'lastPersonToAddExam/identifier/assigningFacility'
        },
        {
            name: 'lastPersonToAddExamAssigningAuthority',
            type: 'string',
            mapping: 'lastPersonToAddExam/identifier/assigningAuthority'
        },
        {
            name: 'lastPersonToAddExamIdSourceTable',
            type: 'string',
            mapping: 'lastPersonToAddExam/idSourceTable'
        },
        {
            name: 'lastPersonToAddExamPrefix',
            type: 'string',
            mapping: 'lastPersonToAddExam/name/prefix'
        },
        {
            name: 'lastPersonToAddExamGiven',
            type: 'string',
            mapping: 'lastPersonToAddExam/name/given'
        },
        {
            name: 'lastPersonToAddExamMiddle',
            type: 'string',
            mapping: 'lastPersonToAddExam/name/middle'
        },
        {
            name: 'lastPersonToAddExamFamily',
            type: 'string',
            mapping: 'lastPersonToAddExam/name/family'
        },
        {
            name: 'lastPersonToAddExamSuffix',
            type: 'string',
            mapping: 'lastPersonToAddExam/name/suffix'
        },
        {
            name: 'lastPersonToAddExamTitle',
            type: 'string',
            mapping: 'lastPersonToAddExam/name/title'
        },
        {
            name: 'lastPersonToAddExamName',
            type: 'string',
            mapping: 'lastPersonToAddExam/name/displayName'
        },
        {
            name: 'remarksModificationDate',
            type: 'date',
            mapping: 'remarksModificationDate/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'remarksModifiedByIdentity',
            type: 'string',
            mapping: 'lastPersonToAddExam/identifier/identity'
        },
        {
            name: 'remarksModifiedByAssigningFacility',
            type: 'string',
            mapping: 'remarksModifiedBy/identifier/assigningFacility'
        },
        {
            name: 'remarksModifiedByAssigningAuthority',
            type: 'string',
            mapping: 'remarksModifiedBy/identifier/assigningAuthority'
        },
        {
            name: 'remarksModifiedByIdSourceTable',
            type: 'string',
            mapping: 'remarksModifiedBy/idSourceTable'
        },
        {
            name: 'remarksModifiedByPrefix',
            type: 'string',
            mapping: 'remarksModifiedBy/name/prefix'
        },
        {
            name: 'remarksModifiedByGiven',
            type: 'string',
            mapping: 'remarksModifiedBy/name/given'
        },
        {
            name: 'remarksModifiedByMiddle',
            type: 'string',
            mapping: 'remarksModifiedBy/name/middle'
        },
        {
            name: 'remarksModifiedByFamily',
            type: 'string',
            mapping: 'remarksModifiedBy/name/family'
        },
        {
            name: 'remarksModifiedBySuffix',
            type: 'string',
            mapping: 'remarksModifiedBy/name/suffix'
        },
        {
            name: 'remarksModifiedByTitle',
            type: 'string',
            mapping: 'remarksModifiedBy/name/title'
        },
        {
            name: 'remarksModifiedByName',
            type: 'string',
            mapping: 'remarksModifiedBy/name/displayName'
        },
        {
            name: 'routingLocationIdentity',
            type: 'string',
            mapping: 'routingLocation/identity'
        },
        {
            name: 'routingLocationName',
            type: 'string',
            mapping: 'routingLocation/name'
        },
        {
            name: 'routingLocationAssigningAuthority',
            type: 'string',
            mapping: 'routingLocation/assigningAuthority'
        },
        {
            name: 'approvalDateTime',
            type: 'date',
            mapping: 'approvalDateTime/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'returnedToOwnerSiteByName',
            type: 'string',
            mapping: 'returnedToOwnerSiteBy'
        },
        {
            name: 'ownerDomainCode',
            type: 'string',
            mapping: 'ownerDomain/code'
        },
        {
            name: 'ownerDomainText',
            type: 'string',
            mapping: 'ownerDomain/displayText'
        },
        {
            name: 'ownerDomainCodingSystem',
            type: 'string',
            mapping: 'ownerDomain/codingSystem'
        },
        {
            name: 'ownerDomain',
            type: 'string',
            mapping: 'ownerDomain/displayText'
        },
        {
            name: 'lastRatingExamDate',
            type: 'date',
            mapping: 'lastRatingExamDate/literal',
            dateFormat: 'Ymd'
        },
        {
            name: 'lastRatingExamDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('lastRatingExamDate'))) {
                    return Ext.Date.format(record.get('lastRatingExamDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'originalRequestPointer',
            type: 'string'
        },
        {
            name: 'transferredToAnotherSiteCode',
            type: 'string',
            mapping: 'transferredToAnotherSite/code'
        },
        {
            name: 'transferredToAnotherSiteText',
            type: 'string',
            mapping: 'transferredToAnotherSite/displayText'
        },
        {
            name: 'transferredToAnotherSiteCodingSystem',
            type: 'string',
            mapping: 'transferredToAnotherSite/codingSystem'
        },
        {
            name: 'transferredToAnotherSite',
            type: 'string',
            mapping: 'transferredToAnotherSite/displayText'
        },
        {
            name: 'dateTransferredToOtherSite',
            type: 'date',
            mapping: 'dateTransferredToOtherSite/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'dateTransferredInFromRemoteSite',
            type: 'date',
            mapping: 'dateTransferredInFromRemoteSite/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'dateAllTransfersReturned',
            type: 'date',
            mapping: 'dateAllTransfersReturned/literal',
            dateFormat: 'YmdHis'
        },
        {
            name: 'original2507RequestIdentity',
            type: 'string',
            mapping: 'original2507Request/identity'
        },
        {
            name: 'original2507RequestNamespace',
            type: 'string',
            mapping: 'original2507Request/namespaceId'
        },
        {
            name: 'original2507RequestUniversalId',
            type: 'string',
            mapping: 'original2507Request/universalId'
        },
        {
            name: 'original2507RequestUniversalIdType',
            type: 'string',
            mapping: 'original2507Request/universalIdType'
        },
        {
            name: 'original2507ProcessingTime',
            type: 'string'
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
            type: 'date',
            mapping: 'recordUpdateTime/literal',
            dateFormat: 'YmdHis'
        }
    ],
    belongsTo: 'VIP.model.pathways.Patient',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'examRequest'
        }
    }
});