/**
* @author Josh Oliver
* @class VIP.model.claims.TrackedItem
*
* The model for claim tracked items
*/
Ext.define('VIP.model.claims.TrackedItems', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mapd.trackeditem.FindTrackedItems',
        'VIP.model.claims.Letters'
    ],
    fields: [
        {
            name: 'acceptDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'acceptDt'
        },
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
            name: 'claimId',
            type: 'string',
            mapping: 'claimId',
            ignoreMappingOnRequest: true
        },
        {
            name: 'developmentActionId',
            type: 'string',
            mapping: 'devactnId'
        },
        {
            name: 'documentId',
            type: 'string',
            mapping: 'docid'
        },
        {
            name: 'trackedItemId',
            type: 'string',
            mapping: 'dvlpmtItemId'
        },
        {
            name: 'developmentTypeCode',
            type: 'string',
            mapping: 'dvlpmtTc'
        },
        {
            name: 'followupDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'followDt'
        },
        {
            name: 'inErrorDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'inErrorDt'
        },
        {
            name: 'receiveDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'receiveDt'
        },
        {
            name: 'requestDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'reqDt'
        },
        {
            name: 'secondFollowUpDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'scndFlowDate'
        },
        {
            name: 'shortName',
            type: 'string',
            mapping: 'shortNm'
        },

        {
            name: 'standardDevelopmentActionId',
            type: 'string',
            mapping: 'stdDevactnId'
        },
        {
            name: 'suspenseDate',
            dateFormat: 'c', //ISO 8601 date
            type: 'date',
            mapping: 'suspnsDt'
        },
        {
            name: 'recipient',
            type: 'string'
        }
    ],
    hasMany: [
        {
            model: 'VIP.model.claims.Letters',
            name: 'letters',
            associationKey: 'letter',
            storeConfig: {
                filters: []
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
            record: 'dvlpmtItems'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.mapd.trackeditem.FindTrackedItems',
            update: '',
            destroy: ''
        }
    }
});