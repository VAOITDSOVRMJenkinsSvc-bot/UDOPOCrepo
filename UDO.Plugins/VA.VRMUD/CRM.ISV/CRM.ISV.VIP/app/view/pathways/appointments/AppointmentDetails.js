/**
* @author Josh Oliver
* @class VIP.view.appointments.AppointmentDetails
*
* The main panel for personal information
*/
Ext.define('VIP.view.pathways.appointments.AppointmentDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.pathways.appointments.appointmentdetails',
    store: 'pathways.Appointment',
    title: 'Appointment Details',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
        //Date/Time fields need to be formatted
        {
        xtype: 'displayfield',
        fieldLabel: 'Patient',
        name: 'patientName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Appointment Status',
        name: 'appointmentStatus'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Institution',
        name: 'locationInstitutionName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'EKG Date/Time',
        name: 'ekgDateTime'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Appointment Date/Time',
        name: 'appointmentDateTime_f'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Appointment Type',
        name: 'appointmentType'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Lab Date/Time',
        name: 'labDateTime'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'X-Ray Date/Time',
        name: 'xrayDateTime'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Location',
        name: 'locationName'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Status',
        name: 'status'
    }, {
        xtype: 'displayfield',
        fieldLabel: 'Telephone',
        name: 'locationPhone'
    }];

    me.callParent();
}
});