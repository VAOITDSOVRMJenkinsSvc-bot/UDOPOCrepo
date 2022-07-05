/**
* @author Ivan Yurisevic
* @class VIP.controller.PersonInfo
*
*/
Ext.define('VIP.controller.PersonInfo', {
    extend: 'Ext.app.Controller',
    requires: ['VIP.services.ClaimsProcessing'],
    stores: ['Corp', 'personinfo.GeneralDetails', 'personinfo.Dependents', 'personinfo.AllRelationships', 'personinfo.Addresses', 'personinfo.Flashes',
             'CorpPersonSelection', 'Birls', 'debug.WebServiceRequestHistory', 'ebenefits.Ebenefits'],
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    refs: [{
        ref: 'corpDetails',
        selector: '[xtype="personinfo.corp"]'
    }, {
        ref: 'details',
        selector: '[xtype="personinfo.details"]'
    }, {
        ref: 'generalDetails',
        selector: '[xtype="personinfo.details.generaldetails"]'
    }, {
        ref: 'serviceRequestButton',
        selector: '[xtype="personinfo.details.dependents"] > splitbutton[action="startservicerequest"]'
    }, {
            ref: 'claimFolderText',
            selector: '[xtype="personinfo.details.generaldetails"] > fieldcontainer > displayfield[name="claimFolderLocation"]'
    }, {
        ref: 'depGrid',
        selector: '[xtype="personinfo.details.dependents"]'
    }, {
        ref: 'allRelationsGrid',
        selector: '[xtype="personinfo.details.allrelationships"]'
    }, {
        ref: 'dateOfDeathField',
        selector: '[xtype="personinfo.corp"] > displayfield[name="dod"]'
    }, {
        ref: 'militaryBranchField',
        selector: '[xtype="personinfo.corp"] > displayfield[name="branchOfService1"]'
    }, {
        ref: 'personSelection',
        selector: 'personselection'
    }, {
        ref: 'addressGrid',
        selector: '[xtype="personinfo.details.addresses.addresslist"]'
    }, {
        ref: 'treasuryAddress',
        selector: '[xtype="personinfo.details.addresses.treasuryaddress"]'
    }, {
        ref: 'flashesGrid',
        selector: '[xtype="personinfo.details.flashes"]'
    }, {
        ref: 'registrationStatusField',
        selector: '[xtype="personinfo.details.generaldetails"] > displayfield[name="registrationStatus"]'
    }],

    init: function () {
        var me = this;

        me.control({
            '[xtype="personinfo.details.dependents"] > toolbar > splitbutton[action="startservicerequest"]': {
                click: me.serviceRequest
            },
            '[xtype="personinfo.details.dependents"] > toolbar > splitbutton[action="startservicerequest"] > menu': {
                click: me.serviceRequestMenuClick
            },
            'button[action="viewdependentscontact"]': {
                click: me.viewDependentsContactButtonClick
            },
            'button[action="viewallrelationshipscontact"]': {
                click: me.viewAllRelationshipsContactButtonClick
            },
            'button[action="intentToFile"]': {
                click: me.intentToFileButtonClick
            },
            '[xtype="personinfo.details.generaldetails"] > fieldcontainer > button[action="claimfolderlocation"]': {
                click: me.onClaimProcessingTimesFieldSetButtonClick
            },
            '[xtype="personinfo.details"]': {
                tabchange: me.onTabChange
            },
            '[xtype="personinfo.details.addresses.addresslist"]': {
                selectionchange: me.onAddressGridSelection
            },
            '[xtype="personinfo.corp"] > toolbar > button[action="createpersonservicerequest"]': {
                click: me.personServiceRequest
            },
            '[xtype="personinfo.corp"] > toolbar > button[action="createvai"]': {
                click: me.createCorpVAI
            },
            '[xtype="personinfo.details.dependents"] > toolbar > button[action="createvai"]': {
                click: me.createDependentVAI
            }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            updateclaimprocessingtimelabel: me.onUpdatedClaimProcessingTimeLabel,
            displayclaimprocessingtimemainclaim: me.onClaimProcessingTimesMainClaimButtonClick,
            displayclaimprocessingtimefieldset: me.onClaimProcessingTimesFieldSetButtonClick,
            cacheddataloaded: me.onCachedDataLoaded,
            viewmorefiduciarydata: me.onViewMoreFiduciaryData,
            redrawcaddfields: me.onRedrawCADDFields,
            personinfotabchange: me.onTabChange,
            edipirecordloaded: me.displayRegistrationStatus,
            scope: me
        });

        Ext.log('The PersonInfo controller was successfully initialized.');
    },

    displayRegistrationStatus: function (edipi) {
        var me = this;

        //if (Ext.isEmpty(edipi)) {
        //    me.getRegistrationStatusField().setValue('No EDIPI');
        //}
        //else {
        me.getEbenefitsEbenefitsStore().load({
            filters: [{
                property: 'edipi',
                value: edipi
            }],
            callback: function (records, operation, success) {
                var status = '';

                if (success && !Ext.isEmpty(records)) {
                    var registrationStatusRecord = records[0];
                    if (!Ext.isEmpty(registrationStatusRecord)) {
                        status = (registrationStatusRecord.get('isRegistered') ? 'Registered: ' + registrationStatusRecord.get('isRegistered') : '');
                        if (!Ext.isEmpty(parent.Xrm)) {
                            switch (registrationStatusRecord.get('isRegistered')) {
                                case true: //is Registered
                                    parent.Xrm.Page.getAttribute('va_hasebenefitsaccount').setValue(true);
                                    break;
                                case false:  //is not registered
                                    parent.Xrm.Page.getAttribute('va_hasebenefitsaccount').setValue(false);
                                    break;
                                default:
                                    parent.Xrm.Page.getAttribute('va_hasebenefitsaccount').setValue(false);
                                    break;
                            }
                        }
                    }

                    if (registrationStatusRecord.get('credLevelAtLastLogin')) {
                        if (!Ext.isEmpty(parent.Xrm)) {
                            var cred = registrationStatusRecord.get('credLevelAtLastLogin');
                            var credOption = null;
                            /*Values for Option Set:
                            Basic:      953850000; WS Return = 1
                            Premium:    953850001; WS Return = 2
                            Admin:      953850002; WS Return = 3
                            */
                            switch (cred) {
                                case '1':
                                    cred = 'Basic';
                                    credOption = '953850000';
                                    break;
                                case '2':
                                    cred = 'Premium';
                                    credOption = '953850001';
                                    break;
                                case '3':
                                    cred = 'Admin';
                                    credOption = '953850002';
                                    break;
                                default:
                                    cred = '';
                                    credOption = null;
                                    break;
                            }
                            parent.Xrm.Page.getAttribute('va_credlevel').setValue(credOption);
                        }
                        status += '; Cred Level: ' + cred;
                    }

                    if (registrationStatusRecord.get('status') && (!Ext.isEmpty(parent.Xrm))) {
                        var ebenefitsStatus = registrationStatusRecord.get('status');
                        parent.Xrm.Page.getAttribute('va_ebenefitsstatus').setValue(ebenefitsStatus);
                        status += '; Status: ' + ebenefitsStatus;
                    }
                }
                me.getRegistrationStatusField().setValue(status);

                me.application.fireEvent('webservicecallcomplete', operation, 'ebenefits.Ebenefits');
            },
            scope: me
        });
        //}
    },

    onTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
            activeTab = me.getDetails().getActiveTab(),
            tabTitle = activeTab.title,
            gridCount = null;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            if (Ext.get('id_personinfo_Corp_01')) Ext.get('id_personinfo_Corp_01').hide();
            if (Ext.get('id_personinfo_Corp_02')) Ext.get('id_personinfo_Corp_02').hide();
            if (Ext.get('id_personinfo_Corp_03')) Ext.get('id_personinfo_Corp_03').hide();
            if (Ext.get('id_personinfo_Corp_04')) Ext.get('id_personinfo_Corp_04').hide();

            //if (Ext.get('id_personinfo_details_Dependents_01')) Ext.get('id_personinfo_details_Dependents_01').hide();
            if (Ext.get('id_personinfo_details_Dependents_02')) Ext.get('id_personinfo_details_Dependents_02').hide();
            if (Ext.get('id_personinfo_details_Dependents_03')) Ext.get('id_personinfo_details_Dependents_03').hide();
            if (Ext.get('id_personinfo_details_Dependents_04')) Ext.get('id_personinfo_details_Dependents_04').hide();
            if (Ext.get('id_personinfo_details_Dependents_05')) Ext.get('id_personinfo_details_Dependents_05').hide();
            if (Ext.get('id_personinfo_details_Dependents_06')) Ext.get('id_personinfo_details_Dependents_06').hide();

            if (Ext.get('id_personinfo_details_AllRelationships_01')) Ext.get('id_personinfo_details_AllRelationships_01').hide();
            if (Ext.get('id_personinfo_details_AllRelationships_02')) Ext.get('id_personinfo_details_AllRelationships_02').hide();
        }

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        } else if (activeTab.getXType() == 'personinfo.details.addresses') {
            tabTitle = 'Addresses: ' + me.getAddressGrid().getStore().getCount();
        } else {
            tabTitle = null;
        }

        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    onCachedDataLoaded: function () {
        var me = this,
            corpRecord = me.getCorpStore().getAt(0),
            birlsRecord = me.getBirlsStore().getAt(0),
            corpSelection = me.getCorpPersonSelectionStore().getAt(0),
            genDetailsRecord = me.getPersoninfoGeneralDetailsStore().getAt(0),
            ebenefitsStore = me.getEbenefitsEbenefitsStore().getAt(0);

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            if (Ext.get('id_personinfo_Corp_01')) Ext.get('id_personinfo_Corp_01').hide();
            if (Ext.get('id_personinfo_Corp_02')) Ext.get('id_personinfo_Corp_02').hide();
            if (Ext.get('id_personinfo_Corp_03')) Ext.get('id_personinfo_Corp_03').hide();
            if (Ext.get('id_personinfo_Corp_04')) Ext.get('id_personinfo_Corp_04').hide();

            //if (Ext.get('id_personinfo_details_Dependents_01')) Ext.get('id_personinfo_details_Dependents_01').hide();
            if (Ext.get('id_personinfo_details_Dependents_02')) Ext.get('id_personinfo_details_Dependents_02').hide();
            if (Ext.get('id_personinfo_details_Dependents_03')) Ext.get('id_personinfo_details_Dependents_03').hide();
            if (Ext.get('id_personinfo_details_Dependents_04')) Ext.get('id_personinfo_details_Dependents_04').hide();
            if (Ext.get('id_personinfo_details_Dependents_05')) Ext.get('id_personinfo_details_Dependents_05').hide();
            if (Ext.get('id_personinfo_details_Dependents_06')) Ext.get('id_personinfo_details_Dependents_06').hide();

            if (Ext.get('id_personinfo_details_AllRelationships_01')) Ext.get('id_personinfo_details_AllRelationships_01').hide();
        }

        //Set Registration Status On Cached Load...
        if (!Ext.isEmpty(me.getEbenefitsEbenefitsStore().getAt(0))) {
            var ebenefitsStatus = '';
            ebenefitsStatus = (ebenefitsStore.get('isRegistered') ? 'Registered: ' + ebenefitsStore.get('isRegistered') : '');
            if (ebenefitsStore.get('credLevelAtLastLogin')) {
                var cred = ebenefitsStore.get('credLevelAtLastLogin');
                /*Values for Option Set:
                Basic:      953850000; WS Return = 1
                Premium:    953850001; WS Return = 2
                Admin:      953850002; WS Return = 3
                */
                switch (cred) {
                    case '1':
                        cred = 'Basic';
                        break;
                    case '2':
                        cred = 'Premium';
                        break;
                    case '3':
                        cred = 'Admin';
                        break;
                    default:
                        cred = '';
                        break;
                }
                ebenefitsStatus += '; Cred Level: ' + cred;
            }
            if (ebenefitsStore.get('status')) {
                var recordStatus = ebenefitsStore.get('status');
                ebenefitsStatus += '; Status: ' + recordStatus;
            }
            me.getRegistrationStatusField().setValue(ebenefitsStatus);
        }

        //debugger;
        var corpData = new Array();
        corpData.push(corpRecord);
        corpData.push(corpSelection);
        corpData.push(genDetailsRecord);
        me.application.fireEvent('gotcorp', corpData);

        // update search context.  Sometimes BIRLS fails, so saved data is fault return.  We then would get a null pointer on birlsRecord.get().
        // THIS WILL UPDATE THE PERSONINQUIRYMODEL FOUND AT THIS.APPLICATION.PERSONINQUIRYMODEL. UPDATE WITH BIRLS FIRST, AUGEMENT WITH CORP
        var fileNumber = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('fileNumber')) ? corpRecord.get('fileNumber') : birlsRecord.get('fileNumber'),
            participantId = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('participantId')) ? corpRecord.get('participantId') : birlsRecord.get('participantId'),
            ssn = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('ssn')) ? corpRecord.get('ssn') : birlsRecord.get('ssn'),
            firstName = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('firstName')) ? corpRecord.get('firstName') : birlsRecord.get('firstName'),
            lastName = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('lastName')) ? corpRecord.get('lastName') : birlsRecord.get('lastName'),
            middleName = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('middleName')) ? corpRecord.get('middleName') : birlsRecord.get('middleName'),
            dob = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('dob')) ? corpRecord.get('dob') : birlsRecord.get('dob'),
            gender = Ext.isEmpty(birlsRecord) || Ext.isEmpty(birlsRecord.get('gender')) ? corpRecord.get('gender') : birlsRecord.get('gender');

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

        var personModel = Ext.create('VIP.model.Person');
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
        me.application.selectedPersonFromCache = personModel;
        me.application.fireEvent('personloadedfromcache', personModel);

        if (!Ext.isEmpty(corpRecord)) {
            me.getCorpDetails().loadRecord(corpRecord);
            me.getGeneralDetails().loadRecord(corpRecord); //This essentially just load the Participant Id
        }
        if (Ext.isEmpty(me.getDateOfDeathField().getValue()) && !Ext.isEmpty(birlsRecord)) {
            me.getDateOfDeathField().setValue(birlsRecord.data.dod);
        }

        if (!Ext.isEmpty(genDetailsRecord)) {
            me.getGeneralDetails().loadRecord(genDetailsRecord);
            if (Ext.isEmpty(me.getMilitaryBranchField().getValue())) {
                me.getMilitaryBranchField().setValue(genDetailsRecord.get('militaryBranch')); //updates top portion if empty
            }
        }
    },

    onAddressGridSelection: function (selection, address, index) {
        var me = this;

        if (!Ext.isEmpty(selection)) {
            me.getTreasuryAddress().loadRecord(address[0]);
        }
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this,
            corpRecord = me.getCorpStore().getAt(0),
            birlsRecord = me.getBirlsStore().getAt(0),
            corpSelection = me.getCorpPersonSelectionStore().getAt(0);

        var corpData = new Array();
        corpData.push(corpRecord);
        corpData.push(corpSelection);
        me.application.fireEvent('gotcorp', corpData);

        me.getCorpDetails().getForm().reset();
        me.getGeneralDetails().getForm().reset();

        if (!Ext.isEmpty(corpRecord)) {
            me.getCorpDetails().loadRecord(corpRecord);
            me.getGeneralDetails().loadRecord(corpRecord); //This essentially just load the Participant Id
        }
        if (Ext.isEmpty(me.getDateOfDeathField().getValue()) && !Ext.isEmpty(birlsRecord)) {
            me.getDateOfDeathField().setValue(birlsRecord.data.dod);
        }

        me.getPersoninfoGeneralDetailsStore().load({
            filters: [{
                property: 'ptcpntVetId',
                value: selectedPerson.get('participantId')
            }, {
                property: 'ptcpntBeneId',
                value: selectedPerson.get('participantId')
            }, {
                property: 'ptpcntRecipId',
                value: selectedPerson.get('participantId')
            }],
            callback: function (records, operation, success) {
                if (success && !Ext.isEmpty(records)) {

                    corpData.push(records[0]);
                    me.application.fireEvent('gotcorp', corpData);

                    me.getGeneralDetails().loadRecord(records[0]);
                    if (Ext.isEmpty(me.getMilitaryBranchField().getValue())) {
                        me.getMilitaryBranchField().setValue(records[0].get('militaryBranch')); //updates top portion if empty
                    }
                    me.getFlashesGrid().reconfigure(records[0].flashes());
                    me.updateCRMFlashesHeader(records[0].flashes());
                }
                me.application.fireEvent('webservicecallcomplete', operation, 'personinfo.GeneralDetails');
            },
            scope: me
        });

        me.getPersoninfoAddressesStore().load({
            filters: [{
                property: 'ptcpntId',
                value: selectedPerson.get('participantId')
            }],
            callback: function (records, operation, success) {
                me.application.fireEvent('webservicecallcomplete', operation, 'personinfo.Addresses');
            },
            scope: me
        });

        me.getPersoninfoDependentsStore().load({
            filters: [{
                property: 'fileNumber',
                value: selectedPerson.get('fileNumber')
            }],
            callback: function (records, operation, success) {
                me.application.fireEvent('webservicecallcomplete', operation, 'personinfo.Dependents');
            },
            scope: me
        });

        me.getPersoninfoAllRelationshipsStore().load({
            filters: [{
                property: 'ptcpntId',
                value: selectedPerson.get('participantId')
            }],
            callback: function (records, operation, success) {
                me.application.fireEvent('webservicecallcomplete', operation, 'personinfo.AllRelationships');
            },
            scope: me
        });
    },

    updateCRMFlashesHeader: function (flashStore) {
        var flashesText = '',
            flashesToolTip = '';

        flashStore.each(function (record) {
            flashesText += record.get('flashName').replace(/^\s+|\s+$/g, "") + '; ';

            if (flashesText.length > 16) {
                flashesText = flashesText.slice(0, 14) + '[...]';
            }
            flashesToolTip += record.get('flashName').replace(/^\s+|\s+$/g, "") + '\n';
        });

        if (parent && parent.SetHeaderField) {
            parent.SetHeaderField('va_flashes', flashesText, null, 'solid 1px blue', 'blue', null, null, flashesToolTip);;
        }
    },

    GetSelectedDependentRecord: function (promptIfNotSelected) {
        var me = this,
            dependentGrid = me.getDepGrid(),
            selectedRecord = dependentGrid.getSelectionModel().getSelection(),
            record;

        if (Ext.isEmpty(selectedRecord)) {
            if (promptIfNotSelected) {
                Ext.Msg.alert('No Dependent Selected', 'Please select a dependent from the grid first.');
            }
            return null;
        }

        record = selectedRecord[0];
        return record;
    },

    /**
    * This function fires off when the service request button is clicked in the Claim toolbar without selecting an option from the dropdown
    * Default is the 0820 option
    */
    serviceRequest: function (button) {
        var me = this,
			defaultSelection = button.defaultMenuSelection,
            selectedDependent = me.GetSelectedDependentRecord(true);

        if (selectedDependent == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else if (!Ext.isEmpty(defaultSelection)) {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Dependent service request.', function (button) {
                if (button == 'no') return;
                else me.fireDependentServiceRequestEvent({ name: defaultSelection.text, value: parseInt(defaultSelection.value) }, selectedDependent);
            });
        }
    },

    /**
    * This function fires off when a service request menu button is clicked in the Claim toolbar.
    */
    serviceRequestMenuClick: function (menu, item, e, eOpts) {
        var me = this,
            selectedDependent = me.GetSelectedDependentRecord(true);

        if (selectedDependent == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else if (!Ext.isEmpty(item)) {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Dependent service request.', function (button) {
                if (button == 'no') return;
                else me.fireDependentServiceRequestEvent({ name: item.text, value: parseInt(item.value) }, selectedDependent);
            });
        }
    },

    /**
    * This function is called from the service request menu click.  It will gather the information from the selected award and 
    * and detail panel and fire off the service request event in the Service Request controller.
    */
    fireDependentServiceRequestEvent: function (serviceRequestType, selectedDependent) {
        var me = this,
            dependentStore = me.getPersoninfoDependentsStore(),
            dependentAddressesOfBirth = '',
            dependentNames = '',
            data;

        dependentStore.each(function (record) {
            var name = '',
                address = '';
            if (record.get('cityOfBirth')) address = record.get('cityOfBirth');
            if (record.get('stateOfBirth')) address += ' ' + record.get('stateOfBirth');
            if (record.get('firstName')) name = record.get('firstName');
            if (record.get('middleName')) name += ' ' + record.get('middleName');
            if (record.get('lastName')) name += ' ' + record.get('lastName');

            dependentNames += name + '\n';
            dependentAddressesOfBirth += address + '\n';
        });

        data = {
            "va_SRDOB": Ext.Date.format(selectedDependent.get('dob'), "m/d/Y"),
            "va_SRDOBText": selectedDependent.get('dob'),
            "va_SREmail": selectedDependent.get('emailAddress'),
            "va_SRFirstName": selectedDependent.get('firstName'),
            "va_SRLastName": selectedDependent.get('lastName'),
            "va_SRGender": selectedDependent.get('gender'),
            "va_SRRelation": selectedDependent.get('relationship'),
            "va_SRSSN": selectedDependent.get('ssn'),
            "va_DependentAddresses": dependentAddressesOfBirth,
            "va_DependentNames": dependentNames
        };

        //Add Global Vars 2/23/13
        me.application.serviceRequest.va_SelectedPayeeCode = '';
        me.application.serviceRequest.va_ServiceRequestType = 'Dependent';
        me.application.serviceRequest.va_SelectedSSN = selectedDependent.get('ssn');
        me.application.serviceRequest.va_SelectedPID = selectedDependent.get('participantId');

        //This event is caught in the Service Request controller
        me.application.fireEvent('createdependentservicerequest', serviceRequestType, data);
    },

    viewDependentsContactButtonClick: function () {
        var me = this;

        me.viewContact('Dependent');
    },

    viewAllRelationshipsContactButtonClick: function () {
        var me = this;

        me.viewContact('AllRelationships');
    },

    intentToFileButtonClick: function () {
        var me = this;

        var selGrid = me.getAllRelationsGrid();

        var selectedRecord = selGrid.getSelectionModel().getSelection();
        var selection = null;

        if (selectedRecord || selectedRecord.length > 0) {
            if (typeof selectedRecord[0] != "undefined") {
                var rec = selectedRecord[0];
                selection = {
                    "ptcpntId": rec.get('participantId'),
                    "firstname": rec.get('firstName'),
                    "middlename": rec.get('middleName'),
                    "lastname": rec.get('lastName'),
                    "SSN": rec.get('ssn'),
                    "dod": rec.get('dod') && rec.get('dod').toString().length > 0 ? new Date(rec.get('dod')) : null,
                    "dob": rec.get('dob') && rec.get('dob').toString().length > 0 ? new Date(rec.get('dob')) : null
                };
            }
        }

        if (!parent || !parent._IntentToFile) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return;
        }

        parent._IntentToFile(selection);
    },

    // handle View Contact
    viewContact: function (gridToUse) {
        var me = this;

        var selGrid;

        if (gridToUse == null || gridToUse == 'Dependent') {
            selGrid = me.getDepGrid();
        } else if (gridToUse == 'AllRelationships') {
            selGrid = me.getAllRelationsGrid();
        } else {
            alert('Invalid Parameter Passed.');
            return;
        }


        var selectedRecord = selGrid.getSelectionModel().getSelection();
        if (!selectedRecord || selectedRecord.length == 0) {
            alert('No record is selected.');
            return;
        }

        var rec = selectedRecord[0];
        var selection = null;

        selection = {
            "ptcpntId": rec.get('participantId'),
            "firstname": rec.get('firstName'),
            "middlename": rec.get('middleName'),
            "lastname": rec.get('lastName'),
            "SSN": rec.get('ssn'),
            "dod": rec.get('dod') && rec.get('dod').toString().length > 0 ? new Date(rec.get('dod')) : null,
            "dob": rec.get('dob') && rec.get('dob').toString().length > 0 ? new Date(rec.get('dob')) : null
        };

        if (!parent || !parent._ViewContact) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return;
        }
        parent._ViewContact(selection);

    },

    // receive event from BIRLS tab and show claims folder SOJ
    onUpdatedClaimProcessingTimeLabel: function (claimFolderLocation) {
        var me = this;
        me.getClaimFolderText().setValue(claimFolderLocation);
    },

    // handle View Proc Time using BIRLS SOJ folder location
    onClaimProcessingTimesFieldSetButtonClick: function () {
        var me = this;
        var data = me.getBirlsStore().data;
        var sojCode = null;
        if (data.length > 0) {
            sojCode = data.getAt(0).data.claimFolderLocation;
        }

        if (!sojCode) {
            Ext.Msg.alert('Average Claim Processing Times', 'No SOJ Id was found, processing times can not be found.');
        } else {
            var claimsProc = Ext.create('VIP.services.ClaimsProcessing');
            claimsProc.DisplayClaimProcessingTimes(sojCode);
        }
    },

    // handle button click on Main Claims grid to display View Proc Time using BIRLS SOJ folder location
    onClaimProcessingTimesMainClaimButtonClick: function (soj) {
        var me = this;

        var sojCode = soj;

        if (Ext.isEmpty(sojCode)) {
            var data = me.getBirlsStore().data;
            if (data.length > 0) {
                sojCode = data.getAt(0).data.claimFolderLocation;
            }
        }

        if (!sojCode) {
            Ext.Msg.alert('Average Claim Processing Times', 'No SOJ Id was found, processing times can not be found.');
        } else {
            var claimsProc = Ext.create('VIP.services.ClaimsProcessing');
            claimsProc.DisplayClaimProcessingTimes(sojCode, 'mainclaim');
        }
    },

    onViewMoreFiduciaryData: function (ptcpntId) {
        var me = this;
        //Run findVeteranByPtcpntId .. Since PID is supplied, CorpStore will automatically determine that it needs to search by PID 

        me.getCorpStore().load({
            filters: [{
                property: 'ptcpntId',
                value: ptcpntId
            }],
            callback: function (records, operation, success) {
                me.application.fireEvent('webservicecallcomplete', operation);
                if (records[0].data) {
                    var fiduciary = records[0];

                    var message = '';
                    message += '<b>Name:</b> ' + fiduciary.get('firstName') + ' ' + fiduciary.get('middleName') + ' ' + fiduciary.get('lastName') + '<br />';

                    if (fiduciary.get('addressLine1') && fiduciary.get('addressLine1') != '') {
                        message += '<b>Address:</b> <br />' + fiduciary.get('addressLine1') + '<br />';
                    }
                    if (fiduciary.get('addressLine2') && fiduciary.get('addressLine2') != '') {
                        message += fiduciary.get('addressLine2') + '<br />';
                    }
                    if (fiduciary.get('addressLine3') && fiduciary.get('addressLine3') != '') {
                        message += fiduciary.get('addressLine3') + '<br />';
                    }

                    Ext.Msg.alert('Additional Fiduciary Info', message);
                } else {
                    Ext.Msg.alert('Additional Fiduciary Info', 'No additional information was found.');
                }
            },
            scope: me
        });
    },

    onRedrawCADDFields: function (ptcpntId) {
        var me = this;

        //update Address grid (child tab under Person Info)
        me.getPersoninfoAddressesStore().load({
            filters: [{
                property: 'ptcpntId',
                value: ptcpntId
            }],
            callback: function (records, operation, success) {
                if (success && !Ext.isEmpty(records)) {
                    me.getAddressGrid().reconfigure(me.getPersoninfoAddressesStore()); //TODO: use records[0].addresses() ??
                }
                me.application.fireEvent('webservicecallcomplete', operation, 'personinfo.Addresses');
            },
            scope: me
        });

        //update General panel as phone numbers might have changed
        me.getCorpStore().load({
            filters: [{
                property: 'ptcpntId',
                value: ptcpntId
            }],
            callback: function (records, operation, success) {
                if (success && !Ext.isEmpty(records)) {
                    me.getCorpDetails().getForm().reset();
                    me.getCorpDetails().loadRecord(records[0]);
                }
                me.application.fireEvent('webservicecallcomplete', operation);
            },
            scope: me
        });
    },

    recordIsLoaded: function (promptIfNotSelected) {
        var me = this,
            corpStore = me.getCorpStore(); //Corp Store
        if (Ext.isEmpty(corpStore.data) || Ext.isEmpty(corpStore.data.items) || corpStore.getCount() == 0) {
            if (promptIfNotSelected) {
                Ext.Msg.alert('Corporate Store is not loaded.', 'Please Execute a Search first.');
            }
            return null;
        }

        var corpRecord = corpStore.data.items[0].data;
        return corpRecord;
    },

    personServiceRequest: function () {
        me = this,
        corpVetRecord = me.recordIsLoaded(true);

        if (corpVetRecord == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Corp service request', function (button) {
                if (button == 'no') return;
                else me.fireServiceRequestEvent(corpVetRecord);
            });
        }
    },

    fireServiceRequestEvent: function (corpVetRecord) {
        var me = this,
        //This json object will be sent to the CRM Restkit
        data = {
            "va_SRFirstName": corpVetRecord.firstName,
            "va_SRLastName": corpVetRecord.lastName,
            "va_SRSSN": corpVetRecord.ssn,
            "va_SRDOBText": corpVetRecord.dob,
            "va_mailing_address1": corpVetRecord.addressLine1,
            "va_mailing_address2": corpVetRecord.addressLine2,
            "va_mailing_address3": corpVetRecord.addressLine3,
            "va_mailing_city": corpVetRecord.city,
            "va_mailing_state": corpVetRecord.state,
            "va_mailing_zip": corpVetRecord.zipCode,
            "va_DayPhone": corpVetRecord.fullPhone1,
            "va_EveningPhone": corpVetRecord.fullPhone2,
            "va_MailingCountry": corpVetRecord.country,
            "va_SREmail": corpVetRecord.emailAddress,
            "va_EmailofVeteran": corpVetRecord.emailAddress,
            "va_DateofDeath": Ext.Date.format(new Date(corpVetRecord.dod), 'm/d/Y'),
            "va_AwardBenefitType": null,
            "va_CurrentMonthlyRate": {
                Value: null
            },
            "va_NetAmountPaid": {
                Value: null
            },
            "va_EffectiveDate": null
        };

        //Added Global Vars 2/23/13
        me.application.serviceRequest.va_SelectedPayeeCode = '00'
        me.application.serviceRequest.va_ServiceRequestType = 'PERSON';
        me.application.serviceRequest.va_SelectedSSN = corpVetRecord.ssn;
        me.application.serviceRequest.va_SelectedPID = corpVetRecord.participantId;

        me.application.fireEvent('createpersonservicerequest', data);
    },

    /**
    * This function fires off when the VAI button is clicked in the BIRLS toolbar   
    */
    createCorpVAI: function (button) {
        var me = this,
            corpVetRecord = me.recordIsLoaded(true);

        if (corpVetRecord == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a CORP VAI', function (button) {
                if (button == 'no') return;
                else me.fireCorpVaiEvent(corpVetRecord);
            });
        }
    },

    fireCorpVaiEvent: function (corpVetRecord) {
        var me = this,
            selectionVariables = [];
        /*selectionVariables.Location
          selectionVariables.PID
          selectionVariables.SSN
          selectionVariables.PayeeCode*/

        //Add Global Vars for VAI params
        selectionVariables.PayeeCode = '00';
        selectionVariables.Location = 'CORP';
        selectionVariables.SSN = corpVetRecord.ssn;
        selectionVariables.PID = corpVetRecord.participantId;

        me.application.fireEvent('createcrmvai', selectionVariables, corpVetRecord);

        Ext.log('A Create VAI event was fired by the PersonInfo controller');
    },

    /**
    * This function fires off when the VAI button is clicked in the BIRLS toolbar   
    */
    createDependentVAI: function (button) {
        var me = this,
            selectedDependent = me.GetSelectedDependentRecord(true);

        if (selectedDependent == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Dependent VAI', function (button) {
                if (button == 'no') return;
                else me.fireDependentVaiEvent(selectedDependent);
            });
        }
    },

    fireDependentVaiEvent: function (selectedDependent) {
        var me = this,
            selectionVariables = [];
        /*selectionVariables.Location
          selectionVariables.PID
          selectionVariables.SSN
          selectionVariables.PayeeCode*/

        //Add Global Vars for VAI params
        selectionVariables.PayeeCode = null;
        selectionVariables.Location = 'DEPENDENT';
        selectionVariables.SSN = selectedDependent.get('ssn');
        selectionVariables.PID = selectedDependent.get('participantId');

        me.application.fireEvent('createcrmvai', selectionVariables, selectedDependent);

        Ext.log('A Create VAI event was fired by the Dependent controller');
    }
});