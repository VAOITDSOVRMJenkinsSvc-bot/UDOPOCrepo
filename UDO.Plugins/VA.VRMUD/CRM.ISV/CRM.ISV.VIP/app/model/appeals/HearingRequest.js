/**
* @author Josh Oliver
* @class VIP.model.appeals.HearingRequest
*
* The model for appeal hearing requests
*/
Ext.define('VIP.model.appeals.HearingRequest', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'hearingRequestedTypeCode',
            type: 'string',
            mapping: 'HearingRequestedTypeCode'
        },
        {
            name: 'hearingRequestedTypeDescription',
            type: 'string',
            mapping: 'HearingRequestedTypeDescription'
        },
        {
            name: 'hearingRequestRequestedDate',
            type: 'date',
            mapping: 'HearingRequestRequestedDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'hearingRequestRequestedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('hearingRequestRequestedDate'))) {
                    return Ext.Date.format(record.get('hearingRequestRequestedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'hearingRequestScheduledDate',
            type: 'date',
            mapping: 'HearingRequestScheduledDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'hearingRequestScheduledDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('hearingRequestScheduledDate'))) {
                    return Ext.Date.format(record.get('hearingRequestScheduledDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'hearingRequestDispositionCode',
            type: 'string',
            mapping: 'HearingRequestDispositionCode'
        },
        {
            name: 'hearingRequestDispositionDescription',
            type: 'string',
            mapping: 'HearingRequestDispositionDescription'
        },
        {
            name: 'hearingRequestClosedDate',
            type: 'date',
            mapping: 'HearingRequestClosedDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'hearingRequestClosedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('hearingRequestClosedDate'))) {
                    return Ext.Date.format(record.get('hearingRequestClosedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'boardMemberId',
            type: 'string',
            mapping: 'BoardMemberId'
        },
        {
            name: 'decisionTeamCode',
            type: 'string',
            mapping: 'DecisionTeamCode'
        },
        {
            name: 'hearingActionDescription',
            type: 'string',
            mapping: 'HearingActionDescription'
        }               
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'HearingRequest'
        }
    }
});