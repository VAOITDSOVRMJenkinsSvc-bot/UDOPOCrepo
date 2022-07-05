/**
* @author Ivan Yurisevic
* @class VIP.view.RequestsExams
*
* The main panel for Requests/Exams service. Contains the military tab panel
*/
Ext.define('VIP.view.Ratings', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.ratings',
    title: 'Ratings',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    layout: 'fit',
    requires: [
        'VIP.view.ratings.RatingsTabPanel'
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'ratings.ratingstabpanel' }
        ];

        me.callParent();
    }
});