/**
* @class VIP.controller.Claims
*
* Controller for the claims tabs under the claims tab
*/

Ext.define('VIP.controller.Claims', {
    extend: 'Ext.app.Controller',
    
    requires: [
        'VIP.model.Claims'
    ],
    
    mixins: {
        soj: 'VIP.mixin.StationOfJurisdiction',
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    
    refs: [{
        ref: 'loadRefreshAllClaims',
        selector: 'button[action="reloadAllClaims"]'
    }, {
        ref: 'claims',
        selector: 'claims'
    }, {
        ref: 'benefits',
        selector: '[xtype="claims.benefits"]'
    }, {
        ref: 'claimCounter',
        selector: '[xtype="claims.benefits"] > toolbar > tbtext[notificationType="claimcount"]'
    }, {
        ref: 'detailsTabPanel',
        selector: '[xtype="claims.details"]'
    }, {
        ref: 'trackedItemsGrid',
        selector: '[xtype="claims.details.trackeditems"]'
    }, {
        ref: 'lettersGrid',
        selector: '[xtype="claims.details.letters"]'
    }, {
        ref: 'claimDetailsSection',
        selector: '[xtype="claims.details.claimdetails"]'
    }, {
        ref: 'lifeCycleGrid',
        selector: '[xtype="claims.details.lifecycle"]'
    }, {
        ref: 'suspenseGrid',
        selector: '[xtype="claims.details.suspenses"]'
    }, {
        ref: 'notesGrid',
        selector: '[xtype="claims.details.notes"]'
    }, {
        ref: 'evidenceGrid',
        selector: '[xtype="claims.details.evidence"]'
    }, {
        ref: 'contentionsGrid',
        selector: '[xtype="claims.details.contentions"]'
    }, {
        ref: 'statusGrid',
        selector: '[xtype="claims.details.status"]'
    }, {
        ref: 'serviceRequestButton',
        selector: '[xtype="claims.benefits"] > splitbutton[action="startservicerequest"]'
    }],

    stores: [
        'Claims',
        'claims.ClaimDetail',
        'claims.Evidence',
        'claims.BenefitClaimDetailsByBnftClaimId'
],

    init: function () {
        var me = this;

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            awardcountforcadd: me.onReceivedAwardCountForCadd,
            cacheddataloaded: me.onCachedDataLoaded,
            triggerclaimtabchange: me.onClaimsTabChange,
            crmscriptinvoked: me.onCrmSriptInvoked,
            scope: me
        });

        me.control({
            'button[action="cad"]': {
                click: me.startChangeOfAddress
            },
            '[xtype="claims.benefits"] > toolbar > button[action="reloadAllClaims"]': {
                click: me.reloadAllClaimsData
            },
            '[xtype="claims.benefits"] > toolbar > splitbutton[action="createservicerequest"]': {
                click: me.serviceRequest
            },
            '[xtype="claims.benefits"] > toolbar > splitbutton[action="createservicerequest"] > menu': {
                click: me.serviceRequestMenuClick
            },
            '[xtype="claims.benefits"] > toolbar > button[action="createvai"]': {
                click: me.createVAI
            },
            '[xtype="claims.benefits"] > toolbar > button[action="claimprocessingtimes"]': {
                click: me.claimProcessingTimesMenuClick
            },
            '[xtype="claims.benefits"]': {
                select: me.onClaimsGridSelection
            },
            '[xtype="claims.benefits"] > toolbar > button[action="displayclaimscript"]': {
                click: me.onDisplayClaimScript
            },
            '[xtype="claims.details"]': {
                tabchange: me.onClaimsTabChange
            },
            '[xtype="claims.benefits"] > toolbar > button[action="viewclaimantcontact"]': {
                click: me.viewClaimantContactMenuClick
            },
            '[xtype="claims.benefits"] > toolbar > button[action="viewclaimvva"]': {
                click: me.invokeGetDocumentList
            }
        });

        Ext.log('The Claims controller has been initialized');
    },

    invokeGetDocumentList: function () {
        var me = this,
            selectedClaim = me.getBenefits().getSelectionModel().hasSelection() ? me.getBenefits().getSelectionModel().getSelection()[0] : null,
            vvaSearchValue = getVvaSearchValue(selectedClaim);

        if (!Ext.isEmpty(selectedClaim)) {
            vvaSearchValue = selectedClaim.get('claimId');
        }

        me.application.fireEvent('getDocumentListInvoked', vvaSearchValue);

        function getVvaSearchValue(selectedClaim) {
            var vvaSearchValue = !Ext.isEmpty(me.application.personInquiryModel) ? me.application.personInquiryModel.get('ssn') : null;

            if (Ext.isEmpty(vvaSearchValue)) {
                vvaSearchValue = !Ext.isEmpty(me.application.personInquiryModel) ? me.application.personInquiryModel.get('fileNumber') : null;
            }

            return vvaSearchValue;
        }
    },

    onCrmSriptInvoked: function (callType) {
        var me = this;

        if (callType != 'Claim') return;

        me.onDisplayClaimScript();
    },

    onClaimsGridSelection: function (rowModel, claim, index) {
        var me = this;
        _selectedClaim = claim;

        if (!Ext.isEmpty(claim) && me.getBenefits().getStore().getCount() > 0) {
            me.application.fireEvent('claimselected');
            me.application.fireEvent('claimrecordsloaded', claim, null);

            me.getClaimsBenefitClaimDetailsByBnftClaimIdStore().load({
                filters: [{
                    property: 'bnftClaimId',
                    value: _selectedClaim.data.claimId
                }],
                callback: function (records, operation, success) {
                    _selectedClaim.ebsRecords = records;
                    _selectedClaim.ebsXml = operation.response.xml;

                    me.application.fireEvent('webservicecallcomplete', operation, 'claims.BenefitClaimDetailsByBnftClaimId');
                },
                scope: me
            });
        }
    },

    reloadAllClaimsData: function () {
        var me = this;
        me.onIndividualIdentified(me.application.personInquiryModel);
        me.application.fireEvent('claimsrefreshed');
    },

    onClaimsTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
			activeTab = me.getDetailsTabPanel().getActiveTab(),
			tabTitle = activeTab.title,
			gridCount = null;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            //if (Ext.get('id_claims_Benefits_01')) Ext.get('id_claims_Benefits_01').hide();
            if (Ext.get('id_claims_Benefits_02')) Ext.get('id_claims_Benefits_02').hide();
            if (Ext.get('id_claims_Benefits_03')) Ext.get('id_claims_Benefits_03').hide();
            if (Ext.get('id_claims_Benefits_04')) Ext.get('id_claims_Benefits_04').hide();
            if (Ext.get('id_claims_Benefits_05')) Ext.get('id_claims_Benefits_05').hide();
            if (Ext.get('id_claims_Benefits_06')) Ext.get('id_claims_Benefits_06').hide();
            if (Ext.get('id_claims_Benefits_07')) Ext.get('id_claims_Benefits_07').hide();
            if (Ext.get('id_claims_Benefits_08')) Ext.get('id_claims_Benefits_08').hide();
            if (Ext.get('id_claims_Benefits_09')) Ext.get('id_claims_Benefits_09').hide();
            if (Ext.get('id_claims_Benefits_10')) Ext.get('id_claims_Benefits_10').hide();
            if (Ext.get('id_claims_Benefits_11')) Ext.get('id_claims_Benefits_11').hide();
            if (Ext.get('id_claims_Benefits_12')) Ext.get('id_claims_Benefits_12').hide();
            if (Ext.get('id_claims_Benefits_13')) Ext.get('id_claims_Benefits_13').hide();
            if (Ext.get('id_claims_Benefits_14')) Ext.get('id_claims_Benefits_14').hide();
            if (Ext.get('id_claims_Benefits_15')) Ext.get('id_claims_Benefits_15').hide();
            if (Ext.get('id_claims_Benefits_16')) Ext.get('id_claims_Benefits_16').hide();
            if (Ext.get('id_claims_Benefits_17')) Ext.get('id_claims_Benefits_17').hide();
            if (Ext.get('id_claims_Benefits_18')) Ext.get('id_claims_Benefits_18').hide();

            if (Ext.get('id_createNoteButton')) Ext.get('id_createNoteButton').hide();
            if (Ext.get('createNoteButton')) Ext.get('createNoteButton').hide();
            if (Ext.get('id_editNoteButton')) Ext.get('id_editNoteButton').hide();
            if (Ext.get('editNoteButton')) Ext.get('editNoteButton').hide();
            if (Ext.get('id_openSelectedNote')) Ext.get('id_openSelectedNote').hide();
            if (Ext.get('openSelectedNote')) Ext.get('openSelectedNote').hide();
            if (Ext.get('id_openDisplayedNotes')) Ext.get('id_openDisplayedNotes').hide();
            if (Ext.get('openDisplayedNotes')) Ext.get('openDisplayedNotes').hide();
        }

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        } else {
            tabTitle = null;
        }
        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    onCachedDataLoaded: function () {
        var me = this,
			cStore = null,
			claimsStore = me.getClaimsStore(),
			evidenceStore = me.getClaimsEvidenceStore();

        _selectedClaim = null;
        _claimDetailLoaded = false;
        _claimDetailRecord = null;

        if (!Ext.isEmpty(claimsStore)) {
            cStore = claimsStore;
            me.getBenefits().reconfigure(claimsStore);
            me.application.fireEvent('claimscacheddataloaded', claimsStore);
        }
        me.application.fireEvent('claimsloaded', cStore);

        if (!Ext.isEmpty(evidenceStore)) {
            me.getEvidenceGrid().reconfigure(evidenceStore);
        }
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;

        _selectedClaim = null;
        _claimDetailLoaded = false;
        _claimDetailRecord = null;

        me.getBenefits().setLoading(true, true);

        me.getClaimsStore().load({
            filters: [{
                property: 'fileNumber',
                value: selectedPerson.get('fileNumber')
            }],
            callback: function (records, operation, success) {
                var resultSet = operation.getResultSet(),
					claimStore = me.getClaimsStore(),
					claimRecords = [],
					claim = Ext.create('VIP.model.Claims'),
                    participantId = !Ext.isEmpty(records) ? records[0].get('participantClaimantId') : null;

                me.getBenefits().setLoading(false);


                me.application.fireEvent('webservicecallcomplete', operation, 'Claims');

                var cStore = null;
                if (success && !Ext.isEmpty(records) && !Ext.isEmpty(claimStore)) {
                    cStore = claimStore;
                    for (var i = 0; i < records.length; i++) {
                        var tempClaim = records[i];
                        if (!Ext.isEmpty(tempClaim)) {
                            claimRecords.push(tempClaim);
                        }
                    }

                    claimStore.removeAll();
                    if (!Ext.isEmpty(resultSet.isSingleResponse)) {
                        claimStore = me.getClaimsClaimDetailStore();

                        claimStore.loadRecords(claimRecords, {
                            addRecords: false
                        });
                        me.getBenefits().reconfigure(claimStore);

                        claim = claimStore.getAt(0);

                        me.application.fireEvent('webservicecallcomplete', operation, 'claims.ClaimDetail');
                    }
                    else {
                        claimStore.loadRecords(claimRecords, {
                            addRecords: false
                        });
                        me.getBenefits().reconfigure(claimStore);
                    }
                    if (!participantId) participantId = selectedPerson.get('participantId');
                    claim.set('participantId', participantId);
                    claim.commit();
                    me.updateClaimCount();
                    me.application.fireEvent('claimrecordsloaded', claim, operation.response);
                }
                me.application.fireEvent('claimsloaded', cStore);
            },
            scope: me
        });

        me.getEvidenceGrid().reconfigure(
		me.getClaimsEvidenceStore().load({
		    filters: [{
		        property: 'Claiment_ptpcpnt_id',
		        value: selectedPerson.get('participantId')
		    }],
		    callback: function (records, operation, success) {
		        me.application.fireEvent('webservicecallcomplete', operation, 'claims.Evidence');
		    },
		    scope: me
		}));
    },

    GetSelectedClaimRecord: function (promptIfNotSelected) {
        var me = this;
        var claimsGrid = me.getBenefits(); //claims grid
        var selectedRecords = claimsGrid.getSelectionModel().getSelection();
        if (Ext.isEmpty(selectedRecords)) {
            if (promptIfNotSelected) {
                Ext.Msg.alert('No Claim Selected', 'Please select a claim from the grid first.');
            }
            return null;
        }

        var rec = selectedRecords[0];
        return rec;
    },

    onReceivedAwardCountForCadd: function (awardCount) {
        Ext.log('An awardcount event was received by the Claims controller');
        //debugger
        var me = this;
        if (awardCount > 0) {
            //Ext.Msg.alert('Change of Address', "You cannot initiate CAD from Claims if there are awards on the person's record. Please initiate CAD from Awards.");
            /*
            if (confirm("There are awards on the person's record.\n\nTo initiate Change of Address from Awards tab, please select OK to stop current operation and navigate to Awards tab.\nTo override standard processing and initiate Change of Address from Claims, regardless of existing Awards, please select Cancel.")) {
            return;
            } */

            Ext.Msg.alert('Change of Address - Awards Exist', 'There are awards on the person\'s record.<br/><br/>Please click on Awards tab and select the approriate award for the change of address');
            return;

            //Ext.Msg.show({
            //    title: 'Change of Address - Awards Exist',
            //    msg: "There are awards on the person's record. Are you sure you want to initiate Change of Address?<br/><br/>To stop current operation and to initiate Change of Address from Awards tab, please select 'No' and navigate to Awards tab.<br/>To override standard processing and initiate Change of Address from Claims, regardless of existing Awards, please select 'Yes'.",
            //    icon: 'ext-mb-warning',
            //    buttons: Ext.Msg.YESNO,
            //    fn: function (selection) {
            //        if (selection == 'yes') {
            //            me.doContinueCAD(awardCount);
            //        }
            //        else {
            //            return;
            //        }
            //    }
            //});
        }
        else {
            me.doContinueCAD(awardCount);
        }
    },

    doContinueCAD: function (awardCount) {
        var me = this, selectedClaim = me.GetSelectedClaimRecord(true);

        if (!selectedClaim) {
            return;
        }
        var selection = null;

        me.getClaimsClaimDetailStore().load({
            filters: [{
                property: 'benefitClaimId',
                value: selectedClaim.get('claimId')
            }],
            callback: function (records, operation, success) {
                me.application.fireEvent('webservicecallcomplete', operation, 'claims.ClaimDetail');

                if (records[0].data) {
                    participantVetID = records[0].get('participantVetID');
                    participantClaimantID = records[0].get('participantClaimantID');
                    programTypeCode = records[0].get('programTypeCode');
                    payeeTypeCode = records[0].get('payeeTypeCode');

                    selection = {
                        "participantVetID": participantVetID,
                        "participantClaimantID": participantClaimantID,
                        "programTypeCode": programTypeCode,
                        "payeeTypeCode": payeeTypeCode,
                        "openedFromClaimTab": true,
                        "appealsOnly": false,
                        "openedFromAwardsTab": false,
                        "ro": false
                    };

                    parent._ChangeOfAddressOnClick(selection);
                } else {
                    Ext.Msg.alert('Change of Address', 'Unable to retrieve key information to initiate CADD.');
                }
            }
        });
    },

    updateClaimCount: function () {
        var me = this,
			claimStore = me.getBenefits().getStore();

        me.getClaimCounter().setText('Claims: ' + claimStore.getCount());
    },

    startChangeOfAddress: function () {
        var me = this;
        var selectedClaim  = me.GetSelectedClaimRecord(true);
        me.application.fireEvent('claimscaddstarted', selectedClaim.data.payeeTypeCode);
        Ext.log('A cadd event was fired by the Claims controller');
    },

    /**
    * This function fires off when the service request button is clicked in the Claim toolbar without selecting an option from the dropdown
    * Default is the 0820 option
    */
    serviceRequest: function (button) {
        var me = this,
			defaultSelection = button.defaultMenuSelection,
            selectedClaim = me.GetSelectedClaimRecord(true);

        if (selectedClaim == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else if (!Ext.isEmpty(defaultSelection)) {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Claim service request', function (button) {
                if (button == 'no') return;
                else me.fireServiceRequestEvent({ name: defaultSelection.text, value: parseInt(defaultSelection.value) }, selectedClaim);
            });
        }
    },

    /**
    * This function fires off when a service request menu button is clicked in the Claim toolbar.
    */
    serviceRequestMenuClick: function (menu, item, e, eOpts) {
        var me = this,
            selectedClaim = me.GetSelectedClaimRecord(true);

        if (selectedClaim == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else if (!Ext.isEmpty(item)) {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Claim service request', function (button) {
                if (button == 'no') return;
                else me.fireServiceRequestEvent({ name: item.text, value: parseInt(item.value) }, selectedClaim);
            });
        }
    },

    /**
    * This function is called from the service request menu click.  It will gather the information from the selected award and 
    * and detail panel and fire off the service request event in the Service Request controller.
    */
    fireServiceRequestEvent: function (serviceRequestType, selectedClaim) {
        var me = this,
            participantId = selectedClaim.get('participantClaimantID') ? selectedClaim.get('participantClaimantID') : selectedClaim.get('participantId'),
            contentionStore = me.getContentionsGrid().getStore(),
            contentions = '',
            lifeCycleStore = me.getLifeCycleGrid().getStore(),
            lifeCycleModel = '',
            claimDetails = '',
            claimId = selectedClaim.get('benefitClaimID') ? selectedClaim.get('benefitClaimID') : selectedClaim.get('claimId'),
            trackedItemsStore = me.getTrackedItemsGrid().getStore(),
            closedOrReceivedTrackedItems = 0,
            trackedItemNote = null,
            personOrOrgIndicator = selectedClaim.get('claimantPersonOrOrganizationIndicator') ? selectedClaim.get('claimantPersonOrOrganizationIndicator') : selectedClaim.get('personOrOrganizationIndicator'),
            data = null;

        //Get a list of a contentions
        contentionStore.each(function (record) {
            contentions += record.get('contentclass') + '\n';
        });

        //Added Global Vars 2/23/13
        me.application.serviceRequest.va_SelectedPayeeCode = selectedClaim.get('payeeTypeCode');
        me.application.serviceRequest.va_ServiceRequestType = 'Claim';
        me.application.serviceRequest.va_SelectedSSN = '';
        me.application.serviceRequest.va_SelectedPID = selectedClaim.get('participantClaimantID') ? selectedClaim.get('participantClaimantID') : selectedClaim.get('participantId');

        //Determine if all tracked items are closed or received
        trackedItemsStore.each(function (record) {
            if (record.get('acceptDate') || record.get('receiveDate')) {
                closedOrReceivedTrackedItems++;
            }
        });
        if (trackedItemsStore.count() == closedOrReceivedTrackedItems) {
            trackedItemNote = "\nThis claim has all tracked items received or closed and should have the status changed to 'Ready for Decision'.\n";
        }

        //if the claim selected claim doesn't have a Participant Claimant Id, get the Vet participant Id
        if (Ext.isEmpty(participantId)) participantId = me.application.personInquiryModel.get('participantId');

        //Get the newest life cycle record so we can get the SOJ from it
        lifeCycleStore.sort('changedDate', 'DESC');
        lifeCycleModel = lifeCycleStore.first();

        //Selected claim info
        var relatedClaimSummary = (claimId ? claimId + " - " : "") +
			(selectedClaim.get('claimTypeName') ? selectedClaim.get('claimTypeName') + " - " : "") +
			(selectedClaim.get('statusTypeCode') ? selectedClaim.get('statusTypeCode') : "");

        if (parent.Xrm.Page.data.entity.getEntityName() == 'phonecall') {
            relatedClaimSummary = relatedClaimSummary + " - " +
                (parent.Xrm.Page.getAttribute('va_dispositionsubtype').getSelectedOption() ?
                    parent.Xrm.Page.getAttribute('va_dispositionsubtype').getSelectedOption().text : '');
        }

        //Block of text of claim details
        claimDetails = "Claim Receive Date: " + selectedClaim.get('claimReceiveDate_f') + "\n" +
		    "Last Action Date: " + selectedClaim.get('lastActionDate_f') + "\n" +
		    "Claimant FN: " + selectedClaim.get('claimantFirstName') + "\n" +
		    "Claimant LN: " + selectedClaim.get('claimantLastName') + "\n" +
		    "Claim Type: " + selectedClaim.get('claimTypeName') + "\n" +
		    "Payee Code: " + selectedClaim.get('payeeTypeCode') + "\n" +
		    "Status Code: " + selectedClaim.get('statusTypeCode') + "\n" +
		    "Program Code: " + selectedClaim.get('programTypeCode') + "\n" +
		    "End Product: " + selectedClaim.get('endProductTypeCode') + "\n" +
		    "Person or Org: " + personOrOrgIndicator + "\n" +
		    "Organization: " + selectedClaim.get('organizationName') + "\n" +
		    (trackedItemNote == null ? "" : trackedItemNote) +
		    "\nCONTENTIONS:\n" + contentions;

        //This json object will be sent to the CRM Restkit
        data = {
            "va_ClaimDetails": claimDetails,
            "va_ClaimNumber": claimId,
            "va_ParticipantID": participantId,
            "va_Claim": relatedClaimSummary,
            "va_AllTrackedItemsReceivedOrClosed": trackedItemNote ? true : false
        };

        if (lifeCycleModel) {
            data.va_RegionalOfficeId = me.getStationOfJurisdiction(lifeCycleModel.get('stationOfJurisdiction'));
        }

        //This event is caught in the Service Request controller
        me.application.fireEvent('createclaimservicerequest', serviceRequestType, data);
    },

    onDisplayClaimScript: function () {
        Ext.log('A displayclaimscript event has been received by the Claim controller');
        var me = this;

        if (!parent || !parent._SetPrimaryTypeSubtype) {
            alert('This operation is available only from CRM window');
            return;
        }

        if (Ext.isEmpty(_selectedClaim) || Ext.isEmpty(_selectedClaim.get('claimId'))) {
            // get first claim in the grid
            if (!confirm('You have not selected a Claim; Claim Script might be populated incorrectly or incompletely.\n\nTo cancel Claim Script, click on Cancel button. To proceed even though Claim is not selected, click on OK.')) {
                return false;
            }
            if (me.getBenefits().getStore().getCount() > 0) { _selectedClaim = me.getBenefits().getStore().getAt(0); }
        }

        var isLoaded = true;
        try {
            if (_claimDetailLoaded != undefined && _claimDetailLoaded == false) {
                isLoaded = false;
            }
        } catch (ler) { }

        if (isLoaded == false) {
            if (me.getClaimsStore().getCount() > 1) {
                var selectedRec = me.GetSelectedClaimRecord(false);
                if (Ext.isEmpty(selectedRec)) {
                    // do nothing
                    //alert('Claim detail data is loading. Please wait a moment and click on Script button again.');
                    //return;
                }
                else {
                    _claimDetailRecord = selectedRec;
                }
            } else {
                var det = me.getClaimsClaimDetailStore();
                if (det && det.getCount() > 0) {
                    _claimDetailRecord = det.getAt(0);
                }
            }
        }

        var claimStatus = '';
        if (!Ext.isEmpty(_selectedClaim)) {
            claimStatus = _selectedClaim.get('statusTypeCode') ? _selectedClaim.get('statusTypeCode').toUpperCase() : '';
            var isCancelled = ((claimStatus == 'CLR') || (claimStatus == 'CAN'));
            //Change this to look at the Claim Status subtab?
            if (isCancelled) {
                if (!confirm('Claim is closed. Are you sure you want to see Claim Script?')) {
                    return;
                }
            }

            // evidence
            _selectedClaim.evidenceRecords = null;
            var evStore = me.getClaimsEvidenceStore();
            if (evStore) {
                _selectedClaim.evidenceRecords = evStore.data;
            }

            // contentions
            _selectedClaim.contentionsRecords = null;
            var contStore = me.getContentionsGrid().getStore();
            if (contStore) {
                _selectedClaim.contentionsRecords = contStore.data;
            }

            // tracked items
            _selectedClaim.trackedItems = null;
            var trStore = me.getTrackedItemsGrid().getStore();
            if (trStore) {
                _selectedClaim.trackedItems = trStore.data;
            }

            // lifecycle
            _selectedClaim.lifeCycleRecords = null;
            var lifeStore = me.getLifeCycleGrid().getStore();
            if (lifeStore) {
                _selectedClaim.lifeCycleRecords = lifeStore.data;
            }

            // status
            _selectedClaim.statusRecords = null;
            var statusStore = me.getStatusGrid().getStore();
            if (statusStore) {
                _selectedClaim.statusRecords = statusStore.data;
            }

            // suspenses
            _selectedClaim.suspenseRecords = null;
            var suspenseStore = me.getSuspenseGrid().getStore();
            if (suspenseStore) {
                _selectedClaim.suspenseRecords = suspenseStore.data;
            }
        }
        parent._SetPrimaryTypeSubtype('CLAIM_GENERAL', true, false);

        var scriptSource = parent._KMRoot + 'claimStatus.html';

        //if (!Ext.isEmpty(_claimScriptWindowHandle) && !_claimScriptWindowHandle.closed) { try { _claimScriptWindowHandle.close(); } catch (ex) { } }
        _claimScriptWindowHandle = window.open(scriptSource, "CallScript", "width=1024,height=800,scrollbars=1,resizable=1");
        _claimScriptWindowHandle.focus();

        Ext.log('A displayclaimscript event has been processed by the Claim controller');
    },

    createVAI: function (button) {
        var me = this,
            selectedClaim = me.GetSelectedClaimRecord(true);

        if (selectedClaim == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Claim VAI', function (button) {
                if (button == 'no') return;
                else me.fireVaiEvent(selectedClaim);
            });
        }
    },

    fireVaiEvent: function (selectedClaim) {
        var me = this,
            lifeCycleModel,
            lifeCycleStore = me.getLifeCycleGrid().getStore(),
            selectionVariables = [];

        //Add Global Vars for VAI params
        selectionVariables.PayeeCode = selectedClaim.get('payeeTypeCode');
        selectionVariables.Location = 'CLAIM';
        selectionVariables.SSN = '';
        selectionVariables.PID = selectedClaim.get('participantClaimantID') ? selectedClaim.get('participantClaimantID') : selectedClaim.get('participantId');

        //Get the newest life cycle record so we can get the SOJ from it
        lifeCycleStore.sort('changedDate', 'DESC');
        lifeCycleModel = lifeCycleStore.first();

        if (lifeCycleModel) {
            var stationOfJurisdiction = lifeCycleModel.get('stationOfJurisdiction');
            
            if (!Ext.isEmpty(stationOfJurisdiction) && stationOfJurisdiction.length > 2) {
                var sojCode = stationOfJurisdiction.substring(0, 3);

                me.getStationOfJurisdictionAsync(sojCode, function (soj) {
                    if (soj) {
                        if (!soj.isPilot) {
                            alert(me.sojIsNotPilotMessage());
                            return;
                        }

                        selectedClaim.data.va_RegionalOfficeId = soj.entityReference;
                    }

                    me.application.fireEvent('createcrmvai', selectionVariables, selectedClaim);
                    Ext.log('A Create VAI event was fired by the Claims controller');
                });
            } else {
                me.application.fireEvent('createcrmvai', selectionVariables, selectedClaim);
                Ext.log('A Create VAI event was fired by the Claims controller');
            }
        } else {
            me.application.fireEvent('createcrmvai', selectionVariables, selectedClaim);
            Ext.log('A Create VAI event was fired by the Claims controller');
        }
    },

    claimProcessingTimesMenuClick: function () {
        var me = this, soj = null;

        var selClaim = me.GetSelectedClaimRecord(false);

        if (!Ext.isEmpty(selClaim)) {
            // Get lifecycle
            var lifeCycleRecords = null;
            //Need to remove this try catch as an if then
            try {
                lifeCycleRecords = _claimDetailRecord.lifeCycleRecords();
                if (lifeCycleRecords) { lifeCycleRecords = lifeCycleRecords.data; }
            }
            catch (lce) {
            //increased validation for single record in grid
                if (parent && parent.opener && parent.opener._claimDetailRecord) { lifeCycleRecords = parent.opener._claimDetailRecord.lifeCycleRecords; }
            }
            if (lifeCycleRecords && lifeCycleRecords.items) {
                var curLFDt = null;
                for (var i = 0; i < lifeCycleRecords.items.length; i++) {
                    var curSOJ = lifeCycleRecords.items[i].data.stationOfJurisdiction;
                    var curDate = lifeCycleRecords.items[i].data.changedDate;

                    if (curLFDt == null || new Date(curDate) > new Date(curLFDt)) {
                        curLFDt = curDate;
                        soj = curSOJ;
                    }
                }
            }

            if (soj == null || soj == undefined || soj.length == 0) {
                // No lifecycle, check Status

                selClaim.statusRecords = null;
                var statusStore = me.getStatusGrid().getStore();
                if (statusStore) {
                    selClaim.statusRecords = statusStore.data;
                }

                var statusChangeDate = null;
                var statusRecords = selClaim.statusRecords;
                var changeDate = null;

                if (statusRecords && statusRecords.items) {

                    for (i = 0; i < statusRecords.items.length; i++) {
                        changeDate = statusRecords.items[i].data.changedDate;
                        //curSOJ = statusRecords.items[i].data.actionLocationId;
                        curSOJ = statusRecords.items[i].data.journalLocationId;
                        //var status = statusRecords.items[i].data.claimLocationStatusTypeName;
                        if ((new Date(changeDate) > new Date(statusChangeDate) || statusChangeDate == null)
                        && !Ext.isEmpty(statusRecords.items[i].data.actionLocationId)) {
                            statusChangeDate = changeDate;
                            soj = curSOJ;
                        }
                    }
                }
            }
        }

        me.application.fireEvent('displayclaimprocessingtimemainclaim', soj);
    },

    viewClaimantContactMenuClick: function () {
        var me = this;

        me.viewClaimant();
    },

    // handle View Contact viewclaimantcontact
    viewClaimant: function () {
        var me = this,
            claimDetail = me.getClaimsClaimDetailStore().getAt(0);

        if (!Ext.isEmpty(claimDetail)) {
            if (claimDetail.get('participantClaimantID').localeCompare(claimDetail.get('participantVetID')) == 0) {
                Ext.Msg.alert('Record for Selected Person is Already Open.',
                    'Claimant on the selected claim is the same person whose data is displayed on the Phone Call screen.<br/>New screen will not open.');
                return null;
            }

        }
        else {
            claimDetail = me.getClaimDetailsSection().getRecord();
            if (Ext.isEmpty(claimDetail)) {
                Ext.Msg.alert('No claim selected', 'Please select a claim from the grid first.');
                return null;
            }
        }

        var claimant = {
            "ptcpntId": claimDetail.get('participantClaimantID'),
            "firstname": claimDetail.get('claimantFirstName'),
            "middlename": claimDetail.get('claimantMiddleName'),
            "lastname": claimDetail.get('claimantLastName'),
            "SSN": null,
            "dod": null,
            "dob": null
        };

        if (!parent || !parent._ViewContact) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return null;
        }

        parent._ViewContact(claimant);
    }

});