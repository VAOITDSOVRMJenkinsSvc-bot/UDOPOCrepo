
Ext.define('VIP.model.personVadir.Alias', {
	extend: 'Ext.data.Model',
	
	fields: [
		{
		    name: 'firstName',
			type: 'string',
            mapping: 'firstName'
		},
		{
			name: 'lastName',
			type: 'string',
            mapping: 'lastName'
        },
		{
			name: 'effectiveDate',
			type: 'date',
            dateFormat: 'c',
            mapping: 'effectiveDate'
		}
	],
	
	proxy: {
        type: 'memory',
        reader: {
            type: 'xml',
            record: 'alias'
        }
    }

});