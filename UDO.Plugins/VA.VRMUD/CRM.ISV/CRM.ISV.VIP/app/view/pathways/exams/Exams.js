/**
* @author Ivan Yurisevic
* @class VIP.view.requestsexams.requestsexamstabs.Exams
*
* The central tab panel to display all tabs for military info
*/
Ext.define('VIP.view.pathways.exams.Exams', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.pathways.exams.exams',
    title: 'Exams',

    requires: [
        'VIP.view.pathways.exams.ExamDetails',
        'VIP.view.pathways.exams.ExamData'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'pathways.exams.examdata', flex: 1 },
            { xtype: 'pathways.exams.examdetails', flex: 1 }
        ];

        me.callParent();
    }
});
