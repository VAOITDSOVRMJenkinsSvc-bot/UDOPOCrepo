﻿var corpMinSearch = function (context) {
	this.context = context;
	this.actionMessage = new actionsMessage();
	this.actionMessage.name = 'corpMinSearch';
	this.actionMessage.documentElement = 'actionsMessage';
	this.actionMessage.stackTrace = '';
	this.webservices = new Array();
	this.analyzers = new Array();
	this.performAction = corpMinSearch_performAction;
	this.analyzeFindCorporateRecordResult = analyzeFindCorporateRecordResult;
	this.analyzeFindVetByPtcpntIdResult = analyzeFindVetByPtcpntIdResult;
	this.analyzeFindBirlsRecordResult = analyzeFindBirlsRecordResult;
}
corpMinSearch.prototype = new search;
corpMinSearch.prototype.constructor = corpMinSearch;
var corpMinSearch_performAction = function () {
	this.context.stackTrace = 'performAction();';
	this.hasErrors = false;
	var fileNumber = Xrm.Page.getAttribute('va_ssn').getValue();
	var participantId = Xrm.Page.getAttribute('va_participantid').getValue();

	var searchOption = 1;

    
	if (Xrm.Page.data.entity.getEntityName() == 'contact') { //contact screen
		if (participantId) {
			searchOption = 2; // search by pid
		} else {
			searchOption = 1; //search by filenumber
		}

	}
	else {
		searchOption = Xrm.Page.getAttribute('va_searchtype').getValue();
	}
	switch (parseInt(searchOption)) {
		case 2:
			// SEARCH BY: PID
			if (participantId) {
			    if (Xrm.Page.data.entity.getEntityName() != 'contact' ) {
					Xrm.Page.getAttribute('va_ssn').setValue(null);
					Xrm.Page.getAttribute('va_firstname').setValue(null);
					Xrm.Page.getAttribute('va_lastname').setValue(null);
				}
				this.context.parameters['ptcpntId'] = participantId;
				this.webservices['findVeteranByPtcpntId'] = new findVeteranByPtcpntId(this.context);
				this.analyzers['findVeteranByPtcpntId'] = this.analyzeFindVetByPtcpntIdResult;
			}
			else {
				alert('Participant ID field is blank.');
				return;
			}
			this.webservices['findBirlsRecordByFileNumber'] = new findBirlsRecordByFileNumber(this.context);
			this.analyzers['findBirlsRecordByFileNumber'] = this.analyzeFindBirlsRecordResult;

			this.webservices['findCorporateRecordByFileNumber'] = new findCorporateRecordByFileNumber(this.context);
			this.analyzers['findCorporateRecordByFileNumber'] = this.analyzeFindCorporateRecordResult;
			this.webservices['findDependents'] = new findDependents(this.context);
			this.webservices['findAllRelationships'] = new findAllRelationships(this.context);
			this.webservices['findAllPtcpntAddrsByPtcpntId'] = new findAllPtcpntAddrsByPtcpntId(this.context);

			this.webservices['findAllFiduciaryPoa'] = new findAllFiduciaryPoa(this.context);

			break;
		case 3:
			// SEARCH BY: EDIPI
			alert('Connection to the authorization store has not yet been granted. Please come back later.');
			return;
		case 1:
		default:
			// SEARCH BY: SSN + other params
		    
		    if (Xrm.Page.data.entity.getEntityName() != 'contact' ) {
				Xrm.Page.getAttribute('va_participantid').setValue(null);
				Xrm.Page.getAttribute('va_edipi').setValue(null);
			}
			if (fileNumber) {
				this.context.parameters['fileNumber'] = fileNumber;

				this.webservices['findBirlsRecordByFileNumber'] = new findBirlsRecordByFileNumber(this.context);
				this.analyzers['findBirlsRecordByFileNumber'] = this.analyzeFindBirlsRecordResult;

				this.webservices['findCorporateRecordByFileNumber'] = new findCorporateRecordByFileNumber(this.context);
				this.analyzers['findCorporateRecordByFileNumber'] = this.analyzeFindCorporateRecordResult;
			}
			else {
				this.webservices['findBirlsRecord'] = new findBirlsRecord(this.context);
				this.analyzers['findBirlsRecord'] = this.analyzeFindBirlsRecordResult;

				this.webservices['findCorporateRecord'] = new findCorporateRecord(this.context);
				this.analyzers['findCorporateRecord'] = this.analyzeFindCorporateRecordResult;

				this.webservices['findCorporateRecordByFileNumber'] = new findCorporateRecordByFileNumber(this.context);
				this.analyzers['findCorporateRecordByFileNumber'] = this.analyzeFindCorporateRecordResult;
			}

			this.webservices['findDependents'] = new findDependents(this.context);
			this.webservices['findAllRelationships'] = new findAllRelationships(this.context);
			this.webservices['findAllPtcpntAddrsByPtcpntId'] = new findAllPtcpntAddrsByPtcpntId(this.context);

			this.webservices['findAllFiduciaryPoa'] = new findAllFiduciaryPoa(this.context);

			break;
	}

	this.executeSearchOperations(this.webservices); //Inherited from Search.js
	return !this.hasErrors;
}
var analyzeFindCorporateRecordResult = function (parentObject) {
	var corpXml = Xrm.Page.getAttribute('va_findcorprecordresponse').getValue();

	if (corpXml && corpXml != '') {
		var corpXmlObject = _XML_UTIL.parseXmlObject(corpXml);

		if (corpXmlObject && corpXmlObject.xml && corpXmlObject.xml != '') {
			var recordCount = 0;

			if (SingleNodeExists(corpXmlObject, '//numberOfRecords')) { recordCount = parseInt(corpXmlObject.selectSingleNode('//numberOfRecords').text); }
			else if (SingleNodeExists(corpXmlObject, '//ptcpntId')) { recordCount = 1; }
			else { recordCount = 0; }

			//Global object for Extjs processing
			_CORP_RECORD_COUNT = recordCount;

			if (recordCount == 1) {
				var fileNumber; var ptcpntId; var ssn;

				if (SingleNodeExists(corpXmlObject, '//ssn')) {
					ssn = corpXmlObject.selectSingleNode('//ssn').text;
					if (ssn && ssn != '') {
						parentObject.context.parameters['ssn'] = ssn;
					}
				}

				if (SingleNodeExists(corpXmlObject, '//fileNumber')) {
					fileNumber = corpXmlObject.selectSingleNode('//fileNumber').text;
					if (fileNumber && fileNumber != '') {
						parentObject.context.parameters['fileNumber'] = fileNumber;

						if (parentObject.context.user && parentObject.context.user.fileNumber
							&& fileNumber == parentObject.context.user.fileNumber) {
							parentObject.context.endState = true;
							parentObject.hasErrors = true;
							var errorMsg = new actionsMessage();
							errorMsg.documentElement = 'actionsMessage';
							errorMsg.stackTrace = '';
							errorMsg.errorFlag = true;
							errorMsg.description = 'Corporate Record Critical Error: User does not have permission to perform the search on his/her own record.  Search ends.';
							errorMsg.pushMessage();
							return;
						}
					}
				}

				if (SingleNodeExists(corpXmlObject, '//ptcpntId')) {
					ptcpntId = corpXmlObject.selectSingleNode('//ptcpntId').text;
					if (ptcpntId && ptcpntId != '') {
						parentObject.context.parameters['ptcpntId'] = ptcpntId;
					}
				}
				else {
					parentObject.context.endState = true;
					parentObject.hasErrors = true;
					var errorMsg = new actionsMessage();
					errorMsg.documentElement = 'actionsMessage';
					errorMsg.stackTrace = '';
					errorMsg.errorFlag = true;
					errorMsg.description = 'Corporate Record Critical Error: "ptcpntId" is not returned from the Corp Database for this record.  Please refer to the Vetsnet Web Service Support group.  Search will exit.';
					errorMsg.pushMessage();
				}

				if (parentObject.webservices['findCorporateRecordByFileNumber']) parentObject.webservices['findCorporateRecordByFileNumber'].context = parentObject.context;
				if (parentObject.webservices['findCorporateRecord']) parentObject.webservices['findCorporateRecord'].context = parentObject.context;
				parentObject.webservices['findDependents'].context = parentObject.context;
				parentObject.webservices['findAllPtcpntAddrsByPtcpntId'].context = parentObject.context;
				parentObject.webservices['findAllFiduciaryPoa'].context = parentObject.context;
				parentObject.context.corpMinComplete = true;

				UpdateSearchOptionsObject(parentObject.context);
				UpdateSearchListObject({ name: 'corpMinSearch', complete: true });
			}
			else {
				if (recordCount == 0) {
					parentObject.actionMessage.errorFlag = true;
					parentObject.actionMessage.nodataFlag = true;
					parentObject.actionMessage.description = 'Find Corporate Record Web Service call did not find any data matching input parameters.';
					parentObject.actionMessage.xmlResponse = corpXml;
					parentObject.actionMessage.pushMessage();
					parentObject.hasErrors = true;
				}

				parentObject.context.endState = true;
			}

		}
	}

	return;
}
var analyzeFindVetByPtcpntIdResult = function (parentObject) {
	var corpXml = Xrm.Page.getAttribute('va_findveteranresponse').getValue();

	if (corpXml && corpXml != '') {
		var corpXmlObject = _XML_UTIL.parseXmlObject(corpXml);

		if (corpXmlObject && corpXmlObject.xml && corpXmlObject.xml != '') {

			var fileNumber; var ptcpntId; var ssn;

			if (SingleNodeExists(corpXmlObject, '//ssn')) {
				ssn = corpXmlObject.selectSingleNode('//ssn').text;
				if (ssn && ssn != '') {
					parentObject.context.parameters['ssn'] = ssn;
				}
			}

			if (SingleNodeExists(corpXmlObject, '//fileNumber')) {
				fileNumber = corpXmlObject.selectSingleNode('//fileNumber').text;
				if (fileNumber && fileNumber != '') {
					parentObject.context.parameters['fileNumber'] = fileNumber;

					if (parentObject.context.user && parentObject.context.user.fileNumber
							&& fileNumber == parentObject.context.user.fileNumber) {
						parentObject.context.endState = true;
						parentObject.hasErrors = true;
						var errorMsg = new actionsMessage();
						errorMsg.documentElement = 'actionsMessage';
						errorMsg.stackTrace = '';
						errorMsg.errorFlag = true;
						errorMsg.description = 'Corporate Record Critical Error: User does not have permission to perform the search on his/her own record.  Search ends.';
						errorMsg.pushMessage();
						return;
					}
				}
			}

			if (SingleNodeExists(corpXmlObject, '//ptcpntId')) {

				ptcpntId = corpXmlObject.selectSingleNode('//ptcpntId').text;
				if (ptcpntId && ptcpntId != '') {
					parentObject.context.parameters['ptcpntId'] = ptcpntId;

					// update findCorporate field with return results
					Xrm.Page.getAttribute('va_findcorprecordresponse').setValue(corpXml);
				}
			}

			parentObject.webservices['findCorporateRecordByFileNumber'].context = parentObject.context;
			parentObject.webservices['findDependents'].context = parentObject.context;
			parentObject.webservices['findAllPtcpntAddrsByPtcpntId'].context = parentObject.context;
			parentObject.webservices['findAllFiduciaryPoa'].context = parentObject.context;

			UpdateSearchOptionsObject(parentObject.context, 'corpMinSearch');
			UpdateSearchListObject({ name: 'corpMinSearch', complete: true });
		}
	}
	return;
}
var analyzeFindBirlsRecordResult = function (parentObject) {
	var birlsXml = Xrm.Page.getAttribute('va_findbirlsresponse').getValue();
	if (birlsXml && birlsXml != '') {
		var birlsXmlObject = _XML_UTIL.parseXmlObject(birlsXml);

		if (birlsXmlObject && birlsXmlObject.xml && birlsXmlObject.xml != '') {
			var findBirlsRecordCount = 0;
			var fileNumber; var ptcpntId; var ssn;

			if (SingleNodeExists(birlsXmlObject, '//NUMBER_OF_RECORDS')) {
				findBirlsRecordCount = parseInt(birlsXmlObject.selectSingleNode('//NUMBER_OF_RECORDS').text);
			}

			if (SingleNodeExists(birlsXmlObject, '//CLAIM_NUMBER')) {
				fileNumber = birlsXmlObject.selectSingleNode('//CLAIM_NUMBER').text;

				if (fileNumber && fileNumber != '') {
					parentObject.context.parameters['fileNumber'] = fileNumber;

					if (parentObject.context.user && parentObject.context.user.fileNumber
							&& fileNumber == parentObject.context.user.fileNumber) {
						parentObject.context.endState = true;
						parentObject.hasErrors = true;
						var errorMsg = new actionsMessage();
						errorMsg.documentElement = 'actionsMessage';
						errorMsg.stackTrace = '';
						errorMsg.errorFlag = true;
						errorMsg.description = 'BIRLS Record Critical Error: User does not have permission to perform the search on his/her own record.  Search ends.';
						errorMsg.pushMessage();
						return;
					}
				}
			}

			if (SingleNodeExists(birlsXmlObject, '//SOC_SEC_NUMBER')) {
				ssn = birlsXmlObject.selectSingleNode('//SOC_SEC_NUMBER').text;

				if (ssn && ssn != '') {
					parentObject.context.parameters['ssn'] = ssn;
				}
			}

			//Store in Global for EXTJS processing
			_BIRLS_RECORD_COUNT = findBirlsRecordCount;

			if (findBirlsRecordCount == 0) {
				var resNode = birlsXmlObject.selectSingleNode('//RETURN_CODE');
				var resMsg = null;
				if (resNode && resNode.text && resNode.text.length > 0) {
					var txt = '';
					if (birlsXmlObject.selectSingleNode('//RETURN_MESSAGE')
						&& birlsXmlObject.selectSingleNode('//RETURN_MESSAGE').text
						&& birlsXmlObject.selectSingleNode('//RETURN_MESSAGE').text.length > 0) {
						txt = birlsXmlObject.selectSingleNode('//RETURN_MESSAGE').text;
					}
					switch (resNode.text) {
						case 'BPNQ0200':
							var warningMsg = new actionsMessage();
							warningMsg.documentElement = 'actionsMessage';
							warningMsg.stackTrace = '';
							warningMsg.warningFlag = true;
							warningMsg.description = 'BIRLS Warning: ' + txt;
							warningMsg.pushMessage();
							break;
						case 'BPNQ9900':
							var warningMsg = new actionsMessage();
							warningMsg.documentElement = 'actionsMessage';
							warningMsg.stackTrace = '';
							warningMsg.errorFlag = true;
							warningMsg.description = 'BIRLS error: ' + txt;
							warningMsg.xmlResponse = birlsXml;
							warningMsg.pushMessage();
							break;
						default:
							var warningMsg = new actionsMessage();
							warningMsg.documentElement = 'actionMessage';
							warningMsg.stackTrace = '';
							warningMsg.errorFlag = true;
							warningMsg.nodataFlag = true;
							warningMsg.description = 'Find BIRLS Record Web Service call did not find any data matching input parameters.';
							warningMsg.xmlResponse = birlsXml;
							warningMsg.pushMessage();
							break;
					}
				}
			}

			if (findBirlsRecordCount && findBirlsRecordCount != undefined && findBirlsRecordCount > 0) { _birlsResult = true; }

			if (parentObject.webservices['findCorporateRecordByFileNumber']) parentObject.webservices['findCorporateRecordByFileNumber'].context = parentObject.context;
			if (parentObject.webservices['findCorporateRecord']) parentObject.webservices['findCorporateRecord'].context = parentObject.context;
			parentObject.webservices['findDependents'].context = parentObject.context;
			parentObject.webservices['findAllPtcpntAddrsByPtcpntId'].context = parentObject.context;
			parentObject.webservices['findAllFiduciaryPoa'].context = parentObject.context;

			UpdateSearchOptionsObject(parentObject.context, 'birlsSearch');
			UpdateSearchListObject({ name: 'birlsSearch', complete: true });
		}
	}

	return;
}