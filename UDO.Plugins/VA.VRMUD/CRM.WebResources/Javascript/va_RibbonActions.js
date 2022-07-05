//_emergency = false;
_emergencyOptions = null;

function fetchCallBack(links) {
    var s = links[0].attributes["va_name"].value;
    if (!links || links.length == 0) {
        alert('System Settings lookup data does not contain any External Links settings.');
        return;
    }
    var s = '';
    for (var i = 0; i < links.length; i++) {
        if (!links[i].attributes["va_name"] || !links[i].attributes["va_name"].value) continue;
        var desc = links[i].attributes["va_description"] && links[i].attributes["va_description"].value ? links[i].attributes["va_description"].value : null;
        if (!desc) desc = links[i].attributes["va_name"].value;

        s = s + '<a id="relLink' + i.toString() + '" style="text-decoration: underline; cursor: pointer;" href="' +
            links[i].attributes["va_name"].value + '" target="_blank" title="Click to Open ' + desc + '">' + desc + '</a><br/>';
    }

    if (s.length == 0) return;

    if (!_progressWindowUrl || _progressWindowUrl == undefined) { _progressWindowUrl = '/WebResources/progress.htm'; }

    var handle = showModelessDialog(_progressWindowUrl, 'VRM Links', "status:false;scroll:off;dialogWidth:400px;dialogHeight:450px");
    if (!handle) return;

    handle.document.title = 'VRM Links';
    var elem1 = handle.document.getElementById('primage'); if (elem1) { elem1.style.visibility = 'hidden'; }
    var elem2 = handle.document.getElementById('load'); if (elem2) { elem2.innerText = 'VRM Links List'; }
    var elem3 = handle.document.getElementById('step'); if (elem3) { elem3.innerHTML = s; }
}

function PCR() {
    //To Change to '/WebResources/va_CallScript.html' after add it to CRM from ISV
    window.open('/ISV/scripts_VIP/callScript.html', 'LinksAndScripts', 'width=830,height=800,location=0,menubar=0,toolbar=0,scrollbars=1,resizable=1');
    return;

    //    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/pcrinfo.htm");
    var cols = ["va_name", "va_Description"];
    // va_systemsettingsSet?$select=va_Description,va_name&$orderby=va_Description asc&$filter=va_Type/Value eq 953850000
    var extLink = '953850000';

    var sFetch = '<fetch version="1.0" output-format="xml-platform" mapping="logical" distinct="false"><entity name="va_systemsettings"><attribute name="va_name"/><attribute name="va_description"/><order attribute="va_description" descending="false"/><filter type="and"><condition attribute="va_type" operator="eq" value="' +
    extLink +
    '"/></filter></entity></fetch>';

    var _oService;
    var _sOrgName = "";
    var _sServerUrl = Xrm.Page.context.getServerUrl();
    _oService = new FetchUtil(_sOrgName, _sServerUrl);
    _oService.Fetch(sFetch, fetchCallBack);
}

function RequestSupervisor() {
    ToggleOption("va_callerrequestedsupervisor", "Caller Requested Supervisor");
}
function Complaint() {
    // requires Supervisor role
    if (UserHasRole("Supervisor")) {
        ToggleOption("va_complaint", "Complaint");
    }
    else {
        alert('I am sorry but a Complaint can only be checked by someone in the Supervisor role.');
    }
}
function Compliment() {
    if (UserHasRole("Supervisor")) {
        ToggleOption("va_compliment", "Compliment");
    }
    else {
        alert('I am sorry but a Compliment can only be checked by someone in the Supervisor role.');
    }
}
function AbusiveCall() {
    ToggleOption("va_abusivecall", "Abusive Call");
}
function CallerDisconnected() {
    ToggleOption("va_callerwasdisconnected", "Caller Disconnected");
}
function PCRDisconnected() {
    ToggleOption("va_pcrterminated", "PCR Disconnected");
}
function TrainingRequest() {
    ToggleOption("va_trainingrequest", "Training Request");
}

function ToggleOption(schemaName, displayName) {
    var attr = Xrm.Page.getAttribute(schemaName);
    if (attr != undefined && attr != null && attr) {
        var currentValue = attr.getValue();
        if (currentValue == 1) {
            attr.setValue(0);
            alert(displayName + " option un-checked");
        }
        else {
            attr.setValue(1);
            alert(displayName + " option checked");
        }
    }
    else {
        alert("You do not have access to the " + displayName + " option.");
    }
}


function VAI() {
    window.open("https://iris.custhelp.com/cgi-bin/iris.cfg");
}
function Rewrite() {
    window.open("http://vbaw.vba.va.gov/BL/21/M21/content/index.asp");
}
function VAGOVINTER() {
    window.open("http://www.va.gov/");
}
function VAGOVINTRA() {
    window.open("http://vaww.va.gov/default.asp");
}
function VIRTUALVA() {
    window.open("http://virtualva.vba.va.gov/");
}
function RateChart() {
    window.open("http://vbaw.vba.va.gov/bl/21/publicat/Manuals/Rates/rates_home.htm");
}
function PhoneScript() {
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/scripts.htm");
}
function FactSheet() {
    window.open("http://www.vba.va.gov/VBA/benefits/factsheets/#BM8");
}
function VBA() {
    window.open("http://vbaw.vba.va.gov/");
}
function FormSite() {
    window.open("http://vaww4.va.gov/vaforms/");
}
function DirectServices() {
    window.open("http://vbaw.vba.va.gov/bl/27/quality_training/training/index.htm ");
}
function CFR() {
    window.open("http://vbaw.vba.va.gov/bl/21/publicat/Regs/Part4/index.htm");
}
function DisabilityCalc() {
    //THIS SHOULD BE CHANGED ONCE WE GET A CENTRAL LOCATION FOR IT
    window.open("http://vbaedwweb1.vba.va.gov:7778/oowa-bin/oowaro/ExpSrv634/dbxwdevkit/xwd_init?door3.db/web3/3003");
}
function AverageDaysOfClaim() {
    window.open("http://vbaedwweb1.vba.va.gov:7778/oowa-bin/oowaro/ExpSrv634/dbxwdevkit/xwd_init?door3.db/web3/3003");
}
function VESLogin() {
    window.open("https://www.vesservices.com/secure/loginform.aspx");
}
function ExamTrack() {
    window.open("https://www.va.examtrak.com/");
}
function PensionCalc() {
    var orgname = Xrm.Page.context.getOrgUniqueName();
    var scriptRoot = Xrm.Page.context.getServerUrl().replace(orgname, '');
    window.open(scriptRoot + "ISV/Documents/SMPportionworksheet.xlsx");
}
function DocRepository() {
    var org = Xrm.Page.context.getOrgUniqueName();
    var Doc_Root = Xrm.Page.context.getServerUrl().replace(org, '');
    window.open(Doc_Root + "ISV/Documents/index.html");
}
function Emergency() {
    debugger;

    _emergencyOptions = {
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
            var filter1 = 'TeamId eq guid\'' + data1.d.results[0].va_Team.Id + '\'';
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
                openEmergencyWindow();
            }
            ).fail(function (error) {
                UTIL.restKitError(error, noRecipientMessage);
                return;
            });
        }
        ).fail(function (err) {
            UTIL.restKitError(err, 'The PCR Site (NCC) does not have a Team assigned!\r\n' +
                    'A Site (NCC) must have an emergency Team assigned in order to get the list of recipients to deliver the emergency notification!');
        });
    }).fail(function (error) {
        UTIL.restKitError(error, 'The PCR does not have a site assigned!\r\n' +
            'A Site must be assigned in order to get the list of recipients to deliver the emergency notification!');
    });
}

function CancelEmergency() {
    debugger;

    if (_emergencyOptions && _emergencyOptions.isCancel) {
        openEmergencyWindow();
    } else {
        alert('No emergency was triggered.');
    }
}

function openEmergencyWindow() {
    var url = '/WebResources/va_Emergency.html',
        windowHandle;

    windowHandle = window.open(url, 'emergencyPage', 'resizeable=yes,status=no,scrollbars=yes,toolbars=no,menubar=no,location=no,width=480,height=600,top=20,left=20');
    windowHandle.focus();
}

function GetOwnerId() {
    var entityName = Xrm.Page.data.entity.getEntityName();
    var owner = "";
    try {
        if (entityName == "va_servicerequest") {
            owner = Xrm.Page.getAttribute("va_pcrofrecordid").getValue()[0].id;
        }
        else if (entityName == "phonecall") {
            owner = Xrm.Page.getAttribute("createdby").getValue()[0].id;
        }
        else if (entityName == "contact") {
            owner = Xrm.Page.getAttribute("createdby").getValue()[0].id;
        }
    }
    catch (oe) {
        owner = Xrm.Page.context.getUserId();
    }
    var ownerId = owner.substring(1, owner.length - 1);
    return ownerId;
}
// FetchUtil **************

var XMLHTTPSUCCESS = 200;
var XMLHTTPREADY = 4;

function FetchUtil(sOrg, sServer) {
    this.org = sOrg;
    this.server = sServer;

    if (sOrg == null) {
        if (typeof (ORG_UNIQUE_NAME) != "undefined") {
            this.org = ORG_UNIQUE_NAME;
        }
    }

    if (sServer == null) {
        this.server = window.location.protocol + "//" + window.location.host;
    }
}

FetchUtil.prototype._ExecuteRequest = function (sXml, sMessage, fInternalCallback, fUserCallback) {
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open("POST", this.server + "/XRMServices/2011/Organization.svc/web", (fUserCallback != null));
    xmlhttp.setRequestHeader("Accept", "application/xml, text/xml, */*");
    xmlhttp.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
    xmlhttp.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");

    if (fUserCallback != null) {
        //asynchronous: register callback function, then send the request.
        var crmServiceObject = this;
        xmlhttp.onreadystatechange = function () {
            fInternalCallback.call(crmServiceObject, xmlhttp, fUserCallback);
        };
        xmlhttp.send(sXml);
    } else {
        //synchronous: send request, then call the callback function directly
        xmlhttp.send(sXml);
        return fInternalCallback.call(this, xmlhttp, null);
    }
};

FetchUtil.prototype._HandleErrors = function (xmlhttp) {
    /// <summary>(private) Handles xmlhttp errors</summary>
    if (xmlhttp.status != XMLHTTPSUCCESS) {
        var sError = "Error: " + xmlhttp.responseText + " " + xmlhttp.statusText;
        alert(sError);
        return true;
    } else {
        return false;
    }
};

FetchUtil.prototype.Fetch = function (sFetchXml, fCallback) {
    /// <summary>Execute a FetchXml request. (result is the response XML)</summary>
    /// <param name="sFetchXml">fetchxml string</param>
    /// <param name="fCallback" optional="true" type="function">(Optional) Async callback function if specified. If left null, function is synchronous </param>

    var request = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">";
    request += "<s:Body>";

    request += '<Execute xmlns="http://schemas.microsoft.com/xrm/2011/Contracts/Services">' + '<request i:type="b:RetrieveMultipleRequest" ' + ' xmlns:b="http://schemas.microsoft.com/xrm/2011/Contracts" ' + ' xmlns:i="http://www.w3.org/2001/XMLSchema-instance">' + '<b:Parameters xmlns:c="http://schemas.datacontract.org/2004/07/System.Collections.Generic">' + '<b:KeyValuePairOfstringanyType>' + '<c:key>Query</c:key>' + '<c:value i:type="b:FetchExpression">' + '<b:Query>';

    request += CrmEncodeDecode.CrmXmlEncode(sFetchXml);

    request += '</b:Query>' + '</c:value>' + '</b:KeyValuePairOfstringanyType>' + '</b:Parameters>' + '<b:RequestId i:nil="true"/>' + '<b:RequestName>RetrieveMultiple</b:RequestName>' + '</request>' + '</Execute>';

    request += '</s:Body></s:Envelope>';

    return this._ExecuteRequest(request, "Fetch", this._FetchCallback, fCallback);
};

FetchUtil.prototype._FetchCallback = function (xmlhttp, callback) {
    ///<summary>(private) Fetch message callback.</summary>
    //xmlhttp must be completed
    if (xmlhttp.readyState != XMLHTTPREADY) {
        return;
    }

    //check for server errors
    if (this._HandleErrors(xmlhttp)) {
        return;
    }

    var sFetchResult = xmlhttp.responseXML.selectSingleNode("//a:Entities").xml;

    var resultDoc = new ActiveXObject("Microsoft.XMLDOM");
    resultDoc.async = false;
    resultDoc.loadXML(sFetchResult);

    //parse result xml into array of jsDynamicEntity objects
    var results = new Array(resultDoc.firstChild.childNodes.length);
    for (var i = 0; i < resultDoc.firstChild.childNodes.length; i++) {
        var oResultNode = resultDoc.firstChild.childNodes[i];
        var jDE = new jsDynamicEntity();
        var obj = new Object();

        for (var j = 0; j < oResultNode.childNodes.length; j++) {
            switch (oResultNode.childNodes[j].baseName) {
                case "Attributes":
                    var attr = oResultNode.childNodes[j];

                    for (var k = 0; k < attr.childNodes.length; k++) {

                        // Establish the Key for the Attribute
                        var sKey = attr.childNodes[k].firstChild.text;
                        var sType = '';

                        // Determine the Type of Attribute value we should expect
                        for (var l = 0; l < attr.childNodes[k].childNodes[1].attributes.length; l++) {
                            if (attr.childNodes[k].childNodes[1].attributes[l].baseName == 'type') {
                                sType = attr.childNodes[k].childNodes[1].attributes[l].text;
                            }
                        }

                        switch (sType) {
                            case "a:OptionSetValue":
                                var entOSV = new jsOptionSetValue();
                                entOSV.type = sType;
                                entOSV.value = attr.childNodes[k].childNodes[1].text;
                                obj[sKey] = entOSV;
                                break;
                            case "a:EntityReference":
                                var entRef = new jsEntityReference();
                                entRef.type = sType;
                                entRef.guid = attr.childNodes[k].childNodes[1].childNodes[0].text;
                                entRef.logicalName = attr.childNodes[k].childNodes[1].childNodes[1].text;
                                entRef.name = attr.childNodes[k].childNodes[1].childNodes[2].text;
                                obj[sKey] = entRef;
                                break;
                            default:
                                var entCV = new jsCrmValue();
                                entCV.type = sType;
                                entCV.value = attr.childNodes[k].childNodes[1].text;
                                obj[sKey] = entCV;

                                break;
                        }

                    }

                    jDE.attributes = obj;
                    break;
                case "Id":
                    jDE.guid = oResultNode.childNodes[j].text;
                    break;
                case "LogicalName":
                    jDE.logicalName = oResultNode.childNodes[j].text;
                    break;
                case "FormattedValues":
                    var foVal = oResultNode.childNodes[j];

                    for (var k = 0; k < foVal.childNodes.length; k++) {
                        // Establish the Key, we are going to fill in the formatted value of the already found attribute
                        var sKey = foVal.childNodes[k].firstChild.text;

                        jDE.attributes[sKey].formattedValue = foVal.childNodes[k].childNodes[1].text;

                    }
                    break;
            }

        }

        results[i] = jDE;
    }

    //return entities
    if (callback != null) callback(results);
    else return results;

};

function jsDynamicEntity(gID, sLogicalName) {
    this.guid = gID;
    this.logicalName = sLogicalName;
    this.attributes = new Object();
}

function jsCrmValue(sType, sValue) {
    this.type = sType;
    this.value = sValue;
}

function jsEntityReference(gID, sLogicalName, sName) {
    this.guid = gID;
    this.logicalName = sLogicalName;
    this.name = sName;
    this.type = 'EntityReference';
}

function jsOptionSetValue(iValue, sFormattedValue) {
    this.value = iValue;
    this.formattedValue = sFormattedValue;
    this.type = 'OptionSetValue';
}