﻿"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Payments = Va.Udo.Crm.Scripts.Payments || {};

window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "udo_payeecode": "udo_payeecodes"
});

//Request Data Model
Va.Udo.Crm.Scripts.Payments.Data = {
    IdProofId: null,
    PayeeCodes: null,
    PayeeCodeEtc: null,
    ExecutingActions: [],
    AppId: null,
    AllComplete: false
};

// Request Controller - Contains the business logic
Va.Udo.Crm.Scripts.Payments.Controller = {

    // Raise this event when the request types data is retrieved
    PayeeCodesDataFetchComplete: null,

    PaymentDataFetchComplete: null,

    //Raise this evnet when the request sub types data is retrieved
    SelectedPayeeCodeChanged: null,

    UpdateStatus: null,

    IsBusy: true,

    initiatlize: function (payeeCodeEtc, idProofId, displayPayeeCodes, displayPayments, updateStatus, appId) {
        Va.Udo.Crm.Scripts.Payments.Controller.PayeeCodesDataFetchComplete = displayPayeeCodes;
        Va.Udo.Crm.Scripts.Payments.Controller.PaymentDataFetchComplete = displayPayments;
        Va.Udo.Crm.Scripts.Payments.Controller.UpdateStatus = updateStatus;
        Va.Udo.Crm.Scripts.Payments.Data.PayeeCodeEtc = payeeCodeEtc;
        Va.Udo.Crm.Scripts.Payments.Data.IdProofId = idProofId;
        Va.Udo.Crm.Scripts.Payments.Data.AppId = appId;
        Va.Udo.Crm.Scripts.Payments.Controller.loadPayeeCodesAsync();       
    },

    //Initiate Async fetch of PayeeCodes
    loadPayeeCodesAsync: function () {
        Xrm.WebApi.retrieveMultipleRecords("udo_payeecode", "?$filter=_udo_idproofid_value eq ('" +
            Va.Udo.Crm.Scripts.Payments.Data.IdProofId
            + "')&$orderby=udo_payeecode").then(function reply(response) {
                Va.Udo.Crm.Scripts.Payments.Controller.loadPayeeCodesComplete(response);
                Va.Udo.Crm.Scripts.Payments.Controller.updateStatus("Select a Payee Code", false);
            });
    },
    //Translate the data into a common format and invoke the UI event handler.
    loadPayeeCodesComplete: function (data) {
        Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes = [];
       
        var etc = Va.Udo.Crm.Scripts.Payments.Data.PayeeCodeEtc;
        var appId = Va.Udo.Crm.Scripts.Payments.Data.AppId;

        if (data && data.value) {
            for (var i = 0; i < data.value.length; i++) {
                var code = data.value[i];
                var payeeCode = {};
                payeeCode.Id = code.udo_payeecodeid;
                payeeCode.Name = code.udo_name;
                payeeCode.LoadPayment = code["udo_loadpayment@OData.Community.Display.V1.FormattedValue"];

                payeeCode.RelatedPaymentUrl = "/main.aspx?appid=" + appId + "&pagetype=entityrecord&etn=udo_payeecode&id=" + code.udo_payeecodeid + "&formid=5fb4491c-dbd9-469e-8c82-75e5ecae7f3f&cmdbar=false&navbar=off";

                Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes[i] = payeeCode;
            }
        }
        if (Va.Udo.Crm.Scripts.Payments.Controller.PayeeCodesDataFetchComplete !== null) {
            Va.Udo.Crm.Scripts.Payments.Controller.PayeeCodesDataFetchComplete(Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes);
        }
        if (data == null || data.value == null || data.value.length === 0) {
            Va.Udo.Crm.Scripts.Payments.Controller.updateStatus("No Payee Codes available", false);
        }
    },

    GetPaymentUrl: function (payeeCodeId) {
        for (var i = 0; i < Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes.length; i++) {
            var payeeCode = Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes[i];
            if (payeeCode.Id === payeeCodeId) {
                Va.Udo.Crm.Scripts.Payments.Controller.loadPaymentsAsync(Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes[i]);
            }
        }
    },
    loadPaymentsAsync: function (payeeCode) {
        //Call update of payee code then call load payments complete.
        if (payeeCode.LoadPayment !== null && payeeCode.LoadPayment === true) {

            Va.Udo.Crm.Scripts.Payments.Controller.loadPaymentsComplete(payeeCode);
        }
        else {
            try {
                var requestName = "udo_GetPayments";
                if (Va.Udo.Crm.Scripts.Payments.Data.ExecutingActions.indexOf(requestName) === -1)
                    Va.Udo.Crm.Scripts.Payments.Data.ExecutingActions.push(requestName);
                else
                    return;

                Va.Udo.Crm.Scripts.Payments.Controller.executeAction(payeeCode.Id, "udo_payeecode", "udo_paymentloadcomplete", requestName, payeeCode);
            }
            catch (ex) {
                alert(ex.message);
            }
        }
    },

    loadPaymentsComplete: function (payeeCode) {
        if (payeeCode !== null) {

            payeeCode.LoadPayment = true;

            if (Va.Udo.Crm.Scripts.Payments.Controller.PaymentDataFetchComplete !== null) {
                Va.Udo.Crm.Scripts.Payments.Controller.PaymentDataFetchComplete(payeeCode.RelatedPaymentUrl);
            }

            $('#loadingGifDiv').hide();
            $('#notFoundDiv').hide();

            $('#payeeSelect').show();
            window.open(payeeCode.RelatedPaymentUrl);
        }
    },

    updateStatus: function (message, isError) {
        if (Va.Udo.Crm.Scripts.Payments.UI.UpdateStatus !== null) {
            Va.Udo.Crm.Scripts.Payments.UI.UpdateStatus(message, isError);
        }
    },

    onResponse: function (responseObject, requestName, payeeCode, entityName) {
        Va.Udo.Crm.Scripts.Payments.Controller.actionsComplete(requestName);
        
        if (responseObject.Exception == true) {
            $('#notFoundDiv').text(responseObject.ResponseMessage);
            $('#notFoundDiv').show();
        } else {
            var payeeCodeRecord = {};
            payeeCodeRecord.udo_loadpayment = true;
            payeeCodeRecord.udo_paymentloadcomplete = true;

            Xrm.WebApi.updateRecord(entityName, payeeCode.Id, payeeCodeRecord).then(
                function () {
                    Va.Udo.Crm.Scripts.Payments.Controller.loadPaymentsComplete(payeeCode);
                }
            );
        }
    },

    actionsComplete: function (requestName) {

        var i = Va.Udo.Crm.Scripts.Payments.Data.ExecutingActions.indexOf(requestName);

        if (i !== -1)
            Va.Udo.Crm.Scripts.Payments.Data.ExecutingActions.splice(i, 1);

        if (Va.Udo.Crm.Scripts.Payments.Data.ExecutingActions.length > 0)
            return false;
        else {
            var allComplete = true;
            return true;
        }
    },

    assembleRequest: function (referenceId, referenceEntity, actionName) {
        var parameters = {};
        var parententityreference = {};
        parententityreference[referenceEntity + "id"] = referenceId;
        parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM." + referenceEntity;
        parameters.parententityreference = parententityreference;
        parameters.createpaymentrecords = true;

        parameterTypes = {
            "ParentEntityReference": {
                "typeName": "mscrm.crmbaseentity",
                "structuralProperty": 5
            },
            "CreatePaymentRecords": {
                "typeName": "Edm.Boolean",
                "structuralProperty": 1
            }
        };

        var request = {
            ParentEntityReference: parameters.parententityreference,
            CreatePaymentRecords: parameters.createpaymentrecords,
            getMetadata: function () {
                return {
                    boundParameter: null,
                    parameterTypes: parameterTypes,
                    operationType: 0,
                    operationName: actionName
                }
            }
        };
        return request;
    },

    GetPaymentsRequest: function (parentEntityReference, createPaymentRecords) {
        this.ParentEntityReference = parentEntityReference;
        this.CreatePaymentRecords = createPaymentRecords;
    },

    RunActionUCI: function (referenceId, referenceEntity, actionName, payeeCode, entityName) {

        Va.Udo.Crm.Scripts.Payments.Controller.GetPaymentsRequest.prototype.getMetadata = function () {
            return {
                boundParameter: null,
                parameterTypes: {
                    "ParentEntityReference": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5 // Entity Type
                    },
                    "CreatePaymentRecords": {
                        "typeName": "Edm.Boolean",
                        "structuralProperty": 1 // Primitive Type
                    }
                },
                operationType: 0, // This is an action. Use '1' for functions and '2' for CRUD
                operationName: actionName,
            };
        };

        var parententityreference = {};
        parententityreference[referenceEntity + "id"] = referenceId;
        parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM." + referenceEntity;

        var request = new Va.Udo.Crm.Scripts.Payments.Controller.GetPaymentsRequest(parententityreference, true);

        // Use the request object to execute the function
        Xrm.WebApi.online.execute(request).then(
            function (result) {
                if (result.ok) {
                    result.json().then(
                        function (response) {
                            Va.Udo.Crm.Scripts.Payments.Controller.onResponse(response, actionName, payeeCode, entityName);
                        });
                }
            },
            function (error) {
                Xrm.Utility.alertDialog(error.message);
            }
        );
    },

    executeAction: function (entityId, entityName, completeField, requestName, payeeCode) {

        var _destURL;
        var _completeEntity;
        var _completeField;
        var _entityId;

        $('#payeeSelect').hide();
        $('#loadingGifDiv').show();


        $('#notFoundDiv').hide();
        $('#loadingGifDiv').focus();

        _completeEntity = entityName;
        _completeField = completeField;
        _destURL = payeeCode.RelatedPaymentUrl;
        _entityId = entityId;

        Va.Udo.Crm.Scripts.Payments.Controller.RunActionUCI(entityId, entityName, requestName, payeeCode, entityName);
    }

}
//UI Event handlers
Va.Udo.Crm.Scripts.Payments.UI = {

    initiatlize: function () {
        var dataParameter = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.Utility.getQueryParameter("data"));
        var idProofId = dataParameter.idProofId;
        var appId = Va.Udo.Crm.Scripts.Utility.getAppId();
        if (idProofId !== null) {
            Va.Udo.Crm.Scripts.Payments.Controller.Initiatlize(dataParameter.payeeCodeEtc, dataParameter.idProofId, Va.Udo.Crm.Scripts.Payments.UI.loadPayeeCodes, Va.Udo.Crm.Scripts.Payments.UI.openRelatedPaymentsUrl, Va.Udo.Crm.Scripts.Payments.UI.UpdateStatus, appId);
            $("#payeeCodeList").change(function () {
                $("#payeeCodeList").prop("disabled", true);

                $("#payeeCodeList option:selected").each(function (index, element) {
                    if (element.value !== "0") {
                        Va.Udo.Crm.Scripts.Payments.Controller.GetPaymentUrl(element.value);
                    }
                    else {
                        $('#paymentList').css('display', 'inline');

                        Va.Udo.Crm.Scripts.Payments.UI.openRelatedPaymentsUrl("/WebResources/udo_PaymentsNoPayeeSelected.html");
                        $("#payeeCodeList").prop("disabled", false);
                        $('div#tmpDialog').hide();
                    }
                });
            });
        }
        else {
            alert('Unknown Id Proof');
        }
    },

    updatePayeeCodeList: function () {
        $("#payeeCodeList").prop("disabled", true);

        $("#payeeCodeList option:selected").each(function (index, element) {
            if (element.value !== "0") {
                Va.Udo.Crm.Scripts.Payments.Controller.GetPaymentUrl(element.value);
            }
            else {
                $('#paymentList').css('display', 'inline');

                Va.Udo.Crm.Scripts.Payments.UI.openRelatedPaymentsUrl("/WebResources/udo_PaymentsNoPayeeSelected.html");
                $("#payeeCodeList").prop("disabled", false);
                $('div#tmpDialog').hide();
            }
        });
    },
    //Load the payee codes drop down
    loadPayeeCodes: function (data) {
        $('#payeeCodeList').append("<option aria-label='Select a pay code' value='0'>-- Select --</option>");
        for (var i = 0; i < data.length; i++) {
            $('#payeeCodeList').append("<option aria-label='" + data[i].Name + "' value='" + data[i].Id + "'>" + data[i].Name + "</option>");
        }
        $('#payeeCodeList').prop("selectedIndex", 1);
        Va.Udo.Crm.Scripts.Payments.UI.updatePayeeCodeList();
    },

    //Open related payments url
    openRelatedPaymentsUrl: function (url) {
        window.open(url);
        $("#payeeCodeList").prop("disabled", false);
    },

    UpdateStatus: function (message, isError) {
        $("#statusLabel").text(message);
        $('#statusLabel').focus();

        if (isError) {
            $("label[for=statusLabel]").css({ background: "#fa334f" });
        }
        else {
            $("label[for=statusLabel]").css({ background: "#787c81" });
        }
    },

    BrowserWindowReadyEvent: function (context) {
        context.ui.formSelector.items.forEach(
            function (control, index) {
                control.setVisible(false);
            }
        );            

        context.ui.headerSection.setBodyVisible(false);
        context.ui.headerSection.setCommandBarVisible(false);
        context.ui.headerSection.setTabNavigatorVisible(false);
        context.ui.footerSection.setVisible(false);
    }
}