/**
* @author Ivan Yurisevic
* @class VIP.view.birls.birlsdetails.folderlocinfo.FolderLocationList
*
* The view for FolderLocations associated with the person
*/
Ext.define('VIP.view.birls.birlsdetails.folderlocinfo.FolderLocationList', {
    extend: 'Ext.grid.Panel',
    alias: 'widget.birls.birlsdetails.folderlocinfo.folderlocationlist',
    store: 'birls.FolderLocations',

    initComponent: function () {
        var me = this;
        me.columns = {
            defaults: { width: 100 },
            items: [
                { xtype: 'rownumberer' },
                { header: 'Type', dataIndex: 'folderType' },
                { header: 'Curr Loc', dataIndex: 'folderCurrentLocation' },
                { header: 'Transfer Date', dataIndex: 'dateOfTransfer', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Prior Loc', dataIndex: 'folderPriorLocation' },
                { header: 'In Transit Loc', dataIndex: 'inTransitToStation' },
                { header: 'In Transit Date', dataIndex: 'dateOfTransit' },
                { header: 'Relocation', dataIndex: 'relocationInd' },
                { header: 'FARC Accession Nbr', dataIndex: 'farcAccessionNum' },
                { header: 'No Folder Reason', dataIndex: 'noFolderEstReason' },
                { header: 'Destroyed', dataIndex: 'folderDestroyedInd' },
                { header: 'Rebuilt', dataIndex: 'folderRebuiltInd' },
                { header: 'No Record', dataIndex: 'noRecordInd' },
                { header: 'Folder Retire Date', dataIndex: 'dateOfFolderRetire', xtype: 'datecolumn', format: 'm/d/Y' },
                { header: 'Box Sequence Nbr', dataIndex: 'boxSequenceNum' },
                { header: 'Location Nbr', dataIndex: 'locationNum' },
                { header: 'Insurance Folder Type', dataIndex: 'insuranceFolderType', width: 150 }
            ]
        };

        me.callParent();
    }
});