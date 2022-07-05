/**
* @author Josh Oliver
* @class VIP.model.Claims
*
* The model for claims
*/
Ext.define('VIP.model.Claims', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.benefitclaim.FindBenefitClaim'
    ],
    fields: [
        {
            name: 'claimId',
            type: 'string',
            mapping: 'benefitClaimID'
        },
        {
            name: 'claimReceiveDate',
            type: 'date',
            dateFormat: 'm/d/Y', // format from ws - mm/dd/yyyy
            mapping: 'claimReceiveDate'
        },
        {
            name: 'claimReceiveDate_f',
            type: 'string',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('claimReceiveDate'))) {
                    return Ext.Date.format(record.get('claimReceiveDate'), "m/d/Y");
                }
            }
        }, {
            name: 'claimTypeCode',
            type: 'string'
        },
        {
            name: 'claimTypeName',
            type: 'string'
        },
        {
            name: 'claimantFirstName',
            type: 'string'
        },
        {
            name: 'claimantLastName',
            type: 'string'
        },
        {
            name: 'claimantMiddleName',
            type: 'string'
        },
        {
            name: 'claimantSuffix',
            type: 'string'
        },
        {
            name: 'endProductTypeCode',
            type: 'string'
        },
        {
            name: 'lastActionDate',
            type: 'date',
            dateFormat: 'm/d/Y', // format from ws - mm/dd/yyyy
            mapping: 'lastActionDate'
        },
        {
            name: 'lastActionDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('lastActionDate'))) {
                    return Ext.Date.format(record.get('lastActionDate'), "m/d/Y");
                }
                else {
                    return null;   
                }
            }
        },
        {
            name: 'organizationName',
            type: 'string'
        },
        {
            name: 'organizationTitleTypeName',
            type: 'string'
        },
        {
            name: 'payeeTypeCode',
            type: 'string'
        },
        {
            name: 'personOrOrganizationIndicator',
            type: 'string'
        },
        {
            name: 'programTypeCode',
            type: 'string'
        },
        {
            name: 'statusTypeCode',
            type: 'string'
        },
        {
            name: 'participantId'
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
            record: 'selection'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.benefitclaim.FindBenefitClaim',
            update: '',
            destroy: ''
        }
    }
});