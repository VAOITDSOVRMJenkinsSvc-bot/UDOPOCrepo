/**
* @author Ivan Yurisevic
* @class VIP.view.ratings.smcratings.ParagraphRatings
*
* The view for the past ParagraphRatings grid
*/
Ext.define('VIP.view.ratings.smcratings.Ratings', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.ratings.smcratings.ratings',
    store: 'ratings.SmcRating',
    title: 'Special Monthly Compensation Ratings',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Anatomical Loss', dataIndex: 'anatomicalLossTypeName' },
			    { header: 'Begin Date', dataIndex: 'beginDate' },
			    { header: 'Hospital SMC', dataIndex: 'hospitalSmcTypeName'},
			    { header: 'Loss Use', dataIndex:'lossUseTypeName' },
                { header: 'Other Loss', dataIndex: 'otherLossTypeName'},
                { header: 'Rating Percent' , dataIndex:'ratingPercent'},
                { header: 'SMC', dataIndex:'smcTypeName' },
                { header: 'Supplemental Decision', dataIndex: 'supplementalDecisonTypeName' }
            ]
        };

        me.callParent();
    }
});
