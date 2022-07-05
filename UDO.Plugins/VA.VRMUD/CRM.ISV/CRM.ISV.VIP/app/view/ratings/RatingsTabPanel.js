/**
* @author Josh Oliver
* @class VIP.view.ratings.RatingsTabPanel
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.ratings.RatingsTabPanel', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.ratings.ratingstabpanel',

    requires: [
        'VIP.view.ratings.DisabilityRatings',
        'VIP.view.ratings.DeathRatings',
        'VIP.view.ratings.FamilyMemberRatings',
        'VIP.view.ratings.OtherRatings',
        'VIP.view.ratings.SMCRatings'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'ratings.disabilityratings' },
            { xtype: 'ratings.deathratings' },
            { xtype: 'ratings.familymemberratings' },
            { xtype: 'ratings.otherratings' },
            { xtype: 'ratings.smcratings' }
        ];

        me.callParent();
    }
});
