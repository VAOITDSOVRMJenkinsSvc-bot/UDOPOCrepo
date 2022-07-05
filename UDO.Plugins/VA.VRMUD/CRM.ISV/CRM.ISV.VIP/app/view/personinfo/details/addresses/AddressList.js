/**
* @author Josh Oliver
* @class VIP.view.personinfo.details.Addresses
*
* The view for Addresses associated with the person
*/
Ext.define('VIP.view.personinfo.details.addresses.AddressList', {
	extend: 'Ext.grid.Panel',
	alias: 'widget.personinfo.details.addresses.addresslist',
	store: 'personinfo.Addresses',

	initComponent: function () {
	    var me = this;
	    me.columns = {
	        //defaults: { width: 100 },
	        items: [
                { xtype: 'rownumberer' },
                { header: 'Address Type', dataIndex: 'participantAddressTypeName' },
			    { header: 'Effective Date', dataIndex: 'effectiveDate', xtype: 'datecolumn', format: 'm/d/Y' },
			    { header: 'Email Address', dataIndex: 'emailAddress' },
			    { header: 'Address 1', dataIndex: 'address1' },
			    { header: 'Address 2', dataIndex: 'address2' },
			    { header: 'Address 3', dataIndex: 'address3' },
			    { header: 'City', dataIndex: 'city' },
			    { header: 'State', dataIndex: 'postalCode' },
			    { header: 'Zipcode', dataIndex: 'zipPrefix' },
			    { header: 'Country', dataIndex: 'country' },
			    { header: 'Group 1 Verified', dataIndex: 'group1VerifiedTypeCode' },
			    { header: 'Mil PO Type', dataIndex: 'militaryPostOfficeTypeCode' },
			    { header: 'Mil Postal', dataIndex: 'militaryPostalTypeCode' },
			    { header: 'Territory', dataIndex: 'territoryName' },
			    { header: 'Province', dataIndex: 'providenceName' },
			    { header: 'Foreign Postal Code', dataIndex: 'foreignPostalCode' },
			    { header: 'Shared Address Ind', dataIndex: 'sharedAddresssInd' },
			    { header: 'Treasury 1', dataIndex: 'trsuryAddrsOneTxt' },
			    { header: 'Treasury 2', dataIndex: 'trsuryAddrsTwoTxt' },
			    { header: 'Treasury 3', dataIndex: 'trsuryAddrsThreeTxt' },
			    { header: 'Treasury 4', dataIndex: 'trsuryAddrsFourTxt' },
			    { header: 'Treasury 5', dataIndex: 'trsuryAddrsFiveTxt' },
			    { header: 'Treasury 6', dataIndex: 'trsuryAddrsSixTxt' },
			    { header: 'Treasury 7', dataIndex: 'trsuryAddrsSevenTxt' },
			    { header: 'Ptcpnt Id', dataIndex: 'participantId' }
            ]
	    };
		me.callParent();
	}
});
