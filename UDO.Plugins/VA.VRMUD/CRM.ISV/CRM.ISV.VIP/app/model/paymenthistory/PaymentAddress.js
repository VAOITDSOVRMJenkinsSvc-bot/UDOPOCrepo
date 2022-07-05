/**
* @author Ivan Yurisevic
* @class VIP.model.paymenthistory.PaymentAddress
*
* The model for old payment history service
*/
Ext.define('VIP.model.paymenthistory.PaymentAddress', {
    extend: 'Ext.data.Model',
    requires: ['VIP.soap.envelopes.share.paymenthistory.FindPayHistoryBySSN'],
    fields: [{
        name: 'addressAccountNumTemp',
        type: 'string',
        mapping: 'addressAccntNum'
    },
    {
        name: 'addressAccountNum',
        type: 'string',
        convert: function (v, record) {
            var accountNumber = record.get('addressAccountNumTemp');
            var routingNumber = record.get('addressZipCodeTemp');
            var va_crn = parent.Xrm.Page.getAttribute('va_crn').getValue();

            if (!Ext.isEmpty(routingNumber) && routingNumber == va_crn) {
                accountNumber = '**********';
            }

            return accountNumber;
        }
    },
    {
        name: 'addressAccountType',
        type: 'string',
        mapping: 'addressAccntType'
    }, {
        name: 'addressId',
        type: 'string',
        mapping: 'addressID'
    }, {
        name: 'addressLine1Temp',
        type: 'string',
        mapping: 'addressLine1'
    },
    {
        name: 'addressLine1',
        type: 'string',
        convert: function (v, record) {
            var addressLine1 = record.get('addressLine1Temp');
            var routingNumber = record.get('addressZipCodeTemp');
            var va_crn = parent.Xrm.Page.getAttribute('va_crn').getValue();

            if (!Ext.isEmpty(routingNumber) && routingNumber == va_crn) {
                addressLine1 = '**********';
            }

            return addressLine1;
        }
    }, {
        name: 'addressLine2',
        type: 'string',
        mapping: 'addressLine2'
    }, {
        name: 'addressLine3',
        type: 'string',
        mapping: 'addressLine3'
    }, {
        name: 'addressLine4',
        type: 'string',
        mapping: 'addressLine4'
    }, {
        name: 'addressLine5',
        type: 'string',
        mapping: 'addressLine5'
    }, {
        name: 'addressLine6',
        type: 'string',
        mapping: 'addressLine6'
    }, {
        name: 'addressLine7',
        type: 'string',
        mapping: 'addressLine7'
    }, {
        name: 'addressPayMethod',
        type: 'string',
        mapping: 'addressPayMethod'
    }, {
        name: 'addressRO',
        type: 'string',
        mapping: 'addressRO'
    }, {
        name: 'addressRoutingNum',
        type: 'string',
        mapping: 'addressRoutingNum'
    }, {
        name: 'addressZipCodeTemp',
        type: 'string',
        mapping: 'addressZipCode'
    },
    {
        name: 'addressZipCode',
        type: 'string',
        convert: function (v, record) {
            var routingNumber = record.get('addressZipCodeTemp');
            var va_crn = parent.Xrm.Page.getAttribute('va_crn').getValue();

            if (!Ext.isEmpty(routingNumber) && routingNumber == va_crn) {
                var len = routingNumber.length - 4;
                var masked = '';
                for (i = 0; i < len; i++) {
                    masked += '*';
                }
                masked += routingNumber.substr(len, 4);
                routingNumber = masked;
            }

            return routingNumber;
        }
    }],

    //belongsTo: 'VIP.model.paymenthistory.Payments',

    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'paymentAddress'
        }
    }
});