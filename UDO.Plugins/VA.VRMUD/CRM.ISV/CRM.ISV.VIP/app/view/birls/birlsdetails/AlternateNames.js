/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.AlternateNames
*
* The view for Alternate Addresses associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.AlternateNames', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.birls.birlsdetails.alternatenames',
    title: 'Alternate Names',
    store: 'birls.AlternateNames',

    initComponent: function () {
        var me = this;

        me.columns = [
            { xtype: 'rownumberer' },
            { header: 'Last Name', flex: 1, dataIndex: 'altLastName' },
            { header: 'First Name', flex: 1, dataIndex: 'altFirstName' },
            { header: 'Middle Name', flex: 1, dataIndex: 'altMiddleName' },
            { header: 'Suffix', flex: 1, dataIndex: 'altNameSuffix' }
        ];

        me.callParent();
    }
});
