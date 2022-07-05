var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.SelectRequest = Va.Udo.Crm.Scripts.SelectRequest || {};
var globalContext = GetGlobalContext();
var webApi;


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
    "udo_request" : "udo_requestid"
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

        var roleSecurityEnabled = "N";
        var urlName = globalContext.getClientUrl();

        $('#requestTypeSelect').css('display', 'none');
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        var filter = "$filter=";
        filter += "uii_name eq 'RoleSecurityEnabled'";
        var columns = ['uii_value'];
        //todo: convert to webapi        
        webApi.RetrieveMultiple("uii_option", columns, filter)
            .then(
                function (data) {

                    if (data && data.length > 0) {

                        roleSecurityEnabled = data[0].uii_value;
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
                        //todo: convert to webapi
                        webApi.RetrieveByFetchXml("udo_requesttype", fetchXml)
                            .then(function (data) {
                                Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestTypeComplete(data);
                            })
                            .catch(function (error) {
                                throw error;
                            });
                    }
                    else {
                        alert("Error occurred - Unable to retrieve RoleSecurityEnabled option value.");
                    }
                }).catch(function (error) { alert(error.message); });


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

        var urlName = Xrm.Page.context.getClientUrl();

        $('#requestSubTypeSelect').css('display', 'none');
        $('div#tmpDialog').show();
        $('div#tmpDialog').focus();

        

        //Check if data in cache
        if (Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType in Va.Udo.Crm.Scripts.SelectRequest.Data) {
            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete(Va.Udo.Crm.Scripts.SelectRequest.Data[Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType]);
        }
        else {
            //todo: convert to webapi
            var cols = ["udo_name", "udo_requestsubtypeid", "_udo_type_value", "udo_order"];
            var filter = "$filter=_udo_type_value eq " + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType.replace("{", "").replace("}", "") + " and udo_order ne null&$orderby=udo_order";
            webApi.RetrieveMultiple("udo_requestsubtype", cols, filter)
                .then(function (data) {
                    Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete(data);
                })
                .catch(function (data) {
                    alert("failure");
                    Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete(data);
                });

            /*SDK.JQuery.retrieveMultipleRecords("udo_requestsubtype", "?$orderby=udo_Order&$select=udo_name,udo_requestsubtypeId,udo_Type,udo_Order&$filter=udo_Type/Id eq (guid'" +
                Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType
                + "') and udo_Order ne null",
            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete,
            function () { alert("failure"); },
            Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestSubTypesComplete);*/
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
        //var dataParameter = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.Utility.getQueryParameter("data"));

        //var idproofid = dataParameter.idProofId.replace("{", "").replace("}", "");
        //var interactionid = dataParameter.intrId.replace("{", "").replace("}", "");
        //var requesttypeid = Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType;
        //var requesttypename = Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestTypeName;
        //var requestsubtypeid = Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType;
        //var requestsubtypename = Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubTypeName;
        //var veteranid = dataParameter.vetId.replace("{", "").replace("}", "");
        //var veteranname = dataParameter.vetName;

        //if (window.IsUSD == true) {
        //    window.open("http://event/?eventname=CreateRequest&interactionid=" + interactionid +
        //        "&idproofid=" + idproofid + "&requesttypeid=" + requesttypeid + "&requesttypename=" + requesttypename +
        //        "&requestsubtypeid=" + requestsubtypeid + "&requestsubtypename=" + requestsubtypename +
        //        "&veteranid=" + veteranid + "&veteranname=" + veteranname);
        //}

        //var request = {};
        //request.udo_title = requestTypeSelect.options[requestTypeSelect.selectedIndex].innerHTML + "/" +
        //    requestSubTypeSelect.options[requestSubTypeSelect.selectedIndex].innerHTML + " for " + dataParameter.vetName;

        //request["udo_idproof@odata.bind"] = "/udo_idproofs(" + dataParameter.idProofId.replace("{", "").replace("}", "") + ")";
        //request["udo_interaction@odata.bind"] = "/udo_interactions(" + dataParameter.intrId.replace("{", "").replace("}", "") + ")";
        //request["udo_type@odata.bind"] = "/udo_requesttypes(" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType.replace("{", "").replace("}", "") + ")";
        //request["udo_subtype@odata.bind"] = "/udo_requestsubtypes(" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType.replace("{", "").replace("}", "") + ")";
        //request["udo_veteran@odata.bind"] = "/contacts(" + dataParameter.vetId.replace("{", "").replace("}", "") + ")";
        ////todo: convert to webapi
        //webApi.CreateRecord(request, "udo_request")
        //    .then(function reply(response) {
        //        //Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestComplete(data);
        //        if (window.IsUSD == true) {
        //            window.open("http://event/?eventname=RequestCreated&id=" + response.id);
        //        }
        //    })
        //    .catch(function (error) {
        //        alert("Request Creation Failed");
        //     //   Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = false;
        //     //   Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
        //    });
        /*SDK.JQuery.createRecord(request, "udo_request", Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestComplete,
            function () {
                alert("Request Creation Failed");
                Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = false;
                Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
            });*/
    },

    //Complete request creation
    //todo: convert to global context
    createRequestComplete: function (req) {

        var url = globalContext.getClientUrl();
        if (req !== null && req.udo_requestid !== null) {
            if (url !== null) {

                url += "/main.aspx?etn=udo_request&extraqs=" + encodeURIComponent("id=" + req.id) + "&pagetype=entityrecord";
                //var entityFormOptions = {};
                //entityFormOptions.entityName = "udo_request";
                //entityFormOptions.entityId = req.id;
                //Xrm.Navigation.openForm(entityFormOptions);
                //window.open(url);

                var dataParameter = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.Utility.getQueryParameter("data"));

                var idproofid = dataParameter.idProofId.replace("{", "").replace("}", "");
                var interactionid = dataParameter.intrId.replace("{", "").replace("}", "");
                var veteranid = dataParameter.vetId.replace("{", "").replace("}", "");
                var veteranname = dataParameter.vetName;

                if (window.IsUSD == true) {
                    window.open("http://event/?eventname=RequestAutomation&interactionid=" + interactionid +
                        "&idproofid=" + idproofid +
                        "&selectedrequesttypeid=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestType +
                        "&selectedrequesttypename=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestTypeName +
                        "&selectedrequestsubtypeid=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubType +
                        "&selectedrequestsubtypename=" + Va.Udo.Crm.Scripts.SelectRequest.Controller.SelectedRequestSubTypeName +
                        "&veteranid=" + veteranid +
                        "&veteranname=" + veteranname);
                }
            }
        }

        Va.Udo.Crm.Scripts.SelectRequest.Controller.RequestCreationInProgress = false;
        Va.Udo.Crm.Scripts.SelectRequest.Controller.refreshRequestCreationStatus();
    }
};

//UI Event handlers
Va.Udo.Crm.Scripts.SelectRequest.UI = {

    initiatlize: function () {
        //todo: setup webapi
        var version = globalContext.getVersion();
        webApi = new CrmCommonJS.WebApi(version);

        Va.Udo.Crm.Scripts.SelectRequest.Controller.registerNotification(
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshRequestTypes,
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshRequestSubType,
            Va.Udo.Crm.Scripts.SelectRequest.UI.refreshCreateButton);
        Va.Udo.Crm.Scripts.SelectRequest.Controller.loadRequestTypesAsync();
        //Load Event Handlers

        $("#createRequestButtonTop").one("click", Va.Udo.Crm.Scripts.SelectRequest.UI.createRequestButtonClick);
        $("#createRequestButtonBottom").one("click", Va.Udo.Crm.Scripts.SelectRequest.UI.createRequestButtonClick);

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
                    //var option = new Option(data[i].udo_name, data[i].udo_requestsubtypeId, i == 0);
                    //requestSubTypeSelect.options[i+optionCount] = option;
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
            //Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestAsync();
            $('#createRequestButtonTop').prop('disabled', true);
            $('#createRequestButtonBottom').prop('disabled', true);
            Va.Udo.Crm.Scripts.SelectRequest.Controller.createRequestComplete(response);
        }
        catch (e) {
            alert("Encountered an exception while creating a request. Please contact your administrator");
        }
    }
};

