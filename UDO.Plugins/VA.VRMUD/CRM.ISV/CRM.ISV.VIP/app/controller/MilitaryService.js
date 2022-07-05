/**
* @author Ivan Yurisevic
* @class VIP.controller.MilitaryService
*
* The controller for the Military Service tab
*/

Ext.define('VIP.controller.MilitaryService', {
    extend: 'Ext.app.Controller',
    stores: [
        'MilitaryService',
        'militaryservice.Decoration',
		'militaryservice.Pow',
		'militaryservice.ReadjustmentBalance',
		'militaryservice.ReadjustmentPayment',
		'militaryservice.RetirementPayment',
		'militaryservice.SeparationBalance',
		'militaryservice.SeparationPayment',
		'militaryservice.SeveranceBalance',
		'militaryservice.SeverancePayment',
		'militaryservice.Theater',
		'militaryservice.TourHistory',
        'militaryservice.MilitaryPersons'
    ],

    refs: [{
        ref: 'militaryTabs',
        selector: '[xtype="militaryservice.militarytabs"]'
    }, {
        ref: 'decorationsGrid',
        selector: '[xtype="militaryservice.militarytabs.decorations"]'
    }, {
        ref: 'retirementPayGrid',
        selector: '[xtype="militaryservice.militarytabs.retirementpay"]'
    }, {
        ref: 'readjustmentBalanceGrid',
        selector: '[xtype="militaryservice.militarytabs.readjustmentpay.readjustmentbalances"]'
    }, {
        ref: 'readjustmentPaymentGrid',
        selector: '[xtype="militaryservice.militarytabs.readjustmentpay.readjustmentpayments"]'
    }, {
        ref: 'separationBalanceGrid',
        selector: '[xtype="militaryservice.militarytabs.separationpay.separationbalances"]'
    }, {
        ref: 'separationPaymentGrid',
        selector: '[xtype="militaryservice.militarytabs.separationpay.separationpayments"]'
    }, {
        ref: 'severanceBalanceGrid',
        selector: '[xtype="militaryservice.militarytabs.severancepay.severancebalances"]'
    }, {
        ref: 'severancePaymentGrid',
        selector: '[xtype="militaryservice.militarytabs.severancepay.severancepayments"]'
    }, {
        ref: 'powGrid',
        selector: '[xtype="militaryservice.militarytabs.theatersandpow.powlist"]'
    }, {
        ref: 'theatersGrid',
        selector: '[xtype="militaryservice.militarytabs.theatersandpow.theaterslist"]'
    }, {
        ref: 'tourHistoryDetails',
        selector: '[xtype="militaryservice.militarytabs.tourhistory.tourhistorydetails"]'
    }, {
        ref: 'tourHistoryGrid',
        selector: '[xtype="militaryservice.militarytabs.tourhistory.tourhistorylist"]'
    }, {
        ref: 'militaryPersonsGrid',
        selector: '[xtype="militaryservice.militarytabs.militarypersons"]'
    }],


    init: function () {
        var me = this;

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            triggermilitarytabchange: me.onMilitaryServiceTabChange,
            cacheddataloaded: me.onCachedDataLoaded,
            scope: me
        });

        me.control({
            '[xtype="militaryservice.militarytabs.tourhistory.tourhistorylist"]': {
                selectionchange: me.OnTourHistorySelect
            },
            '[xtype="militaryservice.militarytabs"]': {
                tabchange: me.onMilitaryServiceTabChange
            }
        });

        Ext.log('The Military Service controller was successfully initialized.');
    },


    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        me.getMilitaryServiceStore().load({
            filters: [{
                property: 'ptcpntId',
                value: selectedPerson.get('participantId')
            }],
            callback: function (records, operation, success) {
                if (success && !Ext.isEmpty(records)) {
                    me.getTourHistoryGrid().reconfigure(records[0].militaryTours());

                    me.getDecorationsGrid().reconfigure(records[0].decorations());

                    me.getTheatersGrid().reconfigure(records[0].theatres());
                    me.getPowGrid().reconfigure(records[0].personPows());

                    me.getRetirementPayGrid().reconfigure(records[0].retirementPayments());

                    me.getSeverancePaymentGrid().reconfigure(records[0].severancePayments());
                    me.getSeveranceBalanceGrid().reconfigure(records[0].severanceBalances());

                    me.getReadjustmentBalanceGrid().reconfigure(records[0].readjustmentBalances());
                    me.getReadjustmentPaymentGrid().reconfigure(records[0].readjustmentPayments());

                    me.getSeparationBalanceGrid().reconfigure(records[0].separationBalances());
                    me.getSeparationPaymentGrid().reconfigure(records[0].separationPayments());

                    me.getMilitaryPersonsGrid().reconfigure(records[0].persons());
                }
                me.application.fireEvent('webservicecallcomplete', operation, 'MilitaryService');
            },
            scope: me
        });

    },

    OnTourHistorySelect: function (selectionModel, selection, options) {
        var me = this;
        me.getTourHistoryDetails().loadRecord(selection[0]);
    },

    onMilitaryServiceTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this,
            activeTab = me.getMilitaryTabs().getActiveTab(),
            tabTitle = activeTab.title,
            gridCount = null;

        if (activeTab && activeTab.viewType == 'gridview') {
            gridCount = activeTab.getStore().getCount();
        } else if (activeTab.getXType() == 'militaryservice.militarytabs.tourhistory') {
            tabTitle = 'Tours: ' + me.getTourHistoryGrid().getStore().getCount();
        } else if (activeTab.getXType() == 'militaryservice.militarytabs.theatersandpow') {
            tabTitle = 'Theaters: ' + me.getTheatersGrid().getStore().getCount() +
                       ', POWs: ' + me.getPowGrid().getStore().getCount();
        } else if (activeTab.getXType() == 'militaryservice.militarytabs.severancepay') {
            tabTitle = 'Severance Payments: ' + me.getSeverancePaymentGrid().getStore().getCount() +
                       ', Balances: ' + me.getSeveranceBalanceGrid().getStore().getCount();
        } else if (activeTab.getXType() == 'militaryservice.militarytabs.readjustmentpay') {
            tabTitle = 'Readjustment Payments: ' + me.getReadjustmentPaymentGrid().getStore().getCount() +
                       ', Balances: ' + me.getReadjustmentBalanceGrid().getStore().getCount();
        } else if (activeTab.getXType() == 'militaryservice.militarytabs.separationpay') {
            tabTitle = 'Separation Payments: ' + me.getSeparationPaymentGrid().getStore().getCount() +
                       ', Balances: ' + me.getSeparationBalanceGrid().getStore().getCount();
        } else {
            tabTitle = null;
        }
        me.application.fireEvent('setstatisticstext', tabTitle, gridCount);
    },

    onCachedDataLoaded: function () {
        var me = this,
            militaryData = me.getMilitaryServiceStore().getAt(0);

        if (!Ext.isEmpty(militaryData)) {

            me.getTourHistoryGrid().reconfigure(militaryData.militaryTours());
            me.getDecorationsGrid().reconfigure(militaryData.decorations());
            me.getTheatersGrid().reconfigure(militaryData.theatres());
            me.getPowGrid().reconfigure(militaryData.personPows());
            me.getRetirementPayGrid().reconfigure(militaryData.retirementPayments());
            me.getSeverancePaymentGrid().reconfigure(militaryData.severancePayments());
            me.getSeveranceBalanceGrid().reconfigure(militaryData.severanceBalances());
            me.getReadjustmentBalanceGrid().reconfigure(militaryData.readjustmentBalances());
            me.getReadjustmentPaymentGrid().reconfigure(militaryData.readjustmentPayments());
            me.getSeparationBalanceGrid().reconfigure(militaryData.separationBalances());
            me.getSeparationPaymentGrid().reconfigure(militaryData.separationPayments());
            me.getMilitaryPersonsGrid().reconfigure(militaryData.persons());

            return;
        }
    }
});