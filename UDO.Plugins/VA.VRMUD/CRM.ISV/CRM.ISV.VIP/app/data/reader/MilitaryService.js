Ext.define('VIP.data.reader.MilitaryService', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.militaryservice',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("militaryTours", "militaryPersonTours", "return", response);
        VIP.util.Xml.wrap("decorations", "militaryPersonDecorations", "return", response);
        VIP.util.Xml.wrap("theatres", "militaryTheatres", "return", response);
        VIP.util.Xml.wrap("personPows", "militaryPersonPows", "return", response);
        VIP.util.Xml.wrap("retirementPayments", "militaryRetirementPays", "return", response);
        VIP.util.Xml.wrap("severancePayments", "militarySeverancePays", "return", response);
        VIP.util.Xml.wrap("severanceBalances", "militarySeveranceBalances", "return", response);
        VIP.util.Xml.wrap("readjustmentPayments", "militaryReadjustmentPays", "return", response);
        VIP.util.Xml.wrap("readjustmentBalances", "militaryReadjustmentBalances", "return", response);
        VIP.util.Xml.wrap("separationPayments", "militarySeperationPays", "return", response);
        VIP.util.Xml.wrap("separationBalances", "militarySeperationBalances", "return", response);
        VIP.util.Xml.wrap("persons", "militaryPersons", "return", response);

        return me.callParent([response]);
    }
});