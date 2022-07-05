/**
* @author Ivan Yurisevic
* @class VIP.view.Fiduciary
*
* The Fiduciary base panel, stores two components inside
*/
Ext.define('VIP.view.Fiduciary', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.fiduciary',
    title: 'Fiduciary',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.fiduciary.CurrentFiduciary', 'VIP.view.fiduciary.PastFiduciaries'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'fiduciary.currentfiduciary',
            flex: 1
        }, {
            xtype: 'fiduciary.pastfiduciaries',
            flex: 1
        }];

        me.callParent();
    }
});