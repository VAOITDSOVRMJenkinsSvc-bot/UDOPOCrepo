/**
* @author Josh Oliver
* @class VIP.model.ratings.SmcRating
*
* The model for smc ratings
*/
Ext.define('VIP.model.ratings.SmcRating', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'anatomicalLossTypeName',
        type: 'string'
    }, {
        name: 'beginDate',
        type: 'string'
    }, {
        name: 'hospitalSmcTypeName',
        type: 'string',
        mapping: 'hospitalSMCTypeName'
    }, {
        name: 'lossUseTypeName',
        type: 'string'
    }, {
        name: 'otherLossTypeName',
        type: 'string'
    }, {
        name: 'ratingPercent',
        type: 'string'
    }, {
        name: 'smcTypeName',
        type: 'string'
    }, {
        name: 'supplementalDecisonTypeName',
        type: 'string'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'smcRatings'
        }
    }
});