/**
* @author Josh Oliver
* @class VIP.store.ratings.SmcRating
*
* Store associated with smc ratings
*/
Ext.define('VIP.store.ratings.SmcRating', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ratings.SmcRating'
    ],
    model: 'VIP.model.ratings.SmcRating'
});