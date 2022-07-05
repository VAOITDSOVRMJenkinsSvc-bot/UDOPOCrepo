/**
* @author Ivan Yurisevic
* @class VIP.view.birls.BirlsDetails
*
* Details on the queried person.
*/
Ext.define('VIP.view.birls.BirlsDetails', {
    extend: 'Ext.tab.Panel',
    alias: 'widget.birls.birlsdetails',
    tabPosition: 'top',
    plain: true,
    requires: [
        'VIP.view.birls.birlsdetails.AlternateNames',
        'VIP.view.birls.birlsdetails.Disclosures',
        'VIP.view.birls.birlsdetails.Flashes',
        'VIP.view.birls.birlsdetails.FolderLocations',
        'VIP.view.birls.birlsdetails.InactiveCompAndPension',
        'VIP.view.birls.birlsdetails.InsurancePolicy',
        'VIP.view.birls.birlsdetails.MilitaryService',
        'VIP.view.birls.birlsdetails.Misc1',
        'VIP.view.birls.birlsdetails.Misc2',
        'VIP.view.birls.birlsdetails.ServiceDiagnostics'
    ],

    initComponent: function () {
        var me = this;

        me.items = [
            { xtype: 'birls.birlsdetails.militaryservice' },
            { xtype: 'birls.birlsdetails.alternatenames' },
            { xtype: 'birls.birlsdetails.insurancepolicy' },
            { xtype: 'birls.birlsdetails.inactivecompandpension' },
            { xtype: 'birls.birlsdetails.servicediagnostics' },
            { xtype: 'birls.birlsdetails.folderlocations' },
            { xtype: 'birls.birlsdetails.flashes' },
            { xtype: 'birls.birlsdetails.disclosures' },
            { xtype: 'birls.birlsdetails.misc1' },
            { xtype: 'birls.birlsdetails.misc2' }
        ];

        me.callParent();
    }
});


