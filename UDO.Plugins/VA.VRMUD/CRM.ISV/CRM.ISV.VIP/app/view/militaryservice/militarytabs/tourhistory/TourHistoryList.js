/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.tourhistory.TourHistoryList
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.tourhistory.TourHistoryList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.tourhistory.tourhistorylist',
    store: 'militaryservice.TourHistory',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                width: 150
            },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Branch', dataIndex: 'militaryServiceBranchTypeName' },
                { header: 'Other Branch', dataIndex: 'militaryServiceOtherBranchTypeName' },
                { header: 'Military Separation Reason',  dataIndex: 'militarySeperationReasonTypeName' },
                { header: 'Discharge Type', dataIndex: 'mpDischargeCharacterTypeName' },
                { header: 'Discharge Pay Grade', dataIndex: 'dischargePayGradeName' },
                { header: 'Verified', dataIndex: 'verifiedIndicator', width: 50 },
                { header: 'Entered Active Duty', dataIndex: 'eodDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Released Active Duty', dataIndex: 'radDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Service Number', dataIndex: 'serviceNumber' },
                { header: 'Tour #', dataIndex: 'militaryPersonTourNumber', width: 50 },
                { header: 'Days Active', dataIndex: 'daysActiveQuantity', width: 80 },
                { header: 'Six Year Obligation', dataIndex: 'sixYearObligationDate' }
            ]
        };

        me.callParent();
    }
});
