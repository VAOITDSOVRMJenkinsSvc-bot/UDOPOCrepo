/**
 * @author Ivan Yurisevic
 * @class VIP.view.paymenthistory.payments.paymentdata.PaymentAddress
 *
 * Tab panel for payment address for old payment history
 */
Ext.define('VIP.view.paymenthistory.payments.paymentdata.PaymentAddress', {
    extend: 'Ext.form.Panel',
    alias: 'widget.paymenthistory.payments.paymentdata.paymentaddress',
    title: 'Payment Address (Select Payment From Grid to Populate)',
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
        labelWidth: 120,
        width: 300
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            name: 'addressLine1',
            fieldLabel: 'Address 1'
        }, {
            xtype: 'displayfield',
            name: 'addressAccountNum',
            fieldLabel: 'Account #'
        }, {
            xtype: 'displayfield',
            name: 'addressZipCode',
            fieldLabel: 'Routing Code'
        }, {
            xtype: 'displayfield',
            name: 'addressLine2',
            fieldLabel: 'Address 2'
        }, {
            xtype: 'displayfield',
            name: 'addressAccountType',
            fieldLabel: 'Account Type'
        }, {
            xtype: 'displayfield',
            name: 'addressRoutingNum',
            fieldLabel: 'Routing #'
        }, {
            xtype: 'displayfield',
            name: 'addressLine3',
            fieldLabel: 'Address 3'
        }, {
            xtype: 'displayfield',
            name: 'addressPayMethod',
            fieldLabel: 'Pay Method'
        }, {
            xtype: 'displayfield',
            name: 'addressRO',
            fieldLabel: 'RO'
        }, {
            xtype: 'displayfield',
            name: 'addressLine4',
            fieldLabel: 'Address 4'
        }];
        //                {//NOT PART OF PAYMENT ADDRESS, PART OF RELATED PAYMENT
        //                    xtype: 'displayfield',
        //                    name: 'payCheckDtFormatted', 
        //                    fieldLabel: 'Pay Date'
        //                },
        //                {//NOT PART OF PAYMENT ADDRESS, PART OF RELATED PAYMENT
        //                    xtype: 'displayfield',
        //                    name: 'payCheckID',
        //                    fieldLabel: 'Check Id'
        //                },
        //                {
        //                    xtype: 'displayfield',
        //                    fieldLabel: '',
        //                    colspan: 3
        //                },
        //				{//NOT PART OF PAYMENT ADDRESS, PART OF RELATED PAYMENT
        //				    xtype: 'displayfield',
        //				    name: 'payCheckAmount', 
        //				    fieldLabel: 'Amount'
        //				},];

        me.callParent();
    }
});