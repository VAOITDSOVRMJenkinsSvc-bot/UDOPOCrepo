/**
* @class VIP.view.ratings.disabilityratings.DisabilityData
* The view for disability data
*/
Ext.define('VIP.view.ratings.disabilityratings.DisabilityData', {
	extend: 'Ext.grid.Panel',
	alias: 'widget.ratings.disabilityratings.disabilitydata',
	store: 'ratings.DisabilityRating',

	initComponent: function () {
		var me = this;

		me.columns = {
			defaults: {
				//flex: 1
			},
			items: [
				{ xtype: 'rownumberer' },
				{ header: 'Begin Date', dataIndex: 'beginDate', width: 70 , xtype: 'datecolumn', format: 'm/d/Y' },
				{ header: 'Combat Ind', dataIndex: 'combatIndicator', width: 70 },
				{ header: 'Diagnostic %', dataIndex: 'diagnosticPercent', width: 80 },
				{ header: 'Diagnostic', dataIndex: 'diagnosticText', width: 165 },
				{ header: 'Description', dataIndex: 'disabilityDecisionTypeName', width: 200 },
				{ header: 'Code', dataIndex: 'disabilityDecisionTypeCode', width: 90 },
				{ header: 'Diagnostic Type', dataIndex: 'diagnosticTypeName', width: 300 },
				{ header: 'Diagnostic Type Code', dataIndex: 'diagnosticTypeCodeFormatted', width: 120 },
				{ header: 'Disability Id', dataIndex: 'disabilityId' }
			]
		};

		me.callParent();
	}
});
