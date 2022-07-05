Ext.define('VIP.data.reader.Ratings', {
    extend: 'Ext.data.reader.Xml',
    alias: 'reader.ratings',
    requires: [
        'VIP.util.Xml'
    ],
    read: function (response) {
        var me = this;

        VIP.util.Xml.wrap("deathratings", "ratings", "deathRatingRecord", response);
        VIP.util.Xml.wrap("disabilityratings", "ratings", "disabilityRatingRecord", response);
        VIP.util.Xml.wrap("familyratings", "ratings", "familyMemberRatingRecord", response);
        VIP.util.Xml.wrap("otherratings", "ratings", "otherRatingRecord", response);
        VIP.util.Xml.wrap("smcparagraphrating", "smcParagraphRatings", "specialMonthlyCompensationRatingRecord", response);
        VIP.util.Xml.wrap("smcrating", "smcRatings", "specialMonthlyCompensationRatingRecord", response);

        return me.callParent([response]);
    }
});