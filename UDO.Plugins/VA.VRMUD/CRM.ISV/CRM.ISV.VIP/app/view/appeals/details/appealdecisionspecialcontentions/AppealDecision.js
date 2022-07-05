/**
* @author Josh Oliver
* @class VIP.view.appeals.details.appealdecisionspecialcontentions.AppealDecision
*
* View for appeals hearing request data
*/
Ext.define('VIP.view.appeals.details.appealdecisionspecialcontentions.AppealDecision', {
    extend: 'Ext.form.Panel',
    alias: 'widget.appeals.details.appealdecisionspecialcontentions.appealdecision',
    title: 'Appeal Decision',
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
            fieldLabel: 'Date',
            name: 'decisionDate_f'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Remand To Name',
            name: 'decisionRemandedToName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Disposition Description',
            name: 'decisionDispositionDescription'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Remand To Code',
            name: 'remandedToCode'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Team Code',
            name: 'decisionTeamCode'
        }];

        me.callParent();
    }
});