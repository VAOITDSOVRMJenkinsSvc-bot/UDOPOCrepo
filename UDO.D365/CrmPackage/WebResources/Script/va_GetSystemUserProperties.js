/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
"use strict";

var user = function () {
};
user.prototype.constructor = user;
user.prototype.userName;
user.prototype.password;
user.prototype.clientMachine;
user.prototype.stationId;
user.prototype.applicationName;
user.prototype.pcrSensitivityLevel;
user.prototype.loginName;
user.prototype.ssn;
user.prototype.fileNumber;
user.prototype.fullName;
user.prototype.email;
user.prototype.pcrId;
user.prototype.site;
user.prototype.mamager;

var GetUserSettingsForWebService = function () {

    var globCon = Xrm.Utility.getGlobalContext();
    var version = globCon.getVersion();
    var webApi = new CrmCommonJS.WebApi(version);
    var currentUserId = globCon.userSettings.userId.replace(/}/g, "").replace(/{/g, "");
    var columns = ['systemuserid', 'va_pcrsensitivitylevel', 'va_ipaddress', 'va_stationnumber', 'va_pcrssn',
        'va_wsloginname', 'va_filenumber', 'va_applicationname', 'domainname', 'fullname', 'internalemailaddress', 'mobilealertemail', '_siteid_value', '_parentsystemuserid_value'];

    return new Promise(function (resolve, reject) {
        webApi.RetrieveRecord(currentUserId, "systemuser", columns)
            .then(function (data) {
                console.log(data);
                var systemUserSettings, systemUser, slashPos;

                if (data) {
                    systemUserSettings = data;
                    systemUser = new user();

                    // User name comes as a Login name part of domain login. If va_WSLoginName is present, it is used as an override
                    var loginName = systemUserSettings["va_wsloginname"];
                    if (loginName && loginName.length > 0) {
                        systemUser.userName = loginName;
                    }
                        
                    else {
                        var domainName = systemUserSettings["domainname"];
                        slashPos = domainName.indexOf('\\');
                        systemUser.userName = domainName.substr(slashPos + 1);
                    }

                    // Per Cory and Johnny Kahn, password is never used by WS and is not needed systemUser.password = systemUserSettings.va_WSPassword;
                    systemUser.clientMachine = systemUserSettings["va_ipaddress"];
                    systemUser.stationId = systemUserSettings["va_stationnumber"];
                    systemUser.applicationName = systemUserSettings["va_applicationname"];
                    systemUser.pcrSensitivityLevel = (systemUserSettings["va_pcrsensitivitylevel"] ? systemUserSettings["va_pcrsensitivitylevel"] : 0);
                    systemUser.loginName = systemUserSettings["domainname"];
                    systemUser.ssn = systemUserSettings["va_filenumber"];
                    systemUser.fileNumber = systemUserSettings["va_filenumber"];
                    systemUser.fullName = systemUserSettings["fullname"];
                    var internalEmailAddress = systemUserSettings["internalemailaddress"];
                    systemUser.email = (internalEmailAddress && internalEmailAddress.length > 0 ? internalEmailAddress : systemUserSettings["mobilealertemail"]);
                    systemUser.pcrId = systemUserSettings["va_pcrssn"];

                    systemUser.site = systemUserSettings["_siteid_value"] && systemUserSettings["_siteid_value@OData.Community.Display.V1.FormattedValue"] ? systemUserSettings["_siteid_value@OData.Community.Display.V1.FormattedValue"] : '';
                    systemUser.manager = systemUserSettings["_parentsystemuserid_value"] && systemUserSettings["_parentsystemuserid_value@OData.Community.Display.V1.FormattedValue"] ? systemUserSettings["_parentsystemuserid_value@OData.Community.Display.V1.FormattedValue"] : '';

                    return resolve(systemUser);

                } else {
                    return reject(new Error('CRM did not return the current User!'));
                }
            }).catch(function (err) {
                var error = 'Failed to retrieve the User Settings.\r\nError: ' + err.message;
                if (err.status === 400 && err.responseText)
                    error += '\r\n' + $.parseJSON(err.responseText).error.message.value;
                //alert(error);
                return reject(new Error(error));
            });
    });
}
//GetUserSettingsForWebservice = GetUserSettingsForWebservice;

var contact = function () {
};
user.prototype.constructor = contact;
user.prototype.fileNumber;
user.prototype.contactId;

function GetContactInformation(id, exCon, callback) {
    var globCon = Xrm.Utility.getGlobalContext();
    var version = globCon.getVersion();
    var lib = new CrmCommonJS(version, exCon);
    var webApi = lib.webApi;
    var crmContact = new contact();
    
    var columns = ['contactId', 'va_filenumber'];
    
    webApi.RetrieveRecord(id, "Contact", columns)
    .done(function (crmContactSettings) {
        if (crmContactSettings) {
            crmContact.contactId = crmContactSettings.contactid;
            crmContact.fileNumber = crmContactSettings.va_filenumber;
        }

        callback(crmContact);
    }).fail(function(err) {
        UTIL.restKitError(err, 'Failed to retrieve the contact information');
    });
}