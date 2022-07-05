/**
* @author Ivan Yurisevic
* @class VIP.store.PastFiduciaries
*
* Store associated with CorpDetails model
*/
Ext.define('VIP.store.fiduciary.PastFiduciaries', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.fiduciary.PastFiduciaries',
    model: 'VIP.model.fiduciary.PastFiduciaries'
});