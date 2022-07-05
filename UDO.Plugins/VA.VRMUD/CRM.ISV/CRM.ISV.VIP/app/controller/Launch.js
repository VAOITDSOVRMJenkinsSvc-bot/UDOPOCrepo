/**
* @author Dmitri Riz
* @class VIP.controller.Launch
*
*/
Ext.define('VIP.controller.Launch', {
    extend: 'Ext.app.Controller',

    refs: [{
        ref: 'launch',
        selector: 'launch'
    }, {
        ref: 'parentLaunch',
        selector: 'crmlaunch'
    }, {
        ref: 'userName',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="userName"]'
    }, {
        ref: 'password',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="password"]'
    }, {
        ref: 'clientMachine',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="clientMachine"]'
    }, {
        ref: 'stationId',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="stationId"]'
    }, {
        ref: 'applicationName',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="applicationName"]'
    }, {
        ref: 'pcrSensitivityLevel',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="pcrSensitivityLevel"]'
    }, {
        ref: 'loginName',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="loginName"]'
    }, {
        ref: 'userSsn',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="ssn"]'
    }, {
        ref: 'userFileNumber',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="fileNumber"]'
    }, {
        ref: 'envComboBox',
        selector: 'launch > panel[itemId="userContext"] combobox[itemId="environment"]'
    }, {
        ref: 'email',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="email"]'
    }, {
        ref: 'pcrId',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="pcrId"]'
    }, {
        ref: 'site',
        selector: 'launch > panel[itemId="userContext"] textfield[itemId="site"]'
    }, {
        ref: 'firstName',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="firstName"]'
    }, {
        ref: 'lastName',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="lastName"]'
    }, {
        ref: 'middleName',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="middleName"]'
    }, {
        ref: 'dob',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="dob"]'
    }, {
        ref: 'city',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="city"]'
    }, {
        ref: 'state',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="state"]'
    }, {
        ref: 'zipCode',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="zipCode"]'
    }, {
        ref: 'branchOfService',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="branchOfService"]'
    }, {
        ref: 'serviceNumber',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="serviceNumber"]'
    }, {
        ref: 'insuranceNumber',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="insuranceNumber"]'
    }, {
        ref: 'dod',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="dod"]'
    }, {
        ref: 'eod',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="eod"]'
    }, {
        ref: 'rad',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="rad"]'
    }, {
        ref: 'suffix',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="suffix"]'
    }, {
        ref: 'folderLocation',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="folderLocation"]'
    }, {
        ref: 'fileNumber',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="fileNumber"]'
    }, {
        ref: 'participantId',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="participantId"]'
    }, {
        ref: 'appealsSearchValue',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="appealsSearchValue"]'
    }, {
        ref: 'appealsSsn',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="appealsSsn"]'
    }, {
        ref: 'appealsLastName',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="appealsLastName"]'
    }, {
        ref: 'appealsFirstName',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="appealsFirstName"]'
    }, {
        ref: 'appealsDateOfBirth',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="appealsDateOfBirth"]'
    }, {
        ref: 'appealsCity',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="appealsCity"]'
    }, {
        ref: 'appealsState',
        selector: 'launch > panel[itemId="searchCriteria"] textfield[itemId="appealsState"]'
    }],

    models: ['Pcr', 'PersonInquiry'],

    init: function () {
        var me = this;

        me.control({
            'launch > panel[itemId="searchCriteria"] button[action="search"]': {
                click: me.search
            },
            'launch > panel[itemId="userContext"] combobox[itemId="environment"]': {
                select: me.updateUserContext
            }
        });

        me.application.on({
            multiplepeoplefound: me.hideLaunch,
            personinquirysuccess: me.hideLaunch,
            personinquirynoresults: me.showLaunch,
            scope: me
        });

        Ext.log('The Launch controller has been successfully initialized.');
    },

    updateUserContext: function (combo, records, eOpts) {
        var me = this;

        if (combo.getValue() == 'PROD') {
            me.getUserName().setValue('NCCBFURM');
            me.getApplicationName().setValue('CRMUD');
            me.getStationId().setValue('331');
            me.getClientMachine().setValue('10.198.1.69');
        }
        else {
            me.getUserName().setValue('281CEASL');
            me.getApplicationName().setValue('VBMS');
            me.getStationId().setValue('281');
            me.getClientMachine().setValue('10.224.104.174');
        }
    },

    onLaunch: function () {
        var me = this;
        me.showLaunch();
    },

    hideLaunch: function () {
        var lp = this.getLaunch();
        if (!Ext.isEmpty(lp)) {
            lp.collapse('top');
        }
    },

    showLaunch: function () {
        var lp = this.getLaunch();
        if (!Ext.isEmpty(lp)) {
            lp.expand();
        }
    },


    search: function () {
        var me = this;
        //This is so it will always use the proper environment context before search.
        //Only needed when debugging locally
        //me.updateUserContext(me.getEnvComboBox());

        Ext.data.StoreManager.each(function (item, index, length) {
            item.removeAll();
        });

        if (me.getUserName) {
            var userName = me.getUserName().getValue(),
			password = me.getPassword().getValue(),
			clientMachine = me.getClientMachine().getValue(),
			stationId = me.getStationId().getValue(),
			applicationName = me.getApplicationName().getValue(),
			pcrSensitivityLevel = me.getPcrSensitivityLevel().getValue(),
			loginName = me.getLoginName().getValue(),
			userSsn = me.getUserSsn().getValue(),
			userFileNumber = me.getUserFileNumber().getValue(),
			environmentName = me.getEnvComboBox().getValue(),
			email = me.getEmail().getValue(),
			pcrId = me.getPcrId().getValue(),
			site = me.getSite().getValue(),
			firstName = me.getFirstName().getValue(),
			lastName = me.getLastName().getValue(),
			middleName = me.getMiddleName().getValue(),
			dob = me.getDob().getValue(),
			city = me.getCity().getValue(),
			state = me.getState().getValue(),
			zipCode = me.getZipCode().getValue(),
			branchOfService = me.getBranchOfService().getValue(),
			serviceNumber = me.getServiceNumber().getValue(),
			insuranceNumber = me.getInsuranceNumber().getValue(),
			dod = me.getDod().getValue(),
			eod = me.getEod().getValue(),
			rad = me.getRad().getValue(),
			suffix = me.getSuffix().getValue(),
			folderLocation = me.getFolderLocation().getValue(),
			fileNumber = me.getFileNumber().getValue(),
			participantId = me.getParticipantId().getValue(),
			appealsSearchValue = me.getAppealsSearchValue().getValue(),
			appealsSsn = me.getAppealsSsn().getValue(),
			appealsLastName = me.getAppealsLastName().getValue(),
			appealsFirstName = me.getAppealsFirstName().getValue(),
			appealsDateOfBirth = me.getAppealsDateOfBirth().getValue(),
			appealsCity = me.getAppealsCity().getValue(),
			appealsState = me.getAppealsState().getValue();
        }
        var pcrModel, personInquiryModel;

        if (parent && parent._vipEntryPoint && parent.Xrm) {
            // user data from parent screen
            if (parent._vipSearchContext) {
                var userName = parent._vipSearchContext.user.userName,
					password = '',
					clientMachine = parent._vipSearchContext.user.clientMachine,
					stationId = parent._vipSearchContext.user.stationId,
					applicationName = parent._vipSearchContext.user.applicationName,
					pcrSensitivityLevel = parent._vipSearchContext.user.pcrSensitivityLevel,
					loginName = parent._vipSearchContext.user.loginName,
					userSsn = parent._vipSearchContext.user.ssn,
					userFileNumber = parent._vipSearchContext.user.fileNumber,
					userFullName = parent._vipSearchContext.user.fullName,
					email = parent._vipSearchContext.user.email,
					pcrId = parent._vipSearchContext.user.pcrId,
					site = parent._vipSearchContext.user.site;
            }

            // search parameters
            var firstName = parent.Xrm.Page.getAttribute('va_firstname').getValue(),
				lastName = parent.Xrm.Page.getAttribute('va_lastname').getValue(),
				middleName = parent.Xrm.Page.getAttribute('va_middleinitial').getValue(),

                dobDate = (parent.Xrm.Page.getAttribute('va_dobtext') && parent.Xrm.Page.getAttribute('va_dobtext').getValue() != null) ? new Date(parent.Xrm.Page.getAttribute('va_dobtext').getValue()) : null,
				dob = dobDate ? dobDate.format("MMddyyyy") : null,
				city = parent.Xrm.Page.getAttribute('va_citysearch').getValue(),
				state = parent.Xrm.Page.getAttribute('va_statesearch').getValue(),
				zipCode = parent.Xrm.Page.getAttribute('va_zipcodesearch').getValue(),

				branchOfService = parent.Xrm.Page.getAttribute('va_branchofservice').getValue(),
				serviceNumber = parent.Xrm.Page.getAttribute('va_servicenumber').getValue(),
				insuranceNumber = parent.Xrm.Page.getAttribute('va_insurancenumber').getValue(),
				dod = parent.Xrm.Page.getAttribute('va_dod').getValue() ? parent.Xrm.Page.getAttribute('va_dod').getValue().format("MMddyyyy") : null,
				eod = parent.Xrm.Page.getAttribute('va_enteredondutydate').getValue() ? parent.Xrm.Page.getAttribute('va_enteredondutydate').getValue().format("MMddyyyy") : null,
				rad = parent.Xrm.Page.getAttribute('va_releasedactivedutydate').getValue() ? parent.Xrm.Page.getAttribute('va_releasedactivedutydate').getValue().format("MMddyyyy") : null,
				suffix = parent.Xrm.Page.getAttribute('va_suffix').getValue(),
				folderLocation = parent.Xrm.Page.getAttribute('va_folderlocation').getValue(),
				payeeNumber = parent.Xrm.Page.getAttribute('va_payeenumber').getValue(),

			fileNumber = parent.Xrm.Page.getAttribute('va_ssn').getValue(),
			participantId = parent.Xrm.Page.getAttribute('va_participantid').getValue(),
			appealsSearchValue = parent.Xrm.Page.getAttribute('va_findappealsby').getValue(),
			appealsSsn = parent.Xrm.Page.getAttribute('va_appealsssn').getValue(),
			appealsLastName = parent.Xrm.Page.getAttribute('va_appealslastname').getValue(),
			appealsFirstName = parent.Xrm.Page.getAttribute('va_appealsfirstname').getValue(),
			appealsDateOfBirth = parent.Xrm.Page.getAttribute('va_appealsdateofbirth').getValue(),
			appealsCity = parent.Xrm.Page.getAttribute('va_appealscity').getValue(),
			appealsState = parent.Xrm.Page.getAttribute('va_appealsstate').getValue();

            //if (appealsSearchValue == 953850002) { appealsSsn = parent.Xrm.Page.getAttribute('va_appealsssn').getValue(); }
        }

        pcrModel = Ext.create('VIP.model.Pcr', {
            userName: userName,
            password: password,
            clientMachine: clientMachine,
            stationId: stationId,
            applicationName: applicationName,
            pcrSensitivityLevel: pcrSensitivityLevel,
            loginName: loginName,
            ssn: userSsn,
            fileNumber: userFileNumber,
            fullName: userFullName,
            email: email,
            pcrId: pcrId,
            site: site
        });

        personInquiryModel = Ext.create('VIP.model.PersonInquiry', {
            firstName: firstName,
            lastName: lastName,
            middleName: middleName,
            dob: dob,
            city: city,
            state: state,
            zipCode: zipCode,
            branchOfService: branchOfService,
            serviceNumber: serviceNumber,
            insuranceNumber: insuranceNumber,
            dod: dod,
            eod: eod,
            rad: rad,
            suffix: suffix,
            folderLocation: folderLocation,
            fileNumber: fileNumber,
            participantId: participantId,
            payeeNumber: payeeNumber,
            appealsSearchValue: appealsSearchValue,
            appealsSsn: appealsSsn,
            appealsLastName: appealsLastName,
            appealsFirstName: appealsFirstName,
            appealsDateOfBirth: appealsDateOfBirth,
            appealsCity: appealsCity,
            appealsState: appealsState,
            payeeNumber: payeeNumber,
            doSearchVadir: true
        });

        var theApp = me.application;
        if (Ext.isEmpty(theApp)) {
            theApp = _extApp;
        }
        // Set execution context
        theApp.pcrContext = pcrModel;
        theApp.personInquiryModel = personInquiryModel;
        theApp.crmEnvironment = theApp.initializeEnvironment(me.getEnvironment(environmentName));
        theApp.useCachedResponseData = false;
        _extApp = theApp;

        //Will fire events in the ContentPanel controller and the PersonSelection controller
        me.application.fireEvent('personinquirystarted', personInquiryModel);
        me.application.fireEvent('calltimerstarted');

        Ext.log('A personinquirystarted event has been fired by the Launch controller');
    },

    getEnvironment: function (envName) {
        var _environments = [{
            name: 'INTI',
            isPROD: false,
            globalDAC: 'http://10.153.95.73/RedirectSvc.asmx',
            CORP: 'http://beplinktest.vba.va.gov/',
            VADIR: 'http://vavdrapp80.aac.va.gov/',
            MVI: 'psim_webservice/IdMWebService',
            MVIDAC: 'http://10.153.95.73/RedirectSvc.asmx',
            MVIBase: 'https://int.services.eauth.va.gov:9193/',
            Pathways: 'http://vahdrtvapp02.aac.va.gov:7251/repositories.med.va.gov/pathways',
            PathwaysBase: 'http://vahdrtvapp02.aac.va.gov:7251/',
            Vacols: 'http://vaausvrsapp81.aac.va.gov/VIERSService/v1/AppealService/Appeal',
            VacolsDAC: 'http://10.153.95.73/RedirectSvc.asmx',
            VacolsBase: 'http://vaausvrsapp81.aac.va.gov/',
            RepWS: 'http://10.153.96.36/ReportGen/ReportGen.asmx',
            UseMSXML: false,
            EbenefitsBase: 'http://vaebnweb2.aac.va.gov/',
            VVABase: 'https://vbaphi5topp.vba.va.gov:7002/',
            VVAUser: '',
            VVAPassword: ''
        }];

        var _currentEnv = null;
        for (var i in _environments) {
            if (envName == _environments[i].name) {
                _currentEnv = _environments[i];
                break;
            }
        }
        return _currentEnv;
    }
});
