/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.misc2.Indicators
*
* The view for the BIRLS fieldset at the top section
*/
Ext.define('VIP.view.birls.birlsdetails.misc2.Indicators', {
    extend: 'Ext.form.Panel',
    alias: 'widget.birls.birlsdetails.misc2.indicators',
    store: 'Birls',
    title: 'Indicators',
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

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Guardianship Case',
            name: 'guardianshipCaseInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Vet has Beneficiaries',
            name: 'vetHasBeneInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Vet is Beneficiary',
            name: 'vetIsBeneInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Appeals',
            name: 'appealsInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Bankruptcy',
            name: 'bankruptcyInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Persian Gulf Service',
            name: 'persianGulfServiceInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Vietnam Service',
            name: 'vietnamServiceInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Purple Heart',
            name: 'purpleHeartInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Medal of Honor',
            name: 'medalOfHonorInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Reenlisted',
            name: 'reenlistedInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Active Duty Training',
            name: 'activeDutyTrainingInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Transfer to Reserves',
            name: 'transferToReservesInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Incompetent',
            name: 'incompetentInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Disability',
            name: 'disabilityInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Homeless',
            name: 'homelessVetInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'VA Employee',
            name: 'vaEmployeeInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'CH 30',
            name: 'ch30Ind'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'CH 106',
            name: 'ch106Ind'
        }];

        me.callParent();
    }
});