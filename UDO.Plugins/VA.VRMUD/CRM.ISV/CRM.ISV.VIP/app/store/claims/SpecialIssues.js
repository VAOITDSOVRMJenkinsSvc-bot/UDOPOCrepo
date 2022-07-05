/**
* @author Josh Oliver
* @class VIP.store.claims.SpecialIssue
*
* Store associated with special issues
*/
Ext.define('VIP.store.claims.SpecialIssues', {
    extend: 'Ext.data.Store',
    requires: [
        'VIP.model.claims.SpecialIssues'
    ],
    model: 'VIP.model.claims.SpecialIssues'
});