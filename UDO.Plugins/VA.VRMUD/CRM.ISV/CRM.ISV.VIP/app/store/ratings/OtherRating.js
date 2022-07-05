/**
* @class VIP.store.ratings.OtherRating
* Store associated with other ratings
*/
Ext.define('VIP.store.ratings.OtherRating', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ratings.OtherRating'
    ],
    model: 'VIP.model.ratings.OtherRating'
});