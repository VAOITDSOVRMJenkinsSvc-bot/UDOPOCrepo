var commonFunctionsVip = {};


/**
* Global functions and constants for VIP version of CRMUD
*/
// Constants
CRM_FORM_TYPE_CREATE = 1;
CRM_FORM_TYPE_UPDATE = 2;
CRM_FORM_TYPE_COMPLETED_ACTIVITY = 4;
WEBSERVICE_RESPONSE = '';

_vipEntryPoint = null;
_vipSearchContext = null;
_VIPEndOfSearch = null;
_VIPEndOfServiceCall = null;
_GetEnvironment = null;
_cachedData = null; // global set of Environment, UserSetting, etc passed to extjs
_serviceResultsCollection = null; // collection of ws xml responses keyed to xml field name
_endOfSearchReached = false;
_usingIFD = false;
_KMRoot = null;
_IFRAME_SOURCE_SINGLE = null;
_IFRAME_SOURCE_MULTIPLE = null;
_XML_UTIL = null;
_context = null;
// Global variables
// For backward compatability ********************
WebServiceURLRoot = null;
_vrmVersion = null;
_envName = null;
_MVIServiceURLRoot = null;
_MVIDAC = null;
_PathwaysServiceURLRoot = null;
_PathwaysDAC = null;
_VacolsServiceURLRoot = null;
_VacolsDAC = null;
_globalDACUrl = null;
_isPROD = null;
//***********************************************
_progressWindow = null;
_noticeDlg = null;
_allowToCashXMLResponses = true;    // if true will save xml responses in phone call, contact records. if false, will flush them on save
_scriptWindowHandle = null;
_logErrors = true;
_logWarnings = false;
_logQueries = true;
_per = false;
_suppressWSNotice = false;
_progressWindowUrl = '/WebResources/va_progress.htm';
_totalProgress = 0;
_progressInterval = 1;
_totalWebServiceExecutionTime = 0;
_WebServiceResponse = null;
_UserSettings = null;
//*******************************************************************************
/* Inizalization function
 * must be called from the onload event of the form
 */
commonFunctionsVip.initalize = function () {
    // Set the envrionment settings
    window._currentEnv = window._currentEnv || environmentConfigurations.get();
    if (!_currentEnv)
        throw 'Global environment settings initalization failed!';

    // Global variables
    // For backward compatability ********************
    WebServiceURLRoot = _currentEnv.CORP;
    _vrmVersion = _currentEnv.Version;
    _envName = _currentEnv.name;
    _MVIServiceURLRoot = _currentEnv.MVI;
    _MVIDAC = _currentEnv.MVIDAC;
    _PathwaysServiceURLRoot = _currentEnv.Pathways;
    _PathwaysDAC = _currentEnv.PathwaysDAC;
    _VacolsServiceURLRoot = _currentEnv.Vacols;
    _VacolsDAC = _currentEnv.VacolsDAC;
    _globalDACUrl = _currentEnv.globalDAC;
    _isPROD = _currentEnv.isPROD;
    //***********************************************

    // validate it is set correctly
    if (!WebServiceURLRoot || !_currentEnv.MVI || !_currentEnv.Pathways || !_currentEnv.Vacols) {
        alert('Environment Configuration is not complete. One of the Web Service root URLs is not specified. Please contact System Administrator.');
    }

    _KMRoot = Xrm.Page.context.getServerUrl().replace(Xrm.Page.context.getOrgUniqueName(), '') + 'isv/scripts_VIP/'; // used as a root for all scripts (claims, appeals, issue-based, etc.)

    //if this is set to true, IFD compatible links will be generated for popups such as SR, Change of Addr. if false - regular links will be used.

    if (_currentEnv.isPROD || _currentEnv.name === 'PINT') { _usingIFD = true; }

    _IFRAME_SOURCE_MULTIPLE = 'ISV/ext-4.0.1/VA' + _vrmVersion + '/VA-phone/phone-multiple.html';
    _IFRAME_SOURCE_SINGLE = 'ISV/ext-4.0.1/VA' + _vrmVersion + '/VA-contact/contact.html';

    _XML_UTIL = new XmlUtilities();

    _context = new vrmContext();
};
// end of inializations

//=============================================
//  getContext()
//=============================================

function getContext() {
    return _context;
}
//=============================================
//  getVrmMessage() 
//=============================================
_VRMMESSAGE = new Array();
function GetEnvironment() {
    return _currentEnv;
}
_GetEnvironment = GetEnvironment;

function getCurrentEnvironment() {
    return _currentEnv.name;
}

function getVrmVersion() {
    return _vrmVersion;
}

function getVrmMessage() {
    return _VRMMESSAGE;
}

function prepareProgress() {
    return;
    try {
        var width = 350;
        var height = 120;
        var top = (screen.height - height) / 2;
        var left = (screen.width - width) / 2;
        var params = "width=" + width + ",height=" + height + ",scrollbars=0,resizable=0,location=0,menubar=0,toolbar=0,top=" + top +
			 ",left=" + left + ",status=0,titlebar=no";
        _progressWindow = window.open(_progressWindowUrl, 'SearchResults', params);
    }
    catch (err) { }
}
function positionProgress() {
    try {
        var width = 350;
        var height = 120;
        var top = (screen.height - height) / 2;
        var left = (screen.width - width) / 2;
        if (!_progressWindow) return;
        _progressWindow.innerWidth = width;
        _progressWindow.innerHeight = height;
        _progressWindow.screenX = left;
        _progressWindow.screenY = top;
    }
    catch (err) { }
}

function ShowProgress(text) {
    if (!_progressWindow) {
        prepareProgress();
    }
    else {
        positionProgress();
    }

    if (!_progressWindow) return;
    try {
        var elem = _progressWindow.document.getElementById('step');
        if (elem) elem.innerText = text;
    }
    catch (err) { }
}
function ShowProgressIEModeless(text) {
    var cnt = 0;
    while (cnt < 3) {
        if (!_noticeDlg) {
            _noticeDlg = showModelessDialog(_progressWindowUrl, text, "status:false;scroll:off;dialogWidth:350px;dialogHeight:150px");
        }
        if (!_noticeDlg) return;

        var elem = null;
        try { elem = _noticeDlg.document.getElementById('step'); cnt = 5; }
        catch (ee) { CloseProgress(); cnt++; }
        if (elem) elem.innerText = text;
    }
}
_ShowProgress = ShowProgress;

function CloseProgressIEModeless() {
    if (_noticeDlg) {
        try { _noticeDlg.close(); } catch (err) { }
        _noticeDlg = null;
    }
}

function CloseProgress() {
    try {
        if (_progressWindow) {
            //Minimize();
            _progressWindow.close();
            _progressWindow = null;
        }
    } catch (Error) { }
}

function Minimize() {
    if (!_progressWindow) return;
    _progressWindow.blur();
    return;
    _progressWindow.innerWidth = 100;
    _progressWindow.innerHeight = 100;
    _progressWindow.screenX = screen.width;
    _progressWindow.screenY = screen.height;
    //alwaysLowered = true;
}

function UserHasRole(roleName) {

    var context = Xrm.Page.context;
    var serverUrl = context.getServerUrl();
    var ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc/";
    var odataSelect = ODataPath + "RoleSet?$filter=Name eq '" + roleName + "'&$select=RoleId";

    var rv = false;

    $.ajax({
        async: false,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: odataSelect,
        beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
        success: function (data, textStatus, XmlHttpRequest) {
            var requestResults = data.d;
            if (requestResults != null && requestResults.results.length > 0) {
                for (var i = 0; i < requestResults.results.length; i++) {
                    var role = requestResults.results[i];
                    var id = role.RoleId;
                    var currentUserRoles = Xrm.Page.context.getUserRoles();
                    for (var j = 0; j < currentUserRoles.length; j++) {
                        var userRole = currentUserRoles[j];
                        if (GuidsAreEqual(userRole, id)) {
                            rv = true;
                            break;
                        }
                    }
                    if (rv) break;
                }
            }
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) { alert('OData Select Failed: ' + odataSelect); }
    });

    return rv;
}
function GuidsAreEqual(guid1, guid2) {
    var isEqual = false;

    if (guid1 == null || guid2 == null) {
        isEqual = false;
    }
    else {
        isEqual = guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase();
    }

    return isEqual;
}
function GetRequestObject() {
    if (window.XMLHttpRequest) {
        return new window.XMLHttpRequest;
    }
    else {
        try {
            return new ActiveXObject("MSXML2.XMLHTTP.3.0");
        }
        catch (ex) {
            return null;
        }
    }
}

function FormatPhone(oField) {
    // Validate the field information.
    if (typeof (oField) != "undefined" && oField != null) {
        // Remove any non-numeric characters.
        var sTmp = oField.DataValue.replace(/[^0-9]/g, "");

        // If the number has a valid length, format the number.
        switch (sTmp.length) {
            case "4105551212".length:
                oField.DataValue = "(" + sTmp.substr(0, 3) + ") " +
	  sTmp.substr(3, 3) + "-" + sTmp.substr(6, 4);
                break;

            case "5551212".length:
                oField.DataValue = sTmp.substr(0, 3) + "-" +
	  sTmp.substr(3, 4);
                break;
        }
    }
}

// RU12 Updated InlineToolbar button method
function InlineToolbar(containerId) {
    var toolbar = this,
		container = document.getElementById(containerId);

    if (!container) {
        return alert("Toolbar Field: " + containerId + " is missing");
    }

    var btnTabIndex = container.tabIndex;

    document.getElementById(containerId + "_c").style.display = 'none';

    container.style.display = "none";
    container = container.parentElement;

    toolbar.AddButton = function (id, text, width, callback, imgSrc) {
        var btn = document.createElement("div");
        var btStyle = new StyleBuilder();
        btStyle.Add("font-family", "Arial");
        btStyle.Add("font-size", "12px");
        btStyle.Add("line-height", "16px");
        btStyle.Add("text-align", "center");
        btStyle.Add("cursor", "pointer");
        btStyle.Add("border", "1px solid #3366CC");
        btStyle.Add("background-color", "#CEE7FF");
        btStyle.Add("background-image", "url( '/_imgs/btn_rest.gif' )");
        btStyle.Add("background-repeat", "repeat-x");
        btStyle.Add("overflow", "visible");
        btStyle.Add("width", "98%");

        btn.style.cssText = btStyle.ToString();
        btn.attachEvent("onclick", callback);
        btn.id = id;

        if (btnTabIndex && btnTabIndex != undefined && btnTabIndex != '') {
            btn.tabIndex = btnTabIndex;
        }

        if (imgSrc) {
            var img = document.createElement("img");
            img.src = imgSrc;
            img.style.verticalAlign = "middle";
            btn.appendChild(img);
            btn.appendChild(document.createTextNode(" "));
            var spn = document.createElement("span");
            spn.innerText = text;
            btn.appendChild(spn);
        } else {
            btn.innerText = text;
        }

        container.style.backgroundColor = "#f6f8fa";
        container.style.width = width;
        container.style.padding = "0px";
        container.style.border = "0px";
        container.appendChild(btn);

        return btn;
    };

    toolbar.RemoveButton = function (id) {
        var btn = toolbar.GetButton(id);
        if (btn) {
            btn.parentNode.removeChild(btn);
        }
    };

    toolbar.GetButton = function (id) {
        return document.getElementById(id);
    };

    function StyleBuilder() {
        var cssText = new StringBuilder();
        this.Add = function (key, value) { cssText.Append(key).Append(":").Append(value).Append(";"); };
        this.ToString = function () { return cssText.ToString(); };
    };

    function StringBuilder() {
        var parts = [];
        this.Append = function (text) {
            parts[parts.length] = text;
            return this;
        };
        this.Reset = function () { parts = []; };
        this.ToString = function () { return parts.join(""); };
    };
}
//************************************************************************

FormatExtjsDate = function (str_date) {
    if (str_date) {
        if (str_date.toString().length == 8) {
            str_date = str_date.toString().substring(0, 2) + '/' + str_date.toString().substring(2, 4) + '/' + str_date.toString().substring(4, 8);
        }
    } else {
        str_date = '';
    }
    return str_date;
};
parseXmlObject = function (xmlString) {
    if (xmlString == null) {
        alert('XML Object contains a null value');
        return null;
    }
    var xmlObject = new ActiveXObject("Microsoft.XMLDOM");
    xmlObject.async = false;
    xmlObject.loadXML(xmlString);

    return xmlObject;
};

function getLocalTime(timeZone) {
    // TO DO: Determine system user's time zone and then create time
    // diffs based on that.  Right now this assumes the user is in EST
    var usersTZOffset = 5;

    if (timeZone == null || timeZone.length == 0) return null;
    // get time dispacement
    var tz = timeZone.substr(4, 3);
    if (tz.substr(tz.length - 1, 1) == ')') tz = tz.substr(0, tz.length - 1);
    var tzInt = parseInt(tz);

    var ONE_HOUR = 1000 * 60 * 60; // 1 hour = 2400000 milliseconds

    var tz_D = (tzInt + usersTZOffset) * ONE_HOUR;

    var contactLocalTime = new Date();
    // We have to assume that the local time is not going to show seconds.  Thus,
    // we check if the currently set time has the same date, hour, and minutes.
    var remainder = contactLocalTime.getTime() % 60000;
    contactLocalTime.setTime(contactLocalTime.getTime() + tz_D - remainder);
    return contactLocalTime;
}

function formatCurrency(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    var sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    var cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));

    return (((sign) ? '' : '-') + '$' + num + '.' + cents);
}

function formatXml(xml) {
    var formatted = '';
    var reg = /(>)(<)(\/*)/g;
    xml = xml.replace(reg, '$1\r\n$2$3');
    var pad = 0;
    jQuery.each(xml.split('\r\n'), function (index, node) {
        var indent = 0;
        if (node.match(/.+<\/\w[^>]*>$/)) {
            indent = 0;
        } else if (node.match(/^<\/\w/)) {
            if (pad != 0) {
                pad -= 1;
            }
        } else if (node.match(/^<\w[^>]*[^\/]>.*$/)) {
            indent = 1;
        } else {
            indent = 0;
        }

        var padding = '';
        for (var i = 0; i < pad; i++) {
            padding += '  ';
        }

        formatted += padding + node + '\r\n';
        pad += indent;
    });

    return formatted;
}


function SingleNodeExists(xmlObj, node_path) {
    var exists = false;
    var node = xmlObj.selectSingleNode(node_path);
    if (node && node != undefined && node.text && node.text != undefined && node.text.length > 0) {
        exists = true;
    }
    return exists;
}

function MultipleNodesExist(xmlObj, node_path) {
    var exists = false;
    var node = xmlObj.selectNodes(node_path);
    if (node && node != undefined && node[0] && node[0] != undefined && node[0].hasChildNodes()) {
        exists = true;
    }
    return exists;
}



//formating phone numbers
function FormatTelephone(telephoneNumber) {
    var Phone = telephoneNumber;
    var ext = '';
    var result;

    if (0 != Phone.indexOf('+')) {
        if (1 < Phone.lastIndexOf('x')) {
            ext = Phone.slice(Phone.lastIndexOf('x'));
            Phone = Phone.slice(0, Phone.lastIndexOf('x'));
        }

        Phone = Phone.replace(/[^\d]/gi, '');
        result = Phone;
        if (7 == Phone.length) {
            result = Phone.slice(0, 3) + '-' + Phone.slice(3)
        }
        if (10 == Phone.length) {
            result = '(' + Phone.slice(0, 3) + ') ' + Phone.slice(3, 6) + '-' + Phone.slice(6);
        }
        if (0 < ext.length) {
            result = result + ' ' + ext;
        }
        return result;
    }
}

//This function can pull value of the field from another entity by lookup id. (Using retrieve multiples)

function GetAttributeValueFromID(sEntityName, sGUID, sAttributeName, sID) {
    var xml = "" +
	"<?xml version="1.0" encoding="utf-8"?>" +
	"<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">" +
	Xrm.Page.context.getAuthenticationHeader() +
	"  <soap:Body>" +
	"    <RetrieveMultiple xmlns="http://schemas.microsoft.com/crm/2007/WebServices">" +
	"      <query xmlns:q1="http://schemas.microsoft.com/crm/2006/Query" xsi:type="q1:QueryExpression">" +
	"        <q1:EntityName>" + sEntityName + "</q1:EntityName>" +
	"        <q1:ColumnSet xsi:type="q1:ColumnSet">" +
	"          <q1:Attributes>" +
	"            <q1:Attribute>" + sAttributeName + "</q1:Attribute>" +
	"          </q1:Attributes>" +
	"        </q1:ColumnSet>" +
	"        <q1:Distinct>false</q1:Distinct>" +
	"        <q1:PageInfo>" +
	"          <q1:PageNumber>1</q1:PageNumber>" +
	"          <q1:Count>1</q1:Count>" +
	"        </q1:PageInfo>" +
	"        <q1:Criteria>" +
	"          <q1:FilterOperator>And</q1:FilterOperator>" +
	"          <q1:Conditions>" +
	"            <q1:Condition>" +
	"              <q1:AttributeName>" + sID + "</q1:AttributeName>" +
	"              <q1:Operator>Equal</q1:Operator>" +
	"              <q1:Values>" +
	"                <q1:Value xsi:type="xsd:string">" + sGUID + "</q1:Value>" +
	"              </q1:Values>" +
	"            </q1:Condition>" +
	"          </q1:Conditions>" +
	"        </q1:Criteria>" +
	"      </query>" +
	"    </RetrieveMultiple>" +
	"  </soap:Body>" +
	"</soap:Envelope>" +
	"";
    var xmlHttpRequest = new ActiveXObject("Msxml2.XMLHTTP");
    xmlHttpRequest.Open("POST", "/mscrmservices/2007/CrmService.asmx", false);
    xmlHttpRequest.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/crm/2007/WebServices/RetrieveMultiple");
    xmlHttpRequest.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
    xmlHttpRequest.setRequestHeader("Content-Length", xml.length);
    xmlHttpRequest.send(xml);
    // retrieve response and find attribute value
    var result = xmlHttpRequest.responseXML.selectSingleNode("//q1:" + sAttributeName);
    if (result == null)
        return "";
    else
        return result.text;
}