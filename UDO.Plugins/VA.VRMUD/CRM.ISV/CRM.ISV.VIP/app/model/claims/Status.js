/**
* @author Josh Oliver
* @class VIP.model.claims.Status
*
* The model for claim evidence
*/
Ext.define('VIP.model.claims.Status', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mapd.claimmanagement.FindClaimStatus'
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
            name: 'rowId',
            type: 'string'
        },
        {
            name: 'actionLocationId',
            type: 'string',
            mapping: 'actionLctnId'
        },
        {
            name: 'actionParticipantId',
            type: 'string',
            mapping: 'actionPtcpntId'
        },
        {
            name: 'claimLocationStatusTypeName',
            type: 'string',
            mapping: 'bnftClmLcSttTn'
        },
        {
            name: 'changedDate',
            type: 'date',
            dateFormat: 'c',
            mapping: 'chngdDt'
        },
        {
            name: 'claimId',
            type: 'string',
            mapping: 'clmId',
            ignoreMappingOnRequest: true
        },
        {
            name: 'locationId',
            type: 'string',
            mapping: 'lctnId'
        },
        {
            name: 'sn',
            type: 'string'
        },
        {
            name: 'userName',
            type: 'string'
        },
        {
            name: 'daysInStatus'           
        }
    ],
    belongsTo: 'VIP.model.claims.ClaimDetail',
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'claimLifecycleStatusList'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.mapd.claimmanagement.FindClaimStatus',
            update: '',
            destroy: ''
        }
    }
});