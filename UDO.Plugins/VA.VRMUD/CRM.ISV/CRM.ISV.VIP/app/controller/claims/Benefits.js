/**
* @author Jonas Dawson
* @class VIP.controller.claims.Benefits
*
* Controller for the benefits grid
*/
Ext.define('VIP.controller.claims.Benefits', {
    extend: 'Ext.app.Controller',
    stores: ['Claims', 'Epc'],
    views: ['claims.Epc', 'claims.Benefits'],
    refs: [{
        ref: 'benefits',
        selector: '[xtype="claims.benefits"]'
    }, {
        ref: 'epcCombo',
        selector: '[xtype="claims.epc"] > combo'
    }],

    init: function () {
        var me = this;

        me.control({
            '[xtype="claims.epc"] > checkbox': {
                change: me.filterEpcByCode
            },
            '[xtype="claims.epc"] > combo': {
                select: me.filterClaims
            },
            '[xtype="claims.epc"] > button[action="clearclaimsfilter"]': {
                click: me.clearClaimsFilter
            },
            '[xtype="claims.benefits"] > toolbar > button[action="displayclaimscript"]': {
                click: me.displayClaimScript
            },
            '[xtype="claims.benefits"] > toolbar > button[action="startservicerequest"]': {
                click: me.serviceRequest
            },
            '[xtype="claims.benefits"] > toolbar > splitbutton[action="startservicerequest"] > menu': {
                click: me.serviceRequestMenuClick
            },
            '[xtype="claims.benefits"]': {
                select: me.onBenefitSelect
            }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            scope: me
        });

        Ext.log('The Claims.Benefits controller has been initialized');
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this,
            benefitsView = me.getBenefits(),
            epcCombo = me.getEpcCombo();

        epcCombo.getStore().filter([{
            id: 'code',
            property: 'code',
            value: false
        }]);
    },

    onBenefitSelect: function (rowModel, record, index) {

    },

    filterEpcByCode: function (checkBox, newValue, oldValue) {
        var me = this,
            epcCombo = me.getEpcCombo(),
            comboStore = epcCombo.getStore();

        comboStore.clearFilter(true);

        if (newValue) {
            comboStore.filter([{
                id: 'code',
                property: 'code',
                value: true
            }]);
        } else {
            comboStore.filter([{
                id: 'code',
                property: 'code',
                value: false
            }]);
        }

    },

    filterClaims: function () {
        var me = this,
            benefitsGrid = me.getBenefits(),
            epcCombo = me.getEpcCombo(),
            benefitsGridStore = benefitsGrid.getStore();

        me.clearClaimsFilter(false);
        benefitsGridStore.filterBy(function (record, id) {
            if (record.get('endProductTypeCode') == epcCombo.getValue())
                return true;
            else
                return false;
        });

    },

    clearClaimsFilter: function (clearComboBoxText) {
        var me = this,
            epcCombo = me.getEpcCombo(),
            benefitsGrid = me.getBenefits(),
            benefitsGridStore = benefitsGrid.getStore();

        if (benefitsGridStore.isFiltered()) {
            benefitsGridStore.clearFilter();
        }
        if (clearComboBoxText != false) {
            epcCombo.clearValue();
        }
    },

    displayClaimScript: function () {
        var me = this;
        Ext.log('A displayclaimscript event has been fired by the Claim.Benefits controller');
        me.application.fireEvent('displayclaimscript');
    },

    serviceRequest: function (button) {
        var me = this,
            defaultSelection = button.defaultMenuSelection;

        if (!Ext.isEmpty(defaultSelection)) {
            me.fireServiceRequestEvent(defaultSelection.text, defaultSelection.value);
        }
    },

    serviceRequestMenuClick: function (menu, item, e, eOpts) {
        var me = this;

        if (!Ext.isEmpty(item)) {
            me.fireServiceRequestEvent(item.text, item.value);
            Ext.log('A startservicerequest event was fired by the Claims.Benefits controller');
        }
    },

    fireServiceRequestEvent: function (name, value) {
        var me = this;

        me.application.fireEvent('startservicerequest', {
            name: name,
            value: value
        }, 'Claims');

        Ext.log('A startservicerequest event was fired by the PersonInfo controller');
    }
});