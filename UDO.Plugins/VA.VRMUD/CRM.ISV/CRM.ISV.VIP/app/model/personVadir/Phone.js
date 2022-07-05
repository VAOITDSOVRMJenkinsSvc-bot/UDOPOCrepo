/**
* @class VIP.model.personVadir.Phone
*
* The model for phone associated with the person
*/
Ext.define('VIP.model.personVadir.Phone', {
    extend: 'Ext.data.Model',
    fields: [{
        name: 'phoneType',
        mapping: 'phoneType',
        type: 'string'
    }, {
        name: 'phoneNumber',
        mapping: 'phoneNumber',
        type: 'string'
    }, {
        name: 'phoneNumberFormatted',
        convert: function (v, record) {
            var Phone = record.get('phoneNumber');
            var ext = '';
            var result;

            if (0 != Phone.indexOf('+')) {
                if (1 < Phone.lastIndexOf('x')) {
                    ext = Phone.slice(Phone.lastIndexOf('x'));
                    Phone = Phone.slice(0, Phone.lastIndexOf('x'));
                }

                Phone = Phone.replace(/[^\d]/gi, '');
                result = Phone;
                if (7 == Phone.length) {
                    result = Phone.slice(0, 3) + '-' + Phone.slice(3)
                }
                if (10 == Phone.length) {
                    result = '(' + Phone.slice(0, 3) + ') ' + Phone.slice(3, 6) + '-' + Phone.slice(6);
                }
                if (0 < ext.length) {
                    result = result + ' ' + ext;
                }
                return result;
            }
        }
    }],

    //Start Proxy
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Phone'
        }
    }

});