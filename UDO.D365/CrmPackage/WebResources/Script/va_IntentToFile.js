"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Buttons = Va.Udo.Crm.Buttons || {};

Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
Va.Udo.Crm.Scripts.Buttons = Va.Udo.Crm.Scripts.Buttons || {};

function startTrackEvent(name, properties) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.startTrackEvent) {
            Va.Udo.AppInsights.startTrackEvent(name, properties);
        }
    }
    catch (ex) {
        console.log("Error occured while logging startTrackEvent to App Insights: " + ex.message);
    }
}

function stopTrackEvent(name, properties) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.stopTrackEvent) {
            Va.Udo.AppInsights.stopTrackEvent(name, properties);
        }
    }
    catch (ex) {
        console.log("Error occured while logging stopTrackEvent to App Insights: " + ex.message);
    }
}

function trackException(ex) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.trackException) {
            Va.Udo.AppInsights.trackException(ex);
        }
    }
    catch (ex) {
        console.log("Error occured while logging trackException to App Insights: " + ex.message);
    }
}

function trackPageView(name, properties) {
    try {
        if (Va.Udo.AppInsights.IsInitialized && Va.Udo.AppInsights.trackPageView) {
            Va.Udo.AppInsights.trackPageView(name, properties);
        }
    }
    catch (ex) {
        console.log("Error occured while logging trackPageView to App Insights: " + ex.message);
    }
}


Va.Udo.Crm.Scripts.Buttons.AddButton = function (id, name, label, method, image, enabled) {
    var config = {};
    config.name = name;
    config.label = label;
    config.onClick = method;
    config.image = image;
    config.id = id;
    if (typeof enabled === "undefined" || enabled === null) enabled = true;
    config.enabled = enabled;
    Va.Udo.Crm.Buttons[id] = config;
};
Va.Udo.Crm.Scripts.Buttons.EnableButton = function (id) {
    try {
        var btn = Va.Udo.Crm.Buttons[id];
        btn.enable();
    } catch (err) {
        UDO.Shared.openAlertDialog("The button [" + id + "] was not found or initialized.");
    }
};
Va.Udo.Crm.Scripts.Buttons.DisableButton = function (id) {
    try {
        var btn = Va.Udo.Crm.Buttons[id];
        btn.disable();
    } catch (err) {
        UDO.Shared.openAlertDialog("The button [" + id + "] was not found or initialized.");
    }
};

//udo_utility.js manually pasted into this file to circumvent load order issues - CChannon 5/30/17

Va.Udo.Crm.Scripts.Utility = Va.Udo.Crm.Scripts.Utility || {};

Va.Udo.Crm.Scripts.Utility.getPageCache = 0;
Va.Udo.Crm.Scripts.Utility.getPage = function () {
    if (Va.Udo.Crm.Scripts.Utility.getPageCache === 0) {
        var search = location.search;
        var url = location.href;
        var page = "";
        if (search === "") {
            var pos = url.toLowerCase().indexOf("%3f");
            if (pos > -1) {
                search = "?" + decodeURIComponent(url.substring(pos + 3));
                page = url.substring(0, pos);
                url = page + search;
            }
        } else {
            page = url.substring(0, url.indexOf("?"));
        }
        Va.Udo.Crm.Scripts.Utility.getPageCache = {};
        Va.Udo.Crm.Scripts.Utility.getPageCache.page = page;
        Va.Udo.Crm.Scripts.Utility.getPageCache.search = search;
        Va.Udo.Crm.Scripts.Utility.getPageCache.url = url;
    }
    return Va.Udo.Crm.Scripts.Utility.getPageCache;
};

Va.Udo.Crm.Scripts.Utility.buildURL = function (baseURL, querystring) {
    var url = baseURL;
    if (url === "") {
        url = Va.Udo.Crm.Scripts.Utility.getPage().url;
    }
    if (querystring.length > 0) {
        if (querystring[0] !== "?") {
            url = url + "?";
        }
        url = url + querystring;
    }
    return url;
};

Va.Udo.Crm.Scripts.Utility.getUrlParamsCache = 0;
Va.Udo.Crm.Scripts.Utility.getUrlParams = function () {
    if (Va.Udo.Crm.Scripts.Utility.getUrlParamsCache === 0) {
        var result = {};
        var search = Va.Udo.Crm.Scripts.Utility.getPage().search;
        var pArr = search.substr(1).split("&");

        for (var i = 0; i < pArr.length; i++) {
            var p = pArr[i].split('=');
            if (p[0].toLowerCase() !== "data") {
                result[p[0].toLowerCase()] = decodeURIComponent(p[1]);
            } else {
                var dResult = {};
                var data = p[1];
                if (p.length > 2) {
                    data = "";
                    for (var n = 1; n < p.length; n = n + 2) {
                        data = data + p[n] + "=" + p[n + 1] + "&";
                    }
                    data = data.substring(0, data.length - 1);
                }
                var dArr = decodeURIComponent(data).split("&");
                for (var k = 0; k < dArr.length; k++) {
                    var d = dArr[k].split('=');
                    dResult[d[0].toLowerCase()] = d[1];
                }
                result["data"] = dResult;
            }
        }
        Va.Udo.Crm.Scripts.Utility.getUrlParamsCache = result;
    }
    return Va.Udo.Crm.Scripts.Utility.getUrlParamsCache;
};

Va.Udo.Crm.Scripts.Utility.getQueryParameter = function (name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(Va.Udo.Crm.Scripts.Utility.getPage().search);  //was location.search
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
};

Va.Udo.Crm.Scripts.Utility.getDataParameter = function (dataValue) {
    var dataParameter = null;
    if (dataValue !== "") {
        dataParameter = {};
        var vals = new Array();
        vals = decodeURIComponent(dataValue).split("&");
        for (var i in vals) {
            vals[i] = vals[i].replace(/\+/g, " ").split("=")
        }
        for (i in vals) {
            dataParameter[vals[i][0]] = vals[i][1];
        }
    }
    return dataParameter;
};

Va.Udo.Crm.Scripts.Utility.dateFilter = function (date) {
    var monthString;
    var rawMonth = (date.getMonth() + 1).toString();
    if (rawMonth.length === 1) {
        monthString = "0" + rawMonth;
    }
    else { monthString = rawMonth; }

    var dateString;
    var rawDate = date.getDate().toString();
    if (rawDate.length === 1) {
        dateString = "0" + rawDate;
    }
    else { dateString = rawDate; }


    var DateFilter = "datetime\'";
    DateFilter += date.getFullYear() + "-";
    DateFilter += monthString + "-";
    DateFilter += dateString;
    DateFilter += "T00:00:00Z\'";
    return DateFilter;
};

Va.Udo.Crm.Scripts.Utility.dateTimeFilter = function (date) {
    var monthString;
    var rawMonth = (date.getMonth() + 1).toString();
    if (rawMonth.length === 1) {
        monthString = "0" + rawMonth;
    }
    else { monthString = rawMonth; }

    var dateString;
    var rawDate = date.getDate().toString();
    if (rawDate.length === 1) {
        dateString = "0" + rawDate;
    }
    else { dateString = rawDate; }


    var DateFilter = "datetime\'";
    DateFilter += date.getFullYear() + "-";
    DateFilter += monthString + "-";
    DateFilter += dateString;
    DateFilter += "T" + date.getHours() + ":";
    DateFilter += date.getMinutes() + ":";
    DateFilter += date.getSeconds();
    DateFilter += "Z\'";
    return DateFilter;
};

Va.Udo.Crm.Scripts.Utility.getEntityTypeCode = function (entityName) {
    try {
        var command = new RemoteCommand("LookupService", "RetrieveTypeCode");
        command.SetParameter("entityName", entityName);
        var result = command.Execute();
        if (result.Success && typeof result.ReturnValue === "number") {
            return result.ReturnValue;
        }
        else {
            return null;
        }
    }
    catch (ex) {
        return null;
    }
};

Va.Udo.Crm.Scripts.Utility.formatTelephone = function (phone) {
    var Phone = phone;
    var ext = '';
    var result;
    if (Phone !== null) {
        if (0 !== Phone.indexOf('+')) {
            if (1 < Phone.lastIndexOf('x')) {
                ext = Phone.slice(Phone.lastIndexOf('x'));
                Phone = Phone.slice(0, Phone.lastIndexOf('x'));
            }
            Phone = Phone.replace(/[^\d]/gi, '');
            result = Phone;
            if (7 === Phone.length) {
                result = Phone.slice(0, 3) + '-' + Phone.slice(3)
            }
            if (10 === Phone.length) {
                result = '(' + Phone.slice(0, 3) + ') ' + Phone.slice(3, 6) + '-' + Phone.slice(6);
            }
            if (11 === Phone.length) {
                result = Phone.slice(0, 1) + ' (' + Phone.slice(1, 4) + ') ' + Phone.slice(4, 7) + '-' + Phone.slice(7);
            }
            if (0 < ext.length) {
                result = result + ' ' + ext;
            }
            return result;
        }
    }
    return "";
}

Va.Udo.Crm.Scripts.Utility.setIframeTabIndex = function (iframeId, prevElement, offset, attempt) {
    if (!attempt)
        attempt = 0;
    if (attempt > 5) {
        return;
    }
    if ($('#' + prevElement).attr('tabindex') !== null) {
        setTimeout(function () {
            var tabIndex = $('#' + prevElement).attr('tabindex');
            tabIndex = parseInt(tabIndex) + 1 + offset;
            $('#' + iframeId).attr('tabindex', tabIndex.toString());
        }, 200);
    }
    else {
        Va.Udo.Crm.Scripts.Utility.setIframeTabIndex(iframeId, prevElement, offset, attempt + 1);
    }
}


var _noaddress = false;
var _UserSettings = null;
var exCon = null;
var formContext = null;

parent.copyClaimantInfo = copyClaimantInfo;
//window.parent.ITFSubmit = ITFSubmit;

function onFormSave(context) {

    if (context.getEventArgs().getSaveMode() === 70) {
        context.getEventArgs().preventDefault();
        return false;
    }

    var claimantFirstName = "";
    var claimantLastName = "";
    var intenttofilestatus = "";

    exCon = context;
    formContext = exCon.getFormContext();

    var recordName = "ITF for ";

    if (formContext.getAttribute("va_claimantfirstname") !== null) {
        claimantFirstName = formContext.getAttribute("va_claimantfirstname").getValue();
    }

    if (formContext.getAttribute("va_claimantlastname") !== null) {
        claimantLastName = formContext.getAttribute("va_claimantlastname").getValue();
    }

    //if (formContext.getAttribute("va_intenttofilestatus") !== null) {
    //    intenttofilestatus = formContext.getAttribute("va_intenttofilestatus").getValue();
    //}

    recordName += claimantFirstName + ' ' + claimantLastName + ' on ';

    var currentDt = new Date();

    var mm = currentDt.getMonth() + 1;
    mm = (mm < 10) ? '0' + mm : mm;

    var dd = currentDt.getDate().toString();
    dd = (dd < 10) ? '0' + dd : dd;

    var yyyy = currentDt.getFullYear();

    recordName += mm + '/' + dd + '/' + yyyy;

    formContext.getAttribute('va_name').setValue(recordName);
    formContext.ui.setFormNotification("Current ITF Status - " + GetRecordStatus(), "INFO", "ITF");
}

function GetRecordStatus() {

    var intenttofilestatus = "";

    if (formContext.getAttribute("va_intenttofilestatus").getValue() !== null) {
        intenttofilestatus = formContext.getAttribute("va_intenttofilestatus").getValue();
    }

    if (intenttofilestatus === "") {
        intenttofilestatus = "Draft";
    }

    return intenttofilestatus;
}

function ITFSubmit() {

    formContext.ui.clearFormNotification("ITF");
    formContext.ui.clearFormNotification("ITFERROR");

    SaveITF().then(function () {

        formContext.ui.setFormNotification("The ITF process has started...", "INFO", "ITF");

        validateITF().then(function () {

            confirmITFPrompt().then(function (confirm) {

                if (confirm) {
                    ProcessITFRequest().then(function () {


                        return true;

                    }).catch(function (err) {
                        console.log(err.message);
                        exCon.getEventArgs().preventDefault();
                        return false;
                    });
                } else {

                    formContext.ui.setFormNotification("ITF has not started processing.  Click Submit Button when you are ready to process ITF", "INFO", "ITF");
                    exCon.getEventArgs().preventDefault();
                    return false;
                }

            }).catch(function (err) {
                console.log(err.message);
                exCon.getEventArgs().preventDefault();
                return false;
            });

        }).catch(function () {
            exCon.getEventArgs().preventDefault();
            return false;
        });
    });
}

function SaveITF() {

    return new Promise(function (resolve, reject) {

        var formtype = formContext.ui.getFormType();
        var isDirty = formContext.data.entity.getIsDirty();

        if (isDirty) {
            formContext.data.save();
            return resolve();
        } else {
            return resolve();
        }
    });
}

function validateITF() {

    return new Promise(function (resolve, reject) {

        var veteranPartID = formContext.getAttribute('va_participantid').getValue();
        var claimantPartID = formContext.getAttribute('va_claimantparticipantid').getValue();
        var generalBenefitTypeOption = formContext.getAttribute('va_generalbenefittype').getSelectedOption();

        if (veteranPartID === null || veteranPartID === "") {

            formContext.ui.setFormNotification("Veteran Participant ID has not been populated", "ERROR", "ITFERROR");
            return reject();

        } else if (generalBenefitTypeOption === null) {

            formContext.getControl('va_generalbenefittype').setFocus();
            formContext.ui.setFormNotification("The General Benefit Type has not been selected.", "ERROR", "ITFERROR");
            return reject();

        } else {

            var generalBenefitType = formContext.getAttribute('va_generalbenefittype').getText();

            if (((generalBenefitType === "Compensation") || (generalBenefitType === "Pension")) && (veteranPartID !== claimantPartID)) { // Compensation and Pension

                formContext.getControl('va_claimantparticipantid').setFocus();
                formContext.ui.setFormNotification("Veteran Participant ID and Claimant Participant ID must be same value for Compensation and Pension Intent to File", "ERROR", "ITFERROR");
                return reject();

            } else if ((generalBenefitType === "Survivors pension or DIC") && (veteranPartID === claimantPartID)) { // Survivor

                formContext.getControl('va_claimantparticipantid').setFocus();
                formContext.ui.setFormNotification("Veteran Participant ID and Claimant Participant ID must not be same value for Survivor Intent to File", "ERROR", "ITFERROR");
                return reject();

            } else {

                // All Good - continue
                return resolve();
            }
        }
    });
}

function confirmITFPrompt() {

    return new Promise(function (resolve, reject) {
        var msg = "Please confirm that you want to submit this Intent to File (ITF)?";
        var title = "Confirm ITF";
        var confirmOptions = { height: 200, width: 450 };
        var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
            .then(
                function (response) {
                    return resolve(response.confirmed);
                },
                function (error) {
                    return reject(error.message);
                });
    });

}

function ProcessITFRequest() {

    return new Promise(function (resolve, reject) {

        var addressTypeOption = formContext.getAttribute('va_addresstype').getSelectedOption();
        if (addressTypeOption !== null) {

            var addressType = formContext.getAttribute('va_addresstype').getText();
            var error = false;
            var addressError = "Please enter values for: \n";
            var address1 = formContext.getAttribute('va_veteranaddressline1').getValue();
            var city = formContext.getAttribute('va_veterancity').getValue();
            var state = formContext.getAttribute('va_veteranstate').getValue();
            var zip = formContext.getAttribute('va_veteranzip').getValue();
            var country = formContext.getAttribute('va_veterancountry').getValue();
            var va_mailingmilitarypostofficetypecode = formContext.getAttribute('va_mailingmilitarypostofficetypecode').getValue();
            var va_mailingmilitarypostaltypecode = formContext.getAttribute('va_mailingmilitarypostaltypecode').getValue();

            if (addressTypeOption === null) {

                country = formContext.getAttribute('va_veterancountry').getValue();
                if (country !== null && country === "USA") {
                    setOptionSetByOptionText("va_addresstype", "Domestic");
                }
            }

            if ((addressType === "Domestic")) {

                if (country === null || country === "") {
                    formContext.getAttribute('va_veterancountry').setValue("USA");
                    SetCountryList();
                }

                return resolve();

                /*
                * Commenting out the call to the VA's address validation backend service until further notice.
                * This service at the VA was disabled in the Fall of 2020.
                * */

                //var addressValidationCtxITF = new vrmContext(exCon);
                //addressValidationCtxITF.addressParameters = _UserSettings;

                //addressValidationCtxITF.user = _UserSettings;
                //var addressValidationDetail = new addressValidationITF(addressValidationCtxITF);

                //addressValidationDetail.executeRequest();

                //formContext.getAttribute('va_addressvalidationrequest').setValue(addressValidationDetail.buildSoapEnvelope());
                //formContext.getAttribute('va_addressvalidationresponse').setValue(addressValidationDetail.responseXml);

                //formContext.data.save().then(function () {

                //    CloseProgress();

                //    var $xml = $($.parseXML(addressValidationDetail.responseXml));

                //    if ($xml.find('return > status').text() === 'F') {

                //        var msg = "The address is not recognized\nDo you want to continue anyway?";
                //        var title = "Address Validation";
                //        var confirmOptions = { height: 200, width: 450 };
                //        var confirmStrings = { title: title, text: msg, confirmButtonLabel: "Yes", cancelButtonLabel: "No" };
                //        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                //            .then(
                //                function (response) {

                //                    if (!response.confirmed) {
                //                        return reject();
                //                    } else {
                //                        CallITFCustomAction().then(function () {
                //                            return resolve();
                //                        }).catch(function () {
                //                            return reject();
                //                        });
                //                    }

                //                },
                //                function (error) {
                //                    console.log(error.message);
                //                    return reject();
                //                });

                //    }
                //    else if ($xml.find('return > stateProvinceResult').text() === 'C') {

                //        formContext.ui.setFormNotification('State and ZIP Code are an invalid combination.  The state for this ZIP Code is: ' + $xml.find('return > stateProvince').text() + '. Please make necessary corrections and click Save again', "ERROR", "ITF");
                //        return reject();

                //    }
                //    else if ($xml.find('return > statusCode').text() === 'UnableToDPVConfirm' || $xml.find('return > confidence').text() !== '100') {
                //        msg = 'The address entered is not a valid format by United States Postal Service Standards.\n\nDo you want to use the format below instead?\n';

                //        var address = "";
                //        if ($xml.find('return > addressBlock1').text() !== '') address += '\n' + $xml.find('return > addressBlock1').text();
                //        if ($xml.find('return > addressBlock2').text() !== '') address += '\n' + $xml.find('return > addressBlock2').text();
                //        if ($xml.find('return > addressBlock3').text() !== '') address += '\n' + $xml.find('return > addressBlock3').text();
                //        if ($xml.find('return > addressBlock4').text() !== '') address += '\n' + $xml.find('return > addressBlock4').text();
                //        if ($xml.find('return > addressBlock5').text() !== '') address += '\n' + $xml.find('return > addressBlock5').text();
                //        if ($xml.find('return > addressBlock6').text() !== '') address += '\n' + $xml.find('return > addressBlock6').text();
                //        if ($xml.find('return > addressBlock7').text() !== '') address += '\n' + $xml.find('return > addressBlock7').text();
                //        if ($xml.find('return > addressBlock8').text() !== '') address += '\n' + $xml.find('return > addressBlock8').text();
                //        if ($xml.find('return > addressBlock9').text() !== '') address += '\n' + $xml.find('return > addressBlock9').text();

                //        msg += address;
                //        msg += '\n\nIf Yes click Cancel and update the form OR click OK to submit the form as is';

                //        title = "Invalid Address Format";
                //        confirmOptions = { height: 400, width: 450 };
                //        confirmStrings = { title: title, text: msg, confirmButtonLabel: "OK", cancelButtonLabel: "Cancel" };
                //        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                //            .then(
                //                function (response) {

                //                    if (!response.confirmed) {
                //                        formContext.ui.setFormNotification('Modify Claimant Information Address Details as suggested and submit again.\r\n' + address, "ERROR", "ITF");

                //                        return reject();
                //                    } else {
                //                        CallITFCustomAction().then(function () {
                //                            return resolve();
                //                        }).catch(function () {
                //                            return reject();
                //                        });
                //                    }
                //                },
                //                function (error) {
                //                    console.log(error.message);
                //                    return reject();
                //                });

                //    }
                //    else {
                //        console.debug("[LOGINFO] Inside default else. Calling CallITFCustomAction. Address Validation Response:: Confidence:" + $xml.find('return > confidence'));
                //        CallITFCustomAction().then(function () {
                //            return resolve();
                //        }).catch(function () {
                //            return reject();
                //        });
                //    }

                //});

            }
            else if (addressType === "International") {
                var country = formContext.getAttribute('va_veterancountry').getValue();

                if (country === null || country === "") {

                    formContext.ui.setFormNotification('You must provide a value for Country', "ERROR", "ITF");
                    return reject();
                }
                else {
                    CallITFCustomAction().then(function () {
                        return resolve();
                    }).catch(function () {
                        return reject();
                    });
                }
            }
            else if ((addressType === "Overseas Military")) {

                CallITFCustomAction().then(function () {
                    return resolve();
                }).catch(function () {
                    return reject();
                });
            }

            else if (addressType === "Use CORP Mailing Address") {
                //error = true;
                //addressError = "Warning: incomplete or no address available from CORP DB";
                if (_noaddress) {

                    formContext.ui.setFormNotification("Incomplete or no address available from CORP DB", "ERROR", "ITF");
                    return reject();
                }
                else {
                    CallITFCustomAction().then(function () {
                        return resolve();
                    }).catch(function () {
                        return reject();
                    });
                }

            }
            else {

                CallITFCustomAction().then(function () {
                    return resolve();
                }).catch(function () {
                    return reject();
                });

                //addressType = formContext.getAttribute('va_addresstype').getText();

            }
        }
    });

}

function CallITFCustomAction() {

    return new Promise(function (resolve, reject) {

        formContext.ui.setFormNotification("Submitting ITF request. Please wait...", "INFO", "ITF");

        try {
          

            var parameters = {};
            var entity = {};
            entity.id = formContext.data.entity.getId().replace("{", "").replace("}", "");
            entity.entityType = "va_intenttofile";
            parameters.entity = entity;
            var parententityreference = {};
            parententityreference.va_intenttofileid = entity.id; //Delete if creating new record 
            parententityreference["@odata.type"] = "Microsoft.Dynamics.CRM.va_intenttofile";
            parameters.ParentEntityReference = parententityreference;

            var udo_SubmitITFRequest = {
                entity: parameters.entity,
                ParentEntityReference: parameters.ParentEntityReference,

                getMetadata: function () {
                    return {
                        boundParameter: "entity",
                        parameterTypes: {
                            "entity": {
                                "typeName": "mscrm.va_intenttofile",
                                "structuralProperty": 5
                            },
                            "ParentEntityReference": {
                                "typeName": "mscrm.crmbaseentity",
                                "structuralProperty": 5
                            }
                        },
                        operationType: 0,
                        operationName: "udo_SubmitITF"
                    };
                }
            };

            return Xrm.WebApi.online.execute(udo_SubmitITFRequest).then(
                function success(result) {
                    if (result.ok) {
                        result.json().then(function (response) {
                            formContext.data.refresh(false).then(function () {
                                formContext.ui.setFormNotification("ITF Returned.  Status Code - " + response.ITFStatusCode, "INFO", "ITF");

                                Va.Udo.Crm.MapdNote.Initialize(exCon);
                                Va.Udo.Crm.MapdNote.PromptQuestion().then(function (createNote) {
                                    if (createNote) {
                                        Va.Udo.Crm.MapdNote.CreateITFNote().then(
                                            function () {
                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFO");
                                                return resolve();
                                            }).catch(
                                                function (err) {
                                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                    return resolve();
                                                });
                                    } else {
                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                        return resolve();
                                    }

                                });

                            });


                        });
                    }
                },
                function (error) {
                    formContext.ui.setFormNotification(response.ResponseMessage, "ERROR", "ITF");
                    return reject();
                }
            );

        }
        catch (error) {
            formContext.ui.setFormNotification("Error occurred during ITF Submit", "ERROR", "ITF");
            return reject();
        }

    });

}

function setOptionSetByOptionText(optionsetAttribute, optionText) {
    var options = formContext.getAttribute(optionsetAttribute).getOptions();
    var i;
    for (i = 0; i < options.length; i++) {
        if (options[i].text === optionText) formContext.getAttribute(optionsetAttribute).setValue(options[i].value);
    }
}

function onFormLoad(execCon) {
    var propertiesAppInsights = {
        "method": "Va.Udo.Crm.Scripts.ITF.OnFormLoad", "description": "Called on load of UDO ITF"
    };
    startTrackEvent("UDO ITF onLoad", propertiesAppInsights);
    exCon = execCon;
    formContext = exCon.getFormContext();

    environmentConfigurations.initalize(exCon).then(function () {

        commonFunctions.initalize(exCon);
        ws.shareStandardData.initalize(exCon);
        ws.intentToFile.initalize(exCon);

        var vetSearchCtx = new vrmContext(exCon);
        GetUserSettingsForWebService().then(function (userData) {

            _UserSettings = userData;
            vetSearchCtx.user = _UserSettings;
            vetSearchCtx.parameters['fileNumber'] = formContext.getAttribute('va_veteranfilenumber').getValue();
            vetSearchCtx.parameters['ptcpntId'] = formContext.getAttribute('va_participantid').getValue();


            SetCountryList();
            //CloseProgress();


            var addressTypeOption = formContext.getAttribute('va_addresstype').getSelectedOption();
            var address1 = formContext.getAttribute('va_veteranaddressline1').getValue();

            var stateCode = formContext.getAttribute('statecode').getText();

            if (stateCode !== "Inactive" && formContext.getAttribute("va_addresstype").getText() !== "Use CORP Mailing Address") {
                if (address1 === null || address1 === "") {
                    _noaddress = true;
                    UDO.Shared.openAlertDialog("Warning: incomplete or no address available from CORP DB");
                }
            }

            var myState = formContext.getAttribute('va_veteranstate').getValue();
            var myCity = formContext.getAttribute('va_veterancity').getValue();

            var militaryPostalTypeCodeValue = formContext.getAttribute('va_militarypostalcodevalue').getValue();
            var militaryPostOfficeTypeCodeValue = formContext.getAttribute('va_militarypostofficetypecodevalue').getValue();
            if ((militaryPostalTypeCodeValue !== null || militaryPostalTypeCodeValue !== "") &&
                (militaryPostOfficeTypeCodeValue !== null && militaryPostOfficeTypeCodeValue !== ""
                    || (addressTypeOption !== null && formContext.getAttribute('va_addresstype').getText() === "Overseas Military"))) {

                setOptionSetByOptionText("va_addresstype", "Overseas Military");
                setMailingAddressVisible(false);
                setOptionSetByOptionText("va_mailingmilitarypostaltypecode", militaryPostalTypeCodeValue);
                setOptionSetByOptionText("va_mailingmilitarypostofficetypecode", militaryPostOfficeTypeCodeValue);
                formContext.getAttribute("va_veterancity").setValue(militaryPostOfficeTypeCodeValue);
                formContext.getAttribute("va_veteranstate").setValue(militaryPostalTypeCodeValue);

            } else {
                var country = formContext.getAttribute('va_veterancountry').getValue();
                if (country !== null && country === "USA") {
                    setOptionSetByOptionText("va_addresstype", "Domestic");
                }
                if (country !== null && country !== "USA") {
                    setOptionSetByOptionText("va_addresstype", "International");
                }
                if (stateCode === 'Inactive' && address1 === null) {
                    setOptionSetByOptionText("va_addresstype", "Use CORP Mailing Address");
                }
                formContext.getAttribute("va_veterancountrylist").addOnChange(onChange_VeteranCountryList);
                formContext.getAttribute("va_mailingmilitarypostaltypecode").addOnChange(checkAddressType);
                formContext.getAttribute("va_mailingmilitarypostofficetypecode").addOnChange(checkAddressType);
                formContext.getAttribute("va_addresstype").addOnChange(checkAddressType);
                setMailingAddressVisible(true);
            }

            checkAddressType();

            formContext.ui.setFormNotification("Current ITF Status - " + GetRecordStatus(), "INFO", "ITF");
        });
    });

    var wrControl1 = formContext.getControl("WebResource_udo_itf_copyveteraninfo");
    if (wrControl1 !== 'undefined' && wrControl1 !== null) {
        wrControl1.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext123(exCon);
            }
        )
    }
    if (formContext.getAttribute("statecode").getValue() === 1) {
        hideReadOnly();
    }
       
    stopTrackEvent("UDO ITF onLoad", propertiesAppInsights);
}

function setMailingAddressVisible(isVisible) {
    var gentab = formContext.ui.tabs.get('GeneralTab');

    if (isVisible) {
        gentab.sections.get("MailingAddress").setVisible(true);
        gentab.sections.get("MilitaryAddress").setVisible(false);
    }
    else {
        gentab.sections.get("MailingAddress").setVisible(false);
        gentab.sections.get("MilitaryAddress").setVisible(true);
    }

    formContext.getControl("va_veteranzip").setVisible(true);

    var addressTypeOption = formContext.getAttribute("va_addresstype").getSelectedOption();
    if (addressTypeOption !== null) {
        var addressType = formContext.getAttribute("va_addresstype").getText();
        if (addressType === "International") {
            formContext.getAttribute("va_veteranstate").setValue(null);
            formContext.getAttribute("va_veteranzip").setValue(null);
            formContext.getControl('va_veteranstate').setVisible(false);
            formContext.getControl('va_veteranzip').setVisible(false);
            formContext.getAttribute('va_mailingmilitarypostaltypecode').setValue(null);
            formContext.getAttribute('va_mailingmilitarypostofficetypecode').setValue(null);
            formContext.getControl('va_veterancountrylist').setVisible(true);
        }
        else {
            if (addressType !== "Use CORP Mailing Address") {
                formContext.getControl('va_veterancountrylist').setVisible(true);
                formContext.getAttribute('va_veterancountry').setValue("USA");
                SetCountryList();
            }
            if (addressType !== "Overseas Military") {
                formContext.getControl('va_veteranstate').setVisible(true);
                formContext.getControl('va_veteranzip').setVisible(true);
                formContext.getAttribute('va_mailingmilitarypostaltypecode').setValue(null);
                formContext.getAttribute('va_mailingmilitarypostofficetypecode').setValue(null);
                formContext.getControl('va_veterancountrylist').setVisible(true);
            }

            if (addressType === "Overseas Military") {
                formContext.getControl('va_veterancountrylist').setVisible(false);
                formContext.getAttribute("va_veterancountry").setValue(null);
            }
        }
    }
}

function checkAddressType() {
    var addressTypeOption = formContext.getAttribute("va_addresstype").getSelectedOption();
    if (addressTypeOption !== null) {
        var addressType = formContext.getAttribute("va_addresstype").getText();

        formContext.getAttribute("va_veteranaddressline1").setRequiredLevel("none");
        formContext.getAttribute("va_veterancity").setRequiredLevel("none");
        formContext.getAttribute("va_veteranstate").setRequiredLevel("none");
        formContext.getAttribute("va_veteranzip").setRequiredLevel("none");
        formContext.getAttribute("va_mailingmilitarypostofficetypecode").setRequiredLevel("none");
        formContext.getAttribute("va_mailingmilitarypostaltypecode").setRequiredLevel("none");

        if (addressType === 'Domestic') {
            formContext.getAttribute('va_veteranaddressline1').setRequiredLevel('required');
            formContext.getAttribute('va_veterancity').setRequiredLevel('required');
            formContext.getAttribute('va_veteranstate').setRequiredLevel('required');
            formContext.getAttribute('va_veteranzip').setRequiredLevel('required');
            formContext.getControl('va_veterancountrylist').setVisible(true);
            formContext.getAttribute('va_veterancountrylist').setValue(100000237);
            formContext.getAttribute('va_veterancountrylist').setRequiredLevel('required');

        }
        else if ((addressType === "International")) {
            formContext.getAttribute('va_veteranaddressline1').setRequiredLevel('required');
            formContext.getAttribute('va_veterancity').setRequiredLevel('required');
            formContext.getControl('va_veterancountrylist').setVisible(true);
            formContext.getAttribute('va_veterancountrylist').setRequiredLevel('required');


        }
        else if ((addressType === "Overseas Military")) {
            formContext.getAttribute('va_veteranaddressline1').setRequiredLevel('required');
            formContext.getAttribute('va_mailingmilitarypostofficetypecode').setRequiredLevel('required');
            formContext.getAttribute('va_mailingmilitarypostaltypecode').setRequiredLevel('required');
            formContext.getAttribute('va_veteranzip').setRequiredLevel('required');
            formContext.getControl('va_veterancountrylist').setVisible(false);
            formContext.getAttribute('va_veterancountrylist').setRequiredLevel('none');
        }

        if (addressType === "Use CORP Mailing Address") {

            formContext.getAttribute("va_veteranaddressline1").setValue(null);
            formContext.getControl('va_veteranaddressline1').setVisible(true);

            formContext.getAttribute("va_veteranaddressline2").setValue(null);
            formContext.getControl('va_veteranaddressline2').setVisible(true);

            formContext.getAttribute("va_veteranunitnumber").setValue(null);
            formContext.getControl('va_veteranunitnumber').setVisible(true);

            formContext.getAttribute("va_veterancity").setValue(null);
            formContext.getControl('va_veterancity').setVisible(true);

            formContext.getAttribute("va_veteranstate").setValue(null);
            formContext.getControl('va_veteranstate').setVisible(true);

            formContext.getAttribute("va_veteranzip").setValue(null);
            formContext.getControl('va_veteranzip').setVisible(true);

            formContext.getAttribute("va_veterancountry").setValue(null);
            formContext.getControl('va_veterancountrylist').setVisible(true);
            SetCountryList();

            formContext.getAttribute('va_mailingmilitarypostofficetypecode').setValue(null);

            formContext.getAttribute('va_mailingmilitarypostaltypecode').setValue(null);



            formContext.getControl('va_veteranaddressline1').setDisabled(true);
            formContext.getControl('va_veteranaddressline2').setDisabled(true);
            formContext.getControl('va_veteranunitnumber').setDisabled(true);
            formContext.getControl('va_veterancity').setDisabled(true);
            formContext.getControl('va_veteranstate').setDisabled(true);
            formContext.getControl('va_veteranzip').setDisabled(true);
            formContext.getControl('va_veterancountrylist').setDisabled(true);
            formContext.getControl('va_mailingmilitarypostofficetypecode').setDisabled(true);
            formContext.getControl('va_mailingmilitarypostaltypecode').setDisabled(true);

        }
        else {

            formContext.getControl('va_veteranaddressline1').setDisabled(false);
            formContext.getControl('va_veteranaddressline2').setDisabled(false);
            formContext.getControl('va_veteranunitnumber').setDisabled(false);
            formContext.getControl('va_veterancity').setDisabled(false);
            formContext.getControl('va_veteranstate').setDisabled(false);
            formContext.getControl('va_veteranzip').setDisabled(false);
            formContext.getControl('va_veterancountrylist').setDisabled(false);
            formContext.getControl('va_mailingmilitarypostofficetypecode').setDisabled(false);
            formContext.getControl('va_mailingmilitarypostaltypecode').setDisabled(false);

        }

        if (addressType === "Overseas Military") {

            setMailingAddressVisible(false);
            var militaryPostalCodeOption = formContext.getAttribute('va_mailingmilitarypostaltypecode').getSelectedOption();
            var militaryPostalOfficeTypeCodeOption = formContext.getAttribute('va_mailingmilitarypostofficetypecode').getSelectedOption();
            if (militaryPostalCodeOption !== null) {
                var militaryPostalCode = formContext.getAttribute('va_mailingmilitarypostaltypecode').getText();
                formContext.getAttribute("va_veteranstate").setValue(militaryPostalCode);
            }

            if (militaryPostalOfficeTypeCodeOption !== null) {
                var militaryPostOfficeCode = formContext.getAttribute('va_mailingmilitarypostofficetypecode').getText();
                formContext.getAttribute("va_veterancity").setValue(militaryPostOfficeCode);
            }

        }
        else {
            setMailingAddressVisible(true);
        }
    }
}

function copyClaimantInfo() {
    var va_veteranssn = formContext.getAttribute('va_veteranssn').getValue();
    var va_veteranfirstname = formContext.getAttribute('va_veteranfirstname').getValue();
    var va_veteranlastname = formContext.getAttribute('va_veteranlastname').getValue();
    var va_veteranmiddleinitial = formContext.getAttribute('va_veteranmiddleinitial').getValue();

    if (va_veteranfirstname) {
        formContext.getAttribute('va_claimantfirstname').setValue(va_veteranfirstname);
    }
    if (va_veteranlastname) {
        formContext.getAttribute('va_claimantlastname').setValue(va_veteranlastname);
    }
    if (va_veteranmiddleinitial) {
        formContext.getAttribute('va_claimantmiddleinitial').setValue(va_veteranmiddleinitial);
    }
    if (va_veteranssn) {
        formContext.getAttribute('va_claimantssn').setValue(va_veteranssn);
    }
    return false;
}



//function GetCountryList(vetSearchCtx) {

//    var getCountryList = new findCountries(vetSearchCtx);
//    getCountryList.executeRequest();

//    CountryList_xmlObject = _XML_UTIL.parseXmlObject(getCountryList.responseXml);
//    returnNode = CountryList_xmlObject.selectNodes('//return');
//    CountryListNodes = returnNode[0].childNodes;
//    var oOption = document.createElement("option");

//    if (CountryListNodes) {
//        for (var i = 0; i < CountryListNodes.length; i++) {         //looping through countries and
//            if (CountryListNodes[i].nodeName === 'types') {  //making sure we dont check irrelevant nodes

//                oOption.value = parseInt(CountryListNodes[i].selectSingleNode('code').text);
//                oOption.text = CountryListNodes[i].selectSingleNode('name').text;

//                if (oOption.text === 'Israel (Jerusalem)' || oOption.text === 'Turkey (Adana only)' || oOption.text === 'Philippines (restricted payments)') {
//                    continue;
//                }
//                else if (oOption.text === 'Israel (Tel Aviv)') {
//                    oOption.text = 'Israel';
//                }
//                else if (oOption.text === 'Turkey (except Adana)') {
//                    oOption.text = 'Turkey';
//                }

//                formContext.getControl('va_veterancountrylist').addOption(oOption);
//            }
//        }
//    }
//}

function SetCountryList() {

    var countryText;
    countryText = formContext.getAttribute("va_veterancountry").getValue();
    if (countryText !== null && countryText === "USA") {
        countryText = "USA";
    }
   
}

function onChange_VeteranCountryList() {
    var country = formContext.getAttribute("va_veterancountrylist").getSelectedOption();
    if (country !== null && country.text) {
        if (country.text === "United States") {
            formContext.getAttribute("va_veterancountry").setValue("USA");
        }
        else {
            formContext.getAttribute("va_veterancountry").setValue(country.text);
        }
    }
    else {
        formContext.getAttribute("va_veterancountry").setValue(null);
    }
    SetCountryList();
}

function ITFClose() {

    if (formContext.data.entity.getIsDirty()) {

        if (UDOCheckMandatoryFields()) {
            formContext.data.save().then(function () {
                window.open("http://close/");
            });

        } else {
                UDO.Shared.openConfirmDialog("Are you sure you want to close this form?")
                    .then(
                        function (response) {
                            if (response.confirmed) {
                                UDOSetRequiredLevelOnAllRequiredFields();
                                formContext.data.save().then(function () {
                                    window.open("http://close/");
                                });
                            }
                            return resolve();
                        },
                        function (error) {
                            return reject();
                        });
        }
    }
    else {
        window.open("http://close/");
    }
}

function UDOCheckMandatoryFields() {
    var populated = true;
    formContext.getAttribute(function (attribute, index) {
        if (attribute.getRequiredLevel() === "required") {
            if (attribute.getValue() === null) {
                populated = false;
            }
        }
    });
    return populated;
}

function UDOSetRequiredLevelOnAllRequiredFields() {
    formContext.getAttribute(function (attribute, index) {
        if (attribute.getRequiredLevel() === "required") {
            if (attribute.getValue() === null) {
                var myName = attribute.getName();
                formContext.getAttribute(myName).setRequiredLevel("none");
            }
        }
    });
}


function hideReadOnly() {
    if (formContext.getAttribute("statecode").getValue() === 1) {
        var customStyle = '#warningNotification > div { display: none }';
        customStyle += ' div[role="alert"] > li { display: none }';
        var css = document.createElement('style');
        css.type = 'text/css';
        css.innerHTML = customStyle;
        parent.document.head.appendChild(css);
    }
}
