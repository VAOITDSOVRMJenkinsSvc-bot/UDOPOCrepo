/**
* @author Ivan Yurisevic
* @class VIP.view.Poa
*
* The POA base panel, stores the two components inside
*/
Ext.define('VIP.view.Poa', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.poa',
    title: 'POA',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.poa.CurrentPoa', 'VIP.view.poa.PastPoas'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'poa.currentpoa',
            flex: 1
        }, {
            xtype: 'poa.pastpoas',
            flex: 1
        }];

        me.callParent();
    }
});