/**
* @author Ivan Yurisevic
* @class VIP.controller.PaymentHistory
*
* The controller for the old payment history service
*/

Ext.define('VIP.controller.PaymentHistory', {
    extend: 'Ext.app.Controller',
    stores: [
        'PaymentHistory',
        'paymenthistory.PaymentAddress',
        'paymenthistory.Payments',
        'paymenthistory.ReturnedPayments',
		'paymentinformation.Comerica'
    ],
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    refs: [{
        ref: 'paymenthistory',
        selector: 'paymenthistory'
    }, {
        ref: 'paymentDetails',
        selector: '[xtype="paymenthistory.details"]'
    }, {
        ref: 'paymentTabs',
        selector: '[xtype="paymenthistory.payments"]'
    }, {
        ref: 'returnedPayments',
        selector: '[xtype="paymenthistory.payments.returnedpayments"]'
    }, {
        ref: 'paymentAddress',
        selector: '[xtype="paymenthistory.payments.paymentdata.paymentaddress"]'
    }, {
        ref: 'payments',
        selector: '[xtype="paymenthistory.payments.paymentdata.payments"]'
    }, {
        ref: 'corpinfo',
        selector: '[xtype="personinfo.corp"]'
    }],

    init: function () {
        var me = this;
        me.control({
            'paymenthistory > toolbar > paymentdisposition': { click: me.setPaymentDispositionHist },
            'paymenthistory > toolbar > paymentdisposition > menu': { click: me.setPaymentDispositionMenuClickHist },
            'paymenthistory > toolbar > button[action="showeducationcutoff"]': { click: me.showEducationCutOff },
            'paymenthistory > toolbar > button[action="showpaymentscript"]': { click: me.showPaymentScript },
            'paymenthistory > toolbar > button[action="showcandpscript"]': { click: me.showCandPScript },
            '[xtype="paymenthistory.payments.paymentdata.payments"]': { selectionchange: me.onPaymentHistorySelection },
            '[xtype="paymenthistory.payments"]': { tabchange: me.onPaymentHistoryTabChange }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            cacheddataloaded: me.onCachedDataLoaded,
            updateclaimprocessingtimelabel: me.onUpdatedClaimProcessingTimeLabel,
            gotcorp: me.onGotCorpData,
            claimsloaded: me.onGotClaims,
            triggerpaymenthistorytabchange: me.onPaymentHistoryTabChange,
            personloadedfromcache: me.onIndividualIdentified,
            scope: me
        });
    },

    onCachedDataLoaded: function () {
        var me = this,
			record = me.getPaymentHistoryStore().getAt(0);

        if (!Ext.isEmpty(record)) {
            me.getPayments().reconfigure(record.payments());
            me.getReturnedPayments().reconfigure(record.returnPayments());
            me.getPaymentDetails().loadRecord(record);

            record.payments().each(function (payment) {
                var paymentAddresses = record.paymentAddress().queryBy(function (paymentAddress, id) {
                    if (payment.get('payCheckID') == paymentAddress.get('addressId')) {
                        return true;
                    } else { return false; }
                }, me);

                payment.paymentAddress().loadRecords(paymentAddresses.getRange(), { addRecords: false });

            });
        }
    },

    // receive event from BIRLS tab and show claims folder SOJ
    onUpdatedClaimProcessingTimeLabel: function (claimFolderLocation) {
        var me = this;
        me.claimFolderLocation = claimFolderLocation;
    },
    onGotCorpData: function (corpData) {
        var me = this;
        me.corpData = corpData;
    },
    onGotClaims: function (claims) {
        var me = this;
        me.claims = claims;
    },

    showCandPScript: function () {
        window.open('https://vaww.vrm.km.va.gov/system/templates/selfservice/va_ka/portal.html?portalid=554400000001001&articleid=554400000002433', "CPScript", "width=600,height=700,scrollbars=1,resizable=1");
    },

    onPaymentHistoryTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
            activeTab = me.getPaymentTabs().getActiveTab(),
            tabTitle = activeTab.title,
            gridCount = null;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            if (Ext.get('id_PaymentHistory_01')) Ext.get('id_PaymentHistory_01').hide();
            if (Ext.get('id_PaymentHistory_02')) Ext.get('id_PaymentHistory_02').hide();
            if (Ext.get('id_PaymentHistory_03')) Ext.get('id_PaymentHistory_03').hide();
            if (Ext.get('id_PaymentHistory_04')) Ext.get('id_PaymentHistory_04').hide();
            if (Ext.get('id_PaymentHistory_05')) Ext.get('id_PaymentHistory_05').hide();
            if (Ext.get('id_PaymentHistory_06')) Ext.get('id_PaymentHistory_06').hide();
            if (Ext.get('id_PaymentHistory_07')) Ext.get('id_PaymentHistory_07').hide();
        }

        if (activeTab.getXType() == 'paymenthistory.payments.returnedpayments') {
            tabTitle = 'Returned Payments: ' + me.getReturnedPayments().getStore().getCount();
        } else if (activeTab.getXType() == 'paymenthistory.payments.paymentdata') {
            tabTitle = 'Payments: ' + me.getPayments().getStore().getCount();
        } else {
            tabTitle = null;
        }

        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    onPaymentHistorySelection: function (selectionModel, selection, options) {
        //debugger;
        var me = this;
        _selectedPayHistRecord = null;
        if (!Ext.isEmpty(selection[0]) && !Ext.isEmpty(selection[0].get('payCheckID'))) {
            if (!Ext.isEmpty(selection[0].data)) { _selectedPayHistRecord = selection[0].data; }
            me.getPaymentAddress().loadRecord(selection[0].paymentAddress().getAt(0));
        }
    },

    getSsnOrFileNumber: function () {
        var me = this;

        var ssn = me.application.personInquiryModel.get('ssn');
        if (Ext.isEmpty(ssn)) { ssn = me.application.personInquiryModel.get('fileNumber'); }

        return ssn;
    },

    setPaymentDispositionHist: function (button) {
        alert("Please select a payment disposition from the drop down.");
    },

    setPaymentDispositionMenuClickHist: function (menu, item, e, eOpts) {
        if (!parent || !parent._SetPrimaryTypeSubtype) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return;
        }
        parent._SetPrimaryTypeSubtype('PAYMENT_ANY', false, true, item.value);
    },

    showEducationCutOff: function () {
        window.open("https://vaww.vrm.km.va.gov/system/templates/selfservice/va_ka/portal.html?portalid=554400000001001&articleid=554400000003847");
    },

    showPaymentScript: function () {
        if (!parent || !parent._KMRoot) {
            alert('This feature works only in context of CRM screen hosting the UI');
            return;
        }

        var me = this;

        _claimFolderLocation = null, _corpData = null, _selectedRecForPayScript = null;
        try {
            if (_selectedPayHistRecord != undefined && !Ext.isEmpty(_selectedPayHistRecord)) { _selectedRecForPayScript = _selectedPayHistRecord; }
        } catch (er) { }
        if (!Ext.isEmpty(me.claimFolderLocation)) { _claimFolderLocation = me.claimFolderLocation; }
        if (!Ext.isEmpty(me.corpData)) { _corpData = me.corpData; }
        if (!Ext.isEmpty(me.claims)) { _claims = me.claims; }

        parent._SetPrimaryTypeSubtype('PAYMENT_GENERAL', true, true);

        var scriptSource = parent._KMRoot + 'PaymentSmart.html';
        window.open(scriptSource, "CallScript", "width=1024,height=800,scrollbars=1,resizable=1");
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        //debugger;
        var ssn = selectedPerson.get('ssn'), fNo = selectedPerson.get('fileNumber');
        if (!Ext.isEmpty(fNo) && fNo != ssn) { ssn = fNo; }

        if (!Ext.isEmpty(ssn)) {

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

                    me.getPaymentHistoryStore().load({
                        filters: [{
                            property: 'ssn',
                            value: ssn
                        }],
                        callback: function (records, operation, success) {
                            if (!Ext.isEmpty(records)) {
                                me.getPayments().reconfigure(records[0].payments());
                                me.getReturnedPayments().reconfigure(records[0].returnPayments());
                                me.getPaymentDetails().loadRecord(records[0]);


                                records[0].payments().each(function (payment) {
                                    var paymentAddresses = records[0].paymentAddress().queryBy(function (paymentAddress, id) {
                                        if (payment.get('payCheckID') == paymentAddress.get('addressId')) {
                                            return true;
                                        } else { return false; }
                                    }, me);

                                    payment.paymentAddress().loadRecords(paymentAddresses.getRange(), { addRecords: false });
                                });
                            }
                            me.application.fireEvent('webservicecallcomplete', operation, 'PaymentHistory');
                        },
                        scope: me
                    });
                },
                scope: me
            });
        }
    }

});
