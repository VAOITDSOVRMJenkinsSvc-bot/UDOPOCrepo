/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.Decorations
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.Decorations', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.decorations',
    store: 'militaryservice.Decoration',
    title: 'Decorations',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Decoration Id', dataIndex: 'militaryDecorationId' },
                { header: 'Decoration Name', dataIndex: 'militaryDecorationName' },
                { header: 'Participant Id', dataIndex: 'participantId' }
            ]
        };

        me.callParent();
    }
});
