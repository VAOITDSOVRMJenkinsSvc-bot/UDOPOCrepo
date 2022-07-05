/**
* @class VIP.controller.services.ServiceRequest
* The controller for the service request creation event.
*/

Ext.define('VIP.controller.services.ServiceRequest', {
    extend: 'Ext.app.Controller',

    stores: ['FiduciaryPoa', 'Birls'],

    mixins: {
        soj: 'VIP.mixin.StationOfJurisdiction',
        store: 'VIP.mixin.Stores'
    },

    refs: [{
        ref: 'corpDetails',
        selector: '[xtype="personinfo.corp"]'
    }, {
        ref: 'personInfoGeneralDetails',
        selector: '[xtype="personinfo.details.generaldetails"]'
    }, {
        ref: 'addressesGrid',
        selector: '[xtype="personinfo.details.addresses.addresslist"]'
    }, {
        ref: 'dependentsGrid',
        selector: '[xtype="personinfo.details.dependents"]'
    }, {
        ref: 'militaryToursGrid',
        selector: '[xtype="militaryservice.militarytabs.tourhistory.tourhistorylist"]'
    }, {
        ref: 'disabilityRecordInfo',
        selector: '[xtype="ratings.disabilityratings.disabilityrecorddetails"]'
    }, {
        ref: 'disabilityRatingGrid',
        selector: '[xtype="ratings.disabilityratings.disabilitydata"]'
    }, {
        ref: 'otherRatingGrid',
        selector: '[xtype="ratings.otherratings"]'
    }, {
        ref: 'paymentHistoryGrid',
        selector: '[xtype="paymenthistory.payments.paymentdata.payments"]'
    }, {
        ref: 'PaymentInformationGrid',
        selector: '[xtype="paymentinformation.payments"]'
    }, {
        ref: 'awardsGrid',
        selector: '[xtype="awards.benefits"]'
    }, {
        ref: 'awardDetailsTab',
        selector: '[xtype="awards.details"]'
    }, {
        ref: 'awardLinesGrid',
        selector: '[xtype="awards.details.awardlines"]'
    }, {
        ref: 'appointmentsGrid',
        selector: '[xtype="pathways.appointments.appointmentdata"]'
    }, {
        ref: 'birlsDetails',
        selector: '[xtype="birls"]'
    },

    //{
    //    ref: 'claimFolderLocationButton',
    //    selector: '[xtype="personinfo.details.generaldetails"] > fieldcontainer > button[action="claimfolderlocation"]'
    //},

    {
        ref: 'claimFolderLocationText',
        selector: '[xtype="personinfo.details.generaldetails"] > fieldcontainer > displayfield[name="claimFolderLocation"]'
    },

    {
        ref: 'birls',
        selector: '[xtype="birls"]'
    }, {
        ref: 'folderLocationsList',
        selector: '[xtype="birls.birlsdetails.folderlocinfo.folderlocationlist"]'
    }],

    dependentData: {
        addressStore: [],
        fiduciaryPoaStore: [],
        paymentStore: [],
        corpStore: []
    },

    init: function () {
        var me = this;

        //me.control();
        me.application.on({
            createclaimservicerequest: me.onCreateClaimServiceRequest,
            createawardservicerequest: me.onCreateAwardServiceRequest,
            createdependentservicerequest: me.onCreateDependentServiceRequest,
            createcrmservicerequest: me.onCreateGeneralServiceRequest,
            createphonecallservicerequest: me.onCreatePhoneCallServiceRequest,
            createvadirservicerequest: me.onCreateVadirServiceRequest,
            createpersonservicerequest: me.onCreatePersonServiceRequest,
            scope: me
        });

        me.callParent();

        Ext.log('The service request controller has been initialized');
    },


    //Clean-up Task: Should consolidate claim, award, dependent functions below into one and call it from the other controllers.

    /**
    * This function is the handler for the claim service request button. 
    * @return {null}
    */
    onCreateClaimServiceRequest: function (serviceRequestType, selectedClaim) {
        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        var me = this,
            serviceRequest;
        me.retrieveServiceRequestData(serviceRequestType, selectedClaim);
    },

    /**
    * This function is the handler for the award service request button. 
    * @return {null}
    */
    onCreateAwardServiceRequest: function (serviceRequestType, selectedAward) {
        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        var me = this,
            serviceRequest;
        me.retrieveServiceRequestData(serviceRequestType, selectedAward);
    },

    /**
    * This function is the handler for the dependent service request button. 
    * @return {null}
    */
    onCreateDependentServiceRequest: function (serviceRequestType, selectedDependent) {
        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        var me = this,
            serviceRequest;
        me.retrieveServiceRequestData(serviceRequestType, selectedDependent);
    },

    /**
    * This function is called after phone call save, when the PCR is prompted if they want to create one based on the disposition
    * @return {null} 
    */
    onCreatePhoneCallServiceRequest: function (objectParameters) {
        var me = this,
            defaultType = { name: 'Other', value: objectParameters.defaultType };
        me.retrieveServiceRequestData(defaultType, null);
    },

    /**
    * This function is the handler for the Service Request creation through CRM UI. Will have to call a function
    * in the parent form and that iterates through JSON object and populates fields in SR form.
    * @return {null} 
    */
    onCreateGeneralServiceRequest: function (serviceRequestDom) {
        var me = this,
            defaultType = { name: 'Other', value: 953850007 };
        me.retrieveServiceRequestData(defaultType, null);

    },

    /** 
    * This function is the handler for the vadir service request button. 
    * @return {null} 
    */
    onCreateVadirServiceRequest: function (selectedVadir) {
        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        var me = this,
            defaultType = { name: 'Other', value: 953850000 };
        me.retrieveServiceRequestData(defaultType, selectedVadir);
    },

    onCreatePersonServiceRequest: function (corpVetRecord) {
        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        var me = this,
            defaultType = { name: 'Other', value: 1 };
        me.retrieveServiceRequestData(defaultType, corpVetRecord);
    },

    retrieveServiceRequestData: function (serviceRequestType, selectedRecord) {
        var me = this,
            crmForm = null,
            vipData = null,
            crmFormType = null,
            crmEntityName = null,
            crmFormId = null;

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
        if (crmFormId == null || crmFormType == 1) {
            Ext.Msg.confirm('CRM Form Must Be Saved',
                'The Phone Call must be saved prior to creating a new Service Request. Would you like to save it now?', function (button) {
                    if (button == 'no') return;
                    else parent.Xrm.Page.data.entity.save();
                });
        } else {
            me.getParticipantSRInfo(serviceRequestType, selectedRecord, crmForm);
        }
    },

    /**
    * This is the common function that all create SR functions will call.  It will return a JSON
    * with CRM schema names as keys and the proper types as values.
    * @return {JSON Object} dataCols
    */
    retrieveServiceRequestGeneralInfo: function (serviceRequestType, selectedRecord, crmForm) {
        var me = this;
        if (me.application.serviceRequest.va_ServiceRequestType == 'ECC') {

            var data = {//setting 'data' as a JSON object is important
                va_DateOpened: Ext.Date.format(new Date(), "m/d/Y"),
                va_reqnumber: '',
                va_Issue: {
                    Value: 953850007
                },
                va_Action: {
                    Value: 953850000
                },
                va_RPO: {
                    Value: parent.Xrm.Page.getAttribute('va_rpo').getValue()
                },
                TransactionCurrencyId: me.getUsCurrencyRecord(),
                va_ServiceRequestType: 'ECC'
            }

            data.va_PCROfRecordId = {
                Id: parent.Xrm.Page.getAttribute("createdby").getValue()[0].id,
                Name: parent.Xrm.Page.getAttribute("createdby").getValue()[0].name,
                LogicalName: "systemuser"
            };

            if (crmForm.LogicalName == "phonecall") {
                data.va_Phone = parent.Xrm.Page.getAttribute("phonenumber").getValue();
                data.va_Email = parent.Xrm.Page.getAttribute("va_email").getValue();
                data.va_FirstName = parent.Xrm.Page.getAttribute("va_callerfirstname").getValue();
                data.va_LastName = parent.Xrm.Page.getAttribute("va_callerlastname").getValue();
                data.va_VetFirstName = parent.Xrm.Page.getAttribute("va_firstname").getValue();
                data.va_VetLastName = parent.Xrm.Page.getAttribute("va_lastname").getValue();
                data.va_RelationshipDetails = parent.Xrm.Page.getAttribute("va_callerinformation").getValue();
                data.va_Address1 = parent.Xrm.Page.getAttribute("va_calleraddress1").getValue();
                data.va_Address2 = parent.Xrm.Page.getAttribute("va_calleraddress2").getValue();
                data.va_Address3 = parent.Xrm.Page.getAttribute("va_calleraddress3").getValue();
                data.va_City = parent.Xrm.Page.getAttribute("va_callercity").getValue();
                data.va_State = parent.Xrm.Page.getAttribute("va_callerstate").getValue();
                data.va_Country = parent.Xrm.Page.getAttribute("va_callercountry").getValue();
                data.va_ZipCode = parent.Xrm.Page.getAttribute("va_callerzipcode").getValue();
                //                data.va_Disposition = {
                //                    Value: parent.Xrm.Page.getAttribute("va_dispositionsubtype").getValue()
                //                };
                data.va_RelationtoVeteran = {
                    Value: parent.Xrm.Page.getAttribute("va_callerrelationtoveteran").getValue()
                };

                var callerFullName = '';
                var callerFirstName = parent.Xrm.Page.getAttribute("va_callerfirstname").getValue();
                var callerLastName = parent.Xrm.Page.getAttribute("va_callerlastname").getValue();
                if (callerFirstName == null) {
                    callerFullName = callerLastName;
                }
                else if (callerFirstName != null) {
                    callerFullName = callerFirstName + ' ' + callerLastName;
                }

                data.va_NameofReportingIndividual = callerFullName;
            }
        }
        else {
            //Veteran stores
            var addressStore = me.getAddressesGrid().getStore(),
            dependentStore = me.getDependentsGrid().getStore(),
            fidPoaStore = me.getFiduciaryPoaStore().getAt(0),
            currentPoa = fidPoaStore && (typeof fidPoaStore.currentPoa().getAt(0) !== 'undefined') ? fidPoaStore.currentPoa().getAt(0).data : null,
            currentFiduciary = fidPoaStore && (typeof fidPoaStore.currentFiduciary().getAt(0) !== 'undefined') ? fidPoaStore.currentFiduciary().getAt(0).data : null,
            militaryTourStore = me.getMilitaryToursGrid().getStore(),
            latestTourModel = null,
            disabilityRatingStore = me.getDisabilityRatingGrid().getStore(),
            paymentinformationStore = me.dependentData.paymentStore,
            latestpaymentinformationModel = null,
            awardsGrid = me.getAwardsGrid(),
            awardLinesStore = me.getAwardLinesGrid().getStore(),
            appointmentStore = me.getAppointmentsGrid().getStore(),
            data = {//setting 'data' as a JSON object is important
                va_DateOpened: Ext.Date.format(new Date(), "m/d/Y"),
                va_reqnumber: '',
                va_Issue: {
                    Value: 953850007
                },
                va_Action: {
                    Value: serviceRequestType.value
                },
                va_RPO: {
                    Value: (parent.Xrm.Page.data.entity.getEntityName() == 'phonecall') ? parent.Xrm.Page.getAttribute('va_rpo').getValue() : null
                },
                TransactionCurrencyId: me.getUsCurrencyRecord(),
                va_SSN: me.getCorpDetails().items.get("ssn").value,
                va_FileNumber: me.getCorpDetails().items.get("fileNumber").value,
                va_CharacterOfDischarge: '',
                va_MilitaryServiceBranch: '',
                va_MilitaryServiceEODDate: '',
                va_MilitaryServiceRADDate: '',
                va_DisabilityList: '',
                va_DisabilityPercentages: '',
                va_DiagnosticCodes: ''
            };
        }
        if (Ext.isEmpty(data.va_SSN)) { data.va_SSN = parent.Xrm.Page.getAttribute('va_ssn').getValue(); }
        //IF CFID is empty, grab from ID in Phone Call header

        if (Ext.isEmpty(data.va_FileNumber)) {
            if (parent.Xrm.Page.data.entity.getEntityName() == 'phonecall') {
                if (parent.Xrm.Page.getAttribute("va_headerid").getValue() != null)
                    var claimfromID = (parent.Xrm.Page.getAttribute("va_headerid").getValue()).split(' ');
                if ((claimfromID) && (claimfromID[4]) && (claimfromID[4] != 'n/a')) {
                    data.va_FileNumber = claimfromID[4];
                }
            }
            else { //Contact - Could try to grab FN from the store
                data.va_FileNumber = data.va_SSN;
            }
        }

        /*****End variable definitions*******/

        if (crmForm.LogicalName == 'phonecall') {
            data.va_OriginatingCallId = crmForm;
            if (parent.Xrm.Page.getAttribute('regardingobjectid').getValue() && parent.Xrm.Page.getAttribute('regardingobjectid').getValue() != null && parent.Xrm.Page.getAttribute('regardingobjectid').getValue() != undefined) {
                data.va_RelatedVeteranId = {
                    Id: parent.Xrm.Page.getAttribute('regardingobjectid').getValue()[0].id,
                    LogicalName: 'contact',
                    Name: parent.Xrm.Page.getAttribute('regardingobjectid').getValue()[0].name
                };
            }
        }
        else if (crmForm.LogicalName == 'contact') {
            data.va_RelatedVeteranId = crmForm;
            if (parent.window.parent.opener.Xrm.Page.data && parent.window.parent.opener.Xrm.Page.data.entity.getId()) {
                data.va_OriginatingCallId = {
                    Id: parent.window.parent.opener.Xrm.Page.data.entity.getId(),
                    LogicalName: 'phonecall',
                    Name: parent.window.parent.opener.Xrm.Page.getAttribute('subject').getValue()
                };
            }
        }

        //Iterate through selected record data and add it to main data columns
        if (selectedRecord) {
            for (var i in selectedRecord) {
                data[i] = selectedRecord[i];
            }
        }

        if (this.application.serviceRequest.va_ServiceRequestType !== 'ECC') {
            /* If SR is NOT for a selected Dependent (Off of the PersonInfo>Dependent subtab),
            OR PayeeCode is '00' (Veteran) or '10 (Spouse) - match the Spouse and set as default Dependent
            This next IF only continues if Dependent tab info is not preloaded into 'data'   */
            if (data.va_SRLastName == undefined) {
                var dependentAddressesOfBirth = '',
                dependentNames = '';

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

                    //try to match the pid to dependent store if  !== 00//(record.get('ssn') == serviceRequest.va_SelectedSSN)
                    if (record.get('participantId') == me.application.serviceRequest.va_SelectedPID) {
                        data.va_SRGender = record.get('gender');
                        data.va_SRRelation = record.get('relationship');
                    }
                });

                data.va_DependentAddresses = dependentAddressesOfBirth;
                data.va_DependentNames = dependentNames;
            }

            //Use new CORP results
            if (me.application.serviceRequest.va_DepCorp && me.application.serviceRequest.va_DepCorp[0]) {
                data.va_SRFirstName = me.application.serviceRequest.va_DepCorp[0].data.firstName;
                data.va_SRLastName = me.application.serviceRequest.va_DepCorp[0].data.lastName;
                data.va_SRSSN = me.application.serviceRequest.va_DepCorp[0].data.ssn;
                data.va_SRDOBText = me.application.serviceRequest.va_DepCorp[0].data.dob;
            }
            else {  //No new CORP result, since run from Pop up...use existing CORP store.
                if (me.application.serviceRequest.va_SelectedPayeeCode == '' || me.application.serviceRequest.va_SelectedPayeeCode == null) {
                    data.va_SRSSN = parent.Xrm.Page.getAttribute('va_ssn').getValue();
                    data.va_SRFirstName = parent.Xrm.Page.getAttribute('va_firstname').getValue();
                    data.va_SRLastName = parent.Xrm.Page.getAttribute('va_lastname').getValue();
                    data.va_SRDOBText = parent.Xrm.Page.getAttribute('va_dobtext').getValue();
                }
            }
            //Payee code !== 00, '', null, USE NEW CORP results
            if ((me.application.serviceRequest.va_SelectedPayeeCode !== '') && (me.application.serviceRequest.va_SelectedPayeeCode !== null) && (me.application.serviceRequest.va_SelectedPayeeCode !== '00')) {
                data.va_SREmail = me.application.serviceRequest.va_DepCorp[0].data.emailAddress;
                data.va_DayPhone = me.application.serviceRequest.va_DepCorp[0].data.fullPhone1;
                data.va_EveningPhone = me.application.serviceRequest.va_DepCorp[0].data.fullPhone2;

                //Set it to new CORP result address
                if (me.dependentData.addressStore) {
                    for (var i in me.dependentData.addressStore.data.items) {
                        if (me.dependentData.addressStore.data.items[i].data.participantAddressTypeName == 'Mailing') {
                            data.va_mailing_address1 = me.dependentData.addressStore.data.items[i].data.address1;
                            data.va_mailing_address2 = me.dependentData.addressStore.data.items[i].data.address2;
                            data.va_mailing_address3 = me.dependentData.addressStore.data.items[i].data.address3;
                            data.va_mailing_city = me.dependentData.addressStore.data.items[i].data.city;
                            data.va_MailingCountry = me.dependentData.addressStore.data.items[i].data.country;

                            //Select State Value based on Country
                            if (me.dependentData.addressStore.data.items[i].data.country === ('USA' || '')) {
                                data.va_mailing_state = me.dependentData.addressStore.data.items[i].data.postalCode;
                                data.va_mailing_zip = me.dependentData.addressStore.data.items[i].data.zipPrefix;
                            }
                            else {
                                //data.va_mailing_state = 'No State for Foreign Country';
                                data.va_mailing_zip = me.dependentData.addressStore.data.items[i].data.foreignPostalCode;
                            }
                            if (!data.va_SREmail) {
                                data.va_SREmail = me.dependentData.addressStore.data.items[i].data.emailAddress;
                            }
                        }
                    }
                }
                //else there is no store, and then we need to get the data from the fields on the phone call screen

                //Set dependent FID POA stores
                if ((me.dependentData.fiduciaryPoaStore)) {
                    fidPoaStore = me.dependentData.fiduciaryPoaStore.data.items[0];
                    currentPoa = (fidPoaStore && fidPoaStore.currentPoaStore && fidPoaStore.currentPoaStore.data && fidPoaStore.currentPoaStore.data.items[0].data) ? fidPoaStore.currentPoaStore.data.items[0].data : null;
                    currentFiduciary = (fidPoaStore && fidPoaStore.currentFiduciaryStore) ? fidPoaStore.currentFiduciaryStore.data.items[0].data : null;
                }
            }
                //For Payee 00 '' or null
            else {
                data.va_SRRelation = 'Self';
                data.va_DayPhone = me.getCorpDetails().items.get("fullPhone1").value;
                data.va_EveningPhone = me.getCorpDetails().items.get("fullPhone2").value;

                //Setting from PC fields if SR is created from Phone Call, it will be overwritten if there is data in the addressStore typed Mailing
                if (parent.Xrm.Page.data.entity.getEntityName() == 'phonecall') {
                    data.va_mailing_address1 = parent.Xrm.Page.getAttribute('va_calleraddress1').getValue();
                    data.va_mailing_address2 = parent.Xrm.Page.getAttribute('va_calleraddress2').getValue();
                    data.va_mailing_address3 = parent.Xrm.Page.getAttribute('va_calleraddress3').getValue();
                    data.va_mailing_city = parent.Xrm.Page.getAttribute('va_callercity').getValue();
                    data.va_mailing_state = parent.Xrm.Page.getAttribute('va_callerstate').getValue();
                    data.va_mailing_zip = parent.Xrm.Page.getAttribute('va_callerzipcode').getValue();
                    data.va_MailingCountry = parent.Xrm.Page.getAttribute('va_callercountry').getValue();
                    data.va_EmailofVeteran = parent.Xrm.Page.getAttribute('va_calleremail').getValue();
                    data.va_SREmail = parent.Xrm.Page.getAttribute('va_calleremail').getValue();
                    data.va_Email = parent.Xrm.Page.getAttribute('va_calleremail').getValue();
                }

                //Set the Vet's current mailing address and Email Address
                addressStore.each(function (record) {
                    if (record.get("participantAddressTypeName") == "Mailing") {
                        var territoryProvidence = record.get('territoryName'),
                    militaryPostal = record.get('militaryPostOfficeTypeCode');

                        if (!Ext.isEmpty(record.get('providenceName')))
                            territoryProvidence = (Ext.isEmpty(territoryProvidence) ? record.get('providenceName') : territoryProvidence + ' / ' + record.get('providenceName'));
                        if (!Ext.isEmpty(record.get('militaryPostalTypeCode')))
                            militaryPostal = (Ext.isEmpty(militaryPostal) ? record.get('militaryPostalTypeCode') : militaryPostal + ' ' + record.get('militaryPostalTypeCode'));

                        data.va_mailing_address1 = record.get('address1');
                        data.va_mailing_address2 = record.get('address2');
                        data.va_mailing_address3 = record.get('address3');
                        data.va_mailing_city = record.get('city');
                        data.va_mailing_state = record.get('postalCode');
                        data.va_mailing_zip = record.get('zipPrefix');
                        data.va_MailingCountry = record.get('country');

                        if (!Ext.isEmpty(record.get('foreignPostalCode')))
                            data.va_mailing_zip = record.get('foreignPostalCode');
                        if (!Ext.isEmpty(militaryPostal))
                            data.va_mailing_state = (Ext.isEmpty(data.va_mailing_state) ? militaryPostal : data.va_mailing_state + ' / ' + militaryPostal);

                        //Add the Territory Providence field to the first empty address field, default to the end of address 3
                        if (!Ext.isEmpty(territoryProvidence)) {
                            if (Ext.isEmpty(data.va_mailing_address1))
                                data.va_mailing_address1 = territoryProvidence;
                            else if (Ext.isEmpty(data.va_mailing_address2))
                                data.va_mailing_address2 = territoryProvidence;
                            else if (Ext.isEmpty(data.va_mailing_address3))
                                data.va_mailing_address3 = territoryProvidence;
                            else
                                data.va_mailing_address3 += '; ' + territoryProvidence;
                        }
                    }
                    if (record.get("participantAddressTypeName") == "Email") {
                        data.va_EmailofVeteran = record.get("emailAddress");
                        data.va_SREmail = record.get('emailAddress');
                    }
                });
            }

            //Get Military Info
            militaryTourStore.sort("eodDate", "DESC");
            latestTourModel = militaryTourStore.first();
            militaryTourStore.each(function (record) {
                data.va_CharacterOfDischarge += record.get("mpDischargeCharacterTypeName") + '\n';
                data.va_MilitaryServiceBranch += record.get("militaryServiceBranchTypeName") + '\n';
                data.va_MilitaryServiceEODDate += record.get("eodDate_f") + '\n';
                data.va_MilitaryServiceRADDate += record.get("radDate_f") + '\n';
            });
            if (latestTourModel) {
                data.va_BranchofService = latestTourModel.get("militaryServiceBranchTypeName");
                data.va_Discharge = latestTourModel.get("mpDischargeCharacterTypeName");
                data.va_ServiceDates = latestTourModel.get("eodDate_f") + '-' + latestTourModel.get("radDate_f");
            }

            //Get Rating Info 
            //OtherRatings grid, check the Disability text to equal 'Basic Eligibility under 38 USC Ch 35', if record then get Begin Date for P&T Rating
            if ((me.getOtherRatingGrid()) && (me.getOtherRatingGrid().items) && (me.getOtherRatingGrid().items.first()) && (me.getOtherRatingGrid().items.first().getStore()) && (me.getOtherRatingGrid().items.first().getStore().data.length > 0)) {
                for (var i in me.getOtherRatingGrid().items.first().getStore().data.items) {
                    if (me.getOtherRatingGrid().items.first().getStore().data.items[i].data.disabilityTypeName == 'Basic Eligibility under 38 USC Ch 35') {
                        data.va_RatingEffectiveDate = Ext.Date.format(me.getOtherRatingGrid().items.first().getStore().data.items[i].data.beginDate, 'm/d/Y');
                    }
                }
            }
            data.va_RatingDegree = me.getDisabilityRecordInfo().items.get("disabilityServiceConnectedDegree").value;
            data.va_ServiceConnectedDisability = (Ext.isEmpty(data.va_RatingDegree) ? false : true);
            disabilityRatingStore.each(function (record) {
                if (record.get("disabilityDecisionTypeCode") === "SVCCONNCTED") {
                    data.va_DisabilitiesRaw += record.get("diagnosticText") + "|";
                    data.va_DisabilityPercentagesRaw += record.get("diagnosticPercent") + "|";
                    data.va_DiagnosticCodesRaw += record.get("diagnosticTypeCodeFormatted") + "|";

                    data.va_DisabilityList += record.get("diagnosticText") + "\n";
                    data.va_DisabilityPercentages += record.get("diagnosticPercent") + "\n";
                    data.va_DiagnosticCodes += record.get("diagnosticTypeCodeFormatted") + "\n";
                }
            });

            //Verify that there is data in the Store
            if ((paymentinformationStore) && (paymentinformationStore.data) && (paymentinformationStore.data.length > 0)) {
                //Populate SR: Multiple Payment section (PaymentInformation Grid)
                for (var i = paymentinformationStore.data.length - 1; i > -1; i--) {
                    var record = paymentinformationStore.data.items[i];
                    if (record.get("returnReason") != null && record.get("returnReason") != "") {
                        continue;
                    }
                    if (((record.get("programType2") == "Compensation") || (record.get("programType2") == "Pension")) && (record.get("paymentDate") !== null)) {
                        data.va_mpraw += record.get("paymentDate") + "_" + record.get("paymentAmount") + "#";

                        //Populate SR: Latest Payment Information
                        //Insert latest payment fields here, it will overwrite until the last one that fits the criteria
                        data.va_BenefitType = record.get("programType2");
                        data.va_PaymentAmount = { Value: me.formatCurrencyToString(record.get("paymentAmount")) };
                        data.va_PayDate = Ext.Date.format(record.get("paymentDate"), "m/d/Y");
                    }
                }
            }

            //Use Award Line here if no Current Monthly Rate and if only one award                 
            if (!data.va_CurrentMonthlyRate) {
                if ((awardsGrid) && (awardsGrid.getStore()) && (awardsGrid.getStore().count() == 1)) {
                    //If need to set defaultAward = awardsGrid.getStore().getAt(0);
                    if (awardLinesStore) {  //If Award Lines Store exists
                        var awardLineModel = null;
                        awardLinesStore.sort("effectiveDate", "DESC");
                        awardLinesStore.each(function (record) {
                            //Determine the correct Award Line = effectiveDate is not greater than today   
                            if (record.get("effectiveDate") <= new Date) {
                                awardLineModel = record;
                                return false; //stops iteration of the 'each' function
                            }
                            return true;
                        });

                        //add latest award line data if there is any
                        if (awardLineModel) {
                            data.va_EffectiveDate = Ext.Date.format(awardLineModel.get("effectiveDate"), "m/d/Y");
                            data.va_CurrentMonthlyRate = {
                                Value: me.formatCurrencyToString(awardLineModel.get("totalAward"))
                            };
                            data.va_NetAmountPaid = {
                                Value: me.formatCurrencyToString(awardLineModel.get("netAward"))
                            };
                            data.va_DependentAmount = {
                                Value: me.formatCurrencyToString(awardLineModel.get("spouse"))
                            };
                            data.va_AAAmount = {
                                Value: me.formatCurrencyToString(awardLineModel.get("aaHbInd"))
                            };
                            data.va_PensionBenefitAmount = {
                                Value: me.formatCurrencyToString(awardLineModel.get("altmnt"))
                            };
                        }
                    }
                }
            }

            //Get the Date of Death and Date of Birth fields
            if (!Ext.isEmpty(me.getCorpDetails().items.get("dateOfDeath").value)) {
                data.va_DateofDeath = Ext.Date.format(new Date(me.getCorpDetails().items.get("dateOfDeath").value), "m/d/Y");
            }
            else {
                data.va_DateofDeath = null;
            }

            if (!Ext.isEmpty(me.getCorpDetails().items.get("dateOfBirth").value)) {
                data.va_VetDOBText = me.getCorpDetails().items.get("dateOfBirth").value;
            }

            //Get the Future Exam date
            appointmentStore.sort("appointmentDateTime", "DESC");
            appointmentStore.each(function (record) {
                if (record.get("appointmentDateTime") && record.get("status") === "FUTURE") {
                    data.va_FutureExamDate = Ext.Date.format(record.get("appointmentDateTime"), "m/d/Y");
                    return false; //stops iteration of the 'each' function
                }
                return true;
            });

            //Get Data from the phone call form
            if (crmForm.LogicalName == "phonecall") {
                data.va_Phone = parent.Xrm.Page.getAttribute("phonenumber").getValue();
                data.va_Email = (parent.Xrm.Page.getAttribute("va_email").getValue()) ? parent.Xrm.Page.getAttribute("va_email").getValue() : parent.Xrm.Page.getAttribute('va_calleremail').getValue();
                data.va_FirstName = parent.Xrm.Page.getAttribute("va_callerfirstname").getValue();
                data.va_LastName = parent.Xrm.Page.getAttribute("va_callerlastname").getValue();
                data.va_VetFirstName = parent.Xrm.Page.getAttribute("va_firstname").getValue();
                data.va_VetLastName = parent.Xrm.Page.getAttribute("va_lastname").getValue();
                data.va_RelationshipDetails = parent.Xrm.Page.getAttribute("va_callerinformation").getValue();
                data.va_Address1 = parent.Xrm.Page.getAttribute("va_calleraddress1").getValue();
                data.va_Address2 = parent.Xrm.Page.getAttribute("va_calleraddress2").getValue();
                data.va_Address3 = parent.Xrm.Page.getAttribute("va_calleraddress3").getValue();
                data.va_City = parent.Xrm.Page.getAttribute("va_callercity").getValue();
                data.va_State = parent.Xrm.Page.getAttribute("va_callerstate").getValue();
                data.va_Country = parent.Xrm.Page.getAttribute("va_callercountry").getValue();
                data.va_ZipCode = parent.Xrm.Page.getAttribute("va_callerzipcode").getValue();
                data.va_CompensationClaim = parent.Xrm.Page.getAttribute("va_compensationclaim").getValue();
                data.va_PensionClaim = parent.Xrm.Page.getAttribute("va_pensionclaim").getValue();
                data.va_Disposition = {
                    Value: parent.Xrm.Page.getAttribute("va_dispositionsubtype").getValue()
                };
                data.va_FNODReportingFor = {
                    Value: parent.Xrm.Page.getAttribute("va_fnodreportingfor").getValue()
                };
                data.va_RelationtoVeteran = {
                    Value: parent.Xrm.Page.getAttribute("va_callerrelationtoveteran").getValue()
                };

                var callerFullName = '';
                var callerFirstName = parent.Xrm.Page.getAttribute("va_callerfirstname").getValue();
                var callerLastName = parent.Xrm.Page.getAttribute("va_callerlastname").getValue();
                if (callerFirstName == null) {
                    callerFullName = callerLastName;
                }
                else if (callerFirstName != null) {
                    callerFullName = callerFirstName + ' ' + callerLastName;
                }

                data.va_NameofReportingIndividual = callerFullName;

                if (parent.Xrm.Page.getAttribute("va_specialsituationid").getValue()) {
                    data.va_SpecialSituationId = {
                        Id: parent.Xrm.Page.getAttribute("va_specialsituationid").getValue()[0].id,
                        LogicalName: "va_specialsituation",
                        Name: parent.Xrm.Page.getAttribute("va_specialsituationid").getValue()[0].name
                    };
                }
            }
            if (Ext.isEmpty(data.va_ParticipantID)) {
                data.va_ParticipantID = me.getPersonInfoGeneralDetails().items.get("participantId").value;
            }

            //Get current POA and Fiduciary Info
            if (currentFiduciary) {
                data.va_HasFiduciary = true;
                data.va_FiduciaryData =
                'Name: ' + currentFiduciary.personOrgName + '\n' +
                'From: ' + currentFiduciary.beginDate + '\n' +
                'To: ' + currentFiduciary.endDate + '\n' +
                'Relation: ' + currentFiduciary.relationshipName + '\n' +
                'Person or Org: ' + currentFiduciary.personOrOrganizationInd + '\n' +
                'Temp Custodian: ' + currentFiduciary.temporaryCustodianInd;
            }
            if (currentPoa) {
                data.va_HasPOA = true;
                data.va_POAData =
                'Name: ' + currentPoa.personOrgName + '\n' +
                'From: ' + currentPoa.beginDate + '\n' +
                'To: ' + currentPoa.endDate + '\n' +
                'Relation: ' + currentPoa.relationshipName + '\n' +
                'Person or Org: ' + currentPoa.personOrOrganizationInd + '\n' +
                'Temp Custodian: ' + currentPoa.temporaryCustodianInd;
            }
        }

        //Obtain and Set a unique request number
        if (data.va_RelatedVeteranId != (null || undefined) && (data.va_SRSSN != (null || undefined))) {
            data.va_reqnumber = me.getUniqueRequestNumber(data.va_RelatedVeteranId.Id, data.va_SRSSN);
        }
        else {
            data.va_reqnumber = ((parent.Xrm.Page.getAttribute('va_ssn').getValue() != null) ? parent.Xrm.Page.getAttribute('va_ssn').getValue() + ': 1' : 'Email Blank Forms');
        }

        //Set the PCR of record to be the createdby user
        data.va_PCROfRecordId = {
            Id: parent.Xrm.Page.getAttribute("createdby").getValue()[0].id,
            Name: parent.Xrm.Page.getAttribute("createdby").getValue()[0].name,
            LogicalName: "systemuser"
        };

        //Set the SOJ and Participant Id to claim folder location if there isn't one already set
        if (Ext.isEmpty(data.va_RegionalOfficeId)) {
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
            data.va_RegionalOfficeId = me.getStationOfJurisdiction(sojCode);
        }

        return data;
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

    /*
    *
    */
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

    getUniqueRequestNumber: function (vetId, ssn) {
        var requestNumber = '[SET REQ NO]';
        if (Ext.isEmpty(vetId) && Ext.isEmpty(ssn)) { return requestNumber; }

        if (parent && parent.CrmRestKit2011) {
            // search by vet id if present, or ssn if present, or generate generic req number if none are available
            //Figure out if it is phonecall or contact
            // because of usage, this call is run synchronous.  TAS 10/24/2013
            var priorServiceRequests = null;
            var count = 0;
            var filter = !Ext.isEmpty(vetId) ? "va_RelatedVeteranId/Id eq (guid'" + vetId + "')" : "va_SSN eq '" + ssn + "'";
            var columns = ['va_reqnumber'];
            var orderby = "&$orderby=" + encodeURIComponent("CreatedOn desc");
            var query = this.getServerUrl() + '/' + 'XRMServices/2011/OrganizationData.svc' + "/" + "va_servicerequestSet"
			     + "?$select=" + columns.join(',') + "&$filter=" + encodeURIComponent(filter) + orderby;
            var calls = parent.CrmRestKit2011.ByQueryUrl(query, false);
            calls.fail(
				 function (error) {
				     UTIL.restKitError(err, 'Failed to retrieve Unique Request ID');
				 })
            calls.done(function (data) {
                if (data && data.d.results && data.d.results.length > 0) {
                    priorServiceRequests = data.d;
                }
            });

            if (Ext.isEmpty(ssn)) { ssn = 'SR: '; }

            if (priorServiceRequests != null && priorServiceRequests.results && priorServiceRequests.results.length > 0) {
                for (var i in priorServiceRequests.results) {
                    var temp = priorServiceRequests.results[i].va_reqnumber.split(":");
                    if (parseInt(temp[1]) >= count) {
                        count = parseInt(temp[1]) + 1;
                    }
                }
                requestNumber = ssn + ": " + count;
            } else {
                requestNumber = ssn + ": 1";
            }

        }
        return requestNumber;
    },

    /**
    * This function opens up the new service request window
    */
    openServiceRequestWindow: function (serviceRequest) {
        if (serviceRequest) {
            var scCode = '10012', me = this, url = '', serviceRequestId = serviceRequest.va_servicerequestId,
                crmFormId = parent.Xrm.Page.data.entity.getId();

            if (parent && parent._currentEnv != undefined && parent._currentEnv != null && parent._currentEnv.srCode != undefined && parent._currentEnv.srCode != null) {
                scCode = parent._currentEnv.srCode;
            }

            // win = Xrm.Utility.openEntityForm("va_servicerequest", serviceRequestId);

            if (parent && parent._usingIFD != undefined && parent._usingIFD === true) {
                url = me.getServerUrl() + "/main.aspx?etc=" + scCode + "&extraqs=%3f_CreateFromId%3d" + crmFormId +
				"%26_CreateFromType%3d4210%26_gridType%3d" + scCode + "%26etc%3d" + scCode + "%26id%3d%257b" + serviceRequestId +
				"%257d&pagetype=entityrecord";
            } else {
                url = me.getServerUrl() + "/main.aspx?etc=10004&extraqs=%3f_CreateFromId%3d" +
					crmFormId + "%26_CreateFromType%3d4210%26_gridType%3d10004%26etc%3d10004%26id%3d" +
					serviceRequestId + "%26rskey%3d302341238&pagetype=entityrecord";
            }

            var
            //                url = me.getServerUrl() + "/main.aspx?etc=10004&extraqs=%3f_CreateFromId%3d" +
            //					crmFormId + "%26_CreateFromType%3d4210%26_gridType%3d10004%26etc%3d10004%26id%3d" +
            //					serviceRequestId + "%26rskey%3d302341238&pagetype=entityrecord",
                width = 1024,
                height = 768,
                top = (screen.height - height) / 2,
                left = (screen.width - width) / 2,
                params = "width=" + width + ",height=" + height + ",location=0,menubar=0,toolbar=0,top=" + top + ",left=" + left + ",status=0,titlebar=no,resizable=yes",
                win = window.open(url, 'ServiceRequest', params);
            if (win) {
                try { win.focus(); } catch (err) { }
            }
            setTimeout(function () {
                win.focus();
            }, 1500);
        }
        else {
            Ext.Msg.alert('Error', 'Error occurred when creating a new service request');
            return null;
        }
    },

    getParticipantSRInfo: function (serviceRequestType, selectedRecord, crmForm) {
        var me = this;
        //if the search has not been run, then continue 
        if ((!parent._WebServiceExecutionStatusLists) || (parent._WebServiceExecutionStatusLists == '')) {
            parent.blankLetters();
            return;
        }

        if (me.application.serviceRequest.va_ServiceRequestType == ('ECC' || 'PERSON' || '' || null)) {
            me.continueWork(serviceRequestType, selectedRecord, crmForm);
        }
        else {
            //General Corp
            me.dependentData.corpStore = Ext.create("VIP.store.Corp");
            var corpFilters = [{
                property: 'ptcpntId',
                value: me.application.serviceRequest.va_SelectedPID
            }];

            //Declare and load Address stores
            me.dependentData.addressStore = Ext.create("VIP.store.personinfo.Addresses");
            var addressFilters = [{
                property: 'ptcpntId',
                value: me.application.serviceRequest.va_SelectedPID
            }];

            //Load Fiduciary POA - look for currentFiduciary and currentPOA
            me.dependentData.fiduciaryPoaStore = Ext.create("VIP.store.FiduciaryPoa");
            var fiduciaryPoaFilters = [{
                property: 'fileNumber',
                value: me.application.serviceRequest.va_SelectedSSN
            }];

            //Load Payment
            me.dependentData.paymentStore = Ext.create("VIP.store.Payment");
            var paymentFilters = [{
                property: 'ParticipantId',
                value: me.application.serviceRequest.va_SelectedPID
            }, {
                property: 'FileNumber',
                value: me.application.serviceRequest.va_SelectedSSN
            }, {
                property: 'PayeeCode',
                value: me.application.serviceRequest.va_SelectedPayeeCode
            }];

            if (me.application.serviceRequest.va_SelectedPayeeCode !== '00') {
                $.when(
                me.loadStore(me.dependentData.corpStore, corpFilters, function (records) {
                    me.application.serviceRequest.va_DepCorp = records;
                    me.application.serviceRequest.va_SelectedSSN = records[0].data.ssn;
                })
            )
            .then(function () {
                $.when(
                    me.loadStore(me.dependentData.addressStore, addressFilters, function (records) {
                        me.application.serviceRequest.va_DepAdd[0] = (records && records[0] && records[0].data) ? records[0].data : [];
                    }),
                    me.loadStore(me.dependentData.fiduciaryPoaStore, fiduciaryPoaFilters, function (records) {
                        me.application.serviceRequest.va_DepFiduciaryPoa[0] = records[0];
                    }),
                    me.loadStore(me.dependentData.paymentStore, paymentFilters, function (records) {
                        me.application.serviceRequest.va_DepPayment = records;
                    }))
                .then(function () {
                    me.loadPaymentDetails().done(function (data) {
                    }).always(function () {
                        me.continueWork(serviceRequestType, selectedRecord, crmForm);
                    });
                })
                .fail(function () {
                    alert('FAILED at loadStore');
                });
            })
            .fail(function () {
                alert('FAILED at loadStore');
            });
            }
                //Continue normally and just run CORPSTORE
            else {
                $.when(
                me.loadStore(me.dependentData.corpStore, corpFilters, function (records) {
                    me.application.serviceRequest.va_DepCorp = records;
                    me.application.serviceRequest.va_SelectedSSN = ((records) && (records[0]) && (records[0].data) && (records[0].data.ssn)) ? records[0].data.ssn : me.application.serviceRequest.va_SelectedSSN;
                })
            )
            .then(function () {
                me.dependentData.paymentStore = me.getPaymentInformationGrid().getStore();

                me.loadPaymentDetails().done(function (data) {
                }).always(function () {
                    me.continueWork(serviceRequestType, selectedRecord, crmForm);
                });
            })
            .fail(function () {
                alert('FAILED at loadStore');
            });
            }
        }
    },
    //Continue on after loading or not loading the dependent store
    continueWork: function (serviceRequestType, selectedRecord, crmForm) {
        var me = this,
        serviceRequest = "";

        me.dependentData.vipData = me.retrieveServiceRequestGeneralInfo(serviceRequestType, selectedRecord, crmForm);
        if (me.dependentData.vipData) {
            serviceRequest = (typeof parent.UTIL.CreateEntity === "function") ? parent.UTIL.CreateEntity("va_servicerequest", me.dependentData.vipData) : "none";
            if (serviceRequest === "none") {
                alert("Could not find CreateEntity function, please refresh page and try again or contact HelpDesk.");
                return;
            }
            else {
                me.openServiceRequestWindow(serviceRequest);
            }
        }
    },

    loadPaymentDetails: function () {
        var me = this,
            deferred = $.Deferred();

        //Get Payment Details to popoulate the Award info section of the SR
        if (me.dependentData.paymentStore.getCount() === 0 || (me.application.serviceRequest.va_ServiceRequestType == 'Claim')) {
            deferred.reject();
            return deferred.promise();
        }

        var firstPayment = me.dependentData.paymentStore.getAt(0);
        //Load Payment details
        var paymentDetailsStore = Ext.create("VIP.store.paymentinformation.PaymentDetail");
        var paymentFilters = [{
            property: 'PaymentId',
            value: firstPayment.get('paymentId')
        }, {
            property: 'FbtId',
            value: ''
        }];

        paymentDetailsStore.load({
            filters: paymentFilters,
            callback: function (records, operation, success) {
                var paymentInfo;
                if (success) {
                    if (!Ext.isEmpty(records)) {
                        paymentInfo = {
                            netAmount: (records[0].get('netPaymentAmount') ? records[0].get('netPaymentAmount').replace(/[\$,]/g, '') : null),
                            grossAmount: (records[0].get('grossPaymentAmount') ? records[0].get('grossPaymentAmount').replace(/[\$,]/g, '') : null)
                        };
                    }
                    else {
                        paymentInfo = {
                            netAmount: null,
                            grossAmount: null,
                            effectiveDate: null
                        }
                    }
                    deferred.resolve(paymentInfo);
                } else {
                    deferred.reject();
                }
            },
            scope: this
        });

        return deferred.promise();
    }
});