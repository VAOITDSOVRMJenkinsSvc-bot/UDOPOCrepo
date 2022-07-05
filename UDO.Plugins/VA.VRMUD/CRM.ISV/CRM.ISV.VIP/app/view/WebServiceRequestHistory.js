/**
* @author Jonas Dawson
* @class VIP.view.WebServiceRequestHistory
*
* The history for all web service requests.
*/
Ext.define('VIP.view.WebServiceRequestHistory', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.webservicerequesthistory',
    store: 'debug.WebServiceRequestHistory',
    bodyStyle: {
        background: '#DEEBF7',
        padding: '2px'
    },
    preventheader: true,
    autoScroll: true,
    layout: 'fit',
    initComponent: function () {
        var me = this;
        me.columns = [
                { header: 'ID', dataIndex: 'id', width: 30 },
                { header: 'Started', dataIndex: 'startTime', xtype: 'datecolumn', format: 'g:i:s A', width: 80 },
                { header: 'Web Service URL', dataIndex: 'webServiceUrl', width: 130 },
                { header: 'Method', dataIndex: 'method', width: 180 },
                { header: 'Total(ms)', dataIndex: 'totalTime', width: 50 },
                { header: 'Success', dataIndex: 'success', width: 50 },
                { header: 'Message', dataIndex: 'message', flex: 1 }
        ];

        me.dockedItems = [
            {
                xtype: 'toolbar',
                items: [
                    {
                        xtype: 'button',
                        text: 'View Selected Service Info',
                        action: 'displaypayloads',
                        iconCls: 'icon-viewWindow'
                    }
                ]
            }
        ];

        me.callParent();
    }
});