/**
* @author Josh Oliver
* @class VIP.view.ratings.disabilityratings.DisabilityDetails
*
* The view for disability details
*/
Ext.define('VIP.view.ratings.disabilityratings.DisabilityDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.ratings.disabilityratings.disabilitydetails',
    title: 'Disability Details (Select Grid Item To Populate)',
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
        labelWidth: 120,
        width: 220
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Diag Type Cd',
            name: 'diagnosticTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Diag Type',
            name: 'diagnosticTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'End Date',
            name: 'endDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Future Exam Date',
            name: 'futureExamDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'H. Diag Cd',
            name: 'hyphenatedDiagnosticTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'H. Diag',
            name: 'hyphenatedDiagnosticTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'H. Related Dis Cd',
            name: 'hyphenatedRelatedDisabilityTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'H. Related Disability',
            name: 'hyphenatedRelatedDisabilityTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Last Exam Date',
            name: 'lastExamDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Major Ind',
            name: 'majorIndicator'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil Svc Period Cd',
            name: 'militaryServicePeriodTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Mil Svc Period',
            name: 'militaryServicePeriodTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Paragraph Type Cd',
            name: 'paragraphTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Paragraph Type',
            name: 'paragraphTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Previous Service %',
            name: 'previousServicePercent'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Related Disability Cd',
            name: 'relatedDisabilityTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Related Disability',
            name: 'relatedDisabilityTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Supplemental Dec Cd',
            name: 'supplementalDecisionTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Supplemental Dec',
            name: 'supplementalDecisionTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Withholding %',
            name: 'withholdingPercent'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Withholding Cd',
            name: 'withholdingTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Withholding',
            name: 'withholdingTypeName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Bilateral Cd',
            name: 'bilateralTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Bilateral',
            name: 'bilateralTypeName'
        }];

        me.callParent();
    }
});