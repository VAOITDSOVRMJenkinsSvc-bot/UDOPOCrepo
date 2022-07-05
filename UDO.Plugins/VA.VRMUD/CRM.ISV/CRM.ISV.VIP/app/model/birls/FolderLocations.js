/**
* @author Ivan Yurisevic
* @class VIP.model.birls.FolderLocations
*
* The model for BIRLS FolderLocations record details
*/
Ext.define('VIP.model.birls.FolderLocations', {
    extend: 'Ext.data.Model',
//    requires: [
//        'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber'
//    ],
    fields: [{
        name: 'folderType',
        mapping: 'FOLDER_TYPE',
        type: 'string'
    }, {
        name: 'folderCurrentLocation',
        mapping: 'FOLDER_CURRENT_LOCATION',
        type: 'string'
    }, {
        name: 'dateOfTransfer',
        mapping: 'DATE_OF_TRANSFER',
        type: 'date',
        dateFormat: 'm/d/Y'
    }, {
        name: 'folderPriorLocation',
        mapping: 'FOLDER_PRIOR_LOCATION',
        type: 'string'
    }, {
        name: 'inTransitToStation',
        mapping: 'IN_TRANSIT_TO_STATION',
        type: 'string'
    }, {
        name: 'dateOfTransit',
        mapping: 'DATE_OF_TRANSIT',
        type: 'string'
    }, {
        name: 'relocationInd',
        mapping: 'RELOCATION_INDICATOR',
        type: 'string'
    }, {
        name: 'farcAccessionNum',
        mapping: 'FARC_ACCESSION_NUM',
        type: 'string'
    }, {
        name: 'noFolderEstReason',
        mapping: 'NO_FOLDER_EST_REASON',
        type: 'string'
    }, {
        name: 'folderDestroyedInd',
        mapping: 'FOLDER_DESTROYED_IND',
        type: 'string'
    }, {
        name: 'folderRebuiltInd',
        mapping: 'FOLDER_REBUILT_IND',
        type: 'string'
    }, {
        name: 'noRecordInd',
        mapping: 'NO_RECORD_IND',
        type: 'string'
    }, {
        name: 'dateOfFolderRetire',
        mapping: 'DATE_OF_FLDR_RETIRE',
        type: 'date',
        dateFormat: 'm/d/Y'
    }, {
        name: 'boxSequenceNum',
        mapping: 'BOX_SEQUENCE_NUMBER',
        type: 'string'
    }, {
        name: 'locationNum',
        mapping: 'LOCATION_NUMBER',
        type: 'string'
    }, {
        name: 'insuranceFolderType',
        mapping: 'INSURANCE_FOLDER_TYPE',
        type: 'string'
    }],

    belongsTo: 'VIP.model.Birls',
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'FOLDER'
        }
    }
//    proxy: {
//        type: 'soap',
//        headers: {
//            "SOAPAction": "",
//            "Content-Type": "text/xml; charset=utf-8"
//        },
//        reader: {
//            type: 'xml',
//            record: 'FOLDER'
//        },
//        envelopes: {
//            create: '',
//            read: 'VIP.soap.envelopes.share.vetrecord.FindBirlsRecordByFileNumber',
//            update: '',
//            destroy: ''
//        }
//    }
});