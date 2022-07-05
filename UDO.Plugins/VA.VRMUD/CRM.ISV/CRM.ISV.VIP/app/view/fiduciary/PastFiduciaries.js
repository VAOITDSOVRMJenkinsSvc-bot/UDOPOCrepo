/**
* @author Ivan Yurisevic
* @class VIP.view.fiduciary.PastFiduciaries
*
* The view for the past fiduciaries grid
*/
Ext.define('VIP.view.fiduciary.PastFiduciaries', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.fiduciary.pastfiduciaries',
    title: 'Past Fiduciaries',

    initComponent: function () {
        var me = this;

        me.columns = [
            { header: 'Begin Date',dataIndex:'beginDate,', xtype:'datecolumn', format:'M/d/Y' },
            {header: 'End Date', dataIndex: 'endDate', xtype:'datecolumn', format:'M/d/Y' },
            { header: 'Person/Org',dataIndex:'personOrOrganizationInd' },
            { header: 'Name',dataIndex:'personOrgName'},
            { header: 'Attn',dataIndex:'personOrgAttentionText' },
            { header: 'Cd',dataIndex: 'personOrganizationCode'},
            { header: 'Person/Org Name',dataIndex:'personOrgName' },
            { header: 'Status',dataIndex:'statusCode' },
            { header: 'Event Date', dataIndex: 'eventDate', xtype:'datecolumn', format:'M/d/Y' },
            { header: 'HC Provider Release',dataIndex:'healthcareProviderReleaseInd' },
            { header: 'Phrase',dataIndex:'prepositionalPhraseName'},
            { header: 'Rate',dataIndex:'rateName' },
            { header: 'Relationship',dataIndex:'relationshipName' },
            { header: 'Temp Custodian',dataIndex:'temporaryCustodianInd' },
            { header: 'Jrn Date', dataIndex: 'journalDate', xtype:'datecolumn', format:'M/d/Y' },
            { header: 'Jrn Loc',dataIndex:'journalLocationID' },
            { header: 'Jrn Obj',dataIndex:'journalObjectID' },
            { header: 'Jrn Status', dataIndex:'journalStatusTypeCode' },
            { header: 'Jrn User',dataIndex:'journalUserID' },
            { header: 'Person/Org Ptcpnt Id',dataIndex:'personOrgParticipantID' },
            { header: 'Vet Ptcpnt Id', dataIndex:'veteranParticipantID'}
        ];

        me.callParent();
    }
});
