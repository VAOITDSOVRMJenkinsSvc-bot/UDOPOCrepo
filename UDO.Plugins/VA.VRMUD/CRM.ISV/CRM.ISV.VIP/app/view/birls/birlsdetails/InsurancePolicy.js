/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.InsurancePolicy
*
* The view for InsurancePolicy associated with the person
*/

Ext.define('VIP.view.birls.birlsdetails.InsurancePolicy', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.birls.birlsdetails.insurancepolicy',
    title: 'Insurance Policy',
    layout: {
        type: 'hbox',
        align: 'stretch'
    },

    requires: [
        'VIP.view.birls.birlsdetails.insurancepolicy.Policies',
        'VIP.view.birls.birlsdetails.insurancepolicy.PolicyInfo'
    ],
    autoScroll: true,
    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'birls.birlsdetails.insurancepolicy.policies', padding: '0 5 0 0' },
            { xtype: 'birls.birlsdetails.insurancepolicy.policyinfo' }
        ];

        me.callParent();
    }

});
