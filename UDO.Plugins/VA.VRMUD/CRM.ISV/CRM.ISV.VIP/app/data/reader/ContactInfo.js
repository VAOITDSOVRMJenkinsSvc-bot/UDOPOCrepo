Ext.define('VIP.data.reader.ContactInfo', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.contactinfo',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("Addresses", "Address", "ContactInfo", response);
        VIP.util.Xml.wrap("Emails", "Email", "ContactInfo", response);
        VIP.util.Xml.wrap("Phones", "Phone", "ContactInfo", response);

        return me.callParent([response]);
    }
});