/**
* @class VIP.model.Payment
*
* The model for paymentinformation payment
*/
Ext.define('VIP.model.Payment', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.share.paymentinfo.RetrievePaymentSummaryWithBDN'
    ],
    fields: [
        {
            name: 'paymentAmount',
            type: 'float',
            mapping: 'paymentAmount'
        },
        {
            name: 'paymentDate',
            convert: function (v) {
                return _dtZoneless(v);
            }
        },
        {
            name: 'authorizedDate',
            convert: function (v) {
                return _dtZoneless(v);
            }
        },
        {
            name: 'scheduledDate',
            convert: function (v) {
                return _dtZoneless(v);
            }
        },
        {
            name: 'paymentStatus',
            type: 'string',
            mapping: 'paymentStatus'
        },
        {
            name: 'paymentType',
            type: 'string',
            mapping: 'paymentType'
        },
        {
            name: 'programType',
            type: 'string',
            mapping: 'programType'
        },
        {
            name: 'paymentId',
            type: 'string',
            mapping: 'paymentID'
        },
        {
            name: 'transactionId',
            type: 'string',
            mapping: 'transactionID'
        },
        {
            name: 'recipientName',
            type: 'string',
            mapping: 'recipientName'
        },
        {
            name: 'recipientParticipantId',
            type: 'string',
            mapping: 'recipientParticipantID'
        },
        {
            name: 'veteranName',
            type: 'string',
            mapping: 'veteranName'
        },
        {
            name: 'veteranParticipantId',
            type: 'string',
            mapping: 'veteranParticipantID'
        },
        {
            name: 'addressLine1',
            type: 'string',
            mapping: 'addressLine1'
        },
        {
            name: 'addressLine2',
            type: 'string',
            mapping: 'addressLine2'
        },
        {
            name: 'addressLine3',
            type: 'string',
            mapping: 'addressLine3'
        },
        {
            name: 'addressLine4',
            type: 'string',
            mapping: 'addressLine4'
        },
        {
            name: 'addressLine5',
            type: 'string',
            mapping: 'addressLine5'
        },
        {
            name: 'addressLine6',
            type: 'string',
            mapping: 'addressLine6'
        },
        {
            name: 'addressLine7',
            type: 'string',
            mapping: 'addressLine7'
        },
        {
            name: 'accountType',
            type: 'string',
            mapping: 'accountType'
        },
        {
            name: 'routingNumberTemp',
            type: 'string',
            mapping: 'routingNumber'
        },
		{
		    name: 'routingNumber',
		    type: 'string',
		    convert: function (v, record) {
		        var routingNumber = record.get('routingNumberTemp');
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
		},
        {
            name: 'checkTraceNumber',
            type: 'string',
            mapping: 'checkTraceNumber'
        },
        {
            name: 'returnReason',
            type: 'string',
            mapping: 'returnReason'
        },
        {
            name: 'returnPayment',
            convert: function (v, record) {
                var traceNum = record.get('checkTraceNumber'),
                    reason = record.get('returnReason'),
                    returnPayment = '';

                if (!Ext.isEmpty(traceNum)) {
                    returnPayment = traceNum + ' - ' + reason;
                } else returnPayment = reason;

                return returnPayment;
            }
        },
        {
            name: 'paymentStatus2',
            convert: function (v, record) {
                payStat = record.get('paymentStatus');

                if (!Ext.isEmpty(record.get('bdnRecordType'))) {
                    payStat = 'Processed';
                }
                return payStat;
            }
        },
        {
            name: 'accountNumberTemp',
            type: 'string',
            mapping: 'accountNumber'
        },
		{
		    name: 'accountNumber',
		    type: 'string',
		    convert: function (v, record) {
		        var accountNumber = record.get('accountNumberTemp');
		        var routingNumber = record.get('routingNumberTemp');
		        var va_crn = parent.Xrm.Page.getAttribute('va_crn').getValue();

		        if (!Ext.isEmpty(routingNumber) && routingNumber == va_crn) {
		            accountNumber = '**********';
		        }

		        return accountNumber;
		    }
		},
        {
            name: 'bankName',
            type: 'string',
            mapping: 'bankName'
        },
        {
            name: 'payeeType',
            type: 'string',
            mapping: 'payeeType'
        },

    // ********** Starting computed fields
        {
            name: 'fullAddress',
            type: 'string',
            convert: function (v, record) {
                var address = null;

                if (!Ext.isEmpty(record.get('addressLine1'))) {
                    address = record.get('addressLine1') + " " +
                            (!Ext.isEmpty(record.get('addressLine2')) ? record.get('addressLine2') + " " : '') +
                            (!Ext.isEmpty(record.get('addressLine3')) ? record.get('addressLine3') + " " : '') +
                            (!Ext.isEmpty(record.get('addressLine4')) ? record.get('addressLine4') + " " : '') +
                            (!Ext.isEmpty(record.get('addressLine5')) ? record.get('addressLine5') + " " : '') +
                            (!Ext.isEmpty(record.get('addressLine6')) ? record.get('addressLine6') + " " : '') +
                            (!Ext.isEmpty(record.get('addressLine7')) ? record.get('addressLine7') : '');
                } else if (!Ext.isEmpty(record.get('accountNumber')) || !Ext.isEmpty(record.get('accountType')) ||
                        !Ext.isEmpty(record.get('bankName')) || !Ext.isEmpty(record.get('routingNumber'))) {
                    address =
                        (!Ext.isEmpty(record.get('accountNumber')) ? 'Acc. #: ' + record.get('accountNumber') + ' ** ' : '') +
                            (!Ext.isEmpty(record.get('accountType')) ? 'Acc. Type: ' + record.get('accountType') + ' ** ' : '') +
                            (!Ext.isEmpty(record.get('bankName')) ? 'Bank: ' + record.get('bankName') + ' ** ' : '') +
                            (!Ext.isEmpty(record.get('routingNumber')) ? 'Routing #: ' + record.get('routingNumber') : '');
                }

                return address;
            }
        },
        {
            name: 'bdnRecordType',
            type: 'string',
            mapping: 'bdnRecordType'
        },
        {
            name: 'programType2',
            convert: function (v, record) {
                var recordType = record.get('programType');

                if ((Ext.isEmpty(recordType)) && (!Ext.isEmpty(record.get('bdnRecordType')))) {
                    recordType = record.get('bdnRecordType');
                }
                return recordType;
            }
        },
        {
            name: 'beneficiaryName',
            type: 'string',
            mapping: 'beneficiaryName'
        },
        {
            name: 'recipientName2',
            convert: function (v, record) {
                var recip = record.get('recipientName');

                if ((Ext.isEmpty(recip)) && (!Ext.isEmpty(record.get('beneficiaryName')))) {
                    recip = record.get('beneficiaryName');
                }
                return recip;
            }
        },
        {
            name: 'beneficiaryParticipantId',
            type: 'string',
            mapping: 'beneficiaryParticipantID'
        },
        {
            name: 'fileNumber',
            type: 'string',
            mapping: 'fileNumber'
        },
        {   //Determine Which Date to Use Based off of Payment Status
            name: 'paymentSortDate',
            convert: function (v, record) {

                if (record.get('paymentDate') !== null) {
                    return record.get('paymentDate');
                }

                if (record.get('scheduledDate') !== null) {
                    return record.get('scheduledDate');
                }

                if (record.get('authorizedDate') !== null) {
                    return record.get('authorizedDate');
                }

                return null;
            }
        },
        {
            name: 'returnDate',
            convert: function (v, record) {
                var dt = record.get('returnDate');

                if (Ext.isEmpty(dt)) {
                    dt = new Date('01/01/2150');
                }

                return dt;
            }
        },
        {
            name: 'zipCode',
            type: 'string',
            mapping: 'zipCode'
        }
    ],
    idProperty: 'paymentId',
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'payment'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.share.paymentinfo.RetrievePaymentSummaryWithBDN',
            update: '',
            destroy: ''
        }
    }
});