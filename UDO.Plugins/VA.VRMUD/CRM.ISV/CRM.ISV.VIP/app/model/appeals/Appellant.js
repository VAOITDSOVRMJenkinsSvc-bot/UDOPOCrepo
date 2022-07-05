/**
* @author Josh Oliver
* @class VIP.model.appeals.Appellant
*
* The model for appeal appellant
*/
Ext.define('VIP.model.appeals.Appellant', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'appellantRelationshipToVeteranCode',
            type: 'string',
            mapping: 'AppellantRelationshipToVeteranCode'
        },
        {
            name: 'appellantRelationshipToVeteranDescription',
            type: 'string',
            mapping: 'AppellantRelationshipToVeteranDescription'
        },
        {
            name: 'appellantFirstName',
            type: 'string',
            mapping: 'AppellantFirstName'
        },
        {
            name: 'appellantMiddleInitial',
            type: 'string',
            mapping: 'AppellantMiddleInitial'
        },
        {
            name: 'appellantLastName',
            type: 'string',
            mapping: 'AppellantLastName'
        },
        {
            name: 'appellantTitle',
            type: 'string',
            mapping: 'AppellantTitle'
        },
        {
            name: 'appellantNameSuffix',
            type: 'string',
            mapping: 'AppellantNameSuffix'
        },
        {
            name: 'appellantFullName',
            convert: function (v, record) {
                var name = '';
                if (!Ext.isEmpty(record.get('appellantLastName'))) {
                    name += record.get('appellantLastName');
                    if (!Ext.isEmpty(record.get('appellantFirstName'))) {
                        name += ', ' + record.get('appellantFirstName');
                    }
                    if (!Ext.isEmpty(record.get('appellantMiddleInitial'))) {
                        var mi = record.get('appellantMiddleInitial');
                        name += ' ' + mi;
                        if (mi.length == 1) { name += '.'; }
                    }
                }
                return name;
            }
        }, //Addition of Appellant Address Fields to Appellant Model.
        {
            name: 'addressKey',
            type: 'string',
            mapping: 'AddressKey'
        },
        {
            name: 'appellantAddressLine1',
            type: 'string',
            mapping: 'AppellantAddressLine1'
        },
        {
            name: 'appellantAddressLine2',
            type: 'string',
            mapping: 'AppellantAddressLine2'
        },
        {
            name: 'appellantAddressCityName',
            type: 'string',
            mapping: 'AppellantAddressCityName'
        },
        {
            name: 'appellantAddressStateCode',
            type: 'string',
            mapping: 'AppellantAddressStateCode'
        },
        {
            name: 'appellantAddressZipCode',
            type: 'string',
            mapping: 'AppellantAddressZipCode'
        },
        {
            name: 'appellantAddressCountryName',
            type: 'string',
            mapping: 'AppellantAddressCountryName'
        },
        {
            name: 'appellantAddressLastModifiedByROName',
            type: 'string',
            mapping: 'AppellantAddressLastModifiedByROName'
        },
        {
            name: 'appellantAddressLastModifiedDate',
            type: 'date',
            mapping: 'AppellantAddressLastModifiedDate',
            dateFormat: 'Y-m-d H:i:s.u'
        },
        {
            name: 'appellantAddressLastModifiedDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('appellantAddressLastModifiedDate'))) {
                    return Ext.Date.format(record.get('appellantAddressLastModifiedDate'), 'm/d/Y');
                } else { return ''; }
            }
        },
        {
            name: 'appellantAddressNotes',
            type: 'string',
            mapping: 'AppellantAddressNotes'
        },
        {
            name: 'appellantWorkPhone',
            type: 'string',
            mapping: 'AppellantWorkPhoneNumber'
        },
        {
            name: 'appellantHomePhone',
            type: 'string',
            mapping: 'AppellantHomePhoneNumber'
        },
        { //Addition of Vet Info to Apellant Model
            name: 'birthDate',
            type: 'string',
            mapping: 'BirthDate'
        },
        {
            name: 'veteranFirstName',
            type: 'string',
            mapping: 'VeteranFirstName'
        },
        {
            name: 'veteranMiddleInitial',
            type: 'string',
            mapping: 'VeteranMiddleInitial'
        },
        {
            name: 'veteranGender',
            type: 'string',
            mapping: 'VeteranGender'
        },
        {
            name: 'veteranLastName',
            type: 'string',
            mapping: 'VeteranLastName'
        },
        {
            name: 'veteranFullName',
            convert: function (v, record) {
                var name = '';
                if (!Ext.isEmpty(record.get('veteranLastName'))) {
                    name += record.get('veteranLastName');
                    if (!Ext.isEmpty(record.get('veteranFirstName'))) {
                        name += ', ' + record.get('veteranFirstName');
                    }
                    if (!Ext.isEmpty(record.get('veteranMiddleInitial'))) {
                        var mi = record.get('veteranMiddleInitial');
                        name += ' ' + mi;
                        if (mi.length == 1) { name += '.'; }
                    }
                }
                return name;
            }
        },
        {
            name: 'veteranSsn',
            type: 'string',
            mapping: 'VeteranSSN'
        },
        {
            name: 'finalNoticeOfDeathDate',
            type: 'date',
            mapping: 'FinalNoticeOfDeathDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'finalNoticeOfDeathDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('finalNoticeOfDeathDate'))) {
                    return Ext.Date.format(record.get('finalNoticeOfDeathDate'), 'm/d/Y');
                } else { return ''; }
            }
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Appellant'
        }
    }
});