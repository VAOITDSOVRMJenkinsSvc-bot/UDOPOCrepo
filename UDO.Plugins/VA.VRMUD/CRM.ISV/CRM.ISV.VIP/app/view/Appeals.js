/**
* @author Josh Oliver
* @class VIP.view.Appeals
*
* The main panel for appeals information
*/
Ext.define('VIP.view.Appeals', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.appeals',
    title: 'Appeals',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: [
        'VIP.view.appeals.AppealData',
        'VIP.view.appeals.Details'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'appeals.appealdata', flex: 1 },
            { xtype: 'appeals.details', flex: 2 }
        ];

        me.callParent();
    }
});