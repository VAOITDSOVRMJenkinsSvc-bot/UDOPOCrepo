/**
* @class VIP.view.personinfo.details.AllRelationships
* View for AllRelationships associated with the person
*/
Ext.define('VIP.view.personinfo.details.AllRelationships', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personinfo.details.allrelationships',
    title: 'All Relationships',
    store: 'personinfo.AllRelationships',


    initComponent: function () {
        var me = this;

        me.columns = [
				   { header: 'Full Name', dataIndex: 'fullName', width: 170 },
				   { header: 'SSN', dataIndex: 'ssn', width: 70 },
				   { header: 'SSN Verify Status', dataIndex: 'ssnVerifiedInd' },
				   { header: 'Relationship', dataIndex: 'relationshipType', width: 75 },
				   { header: 'Proof Of Dependency', dataIndex: 'proofOfDependecyInd' },
				   { header: 'Gender', dataIndex: 'gender', width: 50 },
				   { header: 'Award Ind', dataIndex: 'awardInd' },
				   { header: 'Award Type', dataIndex: 'awardType' },
				   { header: 'Award Begin Date', dataIndex: 'awardBeginDate', xtype: 'datecolumn', format: 'm/d/Y' },
				   { header: 'Award End Date', dataIndex: 'awardEndDate', xtype: 'datecolumn', format: 'm/d/Y' },
				   { header: 'DOB', dataIndex: 'dob', xtype: 'datecolumn', format: 'm/d/Y' },
				   { header: 'DOD', dataIndex: 'dod' },
				   { header: 'Fiduciary', dataIndex: 'fiduciary' },
				   { header: 'POA', dataIndex: 'poa' }, 
                   { header: 'Email', dataIndex: 'emailAddress' },
				   { header: 'Relationship Begin Date', dataIndex: 'relationshipBeginDate', xtype: 'datecolumn', format: 'm/d/Y' },
				   { header: 'Relationship End Date', dataIndex: 'relationshipEndDate', xtype: 'datecolumn', format: 'm/d/Y' },
				   { header: 'Dependent Reason', dataIndex: 'dependentReason' },
				   { header: 'Dependent Terminate Date', dataIndex: 'dependentTerminateDate', xtype: 'datecolumn', format: 'm/d/Y' },
				   { header: 'Terminate Reason', dataIndex: 'terminateReason' },
				   { header: 'File Number', dataIndex: 'fileNumber' },
				   { header: 'Ptcpnt Id', dataIndex: 'participantId' } 

        ];

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'button',
                    text: 'View Contact',
                    tooltip: 'View selected Person in a new window. ' +
                             'Autocreates contact if the person is not ' +
                             'already in the system.',
                    iconCls: 'icon-contact',
                    action: 'viewallrelationshipscontact',
                    id: 'id_personinfo_details_AllRelationships_01'
                },
                {
                    xtype: 'button',
                    text: 'Intent To File',
                    tooltip: 'Intent To File',
                    action: 'intentToFile',
                    id: 'id_personinfo_details_AllRelationships_02'
                }
            ]
        }];

        me.callParent();
    }
});