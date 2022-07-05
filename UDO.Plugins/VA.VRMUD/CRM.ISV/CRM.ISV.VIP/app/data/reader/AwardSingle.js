Ext.define('VIP.data.reader.AwardSingle', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.awardsingle',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("evr", "evrs", "return", response);
        VIP.util.Xml.wrap("flash", "flashes", "return", response);
        VIP.util.Xml.wrap("diary", "diaries", "return", response);

        return me.callParent([response]);
    }
});