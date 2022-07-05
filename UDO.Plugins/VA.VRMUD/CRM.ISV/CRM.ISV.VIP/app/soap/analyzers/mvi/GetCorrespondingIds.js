Ext.define('VIP.soap.analyzers.mvi.GetCorrespondingIds', {
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
			resultSetRecords = me.getPatientIdRecords(patientNodes);

			resultSet = Ext.create('Ext.data.ResultSet', {
				records: resultSetRecords,
				count: resultSetRecords.length,
				success: (resultSetRecords.length > 0)
			});

			return resultSet;
		};

		return me;
	},

	getPatientIdRecords: function (patientNodes) {
		var idRecords = [];

		if (!Ext.isEmpty(patientNodes) && patientNodes.length > 0) {
			var idNodes = patientNodes[0].selectNodes('id'),
                    corModel = Ext.create('VIP.model.mvi.CorrespondingIds');

			if (!Ext.isEmpty(idNodes)) {
				for (var i = 0; i < idNodes.length; i++) {
					var idNode = idNodes[i],
						edipiRegex = /NI\^200DOD\^USDOD/g,
						edipiIsMatch = false,
						idExt = '';

					idExt = idNode.getAttribute('extension');

					if (Ext.isEmpty(idExt)) { continue; }

					edipiIsMatch = idExt.match(edipiRegex);

					if (edipiIsMatch) {
						corModel.set('fullEdipi', idExt);
						corModel.set('edipi', idExt.substring(0, idExt.indexOf('^')));
						continue;
					}
				}
				if (!Ext.isEmpty(corModel.get('edipi'))) {
					corModel.commit();
					idRecords.push(corModel);
				}
			}
		}

		return idRecords;
	}
});