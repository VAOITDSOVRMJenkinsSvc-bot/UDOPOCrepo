/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.TheatersAndPow
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.militaryservice.militarytabs.TheatersAndPow', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.militaryservice.militarytabs.theatersandpow',
    title: 'Theaters & POW',

    requires: [
        'VIP.view.militaryservice.militarytabs.theatersandpow.PowList',
        'VIP.view.militaryservice.militarytabs.theatersandpow.TheatersList'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'militaryservice.militarytabs.theatersandpow.theaterslist', flex: 1 },
            { xtype: 'militaryservice.militarytabs.theatersandpow.powlist', flex: 1 }
        ];

        me.callParent();
    }
});
