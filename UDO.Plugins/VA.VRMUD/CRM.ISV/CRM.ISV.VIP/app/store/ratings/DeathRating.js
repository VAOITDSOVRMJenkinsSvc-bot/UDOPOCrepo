/**
* @author Josh Oliver
* @class VIP.store.ratings.DeathRating
*
* Store associated with death ratings
*/
Ext.define('VIP.store.ratings.DeathRating', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ratings.DeathRating'
    ],
    model: 'VIP.model.ratings.DeathRating'
});