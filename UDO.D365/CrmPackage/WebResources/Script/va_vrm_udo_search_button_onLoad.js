var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Code = Va.Udo.Crm.Scripts.Code || {};


Va.Udo.Crm.Scripts.Code.SearchBtn = {
    onLoad: function () {
        Va.Udo.Crm.Scripts.Code.SearchBtn.addButton("va_ssn", "Search", Va.Udo.Crm.Scripts.Code.SearchBtn.onNewButtonClick);
    },
    addButton: function (attributename, buttonText, onClickAction) {
        if (document.getElementById(attributename) != null) {
            var sFieldID = "field_" + attributename;
            var elementID = document.getElementById(attributename + "_d");
            var div = document.createElement("div");
            div.style.width = "20%";
            div.style.textAlign = "center";
            div.style.display = "inline";
            elementID.appendChild(div, elementID);
            div.innerHTML = '<button id="' + sFieldID + '"  type="button" style="margin-left: 4px; width: 20%;" >' + buttonText + '</button>';
            document.getElementById(sFieldID).onclick = onClickAction;
        }
    },
    onNewButtonClick: function () {
        var parameters = {};
        try {
            //Set the the basic fields and the auto load flag
            parameters["va_ssn"] = Xrm.Page.getAttribute("va_ssn").getValue();
            parameters["va_autoload_vip"] = "true";
            parameters["va_searchtype"] = Xrm.Page.getAttribute("va_searchtype").getValue();
            parameters["va_calleridentityverified"] = Xrm.Page.getAttribute("va_calleridentityverified").getValue();
            parameters["va_filessnverified"] = Xrm.Page.getAttribute("va_filessnverified").getValue();
            parameters["va_bosverified"] = Xrm.Page.getAttribute("va_bosverified").getValue();
            parameters["va_eccphonecall"] = Xrm.Page.getAttribute("va_eccphonecall").getValue();
            parameters["va_searchcorpall"] = Xrm.Page.getAttribute("va_searchcorpall").getValue();
            parameters["va_edipi"] = Xrm.Page.getAttribute("va_edipi").getValue();
            parameters["va_searchmonthofdeath"] = Xrm.Page.getAttribute("va_searchmonthofdeath").getValue();
            parameters["va_firstname"] = Xrm.Page.getAttribute("va_firstname").getValue();
            parameters["va_lastname"] = Xrm.Page.getAttribute("va_lastname").getValue();
            parameters["va_middleinitial"] = Xrm.Page.getAttribute("va_middleinitial").getValue();
            parameters["va_participantid"] = Xrm.Page.getAttribute("va_participantid").getValue();
            parameters["va_dobtext"] = Xrm.Page.getAttribute("va_dobtext").getValue();

            // Open the window - entity, entity id (null for new), prefilled values
            Xrm.Utility.openEntityForm("va_vipentity", null, parameters);
        } catch (err) {
            alert("One or more attributes is missing.");
        }
    }

}