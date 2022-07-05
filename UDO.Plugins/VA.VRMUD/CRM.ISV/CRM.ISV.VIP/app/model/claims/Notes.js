/**
* @author Josh Oliver
* @class VIP.model.claims.Notes
*
* The model for claim notes
*/
Ext.define('VIP.model.claims.Notes', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mapd.developmentnotes.FindDevelopmentNotes',
        'VIP.soap.envelopes.mapd.developmentnotes.CreateNote',
        'VIP.soap.envelopes.mapd.developmentnotes.UpdateNote'
    ],
    fields: [
        {
            name: 'callId',
            type: 'string'
        },
        {
            name: 'journalDate',
            type: 'date',
            mapping: 'jrnDt',
            dateFormat: 'c',
            ignoreMappingOnRequest: true
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
            name: 'benefitClaimNoteTypeCode',
            type: 'string',
            mapping: 'bnftClmNoteTc'
        },
        {
            name: 'claimId',
            type: 'string',
            mapping: 'clmId',
            ignoreMappingOnRequest: true
        },
        {
            name: 'createDate',
            type: 'date',
            mapping: 'createDt',
            dateFormat: 'c',
            ignoreMappingOnRequest: true
        },
        {
            name: 'modifiedDate',
            type: 'string',
            mapping: 'modifdDt',
            ignoreMappingOnRequest: true
        },
        {
            name: 'noteId',
            type: 'string',
            ignoreMappingOnRequest: true
        },
        {
            name: 'noteOutTypeName',
            type: 'string',
            mapping: 'noteOutTn',
            ignoreMappingOnRequest: true
        },
        {
            name: 'participantId',
            type: 'string',
            mapping: 'ptcpntId',
            ignoreMappingOnRequest: true
        },
        {
            name: 'text',
            type: 'string',
            mapping: 'txt',
            ignoreMappingOnRequest: true
        },
        {
            name: 'suspenseDate',
            type: 'date',
            mapping: 'suspnsDt',
            dateFormat: 'c',
            ignoreMappingOnRequest: true
        },
        {
            name: 'userId',
            type: 'string',
            ignoreMappingOnRequest: true
        },
        {
            name: 'userName',
            type: 'string',
            mapping: 'userNm'
        },

        // ********** Starting computed fields
        {
            name: 'fullUserID',
            type: 'string',
            convert: function (v, record) {
                var usernm = record.get('userName') ? record.get('userName') : '';
                var jrnUserId = record.get('journalUserId') ? record.get('journalUserId') : '';
                if (usernm != '') { return usernm + ' (' + jrnUserId + ')'; }

                return jrnUserId;
            }
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
            record: 'notes'
        },
        envelopes: {
            create: 'VIP.soap.envelopes.mapd.developmentnotes.CreateNote',
            read: 'VIP.soap.envelopes.mapd.developmentnotes.FindDevelopmentNotes',
            update: 'VIP.soap.envelopes.mapd.developmentnotes.UpdateNote',
            destroy: ''
        }
    }
});