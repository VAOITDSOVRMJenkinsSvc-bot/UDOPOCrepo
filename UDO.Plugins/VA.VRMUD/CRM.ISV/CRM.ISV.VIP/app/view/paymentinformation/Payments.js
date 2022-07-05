/**
* @class VIP.view.paymentinformation.Payments
*
* The view for the main awards grid
*/

Ext.define('VIP.view.paymentinformation.Payments', {
    extend: 'Ext.grid.Panel',
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    alias: 'widget.paymentinformation.payments',
    requires: [
        'VIP.view.PaymentDisposition',
        'VIP.view.PayeeCodes'
    ],
    listeners: {
        afterrender: function () {
            var me = this;

            if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
                //if (Ext.get('id_paymentinformation_Payments_01')) Ext.get('id_paymentinformation_Payments_01').hide();
                if (Ext.get('id_paymentinformation_Payments_02')) Ext.get('id_paymentinformation_Payments_02').hide();
                if (Ext.get('id_paymentinformation_Payments_03')) Ext.get('id_paymentinformation_Payments_03').hide();
                if (Ext.get('id_paymentinformation_Payments_04')) Ext.get('id_paymentinformation_Payments_04').hide();
                if (Ext.get('id_paymentinformation_Payments_05')) Ext.get('id_paymentinformation_Payments_05').hide();
                if (Ext.get('id_paymentinformation_Payments_06')) Ext.get('id_paymentinformation_Payments_06').hide();
                if (Ext.get('id_paymentinformation_Payments_07')) Ext.get('id_paymentinformation_Payments_07').hide();
                //if (Ext.get('id_paymentinformation_Payments_08')) Ext.get('id_paymentinformation_Payments_08').hide();
                //if (Ext.get('id_paymentinformation_Payments_09')) Ext.get('id_paymentinformation_Payments_09').hide();
                if (Ext.get('id_paymentinformation_Payments_10')) Ext.get('id_paymentinformation_Payments_10').hide();
                if (Ext.get('id_paymentinformation_Payments_11')) Ext.get('id_paymentinformation_Payments_11').hide();

                $("#id_paymentinformation_Payments_08").css('left', 140);
                $("#id_paymentinformation_Payments_09").css('left', 145);
            }
        }
    },
    initComponent: function () {
        var me = this;

        me.store = 'Payment';

        me.columns = {
            defaults: {
                sortable: false
            },
            items: [
                { header: 'Pay Date', dataIndex: 'paymentDate', xtype: 'datecolumn', format: 'm/d/Y', width: 80 },
                { header: 'Scheduled Date', dataIndex: 'scheduledDate', xtype: 'datecolumn', format: 'm/d/Y', width: 100 },
                { header: 'Authorized Date', dataIndex: 'authorizedDate', xtype: 'datecolumn', format: 'm/d/Y', width: 100 },
                { header: 'Amount', dataIndex: 'paymentAmount', xtype: 'numbercolumn', format: '$0,000.00', width: 80 },
                //{ header: 'Status', dataIndex: 'paymentStatus2', width: 80 },
                { header: 'Program Type', dataIndex: 'programType2', width: 110 },
                { header: 'Acc. No', dataIndex: 'accountNumber', width: 80 },
                { header: 'Acc. Type', dataIndex: 'accountType', width: 80 },
                { header: 'Bank Name', dataIndex: 'bankName', width: 150 },
                { header: 'Routing Nbr', dataIndex: 'routingNumber', width: 80 },
                { header: 'Recipient', dataIndex: 'recipientName2', width: 100 },
                { header: 'Payment Type', dataIndex: 'paymentType', width: 120 },
                { header: 'Payee Type', dataIndex: 'payeeType', width: 80 },
                { header: 'Return Payment', dataIndex: 'returnPayment', width: 100 },
                { header: 'Payment ID', dataIndex: 'paymentId', width: 80 },
                { header: 'Transaction ID', dataIndex: 'transactionId', width: 80 }
            ]
        };

        me.dockedItems = [
            {
                xtype: 'toolbar',
                items: [{
                    xtype: 'paymentdisposition',
                    id: 'id_paymentinformation_Payments_01'
                }, {
                    xtype: 'tbseparator',
                    id: 'id_paymentinformation_Payments_02'
                }, {
                    xtype: 'button',
                    text: 'Education Cutoff Dates',
                    tooltip: 'Opens a new window with the education cutoff dates website',
                    iconCls: 'icon-request',
                    action: 'showeducationcutoff',
                    id: 'id_paymentinformation_Payments_03'
                }, {
                    xtype: 'tbseparator',
                    id: 'id_paymentinformation_Payments_04'
                }, {
                    xtype: 'button',
                    text: 'Payment Script',
                    tooltip: 'Opens up the Payments script.',
                    iconCls: 'icon-script',
                    action: 'showpaymentscript',
                    id: 'id_paymentinformation_Payments_05'
                }, {
                    xtype: 'tbseparator',
                    id: 'id_paymentinformation_Payments_06'
                }, {
                    xtype: 'button',
                    text: 'Script for C&P',
                    width: 100,
                    tooltip: 'Opens up the Script for C&P.',
                    iconCls: 'icon-script',
                    action: 'showcandpscript',
                    id: 'id_paymentinformation_Payments_07'
                }, {
                    xtype: 'tbseparator',
                    id: 'id_paymentinformation_Payments_08'
                }, {
                    xtype: 'payeecodes',
                    text: 'Payee Codes',
                    tooltip: '',
                    action: 'filterpaymentsbypayeecode',
                    id: 'id_paymentinformation_Payments_09'
                }, {
                    xtype: 'tbfill',
                    id: 'id_paymentinformation_Payments_10'
                }, {
                    xtype: 'tbtext',
                    notificationType: 'paymentinformationcount',
                    text: 'Payments: 0',
                    id: 'id_paymentinformation_Payments_11'
                }]
            }
        ];

        me.callParent();
    }
});
