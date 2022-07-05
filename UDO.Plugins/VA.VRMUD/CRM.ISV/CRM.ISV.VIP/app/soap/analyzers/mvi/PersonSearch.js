Ext.define('VIP.soap.analyzers.mvi.PersonSearch', {
	extend: 'VIP.util.soap.WebserviceResponseAnalyzer',
	constructor: function (config) {
		var me = this;

		me.initConfig(config);

		me.customReadRecords = function(response, currentResultSet, reader) {
			var resultSet,
			    resultSetRecords;

			if (Ext.isEmpty(response)) {
				return currentResultSet;
			}

			var patientNodes = response.selectNodes('//patient');

			if (Ext.isEmpty(patientNodes)) {
				return currentResultSet;
			}
			resultSetRecords = me.getPatientMviRecords(patientNodes);

			resultSet = Ext.create('Ext.data.ResultSet', {
				records: resultSetRecords,
				count: resultSetRecords.length,
				success: (resultSetRecords.length > 0)
			});

			return resultSet;
		};

		return me;
	},

	getPatientMviRecords: function (patientNodes) {
		var me = this,
                patientRecords = [];

		for (var p = 0; p < patientNodes.length; p++) {
			var patientNode = patientNodes[p],
                    patientModel = Ext.create('VIP.model.mvi.Patient'),
					idNodes = patientNode.selectNodes('id'),
					nameNode = patientNode.selectSingleNode('patientPerson/name');

			if (!Ext.isEmpty(nameNode)) {
				patientModel.set('name', me.getPatientName(nameNode));
			}

			patientModel.set('gender',
                        patientNode.selectSingleNode('patientPerson/administrativeGenderCode') ?
                        patientNode.selectSingleNode('patientPerson/administrativeGenderCode').getAttribute('code') : ''
                    );

			patientModel.set('dob',
                        patientNode.selectSingleNode('patientPerson/birthTime') ?
                        patientNode.selectSingleNode('patientPerson/birthTime').getAttribute('value') : ''
                    );

			patientModel.set('city',
                        patientNode.selectSingleNode('patientPerson/birthPlace/addr/city') ?
                        patientNode.selectSingleNode('patientPerson/birthPlace/addr/city').text : ''
                    );

			patientModel.set('state',
                        patientNode.selectSingleNode('patientPerson/birthPlace/addr/state') ?
                        patientNode.selectSingleNode('patientPerson/birthPlace/addr/state').text : ''
                    );

			patientModel.set('country',
                        patientNode.selectSingleNode('patientPerson/birthPlace/addr/country') ?
                        patientNode.selectSingleNode('patientPerson/birthPlace/addr/country').text : ''
                    );

			if (idNodes && idNodes != undefined) {
				for (var i = 0; i < idNodes.length; i++) {
					var idNode = idNodes[i],
						nationalIdRegex = /NI\^200M\^USVHA/g,
						nationalIdIsMatch = false,
						ssnRegex = /SS/g,
						ssnIsMatch = false,
						idExt = '';

					idExt = idNode.getAttribute('extension');

					if (!idExt || idExt == undefined || idExt == '') { continue; }

					nationalIdIsMatch = idExt.match(nationalIdRegex);
					ssnIsMatch = idExt.match(ssnRegex);

					if (nationalIdIsMatch) {
						patientModel.set('fullNationalId', idExt.substring(0, idExt.lastIndexOf('^')));
						patientModel.set('nationalId', idExt.substring(0, idExt.indexOf('^')));
						continue;
					}

					if (ssnIsMatch) {
						patientModel.set('fullSsn', idExt.substring(0, idExt.lastIndexOf('^')));
						patientModel.set('ssn', idExt.substring(0, idExt.indexOf('^')));
						continue;
					}
				}
			}

			if (!Ext.isEmpty(patientModel.get('nationalId'))) {
				patientModel.commit();
				patientRecords.push(patientModel);
			}
		}

		return patientRecords;
	},

	getPatientName: function (nameNode) {
		if (nameNode && nameNode != undefined) {
			var fullName = '',
			                    givenNodes = nameNode.selectNodes('given');

			for (var g = 0; g < givenNodes.length; g++) {
				var givenNode = givenNodes[g];

				fullName += givenNode ? givenNode.text : '';
				if ((g + 1) < givenNodes.length) { fullName += ' '; }
			}

			fullName += nameNode.selectSingleNode('family') ? ' ' + nameNode.selectSingleNode('family').text : '';
			fullName += nameNode.selectSingleNode('suffix') ? ' ' + nameNode.selectSingleNode('suffix').text : '';

			return fullName;
		}

		return '';
	}
});