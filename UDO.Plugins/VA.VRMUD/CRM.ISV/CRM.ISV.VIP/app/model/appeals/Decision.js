/**
* @author Josh Oliver
* @class VIP.model.appeals.Decision
*
* The model for appeal decisions
*/
Ext.define('VIP.model.appeals.Decision', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'decisionDate',
            type: 'date',
            mapping: 'DecisionDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'decisionDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('decisionDate'))) {
                    return Ext.Date.format(record.get('decisionDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'decisionDispositionCode',
            type: 'string',
            mapping: 'DecisionDispositionCode'
        },
        {
            name: 'decisionDispositionDescription',
            type: 'string',
            mapping: 'DecisionDispositionDescription'
        },
        {
            name: 'decisionCitationNumber',
            type: 'string',
            mapping: 'DecisionCitationNumber'
        },
        {
            name: 'decisionReviewOfficerDecisionIndicator',
            type: 'string',
            mapping: 'DecisionReviewOfficerDecisionIndicator'
        },
        {
            name: 'decisionReviewOfficerInformalHearingHeldIndicator',
            type: 'string',
            mapping: 'DecisionReviewOfficerInformalHearingHeldIndicator'
        },
        {
            name: 'decisionReviewOfficerFormalHearingHeldIndicator',
            type: 'string',
            mapping: 'DecisionReviewOfficerFormalHearingHeldIndicator'
        },
        {
            name: 'decisionRemandedToName',
            type: 'string',
            mapping: 'DecisionRemandedToName'
        },
        {
            name: 'decisionTeamCode',
            type: 'string',
            mapping: 'DecisionTeamCode'
        },
        {
            name: 'counselId',
            type: 'string',
            mapping: 'CounselId'
        },
        {
            name: 'boardMemberId',
            type: 'string',
            mapping: 'BoardMemberId'
        },
        {
            name: 'hearingActionCode',
            type: 'string',
            mapping: 'HearingActionCode'
        },
        {
            name: 'hearingActionDescription',
            type: 'string',
            mapping: 'HearingActionDescription'
        },
        {
            name: 'regionalOfficeMailingStatusAction',
            type: 'string',
            mapping: 'RegionalOfficeMailingStatusAction'
        },
        {
            name: 'regionalOfficeMailingStatusDate',
            type: 'date',
            mapping: 'RegionalOfficeMailingStatusDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'regionalOfficeMailingStatusDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('regionalOfficeMailingStatusDate'))) {
                    return Ext.Date.format(record.get('regionalOfficeMailingStatusDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'remandedToCode',
            type: 'string',
            mapping: 'RemandedToCode'
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'AppealDecision'
        }
    }
});