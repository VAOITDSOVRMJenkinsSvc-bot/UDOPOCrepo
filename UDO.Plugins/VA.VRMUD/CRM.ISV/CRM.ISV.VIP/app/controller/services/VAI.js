/*
* @class VIP.controller.services.VAI
* The controller for the VAI creation event.
*/

Ext.define('VIP.controller.services.VAI', {
    extend: 'Ext.app.Controller',

    stores: ['Birls', 'Corp', 'personinfo.Addresses', 'FiduciaryPoa'],

    mixins: {
        soj: 'VIP.mixin.StationOfJurisdiction', //getStationOfJurisdiction
        store: 'VIP.mixin.Stores' //loadStore
    },

    refs: [{
        ref: 'corpDetails', //CORP BOS
        selector: '[xtype="personinfo.corp"]'
    }, {
        ref: 'personInfoGeneralDetails',    //PID
        selector: '[xtype="personinfo.details.generaldetails"]'
    },

    //{
    //    ref: 'claimFolderLocationButton', //SOJ
    //    selector: '[xtype="personinfo.details.generaldetails"] > fieldcontainer > button[action="claimfolderlocation"]'
    //},

    {
        ref: 'claimFolderLocationText', //SOJ
        selector: '[xtype="personinfo.details.generaldetails"] > fieldcontainer > displayfield[name="claimFolderLocation"]'
    },

    {
        ref: 'folderLocationsList', //Not Used Yet
        selector: '[xtype="birls.birlsdetails.folderlocinfo.folderlocationlist"]'
    }],

    dependentData: {
        addressStore: [],
        fiduciaryPoaStore: [],
        corpStore: []
    },

    init: function () {
        var me = this;
        me.application.on({
            createcrmvai: me.onInitiateVAI,
            scope: me
        });
        me.callParent();
        Ext.log('The VAI controller has been initialized');
    },

    /* This function is the handler for the VAI creation through CRM UI. Will have to call a function
    * in the parent form and that iterates through JSON object and populates fields in VAI form.
    */
    //First Step: Determine if Phone Call is saved
    onInitiateVAI: function (selectionVariables, selectedRecord) {
        var me = this,
            crmForm = null,
            vipData = null,
            crmFormType = null,
            crmEntityName = null,
            crmFormId = null;

        //Set flag to false unless Reload function is called
        selectionVariables.loadingDependent = false;

        if (parent && parent.Xrm) {
            crmEntityName = parent.Xrm.Page.data.entity.getEntityName();
            crmFormType = parent.Xrm.Page.ui.getFormType();
            crmFormId = parent.Xrm.Page.data.entity.getId();
            crmForm = {
                Id: crmFormId,
                LogicalName: crmEntityName,
                Name: crmEntityName == 'contact' ? parent.Xrm.Page.getAttribute('fullname').getValue() : parent.Xrm.Page.getAttribute('subject').getValue()
            };
        }
        //Save Prompt
        if ((crmFormId == null) || (crmFormType == 1)) {
            Ext.Msg.confirm('CRM Form Must Be Saved',
                'The Phone Call must be saved prior to creating a new VAI. Would you like to save it now?', function (button) {
                    if (button == 'no') return;
                    else parent.Xrm.Page.data.entity.save();
                });
        }
        else {
            //Check initiation location to possibly reload stores
            switch (selectionVariables.Location) {
                case 'DEPENDENT':
                case 'AWARD':
                    if (selectionVariables.PayeeCode !== '00') {
                        me.reloadStores(selectionVariables, selectedRecord, crmForm);
                    }
                    else {
                        me.finalTasks(selectionVariables, selectedRecord, crmForm);
                    }
                    return;
                case 'CLAIM':
                    if (selectionVariables.PayeeCode !== '00') {
                        me.reloadStores(selectionVariables, selectedRecord, crmForm);
                    }
                    else {
                        me.finalTasks(selectionVariables, selectedRecord, crmForm);
                    }
                    return;
                case 'CORP':
                case 'BIRLS':
                default:
                    me.finalTasks(selectionVariables, selectedRecord, crmForm);
                    return;
            }
        }
    },

    //Second Step: Reload Stores if Payee Code !== '00'
    reloadStores: function (selectionVariables, selectedRecord, crmForm) {
        var me = this;
        //Set flag to true because attempting to reload stores
        selectionVariables.loadingDependent = true;
        //Declare Corp store
        me.dependentData.corpStore = Ext.create("VIP.store.Corp");
        var corpFilters = [{
            property: 'ptcpntId',
            value: selectionVariables.PID
        }];

        //Declare Address store
        me.dependentData.addressStore = Ext.create("VIP.store.personinfo.Addresses");
        var addressFilters = [{
            property: 'ptcpntId',
            value: selectionVariables.PID
        }];

        //Declare Fiduciary/POA store
        me.dependentData.fiduciaryPoaStore = Ext.create("VIP.store.FiduciaryPoa");
        var fiduciaryPoaFilters = [{
            property: 'fileNumber',
            value: selectionVariables.SSN
        }];

        //Load Dependent Stores
        $.when(
            me.loadStore(me.dependentData.corpStore, corpFilters)
        ).then(function () {
            $.when(
                me.loadStore(me.dependentData.addressStore, addressFilters),
                me.loadStore(me.dependentData.fiduciaryPoaStore, fiduciaryPoaFilters)
            ).then(function () {
                me.finalTasks(selectionVariables, selectedRecord, crmForm);
            }).fail(function () {
                alert('Failed to load dependent address/fiduciary/poa store.');
            });
        }).fail(function () {
            alert('Failed to load dependent corp store.');
        });
    },

    //Third Step: Final set of tasks after loading or not loading the dependent store
    finalTasks: function (selectionVariables, selectedRecord, crmForm) {
        var me = this,
            vaiRecord = "";

        me.dependentData.vipData = me.createVAIobject(selectionVariables, selectedRecord, crmForm);
        if (me.dependentData.vipData) {
            vaiRecord = (typeof parent.UTIL.CreateEntity === "function") ? parent.UTIL.CreateEntity("va_vai", me.dependentData.vipData) : "none";
            if (vaiRecord === "none") {
                alert("Could not find CreateEntity function, please refresh page and try again or contact HelpDesk.");
                return;
            }
            else {
                me.createVAI(vaiRecord);
            }
        }
    },
    //Fourth Step: Create JSON object with CRM schema names as keys and the proper types as values
    createVAIobject: function (selectionVariables, selectedRecord, crmForm) {
        var me = this;
        var data = {    //JSON object 'data' for VAI record
            va_DateOpened: Ext.Date.format(new Date(), "m/d/Y g:i a"),
            va_vainumber: '',
            va_ParticipantID: ''
            //TransactionCurrencyId: me.getUsCurrencyRecord()
        };
        //General Section - VAI Inquiry Number
        data.va_vainumber = me.createVAINumber();
        if ((me.getPersonInfoGeneralDetails()) && (me.getPersonInfoGeneralDetails().items)) {
            data.va_ParticipantID = me.getPersonInfoGeneralDetails().items.get("participantId").value;
        }

        //Originating Call & Veteran
        if (crmForm.LogicalName == 'phonecall') {
            data.va_OriginatingCall = crmForm;
            if (parent.Xrm.Page.getAttribute('regardingobjectid').getValue() && parent.Xrm.Page.getAttribute('regardingobjectid').getValue() != null && parent.Xrm.Page.getAttribute('regardingobjectid').getValue() != undefined) {
                data.va_Veteran = {
                    Id: parent.Xrm.Page.getAttribute('regardingobjectid').getValue()[0].id,
                    LogicalName: 'contact',
                    Name: parent.Xrm.Page.getAttribute('regardingobjectid').getValue()[0].name
                };
            }
        }
        else if (crmForm.LogicalName == 'contact') {    //Assuming the contact is created from the phone call
            data.va_Veteran = crmForm;
            if (parent.window.parent.opener.Xrm.Page.data && parent.window.parent.opener.Xrm.Page.data.entity.getId()) {
                data.va_OriginatingCall = {
                    Id: parent.window.parent.opener.Xrm.Page.data.entity.getId(),
                    LogicalName: 'phonecall',
                    Name: parent.window.parent.opener.Xrm.Page.getAttribute('subject').getValue()
                };
            }
        }

        var vetCorpStore = '',
            vetCorpBOS = '',
            vetBirlsStore = '',
            vetBirlsBOS = '',
            vetAddressStore = '',
            vetEmail = '',
            vetFiduciaryPoaStore = '',
            vetCurrentFiduciaryStore = '',
            vetCurrentPoaStore = '',
            depCorpStore = '',
            depAddressStore = '',
            depEmail = '',
            depFidPoaStore = '',
            depCurrentFiduciaryStore = '',
            depCurrentPoaStore = '';

        //Setting Veteran Stores
        if ((me.getCorpStore()) && (me.getCorpStore().first()) && (me.getCorpStore().first().data)) {
            vetCorpStore = me.getCorpStore().first().data;
        }

        if ((me.getBirlsStore()) && (me.getBirlsStore().first()) && (me.getBirlsStore().first().data)) {
            vetBirlsStore = me.getBirlsStore().first().data;
        }

        if ((me.getBirlsStore()) && (me.getBirlsStore().first()) && (me.getBirlsStore().first().servicesStore) && (me.getBirlsStore().first().servicesStore.data) && (me.getBirlsStore().first().servicesStore.data.length) && (me.getBirlsStore().first().servicesStore.data.length > 0) && (me.getBirlsStore().first().servicesStore.data.items)) {
            var latestEAD = '';
            for (i in me.getBirlsStore().first().servicesStore.data.items) {
                if (me.getBirlsStore().first().servicesStore.data.items[i].data.enteredOnDutyDate > latestEAD) {
                    latestEAD = me.getBirlsStore().first().servicesStore.data.items[i].data.enteredOnDutyDate;
                    vetBirlsBOS = me.getBirlsStore().first().servicesStore.data.items[i].data.branchOfService;
                }
            }
        }
        if ((me.getCorpDetails()) && (me.getCorpDetails().items) && (me.getCorpDetails().items.get('branchOfService1'))) {
            vetCorpBOS = me.getCorpDetails().items.get('branchOfService1').value;
        }

        if ((me.getPersoninfoAddressesStore()) && (me.getPersoninfoAddressesStore().data) && (me.getPersoninfoAddressesStore().data.length) && (me.getPersoninfoAddressesStore().data.length > 0) && (me.getPersoninfoAddressesStore().data.items)) {
            for (var i in me.getPersoninfoAddressesStore().data.items) {
                if (me.getPersoninfoAddressesStore().data.items[i].data.participantAddressTypeName == 'Mailing') {
                    vetAddressStore = me.getPersoninfoAddressesStore().data.items[i].data;
                }
                if (me.getPersoninfoAddressesStore().data.items[i].data.participantAddressTypeName == 'Email') {
                    vetEmail = me.getPersoninfoAddressesStore().data.items[i].data.emailAddress;
                }
            }
        }

        if ((me.getFiduciaryPoaStore()) && (me.getFiduciaryPoaStore().first())) {
            vetFiduciaryPoaStore = me.getFiduciaryPoaStore().first();
            if (vetFiduciaryPoaStore.data) {
                //Check for Vet Fiduciary
                if (parseInt(vetFiduciaryPoaStore.data.numberOfFiduciaries) > 0) {
                    //Check if Current Fiduciaries Store is populated
                    if ((vetFiduciaryPoaStore.currentFiduciaryStore) && (vetFiduciaryPoaStore.currentFiduciaryStore.data) && (vetFiduciaryPoaStore.currentFiduciaryStore.data.length) && (vetFiduciaryPoaStore.currentFiduciaryStore.data.length > 0)) {
                        if (vetFiduciaryPoaStore.currentFiduciaryStore.first()) {
                            vetCurrentFiduciaryStore = vetFiduciaryPoaStore.currentFiduciaryStore.first().data;
                        }
                    }
                }
                //Check for Vet POA
                if (parseInt(vetFiduciaryPoaStore.data.numberOfPOA) > 0) {
                    //Check if Current POA Store is populated
                    if ((vetFiduciaryPoaStore.currentPoaStore) && (vetFiduciaryPoaStore.currentPoaStore.data) && (vetFiduciaryPoaStore.currentPoaStore.data.length) && (vetFiduciaryPoaStore.currentPoaStore.data.length > 0)) {
                        if (vetFiduciaryPoaStore.currentPoaStore.first()) {
                            vetCurrentPoaStore = vetFiduciaryPoaStore.currentPoaStore.first().data;
                        }
                    }
                }
            }
        }

        //Setting Dependent Stores
        if ((me.dependentData) && (!Ext.isEmpty(me.dependentData.corpStore)) && (typeof me.dependentData.corpStore.first === 'function') && (me.dependentData.corpStore.first()) && (me.dependentData.corpStore.first().data)) {
            depCorpStore = me.dependentData.corpStore.first().data;
        }
        if ((me.dependentData) && (!Ext.isEmpty(me.dependentData.addressStore)) && (me.dependentData.addressStore.data) && (me.dependentData.addressStore.data.length) && (me.dependentData.addressStore.data.length > 0) && (me.dependentData.addressStore.data.items)) {
            for (var i in me.dependentData.addressStore.data.items) {
                if (me.dependentData.addressStore.data.items[i].data.participantAddressTypeName == 'Mailing') {
                    depAddressStore = me.dependentData.addressStore.data.items[i].data;
                }
                if (me.dependentData.addressStore.data.items[i].data.participantAddressTypeName == 'Email') {
                    depEmail = me.dependentData.addressStore.data.items[i].data.emailAddress;
                }
            }
        }
        if ((me.dependentData) && (!Ext.isEmpty(me.dependentData.fiduciaryPoaStore)) && (typeof me.dependentData.fiduciaryPoaStore.first === 'function') && (me.dependentData.fiduciaryPoaStore.first())) {
            depFidPoaStore = me.dependentData.fiduciaryPoaStore.first();
            if (depFidPoaStore.data) {
                //Check for Dep Fiduciary
                if (parseInt(depFidPoaStore.data.numberOfFiduciaries) > 0) {
                    //Check if Current Fiduciaries Store is populated
                    if ((depFidPoaStore.currentFiduciaryStore) && (depFidPoaStore.currentFiduciaryStore.data) && (depFidPoaStore.currentFiduciaryStore.data.length) && (depFidPoaStore.currentFiduciaryStore.data.length > 0)) {
                        if (depFidPoaStore.currentFiduciaryStore.first()) {
                            depCurrentFiduciaryStore = depFidPoaStore.currentFiduciaryStore.first().data;
                        }
                    }
                }
                //Check for Dep POA
                if (parseInt(depFidPoaStore.data.numberOfPOA) > 0) {
                    //Check if Current POA Store is populated
                    if ((depFidPoaStore.currentPoaStore) && (depFidPoaStore.currentPoaStore.data) && (depFidPoaStore.currentPoaStore.data.length) && (depFidPoaStore.currentPoaStore.data.length > 0)) {
                        if (depFidPoaStore.currentPoaStore.first()) {
                            depCurrentPoaStore = depFidPoaStore.currentPoaStore.first().data;
                        }
                    }
                }
            }
        }

        //Veteran Information Section - From CORP store unless initiated from BIRLS        
        if (vetCorpStore) {     //Use Corp, unless empty then try Birls
            data.va_VeteranFirstName = ((vetCorpStore.firstName != '') ? vetCorpStore.firstName : ((vetBirlsStore) ? vetBirlsStore.firstName : ''));
            data.va_VeteranMiddleName = ((vetCorpStore.middleName != '') ? vetCorpStore.middleName : ((vetBirlsStore) ? vetBirlsStore.middleName : ''));
            data.va_VeteranLastName = ((vetCorpStore.lastName != '') ? vetCorpStore.lastName : ((vetBirlsStore) ? vetBirlsStore.lastName : ''));
            data.va_VeteranSuffix = ((vetCorpStore.suffixName != '') ? vetCorpStore.suffixName : ((vetBirlsStore) ? vetBirlsStore.nameSuffix : ''));
            data.va_VeteranBOS = ((vetCorpBOS != '') ? vetCorpBOS : ((vetBirlsBOS != '') ? vetBirlsBOS : ''));
            data.va_VeteranDOB = ((vetCorpStore.dob != '') ? vetCorpStore.dob : ((vetBirlsStore) ? vetBirlsStore.dob : ''));
            data.va_VeteranDOD = ((vetCorpStore.dod != '') ? vetCorpStore.dod : ((vetBirlsStore) ? vetBirlsStore.dod : ''));
            data.va_VeteranFileNumber = ((vetCorpStore.fileNumber != '') ? vetCorpStore.fileNumber : ((vetBirlsStore) ? vetBirlsStore.fileNumber : ''));
            data.va_VeteranSSN = ((vetCorpStore.ssn != '') ? vetCorpStore.ssn : ((vetBirlsStore) ? vetBirlsStore.ssn : ''));
        }
        if ((selectionVariables.Location == 'BIRLS') && (vetBirlsStore)) {    //Created from BIRLS, Use Birls, unless empty, then try Corp
            data.va_VeteranFirstName = ((vetBirlsStore.firstName != '') ? vetBirlsStore.firstName : ((vetCorpStore) ? vetCorpStore.firstName : ''));
            data.va_VeteranMiddleName = ((vetBirlsStore.middleName != '') ? vetBirlsStore.middleName : ((vetCorpStore) ? vetCorpStore.middleName : ''));
            data.va_VeteranLastName = ((vetBirlsStore.lastName != '') ? vetBirlsStore.lastName : ((vetCorpStore) ? vetCorpStore.lastName : ''));
            data.va_VeteranSuffix = ((vetBirlsStore.nameSuffix != '') ? vetBirlsStore.nameSuffix : ((vetCorpStore) ? vetCorpStore.suffixName : ''));
            data.va_VeteranBOS = ((vetBirlsBOS != '') ? vetBirlsBOS : ((vetCorpBOS != '') ? vetCorpBOS : ''));
            data.va_VeteranDOB = ((vetBirlsStore.dob != '') ? vetBirlsStore.dob : ((vetCorpStore) ? vetCorpStore.dob : ''));
            data.va_VeteranDOD = ((vetBirlsStore.dod != '') ? vetBirlsStore.dod : ((vetCorpStore) ? vetCorpStore.dod : ''));
            data.va_VeteranFileNumber = ((vetBirlsStore.fileNumber != '') ? vetBirlsStore.fileNumber : ((vetCorpStore) ? vetCorpStore.fileNumber : ''));
            data.va_VeteranSSN = ((vetBirlsStore.ssn != '') ? vetBirlsStore.ssn : ((vetCorpStore) ? vetCorpStore.ssn : ''));
        }

        //Inquirer Information Section & Setting Email
        if ((selectedRecord) && (selectedRecord.data)) { //AWARD/CLAIM/DEPENDENT from selected Record
            switch (selectionVariables.Location) {
                case 'AWARD':
                    var InquirerStore = '';
                    switch (selectionVariables.PayeeCode) {
                        case '00':
                            InquirerStore = vetCorpStore;
                            data.va_Email = vetEmail;
                            data.va_InquirerRelationshiptoVeteran = 'Self';
                            break;
                        case '10':
                            InquirerStore = depCorpStore;
                            data.va_Email = depEmail;
                            data.va_InquirerRelationshiptoVeteran = 'Spouse';
                            break;
                        default:
                            InquirerStore = depCorpStore;
                            data.va_Email = depEmail;
                            data.va_InquirerRelationshiptoVeteran = 'Dependent';
                            break;
                    }
                    data.va_InquirerFirstName = InquirerStore.firstName;
                    data.va_InquirerMiddleName = InquirerStore.middleName;
                    data.va_InquirerLastName = InquirerStore.lastName;
                    data.va_InquirerSuffix = InquirerStore.suffixName;
                    break;
                case 'CLAIM':
                    switch (selectionVariables.PayeeCode) {
                        case '00':
                            data.va_Email = vetEmail;
                            data.va_InquirerRelationshiptoVeteran = 'Self';
                            break;
                        case '10':
                            data.va_Email = depEmail;
                            data.va_InquirerRelationshiptoVeteran = 'Spouse';
                            break;
                        default:
                            data.va_Email = depEmail;
                            data.va_InquirerRelationshiptoVeteran = 'Dependent';
                            break;
                    }
                    data.va_InquirerFirstName = selectedRecord.data.claimantFirstName;
                    data.va_InquirerMiddleName = selectedRecord.data.claimantMiddleName;
                    data.va_InquirerLastName = selectedRecord.data.claimantLastName;
                    data.va_InquirerSuffix = selectedRecord.data.claimantSuffix;
                    break;
                case 'DEPENDENT':
                    data.va_Email = depEmail;
                    data.va_InquirerFirstName = selectedRecord.data.firstName;
                    data.va_InquirerMiddleName = selectedRecord.data.middleName;
                    data.va_InquirerLastName = selectedRecord.data.lastName;
                    data.va_InquirerRelationshiptoVeteran = ((selectedRecord.data.relationship != '') ? selectedRecord.data.relationship : 'Spouse or Dependent');
                    break;
            }
        }
        else {  //OR CORP/BIRLS
            data.va_InquirerRelationshiptoVeteran = 'Self';
            data.va_InquirerFirstName = data.va_VeteranFirstName;
            data.va_InquirerMiddleName = data.va_VeteranMiddleName;
            data.va_InquirerLastName = data.va_VeteranLastName;
            data.va_InquirerSuffix = data.va_VeteranSuffix;
            data.va_Email = vetEmail;
        }

        //Requestor Information Section: Mappings based on va_CallerRelationtoVeteran (Check field to determine if on PC)
        if ((parent.Xrm.Page.getAttribute('va_callerrelationtoveteran') != null) && (parent.Xrm.Page.getAttribute('va_callerrelationtoveteran').getValue())) {
            switch (parent.Xrm.Page.getAttribute('va_callerrelationtoveteran').getValue()) {
                case 953850000: //Self - Based on Veteran
                    data.va_RequestorFirstName = data.va_VeteranFirstName;
                    data.va_RequestorMiddleName = data.va_VeteranMiddleName;
                    data.va_RequestorLastName = data.va_VeteranLastName;
                    data.va_RequestorSuffix = data.va_VeteranSuffix;
                    if ((vetCorpStore != '') && (vetCorpStore.fullPhone1)) {
                        data.va_RequestorPhone = vetCorpStore.fullPhone1;
                    }
                    setRequestorAddress(vetAddressStore);
                    break;
                case 953850001: //Spouse
                case 953850002: //Dependent
                    //Based from selected Recipient, Claimant or Dependent
                    data.va_RequestorFirstName = data.va_InquirerFirstName;
                    data.va_RequestorMiddleName = data.va_InquirerMiddleName;
                    data.va_RequestorLastName = data.va_InquirerLastName;
                    data.va_RequestorSuffix = data.va_InquirerSuffix;
                    var RequestorAddressStore = '';

                    if ((selectionVariables.PayeeCode !== '00') && (depAddressStore != '') && (selectionVariables.loadingDependent === true)) {  //Dependent bec check is true
                        RequestorAddressStore = depAddressStore;
                    }
                    else { //Must be 00
                        RequestorAddressStore = vetAddressStore;
                    }
                    //areaNumber1 + phoneNumberOne || fullPhone1
                    if ((selectionVariables.PayeeCode !== '00') && (depCorpStore != '') && (depCorpStore.fullPhone1)) {
                        data.va_RequestorPhone = depCorpStore.fullPhone1;
                    }
                    else if ((vetCorpStore != '') && (vetCorpStore.fullPhone1)) {
                        data.va_RequestorPhone = vetCorpStore.fullPhone1;
                    }
                    setRequestorAddress(RequestorAddressStore);
                    break;
                case 953850008: //Fiduciary      
                    var RequestorFiduciaryStore = '';
                    if ((selectionVariables.PayeeCode !== '00') && (depCurrentFiduciaryStore != '') && (selectionVariables.loadingDependent === true)) {  //Dependent bec check is true
                        RequestorFiduciaryStore = depCurrentFiduciaryStore;
                    }
                    else {  //Ok for Dependent VAIs to use Vet Fid, if can't find Dep Fid
                        RequestorFiduciaryStore = vetCurrentFiduciaryStore;
                    }

                    if ((RequestorFiduciaryStore != '') && (RequestorFiduciaryStore.personOrgName)) {
                        if ((RequestorFiduciaryStore.personOrgName.indexOf(' - ')) > 0) {
                            var splitOrg = RequestorFiduciaryStore.personOrgName.split(' - ');
                            data.va_RequestorOrgName = splitOrg[1];
                        }
                        else {
                            data.va_RequestorOrgName = RequestorFiduciaryStore.personOrgName; //VIP Fiduciary Tab, Person/Org Name field
                        }
                    }
                    useCallerAddress();
                    break;
                case 953850003: //Representative
                case 953850004: //VSO        
                    var RequestorPoaStore = '';
                    if ((selectionVariables.PayeeCode !== '00') && (depCurrentPoaStore != '') && (selectionVariables.loadingDependent === true)) {  //Dependent bec check is true
                        RequestorPoaStore = depCurrentPoaStore;
                    }
                    else { //Ok for Dependent VAIs to use Vet POA, if can't find Dep POA
                        RequestorPoaStore = vetCurrentPoaStore;
                    }
                    if ((RequestorPoaStore != '') && (RequestorPoaStore.personOrgName)) {
                        if ((RequestorPoaStore.personOrgName.indexOf(' - ')) > 0) {
                            var splitOrg = RequestorPoaStore.personOrgName.split(' - ');
                            data.va_RequestorOrgName = splitOrg[1];
                        }
                        else {
                            data.va_RequestorOrgName = RequestorPoaStore.personOrgName; //VIP POA Tab, Person/Org Name field
                        }
                    }
                    useCallerAddress();
                    break;
                case 953850007: //Authorized Third Party
                case 953850006: //Unknown Caller
                case 953850005: //Other 
                    useCallerAddress();
                    break;
            }

            //Used for Self/Spouse/Dependent
            function setRequestorAddress(RequestorAddressStore) {
                if (RequestorAddressStore != '') {
                    data.va_RequestorAddress1 = RequestorAddressStore.address1;
                    data.va_RequestorAddress2 = RequestorAddressStore.address2 + ((RequestorAddressStore.address3) ? ' ' + RequestorAddressStore.address3 : '');
                    data.va_RequestorCity = RequestorAddressStore.city;
                    data.va_RequestorCountry = RequestorAddressStore.country;
                    if ((data.va_RequestorCountry == 'USA') || (data.va_RequestorCountry == '')) {
                        data.va_RequestorState = RequestorAddressStore.postalCode;
                        data.va_RequestorZipCode = RequestorAddressStore.zipPrefix;
                    }
                    if (!Ext.isEmpty(RequestorAddressStore.foreignPostalCode)) {
                        //data.va_RequestorState = 'n/a'; //No State
                        data.va_RequestorZipCode = RequestorAddressStore.foreignPostalCode;
                    }
                    //Military Addresses                   
                    var milAdd = ((!Ext.isEmpty(RequestorAddressStore.militaryPostOfficeTypeCode)) ? RequestorAddressStore.militaryPostOfficeTypeCode : '');
                    milAdd = ((!Ext.isEmpty(milAdd)) ? milAdd + ((!Ext.isEmpty(RequestorAddressStore.militaryPostalTypeCode)) ? ' ' + RequestorAddressStore.militaryPostalTypeCode : '') : milAdd);
                    data.va_RequestorState = ((!Ext.isEmpty(data.va_RequestorState)) ? data.va_RequestorState + ((!Ext.isEmpty(milAdd)) ? ' / ' + milAdd : '') : data.va_RequestorState);

                    //Add the Territory / Providence field to the first empty address field
                    var territoryProvidence = ((!Ext.isEmpty(RequestorAddressStore.territoryName)) ? RequestorAddressStore.territoryName + ((!Ext.isEmpty(RequestorAddressStore.providenceName)) ? ' / ' + RequestorAddressStore.providenceName : '') : '');

                    if (!Ext.isEmpty(territoryProvidence)) {
                        if (Ext.isEmpty(data.va_RequestorAddress1))
                            data.va_RequestorAddress1 = territoryProvidence;
                        else if (Ext.isEmpty(data.va_RequestorAddress2))
                            data.va_RequestorAddress2 = territoryProvidence;
                        else
                            data.va_RequestorAddress2 += '; ' + territoryProvidence;
                    }
                }
            }
            //All scenarios except for Self/Spouse/Dependent use Caller First/Last Names
            function useCallerAddress() {
                if (parent.Xrm.Page.getAttribute('va_callerfirstname')) {
                    data.va_RequestorFirstName = ((parent.Xrm.Page.getAttribute('va_callerfirstname').getValue() != null) ? parent.Xrm.Page.getAttribute('va_callerfirstname').getValue() : '');
                    data.va_RequestorLastName = ((parent.Xrm.Page.getAttribute('va_callerlastname').getValue() != null) ? parent.Xrm.Page.getAttribute('va_callerlastname').getValue() : '');
                    data.va_RequestorAddress1 = ((parent.Xrm.Page.getAttribute('va_calleraddress1').getValue() != null) ? parent.Xrm.Page.getAttribute('va_calleraddress1').getValue() : '');
                    data.va_RequestorAddress2 = ((parent.Xrm.Page.getAttribute('va_calleraddress2').getValue() != null) ? parent.Xrm.Page.getAttribute('va_calleraddress2').getValue() + ((parent.Xrm.Page.getAttribute('va_calleraddress3').getValue() != null) ? ' ' + parent.Xrm.Page.getAttribute('va_calleraddress3').getValue() : '') : (parent.Xrm.Page.getAttribute('va_calleraddress3').getValue() != null) ? parent.Xrm.Page.getAttribute('va_calleraddress3').getValue() : '');
                    data.va_RequestorCity = ((parent.Xrm.Page.getAttribute('va_callercity').getValue() != null) ? parent.Xrm.Page.getAttribute('va_callercity').getValue() : '');
                    data.va_RequestorState = ((parent.Xrm.Page.getAttribute('va_callerstate').getValue() != null) ? parent.Xrm.Page.getAttribute('va_callerstate').getValue() : '');
                    data.va_RequestorZipCode = ((parent.Xrm.Page.getAttribute('va_callerzipcode').getValue() != null) ? parent.Xrm.Page.getAttribute('va_callerzipcode').getValue() : '');
                    data.va_RequestorCountry = ((parent.Xrm.Page.getAttribute('va_callercountry').getValue() != null) ? parent.Xrm.Page.getAttribute('va_callercountry').getValue() : '');
                    data.va_RequestorPhone = ((parent.Xrm.Page.getAttribute('va_identifycallerphone').getValue() != null) ? parent.Xrm.Page.getAttribute('va_identifycallerphone').getValue() : '');
                }
            }
        }

        //Populate Caller Email before Email on file, if exists (Check field to determine if on PC)
        if ((parent.Xrm.Page.getAttribute('va_calleremail') != null) && (parent.Xrm.Page.getAttribute('va_calleremail').getValue() != null)) {
            data.va_Email = parent.Xrm.Page.getAttribute('va_calleremail').getValue();
        }

        //SOJ if there isn't one already set (only for CLAIM or AWARD VAIs)
        if ((selectionVariables.Location === 'CLAIM') || (selectionVariables.Location === 'AWARD')) {
            if ((!selectedRecord.data) || (Ext.isEmpty(selectedRecord.data.va_RegionalOfficeId))) {
                //var sojCode = me.getClaimFolderLocationButton().getText();
                var sojCode = me.getClaimFolderLocationText().getValue();
                if (Ext.isEmpty(sojCode)) {
                    sojCode = '';
                    var birlsStore = me.getBirlsDetails();
                    if (birlsStore && birlsStore.items.length > 0) {
                        try {
                            sojCode = birlsStore.items.first().getRecord().data.claimFolderLocation;
                        }
                        catch (sojerr) { }
                    }
                }
                data.va_Sendto = me.getStationOfJurisdiction(sojCode);
            }
            else {
                if ((selectedRecord.data) && (selectedRecord.data.va_RegionalOfficeId)) {
                    data.va_Sendto = selectedRecord.data.va_RegionalOfficeId;
                }
            }
        }
        return data;
    },

    //Fifth Step: This function opens up the new VAI record
    createVAI: function (vaiRecord) {
        if (vaiRecord) {
            var vaiCode = '10024',
                me = this,
                url = '',
                vaiNumber = vaiRecord.va_vaiId,
                crmFormId = parent.Xrm.Page.data.entity.getId();

            if (parent && parent._currentEnv != undefined && parent._currentEnv != null && parent._currentEnv.vaiCode != undefined && parent._currentEnv.vaiCode != null) {
                vaiCode = parent._currentEnv.vaiCode;
            }

            if (parent && parent._usingIFD != undefined && parent._usingIFD === true) {
                url = me.getServerUrl() + "/main.aspx?etc=" + vaiCode + "&extraqs=%3f_CreateFromId%3d" + crmFormId +
				"%26_CreateFromType%3d4210%26_gridType%3d" + vaiCode + "%26etc%3d" + vaiCode + "%26id%3d%257b" + vaiNumber +
				"%257d&pagetype=entityrecord";
            } else {
                url = me.getServerUrl() + "/main.aspx?etc=10004&extraqs=%3f_CreateFromId%3d" +
					crmFormId + "%26_CreateFromType%3d4210%26_gridType%3d" + vaiCode + "%26etc%3d" + vaiCode + "%26id%3d" +
					vaiNumber + "%26rskey%3d302341238&pagetype=entityrecord";
            }

            var width = 1024,
                height = 768,
                top = (screen.height - height) / 2,
                left = (screen.width - width) / 2,
                params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,top=" + top + ",left=" + left + ",status=0,titlebar=no,resizable=yes",
                win = window.open(url, 'VAI', params);
            if (win) {
                try { win.focus(); } catch (err) { }
            }
            setTimeout(function () {
                win.focus();
            }, 1500);
        }
        else {
            Ext.Msg.alert('Error', 'Error occurred when creating a new VAI');
            return null;
        }
    },

    formatCurrencyToString: function (value) {
        var stringValue = value.toString(),
            concatString = '';

        if (Ext.isNumeric(stringValue)) {
            return stringValue;
        }
        else if (stringValue.substring(0, 1) == "$") {
            concatString = stringValue.substring(1);
            if (Ext.isNumeric(concatString))
                return concatString;
            else if (Ext.isNumeric(concatString.replace(',', '')))
                return concatString.replace(',', '');
            else
                return "0";
        }
        else if (Ext.isNumeric(concatString.replace(',', ''))) {
            return concatString.replace(',', '');
        }
        else
            return "0";
    },

    getServerUrl: function () {
        var url = parent.Xrm.Page.context.getServerUrl();
        if (url && url.match(/\/$/)) {
            url = url.substring(0, url.length - 1);
        }
        return url;
    },

    getUsCurrencyRecord: function () {
        var resultSet = null,
            TransactionCurrencyId = null;
        var columns = ['TransactionCurrencyId', 'CurrencyName'];
        // because of usage, this call is run synchronous.  TAS 10/24/2013 
        parent.CrmRestKit2011.ByQuery('TransactionCurrency', columns, "CurrencyName eq 'US Dollar' ", false).done(function (resultSet) {
            if (resultSet && resultSet.d.results && resultSet.d.results.length > 0) {
                TransactionCurrencyId = {
                    Id: resultSet.d.results[0].TransactionCurrencyId,
                    LogicalName: 'transactioncurrency',
                    Name: resultSet.d.results[0].CurrencyName
                };
                return TransactionCurrencyId;
            } else return null;
        }).fail(function (err) {
            UTIL.restKitError(err, 'Failed to retrieve Current Record');
        });
        return TransactionCurrencyId;
    },

    //Create VAI Number 'CRM' + YYMMDD-123456
    createVAINumber: function () {
        var d = new Date();
        var Y = d.getFullYear().toString().slice(2, 4);

        if ((d.getMonth() + 1).toString().length < 2) {
            var M = "0" + (d.getMonth() + 1).toString();
        }
        else {
            var M = (d.getMonth() + 1).toString();
        }

        if (d.getDate().toString().length < 2) {
            var D = "0" + d.getDate().toString();
        }
        else {
            var D = d.getDate().toString();
        }

        var vaiNumber = 'CRM' + Y + M + D + "";

        if (parent && parent.CrmRestKit2011) {
            var priorVAIs = null;
            var count = 0;
            var columns = ['va_vainumber', 'CreatedOn'];
            var orderby = "&$orderby=" + encodeURIComponent("CreatedOn desc");
            var query = this.getServerUrl() + '/' + 'XRMServices/2011/OrganizationData.svc' + "/" + "va_vaiSet"
                 + "?$select=" + columns.join(',') + orderby;
            var calls = parent.CrmRestKit2011.ByQueryUrl(query, false);
            calls.fail(
                 function (error) {
                     UTIL.restKitError(err, 'Failed to retrieve Unique VAI ID');
                 })
            calls.done(function (data) {
                if (data && data.d.results && data.d.results.length > 0) {
                    priorVAIs = data.d;
                }
            });



            //if (parent && parent.CrmRestKit2011) {
            //    var priorVAIs = parent.CrmRestKit.RetrieveMultiple("va_vai", ["va_vainumber", "CreatedOn"], null, null, 10, "CreatedOn desc");
            //    var count = 0;

            if (priorVAIs != null && priorVAIs.results && priorVAIs.results.length > 0) {
                for (var i in priorVAIs.results) {
                    var temp = priorVAIs.results[i].va_vainumber.split("-");
                    if (temp[0] == vaiNumber) {
                        if (parseInt(temp[1], 10) >= count) { //Hexidecimal 10 because of leading 0s
                            count = parseInt(temp[1], 10) + 1;
                        }
                    }
                    else { //Doesn't match today
                        if (count < 1) { //None qualified      
                            count = 1;  //None created today - restart at 1
                        }
                    }
                }
                switch ((count + '').length) {
                    case 1:
                        vaiNumber += "-00000" + count;
                        break;
                    case 2:
                        vaiNumber += "-0000" + count;
                        break;
                    case 3:
                        vaiNumber += "-000" + count;
                        break;
                    case 4:
                        vaiNumber += "-00" + count;
                        break;
                    case 5:
                        vaiNumber += "-0" + count;
                        break;
                    default:
                        vaiNumber += count;
                        break;
                }
                //No return: Error or the first VAI in dB created
            } else {
                vaiNumber += "-000001";
            }
        }
        else {
            //No RestKit
            vaiNumber += "-ERROR";
        }
        return vaiNumber;
    }
})