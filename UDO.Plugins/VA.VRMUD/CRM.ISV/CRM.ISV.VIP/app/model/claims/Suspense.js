/**
* @author Josh Oliver
* @class VIP.model.claims.Suspense
*
* The model for claim suspenses
*/
Ext.define('VIP.model.claims.Suspense', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'suspenseDate',
            type: 'string',
            mapping: 'claimSuspenceDate'
        },
        {
            name: 'firstName',
            type: 'string'
        },
        {
            name: 'journalDate',
            type: 'string'
        },
        {
            name: 'journalObjectId',
            type: 'string',
            mapping: 'journalObjectID'
        },
        {
            name: 'journalStation',
            type: 'string'
        },
        {
            name: 'journalStatusTypeCode',
            type: 'string'
        },
        {
            name: 'journalUserId',
            type: 'string',
            mapping: 'journalUserID'
        },
        {
            name: 'lastName',
            type: 'string'
        },
        {
            name: 'middleName',
            type: 'string'
        },
        {
            name: 'suffix',
            type: 'string'
        },
        {
            name: 'suspenseActionDate',
            type: 'string',
            mapping: 'suspenceActionDate'
        },
        {
            name: 'suspenseCode',
            type: 'string',
            mapping: 'suspenceCode'
        },
        {
            name: 'suspenseReasonText',
            type: 'string',
            mapping: 'suspenceReasonText'
        },
        {
            name: 'claimId',
            type: 'string',
            mapping: 'benefitClaimID'
        },

    // ********** Starting computed fields
        {
            name: 'updatedBy',
            type: 'string',
            convert: function (v, record) {
                var updatedByValue = (!Ext.isEmpty(record.get('firstName')) ? record.get('firstName') + ' ' : '') +
                    (!Ext.isEmpty(record.get('lastName')) ? record.get('lastName') + ' ' : '') +
                    (!Ext.isEmpty(record.get('journalDate')) ? record.get('journalDate') + ' ' : '') +
                    (!Ext.isEmpty(record.get('journalUserID')) ? record.get('journalUserID') + ' ' : '') +
                    (!Ext.isEmpty(record.get('journalStation')) ? record.get('journalStation') + ' ' : '') +
                    (!Ext.isEmpty(record.get('journalObjectID')) ? record.get('journalObjectID') + ' ' : '');

                return updatedByValue;

            }
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'suspenceRecords'
        }
    }
});