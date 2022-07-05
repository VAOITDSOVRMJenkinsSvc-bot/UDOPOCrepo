/**
* @author Ivan Yurisevic
* @class VIP.model.awards.awardinfo.AwardLines
*
* The model for Awards AwardLines record details
*/
Ext.define('VIP.model.awards.awardinfo.AwardLines', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.model.awards.awardinfo.awardlines.AwardReasons'
    ],
    fields: [{
        name: 'aaHbInd',
        mapping: 'aaHbInd',
        type: 'string'
    }, {
        name: 'altmnt',
        mapping: 'altmnt',
        type: 'float'
    }, {
        name: 'crdpAmt',
        mapping: 'crdpAmt',
        type: 'string'
    }, {
        name: 'crscAmt',
        mapping: 'crscAmt',
        type: 'string'
    }, {
        name: 'drillWitholding',
        mapping: 'drillWitholding',
        type: 'string'
    }, {
        name: 'effectiveDate',
        mapping: 'effectiveDate',
        dateFormat: 'mdY',
        type: 'date'
    }, {
        name: 'entitlementCode',
        mapping: 'entitlementCd',
        type: 'string'
    }, {
        name: 'entitlementName',
        mapping: 'entitlementNm',
        type: 'string'
    }, {
        name: 'helplessChild',
        mapping: 'helplessChild',
        type: 'string'
    }, {
        name: 'income',
        mapping: 'income',
        type: 'float'
    }, {
        name: 'instznWthldg',
        mapping: 'instznWthldg',
        type: 'string'
    }, {
        name: 'minorChild',
        mapping: 'minorChild',
        type: 'string'
    }, {
        name: 'netAward',
        mapping: 'netAward',
        type: 'float'
    }, {
        name: 'numberOfReasons',
        mapping: 'numberOfReasons',
        type: 'string'
    }, {
        name: 'otherAdjustments',
        mapping: 'otherAdjustments',
        type: 'string'
    }, {
        name: 'parentNumber',
        mapping: 'parentNbr',
        type: 'string'
    }, {
        name: 'recoupDisability',
        mapping: 'recoupDisability',
        type: 'string'
    }, {
        name: 'recoupSeperation',
        mapping: 'recoupSeperation',
        type: 'string'
    }, {
        name: 'recoupTort',
        mapping: 'recoupTort',
        type: 'string'
    }, {
        name: 'recoupTotal',
        mapping: 'recoupTotal',
        type: 'string'
    }, {
        name: 'schoolChild',
        mapping: 'schoolChild',
        type: 'string'
    }, {
        name: 'spouse',
        mapping: 'spouse',
        type: 'string'
    }, {
        name: 'totalAward',
        mapping: 'totalAward',
        type: 'float'
    }, {
        name: 'witholdingAmount',
        mapping: 'witholdingAmt',
        type: 'string'
    }, {
        name: 'reasons',
        type: 'string'
    }],

    //Start Associations
    hasMany: {
        model: 'VIP.model.awards.awardinfo.awardlines.AwardReasons',
        name: 'awardReasons',
        associationKey: 'awardreason'
    },

    //Start Memory Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'awardLines'
        }
    }
});