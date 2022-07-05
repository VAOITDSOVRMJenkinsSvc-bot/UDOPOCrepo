/**
* @author Josh Oliver
* @class VIP.view.personinfo.CorpDetails
*
* The view for person corporate detail information
*/
Ext.define('VIP.view.militaryservice.militarytabs.tourhistory.TourHistoryDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.militaryservice.militarytabs.tourhistory.tourhistorydetails',
    title: 'Tour History Details (Select Grid Item to Populate)',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120,
        width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'War Time Country',
            name: 'warTimeServiceCountryName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'War Time Service',
            name: 'warTimeServiceIndicator'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Lost Time Days',
            name: 'lostTimeDaysNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Travel Time Days',
            name: 'travelTimeDaysNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Travel Time Verified',
            name: 'travelTimeVerifiedIndicator'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Discharge Authority',
            name: 'mpDischargeAuthorityTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Discharge Type',
            name: 'mpDischargeCharacterTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Discharge Pay Grade',
            name: 'dischargePayGradeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Military Branch',
            name: 'militaryBranchIndicator'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil Duty VA Purpose',
            name: 'militaryDutyVaPurposeTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil Sep Narrative',
            name: 'militarySeperationNarritiveTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil Sep Reason',
            name: 'militarySeperationReasonTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Tour Service Status',
            name: 'militaryTourServiceStatusTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Pay Grade',
            name: 'payGradeTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'VADS Code',
            name: 'vadsCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'VAR',
            name: 'varIndicator'
        }];

        me.callParent();
    }
});