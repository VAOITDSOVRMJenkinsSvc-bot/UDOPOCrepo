/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.Pow
*
* The model service pows
*/
Ext.define('VIP.model.militaryservice.Pow', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'campSectorText',
        type: 'string',
        mapping: 'campSectorTxt'
    }, {
        name: 'captorText',
        type: 'string',
        mapping: 'captorTxt'
    }, {
        name: 'captureDate',
        type: 'string',
        mapping: 'captureDate'
    }, {
        name: 'days',
        type: 'string',
        mapping: 'days'
    }, {
        name: 'militaryPersonPowSequenceNumber',
        type: 'string',
        mapping: 'militaryPersonPowSeqNbr'
    }, {
        name: 'militaryTheatreTypeName',
        type: 'string',
        mapping: 'militaryTheatreTypeName'
    }, {
        name: 'powCountryTypeCode',
        type: 'string',
        mapping: 'powCountryTypeCd'
    }, {
        name: 'participantId',
        type: 'string',
        mapping: 'ptcpntId'
    }, {
        name: 'releaseDate',
        type: 'string',
        mapping: 'releaseDate'
    }, {
        name: 'underThirtyDaysIndicator',
        type: 'string',
        mapping: 'underThirtyDaysInd'
    }, {
        name: 'verifiedIndicator',
        type: 'string',
        mapping: 'verifiedInd'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militaryPersonPows'
        }
    }
});