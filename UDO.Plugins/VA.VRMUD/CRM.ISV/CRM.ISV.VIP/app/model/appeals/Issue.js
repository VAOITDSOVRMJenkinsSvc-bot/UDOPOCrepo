/**
* @author Josh Oliver
* @class VIP.model.appeals.Issue
*
* The model for appeal issues
*/
Ext.define('VIP.model.appeals.Issue', {
    extend: 'Ext.data.Model',
    requires: [
        'VIP.model.appeals.RemandReason'
    ],
    fields: [
        {
            name: 'issueSequenceNumber',
            type: 'int',
            mapping: 'IssueSequenceNumber'
        },
        {
            name: 'issueProgramCode',
            type: 'string',
            mapping: 'IssueProgramCode'
        },
        {
            name: 'issueProgramDescription',
            type: 'string',
            mapping: 'IssueProgramDescription'
        },
        {
            name: 'issueCode',
            type: 'string',
            mapping: 'IssueCode'
        },
        {
            name: 'issueCodeDescription',
            type: 'string',
            mapping: 'IssueCodeDescription'
        },
        {
            name: 'issueDescription',
            type: 'string',
            mapping: 'IssueDescription'
        },
        {
            name: 'issueDispositionCode',
            type: 'string',
            mapping: 'IssueDispositionCode'
        },
        {
            name: 'issueDispositionDescription',
            type: 'string',
            mapping: 'IssueDispositionDescription'
        },
        {
            name: 'issueDispositionDate',
            type: 'date',
            mapping: 'IssueDispositionDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'issueLevel1Code',
            type: 'string',
            mapping: 'IssueLevel1Code'
        },
        {
            name: 'issueLevel1Description',
            type: 'string',
            mapping: 'IssueLevel1Description'
        },
        {
            name: 'issueLevel2Code',
            type: 'string',
            mapping: 'IssueLevel2Code'
        },
        {
            name: 'issueLevel2Description',
            type: 'string',
            mapping: 'IssueLevel2Description'
        },
        {
            name: 'issueLevel2',
            convert: function (v, record) {
                var code = record.get('issueLevel2Code'),
                    description = record.get('issueLevel2Description');

                if (!Ext.isEmpty(code) && !Ext.isEmpty(description)) {
                    return code + ' ' + description;
                }

                if (!Ext.isEmpty(code) && Ext.isEmpty(description)) {
                    return code;
                }

                if (Ext.isEmpty(code) && !Ext.isEmpty(description)) {
                    return description;
                }

                return '';
            }
        },
        {
            name: 'issueLevel3Code',
            type: 'string',
            mapping: 'IssueLevel3Code'
        },
        {
            name: 'issueLevel3Description',
            type: 'string',
            mapping: 'IssueLevel3Description'
        }
    ],
    hasMany: [
        {
            model: 'VIP.model.appeals.RemandReason',
            name: 'remandReasons',
            associationKey: 'remandReasons',
            storeConfig: {
                filters: []
            }
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'Issue'
        }
    }
});