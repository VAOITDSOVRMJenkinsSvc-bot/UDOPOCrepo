/**
* @author Josh Oliver
* @class VIP.controller.Mvi
*
* The controller for the pathways tabs
*/
Ext.define('VIP.controller.Mvi', {
    extend: 'Ext.app.Controller',
    requires: [
        'VIP.util.xml.FragmentBuilder'
    ],
    stores: [
        'mvi.Patient',
        'mvi.CorrespondingIds'
    ],
    refs: [{
        ref: 'mviGrid',
        selector: '[xtype="pathways.mvi"]'
    }],
    init: function () {
        var me = this;

        me.application.on({
            individualidentified: me.onIndividualIdentified,
            cacheddataloaded: me.onCachedDataLoaded,
            scope: me
        });

        me.control({
            '[xtype="pathways.mvi"] > toolbar > button[action="searchEBenefits"]': {
                click: me.onSearchEBenefitsClick
            },
            '[xtype="pathways.mvi"] > toolbar > button[action="searchPathways"]': {
                click: me.onSearchPathwaysClick
            },
            '[xtype="pathways.mvi"] > toolbar > button[action="popupTips"]': {
                click: me.onPopupTipsClick
            }
        });
    },

    onCachedDataLoaded: function () {
        var me = this,
            mviStore = me.getMviPatientStore();

        if (!Ext.isEmpty(mviStore)) { me.getMviGrid().reconfigure(mviStore); }
    },

    onIndividualIdentified: function (selectedPerson) {
        var me = this,
            searchPathways = me.application.personInquiryModel.get('doSearchPathways');

        if (!Ext.isEmpty(selectedPerson.get('ssn'))) {
            me.getMviPatientStore().load({
                filters: [{
                    property: 'firstName',
                    value: selectedPerson.get('firstName')
                }, {
                    property: 'lastName',
                    value: selectedPerson.get('lastName')
                }, {
                    property: 'middleName',
                    value: selectedPerson.get('middleName')
                }, {
                    property: 'dob',
                    value: selectedPerson.get('dob')
                }, {
                    property: 'ssn',
                    value: selectedPerson.get('ssn')
                }, {
                    property: 'gender',
                    value: selectedPerson.get('gender')
                }],
                callback: function (records, operation, success) {
                    var patienRecord;
                    
                    if (success && !Ext.isEmpty(records)) {
                        var mviStore = me.getMviPatientStore(),
                        patientXmlObject;

                        if (records.length == 1) {
                            patientRecord = records[0];

                            if (!Ext.isEmpty(patientRecord)) {
                                me.getCoorespondingIdsRecord(patientRecord);

                                if (searchPathways) {
                                    me.application.fireEvent('mvirecordloaded', patientRecord.get('nationalId'));
                                }
                            }
                        }

                        mviStore.loadRecords(records, { addRecords: false });

                        patientXmlObject = me.createPatientXml(records);

                        if (!Ext.isEmpty(patientXmlObject) && !Ext.isEmpty(patientXmlObject.xml)) {
                            operation.response = patientXmlObject;
                        }
                    }
                    //else {
                    //    me.application.fireEvent('edipirecordloaded', null);
                    //}
                    me.application.fireEvent('webservicecallcomplete', operation, 'mvi.Patient');
                },
                scope: me
            });
        }
    },

    getCoorespondingIdsRecord: function (patientRecord) {
        var me = this,
            edipiRecord;

        me.getMviCorrespondingIdsStore().load({
            filters: [{
                property: 'nationalId',
                value: patientRecord.get('fullNationalId')
            }],
            callback: function (records, operation, success) {
                if (success && !Ext.isEmpty(records)) {
                    edipiRecord = records[0];
                    if (!Ext.isEmpty(edipiRecord)) {
                        me.application.fireEvent('edipirecordloaded', edipiRecord.get('edipi'));
                    }
                }

                me.application.fireEvent('webservicecallcomplete', operation, 'mvi.CorrespondingIds');

            },
            scope: me
        });

        return edipiRecord;
    },

    createPatientXml: function (patientRecords) {
        var patientObject = { People: [] },
            patientXmlFragmentBuilder,
            patientXmlObject;

        for (var i = 0; i < patientRecords.length; i++) {
            var patientRecord = patientRecords[i],
                personObject = { Person: patientRecord.data };

            patientObject.People.push(personObject);
        }

        patientXmlFragmentBuilder = Ext.create('VIP.util.xml.FragmentBuilder', {
            xmlFragment: patientObject,
            rootNodeName: 'People'
        });

        if (!Ext.isEmpty(patientXmlFragmentBuilder)) {
            patientXmlObject = patientXmlFragmentBuilder.parseXml();
        }

        return patientXmlObject;
    },

    onPopupTipsClick: function () {
        Ext.Msg.alert('Reminder', 'Do not forget to check "Search Exams/Appts" box next to the SSN/File No./Claim No* which is located under the Search Parameters area.<br/><br/>You can adjust search criteria (such as date range) by clicking on "More Search Options" checkbox.');
    },
    
    onSearchPathwaysClick: function () {
        var me = this,
            mviGrid = me.getMviGrid();

        if (!Ext.isEmpty(mviGrid.getSelectionModel())) {
            if (!mviGrid.getSelectionModel().hasSelection()) {
                return;
            }
            var patientRecord = mviGrid.getSelectionModel().getSelection()[0],
                nationalId = !Ext.isEmpty(patientRecord) ? patientRecord.get('nationalId') : '';

            if (!Ext.isEmpty(nationalId)) {
                me.application.fireEvent('mvirecordloaded', nationalId);
            }
        }
    },

    onSearchEBenefitsClick: function () {
        var me = this,
            mviGrid = me.getMviGrid();

        if (!Ext.isEmpty(mviGrid.getSelectionModel())) {
            if (!mviGrid.getSelectionModel().hasSelection()) {
                return;
            }
            var patientRecord = mviGrid.getSelectionModel().getSelection()[0];

            if (!Ext.isEmpty(patientRecord)) {
                me.getCoorespondingIdsRecord(patientRecord);
            }
        }
    }
});
