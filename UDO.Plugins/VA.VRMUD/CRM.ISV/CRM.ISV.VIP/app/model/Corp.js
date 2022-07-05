/**
* @author Ivan Yurisevic
* @class VIP.model.Corp
*
* The model for person corporate detail information
*/

Ext.define('VIP.model.Corp', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.vetrecord.FindCorporateRecord'
    ],
    fields: [{
        name: 'ssn',
        mapping: 'ssn',
        type: 'string'
    }, {
        name: 'firstName',
        mapping: 'firstName',
        type: 'string'
    }, {
        name: 'lastName',
        mapping: 'lastName',
        type: 'string'
    }, {
        name: 'middleName',
        mapping: 'middleName',
        type: 'string'
    }, {
        name: 'participantId',
        mapping: 'ptcpntId',
        type: 'string'
    }, {
        name: 'branchOfService1',
        mapping: 'branchOfService1',
        type: 'string'
    }, {
        name: 'dob',
        mapping: 'dateOfBirth',
        type: 'string'
    }, {
        name: 'dod',
        mapping: 'dateOfDeath',
        type: 'string'
    }, {
        name: 'suffixName',
        mapping: 'suffixName',
        type: 'string'
    }, {
        name: 'addressLine1',
        mapping: 'addressLine1',
        type: 'string'
    }, {
        name: 'addressLine2',
        mapping: 'addressLine2',
        type: 'string'
    }, {
        name: 'addressLine3',
        mapping: 'addressLine3',
        type: 'string'
    }, {
        name: 'areaNumber1',
        mapping: 'areaNumberOne',
        type: 'string'
    }, {
        name: 'areaNumber2',
        mapping: 'areaNumberTwo',
        type: 'string'
    }, {
        name: 'city',
        mapping: 'city',
        type: 'string'
    }, {
        name: 'competencyDecisionTypeCode',
        mapping: 'competencyDecisionTypeCode',
        type: 'string'
    }, {
        name: 'country',
        mapping: 'country',
        type: 'string'
    }, {
        name: 'cpPaymentAddressLine1',
        mapping: 'cpPaymentAddressLine1',
        type: 'string'
    }, {
        name: 'cpPaymentAddressLine2',
        mapping: 'cpPaymentAddressLine2',
        type: 'string'
    }, {
        name: 'cpPaymentAddressLine3',
        mapping: 'cpPaymentAddressLine3',
        type: 'string'
    }, {
        name: 'cpPaymentCity',
        mapping: 'cpPaymentCity',
        type: 'string'
    }, {
        name: 'cpPaymentCountry',
        mapping: 'cpPaymentCountry',
        type: 'string'
    }, {
        name: 'cpPaymentForeignZip',
        mapping: 'cpPaymentForeignZip',
        type: 'string'
    }, {
        name: 'cpPaymentPostOfficeTypeCode',
        mapping: 'cpPaymentPostOfficeTypeCode',
        type: 'string'
    }, {
        name: 'cpPaymentPostalTypeCode',
        mapping: 'cpPaymentPostalTypeCode',
        type: 'string'
    }, {
        name: 'cpPaymentState',
        mapping: 'cpPaymentState',
        type: 'string'
    }, {
        name: 'cpPaymentZipCode',
        mapping: 'cpPaymentZipCode',
        type: 'string'
    }, {
        name: 'eftAccountNumber',
        mapping: 'eftAccountNumber',
        type: 'string'
    }, {
        name: 'eftAccountType',
        mapping: 'eftAccountType',
        type: 'string'
    }, {
        name: 'eftRoutingNumber',
        mapping: 'eftRoutingNumber',
        type: 'string'
    }, {
        name: 'emailAddress',
        mapping: 'emailAddress',
        type: 'string'
    }, {
        name: 'fiduciaryDecisionCategoryTypeCode',
        mapping: 'fiduciaryDecisionCategoryTypeCode',
        type: 'string'
    }, {
        name: 'fiduciaryFolderLocation',
        mapping: 'fiduciaryFolderLocation',
        type: 'string'
    }, {
        name: 'fileNumber',
        mapping: 'fileNumber',
        type: 'string'
    }, {
        name: 'foreignCode',
        mapping: 'foreignCode',
        type: 'string'
    }, {
        name: 'militaryPostOfficeTypeCode',
        mapping: 'militaryPostOfficeTypeCode',
        type: 'string'
    }, {
        name: 'militaryPostalTypeCode',
        mapping: 'militaryPostalTypeCode',
        type: 'string'
    }, {
        name: 'orgName',
        mapping: 'orgName',
        type: 'string'
    }, {
        name: 'orgTitle',
        mapping: 'orgTitle',
        type: 'string'
    }, {
        name: 'orgType',
        mapping: 'orgType',
        type: 'string'
    }, {
        name: 'phoneNumberOne',
        mapping: 'phoneNumberOne',
        type: 'string'
    }, {
        name: 'phoneNumberTwo',
        mapping: 'phoneNumberTwo',
        type: 'string'
    }, {
        name: 'phoneTypeNameOne',
        mapping: 'phoneTypeNameOne',
        type: 'string'
    }, {
        name: 'phoneTypeNameTwo',
        mapping: 'phoneTypeNameTwo',
        type: 'string'
    }, {
        name: 'prepPhraseType',
        mapping: 'prepPhraseType',
        type: 'string'
    }, {
        name: 'provinceName',
        mapping: 'provinceName',
        type: 'string'
    }, {
        name: 'participantRelationship',
        mapping: 'ptcpntRelationship',
        type: 'string'
    }, {
        name: 'salutationName',
        mapping: 'salutationName',
        type: 'string'
    }, {
        name: 'state',
        mapping: 'state',
        type: 'string'
    }, {
        name: 'temporaryCustodianInd',
        mapping: 'temporaryCustodianIndicator',
        type: 'string'
    }, {
        name: 'territoryName',
        mapping: 'territoryName',
        type: 'string'
    }, {
        name: 'treasuryMailingAddressLine1',
        mapping: 'treasuryMailingAddressLine1',
        type: 'string'
    }, {
        name: 'treasuryMailingAddressLine2',
        mapping: 'treasuryMailingAddressLine2',
        type: 'string'
    }, {
        name: 'treasuryMailingAddressLine3',
        mapping: 'treasuryMailingAddressLine3',
        type: 'string'
    }, {
        name: 'treasuryMailingAddressLine4',
        mapping: 'treasuryMailingAddressLine4',
        type: 'string'
    }, {
        name: 'treasuryMailingAddressLine5',
        mapping: 'treasuryMailingAddressLine5',
        type: 'string'
    }, {
        name: 'treasuryMailingAddressLine6',
        mapping: 'treasuryMailingAddressLine6',
        type: 'string'
    }, {
        name: 'treasuryPaymentAddressLine1',
        mapping: 'treasuryPaymentAddressLine1',
        type: 'string'
    }, {
        name: 'treasuryPaymentAddressLine2',
        mapping: 'treasuryPaymentAddressLine2',
        type: 'string'
    }, {
        name: 'treasuryPaymentAddressLine3',
        mapping: 'treasuryPaymentAddressLine3',
        type: 'string'
    }, {
        name: 'treasuryPaymentAddressLine4',
        mapping: 'treasuryPaymentAddressLine4',
        type: 'string'
    }, {
        name: 'treasuryPaymentAddressLine5',
        mapping: 'treasuryPaymentAddressLine5',
        type: 'string'
    }, {
        name: 'treasuryPaymentAddressLine6',
        mapping: 'treasuryPaymentAddressLine6',
        type: 'string'
    }, {
        name: 'zipCode',
        mapping: 'zipCode',
        type: 'string'
    },

    {
        name: 'fullName',
        convert: function (v, record) {
            var firstName = record.get('firstName'),
                middleName = record.get('middleName'),
                lastName = record.get('lastName'),
                fullName = '';

            fullName += Ext.isEmpty(firstName) ? '' : firstName + ' ';
            fullName += Ext.isEmpty(middleName) ? '' : middleName + ' ';
            fullName += Ext.isEmpty(lastName) ? '' : lastName;

            return fullName;

        }
    },
    {
        name: 'fullPhone1',
        convert: function (v, record) {
            var areaCode = record.get('areaNumber1'),
                phoneNumber = record.get('phoneNumberOne'),
                fullPhone = ''; ;

            if (!Ext.isEmpty(phoneNumber) && !Ext.isEmpty(phoneNumber)) {
                fullPhone = '(' + areaCode + ') ' + phoneNumber.substring(0, 3) + '-' + phoneNumber.substring(3);
                return fullPhone;
            } else return areaCode + ' ' + phoneNumber;
        }
    },
    {
        name: 'fullPhone2',
        convert: function (v, record) {
            var areaCode = record.get('areaNumber2'),
                phoneNumber = record.get('phoneNumberTwo'),
                fullPhone = '';

            if (!Ext.isEmpty(phoneNumber) && !Ext.isEmpty(phoneNumber)) {
                fullPhone = '(' + areaCode + ') ' + phoneNumber.substring(0, 3) + '-' + phoneNumber.substring(3);
                return fullPhone;
            } else return areaCode + ' ' + phoneNumber;
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
            record: 'return'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.vetrecord.FindCorporateRecord', //This is the default.
            update: '',                                                     //Also set at runtime in the Corp store in its beforeload event
            destroy: ''
        }
    }
});
