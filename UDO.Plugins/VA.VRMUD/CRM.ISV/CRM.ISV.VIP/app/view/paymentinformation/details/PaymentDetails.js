/**
* @author Josh Oliver
* @class VIP.view.paymentinformation.details.PaymentDetails
*
* The view for paymentinformation payment details
*/
Ext.define('VIP.view.paymentinformation.details.PaymentDetails', {
    extend: 'Ext.form.Panel',
    alias: 'widget.paymentinformation.details.paymentdetails',
    title: 'Payment Details',
    layout: {
        type: 'table',
        columns: 3,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120
        , width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [ {
            xtype: 'displayfield',
            fieldLabel: 'Gross Award Amt',
            name: 'grossAwardAmount', // TODO: format grossAwardAmount
            allowBlank: true
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Net Award Amt',
            name: 'netAwardAmount', // TODO: format netAwardAmount
            allowBlank: true
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Award Effective Date',
            name: 'awardEffectiveDate_F', // TODO: format awardEffectiveDate
            allowBlank: true
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Gross Payment Amt',
            name: 'grossPaymentAmount', // TODO: format grossPaymentAmount
            allowBlank: true
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Net Payment Amt',
            name: 'netPaymentAmount', // TODO: format netPaymentAmount
            allowBlank: true,
            colspan: 2
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Address',
            name: 'fullAddress',
            allowBlank: true,
            action: 'address',
            colspan: 3,
            width: 800
        }];

        me.callParent();
    }
});