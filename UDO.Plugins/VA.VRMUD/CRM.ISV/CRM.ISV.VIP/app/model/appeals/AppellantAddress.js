/**
* @author Josh Oliver
* @class VIP.model.appeals.AppellantAddress
*
* The model for appeal appellant address
*/
Ext.define('VIP.model.appeals.AppellantAddress', {
    extend: 'Ext.data.Model',
    fields: [
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
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'AppellantAddress'
        }
    }
});