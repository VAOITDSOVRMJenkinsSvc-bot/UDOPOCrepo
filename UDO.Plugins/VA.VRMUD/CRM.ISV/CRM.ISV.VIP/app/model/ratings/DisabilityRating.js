/**
* @class VIP.model.ratings.DisabilityRating
* The model for disability ratings
*/
Ext.define('VIP.model.ratings.DisabilityRating', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'beginDate',
        type: 'date',
        mapping: 'beginDate',
        dateFormat: 'mdY'
    }, {
        name: 'bilateralTypeCode',
        type: 'string',
        mapping: 'bilateralTypeCode'
    }, {
        name: 'bilateralTypeName',
        type: 'string',
        mapping: 'bilateralTypeName'
    }, {
        name: 'combatIndicator',
        type: 'string',
        mapping: 'combatIndicator'
    }, {
        name: 'diagnosticPercent',
        type: 'string',
        mapping: 'diagnosticPercent'
    }, {
        name: 'diagnosticText',
        type: 'string',
        mapping: 'diagnosticText'
    }, {
        name: 'diagnosticTypeCode',
        type: 'string',
        mapping: 'diagnosticTypeCode'
    }, {
        name: 'diagnosticTypeName',
        type: 'string',
        mapping: 'diagnosticTypeName'
    }, {
        name: 'disabilityDate',
        type: 'string',
        mapping: 'disabilityDate'
    }, {
        name: 'disabilityDecisionTypeCode',
        type: 'string',
        mapping: 'disabilityDecisionTypeCode'
    }, {
        name: 'disabilityDecisionTypeName',
        type: 'string',
        mapping: 'disabilityDecisionTypeName'
    }, {
        name: 'disabilityId',
        type: 'string',
        mapping: 'disabilityID'
    }, {
        name: 'endDate',
        type: 'string',
        mapping: 'endDate'
    }, {
        name: 'futureExamDate',
        type: 'string',
        mapping: 'futureExamDate'
    }, {
        name: 'hyphenatedDiagnosticTypeCode',
        type: 'string',
        mapping: 'hyphenatedDiagnosticTypeCode'
    }, {
        name: 'hyphenatedDiagnosticTypeName',
        type: 'string',
        mapping: 'hyphenatedDiagnosticTypeName'
    }, {
        name: 'hyphenatedRelatedDisabilityTypeCode',
        type: 'string',
        mapping: 'hyphenatedRelatedDisabilityTypeCode'
    }, {
        name: 'hyphenatedRelatedDisabilityTypeName',
        type: 'string',
        mapping: 'hyphenatedRelatedDisabilityTypeName'
    }, {
        name: 'lastExamDate',
        type: 'string',
        mapping: 'lastExamDate'
    }, {
        name: 'majorIndicator',
        type: 'string',
        mapping: 'majorIndicator'
    }, {
        name: 'militaryServicePeriodTypeCode',
        type: 'string',
        mapping: 'militaryServicePeriodTypeCode'
    }, {
        name: 'militaryServicePeriodTypeName',
        type: 'string',
        mapping: 'militaryServicePeriodTypeName'
    }, {
        name: 'paragraphTypeCode',
        type: 'string',
        mapping: 'paragraphTypeCode'
    }, {
        name: 'paragraphTypeName',
        type: 'string',
        mapping: 'paragraphTypeName'
    }, {
        name: 'previousServicePercent',
        type: 'string',
        mapping: 'previousServicePercent'
    }, {
        name: 'relatedDisabilityTypeCode',
        type: 'string',
        mapping: 'relatedDisabilityTypeCode'
    }, {
        name: 'relatedDisabilityTypeName',
        type: 'string',
        mapping: 'relatedDisabilityTypeName'
    }, {
        name: 'supplementalDecisionTypeCode',
        type: 'string',
        mapping: 'supplementalDecisionTypeCode'
    }, {
        name: 'supplementalDecisionTypeName',
        type: 'string',
        mapping: 'supplementalDecisionTypeName'
    }, {
        name: 'withholdingPercent',
        type: 'string',
        mapping: 'withholdingPercent'
    }, {
        name: 'withholdingTypeCode',
        type: 'string',
        mapping: 'withholdingTypeCode'
    }, {
        name: 'withholdingTypeName',
        type: 'string',
        mapping: 'withholdingTypeName'
    },

    // ********** Starting computed fields
    {
    name: 'diagnosticTypeCodeFormatted',
    type: 'string',
    convert: function (v, record) {
        var formattedCodeAndDescription = '';

        if (!Ext.isEmpty(record.get('hyphenatedDiagnosticTypeCode'))) {
            formattedCodeAndDescription = record.get('hyphenatedDiagnosticTypeCode');
        }

        if (!Ext.isEmpty(record.get('diagnosticTypeCode'))) {
            if (formattedCodeAndDescription.length > 0) {
                formattedCodeAndDescription += ' - ';
            }
            formattedCodeAndDescription += record.get('diagnosticTypeCode');
        }

        return formattedCodeAndDescription;
    }
}],
//Start reader:
proxy: {
    type: 'memory',
    reader: {
        type: 'xml',
        record: 'ratings'
    }
}
});