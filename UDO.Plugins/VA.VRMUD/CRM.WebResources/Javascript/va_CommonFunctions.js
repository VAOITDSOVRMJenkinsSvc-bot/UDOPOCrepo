var commonFunctions = {};

// Contstants
CRM_FORM_TYPE_CREATE = 1;
CRM_FORM_TYPE_UPDATE = 2;
CRM_FORM_TYPE_COMPLETED_ACTIVITY = 4;
WEBSERVICE_RESPONSE = '';

_usingIFD = false;
_KMRoot = null;
_IFRAME_SOURCE_MULTIPLE = null;
_IFRAME_SOURCE_SINGLE = null;
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

commonFunctions.initalize = function () {
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
// End of initalization

function getContext() {
    return _context;
}
//=============================================
//  getVrmMessage() 
//=============================================
_VRMMESSAGE = new Array();

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

function ShowProgressHTML(text) {
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
function ShowProgress(text) {
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

function CloseProgress() {
    if (_noticeDlg) {
        try { _noticeDlg.close(); } catch (err) { }
        _noticeDlg = null;
    }
}

function CloseProgressHTML() {
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

function UpdateClaimScriptText(claim, claimScriptWindowHandle) {
    try {
        var text = claimScriptWindowHandle.document.body.innerHTML;
        var contentions = '';
        if (claim.outerContentions() && claim.outerContentions().getCount() > 0) {
            claim.outerContentions().getAt(0).contentions().each(function concatenateContentionData(contention) { contentions += contention.data.clmntTxt + '; '; });

            if (contentions.length > 1) contentions = contentions.substr(0, contentions.length - 2);
        }

        if (contentions.length == 0) contentions = '(name contentions)';

        var map = [
		{ value: claim.data.claimTypeName, name: '[type of claim]' },
		{ value: claim.data.claimReceiveDate, name: '[date of claim]' },
		{ value: contentions, name: '[contentions]' }
        ];

        for (var tag in map) {
            text = text.replace(map[tag].name, map[tag].value);
        }

        claimScriptWindowHandle.document.body.innerHTML = text;
        claimScriptWindowHandle.blur();
    }
    catch (ex) {
        debugger
    }
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

// OData: make sure that you include jquery1.4.1.min.js and json2.js as libraries first since this script relies on them
function SetDefaultCurrency() {
    var context = Xrm.Page.context;
    var serverUrl = context.getServerUrl();
    var ODataPath = serverUrl + "/XRMServices/2011/OrganizationData.svc";
    var odataSelect = ODataPath + "/TransactionCurrencySet?$select=TransactionCurrencyId,CurrencyName";

    $.ajax({
        type: "GET",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        url: odataSelect,
        beforeSend: function (XMLHttpRequest) { XMLHttpRequest.setRequestHeader("Accept", "application/json"); },
        success: function (data, textStatus, XmlHttpRequest) {
            var myCurrency = data.d.results[0];
            var idValue = eval('myCurrency.TransactionCurrencyId');
            var textValue = eval('myCurrency.CurrencyName');
            var thisEntityType = 'transactioncurrency';
            Xrm.Page.getAttribute("transactioncurrencyid").setValue([{ id: idValue, name: textValue, entityType: thisEntityType }]);
        },
        error: function (XmlHttpRequest, textStatus, errorThrown) { alert('OData Select Failed: ' + odataSelect); }
    });
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

function formatNumber(context) {

    try {

        var phoneNumber = context.getEventSource().getValue();

        //        var oField = event.srcElement;

        //        if ( typeof(phoneNumber) != "undefined" && phoneNumber != null && phoneNumber.getValue() != null ) {
        if (phoneNumber != null) {

            var sTmp = phoneNumber.replace(/[^0-9]/g, "");
            switch (sTmp.length) {
                case 10:
                    var formattedPhone = "(" + sTmp.substr(0, 3) + ") " + sTmp.substr(3, 3) + "-" + sTmp.substr(6, 4);
                    context.getEventSource().setValue(formattedPhone);
                    //                oField.DataValue = “(” + sTmp.substr(0, 3) + “) ” + sTmp.substr(3, 3) + “-” + sTmp.substr(6, 4);
                    break;
                default:
                    //                alert("Phone must contain 10 numbers.");
                    break;
            }
        }
    } catch (err) {
        alert("Error in formatting phone number");
    }

}

//Operation: Entity Clone
function Clone() {
    var cloneUrl = location.pathname + "?";
	cloneUrl += "_CreateFromType=" + Xrm.Page.context.getQueryStringParameters().etc;
	cloneUrl += "&_CreateFromId=" + Xrm.Page.data.entity.getId();
	cloneUrl += "&etc=" + Xrm.Page.context.getQueryStringParameters().etc + "#";

    var cloneFeatures = 'toolbars=0,status=1,width=' + document.body.offsetWidth + "height=" + document.body.offsetHeight;

    window.open(cloneUrl, '', cloneFeatures);
}

// Operation: Inline toolbar and button
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
        var btn = toolbar.GetButton(id)
        if (btn) {
            btn.parentNode.removeChild(btn);
        }
    };

    toolbar.GetButton = function (id) {
        return document.getElementById(id);
    };

    function StyleBuilder() {
        var cssText = new StringBuilder();
        this.Add = function (key, value) { cssText.Append(key).Append(":").Append(value).Append(";"); }
        this.ToString = function () { return cssText.ToString(); }
    }

    function StringBuilder() {
        var parts = [];
        this.Append = function (text) { parts[parts.length] = text; return this; }
        this.Reset = function () { parts = []; }
        this.ToString = function () { return parts.join(""); }
    }
}

//************************************************************************



Encoder = {

    // When encoding do we convert characters into html or numerical entities
    EncodeType: "entity",  // entity OR numerical

    isEmpty: function (val) {
        if (val) {
            return ((val === null) || val.length == 0 || /^\s+$/.test(val));
        } else {
            return true;
        }
    },
    // Convert HTML entities into numerical entities
    HTML2Numerical: function (s) {
        var arr1 = new Array('%20', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&Auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&Ouml;', '&times;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&Uuml;', '&yacute;', '&thorn;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&Oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&oelig;', '&oelig;', '&scaron;', '&scaron;', '&yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;');
        var arr2 = new Array('&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;');
        return this.swapArrayVals(s, arr1, arr2);
    },

    // Convert Numerical entities into HTML entities
    NumericalToHTML: function (s) {
        var arr1 = new Array('&#160;', '&#161;', '&#162;', '&#163;', '&#164;', '&#165;', '&#166;', '&#167;', '&#168;', '&#169;', '&#170;', '&#171;', '&#172;', '&#173;', '&#174;', '&#175;', '&#176;', '&#177;', '&#178;', '&#179;', '&#180;', '&#181;', '&#182;', '&#183;', '&#184;', '&#185;', '&#186;', '&#187;', '&#188;', '&#189;', '&#190;', '&#191;', '&#192;', '&#193;', '&#194;', '&#195;', '&#196;', '&#197;', '&#198;', '&#199;', '&#200;', '&#201;', '&#202;', '&#203;', '&#204;', '&#205;', '&#206;', '&#207;', '&#208;', '&#209;', '&#210;', '&#211;', '&#212;', '&#213;', '&#214;', '&#215;', '&#216;', '&#217;', '&#218;', '&#219;', '&#220;', '&#221;', '&#222;', '&#223;', '&#224;', '&#225;', '&#226;', '&#227;', '&#228;', '&#229;', '&#230;', '&#231;', '&#232;', '&#233;', '&#234;', '&#235;', '&#236;', '&#237;', '&#238;', '&#239;', '&#240;', '&#241;', '&#242;', '&#243;', '&#244;', '&#245;', '&#246;', '&#247;', '&#248;', '&#249;', '&#250;', '&#251;', '&#252;', '&#253;', '&#254;', '&#255;', '&#34;', '&#38;', '&#60;', '&#62;', '&#338;', '&#339;', '&#352;', '&#353;', '&#376;', '&#710;', '&#732;', '&#8194;', '&#8195;', '&#8201;', '&#8204;', '&#8205;', '&#8206;', '&#8207;', '&#8211;', '&#8212;', '&#8216;', '&#8217;', '&#8218;', '&#8220;', '&#8221;', '&#8222;', '&#8224;', '&#8225;', '&#8240;', '&#8249;', '&#8250;', '&#8364;', '&#402;', '&#913;', '&#914;', '&#915;', '&#916;', '&#917;', '&#918;', '&#919;', '&#920;', '&#921;', '&#922;', '&#923;', '&#924;', '&#925;', '&#926;', '&#927;', '&#928;', '&#929;', '&#931;', '&#932;', '&#933;', '&#934;', '&#935;', '&#936;', '&#937;', '&#945;', '&#946;', '&#947;', '&#948;', '&#949;', '&#950;', '&#951;', '&#952;', '&#953;', '&#954;', '&#955;', '&#956;', '&#957;', '&#958;', '&#959;', '&#960;', '&#961;', '&#962;', '&#963;', '&#964;', '&#965;', '&#966;', '&#967;', '&#968;', '&#969;', '&#977;', '&#978;', '&#982;', '&#8226;', '&#8230;', '&#8242;', '&#8243;', '&#8254;', '&#8260;', '&#8472;', '&#8465;', '&#8476;', '&#8482;', '&#8501;', '&#8592;', '&#8593;', '&#8594;', '&#8595;', '&#8596;', '&#8629;', '&#8656;', '&#8657;', '&#8658;', '&#8659;', '&#8660;', '&#8704;', '&#8706;', '&#8707;', '&#8709;', '&#8711;', '&#8712;', '&#8713;', '&#8715;', '&#8719;', '&#8721;', '&#8722;', '&#8727;', '&#8730;', '&#8733;', '&#8734;', '&#8736;', '&#8743;', '&#8744;', '&#8745;', '&#8746;', '&#8747;', '&#8756;', '&#8764;', '&#8773;', '&#8776;', '&#8800;', '&#8801;', '&#8804;', '&#8805;', '&#8834;', '&#8835;', '&#8836;', '&#8838;', '&#8839;', '&#8853;', '&#8855;', '&#8869;', '&#8901;', '&#8968;', '&#8969;', '&#8970;', '&#8971;', '&#9001;', '&#9002;', '&#9674;', '&#9824;', '&#9827;', '&#9829;', '&#9830;');
        var arr2 = new Array('%20', '&iexcl;', '&cent;', '&pound;', '&curren;', '&yen;', '&brvbar;', '&sect;', '&uml;', '&copy;', '&ordf;', '&laquo;', '&not;', '&shy;', '&reg;', '&macr;', '&deg;', '&plusmn;', '&sup2;', '&sup3;', '&acute;', '&micro;', '&para;', '&middot;', '&cedil;', '&sup1;', '&ordm;', '&raquo;', '&frac14;', '&frac12;', '&frac34;', '&iquest;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&Auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&Ouml;', '&times;', '&oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&Uuml;', '&yacute;', '&thorn;', '&szlig;', '&agrave;', '&aacute;', '&acirc;', '&atilde;', '&auml;', '&aring;', '&aelig;', '&ccedil;', '&egrave;', '&eacute;', '&ecirc;', '&euml;', '&igrave;', '&iacute;', '&icirc;', '&iuml;', '&eth;', '&ntilde;', '&ograve;', '&oacute;', '&ocirc;', '&otilde;', '&ouml;', '&divide;', '&Oslash;', '&ugrave;', '&uacute;', '&ucirc;', '&uuml;', '&yacute;', '&thorn;', '&yuml;', '&quot;', '&amp;', '&lt;', '&gt;', '&oelig;', '&oelig;', '&scaron;', '&scaron;', '&yuml;', '&circ;', '&tilde;', '&ensp;', '&emsp;', '&thinsp;', '&zwnj;', '&zwj;', '&lrm;', '&rlm;', '&ndash;', '&mdash;', '&lsquo;', '&rsquo;', '&sbquo;', '&ldquo;', '&rdquo;', '&bdquo;', '&dagger;', '&dagger;', '&permil;', '&lsaquo;', '&rsaquo;', '&euro;', '&fnof;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&alpha;', '&beta;', '&gamma;', '&delta;', '&epsilon;', '&zeta;', '&eta;', '&theta;', '&iota;', '&kappa;', '&lambda;', '&mu;', '&nu;', '&xi;', '&omicron;', '&pi;', '&rho;', '&sigmaf;', '&sigma;', '&tau;', '&upsilon;', '&phi;', '&chi;', '&psi;', '&omega;', '&thetasym;', '&upsih;', '&piv;', '&bull;', '&hellip;', '&prime;', '&prime;', '&oline;', '&frasl;', '&weierp;', '&image;', '&real;', '&trade;', '&alefsym;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&crarr;', '&larr;', '&uarr;', '&rarr;', '&darr;', '&harr;', '&forall;', '&part;', '&exist;', '&empty;', '&nabla;', '&isin;', '&notin;', '&ni;', '&prod;', '&sum;', '&minus;', '&lowast;', '&radic;', '&prop;', '&infin;', '&ang;', '&and;', '&or;', '&cap;', '&cup;', '&int;', '&there4;', '&sim;', '&cong;', '&asymp;', '&ne;', '&equiv;', '&le;', '&ge;', '&sub;', '&sup;', '&nsub;', '&sube;', '&supe;', '&oplus;', '&otimes;', '&perp;', '&sdot;', '&lceil;', '&rceil;', '&lfloor;', '&rfloor;', '&lang;', '&rang;', '&loz;', '&spades;', '&clubs;', '&hearts;', '&diams;');
        return this.swapArrayVals(s, arr1, arr2);
    },


    // Numerically encodes all unicode characters
    numEncode: function (s) {

        if (this.isEmpty(s)) return "";

        var e = "";
        for (var i = 0; i < s.length; i++) {
            var c = s.charAt(i);
            if (c < " " || c > "~") {
                c = "&#" + c.charCodeAt() + ";";
            }
            e += c;
        }
        return e;
    },

    // HTML Decode numerical and HTML entities back to original values
    htmlDecode: function (s) {

        var c, m, d = s;

        if (this.isEmpty(d)) return "";

        // convert HTML entites back to numerical entites first
        d = this.HTML2Numerical(d);

        // look for numerical entities &#34;
        arr = d.match(/&#[0-9]{1,5};/g);

        // if no matches found in string then skip
        if (arr != null) {
            for (var x = 0; x < arr.length; x++) {
                m = arr[x];
                c = m.substring(2, m.length - 1); //get numeric part which is refernce to unicode character
                // if its a valid number we can decode
                if (c >= -32768 && c <= 65535) {
                    // decode every single match within string
                    d = d.replace(m, String.fromCharCode(c));
                } else {
                    d = d.replace(m, ""); //invalid so replace with nada
                }
            }
        }

        return d;
    },

    // encode an input string into either numerical or HTML entities
    htmlEncode: function (s, dbl) {

        if (this.isEmpty(s)) return "";

        // do we allow double encoding? E.g will &amp; be turned into &amp;amp;
        dbl = dbl | false; //default to prevent double encoding

        // if allowing double encoding we do ampersands first
        if (dbl) {
            if (this.EncodeType == "numerical") {
                s = s.replace(/&/g, "&#38;");
            } else {
                s = s.replace(/&/g, "&amp;");
            }
        }

        // convert the xss chars to numerical entities ' " < >
        s = this.XSSEncode(s, false);

        if (this.EncodeType == "numerical" || !dbl) {
            // Now call function that will convert any HTML entities to numerical codes
            s = this.HTML2Numerical(s);
        }

        // Now encode all chars above 127 e.g unicode
        s = this.numEncode(s);

        // now we know anything that needs to be encoded has been converted to numerical entities we
        // can encode any ampersands & that are not part of encoded entities
        // to handle the fact that I need to do a negative check and handle multiple ampersands &&&
        // I am going to use a placeholder

        // if we don't want double encoded entities we ignore the & in existing entities
        if (!dbl) {
            s = s.replace(/&#/g, "##AMPHASH##");

            if (this.EncodeType == "numerical") {
                s = s.replace(/&/g, "&#38;");
            } else {
                s = s.replace(/&/g, "&amp;");
            }

            s = s.replace(/##AMPHASH##/g, "&#");
        }

        // replace any malformed entities
        s = s.replace(/&#\d*([^\d;]|$)/g, "$1");

        if (!dbl) {
            // safety check to correct any double encoded &amp;
            s = this.correctEncoding(s);
        }

        // now do we need to convert our numerical encoded string into entities
        if (this.EncodeType == "entity") {
            s = this.NumericalToHTML(s);
        }

        return s;
    },

    // Encodes the basic 4 characters used to malform HTML in XSS hacks
    XSSEncode: function (s, en) {
        if (!this.isEmpty(s)) {
            en = en || true;
            // do we convert to numerical or html entity?
            if (en) {
                s = s.replace(/\'/g, "&#39;"); //no HTML equivalent as &apos is not cross browser supported
                s = s.replace(/"/g, "&quot;");
                s = s.replace(/</g, "&lt;");
                s = s.replace(/>/g, "&gt;");
            } else {
                s = s.replace(/\'/g, "&#39;"); //no HTML equivalent as &apos is not cross browser supported
                s = s.replace(/"/g, "&#34;");
                s = s.replace(/</g, "&#60;");
                s = s.replace(/>/g, "&#62;");
            }
            return s;
        } else {
            return "";
        }
    },

    // returns true if a string contains html or numerical encoded entities
    hasEncoded: function (s) {
        if (/&#[0-9]{1,5};/g.test(s)) {
            return true;
        } else if (/&[A-Z]{2,6};/gi.test(s)) {
            return true;
        } else {
            return false;
        }
    },

    // will remove any unicode characters
    stripUnicode: function (s) {
        return s.replace(/[^\x20-\x7E]/g, "");

    },

    // corrects any double encoded &amp; entities e.g &amp;amp;
    correctEncoding: function (s) {
        return s.replace(/(&amp;)(amp;)+/, "$1");
    },


    // Function to loop through an array swaping each item with the value from another array e.g swap HTML entities with Numericals
    swapArrayVals: function (s, arr1, arr2) {
        if (this.isEmpty(s)) return "";
        var re;
        if (arr1 && arr2) {
            //ShowDebug("in swapArrayVals arr1.length = " + arr1.length + " arr2.length = " + arr2.length)
            // array lengths must match
            if (arr1.length == arr2.length) {
                for (var x = 0, i = arr1.length; x < i; x++) {
                    re = new RegExp(arr1[x], 'g');
                    s = s.replace(re, arr2[x]); //swap arr1 item with matching item from arr2	
                }
            }
        }
        return s;
    },

    inArray: function (item, arr) {
        for (var i = 0, x = arr.length; i < x; i++) {
            if (arr[i] === item) {
                return i;
            }
        }
        return -1;
    }

}

FormatExtjsDate = function (str_date) {
    if (str_date) {
        if (str_date.toString().length == 8) {
            str_date = str_date.toString().substring(0, 2) + '/' + str_date.toString().substring(2, 4) + '/' + str_date.toString().substring(4, 8)
        }
    } else {
        str_date = '';
    }
    return str_date;
}

parseXmlObject = function (xmlString) {
    if (xmlString == null) { alert('XML Object contains a null value'); return null; }
    xmlObject = new ActiveXObject("Microsoft.XMLDOM");
    xmlObject.async = false;
    xmlObject.loadXML(xmlString);

    return xmlObject;
}

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

/// base64 encode/decode
/**
*
*  Base64 encode / decode
*  http://www.webtoolkit.info/
*
**/

var Base64 = {

    // private property
    _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",

    // public method for encoding
    encode: function (input) {
        var output = "";
        var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
        var i = 0;

        input = Base64._utf8_encode(input);

        while (i < input.length) {

            chr1 = input.charCodeAt(i++);
            chr2 = input.charCodeAt(i++);
            chr3 = input.charCodeAt(i++);

            enc1 = chr1 >> 2;
            enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
            enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
            enc4 = chr3 & 63;

            if (isNaN(chr2)) {
                enc3 = enc4 = 64;
            } else if (isNaN(chr3)) {
                enc4 = 64;
            }

            output = output +
			this._keyStr.charAt(enc1) + this._keyStr.charAt(enc2) +
			this._keyStr.charAt(enc3) + this._keyStr.charAt(enc4);

        }

        return output;
    },

    // public method for decoding
    decode: function (input) {
        var output = "";
        var chr1, chr2, chr3;
        var enc1, enc2, enc3, enc4;
        var i = 0;

        input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

        while (i < input.length) {

            enc1 = this._keyStr.indexOf(input.charAt(i++));
            enc2 = this._keyStr.indexOf(input.charAt(i++));
            enc3 = this._keyStr.indexOf(input.charAt(i++));
            enc4 = this._keyStr.indexOf(input.charAt(i++));

            chr1 = (enc1 << 2) | (enc2 >> 4);
            chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
            chr3 = ((enc3 & 3) << 6) | enc4;

            output = output + String.fromCharCode(chr1);

            if (enc3 != 64) {
                output = output + String.fromCharCode(chr2);
            }
            if (enc4 != 64) {
                output = output + String.fromCharCode(chr3);
            }

        }

        output = Base64._utf8_decode(output);

        return output;

    },

    // private method for UTF-8 encoding
    _utf8_encode: function (string) {
        string = string.replace(/\r\n/g, "\n");
        var utftext = "";

        for (var n = 0; n < string.length; n++) {

            var c = string.charCodeAt(n);

            if (c < 128) {
                utftext += String.fromCharCode(c);
            }
            else if ((c > 127) && (c < 2048)) {
                utftext += String.fromCharCode((c >> 6) | 192);
                utftext += String.fromCharCode((c & 63) | 128);
            }
            else {
                utftext += String.fromCharCode((c >> 12) | 224);
                utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                utftext += String.fromCharCode((c & 63) | 128);
            }

        }

        return utftext;
    },

    // private method for UTF-8 decoding
    _utf8_decode: function (utftext) {
        var string = "";
        var i = 0;
        var c = c1 = c2 = 0;

        while (i < utftext.length) {

            c = utftext.charCodeAt(i);

            if (c < 128) {
                string += String.fromCharCode(c);
                i++;
            }
            else if ((c > 191) && (c < 224)) {
                c2 = utftext.charCodeAt(i + 1);
                string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                i += 2;
            }
            else {
                c2 = utftext.charCodeAt(i + 1);
                c3 = utftext.charCodeAt(i + 2);
                string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                i += 3;
            }

        }

        return string;
    }

}

//***********************************************************************
// Hides buttons from  IFRAME view
function HideIframeButtons(Iframe, buttonTitles) {
    Iframe.onreadystatechange = function HideTitledButtons() {
        if (Iframe.readyState == 'complete') {
            var iFrame = frames[window.event.srcElement.id];
            var liElements = iFrame.document.getElementsByTagName('li');

            for (var j = 0; j < buttonTitles.length; j++) {
                for (var i = 0; i < liElements.length; i++) {
                    if (liElements[i].getAttribute('title') == buttonTitles[j]) {
                        liElements[i].style.display = 'none';
                        break;
                    }
                }
            }
        }
    }
}

function getElementsByClassName(className, anchorNode) {
    if (!anchorNode) anchorNode = document.body;
    var result = [];
    var regEx = new RegExp("\\b" + className + "\\b");
    var children = anchorNode.getElementsByTagName("*");
    for (var i = 0; i < children.length; i++) {
        if (regEx.test(children[i].className))
            result.push(children[i]);
    }
    return result;
}
//************************************************************************

function formatCurrency(num) {
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
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
    exists = false;
    node = xmlObj.selectSingleNode(node_path);
    if (node && node != undefined && node.text && node.text != undefined && node.text.length > 0) {
        exists = true;
    }
    return exists;
}

function MultipleNodesExist(xmlObj, node_path) {
    exists = false;
    node = xmlObj.selectNodes(node_path);
    if (node && node != undefined && node[0] && node[0] != undefined && node[0].hasChildNodes()) {
        exists = true;
    }
    return exists;
}


if (typeof (SDK) == "undefined")
{ SDK = { __namespace: true }; }
// Namespace container for functions in this library.
SDK.MetaData = {
    _Context: function () {
        var errorMessage = "Context is not available.";
        if (typeof GetGlobalContext != "undefined")
        { return GetGlobalContext(); }
        else {
            if (typeof Xrm != "undefined") {
                return Xrm.Page.context;
            }
			else
			{ return new Error(errorMessage); }
        }
    },
    _ServerUrl: function () {///<summary>
        /// Private function used to establish the path to the SOAP endpoint based on context
        /// provided by the Xrm.Page object or the context object returned by the GlobalContext object.
        ///</summary>
        var ServerUrl = this._Context().getServerUrl();
        if (ServerUrl.match(/\/$/)) {
            ServerUrl = ServerUrl.substring(0, ServerUrl.length - 1);
        }
        return ServerUrl + "/XRMServices/2011/Organization.svc/web";
    },
    RetrieveAllEntitiesAsync: function (EntityFilters, RetrieveAsIfPublished, successCallBack, errorCallBack) {
        ///<summary>
        /// Sends an asynchronous RetrieveAllEntities Request to retrieve all entities in the system
        ///</summary>
        ///<returns>entityMetadataCollection</returns>
        ///<param name="EntityFilters" type="String">
        /// SDK.MetaData.EntityFilters provides dictionary for the filters available to filter which data is retrieved.
        /// Alternatively a string consisting of the values 'Entity Attributes Relationships Privileges' can be used directly.
        /// Include only those elements of the entity you want to retrieve. Retrieving all parts of all entitities may take significant time.
        ///</param>
        ///<param name="RetrieveAsIfPublished" type="Boolean">
        /// Sets whether to retrieve the metadata that has not been published.
        ///</param>
        ///<param name="successCallBack" type="Function">
        /// The function that will be passed through and be called by a successful response.
        /// This function must accept the entityMetadataCollection as a parameter.
        ///</param>
        ///<param name="errorCallBack" type="Function">
        /// The function that will be passed through and be called by a failed response.
        /// This function must accept an Error object as a parameter.
        ///</param>

        var request = "<Execute xmlns="http://schemas.microsoft.com/xrm/2011/Contracts/Services" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">";
        request += "<request i:type="a:RetrieveAllEntitiesRequest" xmlns:a="http://schemas.microsoft.com/xrm/2011/Contracts">";
        request += "<a:Parameters xmlns:b="http://schemas.datacontract.org/2004/07/System.Collections.Generic">";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>EntityFilters</b:key>";
        request += "<b:value i:type="c:EntityFilters" xmlns:c="http://schemas.microsoft.com/xrm/2011/Metadata">" + EntityFilters + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>RetrieveAsIfPublished</b:key>";
        request += "<b:value i:type="c:boolean" xmlns:c="http://www.w3.org/2001/XMLSchema">" + RetrieveAsIfPublished + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "</a:Parameters>";
        request += "<a:RequestId i:nil="true" /><a:RequestName>RetrieveAllEntities</a:RequestName></request>";
        request += "</Execute>";
        request = this._getSOAPWrapper(request);

        var req = new XMLHttpRequest();
        req.open("POST", this._ServerUrl(), true);
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", this._Action.Execute);
        req.onreadystatechange = function () { SDK.MetaData._returnAllEntities(req, successCallBack, errorCallBack) };
        req.send(request);
    },
    _returnAllEntities: function (resp, successCallBack, errorCallBack) {
        ///<summary>
        /// Private function that processes the response from SDK.MetaData.RetrieveAllEntitiesAsync
        ///</summary>
        ///<param name="resp" type="XMLHttpRequest">
        /// The XMLHttpRequest representing the response.
        ///</param>
        ///<param name="successCallBack" type="Function">
        /// The function passed through to be executed when a successful retrieval is complete.
        ///</param>
        ///<param name="errorCallBack" type="Function">
        /// The function that will be passed through and be called by a failed response.
        /// This function must accept an Error object as a parameter.
        ///</param>
        if (resp.readyState == 4 /* complete */) {
            if (resp.status == 200) {
                //Success				
                var entityMetadataNodes = resp.responseXML.selectNodes("//c:EntityMetadata");
                var entityMetadataCollection = [];
                for (var i = 0; i < entityMetadataNodes.length; i++) {
                    var entityMetadata = new SDK.MetaData._entityMetaData(entityMetadataNodes[i])
                    entityMetadataCollection.push(entityMetadata);
                }
                entityMetadataCollection.sort();
                successCallBack(entityMetadataCollection);

            }
            else {

                errorCallBack(SDK.MetaData._getError(resp));

            }

        }
    },
    RetrieveEntityAsync: function (EntityFilters, LogicalName, MetadataId, RetrieveAsIfPublished, successCallBack, errorCallBack) {
        ///<summary>
        /// Sends an asynchronous RetrieveEntity Request to retrieve a specific entity
        ///</summary>
        ///<returns>entityMetadata</returns>
        ///<param name="EntityFilters" type="String">
        /// SDK.MetaData.EntityFilters provides dictionary for the filters available to filter which data is retrieved.
        /// Alternatively a string consisting of the values 'Entity Attributes Relationships Privileges' can be used directly.
        /// Include only those elements of the entity you want to retrieve.
        ///</param>
        ///<param name="LogicalName" optional="true" type="String">
        /// The logical name of the entity requested. A null value may be used if a MetadataId is provided.
        ///</param>
        ///<param name="MetadataId" optional="true" type="String">
        /// A null value or an empty guid may be passed if a LogicalName is provided.
        ///</param>
        ///<param name="RetrieveAsIfPublished" type="Boolean">
        /// Sets whether to retrieve the metadata that has not been published.
        ///</param>
        ///<param name="successCallBack" type="Function">
        /// The function that will be passed through and be called by a successful response.
        /// This function must accept the entityMetadata as a parameter.
        ///</param>
        ///<param name="errorCallBack" type="Function">
        /// The function that will be passed through and be called by a failed response.
        /// This function must accept an Error object as a parameter.
        ///</param>
        var request = "<Execute xmlns="http://schemas.microsoft.com/xrm/2011/Contracts/Services" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">";
        request += "<request i:type="a:RetrieveEntityRequest" xmlns:a="http://schemas.microsoft.com/xrm/2011/Contracts">";
        request += "<a:Parameters xmlns:b="http://schemas.datacontract.org/2004/07/System.Collections.Generic">";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>EntityFilters</b:key>";
        request += "<b:value i:type="c:EntityFilters" xmlns:c="http://schemas.microsoft.com/xrm/2011/Metadata">" + EntityFilters + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        if (MetadataId == null)
        { MetadataId = "00000000-0000-0000-0000-000000000000"; }
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>MetadataId</b:key>";
        request += "<b:value i:type="ser:guid"  xmlns:ser="http://schemas.microsoft.com/2003/10/Serialization/">" + MetadataId + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>RetrieveAsIfPublished</b:key>";
        request += "<b:value i:type="c:boolean" xmlns:c="http://www.w3.org/2001/XMLSchema">" + RetrieveAsIfPublished + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>LogicalName</b:key>";
        request += "<b:value i:type="c:string"   xmlns:c="http://www.w3.org/2001/XMLSchema">" + LogicalName + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "</a:Parameters>";
        request += "<a:RequestId i:nil="true" /><a:RequestName>RetrieveEntity</a:RequestName></request>";
        request += "</Execute>";
        request = this._getSOAPWrapper(request);

        var req = new XMLHttpRequest();
        req.open("POST", this._ServerUrl(), true);
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", this._Action.Execute);
        req.onreadystatechange = function () { SDK.MetaData._returnEntity(req, successCallBack, errorCallBack) };
        req.send(request);
    },
    _returnEntity: function (resp, successCallBack, errorCallBack) {
        ///<summary>
        /// Private function that processes the response from SDK.MetaData.RetrieveEntityAsync
        ///</summary>
        ///<param name="resp" type="XMLHttpRequest">
        /// The XMLHttpRequest representing the response.
        ///</param>
        ///<param name="successCallBack" type="Function">
        /// The function passed through to be executed when a successful retrieval is complete.
        ///</param>
        ///<param name="errorCallBack" type="Function">
        /// The function that will be passed through and be called by a failed response.
        /// This function must accept an Error object as a parameter.
        ///</param>
        if (resp.readyState == 4 /* complete */) {
            if (resp.status == 200) {
                //Success				
                var entityMetadata = new SDK.MetaData._entityMetaData(resp.responseXML.selectSingleNode("//b:value"));

                successCallBack(entityMetadata);

            }
            else {
                //Failure
                errorCallBack(SDK.MetaData._getError(resp));
            }
        }

    },
    RetrieveAttributeAsync: function (EntityLogicalName, LogicalName, MetadataId, RetrieveAsIfPublished, successCallBack, errorCallBack) {
        var request = "<Execute xmlns="http://schemas.microsoft.com/xrm/2011/Contracts/Services" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">";
        request += "<request i:type="a:RetrieveAttributeRequest" xmlns:a="http://schemas.microsoft.com/xrm/2011/Contracts">";
        request += "<a:Parameters xmlns:b="http://schemas.datacontract.org/2004/07/System.Collections.Generic">";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>EntityLogicalName</b:key>";
        request += "<b:value i:type="c:string" xmlns:c="http://www.w3.org/2001/XMLSchema">" + EntityLogicalName + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        if (MetadataId == null)
        { MetadataId = "00000000-0000-0000-0000-000000000000"; }
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>MetadataId</b:key>";
        request += "<b:value i:type="ser:guid"  xmlns:ser="http://schemas.microsoft.com/2003/10/Serialization/">" + MetadataId + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>RetrieveAsIfPublished</b:key>";
        request += "<b:value i:type="c:boolean" xmlns:c="http://www.w3.org/2001/XMLSchema">" + RetrieveAsIfPublished + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "<a:KeyValuePairOfstringanyType>";
        request += "<b:key>LogicalName</b:key>";
        request += "<b:value i:type="c:string"   xmlns:c="http://www.w3.org/2001/XMLSchema">" + LogicalName + "</b:value>";
        request += "</a:KeyValuePairOfstringanyType>";
        request += "</a:Parameters>";
        request += "<a:RequestId i:nil="true" /><a:RequestName>RetrieveAttribute</a:RequestName></request>";
        request += "</Execute>";
        request = this._getSOAPWrapper(request);

        var req = new XMLHttpRequest();
        req.open("POST", this._ServerUrl(), true);
        req.setRequestHeader("Accept", "application/xml, text/xml, */*");
        req.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        req.setRequestHeader("SOAPAction", this._Action.Execute);
        req.onreadystatechange = function () { SDK.MetaData._returnAttribute(req, successCallBack, errorCallBack) };
        req.send(request);
    },
    _returnAttribute: function (resp, successCallBack, errorCallBack) {
        if (resp.readyState == 4 /* complete */) {
            if (resp.status == 200) {
                //Success				
                //var attributeMetadata = new SDK.MetaData._attributeMetadata(resp.responseXML.selectSingleNode("//b:value"));


                var attributeData = resp.responseXML.selectSingleNode("//b:value");
                var attributeType = attributeData.selectSingleNode("c:AttributeType").text;
                var attribute = {};
                switch (attributeType) {
                    case "BigInt":
                        attribute = new SDK.MetaData._bigIntAttributeMetadata(attributeData);
                        break;
                    case "Boolean":
                        attribute = new SDK.MetaData._booleanAttributeMetadata(attributeData);
                        break;
                    case "CalendarRules":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "Customer":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "DateTime":
                        attribute = new SDK.MetaData._dateTimeAttributeMetadata(attributeData);
                        break;
                    case "Decimal":
                        attribute = new SDK.MetaData._decimalAttributeMetadata(attributeData);
                        break;
                    case "Double":
                        attribute = new SDK.MetaData._doubleAttributeMetadata(attributeData);
                        break;
                    case "EntityName":
                        attribute = new SDK.MetaData._entityNameAttributeMetadata(attributeData);
                        break;
                    case "Integer":
                        attribute = new SDK.MetaData._integerAttributeMetadata(attributeData);
                        break;
                    case "Lookup":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "ManagedProperty":
                        attribute = new SDK.MetaData._managedPropertyAttributeMetadata(attributeData);
                        break;
                    case "Memo":
                        attribute = new SDK.MetaData._memoAttributeMetadata(attributeData);
                        break;
                    case "Money":
                        attribute = new SDK.MetaData._moneyAttributeMetadata(attributeData);
                        break;
                    case "Owner":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "PartyList":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "Picklist":
                        attribute = new SDK.MetaData._picklistAttributeMetadata(attributeData);
                        break;
                    case "State":
                        attribute = new SDK.MetaData._stateAttributeMetadata(attributeData);
                        break;
                    case "Status":
                        attribute = new SDK.MetaData._statusAttributeMetadata(attributeData);
                        break;
                    case "String":
                        attribute = new SDK.MetaData._stringAttributeMetadata(attributeData);
                        break;
                    case "Uniqueidentifier":
                        attribute = new SDK.MetaData._attributeMetadata(attributeData);
                        break;
                    case "Virtual": //Contains the text value of picklist fields
                        attribute = new SDK.MetaData._attributeMetadata(attributeData);

                        break;
                }


                successCallBack(attribute);

            }
            else {
                //Failure
                errorCallBack(SDK.MetaData._getError(resp));
            }
        }
    },
    _getError: function (resp) {
        ///<summary>
        /// Private function that attempts to parse errors related to connectivity or WCF faults.
        ///</summary>
        ///<param name="resp" type="XMLHttpRequest">
        /// The XMLHttpRequest representing failed response.
        ///</param>

        //Error descriptions come from http://support.microsoft.com/kb/193625
        if (resp.status == 12029)
        { return new Error("The attempt to connect to the server failed."); }
        if (resp.status == 12007)
        { return new Error("The server name could not be resolved."); }
        var faultXml = resp.responseXML;
        var errorMessage = "Unknown (unable to parse the fault)";
        if (typeof faultXml == "object") {

            var bodyNode = faultXml.firstChild.firstChild;

            //Retrieve the fault node
            for (var i = 0; i < bodyNode.childNodes.length; i++) {
                var node = bodyNode.childNodes[i];

                //NOTE: This comparison does not handle the case where the XML namespace changes
                if ("s:Fault" == node.nodeName) {
                    for (var j = 0; j < node.childNodes.length; j++) {
                        var faultStringNode = node.childNodes[j];
                        if ("faultstring" == faultStringNode.nodeName) {
                            errorMessage = faultStringNode.text;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        return new Error(errorMessage);

    },
    EntityFilters: {
        All: "Entity Attributes Relationships Privileges",
        Default: "Entity Attributes Relationships Privileges",
        Attributes: "Attributes",
        Entity: "Entity",
        Privileges: "Privileges",
        Relationships: "Relationships"
    },
    _Action: {
        Execute: "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute"
    },
    _getSOAPWrapper: function (request) {
        ///<summary>
        /// Private function that wraps a soap envelope around a request.
        ///</summary>
        var SOAP = "<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"><soapenv:Body>";
        SOAP += request;
        SOAP += "</soapenv:Body></soapenv:Envelope>";
        return SOAP;
    },
    _associatedMenuConfiguration: function (node) {
        ///<summary>
        /// Private function that parses xml data describing AssociatedMenuConfiguration
        ///</summary>
        var orderValue;
        if (isNaN(parseInt(node.selectSingleNode("c:Order").text, 10)))
        { orderValue = null; }
        else
        { orderValue = parseInt(node.selectSingleNode("c:Order").text, 10); }
        return {
            Behavior: node.selectSingleNode("c:Behavior").text,
            Group: node.selectSingleNode("c:Group").text,
            Label: new SDK.MetaData._label(node.selectSingleNode("c:Label")),
            Order: orderValue
        };
    },
    _oneToManyRelationshipMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing OneToManyRelationshipMetadata
        ///</summary>
		return { OneToManyRelationshipMetadata: {
                MetadataId: node.selectSingleNode("c:MetadataId").text,
                IsCustomRelationship: (node.selectSingleNode("c:IsCustomRelationship").text == "true") ? true : false,
                IsCustomizable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsCustomizable")),
                IsManaged: (node.selectSingleNode("c:IsManaged").text == "true") ? true : false,
                IsValidForAdvancedFind: (node.selectSingleNode("c:IsValidForAdvancedFind").text == "true") ? true : false,
                SchemaName: node.selectSingleNode("c:SchemaName").text,
                SecurityTypes: node.selectSingleNode("c:SecurityTypes").text,
                AssociatedMenuConfiguration: new SDK.MetaData._associatedMenuConfiguration(node.selectSingleNode("c:AssociatedMenuConfiguration")),
                CascadeConfiguration: {
                    Assign: node.selectSingleNode("c:CascadeConfiguration/c:Assign").text,
                    Delete: node.selectSingleNode("c:CascadeConfiguration/c:Delete").text,
                    Merge: node.selectSingleNode("c:CascadeConfiguration/c:Merge").text,
                    Reparent: node.selectSingleNode("c:CascadeConfiguration/c:Reparent").text,
                    Share: node.selectSingleNode("c:CascadeConfiguration/c:Share").text,
                    Unshare: node.selectSingleNode("c:CascadeConfiguration/c:Unshare").text
                },
                ReferencedAttribute: node.selectSingleNode("c:ReferencedAttribute").text,
                ReferencedEntity: node.selectSingleNode("c:ReferencedEntity").text,
                ReferencingAttribute: node.selectSingleNode("c:ReferencingAttribute").text,
                ReferencingEntity: node.selectSingleNode("c:ReferencingEntity").text
            }
        };
    },
    _manyToManyRelationshipMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing ManyToManyRelationshipMetadata
        ///</summary>
		return { ManyToManyRelationshipMetadata: {
                MetadataId: node.selectSingleNode("c:MetadataId").text,
                IsCustomRelationship: (node.selectSingleNode("c:IsCustomRelationship").text == "true") ? true : false,
                IsCustomizable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsCustomizable")),
                IsManaged: (node.selectSingleNode("c:IsManaged").text == "true") ? true : false,
                IsValidForAdvancedFind: (node.selectSingleNode("c:IsValidForAdvancedFind").text == "true") ? true : false,
                SchemaName: node.selectSingleNode("c:SchemaName").text,
                SecurityTypes: node.selectSingleNode("c:SecurityTypes").text,
                Entity1AssociatedMenuConfiguration: new SDK.MetaData._associatedMenuConfiguration(node.selectSingleNode("c:Entity1AssociatedMenuConfiguration")),
                Entity1IntersectAttribute: node.selectSingleNode("c:Entity1IntersectAttribute").text,
                Entity1LogicalName: node.selectSingleNode("c:Entity1LogicalName").text,
                Entity2AssociatedMenuConfiguration: new SDK.MetaData._associatedMenuConfiguration(node.selectSingleNode("c:Entity2AssociatedMenuConfiguration")),
                Entity2IntersectAttribute: node.selectSingleNode("c:Entity2IntersectAttribute").text,
                Entity2LogicalName: node.selectSingleNode("c:Entity2LogicalName").text,
                IntersectEntityName: node.selectSingleNode("c:IntersectEntityName").text
            }
        };
    },
    _entityMetaData: function (node) {
        ///<summary>
        /// Private function that parses xml data describing EntityMetaData
        ///</summary>
        //Check for Attributes and add them if they are included.
        var attributes = [];
        var attributesData = node.selectSingleNode("c:Attributes")
        if (attributesData.childNodes.length > 0) {
            //There are attributes
            for (var i = 0; i < attributesData.childNodes.length; i++) {
                var attributeData = attributesData.childNodes[i];
                var attributeType = attributeData.selectSingleNode("c:AttributeType").text;
                var attribute = {};
                switch (attributeType) {
                    case "BigInt":
                        attribute = new SDK.MetaData._bigIntAttributeMetadata(attributeData);
                        break;
                    case "Boolean":
                        attribute = new SDK.MetaData._booleanAttributeMetadata(attributeData);
                        break;
                    case "CalendarRules":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "Customer":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "DateTime":
                        attribute = new SDK.MetaData._dateTimeAttributeMetadata(attributeData);
                        break;
                    case "Decimal":
                        attribute = new SDK.MetaData._decimalAttributeMetadata(attributeData);
                        break;
                    case "Double":
                        attribute = new SDK.MetaData._doubleAttributeMetadata(attributeData);
                        break;
                    case "EntityName":
                        attribute = new SDK.MetaData._entityNameAttributeMetadata(attributeData);
                        break;
                    case "Integer":
                        attribute = new SDK.MetaData._integerAttributeMetadata(attributeData);
                        break;
                    case "Lookup":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "ManagedProperty":
                        attribute = new SDK.MetaData._managedPropertyAttributeMetadata(attributeData);
                        break;
                    case "Memo":
                        attribute = new SDK.MetaData._memoAttributeMetadata(attributeData);
                        break;
                    case "Money":
                        attribute = new SDK.MetaData._moneyAttributeMetadata(attributeData);
                        break;
                    case "Owner":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "PartyList":
                        attribute = new SDK.MetaData._lookupAttributeMetadata(attributeData);
                        break;
                    case "Picklist":
                        attribute = new SDK.MetaData._picklistAttributeMetadata(attributeData);
                        break;
                    case "State":
                        attribute = new SDK.MetaData._stateAttributeMetadata(attributeData);
                        break;
                    case "Status":
                        attribute = new SDK.MetaData._statusAttributeMetadata(attributeData);
                        break;
                    case "String":
                        attribute = new SDK.MetaData._stringAttributeMetadata(attributeData);
                        break;
                    case "Uniqueidentifier":
                        attribute = new SDK.MetaData._attributeMetadata(attributeData);
                        break;
                    case "Virtual": //Contains the text value of picklist fields
                        attribute = new SDK.MetaData._attributeMetadata(attributeData);

                        break;
                }
                attributes.push(attribute);

            }
            attributes.sort();
        }

        //Check for Privileges and add them if they are included.
        var privileges = [];
        var privilegesData = node.selectSingleNode("c:Privileges");
        if (privilegesData.childNodes.length > 0) {
            for (var i = 0; i < privilegesData.childNodes.length; i++) {
                var privilegeData = privilegesData.childNodes[i];
                var securityPrivilegeMetadata = {
                    SecurityPrivilegeMetadata: {
                        CanBeBasic: (privilegeData.selectSingleNode("c:CanBeBasic").text == "true") ? true : false,
                        CanBeDeep: (privilegeData.selectSingleNode("c:CanBeDeep").text == "true") ? true : false,
                        CanBeGlobal: (privilegeData.selectSingleNode("c:CanBeGlobal").text == "true") ? true : false,
                        CanBeLocal: (privilegeData.selectSingleNode("c:CanBeLocal").text == "true") ? true : false,
                        Name: privilegeData.selectSingleNode("c:Name").text,
                        PrivilegeId: privilegeData.selectSingleNode("c:PrivilegeId").text,
                        PrivilegeType: privilegeData.selectSingleNode("c:PrivilegeType").text
                    }
                };
                privileges.push(securityPrivilegeMetadata);
            }
        }

        //Check for Relationships and add them if they are included.
        var manyToManyRelationships = [];
        var manyToManyRelationshipsData = node.selectSingleNode("c:ManyToManyRelationships");
        if (manyToManyRelationshipsData.childNodes.length > 0) {
            for (var i = 0; i < manyToManyRelationshipsData.childNodes.length; i++) {
                var manyToManyRelationshipMetadataData = manyToManyRelationshipsData.childNodes[i];

                var manyToManyRelationshipMetadata = new SDK.MetaData._manyToManyRelationshipMetadata(manyToManyRelationshipMetadataData);
                manyToManyRelationships.push(manyToManyRelationshipMetadata);
            }
        }

        var manyToOneRelationships = [];
        var manyToOneRelationshipsData = node.selectSingleNode("c:ManyToOneRelationships");
        if (manyToOneRelationshipsData.childNodes.length > 0) {

            for (var i = 0; i < manyToOneRelationshipsData.childNodes.length; i++) {
                var manyToOneRelationshipMetadata = new SDK.MetaData._oneToManyRelationshipMetadata(manyToOneRelationshipsData.childNodes[i]);

                manyToOneRelationships.push(manyToOneRelationshipMetadata);

            }

        }

        var oneToManyRelationships = [];
        var oneToManyRelationshipsData = node.selectSingleNode("c:OneToManyRelationships");
        if (oneToManyRelationshipsData.childNodes.length > 0) {
            for (var i = 0; i < oneToManyRelationshipsData.childNodes.length; i++) {
                var oneToManyRelationshipMetadata = new SDK.MetaData._oneToManyRelationshipMetadata(oneToManyRelationshipsData.childNodes[i]);
                oneToManyRelationships.push(oneToManyRelationshipMetadata);
            }
        }


        return {
            ActivityTypeMask: SDK.MetaData._nullableInt(node.selectSingleNode("c:ActivityTypeMask")),
            Attributes: attributes,
            AutoRouteToOwnerQueue: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:AutoRouteToOwnerQueue")),
            CanBeInManyToMany: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanBeInManyToMany")),
            CanBePrimaryEntityInRelationship: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanBePrimaryEntityInRelationship")),
            CanBeRelatedEntityInRelationship: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanBeRelatedEntityInRelationship")),
            CanCreateAttributes: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanCreateAttributes")),
            CanCreateCharts: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanCreateCharts")),
            CanCreateForms: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanCreateForms")),
            CanCreateViews: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanCreateViews")),
            CanModifyAdditionalSettings: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanModifyAdditionalSettings")),
            CanTriggerWorkflow: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:CanTriggerWorkflow")),
            Description: new SDK.MetaData._label(node.selectSingleNode("c:Description")),
            DisplayCollectionName: new SDK.MetaData._label(node.selectSingleNode("c:DisplayCollectionName")),
            DisplayName: new SDK.MetaData._label(node.selectSingleNode("c:DisplayName")),
            IconLargeName: node.selectSingleNode("c:IconLargeName").text,
            IconMediumName: node.selectSingleNode("c:IconMediumName").text,
            IconSmallName: node.selectSingleNode("c:IconSmallName").text,
            IsActivity: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsActivity")),
            IsActivityParty: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsActivityParty")),
            IsAuditEnabled: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsAuditEnabled")),
            IsAvailableOffline: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsAvailableOffline")),
            IsChildEntity: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsChildEntity")),
            IsConnectionsEnabled: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsConnectionsEnabled")),
            IsCustomEntity: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsCustomEntity")),
            IsCustomizable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsCustomizable")),
            IsDocumentManagementEnabled: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsDocumentManagementEnabled")),
            IsDuplicateDetectionEnabled: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsDuplicateDetectionEnabled")),
            IsEnabledForCharts: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsEnabledForCharts")),
            IsImportable: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsImportable")),
            IsIntersect: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsIntersect")),
            IsMailMergeEnabled: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsMailMergeEnabled")),
            IsManaged: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsManaged")),
            IsMappable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsMappable")),
            IsReadingPaneEnabled: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsReadingPaneEnabled")),
            IsRenameable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsRenameable")),
            IsValidForAdvancedFind: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsValidForAdvancedFind")),
            IsValidForQueue: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsValidForQueue")),
            IsVisibleInMobile: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsVisibleInMobile")),
            LogicalName: node.selectSingleNode("c:LogicalName").text,
            ManyToManyRelationships: manyToManyRelationships,
            ManyToOneRelationships: manyToOneRelationships,
            MetadataId: node.selectSingleNode("c:MetadataId").text,
            ObjectTypeCode: SDK.MetaData._nullableInt(node.selectSingleNode("c:ObjectTypeCode")),
            OneToManyRelationships: oneToManyRelationships,
            OwnershipType: node.selectSingleNode("c:OwnershipType").text,
            PrimaryIdAttribute: node.selectSingleNode("c:PrimaryIdAttribute").text,
            PrimaryNameAttribute: node.selectSingleNode("c:PrimaryNameAttribute").text,
            Privileges: privileges,
            RecurrenceBaseEntityLogicalName: node.selectSingleNode("c:RecurrenceBaseEntityLogicalName").text,
            ReportViewName: node.selectSingleNode("c:ReportViewName").text,
            SchemaName: node.selectSingleNode("c:SchemaName").text,
            // So the LogicalName property will be used for an array.sort().
            toString: function () { return this.LogicalName }
        };


    },
    _nullableInt: function (node) {
        ///<summary>
        /// Private function that parses xml data describing nullable Integer values
        ///</summary>
        if (node.text == "")
        { return null; }
        else
        { return parseInt(node.text, 10); }

    },
    _nullableBoolean: function (node) {
        ///<summary>
        /// Private function that parses xml data describing nullable Boolean values
        ///</summary>
        if (node.text == "")
        { return null; }
        if (node.text == "true")
        { return true; }
        else
        { return false; }
    },
    _booleanManagedProperty: function (node) {
        ///<summary>
        /// Private function that parses xml data describing BooleanManagedProperty 
        ///</summary>
        return {
            CanBeChanged: (node.selectSingleNode("a:CanBeChanged").text == "true") ? true : false,
            ManagedPropertyLogicalName: node.selectSingleNode("a:ManagedPropertyLogicalName").text,
            Value: (node.selectSingleNode("a:Value").text == "true") ? true : false
        };

    },
    _requiredLevelManagedProperty: function (node) {
        ///<summary>
        /// Private function that parses xml data describing AttributeRequiredLevelManagedProperty  
        ///</summary>
        return {
            CanBeChanged: (node.selectSingleNode("a:CanBeChanged").text == "true") ? true : false,
            ManagedPropertyLogicalName: node.selectSingleNode("a:ManagedPropertyLogicalName").text,
            Value: node.selectSingleNode("a:Value").text
        };

    },
    _label: function (node) {
        ///<summary>
        /// Private function that parses xml data describing Label 
        ///</summary>
        if (node.text == "") {
            return {
                LocalizedLabels: [],
                UserLocalizedLabel: null
            };
        }
        else {
            var locLabels = node.selectSingleNode("a:LocalizedLabels");
            var userLocLabel = node.selectSingleNode("a:UserLocalizedLabel");
            var arrLocLabels = [];
            for (var i = 0; i < locLabels.childNodes.length; i++) {
                var LocLabelNode = locLabels.childNodes[i];
                var locLabel = {
                    LocalizedLabel: {
                        IsManaged: (LocLabelNode.selectSingleNode("a:IsManaged").text == "true") ? true : false,
                        Label: LocLabelNode.selectSingleNode("a:Label").text,
                        LanguageCode: parseInt(LocLabelNode.selectSingleNode("a:LanguageCode").text, 10)
                    }
                };
                arrLocLabels.push(locLabel);
            }

            return {
                LocalizedLabels: arrLocLabels,
                UserLocalizedLabel: {
                    IsManaged: (userLocLabel.selectSingleNode("a:IsManaged").text == "true") ? true : false,
                    Label: userLocLabel.selectSingleNode("a:Label").text,
                    LanguageCode: parseInt(userLocLabel.selectSingleNode("a:LanguageCode").text, 10)
                }
            };

        }


    },
    _options: function (node) {
        ///<summary>
        /// Private function that parses xml data describing OptionSetMetadata Options 
        ///</summary>
        var optionMetadatas = [];
        for (var i = 0; i < node.childNodes.length; i++) {
            var optionMetadata = node.childNodes[i];
            var option;
            if (optionMetadata.attributes.getNamedItem("i:type") != null && optionMetadata.attributes.getNamedItem("i:type").value == "c:StatusOptionMetadata") {

				option = { StatusOptionMetadata:
			   { MetadataId: optionMetadata.selectSingleNode("c:MetadataId").text,
                       Description: new SDK.MetaData._label(optionMetadata.selectSingleNode("c:Description")),
                       IsManaged: SDK.MetaData._nullableBoolean(optionMetadata.selectSingleNode("c:IsManaged")),
                       Label: new SDK.MetaData._label(optionMetadata.selectSingleNode("c:Label")),
                       Value: parseInt(optionMetadata.selectSingleNode("c:Value").text, 10),
                       State: parseInt(optionMetadata.selectSingleNode("c:State").text, 10)

                   }
                };
            }
            else {
                if (optionMetadata.attributes.getNamedItem("i:type") != null && optionMetadata.attributes.getNamedItem("i:type").value == "c:StateOptionMetadata") {

					option = { StateOptionMetadata:
			   { MetadataId: optionMetadata.selectSingleNode("c:MetadataId").text,
                       Description: new SDK.MetaData._label(optionMetadata.selectSingleNode("c:Description")),
                       IsManaged: SDK.MetaData._nullableBoolean(optionMetadata.selectSingleNode("c:IsManaged")),
                       Label: new SDK.MetaData._label(optionMetadata.selectSingleNode("c:Label")),
                       Value: parseInt(optionMetadata.selectSingleNode("c:Value").text, 10),
                       DefaultStatus: parseInt(optionMetadata.selectSingleNode("c:DefaultStatus ").text, 10),
                       InvariantName: optionMetadata.selectSingleNode("c:InvariantName").text

                   }
                    };
                }
                else {
					option = { OptionMetadata:
				{ MetadataId: optionMetadata.selectSingleNode("c:MetadataId").text,
                        Description: new SDK.MetaData._label(optionMetadata.selectSingleNode("c:Description")),
                        IsManaged: SDK.MetaData._nullableBoolean(optionMetadata.selectSingleNode("c:IsManaged")),
                        Label: new SDK.MetaData._label(optionMetadata.selectSingleNode("c:Label")),
                        Value: parseInt(optionMetadata.selectSingleNode("c:Value").text, 10)

                    }
                    };
                }
            }


            optionMetadatas.push(option);

        }
        return optionMetadatas;
    },
    _booleanOptionSet: function (node) {
        ///<summary>
        /// Private function that parses xml data describing BooleanOptionSetMetadata 
        ///</summary>
        if (node.childNodes.length == 0)
        { return null; }
        else {
            return {
                MetadataId: node.selectSingleNode("c:MetadataId").text,
                Description: new SDK.MetaData._label(node.selectSingleNode("c:Description")),
                DisplayName: new SDK.MetaData._label(node.selectSingleNode("c:DisplayName")),
                IsCustomOptionSet: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsCustomOptionSet")),
                IsCustomizable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsCustomizable")),
                IsGlobal: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsGlobal")),
                IsManaged: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsManaged")),
                Name: node.selectSingleNode("c:Name").text,
                OptionSetType: node.selectSingleNode("c:OptionSetType").text,
                FalseOption: {
                    MetadataId: node.selectSingleNode("c:FalseOption/c:MetadataId").text,
                    Description: new SDK.MetaData._label(node.selectSingleNode("c:FalseOption/c:Description")),
                    IsManaged: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:FalseOption/c:IsManaged")),
                    Label: new SDK.MetaData._label(node.selectSingleNode("c:FalseOption/c:Label")),
                    Value: parseInt(node.selectSingleNode("c:FalseOption/c:Value").text, 10)
                },
                TrueOption: {
                    MetadataId: node.selectSingleNode("c:TrueOption/c:MetadataId").text,
                    Description: new SDK.MetaData._label(node.selectSingleNode("c:TrueOption/c:Description")),
                    IsManaged: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:TrueOption/c:IsManaged")),
                    Label: new SDK.MetaData._label(node.selectSingleNode("c:TrueOption/c:Label")),
                    Value: parseInt(node.selectSingleNode("c:TrueOption/c:Value").text, 10)
                }
            };
        }


    },
    _optionSet: function (node) {
        ///<summary>
        /// Private function that parses xml data describing OptionSetMetadata 
        ///</summary>
        if (node.childNodes.length == 0)
        { return null; }
        else {
            return {
                MetadataId: node.selectSingleNode("c:MetadataId").text,
                Description: new SDK.MetaData._label(node.selectSingleNode("c:Description")),
                DisplayName: new SDK.MetaData._label(node.selectSingleNode("c:DisplayName")),
                IsCustomOptionSet: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsCustomOptionSet")),
                IsCustomizable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsCustomizable")),
                IsGlobal: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsGlobal")),
                IsManaged: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsManaged")),
                Name: node.selectSingleNode("c:Name").text,
                OptionSetType: node.selectSingleNode("c:OptionSetType").text,
                Options: new SDK.MetaData._options(node.selectSingleNode("c:Options"))
            };
        }


    },
    _attributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing AttributeMetadata 
        ///</summary>
        return {
            AttributeOf: node.selectSingleNode("c:AttributeOf").text,
            AttributeType: node.selectSingleNode("c:AttributeType").text,
            CanBeSecuredForCreate: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:CanBeSecuredForCreate")),
            CanBeSecuredForRead: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:CanBeSecuredForRead")),
            CanBeSecuredForUpdate: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:CanBeSecuredForUpdate")),
            CanModifyAdditionalSettings: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:CanModifyAdditionalSettings")),
            ColumnNumber: SDK.MetaData._nullableInt(node.selectSingleNode("c:ColumnNumber")),
            DeprecatedVersion: node.selectSingleNode("c:DeprecatedVersion").text,
            Description: new SDK.MetaData._label(node.selectSingleNode("c:Description")),
            DisplayName: new SDK.MetaData._label(node.selectSingleNode("c:DisplayName")),
            EntityLogicalName: node.selectSingleNode("c:EntityLogicalName").text,
            ExtensionData: null, //No node for ExtensionData
            IsAuditEnabled: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsAuditEnabled")),
            IsCustomAttribute: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsCustomAttribute")),
            IsCustomizable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsCustomizable")),
            IsManaged: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsManaged")),
            IsPrimaryId: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsPrimaryId")),
            IsPrimaryName: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsPrimaryName")),
            IsRenameable: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsRenameable")),
            IsSecured: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsSecured")),
            IsValidForAdvancedFind: new SDK.MetaData._booleanManagedProperty(node.selectSingleNode("c:IsValidForAdvancedFind")),
            IsValidForCreate: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsValidForCreate")),
            IsValidForRead: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsValidForRead")),
            IsValidForUpdate: SDK.MetaData._nullableBoolean(node.selectSingleNode("c:IsValidForUpdate")),
            LinkedAttributeId: node.selectSingleNode("c:LinkedAttributeId").text,
            LogicalName: node.selectSingleNode("c:LogicalName").text,
            MetadataId: node.selectSingleNode("c:MetadataId").text,
            RequiredLevel: new SDK.MetaData._requiredLevelManagedProperty(node.selectSingleNode("c:RequiredLevel")),
            SchemaName: node.selectSingleNode("c:SchemaName").text,
            toString: function ()
            { return this.LogicalName; }
        };
    },
    _enumAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing EnumAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        //FIXED: DefaultFormValue was using _nullableBoolean. 
        attributeMetadata.DefaultFormValue = SDK.MetaData._nullableInt(node.selectSingleNode("c:DefaultFormValue")),
		attributeMetadata.OptionSet = new SDK.MetaData._optionSet(node.selectSingleNode("c:OptionSet"));

        return attributeMetadata;

    },
    _stateAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing StateAttributeMetadata 
        ///</summary>
        var enumAttributeMetadata = new SDK.MetaData._enumAttributeMetadata(node);

        return enumAttributeMetadata;
    },
    _stringAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing StringAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);

        attributeMetadata.Format = node.selectSingleNode("c:Format").text;
        attributeMetadata.ImeMode = node.selectSingleNode("c:ImeMode").text;
        attributeMetadata.MaxLength = parseInt(node.selectSingleNode("c:MaxLength").text, 10);
        attributeMetadata.YomiOf = node.selectSingleNode("c:YomiOf").text;

        return attributeMetadata;

    },
    _managedPropertyAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing ManagedPropertyAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.ManagedPropertyLogicalName = node.selectSingleNode("c:ManagedPropertyLogicalName").text;
        attributeMetadata.ParentAttributeName = node.selectSingleNode("c:ParentAttributeName").text;
        attributeMetadata.ParentComponentType = SDK.MetaData._nullableInt(node.selectSingleNode("c:ParentComponentType"));
        attributeMetadata.ValueAttributeTypeCode = node.selectSingleNode("c:ValueAttributeTypeCode").text;

        return attributeMetadata;

    },
    _bigIntAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing BigIntAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.MaxValue = node.selectSingleNode("c:MaxValue").text;
        attributeMetadata.MinValue = node.selectSingleNode("c:MaxValue").text;

        return attributeMetadata;

    },
    _booleanAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing BooleanAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.DefaultValue = SDK.MetaData._nullableBoolean(node.selectSingleNode("c:DefaultValue"));
        attributeMetadata.OptionSet = new SDK.MetaData._booleanOptionSet(node.selectSingleNode("c:OptionSet"));

        return attributeMetadata;
    },
    _dateTimeAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing DateTimeAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.Format = node.selectSingleNode("c:Format").text;
        attributeMetadata.ImeMode = node.selectSingleNode("c:ImeMode").text;


        return attributeMetadata;
    },
    _decimalAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing DecimalAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.ImeMode = node.selectSingleNode("c:ImeMode").text;
        attributeMetadata.MaxValue = node.selectSingleNode("c:MaxValue").text;
        attributeMetadata.MinValue = node.selectSingleNode("c:MinValue").text;
        attributeMetadata.Precision = parseInt(node.selectSingleNode("c:Precision").text, 10);

        return attributeMetadata;
    },
    _doubleAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing DoubleAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.ImeMode = node.selectSingleNode("c:ImeMode").text,
		attributeMetadata.MaxValue = node.selectSingleNode("c:MaxValue").text;
        attributeMetadata.MinValue = node.selectSingleNode("c:MinValue").text;
        attributeMetadata.Precision = parseInt(node.selectSingleNode("c:Precision").text, 10);

        return attributeMetadata;
    },
    _entityNameAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing EntityNameAttributeMetadata 
        ///</summary>
        var _enumAttributeMetadata = new SDK.MetaData._enumAttributeMetadata(node);

        return _enumAttributeMetadata;
    },
    _integerAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing IntegerAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.Format = node.selectSingleNode("c:Format").text;
        attributeMetadata.MaxValue = parseInt(node.selectSingleNode("c:MaxValue").text, 10);
        attributeMetadata.MinValue = parseInt(node.selectSingleNode("c:MinValue").text, 10);

        return attributeMetadata;
    },
    _picklistAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing PicklistAttributeMetadata 
        ///</summary>
        var enumAttributeMetadata = new SDK.MetaData._enumAttributeMetadata(node);

        return enumAttributeMetadata;
    },
    _statusAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing StatusAttributeMetadata 
        ///</summary>
        var enumAttributeMetadata = new SDK.MetaData._enumAttributeMetadata(node);


        return enumAttributeMetadata;
    },
    _memoAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing MemoAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.Format = node.selectSingleNode("c:Format").text;
        attributeMetadata.ImeMode = node.selectSingleNode("c:ImeMode").text;
        attributeMetadata.MaxLength = parseInt(node.selectSingleNode("c:MaxLength").text, 10);

        return attributeMetadata;
    },
    _moneyAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing MoneyAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.CalculationOf = node.selectSingleNode("c:CalculationOf").text;
        attributeMetadata.ImeMode = node.selectSingleNode("c:ImeMode").text;
        attributeMetadata.MaxValue = node.selectSingleNode("c:MaxValue").text;
        attributeMetadata.MinValue = node.selectSingleNode("c:MinValue").text;
        attributeMetadata.Precision = parseInt(node.selectSingleNode("c:Precision").text, 10);
        attributeMetadata.PrecisionSource = SDK.MetaData._nullableInt(node.selectSingleNode("c:PrecisionSource"));

        return attributeMetadata;
    },
    _lookupAttributeMetadata: function (node) {
        ///<summary>
        /// Private function that parses xml data describing LookupAttributeMetadata 
        ///</summary>
        var attributeMetadata = new SDK.MetaData._attributeMetadata(node);
        attributeMetadata.Targets = null;

        return attributeMetadata;
    },
    __namespace: true
};

function successRetrieveAllEntities(entityMetadataCollection) {
    ///<summary>
    /// Recieves the data from SDK.MetaData.RetrieveAllEntitiesAsync and 
    /// appends a list item to results for each one.
    ///</summary>

    for (var i = 0; i < entityMetadataCollection.length; i++) {

        var entity = entityMetadataCollection[i];
        var entityNode = document.createElement("li");
        var entitySpan = document.createElement("span");

        entitySpan.innerText = entity.SchemaName;
        entitySpan.id = entity.LogicalName;
        entitySpan.title = "Click to Retrieve Attributes.";
        entitySpan.attributesRetrieved = false;
        // Add the event handler to retrieve attributes 
        entitySpan.onclick = retrieveAttributes;
        entitySpan.style.cursor = "pointer";

        entityNode.appendChild(entitySpan);
        results.appendChild(entityNode);

    }

    message.innerText = entityMetadataCollection.length + " entities retrieved.";

}
function errorRetrieveAllEntities(error) {
    ///<summary>
    /// Displays the error returned from  SDK.MetaData.RetrieveAllEntitiesAsync if it fails.
    ///</summary>
    message.innerText = error.message;
}

function retrieveAttributes() {
    ///<summary>
    /// Retrieves attributes for the entity list item that is clicked
    ///</summary>
    if (this.attributesRetrieved == false) {
        var entityLogicalName = this.id;
        // Display an entity list item level notification while retrieving data.
        var notification = document.createElement("span");
        notification.innerText = "   Retrieving attributes for " + this.innerText + "...";
        notification.id = entityLogicalName + "Notification";
        this.parentElement.appendChild(notification);

        SDK.MetaData.RetrieveEntityAsync(SDK.MetaData.EntityFilters.Attributes, this.id, null, false, function (entityMetadata) { successRetrieveEntity(entityLogicalName, entityMetadata); }, errorRetrieveEntity);


    }
    this.attributesRetrieved = true;
    this.title = "";
}

function successRetrieveEntity(logicalName, entityMetadata) {
    ///<summary>
    /// Retrieves attributes for the entity list item that is clicked
    ///</summary>

    // Update the entity list item notification when data is retrieved.
    var notification = document.getElementById(logicalName + "Notification").innerText = "   Retrieved " + entityMetadata.Attributes.length + " attributes.";

    var attributeList = document.createElement("ul");

    for (var i = 0; i < entityMetadata.Attributes.length; i++) {
        var attribute = entityMetadata.Attributes[i];
        var attributeNode = document.createElement("li");
        attributeNode.innerText = attribute.SchemaName;
        attributeList.appendChild(attributeNode);
    }
    // Access the entity list item element
    var entityNode = document.getElementById(logicalName).parentElement;

    entityNode.appendChild(attributeList);
    entityNode.attributesDisplayed = true;
    // Attach event handler to toggle display of attributes.
    entityNode.firstChild.onclick = toggleDisplayAttributes;
    entityNode.firstChild.title = "Click to hide attributes.";

}
function errorRetrieveEntity(error) {
    ///<summary>
    /// Displays the error returned from SDK.MetaData.RetrieveEntityAsync if it fails.
    ///</summary>
    message.innerText = error.message;
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

function GetMultipleAttributeValuesFromID(sEntityName, sGUID, sAttributeName, sID) {
    var xml = "" +
	"<?xml version="1.0" encoding="utf-8"?>" +
	"<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">" +
	Xrm.Page.context.getAuthenticationHeader() +
	"  <soap:Body>" +
	"    <RetrieveMultiple xmlns="http://schemas.microsoft.com/crm/2007/WebServices">" +
	"      <query xmlns:q1="http://schemas.microsoft.com/crm/2006/Query" xsi:type="q1:QueryExpression">" +
	"        <q1:EntityName>" + sEntityName + "</q1:EntityName>" +
	"        <q1:ColumnSet xsi:type="q1:ColumnSet">" +
	"          <q1:Attributes>";
    for (i = 0; i < sAttributeName.length - 1; i++){
        xml += "            <q1:Attribute>" + sAttributeName[i] + "</q1:Attribute>";
    }
    xml +=
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
    var results = new Array();
    for (i = 0; i < sAttributeName.length-1; i++){
        results.push(xmlHttpRequest.responseXML.selectSingleNode("//q1:" + sAttributeName));
    }
    return results;
}
//function that splits and chages format of "Last Name, First Name" to "First Name Last Name"
//var FullName = Xrm.Page.getAttribute('createdby').getValue()[0].name;
function fullnameFormat(FullName) {
    var fullNameArray = FullName.split(',');
    var FormattedFullName = '';
    if (fullNameArray.length > 0) {
        FormattedFullName = fullNameArray[1] + ' ' + fullNameArray[0];
    }
    return FormattedFullName;
}