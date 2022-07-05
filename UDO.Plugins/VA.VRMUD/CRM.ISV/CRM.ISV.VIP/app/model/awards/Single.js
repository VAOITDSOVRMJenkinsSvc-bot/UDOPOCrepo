/**
* @author Ivan Yurisevic
* @class VIP.model.award.Single
*
* The model for Awards record details for a single award.  NOTE THIS HAS A SOAP PROXY, NOT A MEMORY PROXY.
* It also calls FindGeneralInformationByPtcpntIds NOT ByFileNumber
*/
Ext.define('VIP.model.awards.Single', {
	extend: 'Ext.data.Model',
	requires: [
		'VIP.soap.envelopes.share.claimant.FindGeneralInformationByPtcpntIds', //SOAP
		'VIP.data.reader.AwardSingle', //Reader
		'VIP.model.awards.single.Diaries',
		'VIP.model.awards.single.Evrs',
		'VIP.model.awards.single.Flashes'
	],
	fields: [{
		name: 'additionalServiceIndicator',
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
		name: 'convertedCaseIndicator',
		mapping: 'convertedCaseIndicator',
		type: 'string'
	}, {
		name: 'currentMonthlyRate',
		mapping: 'currentMonthlyRate',
		convert: toCurrency
	}, {
		name: 'desertShieldIndicator',
		mapping: 'desertShieldIndicator',
		type: 'string'
	}, {
		name: 'directDepositAccountID',
		mapping: 'directDepositAccountID',
		type: 'string'
	}, {
		name: 'enteredOnDutyDate',
		mapping: 'enteredOnDutyDate',
		type: 'string'
	}, {
		name: 'fiduciaryDecisionTypeCode',
		mapping: 'fiduciaryDecisionTypeCode',
		type: 'string'
	}, {
		name: 'fiduciaryDecisionTypeName',
		mapping: 'fiduciaryDecisionTypeName',
		type: 'string'
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
		type: 'int'
	}, {
		name: 'numberOfDiaries',
		mapping: 'numberOfDiaries',
		type: 'int'
	}, {
		name: 'numberOfEvrs',
		mapping: 'numberOfEvrs',
		type: 'int'
	}, {
		name: 'numberOfFlashes',
		mapping: 'numberOfFlashes',
		type: 'int'
	}, {
		name: 'nursingHomeIndicator',
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
		dateFormat: 'mdY'
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
		dateFormat: 'mdY'
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
		name: 'payeeSSN',
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
		name: 'participantBeneId',
		mapping: 'ptcpntBeneID',
		type: 'string'
	}, {
		name: 'participantRecipientId',
		mapping: 'ptcpntRecipID',
		type: 'string'
	}, {
		name: 'participantVetId',
		mapping: 'ptcpntVetID',
		type: 'string'
	}, {
		name: 'releasedActiveDutyDate',
		mapping: 'releasedActiveDutyDate',
		type: 'string'
	}, {
		name: 'stationOfJurisdiction',
		mapping: 'stationOfJurisdiction',
		type: 'string'
	}, {
		name: 'statusReasonDate',
		mapping: 'statusReasonDate',
		type: 'date',
		dateFormat: 'mdY'
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
		name: 'vetBirthDate',
		mapping: 'vetBirthDate',
		type: 'string'
	}, {
		name: 'vetDeathDate',
		mapping: 'vetDeathDate',
		type: 'string'
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
		name: 'vetSSN',
		mapping: 'vetSSN',
		type: 'string'
	}, {
		name: 'vetSex',
		mapping: 'vetSex',
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
		model: 'VIP.model.awards.single.Diaries',
		name: 'diaries',
		associationKey: 'diary'
	}, {
		model: 'VIP.model.awards.single.Evrs',
		name: 'evrs',
		associationKey: 'evr'
	}, {
		model: 'VIP.model.awards.single.Flashes',
		name: 'flashes',
		associationKey: 'flash'
	}],
	//Start Soap definition
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

function toCurrency(v, record) {
	return Ext.util.Format.usMoney(v);
}