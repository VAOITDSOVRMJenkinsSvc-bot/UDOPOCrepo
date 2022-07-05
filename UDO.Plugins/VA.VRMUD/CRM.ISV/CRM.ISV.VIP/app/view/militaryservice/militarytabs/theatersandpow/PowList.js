/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.theatersandpow.PowList
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.theatersandpow.PowList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.theatersandpow.powlist',
    store: 'militaryservice.Pow',
    title: 'POW Information',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Theater', dataIndex: 'militaryTheatreTypeName' },
                { header: 'Captor', dataIndex: 'captorText' },
                { header: 'POW Country', dataIndex: 'powCountryTypeCode' },
                { header: 'Capture Date', dataIndex: 'captureDate' },
                { header: 'Release Date', dataIndex: 'releaseDate' },
                { header: 'Days', dataIndex: 'days' },
                { header: 'Under 30 Days', dataIndex: 'underThirtyDaysIndicator' },
                { header: 'Camp Sector', dataIndex: 'campSectorText' },
                { header: 'Verified', dataIndex: 'verifiedIndicator' },
                { header: 'POW Seq Num', dataIndex: 'militaryPersonPowSequenceNumber' },
                { header: 'Participant Id', dataIndex: 'participantId' }
            ]
        };

        me.callParent();
    }
});
