/**
* @author Ivan Yurisevic
* @class VIP.view.ratings.disabilityratings.DisabilityRecordDetails
*
* The view for disability details
*/
Ext.define('VIP.view.ratings.disabilityratings.DisabilityRecordDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.ratings.disabilityratings.disabilityrecorddetails',
    title: 'Disability Record Info',
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
        width: 200
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [
            {
                xtype: 'displayfield',
                fieldLabel: 'Comb. Degree Efctv Date',
                name: 'combinedDegreeEffectiveDate_F',
                id: 'combinedDegreeEffectiveDate'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Non Svc Conn. Combined Degree',
                name: 'disabilityNonServiceConnectedCombinedDegree'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Svc Conn. Combined Degree',
                name: 'disabilityServiceConnectedDegree',
                id: 'disabilityServiceConnectedDegree'
            },
            {
                xtype: 'displayfield',
                fieldLabel: 'Legal Effective Date',
                name: 'legalEffectiveDate_F'
            },           
            {
                xtype: 'displayfield',
                fieldLabel: 'Promulgation Date',
                name: 'promulgationDate_F'
            }
        ];

        me.callParent();
    }
});
