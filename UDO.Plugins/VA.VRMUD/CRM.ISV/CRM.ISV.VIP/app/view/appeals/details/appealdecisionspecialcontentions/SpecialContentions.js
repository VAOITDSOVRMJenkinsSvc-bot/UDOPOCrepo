/**
* @author Josh Oliver
* @class VIP.view.appeals.details.appealdecisionspecialcontentions.SpecialContentions
*
* View for appeals dates
*/
Ext.define('VIP.view.appeals.details.appealdecisionspecialcontentions.SpecialContentions', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.appeals.details.appealdecisionspecialcontentions.specialcontentions',
    title: 'Special Contentions',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Contention Code', dataIndex: 'contentionCode' },
			    { header: 'Contention Description', dataIndex: 'contentionDescription' },
			    { header: 'Contention Indicator', dataIndex: 'contentionIndicator' }
            ]
        };

        me.callParent();
    }
});