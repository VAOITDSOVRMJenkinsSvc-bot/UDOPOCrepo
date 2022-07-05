/**
* @author Josh Oliver
* @class VIP.store.ratings.DisabilityRating
*
* Store associated with disability ratings
*/
Ext.define('VIP.store.ratings.DisabilityRating', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ratings.DisabilityRating'
    ],
    model: 'VIP.model.ratings.DisabilityRating'
});