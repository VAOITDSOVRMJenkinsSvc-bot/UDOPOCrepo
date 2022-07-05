/**
* @author Josh Oliver
* @class VIP.model.claims.Evidence
*
* The model for claim evidence
*/
Ext.define('VIP.model.claims.Evidence', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mapd.trackeditem.FindUnsolEvdnce'
    ],
    fields: [
        {
            name: 'callId',
            type: 'string'
        },
        {
            name: 'journalDate',
            type: 'date',
            dateFormat: 'c',
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
            name: 'rowId',
            type: 'string'
        },
        {
            name: 'descriptionText',
            type: 'string',
            mapping: 'descTxt'
        },
        {
            name: 'incomingDocumentId',
            type: 'string',
            mapping: 'incmngDcmntId'
        },
        {
            name: 'claimId',
            type: 'string',
            mapping: 'ptcpntClmntId'
        },
        {
            name: 'receivedDate',
            type: 'date',
            dateFormat: 'c',
            mapping: 'rcvdDt'
        },
        {
            name: 'userName',
            type: 'string',
            mapping: 'userName'
        }
    ],    
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'UnsolicitedEvidence'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.mapd.trackeditem.FindUnsolEvdnce',
            update: '',
            destroy: ''
        }
    }
});