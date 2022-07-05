/**
* @author Josh Oliver
* @class VIP.controller.Pathways
*
* The controller for the pathways tabs
*/
Ext.define('VIP.controller.Pathways', {
    extend: 'Ext.app.Controller',
    requires: [
        'VIP.util.xml.FragmentBuilder'
    ],
    stores: [
        'pathways.Appointment',
        'pathways.Patient'
    ],
    refs: [{
        ref: 'appointment',
        selector: '[xtype="pathways.appointments.appointmentdetails"]'
    }, {
        ref: 'examDetail',
        selector: '[xtype="pathways.exams.examdetails"]'
    }, {
        ref: 'requestDetail',
        selector: '[xtype="pathways.requests.requestdetails"]'
    }, {
        ref: 'startDate',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="startDate"]'
    }, {
        ref: 'endDate',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="endDate"]'
    }, {
        ref: 'examRequestGrid',
        selector: '[xtype="pathways.requests.requestdata"]'
    }, {
        ref: 'examGrid',
        selector: '[xtype="pathways.exams.examdata"]'
    }, {
        ref: 'appointmentGrid',
        selector: '[xtype="pathways.appointments.appointmentdata"]'
    }],
    init: function () {
        var me = this;
        
        me.application.on({
            mvirecordloaded: me.loadPathwaysData,
            cacheddataloaded: me.onCachedDataLoaded,
            scope: me
        });

        me.control({
            '[xtype="pathways.appointments.appointmentdata"]': {
                selectionchange: me.onAppointmentGridSelection
            },
            '[xtype="pathways.exams.examdata"]': {
                selectionchange: me.onExamGridSelection
            },
            '[xtype="pathways.requests.requestdata"]': {
                selectionchange: me.onRequestGridSelection
            }
        });
    },

    onCachedDataLoaded: function () {
        var me = this,
            patientStore = me.getPathwaysPatientStore();

        if (Ext.isEmpty(patientStore)) { return; }
        var patientStoreData = patientStore.getAt(0);

        if (!Ext.isEmpty(patientStoreData) && !Ext.isEmpty(patientStoreData.examRequests())) {
            me.getExamRequestGrid().reconfigure(patientStoreData.examRequests());
        }

        if (!Ext.isEmpty(patientStoreData) && !Ext.isEmpty(patientStoreData.exams())) {
            me.getExamGrid().reconfigure(patientStoreData.exams());
        }
    },

    onExamGridSelection: function (selectionModel, selection, options) {
        var me = this;
        if (!Ext.isEmpty(selection)) {
            me.getExamDetail().loadRecord(selection[0]);
        }
    },

    onRequestGridSelection: function (selectionModel, selection, options) {
        var me = this;
        if (!Ext.isEmpty(selection)) {
            me.getRequestDetail().loadRecord(selection[0]);
        }
    },

    onAppointmentGridSelection: function (selectionModel, selection, options) {
        var me = this;
        if (!Ext.isEmpty(selection)) {
            me.getAppointment().loadRecord(selection[0]);
        }
    },

    loadPathwaysData: function (nationalId) {
        var me = this,
            startDate = !Ext.isEmpty(me.application.personInquiryModel) ? me.application.personInquiryModel.get('appointementFromDate') : null,
            endDate = !Ext.isEmpty(me.application.personInquiryModel) ? me.application.personInquiryModel.get('appointementToDate') : null;
        
        me.getExamGrid().setLoading(true);
        me.getExamRequestGrid().setLoading(true);
        me.getAppointmentGrid().setLoading(true);
        me.getPathwaysPatientStore().load({
            filters: [
                {
                    property: 'nationalId',
                    value: nationalId
                },
                {
                    property: 'startDate',
                    value: startDate
                },
                {
                    property: 'endDate',
                    value: endDate
                }
            ],
            callback: function (records, operation, success) {
                if (success && !Ext.isEmpty(records)) {
                    me.getExamGrid().reconfigure(records[0].exams());
                    me.getExamRequestGrid().reconfigure(records[0].examRequests());
                }
                me.getExamGrid().setLoading(false);
                me.getExamRequestGrid().setLoading(false);
                me.application.fireEvent('webservicecallcomplete', operation, 'pathways.Patient');
            },
            scope: me
        });

        me.getPathwaysAppointmentStore().load({
            filters: [
                {
                    property: 'nationalId',
                    value: nationalId
                },
                {
                    property: 'startDate',
                    value: startDate
                },
                {
                    property: 'endDate',
                    value: endDate
                }
            ],
                callback: function (records, operation, success) {
                me.getAppointmentGrid().setLoading(false);
                me.application.fireEvent('webservicecallcomplete', operation, 'pathways.Appointment');
            },
            scope: me
        });
    }
});
