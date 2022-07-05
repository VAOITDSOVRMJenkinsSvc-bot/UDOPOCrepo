/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.TourHistory
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.pathways.requests.Requests', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.pathways.requests.requests',
    title: 'Exam Requests',

    requires: [
        'VIP.view.pathways.requests.RequestData',
        'VIP.view.pathways.requests.RequestDetails'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'pathways.requests.requestdata', flex: 1 },
            { xtype: 'pathways.requests.requestdetails', flex: 1 }
        ];

        me.callParent();
    }
});
