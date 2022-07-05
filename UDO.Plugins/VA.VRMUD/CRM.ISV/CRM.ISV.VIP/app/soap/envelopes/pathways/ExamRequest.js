/*
TODO: replace CDATA with tags

this.CDATA1 = "![CDATA[", this.CDATA1Rep = "_vrm.dctag_";
this.CDATA2 = "]]", this.CDATA2Rep = "_vrm.dctag2_";
	innerXml = innerXml.replace(this.CDATA1, this.CDATA1Rep).replace(this.CDATA2, this.CDATA2Rep);

*/

Ext.define('VIP.soap.envelopes.pathways.ExamRequest', {
	extend: 'VIP.soap.envelopes.pathways.PathwaysTemplate',
	alias: 'envelopes.ExamRequest',
	uses: [
        'VIP.util.xml.FragmentBuilder'
    ],
	//    config: {//added these because the me.getStartDate() functions aren't available when debugging.
	//        nationalId: '',
	//        startDate: undefined,
	//        endDate: undefined
	//    },
	analyzeResponse: Ext.create('VIP.util.soap.WebserviceResponseAnalyzer'),

	constructor: function (config) {
		var me = this,
            clientRequestInitiationDate = Ext.Date.format(new Date(), 'Y-m-d'),
            clientRequestInitiationTime = Ext.Date.format(new Date(), 'H:i:s'),
            clientRequestInitiationDateTime,
		//startDate = Ext.Date.format(new Date(me.getStartDate()), 'Y-m-d'),
		//endDate = Ext.Date.format(new Date(me.getEndDate()), 'Y-m-d'),
            startDate = Ext.Date.format(config ? config.startDate : new Date(me.getStartDate()), 'Y-m-d'),
            endDate = Ext.Date.format(config ? config.endDate : new Date(me.getEndDate()), 'Y-m-d'),
            cdataFragment,
            requestFragmentObject,
            cdataFragmentBuilder;

		me.initConfig(config);

		me.callParent();
		if (!Ext.isDate(config ? config.startDate : new Date(me.getStartDate()))) {
			//if (!Ext.isDate(startDate)) { **Faulty logic - startDate is formatted to a string above
			startDate = Ext.Date.add(new Date(), Ext.Date.MONTH, -6);
			startDate = Ext.Date.format(new Date(startDate), 'Y-m-d');
		}
		if (!Ext.isDate(config ? config.endDate : new Date(me.getEndDate()))) {
			//if (!Ext.isDate(endDate)) {  **Faulty logic - endDate is formatted to a string above
			endDate = Ext.Date.add(new Date(), Ext.Date.MONTH, +6);
			endDate = Ext.Date.format(new Date(endDate), 'Y-m-d');
		}

		clientRequestInitiationDateTime = clientRequestInitiationDate + 'T' + clientRequestInitiationTime + 'Z';

		requestFragmentObject = {
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
					value: 'REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER'
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
						'queryName': 'Exam-Standardized'
					},
					domainEntryPoint: {
						namespace: '',
						value: 'Exam2507'
					},
					startDate: {
						namespace: '',
						value: startDate
					},
					endDate: {
						namespace: '',
						value: endDate
					}
				},
				'@entryPointFilter': {
					namespace: '',
					attributes: {
						'queryName': 'ExamRequests-Standardized'
					},
					domainEntryPoint: {
						namespace: '',
						value: 'ExamRequest2507'
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
			xmlFragment: requestFragmentObject,
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
				value: 'RequestsAndExamsRead1'
			},
			in1: {
				namespace: '',
				value: cdataFragment
			},
			in2: {
				namespace: '',
				value: 'REQUESTS_AND_EXAMS_SINGLE_PATIENT_FILTER'
			},
			in3: {
				namespace: '',
				value: 'Request196AndExams_ReqId_06_23_0000'
			}
		});
	}
});