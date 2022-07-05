/**
* @author Josh Oliver
* @class VIP.view.virtualva.DocumentList
*
* The view for the Virtual VA document records
*/
Ext.define('VIP.view.virtualva.DocumentList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.virtualva.documentlist',
    store: 'virtualva.DocumentRecord',

    initComponent: function () {
        var me = this;

        me.columns = {
            defaults: {
                flex: 1
            },
            items: [
                { header: 'Document Date', dataIndex: 'receivedDate', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Document Type Desc', dataIndex: 'documentTypeDescriptionText' },
                { header: 'Subject', dataIndex: 'subjectText' },
                { header: 'Document Id', dataIndex: 'fileNetDocumentId' },
                { header: 'Document Source', dataIndex: 'fileNetDocumentSource' },
                { header: 'Document Format', dataIndex: 'documentFormatCode' },
                { header: 'Regional Office', dataIndex: 'jro' }
            ]
        };

        me.dockedItems = [
            {
                xtype: 'toolbar',
                items: [
                    {
                        xtype: 'textfield',
                        readOnly: true,
                        id: 'vvaSearchValue',
                        fieldLabel: 'Search Value (SSN / File #, Read-Only)',
                        labelWidth: 200,
                        style: {
                            fontSize: '11px',
                            color: '#333333'
                        }
                    },
                    {
                        xtype: 'button',
                        text: 'Search Document Records',
                        iconCls: 'icon-refresh',
                        action: 'searchDocumentList',
                        id: 'id_virtualva_DocumentList_02'
                    },
                    { xtype: 'tbseparator', id: 'id_virtualva_DocumentList_03' },
                    {
                        xtype: 'button',
                        text: 'Open Selected Document',
                        tooltip: '',
                        iconCls: 'icon-docDownload',
                        action: 'openDocumentContent',
                        id: 'id_virtualva_DocumentList_04'
                    },
                    { xtype: 'tbfill', id: 'id_virtualva_DocumentList_05' },
                    {
                        xtype: 'tbtext',
                        notificationType: 'documentcount',
                        text: 'Documents: 0',
                        id: 'id_virtualva_DocumentList_06'
                    }
                ]
            }
        ];

        me.callParent();
    }
});
