//Not used
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Notes = Va.Udo.Crm.Scripts.Notes || {};

Va.Udo.Crm.Scripts.Notes.UI = {
    urlParams: {},
    BaseUrl: null,
    UserSettings: null,
    TimeOffset: null,
    vetName: null,
    oldSelection: 0,
    //notesWindow : null,

    initiatlize: function () {
        debugger;
        this.urlParams = this.getUrlParams();
        if (this.urlParams == null)
            return;

        this.BaseUrl = Xrm.Utility.getGlobalContext().getClientUrl();
        //$('#imgLoading').attr('src', this.BaseUrl + "/WebResources/udo_loadingGif.gif");

        $('#relatedList').css('display', 'none');
        //$('#loadingMessage').css('display', 'inline');

        var iFrameGrid = document.getElementById("relatedList");
        //iFrameGrid.src = "udo_progress.html";

        var dropDown = document.getElementById("nameDropDown");
        this.buildDropDownList(dropDown);
        this.oldSelection = 0;
        dropDown.selectedIndex = 0;

        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        //var iFrameGrid = document.getElementById("relatedList");
        var relName = 'udo_udo_person_udo_note_personId';
        var typeCode = this.urlParams.udoPersonEtn;
        if (dropDown.length > 0) {
            iFrameGrid.src = this.buildViewUrl(this.getSelectedPerson(0).personid, relName, typeCode);
            $('#relatedList').ready(function () {
                $('#nameDropDown').prop('disabled', false);
            });
        } else {
            $("<p>No data found. If problem persists please alert an administrator.</p>").appendTo("#statusMessage");
        }
    },
    getUrlParams: function () {

        try {
            //var sPageUrl = location.search.substring(1);
            var sPageUrl = document.location.href;
            var regex = new RegExp("[\\?&]?data=([^&#]*)");
            sPageUrl = decodeURIComponent(regex.exec(sPageUrl)[1]);
            var params = sPageUrl.split('&');
            var UrlParams = {};

            for (var index = 0; index < params.length; index++) {
                param = params[index].split('=');
                UrlParams[param[0]] = decodeURIComponent(param[1].split("#")[0]);
            }

            this.urlParams = UrlParams;
            return UrlParams;
        } catch (err) {
            return null;
        }
    },
    buildViewUrl: function (idStr, relName, TypeCode) {
        var serverUrl = this.BaseUrl + "/userdefined/areas.aspx?oId=" + encodeURI(idStr) + "&oType=" + encodeURI(TypeCode) + "&pagemode=iframe&security=852023&tabSet=" + encodeURI(relName) + "&rof=true";
        // payeeCode.RelatedPaymentUrl="/"+orgName+"/userdefined/areas.aspx?oId="+code.udo_payeecodeId+"&oType="+etc+"&pagemode=iframe&security=852023&tabSet=udo_udo_payeecode_udo_payment_PayeeCodeId&rof=true"


        return serverUrl;
    },
    getSelectedPerson: function (index) {
        var dropDown = document.getElementById("nameDropDown");
        var strObj = JSON.stringify(dropDown[index].value);

        var startIndex = strObj.indexOf("personid");
        var endIndex = strObj.indexOf('"', startIndex);
        var personId = strObj.substr(index + 12, endIndex - 1);

        startIndex = strObj.indexOf("ptcpntid");
        endIndex = strObj.indexOf('"', startIndex);
        var ptcpntId = strObj.substr(index + 12, endIndex - 1);

        var obj = {};
        obj.personid = personId;
        obj.ptcpntid = ptcpntId;

        return obj;
    },
    getUserSettings: function (userid) {
        if (this.UserSettings !== null)
            return this.UserSettings;
        var filter = "SystemUserId eq guid'" + userid + "'";
        CrmRestKit2011.ByQueryAll('UserSettings', ['TimeFormatCode', 'TimeFormatString', 'TimeSeparator', 'TimeZoneCode'], filter, false)
            .done(function (data) {
                if (data.length == 0)
                    return null;
                Va.Udo.Crm.Scripts.Notes.UI.UserSettings = data[0];
            })
            .fail(function (err) {
                alert(err);
            })
        return Va.Udo.Crm.Scripts.Notes.UI.UserSettings;
    },
    getTimeZoneOffset: function () {
        if (this.TimeOffset !== null)
            return this.TimeOffset;
        var settings = this.getUserSettings(this.urlParams.uid);
        var codeToOffset = {
            0: -720, 1: -660, 2: -600, 3: -540, 4: -480, 10: -420, 13: -420, 15: -420, 20: -360, 25: -360,
            30: -360, 33: -360, 35: -300, 40: -300, 45: -300, 50: -240, 55: -240, 56: -240, 60: -210, 65: -180,
            70: -180, 73: -180, 75: -120, 80: -60, 83: -60, 85: 0, 90: 0, 95: 60, 100: 60, 105: 60, 110: 60,
            113: 60, 115: 120, 120: 120, 125: 120, 130: 120, 135: 120, 140: 120, 145: 180, 150: 180, 155: 180,
            158: 180, 160: 210, 165: 240, 170: 240, 175: 270, 180: 300, 185: 300, 190: 330, 193: 345, 195: 360,
            200: 360, 201: 360, 203: 390, 205: 420, 207: 420, 210: 480, 215: 480, 220: 480, 225: 480, 227: 480,
            230: 540, 235: 540, 240: 540, 245: 570, 250: 570, 255: 600, 260: 600, 265: 600, 270: 600, 275: 600,
            280: 660, 285: 720, 290: 720, 300: 780, 360: 0
        };

        var d = new Date();
        var localOffset = d.getTimezoneOffset();
        var crmOffset = codeToOffset[settings.TimeZoneCode];
        this.TimeOffset = (localOffset + crmOffset);
        return this.TimeOffset;
    },
    toCrmTime: function (d) {
        var crmDT = new Date(d.setMinutes(d.getMinutes() + this.getTimeZoneOffset()));
        //var dayOfWeekAbbr = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
        var dayOfWeekFull = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        var monthName = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
        var longDate = dayOfWeekFull[crmDT.getDay()] + ", " +
            monthName[crmDT.getMonth()] + " " + crmDT.getDate() + ", " + crmDT.getYear();
        var shortDate = crmDT.getMonth() + 1 + "/" + crmDT.getDate() + "/" + crmDT.getYear();
        var dt = shortDate + " - " + longDate;

        //get Time
        var tm = ":";
        if (crmDT.getMinutes() < 10) {
            tm = tm + "0";
        }
        tm = tm + crmDT.getMinutes();
        var hours = crmDT.getHours();
        if (hours >= 12) {
            tm = hours + tm + " PM";
        } else if (hours == 0) {
            tm = "12" + tm + " AM";
        } else {
            tm = hours + tm + " AM";
        }


        var crmDTstring = dt + " " + tm;
        return crmDTstring;
    },
    buildDropDownList: function (dropDown) {
        if (this.urlParams == null)
            this.urlParams = this.getUrlParams();
        if ((this.urlParams.relName == null) || (this.urlParams.id == null) || (this.urlParams.Type == null))
            return null; // nothing to do
        //debugger;
        //udo_IDProofId/Id
        var filter = this.urlParams.filterFieldName + "/Id eq guid'" + this.urlParams.id + "'";
        var optionString = null;
        ctx = this;
        CrmRestKit2011.ByQueryAll('udo_person', ['udo_name', 'udo_personId', 'udo_Type', 'udo_ptcpntid'], filter, false)
            .done(function (data) {
                var selected = false;
                if (data && data.length > 0) {
                    for (var index = 0; index < data.length; index++) {
                        var option = document.createElement("option");
                        switch (data[index].udo_Type.Value) {
                            case 752280000:
                                ctx.vetName = data[index].udo_name; //save the name for later  use
                                data[index].udo_name = "(Veteran) " + data[index].udo_name;
                                selected = true;
                                break;
                            case 752280001:
                                data[index].udo_name = "(Dependent) " + data[index].udo_name;
                                selected = false;
                                break;
                            case 752280002:
                                data[index].udo_name = "(Beneficiary) " + data[index].udo_name;
                                selected = false;
                                break;
                        }
                        option.text = data[index].udo_name;
                        option.value = '{ "personid": "{' + data[index].udo_personId + '}" , "ptcpntid": "' + data[index].udo_ptcpntid + '" }';
                        option.selected = selected;
                        dropDown.add(option);
                    }
                    //dropDown.onchange();
                }
            })
            .fail(function (err) { })
    },
    onPersonSelect: function () {
        //debugger;
        if (this.urlParams == null)
            return;

        var dropDown = document.getElementById("nameDropDown");

        // Prevents needless client-side calls to the server.
        if (this.oldSelection == dropDown.selectedIndex)
            return;
        this.oldSelection = dropDown.selectedIndex;

        $("#statusMessage").text("Retrieving notes");
        $('#statusMessage').focus();

        var urlName = Xrm.Utility.getGlobalContext().getClientUrl();
        //$('#imgLoading').attr('src', urlName + "/WebResources/udo_loadingGif.gif");

        $('#relatedList').css('display', 'none');
        //$('#loadingMessage').css('display', 'inline');
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        var iFrameGrid = document.getElementById("relatedList");
        var relName = 'udo_udo_person_udo_note_personId';
        var typeCode = this.urlParams.udoPersonEtn;

        if (dropDown.length > 0) {
            iFrameGrid.src = Va.Udo.Crm.Scripts.Notes.UI.buildViewUrl(this.getSelectedPerson(dropDown.selectedIndex).personid, relName, typeCode);
        }
    },
    getAllRecords: function (idStr) {
        //var dropDown = document.getElementById("nameDropDown");
        //idStr = dropDown[dropDown.selectedIndex].value;
        //debugger;
        var filter = "udo_personId/Id eq guid'" + idStr + "'";
        CrmRestKit2011.ByQueryAllOrdered('udo_note', ['udo_NoteText', 'udo_name', 'udo_dtTime'], filter, 'udo_dtTime desc', false)
            .done(function (data) {
                var notesStr = "";
                if (data && data.length > 0) {
                    for (var index = 0; index < data.length; index++) {
                        var note = {
                            CreatedOn: "Create Date Not Set",
                            Text: data[index].udo_NoteText
                        };
                        if (data[index].udo_dtTime !== null) {
                            var someString = (data[index].udo_dtTime).substring(6, data[index].udo_dtTime.length - 2);
                            var d = new Date(parseInt(someString));
                            note.CreatedOn = Va.Udo.Crm.Scripts.Notes.UI.toCrmTime(d);
                        }
                        notesStr = '<p class="date">' + note.CreatedOn + '</p><p class="note">' + note.Text + '</P><HR />';
                        $(notesStr).appendTo("#SummaryDiv");
                    }
                    //Va.Udo.Crm.Scripts.Notes.UI.displayAllNotes(notesStr);

                } else {
                    notesStr = '<p class = "note">There are no notes on file for this person</p>';
                    $(notesStr).appendTo("#SummaryDiv");
                }
            })
            .fail(function (err) { })
    },
    displayAllNotes: function (notesStr) {
        var dropDown = document.getElementById("nameDropDown");
        if (dropDown == null || dropDown.length == null || dropDown.length < 1) {
            alert("No data found. If problem persists please alert an administrator");
            return;
        }
        var data = "id=" + this.getSelectedPerson(dropDown.selectedIndex).personid +
            "&uid=" + Va.Udo.Crm.Scripts.Notes.UI.urlParams.uid;

        var NoteUrl = Xrm.Utility.getGlobalContext().getClientUrl(); + "/WebResources/udo_notes_summary.html?data=" + encodeURIComponent(data);
        var notesWindow = window.open(NoteUrl, "NotesWindow", "resizeable=1,scrollbars=1,width=500,height=400");
        notesWindow.focus();
    },
    createNewNote: function () {
        var dropDown = document.getElementById("nameDropDown");
        if (dropDown == null || dropDown.length == null || dropDown.length < 1) {
            alert("No data found. If problem persists please alert an administrator");
            return;
        }
        var parameters = {};
        var nameStr = dropDown[dropDown.selectedIndex].text;

        //remove the type that we prefix on for the user
        //in the form of "(type) Name"
        var spot = nameStr.indexOf(") ");
        if (spot != -1)
            nameStr = nameStr.substring(spot + 2);
        parameters["udo_personid"] = this.getSelectedPerson(dropDown.selectedIndex).personid;
        parameters["udo_personidname"] = nameStr;
        parameters["udo_veteranid"] = "{" + Va.Udo.Crm.Scripts.Notes.UI.urlParams.vetId + "}";
        parameters["udo_veteranidname"] = Va.Udo.Crm.Scripts.Notes.UI.vetName;
        parameters["udo_participantid"] = this.getSelectedPerson(dropDown.selectedIndex).ptcpntid;
        parameters["udo_fromudo"] = "1";

        var urlparams = "";
        for (var key in parameters) {
            if (parameters.hasOwnProperty(key)) {
                urlparams = urlparams + "&" + key + "=" + parameters[key];
            }
        }
        //parameters["cmdbar"] = "true";
        //parameters["navbar"] = "off";
        window.open("http://event/?eventName=NewNote" + urlparams);
        //Xrm.Utility.openEntityForm("udo_note", null, parameters);
    },
    editNote: function (selectedItemIds) {
        if (selectedItemIds.length !== 1)
            return;
        var recordId = selectedItemIds[0];
        alert(recordId);
    }
};
