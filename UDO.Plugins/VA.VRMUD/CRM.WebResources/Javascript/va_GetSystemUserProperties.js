/// <reference path="/Intellisense/XrmPage-vsdoc.js" />
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

function GetUserSettingsForWebservice() {
    if (window._UserSettings)
        return _UserSettings;

    var currentUserId = Xrm.Page.context.getUserId();

    var columns = ['SystemUserId', 'va_PCRSensitivityLevel', 'va_IPAddress', 'va_StationNumber', 'va_PCRSSN',
    'va_WSLoginName', 'va_FileNumber', 'va_ApplicationName', 'DomainName', 'FullName', 'InternalEMailAddress', 'MobileAlertEMail', 'SiteId', 'ParentSystemUserId'];
    
    CrmRestKit2011.Retrieve('SystemUser', currentUserId, columns, false)
    .done(function (data) {
        var systemUserSettings, systemUser, slashPos;

        if (data && data.d) {
            systemUserSettings = data.d;
            systemUser = new user();

            // User name comes as a Login name part of domain login. If va_WSLoginName is present, it is used as an override
            if (systemUserSettings.va_WSLoginName && systemUserSettings.va_WSLoginName.length > 0)
                systemUser.userName = systemUserSettings.va_WSLoginName;
            else {
                slashPos = systemUserSettings.DomainName.indexOf('\\');
                systemUser.userName = systemUserSettings.DomainName.substr(slashPos + 1);
            }

            // Per Cory and Johnny Kahn, password is never used by WS and is not needed systemUser.password = systemUserSettings.va_WSPassword;
            systemUser.clientMachine = systemUserSettings.va_IPAddress;
            systemUser.stationId = systemUserSettings.va_StationNumber;
            systemUser.applicationName = systemUserSettings.va_ApplicationName;
            systemUser.pcrSensitivityLevel = (systemUserSettings.va_PCRSensitivityLevel ? systemUserSettings.va_PCRSensitivityLevel.Value : 0);
            systemUser.loginName = systemUserSettings.DomainName;
            systemUser.ssn = systemUserSettings.va_FileNumber;
            systemUser.fileNumber = systemUserSettings.va_FileNumber;
            systemUser.fullName = systemUserSettings.FullName;
            systemUser.email = (systemUserSettings.InternalEMailAddress && systemUserSettings.InternalEMailAddress.length > 0 ? systemUserSettings.InternalEMailAddress : systemUserSettings.MobileAlertEMail);
            systemUser.pcrId = systemUserSettings.va_PCRSSN;
            systemUser.site = systemUserSettings.SiteId && systemUserSettings.SiteId.Name ? systemUserSettings.SiteId.Name : '';
            systemUser.manager = systemUserSettings.ParentSystemUserId && systemUserSettings.ParentSystemUserId.Name ? systemUserSettings.ParentSystemUserId.Name : '';

            _UserSettings = systemUser;

        } else {
            throw new Error('CRM did not return the current User!');
        }
    }).fail(function (err) {
        var error = 'Failed to retrieve the User Settings.\r\nError: ' + err.statusText;
        if (err.status === 400 && err.responseText)
            error += '\r\n' + $.parseJSON(err.responseText).error.message.value;

        alert(error);
        throw new Error(error);
    });
    
    return _UserSettings;
}
_GetUserSettingsForWebservice = GetUserSettingsForWebservice;

var contact = function () {
};
user.prototype.constructor = contact;
user.prototype.fileNumber;
user.prototype.contactId;

function GetContactInformation(id, callback) {
    var crmContact = new contact();
    
    var columns = ['ContactId', 'va_FileNumber'];
    CrmRestKit2011.Retrieve('Contact', id, columns)
    .done(function (crmContactSettings) {
        if (crmContactSettings && crmContactSettings.d) {
            crmContact.contactId = crmContactSettings.d.ContactId;
            crmContact.fileNumber = crmContactSettings.d.va_FileNumber;
        }

        callback(crmContact);
    }).fail(function(err) {
        UTIL.restKitError(err, 'Failed to retrieve the contact information');
    });
}