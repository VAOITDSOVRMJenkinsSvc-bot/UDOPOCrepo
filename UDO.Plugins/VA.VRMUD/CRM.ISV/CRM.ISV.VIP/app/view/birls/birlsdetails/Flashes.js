/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.Flashes
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.Flashes', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.birls.birlsdetails.flashes',
    store: 'birls.Flashes',
    title: 'Flashes',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Flash Code', dataIndex: 'flashCode' },
                { header: 'Flash Station', dataIndex: 'flashStation' },
                { header: 'Flash Routing Symbol', dataIndex: 'flashRoutingSymbol' }
            ]
        };

        me.callParent();
    }
});
