/**
* @author Ivan Yurisevic
* @class VIP.view.poa.PastPoas
*
* The view for the past POA grid
*/
Ext.define('VIP.view.poa.PastPoas', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.poa.pastpoas',
    title: 'Past POAs',

    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'Begin Date', dataIndex: 'beginDate', xtype: 'datecolumn', format: 'm/d/Y', width: 90 },
			{ header: 'End Date', dataIndex: 'endDate', xtype: 'datecolumn', format: 'm/d/Y', width: 90 },
			{ header: 'Person/Org', dataIndex: 'personOrOrganizationInd', width: 50 },
			{ header: 'Name', dataIndex: 'personOrgName', width: 150 },
			{ header: 'Attn', dataIndex: 'personOrgAttentionText' },
			{ header: 'Cd', dataIndex: 'personOrganizationCode' },
			{ header: 'Person/Org Name', dataIndex: 'personOrganizationName' },
			{ header: 'Status', dataIndex: 'statusCode', width: 50 },
			{ header: 'Event Date', dataIndex: 'eventDate', xtype: 'datecolumn', format: 'm/d/Y', width: 70 },
			{ header: 'HC Provider Release', dataIndex: 'healthcareProviderReleaseInd' },
			{ header: 'Phrase', dataIndex: 'prepositionalPhraseName' },
			{ header: 'Rate', dataIndex: 'rateName' },
			{ header: 'Relationship', dataIndex: 'relationshipName' },
			{ header: 'Temp Custodian', dataIndex: 'temporaryCustodianInd' },
			{ header: 'Jrn Date', dataIndex: 'journalDate', xtype: 'datecolumn', format: 'm/d/Y', width: 70 },
			{ header: 'Jrn Loc', dataIndex: 'journalLocationID', width: 50 },
			{ header: 'Jrn Obj', dataIndex: 'journalObjectID' },
			{ header: 'Jrn Status', dataIndex: 'journalStatusTypeCode' },
			{ header: 'Jrn User', dataIndex: 'journalUserID' },
			{ header: 'Person/Org Ptcpnt Id', dataIndex: 'personOrgParticipantID' },
			{ header: 'Vet Ptcpnt Id', dataIndex: 'veteranParticipantID' }
        ];

        me.callParent();
    }
});
