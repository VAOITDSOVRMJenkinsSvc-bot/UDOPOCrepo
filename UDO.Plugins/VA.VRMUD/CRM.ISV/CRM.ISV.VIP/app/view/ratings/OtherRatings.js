/**
* @class VIP.view.ratings.smcratings.ParagraphRatings
* The view for the past ParagraphRatings grid
*/
Ext.define('VIP.view.ratings.OtherRatings', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.ratings.otherratings',
    store: 'ratings.OtherRating',
    title: 'Other Ratings',

    initComponent: function () {
        var me = this;


        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Begin Date', dataIndex:'beginDate', xtype: 'datecolumn', format: 'm/d/Y'},
			    { header: 'Decision', dataIndex:'decisionTypeName' },
			    { header: 'Disability', dataIndex: 'disabilityTypeName' },
			    { header: 'End Date', dataIndex: 'endDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Rating Date', dataIndex: 'ratingDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Supplemental Decision', dataIndex: 'supplementalDecisionTypeName' }
            ]
        };

        me.callParent();
    }
});
