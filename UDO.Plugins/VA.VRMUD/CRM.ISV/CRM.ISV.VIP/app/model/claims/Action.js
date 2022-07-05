/**
* @author Josh Oliver
* @class VIP.model.claims.Action
*
* The model for claim actions
*/
Ext.define('VIP.model.claims.Action', {
    extend: 'Ext.data.Model',
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
            name: 'createDate',
            type: 'string',
            mapping: 'createDt'
        },
        {
            name: 'actionId',
            type: 'string',
            mapping: 'devactnId'
        },
        {
            name: 'documentId',
            type: 'string',
            mapping: 'docid'
        },
        {
            name: 'federalAgencyIndicator',
            type: 'string',
            mapping: 'fedAgencyInd'
        },
        {
            name: 'headingId',
            type: 'string',
            mapping: 'headngId'
        },
        {
            name: 'name',
            type: 'string',
            mapping: 'nm'
        },
        {
            name: 'programTypeCode',
            type: 'string',
            mapping: 'pgmTc'
        },
        {
            name: 'paragraphId',
            type: 'string',
            mapping: 'prgrphId'
        },
        {
            name: 'rulesBasedIndicator',
            type: 'string',
            mapping: 'rulesBasedInd'
        },
        {
            name: 'standardDevelopmentActionCode',
            type: 'string',
            mapping: 'stdDevactnCd'
        },
        {
            name: 'standardDevelopmentActionId',
            type: 'string',
            mapping: 'stdDevactnId'
        },
        {
            name: 'standardActionDescription',
            type: 'string',
            mapping: 'stdactnDescp'
        },
        {
            name: 'suspenseDuration',
            type: 'string',
            mapping: 'suspnsDuratn'
        },
        {
            name: 'suspenseDays',
            type: 'string',
            mapping: 'suspnsDys'
        },
        {
            name: 'suspenseUnit',
            type: 'string',
            mapping: 'suspnsUnit'
        },
        {
            name: 'text',
            type: 'string',
            mapping: 'txt'
        }
    ]
});