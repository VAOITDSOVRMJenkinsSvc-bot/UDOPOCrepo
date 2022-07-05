/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.Disclosures
*
* The view for Alternate Addresses associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.Disclosures', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.birls.birlsdetails.disclosures',
    store: 'birls.Disclosures',
    title: 'Disclosures',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Account Number', dataIndex: 'recurringDisclosureNum' },
                { header: 'Date of Disclosure', dataIndex: 'dateOfDisclosure' },
                { header: 'Disclosure Month', dataIndex: 'recurringDisclosureMonth' },
                { header: 'Disclosure Year', dataIndex: 'recurringDisclosureYear' }
            ]
        };
        me.callParent();
    }
});
