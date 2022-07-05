/**
* @author Josh Oliver
* @class VIP.model.appeals.RemandReason
*
* The model for appeal issue reamand reasons
*/
Ext.define('VIP.model.appeals.RemandReason', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'remandReasonCertifiedToBVAIndicator',
            type: 'string',
            mapping: 'RemandReasonCertifiedToBVAIndicator'
        },
        {
            name: 'remandIssueSequenceNumber',
            type: 'int',
            mapping: 'RemandIssueSequenceNumber'
        },
        {
            name: 'remandReasonCode',
            type: 'string',
            mapping: 'RemandReasonCode'
        },
        {
            name: 'remandReasonDescription',
            type: 'string',
            mapping: 'RemandReasonDescription'
        },
        {
            name: 'lastModifiedByCode',
            type: 'string',
            mapping: 'LastModifiedByCode'
        },
        {
            name: 'lastModifiedByName',
            type: 'string',
            mapping: 'LastModifiedByName'
        },
        {
            name: 'lastModifiedDate',
            type: 'date',
            mapping: 'LastModifiedDate',
            dateFormat: 'Y-m-d'
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'RemandReason'
        }
    }
});