/**
* @author Josh Oliver
* @class VIP.view.appeals.details.HearingRequest
*
* View for appeals hearing request data
*/
Ext.define('VIP.view.appeals.details.HearingRequest', {
    extend: 'Ext.form.Panel',
    alias: 'widget.appeals.details.hearingrequest',
    title: 'Hearing Request',
    layout: {
        type: 'table',
        columns: 3,
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
        labelWidth: 160,
        width: 250
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;

        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Request Type Description',
            name: 'hearingRequestedTypeDescription',
            colspan: 2
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Request Date',
            name: 'hearingRequestRequestedDate_f'
        }, // Row 2
        {
         xtype: 'displayfield',
         fieldLabel: 'Disposition Description',
         name: 'hearingRequestDispositionDescription',
         colspan: 2
        },
        {
            xtype: 'displayfield',
            fieldLabel: 'Scheduled Date',
            name: 'hearingRequestScheduledDate_f'
        }, //Row 3
        {
         xtype: 'displayfield',
         fieldLabel: 'Hearing Action Description',
         name: 'hearingActionDescription',
         colspan: 2
        }, 
        {
            xtype: 'displayfield',
            fieldLabel: 'Closed Date',
            name: 'hearingRequestClosedDate_f'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Notes',
            name: 'hearingRequestNotes'
        }];

        me.callParent();
    }
});