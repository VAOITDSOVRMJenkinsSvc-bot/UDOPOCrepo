//Common Utilities should be first library loaded
var UTIL = {};
var lib;
var formContext;
var executionContext;
var webApi;
UTIL.Initialize = function(){
        lib = this.lib;
        webApi = lib.webApi;
        context = lib.FormHelper.getContext();
        formContext = context.formContext;
        executionContext = context.executionContext;
};
//Function to clean strings (Keeps Alpha, Numeric, Spaces)
//Use: UTIL.trimChars('va_address1', 'AlphaNum', 'Invalid Characters entered in the address.');
UTIL.trimChars = function(fieldName, pattern, message) {
    //Check if obj and value exist
    if (formContext.getAttribute(fieldName) && formContext.getAttribute(fieldName).getValue() != null) {

        //Determine the pattern
        switch (pattern) {
        /*case 'AlphaNum':
			varPattern = /[^\d\w\s]|(_)/gi;
			break;
			*/
        default:
//Defaults to AlphaNum
            var varPattern = /[^\d\w\s]|(_)/gi;
            break;
        }

        var origString = formContext.getAttribute(fieldName).getValue();
        var cleanString = origString.replace(varPattern, '');

        //Return value
        if (origString !== cleanString) {
            alert(message);
            return cleanString.trim();
        } else {
            return origString;
        }
    }
};

//Register Checkbox FireOnClick function
UTIL.registerCheckboxClick = function(attr) {
    var ctrl = formContext.getControl(attr);    
    var a = ctrl.getAttribute();
    //var el = document.getElementById(attr);
    var ef=function() {
        var b = formContext.getAttribute(attr);
        b.setValue(!b.getValue());
        b.fireOnChange();
        };
    a.addOnChange(ef);    
    /*var f = "var ef=function() { " +
        "var a = Xrm.Page.data.entity.attributes.get(attr); " +
        "a.setValue(!a.getValue()); a.fireOnChange();" +
        " };";
    eval(f);
    // Attach to click event
    el.attachEvent('onclick', ef, false);*/
};

//format Telephone Numbers 
//(need to remove other phone functions FormatPhone/formatTelephone, from files such as CommonFunctions, CommonFunctions_VIP, PhoneCall_OnLoad_VIP)
UTIL.formatTelephone = function(telephoneField) {
    var Phone = formContext.getAttribute(telephoneField).getValue();
    var ext = '';
    var result;
    if (Phone != null) {
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
            if (11 == Phone.length) {
                result = Phone.slice(0, 1) + ' (' + Phone.slice(1, 4) + ') ' + Phone.slice(4, 7) + '-' + Phone.slice(7);
            }
            if (0 < ext.length) {
                result = result + ' ' + ext;
            }
            formContext.getAttribute(telephoneField).setValue(result);
        }
    }
};

UTIL.isInteger = function(s) {
    var i;
    for (i = 0; i < s.length; i++) {
        // Check that current character is number.
        var c = s.charAt(i);
        if (((c < "0") || (c > "9"))) return false;
    }
    // All characters are numbers.
    return true;
};

UTIL.stripCharsInBag = function(s, bag) {
    var i;
    var returnString = "";
    // Search through string's characters one by one.
    // If character is not in bag, append to returnString.
    for (i = 0; i < s.length; i++) {
        var c = s.charAt(i);
        if (bag.indexOf(c) == -1) returnString += c;
    }
    return returnString;
};

UTIL.daysInFebruary = function(year) {
    // February has 29 days in any year evenly divisible by four,
    // EXCEPT for centurial years which are not also divisible by 400.
    return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
};

UTIL.DaysArray = function(n) {
    for (var i = 1; i <= n; i++) {
        this[i] = 31;
        if (i == 4 || i == 6 || i == 9 || i == 11) {
            this[i] = 30;
        }
        if (i == 2) {
            this[i] = 29;
        }
    }
    return this;
};

UTIL.isProperDate = function(fieldName) {
    var dtStr = formContext.getAttribute(fieldName).getValue();
    if ((dtStr == null) || (dtStr == '')) {
        return false;
    }

    var dtCh = "/";
    var daysInMonth = UTIL.DaysArray(12);
    var pos1 = dtStr.indexOf(dtCh);
    var pos2 = dtStr.indexOf(dtCh, pos1 + 1);
    var strMonth = dtStr.substring(0, pos1);
    var strDay = dtStr.substring(pos1 + 1, pos2);
    var strYear = dtStr.substring(pos2 + 1);
    strYr = strYear;
    if (strDay.charAt(0) == "0" && strDay.length > 1) {
        strDay = strDay.substring(1);
    }
    if (strMonth.charAt(0) == "0" && strMonth.length > 1) {
        strMonth = strMonth.substring(1);
    }
    for (var i = 1; i <= 3; i++) {
        if (strYr.charAt(0) == "0" && strYr.length > 1) {
            strYr = strYr.substring(1);
        }
    }
    month = parseInt(strMonth);
    day = parseInt(strDay);
    year = parseInt(strYr);
    if (pos1 == -1 || pos2 == -1) {
        alert("The date format should be : mm/dd/yyyy");
        return false;
    }
    if (strMonth.length < 1 || month < 1 || month > 12) {
        alert("Please enter a valid month");
        return false;
    }
    if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > UTIL.daysInFebruary(year)) || day > daysInMonth[month]) {
        alert("Please enter a valid day");
        return false;
    }
    if (strYear.length != 4 || year == 0) {
        alert("Please enter a valid 4 digit year");
        return false;
    }
    if (dtStr.indexOf(dtCh, pos2 + 1) != -1 || UTIL.isInteger(UTIL.stripCharsInBag(dtStr, dtCh)) == false) {
        alert("Please enter a valid date");
        return false;
    }
    return true;
};

// process substition tokens
// each one looks like <!va_ssn!>
// will take input abe<!va_ssn!>asdf and replace with abe555667777asdf
UTIL.ReplaceFieldTokens = function(qw) {
    if (!qw || qw.length == 0) return qw;

    var op = '<!', po = '!>', pos = qw.indexOf(op);
    while (pos != -1) {
        var pos2 = qw.indexOf(po);
        if (pos2 == -1) break; // cannot find closing tag
        var token = qw.substring(pos + po.length, pos2); // token without tags
        var attr = formContext.getAttribute(token);
        var attrVal = '';
        if (attr && attr.getValue()) {
            switch (attr.getAttributeType()) {
            case 'datetime':
                attrVal = attr.getValue().format("MM/dd/yyyy");
                break;
            case 'lookup':
                attrVal = attr.getValue()[0].name;
                break;
            case 'optionset':
                attrVal = attr.getText();
                break;
            default:
                attrVal = attr.getValue().toString();
            }
        }
        qw = qw.replace(new RegExp(op + token + po, 'g'), attrVal);

        pos = qw.indexOf(op);
    }
    return qw;
};

UTIL.GetLookupId = function(lookupAttributeName) {
    var id = null;
    var attr = formContext.getAttribute(lookupAttributeName);
    if (attr.getValue() && attr.getValue().length > 0 &&attr.getValue()[0]) {
        id = attr.getValue()[0].id;
        //data.name = Xrm.Page.getAttribute(lookupAttributeName).getValue()[0].name;
    }
    return id;
};

// Utility function to create or update the map-D notes
UTIL.mapdNote = function (opts) {

    var serviceName, noteExists,
        createNoteCtx, createNoteDetail, result, options,
        defaults = { nodeId: null, isContactNote: true, claimId: '', ptcpntId: null, noteText: '' };

    options = $.extend(defaults, opts);

    noteExists = Boolean(options.nodeId);
    serviceName = noteExists ? 'updateNote' : 'createNote';

    createNoteCtx = new vrmContext(executionContext);
    if (!_UserSettings)
        _UserSettings = GetUserSettingsForWebservice(executionContext);
    createNoteCtx.user = _UserSettings;
    createNoteCtx.isContactNote = options.isContactNote;

    createNoteCtx.parameters['claimId'] = options.claimId; // claimNo;
    createNoteCtx.parameters['ptcpntId'] = options.ptcpntId;
    createNoteCtx.parameters['noteText'] = options.noteText = options.noteText
                                                                .replace(new RegExp('<', 'g'), '&lt;')
                                                                .replace(new RegExp('>', 'g'), '&gt;')
                                                                .replace(new RegExp('&', 'g'), '&amp;')
                                                                .replace(new RegExp("'", 'g'), '&quot;')
                                                                .replace(new RegExp("’", 'g'), '&quot;');

    if (noteExists)
        createNoteCtx.parameters['noteId'] = options.nodeId;

    createNoteDetail = (noteExists ? new updateNote(createNoteCtx) : new createNote(createNoteCtx));
    createNoteDetail.serviceName = serviceName;
    createNoteDetail.wsMessage.serviceName = serviceName;

    result = createNoteDetail.executeRequest();

    return {
        success: result,
        service: serviceName,
        requestXml: createNoteDetail.wsMessage.xmlRequest,
        responseXml: createNoteDetail.responseXml,
        soapBodyXml: createNoteDetail.soapBodyInnerXml
    };
};

// Function accepts a data an the format pattern
// pattern examples: 'MM-dd-yyyy h:mm:ss' gives you a date formated like so: 12-24-2013 12:30:34
UTIL.dateFormat = function (date, format) {
    var o = {
        "M+": date.getMonth() + 1,  //month
        "d+": date.getDate(),       //day
        "h+": date.getHours(),      //hour
        "m+": date.getMinutes(),    //minute
        "s+": date.getSeconds(),    //second
        "q+": Math.floor((date.getMonth() + 3) / 3),  //quarter
        "S": date.getMilliseconds() //millisecond
    };

    if (/(y+)/.test(format))
        format = format.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
    
    for (var k in o)
        if (new RegExp("(" + k + ")").test(format))
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
    
    return format;
};

//Non-Sequential Dynamic Picklist
//Sample Array to pass:
//First value is Main Picklist Option, followed by corresponding SubValues
/*var typePicklist = new Array();
//typePicklist[0] = new Array(953850000, 953850000, 953850001, 953850002, 953850003, 953850004, 953850005);
//typePicklist[1] = new Array(953850001, 953850006, 953850005);
*/
UTIL.createDynamicPicklist = function(varPicklist, varSubPicklist, varRelationArray) {
    var oPicklist = formContext.getAttribute(varPicklist);
    var oSubControl = formContext.getControl(varSubPicklist);
    var oSavedSubList = formContext.getAttribute(varSubPicklist).getOptions();

    if (oSubControl.getAttribute().getOptions().length > 0) {
        oSubControl.clearOptions();
    }
    //Function to Filter SubPicklist

    function filterPicklist(oDesiredOptions) {
        //Scenario: Only 1 real option
        if (oDesiredOptions[2] == 0) {
            //Loop through oSavedSubList to find the Name:
            for (var i = 0; i < oSavedSubList.length; i++) {
                if (oSavedSubList[i].value == oDesiredOptions[1]) {
                    var option = oSavedSubList[i];
                    oSubControl.addOption(option);
                }
            }
            oSubControl.getAttribute().setValue(oDesiredOptions[1]);
        } else {
            //Scenario: Multiple Options
            for (var i = 1; i < oDesiredOptions.length; i++) {
                //Loop through oSavedSubList to find the Name:
                for (var j = 0; j < oSavedSubList.length; j++) {
                    if (oSavedSubList[j].value == oDesiredOptions[i]) {
                        var option = oSavedSubList[j];
                        oSubControl.addOption(option);
                    }
                }
            }          
        }
        oSubControl.setFocus();
    }

//Check that Main Picklist value is not Null
    if (oPicklist.getValue() != null) {
        for (i = 0; i < varRelationArray.length; i++) {
            if (oPicklist.getValue() == varRelationArray[i][0]) {
                filterPicklist(varRelationArray[i]);
            }

        }
    }
};

UTIL.openOutlookEmail = function (opts) {
    var outlookApp, mailItem, options,
        defaults = {
            subject: 'VAI',
            to: null,
            cc: null,
            isHtml: true,
            body: '',
            files: [],
            attachements: []
        };

    options = $.extend(defaults, opts);

    try {
        outlookApp = new ActiveXObject("Outlook.Application");

        mailItem = outlookApp.CreateItem(0);

        mailItem.Subject = options.subject;

        if (options.to) {
            mailItem.To = options.to;
        } else {
            throw new Error('The email requires a recipent!');
        }

        if (options.cc) {
            mailItem.CC = options.cc;
        }

        if (options.isHtml) {
            mailItem.HTMLBody = options.body;
        } else {
            mailItem.Body = options.body;
        }

        mailItem.display(0);
        mailItem.GetInspector.WindowState = 2;

        // filesreports
        if (options.files) {
            for (var j = 0; j < options.files.length; j++) {
                mailItem.Attachments.Add(options.files[j]);
            }
        }

        // add form attachments
        if (options.attachements) {
            for (var i = 0; i < options.attachements.length; i++) {
                mailItem.Attachments.Add(options.attachements[i]);
            }
        }

    } catch (ex) {
        alert('Failed to create Outlook message: ' + ex.description);
    } finally {
        outlookApp = null;
    }
};

//function that splits and chages format of "Last Name, First Name" to "First Name Last Name"
//var FullName = Xrm.Page.getAttribute('createdby').getValue()[0].name;
UTIL.fullnameFormat = function (FullName){
    var fullNameArray = FullName.split(',');
    var FormattedFullName = '';
    if (fullNameArray.length > 0) {
        FormattedFullName = fullNameArray[1] + ' ' + fullNameArray[0];
    }
    return FormattedFullName;
};

//Used to Create ServiceReqeest and VAI from VIP (for phone call and contact)
UTIL.CreateEntity = function (entityName, cols) {
    /**This is a fix for some browsers that fail to convert Dates in JSON stringify**/
    if (cols.va_DateOpened) {
        cols.va_DateOpened = new Date(cols.va_DateOpened);
    }
    if (cols.va_PayDate) {
        cols.va_PayDate = new Date(cols.va_PayDate);
    }
    if (cols.va_EffectiveDate) {
        cols.va_EffectiveDate = new Date(cols.va_EffectiveDate);
    }
    if (cols.va_FutureExamDate) {
        cols.va_FutureExamDate = new Date(cols.va_FutureExamDate);
    }
    if (cols.va_DepDateofBirth) {
        cols.va_DepDateofBirth = new Date(cols.va_DepDateofBirth);
    }
    if (cols.va_DateofDeath) {
        cols.va_DateofDeath = new Date(cols.va_DateofDeath);
    }
    if (cols.va_DateofBirth) {
        cols.va_DateofBirth = new Date(cols.va_DateofBirth);
    }
    /****************************************************************************/

    var entity = '';
    //CrmRestKit2011.Create(entityName, cols, false)
    webApi.Create(cols, entityName)
    .fail(function (error) {
        UTIL.restKitError(error, 'Failed to create record')
    })
     .done(function (data){
         entity= data;
    });
    return entity;
};
UTIL.restKitError = function (err, message) {
    if (err.responseText) {
        alert( message + '\r\n' + $.parseJSON(err.responseText).error.message.value);
    }
    else {
        alert(message);
    }
};