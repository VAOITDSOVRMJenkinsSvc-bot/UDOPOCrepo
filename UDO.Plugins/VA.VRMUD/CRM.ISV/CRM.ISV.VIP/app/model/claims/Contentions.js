/**
* @author Josh Oliver
* @class VIP.model.claims.Contentions
*
* The model for claim contentions
*/
Ext.define('VIP.model.claims.Contentions', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.soap.envelopes.mapd.contention.FindContentions',
        'VIP.model.claims.SpecialIssues'
    ],
    fields: [
        {
            name: 'callId',
            type: 'string'
        },
        {
            name: 'journalDate',
            type: 'string',
            mapping: 'jrnDt'
        },
        {
            name: 'journalLocationId',
            type: 'string',
            mapping: 'jrnLctnId'
        },
        {
            name: 'journalObjectId',
            type: 'string',
            mapping: 'jrnObjId'
        },
        {
            name: 'journalStatusTypeCode',
            type: 'string',
            mapping: 'jrnSttTc'
        },
        {
            name: 'journalUserId',
            type: 'string',
            mapping: 'jrnUserId'
        },
        {
            name: 'parentId',
            type: 'string'
        },
        {
            name: 'parentName',
            type: 'string'
        },
        {
            name: 'rowCount',
            type: 'string',
            mapping: 'rowCnt'
        },
        {
            name: 'rowId',
            type: 'string'
        },
        {
            name: 'beginDate',
            type: 'string',
            mapping: 'beginDt'
        },
        {
            name: 'claimId',
            type: 'string',
            mapping: 'clmId',
            ignoreMappingOnRequest: true
        },
        {
            name: 'claimantTxt',
            type: 'string',
            mapping: 'clmntTxt'
        },
        {
            name: 'claimReceivedDate',
            type: 'date',
            mapping: 'claimRcvdDt'
        },
        {
            name: 'clsfcnId',
            type: 'string'
        },
        {
            name: 'clsfcnTxt',
            type: 'string'
        },
        {
            name: 'completedDate',
            type: 'string',
            mapping: 'cmpltdDt'
        },
        {
            name: 'contentionId',
            type: 'string',
            mapping: 'cntntnId'
        },
        {
            name: 'contentionStatusTypeCode',
            type: 'string',
            mapping: 'cntntnStatusTc'
        },
        {
            name: 'contentionTypeCode',
            type: 'string',
            mapping: 'cntntnTypeCd'
        },
        {
            name: 'contentionTypeName',
            type: 'string',
            mapping: 'cntntnTypeNm'
        },
        {
            name: 'diagnosticTypeCode',
            type: 'string',
            mapping: 'dgnstcTc'
        },
        {
            name: 'diagnosticTypeName',
            type: 'string',
            mapping: 'dgnstcTn'
        },
        {
            name: 'medIndicator',
            type: 'int',
            mapping: 'medInd'
        },
        {
            name: 'medIndicator_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record)) { return record.translateMedicalIndicator(record.get('medIndicator')); }
                else { return 'NO'; }
            }
        },
        {
            name: 'notificationDate',
            type: 'string',
            mapping: 'notfcnDt'
        },
        {
            name: 'wgAplcbInd',
            type: 'string'
        },

    // ********** Starting computed fields
        {
        // TODO: process in controller
        name: 'specialIssues',
        type: 'string',
        convert: function (v, record) {
            return 'TODO: process in controller; collect record.contentionSpecialIssues().each.spisTn';
        }
    },
        {
            name: 'contentclass',
            type: 'string',
            convert: function (v, record) {
                return (!Ext.isEmpty(record.get('claimantTxt')) ? record.get('claimantTxt') : '') + '/' +
                        (!Ext.isEmpty(record.get('clsfcnTxt')) ? record.get('clsfcnTxt') : '');
            }
        }
    ],
    hasMany: [
        {
            model: 'VIP.model.claims.SpecialIssues',
            name: 'specialIssues',
            associationKey: 'specialIssue',
            storeConfig: {
                filters: []
            }
        }
    ],
    proxy: {
        type: 'soap',
        headers: {
            "SOAPAction": "",
            "Content-Type": "text/xml; charset=utf-8"
        },
        reader: {
            type: 'xml',
            record: 'contentions'
        },
        envelopes: {
            create: '',
            read: 'VIP.soap.envelopes.mapd.contention.FindContentions',
            update: '',
            destroy: ''
        }
    },
    translateMedicalIndicator: function (medicalIndicator) {
        var medicalIndicatorStr = 'NO';

        if (!Ext.isEmpty(medicalIndicator) && (medicalIndicator)) {
            medicalIndicatorStr = 'YES';
        }

        return medicalIndicatorStr;
    }
});