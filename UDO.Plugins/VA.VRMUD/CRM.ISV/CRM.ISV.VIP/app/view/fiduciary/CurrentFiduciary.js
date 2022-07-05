/**
* @author Ivan Yurisevic
* @class VIP.view.fiduciary.CurrentFiduciary
*
* The view for the current Fiduciary fieldset
*/
Ext.define('VIP.view.fiduciary.CurrentFiduciary', {
    extend: 'Ext.form.Panel',
    alias: 'widget.fiduciary.currentfiduciary',
    title: 'Current Fiduciary',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        },
        tdAttrs: {
            width: '25%'
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Begin Date',
            name: 'beginDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Person/Org Name',
            name: 'personOrgName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Person Org Code',
            name: 'personOrganizationCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Person or Org',
            name: 'personOrOrganizationInd'
        },

        {
            xtype: 'displayfield',
            fieldLabel: 'End Date',
            name: 'endDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Person/Org Participant Id',
            name: 'personOrgParticipantID'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Person Org Name',
            name: 'personOrganizationName',
            width: 400,
            colspan: 2
        },

        {
            xtype: 'displayfield',
            fieldLabel: 'Relationship',
            name: 'relationshipName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Person/Org Attn',
            name: 'personOrgAttentionText'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Vet Ptcpnt Id',
            name: 'veteranParticipantID'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Temp Custodian',
            name: 'temporaryCustodianInd'
        },

        {
            xtype: 'displayfield',
            fieldLabel: 'Status',
            name: 'statusCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Event Date',
            name: 'eventDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Rate Name',
            name: 'rateName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Prepositional Phrase',
            name: 'prepositionalPhraseName'
        },

        {
            xtype: 'displayfield',
            fieldLabel: 'HC Provider Release',
            name: 'healthcareProviderReleaseInd'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Jrn Obj Id',
            name: 'journalObjectID'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Jrn Status Type',
            name: 'journalStatusTypeCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Jrn Date',
            name: 'journalDate'
        },

        {
            xtype: 'displayfield',
            fieldLabel: 'Jrn Loc Id',
            name: 'journalLocationID'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Jrn User Id',
            name: 'journalUserID'
        }];

        me.callParent();
    }
});