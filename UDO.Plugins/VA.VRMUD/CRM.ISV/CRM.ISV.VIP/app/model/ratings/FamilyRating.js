/**
* @author Josh Oliver
* @class VIP.model.ratings.FamilyRating
*
* The model for family ratings
*/
Ext.define('VIP.model.ratings.FamilyRating', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'beginDate',
        type: 'string'
    }, {
        name: 'decisionTypeName',
        type: 'string'
    }, {
        name: 'disabilityTypeName',
        type: 'string'
    }, {
        name: 'endDate',
        type: 'string'
    }, {
        name: 'familyMemberName',
        type: 'string'
    }, {
        name: 'ratingDate',
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