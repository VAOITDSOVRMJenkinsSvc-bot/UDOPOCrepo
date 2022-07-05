/**
* @author Josh Oliver
* @class VIP.model.PersonInquiry
*
* The model for person inquiry information
*/
Ext.define('VIP.model.PersonInquiry', {
    extend: 'Ext.data.Model',
    fields: [
    // corp

    // CORP Search Type   : ssn/fileno or PID
        { name: 'searchCORPBy' },  //SSN or PARTICIPANTID

        { name: 'ssn' },
        { name: 'participantId' },
        { name: 'edipi' }, 
        { name: 'firstName' },
        { name: 'lastName' },
        { name: 'middleName' },
        { name: 'dob' },
        { name: 'city' },
        { name: 'state' },
        { name: 'zipCode' },

        // Birls
        { name: 'branchOfService' },
        { name: 'serviceNumber' },
        { name: 'insuranceNumber' },
        { name: 'dod' },
        { name: 'eod' },
        { name: 'rad' },
        { name: 'suffix' },
        { name: 'folderLocation' },
        { name: 'fileNumber' },
        { name: 'payeeNumber' },

        // Appeals
        { name: 'appealsSearchValue' },  //true or false depending on additional search criteria from CRM
        { name: 'appealsSsn' },        
        { name: 'appealsFirstName' },
        { name: 'appealsLastName' },
        { name: 'appealsDateOfBirth' },
        { name: 'appealsCity' },
        { name: 'appealsState' },


        // MVI/Pathways
        { name: 'gender' },
        { name: 'appointementFromDate' },
        { name: 'appointementToDate' },
        { name: 'doSearchPathways' },

        // Vadir
        {name: 'doSearchVadir' }
        
    ]
});