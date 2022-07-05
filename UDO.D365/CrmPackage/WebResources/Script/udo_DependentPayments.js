var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.DependentPayments = Va.Udo.Crm.Scripts.DependentPayments || {};

//Request Data Model
Va.Udo.Crm.Scripts.DependentPayments.Data = {
    IdDependentId: null,
    PayeeCodes: null,
    PayeeCodeEtc: null
};

// Request Controller - Contains the business logic
Va.Udo.Crm.Scripts.DependentPayments.Controller = {

    // Raise this event when the request types data is retrieved
    PayeeCodesDataFetchComplete: null,

    PaymentDataFetchComplete: null,

    //Raise this evnet when the request sub types data is retrieved
    SelectedPayeeCodeChanged: null,

    //
    UpdateStatus: null,

    IsBusy: true,

    Initiatlize: function (payeeCodeEtc, IdDependentId, displayPayeeCodes, displayPayments, updateStatus) {
        Va.Udo.Crm.Scripts.DependentPayments.Controller.PayeeCodesDataFetchComplete = displayPayeeCodes;
        Va.Udo.Crm.Scripts.DependentPayments.Controller.PaymentDataFetchComplete = displayPayments;
        Va.Udo.Crm.Scripts.DependentPayments.Controller.UpdateStatus = updateStatus;
        Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodeEtc = payeeCodeEtc;
        Va.Udo.Crm.Scripts.DependentPayments.Data.IdDependentId = IdDependentId;
        Va.Udo.Crm.Scripts.DependentPayments.Controller.loadPayeeCodesAsync();
    },

    //Initiate Async fetch of PayeeCodes
    loadPayeeCodesAsync: function () {

        SDK.JQuery.retrieveMultipleRecords("udo_payeecode", "?$orderby=udo_payeecode&$select=udo_name,udo_payeecodeId,udo_LoadPayment&$filter=udo_dependentid/Id eq (guid'" +
            Va.Udo.Crm.Scripts.DependentPayments.Data.IdDependentId
            + "')",
        Va.Udo.Crm.Scripts.DependentPayments.Controller.loadPayeeCodesComplete,
        function () {
            alert("Could not load payee codes");
            Va.Udo.Crm.Scripts.DependentPayments.Controller.updateStatus("Payee codes retrieve failed.", true);
        },
        function () {

            Va.Udo.Crm.Scripts.DependentPayments.Controller.updateStatus("Select a Payee Code", false);
        });

    },
    //Translate the data into a common format and invoke the UI event handler.
    loadPayeeCodesComplete: function (data) {
        Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodes = [];
        var orgName = Xrm.Page.context.getOrgUniqueName();

        for (var i = 0; i < data.length; i++) {
            var code = data[i];
            var payeeCode = {};
            payeeCode.Id = code.udo_payeecodeId;
            payeeCode.Name = code.udo_name;
            payeeCode.LoadPayment = code.udo_LoadPayment;
            payeeCode.RelatedPaymentUrl = "/" + orgName + "/userdefined/areas.aspx?oId=" + code.udo_payeecodeId + "&oType=" + Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodeEtc + "&pagemode=iframe&security=852023&tabSet=udo_udo_payeecode_udo_payment_PayeeCodeId&rof=false"
            Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodes[i] = payeeCode;
        }
        if (Va.Udo.Crm.Scripts.DependentPayments.Controller.PayeeCodesDataFetchComplete != null) {
            Va.Udo.Crm.Scripts.DependentPayments.Controller.PayeeCodesDataFetchComplete(Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodes);
        }
        if (data.length == 0) {
            Va.Udo.Crm.Scripts.DependentPayments.Controller.updateStatus("No Payee Codes available", false);
        }
    },

    GetPaymentUrl: function (payeeCodeId) {
        for (i = 0; i < Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodes.length; i++) {
            var payeeCode = Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodes[i];
            if (payeeCode.Id == payeeCodeId) {
                Va.Udo.Crm.Scripts.DependentPayments.Controller.loadPaymentsAsync(Va.Udo.Crm.Scripts.DependentPayments.Data.PayeeCodes[i]);
            }
        }

    },

    loadPaymentsAsync: function (payeeCode) {
        //Call update of payee code then call load payments complete.
        if (payeeCode.LoadPayment != null && payeeCode.LoadPayment == true) {
            Va.Udo.Crm.Scripts.DependentPayments.Controller.loadPaymentsComplete(payeeCode);
        }
        else {
            var payeeCodeRecord = {};
            payeeCodeRecord.udo_LoadPayment = true;
            try {
                Va.Udo.Crm.Scripts.DependentPayments.Controller.updateStatus("Retrieving payments...", false)
                SDK.JQuery.updateRecord(payeeCode.Id, payeeCodeRecord, "udo_payeecode", function () { Va.Udo.Crm.Scripts.DependentPayments.Controller.loadPaymentsComplete(payeeCode); },
                    function () {
                        alert('Error occured while retrieving payments. Please contact your administrator.');
                        Va.Udo.Crm.Scripts.DependentPayments.Controller.updateStatus("Error occured while retrieving payments.", false);
                    });
            }
            catch (ex)
            { }
        }
    },

    loadPaymentsComplete: function (payeeCode) {
        if (payeeCode != null) {

            payeeCode.LoadPayment = true;
            if (Va.Udo.Crm.Scripts.DependentPayments.Controller.PaymentDataFetchComplete != null) {
                Va.Udo.Crm.Scripts.DependentPayments.Controller.PaymentDataFetchComplete(payeeCode.RelatedPaymentUrl);
            }
        }
        Va.Udo.Crm.Scripts.DependentPayments.Controller.updateStatus("Select a Payee Code.", false);
    },

    updateStatus: function (message, isError) {
        if (Va.Udo.Crm.Scripts.DependentPayments.UI.UpdateStatus != null) {
            Va.Udo.Crm.Scripts.DependentPayments.UI.UpdateStatus(message, isError);
        }
    },

    getEntityTypeCode: function (entityName) {
        try {
            var command = new window.parent.RemoteCommand("LookupService", "RetrieveTypeCode");
            command.SetParameter("entityName", entityName);
            var result = command.Execute();
            if (result.Success && typeof result.ReturnValue == "number") {
                return result.ReturnValue;
            }
            else {
                return null;
            }
        }
        catch (ex) {
            return null;
        }
    }

}


//UI Event handlers
Va.Udo.Crm.Scripts.DependentPayments.UI = {
    initialize: function () {

        var etc = Va.Udo.Crm.Scripts.DependentPayments.Controller.getEntityTypeCode("udo_payeecode");
        var dataParameter = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.DependentPayments.UI.getQueryParameter("data"));
        var IdDependentId = dataParameter.id;
        if (IdDependentId != null) {
            Va.Udo.Crm.Scripts.DependentPayments.Controller.Initiatlize(etc, IdDependentId, Va.Udo.Crm.Scripts.DependentPayments.UI.loadPayeeCodes, Va.Udo.Crm.Scripts.DependentPayments.UI.openRelatedPaymentsUrl, Va.Udo.Crm.Scripts.DependentPayments.UI.UpdateStatus);

            $("#payeeCodeList").change(function () {
                $("#payeeCodeList").prop("disabled", true);

                var urlName = Xrm.Page.context.getClientUrl();
                //$('#imgLoading').attr('src' , urlName + "/WebResources/udo_loadingGif.gif");

                $('#paymentList').css('display', 'none');
                //$('#loadingMessage').css('display', 'inline');  
                $('div#tmpDialog').show();
                $('div#tmpDialog').focus();

                $("#payeeCodeList option:selected").each(function (index, element) {
                    if (element.value != "0") {
                        Va.Udo.Crm.Scripts.DependentPayments.Controller.GetPaymentUrl(element.value);
                    }
                    else {
                        $('#paymentList').css('display', 'inline');
                        //$('#loadingMessage').css('display', 'none');

                        Va.Udo.Crm.Scripts.DependentPayments.UI.openRelatedPaymentsUrl(urlName + "/WebResources/udo_PaymentsNoPayeeSelected.html");
                        $("#payeeCodeList").prop("disabled", false);
                        $('div#tmpDialog').hide();
                    }
                });
            });
        }
        else {
            alert('Uknown Id Dependent');
        }
    },


    getUrlParams: function () {

        try {
            var sPageUrl = location.search.substring(1);
            var regex = new RegExp("[\\?&]?data=([^&#]*)");
            sPageUrl = decodeURIComponent(regex.exec(sPageUrl)[1]);
            var params = sPageUrl.split('&');
            var UrlParams = {};

            for (var index = 0; index < params.length; index++) {
                param = params[index].split('=');
                UrlParams[param[0]] = decodeURIComponent(param[1].split("#")[0]);
            }

            return UrlParams;
        }
        catch (err) {
            return null;
        }
    },

    getQueryParameter: function (name) {

        //name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        //var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        //    results = regex.exec(window.parent.location.search);  
        //         var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        var results = location.search;
        //return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        return results === null ? "" : decodeURIComponent(results);
    },

    //Load the payee codes drop down
    loadPayeeCodes: function (data) {
        $('#payeeCodeList').append("<option aria-label='Select a pay code' value='0'>-- Select --</option>");
        for (var i = 0; i < data.length; i++) {
            $('#payeeCodeList').append("<option aria-label='" + data[i].Name + "' value='" + data[i].Id + "'>" + data[i].Name + "</option>");
        }
        $('#payeeCodeList').prop("selectedIndex", 1).change();
    },

    //Open related payments url
    openRelatedPaymentsUrl: function (url) {
        var paymentListIframe = document.getElementById("paymentList");
        paymentListIframe.src = url;
        $("#payeeCodeList").prop("disabled", false);
    },

    //Open related payments url
    openRelatedPaymentsHTML: function (html) {
        var paymentListIframe = document.getElementById("paymentList");
        paymentListIframe.innerHTML = html;
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
    }
};
