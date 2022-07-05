/**
* @author Josh Oliver
* @class VIP.controller.PersonSelection
*
* The controller for the person selection grid. 
* Controls the selection of an individual to identify the caller
* and search for details.
*/
Ext.define('VIP.controller.PersonSelection', {
    extend: 'Ext.app.Controller',
    requires: [
		'VIP.model.Person'
	],
    uses: [
		'Ext.util.MixedCollection',
		'Ext.util.Filter'
	],
    refs: [
		{
		    ref: 'personSelection',
		    selector: 'personselection'
		},
		{
		    ref: 'corp',
		    selector: 'personselection > [xtype="personselection.corp"]'
		},
		{
		    ref: 'birls',
		    selector: 'personselection > [xtype="personselection.birls"]'
		}
	],

    stores: [
		'Birls',
		'Corp',
		'CorpPersonSelection',
		'BirlsPersonSelection'
	],

    init: function () {
        var me = this;
        me.control({
            'personselection > grid > toolbar > button': {
                click: me.onSelectRecordButtonClick
            },
            'personselection > [xtype="personselection.birls"]': {
                itemdblclick: me.onBirlsRecordDoubleClick
            },
            'personselection > [xtype="personselection.corp"]': {
                itemdblclick: me.onCorpRecordDoubleClick
            }
        });

        me.application.on({
            personinquirystarted: me.onPersonInquiryStarted,
            multiplepeoplefound: me.clearPersonSelectionLoadMasks,
            scope: me
        });

        //TODO: integration hack
        //        if (parent) {
        //            parent._vipEntryPoint = me.onPersonInquiryStarted;
        //        }

        Ext.log('The PersonSelection controller has been initialized');
    },

    executeInitialPersonInquirySearch: function (fileNumber, participantId, filters) {
        var me = this;
        me.curBirlsXml = '';

        //If we have a Participant Id but no file number, allow Corp store call findVeteranByPtcpntId. Otherwise call Birls web services first.
        if (!Ext.isEmpty(fileNumber) || (Ext.isEmpty(fileNumber) && Ext.isEmpty(participantId))) {
            me.getBirlsStore().load({
                filters: Ext.clone(filters),
                callback: function (records, operation, success) {
                    if (success) {
                        //Load data into multiple BIRLS data store. loadData() removes all previous data first.
                        if (!Ext.isEmpty(records) && records.length > 0) { me.curBirlsXml = records[0].raw.xml; }
                        var birlsMultipleResultSet = me.getBirlsPersonSelectionStore().getProxy().getReader().read(operation.response);
                        me.getBirlsPersonSelectionStore().loadData(birlsMultipleResultSet.records);

                        me.loadBirlsSelectionGrid(records);
                    }

                    me.getCorpStore().load({
                        filters: Ext.clone(filters),
                        callback: callback,
                        scope: me
                    });

                    //On success == false FindBirlsByFileNumber will fire application's searcherror event, no need to do it here
                    me.application.fireEvent('webservicecallcomplete', operation, 'Birls');
                },
                scope: me
            });
        }
        else {
            me.getCorpStore().load({
                filters: Ext.clone(filters),
                callback: callback,
                scope: me
            });
        }

        //This is the callback for both CorpStore load functions.
        function callback(records, operation, success) {

            //This will add data from findVeteranByPtcpntId to the Birls Single store.
            function pushDataToBirlsStore(serviceMethod, operation) {
                var birlsResultSet;
                if (serviceMethod == 'findVeteranByPtcpntId') {
                    birlsResultSet = me.getBirlsStore().getProxy().getReader().read(operation.response);
                    if (birlsResultSet.records[0].get('returnCode') == 'BPNQ0100') {//No Birls Record
                        me.getBirlsStore().removeAll();
                        me.getBirlsPersonSelectionStore().removeAll();
                    }
                    else {
                        me.getBirlsStore().loadData(birlsResultSet.records);
                    }
                }
            };

            var serviceMethod = operation.request.envelope.getMethodName(),
				corpMultipleResultSet;

            if (success) {
                var corpRecord,
					birlsRecord;

                if (me.getCorpStore().getCount() == 1) {
                    corpRecord = me.getCorpStore().getAt(0);
                    if (Ext.isEmpty(corpRecord.get('participantId'))) {
                        corpRecord = null;
                        me.getCorpStore().removeAll();
                    }
                }
                if (me.getBirlsStore().getCount() == 1) {
                    birlsRecord = me.getBirlsStore().getAt(0);
                    if (Ext.isEmpty(birlsRecord.get('fileNumber')) && Ext.isEmpty(birlsRecord.get('ssn'))) {
                        birlsRecord = null;
                        me.getBirlsStore().removeAll();
                    }
                }

                //If we call FindVeteranByPtcpntId, result is similar to findBirlsRecordByFileNumber. Load it into the store
                pushDataToBirlsStore(serviceMethod, operation);

                me.application.fireEvent('personinquirysuccess');

                //Load response into multiple corp store. If data is loaded, it means it was a multiple response. loadData() also clears previous data.
                corpMultipleResultSet = me.getCorpPersonSelectionStore().getProxy().getReader().read(operation.response);
                me.getCorpPersonSelectionStore().loadData(corpMultipleResultSet.records);

                me.loadCorpSelectionGrid(records);

                //Cases: N Birls N Corp, 1 Birls N Corp, 0 Birls N Corp, N Birls 1 Corp, N Birls 0 Corp
                if (me.getCorpPersonSelectionStore().getCount() > 1 || me.getBirlsPersonSelectionStore().getCount() > 1) {
                    me.getPersonSelection().expand();
                    me.application.fireEvent('multiplepeoplefound', me.getCorp().getStore().getCount(), me.getBirls().getStore().getCount());
                }
                //If we get one response from each call but file numbers do not match, force user to select one
                else if (!Ext.isEmpty(corpRecord) && !Ext.isEmpty(birlsRecord) &&
					corpRecord.get('fileNumber') != birlsRecord.get('fileNumber') &&
					corpRecord.get('ssn') != birlsRecord.get('ssn')) {
                    me.getPersonSelection().expand();
                    me.application.fireEvent('multiplepeoplefound', me.getCorp().getStore().getCount(), me.getBirls().getStore().getCount());
                }
                //Case: 0 Birls 0 Corp. If no responses in any of the stores, throw notification to alert user. 
                else if (me.getBirlsStore().getCount() == 0 && me.getCorpStore().getCount() == 0 &&
						me.getCorpPersonSelectionStore().getCount() == 0 && me.getBirlsPersonSelectionStore().getCount() == 0) {

                    if (me.application.personInquiryModel.get('doSearchVadir')) {
                        me.application.fireEvent('dopersonvadirsearch', me.application.personInquiryModel);
                    } else {
                        me.application.fireEvent('personinquirynoresults');
                    }
                }
                // Cases that fall to this: 1 Birls 1 Corp with matching fileNumbers/ssn, 1 Birls 0 Corp, 0 Birls 1 Corp
                else {
                    birlsRecord = me.getBirlsStore().getAt(0);
                    corpRecord = me.getCorpStore().getAt(0);

                    var personModel = Ext.create('VIP.model.Person'),
						record = birlsRecord ? birlsRecord : corpRecord;

                    // THIS WILL UPDATE THE PERSONINQUIRYMODEL FOUND AT THIS.APPLICATION.PERSONINQUIRYMODEL. UPDATE WITH BIRLS FIRST, AUGEMENT WITH CORP
                    var fileNumber = record.get('fileNumber'),
						participantId = record.get('participantId'),
						ssn = record.get('ssn'),
						firstName = record.get('firstName'),
						lastName = record.get('lastName'),
						middleName = record.get('middleName'),
						dob = record.get('dob'),
						gender = record.get('gender');

                    //This is for the Case 1 Birls 1 Corp.  Add Corp data if field is blank
                    if (!Ext.isEmpty(corpRecord)) {
                        var corpfileNumber = corpRecord.get('fileNumber'); if (!Ext.isEmpty(corpfileNumber)) { fileNumber = corpfileNumber; }
                        var corpssn = corpRecord.get('ssn'); if (!Ext.isEmpty(corpssn)) { ssn = corpssn; }
                        if (Ext.isEmpty(participantId)) { participantId = corpRecord.get('participantId'); }
                        if (Ext.isEmpty(firstName)) { firstName = corpRecord.get('firstName'); }
                        if (Ext.isEmpty(lastName)) { lastName = corpRecord.get('lastName'); }
                        if (Ext.isEmpty(middleName)) { middleName = corpRecord.get('middleName'); }
                        if (Ext.isEmpty(dob)) { dob = corpRecord.get('dob'); }
                        if (Ext.isEmpty(gender)) { gender = corpRecord.get('gender'); }
                    }
                    if (Ext.isEmpty(fileNumber)) { fileNumber = ssn; }

                    me.application.personInquiryModel.set({
                        fileNumber: fileNumber,
                        participantId: participantId,
                        ssn: ssn,
                        firstName: firstName,
                        lastName: lastName,
                        middleName: middleName,
                        dob: dob,
                        gender: gender
                    });

                    personModel.set({
                        fileNumber: fileNumber,
                        participantId: participantId,
                        ssn: ssn,
                        firstName: firstName,
                        lastName: lastName,
                        middleName: middleName,
                        dob: dob,
                        gender: gender
                    });

                    //THIS FUNCTION WILL TRIGGER THE onIndividualIdentified EVENT THAT ALL CONTROLLERS LISTEN TO IF 
                    //FILE NUMBER EXISTS. OTHERWISE IT WILL SEARCH AGAIN IF IT HAS A PARTICIPANTID TO OBTAIN A FILE NUMBER.
                    me.identifyIndividualAsCaller(personModel, true);
                    /*if (Ext.isEmpty(fileNumber) && !Ext.isEmpty(participantId)) {
                    me.identifyIndividualAsCaller(personModel, false);
                    }
                    else {
                    me.identifyIndividualAsCaller(personModel, true);
                    } */
                }
            }
            me.application.fireEvent('personinquiryended');
            me.application.fireEvent('webservicecallcomplete', operation);
        }
    },

    loadBirlsSelectionGrid: function (records) {
        var me = this,
			storeName = me.getBirls().getStore().storeId,
			messageCode;

        if (!Ext.isEmpty(records)) {
            messageCode = records[0].get("returnCode");
            if (messageCode == "BPNQ0200" || //Too many records, remove from store so count != 1
				messageCode == "BPNQ0100") {//No BIRLS records, remove from store so count != 1
                me.getBirlsStore().removeAll();
            }
        }

        //If we get a Single return and the current store is set for multiple, reconfigure.
        if (me.getBirlsPersonSelectionStore().getCount() == 0 &&
			me.getBirlsStore().getCount() == 1 && storeName == 'BirlsPersonSelection') {
            me.getBirls().reconfigure(me.getBirlsStore(), [
				{ header: 'SSN', dataIndex: 'ssn', width: 150 },
				{ header: 'File Number', dataIndex: 'fileNumber', width: 150 },
				{ header: 'First Name', dataIndex: 'firstName', width: 150 },
				{ header: 'Middle Name', dataIndex: 'middleName', width: 150 },
				{ header: 'Last Name', dataIndex: 'lastName', width: 150 },
				{ header: 'Suffix', dataIndex: 'nameSuffix', width: 150 },
				{ header: 'Date of Birth', dataIndex: 'dob', width: 150 },
				{ header: 'Date of Death', dataIndex: 'dod', width: 150 },
				{ header: 'SSN Verified', dataIndex: 'verifiedSsnInd', width: 150 }
			]);
        }
        //If we get a Multiple return and the current store is set to Single, reconfigure
        else if (me.getBirlsPersonSelectionStore().getCount() > 0 && storeName == 'Birls') {
            me.getBirls().reconfigure(me.getBirlsPersonSelectionStore(), [
				{ header: 'SSN', dataIndex: 'ssn', width: 150 },
				{ header: 'File Number', dataIndex: 'fileNumber', width: 150 },
				{ header: 'First Name', dataIndex: 'firstName', width: 150 },
				{ header: 'Middle Name', dataIndex: 'middleName', width: 150 },
				{ header: 'Last Name', dataIndex: 'lastName', width: 150 },
				{ header: 'Suffix', dataIndex: 'suffixName', width: 150 },
				{ header: 'Date of Birth', dataIndex: 'dob', width: 150 },
				{ header: 'Date of Death', dataIndex: 'dod', width: 150 },
				{ header: 'Veteran Ind', dataIndex: 'veteranIndicator', width: 150 },
				{ header: 'Payee Code', dataIndex: 'payeeCode', width: 150 },
				{ header: 'Branch', dataIndex: 'branchOfService', width: 150 },
				{ header: 'Current Location', dataIndex: 'currentLocation', width: 150 },
				{ header: 'Entered on Duty', dataIndex: 'eod', width: 150 },
				{ header: 'Released Active Duty', dataIndex: 'rad', width: 150 },
				{ header: 'Service Number', dataIndex: 'serviceNumber', width: 150 },
				{ header: 'SSN Verified', dataIndex: 'ssnVerified', width: 150 }
			]);
        }

        //If we had a single response and it was set up for a single store, it would load automatically.
        //Same holds true for multiple.
    },

    loadCorpSelectionGrid: function (records) {
        var me = this,
			storeName = me.getCorp().getStore().storeId;

        //If we get a Single return and the current store is set for multiple, reconfigure.
        //NOTE: Corp single can read Corp multiple, so we have to make sure multiple is 0.
        if (me.getCorpPersonSelectionStore().getCount() == 0 &&
			me.getCorpStore().getCount() == 1 && storeName == 'CorpPersonSelection') {
            me.getCorp().reconfigure(me.getCorpStore(), [
				{ header: 'SSN', dataIndex: 'ssn', width: 150 },
				{ header: 'File Number', dataIndex: 'fileNumber', width: 150 },
				{ header: 'First Name', dataIndex: 'firstName', width: 150 },
				{ header: 'Middle Name', dataIndex: 'middleName', width: 150 },
				{ header: 'Last Name', dataIndex: 'lastName', width: 150 },
				{ header: 'Suffix', dataIndex: 'suffixName', width: 150 },
				{ header: 'Branch of Service', dataIndex: 'branchOfService1', width: 150 },
				{ header: 'Date of Birth', dataIndex: 'dob', width: 150 },
				{ header: 'Date of Death', dataIndex: 'dod', width: 150 },
				{ header: 'Participant ID', dataIndex: 'participantId', width: 150 }
			]);
        }
        //If we get a Multiple return and the current store is set to Single, reconfigure
        else if (me.getCorpPersonSelectionStore().getCount() > 0 && storeName == 'Corp') {
            me.getCorp().reconfigure(me.getCorpPersonSelectionStore(), [
				{ header: 'SSN', dataIndex: 'ssn', width: 150 },
				{ header: 'First Name', dataIndex: 'firstName', width: 150 },
				{ header: 'Middle Name', dataIndex: 'middleName', width: 150 },
				{ header: 'Last Name', dataIndex: 'lastName', width: 150 },
				{ header: 'Suffix', dataIndex: 'suffix', width: 150 },
				{ header: 'Branch of Service', dataIndex: 'branchOfService', width: 150 },
				{ header: 'Date of Birth', dataIndex: 'dob', width: 150 },
				{ header: 'Date of Death', dataIndex: 'dod', width: 150 },
				{ header: 'Participant ID', dataIndex: 'participantId', width: 150 }
			]);
        }

        //If we had a single response and it was set up for a single store, it would load automatically.
        //Same holds true for multiple.
    },

    onPersonInquiryStarted: function (personInquiryModel) {
        var me = this;
        var fileNumber = personInquiryModel.get('fileNumber'),
			participantId = personInquiryModel.get('participantId'),
			filters = Ext.create('Ext.util.MixedCollection');

        if (!Ext.isEmpty(fileNumber)) {
            filters.add({
                property: 'fileNumber',
                value: fileNumber
            });
        }
        else if (!Ext.isEmpty(participantId)) {
            filters.add({
                property: 'ptcpntId',
                value: participantId
            });
        }
        else {
            filters.addAll([
				{
				    property: 'firstName',
				    value: personInquiryModel.get('firstName')
				},
				{
				    property: 'lastName',
				    value: personInquiryModel.get('lastName')
				},
				{
				    property: 'middleName',
				    value: personInquiryModel.get('middleName')
				},
				{
				    property: 'dateOfBirth',
				    value: personInquiryModel.get('dob')
				},
				{
				    property: 'city',
				    value: personInquiryModel.get('city')
				},
				{
				    property: 'state',
				    value: personInquiryModel.get('state')
				},
				{
				    property: 'zipCode',
				    value: personInquiryModel.get('zipCode')
				},
				{
				    property: 'branchOfService',
				    value: personInquiryModel.get('branchOfService')
				},
				{
				    property: 'serviceNumber',
				    value: personInquiryModel.get('serviceNumber')
				},
				{
				    property: 'insuranceNumber',
				    value: personInquiryModel.get('insuranceNumber')
				},
				{
				    property: 'dateOfDeath',
				    value: personInquiryModel.get('dod')
				},
				{
				    property: 'enteredOnDutyDate',
				    value: personInquiryModel.get('eod')
				},
				{
				    property: 'releasedActiveDutyDate',
				    value: personInquiryModel.get('rad')
				},
				{
				    property: 'suffix',
				    value: personInquiryModel.get('suffix')
				},
				{
				    property: 'folderLocation',
				    value: personInquiryModel.get('folderLocation')
				}
			]);
        }

        //this is a double-check to remove filters if they are empty
        filters.each(function (filter) {
            if (Ext.isEmpty(filter.value)) {
                filters.remove(filter);
            }
        });

        me.executeInitialPersonInquirySearch(fileNumber, participantId, filters.getRange());
    },

    onCorpRecordDoubleClick: function (view, record) {
        var me = this,
			personModel = Ext.create('VIP.model.Person');

        personModel.set({
            ssn: record.get('ssn'),
            participantId: record.get('participantId'),
            fileNumber: record.get('fileNumber'),
            firstName: record.get('firstName'),
            lastName: record.get('lastName'),
            middleName: record.get('middleName'),
            dob: record.get('dob'),
            gender: record.get('gender'),
            branchOfService: record.get('branchOfService'),
            dod: record.get('dod')
        });

        me.identifyIndividualAsCaller(personModel, false);
    },

    onBirlsRecordDoubleClick: function (view, record) {
        var me = this,
			personModel = Ext.create('VIP.model.Person');

        personModel.set({
            ssn: record.get('ssn'),
            participantId: record.get('participantId'),
            fileNumber: record.get('fileNumber'),
            firstName: record.get('firstName'),
            lastName: record.get('lastName'),
            middleName: record.get('middleName'),
            dob: record.get('dob'),
            gender: record.get('gender'),
            branchOfService: record.get('branchOfService'),
            dod: record.get('dod')
        });

        me.identifyIndividualAsCaller(personModel, false);
    },

    onSelectRecordButtonClick: function (button) {
        var me = this,
			personModel = Ext.create('VIP.model.Person'),
			corpGrid = me.getCorp(),
			birlsGrid = me.getBirls(),
			selection;

        if (button.action == 'selectcorp') {
            selection = corpGrid.getSelectionModel().getSelection();
            corpGrid.setLoading(true, true);
        }
        if (button.action == 'selectbirls') {
            selection = birlsGrid.getSelectionModel().getSelection();
            birlsGrid.setLoading(true, true);
        }

        if (Ext.isEmpty(selection)) {
            Ext.Msg.alert(
				'User Action Required',
				'Please select an item from the grid and try again.'
			);
            corpGrid.setLoading(false, true);
            birlsGrid.setLoading(false, true);
            return;
        }
        personModel.set({
            ssn: selection[0].get('ssn'),
            participantId: selection[0].get('participantId'),
            fileNumber: Ext.isEmpty(selection[0].get('fileNumber')) ? selection[0].get('ssn') : selection[0].get('fileNumber'),
            firstName: selection[0].get('firstName'),
            lastName: selection[0].get('lastName'),
            middleName: selection[0].get('middleName'),
            dob: selection[0].get('dob'),
            gender: selection[0].get('gender'),
            branchOfService: selection[0].get('branchOfService'),
            dod: selection[0].get('dod')
        });

        me.identifyIndividualAsCaller(personModel, false);
    },

    clearPersonSelectionLoadMasks: function () {
        var me = this;

        me.getCorp().setLoading(false, true);
        me.getBirls().setLoading(false, true);
    },

    identifyIndividualAsCaller: function (identifiedPerson, preventSearch) {
        var me = this,
			filters = Ext.create('Ext.util.MixedCollection'),
			personSelectionPanel = me.getPersonSelection(),
			ssn = identifiedPerson.get('ssn'),
			fileNumber = identifiedPerson.get('fileNumber') ? identifiedPerson.get('fileNumber') : ssn, //Use SSN as fileNumber if fileNumber is empty
			participantId = identifiedPerson.get('participantId'),
			corpGrid = me.getCorp(),
			birlsGrid = me.getBirls();

        if (!preventSearch) {
            if (!Ext.isEmpty(ssn)) {
                filters.add({
                    property: 'ssn',
                    value: ssn
                });
            }
            if (!Ext.isEmpty(fileNumber)) {
                filters.add({
                    property: 'fileNumber',
                    value: fileNumber
                });
            }
            if (!Ext.isEmpty(participantId)) {
                filters.add({
                    property: 'ptcpntId',
                    value: participantId
                });
            }

            me.executeInitialPersonInquirySearch(fileNumber, participantId, filters.getRange());
        }
        else {
            corpGrid.setLoading(false, true);
            birlsGrid.setLoading(false, true);
            //THIS WILL FIRE THE EVENT onIndividualIdentified THAT ALL CONTROLLERS LISTEN TO. 
            me.application.fireEvent('individualidentified', identifiedPerson);
            personSelectionPanel.collapse('Ext.Component.DIRECTION_TOP');
            //TODO: integration hack                va_findbirlsresponse  va_findcorprecordresponse    va_generalinformationresponsebypid  
            if (parent && parent._VIPEndOfServiceCall) {
                var map = me.application.responseCacheMap,
				store = me.getCorpStore(),
					corpXml = store.getProxy().getReader().rawData.xml,
					birlsXml = me.getBirlsStore().getProxy().getReader().rawData.xml;
                if (Ext.isEmpty(birlsXml) && me.curBirlsXml != undefined) { birlsXml = me.curBirlsXml; }
                parent._VIPEndOfServiceCall(map.get('Birls'), true, '', '', birlsXml);
                parent._VIPEndOfServiceCall(map.get('Corp'), true, '', '', corpXml);
            }

        }
        //debugger;

    }
});
