/**
* @class VIP.controller.PersonVadir
*
* The controller for the application status bar.
*/
Ext.define('VIP.controller.PersonVadir', {
    extend: 'Ext.app.Controller',

    stores: ['personVadir.ContactInfo', 'PersonVadir', 'personVadir.Address', 'personVadir.Alias', 'personVadir.Email', 'personVadir.Phone'],
    mixins: {
        sec: 'VIP.mixin.CRMRoleSecurity'
    },
    refs: [{
        ref: 'PersonVadirGrid',
        selector: '[xtype="personvadir.person"]'
    }, {
        ref: 'personDetailsAliasGrid',
        selector: '[xtype="personvadir.details.alias"]'
    }, {
        ref: 'personDetailsAddressesGrid',
        selector: '[xtype="personvadir.details.addresses"]'
    }, {
        ref: 'personDetailsEmailsGrid',
        selector: '[xtype="personvadir.details.emails"]'
    }, {
        ref: 'personDetailsPhonesGrid',
        selector: '[xtype="personvadir.details.phones"]'
    }, {
        ref: 'personVadirDetailsTab',
        selector: '[xtype="personvadir.details"]'
    }, {
        ref: 'personVadirTab',
        selector: '[xtype="persontabpanel"]'
    }, {
        ref: 'serviceRequestButton',
        selector: 'button[action="startservicerequest"]'
    }],

    personNotFoundCorpBirls: false,

    init: function () {
        var me = this;

        me.control({
            '[xtype="personvadir.person"]': {
                selectionchange: me.onPersonGridSelection
            },
            '[xtype="personvadir.details"]': {
                tabchange: me.onTabChange
            },
            '[xtype="personvadir.person"] > toolbar > button[action="createservicerequest"]': {
                click: me.serviceRequest
            }
        });

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            dopersonvadirsearch: me.onPersonVadirSearch,
            triggerpersonvadirtabchange: me.onTabChange,
            scope: me
        });

    },

    onPersonVadirSearch: function (selectedPerson) {
        var me = this;
        me.personNotFoundCorpBirls = true;
        me.loadPerson(selectedPerson)
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this;
        me.personNotFoundCorpBirls = false;

        if (me.application.personInquiryModel.get('doSearchVadir')) {
            me.loadPerson(selectedPerson);
        }
    },

    loadPerson: function (selectedPerson) {
        var me = this;
        me.getPersonVadirGrid().setLoading(true, true);

        me.getPersonVadirStore().load({
            filters: [{
                property: 'ssn',
                value: selectedPerson.get('fileNumber')
            },
                {
                    property: 'edipi',
                    value: selectedPerson.get('edipi')
                },
                {
                    property: 'firstName',
                    value: selectedPerson.get('firstName')
                },
                {
                    property: 'lastName',
                    value: selectedPerson.get('lastName')
                },
                {
                    property: 'dob',
                    value: selectedPerson.get('dob')
                }
            ],

            callback: function (records, operation, success) {
                me.getPersonVadirGrid().setLoading(false);

                if (success && !Ext.isEmpty(records)) {
                    me.getPersonVadirGrid().reconfigure(me.getPersonVadirStore());

                    if (records.length === 1) {
                        var person = records[0];

                        if (!Ext.isEmpty(person)) {
                            me.LoadPersonAlias(person);
                            me.LoadPersonDetails(person);
                        }
                    }
                } else {
                    if (me.personNotFoundCorpBirls) {
                        me.application.fireEvent('personinquirynoresults');
                    }
                }

                me.application.fireEvent('webservicecallcomplete', operation, 'PersonVadir');
            },
            scope: me
        });
    },

    LoadPersonAlias: function (person) {
        var me = this;
        if (!Ext.isEmpty(person.aliasesStore)) {
            me.getPersonDetailsAliasGrid().reconfigure(person.aliasesStore);
        }
    },

    LoadPersonDetails: function (person) {
        var me = this;
        me.getPersonDetailsAddressesGrid().setLoading(true, true);
        me.getPersonDetailsEmailsGrid().setLoading(true, true);
        me.getPersonDetailsPhonesGrid().setLoading(true, true);

        me.getPersonVadirContactInfoStore().load({
            filters: [{
                property: 'edipi',
                value: person.get('vaId')
            }],

            callback: function (records, operation, success) {
                me.getPersonDetailsAddressesGrid().setLoading(false);
                me.getPersonDetailsEmailsGrid().setLoading(false);
                me.getPersonDetailsPhonesGrid().setLoading(false);

                if (success && !Ext.isEmpty(records)) {
                    if (records.length === 1) {
                        me.getPersonDetailsAddressesGrid().reconfigure(me.getPersonVadirContactInfoStore().getAt(0).AddressesStore);
                        me.getPersonDetailsEmailsGrid().reconfigure(me.getPersonVadirContactInfoStore().getAt(0).EmailsStore);
                        me.getPersonDetailsPhonesGrid().reconfigure(me.getPersonVadirContactInfoStore().getAt(0).PhonesStore);
                    }
                } else {
                    me.getPersonDetailsAddressesGrid().reconfigure(Ext.create('VIP.store.personVadir.Address'));
                    me.getPersonDetailsEmailsGrid().reconfigure(Ext.create('VIP.store.personVadir.Email'));
                    me.getPersonDetailsPhonesGrid().reconfigure(Ext.create('VIP.store.personVadir.Phone'));
                }

                me.application.fireEvent('webservicecallcomplete', operation, 'personVadir.ContactInfo');
            },
            scope: me
        });
    },

    onPersonGridSelection: function (selection, persons, index) {
        var me = this, emptyPerson;

        if (!Ext.isEmpty(selection) && !Ext.isEmpty(persons)) {
            me.LoadPersonAlias(persons[0]);
            me.LoadPersonDetails(persons[0]);
            //me.serviceRequest()
        }
    },

    onTabChange: function (tabPanel, newCard, oldCard, eOpts) {
        var me = this, firstTab, secondTab, statisticText;

        if (me.UserHasRole('VR%26E') || me.UserHasRole('DMC') || me.UserHasRole('IPC')) {
            if (Ext.get('id_personVadir_Person_01')) Ext.get('id_personVadir_Person_01').hide();
        }

        if (tabPanel.xtype === 'persontabpanel') {
            firstTab = newCard;
            secondTab = me.getPersonVadirDetailsTab().activeTab;
        } else {
            secondTab = newCard;
            firstTab = me.getPersonVadirTab().activeTab;
        }

        statisticText = firstTab.title + ': ' + firstTab.items.items[0].getStore().getCount() + ', ' +
                        secondTab.title + ': ' + secondTab.items.items[0].getStore().getCount();

        me.application.fireEvent('setstatisticstext', statisticText, null);
    },

    GetSelectedVadirRecord: function (promptIfNotSelected) {
        var me = this;
        var vadirGrid = me.getPersonVadirGrid(); //vadir grid
        var selectedRecords = vadirGrid.getSelectionModel().getSelection();
        if (Ext.isEmpty(selectedRecords)) {
            if (promptIfNotSelected) {
                Ext.Msg.alert('No Vadir Record Selected', 'Please select a record from the grid first.');
            }
            return null;
        }

        var rec = selectedRecords[0];
        return rec;
    },

    serviceRequest: function (button) {
        var me = this,
            selectedVadir = me.GetSelectedVadirRecord(true);

        if (selectedVadir == null) return;

        if (!(parent && parent.CrmRestKit2011)) {
            Ext.Msg.alert('CRM Not Loaded', 'This feature works only in context of CRM screen hosting the UI');
            return;
        }
        else {
            Ext.Msg.confirm('Confirm Action', 'Please confirm that you would like to create a Vadir service request', function (button) {
                if (button == 'no') return;
                else me.fireServiceRequestEvent(selectedVadir);
            });
        }
    },

    fireServiceRequestEvent: function (selectedVadir) {
        var me = this,
        //This json object will be sent to the CRM Restkit
        data = {
            "va_ECCSSN": selectedVadir.get('ssn'),
            "va_SSN": selectedVadir.get('ssn'),
            "va_ECCFirstName": selectedVadir.get('firstName'),
            "va_ECCMiddleName": selectedVadir.get('middleName'),
            "va_ECCLastName": selectedVadir.get('lastName'),
            "va_ECCAddress1": '',
            "va_ECCAddress2": '',
            "va_ECCCity": '',
            "va_ECCState": '',
            "va_ECCZip": '',
            "va_ECCCountry": ''
        };

        //Added Global Vars 2/23/13
        me.application.serviceRequest.va_ServiceRequestType = 'ECC';
        me.application.serviceRequest.va_SelectedSSN = selectedVadir.get('ssn');

        if (!Ext.isEmpty(me.getPersonDetailsAddressesGrid().getStore())) {
            vadirAddress = me.getPersonDetailsAddressesGrid().getStore();
            vadirAddress.each(function (record) {
                if (record.get('addressType') === 'M') {
                    data.va_ECCAddress1 = record.get('addressLine1');
                    data.va_ECCAddress2 = record.get('addressLine2');
                    data.va_ECCCity = record.get('city');
                    data.va_ECCState = record.get('state');
                    data.va_ECCZip = record.get('zipcode');
                    data.va_ECCCountry = record.get('countryCode');
                }
            });
        }
        //Catapult: To be filled out when new ECC Phone Call Type and Sub Type fields are created.        
        //        if (parent.Xrm.Page.data.entity.getEntityName() == 'phonecall') {
        //            relatedClaimSummary = relatedClaimSummary + " - " +
        //                (parent.Xrm.Page.getAttribute('va_dispositionsubtype').getSelectedOption() ?
        //                    parent.Xrm.Page.getAttribute('va_dispositionsubtype').getSelectedOption().text : '');
        //        }

        //Replace with RPO Code.
        //        if (lifeCycleModel) {
        //            data.va_RegionalOfficeId = me.getStationOfJurisdiction(lifeCycleModel.get('stationOfJurisdiction'));
        //        }

        //This event is caught in the Service Request controller
        me.application.fireEvent('createvadirservicerequest', data);
    }
});