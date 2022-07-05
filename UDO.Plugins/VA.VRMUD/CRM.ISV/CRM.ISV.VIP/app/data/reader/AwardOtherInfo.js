Ext.define('VIP.data.reader.AwardOtherInfo', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.awardotherinfo',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("receivable", "receivables", "awardInfo", response);
        VIP.util.Xml.wrap("parentClothingAllowanceInfo", "clothingAllowanceInfo", "return", response);
        VIP.util.Xml.wrap("deduction", "deductions", "awardInfo", response);
        VIP.util.Xml.wrap("proceeds", "accountBalances", "awardInfo", response);
        VIP.util.Xml.wrap("awardline", "awardLines", "return", response);
        VIP.util.Xml.wrap("awardreason", "awardReasons", "awardLines", response);

        return me.callParent([response]);
    }
});