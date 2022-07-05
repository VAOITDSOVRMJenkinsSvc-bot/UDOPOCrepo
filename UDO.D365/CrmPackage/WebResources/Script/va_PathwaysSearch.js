var pathwaysSearch = function (context) {
	this.context = context;
	this.actionMessage = new actionsMessage();
	this.actionMessage.name = 'pathwaysSearch';
	this.actionMessage.documentElement = 'actionsMessage';
	this.actionMessage.stackTrace = '';
	this.webservices = new Array();
	this.analyzers = new Array();
	this.performAction = pathwaysSearch_performAction;
	this.analyzeExamRequestResult = analyzeExamRequestResult;
	this.analyzeAppointmentResult = analyzeAppointmentResult;
	this.analyzePersonSearchResult = analyzePersonSearchResult;
}
pathwaysSearch.prototype = new search;
pathwaysSearch.prototype.constructor = pathwaysSearch;
var pathwaysSearch_performAction = function () {
	this.context.stackTrace = 'performAction();';
	this.hasErrors = false;

	this.webservices['personSearch'] = new personSearch(this.context);
	this.analyzers['personSearch'] = this.analyzePersonSearchResult;

//	this.webservices['readDataExamRequest'] = new readDataExamRequest(this.context);
//	this.analyzers['readDataExamRequest'] = this.analyzeExamRequestResult;

//	this.webservices['readDataAppointment'] = new readDataAppointment(this.context);
//	this.analyzers['readDataAppointment'] = this.analyzeAppointmentResult;

	this.executeSearchOperations(this.webservices);

	UpdateSearchListObject({ name: 'pathwaysSearch', complete: !this.hasErrors });

	return !this.hasErrors;
}
var analyzeExamRequestResult = function (parentObject) {
	var examXml = Xrm.Page.getAttribute('va_readdataexamresponse').getValue();

	if (examXml && examXml != undefined && examXml != '') {
		var examXmlObject = _XML_UTIL.parseXmlObject(examXml);

		if (examXmlObject && examXmlObject != undefined && examXmlObject.xml && examXmlObject.xml != undefined && examXmlObject.xml != '') {
			var tempXml = examXmlObject;

			if (SingleNodeExists(tempXml, '//out')) {
				var cdataText = tempXml.selectSingleNode('//out').text;

				if (cdataText && cdataText != undefined && cdataText != '') {
					Xrm.Page.getAttribute('va_readdataexamresponse').setValue(cdataText);
				}
			}
		}
	}

	return;
}
var analyzeAppointmentResult = function (parentObject) {
	var appXml = Xrm.Page.getAttribute('va_readdataappointmentresponse').getValue();

	if (appXml && appXml != undefined && appXml != '') {
		var appXmlObject = _XML_UTIL.parseXmlObject(appXml);

		if (appXmlObject && appXmlObject != undefined && appXmlObject.xml && appXmlObject.xml != undefined && appXmlObject.xml != '') {
			var tempXml = appXmlObject;

			if (SingleNodeExists(tempXml, '//out')) {
				var cdataText = tempXml.selectSingleNode('//out').text;

				if (cdataText && cdataText != undefined && cdataText != '') {
					Xrm.Page.getAttribute('va_readdataappointmentresponse').setValue(cdataText);
				}
			}
		}
	}

	return;
}
var analyzePersonSearchResult = function (parentObject) {
	var mviXml = Xrm.Page.getAttribute('va_mviresponse').getValue();
	var simplifiedMviXml = '<People>';
	var natId;
	
	parentObject.context.parameters['nationalId'] = null;
	var mviSearchObject = null;

	if (mviXml && mviXml != undefined && mviXml != '') {
		var mviXmlObject = _XML_UTIL.parseXmlObject(mviXml);

		if (mviXmlObject && mviXmlObject != undefined && mviXmlObject.xml && mviXmlObject.xml != undefined && mviXmlObject.xml != '') {

			var patientNodes = mviXmlObject.selectNodes('//patient');
			mviSearchObject = {
				People: []
			};

			if (patientNodes && patientNodes != undefined) {
				for (var p = 0; p < patientNodes.length; p++) {
					var patientNode = patientNodes[p],
						person = {
							nationalId: '',
							ssn: '',
							name: '',
							dob: '',
							gender: '',
							city: '',
							state: '',
							country: ''
						},
						idNodes = patientNode.selectNodes('id'),
						nameNodes = patientNode.selectNodes('patientPerson/name');

					if (nameNodes && nameNodes != undefined) {
						for (var n = 0; n < nameNodes.length; n++) {
							nameNode = nameNodes[n];
							person.name = getPatientName(nameNode);
						}
					}

					person.gender = patientNode.selectSingleNode('patientPerson/administrativeGenderCode') ?
							patientNode.selectSingleNode('patientPerson/administrativeGenderCode').getAttribute('code') : '';

					person.dob = patientNode.selectSingleNode('patientPerson/birthTime') ?
							patientNode.selectSingleNode('patientPerson/birthTime').getAttribute('value') : '';

					person.city = patientNode.selectSingleNode('patientPerson/birthPlace/addr/city') ?
							patientNode.selectSingleNode('patientPerson/birthPlace/addr/city').text : '';
					person.state = patientNode.selectSingleNode('patientPerson/birthPlace/addr/state') ?
							patientNode.selectSingleNode('patientPerson/birthPlace/addr/state').text : '';
					person.country = patientNode.selectSingleNode('patientPerson/birthPlace/addr/country') ?
							patientNode.selectSingleNode('patientPerson/birthPlace/addr/country').text : '';

					if (idNodes && idNodes != undefined) {
						for (var i = 0; i < idNodes.length; i++) {
							var idNode = idNodes[i],
									nationalIdRegex = /NI|USVHA*/g,
									nationalIdIsMatch = false,
									ssnRegex = /SS/g,
									ssnIsMatch = false,
									idExt = '';

							idExt = idNode.getAttribute('extension');

							if (!idExt || idExt == undefined || idExt == '') { continue; }

							nationalIdIsMatch = idExt.match(nationalIdRegex);
							ssnIsMatch = idExt.match(ssnRegex);

							if (nationalIdIsMatch) {
								natId = idExt.substring(0, idExt.indexOf('^'))
								person.nationalId = natId;
								continue;
							}

							if (ssnIsMatch) {
								person.ssn = idExt.substring(0, idExt.indexOf('^'));
								continue;
							}
						}
					}

					if (person && person.nationalId && person.nationalId != undefined && person.nationalId != '') {
						mviSearchObject.People.push(person);
						simplifiedMviXml = simplifiedMviXml + '<person><nationalId>' + person.nationalId +
							'</nationalId><ssn>' + person.ssn +
							'</ssn><name>' + person.name +
							'</name><dob>' + person.dob +
							'</dob><gender>' + person.gender +
							'</gender><city>' + person.city +
							'</city><state>' + person.state +
							'</state><country>' + person.country +
							'</country></person>';
					}
				}


			}
			else {
				var warningMessage = new actionsMessage();
				warningMessage.warningFlag = true;
				warningMessage.description = 'MVI Person Search did not return ICN for current individual.';
				warningMessage.pushMessage();
				parentObject.context.endState = true;
				parentObject.hasErrors = false;
				return;
			}

//			if (natId == null) {
//				var warningMessage = new actionsMessage();
//				warningMessage.warningFlag = true;
//				warningMessage.description = 'MVI Person Search did not return ICN for current individual.';
//				warningMessage.pushMessage();
//				parentObject.context.endState = true;
//				parentObject.hasErrors = false;
//				return;
//			}
			debugger;
			//mviXml = '<People><person><nationalId>1012658784V776129</nationalId><ssn>555880400</ssn><name>JOHN VRMJONES</name><dob>19700101</dob><gender>M</gender><city>Arlington</city><state>VA</state><country>USA</country></person><person><nationalId>1012658792V574200</nationalId><ssn>555880405</ssn><name>JOHN J VRMJONES I</name><dob>19700101</dob><gender>M</gender><city>Arlington</city><state>VA</state><country>USA</country></person></People>';
			simplifiedMviXml = simplifiedMviXml + '</People>';

			//mviXml = convertToXmlString('People', mviSearchObject);
			Xrm.Page.getAttribute('va_mviresponse').setValue(simplifiedMviXml);
			natId = null;
			Xrm.Page.getAttribute('va_icn').setValue(natId);

			// set natl id in context if single response
			// continue pathways search if single ID
			if (mviSearchObject && mviSearchObject.People && mviSearchObject.People.length == 1) {
				natId = mviSearchObject.People[0].nationalId;
				parentObject.context.parameters['nationalId'] = natId;

				parentObject.webservices['readDataExamRequest'] = new readDataExamRequest(parentObject.context);
				parentObject.analyzers['readDataExamRequest'] = parentObject.analyzeExamRequestResult;

				parentObject.webservices['readDataAppointment'] = new readDataAppointment(parentObject.context);
				parentObject.analyzers['readDataAppointment'] = parentObject.analyzeAppointmentResult;

				parentObject.webservices['readDataExamRequest'].context = parentObject.context;
				parentObject.webservices['readDataAppointment'].context = parentObject.context;
			} 
		}
	}

	return;
}

function getPatientName(nameNode) {
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

function convertToXmlString(rootNodeName, objectToConvert) {
	var xmlDoc,
		xmlEl,
		xmlText = '';

	if (rootNodeName && rootNodeName != undefined && rootNodeName != '') {
		xmlDoc = parseXmlObject('<' + rootNodeName + '/>');
		objectToConvert.documentElement = rootNodeName;
	}

	if (!xmlDoc || xmlDoc == undefined) { return ''; }

	for (propertyName in objectToConvert) {
		if (!(objectToConvert[propertyName] instanceof Function)) {
			if (propertyName != 'documentElement') {
				
				xmlEl = xmlDoc.createElement(propertyName);
				if (objectToConvert[propertyName] instanceof Array) {
					for (var i in objectToConvert[propertyName]) {
						var currentPropertyName = propertyName;
						xmlText += convertToXmlString('person', objectToConvert[propertyName][i]).toString();
						propertyName = currentPropertyName;
					}
					xmlText = xmlDoc.createTextNode(xmlText);
				}
				else {
					xmlText = xmlDoc.createTextNode(objectToConvert[propertyName]);
				}

				xmlEl.appendChild(xmlText);

				xmlDoc = xmlDoc.getElementsByTagName(objectToConvert.documentElement)[0];
				xmlDoc.appendChild(xmlEl);
				xmlDoc = parseXmlObject(xmlDoc.xml);
			}
		}
	}
	return xmlDoc.xml;
}