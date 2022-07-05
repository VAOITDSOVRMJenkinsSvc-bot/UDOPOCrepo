"use strict";

var _loading = true;
var _letterGenerationThroughCode = null;

var _isSystemAdmin = false;
var CRM_FORM_TYPE_CREATE = 1;
var CRM_FORM_TYPE_UPDATE = 2;
var sourceType = null;
var source = "";
var awardRequired = false;
var claimRequired = false;
var paymentRequired = false;
var ssrsReportName = '';
var letterName = '';
var reportId = '';
var formTabstoOpen = '';
var tmpLetterDisplay = '';
var exCon = null;
var formContext = null;
var globeCon = null;
var webApi = null;
var secLib = null;
var formLib = null;
//window.parent.PopulateEnclosures = PopulateEnclosures;
parent.PopulateEnclosures = PopulateEnclosures;
//window.parent.GenerateLetter = GenerateLetter;
parent.GenerateLetter = GenerateLetter;
//window.parent.fnMP = fnMP;
parent.fnMP = fnMP;
function OnLoad(execCon) {
    exCon = execCon;
    formContext = exCon.getFormContext();
    globeCon = Xrm.Utility.getGlobalContext();
    var version = globeCon.getVersion();
    var lib = new CrmCommonJS.CrmCommon(version, exCon);
    webApi = lib.WebApi;
    secLib = lib.Security;
    formLib = lib.FormHelper;
    var wrControl = formContext.getControl("WebResource_PopulateEnclosures");
    if (wrControl !== 'undefined' && wrControl !== null) {
        wrControl.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext123(exCon);
            }
        )
    }
    var wrControl1 = formContext.getControl("WebResource_GenerateAndOpenLetter");
    if (wrControl1 !== 'undefined' && wrControl1 !== null) {
        wrControl1.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext123(exCon);
            }
        )
    }
    var wrControl2 = formContext.getControl("WebResource_DownloadPDF");
    if (wrControl2 !== 'undefined' && wrControl2 !== null) {
        wrControl2.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext123(exCon);
            }
        )
    }

    var wrControl3 = formContext.getControl("WebResource_FilterPayments");
    if (wrControl3 !== 'undefined' && wrControl3 !== null) {
        wrControl3.getContentWindow().then(
            function (contentWindow) {
                contentWindow.setContext123(exCon);
            }
        )
    }
    onFormLoad();
}

function FetchContext() {
    return exCon;
}

function onFormLoad() {
    reports.getReports();
    
    if (secLib.UserHasRole("System Administrator")) {
        _isSystemAdmin = true;
       
        formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get("tab_5_section_1").setVisible(true);
    }
    else {
        formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get("tab_5_section_1").setVisible(false);
    }

    if (formContext.ui.getFormType() === CRM_FORM_TYPE_CREATE || formContext.ui.getFormType() === CRM_FORM_TYPE_UPDATE) {
        if (formContext.getAttribute('udo_pcrofrecordid').getValue() === null) {
            var setUserName = new Array();
            setUserName[0] = new Object();
            setUserName[0].id = globeCon.userSettings.userId;
            setUserName[0].entityType = 'systemuser';
            setUserName[0].name = globeCon.userSettings.userName;

            formContext.getAttribute("udo_pcrofrecordid").setValue(setUserName);
        }
    } else {
        formContext.getControl("udo_lettergenerationbutton").setVisible(false);
        formContext.getControl("udo_enclosuresbutton").setVisible(false);

    }

    _letterGenerationThroughCode = true;

    formContext.getControl('udo_letter').addPreSearch(function () {
        preFilterLookup();
    });

    if (formContext.getAttribute('udo_address1').getValue() === null) {
        formContext.getControl("udo_mailing_address1").setVisible(true);
        formContext.getControl("udo_mailing_address2").setVisible(true);
        formContext.getControl("udo_mailing_address3").setVisible(true);
        formContext.getControl("udo_mailing_city").setVisible(true);
        formContext.getControl("udo_mailing_state").setVisible(true);
        formContext.getControl("udo_mailing_zip").setVisible(true);
        formContext.getControl("udo_mailingcountry").setVisible(true);
    }
    else {
        formContext.getControl("udo_address1").setVisible(true);
        formContext.getControl("udo_address2").setVisible(true);
        formContext.getControl("udo_address3").setVisible(true);
        formContext.getControl("udo_city").setVisible(true);
        formContext.getControl("udo_state").setVisible(true);
        formContext.getControl("udo_zipcode").setVisible(true);
        formContext.getControl("udo_country").setVisible(true);
    }

    formContext.getAttribute('udo_quickwrite').addOnChange(QuickWriteChange);
    formContext.getAttribute('udo_letter').addOnChange(LetterChange);
    LetterChange();
    getVBMSRole();


}

//done
function ConfirmMarkAsSent() {

    return new Promise(function (resolve, reject) {
        var msg = "Would you like to mark this record as Sent?";
        var title = "Mark as Sent";
        UDO.Shared.openConfirmDialog(msg,title,200,450, "Yes", "No")
            .then(
                function (response) {
                    if (response.confirmed) {
                        _letterGenerationThroughCode = true;
                        formContext.getAttribute("udo_requeststatus").setValue(953850002); //Sent
                        formContext.data.save();
                    }
                    return resolve();
                },
                function (error) {
                    return reject();

                });
    });

    /*var dfd = $.Deferred();

   
    var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo;
    var style = Va.Udo.Crm.Scripts.Popup.PopupStyles.Question;

    Va.Udo.Crm.Scripts.Popup.MsgBox(msg, buttons + style, title)
        .done(function (data) {
            _letterGenerationThroughCode = true;
            formContext.getAttribute("udo_requeststatus").setValue(953850006); //Sent
            formContext.data.save();
            setTimeout(function () {
                dfd.resolve();
            }, 2000); // resolve 2 seconds after the popup is gone.
        })
        .fail(function (data) {
            setTimeout(function () {
                dfd.reject();
            }, 2000); // reject 2 seconds after the popup is gone.
        });
    return dfd.promise();*/
}

// action:
//    open: Open the generated letter as a report
//    upload: Upload the generated letter
//    download: Download the generated letter
function GenerateLetter(param) {

    // Set defaults, formatType PDF and action to open
    var formatType = (param.data.formatType !== null && typeof param.data.formatType !== "undefined") ? param.data.formatType : "PDF";
    var action = (param.data.action !== null && typeof param.data.action !== "undefined") ? param.data.action : "open";

    action = action.toLowerCase();

    //Check to make sure form is saved before continuing.
    if (formContext.ui.getFormType() === CRM_FORM_TYPE_CREATE) {
        UDO.Shared.openAlertDialog("You have to save the form before proceeding.")
            .then(function () {
                return;
            });
    }

    formContext.getAttribute("udo_mpdisplay").setSubmitMode("always");

    var dateOpened = formContext.getAttribute('udo_dateopened').getValue();
    if (dateOpened === null) {
        formContext.getControl('udo_dateopened').setFocus();
        return;
    }

    var regionalOffice = formContext.getAttribute('udo_regionalofficeid').getValue();
    if (regionalOffice === null) {
        formContext.getControl('udo_regionalofficeid').setFocus();
        return;
    }

    var letterAddressing = formContext.getAttribute('udo_letteraddressing').getValue();
    if (letterAddressing === null) {
        formContext.getControl('udo_letteraddressing').setFocus();
        return;
    }



    if (source.length === 0) {
        UDO.Shared.openAlertDialog("Error - No Letter Source Found.");
        //  alert("Error - No Letter Source Found.");
        return;
    }
    var confirmStrings = {};
    confirmStrings.confirmButtonLabel = "Yes";
    confirmStrings.cancelButtonLabel = "No";
    var confirmOptions = { height: 200, width: 450 };
    switch (sourceType) {
        case 1: // SSRS

            if (ssrsReportName.length === 0) {
                UDO.Shared.openAlertDialog("SSRS Report Name was not specified.");
                //  alert('SSRS Report Name was not specified.');
                return;
            }

            reportId = getReportID(ssrsReportName);

            if (reportId.length === 0) {
                UDO.Shared.openAlertDialog("No Report ID Found.");
                //alert('No Report ID Found.');
                return;
            }

            Va.Udo.Crm.MapdNote.Initialize(exCon);
            switch (action) {
                case "upload":
                    // Save Form
                    formContext.data.save();
                    // Generate Report/Letter                  
                    ExecuteGenerateLetterAction(formatType, true)
                        .then(function (response) {
                            Xrm.Utility.closeProgressIndicator();
                            var data = JSON.parse(response.responseText);
                            var mimeType = data.MimeType || "";
                            var fileContents = data.Base64FileContents || "";
                            var fileName = data.FileName || "";
                            var uploaded = data.Uploaded || false;
                            var docid = (data.VBMSDocument !== null && data.VBMSDocument.id) ? data.VBMSDocument.id : null;
                            var title = "Upload Error";

                            // Va.Udo.Crm.Scripts.Popup.MsgBox(data.message,
                            //     Va.Udo.Crm.Scripts.Popup.PopupStyles.Information, "VBMS File Uploaded")
                            //Xrm.Navigation.openAlertDialog({ text: "VBMS File Uploaded" })
                            UDO.Shared.openAlertDialog("VBMS File Uploaded")
                                .then(function () {
                                    ConfirmMarkAsSent()
                                        .then(function () {
                                            Va.Udo.Crm.MapdNote.PromptQuestion()
                                                .then(function (createNote) {

                                                    if (createNote) {
                                                        Va.Udo.Crm.MapdNote.CreateNote().then(
                                                            function () {
                                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                                            },
                                                            function (err) {
                                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                            });
                                                    } else {

                                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                                    }

                                                    GenerateLetterCreated(formContext.getAttribute('udo_letter').getValue()[0].name);
                                                    // Save Form
                                                    formContext.data.save();

                                                    //GenerateLetter("download", formatType);
                                                });
                                        });
                                });
                        }).catch(function (data) {
                            var message;
                            var title;
                            var buttons;
                            Xrm.Utility.closeProgressIndicator();
                            if (data.timeout) {
                                //confirmStrings.text = data.message + "\n Would you like to download the document?";
                               // confirmStrings.title = "VBMS Upload Timeout";
                                var message = data.message + "\n Would you like to download the document?";
                                var title = "VBMS Upload Timeout";
                               // Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                                UDO.Shared.openConfirmDialog(message, title, 200,450, "Yes", "No")
                                    .then(function (data) {
                                        if (data.confirmed) {
                                            GenerateLetter({ action: "download", formatType: formatType });
                                        }
                                    });
                            }
                            //prompt and open??
                            else if (data.status !== 200) {
                                //confirmStrings.title = "Upload Error: Open Document";
                                var message = data.UploadMessage || "Letter was not generated successfully.";
                                message += "\n Would you like to open the document?";
                                var title = "Upload Error: Open Document";
                                //Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                                UDO.Shared.openConfirmDialog(message, title, 200, 450, "Yes", "No")
                                    .then(function (data) {
                                        ConfirmMarkAsSent()
                                            .then(function () {
                                                Va.Udo.Crm.MapdNote.PromptQuestion()
                                                    .then(function (createNote) {
                                                        if (createNote) {
                                                            Va.Udo.Crm.MapdNote.CreateNote().then(
                                                                function () {
                                                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                                                    GenerateLetterCallback();
                                                                }).catch(
                                                                    function (err) {
                                                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                                    });
                                                        } else {

                                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                                            GenerateLetterCallback();
                                                        }
                                                    });
                                            });
                                    });
                                return;
                            }
                            else {
                               // confirmStrings.text = data.message + "\n Would you like to open the document?";
                              //  confirmStrings.title = "Letter was not uploaded successfully.";
                                var message = data.message + "\n Would you like to open the document?";
                                var title = "Letter was not uploaded successfully.";
                                //Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                                UDO.Shared.openConfirmDialog(message, title, 200, 450, "Yes", "No" )
                                    .then(function (data) {
                                        ConfirmMarkAsSent()
                                            .then(function () {
                                                Va.Udo.Crm.MapdNote.PromptQuestion()
                                                    .then(function (createNote) {

                                                        if (createNote) {
                                                            Va.Udo.Crm.MapdNote.CreateNote().then(
                                                                function () {
                                                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                                                    GenerateLetterCallback();
                                                                },
                                                                function (err) {
                                                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                                });
                                                        } else {

                                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                                            GenerateLetterCallback();
                                                        }
                                                    });
                                            });
                                    });
                            }
                        });
                    break;
                case "download":
                    // Save Form
                    formContext.data.save();
                    // Generate Report/Letter
                    Xrm.Utility.showProgressIndicator("Generating Letter");

                    //ExecuteGenerateLetterAction(formatType)
                    //    .then(function (response) {
                    //        Xrm.Utility.closeProgressIndicator();
                    //        var data = JSON.parse(response.responseText);
                    //        var mimeType = data.MimeType || "";
                    //        var fileContents = data.Base64FileContents || "";
                    //        var fileName = data.FileName || "";

                    //        if (fileContents === null || fileContents.length === 0) {
                    //           // confirmStrings.title = "Create Failed: Open Document";
                    //         //   confirmStrings.text = data.UploadMessage || "Letter was not generated successfully.";
                    //          //  confirmStrings.text += "\n Would you like to open the document?";
                    //            var message = data.UploadMessage || "Letter was not generated successfully.";
                    //            message += "\n Would you like to open the document?";
                    //            var title = "Create Failed: Open Document";
                    //            //Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                    //            UDO.Shared.openConfirmDialog(message, title, 200, 450, "Yes", "No")
                    //                .then(function (data) {
                    //                    ConfirmMarkAsSent()
                    //                        .then(function () {
                    //                            Va.Udo.Crm.MapdNote.PromptQuestion()
                    //                                .then(function (createNote) {

                    //                                    if (createNote) {
                    //                                        Va.Udo.Crm.MapdNote.CreateNote().then(
                    //                                            function () {
                    //                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                    //                                                GenerateLetterCallback();
                    //                                            },
                    //                                            function (err) {
                    //                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                    //                                            });
                    //                                    } else {

                    //                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                    //                                        GenerateLetterCallback();
                    //                                    }

                    //                                });
                    //                        });
                    //                });
                    //            return;
                    //        }

                    //        // success - download file
                    //        Va.Udo.Crm.Scripts.Download(fileName, mimeType, fileContents)
                    //            .done(function () {
                    //                ConfirmMarkAsSent()
                    //                    .then(function () {
                    //                        Va.Udo.Crm.MapdNote.PromptQuestion()
                    //                            .then(function (createNote) {
                    //                                if (createNote) {
                    //                                    Va.Udo.Crm.MapdNote.CreateNote().then(
                    //                                        function () {
                    //                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                    //                                        },
                    //                                        function (err) {
                    //                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                    //                                        }).catch(error)(function (error) { });
                    //                                } else {

                    //                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                    //                                }

                    //                                GenerateLetterCreated(formContext.getAttribute('udo_letter').getValue()[0].name);
                    //                                // Save Form
                    //                                formContext.data.save();
                    //                            });
                    //                    });
                    //            });
                    //    }).catch(function (data) {
                    //        //prompt and open??
                    //        Xrm.Utility.closeProgressIndicator();
                    //       // confirmStrings.title = "Upload Error: Open Document";
                    //       // confirmStrings.text = data.UploadMessage || "Letter was not generated successfully.";
                    //      //  confirmStrings.text += "\n Would you like to open the document?";
                    //        var message = data.UploadMessage || "Letter was not generated successfully.";
                    //        message += "\n Would you like to open the document?";
                    //        var title = "Upload Error: Open Document";
                    //       // Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                    //        UDO.Shared.openConfirmDialog(message, title, 200, 450, "Yes", "No")
                    //            .then(function (data) {
                    //                ConfirmMarkAsSent()
                    //                    .then(function (data) {
                    //                        Va.Udo.Crm.MapdNote.PromptQuestion()
                    //                            .then(function (createNote) {
                    //                                if (createNote) {
                    //                                    Va.Udo.Crm.MapdNote.CreateNote().then(
                    //                                        function () {
                    //                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                    //                                            GenerateLetterCallback();
                    //                                        }).catch(error)(
                    //                                            function (err) {
                    //                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                    //                                            });
                    //                                } else {

                    //                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                    //                                    GenerateLetterCallback();
                    //                                }
                    //                            });
                    //                    });
                    //            });
                    //        return;
                    //    });
                    ExecuteGenerateLetterAction(formatType)
                        .then(function (result) {
                            if (result.ok) {
                                result.json().then(function (data) {
                                    Xrm.Utility.closeProgressIndicator();
                                    var mimeType = data.MimeType || "";
                                    var fileContents = data.Base64FileContents || "";
                                    var fileName = data.FileName || "";

                                    if (fileContents === null || fileContents.length === 0) {
                                        // confirmStrings.title = "Create Failed: Open Document";
                                        //   confirmStrings.text = data.UploadMessage || "Letter was not generated successfully.";
                                        //  confirmStrings.text += "\n Would you like to open the document?";
                                        var message = "Letter was not generated successfully.";
                                        message += "\n Would you like to open the document?";
                                        var title = "Create Failed: Open Document";
                                        //Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                                        UDO.Shared.openConfirmDialog(message, title, 200, 450, "Yes", "No")
                                            .then(function (data) {
                                                ConfirmMarkAsSent()
                                                    .then(function () {
                                                        Va.Udo.Crm.MapdNote.PromptQuestion()
                                                            .then(function (createNote) {

                                                                if (createNote) {
                                                                    Va.Udo.Crm.MapdNote.CreateNote().then(
                                                                        function () {
                                                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                                                            GenerateLetterCallback();
                                                                        },
                                                                        function (err) {
                                                                            Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                                        });
                                                                } else {

                                                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                                                    GenerateLetterCallback();
                                                                }

                                                            });
                                                    });
                                            });
                                        return;
                                    }

                                    // success - download file
                                    Va.Udo.Crm.Scripts.Download(fileName, mimeType, fileContents)
                                        .done(function () {
                                            ConfirmMarkAsSent()
                                                .then(function () {
                                                    Va.Udo.Crm.MapdNote.PromptQuestion()
                                                        .then(function (createNote) {
                                                            if (createNote) {
                                                                Va.Udo.Crm.MapdNote.CreateNote().then(
                                                                    function () {
                                                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                                                    },
                                                                    function (err) {
                                                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                                    }).catch(error)(function (error) { });
                                                            } else {

                                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                                            }

                                                            GenerateLetterCreated(formContext.getAttribute('udo_letter').getValue()[0].name);
                                                            // Save Form
                                                            formContext.data.save();
                                                        });
                                                });
                                        });




                                });
                            }
                        }, function (error) {
                            //prompt and open??
                            Xrm.Utility.closeProgressIndicator();
                            var message = "Letter was not generated successfully.";
                            message += "\n Would you like to open the document?";
                            var title = "Upload Error: Open Document";
                            // Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                            UDO.Shared.openConfirmDialog(message, title, 200, 450, "Yes", "No")
                                .then(function (data) {
                                    ConfirmMarkAsSent()
                                        .then(function (data) {
                                            Va.Udo.Crm.MapdNote.PromptQuestion()
                                                .then(function (createNote) {
                                                    if (createNote) {
                                                        Va.Udo.Crm.MapdNote.CreateNote().then(
                                                            function () {
                                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                                                GenerateLetterCallback();
                                                            }).catch(error)(
                                                                function (err) {
                                                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                                });
                                                    } else {

                                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                                        GenerateLetterCallback();
                                                    }
                                                });
                                        });
                                });
                            return;

                        });
                    break;
                default: // open
                    ConfirmMarkAsSent()
                        .then(function (data) {
                            Va.Udo.Crm.MapdNote.PromptQuestion()

                                .then(function (createNote) {
                                    if (createNote) {
                                        Va.Udo.Crm.MapdNote.CreateNote().then(
                                            function () {
                                                Va.Udo.Crm.MapdNote.SetFormNotification("", "A note has been created for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "INFORMATION");
                                                GenerateLetterCallback();
                                            }).catch(
                                                function (err) {
                                                    Va.Udo.Crm.MapdNote.SetFormNotification("", "An unexpected error occurred creating the note - " + err, "ERROR");
                                                });
                                    } else {

                                        Va.Udo.Crm.MapdNote.SetFormNotification("", "User decided not to create a note for this " + Va.Udo.Crm.MapdNote.TypeOfNote, "WARNING");
                                        GenerateLetterCallback();
                                    }



                                });
                        });
                    break;
            }

            //tmpLetterDisplay = Xrm.Page.getAttribute("udo_letterdisplay").getValue();

            //Xrm.Page.getAttribute("udo_letterdisplay").setValue(tmpLetterDisplay);
            break;
        default:
            title = "Error";
            msg = "The action cannot be completed for the selected type of letter.";
            //Xrm.Navigation.openAlertDialog({ title: title, text: msg });
            UDO.Shared.openAlertDialog(msg, title);
            break;
    }
}

function ExecuteGenerateLetterAction(formatType, upload) {
    if (typeof upload === "undefined" || upload === null) upload = false;

    var personReference = null;
    var personId = formContext.getAttribute('udo_personid').getValue();
    if (typeof personId !== "undefined" && personId !== null && personId.length > 0) {
        personReference = personId[0];
    } // else throw error?   
    var claimNumber = formContext.getAttribute("udo_claimnumber").getValue() !== null ? formContext.getAttribute("udo_claimnumber") : "";
    var parameters = {};
    var entity = {};
    entity.id = formContext.data.entity.getId().replace("{", "").replace("}", "");
    entity.entityType = "udo_lettergeneration";
    parameters.entity = entity;
    var report = {};
    report.id = reportId.replace("{", "").replace("}", "");
    report.entityType = "report";
    parameters.Report = report;
    parameters.ReportName = ssrsReportName;
    parameters.FormatType = formatType;
    var person = {};
    person.id = personReference.id.replace("{", "").replace("}", "");
    person.entityType = "udo_person";
    parameters.Person = person;
    parameters.ClaimNumber = claimNumber;
    parameters.SourceUrl = source;
    parameters.UploadToVBMS = upload;

    var udo_GenerateLetterRequest = {
        entity: parameters.entity,
        Report: parameters.Report,
        ReportName: parameters.ReportName,
        FormatType: parameters.FormatType,
        Person: parameters.Person,
        ClaimNumber: parameters.ClaimNumber,
        SourceUrl: parameters.SourceUrl,
        UploadToVBMS: parameters.UploadToVBMS,

        getMetadata: function () {
            return {
                boundParameter: "entity",
                parameterTypes: {
                    "entity": {
                        "typeName": "mscrm.udo_lettergeneration",
                        "structuralProperty": 5
                    },
                    "Report": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    },
                    "ReportName": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "FormatType": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "Person": {
                        "typeName": "mscrm.crmbaseentity",
                        "structuralProperty": 5
                    },
                    "ClaimNumber": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "SourceUrl": {
                        "typeName": "Edm.String",
                        "structuralProperty": 1
                    },
                    "UploadToVBMS": {
                        "typeName": "Edm.Boolean",
                        "structuralProperty": 1
                    }
                },
                operationType: 0,
                operationName: "udo_GenerateLetter"
            };
        }
    };

    if (upload) {
        Xrm.Utility.showProgressIndicator("Uploading to VBMS");
        return webApi.PromiseTimeout(150000, webApi.ExecuteRequest(udo_GenerateLetterRequest, null));
    }
    else {
        return Xrm.WebApi.online.execute(udo_GenerateLetterRequest);
    }
}

//function GenerateLetterOnSave() {
//    Xrm.Page.getAttribute("udo_letterdisplay").setValue(tmpLetterDisplay);
//    window.open(serverUrl + source.replace("{0}", reportId) + "&p:LetterGenerationGUID=" + Xrm.Page.data.entity.getId());
//}


function PopulateEnclosures() {
    var recordId = formContext.data.entity.getId();
    var enclosures = '';
    var cols = ['udo_lettergenerationid', '&$expand=udo_udo_lettergeneration_va_externaldocument($select=va_externaldocumentid, va_name, va_documentlocation)'];
    webApi.RetrieveRecord(recordId, "udo_lettergeneration", cols).then(
        function success(data) {
            if (data && data.udo_udo_lettergeneration_va_externaldocument.length > 0) {
                var externalDoc = data.udo_udo_lettergeneration_va_externaldocument;
                for (var i = 0; i < externalDoc.length; i++) {
                    var r = externalDoc[i].va_externaldocumentid;
                    enclosures += (enclosures.length > 0 ? '\n' : '') + externalDoc[i].va_name;

                    if (formContext.getAttribute('udo_enclosures').getValue() !== enclosures) {
                        formContext.getAttribute('udo_enclosures').setValue(enclosures);
                        // update data to make sure enclosures get on report
                        if (formContext.ui.getFormType() === CRM_FORM_TYPE_UPDATE) {
                            //CrmRestKit2011.Update('udo_lettergeneration', formContext.data.entity.getId(), { udo_Enclosures: 'enclosures' })
                            webApi.UpdateRecord(recordId, "udo_lettergeneration", { udo_enclosures: enclosures })
                                .then(function (data) {
                                })
                                .catch(function (err) {
                                    UTIL.restKitError(err, 'Failed to update servicerequest enclosures');
                                });
                        }
                    }
                }
            }
        }).catch(
            function error(error) {
                UTIL.restKitError(error, 'Failed to retrieve service request external document data');
            });

}
//done
function LetterChange() {

    //debugger;

    paymentRequired = false;

    var arrayLength = formTabstoOpen.length;
    if (arrayLength > 0) {
        for (var i = 0; i < arrayLength; i++) {
            //   if(formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get("Tab_119_Report_of_Contact") !== null)
            //   {
            formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get(formTabstoOpen[i]).setVisible(false);
            //    }
        }
    }

    if (formContext.getAttribute('udo_letter').getValue() !== null) {

        formContext.getAttribute("udo_name").setValue(formContext.getAttribute('udo_letter').getValue()[0].name);


        var columns = ['udo_paymentrequired', 'udo_includeenclosures', 'udo_sourcetype', 'udo_source', 'udo_ssrsreportname', 'udo_formtabstoopen', 'udo_name'];
        var letter = formContext.getAttribute("udo_letter").getValue()[0].id;
        var letterid = letter.replace("{", "").replace("}", "");
        var filter = "$filter=udo_letterid eq " + letterid + "";
        webApi.RetrieveMultiple("udo_letter", columns, filter)
            .then(function (data) {
                if (data && data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        if (data[i].udo_PaymentRequired) {
                            paymentRequired = true;
                        }

                        sourceType = data[i].udo_sourcetype;
                        source = data[i].udo_source;
                        if (data[i].udo_includeenclosures) {
                            //   formContext.ui.tabs.get("Tab_Include_Enclosures").setVisible(true);
                            formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get("tab_6_section_1").setVisible(true);
                        }
                        ssrsReportName = data[i].udo_ssrsreportname;

                        if (data[i].udo_formtabstoopen) {
                            formTabstoOpen = data[i].udo_formtabstoopen.split(",");
                        }
                        letterName = data[i].udo_name;
                    }
                }
            })
            .catch(function (err) {
                UTIL.restKitError(err, 'Failed to retrieve letter configuration data');
            });

        arrayLength = formTabstoOpen.length;
        if (arrayLength > 0) {
            var i;
            for (i = 0; i < arrayLength; i++) {
                //   if(formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get("Tab_119_Report_of_Contact") !== null)
                //        {
                formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get(formTabstoOpen[i]).setVisible(true);
                //        }
            }
        }

        if (_isSystemAdmin) {
            // formContext.ui.tabs.get("Tab_Processing_Information").setVisible(true);
            formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get("tab_5_section_1").setVisible(true);
        }
        else {
            //  formContext.ui.tabs.get("Tab_Processing_Information").setVisible(false);
            formContext.ui.tabs.get("{040d30d2-de4c-438a-9c84-7576f6b74327}").sections.get("tab_5_section_1").setVisible(false);
        }
    }
}

function GenerateLetterCallback() {
    var serverUrl = globeCon.getClientUrl();
    GenerateLetterCreated(formContext.getAttribute('udo_letter').getValue()[0].name);
    // Save Form
    formContext.data.save();
    // Open Report
    if (window.IsUSD) {
        //var reportURL = serverUrl + source.replace("{0}", reportId) + "&p:LetterGenerationGUID=" + formContext.data.entity.getId();
        var report = source.replace("{0}", reportId) + "&p:LetterGenerationGUID=" + formContext.data.entity.getId();//.replace("{", "").replace("}", "");
        // window.open("http://event/?eventname=OpenDblReport-Letters&id=" + reportId + "&LetterID=" + formContext.data.entity.getId() + "&Type=" + "3");
        window.open("http://event/?eventname=OpenDblReport-Letters&report=" + report);
        // Xrm.Navigation.openUrl(serverUrl + source.replace("{0}", reportId) + "&p:LetterGenerationGUID=" + formContext.data.entity.getId().replace("{", "").replace("}", ""));

    } else {
        //var crmBaseUrl = globalContext.getClientUrl();
        //var reportBaseUrl = "/crmreports/viewer/viewer.aspx?action=run&helpID={0}&id={1}&p:FNODID={2}";

        //var reportUrl = crmBaseUrl + reportBaseUrl.replace("{0}", helpId).replace("{1}", reportId).replace("{2}", fnodId);

        //Xrm.Navigation.openUrl(reportUrl);
        Xrm.Navigation.openUrl(serverUrl + source.replace("{0}", reportId) + "&p:LetterGenerationGUID=" + formContext.data.entity.getId());
    }
    // Xrm.Navigation.openUrl(serverUrl + source.replace("{0}", reportId) + "&p:LetterGenerationGUID=" + formContext.data.entity.getId());
}

function GenerateLetterCreated(letterName) {

    var lettersCreated = formContext.getAttribute("udo_letterscreated");
    var pcr = globeCon.userSettings.userName;
    var now = new Date();
    var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());

    letterName = letterName + ' was created by ' + pcr + ' on ' + today;

    //Xrm.Page.getAttribute("udo_name").setValue(letterName);

    if (lettersCreated.getValue() === null) {
        lettersCreated.setValue(letterName);
    }
    else {
        lettersCreated.setValue(lettersCreated.getValue() + ';\n' + letterName);
    }
}

function retrieveUserReqCallBack(retrieveUserReq) {
    if (retrieveUserReq.readyState === 4 /* complete */) {
        if (retrieveUserReq.status === 200) {

            var retrievedUser = new DOMParser().parseFromString(retrieveUserReq.responseText, 'text/xml');
            if (retrievedUser !== null && retrievedUser.d !== null && retrieveUser.d.FullName !== null) {
                return retrievedUser.d.FullName;
            }
            else {
                UDO.Shared.openAlertDialog("Error in Fetching User data", "Error");
            }
        }
    }
}

function getReportID(reportName) {
    var len = reports.list.length;
    var i;
    for (i = 0; i < len; i++) {
        if (reports.list[i].name === reportName) {
            return reports.list[i].reportId;
        }
    }

    return '';
}

var reports = {
    list: [],
    getReports: function () {
        var len, me = this;

        var cols = ["reportid", "name"];
        webApi.RetrieveMultiple("report", cols, null)
            .then(function (data) {
                if (data && data.length > 0) {
                    len = data.length;
                    var i;
                    for (i = 0; i < len; i++) {
                        me.list.push({ name: data[i].name, reportId: data[i].reportid });
                    }
                }
            }).catch(function (error) {
                throw new Error(error.message);
                // console.log("fail")
            });
    }
};

function preFilterLookup() {

    //debugger;

    var fetchVeteran = "<filter type='and'><condition attribute='statecode' operator='eq' value='0' /><condition attribute='udo_veteranonly' operator='eq' value='1' /></filter>";
    var fetchAll = "<filter type='and'><condition attribute='statecode' operator='eq' value='0' /><condition attribute='udo_availableforall' operator='eq' value='1' /></filter>";

    if (formContext.getAttribute('udo_isveteran').getValue()) {
        formContext.getControl('udo_letter').addCustomFilter(fetchVeteran);
    }
    else {
        formContext.getControl('udo_letter').addCustomFilter(fetchAll);
    }

}

//Multiple Payment functions
//MP Main function
function fnMP() {
    fnMPCheckAll();
    fnMPdate();
    if (formContext.getAttribute("udo_mpend").getValue() < formContext.getAttribute("udo_mpstart").getValue()) {
        //clear out the display
        var Display = 'Invalid date range. Please verify that the start date is before the end date.';
        formContext.getAttribute("udo_mpdisplay").setValue(Display);
        formContext.getAttribute("udo_letterdisplay").setValue(Display);
    }
    else {
        fnMPsort();
    }
}

//MP checkbox function
function fnMPCheckAll() {
    if (formContext.getAttribute("udo_mpall").getValue() === true) {
        formContext.getControl("udo_mpstart").setVisible(false);
        formContext.getControl("udo_mpend").setVisible(false);
        fnMPsort();
    }
    else {
        formContext.getControl("udo_mpstart").setVisible(true);
        formContext.getControl("udo_mpend").setVisible(true);
    }
}

//MP default dates if nothing is entered(6 months ago - today)
function fnMPdate() {
    var Today = new Date();
    //If no start, yes end
    if ((formContext.getAttribute("udo_mpstart").getValue() === null) && (formContext.getAttribute("udo_mpend").getValue() !== null)) {
        var Add = new Date(formContext.getAttribute("udo_mpend").getValue());
        Add.setDate(1);
        Add.setMonth(varAdd.getMonth() - 6);
        formContext.getAttribute("udo_mpstart").setValue(Add);
    }
    //If no start, no end (catch)
    if (formContext.getAttribute("udo_mpstart").getValue() === null) {
        var Begin = new Date();
        Begin.setDate(1);
        Begin.setMonth(Today.getMonth() - 6);
        formContext.getAttribute("udo_mpstart").setValue(Begin);
    }
    //If no end
    if (formContext.getAttribute("udo_mpend").getValue() === null) {
        formContext.getAttribute("udo_mpend").setValue(Today);
    }
    //Validate start is before end
    if (formContext.getAttribute("udo_mpend").getValue() < formContext.getAttribute("udo_mpstart").getValue()) {
        varValid = new Date(formContext.getAttribute("udo_mpend").getValue());
        UDO.Shared.openAlertDialog("Invalid date range. Please verify that the start date is before the end date.");
        // alert('Invalid date range. Please verify that the start date is before the end date.');
    }
}

//MP need to sort through the data now.
function fnMPsort() {
    var Begin = new Date(formContext.getAttribute("udo_mpstart").getValue()).setHours(0);
    var End = new Date(formContext.getAttribute("udo_mpend").getValue()).setHours(0);
    //varEnd.setHours(0);
    var Display = "";
    var Display2 = "";
    var FormatDate = null;
    var FormatPayment = null;

    if (formContext.getAttribute("udo_mpraw").getValue() !== null) {
        //var SampleString = "1/1/2011_$101.00,2/1/2011_$201.00,3/1/2011_$301.00,3/23/2011_$323.00,4/1/2011_$401.00,5/1/2011_$501.00";
        var SampleString = new String(formContext.getAttribute("udo_mpraw").getValue());
        if (SampleString.substring(0, 9) === "undefined") {
            SampleString = SampleString.substring(9, SampleString.length);
        }

        var SampleString2 = SampleString.substring(0, SampleString.length - 1);
        //set array to the data to be assigned
        var checkArray = SampleString2.split(";");
        //split each selected check into [payment date, amount] array
        var checkArray2d = new Array();
        for (var i = 0; i < checkArray.length; i++) {
            checkArray2d[i] = checkArray[i].split("_");
        }

        //Filter if "all" check is not true
        if (formContext.getAttribute("udo_mpall").getValue() === false) {
            var i;
            for (i = 0; i < checkArray2d.length; i++) {
                var CurrPayDate = new Date(checkArray2d[i][0]).setHours(0);

                if ((CurrPayDate >= Begin) && (CurrPayDate <= End)) {
                    FormatDate = mpFormatDate(CurrPayDate);
                    FormatPayment = checkArray2d[i][1].replace(",", "");
                    Display += FormatDate + "\t\t $" + parseFloat(FormatPayment).toFixed(2) + "\n";
                    Display2 += FormatDate + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; $" + parseFloat(FormatPayment).toFixed(2) + "<br/>";
                }
            }
        }
        else {
            var i;
            for (i = 0; i < checkArray2d.length; i++) {
                FormatDate = mpFormatDate(checkArray2d[i][0]);
                FormatPayment = checkArray2d[i][1].replace(",", "");
                Display += FormatDate + "\t\t $" + parseFloat(FormatPayment).toFixed(2) + "\n";
                Display2 += FormatDate + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; $" + parseFloat(FormatPayment).toFixed(2) + "<br/>";
            }
        }
    }
    else {
        Display += "No Payment History on the Letter Generation record to search.";
        Display2 += "No Payment History on the Letter Generation record to search.";
    }

    formContext.getAttribute("udo_letterdisplay").setValue(Display);
    formContext.getAttribute("udo_mpdisplay").setValue(Display2);

    //updateLetterGeneration();
}

function mpFormatDate(varDate) {
    var varInput = new Date(varDate);
    var varFormat = "";
    var varM = varInput.getMonth() + 1;
    if (varM < 10) {
        varFormat = "0" + varM + "/";
    }
    else {
        varFormat = "" + varM + "/";
    }
    var varD = varInput.getDate();
    if (varD < 10) {
        varFormat += "0" + varD + "/";
    }
    else {
        varFormat += varD + "/";
    }
    var varY = varInput.getFullYear();
    varFormat += varY;
    return varFormat;
}

function GetLookupId(lookupAttributeName) {
    var id = null;
    if (formContext.getAttribute(lookupAttributeName).getValue() &&
        formContext.getAttribute(lookupAttributeName).getValue().length > 0 &&
        formContext.getAttribute(lookupAttributeName).getValue()[0]) {
        id = formContext.getAttribute(lookupAttributeName).getValue()[0].id;
    }

    return id;
}

function QuickWriteChange() {
    var id = GetLookupId('udo_quickwrite');
    if (!id) return;
    var descAttribute = formContext.getAttribute('udo_description');

    var columns = ['va_quickwritetext'];
    webApi.RetrieveRecord(id, "va_quickwrite", columns)
        .then(function (data) {
            if (data) {
                var qw = data.va_quickwritetext;
                var val = descAttribute.getValue();

                // process substition tokens
                // each one looks like <!udo_ssn!> or <!va_ssn!>
                qw = ReplaceFieldTokens(qw);
                if (!qw) qw = '';

                descAttribute.setValue((val ? val + ' \n' + qw : qw));
                formContext.getAttribute('udo_quickwrite').setValue(null);
                formContext.getControl('udo_description').setFocus();

                getManager();
            }
        })
        .catch(function (err) {
            HandleRestError(err, 'Failed to retrieve text.');
        });
}


// process substition tokens
// each one looks like <!va_ssn!> or <!udo_ssn!>
// will take input abe<!va_ssn!>asdf and replace with abe555667777asdf
function ReplaceFieldTokens(qw) {
    if (!qw || qw.length === 0) return qw;

    var op = '<!', po = '!>', pos = qw.indexOf(op);
    while (pos !== -1) {
        var pos2 = qw.indexOf(po);
        if (pos2 === -1) break; // cannot find closing tag
        var token = qw.substring(pos + po.length, pos2); // token without tags
        var attr = formContext.getAttribute(token.replace("va_", "udo_"));
        var attrVal = '';
        if (attr && attr.getValue()) {
            switch (attr.getAttributeType()) {
                case 'datetime':
                    attrVal = attr.getValue().format("MM/dd/yyyy");
                    break;
                case 'lookup':
                    attrVal = attr.getValue()[0].name;
                    break;
                case 'optionset':
                    attrVal = attr.getText();
                    break;
                default:
                    attrVal = attr.getValue().toString();
            }
        }
        qw = qw.replace(new RegExp(op + token + po, 'g'), attrVal);

        pos = qw.indexOf(op);
    }
    return qw;
}

function getManager() {
    var pcrOfRecordId = formContext.getAttribute('udo_pcrofrecordid').getValue()[0].id;
    pcrOfRecordId = pcrOfRecordId.replace("{", "").replace("}", "");
    var columns = ['parentsystemuserid'];
    var filter = "$filter=systemuserid eq " + pcrOfRecordId + "";
    webApi.RetrieveMultiple("systemuser", columns, filter)
        .then(function (data) {
            if (data && data.length === 1) {
                var parentSystemUserId = data[0].parentsystemuserid.id;

                if (parentSystemUserId === null) {
                    UDO.Shared.openAlertDialog("It has been detected that you do not have a manager assigned. It is recommended a manager is assigned to you.");
                    //  alert('It has been detected that you do not have a manager assigned. It is recommended a manager is assigned to you.');

                    var description = formContext.getAttribute('udo_description');
                    description.setValue(description.getValue().replace('(Signature of NCCM)', ''));

                    return;
                }
                parentSystemUserId = parentSystemUserId.replace("{", "").replace("}", "");
                columns = ['firstname', 'lastname'];
                filter = "$filter=systemuserid eq" + parentSystemUserId + "";
                webApi.RetrieveMultiple("systemuser", columns, filter)
                    .then(function (data) {
                        if (data && data.length === 1) {
                            manager = data[0].firstname + ' ' + data[0].lastname;

                            var description = formContext.getAttribute('udo_description');
                            description.setValue(description.getValue().replace('(Signature of NCCM)', 'Sincerely,<br/>' + manager));
                        }
                    })
                    .catch(function (error) { throw new Error(error); });
            }
        })
        .catch(function (err) { HandleRestError(err, 'Failed to retrieve manager data'); });

}

function getVBMSRole() {

    var currentUserId = globeCon.userSettings.userId;
    var vbmsUploadrole;
    webApi.RetrieveRecord(currentUserId, "systemuser", ['udo_vbmsuploadrole'])
        .then(function (data) {
            vbmsUploadrole = data.udo_vbmsuploadrole;

            if (vbmsUploadrole !== null) {
                formContext.getAttribute("udo_vbmsuploadrole").setValue(vbmsUploadrole.Value);
            }

        })
        .catch(function (err) {
            HandleRestError(err, 'Failed to retrieve VBMS Upload Role data');
        });

}

function letterGenerationCancel() {
    return new Promise(function (resolve, reject) {
        UDO.Shared.openConfirmDialog("Do you want to cancel the Letter Generation?")
            .then(
                function (response) {
                    if (response.confirmed) {
                        var action = formContext.getAttribute("udo_requeststatus").setValue(953850007);
                        formContext.data.save();
                    }
                    return resolve();
                },
                function (error) {
                    return reject();
                }
            );
    });
}



function UDOclosePage() {
    if (formContext.data.entity.getIsDirty()) {

        if (UDOCheckMandatoryFields()) {
            formContext.data.save().then(function () {
                window.open("http://close/");
            });

        } else {
            return new Promise(function (resolve, reject) {
                var msg = "Are you sure you want to close this form?";
                var title = "Close Form";

                UDO.Shared.openConfirmDialog(msg, title, 200, 450, "OK", "Cancel")
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
            });
        }
    }
    else {
        formContext.data.save().then(function () {
            window.open("http://close/");
        }, function () {
            return new Promise(function (resolve, reject) {
                var msg = "Are you sure you want to close this form?";
                var title = "Close Form";
              //  var confirmOptions = { height: 200, width: 450 };
              //  var confirmStrings = { title: title, text: msg, confirmButtonLabel: "OK", cancelButtonLabel: "Cancel" };
              //  Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions)
                UDO.Shared.openConfirmDialog(msg, title, 200, 450, "OK", "Cancel")
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
            });
        });
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

function CheckDocType(context) {
    var doctype = context.getAttribute("udo_vbmsdoctype").getValue();
    //var docid = getControlAttrValue("udo_vbmsdoctype");

    if (doctype  !== null) {
    //if (docid !== null) {

        //if(frames.customScriptsFrame)
        //  {
        //   frames.customScriptsFrame.eval("Va.Udo.Crm.Scripts.VBMS.OpenVBMSDialog('','','','','')");
        //  }       
        Va.Udo.Crm.Scripts.VBMS.OpenVBMSDialog('', '', '', '', '');
    }
    else {
        UDO.Shared.openAlertDialog("You must select a valid Document Type, before continuing to upload to VBMS");
        //  alert("You must select a valid Document Type, before continuing to upload to VBMS");
        formContext.getControl("udo_vbmsdoctype").setFocus();
    }

}

function letterGenerationSave(context) {
    formContext.data.save();
}


function getControlAttrValue(control) {

    var doctype1 = formContext.getControl(control);
    var test = doctype1.getAttribute(control);
    var test1 = test.getValue();
    if (test1 !== null) {
        if (test1[0].id !== null) {
            return test1[0].id;
        }
    }
    return test1;
}


function openLetterUSD() {
    if (frames.customScriptsFrame) {
        frames.customScriptsFrame.eval("GenerateLetter('open')");
    }   
}

function letterEnclosuresUSD() {
    if (frames.customScriptsFrame) {
        frames.customScriptsFrame.eval("PopulateEnclosures()");
    }
}


function previewPDFLetter(context, download) {
    GenerateLetter(download); 
}


function openVBMSLetterGenerationfromUSD(context,udofilenumber, firstname, lastname, ssn ) {
    if (frames.customScriptsFrame) {
        frames.customScriptsFrame.eval("Va.Udo.Crm.Scripts.VBMS.OpenVBMSDialog('" + udofilenumber + "','" + firstname + "','','" + lastname + "', '" + ssn + "')");
    }
}