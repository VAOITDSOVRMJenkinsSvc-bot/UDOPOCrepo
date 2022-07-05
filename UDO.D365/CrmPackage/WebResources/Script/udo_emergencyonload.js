﻿var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib;
var webApi;
var formHelper;

function instantiateCommonScripts(exCon) {
    lib = new CrmCommonJS.CrmCommon(version, exCon);
    webApi = lib.WebApi;
    formHelper = new CrmCommonJS.FormHelper(exCon);
}

function OnLoad(exCon) {
    instantiateCommonScripts(exCon);

    var formContext = exCon.getFormContext();
    if ((parent.window.IsUSD !== true) && (window.IsUSD !== true)) {
        var tab = formContext.ui.tabs.get("GeneralTab");
        tab.setVisible(true);

        //var fname = formContext.getAttribute("va_firstname");
        //var lname = formContext.getAttribute("va_lastname");

        var fname = formHelper.getAttribute("va_firstname");
        var lname = formHelper.getAttribute("va_lastname");
        if ((fname !== null) && (lname !== null)) {
            var fnameStr = fname.getValue();
            if (fnameStr === null) {
                fname.setValue("Emergency");
            }
            var lnameStr = lname.getValue();
            if (lnameStr === null) {
                lname.setValue("Emergency");
            }
        }
        Va.Udo.Crm.Scripts.Code.EmergencyBtn.EmergencySave(null);
    }
}

function retrieveFormContext(exCon) {
    if (!!exCon) {
        formContext = exCon.getFormContext();
    }
}
//declare Namespaces
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Code = Va.Udo.Crm.Scripts.Code || {};


Va.Udo.Crm.Scripts.Code.EmergencyBtn = {
    EButton: null,
    CancelButton: null,

    OnEmergencySelect: function (exCon) {
        instantiateCommonScripts(exCon);
        var etype = formHelper.getAttribute("va_emergencytype");
        var nameAttrib = formHelper.getAttribute("va_name");
        nameAttrib.setValue(etype.getText());
        this.EmergencySave(exCon);
    },

    EmergencySave: function (exCon) {
        if (exCon) {
            instantiateCommonScripts(exCon);
        }
        var etype = formHelper.getAttribute("va_emergencytype");
        if (etype !== null) {
            var evalue = etype.getValue();
            if (evalue === null || evalue === "") formHelper.setValue("va_emergencytype", 953850003);
        }
        //formContext.data.entity.save("save");
        formHelper.saveRecord(true);
    },
    MakeEmergencyButton: function () {
        var fieldName = "va_emergencybtn";
        var fieldId = "field" + fieldName;
        if (document.getElementById(fieldId) == null) {
            var elementId = document.getElementById(fieldName + "_d");
            var div = document.createElement("div");
            div.style.width = "100px";
            div.style.textAlign = "right";
            div.style.display = "inline";
            elementId.appendChild(div, elementId);
            div.innerHTML = '<button id="' + fieldId + '"  type="button" style="margin-left: 3px; width: 100%;" >Start Emergency Chat</button>';
            document.getElementById(fieldName).style.width = "0%";
            document.getElementById(fieldId).onclick = function () { Va.Udo.Crm.Scripts.Code.EmergencyBtn.OnIMButtonClick(); };
            this.EButton = document.getElementById(fieldId);
        }
    },
    MakeCancelButton: function () {
        var fieldName = "va_cancelbutton";
        var fieldId = "field" + fieldName;
        if (document.getElementById(fieldId) == null) {
            var elementId = document.getElementById(fieldName + "_d");
            var div = document.createElement("div");
            div.style.width = "100px";
            div.style.textAlign = "right";
            div.style.display = "inline";
            elementId.appendChild(div, elementId);
            div.innerHTML = '<button id="' + fieldId + '"  type="button" style="margin-left: 3px; width: 100%;" >Cancel</button>';
            document.getElementById(fieldName).style.width = "0%";
            document.getElementById(fieldId).onclick = function () { Va.Udo.Crm.Scripts.Code.EmergencyBtn.CancelEmergency(); };
            this.CancelButton = document.getElementById(fieldId);
        }
    },

    CancelEmergency: function () {
        var etype = Xrm.Page.getAttribute("va_emergencytype");
        etype.setValue(1);
        var nameAttrib = Xrm.Page.getAttribute("va_name");
        nameAttrib.setValue("**Cancelled**");
        //Xrm.Page.data.entity.save("saveandclose");
        Xrm.Page.data.entity.save("save");
        //window.open("http://uii/emergency/close");

    },
    restKitError: function (err, message) {
        if (err.responseText) {
            alert(message + '\r\n' + $.parseJSON(err.responseText).error.message.value);
        } else {
            alert(message);
        }
    },
    OnIMButtonClick: function () {
        var _emergencyOptions = {
            'recipients': [],
            'newRecipients': '',
            'message': '',
            'isCancel': false,
            'type': 'radEmergency'
        };
        CrmRestKit2011.Retrieve('SystemUser', Xrm.Page.context.getUserId(), ['va_WSLoginName', 'FullName', 'SiteId'])
			.done(function (data) {
			    var filter = 'va_Site/Id eq guid\'' + data.d.SiteId.Id + '\' and va_name eq \'NCC Emergency List\'';
			    CrmRestKit2011.ByQuery('va_sitesteams', ['va_Team'], filter)
				.done(function (data1) {
				    var noRecipientMessage = 'No recipients to deliver the emergency notification were found for site "' + data.d.SiteId.Name + '"';
				    var columns1 = ['teammembership_association/InternalEMailAddress'];
				    var expand1 = 'teammembership_association';
				    try {
				        var filter1 = 'TeamId eq guid\'' + data1.d.results[0].va_Team.Id + '\'';
				    } catch (err) {
				        Va.Udo.Crm.Scripts.Code.EmergencyBtn.restKitError(err, 'The PCR Site (NCC) does not have a Team assigned!\r\n' +
							'A Site (NCC) must have an emergency Team assigned in order to get the list of recipients to deliver the emergency notification!');
				        return;
				    }

				    CrmRestKit2011.ByExpandQuery('Team', columns1, expand1, filter1)
					.done(function (data2) {
					    var recipients = data2.d.results[0].teammembership_association.results;
					    for (var i = 0; i < recipients.length; i++) {
					        if (recipients[i] && recipients[i].InternalEMailAddress)
					            _emergencyOptions.recipients.push(recipients[i].InternalEMailAddress);
					    }
					    if (_emergencyOptions.recipients.length === 0) {
					        alert(noRecipientMessage);
					        return;
					    }
					    Va.Udo.Crm.Scripts.Code.EmergencyBtn.OpenEmergencyWindow(_emergencyOptions.recipients);
					}
					).fail(function (error) {
					    Va.Udo.Crm.Scripts.Code.EmergencyBtn.restKitError(error, noRecipientMessage);
					    return;
					});
				}
				).fail(function (err) {
				    Va.Udo.Crm.Scripts.Code.EmergencyBtn.restKitError(err, 'The PCR Site (NCC) does not have a Team assigned!\r\n' +
							'A Site (NCC) must have an emergency Team assigned in order to get the list of recipients to deliver the emergency notification!');
				});
			}).fail(function (error) {
			    Va.Udo.Crm.Scripts.Code.EmergencyBtn.restKitError(error, 'The PCR does not have a site assigned!\r\n' +
					'A Site must be assigned in order to get the list of recipients to deliver the emergency notification!');
			});
    },

    ValidEmail: function (emailstr) {
        //basic regex to validate that string at least looks like an email address
        var emailRegEx = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
        return emailRegEx.test(emailstr);
    },

    RefreshTab: function (context) {
        // Hide and show tab workaround in order to get UCI to display page from top instead of first field 
        var tab = context.ui.tabs.get("GeneralTab");
        tab.setVisible(true);        
    },

    OpenEmergencyWindow: function (emails) {

        //Make sure entity is saved before we start IM Session	
        //subject and emergency type are required fields so make
        //sure they are set to something
        var subject = Xrm.Page.getAttribute("va_name");
        if (subject != null) {
            var subjectStr = subject.getValue();
            if (subjectStr == null) {
                subject.setValue("Unknown Emergency");
            }
        }

        this.EmergencySave();
        //Validate that we have email addresses that are useable
        //Lync will refuse to run even if only 1 of the group addresses is bad
        if (typeof emails == 'undefined' || emails == null) { return; }
        var sipString = 'im:';
        for (var i = 0; i < emails.length; i++) {
            if (this.ValidEmail(emails[i])) {
                sipString += '<sip:' + emails[i] + '>';
            }
        }

        //Check for and validate any additional emails entered by the user
        var emailBox = Xrm.Page.getAttribute("va_additionalemails");
        if (emailBox.getValue() != null) {
            var newEmails = emailBox.getValue().split(/\r\n|\n/g);
            for (var i = 0; i < newEmails.length; i++) {
                if (this.ValidEmail(newEmails[i])) {
                    sipString += '<sip:' + newEmails[i] + '>';
                }
            }
        }

        //Call Lync or Skype - which ever is installed
        window.open(sipString);
        //this.EButton.disabled = true;

    }
};