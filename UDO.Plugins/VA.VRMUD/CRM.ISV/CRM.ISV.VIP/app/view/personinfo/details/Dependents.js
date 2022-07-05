/**
* @class VIP.view.personinfo.details.Dependents
* View for dependents associated with the person
*/
Ext.define('VIP.view.personinfo.details.Dependents', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personinfo.details.dependents',
    requires: [
        'VIP.view.ServiceRequest'
    ],
    title: 'Dependents',
    store: 'personinfo.Dependents',
    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'Ptcpnt Id', dataIndex: 'participantId' },
			{ header: 'First', dataIndex: 'firstName' },
			{ header: 'Middle', dataIndex: 'middleName' },
			{ header: 'Last', dataIndex: 'lastName' },
			{ header: 'SSN', dataIndex: 'ssn' },
			{ header: 'SSN Verify Status', dataIndex: 'ssnVerifyStatus' },
			{ header: 'Related to Vet', dataIndex: 'relatedToVet' },
			{ header: 'Relationship', dataIndex: 'relationship' },
			{ header: 'Proof Of Dependency', dataIndex: 'proofOfDependency' },
			{ header: 'Gender', dataIndex: 'gender' },
			/*{ header: 'Award Indicated', dataIndex:'awardInd'},*/
			{ header: 'Curr Relate Status', dataIndex: 'currentRelateStatus' },
			{ header: 'DOB', dataIndex: 'dob', xtype: 'datecolumn', format: 'm/d/Y' },
			{ header: 'DOD', dataIndex: 'dod', xtype: 'datecolumn', format: 'm/d/Y' },
			{ header: 'Death Reason', dataIndex: 'deathReason' },
			{ header: 'Email', dataIndex: 'emailAddress' },
			{ header: 'City of Birth', dataIndex: 'cityOfBirth' },
			{ header: 'State of Birth', dataIndex: 'stateOfBirth' }
            /* , { header: 'Award Indicator', dataIndex: 'awardInd' }*/
        ];

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'button',
                    text: 'View Contact',
                    tooltip: 'View selected Dependent in a new window. ' +
                             'Autocreates contact if the person is not ' +
                             'already in the system.',
                    iconCls: 'icon-contact',
                    action: 'viewdependentscontact',
                    id: 'id_personinfo_details_Dependents_01'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_personinfo_details_Dependents_02'
                },
                {
                    xtype: 'servicerequest',
                    id: 'id_personinfo_details_Dependents_03'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_personinfo_details_Dependents_04'
                },
                {
                    xtype: 'button',
                    text: 'Create VAI',
                    tooltip: 'Creates VAI for Dependent Record',
                    iconCls: 'icon-vai',
                    action: 'createvai',
                    hidden: true,
                    id: 'id_personinfo_details_Dependents_05'
                },
                {
                    xtype: 'tbfill',
                    id: 'id_personinfo_details_Dependents_06'
                }
            ]
        }];

        me.callParent();
    }
});