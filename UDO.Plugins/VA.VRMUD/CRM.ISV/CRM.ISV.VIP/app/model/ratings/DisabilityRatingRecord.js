/**
* @author Josh Oliver
* @class VIP.model.ratings.DisabilityRatingRecord
*
* The model for disability rating records
*/
Ext.define('VIP.model.ratings.DisabilityRatingRecord', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.rating.FindRatingData'
    ],
    fields: [
		{
		    name: 'combinedDegreeEffectiveDate',
		    type: 'date',
		    mapping: 'combinedDegreeEffectiveDate',
		    dateFormat: 'mdY'
		},
        {
            name: 'combinedDegreeEffectiveDate_F',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('combinedDegreeEffectiveDate'))) {
                    return Ext.Date.format(record.get('combinedDegreeEffectiveDate'), "m/d/Y");
                } else return '';
            }
        },
		{
		    name: 'legalEffectiveDate',
		    type: 'date',
		    mapping: 'legalEffectiveDate',
		    dateFormat: 'mdY'
		},
        {
            name: 'legalEffectiveDate_F',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('legalEffectiveDate'))) {
                    return Ext.Date.format(record.get('legalEffectiveDate'), "m/d/Y");
                } else return '';
            }
        },
		{
		    name: 'promulgationDate',
		    type: 'date',
		    mapping: 'promulgationDate',
		    dateFormat: 'mdY'
		},
        {
            name: 'promulgationDate_F',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('promulgationDate'))) {
                    return Ext.Date.format(record.get('promulgationDate'), "m/d/Y");
                } else return '';
            }
        },
		{
		    name: 'numberOfRecords',
		    type: 'string',
		    mapping: 'numberOfRecords'
		},
		{
		    name: 'returnCode',
		    type: 'string',
		    mapping: 'returnCode'
		},
		{
		    name: 'returnMessage',
		    type: 'string',
		    mapping: 'returnMessage'
		},
		{
		    name: 'serviceConnectedCombinedDegree',
		    type: 'string',
		    mapping: 'serviceConnectedCombinedDegree'
		},
		{
		    name: 'nonServiceConnectedCombinedDegree',
		    type: 'string',
		    mapping: 'nonServiceConnectedCombinedDegree'
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
            record: 'disabilityRatingRecord'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.rating.FindRatingData',
            update: '',
            destroy: ''
        }
    }

});