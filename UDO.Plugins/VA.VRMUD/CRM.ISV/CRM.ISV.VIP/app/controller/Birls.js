/**
* @author Jonas Dawson
* @class VIP.controller.Birls
*
*/
Ext.define('VIP.controller.Birls', {
    extend: 'Ext.app.Controller',

    stores: [
		'Birls',
    	'birls.AlternateNames',
    	'birls.Disclosures',
    	'birls.Flashes',
    	'birls.FolderLocations',
    	'birls.InsurancePolicy',
    	'birls.MilitaryService',
    	'birls.ServiceDiagnostics'
	],
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    refs: [{
        ref: 'birls',
        selector: '[xtype="birls.identificationdata"]'
    }, {
        ref: 'birlsDetails',
        selector: '[xtype="birls.birlsdetails"]'
    }, {
        ref: 'alternateNames',
        selector: '[xtype="birls.birlsdetails.alternatenames"]'
    }, {
        ref: 'disclosures',
        selector: '[xtype="birls.birlsdetails.disclosures"]'
    }, {
        ref: 'flashes',
        selector: '[xtype="birls.birlsdetails.flashes"]'
    }, {
        ref: 'folderLocationsList',
        selector: '[xtype="birls.birlsdetails.folderlocinfo.folderlocationlist"]'
    }, {
        ref: 'folderLocationInfo',
        selector: '[xtype="birls.birlsdetails.folderlocinfo.additionalinfo"]'
    }, {
        ref: 'insurancePolicies',
        selector: '[xtype="birls.birlsdetails.insurancepolicy.policies"]'
    }, {
        ref: 'insurancePolicyInfo',
        selector: '[xtype="birls.birlsdetails.insurancepolicy.policyinfo"]'
    }, {
        ref: 'militaryService',
        selector: '[xtype="birls.birlsdetails.militaryservice.servicelist"]'
    }, {
        ref: 'militaryServiceVerification',
        selector: '[xtype="birls.birlsdetails.militaryservice.verificationinfo"]'
    }, {
        ref: 'serviceDiagnosticList',
        selector: '[xtype="birls.birlsdetails.servicediagnostics.diagnosticlist"]'
    }, {
        ref: 'serviceDiagnosticInfo',
        selector: '[xtype="birls.birlsdetails.servicediagnostics.additionalinfo"]'
    }, {
        ref: 'inactiveCompAndPension',
        selector: '[xtype="birls.birlsdetails.inactivecompandpension"]'
    }, {
        ref: 'misc1General',
        selector: '[xtype="birls.birlsdetails.misc1.general"]'
    }, {
        ref: 'misc1RetirePay',
        selector: '[xtype="birls.birlsdetails.misc1.retirepaysbp"]'
    }, {
        ref: 'misc1LastUpdated',
        selector: '[xtype="birls.birlsdetails.misc1.lastupdated"]'
    },
	{
	    ref: 'misc2Indicators',
	    selector: '[xtype="birls.birlsdetails.misc2.indicators"]'
	//},
	//{
	//    ref: 'displayclaimprocessingtimes',
	//    selector: 'button[action="displayclaimprocessingtimes"]'
	}
	],


    init: function () {
        var me = this;

        me.control({
            //'button[action="displayclaimprocessingtimes"]': {
            //    click: me.displayClaimProcessingTimes
            //},
            '[xtype="birls.birlsdetails"]': {
                tabchange: me.onBirlsDetailsTabChange
            },
            '[xtype="birls.identificationdata"] > toolbar > button[action="createpersonservicerequest"]': {
                click: me.personServiceRequest
            },
            '[xtype="birls.identificationdata"] > toolbar > button[action="createvai"]': {
                click: me.createVAI
            }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            cacheddataloaded: me.onCachedDataLoaded,
            birlsdetailstabchange: me.onBirlsDetailsTabChange,
            scope: me
        });

        Ext.log('The BIRLS controller was successfully initialized.');
    },

    onBirlsDetailsTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
            activeTab = me.getBirlsDetails().getActiveTab(),
            tabTitle = activeTab.title,
            gridCount = 0;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            if (Ext.get('id_birls_IdentificationData_01')) Ext.get('id_birls_IdentificationData_01').hide();
            if (Ext.get('id_birls_IdentificationData_02')) Ext.get('id_birls_IdentificationData_02').hide();
            if (Ext.get('id_birls_IdentificationData_03')) Ext.get('id_birls_IdentificationData_03').hide();
            if (Ext.get('id_birls_IdentificationData_04')) Ext.get('id_birls_IdentificationData_04').hide();
        }

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        }
        else if (activeTab.getXType() == 'birls.birlsdetails.militaryservice') {
            gridCount = me.getMilitaryService().getStore().getCount();
        }
        else if (activeTab.getXType() == 'birls.birlsdetails.insurancepolicy') {
            gridCount = me.getInsurancePolicies().getStore().getCount();
        }
        else if (activeTab.getXType() == 'birls.birlsdetails.servicediagnostics') {
            gridCount = me.getServiceDiagnosticList().getStore().getCount();
        }
        else if (activeTab.getXType() == 'birls.birlsdetails.folderlocations') {
            gridCount = me.getFolderLocationsList().getStore().getCount();
        }
        else {
            tabTitle = null;
        }
        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    onCachedDataLoaded: function () {
        var me = this;
        var r = me.getBirlsStore().getAt(0);

        if (Ext.isEmpty(r)) {
            return;
        }
        me.getAlternateNames().reconfigure(r.alternateNames());
        me.getDisclosures().reconfigure(r.recurringDisclosure());
        me.getFlashes().reconfigure(r.flashes());
        me.getFolderLocationsList().reconfigure(r.folders());
        me.getInsurancePolicies().reconfigure(r.insurancePolicies());
        me.getMilitaryService().reconfigure(r.services());
        me.getServiceDiagnosticList().reconfigure(r.serviceDiagnostic());

        me.getBirls().loadRecord(r);
        me.getMilitaryServiceVerification().loadRecord(r);
        me.getInsurancePolicyInfo().loadRecord(r);
        me.getInactiveCompAndPension().loadRecord(r);
        me.getServiceDiagnosticInfo().loadRecord(r);
        me.getFolderLocationInfo().loadRecord(r);
        me.getMisc1General().loadRecord(r);
        me.getMisc1RetirePay().loadRecord(r);
        me.getMisc1LastUpdated().loadRecord(r);
        me.getMisc2Indicators().loadRecord(r);

        // change caption of Claim Proc Time button 
        var claimFolderLocation = r.get('claimFolderLocation');
        if (!claimFolderLocation) claimFolderLocation = 'N/A';
        //me.getDisplayclaimprocessingtimes().setText(claimFolderLocation);
        me.application.fireEvent('updateclaimprocessingtimelabel', claimFolderLocation);
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        if (!Ext.isEmpty(selectedPerson.get('fileNumber'))) {
            me.getBirlsStore().load({
                filters: [{
                    property: 'fileNumber',
                    value: selectedPerson.get('fileNumber')
                }],
                callback: function (records, operation, success) {
                    if (!Ext.isEmpty(records)) {
                        me.getAlternateNames().reconfigure(me.getBirlsStore().getAt(0).alternateNames());
                        me.getDisclosures().reconfigure(me.getBirlsStore().getAt(0).recurringDisclosure());
                        me.getFlashes().reconfigure(me.getBirlsStore().getAt(0).flashes());
                        me.getFolderLocationsList().reconfigure(me.getBirlsStore().getAt(0).folders());
                        me.getInsurancePolicies().reconfigure(me.getBirlsStore().getAt(0).insurancePolicies());
                        me.getMilitaryService().reconfigure(me.getBirlsStore().getAt(0).services());
                        me.getServiceDiagnosticList().reconfigure(me.getBirlsStore().getAt(0).serviceDiagnostic());

                        //me.getBirls().loadRecord(me.getBirlsStore().getAt(0).getAlternatenames());
                        me.getBirls().loadRecord(records[0]);
                        me.getMilitaryServiceVerification().loadRecord(records[0]);
                        me.getInsurancePolicyInfo().loadRecord(records[0]);
                        me.getInactiveCompAndPension().loadRecord(records[0]);
                        me.getServiceDiagnosticInfo().loadRecord(records[0]);
                        me.getFolderLocationInfo().loadRecord(records[0]);
                        me.getMisc1General().loadRecord(records[0]);
                        me.getMisc1RetirePay().loadRecord(records[0]);
                        me.getMisc1LastUpdated().loadRecord(records[0]);
                        me.getMisc2Indicators().loadRecord(records[0]);

                        // change caption of Claim Proc Time button 
                        var claimFolderLocation = records[0].get('claimFolderLocation');
                        if (!claimFolderLocation) claimFolderLocation = 'N/A';
                        //me.getDisplayclaimprocessingtimes().setText(claimFolderLocation);

                        //Update the Person Info tab claim processing button text.
                        me.application.fireEvent('updateclaimprocessingtimelabel', claimFolderLocation);
                    }
                    me.application.fireEvent('webservicecallcomplete', operation, 'Birls');
                },
                scope: me
            });
        }
    },

    displayClaimProcessingTimes: function (claimFolderLocation) {
        var me = this;
        me.application.fireEvent('displayclaimprocessingtimefieldset', claimFolderLocation);
    },

    recordIsLoaded: function (promptIfNotSelected) {
        var me = this,
            birlsStore = me.getBirlsStore(); //Corp Store
        if (Ext.isEmpty(birlsStore.data) || Ext.isEmpty(birlsStore.data.items) || birlsStore.getCount() == 0) {
            if (promptIfNotSelected) {
                Ext.Msg.alert('BIRLS Store is not loaded.', 'Please Execute a Search first.');
            }
            return null;
        }

        var birlsRecord = birlsStore.getAt(0).data;
        return birlsRecord;
    },

    personServiceRequest: function () {
        me = this,
        birlsVetRecord = me.recordIsLoaded(true);

        if (birlsVetRecord == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a BIRLS service request', function (button) {
                if (button == 'no') return;
                else me.fireServiceRequestEvent(birlsVetRecord);
            });
        }
    },

    fireServiceRequestEvent: function (birlsVetRecord) {
        var me = this,
        //This json object will be sent to the CRM Restkit
        data = {
            "va_SRFirstName": birlsVetRecord.firstName,
            "va_SRLastName": birlsVetRecord.lastName,
            "va_SRSSN": birlsVetRecord.fileNumber,
            "va_SRDOBText": birlsVetRecord.dob,
            "va_DateofDeath": Ext.Date.format(new Date(birlsVetRecord.dod), 'm/d/Y'),
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
        me.application.serviceRequest.va_SelectedSSN = birlsVetRecord.ssn;

        me.application.fireEvent('createpersonservicerequest', data);
    },

    /**
    * This function fires off when the VAI button is clicked in the BIRLS toolbar   
    */
    createVAI: function (button) {
        var me = this,
            birlsVetRecord = me.recordIsLoaded(true);

        if (birlsVetRecord == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a BIRLS VAI', function (button) {
                if (button == 'no') return;
                else me.fireVaiEvent(birlsVetRecord);
            });
        }
    },

    fireVaiEvent: function (birlsVetRecord) {
        var me = this,
            selectionVariables = [];
        /*selectionVariables.Location
          selectionVariables.PID
          selectionVariables.SSN
          selectionVariables.PayeeCode*/

        //Add Global Vars for VAI params
        selectionVariables.PayeeCode = '00';
        selectionVariables.Location = 'BIRLS';
        selectionVariables.SSN = birlsVetRecord.ssn;
        selectionVariables.PID = null;

        me.application.fireEvent('createcrmvai', selectionVariables, birlsVetRecord);

        Ext.log('A Create VAI event was fired by the BIRLS controller');
    }
});
