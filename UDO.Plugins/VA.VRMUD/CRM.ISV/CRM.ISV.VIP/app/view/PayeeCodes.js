/**
* @class VIP.view.
*/
Ext.define('VIP.view.PayeeCodes', {
    extend: 'Ext.button.Split',
    alias: 'widget.payeecodes',
    text: 'Payee Codes',
    tooltip: 'Select a payee code.',
    iconCls: 'icon-request',
    
    defaultMenuSelection: { text: '00' }
});