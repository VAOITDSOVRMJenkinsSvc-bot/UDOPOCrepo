/**
* @author Josh Oliver
* @class VIP.view.PersonInfo
*
* The main panel for personal information
*/
Ext.define('VIP.view.PersonVadir', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.personvadir',
    title: 'Vadir',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.personVadir.Person', 'VIP.view.personVadir.Details'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'personvadir.person',
            flex: 1
        }, {
            xtype: 'personvadir.details',
            flex: 1
        }];

        me.callParent();
    }
});