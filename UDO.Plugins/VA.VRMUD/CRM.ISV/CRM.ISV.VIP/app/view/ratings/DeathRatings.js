/**
* @author Josh Oliver
* @class VIP.view.ratings.DeathRatings
*
* The view for ratings death rating data
*/
Ext.define('VIP.view.ratings.DeathRatings', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.ratings.deathratings',
    store: 'ratings.DeathRating',
    title: 'Death Ratings',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { xtype: 'rownumberer' },
	            { text: 'Military Service Period', dataIndex: 'militaryServicePeriodTypeName' },
                { text: 'Person Death Cause', dataIndex: 'personDeathCauseTypeName' },
                { text: 'Rating Date', dataIndex: 'ratingDateFormatted' },
                { text: 'Rating Decision ID', dataIndex: 'ratingDecisionID' },
                { text: 'Service Connected Death Decision', dataIndex: 'serviceConnectedDeathDecisionTypeName' }
            ]
        };

        me.callParent();
    }
});
