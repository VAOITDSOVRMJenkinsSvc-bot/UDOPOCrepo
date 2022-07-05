/**
* @author Josh Oliver
* @class VIP.view.Appointments
*
* The main panel for appointment information
*/
Ext.define('VIP.view.pathways.Appointments', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.pathways.appointments',
    title: 'Appointments',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    requires: [
        'VIP.view.pathways.appointments.AppointmentData',
        'VIP.view.pathways.appointments.AppointmentDetails'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'pathways.appointments.appointmentdata',
                flex: 1
            },
            {
                xtype: 'pathways.appointments.appointmentdetails',
                flex: 1
            }
        ];

        me.callParent();
    }
});