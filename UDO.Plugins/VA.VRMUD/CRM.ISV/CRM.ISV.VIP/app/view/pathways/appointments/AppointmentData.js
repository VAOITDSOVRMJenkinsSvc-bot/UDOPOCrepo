/**
* @author Josh Oliver
* @class VIP.view.appointments.AppointmentData
*
* The main grid for appointment data
*/
Ext.define('VIP.view.pathways.appointments.AppointmentData', {
	extend: 'Ext.grid.Panel',
	alias: 'widget.pathways.appointments.appointmentdata',
	store: 'pathways.Appointment',
	initComponent: function () {
		var me = this;

		me.columns = {
			defaults: {
				flex: 1
			},
			//Date/Time fields need to be formatted
			items: [
				{ header: 'Patient', dataIndex: 'patientName' },
				{ header: 'Appointment Date', dataIndex: 'appointmentDateTime', xtype: 'datecolumn', format: 'm/d/Y h:i a' },
				{ header: 'Location', dataIndex: 'locationName' },
				{ header: 'Appointment Status', dataIndex: 'appointmentStatus' },
				{ header: 'Appointment Type', dataIndex: 'appointmentType' },
				{ header: 'Institution', dataIndex: 'locationInstitutionName' },
				{ header: 'Status', dataIndex: 'status' },
				{ header: 'EKG Date', dataIndex: 'ekgDateTime' },
				{ header: 'X-Ray Date', dataIndex: 'xrayDateTime' },
				{ header: 'Lab Date', dataIndex: 'labDateTime' },
				{ header: 'Telephone', dataIndex: 'locationPhone' },
                { header: 'Patient Id', dataIndex: 'patientIdentity' }
			]
		};

		me.callParent();
	}
});