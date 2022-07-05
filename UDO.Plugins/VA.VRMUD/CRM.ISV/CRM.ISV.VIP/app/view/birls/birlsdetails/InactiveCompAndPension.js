/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.InactiveCompAndPension
*
* The view for InactiveCompAndPension associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.InactiveCompAndPension', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.inactivecompandpension',
    store: 'Birls',
    title: 'Inactive Comp & Pension',
    layout: {
        type: 'table',
        columns: 5,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120
        //,width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{ //ROW 1
            xtype: 'displayfield',
            fieldLabel: 'Burial Flag Issued',
            name: 'burialFlagIssueInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'App for Plot',
            name: 'applicationForPlot'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Burial Award Svc Connected',
            name: 'burialAwardServiceConnected'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Burial Award Transport',
            name: 'burialAwardTransport'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Burial Award Plot',
            name: 'burialAwardPlot'
        }, { //ROW 2
            xtype: 'displayfield',
            fieldLabel: 'Entitlement Cd',
            name: 'entitlementCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Burial Award Non-Svc Connected',
            name: 'burialAwardNonServiceConnected'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Headstone',
            name: 'headstone'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Payment',
            name: 'payment'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Clothing Allowance',
            name: 'clothingAllowance'
        }, { //ROW 3
            xtype: 'displayfield',
            fieldLabel: 'Automobile Allowance',
            name: 'automobileAllowance'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Adaptive Equipment',
            name: 'adaptiveEquipment'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Special Adaptive Housing',
            name: 'specialAdaptiveHousing'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Special Law Cd',
            name: 'specialLawCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Combined Degree',
            name: 'combinedDegree'
        }, { //ROW 4
            xtype: 'displayfield',
            fieldLabel: 'C&P Term Date',
            name: 'cpEffectiveDateOfTerm'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Reason for Term or Disallowance',
            name: 'reasonForTermDisallow'
        }];

        me.callParent();
    }
});