Ext.define('VIP.data.reader.PersonVadir', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.personVadir',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("aliases", "alias", "person", response);

        return me.callParent([response]);
    }
});