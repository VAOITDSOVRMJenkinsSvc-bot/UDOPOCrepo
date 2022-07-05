
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.SelectRequest = Va.Udo.Crm.Scripts.SelectRequest || {};

//Request Data Model
Va.Udo.Crm.Scripts.SelectRequest.Data = {};


// Request Controller - Contains the business logic
Va.Udo.Crm.Scripts.SelectRequest.Controller = {

    // Raise this event when the request types data is retrieved
    RequestDataFetchComplete: null,

    //Raise this evnet when the request sub types data is retrieved
    RequestSubTypeDataFetchComplete: null,

    // Selected request type
    SelectedRequestType: null,
    SelectedRequestTypeName: null,

    //Select request sub type
    SelectedRequestSubType: null,
    SelectedRequestSubTypeName: null,

    //Raise this event when the request is created.
    UpdateRequestCreationStatus: null,

    //True if the system is waiting for the request entity creation to complete.
    RequestCreationInProgress: false,

    //Set the SelectedRequestType. This method initiates the request to pull related sub types.
    setSelectedRequestType: function (requestTypeId, requesttypename) {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType = requestTypeId;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestTypeName = requesttypename;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType = null;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
        Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesAsync();
    },

    //Set the selected request sub type and refresh the enabled status of the create request button.
    setSelectedRequestSubType: function (requestSubTypeId, requestsubtypename) {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType = requestSubTypeId;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubTypeName = requestsubtypename;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
    },

    // Refreshes the status of the create request button
    refreshRequestCreationStatus: function () {
        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.UpdateRequestCreationStatus != null) {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.UpdateRequestCreationStatus(
                Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType != null &&
                Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType != null &&
                !Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress);
        }
    },

    //Register all events here.
    registerNotification: function (requestDataFetchComplete, requestSubTypeDataFetchComplete, requestCreationStatus) {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestDataFetchComplete = requestDataFetchComplete;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestSubTypeDataFetchComplete = requestSubTypeDataFetchComplete;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.UpdateRequestCreationStatus = requestCreationStatus;
    },

    //Async fetch of request types
    loadRequestTypesAsync: function () {

        var urlName = Xrm.Page.context.getClientUrl();
        //$('#imgLoadingRequestType').attr('src', urlName + "/WebResources/udo_loadingGif.gif");

        $('#requestTypeSelect').css('display', 'none');
        //$('#loadingMessageRequestType').css('display', 'inline');
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        SDK.JQuery.retrieveMultipleRecords("udo_requesttype", "?$orderby=udo_Order&$select=udo_name,udo_requesttypeId,udo_Order&$filter=udo_Order ne null",
            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestTypeComplete,
            function () { alert("failure"); },
            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestTypeComplete);

    },

    //Complete async fetch request sub types
    loadRequestTypeComplete: function (data) {

        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestDataFetchComplete != null) {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestDataFetchComplete(data);
        }

        $('div#tmpDialog').hide();
        //$('#loadingMessageRequestType').css('display', 'none');
        $('#requestTypeSelect').css('display', 'inline');
        $('#requestTypeSelect').focus();
    },

    //Initiate Async fetch of request types
    loadRequestSubTypesAsync: function () {

        var urlName = Xrm.Page.context.getClientUrl();
        //$('#imgLoadingRequestSubType').attr('src', urlName + "/WebResources/udo_loadingGif.gif");

        $('#requestSubTypeSelect').css('display', 'none');
        //$('#loadingMessageRequestSubType').css('display', 'inline');
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        //Check if data in cache
        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType in Va.Udo.Crm.Scripts.SelectRequest.Data) {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete(Va.Udo.Crm.Scripts.SelectRequest.Data[Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType]);
        }
        else {//Else load data from CRM Server
            SDK.JQuery.retrieveMultipleRecords("udo_requestsubtype", "?$orderby=udo_Order&$select=udo_name,udo_requestsubtypeId,udo_Type,udo_Order&$filter=udo_Order ne null and udo_Type/Id eq (guid'" +
                Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType
                + "')",
                Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete,
                function () { alert("failure"); },
                Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete);
        }
        //}

    },

    // Complete Async fetch
    loadRequestSubTypesComplete: function (data) {
        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestSubTypeDataFetchComplete != null && data != null) {
            var id = null;
            if (data.length > 0) {
                id = data[0].udo_Type.Id;
            }
            Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestSubTypeDataFetchComplete(data);
            if (!(id in data)) {
                Va.Udo.Crm.Scripts.SelectRequest.Data[id] = data;
            }
        }

        $('div#tmpDialog').hide();
        //$('#loadingMessageRequestSubType').css('display', 'none');
        $('#requestSubTypeSelect').css('display', 'inline');
        $('#requestTypeSelect').focus();
    },

    //Initite Request creation
    createRequestAsync: function () {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = true;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
        var dataParameter = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.Utility.getQueryParameter("data"));
        var request = {};
        request.udo_title = requestTypeSelect.options[requestTypeSelect.selectedIndex].innerHTML + "/" +
            requestSubTypeSelect.options[requestSubTypeSelect.selectedIndex].innerHTML + " for " + dataParameter.vetName;
        request.udo_IdProof = { Id: dataParameter.idProofId, LogicalName: "udo_idproof" };
        request.udo_Interaction = { Id: dataParameter.intrId, LogicalName: "udo_interaction" };
        request.udo_Type = { Id: Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType, LogicalName: "udo_requesttype" };
        request.udo_SubType = { Id: Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType, LogicalName: "udo_requestsubtype" };
        request.udo_Veteran = { Id: dataParameter.vetId, LogicalName: "contact" };
        SDK.JQuery.createRecord(request, "udo_request", Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestComplete,
            function () {
                alert("Request Creation Failed");
                Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = false;
                Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
            });
    },

    //Complete request creation
    createRequestComplete: function (req) {
        if (req != null && req.udo_requestId != null) {
            //if (window.isUSD) {
            debugger;
            var callUSDEvent = "http://event/?eventName=RequestAutomation" +
                encodeURIComponent("selectedrequesttypeid", Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType) +
                encodeURIComponent("selectedrequesttypename=", Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestTypeName) +
                encodeURIComponent("selectedrequestsubtypeid=", Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType) +
                encodeURIComponent("&selectedrequestsubtypename=", Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubTypeName);

            window.open(callUSDEvent);

            //}
        }
        //if (Xrm != null && Xrm.Utility != null) {
        //    var url = Xrm.Page.context.getClientUrl();
        //    if (req != null && req.udo_requestId != null) {
        //        if (url != null) {
        //            var url = url + "/main.aspx?etn=udo_request&extraqs=" + encodeURIComponent("id=" + req.udo_requestId) + "&pagetype=entityrecord";
        //            window.open(url);
        //        }
        //    }
        //}
        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = false;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
    }
}

//UI Event handlers
Va.Udo.Crm.Scripts.SelectRequest.UI = {

    initiatlize: function () {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.registerNotification(
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshRequestTypes,
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshRequestSubType,
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshCreateButton);
        Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestTypesAsync();
        //Load Event Handlers

        $("#createRequestButton").on("click", Va.Udo.Crm.Scripts.SelectRequest.UI.createRequestButtonClick);

        $("#requestTypeSelect").keypress(function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode == 38 || keyCode == 40) {
                $(this).change();
                //Va.Udo.Crm.Scripts.SelectRequest.Controller.setSelectedRequestType(element.value);
            }
        });

        $("#requestTypeSelect").change(function () {
            $("#requestTypeSelect option:selected").each(function (index, element) {
                Va.Udo.Crm.Scripts.SelectRequest.UI.clearSelect(requestSubTypeSelect);
                debugger;
                Va.Udo.Crm.Scripts.SelectRequest.Controller.setSelectedRequestType(element.value, element.innerHTML);
                //$('#requestTypeSelect').selectedIndex = index;
            });

        });

        $("#requestSubTypeSelect").change(function () {
            $("#requestSubTypeSelect option:selected").each(function (index, element) {
                debugger;
                Va.Udo.Crm.Scripts.SelectRequest.Controller.setSelectedRequestSubType(element.value, element.innerHTML);
            });

        });
    },

    //Reload the Request type UI
    refreshRequestTypes: function (data) {
        if (data != null) {
            var optionCount = requestTypeSelect.options.length;
            //Va.Udo.Crm.Scripts.SelectRequest.UI.clearSelect(requestTypeSelect);
            for (var i = 0; i < data.length; i++) {
                var dataIndex = i - data.length;
                if (data[i] != null && data[i].udo_name != null && data[i].udo_requesttypeId != null) {
                    //var option = new Option(data[i].udo_name, data[i].udo_requesttypeId);
                    //requestTypeSelect.options[optionCount+i] = option;
                    $('#requestTypeSelect').append("<option aria-label='" + data[i].udo_name + "' value='" + data[i].udo_requesttypeId + "'>" + data[i].udo_name + "</option>");
                }
            }

            $('#requestTypeSelect')[0].selectedIndex = 0;
            $("#requestTypeSelect").change();
            $('#requestTypeSelect').focus();
        }
    },

    //Reload Request sub types UI
    refreshRequestSubType: function (data) {
        if (data != null) {
            //Va.Udo.Crm.Scripts.SelectRequest.UI.clearSelect(requestSubTypeSelect);
            var optionCount = requestSubTypeSelect.options.length;
            for (var i = 0; i < data.length; i++) {
                if (data[i] != null && data[i].udo_name != null && data[i].udo_requestsubtypeId != null) {
                    //var option = new Option(data[i].udo_name, data[i].udo_requestsubtypeId, i == 0);
                    //requestSubTypeSelect.options[i+optionCount] = option;
                    $('#requestSubTypeSelect').append("<option aria-label='" + data[i].udo_name + "' value='" + data[i].udo_requestsubtypeId + "'>" + data[i].udo_name + "</option>");
                }
            }
        }
    },

    //Enable or disable requestcreate button
    refreshCreateButton: function (allowCreate) {
        if (allowCreate) {
            $('#createRequestButton').prop('disabled', false);
        }
        else {
            $('#createRequestButton').prop('disabled', true);
        }
    },

    //Clear the options in the select element.
    clearSelect: function (selectElement) {
        if (selectElement != null && selectElement.options != null && selectElement.options.length != null) {
            for (i = selectElement.options.length - 1; i >= 0; i--) {
                selectElement.remove(selectElement.options[i]);
            }
        }
    },

    createRequestButtonClick: function () {
        try {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestAsync();
        }
        catch (e) {
            alert("Encountered an exception while creating a request. Please contact your administrator");
        }
    }
};