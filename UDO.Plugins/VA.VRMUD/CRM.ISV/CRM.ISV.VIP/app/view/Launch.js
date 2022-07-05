/**
* @author Josh Oliver
* @class VIP.view.Launch
*
* The launch panel for the application. Precedes any searches.
*/
Ext.define('VIP.view.Launch', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.launch',
    title: 'Welcome',
    initComponent: function () {
        var me = this;
        //***IMPORTANT*** stationId and username must match,  VACOGROSSJ-317.  281CEASL-281.
        // VACOGROSSJ will result in security violations in some cases...aka Adrian Ray
        // PROD DATA: CRMUD NCCBFURM 331 10.198.1.69 PROD
        me.items = [
            {
                xtype: 'panel',
                title: 'User Context',
                layout: 'column',
                itemId: 'userContext',
                padding: '0 0 5 0',
                bodyStyle: {
                    padding: '5'
                },
                items: [
                    {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        items: [{
                            xtype: 'textfield',
                            fieldLabel: 'User Name',
                            value: 'VACOGROSSJ',
                            allowBlank: false,
                            itemId: 'userName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Password',
                            value: '',
                            itemId: 'password'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Client Machine',
                            value: '10.224.104.174',
                            allowBlank: false,
                            itemId: 'clientMachine'
                        }]
                    },
                    {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Station ID',
                            value: '317',
                            allowBlank: false,
                            itemId: 'stationId'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Application Name',
                            value: 'VBMS',
                            allowBlank: false,
                            itemId: 'applicationName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'PCR Sensitivity Level',
                            value: '9',
                            allowBlank: false,
                            itemId: 'pcrSensitivityLevel'
                        }]
                    },
                    {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Login Name',
                            value: 'aide\\jdawson',
                            allowBlank: false,
                            itemId: 'loginName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'SSN',
                            value: '45678',
                            allowBlank: false,
                            itemId: 'ssn'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'File Number',
                            value: '45678',
                            allowBlank: false,
                            itemId: 'fileNumber'
                        }]
                    },
                    {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        items: [
                        {
                            xtype: 'combobox',
                            fieldLabel: 'Enviro. Name',
                            store: ['INTI'],
                            queryMode: 'local',
                            valueField: 'value',
                            allowBlank: false,
                            itemId: 'environment',
                            value: 'INTI'

                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Email',
                            value: 'jonasd@infostrat.com',
                            allowBlank: false,
                            itemId: 'email'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'PCR ID',
                            value: '600022589',
                            allowBlank: false,
                            itemId: 'pcrId'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Site',
                            value: 'VA Headquarters',
                            allowBlank: false,
                            itemId: 'site'
                        }]
                    }]
            },
            {
                xtype: 'panel',
                title: 'Search Criteria',
                layout: {
                    type: 'column'
                },
                bodyStyle: {
                    padding: '5'  
                },
                itemId: 'searchCriteria',
                items: [
                    {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: 'First Name',
                            value: 'JOHN',
                            itemId: 'firstName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Last Name',
                            value: 'SMITH',
                            itemId: 'lastName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Middle Name',
                            value: 'E',
                            itemId: 'middleName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'DOB',
                            value: '',
                            itemId: 'dob'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'City',
                            value: '',
                            itemId: 'city'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'State',
                            value: '',
                            itemId: 'state'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Zipcode',
                            value: '',
                            itemId: 'zipCode'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Appeals Last Name',
                            value: 'CARDER',
                            itemId: 'appealsLastName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Appeals City',
                            value: '',
                            itemId: 'appealsCity'
                        }]
                    },
                    {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Branch of Service',
                            value: '',
                            itemId: 'branchOfService'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Service Number',
                            value: '',
                            itemId: 'serviceNumber'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Insurance Number',
                            value: '',
                            itemId: 'insuranceNumber'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'DOD',
                            value: '',
                            itemId: 'dod'
                        },
                        {
                            xtype: 'datefield',
                            fieldLabel: 'Pathways Start Date',
                            value: '',
                            itemId: 'startDate'
                        },
                        {
                            xtype: 'combobox',
                            fieldLabel: 'Appeals Search Criteria',
                            store: ['fileNumber', 'ssn', 'values' ],
                            queryMode: 'local',
                            valueField: 'value',
                            allowBlank: false,
                            itemId: 'appealsSearchValue'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Appeals First Name',
                            value: 'WILBURN',
                            itemId: 'appealsFirstName'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Appeals State',
                            value: '',
                            itemId: 'appealsState'
                        }]
                    },
                    {
                        xtype: 'container',
                        padding: '0 5 0 0',
                        items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: 'EOD',
                            value: '',
                            itemId: 'eod'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'RAD',
                            value: '',
                            itemId: 'rad'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Suffix',
                            value: '',
                            itemId: 'suffix'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Folder Location',
                            value: '',
                            itemId: 'folderLocation'
                        },
                        {
                            xtype: 'datefield',
                            fieldLabel: 'Pathways Start Date',
                            value: '',
                            itemId: 'endDate'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Appeals SSN/File No.',
                            value: '',
                            itemId: 'appealsSsn'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Appeals Date of Birth',
                            value: '',
                            itemId: 'appealsDateOfBirth'
                        }]
                    },
                    {
                        xtype: 'container',
                        items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: 'File Number',
                            value: '796060339',
                            allowBlank: false,
                            itemId: 'fileNumber'
                        },
                        {
                            xtype: 'textfield',
                            fieldLabel: 'Participant ID',
                            value: '',
                            allowBlank: false,
                            itemId: 'participantId'
                        },
                        {
                            xtype: 'button',
                            text: 'Search',
                            action: 'search'
                        }]
                    }
                ]
            }
        ];

        me.callParent();
    }
});



    
