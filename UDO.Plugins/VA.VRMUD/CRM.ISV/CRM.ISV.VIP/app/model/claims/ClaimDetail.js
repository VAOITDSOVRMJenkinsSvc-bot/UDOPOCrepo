/**
* @author Josh Oliver
* @class VIP.model.claims.ClaimDetail
*
* The model for claims
*/
Ext.define('VIP.model.claims.ClaimDetail', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.benefitclaim.FindBenefitClaimDetail',
        'VIP.model.claims.LifeCycle',
        'VIP.model.claims.Suspense'
    ],
    fields: [
        {
            name: 'siteName',
            type: 'string',
            mapping: 'bddSiteName'
        }, {
            name: 'claimId',
            type: 'string',
            mapping: 'benefitClaimID',
            ignoreMappingOnRequest: true
        }, {
            name: 'returnLabel',
            type: 'string',
            mapping: 'benefitClaimReturnLabel'
        }, {
            name: 'priorityIndicator',
            type: 'string',
            mapping: 'claimPriorityIndicator'
        }, {
            name: 'claimReceiveDate',
            type: 'date',
            dateFormat: 'm/d/Y',
            mapping: 'claimReceiveDate'
        },
        {
            name: 'claimReceiveDate_f',
            type: 'string',
            mapping: 'claimReceiveDate'
        }, 
        {
            name: 'soj',
            type: 'string',
            mapping: 'claimStationOfJurisdiction'
        }, {
            name: 'tempSoj',
            type: 'string',
            mapping: 'claimTemporaryStationOfJurisdiction'
        }, {
            name: 'claimTypeCode',
            type: 'string'
        }, {
            name: 'claimTypeName',
            type: 'string'
        }, {
            name: 'claimantFirstName',
            type: 'string'
        }, {
            name: 'claimantLastName',
            type: 'string'
        }, {
            name: 'claimantMiddleName',
            type: 'string'
        }, {
            name: 'claimantPersonOrOrganizationIndicator',
            type: 'string'
        }, {
            name: 'claimantSuffix',
            type: 'string'
        }, {
            name: 'cpBenefitClaimId',
            type: 'string',
            mapping: 'cpBenefitClaimID'
        }, {
            name: 'cpClaimId',
            type: 'string',
            mapping: 'cpClaimID'
        }, {
            name: 'cpClaimReturnLabel',
            type: 'string'
        }, {
            name: 'cpLocationId',
            type: 'string',
            mapping: 'cpLocationID'
        }, {
            name: 'directDepositAccountId',
            type: 'string',
            mapping: 'directDepositAccountID'
        }, {
            name: 'endProductTypeCode',
            type: 'string'
        }, {
            name: 'informalIndicator',
            type: 'string'
        }, {
            name: 'journalDate',
            type: 'string'
        }, {
            name: 'journalObjectId',
            type: 'string',
            mapping: 'journalObjectID'
        }, {
            name: 'journalStation',
            type: 'string'
        }, {
            name: 'journalStatusTypeCode',
            type: 'string'
        }, {
            name: 'journalUserId',
            type: 'string'
        }, {
            name: 'lastPaidDate',
            type: 'string'
        }, {
            name: 'mailingAddressId',
            type: 'string',
            mapping: 'mailingAddressID'
        }, {
            name: 'numberOfBenefitClaimRecords',
            type: 'string'
        }, {
            name: 'numberOfCPClaimRecords',
            type: 'string'
        }, {
            name: 'organizationName',
            type: 'string'
        }, {
            name: 'organizationTitleTypeName',
            type: 'string'
        }, {
            name: 'participantClaimantID',
            type: 'string',
            mapping: 'participantClaimantID'
        }, {
            name: 'participantVetID',
            type: 'string',
            mapping: 'participantVetID'
        }, {
            name: 'payeeTypeCode',
            type: 'string'
        }, {
            name: 'paymentAddressID',
            type: 'string',
            mapping: 'paymentAddressID'
        }, {
            name: 'programTypeCode',
            type: 'string'
        }, {
            name: 'serviceTypeCode',
            type: 'string'
        }, {
            name: 'statusTypeCode',
            type: 'string'
        }, {
            name: 'vetFirstName',
            type: 'string'
        }, {
            name: 'vetLastName',
            type: 'string'
        }, {
            name: 'vetMiddleName',
            type: 'string'
        }, {
            name: 'vetSuffix',
            type: 'string'
        },
        {
            name: 'daysSinceInception',
            convert: function (v, record) {
                if (!Ext.isEmpty(record)) {
                    return record.calculateClaimDays(record.get('claimReceiveDate'));
                } else { return 'N/A'; }
            }
        },
        {
            name: 'daysPending',
            convert: function (v, record) {
                if (!Ext.isEmpty(record) && record.get('statusTypeCode') == 'PEND') {
                    return record.calculateClaimDays(record.get('claimReceiveDate'));
                } else { return 'N/A'; }
            }
        },
        {
            name: 'reasonText',
            convert: function (v, record) {
                var lcrs = record.lifeCycleRecords();
                return 'N/A';
            }
        }
    ],
    hasMany: [        
        {
            model: 'VIP.model.claims.LifeCycle',
            name: 'lifeCycleRecords',
            associationKey: 'lifeCycleRecord',
            storeConfig: {
                filters: []
            }
        },
        {
            model: 'VIP.model.claims.Suspense',
            name: 'suspenseRecords',
            associationKey: 'suspenceRecord',
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
            type: 'xml',
            record: 'benefitClaimRecord'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.benefitclaim.FindBenefitClaimDetail',
            update: '',
            destroy: ''
        }
    },

    calculateClaimDays: function (claimStatusDate) {
        if (!Ext.isEmpty(claimStatusDate)) {
            var today = new Date();
            var one_day = 1000 * 60 * 60 * 24;
            var dateOfClaim = new Date(claimStatusDate);
            return Math.ceil((today.getTime() - dateOfClaim.getTime()) / (one_day));
        } else { return 'N/A'; }
    }
});