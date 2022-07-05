/**
* @author Ivan Yurisevic
* @class VIP.model.personinfo.GeneralDetails
*
* The model for person general detail information
*/
Ext.define('VIP.model.personinfo.GeneralDetails', {
	extend: 'Ext.data.Model',
	requires: [
		'VIP.soap.envelopes.share.claimant.FindGeneralInformationByPtcpntIds',
		'VIP.data.reader.AwardSingle', //Reader
		'VIP.model.personinfo.Flashes'
	],
	fields: [{
		name: 'additionalServiceInd',
		mapping: 'additionalServiceIndicator',
		type: 'string'
	}, {
		name: 'awardTypeCode',
		mapping: 'awardTypeCode',
		type: 'string'
	}, {
		name: 'benefitTypeCode',
		mapping: 'benefitTypeCode',
		type: 'string'
	}, {
		name: 'benefitTypeName',
		mapping: 'benefitTypeName',
		type: 'string'
	}, {
		name: 'clothingAllowanceTypeCode',
		mapping: 'clothingAllowanceTypeCode',
		type: 'string'
	}, {
		name: 'clothingAllowanceTypeName',
		mapping: 'clothingAllowanceTypeName',
		type: 'string'
	}, {
		name: 'competencyDecisionTypeCode',
		mapping: 'competencyDecisionTypeCode',
		type: 'string'
	}, {
		name: 'competencyDecisionTypeName',
		mapping: 'competencyDecisionTypeName',
		type: 'string'
	}, {
		name: 'convertedCaseInd',
		mapping: 'convertedCaseIndicator',
		type: 'string'
	}, {
		name: 'currentMonthlyRate',
		mapping: 'currentMonthlyRate',
		type: 'string'
	}, {
		name: 'desertShieldInd',
		mapping: 'desertShieldIndicator',
		type: 'string'
	}, {
		name: 'directDepositAccountID',
		mapping: 'directDepositAccountID',
		type: 'string'
	}, {
		name: 'enteredOnDutyDate',
		mapping: 'enteredOnDutyDate',
		type: 'date',
		dateFormat: 'mdY' // <enteredOnDutyDate>03171988</enteredOnDutyDate>
	}, {
		name: 'enteredOnDutyDate_F',
		convert: function (v, record) {
			if (!Ext.isEmpty(record.get('enteredOnDutyDate'))) {
				return Ext.Date.format(record.get('enteredOnDutyDate'), "m/d/Y");
			} else return '';
		}
	}, {
		name: 'fiduciaryDecisionTypeCode',
		mapping: 'fiduciaryDecisionTypeCode',
		type: 'string'
	}, {
		name: 'fiduciaryDecisionTypeName',
		mapping: 'fiduciaryDecisionTypeName',
		type: 'string'
	}, {
		name: 'fiduciaryCategory',
		convert: function (v, record) {
			var code = record.get('fiduciaryDecisionTypeCode'),
				name = record.get('fiduciaryDecisionTypeName');
			if (!Ext.isEmpty(code)) {
				code = code + ' - ';
			} else code = '';
			return code + name;
		}
	}, {
		name: 'fundsDueIncompetentBalance',
		mapping: 'fundsDueIncompetentBalance',
		type: 'string'
	}, {
		name: 'guardianFolderLocation',
		mapping: 'guardianFolderLocation',
		type: 'string'
	}, {
		name: 'gulfWarRegistryIndicator',
		mapping: 'gulfWarRegistryIndicator',
		type: 'string'
	}, {
		name: 'mailingAddressID',
		mapping: 'mailingAddressID',
		type: 'string'
	}, {
		name: 'militaryBranch',
		mapping: 'militaryBranch',
		type: 'string'
	}, {
		name: 'numberOfAwardBenes',
		mapping: 'numberOfAwardBenes',
		type: 'string'
	}, {
		name: 'numberOfDiaries',
		mapping: 'numberOfDiaries',
		type: 'string'
	}, {
		name: 'numberOfEvrs',
		mapping: 'numberOfEvrs',
		type: 'string'
	}, {
		name: 'numberOfFlashes',
		mapping: 'numberOfFlashes',
		type: 'string'
	}, {
		name: 'nursingHomeInd',
		mapping: 'nursingHomeIndicator',
		type: 'string'
	}, {
		name: 'nursingHomeName',
		mapping: 'nursingHomeName',
		type: 'string'
	}, {
		name: 'paidThroughDate',
		mapping: 'paidThroughDate',
		type: 'date',
		dateFormat: 'mdY'// <paidThroughDate>07312011</paidThroughDate>
	}, {
		name: 'paidThroughDate_F',
		convert: function (v, record) {
			if (!Ext.isEmpty(record.get('paidThroughDate'))) {
				return Ext.Date.format(record.get('paidThroughDate'), "m/d/Y");
			} else return '';
		}
	}, {
		name: 'paraplegicHousingNumber',
		mapping: 'paraplegicHousingNumber',
		type: 'string'
	}, {
		name: 'payStatusTypeCode',
		mapping: 'payStatusTypeCode',
		type: 'string'
	}, {
		name: 'payStatusTypeName',
		mapping: 'payStatusTypeName',
		type: 'string'
	}, {
		name: 'payeeBirthDate',
		mapping: 'payeeBirthDate',
		type: 'date',
		dateFormat: 'mdY'//<payeeBirthDate>03171968</payeeBirthDate>
	}, {
		name: 'payeeBirthDate_F',
		convert: function (v, record) {
			if (!Ext.isEmpty(record.get('payeeBirthDate'))) {
				return Ext.Date.format(record.get('payeeBirthDate'), "m/d/Y");
			} else return '';
		}
	}, {
		name: 'payeeName',
		mapping: 'payeeName',
		type: 'string'
	}, {
		name: 'payeeSsn',
		mapping: 'payeeSSN',
		type: 'string'
	}, {
		name: 'payeeSex',
		mapping: 'payeeSex',
		type: 'string'
	}, {
		name: 'payeeTypeCode',
		mapping: 'payeeTypeCode',
		type: 'string'
	}, {
		name: 'payeeTypeIndicator',
		mapping: 'payeeTypeIndicator',
		type: 'string'
	}, {
		name: 'payeeTypeName',
		mapping: 'payeeTypeName',
		type: 'string'
	}, {
		name: 'paymentAddressID',
		mapping: 'paymentAddressID',
		type: 'string'
	}, {
		name: 'personalFundsOfPatientBalance',
		mapping: 'personalFundsOfPatientBalance',
		type: 'string'
	}, {
		name: 'powerOfAttorney',
		mapping: 'powerOfAttorney',
		type: 'string'
	}, {
		name: 'releasedActiveDutyDate',
		mapping: 'releasedActiveDutyDate',
		type: 'date',
		dateFormat: 'mdY'//<releasedActiveDutyDate>03171993</releasedActiveDutyDate>
	}, {
		name: 'releasedActiveDutyDate_F',
		convert: function (v, record) {
			if (!Ext.isEmpty(record.get('releasedActiveDutyDate'))) {
				return Ext.Date.format(record.get('releasedActiveDutyDate'), "m/d/Y");
			} else return '';
		}
	}, {
		name: 'statusReasonDate',
		mapping: 'statusReasonDate',
		type: 'date',
		dateFormat: 'mdY'//<statusReasonDate>08012011</statusReasonDate>
	}, {
		name: 'statusReasonDate_F',
		convert: function (v, record) {
			if (!Ext.isEmpty(record.get('statusReasonDate'))) {
				return Ext.Date.format(record.get('statusReasonDate'), "m/d/Y");
			} else return '';
		}
	}, {
		name: 'statusReasonTypeCode',
		mapping: 'statusReasonTypeCode',
		type: 'string'
	}, {
		name: 'statusReasonTypeName',
		mapping: 'statusReasonTypeName',
		type: 'string'
	}, {
		name: 'stationOfJurisdiction',
		mapping: 'stationOfJurisdiction',
		type: 'string'
	}, {
		name: 'vetBirthDate',
		mapping: 'vetBirthDate',
		type: 'date',
		dateFormat: 'mdY'//<vetBirthDate>03171968</vetBirthDate>
	}, {
		name: 'vetBirthDate_F',
		convert: function (v, record) {
			if (!Ext.isEmpty(record.get('vetBirthDate'))) {
				return Ext.Date.format(record.get('vetBirthDate'), "m/d/Y");
			} else return '';
		}
	}, {
		name: 'vetDeathDate',
		mapping: 'vetDeathDate',
		type: 'date',
		dateFormat: 'mdY'//<vetDeathDate>04021599</vetDeathDate>
	}, {
		name: 'vetDeathDate_F',
		convert: function (v, record) {
			if (!Ext.isEmpty(record.get('vetDeathDate'))) {
				return Ext.Date.format(record.get('vetDeathDate'), "m/d/Y");
			} else return '';
		}
	}, {
		name: 'vetFirstName',
		mapping: 'vetFirstName',
		type: 'string'
	}, {
		name: 'vetLastName',
		mapping: 'vetLastName',
		type: 'string'
	}, {
		name: 'vetMiddleName',
		mapping: 'vetMiddleName',
		type: 'string'
	}, {
		name: 'vetSsn',
		mapping: 'vetSSN',
		type: 'string'
	}, {
		name: 'vetSex',
		mapping: 'vetSex',
		type: 'string'
	}, {
		name: 'participantVetID',
		mapping: 'ptcpntVetID',
		type: 'string'
	}, {
		name: 'participantBeneID',
		mapping: 'ptcpntBeneID',
		type: 'string'
	}, {
		name: 'participantRecipID',
		mapping: 'ptcpntRecipID',
		type: 'string'
	}, {
		name: 'competency',
		convert: function (v, record) {
			var competencyStr = '';

			if (!Ext.isEmpty(record.get('competencyDecisionTypeName'))) {
				competencyStr += record.get('competencyDecisionTypeName');
			}

			if (!Ext.isEmpty(record.get('competencyDecisionTypeCode'))) {
				competencyStr += ' (' + record.get('competencyDecisionTypeCode') + ') ';
			}

			return competencyStr;
		}
	}, {
		name: 'payeeType',
		convert: function (v, record) {
			var payeeTypeStr = '';

			if (!Ext.isEmpty(record.get('payStatusTypeName'))) {
				payeeTypeStr += record.get('payStatusTypeName');
			}

			if (!Ext.isEmpty(record.get('payStatusTypeCode'))) {
				payeeTypeStr += ' (' + record.get('payStatusTypeCode') + ') ';
			}

			return payeeTypeStr;
		}
	}],

	//Start Associations:
	hasMany: [{
		model: 'VIP.model.personinfo.Flashes',
		name: 'flashes',
		associationKey: 'flash'
	}],

	//Start Proxy
	proxy: {
		type: 'soap',
		headers: {
			"SOAPAction": "",
			"Content-Type": "text/xml; charset=utf-8"
		},
		reader: {
			type: 'awardsingle',
			record: 'return'
		},
		envelopes: {
			create: '',
			read: 'VIP.soap.envelopes.share.claimant.FindGeneralInformationByPtcpntIds',
			update: '',
			destroy: ''
		}
	}

});