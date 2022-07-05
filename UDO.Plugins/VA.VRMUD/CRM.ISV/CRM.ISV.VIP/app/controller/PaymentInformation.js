/**
* @author Ivan Yurisevic
* @class VIP.controller.PaymentInformation
*
* The controller for the PaymentInformation tab
*/
Ext.define('VIP.controller.PaymentInformation', {
    extend: 'Ext.app.Controller',
    stores: [
		'Payment',
		'paymentinformation.PaymentDetail',
		'paymentinformation.PaymentAdjustmentVo',
		'paymentinformation.AwardAdjustmentVo',
		'paymentinformation.AwardReason',
		'paymentinformation.Comerica'
    ],
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    refs: [{
        ref: 'PaymentInformationGrid',
        selector: '[xtype="paymentinformation.payments"]'
    }, {
        ref: 'PaymentInformationCounter',
        selector: '[xtype="paymentinformation.payments"] > toolbar > tbtext[notificationType="paymentinformationcount"]'
    }, {
        ref: 'PaymentInformationTabs',
        selector: '[xtype="paymentinformation.details"]'
    }, {
        ref: 'paymentDetails',
        selector: '[xtype="paymentinformation.details.paymentdetails"]'
    }, {
        ref: 'addressField',
        selector: '[xtype="paymentinformation.details.paymentdetails"] > displayfield[action="address"]'
    }, {
        ref: 'awardAdjustments',
        selector: '[xtype="paymentinformation.details.awardadjustments"]'
    }, {
        ref: 'awardReasons',
        selector: '[xtype="paymentinformation.details.awardreasons"]'
    }, {
        ref: 'paymentAdjustments',
        selector: '[xtype="paymentinformation.details.paymentadjustments"]'
    }, {
        ref: 'payeeCodeDropDown',
        selector: '[xtype="paymentinformation.payments"] > toolbar > splitbutton[action="filterpaymentsbypayeecode"]'
    }],

    selectedPerson: null,
    awardsRecipients: null,

    init: function () {
        var me = this;

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            updateclaimprocessingtimelabel: me.onUpdatedClaimProcessingTimeLabel,
            gotcorp: me.onGotCorpData,
            claimsloaded: me.onGotClaims,
            triggerpaymentinformationtabchange: me.onPaymentInformationTabChange,
            crmscriptinvoked: me.onCrmSriptInvoked,
            awardsloaded: me.onAwardsLaoded,
            personloadedfromcache: me.onIndividualIdentified,
            scope: me
        });

        me.control({
            '[xtype="paymentinformation.payments"] > toolbar > paymentdisposition': {
                click: me.setPaymentDisposition
            },
            '[xtype="paymentinformation.payments"] > toolbar > paymentdisposition > menu': {
                click: me.setPaymentDispositionMenuClick
            },
            '[xtype="paymentinformation.payments"] > toolbar > button[action="showeducationcutoff"]': {
                click: me.showEducationCutOff
            },
            '[xtype="paymentinformation.payments"] > toolbar > button[action="showpaymentscript"]': {
                click: me.showPaymentScript
            },
            '[xtype="paymentinformation.payments"] > toolbar > button[action="showcandpscript"]': {
                click: me.showCandPScript
            },
            '[xtype="paymentinformation.payments"]': {
                selectionchange: me.onPaymentInformationGridSelection
            },
            '[xtype="paymentinformation.details"]': {
                tabchange: me.onPaymentInformationTabChange
            },
            '[xtype="paymentinformation.payments"] > toolbar > splitbutton[action="filterpaymentsbypayeecode"] > menu': {
                click: me.filterPaymentsByPayee
            }
        });
    },

    onCrmSriptInvoked: function (callType) {
        var me = this;

        if (callType != 'Payments / Debts') return;

        me.showPaymentScript();
    },

    onGotCorpData: function (corpData) {
        var me = this;
        me.corpData = corpData;
    },

    onGotClaims: function (claims) {
        var me = this;
        me.claims = claims;
    },

    // receive event from BIRLS tab and show claims folder SOJ
    onUpdatedClaimProcessingTimeLabel: function (claimFolderLocation) {
        var me = this;
        me.claimFolderLocation = claimFolderLocation;
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        me.selectedPerson = selectedPerson;
        me.selectedPerson.set('payeeCode', '00');
        me.getPayeeCodeDropDown().setText('Payee: 00');

        me.getPaymentinformationComericaStore().load({
            filters: [],
            callback: function (records, operation, success) {
                var aaa = operation.response.responseText;
                var routingNumber = aaa.substr(aaa.indexOf('&lt;return&gt;') + 14, aaa.indexOf('&lt;/return&gt;') - (aaa.indexOf('&lt;return&gt;') + 14));
                parent.Xrm.Page.getAttribute('va_crn').setValue(routingNumber);

                if (Ext.isEmpty(routingNumber)) {
                    var aaa = operation.response.responseText;
                    var routingNumber = aaa.substr(aaa.indexOf('<return>') + 8, aaa.indexOf('</return>') - (aaa.indexOf('<return>') + 8));
                    parent.Xrm.Page.getAttribute('va_crn').setValue(routingNumber);
                }

                me.loadPaymentStore();
            },
            scope: me
        });
    },

    updatePaymentInformationCount: function () {
        var me = this,
			paymentinformationStore = me.getPaymentInformationGrid().getStore();

        me.getPaymentInformationCounter().setText('Payments: ' + paymentinformationStore.getCount());
    },

    onPaymentInformationTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
			activeTab = me.getPaymentInformationTabs().getActiveTab(),
			tabTitle = activeTab.title,
			gridCount = null;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            //if (Ext.get('id_paymentinformation_Payments_01')) Ext.get('id_paymentinformation_Payments_01').hide();
            if (Ext.get('id_paymentinformation_Payments_02')) Ext.get('id_paymentinformation_Payments_02').hide();
            if (Ext.get('id_paymentinformation_Payments_03')) Ext.get('id_paymentinformation_Payments_03').hide();
            if (Ext.get('id_paymentinformation_Payments_04')) Ext.get('id_paymentinformation_Payments_04').hide();
            if (Ext.get('id_paymentinformation_Payments_05')) Ext.get('id_paymentinformation_Payments_05').hide();
            if (Ext.get('id_paymentinformation_Payments_06')) Ext.get('id_paymentinformation_Payments_06').hide();
            if (Ext.get('id_paymentinformation_Payments_07')) Ext.get('id_paymentinformation_Payments_07').hide();
            //if (Ext.get('id_paymentinformation_Payments_08')) Ext.get('id_paymentinformation_Payments_08').hide();
            //if (Ext.get('id_paymentinformation_Payments_09')) Ext.get('id_paymentinformation_Payments_09').hide();
            if (Ext.get('id_paymentinformation_Payments_10')) Ext.get('id_paymentinformation_Payments_10').hide();
            if (Ext.get('id_paymentinformation_Payments_11')) Ext.get('id_paymentinformation_Payments_11').hide();

            $("#id_paymentinformation_Payments_08").css('left', 140);
            $("#id_paymentinformation_Payments_09").css('left', 145);
        }

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        } else {
            tabTitle = null;
        }
        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    showCandPScript: function () {
        window.open('https://vaww.vrm.km.va.gov/system/templates/selfservice/va_ka/portal.html?portalid=554400000001001&articleid=554400000002433', "CPScript", "width=600,height=700,scrollbars=1,resizable=1");
    },

    setPaymentDisposition: function (button) {
        Ext.Msg.alert(
			'PaymentInformation',
			'Please select a payment disposition for the PaymentInformation tab from the drop down.'
		);
    },

    setPaymentDispositionMenuClick: function (menu, item, e, eOpts) {
        if (!parent || !parent._SetPrimaryTypeSubtype) {
            Ext.Msg.alert(
				'PaymentInformation',
				'This feature works only in context of CRM screen hosting the UI'
			);
            return;
        }
        parent._SetPrimaryTypeSubtype('PAYMENT_ANY', false, true, item.value);
    },

    showEducationCutOff: function () {
        window.open("https://vaww.vrm.km.va.gov/system/templates/selfservice/va_ka/portal.html?portalid=554400000001001&articleid=554400000003847");
    },

    showPaymentScript: function () {
        if (!parent || !parent._KMRoot) {
            Ext.Msg.alert(
				'PaymentInformation',
				'This feature works only in context of CRM screen hosting the UI'
			);
            return;
        }
        var me = this;

        _claimFolderLocation = null, _corpData = null, _selectedRecForPayScript = null;
        try {
            if (_selectedFinRecord != undefined && !Ext.isEmpty(_selectedFinRecord)) { _selectedRecForPayScript = _selectedFinRecord; }
        } catch (er) { }
        if (!Ext.isEmpty(me.claimFolderLocation)) { _claimFolderLocation = me.claimFolderLocation; }
        if (!Ext.isEmpty(me.corpData)) { _corpData = me.corpData; }
        if (!Ext.isEmpty(me.claims)) { _claims = me.claims; }

        if (!Ext.isEmpty(_selectedRecForPayScript)) {
            _selectedRecForPayScript.payAdjust = null;
            var store = me.getPaymentinformationPaymentAdjustmentVoStore();
            if (!Ext.isEmpty(store)) { _selectedRecForPayScript.payAdjust = store.data; }

            _selectedRecForPayScript.awardAdjust = null;
            store = me.getPaymentinformationAwardAdjustmentVoStore();
            if (!Ext.isEmpty(store)) { _selectedRecForPayScript.awardAdjust = store.data; }
        }

        parent._SetPrimaryTypeSubtype('PAYMENT_GENERAL', true, false);
        var scriptSource = parent._KMRoot + 'PaymentSmart.html';
        window.open(scriptSource, "CallScript", "width=1024,height=800,scrollbars=1,resizable=1");
    },

    onPaymentInformationGridSelection: function (selectionModel, selection, options) {
        var me = this;
        _selectedFinRecord = null;

        if (Ext.isEmpty(selection[0])) return;

        if (!Ext.isEmpty(selection[0].data)) { _selectedFinRecord = selection[0].data; }

        var keyValue = selection[0].get('paymentId'),
            keyName = 'PaymentId';

        if (Ext.isEmpty(keyValue)) {
            keyValue = selection[0].get('transactionId');
            keyName = 'FbtId';
        }

        if (!Ext.isEmpty(keyValue)) {
            me.getPaymentDetails().setLoading(true, true);
            me.getAwardAdjustments().setLoading(true, true);
            me.getAwardReasons().setLoading(true, true);
            me.getPaymentAdjustments().setLoading(true, true);

            me.getPaymentinformationPaymentDetailStore().load({
                filters: [{
                    property: keyName,
                    value: keyValue
                }],
                callback: function (records, operation, success) {
                    if (success && !Ext.isEmpty(records)) {
                        me.getPaymentDetails().loadRecord(records[0]);
                        me.getAwardAdjustments().reconfigure(records[0].awardAdjustments());
                        me.getAwardReasons().reconfigure(records[0].awardReasons());
                        me.getPaymentAdjustments().reconfigure(records[0].paymentAdjustments());
                        _selectedFinRecord.paymentDetails = records[0]; //Need to figure out a way to remove globals
                    }
                    me.getPaymentDetails().setLoading(false, true);
                    me.getAwardAdjustments().setLoading(false, true);
                    me.getAwardReasons().setLoading(false, true);
                    me.getPaymentAdjustments().setLoading(false, true);
                    me.application.fireEvent('webservicecallcomplete', operation, 'paymentinformation.PaymentDetail');
                },
                scope: me
            });

        } else {
            var empty = Ext.create('VIP.model.paymentinformation.PaymentDetail');

            me.getPaymentDetails().loadRecord(empty);
            me.getAwardAdjustments().reconfigure(empty.awardAdjustments());
            me.getAwardReasons().reconfigure(empty.awardReasons());
            me.getPaymentAdjustments().reconfigure(empty.paymentAdjustments());
            _selectedFinRecord.paymentDetails = empty; //Need to figure out a way to remove globals
        }

        if (!Ext.isEmpty(selection)) {
            me.getAddressField().setValue(selection[0].get('fullAddress'));
        }
    },

    loadPaymentStore: function () {
        var me = this;

        if (!Ext.isEmpty(me.selectedPerson.get('participantId')) && !Ext.isEmpty(me.selectedPerson.get('fileNumber')) && !Ext.isEmpty(me.selectedPerson.get('payeeCode'))) {
            me.getPaymentStore().load({
                filters: [{
                    property: 'ParticipantId',
                    value: me.selectedPerson.get('participantId')
                },
				{
				    property: 'FileNumber',
				    value: me.selectedPerson.get('fileNumber')
				},
                {
                    property: 'PayeeCode',
                    value: me.selectedPerson.get('payeeCode')
                }],
                callback: function (records, operation, success) {
                    me.application.fireEvent('webservicecallcomplete', operation, 'Payment');
                    me.updatePaymentInformationCount();
                },
                scope: me
            });
        }
    },

    filterPaymentsByPayee: function (menu, item, e, eOpts) {
        var me = this, i;
        
        me.getPayeeCodeDropDown().setText('Payee: ' + item.text);
        
        for (i = 0; i < me.awardsRecipients.length; i++) {
            if (me.awardsRecipients[i].payeeCode === item.text) {
                me.selectedPerson.set('payeeCode', me.awardsRecipients[i].payeeCode);
                me.selectedPerson.set('participantId', me.awardsRecipients[i].participantId || me.application.personInquiryModel.get('participantId'));
                break;
            }
        }

        me.loadPaymentStore();
    },

    onAwardsLaoded: function (awards) {
        var i, j, k, l, m, payeeCode, payeeCodelist = [], exists = false, me = this, menu = [], dropMenu = [];

        me.awardsRecipients = [];
        me.getPayeeCodeDropDown().menu = null;

        if (!Ext.isEmpty(awards) && awards.length > 0) {
            for (i = 0; i < awards.length; i++) {
                payeeCode = awards[i].data.payeeCode || '00';
                me.awardsRecipients.push({ participantId: awards[i].data.participantRecipientId, payeeCode: payeeCode });

                if (menu.length === 0) {
                    menu.push(payeeCode);
                } else {
                    exists = false;
                    for (j = 0; j < menu.length; j++) {
                        if (menu[j] === payeeCode) {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                        menu.push(payeeCode);
                }
            }
        }

        payeeCodelist[0] = '00';
        payeeCodelist[1] = '41';
        payeeCodelist[2] = '42';
        payeeCodelist[3] = '43';
        payeeCodelist[4] = '44';
        payeeCodelist[5] = '45';
        payeeCodelist[6] = '50';
        payeeCodelist[7] = '60';

        for (l = 0; l < payeeCodelist.length; l++) {
            var add = true;
            for (k = 0; k < me.awardsRecipients.length; k++) {
                if (me.awardsRecipients[k].payeeCode === payeeCodelist[l]) {
                    add = false;
                    break;
                }
            }

            if (add)
                me.awardsRecipients.push({ participantId: '000000000', payeeCode: payeeCodelist[l] });
        }

        for (l = 0; l < payeeCodelist.length; l++) {
            add = true;
            for (k = 0; k < menu.length; k++) {
                if (menu[k] === payeeCodelist[l]) {
                    add = false;
                    break;
                }
            }

            if (add)
                menu.push(payeeCodelist[l]);
        }

        menu.sort()

        for (var m = 0; m < menu.length; m++) {
            dropMenu.push({ text: menu[m] });
        }

        me.getPayeeCodeDropDown().menu = Ext.create('Ext.menu.Menu', { defaults: { iconCls: 'icon-doc' }, items: dropMenu });
    }

});
