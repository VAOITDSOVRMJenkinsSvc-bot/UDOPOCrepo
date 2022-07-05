/**
* @author Jonas Dawson
* @class VIP.view.claims.Details
*
* All claims components
*/
Ext.define('VIP.view.claims.Details', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.claims.details',
    tabPosition: 'top',
    plain: true,
    requires: [
        'VIP.view.claims.details.ClaimDetails',
        'VIP.view.claims.details.Contentions',
        'VIP.view.claims.details.Evidence',
        'VIP.view.claims.details.Letters',
        'VIP.view.claims.details.LifeCycle',
        'VIP.view.claims.details.Notes',
        'VIP.view.claims.details.Status',
        'VIP.view.claims.details.Suspenses',
        'VIP.view.claims.details.TrackedItems'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'claims.details.claimdetails' },
            { xtype: 'claims.details.contentions' },
            { xtype: 'claims.details.evidence' },
            { xtype: 'claims.details.lifecycle' },
            { xtype: 'claims.details.status' },
            { xtype: 'claims.details.suspenses' },
            { xtype: 'claims.details.trackeditems' },
            { xtype: 'claims.details.letters' },
            { xtype: 'claims.details.notes' }
        ];

        me.callParent();
    }
});