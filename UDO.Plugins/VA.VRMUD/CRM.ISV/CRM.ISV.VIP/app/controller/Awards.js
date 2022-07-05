/**
* @class VIP.controller.Awards
*
*/
Ext.define('VIP.controller.Awards', {
    extend: 'Ext.app.Controller',

    requires: ['VIP.model.Awards'],

    mixins: {
        soj: 'VIP.mixin.StationOfJurisdiction',
        sec: 'VIP.mixin.CRMRoleSecurity'
    },

    stores: ['Awards', 'awards.SingleAward', 'awards.AwardInfo', 'awards.Fiduciary', 'awards.IncomeExpenseInfo', 'awards.Diaries', 'awards.Evrs', 'awards.Receivables', 'awards.ClothingAllowances', 'awards.Deductions', 'awards.Proceeds', 'awards.AwardLines', 'awards.Income', 'awards.Expense'],

    refs: [{
        ref: 'loadRefreshAllAwards',
        selector: 'button[action="reloadAllAwards"]'
    }, {
        ref: 'changeOfAddressButton',
        selector: 'button[action="changeofaddress"]'
    }, {
        ref: 'viewAddressDetailsButton',
        selector: 'button[action="viewaddressdetails"]'
    }, {
        ref: 'viewPayeeButton',
        selector: 'button[action="viewpayee"]'
    }, {
        ref: 'viewMoreFiduciaryDataButton',
        selector: 'button[action="viewmorefiduciarydata"]'
    }, {
        ref: 'awardDetailsTab',
        selector: '[xtype="awards.details"]'
    }, {
        ref: 'awardsGrid',
        selector: '[xtype="awards.benefits"]'
    }, {
        ref: 'awardCounter',
        selector: '[xtype="awards.benefits"] > toolbar > tbtext[notificationType="awardcount"]'
    }, {
        ref: 'receivablesGrid',
        selector: '[xtype="awards.details.receivables"]'
    }, {
        ref: 'clothingAllowancesGrid',
        selector: '[xtype="awards.details.clothingallowances"]'
    }, {
        ref: 'deductionsGrid',
        selector: '[xtype="awards.details.deductions"]'
    }, {
        ref: 'proceedsGrid',
        selector: '[xtype="awards.details.proceeds"]'
    }, {
        ref: 'awardLinesGrid',
        selector: '[xtype="awards.details.awardlines"]'
    }, {
        ref: 'incomeSummaryGrid',
        selector: '[xtype="awards.details.incomesummary"]'
    }, {
        ref: 'incomeGrid',
        selector: '[xtype="awards.details.incomeexpenserecords.income"]'
    }, {
        ref: 'expenseGrid',
        selector: '[xtype="awards.details.incomeexpenserecords.expense"]'
    }, {
        ref: 'diariesGrid',
        selector: '[xtype="awards.details.diaries"]'
    }, {
        ref: 'evrsGrid',
        selector: '[xtype="awards.details.evrs"]'
    }, {
        ref: 'awardDetails',
        selector: '[xtype="awards.details.awarddetails.otherawardinformation"]'
    }, {
        ref: 'payeeInfo',
        selector: '[xtype="awards.details.payeefiduciary.payeeinformation"]'
    }, {
        ref: 'fiduciaryInfo',
        selector: '[xtype="awards.details.payeefiduciary.fiduciaryinformation"]'
    }, {
        ref: 'serviceRequestButton',
        selector: '[xtype="awards.benefits"] > splitbutton[action="startservicerequest"]'
    }],

    init: function () {
        var me = this;

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            claimscaddstarted: me.onChangeOfAddressFromClaims,
            cacheddataloaded: me.onCachedDataLoaded,
            triggerawardstabchange: me.onAwardsDetailsTabChange,
            scope: me
        });

        me.control({
            '[xtype="awards.benefits"] > toolbar > button[action="changeofaddress"]': {
                click: me.changeOfAddress
            },
            '[xtype="awards.benefits"] > toolbar > button[action="viewaddressdetails"]': {
                click: me.viewAddressDetails
            },
            '[xtype="awards.benefits"] > toolbar > button[action="viewpayee"]': {
                click: me.viewPayee
            },
            '[xtype="awards.benefits"] > toolbar > button[action="viewmorefiduciarydata"]': {
                click: me.viewMoreFiduciaryData
            },
            '[xtype="awards.benefits"] > toolbar > button[action="reloadAllAwards"]': {
                click: me.reloadAllAwardsData
            },
            '[xtype="awards.benefits"]': {
                select: me.onBenefitsGridSelection
            },
            '[xtype="awards.details.incomesummary"]': {
                select: me.onIncomeExpenseSummaryGridSelection
            },
            '[xtype="awards.details"]': {
                tabchange: me.onAwardsDetailsTabChange
            },
            '[xtype="awards.benefits"] > toolbar > splitbutton[action="createservicerequest"]': {
                click: me.serviceRequest
            },
            '[xtype="awards.benefits"] > toolbar > splitbutton[action="createservicerequest"] > menu': {
                click: me.serviceRequestMenuClick
            },
            '[xtype="awards.benefits"] > toolbar > button[action="createvai"]': {
                click: me.createVAI
            }
        });

        Ext.log('The Awards controller was successfully initialized.');
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;

        me.getPayeeInfo().getForm().reset();
        me.getAwardDetails().getForm().reset();
        me.getFiduciaryInfo().getForm().reset();

        me.selectedPerson = selectedPerson;

        me.getAwardsStore().load({
            filters: [{
                property: 'fileNumber',
                value: selectedPerson.get('fileNumber')
            }],
            callback: function (records, operation, success) {
                var resultSet = operation.getResultSet();
                if (success && !Ext.isEmpty(records)) {
                    //This will change the columns so that more information shows
                    me.reconfigureAwardsGrid(resultSet.isMultipleResponse);
                    me.updateAwardCount();

                    //If it's a single award response, load all available data and call web services.
                    if (resultSet.isMultipleResponse == false) {
                        var award = records[0];

                        me.getPayeeInfo().loadRecord(award);
                        me.getAwardDetails().loadRecord(award);
                        me.getDiariesGrid().reconfigure(award.diaries());
                        me.getEvrsGrid().reconfigure(award.evrs());

                        me.loadSelectedAwardData(award, 'VIP.model.Awards');
                    }

                    me.application.fireEvent('awardsloaded', records);
                }
                me.application.fireEvent('webservicecallcomplete', operation, 'Awards');
            },
            scope: me
        });
    },

    onBenefitsGridSelection: function (rowModel, award, index) {
        var me = this,
            participantBeneId = award.get('participantBeneId'),
            participantRecipientId = award.get('participantRecipientId'),
            participantVetId = award.get('participantVetId'),
            awardTypeCode = award.get('awardTypeCode'),
            payeeSSN = award.get('payeeSSN'),
            modelName = rowModel.getSelection()[0].modelName;

        //Specific data retrieval for multiple responses
        if (modelName == 'VIP.model.awards.Multiple') {
            me.getPayeeInfo().getForm().reset();
            me.getAwardDetails().getForm().reset();

            me.getFiduciaryInfo().setLoading(true, true);
            me.getPayeeInfo().setLoading(true, true);
            me.getDiariesGrid().setLoading(true, true);
            me.getEvrsGrid().setLoading(true, true);
            me.getAwardsSingleAwardStore().load({
                filters: [{
                    property: 'ptcpntVetId',
                    value: participantVetId
                }, {
                    property: 'ptcpntBeneId',
                    value: participantBeneId
                }, {
                    property: 'ptpcntRecipId',
                    //NOTE SPELLING PTPCNT
                    value: participantRecipientId
                }, {
                    property: 'awardTypeCd',
                    value: awardTypeCode
                }],
                callback: function (records, operation, success) {
                    if (success && !Ext.isEmpty(records)) {
                        me.getPayeeInfo().loadRecord(records[0]);
                        me.getAwardDetails().loadRecord(records[0]);
                        me.getDiariesGrid().reconfigure(records[0].diaries());
                        me.getEvrsGrid().reconfigure(records[0].evrs());

                        payeeSSN = records[0].get('payeeSSN');
                        me.getPayeeInfo().setLoading(false);
                        me.getDiariesGrid().setLoading(false);
                        me.getEvrsGrid().setLoading(false);

                        //calls findFiduciary for selected Payee
                        me.getFiduciaryInfo().getForm().reset();
                        if (!Ext.isEmpty(payeeSSN)) {
                            me.getAwardsFiduciaryStore().load({
                                filters: [{
                                    property: 'fileNumber',
                                    value: payeeSSN
                                }],
                                callback: function (records, operation, success) {
                                    if (success && !Ext.isEmpty(records)) {
                                        me.getFiduciaryInfo().loadRecord(records[0]);
                                    }
                                    me.getFiduciaryInfo().setLoading(false);
                                    me.application.fireEvent('webservicecallcomplete', operation, 'awards.Fiduciary');
                                },
                                scope: me
                            });
                        }
                        else { me.getFiduciaryInfo().setLoading(false); }
                    }
                    me.application.fireEvent('webservicecallcomplete', operation, 'awards.SingleAward');
                },
                scope: me
            });
        }

        me.loadSelectedAwardData(award, modelName);
    },

    loadSelectedAwardData: function (award, modelName) {
        var me = this,
            payeSsn = award.get('payeeSSN'),
            participantVetId = award.get('participantVetId'),
            participantBeneId = award.get('participantBeneId'),
            participantRecipientId = award.get('participantRecipientId'),
            awardTypeCode = award.get('awardTypeCode');

        //Specific events for single award model
        if (modelName == 'VIP.model.Awards') {
            //calls findFiduciary for selected Payee
            if (!Ext.isEmpty(payeSsn)) {
                me.getFiduciaryInfo().setLoading(true, true);
                me.getAwardsFiduciaryStore().load({
                    filters: [{
                        property: 'fileNumber',
                        value: payeSsn
                    }],
                    callback: function (records, operation, success) {
                        if (success && !Ext.isEmpty(records)) {
                            me.getFiduciaryInfo().loadRecord(records[0]);
                        }
                        me.getFiduciaryInfo().setLoading(false);
                        me.application.fireEvent('webservicecallcomplete', operation, 'awards.Fiduciary');
                    },
                    scope: me
                });
            }
        }

        if (!Ext.isEmpty(participantVetId) && !Ext.isEmpty(participantBeneId)
            && !Ext.isEmpty(participantRecipientId) && !Ext.isEmpty(awardTypeCode)) {
            me.getAwardDetails().setLoading(true, true);
            me.getReceivablesGrid().setLoading(true, true);
            me.getClothingAllowancesGrid().setLoading(true, true);
            me.getDeductionsGrid().setLoading(true, true);
            me.getProceedsGrid().setLoading(true, true);
            me.getAwardLinesGrid().setLoading(true, true);
            me.getAwardsAwardInfoStore().load({
                filters: [{
                    property: 'ptcpntVetId',
                    value: participantVetId
                }, {
                    property: 'ptcpntBeneId',
                    value: participantBeneId
                }, {
                    property: 'ptcpntRecipId',
                    //NOTE THE SPELLING OF PTCPNT
                    value: participantRecipientId
                }, {
                    property: 'awardTypeCd',
                    value: awardTypeCode
                }],
                callback: function (records, operation, success) {
                    if (success && !Ext.isEmpty(records)) {
                        me.getAwardDetails().loadRecord(records[0]);
                        me.getReceivablesGrid().reconfigure(records[0].receivables());
                        me.getClothingAllowancesGrid().reconfigure(records[0].clothingallowances());
                        me.getDeductionsGrid().reconfigure(records[0].deductions());
                        me.getProceedsGrid().reconfigure(records[0].proceeds());
                        me.getAwardLinesGrid().reconfigure(records[0].awardlines());

                    }
                    me.getAwardDetails().setLoading(false);
                    me.getReceivablesGrid().setLoading(false);
                    me.getClothingAllowancesGrid().setLoading(false);
                    me.getDeductionsGrid().setLoading(false);
                    me.getProceedsGrid().setLoading(false);
                    me.getAwardLinesGrid().setLoading(false);
                    me.application.fireEvent('webservicecallcomplete', operation, 'awards.AwardInfo');
                    me.onAwardsDetailsTabChange();
                },
                scope: me
            });
        }

        if (!Ext.isEmpty(participantVetId) && !Ext.isEmpty(participantBeneId)) {
            me.getIncomeSummaryGrid().setLoading(true, true);
            me.getAwardsIncomeExpenseInfoStore().load({
                filters: [{
                    property: 'ptcpntVetId',
                    value: participantVetId
                }, {
                    property: 'ptcpntBeneId',
                    value: participantBeneId
                }],
                callback: function (records, operation, success) {
                    me.getIncomeSummaryGrid().setLoading(false);
                    me.application.fireEvent('webservicecallcomplete', operation, 'awards.IncomeExpenseInfo');
                },
                scope: me
            });
        }
    },

    reconfigureAwardsGrid: function (isMultipleResponse) {
        var me = this,
            cols;

        if (isMultipleResponse == false) {
            cols = [
                { header: 'Benefit Type Code', dataIndex: 'benefitTypeCode', width: 125 },
                { header: 'Benefit Type Name', dataIndex: 'benefitTypeName', width: 160 },
                { header: 'Award Type Code', dataIndex: 'awardTypeCode', width: 125 },
                { header: 'Payee Name', dataIndex: 'payeeName', width: 125 },
                { header: 'Payee Type Code', dataIndex: 'payeeTypeCode', width: 125 },
                { header: 'Payee Type Name', dataIndex: 'payeeTypeName', width: 125 },
                { header: 'Vet First Name', dataIndex: 'vetFirstName', width: 125 },
                { header: 'Vet Last Name', dataIndex: 'vetLastName', width: 125 },
                { header: 'Status Reason Date', dataIndex: 'statusReasonDate', width: 125, xtype: 'datecolumn', format: 'm/d/Y' }
            ];
            me.getAwardsGrid().reconfigure(null, cols);
        } else if (isMultipleResponse == true) {
            cols = [
                { header: 'Benefit Code', dataIndex: 'awardBeneTypeCode', width: 100 },
                { header: 'Benefit Type Name', dataIndex: 'awardBeneTypeName', width: 160 },
                { header: 'Award Code', dataIndex: 'awardTypeCode', width: 80 },
                { header: 'Award Type Name', dataIndex: 'awardTypeName', width: 180 },
                { header: 'Beneficiary', dataIndex: 'beneficiaryName', width: 125 },
                { header: 'Payee Code', dataIndex: 'payeeCode', width: 80 },
                { header: 'Recipient', dataIndex: 'recipientName', width: 160 },
                { header: 'Veteran', dataIndex: 'vetName', width: 160 }
            ];
            me.getAwardsGrid().reconfigure(null, cols);
        }
    },

    onIncomeExpenseSummaryGridSelection: function (rowModel, incomeExpenseSummary, index) {
        var me = this;
        //Load data into expenses and incomes grid.
        me.getExpenseGrid().reconfigure(incomeExpenseSummary.expenses());
        me.getIncomeGrid().reconfigure(incomeExpenseSummary.incomes());
    },

    updateAwardCount: function () {
        var me = this,
            awardStore = me.getAwardsGrid().getStore();

        me.getAwardCounter().setText('Awards: ' + awardStore.getCount());
    },

    reloadAllAwardsData: function () {
        var me = this;
        //me.getAwardsStore().load();
        if (!Ext.isEmpty(me.selectedPerson)) {
            me.onIndividualIdentified(me.selectedPerson);
        }
    },

    changeOfAddress: function () {
        var me = this;
        me.StartChangeOfAddress(false);
    },

    viewAddressDetails: function () {
        var me = this;
        me.StartChangeOfAddress(true);
    },

    onChangeOfAddressFromClaims: function (claimPayeeTypeCode) {
        var me = this;
        //var awardCount = me.getAwardsStore().getCount();
        var awardCount = 0;

        //this accounts for a blank line that should not count as one award
        //if (awardCount == 1) {
        //    if (Ext.isEmpty(me.getAwardsStore().data.items[0].get('awardTypeCode'))) awardCount = 0;
        //}

        for (var i = 0; i < me.getAwardsStore().data.items.length; i++) {
            var payeeCode = me.getAwardsStore().data.items[i].get('payeeCode');
            if (Ext.isEmpty(payeeCode)) {
                payeeCode = me.getAwardsStore().data.items[i].get('payeeTypeCode');
            }

            if (payeeCode == claimPayeeTypeCode) {
                awardCount += 1;
            }
        }

        if (!parent || !parent._ValidateIDProofingForAddressChange) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return;
        }
        if (!parent._ValidateIDProofingForAddressChange()) return;

        me.application.fireEvent('awardcountforcadd', awardCount);
    },

    GetSelectedAwardRecord: function (promptIfNotSelected) {
        var me = this;
        var awardsGrid = me.getAwardsGrid();
        var singleFlag = false;

        var selectedRecords = awardsGrid.getSelectionModel().getSelection();
        if (!selectedRecords || selectedRecords.length == 0) {
            //Check if there is only one record, then select base
            if ((awardsGrid) && (awardsGrid.getStore()) && (awardsGrid.getStore().count() == 1)) {
                selectedRecords = awardsGrid.getStore().getAt(0);
                singleFlag = true;
            }
        }
        if (!selectedRecords || selectedRecords.length == 0) {
            if (promptIfNotSelected) {
                Ext.Msg.alert('No Award Selected', 'Please select an award from the grid first');
            }
            return null;
        }
        if (singleFlag == false) {
            selectedRecords = selectedRecords[0];
        }
        return selectedRecords;
    },

    StartChangeOfAddress: function (ro) {
        var me = this;
        var rec = me.GetSelectedAwardRecord(true);
        if (!rec) {
            return;
        }
        var selection = null;
        var hasActiveFid = false;
        var endDate, now;

        if (!parent || !parent._ChangeOfAddressOnClick) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return;
        }
        // Check if fiduciary exists, directly in a store
        // By default hasActiveFid=false
        // If endDate empty - no fiduciary
        if (!ro) {
            var fidStore = me.getAwardsFiduciaryStore();
            if (!Ext.isEmpty(fidStore) && !Ext.isEmpty(fidStore.data) && fidStore.data.getCount() > 0) {
                if (!Ext.isEmpty(fidStore.data.items[0].get('endDate'))) {
                    now = new Date();
                    endDate = new Date(fidStore.data.items[0].get('endDate'));
                    endDate >= now ? hasActiveFid = true : hasActiveFid = false;
                }
            }

            if (!hasActiveFid) {
                var payeeInfoRecord = me.getPayeeInfo().getRecord();
                if (!Ext.isEmpty(payeeInfoRecord) && !Ext.isEmpty(payeeInfoRecord.get('fiduciaryDecisionTypeName'))
                    && payeeInfoRecord.get('fiduciaryDecisionTypeName').toLocaleLowerCase() === 'fiduciary') {
                    hasActiveFid = true;
                }
            }
        }

        if (!ro && typeof parent._ValidateIDProofingForAddressChange == 'function') {
            if (!parent._ValidateIDProofingForAddressChange()) return;
        }
        selection = {
            "awardTypeCd": (rec.get('PK_awardTypeCode') ? rec.get('PK_awardTypeCode') : rec.get('awardTypeCode')),
            "ptcpntVetID": (rec.get('PK_participantVetId') ? rec.get('PK_participantVetId') : rec.get('participantVetId')),
            "ptcpntBeneID": (rec.get('PK_participantBeneId') ? rec.get('PK_participantBeneId') : rec.get('participantBeneId')),
            "ptcpntRecipID": (rec.get('PK_participantRecipientId') ? rec.get('PK_participantRecipientId') : rec.get('participantRecipientId')),
            "ro": ro,
            "hasFid": hasActiveFid,
            "appealsOnly": false,
            "openedFromClaimTab": false,
            "openedFromAwardsTab": true
        };
        parent._ChangeOfAddressOnClick(selection);

    },

    viewPayee: function () {
        var me = this,
            rec = me.GetSelectedAwardRecord(true);
        if (!rec) {
            return;
        }

        if (!parent || !parent._ViewContact) {
            alert('This feature works only in the context of CRM screen hosting the UI');
            return;
        }

        // TODO: how to get PayeeSSNId = genInfoData.payeeSSN            
        var selection = {
            "ptcpntId": rec.get('participantRecipientId'),
            "SSN": null //Ext.ComponentManager.get('PayeeSSNId').value
        };
        parent._ViewContact(selection);
    },

    viewMoreFiduciaryData: function () {
        var me = this;
        var award = me.GetSelectedAwardRecord(true);
        if (!award) {
            return;
        }

        var hasFid = false;
        var fidStore = me.getAwardsFiduciaryStore();
        if (!Ext.isEmpty(fidStore) && !Ext.isEmpty(fidStore.data) && fidStore.data.getCount() > 0) {
            hasFid = true;
        }

        if (hasFid) {
            try {
                var ptcpntId = fidStore.data.items[0].get('personOrgParticipantID');

                if (!Ext.isEmpty(ptcpntId)) {
                    me.application.fireEvent('viewmorefiduciarydata', ptcpntId);
                } else {
                    Ext.Msg.alert('No Participant ID', 'The selected award does not have a participant ID. No additional data can be loaded.');
                    return;
                }
            } catch (e) {
                Ext.Msg.alert('Error', 'An error has occurred. Additional data cannot be retrieved.');
            }
        } else {
            Ext.Msg.alert('No Fiduciary', 'The selected award line does not have a fiduciary, or all fiduciary info has not been loaded. No additional data can be displayed.');
        }
    },

    onAwardsDetailsTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
            activeTab = me.getAwardDetailsTab().getActiveTab(),
            tabTitle = activeTab.title,
            gridCount = null;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            //if (Ext.get('id_awards_Benefits_01')) Ext.get('id_awards_Benefits_01').hide();
            if (Ext.get('id_awards_Benefits_02')) Ext.get('id_awards_Benefits_02').hide();
            if (Ext.get('id_awards_Benefits_03')) Ext.get('id_awards_Benefits_03').hide();
            if (Ext.get('id_awards_Benefits_04')) Ext.get('id_awards_Benefits_04').hide();
            if (Ext.get('id_awards_Benefits_05')) Ext.get('id_awards_Benefits_05').hide();
            if (Ext.get('id_awards_Benefits_06')) Ext.get('id_awards_Benefits_06').hide();
            if (Ext.get('id_awards_Benefits_07')) Ext.get('id_awards_Benefits_07').hide();
            if (Ext.get('id_awards_Benefits_08')) Ext.get('id_awards_Benefits_08').hide();
            if (Ext.get('id_awards_Benefits_09')) Ext.get('id_awards_Benefits_09').hide();
            if (Ext.get('id_awards_Benefits_10')) Ext.get('id_awards_Benefits_10').hide();
            if (Ext.get('id_awards_Benefits_11')) Ext.get('id_awards_Benefits_11').hide();
            if (Ext.get('id_awards_Benefits_12')) Ext.get('id_awards_Benefits_12').hide();
            if (Ext.get('id_awards_Benefits_13')) Ext.get('id_awards_Benefits_13').hide();
            if (Ext.get('id_awards_Benefits_14')) Ext.get('id_awards_Benefits_14').hide();
            if (Ext.get('id_awards_Benefits_15')) Ext.get('id_awards_Benefits_15').hide();
        }

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        } else if (activeTab.getXType() == 'awards.details.incomeexpenserecords') {
            tabTitle = 'Income Records: ' + me.getIncomeGrid().getStore().getCount() +
                       ', Expense Records: ' + me.getExpenseGrid().getStore().getCount();
        } else {
            tabTitle = null;
        }
        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },


    /**
    * This function fires off when the service request button is clicked in the Award toolbar without selecting an option from the dropdown
    * Default is the 0820 option
    */
    serviceRequest: function (button) {
        var me = this,
			defaultSelection = button.defaultMenuSelection,
            selectedAward = me.GetSelectedAwardRecord(true);

        if (selectedAward == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else if (!Ext.isEmpty(defaultSelection)) {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create an Award service request', function (button) {
                if (button == 'no') return;
                else me.fireServiceRequestEvent({ name: defaultSelection.text, value: parseInt(defaultSelection.value) }, selectedAward);
            });
        }
    },

    /**
    * This function fires off when a service request menu button from the dropdown is clicked in the Award toolbar.
    */
    serviceRequestMenuClick: function (menu, item, e, eOpts) {
        var me = this,
            selectedAward = me.GetSelectedAwardRecord(true);

        if (selectedAward == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else if (!Ext.isEmpty(item)) {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create an Award service request', function (button) {
                if (button == 'no') return;
                else me.fireServiceRequestEvent({ name: item.text, value: parseInt(item.value) }, selectedAward);
            });
        }
    },

    /**
    * This function is called from the service request menu click.  It will gather the information from the selected award and 
    * and detail panel and fire off the service request event in the Service Request controller.
    */
    fireServiceRequestEvent: function (serviceRequestType, selectedAward) {
        var me = this,
            awardDetailsPanel = me.getAwardDetails(),
            awardLinesStore = me.getAwardLinesGrid().getStore(),
            awardType = null,
            awardLineModel = null,
            awardInfo = null,
            stationOfJurisdiction = me.getAwardDetails().items.get("stationOfJurisdiction").value;

        if (selectedAward.get("benefitTypeName")) {
            awardType = selectedAward.get("benefitTypeName");
        } else {
            awardType = selectedAward.get("awardTypeName");
        }

        //Add Global Vars for params of running dep stores 2/23/13
        if (selectedAward.get('payeeCode')) {   //MULTIPLE AWARDS
            me.application.serviceRequest.va_SelectedPayeeCode = selectedAward.get('payeeCode');
        }
        else if (selectedAward.get('payeeTypeCode')) {  //SINGLE AWARD 
            me.application.serviceRequest.va_SelectedPayeeCode = selectedAward.get('payeeTypeCode');
        }
        me.application.serviceRequest.va_ServiceRequestType = 'Award';
        me.application.serviceRequest.va_SelectedSSN = me.getPayeeInfo().items.items[1].getValue('payeeSSN');
        me.application.serviceRequest.va_SelectedPID = (selectedAward.get('PK_participantRecipientId') ? selectedAward.get('PK_participantRecipientId') : selectedAward.get('participantRecipientId'));
        if (me.application.serviceRequest.va_SelectedPID == null || '') {
            me.application.serviceRequest.va_SelectedPID = (selectedAward.get('PK_participantBeneId') ? selectedAward.get('PK_participantBeneId') : selectedAward.get('participantBeneId'));
        }

        //Data array to pass to the SR controller
        awardInfo = {
            va_AwardBenefitType: awardType,
            va_CurrentMonthlyRate: {
                Value: null
            },
            va_NetAmountPaid: {
                Value: null
            },
            va_EffectiveDate: null
        };

        //Determine the correct Award Line = effectiveDate is not greater than today        
        if (awardLinesStore) {  //If Award Lines Store exists
            awardLinesStore.sort("effectiveDate", "DESC");
            awardLinesStore.each(function (record) {
                if (record.get("effectiveDate") <= new Date) {
                    awardLineModel = record;
                    return false; //stops iteration of the 'each' function
                }
                return true;
            });

            //add latest award line data if there is any
            if (awardLineModel) {
                awardInfo.va_EffectiveDate = Ext.Date.format(awardLineModel.get("effectiveDate"), "m/d/Y");
                awardInfo.va_CurrentMonthlyRate.Value = me.formatCurrencyToString(awardLineModel.get("totalAward"));
                awardInfo.va_NetAmountPaid.Value = me.formatCurrencyToString(awardLineModel.get("netAward"));
                awardInfo.va_DependentAmount = {
                    Value: me.formatCurrencyToString(awardLineModel.get("spouse"))
                };
                awardInfo.va_AAAmount = {
                    Value: me.formatCurrencyToString(awardLineModel.get("aaHbInd"))
                };
                awardInfo.va_PensionBenefitAmount = {
                    Value: me.formatCurrencyToString(awardLineModel.get("altmnt"))
                };

            }
        }
        if (!Ext.isEmpty(stationOfJurisdiction)) {
            awardInfo.va_RegionalOfficeId = me.getStationOfJurisdiction(stationOfJurisdiction);
        }

        //This event is in the service request controller
        me.application.fireEvent('createawardservicerequest', serviceRequestType, awardInfo);

        Ext.log('A startservicerequest event was fired by the PersonInfo controller');
    },

    /**
    * This function fires off when the VAI button is clicked in the Award toolbar   
    */
    createVAI: function (button) {
        var me = this,
            selectedAward = me.GetSelectedAwardRecord(true);

        if (selectedAward == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create an Award VAI', function (button) {
                if (button == 'no') return;

                else me.fireVaiEvent(selectedAward);
            });
        }
    },

    fireVaiEvent: function (selectedAward) {
        var me = this,
            stationOfJurisdiction = me.getAwardDetails().items.get("stationOfJurisdiction").value,
            selectionVariables = [];
        /*selectionVariables.Location
          selectionVariables.PID
          selectionVariables.SSN
          selectionVariables.PayeeCode*/


        //Add Global Vars for VAI params
        if (selectedAward.get('payeeCode')) {   //MULTIPLE AWARDS
            selectionVariables.PayeeCode = selectedAward.get('payeeCode');
        }
        else if (selectedAward.get('payeeTypeCode')) {  //SINGLE AWARD 
            selectionVariables.PayeeCode = selectedAward.get('payeeTypeCode');
        }
        selectionVariables.Location = 'AWARD';
        selectionVariables.SSN = me.getPayeeInfo().items.items[1].getValue('payeeSSN');
        selectionVariables.PID = (selectedAward.get('PK_participantRecipientId') ? selectedAward.get('PK_participantRecipientId') : selectedAward.get('participantRecipientId'));
        if (selectionVariables.PID == null || selectionVariables.PID == '') {
            selectionVariables.PID = (selectedAward.get('PK_participantBeneId') ? selectedAward.get('PK_participantBeneId') : selectedAward.get('participantBeneId'));
        }

        if (!Ext.isEmpty(stationOfJurisdiction) && stationOfJurisdiction.length > 2) {
            var sojCode = stationOfJurisdiction.substring(0, 3);

            me.getStationOfJurisdictionAsync(sojCode, function (soj) {
                if (soj) {
                    if (!soj.isPilot) {
                        alert(me.sojIsNotPilotMessage());
                        return;
                    }

                    selectedAward.data.va_RegionalOfficeId = soj.entityReference;
                }

                me.application.fireEvent('createcrmvai', selectionVariables, selectedAward);
                Ext.log('A Create VAI event was fired by the Award controller');
            });
        } else {
            me.application.fireEvent('createcrmvai', selectionVariables, selectedAward);
            Ext.log('A Create VAI event was fired by the Award controller');
        }
    },

    /**
    * This function returns a string that will be accepted by the CrmRestkit
    */
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

    onCachedDataLoaded: function () {
        var me = this,
            awardData, otherAwardData, awardFiduciaryData;

        if (me.getAwardsStore().getCount() == 1) {
            awardData = me.getAwardsStore().getAt(0);
            otherAwardData = me.getAwardsAwardInfoStore().getAt(0);
            awardFiduciaryData = me.getAwardsFiduciaryStore().getAt(0);

            //This changes the columns of the top grid to fit the single award model.
            me.reconfigureAwardsGrid(false);

            //Need to check if data was read from model - if WS fails we will get errors when loading data.
            if (!Ext.isEmpty(awardData)) {
                me.getPayeeInfo().loadRecord(awardData);
                me.getAwardDetails().loadRecord(awardData);
                me.getDiariesGrid().reconfigure(awardData.diaries());
                me.getEvrsGrid().reconfigure(awardData.evrs());
            }
            if (!Ext.isEmpty(awardFiduciaryData)) {
                me.getFiduciaryInfo().loadRecord(awardFiduciaryData);
            }
            if (!Ext.isEmpty(otherAwardData)) {
                me.getAwardDetails().loadRecord(otherAwardData);
                me.getReceivablesGrid().reconfigure(otherAwardData.receivables());
                me.getClothingAllowancesGrid().reconfigure(otherAwardData.clothingallowances());
                me.getDeductionsGrid().reconfigure(otherAwardData.deductions());
                me.getProceedsGrid().reconfigure(otherAwardData.proceeds());
                me.getAwardLinesGrid().reconfigure(otherAwardData.awardlines());
            }
            //No need to add the Income Summary grid here, it is linked to the store so when it is loaded it will populate automatically.
        } else {
            me.reconfigureAwardsGrid(true);
        }

        me.application.fireEvent('awardsloaded', me.getAwardsStore().data.items);

    }


});