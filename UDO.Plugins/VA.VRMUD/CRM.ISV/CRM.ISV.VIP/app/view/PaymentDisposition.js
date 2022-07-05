/**
* @author Josh Oliver
* @class VIP.view.PaymentDisposition
*
* The component for payment disposition
*/
Ext.define('VIP.view.PaymentDisposition', {
    extend: 'Ext.button.Split',
    alias: 'widget.paymentdisposition',
    text: 'Payment Disposition',
    tooltip: 'Set payment dispoistion of phone call',
    iconCls: 'icon-script',
    menu: {
        xtype: 'menu',
        defaults: {
            iconCls: 'icon-script'
        },
        items: [
            { value: '953850070', text: 'Address Change / Account Suspended' },
            { value: '953850004', text: 'Amount of payment' },
            { value: '953850090', text: 'COLA (Cost of Living Adjustment)' },
            { value: '953850018', text: 'DMC' },
            { value: '953850092', text: 'Date of Payment' },
            { value: '953850084', text: 'General Status' },
            { value: '953850036', text: 'Go Direct Master Cards' },
            { value: '953850041', text: 'Incorrect check amount' },
            { value: '953850089', text: 'Medical Center Debts' },
            { value: '953850054', text: 'Non Receipt of Check' },
            { value: '953850062', text: 'Payment Deductions' },
            { value: '953850091', text: 'Payment Lost/Stolen' },
            { value: '953850075', text: 'TRACER' }
        ]
    }
});