/**
* @author Josh Oliver
* @class VIP.store.ratings.FamilyRating
*
* Store associated with family ratings
*/
Ext.define('VIP.store.ratings.FamilyRating', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ratings.FamilyRating'
    ],
    model: 'VIP.model.ratings.FamilyRating'
});