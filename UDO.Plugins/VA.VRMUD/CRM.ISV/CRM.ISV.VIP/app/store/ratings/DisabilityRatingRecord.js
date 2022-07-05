/**
* @author Josh Oliver
* @class VIP.store.ratings.DisabilityRatingRecord
*
* Store associated with disability rating records
*/
Ext.define('VIP.store.ratings.DisabilityRatingRecord', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.ratings.DisabilityRatingRecord'
    ],
    model: 'VIP.model.ratings.DisabilityRatingRecord'
});