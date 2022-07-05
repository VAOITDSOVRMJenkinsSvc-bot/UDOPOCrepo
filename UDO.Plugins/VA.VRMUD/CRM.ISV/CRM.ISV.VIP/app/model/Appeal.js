/**
* @author Josh Oliver
* @class VIP.model.Appeal
*
* The model for appeal record details
*/
Ext.define('VIP.model.Appeal', {
	extend: 'Ext.data.Model',
	requires: [
		'VIP.soap.envelopes.vacols.appeals.FindAppeals',
		'VIP.model.appeals.Detail'
	],
	fields: [
		{
			name: 'appealKey',
			type: 'string',
			mapping: 'AppealKey'
		},
		{
			name: 'appellantId',
			type: 'string',
			mapping: 'AppellantID'
        },
		{
			name: 'firstName',
			type: 'string',
			mapping: 'FirstName'
		},
		{
			name: 'middleInitial',
			type: 'string',
			mapping: 'MiddleInitial'
		},
		{
			name: 'lastName',
			type: 'string',
			mapping: 'LastName'
		},
		{
			name: 'fullName',
			convert: function (v, record) {
				if (!Ext.isEmpty(record)) {
					var s = record.get('lastName'), f = record.get('firstName'), m = record.get('middleInitial');
					if (!Ext.isEmpty(f)) { s += ', ' + f; }
					if (!Ext.isEmpty(m)) { s += ' ' + m; }
					return s;
				}
			}
        },
        {
            name: 'appealStatusCode',
            type: 'string',
            mapping: 'AppealStatusCode'
        },
		{
			name: 'appealStatusDescription',
			convert: function (v, record) {
				if (!Ext.isEmpty(record)) {
					return record.translateAppealStatusCode(record.get('appealStatusCode'));
				}
			}
		},
		{
			name: 'noticeOfDisagreementReceivedDate',
			type: 'date',
			mapping: 'NoticeOfDisagreementReceivedDate',
			dateFormat: 'Y-m-d'
        },
        {
            name: 'noticeOfDisagreementReceivedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('noticeOfDisagreementReceivedDate'))) {
                    return Ext.Date.format(record.get('noticeOfDisagreementReceivedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
			name: 'decisionDate',
			type: 'date',
			mapping: 'DecisionDate',
			dateFormat: 'Y-m-d'
		},
        {
            name: 'decisionDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('decisionDate'))) {
                    return Ext.Date.format(record.get('decisionDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
		{
			name: 'regionOfficeCode',
			type: 'string',
			mapping: 'RegionOfficeCode'
		},
		{
			name: 'regionOfficeDescription',
			type: 'string',
			mapping: 'RegionOfficeDescription'
		},
		{
			name: 'actionTypeCode',
			type: 'string',
			mapping: 'ActionTypeCode'
		},
		{
			name: 'actionTypeDescription',
			type: 'string',
			mapping: 'ActionTypeDescription'
		}
	],
	hasMany: [
		{
			model: 'VIP.model.appeals.Detail',
			name: 'details',
			storeConfig: {
				filters: [],
				remoteFilter: true
			}
		}
	],
	proxy: {
		type: 'soap',
		headers: {
			"SOAPAction": "",
			"Content-Type": "text/xml; charset=utf-8"
		},
		reader: {
			type: 'xml',
			record: 'AppealIdentifier'
		},
		envelopes: {
			create: '',
			read: 'VIP.soap.envelopes.vacols.appeals.FindAppeals',
			update: '',
			destroy: ''
		}
	},

	translateAppealStatusCode: function (statusCode) {
		var description = "";
		switch (statusCode) {
			case "ACT":
				description = "ACTIVE(Case at BVA)";
				break;
			case "ADV":
				description = "ADVANCE(NOD Appeal Filed and/or on Docket—Case in RO)";
				break;
			case "CAV":
				description = "CAVC(U.S. Court of Appeals for Veterans Claims Action pending -case in transit to BVA)";
				break;
			case "REM":
				description = "REMAND (Case has been Remanded to VBA)";
				break;
			case "HIS":
				description = "HISTORY (BVA action is complete)";
				break;
			case "MOT":
				description = "MOTion (A motion for reconsideration under § 20.1000 has been filed pursuant to a prior BVA decision)";
				break;
		};

		return description;
	}
});