/**
* @author Ivan Yurisevic
* @class VIP.model.militaryservice.TourHistory
*
* The model for service tour history
*/
Ext.define('VIP.model.militaryservice.TourHistory', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'daysActiveQuantity',
        type: 'string',
        mapping: 'daysActiveQty'
    }, {
        name: 'dischargePayGradeName',
        type: 'string',
        mapping: 'dischargePayGradeName'
    }, {
        name: 'eodDate',
        type: 'date',
        mapping: 'eodDate',
        dateFormat: 'm/d/Y'
    }, {
        name: 'eodDate_f',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('eodDate'))) {
                return Ext.Date.format(record.get('eodDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'lostTimeDaysNumber',
        type: 'string',
        mapping: 'lostTimeDaysNbr'
    }, {
        name: 'militaryBranchIndicator',
        type: 'string',
        mapping: 'militaryBranchInd'
    }, {
        name: 'militaryDutyVaPurposeTypeCode',
        type: 'string',
        mapping: 'militaryDutyVaPurposeTypeCd'
    }, {
        name: 'militaryPersonTourNumber',
        type: 'int',
        mapping: 'militaryPersonTourNbr'
    }, {
        name: 'militarySeperationNarritiveTypeCode',
        type: 'string',
        mapping: 'militarySeperationNarritiveTypeCd'
    }, {
        name: 'militarySeperationReasonTypeName',
        type: 'string',
        mapping: 'militarySeperationReasonTypeName'
    }, {
        name: 'militaryServiceBranchTypeName',
        type: 'string',
        mapping: 'militarySvcBranchTypeName'
    }, {
        name: 'militaryServiceOtherBranchTypeName',
        type: 'string',
        mapping: 'militarySvcOtherBranchTypeName'
    }, {
        name: 'militaryTourServiceStatusTypeName',
        type: 'string',
        mapping: 'militaryTourSvcStatusTypeName'
    }, {
        name: 'mpDischargeAuthorityTypeName',
        type: 'string',
        mapping: 'mpDischargeAuthorityTypeName'
    }, {
        name: 'mpDischargeCharacterTypeName',
        type: 'string',
        mapping: 'mpDischargeCharTypeName'
    }, {
        name: 'payGradeTypeName',
        type: 'string',
        mapping: 'payGradeTypeName'
    }, {
        name: 'participantId',
        type: 'string',
        mapping: 'ptcpntId'
    }, {
        name: 'radDate',
        type: 'date',
        mapping: 'radDate',
        dateFormat: 'm/d/Y'
    }, {
        name: 'radDate_f',
        convert: function (v, record) {
            if (!Ext.isEmpty(record.get('radDate'))) {
                return Ext.Date.format(record.get('radDate'), "m/d/Y");
            } else return '';
        }
    }, {
        name: 'serviceNumber',
        type: 'string',
        mapping: 'serviceNbr'
    }, {
        name: 'sixYearObligationDate',
        type: 'string',
        mapping: 'sixYearObligationDate'
    }, {
        name: 'travelTimeDaysNumber',
        type: 'string',
        mapping: 'travelTimeDaysNbr'
    }, {
        name: 'travelTimeVerifiedIndicator',
        type: 'string',
        mapping: 'travelTimeVerifiedInd'
    }, {
        name: 'vadsCode',
        type: 'string',
        mapping: 'vadsCd'
    }, {
        name: 'varIndicator',
        type: 'string',
        mapping: 'varInd'
    }, {
        name: 'verifiedIndicator',
        type: 'string',
        mapping: 'verifiedInd'
    }, {
        name: 'warTimeServiceCountryName',
        type: 'string',
        mapping: 'warTimeSvcCountryName'
    }, {
        name: 'warTimeServiceIndicator',
        type: 'string',
        mapping: 'warTimeSvcInd'
    }],

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'militaryPersonTours'
        }
    }
});