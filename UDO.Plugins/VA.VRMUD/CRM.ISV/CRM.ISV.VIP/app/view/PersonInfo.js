/**
* @author Josh Oliver
* @class VIP.view.PersonInfo
*
* The main panel for personal information
*/
Ext.define('VIP.view.PersonInfo', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.personinfo',
    title: 'Person Info',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.personinfo.Corp', 'VIP.view.personinfo.Details'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'personinfo.corp',
            flex: 1
        }, {
            xtype: 'personinfo.details',
            flex: 1
        }];

        me.callParent();
    }
});