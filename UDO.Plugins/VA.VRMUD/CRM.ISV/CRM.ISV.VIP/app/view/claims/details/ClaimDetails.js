/**
* @author Jonas Dawson
* @class VIP.view.claims.details.ClaimDetails
*
* Details for the selected claim
*/
Ext.define('VIP.view.claims.details.ClaimDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.claims.details.claimdetails',
    title: 'Claim Details',
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
            fieldLabel: 'End Product',
            name: 'endProductTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Claim Type Code',
            name: 'claimTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Claimant First Name',
            name: 'claimantFirstName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Claimant Last Name',
            name: 'claimantLastName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Payee Type Code',
            name: 'payeeTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Program Type Code',
            name: 'programTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Days Pending',
            name: 'daysPending'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Days Since Inception',
            name: 'daysSinceInception'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Organization Name',
            name: 'organizationName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Claim Station',
            name: 'soj'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'PCLR/PCAN Explanation',
            name: 'reasonText'
        }];

        me.callParent();
    }
});