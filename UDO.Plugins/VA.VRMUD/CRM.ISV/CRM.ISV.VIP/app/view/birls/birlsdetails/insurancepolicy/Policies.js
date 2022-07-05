/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.insurancepolicy.Policies
*
* The view for FolderLocations associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.insurancepolicy.Policies', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.birls.birlsdetails.insurancepolicy.policies',
    store: 'birls.InsurancePolicy',
    flex: 1,

    initComponent: function () {
        var me = this;

        me.columns = [
            { xtype: 'rownumberer' },
            { header: 'Policy Prefix', dataIndex: 'insurancePolicyPrefix' },
            { header: 'Policy Number', dataIndex: 'insurancePolicyNumber', flex: 1 }
        ];

        me.callParent();
    }
});