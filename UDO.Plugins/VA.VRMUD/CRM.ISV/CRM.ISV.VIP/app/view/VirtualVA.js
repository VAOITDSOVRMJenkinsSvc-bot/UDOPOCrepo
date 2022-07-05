/**
* @author Josh Oliver
* @class VIP.view.VirtualVA
*
* The main panel for Virtual VA tab
*/
Ext.define('VIP.view.VirtualVA', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.virtualva',
    title: 'Virtual VA',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: [
        'VIP.view.virtualva.DocumentList'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'virtualva.documentlist', flex: 1 }
        ];

        me.callParent();
    }
});