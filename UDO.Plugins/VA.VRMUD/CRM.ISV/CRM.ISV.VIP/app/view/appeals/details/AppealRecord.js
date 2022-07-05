/**
* @author Ivan Yurisevic
* @class VIP.view.appeals.details.AppealRecord
*
* The view for the current POA fieldset
*/
Ext.define('VIP.view.appeals.details.AppealRecord', {
    extend: 'Ext.form.Panel',
    alias: 'widget.appeals.details.appealrecord',
    title: 'Appeal Record',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        },
        tdAttrs: {
            width: '25%'
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;
        
        // row 1
        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'DRO Id',
            name: 'decisionReviewOfficerId'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Docket #',
            name: 'docketNumber'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'RO Code',
            name: 'regionalOfficeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Svc Org Name',
            name: 'serviceOrganizationName'

            // row 2
        }, {
            xtype: 'displayfield',
            fieldLabel: 'DRO Elected Date',
            name: 'decisionReviewOfficerElectedDate_f'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Docket Date',
            name: 'docketDate_f'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'RO Name',
            name: 'regionalOfficeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'SVC Org Description',
            name: 'serviceOrganizationDescription'
        }, {
           
            // row 3
            xtype: 'displayfield',
            fieldLabel: 'DRO Ready to Rate Indicator',
            name: 'decisionReviewOfficerReadyToRateIndicator'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'BVA Receive Date',
            name: 'bvaReceivedDate_f'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'File Store Loc Desc',
            name: 'currentFileStoredLocationDescription'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Svc Org Rec Date',
            name: 'serviceOrganizationReceivedDate_f'
        }, {
            
            // row 4
            xtype: 'displayfield',
            fieldLabel: 'DRO Partial Grant/Denial Ind',
            name: 'decisionReviewOfficerPartialGrantOrDenialIndicator'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Appeal Action Type Desc',
            name: 'actionTypeDescription'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Chrg To Current Location',
            name: 'chargeToCurrentLocationDate_f',
            colspan: 2
        }, {
            
            // row 5
            xtype: 'displayfield',
            fieldLabel: 'DRO Informal Hearing Ind',
            name: 'decisionReviewOfficerInformalHearingIndicator',
            colspan: 2
        }, {
           
            xtype: 'displayfield',
            fieldLabel: 'Med Facility Code',
            name: 'medicalFacilityCode',
            colspan: 2
        }, {
           
            // row 6
            xtype: 'displayfield',
            fieldLabel: 'DRO Formal Hearing Ind',
            name: 'decisionReviewOfficerFormalHearingIndicator',
            colspan: 2
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Medical Facility Name',
            name: 'medicalFacilityName',
            colspan: 2
        }];

        me.callParent();
    }
});