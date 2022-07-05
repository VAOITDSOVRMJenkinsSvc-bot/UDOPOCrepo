/**
 * @author Jonas Dawson
 * @class VIP.view.Claims
 *
 * The main panel for the Claims tab
 */
Ext.define('VIP.view.Claims', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.claims',
    title: 'Claims',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.claims.Benefits', 'VIP.view.claims.Details'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'claims.benefits',
            flex: 1
        }, {
            xtype: 'claims.details',
            flex: 1
        }];

    me.callParent();
}
});