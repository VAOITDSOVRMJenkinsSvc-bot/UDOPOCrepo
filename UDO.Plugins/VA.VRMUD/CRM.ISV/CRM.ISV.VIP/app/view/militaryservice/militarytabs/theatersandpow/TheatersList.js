/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.theatersandpow.TheatersList
*
* The view for Flashes associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.theatersandpow.TheatersList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.theatersandpow.theaterslist',
    store: 'militaryservice.Theater',
    title: 'Military Theaters',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { flex: 1 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Tour Number', dataIndex: 'militaryPersonTourNumber' },
                { header: 'Theater Type', dataIndex: 'militaryTheatreTypeName' },
                { header: 'Begin Date', dataIndex: 'beginDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Days', dataIndex: 'days' },
                { header: 'Verified', dataIndex: 'verifiedIndicator' },
                { header: 'Participant Id', dataIndex: 'participantId' }
            ]
        };

        me.callParent();
    }
});
