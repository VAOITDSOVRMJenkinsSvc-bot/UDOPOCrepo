/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.FolderLocations
*
* Details on the queried person.
*/
Ext.define('VIP.view.birls.birlsdetails.FolderLocations', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.birls.birlsdetails.folderlocations',
    title: 'Folder Locations',

    requires: ['VIP.view.birls.birlsdetails.folderlocinfo.AdditionalInfo', 'VIP.view.birls.birlsdetails.folderlocinfo.FolderLocationList'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'birls.birlsdetails.folderlocinfo.folderlocationlist',
            flex: 1
        }, {
            xtype: 'birls.birlsdetails.folderlocinfo.additionalinfo',
            flex: 1
        }];

        me.callParent();
    }
});