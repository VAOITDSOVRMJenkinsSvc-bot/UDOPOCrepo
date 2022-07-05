/**
* @author Ivan Yurisevic
* @class VIP.view.RequestsExams
*
* The main panel for Requests/Exams service. Contains the military tab panel
*/
Ext.define('VIP.view.pathways.RequestsExams', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.pathways.requestsexams',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: [
        'VIP.view.pathways.exams.Exams',
        'VIP.view.pathways.requests.Requests'
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'pathways.exams.exams' },
            { xtype: 'pathways.requests.requests' }
        ];

        me.callParent();
    }
});