/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.TourHistory
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.militaryservice.militarytabs.TourHistory', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.militaryservice.militarytabs.tourhistory',
    title: 'Tour History',

    requires: [
        'VIP.view.militaryservice.militarytabs.tourhistory.TourHistoryDetails',
        'VIP.view.militaryservice.militarytabs.tourhistory.TourHistoryList'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'militaryservice.militarytabs.tourhistory.tourhistorylist', flex: 1 },
            { xtype: 'militaryservice.militarytabs.tourhistory.tourhistorydetails', flex: 1 }
        ];

        me.callParent();
    }
});
