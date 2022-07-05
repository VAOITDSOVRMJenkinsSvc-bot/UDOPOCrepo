/**
* @author Ivan Yurisevic
* @class VIP.controller.Appeals
*
* The controller for the Fiduciary tab
*/
Ext.define('VIP.controller.Appeals', {
    extend: 'Ext.app.Controller',
    uses: [
        'Ext.util.MixedCollection'
    ],
    stores: ['Appeal'],
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    refs: [
		{
		    ref: 'appealsGrid',
		    selector: '[xtype="appeals.appealdata"]'
		},
		{
		    ref: 'appealDetails',
		    selector: '[xtype="appeals.details.appealrecord"]'
		},
        {
            ref: 'appealTabs',
            selector: '[xtype="appeals.details"]'
        },
        {
            ref: 'appealCounter',
            selector: '[xtype="appeals.appealdata"] > toolbar > tbtext[notificationType="appealcount"]'
        },
		{
		    ref: 'appellant',
		    selector: '[xtype="appeals.details.appellant"]'
		},
		{
		    ref: 'appealIssues',
		    selector: '[xtype="appeals.details.issuesremandreasons.issues"]'
		},
		{
		    ref: 'remandReasons',
		    selector: '[xtype="appeals.details.issuesremandreasons.remandreasons"]'
		},
		{
		    ref: 'appealDiaries',
		    selector: '[xtype="appeals.details.diaries"]'
		},
		{
		    ref: 'appealDecision',
		    selector: '[xtype="appeals.details.appealdecisionspecialcontentions.appealdecision"]'
		},
		{
		    ref: 'specialContentions',
		    selector: '[xtype="appeals.details.appealdecisionspecialcontentions.specialcontentions"]'
		},
		{
		    ref: 'appealDates',
		    selector: '[xtype="appeals.details.appealdates"]'
		},
		{
		    ref: 'hearingRequest',
		    selector: '[xtype="appeals.details.hearingrequest"]'
		}
	],

    init: function () {
        var me = this;
        me.control({
            '[xtype="appeals.appealdata"] > toolbar > button[action="appealscript"]': {
                click: me.showPaymentScript
            },
            '[xtype="appeals.appealdata"] > toolbar > button[action="loadappealdata"]': {
                click: me.reloadAllData
            },
            '[xtype="appeals.appealdata"] > toolbar > button[action="changeofaddress"]': {
                click: me.startCAD
            },
            '[xtype="appeals.appealdata"]': {
                select: me.onAppealGridSelection
            },
            '[xtype="appeals.details.issuesremandreasons.issues"]': {
                select: me.onAppealIssueGridSelection
            },
            '[xtype="appeals.details"]': {
                tabchange: me.onAppealTabChange
            }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            updateclaimprocessingtimelabel: me.onUpdatedClaimProcessingTimeLabel,
            gotcorp: me.onGotCorpData,
            gotpoa: me.onGotPOA,
            crmscriptinvoked: me.onCrmScriptInvoked,
            triggerappealtabchange: me.onAppealTabChange,
            crmchangeofaddresscompleted: me.reloadAllData,
            scope: me
        });
    },

    onCrmScriptInvoked: function (callType) {
        var me = this;

        if (callType != 'Appeals') return;

        me.showPaymentScript();
    },

    onGotCorpData: function (corpData) {
        var me = this;
        me.corpData = corpData;
    },
    onGotPOA: function (POAData) {
        var me = this;
        me.POAData = POAData;
    },

    // receive event from BIRLS tab and show claims folder SOJ
    onUpdatedClaimProcessingTimeLabel: function (claimFolderLocation) {
        var me = this;
        me.claimFolderLocation = claimFolderLocation;
    },

    onAppealTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
            activeTab = me.getAppealTabs().getActiveTab(),
            tabTitle = activeTab.title,
            gridCount = null;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            //if (Ext.get('id_appeals_AppealData_01')) Ext.get('id_appeals_AppealData_01').hide();
            if (Ext.get('id_appeals_AppealData_02')) Ext.get('id_appeals_AppealData_02').hide();
            if (Ext.get('id_appeals_AppealData_03')) Ext.get('id_appeals_AppealData_03').hide();
            if (Ext.get('id_appeals_AppealData_04')) Ext.get('id_appeals_AppealData_04').hide();
            if (Ext.get('id_appeals_AppealData_05')) Ext.get('id_appeals_AppealData_05').hide();
            if (Ext.get('id_appeals_AppealData_06')) Ext.get('id_appeals_AppealData_06').hide();
            if (Ext.get('id_appeals_AppealData_07')) Ext.get('id_appeals_AppealData_07').hide();
        }

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        } else if (activeTab.getXType() == 'appeals.details.issuesremandreasons') {
            tabTitle = 'Issues: ' + me.getAppealIssues().getStore().getCount() +
                       ', Remands: ' + me.getRemandReasons().getStore().getCount();
        } else {
            tabTitle = null;
        }
        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    onAppealIssueGridSelection: function (rowModel, appeal, index) {
        var me = this;
        me.selectedAppealIssue = null;

        if (!Ext.isEmpty(appeal) && !Ext.isEmpty(appeal.raw)) {
            me.selectedAppealIssue = appeal;
            var issueXmlData = appeal.raw,
				remandReasons = appeal.remandReasons(),
				remandReasonResultSet = remandReasons.getProxy().getReader().read(issueXmlData),
				rrGrid = me.getRemandReasons();

            if (!Ext.isEmpty(remandReasonResultSet) && !Ext.isEmpty(remandReasonResultSet.records)) {
                remandReasons.loadRecords(remandReasonResultSet.records, { addRecords: false });
                rrGrid.reconfigure(remandReasons);
            }
            else {
                // clear remand reasons
                var rrStore = rrGrid.getStore();
                if (rrStore.getCount() > 0) {
                    rrStore.removeAll();
                }
            }
        }
    },

    onAppealGridSelection: function (rowModel, appeal, index) {
        var me = this;
        me.selectedAppeal = appeal;
        me.selectedAppeal.appealDetailRecord = null;

        me.loadAppealDetails(appeal);
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this,
            personInquiry = me.application.personInquiryModel,
            filters = loadAppealsFilters(selectedPerson, personInquiry);

        //Reset form fields for different search
        me.getAppealDetails().getForm().reset();
        me.getAppellant().getForm().reset();
        me.getAppealDecision().getForm().reset();
        me.getHearingRequest().getForm().reset();

        //Set load mask for main appeal grid
        me.getAppealsGrid().setLoading(true, true);
        me.getAppealStore().load({
            filters: Ext.clone(filters),
            callback: function (records, operation, success) {
                me.getAppealsGrid().setLoading(false);

                if (!Ext.isEmpty(records) && success) {
                    if (records.length == 1) {
                        var appeal = records[0];
                        if (!Ext.isEmpty(appeal)) {
                            me.selectedAppeal = appeal;
                            me.selectedAppeal.appealDetailRecord = null;
                            me.loadAppealDetails(appeal);
                        }
                    }

                    me.updateAppealCount();
                }

                me.application.fireEvent('webservicecallcomplete', operation, 'Appeal');
            },
            scope: me
        });

        function loadAppealsFilters(selectedPerson, personInquiry) {
            var filters = Ext.create('Ext.util.MixedCollection'),
                searchByValue = personInquiry.get('appealsSearchValue');

            if (!Ext.isEmpty(personInquiry)) {
                if (searchByValue == 'values') {
                    filters.addAll([
                        {
                            property: 'ssn',
                            value: personInquiry.get('appealsSsn')
                        },
                        {
                            property: 'firstName',
                            value: personInquiry.get('appealsFirstName')
                        },
                        {
                            property: 'lastName',
                            value: personInquiry.get('appealsLastName')
                        },
                        {
                            property: 'dateOfBirth',
                            value: personInquiry.get('appealsDateOfBirth')
                        },
                        {
                            property: 'city',
                            value: personInquiry.get('appealsCity')
                        },
                        {
                            property: 'state',
                            value: personInquiry.get('appealsState')
                        }
                    ]);
                }
                else {
                    filters.add({
                        property: 'ssn',
                        value: getSearchParameter(selectedPerson, personInquiry, searchByValue)
                    });
                }
            }

            //this is a double-check to remove filters if they are empty
            filters.each(function (filter) {
                if (Ext.isEmpty(filter.value)) {
                    filters.remove(filter);
                }
            });

            return filters.getRange();

            function getSearchParameter(selectedPerson, personInquiry, searchByValue) {
                var me = this,
                searchValue = selectedPerson.get('fileNumber');

                if (!Ext.isEmpty(searchByValue)) {
                    if (searchByValue == 'fileNumber') { searchValue = selectedPerson.get('fileNumber'); }
                    else if (searchByValue == 'ssn') { searchValue = selectedPerson.get('ssn'); }
                }

                return searchValue;
            }
        }
    },

    updateAppealCount: function () {
        var me = this,
            appealStore = me.getAppealsGrid().getStore();

        me.getAppealCounter().setText('Appeals: ' + appealStore.getCount());
    },

    loadAppealDetails: function (appeal) {
        var me = this;

        me.getAppealDetails().getForm().reset();
        me.getAppellant().getForm().reset();
        me.getAppealDecision().getForm().reset();
        me.getHearingRequest().getForm().reset();

        me.getAppealDetails().setLoading(true, true);
        me.getAppellant().setLoading(true, true);
        me.getAppealDecision().setLoading(true, true);
        me.getHearingRequest().setLoading(true, true);
        me.getAppealIssues().setLoading(true, true);
        me.getAppealDiaries().setLoading(true, true);
        me.getSpecialContentions().setLoading(true, true);
        me.getAppealDates().setLoading(true, true);

        appeal.details().load({
            filters: [
				{
				    property: 'AppealKey',
				    value: appeal.get('appealKey')
				}
			],
            callback: function (records, operation, success) {
                me.selectedAppeal.appealDetailRecord = null;

                me.getAppealDetails().setLoading(false);
                me.getAppellant().setLoading(false);
                me.getAppealDecision().setLoading(false);
                me.getHearingRequest().setLoading(false);
                me.getAppealIssues().setLoading(false);
                me.getAppealDiaries().setLoading(false);
                me.getSpecialContentions().setLoading(false);
                me.getAppealDates().setLoading(false);

                if (success && !Ext.isEmpty(records) && !Ext.isEmpty(operation) && !Ext.isEmpty(operation.response)) {
                    var appealDetailRecord = records[0];
                    me.selectedAppeal.appealDetailRecord = appealDetailRecord;

                    if (!Ext.isEmpty(appealDetailRecord)) {
                        //Load the Grids
                        me.getAppealIssues().reconfigure(appealDetailRecord.issues());
                        me.getAppealDiaries().reconfigure(appealDetailRecord.diaries());
                        me.getSpecialContentions().reconfigure(appealDetailRecord.specialContentions());
                        me.getAppealDates().reconfigure(appealDetailRecord.dates());

                        //Load the Panels - check for null data... don't want to send it 'undefined'
                        //RTC: 158386 - Remove Appeallant Address View, Load values from AppellantAddressStore to AppellantStore, Combine Appellant and Appellant Vet.
                        me.getAppealDetails().loadRecord(appealDetailRecord);
                        if (appealDetailRecord.appealDecision().getAt(0)) {
                            me.getAppealDecision().loadRecord(appealDetailRecord.appealDecision().getAt(0));
                        }
                        if (appealDetailRecord.appellant().getAt(0)) {
                            me.getAppellant().loadRecord(appealDetailRecord.appellant().getAt(0));
                            var appellantAddress1, appellantAddress2, appellantCity, appellantState, appellantZip, appellantCountry, addressModBy, addressModDate, addressNotes, workPhone, homePhone, birthDate, gender, fullName, ssn, fnodDate, appellantName, appellantRelationship, appellantTitle;
                            //RTC: 158386 - Appellant Info items.items[] = 0. appellantFullName, 1. veteranSsn, 2. birthDate, 3. veteranGender, 4. appellantRelationshipToVeteranDescription, 5. appellantTitle, 6. veteranFullName, 7. finalNoticeOfDeathDate_f, 8. appellantAddressLine1, 9. appellantAddressLine2, 10. appellantAddressCityName, 11. appellantAddressStateCode, 12. appellantAddressZipCode, 13. appellantAddressCountryName, 14. appellantAddressLastModifiedByROName, 15. appellantAddressLastModifiedDate_f, 16. appellantAddressNotes, 17. appellantWorkPhone, 18. appellantHomePhone
                            if (appealDetailRecord.appellantAddress().getAt(0)) {
                                appellantAddress1 = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressLine1'),
                                appellantAddress2 = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressLine2'),
                                appellantCity = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressCityName'),
                                appellantState = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressStateCode'),
                                appellantZip = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressZipCode'),
                                appellantCountry = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressCountryName'),
                                addressModBy = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressLastModifiedByROName'),
                                addressModDate = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressLastModifiedDate_f'),
                                addressNotes = appealDetailRecord.appellantAddress().getAt(0).get('appellantAddressNotes'),
                                workPhone = appealDetailRecord.appellantAddress().getAt(0).get('appellantWorkPhone'),
                                homePhone = appealDetailRecord.appellantAddress().getAt(0).get('appellantHomePhone');
                            }
                            //Start ApellantVet Info
                            if (appealDetailRecord.veteran().getAt(0)) {
                                birthDate = appealDetailRecord.veteran().getAt(0).get('birthDate'),
                                gender = appealDetailRecord.veteran().getAt(0).get('veteranGender'),
                                fullName = appealDetailRecord.veteran().getAt(0).get('veteranFullName'),
                                ssn = appealDetailRecord.veteran().getAt(0).get('veteranSsn'),
                                fnodDate = appealDetailRecord.veteran().getAt(0).get('finalNoticeOfDeathDate_f');
                            }
                            //Start Appellant Info
                            if (appealDetailRecord.appellant().getAt(0)) {
                                appellantName = appealDetailRecord.appellant().getAt(0).get('appellantFullName'),
                                appellantRelationship = appealDetailRecord.appellant().getAt(0).get('appellantRelationshipToVeteranDescription'),
                                appellantTitle = appealDetailRecord.appellant().getAt(0).get('appellantTitle');
                            }

                            me.getAppellant().items.items[0].setValue(appellantName);
                            me.getAppellant().items.items[1].setValue(ssn);
                            me.getAppellant().items.items[2].setValue(birthDate);
                            me.getAppellant().items.items[3].setValue(gender);
                            me.getAppellant().items.items[4].setValue(appellantRelationship);
                            me.getAppellant().items.items[5].setValue(appellantTitle);
                            me.getAppellant().items.items[6].setValue(fullName);
                            me.getAppellant().items.items[7].setValue(fnodDate);
                            me.getAppellant().items.items[8].setValue(appellantAddress1);
                            me.getAppellant().items.items[9].setValue(appellantAddress2);
                            me.getAppellant().items.items[10].setValue(appellantCity);
                            me.getAppellant().items.items[11].setValue(appellantState);
                            me.getAppellant().items.items[12].setValue(appellantZip);
                            me.getAppellant().items.items[13].setValue(appellantCountry);
                            me.getAppellant().items.items[14].setValue(addressModBy);
                            me.getAppellant().items.items[15].setValue(addressModDate);
                            me.getAppellant().items.items[16].setValue(addressNotes);
                            me.getAppellant().items.items[17].setValue(workPhone);
                            me.getAppellant().items.items[18].setValue(homePhone);
                        }
                        if (appealDetailRecord.hearingRequest().getAt(0)) {
                            me.getHearingRequest().loadRecord(appealDetailRecord.hearingRequest().getAt(0));
                            var hearingActionDesc = appealDetailRecord.appealDecision().getAt(0).get('hearingActionDescription');
                            me.getHearingRequest().items.items[4].setValue(hearingActionDesc);
                        }
                    }
                    // clear remand reasons
                    var rrGrid = me.getRemandReasons(), rrStore = rrGrid.getStore();
                    if (rrStore.getCount() > 0) {
                        rrStore.removeAll();
                    }
                }

                me.application.fireEvent('webservicecallcomplete', operation, 'appeals.Detail');
            },
            scope: me
        });
    },

    showPaymentScript: function () {
        if (!parent || !parent._KMRoot) {
            Ext.Msg.alert(
				'Appeals',
				'This feature works only in context of CRM screen hosting the UI'
			);
            return;
        }

        var me = this;

        _claimFolderLocation = null, _corpData = null, _selectedRecForPayScript = null;
        var isLoaded = false, _selectedAppeal = null;
        if (!Ext.isEmpty(me.claimFolderLocation)) { _claimFolderLocation = me.claimFolderLocation; }
        if (!Ext.isEmpty(me.corpData)) { _corpData = me.corpData; }

        try {
            if (me.selectedAppeal != undefined && me.selectedAppeal /*&& !Ext.isEmpty(me.selectedAppeal.appealDetailRecord)*/) {
                _selectedAppeal = me.selectedAppeal;

                if (_corpData) { _corpData.appeal = me.selectedAppeal; }
                isLoaded = true;
            }
        }
        catch (ae) { }
        try { if (_corpData) { _corpData.poa = me.POAData; } } catch (pe) { }

        if (!isLoaded) {
            if (confirm('Appeal Record is not selected or details for selected appeal record has not loaded yet.\n\nPress OK to stop loading script to allow appeal record to fully load (try again in a few moments).\nPress CANCEL to continue loading script even though full appeal details are not yet available.')) {
                return false;
            }
        }

        parent._SetPrimaryTypeSubtype('APPEAL_GENERAL', true, false);

        var scriptSource = parent._KMRoot + 'appealSmart.html';
        window.open(scriptSource, "CallScript", "width=1024,height=800,scrollbars=1,resizable=1");
    },

    reloadAllData: function () {
        var me = this,
            selectedPerson = me.application.personInquiryModel;

        if (!Ext.isEmpty(selectedPerson)) {
            me.onIndividualIdentified(selectedPerson);
        }
        else {
            Ext.Msg.alert(
			'Appeals',
			'Unable to retrieve appeal information'
		);
        }
    },

    startCAD: function () {
        var me = this;

        if (!parent || !parent._ChangeOfAddressOnClick) {
            Ext.Msg.alert(
				'Appeals',
				'This feature works only in context of CRM screen hosting the UI'
			);
            return;
        }

        if (!parent || !parent._ValidateIDProofingForAddressChange) {
            Ext.Msg.alert(
				'Appeals',
				'This feature works only in context of CRM screen hosting the UI'
			);
            return;
        }
        if (!parent._ValidateIDProofingForAddressChange()) return;

        var selection = {
            "SSN": me.application.personInquiryModel.get('ssn'),
            "participantId": me.application.personInquiryModel.get('participantId'),
            "ro": false,
            "appealsOnly": true,
            "openedFromClaimTab": false,
            "openedFromAwardsTab": false
        };

        if (selection.SSN) {
            parent._ChangeOfAddressOnClick(selection);
        } else {
            Ext.Msg.alert(
				'Appeals',
				'Unable to obtain SSN of the individual.'
			);
            return;
        }
    }
});