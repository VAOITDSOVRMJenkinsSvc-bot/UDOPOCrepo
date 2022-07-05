var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.SelectRequest = Va.Udo.Crm.Scripts.SelectRequest || {};

window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "uii_option": "uii_options",
    "udo_requesttype": "udo_requesttypes",
    "udo_securityrole": "udo_securityroles",
    "role": "roles",
    "systemuserrole": "systemuserroles",
    "udo_requestsubtype": "udo_requestsubtypes",
    "udo_request": "udo_requests"
});

window["ENTITY_PRIMARY_KEYS"] = window["ENTITY_PRIMARY_KEYS"] || JSON.stringify({
    "uii_option": "uii_optionid",
    "udo_requesttype": "udo_requesttypeid",
    "role": "roleid",
    "systemuserrole": "systemuserroleid",
    "udo_requestsubtype": "udo_requestsubtypeid",
    "udo_request": "udo_requestid"
});
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
        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.UpdateRequestCreationStatus !== null) {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.UpdateRequestCreationStatus(
                Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType !== null &&
                Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType !== null &&
                !Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress);
        }
    },

    //Register all events here.
    registerNotification: function (requestDataFetchComplete, requestSubTypeDataFetchComplete, requestCreationStatus) {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestDataFetchComplete = requestDataFetchComplete;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestSubTypeDataFetchComplete = requestSubTypeDataFetchComplete;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.UpdateRequestCreationStatus = requestCreationStatus;
    },

    loadRequestTypesAsync: function () {
        Xrm.Utility.getGlobalContext()
            .then(
                function (globalContext) {
                    var roleSecurityEnabled = "N";

                    $('#requestTypeSelect').css('display', 'none');
                    $('div#tmpDialog').show();
                    $('div#tmpDialog').focus();

                    var filter = "?$select=uii_value&$filter=uii_name eq  'RoleSecurityEnabled'";
                    Xrm.WebApi.retrieveMultipleRecords("uii_option", filter)
                        .then(
                            function (data) {
                                if (data && data.value && data.value.length > 0) {
                                    roleSecurityEnabled = data.value[0].uii_value;
                                    var fetchXml = "";

                                    if (roleSecurityEnabled === "Y") {
                                        var userId = globalContext.userSettings.userId.replace("{", "").replace("}", "");
                                        fetchXml = "<fetch><entity name='udo_requesttype'>" +
                                            "<attribute name='udo_name'/>" +
                                            "<attribute name='udo_requesttypeid'/>" +
                                            "<attribute name='udo_order'/>" +
                                            "<link-entity name='udo_securityrole' from='udo_securityroleid' to='udo_securityroleid' alias='secRole' link-type='outer'>" +
                                            "<link-entity name='role' from='name' to='udo_name' alias='userrole' link-type='outer'>" +
                                            "<link-entity name='systemuserroles' from='roleid' to='roleid' alias='r' link-type='outer'>" +
                                            "<attribute name='roleid'/>" +
                                            "<filter type='and'>" +
                                            "<condition attribute='systemuserid' operator='eq' value='" + userId + "'/>" +
                                            "</filter>" +
                                            "</link-entity></link-entity>" +
                                            "</link-entity>" +
                                            "<filter type='and'>" +
                                            "<condition attribute='udo_order' operator='not-null'/>" +
                                            "<filter type='or'>" +
                                            "<condition attribute='udo_securityroleid' operator='null'/>" +
                                            "<filter type='and'>" +
                                            "<condition attribute='udo_securityroleid' operator='not-null'/>" +
                                            "<condition entityname='r' attribute='roleid' operator='not-null'/>" +
                                            "</filter></filter>" +
                                            "</filter>" +
                                            "<order attribute='udo_order' descending='false'/>" +
                                            "</entity></fetch>";
                                    }
                                    else {
                                        fetchXml = "<fetch><entity name='udo_requesttype'>" +
                                            "<attribute name='udo_name'/>" +
                                            "<attribute name='udo_requesttypeid'/>" +
                                            "<attribute name='udo_order'/>" +
                                            "<filter type='and'>" +
                                            "<condition attribute='udo_order' operator='not-null'/>" +
                                            "</filter>" +
                                            "<order attribute='udo_order' descending='false'/>" +
                                            "</entity></fetch>";
                                    }

                                    var fetch = "?fetchXml=" + encodeURIComponent(fetchXml);
                                    Xrm.WebApi.retrieveMultipleRecords("udo_requesttype", fetch, null)
                                        .then(function (data) {
                                            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestTypeComplete(data.value);
                                        },
                                            function (error) {
                                                console.log(error);
                                            });
                                }
                                else {
                                    alert("Error occurred - Unable to retrieve RoleSecurityEnabled option value.");
                                }
                            });
                },
                function (error) {
                    console.log("An error has occurred: " + error);
                }, function error(err) {
                    alert("An error has occurred");
                });
    },

    //Complete async fetch request sub types
    loadRequestTypeComplete: function (data) {

        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestDataFetchComplete !== null) {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestDataFetchComplete(data);
        }

        $('div#tmpDialog').hide();
        $('#requestTypeSelect').css('display', 'inline');
        $('#requestTypeSelect').focus();
    },

    //Initiate Async fetch of request types
    loadRequestSubTypesAsync: function () {

        $('#requestSubTypeSelect').css('display', 'none');
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        //Check if data in cache
        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType in Va.Udo.Crm.Scripts.SelectRequest.Data) {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete(Va.Udo.Crm.Scripts.SelectRequest.Data[Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType]);
        }
        else {
            var filter = "?$select=udo_name,udo_requestsubtypeid,_udo_type_value,udo_order&$filter=_udo_type_value eq " + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType.replace("{", "").replace("}", "") + " and statuscode eq 1 and udo_order ne null&$orderby=udo_order";
            Xrm.WebApi.retrieveMultipleRecords("udo_requestsubtype", filter)
                .then(function (data) {
                        Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete(data.value);
                    },
                    function (data) {
                        alert("failure");
                        Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete(data.value);
                    }
                );
        }
    },

    // Complete Async fetch
    loadRequestSubTypesComplete: function (data) {
        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestSubTypeDataFetchComplete !== null && data !== null) {
            var id = null;
            if (data.length > 0) {
                id = data[0]["_udo_type_value"];
            }
            Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestSubTypeDataFetchComplete(data);
            if (!(id in data)) {
                Va.Udo.Crm.Scripts.SelectRequest.Data[id] = data;
            }
        }

        $('div#tmpDialog').hide();
        $('#requestSubTypeSelect').css('display', 'inline');
        $('#requestTypeSelect').focus();
    },

    //Initite Request creation
    createRequestAsync: function () {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = true;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
    },

    setErrorMessage1: function(message) {
        $('#ErrorMessage1').text(message);
    },

    setErrorMessage2: function(message) {
        $('#ErrorMessage2').text(message);
    },

    setCreateRequestButtonMessage: function(message) {
        $("#createRequestButtonTop").html(message);
        $("#createRequestButtonBottom").html(message);
    },

    disableCreateButtons: function(message) {
        $("#createRequestButtonTop").prop("disabled",true);
        $("#createRequestButtonBottom").prop("disabled",true);
    },

    enableCreateButtons: function(message) {
        $("#createRequestButtonTop").prop("disabled",false);
        $("#createRequestButtonBottom").prop("disabled",false);
    },

    createRequestFinish: function (req, dataParameter) {

        var idproofid = dataParameter.idProofId.replace("{", "").replace("}", "");
        var interactionid = dataParameter.intrId.replace("{", "").replace("}", "");
        var veteranid = dataParameter.vetId.replace("{", "").replace("}", "");
        var veteranname = dataParameter.vetName;

        // Confirm combination is correct
        var filter = "?$select=udo_name&$filter=_udo_type_value eq " + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType.replace("{", "").replace("}", "") + " and statuscode eq 1 and udo_requestsubtypeid eq " + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType.replace("{", "").replace("}", "");
        Xrm.WebApi.retrieveMultipleRecords("udo_requestsubtype", filter).then(
            function (data) {
                if (data !== null && data.value !== null && data.value.length > 0) {
                    window.open("http://event/?eventname=RequestAutomation&interactionid=" + interactionid +
                    "&idproofid=" + idproofid +
                    "&selectedrequesttypeid=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType +
                    "&selectedrequesttypename=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestTypeName +
                    "&selectedrequestsubtypeid=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType +
                    "&selectedrequestsubtypename=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubTypeName +
                    "&veteranid=" + veteranid +
                    "&veteranname=" + veteranname);                            
                } else {
                    alert("Please select a valid request combination.");                        
                    Va.Udo.Crm.Scripts.SelectRequest.Controller.setCreateRequestButtonMessage("Create Request");
                }
            },
            function(error) {
                alert("Please select a valid request combination.");
                Va.Udo.Crm.Scripts.SelectRequest.Controller.setCreateRequestButtonMessage("Create Request");
            });

            Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = false;
            Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
        },
    
    //Complete request creation
    createRequestComplete: function (req) {

        var dataParameter = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.Utility.getQueryParameter("data"));

        var interactionid = dataParameter.intrId.replace("{", "").replace("}", "");

        if (window.IsUSD == true) {

            var filter = "?$select=udo_abletoservicechatrequest,udo_interactiontype,udo_channel&$filter=udo_interactionid eq " + interactionid;
            Xrm.WebApi.retrieveMultipleRecords("udo_interaction", filter).then(
                function (data) {
                    if (data !== null && data.value !== null && data.value.length > 0) {
                        if ((data.value[0].udo_channel === 752280001) && (data.value[0].udo_interactiontype === 751880001)) {
                            if (data.value[0].udo_abletoservicechatrequest === null) {
                                Va.Udo.Crm.Scripts.SelectRequest.Controller.setErrorMessage1("Please fill out the Able to Service Chat Request field on the Interaction record above and press Save before continuing.");
                                Va.Udo.Crm.Scripts.SelectRequest.Controller.setErrorMessage2("Please fill out the Able to Service Chat Request field on the Interaction record above and press Save before continuing.");
                                Va.Udo.Crm.Scripts.SelectRequest.Controller.setCreateRequestButtonMessage("Create Request");
                            } else {
                                Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestFinish(req, dataParameter);
                            }    
                        } else {
                            Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestFinish(req, dataParameter);
                        }
                    } else {
                        alert("Please select a valid request combination.");                        
                        Va.Udo.Crm.Scripts.SelectRequest.Controller.setCreateRequestButtonMessage("Create Request");
                    }
                },
                function(error) {
                    alert("Please select a valid request combination.");
                    Va.Udo.Crm.Scripts.SelectRequest.Controller.setCreateRequestButtonMessage("Create Request");
                });
        }
    }
};

//UI Event handlers
Va.Udo.Crm.Scripts.SelectRequest.UI = {

    initiatlize: function () {
        Va.Udo.Crm.Scripts.SelectRequest.Controller.registerNotification(
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshRequestTypes,
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshRequestSubType,
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshCreateButton);
        Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestTypesAsync();
        //Load Event Handlers

        $("#requestTypeSelect").keypress(function (e) {
            var keyCode = e.keyCode || e.which;
            if (keyCode === 38 || keyCode === 40) {
                $(this).change();
                //Va.Udo.Crm.Scripts.SelectRequest.Controller.setSelectedRequestType(element.value);
            }
        });

        $("#requestTypeSelect").change(function () {
            $("#requestTypeSelect option:selected").each(function (index, element) {
                Va.Udo.Crm.Scripts.SelectRequest.UI.clearSelect(requestSubTypeSelect);

                Va.Udo.Crm.Scripts.SelectRequest.Controller.setSelectedRequestType(element.value, element.innerHTML);
                //$('#requestTypeSelect').selectedIndex = index;
            });
        });

        $("#requestSubTypeSelect").change(function () {
            $("#requestSubTypeSelect option:selected").each(function (index, element) {
                Va.Udo.Crm.Scripts.SelectRequest.Controller.setSelectedRequestSubType(element.value, element.innerHTML);
            });

        });
    },

    //Reload the Request type UI
    refreshRequestTypes: function (data) {
        if (data !== null) {
            var optionCount = requestTypeSelect.options.length;
            Va.Udo.Crm.Scripts.SelectRequest.UI.clearSelect(requestTypeSelect);
            for (var i = 0; i < data.length; i++) {
                var dataIndex = i - data.length;
                if (data[i] != null && data[i].udo_name != null && data[i].udo_requesttypeid != null) {

                    $('#requestTypeSelect').append("<option aria-label='" + data[i].udo_name + "' value='" + data[i].udo_requesttypeid + "'>" + data[i].udo_name + "</option>");

                }
            }

            $('#requestTypeSelect')[0].selectedIndex = 0;
            $("#requestTypeSelect").change();
            $('#requestTypeSelect').focus();
        }
    },

    //Reload Request sub types UI
    refreshRequestSubType: function (data) {


        if (data !== null) {
            Va.Udo.Crm.Scripts.SelectRequest.UI.clearSelect(requestSubTypeSelect);
            var optionCount = requestSubTypeSelect.options.length;
            for (var i = 0; i < data.length; i++) {
                if (data[i] !== null && data[i].udo_name !== null && data[i].udo_requestsubtypeid !== null) {
                    $('#requestSubTypeSelect').append("<option aria-label='" + data[i].udo_name + "' value='" + data[i].udo_requestsubtypeid + "'>" + data[i].udo_name + "</option>");
                }
            }
        }
    },

    //Enable or disable requestcreate button
    refreshCreateButton: function (allowCreate) {
       if (allowCreate) {
            $('#createRequestButtonTop').prop('disabled', false);
            $('#createRequestButtonBottom').prop('disabled', false);
        }
        else {
            $('#createRequestButtonTop').prop('disabled', true);
            $('#createRequestButtonBottom').prop('disabled', true);
        }
    },

    //Clear the options in the select element.
    clearSelect: function (selectElement) {

        if (selectElement !== null && selectElement.options !== null && selectElement.options.length !== null) {
            for (i = selectElement.options.length - 1; i >= 0; i--) {
                selectElement.remove(selectElement.options[i]);
            }
        }
    },

    createRequestButtonClick: function (response) {
        try {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.setCreateRequestButtonMessage("Processing...");
            //Va.Udo.Crm.Scripts.SelectRequest.Controller.disableCreateButtons();
            Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestComplete(response);
        }
        catch (e) {
            alert("Encountered an exception while creating a request. Please contact your administrator");
            Va.Udo.Crm.Scripts.SelectRequest.Controller.setCreateRequestButtonMessage("Create Request");
            Va.Udo.Crm.Scripts.SelectRequest.Controller.enableCreateButtons();
        }
    }
};
