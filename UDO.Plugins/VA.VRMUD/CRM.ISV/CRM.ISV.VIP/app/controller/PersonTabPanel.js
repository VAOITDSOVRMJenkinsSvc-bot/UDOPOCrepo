/**
* @author Jonas Dawson
* @class VIP.controller.PersonTabPanel
*
* The controller for the tab panel which encapsulates the
* associated data centers of a person.
* Controls the selection of an individual to identify the caller
*/
Ext.define('VIP.controller.PersonTabPanel', {
    extend: 'Ext.app.Controller',

    stores: [
		'debug.WebServiceRequestHistory'
	],

    refs: [{
        ref: 'personTabPanel',
        selector: 'persontabpanel'
    }, {
        ref: 'personInfo',
        selector: 'personinfo'
    }, {
        ref: 'personvadir',
        selector: 'personvadir'
    }, {
        ref: 'poa',
        selector: 'poa'
    }, {
        ref: 'fiduciary',
        selector: 'fiduciary'
    }, {
        ref: 'birls',
        selector: 'birls'
    }, {
        ref: 'awards',
        selector: 'awards'
    }, {
        ref: 'claims',
        selector: 'claims'
    },
    {
        ref: 'virtualva',
        selector: 'virtualva'
    },
    {
        ref: 'militaryservice',
        selector: 'militaryservice'
    }, {
        ref: 'paymenthistory',
        selector: 'paymenthistory'
    }, {
        ref: 'paymentinformation',
        selector: 'paymentinformation'
    }, {
        ref: 'ratings',
        selector: 'ratings'
    }, {
        ref: 'denials',
        selector: 'denials'
    }, {
        ref: 'appeals',
        selector: 'appeals'
    }, {
        ref: 'pathways',
        selector: 'pathways'
    }],

    init: function () {
        var me = this;

        me.control({
            'persontabpanel': { tabchange: me.onTabChange },
            scope: me
        });

        me.application.on({
            individualidentified: me.showPersonTabPanel,
            webservicecallcomplete: me.triggerTabChangeEvent,
            scope: me
        });

        Ext.log('The PersonTabPanel controller has been initialized');
    },

    showPersonTabPanel: function () {
        this.getPersonTabPanel().expand();
        this.onPersonInfoTabSelect();
    },

    /*
    * @triggerTabChangeEvent
    * This is attached to the same event that handles caching. It is triggered in the callback of store loads. In here it is used
    * to figure out if we need to show the alert icon in the status bar.
    */
    triggerTabChangeEvent: function (operation, storeName) {
        var me = this,
            activeTab = me.getPersonTabPanel().getActiveTab().getXType();

        if (storeName == 'Corp' || storeName == 'personinfo.Addresses' || storeName == 'personinfo.Dependents' ||
           storeName == 'personinfo.AllRelationships' || storeName == 'personinfo.GeneralDetails' || storeName == 'Birls' ||
            storeName == 'ebenefits.Ebenefits') {
            if (activeTab == 'personinfo') { me.onPersonInfoTabSelect(); me.application.fireEvent('personinfotabchange'); }
        }
        if (storeName == 'FiduciaryPoa') {
            if (activeTab == 'poa') { me.onPoaTabSelect(); me.application.fireEvent('fidpoatabchange', me.getPoa()); }
            if (activeTab == 'fiduciary') { me.onFiduciaryTabSelect(); me.application.fireEvent('fidpoatabchange', me.getFiduciary()); }
        }
        if (storeName == 'Birls') {
            if (activeTab == 'birls') { me.onBirlsTabSelect(); }
        }
        if (storeName == 'Awards' || storeName == 'awards.SingleAward' || storeName == 'awards.Fiduciary' ||
           storeName == 'awards.AwardInfo' || storeName == 'awards.IncomeExpenseInfo') {
            if (activeTab == 'awards') { me.onAwardsTabSelect(); me.application.fireEvent('triggerawardstabchange'); }
        }
        if (storeName == 'claims.Evidence' || storeName == 'claims.notes.All' || storeName == 'Claims' ||
           storeName == 'claims.Contentions' || storeName == 'claims.ClaimDetail' || storeName == 'claims.LifeCycle' ||
           storeName == 'claims.Suspense' || storeName == 'claims.TrackedItems' || storeName == 'claims.Letters' || storeName == 'claims.Status') {
            if (activeTab == 'claims') { me.onClaimsTabSelect(); me.application.fireEvent('triggerclaimtabchange'); }
        }
        if (storeName == 'VirtualVA') {
            if (activeTab == 'virtualva') { me.onVVATabChange(); me.application.fireEvent('triggervirtualvatabchange'); }
        }

        if (storeName == 'MilitaryService') {
            if (activeTab == 'militaryservice') { me.onMilitaryServiceTabSelect(); me.application.fireEvent('triggermilitarytabchange'); }
        }
        if (storeName == 'PaymentHistory') {
            if (activeTab == 'paymenthistory') { me.onPaymentHistoryTabSelect(); me.application.fireEvent('triggerpaymenthistorytabchange'); }
        }
        if (storeName == 'Payment' || storeName == 'paymentinformation.PaymentDetail') {
            if (activeTab == 'paymentinformation') { me.onPaymentInformationTabSelect(); me.application.fireEvent('triggerpaymentinformationtabchange'); }
        }
        if (storeName == 'Ratings') {
            if (activeTab == 'ratings') { me.onRatingsTabSelect(); me.application.fireEvent('triggerratingtabchange'); }
        }
        if (storeName == 'Denial' || storeName == 'denials.FullDenialReason') {
            if (activeTab == 'denials') { me.onDenialsTabSelect(); me.application.fireEvent('triggerdenialtabchange'); }
        }
        if (storeName == 'Appeal' || storeName == 'appeals.Detail') {
            if (activeTab == 'appeals') { me.onAppealsTabSelect(); me.application.fireEvent('triggerappealtabchange'); }
        }
        if (storeName == 'pathways.Mvi' || storeName == 'pathways.Appointment' || storeName == 'pathways.Patient') {
            if (activeTab == 'pathways') { me.onExamsAppointmentsTabSelect(); }
        }
        if (storeName == 'PersonVadir' || storeName == 'personVadir.ContactInfo') {
            if (activeTab == 'personvadir') { me.onPersonVadirTabSelect(); me.application.fireEvent('triggerpersonvadirtabchange', me.getPersonTabPanel(), me.getPersonTabPanel().getActiveTab()); }
        }
    },

    onTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this;

        me.application.fireEvent('setstatisticstext', null);
        if (newCard.getXType() == 'personinfo') { me.onPersonInfoTabSelect(); me.application.fireEvent('personinfotabchange'); }
        if (newCard.getXType() == 'poa') { me.onPoaTabSelect(); me.application.fireEvent('fidpoatabchange', newCard); }
        if (newCard.getXType() == 'fiduciary') { me.onFiduciaryTabSelect(); me.application.fireEvent('fidpoatabchange', newCard); }
        if (newCard.getXType() == 'birls') { me.onBirlsTabSelect(); me.application.fireEvent('birlsdetailstabchange'); }
        if (newCard.getXType() == 'awards') { me.onAwardsTabSelect(); me.application.fireEvent('triggerawardstabchange'); }
        if (newCard.getXType() == 'claims') { me.onClaimsTabSelect(); me.application.fireEvent('triggerclaimtabchange'); }
        if (newCard.getXType() == 'militaryservice') { me.onMilitaryServiceTabSelect(); me.application.fireEvent('triggermilitarytabchange'); }
        if (newCard.getXType() == 'paymenthistory') { me.onPaymentHistoryTabSelect(); me.application.fireEvent('triggerpaymenthistorytabchange'); }
        if (newCard.getXType() == 'paymentinformation') { me.onPaymentInformationTabSelect(); me.application.fireEvent('triggerpaymentinformationtabchange'); }
        if (newCard.getXType() == 'ratings') { me.onRatingsTabSelect(); me.application.fireEvent('triggerratingtabchange'); }
        if (newCard.getXType() == 'denials') { me.onDenialsTabSelect(); me.application.fireEvent('triggerdenialtabchange'); }
        if (newCard.getXType() == 'appeals') { me.onAppealsTabSelect(); me.application.fireEvent('triggerappealtabchange'); }
        if (newCard.getXType() == 'pathways') { me.onExamsAppointmentsTabSelect(); }
        if (newCard.getXType() == 'personvadir') { me.onPersonVadirTabSelect(); me.application.fireEvent('triggerpersonvadirtabchange', tabPanel, newCard, oldCard, eOpts) }
        if (newCard.getXType() == 'virtualva') { me.onVVATabChange(); me.application.fireEvent('triggervirtualvatabchange'); }
    },

    onPersonInfoTabSelect: function () {
        var me = this,
            index,
            methods = new Array('findBirlsRecordByFileNumber',
                       'findCorporateRecordByFileNumber',
                       'findGeneralInformationByPtcpntIds',
                       'findAllRelationships',
                       'findDependents',
                       'findAllPtcpntAddrsByPtcpntId',
                       'findVeteranByPtcpntId',
                       'getRegistrationStatus');

        me.application.fireEvent('hidealerticon');
        //The findBy function will see if any of the functions above have failures. If they do, show alert icon.
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        }
    },
    onPoaTabSelect: function () {
        var me = this,
            index;

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().find('method', 'findAllFiduciaryPoa');
        if (index != -1) {
            if (me.getDebugWebServiceRequestHistoryStore().getAt(index).get('success') == false) {
                me.application.fireEvent('showalerticon');
            }
        }
    },
    onFiduciaryTabSelect: function () {
        var me = this;
        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().find('method', 'findAllFiduciaryPoa');
        if (index != -1) {
            if (me.getDebugWebServiceRequestHistoryStore().getAt(index).get('success') == false) {
                me.application.fireEvent('showalerticon');
            }
        }
    },
    onBirlsTabSelect: function () {
        var me = this;
        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().find('method', 'findBirlsRecordByFileNumber');
        if (index != -1) {
            if (me.getDebugWebServiceRequestHistoryStore().getAt(index).get('success') == false) {
                me.application.fireEvent('showalerticon');
            }
        }
    },
    onAwardsTabSelect: function () {
        var me = this,
            index,
            methods = new Array('findGeneralInformationByFileNumber',
                               'findGeneralInformationByPtcpntIds',
                               'findOtherAwardInformation',
                               'findIncomeExpense',
                               'findFiduciary');

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        }
    },
    onClaimsTabSelect: function () {
        var me = this,
            index,
            methods = new Array('findDevelopmentNotes',
                               'findBenefitClaim',
                               'findUnsolEvdnce',
                               'findBenefitClaimDetail',
                               'findTrackedItems',
                               'findContentions',
                               'findClaimStatus');

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        }
    },
    onMilitaryServiceTabSelect: function () {
        var me = this;
        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().find('method', 'findMilitaryRecordByPtcpntId');
        if (index != -1) {
            if (me.getDebugWebServiceRequestHistoryStore().getAt(index).get('success') == false) {
                me.application.fireEvent('showalerticon');
            }
        }
    },
    onPaymentHistoryTabSelect: function () {
        var me = this;
        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().find('method', 'findPayHistoryBySSN');
        if (index != -1) {
            if (me.getDebugWebServiceRequestHistoryStore().getAt(index).get('success') == false) {
                me.application.fireEvent('showalerticon');
            }
        }
    },
    onPaymentInformationTabSelect: function () {
        var me = this,
            index,
            methods = new Array('retrievePaymentSummary',
                               'retrievePaymentDetail');

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        }
    },
    onRatingsTabSelect: function () {
        var me = this;
        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().find('method', 'findRatingData');
        if (index != -1) {
            if (me.getDebugWebServiceRequestHistoryStore().getAt(index).get('success') == false) {
                me.application.fireEvent('showalerticon');
            }
        }
    },
    onDenialsTabSelect: function () {
        var me = this,
            index,
            methods = new Array('findDenialsByPtcpntId',
                                'findReasonsByRbaIssueId');

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        }
    },
    onAppealsTabSelect: function () {
        var me = this;
        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().find('method', 'findAppeals');
        if (index != -1) {
            if (me.getDebugWebServiceRequestHistoryStore().getAt(index).get('success') == false) {
                me.application.fireEvent('showalerticon');
            }
        }
    },
    onExamsAppointmentsTabSelect: function () {
        var me = this,
            index,
            methods = new Array('PRPA_IN201305UV02',
                                'PRPA_IN201309UV02',
                                'readData');

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        }
    },
    onPersonVadirTabSelect: function () {
        var me = this,
            index,
            methods = new Array('FindPersonBySsnRequest',
                                'GetContactInfoRequest',
                                'FindPersonByEdipiRequest',
                                'FindPersonByFnameLnameRequest',
                                'FindPersonByFnameLnameDobRequest',
                                'FindPersonByLnameDobRequest');

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        }
    },

    onVVATabChange: function () {
        var me = this,
        index,
        methods = new Array('GetDocumentList');

        me.application.fireEvent('hidealerticon');
        index = me.getDebugWebServiceRequestHistoryStore().findBy(function (record, id) {
            var success = record.get('success'),
                method = record.get('method');
            if (success == false && me.arrayContainsObject(methods, method)) return true;
        });
        if (index != -1) {
            me.application.fireEvent('showalerticon');
        } 
    },

    arrayContainsObject: function (array, object) {
        if (array.indexOf) {
            if (array.indexOf(object) != -1) {
                return true;
            }
        }
        else {
            for (var i = 0; i < array.length; i++) {
                if (array[i] == object) {
                    return true;
                }
            }
        }
        //default
        return false;
    }
});