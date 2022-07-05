/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.Misc2
*
* The view for Misc2 associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.Misc2', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.birls.birlsdetails.misc2',
    title: 'Misc 2',
    requires: ['VIP.view.birls.birlsdetails.misc2.Indicators'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;
        me.items = [{
            xtype: 'birls.birlsdetails.misc2.indicators',
            flex: 1
        }];

        me.callParent();
    }
});