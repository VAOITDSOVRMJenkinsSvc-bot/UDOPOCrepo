/**
* @author Josh Oliver
* @class VIP.view.personVadir.details.Addresses
*
* View for Addresses associated with the person
*/
Ext.define('VIP.view.personVadir.details.Addresses', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personvadir.details.addresses',
    store: 'personVadir.Address',
    title: 'Addresses',

    initComponent: function () {
        var me = this;

        me.store = 'personVadir.Address';

        me.columns = [
            { header: '', xtype: 'rownumberer' },
			{ header: 'Effective Date', flex: 1, dataIndex: 'effectiveDate', xtype: 'datecolumn', format: 'm/d/Y' },
			{ header: 'Address Type', flex: 1, dataIndex: 'addressType' },
			{ header: 'Address Line 1', flex: 1, dataIndex: 'addressLine1' },
            { header: 'Address Line 2', flex: 1, dataIndex: 'addressLine2' },
            { header: 'City', flex: 1, dataIndex: 'city' },
            { header: 'State', flex: 1, dataIndex: 'state' },
            { header: 'Zip Code', flex: 1, dataIndex: 'zipcode' },
            { header: 'Zip Code Extension', flex: 1, dataIndex: 'zipcodeExtension' },
            { header: 'Country Code', flex: 1, dataIndex: 'countryCode' }
        ];

        me.callParent();
    }
}); 