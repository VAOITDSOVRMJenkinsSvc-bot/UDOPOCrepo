Ext.define('VIP.data.reader.Appeals', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.appeals',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("issues", "Issue", "AppealRecord", response);
        VIP.util.Xml.wrap("remandReasons", "RemandReason", "Issue", response);
        VIP.util.Xml.wrap("diaries", "Diary", "AppealRecord", response);
        VIP.util.Xml.wrap("specialContentions", "SpecialContentions", "AppealDecision", response);
        VIP.util.Xml.wrap("dates", "AppealDate", "AppealRecord", response);

        return me.callParent([response]);
    }
});