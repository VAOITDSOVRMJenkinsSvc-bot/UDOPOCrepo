/**
* @author Dmitri Riz
* @class VIP.model.personinfo.AllRelationships
*
* The model for AllRelationships associated with the person
*/
Ext.define('VIP.model.personinfo.AllRelationships', {
	extend: 'Ext.data.Model',
	requires: ['VIP.soap.envelopes.share.claimant.FindAllRelationships'],
	fields: [{
		name: 'awardBeginDate',
		type: 'date',
		dateFormat: 'mdY',
		mapping: 'awardBeginDate'
	}, {
		name: 'awardEndDate',
		type: 'date',
		dateFormat: 'mdY',
		mapping: 'awardEndDate'
	}, {
		name: 'awardInd',
		type: 'string',
		mapping: 'awardInd'
	}, {
		name: 'awardType',
		type: 'string',
		mapping: 'awardType'
	}, {
		name: 'dob', //'dateOfBirth',
		mapping: 'dateOfBirth',
		type: 'date',
		dateFormat: 'mdY'
	}, {
		name: 'dod', //'dateOfDeath',
		mapping: 'dateOfDeath',
		type: 'date',
		dateFormat: 'mdY'
	}, {
		name: 'dependentReason',
		type: 'string',
		mapping: 'dependentReason'
	}, {
		name: 'dependentTerminateDate',
		type: 'date',
		dateFormat: 'mdY',
		mapping: 'dependentTerminateDate'
	}, {
		name: 'emailAddress',
		type: 'string',
		mapping: 'emailAddress'
	}, {
		name: 'fiduciary',
		type: 'string',
		mapping: 'fiduciary'
	}, {
		name: 'fileNumber',
		type: 'string',
		mapping: 'fileNumber'
	}, {
		name: 'firstName',
		type: 'string',
		mapping: 'firstName'
	}, {
		name: 'gender',
		type: 'string',
		mapping: 'gender'
	}, {
		name: 'lastName',
		type: 'string',
		mapping: 'lastName'
	}, {
		name: 'middleName',
		type: 'string',
		mapping: 'middleName'
	}, {
		name: 'fullName',
		mapping: 'fullName',
		convert: function (v, record) {
			var s = record.data.lastName;
			if (!Ext.isEmpty(record.data.firstName)) s += ', ' + record.data.firstName;
			if (!Ext.isEmpty(record.data.middleName)) s += ' ' + record.data.middleName;
			return s;
		}
	}, {
		name: 'poa',
		type: 'string',
		mapping: 'poa'
	}, {
		name: 'proofOfDependecyInd',
		type: 'string',
		mapping: 'proofOfDependecyInd'
	}, {
		name: 'participantId', // name: 'ptcpntId',
		type: 'string',
		mapping: 'ptcpntId'
	}, {
		name: 'relationshipBeginDate',
		type: 'date',
		dateFormat: 'mdY',
		mapping: 'relationshipBeginDate'
	}, {
		name: 'relationshipEndDate',
		type: 'date',
		dateFormat: 'mdY',
		mapping: 'relationshipEndDate'
	}, {
		name: 'relationshipType',
		type: 'string',
		mapping: 'relationshipType'
	}, {
		name: 'ssn',
		type: 'string',
		mapping: 'ssn'
	}, {
		name: 'ssnVerifiedInd',
		mapping: 'ssnVerifiedInd',
		convert: function (v, record) {
			if (!record.get('ssnVerifiedInd') || record.get('ssnVerifiedInd').length != 1) {
				return record.get('ssnVerifiedInd');
			}
			return record.get('ssnVerifiedInd') == '1' ? 'Y' : 'N';
		}
	}, {
		name: 'terminateReason',
		type: 'string',
		mapping: 'terminateReason'
	}],
	proxy: {
		type: 'soap',
		headers: {
			"SOAPAction": "",
			"Content-Type": "text/xml; charset=utf-8"
		},
		reader: {
			type: 'xml',
			record: 'dependents'
		},
		envelopes: {
			create: '',
			read: 'VIP.soap.envelopes.share.claimant.FindAllRelationships',
			update: '',
			destroy: ''
		}
	}
});