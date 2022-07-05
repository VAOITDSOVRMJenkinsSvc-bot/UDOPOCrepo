/**
* @author Ivan Yurisevic
* @class VIP.model.fiduciary.PastFiduciaries
*
* The model for past fiduciaries
*/
Ext.define('VIP.model.fiduciary.PastFiduciaries', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'beginDate',
        mapping: 'beginDate',
        type: 'date',
        dateFormat: 'm/d/Y'
    }, {
        name: 'endDate',
        mapping: 'endDate',
        type: 'date',
        dateFormat: 'm/d/Y'
    }, {
        name: 'eventDate',
        mapping: 'eventDate',
        type: 'string'
    }, {
        name: 'healthcareProviderReleaseInd',
        mapping: 'healthcareProviderReleaseIndicator',
        type: 'string'
    }, {
        name: 'journalDate',
        mapping: 'journalDate',
        type: 'date',
        dateFormat: 'm/d/Y'
    }, {
        name: 'journalLocationID',
        mapping: 'journalLocationID',
        type: 'string'
    }, {
        name: 'journalObjectID',
        mapping: 'journalObjectID',
        type: 'string'
    }, {
        name: 'journalStatusTypeCode',
        mapping: 'journalStatusTypeCode',
        type: 'string'
    }, {
        name: 'journalUserID',
        mapping: 'journalUserID',
        type: 'string'
    }, {
        name: 'personOrOrganizationInd',
        mapping: 'personOrOrganizationIndicator',
        type: 'string'
    }, {
        name: 'personOrgAttentionText',
        mapping: 'personOrgAttentionText',
        type: 'string'
    }, {
        name: 'personOrgName',
        mapping: 'personOrgName',
        type: 'string'
    }, {
        name: 'personOrgParticipantID',
        mapping: 'personOrgPtcpntID',
        type: 'string'
    }, {
        name: 'personOrganizationCode',
        mapping: 'personOrganizationCode',
        type: 'string'
    }, {
        name: 'personOrganizationName',
        mapping: 'personOrganizationName',
        type: 'string'
    }, {
        name: 'prepositionalPhraseName',
        mapping: 'prepositionalPhraseName',
        type: 'string'
    }, {
        name: 'rateName',
        mapping: 'rateName',
        type: 'string'
    }, {
        name: 'relationshipName',
        mapping: 'relationshipName',
        type: 'string'
    }, {
        name: 'statusCode',
        mapping: 'statusCode',
        type: 'string'
    }, {
        name: 'temporaryCustodianInd',
        mapping: 'temporaryCustodianIndicator',
        type: 'string'
    }, {
        name: 'veteranParticipantID',
        mapping: 'veteranPtcpntID',
        type: 'string'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'pastFiduciaries'
        }
    }
});