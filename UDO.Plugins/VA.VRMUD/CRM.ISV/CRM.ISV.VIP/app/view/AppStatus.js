/**
* @author Jonas Dawson
* @class VIP.view.AppStatus
*
* The sole status bar for the VIP application.
*/
Ext.define('VIP.view.AppStatus', {
    extend: 'Ext.toolbar.Toolbar',
    alias: 'widget.appstatus',
    requires: ['VIP.view.WebServiceRequestHistory'],

    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'button',
            enableToggle: true,
            iconCls: 'icon-systemMonitor',
            action: 'togglewebservicerequesthistory'
        }, {
            xtype: 'button',
            iconCls: 'icon-status-warning',
            action: 'displayalerts'
        }, '-', '->', {
            xtype: 'tbtext',
            notificationType: 'statistics',
            text: ''
        }];

        me.callParent();
    }
});