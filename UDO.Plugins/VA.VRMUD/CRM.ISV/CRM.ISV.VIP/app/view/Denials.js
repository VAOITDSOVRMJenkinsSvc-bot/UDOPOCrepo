/**
* @author Ivan Yurisevic
* @class VIP.view.Denials
*
* The main panel for Denials tab. Contains the military tab panel
*/
Ext.define('VIP.view.Denials', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.denials',
    title: 'Denials',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: [
        'VIP.view.denials.DenialsList',
        'VIP.view.denials.DenialDetail'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'denials.denialslist', flex: 1 },
            { xtype: 'denials.denialdetail', flex: 1 }
        ];

        me.callParent();
    }
});