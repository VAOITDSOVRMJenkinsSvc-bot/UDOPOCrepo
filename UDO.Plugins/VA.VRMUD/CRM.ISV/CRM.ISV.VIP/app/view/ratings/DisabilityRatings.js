/**
* @author Ivan Yurisevic
* @class VIP.view.ratings.DisabilityRatings
*
* View for disability rating data
*/
Ext.define('VIP.view.ratings.DisabilityRatings', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.ratings.disabilityratings',
    title: 'Disability Ratings',
    requires: [
        'VIP.view.ratings.disabilityratings.DisabilityData',
        'VIP.view.ratings.disabilityratings.DisabilityDetails',
        'VIP.view.ratings.disabilityratings.DisabilityRecordDetails'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'ratings.disabilityratings.disabilitydata', flex: 2 },
            { xtype: 'ratings.disabilityratings.disabilitydetails', flex: 3},
            { xtype: 'ratings.disabilityratings.disabilityrecorddetails', flex: 2}
        ];

        me.callParent();
    }
});
