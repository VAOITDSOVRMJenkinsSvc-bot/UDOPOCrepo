"use strict";

var UDO = UDO || {};
UDO.Claims = UDO.Claims || {};

//var globalContext = Xrm.Utility.getGlobalContext();
//var version = globalContext.getVersion();
//var lib = new CrmCommonJS.CrmCommon(version, null);
//var webApi = lib.WebApi;

window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "udo_claim": "udo_claims",
    "udo_payeecode": "udo_payeecodes",
    "udo_payment": "udo_payments",
    "udo_legacypaymenthistory": "udo_legacypaymenthistories",
    "contact": "contacts",
    "va_claimprocessingtimes": "va_claimprocessingtimes"
});

UDO.Claims.Payments = {
    results: {},
    claim: {},

    getUrlParams: function () {

        if (Va && Va.Udo && Va.Udo.Crm && Va.Udo.Crm.Scripts && Va.Udo.Crm.Scripts.Utility) {
            var params = Va.Udo.Crm.Scripts.Utility.getUrlParams();
            if (params.data) return params.data;
            return params;
        }
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
        } catch (err) {
            return null;
        }
    },

    fetchClaim: function (idStr) {
        var filter = "$filter=udo_claimId eq " + idStr.replace("{", "").replace("}", "");
        //var columns = ['*'];
        var columns = ['udo_statuscode', '_udo_idproofid_value', '_udo_claimid_value', 'udo_claimstation', 'udo_dateofclaim'];
        UDO.Claims.Payments.results.claim = {};

        var me = UDO.Claims.Payments; //save this pointer through REST call
        //TODO: update to WebAPI
        console.log("i am a bad retrieve");
        Xrm.WebApi.retrieveMultipleRecords('udo_claim', "?$select=" + columns.join(',') + "&" + filter)
        //CrmRestKit2011.ByQuery('udo_claim', columns, filter, false)
        
        .then(
            function (data) {
                if (data.value && data.value.length > 0) {
                    me.results.claim.udo_StatusCode = data.value[0].udo_statuscode; //('endProductTypeCode');
                    me.results.claim.udo_IdProofId = data.value[0]["_udo_idproofid_value"];
                    me.results.claim.Id = data.value[0]["_udo_claimid_value"];
                    me.results.claim.udo_ClaimStation = data.value[0].udo_claimstation;
                    //me.results.claim.udo_ClaimRecieveDate = me.convertODataDate(data[0].udo_dateofclaim);
                    me.results.claim.udo_ClaimRecieveDate = me.convertUrlDate(data.value[0].udo_dateofclaim);
                }
            });
    },
    fetchAllClaims: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "$filter=_udo_idproofid_value eq " + idStr.replace("{", "").replace("}", "");
            var columns = ['udo_claimstatus', '_udo_idproofid_value', 'udo_claimstation', 'udo_dateofclaim'];
            UDO.Claims.Payments.results.allClaim = {};

            var me = UDO.Claims.Payments; //save this pointer through REST call
            //TODO: update to WebAPI
            Xrm.WebApi.retrieveMultipleRecords('udo_claim', "?$select=" + columns.join(',') + "&" + filter)
                //CrmRestKit2011.ByQuery('udo_claim', columns, filter, false)

                .then(
                    function (data) {
                        console.log(data);
                        if (data.value.length > 0) {
                            me.results.allClaim = [];
                            for (var index = 0; index < data.value.length; index++) {
                                me.results.allClaim[index] = {};
                                me.results.allClaim[index].udo_StatusTypeCode = data.value[index].udo_claimstatus; //('endProductTypeCode');
                                me.results.allClaim[index].udo_IdProofId = data.value[index]["_udo_idproofid_value"];
                                me.results.allClaim[index].Id = data.value[index].udo_claimid;
                                me.results.allClaim[index].udo_ClaimStation = data.value[index].udo_claimstation;
                                //me.results.allClaim[index].udo_ClaimRecieveDate = me.convertODataDate(data[index].udo_dateofclaim);
                                me.results.allClaim[index].udo_ClaimRecieveDate = data.value[index]["udo_dateofclaim@OData.Community.Display.V1.FormattedValue"];
                                resolve();
                            }
                        }
                    });
        });
        
    },
    fetchPayeeCode: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "$filter=udo_payeecodeid eq " + idStr.replace("{", "").replace("}", "");
            //var columns = ['_udo_idproofid_value'];
            UDO.Claims.Payments.results.payeecode = {};

            var me = UDO.Claims.Payments; //save this pointer through REST call
            Xrm.WebApi.retrieveMultipleRecords('udo_payeecode', "?$select=_udo_idproofid_value&" + filter)
            .then(
                function (data) {
                    if (data.value && data.value.length > 0) {
                        if (!me.results.IdProof) {
                            me.results.IdProof = {};
                            me.results.IdProof.Id = data.value[0]["_udo_idproofid_value"];
                            resolve();
                        }
                    }
                });
        });
        
    },
    fetchPayment: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "$filter=udo_paymentid eq " + idStr.replace("{", "").replace("}", "");
            var columns = ['udo_programtype', '_udo_idproofid_value', 'udo_amount', 'udo_paydate', 'udo_paymenttype', '_udo_payeecodeid_value', '_udo_veteranid_value'];
            UDO.Claims.Payments.results.payment = {};

            var me = UDO.Claims.Payments; //save this pointer through REST call
            //TODO: update to WebAPI
            Xrm.WebApi.retrieveMultipleRecords('udo_payment', "?$select=" + columns.join(',') + "&" + filter)
                //CrmRestKit2011.ByQuery('udo_payment', columns, filter, false)
                .then(
                    function (data) {
                        if (data.value && data.value.length > 0) {
                            me.results.payment = {};
                            me.results.payment.programType = data.value[0].udo_programtype; //('endProductTypeCode');
                            me.results.payment.paymentType = data.value[0].udo_paymenttype;
                            me.results.payment.IdProofId = data.value[0]["_udo_idproofid_value"];
                            me.results.payment.VeteranId = data.value[0]["_udo_veteranid_value"];
                            me.results.payment.PayeeCodeId = data.value[0]["_udo_payeecodeid_value"];
                            me.results.payment.paymentStatus = null;
                            me.results.payment.paymentAmount = data.value[0].udo_amount;
                            //me.results.payment.paymentDate = me.convertODataDate(data.value[0].udo_paydate);
                            me.results.payment.paymentDate = data.value[0]["udo_paydate@OData.Community.Display.V1.FormattedValue"];
                            me.results.payment.payCheckAmount = null; //data.value.[0].udo_ClaimRecieveDate;
                            me.results.payment.payCheckDate = null; //data.value[0].udo_ClaimRecieveDate;
                            resolve();
                        }
                        //}
                    });
        });
        
    },
    fetchLegacyPayment: function (idStr) {
        return new Promise(function (resolve, reject) {
            console.log("fetchLegacyPayment");
            var filter = "$filter=udo_legacypaymenthistoryid eq " + idStr.replace("{", "").replace("}", "");
            var columns = ['udo_payee', '_udo_idproofid_value', '_udo_veteranid_value'];
            UDO.Claims.Payments.results.payment = {};

            var me = UDO.Claims.Payments; //save this pointer through REST call
            //TODO: update to WebAPI
            Xrm.WebApi.retrieveMultipleRecords('udo_legacypaymenthistory', "?$select=" + columns.join(',') + "&" + filter)
                //CrmRestKit2011.ByQuery('udo_legacypaymenthistory', columns, filter, false)
                .then(
                    function (data) {
                        if (data.value && data.value.length > 0) {
                            me.results.payment = {};
                            me.results.payment.Payee = data.value[0].udo_payee;
                            me.results.IdProof = data.value[0]["_udo_idproofid_value"];
                            me.results.payment.VeteranId = data.value[0]["_udo_veteranid_value"];
                        }
                        resolve();
                    });
        });
        
    },
    fetchContact: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "$filter=contactid eq " + idStr;
            //var columns = ['fullname'];
            UDO.Claims.Payments.results.contact = {};

            var me = UDO.Claims.Payments; //save this pointer through REST call
            //TODO: update to WebAPI
            Xrm.WebApi.retrieveMultipleRecords('contact', "?$select=fullname&" + filter)
                .then(
                function (data) {
                        if (data.value && data.value.length > 0) {
                            if (!me.results.contact) {
                                me.results.contact = {};
                            }
                            me.results.contact.Id = data.value[0].contactid;
                            me.results.contact.FullName = data.value[0].fullname;
                            resolve(data.value);
                        }
                    });
        });
        
    },
    buildContactObject: function (contactRef, roj, rad) {
        return new Promise(function (resolve, reject) {
            UDO.Claims.Payments.results.contact = {};
            if (roj && roj !== null) { UDO.Claims.Payments.results.contact.FolderLocation = roj; }
            if (contactRef.contactid && contactRef.fullname) {
                UDO.Claims.Payments.results.contact.contactId = contactRef.contactid;
                UDO.Claims.Payments.results.contact.FullName = contactRef.fullname;
            }
            if (rad && rad !== null) { UDO.Claims.Payments.results.contact.ReleasedActiveDutyDate = rad; }
            resolve();
        });
        
    },
    convertODataDate: function (OdataDate) {
        if (OdataDate === null || OdataDate.length === 0)
            return OdataDate;
        if (OdataDate.indexOf("/Date(") == 0) {
            var date = new Date(parseInt(OdataDate.substr(6)));
            var dateStr = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
            return dateStr;
        } else {
            return OdataDate;
        }
    },
    convertUrlDate: function (date) {
        var uDate = date.split("/");
        var year = uDate[2];
        var month = parseInt(uDate[0]) - 1;
        var day = uDate[1];
        return new Date(year, month, day);
    },
    getDataFromCRM: function () {
        console.log("getDataFromCRM");
        return new Promise(function (resolve, reject) {
            console.log("inpromise");
            var UrlParams = UDO.Claims.Payments.getUrlParams();
            if (UrlParams === null)
                return null;
            console.log(UrlParams);
            var parameterPromises = [];
            if (UrlParams.id === undefined || UrlParams.id === null && Url.Params.vid)
                parameterPromises.push(UDO.Claims.Payments.buildContactObject(UDO.Claims.Payments.fetchContact(UrlParams.vid), UrlParams.roj, UrlParams.rad));

            if (UrlParams.islegacypayment && UrlParams.islegacypayment === "true")
                parameterPromises.push(UDO.Claims.Payments.fetchLegacyPayment(UrlParams.id));
            else
                parameterPromises.push(UDO.Claims.Payments.fetchPayment(UrlParams.id));

            return Promise.all(parameterPromises).then(function (parameterResult) {
                console.log("in promise all");
                console.log(UDO.Claims.Payments.results);
                if (UDO.Claims.Payments.results.payment) {
                    if (UDO.Claims.Payments.results.payment.PayeeCodeId) {
                        UDO.Claims.Payments.fetchPayeeCode(UDO.Claims.Payments.results.payment.PayeeCodeId).then(function () {
                            if (UDO.Claims.Payments.results.IdProof.Id) {
                                UDO.Claims.Payments.fetchAllClaims(UDO.Claims.Payments.results.IdProof.Id).then(function () {
                                    if (UDO.Claims.Payments.results.payment.VeteranId) {
                                        UDO.Claims.Payments.fetchContact(UDO.Claims.Payments.results.payment.VeteranId).then(function (contactResult) {
                                            console.log(contactResult);
                                            UDO.Claims.Payments.buildContactObject(contactResult[0], UrlParams.roj, UrlParams.rad).then(function () {
                                                resolve(UDO.Claims.Payments.results);
                                            });
                                        });
                                    }
                                });
                            }
                        });
                    } else {
                        if (UDO.Claims.Payments.results.IdProof) {
                            UDO.Claims.Payments.fetchAllClaims(UDO.Claims.Payments.results.IdProof).then(function () {
                                if (UDO.Claims.Payments.results.payment.VeteranId) {
                                    UDO.Claims.Payments.fetchContact(UDO.Claims.Payments.results.payment.VeteranId).then(function (contactResult) {
                                        console.log(contactResult);
                                        UDO.Claims.Payments.buildContactObject(contactResult[0], UrlParams.roj, UrlParams.rad).then(function () {
                                            resolve(UDO.Claims.Payments.results);
                                        });
                                    });
                                }
                            });
                        }
                    }
                }
                //return this.results;
            });
            
        });
        
    }
};
