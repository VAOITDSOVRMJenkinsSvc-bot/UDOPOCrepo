/**
* @class VIP.view.birls.birlsdetails.militaryservice.ServiceList
*
* The view for MilitaryService associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.militaryservice.ServiceList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.birls.birlsdetails.militaryservice.servicelist',
    store: 'birls.MilitaryService',

    initComponent: function () {
        var me = this;

        me.columns = {
            //defaults: { width: 100 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Service Num', dataIndex: 'shortServiceNumber', flex: 1 },
                { header: 'Service Num Fill', dataIndex: 'serviceNumberFill' },
                { header: 'Branch', dataIndex: 'branchOfService' },
                { header: 'EOD Date', dataIndex: 'enteredOnDutyDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'RAD Date', dataIndex: 'releasedActiveDutyDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Sep Reason', dataIndex: 'separationReasonCode' },
                { header: 'Non Pay Days', dataIndex: 'nonpayDays' },
                { header: 'Pay Grade', dataIndex: 'payGrade' },
                { header: 'Char Service', dataIndex: 'charOfServiceCode' }
            ]
        };

        me.callParent();
    }
});
