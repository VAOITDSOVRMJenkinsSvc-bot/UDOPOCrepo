/**
* @author Ivan Yurisevic
* @class VIP.view.ratings.smcratings.ParagraphRatings
*
* The view for the past ParagraphRatings grid
*/
Ext.define('VIP.view.ratings.FamilyMemberRatings', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.ratings.familymemberratings',
    store: 'ratings.FamilyRating',
    title: 'Family Member Ratings',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Begin Date', dataIndex: 'beginDate' },
			    { header: 'Decision', dataIndex:'decisionTypeName' },
			    { header: 'Disability', dataIndex:'disabilityTypeName' },
			    { header: 'End Date', dataIndex: 'endDate' }, 
                { header: 'Family Member Name', dataIndex: 'familyMemberName' },
                { header: 'Rating Date', dataIndex: 'ratingDate' }
            ]
        };

        me.callParent();
    }
});
