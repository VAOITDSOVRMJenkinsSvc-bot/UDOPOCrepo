/**
* @class VIP.model.ratings.OtherRating
* The model for other ratings
*/
Ext.define('VIP.model.ratings.OtherRating', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'beginDate',
        mapping: 'beginDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'decisionTypeName',
        mapping: 'decisionTypeName',
        type: 'string'
    }, {
        name: 'disabilityTypeName',
        mapping: 'disabilityTypeName',
        type: 'string'
    }, {
        name: 'endDate',
        mapping: 'endDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'ratingDate',
        mapping: 'ratingDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'supplementalDecisionTypeName',
        mapping: 'supplementalDecisionTypeName',
        type: 'string'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'ratings'
        }
    }
});