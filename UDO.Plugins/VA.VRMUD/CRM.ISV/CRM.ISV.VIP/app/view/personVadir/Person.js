/**
* @class VIP.view.personVadir.Person
*
* The view for the past Person grid
*/
Ext.define('VIP.view.personVadir.Person', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personvadir.person',
    title: 'Person Search',
    store: 'PersonVadir',

    initComponent: function () {
        var me = this;

        me.store = 'PersonVadir'

        me.columns = [
            { header: 'SSN', dataIndex: 'ssn' },
            { header: 'Edipi', dataIndex: 'vaId' },
            { header: 'First Name', dataIndex: 'firstName' },
            { header: 'Middle Name', dataIndex: 'middleName' },
            { header: 'Last Name', dataIndex: 'lastName' },
            { header: 'Cadency', dataIndex: 'cadency' },
            { header: 'DOB', dataIndex: 'dob', xtype: 'datecolumn', format: 'm/d/Y' },
            { header: 'DOD', dataIndex: 'dod', xtype: 'datecolumn', format: 'm/d/Y' },
            { header: 'Death Ind', dataIndex: 'deathInd' },
            { header: 'Gender', dataIndex: 'gender' }
        ];

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'button',
                    text: 'Create Vadir Service Request',
                    tooltip: 'Creates an Vadir Service Request',
                    iconCls: 'icon-request',
                    action: 'createservicerequest',
                    id: 'id_personVadir_Person_01'
                }]
        }];

        me.callParent();
    }
});
