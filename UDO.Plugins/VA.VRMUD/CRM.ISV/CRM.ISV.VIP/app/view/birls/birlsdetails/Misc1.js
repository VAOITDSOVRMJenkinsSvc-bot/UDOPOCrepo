/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.Misc1
*
* The view for Misc1 associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.Misc1', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.birls.birlsdetails.misc1',
    title: 'Misc 1',

    requires: ['VIP.view.birls.birlsdetails.misc1.General', 'VIP.view.birls.birlsdetails.misc1.LastUpdated', 'VIP.view.birls.birlsdetails.misc1.RetirePaySBP'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'birls.birlsdetails.misc1.general',
            flex: 1
        }, {
            xtype: 'birls.birlsdetails.misc1.retirepaysbp',
            flex: 1
        }, {
            xtype: 'birls.birlsdetails.misc1.lastupdated',
            flex: 1
        }];

        me.callParent();
    }

});