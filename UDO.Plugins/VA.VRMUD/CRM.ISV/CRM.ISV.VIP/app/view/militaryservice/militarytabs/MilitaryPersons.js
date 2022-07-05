/**
* @author Ivan Yurisevic
* @class VIP.view.militaryservice.militarytabs.MilitaryPersons
*
* The view for MilitaryPersons associated with the person
*/
Ext.define('VIP.view.militaryservice.militarytabs.MilitaryPersons', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.militaryservice.militarytabs.militarypersons',
    store: 'militaryservice.MilitaryPersons',
    title: 'Military Persons',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { width: 100 },
            items: [
                { header: 'Participant Id', dataIndex: 'participantId' },
                { header: 'Insurance File #', dataIndex: 'insuranceFileNumber' },
                { header: 'Insurance Policy #', dataIndex: 'insurancePolicyNumber' },
                { header: 'LGY Entitlement Amount', dataIndex: 'lgyEntitlementAmount', width: 140 },
                { header: 'Total Active Service Days', dataIndex: 'totalActiveServiceDays', width: 140 },
                { header: 'Total Active Service Months', dataIndex: 'totalActiveServiceMonths', width: 140 },
                { header: 'Total Active Service Years', dataIndex: 'totalActiveServiceYears', width: 140 },
                { header: 'Active Duty', dataIndex: 'activeDutyStatusInd' },
                { header: 'Death In Service', dataIndex: 'deathInServiceInd' },
                { header: 'Disability Service', dataIndex: 'disabilityServiceInd' },
                { header: 'Gulf War Registry', dataIndex: 'gulfWarRegistryInd' },
                { header: 'Incompetent', dataIndex: 'incompetentInd' },
                { header: 'Reserve', dataIndex: 'reserveInd' }
                
            ]
        };

        me.callParent();
    }
});
