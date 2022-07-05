/**
* @author Ivan Yurisevic
* @class VIP.view.appeals.Details
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.appeals.Details', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.appeals.details',    
    requires: [
        'VIP.view.appeals.details.AppealDates',
        'VIP.view.appeals.details.AppealDecisionSpecialContentions',
        'VIP.view.appeals.details.AppealRecord',
        'VIP.view.appeals.details.Appellant',
        'VIP.view.appeals.details.Diaries',
        'VIP.view.appeals.details.HearingRequest',
        'VIP.view.appeals.details.IssuesRemandReasons'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'appeals.details.appealrecord' },
            { xtype: 'appeals.details.issuesremandreasons' },
            { xtype: 'appeals.details.diaries' },
            { xtype: 'appeals.details.appealdecisionspecialcontentions' },
            { xtype: 'appeals.details.appealdates' },
            { xtype: 'appeals.details.hearingrequest' },
            { xtype: 'appeals.details.appellant' }
        ];

        me.callParent();
    }
});
