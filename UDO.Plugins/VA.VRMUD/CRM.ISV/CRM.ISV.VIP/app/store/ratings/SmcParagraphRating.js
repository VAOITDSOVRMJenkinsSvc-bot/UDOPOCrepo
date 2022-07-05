/**
* @author Josh Oliver
* @class VIP.store.ratings.SmcParagraphRating
*
* Store associated with smc paragraph ratings
*/
Ext.define('VIP.store.ratings.SmcParagraphRating', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ratings.SmcParagraphRating'
    ],
    model: 'VIP.model.ratings.SmcParagraphRating'
});