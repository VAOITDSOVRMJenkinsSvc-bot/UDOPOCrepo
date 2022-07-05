/**
* @class VIP.view.awards.Benefits
*
* The view for the main awards grid
*/
Ext.define('VIP.view.awards.Benefits', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.awards.benefits',
    store: 'Awards',
    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                //flex: 1
            },
            items: [
                { header: 'Bene Type Code', dataIndex: 'awardBeneTypeCode', width: 100 },
                { header: 'Bene Type Name', dataIndex: 'awardBeneTypeName', width: 160 },
                { header: 'Type Code', dataIndex: 'awardTypeCode', width: 80 },
                { header: 'Type Name', dataIndex: 'awardTypeName', width: 180 },
                { header: 'Beneficiary', dataIndex: 'beneficiaryName', width: 125 },
                { header: 'Payee Code', dataIndex: 'payeeCode', width: 80 },
                { header: 'Recipient', dataIndex: 'recipientName', width: 160 },
                { header: 'Veteran', dataIndex: 'vetName', width: 160 }
            ]
        };

        me.dockedItems = [{
            xtype: 'toolbar',
            items: [
                {
                    xtype: 'button',
                    text: 'Refresh',
                    tooltip: '',
                    iconCls: 'icon-refresh',
                    action: 'reloadAllAwards',
                    id: 'id_awards_Benefits_01'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_awards_Benefits_02'
                },
                {
                    xtype: 'button',
                    text: 'Initiate CADD',
                    tooltip: 'Initiates Change of Address action',
                    iconCls: 'icon-addrChange',
                    action: 'changeofaddress',
                    id: 'id_awards_Benefits_03'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_awards_Benefits_04'
                },
                {
                    xtype: 'button',
                    text: 'View Address Details',
                    tooltip: '',
                    iconCls: 'icon-addrView',
                    action: 'viewaddressdetails',
                    id: 'id_awards_Benefits_05'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_awards_Benefits_06'
                },
                {
                    xtype: 'button',
                    text: 'View Payee',
                    tooltip: '',
                    iconCls: 'icon-contact',
                    action: 'viewpayee',
                    id: 'id_awards_Benefits_07'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_awards_Benefits_08'
                },
                {
                    xtype: 'button',
                    text: 'View Fiduciary Data',
                    tooltip: '',
                    iconCls: 'icon-contact',
                    action: 'viewmorefiduciarydata',
                    id: 'id_awards_Benefits_09'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_awards_Benefits_10'
                },
                {
                    xtype: 'servicerequest',
                    text: 'Create Service Request',
                    tooltip: '',
                    //iconCls: 'icon-script',
                    action: 'createservicerequest',
                    id: 'id_awards_Benefits_11'
                },
                {
                    xtype: 'tbseparator',
                    id: 'id_awards_Benefits_12'
                },
                {
                    xtype: 'button',
                    text: 'Create VAI',
                    tooltip: 'Creates VAI for Selected Award',
                    iconCls: 'icon-vai',
                    action: 'createvai',
                    hidden: true,
                    id: 'id_awards_Benefits_13'
                },
                {
                    xtype: 'tbfill',
                    id: 'id_awards_Benefits_14'
                },
                {
                    xtype: 'tbtext',
                    notificationType: 'awardcount',
                    text: 'Awards: 0',
                    id: 'id_awards_Benefits_15'
                }
            ]
        }];

        me.callParent();
    }
});
