/**
* @author Ivan Yurisevic
* @class VIP.view.ratings.smcratings.ParagraphRatings
*
* The view for the past ParagraphRatings grid
*/
Ext.define('VIP.view.ratings.smcratings.ParagraphRatings', {
	extend: 'Ext.grid.Panel',
	alias: 'widget.ratings.smcratings.paragraphratings',
	store: 'ratings.SmcParagraphRating',
	title: 'Special Monthly Compensation Paragraph Ratings',

	initComponent: function () {
		var me = this;

		me.columns = {
			defaults: {
				//flex: 1
			},
		items: [
				{ header: 'Profile Date', dataIndex: 'profileDate', width: 70 },
				{ header: 'Rating Id', dataIndex: 'ratingId', width: 70 },
				{ header: 'Paragraph Key', dataIndex: 'smcParagraphKeyTypeName', width: 90 },
				{ header: 'SMC Paragraph Text', dataIndex: 'smcParagraphText', flex: 1 }
			]            
		};

		me.callParent();
	}
});
