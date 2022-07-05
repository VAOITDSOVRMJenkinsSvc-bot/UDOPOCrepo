/**
* @class VIP.view.personinfo.details.Flashes
* View for Flashes associated with the person
*/
Ext.define('VIP.view.personinfo.details.Flashes', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personinfo.details.flashes',
    store: 'personinfo.Flashes',
    title: 'Flashes',

    initComponent: function () {
        var me = this;

        me.columns = [
            { xtype: 'rownumberer' },
			{ text: 'Flash Name', flex: 1, dataIndex: 'flashName' },
			{ text: 'Flash Type', flex: 1, dataIndex: 'flashType' },
			{ text: 'Assigned Ind', flex: 1, dataIndex: 'assignedInd'}
        ];

        me.callParent();
    }
}); 