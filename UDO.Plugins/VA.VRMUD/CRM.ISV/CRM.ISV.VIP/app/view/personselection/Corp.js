/**
* @author Jonas Dawson
* @class VIP.view.personselection.Corp
*
* The view for the corp selection grid
*/
Ext.define('VIP.view.personselection.Corp', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personselection.corp',
    store: 'CorpPersonSelection',
    title: 'Corp',
    layout: 'fit',
    autoScroll: true,
    initComponent: function () {
        var me = this;

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'button',
                    text: 'Selected Record Identifies Caller',
                    tooltip: 'Verify Identity of the Caller and view full details',
                    iconCls: 'icon-add',
                    action: 'selectcorp'
                }
            ]
        }];

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'SSN', dataIndex: 'ssn' },
                { header: 'First Name', dataIndex: 'firstName' },
                { header: 'Middle Name', dataIndex: 'middleName' },
                { header: 'Last Name', dataIndex: 'lastName' },
                { header: 'Suffix', dataIndex: 'suffix' },
                { header: 'Branch of Service', dataIndex: 'branchOfService' },
                { header: 'Date of Birth', dataIndex: 'dob' },
                { header: 'Date of Death', dataIndex: 'dod' },
                { header: 'Participant ID', dataIndex: 'participantId' }
            ]
        };

        me.callParent();
    }
});
