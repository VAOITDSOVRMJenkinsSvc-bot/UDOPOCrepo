/**
* @author Ivan Yurisevic
* @class VIP.store.personinfo.GeneralDetails
*
* Store associated with CorpDetails model
*/
Ext.define('VIP.store.personinfo.GeneralDetails', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.personinfo.GeneralDetails'
    ],
    model: 'VIP.model.personinfo.GeneralDetails'
});