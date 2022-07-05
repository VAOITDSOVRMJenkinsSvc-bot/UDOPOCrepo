/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.servicediagnostics.DiagnosticList
*
* The view for ServiceDiagnostics associated with the person
*/

Ext.define('VIP.view.birls.birlsdetails.servicediagnostics.DiagnosticList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.birls.birlsdetails.servicediagnostics.diagnosticlist',
    store: 'birls.ServiceDiagnostics',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Diagnostic Code ', dataIndex: 'serviceDiagnostics' },
                { header: 'Percentage', dataIndex: 'servicePercent1'},
                { header: 'Percentage 2', dataIndex: 'servicePercent2' },
                { header: 'Analogous Code', dataIndex: 'recurringAnalogusCode' },
                { header: 'Service Connected Disability', dataIndex: 'recurringServiceConnectedDisability' }
            ]
        };

        me.callParent();
    }
});