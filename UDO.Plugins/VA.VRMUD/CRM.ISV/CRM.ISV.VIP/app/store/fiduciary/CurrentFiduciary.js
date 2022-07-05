/**
* @author Ivan Yurisevic
* @class VIP.store.fiduciary.CurrentFiduciary
*
* Store associated with CorpDetails model
*/
Ext.define('VIP.store.fiduciary.CurrentFiduciary', {
    extend: 'Ext.data.Store',
    requires: 'VIP.model.fiduciary.CurrentFiduciary',
    model: 'VIP.model.fiduciary.CurrentFiduciary'
});