/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.ServiceDiagnostics
*
* The view for ServiceDiagnostics associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.ServiceDiagnostics', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.birls.birlsdetails.servicediagnostics',
    title: 'Service Diagnostics',

    requires: ['VIP.view.birls.birlsdetails.servicediagnostics.DiagnosticList', 'VIP.view.birls.birlsdetails.servicediagnostics.AdditionalInfo'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'birls.birlsdetails.servicediagnostics.diagnosticlist',
            flex: 3
        }, {
            xtype: 'birls.birlsdetails.servicediagnostics.additionalinfo',
            flex: 2
        }];

        me.callParent();
    }
});