/**
* @author Josh Oliver
* @class VIP.model.claims.SpecialIssues
*
* The model for claim contention special issues
*/
Ext.define('VIP.model.claims.SpecialIssues', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'callId',
            type: 'string'
        }, {
            name: 'journalDate',
            type: 'string',
            mapping: 'jrnDt'
        },
        {
            name: 'claimReceivedDate',
            type: 'string',
            mapping: 'claimRcvdDt'
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
            name: 'parentId',
            type: 'string'
        },
        {
            name: 'parentName',
            type: 'string'
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
            name: 'contentionId',
            type: 'string',
            mapping: 'cntntnId'
        },
        {
            name: 'specialIssueId',
            type: 'string',
            mapping: 'cntntnSpisId'
        },
        {
            name: 'specialIssueTypeCode',
            type: 'string',
            mapping: 'spisTc'
        },
        {
            name: 'specialIssueTypeName',
            type: 'string',
            mapping: 'spisTn'
        }
    ],
    belongsTo: 'VIP.model.claims.Contentions',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'specialIssues'
        }
    }
});