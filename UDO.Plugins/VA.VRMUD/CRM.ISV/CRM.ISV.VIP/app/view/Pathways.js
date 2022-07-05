/**
* @author Ivan Yurisevic
* @class VIP.view.Pathways
*
*/
Ext.define('VIP.view.Pathways', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.pathways',
    title: 'Exams & Appts',
    layout: 'fit',
    requires: [
        'VIP.view.pathways.Mvi',
        'VIP.view.pathways.Appointments',
        'VIP.view.pathways.exams.Exams',
        'VIP.view.pathways.requests.Requests'        
    ],
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'pathways.mvi' },
            { xtype: 'pathways.appointments' },
            { xtype: 'pathways.requests.requests' },
            { xtype: 'pathways.exams.exams' }
        ];

        me.callParent();
    }
});
