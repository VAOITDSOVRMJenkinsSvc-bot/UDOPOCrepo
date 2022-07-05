/**
* @author Ivan Yurisevic
* @class VIP.view.MilitaryService
*
* The main panel for Military service. Contains the military tab panel
*/
Ext.define('VIP.view.MilitaryService', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.militaryservice',
    title: 'Military Service',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    layout: 'fit',
    requires: [
        'VIP.view.militaryservice.MilitaryTabs'
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'militaryservice.militarytabs'
            }
        ];

        me.callParent();
    }
});