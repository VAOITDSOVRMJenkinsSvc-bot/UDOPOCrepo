Ext.define('VIP.data.reader.FiduciaryPoa', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.fiduciarypoa',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("fiduciaries", "pastFiduciaries", "return", response);
        VIP.util.Xml.wrap("poas", "pastPowerOfAttorneys", "return", response);

        return me.callParent([response]);
    }
});