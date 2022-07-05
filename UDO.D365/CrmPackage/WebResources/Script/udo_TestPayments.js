var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Payments = Va.Udo.Crm.Scripts.Payments || {};

//Request Data Model
Va.Udo.Crm.Scripts.Payments.Data = {
    IdProofId: null,
    PayeeCodes: null,
    PayeeCodeEtc: null
};


// Request Controller - Contains the business logic
Va.Udo.Crm.Scripts.Payments.Controller = {

    // Raise this event when the request types data is retrieved
    PayeeCodesDataFetchComplete: null,

    PaymentDataFetchComplete: null,

    //Raise this evnet when the request sub types data is retrieved
    SelectedPayeeCodeChanged: null,

    //
    UpdateStatus: null,

    IsBusy: true,

    Initiatlize: function (payeeCodeEtc, idProofId, displayPayeeCodes, displayPayments, updateStatus) {
        Va.Udo.Crm.Scripts.Payments.Controller.PayeeCodesDataFetchComplete = displayPayeeCodes;
        Va.Udo.Crm.Scripts.Payments.Controller.PaymentDataFetchComplete = displayPayments;
        Va.Udo.Crm.Scripts.Payments.Controller.UpdateStatus = updateStatus;
        Va.Udo.Crm.Scripts.Payments.Data.PayeeCodeEtc = payeeCodeEtc;
        Va.Udo.Crm.Scripts.Payments.Data.IdProofId = idProofId;
        Va.Udo.Crm.Scripts.Payments.Controller.loadPayeeCodesAsync();
    },

    //Initiate Async fetch of PayeeCodes
    loadPayeeCodesAsync: function () {
        SDK.JQuery.retrieveMultipleRecords("udo_payeecode", "?$orderby=udo_payeecode&$select=udo_name,udo_payeecodeId,udo_LoadPayment&$filter=udo_IdProofId/Id eq (guid'" +
            Va.Udo.Crm.Scripts.Payments.Data.IdProofId
            + "')",
        Va.Udo.Crm.Scripts.Payments.Controller.loadPayeeCodesComplete,
        function () {
            alert("Could not load payee codes");
            Va.Udo.Crm.Scripts.Payments.Controller.updateStatus("Payee codes retrieve failed.", true);
        },
        function () { Va.Udo.Crm.Scripts.Payments.Controller.updateStatus("Select a Payee Code", false); });

    },
    //Translate the data into a common format and invoke the UI event handler.
    loadPayeeCodesComplete: function (data) {
        Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes = [];
        var orgName = Xrm.Page.context.getOrgUniqueName();
        //var etc = Va.Udo.Crm.Scripts.Utility.getEntityTypeCode("udo_payeecode");
        etc = Va.Udo.Crm.Scripts.Payments.Data.PayeeCodeEtc;
        for (var i = 0; i < data.length; i++) {
            var code = data[i];
            var payeeCode = {};
            payeeCode.Id = code.udo_payeecodeId;
            payeeCode.Name = code.udo_name;
            payeeCode.LoadPayment = code.udo_LoadPayment;
            payeeCode.RelatedPaymentUrl = "/" + orgName + "/userdefined/areas.aspx?oId=" + code.udo_payeecodeId + "&oType=" + etc + "&pagemode=iframe&security=852023&tabSet=udo_udo_payeecode_udo_payment_PayeeCodeId&rof=false";

			//"/" + orgName + "main.aspx?etc=10177&id="+code.udo_payeecodeId+"&pagetype=entityrecord&navbar=off&cmdbar=false";
            Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes[i] = payeeCode;
        }
        if (Va.Udo.Crm.Scripts.Payments.Controller.PayeeCodesDataFetchComplete != null) {
            Va.Udo.Crm.Scripts.Payments.Controller.PayeeCodesDataFetchComplete(Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes);
        }
        if (data.length == 0) {
            Va.Udo.Crm.Scripts.Payments.Controller.updateStatus("No Payee Codes available", false);
        }
    },

    GetPaymentUrl: function (payeeCodeId) {
        for (i = 0; i < Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes.length; i++) {
            var payeeCode = Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes[i];
            if (payeeCode.Id == payeeCodeId) {
                Va.Udo.Crm.Scripts.Payments.Controller.loadPaymentsAsync(Va.Udo.Crm.Scripts.Payments.Data.PayeeCodes[i]);
            }
        }

    },

    loadPaymentsAsync: function (payeeCode) {
        //Call update of payee code then call load payments complete.
        if (payeeCode.LoadPayment != null && payeeCode.LoadPayment == true) {
            Va.Udo.Crm.Scripts.Payments.Controller.loadPaymentsComplete(payeeCode);
        }
        else {
            var payeeCodeRecord = {};
            payeeCodeRecord.udo_LoadPayment = true;
            try {
                Va.Udo.Crm.Scripts.Payments.Controller.updateStatus("Retrieving payments...", false)
                SDK.JQuery.updateRecord(payeeCode.Id, payeeCodeRecord, "udo_payeecode", function () { Va.Udo.Crm.Scripts.Payments.Controller.loadPaymentsComplete(payeeCode); },
                    function () {
                        alert('Error occured while retrieving payments. Please contact your administrator.');
                        Va.Udo.Crm.Scripts.Payments.Controller.updateStatus("Error occured while retrieving payments.", false);
                    });
            }
            catch (ex)
            { }
        }
    },

    loadPaymentsComplete: function (payeeCode) {
        if (payeeCode != null) {

            payeeCode.LoadPayment = true;
            if (Va.Udo.Crm.Scripts.Payments.Controller.PaymentDataFetchComplete != null) {
                Va.Udo.Crm.Scripts.Payments.Controller.PaymentDataFetchComplete(payeeCode.RelatedPaymentUrl);
            }
        }        
    },

    updateStatus: function (message, isError) {
        if (Va.Udo.Crm.Scripts.Payments.UI.UpdateStatus != null) {
            Va.Udo.Crm.Scripts.Payments.UI.UpdateStatus(message, isError);
        }
    }
}


//UI Event handlers
Va.Udo.Crm.Scripts.Payments.UI = {
    initiatlize: function () {
        var dataParameter = Va.Udo.Crm.Scripts.Utility.getDataParameter(Va.Udo.Crm.Scripts.Utility.getQueryParameter("data"));
        var idProofId = dataParameter.idProofId;
        if (idProofId != null) {
            Va.Udo.Crm.Scripts.Payments.Controller.Initiatlize(dataParameter.payeeCodeEtc, dataParameter.idProofId, Va.Udo.Crm.Scripts.Payments.UI.loadPayeeCodes, Va.Udo.Crm.Scripts.Payments.UI.openRelatedPaymentsUrl, Va.Udo.Crm.Scripts.Payments.UI.UpdateStatus);
            $("#payeeCodeList").change(function () {
                $("#payeeCodeList").prop("disabled", true);

                var urlName = Xrm.Page.context.getClientUrl();
                //$('#imgLoading').attr('src', urlName + "/WebResources/udo_loadingGif.gif");

                $('#paymentList').css('display', 'none');
                //$('#loadingMessage').css('display', 'inline');
                $('div#tmpDialog').show();
                $('div#tmpDialog').focus();

                $("#payeeCodeList option:selected").each(function (index, element) {
                    if (element.value != "0") {
                        Va.Udo.Crm.Scripts.Payments.Controller.GetPaymentUrl(element.value);
                    }
                    else {
                        $('#paymentList').css('display', 'inline');
                        //$('#loadingMessage').css('display', 'none');
                        Va.Udo.Crm.Scripts.Payments.UI.openRelatedPaymentsUrl(urlName + "/WebResources/udo_PaymentsNoPayeeSelected.html");
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
        // var paymentListIframe = document.getElementById("paymentList");
        // paymentListIframe.src = url;
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
    }
};




