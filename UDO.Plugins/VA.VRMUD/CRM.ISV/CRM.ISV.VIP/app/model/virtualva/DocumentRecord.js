/**
* @author Josh Oliver
* @class VIP.model.virtualva.DocumentRecord
*
* The model for Virtual VA document record
*/
Ext.define('VIP.model.virtualva.DocumentRecord', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.virtualva.GetDocumentList'
    ],
    fields: [
        {
            name: 'documentRecordId',
            mapping: 'dcmntRecordId'
        },
        {
            name: 'authorNumber',
            mapping: 'authorNm'
        },
        {
            name: 'authorRegionalOfficeNumber',
            mapping: 'authorRoNbr'
        },
        {
            name: 'batchNumber',
            mapping: 'batchNm'
        },
        {
            name: 'categoryListText',
            mapping: 'categyListTxt'
        },
        {
            name: 'contactNumber',
            mapping: 'cntctNm'
        },
        {
            name: 'documentDate',
            mapping: 'dcmntDt',
            type: 'date',
            dateFormat: 'm/d/Y'
        },
        {
            name: 'documentFormatCode',
            mapping: 'dcmntFormatCd'
        },
        {
            name: 'documentSizeNumber',
            mapping: 'dcmntSizeNbr'
        },
        {
            name: 'documentTypeLupId',
            mapping: 'dcmntTypeLupId'
        },
        {
            name: 'electronicFolderId',
            mapping: 'efoldrId'
        },
        {
            name: 'ivmYearNumber',
            mapping: 'ivmYearNbr'
        },
        {
            name: 'lastUserModifiedNumber',
            mapping: 'lastUserModifdNm'
        },
        {
            name: 'receivedDate',
            mapping: 'rcvdDt',
            type: 'date',
            dateFormat: 'm/d/Y'
        },
        {
            name: 'readIndicator',
            mapping: 'readInd'
        },
        {
            name: 'recordsManagementNumber',
            mapping: 'recdsMgmntNbr'
        },
        {
            name: 'regionalOfficeLupId',
            mapping: 'roLupId'
        },
        {
            name: 'restrictedDocumentIndicator',
            mapping: 'rstrcdDcmntInd'
        },
        {
            name: 'restrictedReasonLupId',
            mapping: 'rstrcdReasonLupId'
        },
        {
            name: 'sourceCommentText',
            mapping: 'sourceComntTxt'
        },
        {
            name: 'sourceText',
            mapping: 'sourceTxt'
        },
        {
            name: 'storageDate',
            mapping: 'storgDt',
            type: 'date',
            dateFormat: 'm/d/Y'
        },
        {
            name: 'subjectText',
            mapping: 'subjctTxt'
        },
        {
            name: 'treatmentBeginDate',
            mapping: 'trtmntBeginDt',
            type: 'date',
            dateFormat: 'm/d/Y'
        },
        {
            name: 'treatmentConditionText',
            mapping: 'trtmntCndtnTxt'
        },
        {
            name: 'treatmentEndDate',
            mapping: 'trtmntEndDt',
            type: 'date',
            dateFormat: 'm/d/Y'
        },
        {
            name: 'writeOutTypeText',
            mapping: 'writeOutTypeTxt'
        },
        {
            name: 'documentTypeDescriptionText',
            mapping: 'dcmntTypeDescpTxt'
        },
        {
            name: 'fileNetDocumentId',
            mapping: 'fnDcmntId'
        },
        {
            name: 'fileNetDocumentSource',
            mapping: 'fnDcmntSource'
        },
        {
            name: 'claimId',
            mapping: 'claimNbr'
        },
        {
            name: 'ssn',
            mapping: 'ssnNbr'
        },
        {
            name: 'veteranFirstName',
            mapping: 'vetFirstName'
        },
        {
            name: 'veteranMiddleName',
            mapping: 'vetMiddleNm'
        },
        {
            name: 'veteranLastName',
            mapping: 'vetLastName'
        },
        {
            name: 'veteranBirthDate',
            mapping: 'vetBirthDt',
            type: 'date',
            dateFormat: 'm/d/Y'
        },
        {
            name: 'jro',
            mapping: 'jrsdtnRoNbr'
        }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset:utf-8"
        },
        reader: {
            type: 'xml',
            record: 'dcmntRecord'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.virtualva.GetDocumentList',
            update: '',
            destroy: ''
        }
    }
});