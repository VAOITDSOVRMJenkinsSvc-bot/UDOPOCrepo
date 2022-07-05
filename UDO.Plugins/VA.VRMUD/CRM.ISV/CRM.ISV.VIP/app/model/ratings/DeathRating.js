/**
* @author Stan Salatov
* @class VIP.model.ratings.DeathRating
*
* The model for disability ratings
*/
Ext.define('VIP.model.ratings.DeathRating', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'militaryServicePeriodTypeName',
        type: 'string',
        mapping: 'militaryServicePeriodTypeName'
    }, {
        name: 'personDeathCauseTypeName',
        type: 'string',
        mapping: 'personDeathCauseTypeName'
    }, {
        name: 'ratingDate',
        type: 'string',
        mapping: 'ratingDate'
    }, {
        name: 'ratingDecisionID',
        type: 'string',
        mapping: 'ratingDecisionID'
    }, {
        name: 'serviceConnectedDeathDecisionTypeName',
        type: 'string',
        mapping: 'serviceConnectedDeathDecisionTypeName'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'ratings'
        }
    }
});