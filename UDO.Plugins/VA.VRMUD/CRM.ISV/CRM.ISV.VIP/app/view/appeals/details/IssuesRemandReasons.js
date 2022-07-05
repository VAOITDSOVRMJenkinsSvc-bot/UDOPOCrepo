/**
* @author Ivan Yurisevic
* @class VIP.view.appeals.details.IssuesRemandReasons
*
* View for appeal decision and special contentions
*/
Ext.define('VIP.view.appeals.details.IssuesRemandReasons', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.appeals.details.issuesremandreasons',
    title: 'Issues/Remand Reasons',
    requires: [
        'VIP.view.appeals.details.issuesremandreasons.Issues',
        'VIP.view.appeals.details.issuesremandreasons.RemandReasons'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'appeals.details.issuesremandreasons.issues', flex: 2 },
            { xtype: 'appeals.details.issuesremandreasons.remandreasons', flex: 1 }
        ];

        me.callParent();
    }
});
