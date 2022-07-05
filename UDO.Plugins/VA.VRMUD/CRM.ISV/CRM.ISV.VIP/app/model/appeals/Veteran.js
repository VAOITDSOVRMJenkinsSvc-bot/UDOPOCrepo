/**
* @author Josh Oliver
* @class VIP.model.appeals.Veteran
*
* The model for appeal Veteran
*/
Ext.define('VIP.model.appeals.Veteran', {
    extend: 'Ext.data.Model',
    fields: [
        {
            name: 'birthDate',
            type: 'string',
            mapping: 'BirthDate'
        },
        {
            name: 'veteranFirstName',
            type: 'string',
            mapping: 'VeteranFirstName'
        },
        {
            name: 'veteranMiddleInitial',
            type: 'string',
            mapping: 'VeteranMiddleInitial'
        },
        {
            name: 'veteranGender',
            type: 'string',
            mapping: 'VeteranGender'
        },
        {
            name: 'veteranLastName',
            type: 'string',
            mapping: 'VeteranLastName'
        },
        {
            name: 'veteranFullName',
            convert: function (v, record) {
                var name = '';
                if (!Ext.isEmpty(record.get('veteranLastName'))) {
                    name += record.get('veteranLastName');
                    if (!Ext.isEmpty(record.get('veteranFirstName'))) {
                        name += ', ' + record.get('veteranFirstName');
                    }
                    if (!Ext.isEmpty(record.get('veteranMiddleInitial'))) {
                        var mi = record.get('veteranMiddleInitial');
                        name += ' ' + mi;
                        if (mi.length == 1) { name += '.'; }
                    }
                }
                return name;
            }
        },
        {
            name: 'veteranSsn',
            type: 'string',
            mapping: 'VeteranSSN'
        },
        {
            name: 'finalNoticeOfDeathDate',
            type: 'date',
            mapping: 'FinalNoticeOfDeathDate',
            dateFormat: 'Y-m-d'
        },
        {
            name: 'finalNoticeOfDeathDate_f',
            convert: function (v, record) {
                if (!Ext.isEmpty(record.get('finalNoticeOfDeathDate'))) {
                    return Ext.Date.format(record.get('finalNoticeOfDeathDate'), 'm/d/Y');
                } else { return ''; }
            }
        }
    ],
    proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'AppealVeteran'
        }
    }
});