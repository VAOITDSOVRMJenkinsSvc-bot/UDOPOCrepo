<html><head><meta charset="utf-8"></head>
<body>
    ﻿
    <title>People List</title>
    <style type="text/css">
        fieldset {
            color: rgb(0,0,0);
            direction: ltr;
            font-family: "Segoe UI","Segoe UI Web Regular","Segoe UI Symbol","Helvetica Neue","BBAlpha Sans","S60 Sans",Arial,sans-serif;
            font-size: 16px;
            line-height: 20px;
            font-weight: 100;
            margin-bottom: 14px;
            margin-left: 0;
            margin-right: 0;
            margin-top: 0;
            background-color: rgb(255,255,255);
            padding: 2px;
            border: solid 0 #ffffff;
            width: 100%;
        }

        table {
            border-collapse: collapse;
            font-family: "Segoe UI","Segoe UI Web Regular","Segoe UI Symbol","Helvetica Neue","BBAlpha Sans","S60 Sans",Arial,sans-serif;
            width: 705px;
        }

        th {
            background-color: #ffffff;
            color: black;
            font-family: "Segoe UI Light","Segoe UI Web Regular","Segoe UI Symbol","Helvetica Neue","BBAlpha Sans","S60 Sans",Arial,sans-serif;
            font-size: 16px;
            line-height: 20px;
            font-weight: 600;
            white-space: nowrap;
            text-align: left;
        }

        td {
            padding: 5px;
            white-space: nowrap;
        }

        .odd {
            background-color: white;
            cursor: pointer;
        }

        .even {
            background-color: whitesmoke;
            cursor: pointer;
        }

        .odd:hover {
            background-color: #99ccff;
            color: #800000;
        }

        .even:hover {
            background-color: #99ccff;
            color: #800000;
        }


        div#tmpDialog {
            display: none;
            position: absolute;
            top: 50%;
            left: 50%;
            width: 250px;
            height: 50px;
            margin: -101px 0 0 -251px;
            background: rgb(255, 255, 255);
            border: 1px solid black;
        }

            div#tmpDialog img {
                float: left;
                height: 45px;
                width: 45px;
                padding: 5px 10px 5px 5px;
                text-align: left;
            }

            div#tmpDialog p {
                padding: 5px;
                margin: 15px 5px 5px 5px;
                text-align: left;
            }

        input {
            background-color: rgba(255,255,255,0);
            border-color: rgb(186,186,186);
            border-width: 1px;
            border-style: solid;
            color: rgb(0,0,0);
            font-family: "Segoe UI","Segoe UI Web Regular","Segoe UI Symbol","Helvetica Neue","BBAlpha Sans","S60 Sans",Arial,sans-serif;
            font-size: 14.06px;
            height: 30px;
            line-height: normal;
        }

        div {
            color: rgb(0,0,0);
            direction: ltr;
            /*float: left;*/
            font-family: "Segoe UI","Segoe UI Web Regular","Segoe UI Symbol","Helvetica Neue","BBAlpha Sans","S60 Sans",Arial,sans-serif;
            font-size: 12.06px;
            line-height: 17px;
            padding-bottom: 4px;
            padding-top: 0;
            text-align: left;
        }

        button {
            background-color: rgb(0,32,80);
            border-color: rgb(255,255,255);
            border-style: none;
            border-width: 0px;
            color: rgb(255,255,255);
            direction: ltr;
            font-family: "Segoe UI Semibold","Segoe UI Web Semibold","Segoe UI Web Regular","Segoe UI","Segoe UI Symbol","HelveticaNeue-Medium","Helvetica Neue",Arial,sans-serif;
            font-size: 14.06px;
            height: 30px;
            line-height: 20px;
            min-width: 84.4px;
            padding-bottom: 10px;
            padding-left: 12px;
            padding-right: 12px;
            padding-top: 5px;
            margin-right: 6px;
        }

        h1 {
            font-family: 'Segoe UI',Tahoma,Arial;
            font-weight: 100;
            color: #262626;
            font-size: 36px;
            line-height: 20px;
            margin-bottom: 12px;
        }

        h3 {
            font-family: 'Segoe UI',Tahoma,Arial;
            font-weight: 100;
            color: #262626;
            font-size: 22px;
            line-height: 10px;
            margin-bottom: 12px;
            margin-top: 8px;
        }

        h2 {
            color: rgb(0,0,0);
            direction: ltr;
            font-family: "Segoe UI","Segoe UI Web Regular","Segoe UI Symbol","Helvetica Neue","BBAlpha Sans","S60 Sans",Arial,sans-serif;
            font-size: 14.06px;
            line-height: 20px;
            font-weight: 100;
            margin-bottom: 14px;
            margin-left: 0px;
            margin-right: 0px;
            margin-top: 0px;
        }

        fieldset {
            border: 0;
        }

        span {
            color: red;
            font-family: "Segoe UI","Segoe UI Web Regular","Segoe UI Symbol","Helvetica Neue","BBAlpha Sans","S60 Sans",Arial,sans-serif;
            font-size: 14.06px;
            font-weight: 100;
            display: none;
        }

        .auto-style1 {
            width: 142px;
        }
    </style>

    <script src="udo_Xrm.min.js" type="text/javascript"></script>

    <script src="crme_jquery1.4.1.min.js" type="text/javascript"></script>

    <script src="udo_jquery_1.9.1.min.js" type="text/javascript"></script>

    <script src="crme_SDK.REST.js" type="text/javascript"></script>

    <script src="crme_json2.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            var veteran;
            var parameters = {};
            var veteranId;
            var idProofId;
            var relationshipStr;

            function retrievepeopleList() {
                formatExecutingSearch();
                getDataParam();
                //debugger;
                var filter = "$select=*&$filter=";
                veteranId = getParamValue("vetId");
                //relationshipStr = getParamValue("relStr");

                idProofId = getParamValue("idProofId");
                if (idProofId != "") {

                    filter += "udo_IDProofId/Id eq (guid'" + idProofId + "')";
                    //filter += " and udo_payeeCode ne null";
                    //prompt("Person Filter", filter);

                    //filter = "$select=*&$filter=udo_IDProofId/Id eq (guid'208FDB06-EF44-E511-944D-00155D049EFA')" //Advanced find middlename SSRecord
                    //alert('using id proof with middle name SSRecord');
                }

                SDK.REST.retrieveMultipleRecords("udo_person", filter, peopleListCallBack, function (error) { alert(error.message); }, peopleListSearchComplete);

                //SDK.REST.retrieveRecord(veteranId, "Contact", null, null, function (contact) { veteran = contact }, function (error) { alert(error.message); });
            }

            retrievepeopleList();

            function peopleListSearchComplete() {
                $('div#tmpDialog').hide();
            }

            function getDataParam() {
                //Get the any query string parameters and load them
                //into the vals array

                var vals = new Array();
                if (location.search != "") {
                    vals = location.search.substr(1).split("&");
                    for (var i in vals) {
                        vals[i] = vals[i].replace(/\+/g, " ").split("=");
                    }
                    //look for the parameter named 'data'
                    var found = false;
                    for (var i in vals) {
                        if (vals[i][0].toLowerCase() == "data") {
                            parseDataValue(vals[i][1]);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    { noParams(); }
                }
                else {
                    noParams();
                }
            }

            function parseDataValue(datavalue) {
                if (datavalue != "") {
                    vals = new Array();

                    //var message = document.createElement("p");
                    //setText(message, "These are the data parameters values that were passed to this page:");
                    //document.body.appendChild(message);

                    vals = decodeURIComponent(datavalue).split("&");
                    for (var i in vals) {
                        vals[i] = vals[i].replace(/\+/g, " ").split("=");
                    }
                }
            }

            function getParamValue(key) {
                if (vals != null) {
                    for (var i in vals) {
                        if (vals[i][0] == key)
                            return vals[i][1];
                    }
                }
                return null;
            }

            function buildQueryFilter(field, value, and) {
                if (and) {
                    return " and " + field + " eq '" + value + "'";
                } else {
                    return field + " eq '" + value + "'";
                }
            }

            function createAndOpenITF(obj) {
                debugger;
                vetSnapShotId = getParamValue("snapshot");
                retrieveVeteranSnapShot(vetSnapShotId, obj);
            }


            function retrieveVeteranSnapShot(VetSnapShotId, obj) {
                SDK.REST.retrieveRecord(
                    VetSnapShotId,
                    "udo_veteransnapshot",
                    null, null,
                    function (vetsnapshot) {

                        if (vetsnapshot.udo_integrationstatus == "Success") {
                            $('div#tmpDialog').show();
                            var personId = obj.getAttribute('personid');
                            if (personId == '') {
                                alert('Veteran or Dependent has not been initiated');
                                return false;
                            }
                            var filter = "$select=*&$filter=";
                            filter += buildQueryFilter("crme_udopersonguid", personId, false);
                            filter += buildQueryFilter("crme_SearchType", 'LETTERS', true);

                            SDK.REST.retrieveMultipleRecords("crme_person", filter, selectedPersonCallBack, function (error) { alert(error.message); }, personSearchComplete);
                        }
                        else {

                            alert("Veteran Snapshot is not completely loaded.  Please try again when the data is loaded");
                        }

                    },
                    errorHandler
                  );
            }


            function errorHandler(error) {
                alert(error.message);
            }



            function peopleListCallBack(data, textStatus, XmlHttpRequest) {
                $('div#tmpDialog').show();
                // get the table
                var table = $("#personSearchResultsTable");

                // reset the table by removing all data rows
                $("#personSearchResultsTable").find("thead, tr, th").remove();
                $("#personSearchResultsTable").find("tr:gt(0)").remove();
                $("#resultsFieldSetDiv").hide();

                if (data != null && data.length > 0) {

                    var thead = document.createElement('thead');
                    var theadRow = document.createElement('tr');

                    for (var i = 0; i < data.length; i++) {

                        var personid = data[i].udo_personId == null ? "" : data[i].udo_personId;
                        var name = data[i].udo_name == null ? "" : data[i].udo_name;
                        //var DOB = data[i].udo_DOB == null ? "" : data[i].udo_DOB.toLocaleDateString();
                        var DOB = data[i].udo_dobstr == null ? "" : data[i].udo_dobstr;

                        var SSN = data[i].udo_SSN == null ? "" : data[i].udo_SSN;
                        var participantId = data[i].udo_ptcpntid == null ? "" : data[i].udo_ptcpntid;
                        var payeecode = data[i].udo_payeeCode == null ? "" : data[i].udo_payeeCode;
                        //var Type = data[i].udo_type == null ? "" : data[i].udo_type;
                        var Type = data[i].udo_awardtypecode == null ? "" : data[i].udo_awardtypecode;
                        var fidExists = data[i].udo_fidexists == null ? false : data[i].udo_fidexists;
                        var awardExist = data[i].udo_awardsexist == null ? false : data[i].udo_awardsexist;
                        var pendingClaimsExists = data[i].udo_pendingclaimsexist == null ? false : data[i].udo_pendingclaimsexist;


                        if (i == 0) {
                            //var th1 = document.createElement('th');
                            var th1 = document.createElement('th');
                            var th2 = document.createElement('th');
                            var th3 = document.createElement('th');
                            var th4 = document.createElement('th');
                            var th5 = document.createElement('th');
                            var th6 = document.createElement('th');

                            th1.appendChild(document.createTextNode('Name'));
                            th1.id = 'name';
                            th1.title = 'Name';
                            th1.scope = 'col';
                            th2.appendChild(document.createTextNode('DOB'));
                            th2.id = 'dob';
                            th2.title = 'DOB';
                            th2.scope = 'col';
                            th3.appendChild(document.createTextNode('SSN'));
                            th3.id = 'ssn';
                            th3.title = 'SSN';
                            th3.scope = 'col';
                            th4.appendChild(document.createTextNode('Particpant Id'));
                            th4.id = 'participantId'
                            th4.title = 'Participant Id';
                            th4.scope = 'col';
                            th5.appendChild(document.createTextNode('Payee Code'));
                            th5.id = 'payeeCode';
                            th5.title = 'Payee Code';
                            th5.scope = 'col';
                            th6.appendChild(document.createTextNode('Award Type'));
                            th6.id = 'awardType';
                            th6.title = 'Award Type';
                            th6.scope = 'col';

                            theadRow.appendChild(th1);
                            theadRow.appendChild(th2);
                            theadRow.appendChild(th3);
                            theadRow.appendChild(th4);
                            theadRow.appendChild(th5);
                            theadRow.appendChild(th6);

                            thead.appendChild(theadRow);
                        }

                        // Table rows
                        var row = document.createElement('tr');

                        var col1 = document.createElement('td');
                        var col2 = document.createElement('td');
                        var col3 = document.createElement('td');
                        var col4 = document.createElement('td');
                        var col5 = document.createElement('td');
                        var col6 = document.createElement('td');

                        col1.appendChild(document.createTextNode(name));
                        col1.scope = 'row'
                        col2.appendChild(document.createTextNode(DOB));
                        col3.appendChild(document.createTextNode(SSN));
                        col4.appendChild(document.createTextNode(participantId));
                        col5.appendChild(document.createTextNode(payeecode));
                        col6.appendChild(document.createTextNode(Type));

                        row.appendChild(col1);
                        row.appendChild(col2);
                        row.appendChild(col3);
                        row.appendChild(col4);
                        row.appendChild(col5);
                        row.appendChild(col6);

                        row.setAttribute('personid', personid);
                        row.setAttribute('fidExists', fidExists);
                        row.setAttribute('awardExist', awardExist);
                        row.setAttribute('pendingClaimsExist', pendingClaimsExists);

                        row.tabIndex = 100 + i;
                        row.className = (i % 2 == 0) ? "even" : "odd";
                        row.ondblclick = function () { createAndOpenITF(this); };

                        row.onkeypress = function (e) {
                            if (e.keyCode === 13 || e.keyCode === 32) {
                                createAndOpenITF(this);
                            }
                        }
                        row.title = "Name - " + name + ", DOB - " + DOB + ", SSN - " + SSN + ", Participant Id - " + participantId + ", Payee Code - " + payeecode + ", Award Type - " + Type


                        table.append(thead);
                        table.append(row);

                        $("#resultsFieldSetDiv").show();
                        if (i == 1)
                            row.focus = true;
                    }
                }
                else {
                    $("#notFoundDiv").show();
                }
                $("#searchResultsMessageDiv").show();
                $("#searchResultsMessageDiv").text((data != null && data.length > 0) ? data.length + " records found." : "No valid people found for CADD.");

            }

            function selectedPersonCallBack(data) {
                $('div#tmpDialog').hide(100);
                //var cadd_formid = "&extraqs=formid%3D55B7B6EE-E430-4C7B-944B-8DA67B65284C%0D";

                if (data != null && data.length > 0) {
                    var url = data[0].crme_url;
                    if (url != null && url.length > 0) {
                        //prompt("url", url + cadd_formid);
                        //alert("URL - " + url);
                        window.open(url);
                    }
                    else {
                        var returnMessage = data[0].crme_ReturnMessage;
                        alert("No URL returned - " + returnMessage);
                    }

                }
                else {
                    alert("No Data Returned.");
                }
            }

            //function getUrl(etn,urlParameters)
            //{
            //    var url="";
            //    for(var key in urlParameters)
            //    {
            //        if(parameters.hasOwnProperty(key))
            //        {
            //            url=url+key+"="+parameters[key]+"&";
            //        }
            //    }

            //    url=encodeURIComponent(url);
            //    url = "../main.aspx?etn=" + etn + "&pagetype=entityrecord&extraqs=" + url;
            //    return url;
            //}

            function personSearchComplete() {
                $('div#tmpDialog').hide();
            }

            function formatExecutingSearch() {
                $('div#tmpDialog').show();
                $("#resultsFieldSetDiv").hide();
                $("#notFoundDiv").hide();
            }

            function setITFParameters(xmlData) {
                //alert('In set ITF params');


                parameters["udo_idproofid"] = idProofId;
                //parameters["udo_idproofidname"] = '[[Id Proof.udo_title]+]';
                //parameters["udo_idproofidtype"] = '[[Id Proof.LogicalName]+]';

                parameters["udo_veteranid"] = veteranId;
                //parameters["udo_veteranidname"] = '[[Veteran.fullname]+]';
                //parameters["udo_veteranidtype"] = '[[Veteran.LogicalName]+]';

                var relStr = relationshipStr;
                if (relStr != "") {
                    var relArr = relStr.split(',');
                    relationship = relArr[1];
                }


                //alert(parameters["udo_veteranid"]);

                var xmlDoc = $.parseXML(xmlData);
                var $xml = $(xmlDoc);
                //parameters["va_phonecallid"] = $xml.find('va_phonecallid').text();
                //parameters["va_phonecallidname"] = $xml.find('va_phonecallidname').text();
                parameters["va_participantid"] = $xml.find('va_participantid').text();


                //if (($xml.find('va_claimantparticipantid').text() != "") && (relationship == "Veteran(Self)" || relationship == "VSO/POA/AA" || relationship == "Fiduciary" || relationship || "Other")) {
                //    parameters["va_claimantparticipantid"] = $xml.find('va_participantid').text();
                //}
                //else {
                parameters["va_claimantparticipantid"] = $xml.find('va_claimantparticipantid').text();
                //}
                if ($xml.find('va_veteranfirstname').text() != "") { parameters["va_veteranfirstname"] = $xml.find('va_veteranfirstname').text(); }

                //if (($xml.find('va_veteranfirstname').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantfirstname"] = $xml.find('va_veteranfirstname').text(); }
                //else {
                parameters["va_claimantfirstname"] = $xml.find('va_claimantfirstname').text();
                //}
                if ($xml.find('va_veteranlastname').text() != "") { parameters["va_veteranlastname"] = $xml.find('va_veteranlastname').text(); }
                //if (($xml.find('va_veteranlastname').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantlastname"] = $xml.find('va_veteranlastname').text(); }
                //else {
                parameters["va_claimantlastname"] = $xml.find('va_claimantlastname').text();
                //}
                if ($xml.find('va_veteranmiddleinitial').text() != "") { parameters["va_veteranmiddleinitial"] = $xml.find('va_veteranmiddleinitial').text(); }
                //if (($xml.find('va_veteranmiddleinitial').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantmiddleinitial"] = $xml.find('va_veteranmiddleinitial').text(); }
                //else {
                parameters["va_claimantmiddleinitial"] = $xml.find('va_claimantmiddleinitial').text();
                //}
                if ($xml.find('va_veteranssn').text() != "") { parameters["va_veteranssn"] = $xml.find('va_veteranssn').text(); }
                //if (($xml.find('va_veteranssn').text() != "") && (relationship == "Veteran(Self)")) { parameters["va_claimantssn"] = $xml.find('va_veteranssn').text(); }
                //else {
                parameters["va_claimantssn"] = $xml.find('va_claimantssn').text();
                //}
                if ($xml.find('va_filenumber').text() != "") { parameters["va_veteranfilenumber"] = $xml.find('va_filenumber').text(); }
                if ($xml.find('va_veteranphone').text() != "") { parameters["va_veteranphone"] = $xml.find('va_veteranphone').text(); }
                if ($xml.find('va_veterandateofbirth').text() != "") { parameters["va_veterandateofbirth"] = $xml.find('va_veterandateofbirth').text(); }
                if ($xml.find('va_veteranaddressline1').text() != "") { parameters["va_veteranaddressline1"] = $xml.find('va_veteranaddressline1').text(); }
                if ($xml.find('va_veteranaddressline2').text() != "") { parameters["va_veteranaddressline2"] = $xml.find('va_veteranaddressline2').text(); }
                if ($xml.find('va_veteranunitnumber').text() != "") { parameters["va_veteranunitnumber"] = $xml.find('va_veteranunitnumber').text(); }
                if ($xml.find('va_veterancity').text() != "") { parameters["va_veterancity"] = $xml.find('va_veterancity').text(); }
                if ($xml.find('va_veteranstate').text() != "") { parameters["va_veteranstate"] = $xml.find('va_veteranstate').text(); }
                if ($xml.find('va_veteranzip').text() != "") { parameters["va_veteranzip"] = $xml.find('va_veteranzip').text(); }
                if ($xml.find('va_veterancountry').text() != "") { parameters["va_veterancountry"] = $xml.find('va_veterancountry').text(); }
                if ($xml.find('va_veteranemail').text() != "") { parameters["va_veteranemail"] = $xml.find('va_veteranemail').text(); }
                if ($xml.find('va_militarypostalcodevalue').text() != "") { parameters["va_militarypostalcodevalue"] = $xml.find('va_militarypostalcodevalue').text(); }
                if ($xml.find('va_militarypostofficetypecodevalue').text() != "") { parameters["va_militarypostofficetypecodevalue"] = $xml.find('va_militarypostofficetypecodevalue').text(); }
                if ($xml.find('va_gender').text() != "") {
                    if ($xml.find('va_gender').text() == "Male") { parameters["va_veterangender"] = "0"; }
                    else parameters["va_veterangender"] = "1";

                    // parameters["va_veterangender"] = $xml.find('va_gender').text(); }             
                }
            }

            function personSearchComplete() {

            }


            function buildQueryFilter(field, value, and) {
                if (and) {
                    return " and " + field + " eq '" + value + "'";
                } else {
                    return field + " eq '" + value + "'";
                }
            }



        });


    </script>
    <h1 style="padding-top: 12px;">People List</h1>
    <hr>

    <!--<div id="resultsFieldSetDiv" style="width: 100%; display: none;">
        <fieldset id="SearchResultFieldSet">
            <legend>People Results</legend>
            <table id="personSearchResultsTable" style="width: 100%;"></table>
        </fieldset>
    </div>-->

    <div id="resultsFieldSetDiv" style="width: 100%; float: left; display: none;">
        <div style="clear: both;"></div>
        <div id="searchResultsMessageDiv" style="color: rgb(128, 0, 0); font-size: 1em; font-weight: bold; display: none;" aria-label="Search Results Message"></div>
        <fieldset id="SearchResultFieldSet">
            <legend style="display: none;">Search Results</legend>
            <table id="personSearchResultsTable" style="width: 100%;">
            </table>
        </fieldset>
    </div>

    <div id="notFoundDiv" style="width: 100%; float: left; display: none;">
        <fieldset id="notFoundFieldset">
            <legend>No Records Found</legend>
            <hr style="border: 1px solid gray;">
            <table>
                <tbody>
                    <tr>
                        <td>
                            <h3 style="padding-bottom: 5px;">People Not Found.</h3>
                        </td>
                    </tr>
                </tbody>
            </table>
        </fieldset>
    </div>

    <div id="tmpDialog">
        <img tabindex="0" alt="People List loading. Please wait." src="udo_/images/search/loading.gif">
        <p aria-label="Please Wait">Working on it. Please wait...</p>
    </div>




</body></html>