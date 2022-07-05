/**
* @author Josh Oliver
* @class VIP.view.appeals.AppealData
*
* The view for the main awards grid
*/
Ext.define('VIP.view.appeals.AppealData', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.appeals.appealdata',
    store: 'Appeal',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Name', dataIndex: 'fullName' },
			    { header: 'Status Code', dataIndex: 'appealStatusCode', width: '25' },
			    { header: 'Status Description', dataIndex: 'appealStatusDescription' },
                { header: 'Notice Of Disagreement Rec', dataIndex: 'noticeOfDisagreementReceivedDate_f' },
                { header: 'Decision Date', dataIndex: 'decisionDate_f' },
                { header: 'Region Office Code', dataIndex: 'regionOfficeCode' },
                { header: 'Region Office Description', dataIndex: 'regionOfficeDescription' },
                { header: 'Action Type Code', dataIndex: 'actionTypeCode' },
                { header: 'Action Type Description', dataIndex: 'actionTypeDescription' }
            ]
        };

        me.dockedItems = [
            {
                xtype: 'toolbar',
                items: [
                    {
                        xtype: 'button',
                        text: 'Load/Refresh All Data',
                        tooltip: 'Loads or refreshes all data, including subgrids, for the award.',
                        iconCls: 'icon-refresh',
                        action: 'loadappealdata',
                        id: 'id_appeals_AppealData_01'
                    },
                    {
                        xtype: 'tbseparator',
                        id: 'id_appeals_AppealData_02'
                    },
                    {
                        xtype: 'button',
                        text: 'Initiate CADD',
                        tooltip: 'Initiates Change of Address action',
                        iconCls: 'icon-addrChange',
                        action: 'changeofaddress',
                        id: 'id_appeals_AppealData_03'
                    },
                    {
                        xtype: 'tbseparator',
                        id: 'id_appeals_AppealData_04'
                    },
                    {
                        xtype: 'button',
                        text: 'Script',
                        tooltip: 'Brings up a dialog box with the call script for this appeal.',
                        iconCls: 'icon-script',
                        action: 'appealscript',
                        id: 'id_appeals_AppealData_05'
                    },
                    {
                        xtype: 'tbfill',
                        id: 'id_appeals_AppealData_06'
                    },
                    {
                        xtype: 'tbtext',
                        notificationType: 'appealcount',
                        text: 'Appeals: 0',
                        id: 'id_appeals_AppealData_07'
                    }
                ]
                }
        ];

        me.callParent();
    }
});
