/**
* @author Josh Oliver
* @class VIP.model.appeals.Detail
*
* The model for appeal record details
*/
Ext.define('VIP.model.appeals.Detail', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.data.reader.Appeals',
        'VIP.soap.envelopes.vacols.appeals.GetAppeal',
        'VIP.model.appeals.Appellant',
        'VIP.model.appeals.AppellantAddress',
        'VIP.model.appeals.Veteran',
        'VIP.model.appeals.Issue',
        'VIP.model.appeals.Diary',
        'VIP.model.appeals.Decision',
        'VIP.model.appeals.SpecialContentions',
        'VIP.model.appeals.Date',
        'VIP.model.appeals.HearingRequest'
    ],
    fields: [
        {
            name: 'appealKey',
            type: 'string',
            mapping: 'AppealKey'
        },
        {
            name: 'appellantID',
            type: 'string',
            mapping: 'AppellantID'
        },
        {
            name: 'appealStatusCode',
            type: 'string',
            mapping: 'AppealStatusCode'
        },
        {
            name: 'bvaReceivedDate',
            mapping: 'BVAReceivedDate',
            type: 'date',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'bvaReceivedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('bvaReceivedDate'))) {
                    return Ext.Date.format(record.get('bvaReceivedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'docketNumber',
            type: 'string',
            mapping: 'DocketNumber'
        },
        {
            name: 'docketDate',
            mapping: 'DocketDate',
            type: 'date',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'docketDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('docketDate'))) {
                    return Ext.Date.format(record.get('docketDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'docketReceivedDate',
            mapping: 'DocketReceivedDate',
            type: 'date',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'docketReceivedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('docketReceivedDate'))) {
                    return Ext.Date.format(record.get('docketReceivedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'insuranceOrLoanNumber',
            type: 'string',
            mapping: 'InsuranceOrLoanNumber'
        },
        {
            name: 'medicalFacilityCode',
            type: 'string',
            mapping: 'MedicalFacilityCode'
        },
        {
            name: 'medicalFacilityName',
            type: 'string',
            mapping: 'MedicalFacilityName'
        },
        {
            name: 'actionTypeCode',
            type: 'string',
            mapping: 'ActionTypeCode'
        },
        {
            name: 'actionTypeDescription',
            type: 'string',
            mapping: 'ActionTypeDescription'
        },
        {
            name: 'appealSubstitutionIndicator',
            type: 'string',
            mapping: 'AppealSubstitutionIndicator'
        },
        {
            name: 'decisionReviewOfficerElectedDate',
            type: 'date',
            mapping: 'DecisionReviewOfficerElectedDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'decisionReviewOfficerElectedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('decisionReviewOfficerElectedDate'))) {
                    return Ext.Date.format(record.get('decisionReviewOfficerElectedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'decisionReviewOfficerId',
            type: 'string',
            mapping: 'DecisionReviewOfficerId'
        },
        {
            name: 'decisionReviewOfficerReadyToRateIndicator',
            type: 'string',
            mapping: 'DecisionReviewOfficerReadyToRateIndicator'
        },
        {
            name: 'currentFileStoredLocationCode',
            type: 'string',
            mapping: 'CurrentFileStoredLocationCode'
        },
        {
            name: 'currentFileStoredLocationDescription',
            type: 'string',
            mapping: 'CurrentFileStoredLocationDescription'
        },
        {
            name: 'regionalOfficeCode',
            type: 'string',
            mapping: 'RegionalOfficeCode'
        },
        {
            name: 'regionalOfficeName',
            type: 'string',
            mapping: 'RegionalOfficeName'
        },
        {
            name: 'serviceOrganizationCode',
            type: 'string',
            mapping: 'ServiceOrganizationCode'
        },
        {
            name: 'serviceOrganizationDescription',
            type: 'string',
            mapping: 'ServiceOrganizationDescription'
        },
        {
            name: 'serviceOrganizationName',
            type: 'string',
            mapping: 'ServiceOrganizationName'
        },
        {
            name: 'serviceOrganizationReceivedDate',
            mapping: 'ServiceOrganizationReceivedDate',
            type: 'date',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'serviceOrganizationReceivedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('serviceOrganizationReceivedDate'))) {
                    return Ext.Date.format(record.get('serviceOrganizationReceivedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'decisionReviewOfficerPartialGrantOrDenialIndicator',
            type: 'string',
            mapping: 'DecisionReviewOfficerPartialGrantOrDenialIndicator'
        },
        {
            name: 'decisionReviewOfficerInformalHearingIndicator',
            type: 'string',
            mapping: 'DecisionReviewOfficerInformalHearingIndicator'
        },
        {
            name: 'decisionReviewOfficerFormalHearingIndicator',
            type: 'string',
            mapping: 'DecisionReviewOfficerFormalHearingIndicator'
        },
        {
            name: 'caseStorageShelfLocationDescription',
            type: 'string',
            mapping: 'CaseStorageShelfLocationDescription'
        },
        {
            name: 'chargeToCurrentLocationDate',
            type: 'date',
            mapping: 'ChargeToCurrentLocationDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'chargeToCurrentLocationDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('chargeToCurrentLocationDate'))) {
                    return Ext.Date.format(record.get('chargeToCurrentLocationDate'), 'm/d/Y');
                } else { return ''; }
            }
        }
    ],
    hasMany: [
        {
            model: 'VIP.model.appeals.Appellant',
            name: 'appellant',
            associationKey: 'Appellant',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.AppellantAddress',
            name: 'appellantAddress',
            associationKey: 'AppellantAddress',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.Veteran',
            name: 'veteran',
            associationKey: 'AppealVeteran',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.Issue',
            name: 'issues',
            associationKey: 'issues',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.Diary',
            name: 'diaries',
            associationKey: 'diaries',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.Decision',
            name: 'appealDecision',
            associationKey: 'AppealDecision',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.SpecialContentions',
            name: 'specialContentions',
            associationKey: 'specialContentions',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.Date',
            name: 'dates',
            associationKey: 'dates',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.appeals.HearingRequest',
            name: 'hearingRequest',
            associationKey: 'HearingRequest',
            storeConfig: {
                filters: []
            }
        }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'appeals',
            record: 'AppealRecord'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.vacols.appeals.GetAppeal',
            update: '',
            destroy: ''
        }
    }
});