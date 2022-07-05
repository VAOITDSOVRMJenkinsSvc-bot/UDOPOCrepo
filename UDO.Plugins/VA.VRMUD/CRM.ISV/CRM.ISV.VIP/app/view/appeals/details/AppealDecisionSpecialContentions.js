/**
* @author Ivan Yurisevic
* @class VIP.view.appeals.details.AppealDecisionSpecialContentions
*
* View for appeal decision and special contentions
*/
Ext.define('VIP.view.appeals.details.AppealDecisionSpecialContentions', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.appeals.details.appealdecisionspecialcontentions',
    title: 'Appeal Decision/Special Contentions',
    requires: [
        'VIP.view.appeals.details.appealdecisionspecialcontentions.AppealDecision',
        'VIP.view.appeals.details.appealdecisionspecialcontentions.SpecialContentions'
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'appeals.details.appealdecisionspecialcontentions.appealdecision', flex: 1 },
            { xtype: 'appeals.details.appealdecisionspecialcontentions.specialcontentions', flex: 1 }
        ];

        me.callParent();
    }
});
