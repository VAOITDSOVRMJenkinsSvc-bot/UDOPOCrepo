/**
* @author Jonas Dawson
* @class VIP.view.awards.details.payeefiduciary.FiduciaryInformation
*
* The field set for the fiduciary information in the awards tab details section.
*/
Ext.define('VIP.view.awards.details.payeefiduciary.FiduciaryInformation', {
    extend: 'Ext.form.Panel',
    alias: 'widget.awards.details.payeefiduciary.fiduciaryinformation',
    title: 'Fiduciary Information (Select Award to Populate)',
    layout: {
        type: 'table',
        columns: 4,
        tableAttrs: {
            style: {
                width: '100%'
            }
        }
    },
    defaults: {
        labelStyle: 'font-weight:bold;font-size:11px;',
        fieldStyle: 'font-size:11px;',
        labelWidth: 120,
        width: 270
    },
    autoScroll: true,
    bodyPadding: 5,
    initComponent: function () {
        var me = this;
        me.items = [{
            xtype: 'displayfield',
            fieldLabel: 'Name',
            name: 'personOrgName'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Begin Date',
            name: 'beginDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'End Date',
            name: 'endDate'
        }, {
            xtype: 'displayfield',
            fieldLabel: 'Relationship Name',
            name: 'relationshipName'
        }];

        me.callParent();
    }
});