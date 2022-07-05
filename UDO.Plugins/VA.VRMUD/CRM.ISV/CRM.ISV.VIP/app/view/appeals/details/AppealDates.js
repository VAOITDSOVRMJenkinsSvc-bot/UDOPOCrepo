/**
* @author Josh Oliver
* @class VIP.view.appeals.details.AppealDates
*
* View for appeals dates
*/
Ext.define('VIP.view.appeals.details.AppealDates', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.appeals.details.appealdates',
    title: 'Appeal Dates',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Date Type Code', dataIndex: 'dateTypeCode' },
			    { header: 'Date Type Description', dataIndex: 'dateTypeDescription' },
			    { header: 'Date', dataIndex: 'date', xtype: 'datecolumn', format: 'm/d/Y' }
            ]
        };

        me.callParent();
    }
});