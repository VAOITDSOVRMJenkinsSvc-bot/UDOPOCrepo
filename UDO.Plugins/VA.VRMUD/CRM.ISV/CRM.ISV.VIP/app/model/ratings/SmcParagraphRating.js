/**
* @author Josh Oliver
* @class VIP.model.ratings.SmcParagraphRating
*
* The model for smc ratings
*/
Ext.define('VIP.model.ratings.SmcParagraphRating', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'profileDate',
        type: 'string'
    }, {
        name: 'ratingId',
        type: 'string',
        mapping: 'ratingID'
    }, {
        name: 'smcParagraphKeyTypeName',
        type: 'string'
    }, {
        name: 'smcParagraphText',
        type: 'string'
    }],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'smcParagraphRatings'
        }
    }
});