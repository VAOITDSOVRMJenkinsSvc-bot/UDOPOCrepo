/**
* @author Ivan Yurisevic
* @class VIP.model.Ratings
*
* The model for Ratings record details
*/
Ext.define('VIP.model.Ratings', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.Ratings', //reader
        'VIP.soap.envelopes.share.rating.FindRatingData', //soap
        'VIP.model.ratings.DeathRating',
        'VIP.model.ratings.DisabilityRating',
        'VIP.model.ratings.FamilyRating',
        'VIP.model.ratings.OtherRating',
        'VIP.model.ratings.SmcParagraphRating',
        'VIP.model.ratings.SmcRating'
    ],

    fields: [{
        name: 'numberOfDeathRatings',
        mapping: 'deathRatingRecord/numberOfRecords',
        type: 'string'
    }, {
        name: 'numberOfDisabilityRatings',
        mapping: 'disabilityRatingRecord/numberOfRecords',
        type: 'string'
    }, {
        name: 'disabilityCombinedDegreeEffectiveDate',
        mapping: 'disabilityRatingRecord/combinedDegreeEffectiveDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'combinedDegreeEffectiveDate_F',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('disabilityCombinedDegreeEffectiveDate'))) {
                return Ext.Date.format(record.get('disabilityCombinedDegreeEffectiveDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'disabilityLegalEffectiveDate',
        mapping: 'disabilityRatingRecord/legalEffectiveDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'legalEffectiveDate_F',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('disabilityLegalEffectiveDate'))) {
                return Ext.Date.format(record.get('disabilityLegalEffectiveDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'disabilityNonServiceConnectedCombinedDegree',
        mapping: 'disabilityRatingRecord/nonServiceConnectedCombinedDegree',
        type: 'string'
    }, {
        name: 'disabilityPromulgationDate',
        mapping: 'disabilityRatingRecord/promulgationDate',
        type: 'date',
        dateFormat: 'mdY'
    }, {
        name: 'promulgationDate_F',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('disabilityPromulgationDate'))) {
                return Ext.Date.format(record.get('disabilityPromulgationDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'disabilityServiceConnectedDegree',
        mapping: 'disabilityRatingRecord/serviceConnectedCombinedDegree',
        type: 'string'
    }, {
        name: 'numberOfFamilyRatings',
        mapping: 'familyMemberRatingRecord/numberOfRecords',
        type: 'string'
    }, {
        name: 'numberOfOtherRatings',
        mapping: 'otherRatingRecord/numberOfRecords',
        type: 'string'
    }, {
        name: 'numberOfSMCParagraphRatings',
        mapping: 'specialMonthlyCompensationRatingRecord/numberOfSMCParagraphRecords',
        type: 'string'
    }, {
        name: 'numberOfSMCRatings',
        mapping: 'specialMonthlyCompensationRatingRecord/numberOfSMCRecords',
        type: 'string'
    }],

    //Associations begin:
    hasMany: [{
        model: 'VIP.model.ratings.DeathRating',
        name: 'deathratings',
        associationKey: 'deathratings'
    }, {
        model: 'VIP.model.ratings.DisabilityRating',
        name: 'disabilityratings',
        associationKey: 'disabilityratings'
    }, {
        model: 'VIP.model.ratings.FamilyRating',
        name: 'familyratings',
        associationKey: 'familyratings'
    }, {
        model: 'VIP.model.ratings.OtherRating',
        name: 'otherratings',
        associationKey: 'otherratings'
    }, {
        model: 'VIP.model.ratings.SmcParagraphRating',
        name: 'smcparagraphrating',
        associationKey: 'smcparagraphrating'
    }, {
        model: 'VIP.model.ratings.SmcRating',
        name: 'smcrating',
        associationKey: 'smcrating'
    }],

    //Proxy and custom reader begins:
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'ratings',
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.rating.FindRatingData',
            update: '',
            destroy: ''
        }
    }
});