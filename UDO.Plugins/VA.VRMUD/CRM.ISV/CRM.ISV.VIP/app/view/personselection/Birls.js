/**
* @author Jonas Dawson
* @class VIP.view.personselection.Birls
*
* The view for the birls inquiry grid
*/
Ext.define('VIP.view.personselection.Birls', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.personselection.birls',
    store: 'BirlsPersonSelection',
    title: 'BIRLS',
    layout: 'fit',
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
                    action: 'selectbirls'
                }
            ]
        }];

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'SSN', dataIndex: 'ssn' },
                { header: 'File Number', dataIndex: 'fileNumber' },
                { header: 'First Name', dataIndex: 'firstName' },
                { header: 'Middle Name', dataIndex: 'middleName' },
                { header: 'Last Name', dataIndex: 'lastName' },
                { header: 'Suffix', dataIndex: 'suffixName' },
                { header: 'Date of Birth', dataIndex: 'dob' },
                { header: 'Date of Death', dataIndex: 'dod' },
                { header: 'Veteran Ind', dataIndex: 'veteranIndicator' },
                { header: 'Payee Code', dataIndex: 'payeeCode' },
                { header: 'Branch', dataIndex: 'branchOfService' },
                { header: 'Current Location', dataIndex: 'currentLocation' },
                { header: 'Entered on Duty', dataIndex: 'eod' },
                { header: 'Released Active Duty', dataIndex: 'rad' },
                { header: 'Service Number', dataIndex: 'serviceNumber' },
                { header: 'SSN Verified', dataIndex: 'ssnVerified' }
            ]
        };

        me.callParent();
    }
});
