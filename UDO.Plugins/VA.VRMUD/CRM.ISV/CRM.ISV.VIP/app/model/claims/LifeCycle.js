/**
* @author Josh Oliver
* @class VIP.model.claims.LifeCycle
*
* The model for claim life cycle
*/
Ext.define('VIP.model.claims.LifeCycle', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'claimId',
            type: 'string',
            mapping: 'benefitClaimID'
        },
        {
            name: 'actionFirstName',
            type: 'string'
        }, {
            name: 'actionLastName',
            type: 'string'
        }, {
            name: 'actionMiddleName',
            type: 'string'
        },
        {
            name: 'actionStationNumber',
            type: 'string',
            mapping: 'actionStationNumber'
        },
        {
            name: 'actionSuffix',
            type: 'string'
        },

        {
            name: 'caseAssignmentLocationId',
            type: 'string',
            mapping: 'caseAssignmentLocationID'
        },
        {
            name: 'caseAssignmentStatusNumber',
            type: 'string'
        },
        {
            name: 'caseId',
            type: 'string',
            mapping: 'caseID'
        },
        {
            name: 'changedDate',
            type: 'date',
            dateFormat: 'm/d/Y',
            mapping: 'changedDate'
        },
        {
            name: 'closedDate',
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
            name: 'lifeCycleStatusId',
            type: 'string',
            mapping: 'lifeCycleStatusID'
        },
        {
            name: 'lifeCycleStatusTypeName',
            type: 'string',
            mapping: 'lifeCycleStatusTypeName'
        },
        {
            name: 'reasonText',
            type: 'string',
            mapping: 'reasonText'
        },
        {
            name: 'stationOfJurisdiction',
            type: 'string',
            mapping: 'stationofJurisdiction'
        },
        {
            name: 'statusReasonTypeCode',
            type: 'string'
        },
        {
            name: 'statusReasonTypeName',
            type: 'string',
            mapping: 'statusReasonTypeName'
        },

        // ********** Starting computed fields
        {
            name: 'actionPerson',
            type: 'string',
            convert: function (v, record) {
                return (!Ext.isEmpty(record.get('actionLastName')) ? record.get('actionLastName') + ', ' : '') +
                    (!Ext.isEmpty(record.get('actionFirstName')) ? record.get('actionFirstName') + ' ' : '') +
                    (!Ext.isEmpty(record.get('actionMiddleName')) ? ' ' +record.get('actionMiddleName') : '');
            }
        }
    ],
    proxy: {
        type: 'memory',        
        reader: {
            type: 'xml',
            record: 'lifeCycleRecords'
        }
    }
});