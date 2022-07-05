Ext.define('VIP.data.reader.Comerica', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.comerica',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("evr", "evrs", "return", response);

        return me.callParent([response]);
    }
});