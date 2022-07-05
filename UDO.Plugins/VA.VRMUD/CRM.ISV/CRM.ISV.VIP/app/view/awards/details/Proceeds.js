/**
* @author Jonas Dawson
* @class VIP.view.awards.details.Proceeds
*
* Proceeds for the selected award line
*/
Ext.define('VIP.view.awards.details.Proceeds', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.proceeds',
    store: 'awards.Proceeds',
    title: 'Proceeds',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Type', dataIndex: 'code' },
                { header: 'Description', dataIndex: 'name' },
                { header: 'Balance Amount', dataIndex: 'balance', xtype: 'numbercolumn', format: '$0,000.00' }
            ]
        };

        me.callParent();
    }
});
