/**
* @author Josh Oliver
* @class VIP.model.ratings.DisabilityRating
*
* The model for disability ratings
*/
Ext.define('VIP.model.ratings.DisabilityRating', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.rating.FindRatingData'
    ],
    fields: [
        // records in root
         {
             name: 'combinedDegreeEffectiveDate',
             type: 'date', mapping: 'combinedDegreeEffectiveDate'
         },
        {
            name: 'legalEffectiveDate',
            type: 'date', mapping: 'legalEffectiveDate'
        },
         {
             name: 'promulgationDate',
             type: 'date', mapping: 'promulgationDate'
         },
        {
            name: 'numberOfRecords',
            type: 'string', mapping: 'numberOfRecords'
        },
        {
            name: 'returnCode',
            type: 'string', mapping: 'returnCode'
        },
        {
            name: 'returnMessage',
            type: 'string', mapping: 'returnMessage'
        },
        {
            name: 'serviceConnectedCombinedDegree',
            type: 'string', mapping: 'serviceConnectedCombinedDegree'
        },
        {
            name: 'nonServiceConnectedCombinedDegree',
            type: 'string', mapping: 'nonServiceConnectedCombinedDegree'
        }
        
           ],

    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            root: 'disabilityRatingRecord',
            record: 'ratings'
            //record: 'disabilityRatingRecord'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.rating.FindRatingData',
            update: '',
            destroy: ''
        },
        url: 'http://vbmscert.vba.va.gov/RatingServiceBean/RatingWebService'
    }


});