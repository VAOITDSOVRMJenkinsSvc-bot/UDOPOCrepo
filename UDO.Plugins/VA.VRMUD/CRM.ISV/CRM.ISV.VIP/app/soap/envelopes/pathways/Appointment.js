/*
TODO: replace CDATA with tags

	this.CDATA1 = "![CDATA[", this.CDATA1Rep = "_vrm.dctag_";
	this.CDATA2 = "]]", this.CDATA2Rep = "_vrm.dctag2_";
	innerXml = innerXml.replace(this.CDATA1, this.CDATA1Rep).replace(this.CDATA2, this.CDATA2Rep);

*/

Ext.define('VIP.soap.envelopes.pathways.Appointment', {
	extend: 'VIP.soap.envelopes.pathways.PathwaysTemplate',
	alias: 'envelopes.Appointment',
	requires: [
        'VIP.util.xml.FragmentBuilder'
    ],
	config: {
		nationalId: '',
		startDate: undefined,
		endDate: undefined,
		rootNodeName: 'filter'
	},

	analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

	constructor: function (config) {
		var me = this,
			clientRequestInitiationDate = Ext.Date.format(new Date(), 'Y-m-d'),
            clientRequestInitiationTime = Ext.Date.format(new Date(), 'H:i:s'),
            clientRequestInitiationDateTime,
            startDate = Ext.Date.format(new Date(me.getStartDate()), 'Y-m-d'),
            endDate = Ext.Date.format(new Date(me.getEndDate()), 'Y-m-d'),
            cdataFragment,
            appointmentFragmentObject,
            cdataFragmentBuilder;

		me.initConfig(config);

		me.callParent();

		if (!Ext.isDate(startDate)) {
			startDate = Ext.Date.add(new Date(), Ext.Date.MONTH, -6);
			startDate = Ext.Date.format(new Date(startDate), 'Y-m-d');
		}

		if (!Ext.isDate(endDate)) {
			endDate = Ext.Date.add(new Date(), Ext.Date.MONTH, +6);
			endDate = Ext.Date.format(new Date(endDate), 'Y-m-d');
		}

		clientRequestInitiationDateTime = clientRequestInitiationDate + 'T' + clientRequestInitiationTime + 'Z';

		appointmentFragmentObject = {
			filter: {
				namespace: 'filter',
				namespaces: {
					'filter': 'Filter'
				},
				attributes: {
					'vhimVersion': 'Vhim_4_00'
				},
				filterId: {
					namespace: '',
					value: 'APPOINTMENTS_SINGLE_PATIENT_FILTER'
				},
				clientName: {
					namespace: '',
					value: 'VRM 1.0'
				},
				clientRequestInitiationTime: {
					namespace: '',
					value: clientRequestInitiationDateTime
				},
				patients: {
					namespace: '',
					NationalId: {
						namespace: '',
						value: me.getNationalId()
					}
				},
				entryPointFilter: {
					namespace: '',
					attributes: {
						'queryName': 'Appointments-Standardized'
					},
					domainEntryPoint: {
						namespace: '',
						value: 'Appointment'
					},
					startDate: {
						namespace: '',
						value: startDate
					},
					endDate: {
						namespace: '',
						value: endDate
					}
				}
			}
		};

		cdataFragmentBuilder = Ext.create('VIP.util.xml.FragmentBuilder', {
			xmlFragment: appointmentFragmentObject,
			rootNodeName: 'filter'
		});

		cdataFragment = me.getVrmDcOpeningTag() + cdataFragmentBuilder.toString() + me.getVrmDcClosingTag();

		me.setBody('readData', {
			namespace: 'pat',
			namespaces: {
				'pat': 'http://repositories.med.va.gov/pathways'
			},
			in0: {
				namespace: '',
				value: 'AppointmentsRead1'
			},
			in1: {
				namespace: '',
				value: cdataFragment
			},
			in2: {
				namespace: '',
				value: 'APPOINTMENTS_SINGLE_PATIENT_FILTER'
			},
			in3: {
				namespace: '',
				value: 'Appointments_ReqId_06_23_0000'
			}

		});
	}
});
