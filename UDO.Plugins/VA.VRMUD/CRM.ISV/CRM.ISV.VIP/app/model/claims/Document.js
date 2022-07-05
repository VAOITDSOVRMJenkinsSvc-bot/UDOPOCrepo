/**
* @author Josh Oliver
* @class VIP.model.claims.Document
*
* The model for claim documents
*/
Ext.define('VIP.model.claims.Document', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mapd.document.FindClaimantLetters'
    ],
    fields: [
        {
            name: 'callId',
            type: 'string'
        },
        {
            name: 'journalDate',
            type: 'string',
            mapping: 'jrnDt'
        },
        {
            name: 'journalLocationId',
            type: 'string',
            mapping: 'jrnLctnId'
        },
        {
            name: 'journalObjectId',
            type: 'string',
            mapping: 'jrnObjId'
        },
        {
            name: 'journalStatusTypeCode',
            type: 'string',
            mapping: 'jrnSttTc'
        },
        {
            name: 'journalUserId',
            type: 'string',
            mapping: 'jrnUserId'
        },
        {
            name: 'rowCount',
            type: 'string',
            mapping: 'rowCnt'
        },
        {
            name: 'rowId',
            type: 'string'
        },
        {
            name: 'claimId',
            type: 'string',
            mapping: 'clmId'
        },
        {
            name: 'documentDate',
            type: 'string',
            mapping: 'dcmntDt'
        },
        {
            name: 'documentId',
            type: 'string',
            mapping: 'docid',
            ignoreMappingOnRequest: true
        },
        {
            name: 'developmentTypeCode',
            type: 'string',
            mapping: 'dvlpmtTc'
        },
        {
            name: 'establishedDate',
            type: 'string',
            mapping: 'estabdDt'
        },
        {
            name: 'fileStatusDate',
            type: 'string',
            mapping: 'fileSttDt'
        },
        {
            name: 'fileStatusTypeCode',
            type: 'string',
            mapping: 'fileSttTc'
        },
        {
            name: 'includeEnclosureIndicator',
            type: 'string',
            mapping: 'incldEnclsrInd'
        },
        {
            name: 'letterTemplateId',
            type: 'string',
            mapping: 'lttrTmplatId'
        },
        {
            name: 'letterText',
            type: 'string',
            mapping: 'lttrTxt'
        },
        {
            name: 'name',
            type: 'string',
            mapping: 'nm'
        },
        {
            name: 'outDocumentTypeCode',
            type: 'string',
            mapping: 'outdcmtTc'
        },
        {
            name: 'printDate',
            type: 'string',
            mapping: 'printDt'
        },
        {
            name: 'participantDocumentTypeName',
            type: 'string',
            mapping: 'ptcpntDcmntTn'
        },
        {
            name: 'participantId',
            type: 'string',
            mapping: 'ptcpntId'
        },
        {
            name: 'templateTypeCode',
            type: 'string',
            mapping: 'templatTc'
        }
    ]
});