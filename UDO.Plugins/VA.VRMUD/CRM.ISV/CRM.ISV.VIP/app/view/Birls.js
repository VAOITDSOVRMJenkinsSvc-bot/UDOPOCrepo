/**
* @author Ivan Yurisevic
* @class VIP.view.Birls
*
* The main panel to contain BIRLS data
*/
Ext.define('VIP.view.Birls', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.birls',
    title: 'BIRLS Data',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.birls.IdentificationData', 'VIP.view.birls.BirlsDetails'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'birls.identificationdata',
            flex: 1
        }, {
            xtype: 'birls.birlsdetails',
            flex: 1
        }];

        me.callParent();
    }
});