/**
* @author Dmitri Riz
* @class VIP.model.Environment
*
* The model for CRM Environment
*/
Ext.define('VIP.model.Environment', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'envName' },
    { name: 'isPROD' },
    { name: 'globalDAC' },
    { name: 'CORP' },
    { name: 'VADIR' },
    { name: 'MVI' },
    { name: 'MVIDAC' },
    { name: 'MVIBase' },
    { name: 'Pathways' },
    { name: 'PathwaysBase' },
    { name: 'Vacols' },
    { name: 'VacolsBase' },
    { name: 'VacolsDAC' },
    { name: 'ShFld' },
    { name: 'RepWS' },
    { name: 'UseMSXML' }
]
});