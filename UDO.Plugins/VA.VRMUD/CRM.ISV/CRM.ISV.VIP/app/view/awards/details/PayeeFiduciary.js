/**
* @author Jonas Dawson
* @class VIP.view.awards.details.PayeeFiduciary
*
* The view for person general detail information
*/
Ext.define('VIP.view.awards.details.PayeeFiduciary', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.awards.details.payeefiduciary',
    title: 'Payee/Fiduciary',
    requires: ['VIP.view.awards.details.payeefiduciary.PayeeInformation', 'VIP.view.awards.details.payeefiduciary.FiduciaryInformation'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'awards.details.payeefiduciary.payeeinformation',
            flex: 1
        }, {
            xtype: 'awards.details.payeefiduciary.fiduciaryinformation',
            flex: 1
        }];

        me.callParent();
    }
});