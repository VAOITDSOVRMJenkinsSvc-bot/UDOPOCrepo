/**
* @author Jonas Dawson
* @class VIP.view.ContentPanel
*
* The main view for the VIP application.
*/
Ext.define('VIP.view.ContentPanel', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.contentpanel',
    requires: [
        'VIP.view.PersonSelection',
        'VIP.view.CrmLaunch',
        'VIP.view.Launch',
        'VIP.view.PersonTabPanel'
    ],
    layout: {
        type: 'accordion',
        titleCollapse: false,
        animate: true,
        bindToOwnerCtContainer: true,
        bindToOwnerCtComponent: true
    },
    autoScroll: true,
    initComponent: function () {
        var me = this;

        me.items = [
             {
                xtype: 'persontabpanel',
                title: 'Search Results', collapsed: true
            },
            {
                xtype: 'personselection',
                title: 'Person Selection', collapsed: true
            }
        ];

        me.callParent();
    }
});


