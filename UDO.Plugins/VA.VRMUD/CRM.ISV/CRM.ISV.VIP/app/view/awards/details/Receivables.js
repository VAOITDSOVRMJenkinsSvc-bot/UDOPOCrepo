/**
* @author Jonas Dawson
* @class VIP.view.awards.details.Receivables
*
* Receivables for the selected award line
*/
Ext.define('VIP.view.awards.details.Receivables', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.details.receivables',
    store: 'awards.Receivables',
    title: 'Receivables',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Type', dataIndex: 'code' },
                { header: 'Description', dataIndex: 'name' },
                { header: 'Discovery Date', dataIndex: 'discoveryDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Balance Amount', dataIndex: 'balance', xtype: 'numbercolumn', format: '$0,000.00' }
            ]

        };

        me.callParent();
    }
});