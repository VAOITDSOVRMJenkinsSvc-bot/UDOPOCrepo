/**
* @author Ivan Yurisevic
* @class VIP.view.RequestsExams
*
* The main panel for Requests/Exams service. Contains the military tab panel
*/
Ext.define('VIP.view.ratings.SMCRatings', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.ratings.smcratings',
    title: 'SMC Ratings',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: ['VIP.view.ratings.smcratings.Ratings', 'VIP.view.ratings.smcratings.ParagraphRatings'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'ratings.smcratings.ratings', flex: 1}, 
            { xtype: 'ratings.smcratings.paragraphratings', flex: 1}
        ];

        me.callParent();
    }
});