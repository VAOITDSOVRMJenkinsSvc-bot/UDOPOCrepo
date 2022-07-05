//Not used
function GenerateDocument(s) {
	function GenerateDocumentCreated(name) {
		var logfield = Xrm.Page.getAttribute("udo_description");
		var pcr = Getinfo();
		var now = new Date();
		var today = (now.getMonth() + 1) + '/' + now.getDate() + '/' + now.getFullYear() + ' ' + now.getHours() + ':' + ((now.getMinutes() < 10 ? '0' : '') + now.getMinutes());

		name += ' was created by ' + pcr + ' on ' + today;

		//Xrm.Page.getAttribute("udo_name").setValue(letterName);

		if (logfield.getValue() == null) {
			logfield.setValue(letterName);
		}
		else {
			logfield.setValue(logfield.getValue() + '\n' + letterName);
		}
	}

	function ExecuteGenerateActionProcess(processName, formatType, upload) {
		if (typeof upload === "undefined" || upload == null) upload = false;

		var personReference = null;
		var personId = Xrm.Page.getAttribute('udo_personid').getValue();
		if (typeof personId !== "undefined" && personId !== null && personId.length > 0) {
			personReference = personId[0];
		} // else throw error?

		return Va.Udo.Crm.Scripts.Process.ExecuteAction(processName,
		[{
			Key: "Target",
			Type: Va.Udo.Crm.Scripts.Process.DataType.EntityReference,
			Value: { id: Xrm.Page.data.entity.getId(), entityType: Xrm.Page.data.entity.getEntityName() }
		},
		{
			Key: "Person",
			Type: Va.Udo.Crm.Scripts.Process.DataType.EntityReference,
			Value: personReference
		},
		{
			Key: "SourceUrl",
			Type: Va.Udo.Crm.Scripts.Process.DataType.String,
			Value: source
		},
		{
			Key: "ReportName",
			Type: Va.Udo.Crm.Scripts.Process.DataType.String,
			Value: ssrsReportName
		},
		{
			Key: "ClaimNumber",
			Type: Va.Udo.Crm.Scripts.Process.DataType.String,
			Value: Xrm.Page.getAttribute("udo_claimnumber").getValue()
		},
		{
			Key: "Report",
			Type: Va.Udo.Crm.Scripts.Process.DataType.EntityReference,
			Value: { id: reportId, entityType: "report" }
		},
		{
			Key: "FormatType",
			Type: Va.Udo.Crm.Scripts.Process.DataType.String,
			Value: formatType
		},
		{
			Key: "UploadToVBMS",
			Type: Va.Udo.Crm.Scripts.Process.DataType.Bool,
			Value: upload
		}]);
	}

	function getReportID(reports, reportName) {
		var len = reports.list.length;

		for (var i = 0; i < len; i++) {
			if (reports.list[i].name === reportName) {
				return reports.list[i].reportId;
			}
		}

		return '';
	}

	// s = settings object
	if (typeof s == "undefined" || s == null) {
		s = {};
	}
	if (s.hasOwnProperty('stage')) { s.stage='start';}
	
	switch (s.stage) {
		case 'start':
			// Set defaults, formatType PDF and action to open
			if (typeof s.formatType === "undefined" || s.formatType == null) s.formatType = "PDF";
			if (typeof s.action === "undefined" || s.action == null) s.action = "open";
			s.action = s.action.toLowerCase();
			
			//Check to make sure form is saved before continuing.
			if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE) {
				var message = 'You have to save the form before proceeding.';
				Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, "Error", {width:400,height:165});
				return;
			}
		case 'confirmregeneration':
			s.stage = 'confirmregeneration';
			if (Xrm.Page.getAttribute("udo_requeststatus").getSelectedOption().value == Globals.srRequestStatus.SentValue) { //Sent
				var message = 'Current Request Status is "Sent". ';
				switch (s.action) {
					case 'open': message += 'Do you want to open the generated document?'; break;
					case 'udpload': message += 'Do you want to upload the generated document to VBMS?'; break;
					case 'download': message += 'Do you want to download the generated document?'; break;
					case 'email': message += 'Are you sure you want to resend the email?' break;
					default: message+= 'Do you want to continue?';
				}
				Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Question+Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo, "Continue?", {width:400, height:165})
				.done(function() {
					s.stage = 'verifyregionaloffice';
					GenerateDocument(s);
				});
				return;
			}
		case 'verifyregionaloffice':
			s.stage = 'verifyregionaloffice';
			if (Xrm.Page.getAttribute('udo_regionalofficeid').getValue() == null) {
				Xrm.Page.getControl('udo_regionalofficeid').setFocus();
				var message = "Please specify the regional office.";
				Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, "Error", {width:400,height:165});
				return;
			}
			/*
			// some defaults for what is used beyond the ro verification
			s.sendToRO = true;
			s.sendToVet = (Xrm.Page.getAttribute('udo_sendemailtoveteran').getValue());
			s.sojAddress = '';
			s.to = '';
			
			s.roId = GetLookupId('udo_regionalofficeid');
			if (!s.roId) {
				s.roId = Globals.originalRO;
				s.sendToRO = false;
			}
			if (s.roId) {
				var columns = ['EmailAddress', 'va_IntakeCenterId', 'va_PensionCenterId'];
				CrmRestKit2011.Retrieve('va_regionaloffice', roId, columns, false)
					.fail(function (err) {
						HandleRestError(err, 'Failed to retrieve regional office data');
					})
					.done(function (data) {
						if (data) {
							var r = data.d;
							s.to = r.EmailAddress;
							if (Xrm.Page.getAttribute('udo_letteraddressing').getValue() == 953850000 && r.va_IntakeCenterId && r.va_IntakeCenterId.Id)//Compensation
							{
								var columns = ['udo_ReturnMailingAddress'];
								CrmRestKit2011.Retrieve('va_intakecenter', r.va_IntakeCenterId.Id, columns, false)
									.fail(function (err) {
										HandleRestError(err, 'Failed to retrieve intake center data');
									})
									.done(function (data) {
										if (data && data.d.udo_ReturnMailingAddress) {
											s.sojAddress = data.d.udo_ReturnMailingAddress.replaceAll('\n', '<br/>');
											s.stage = 'regionalofficeverified';
											GenerateDocument(s);
											return;
										}
									});
							}
							else
								if (r.va_PensionCenterId && r.va_PensionCenterId.Id) { //Pension Center
								var columns = ['udo_ReturnMailingAddress'];
								CrmRestKit2011.Retrieve('va_pensioncenter', r.va_PensionCenterId.Id, columns, false)
									.fail(function (err) {
										HandleRestError(err, 'Failed to retrieve pension center data');
									})
									.done(function (data) {
										if (data && data.d.udo_ReturnMailingAddress) {
											s.sojAddress = data.d.udo_ReturnMailingAddress.replaceAll('\n', '<br/>');
											s.stage = 'regionalofficeverified';
											GenerateDocument(s);
											return;
										}
									});
							}
						}
					});
			}
			*/
		case 'regionalofficeverified':
			s.stage = 'regionalofficeverified';
			/*
			s.newReq = (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_CREATE ? "New " : "");
			s.veteran = (Xrm.Page.getAttribute('udo_relatedveteranid').getValue() ? Xrm.Page.getAttribute('udo_relatedveteranid').getValue()[0].name : 'Unknown');
			s.issue = Xrm.Page.getAttribute('udo_issue').getText();
			s.subject = newReq + "Request for" + ": " + (!dispSubType || dispSubType.length == 0 ? issue : dispSubType);
			
			if (s.sendToVet) {
				if (s.sraction == Globals.srAction.ActionEmailFormsText) {
					s.subject = 'Requested VA Forms';
				} else {
					s.subject = 'Requested Information from VA';
				}
			}
			
			s.body = '';
			s.nonEmergency = (s.sraction == Globals.srAction.ActionNonEmergencyEmailText);
			var PCRLookupValue = Xrm.Page.getAttribute('udo_pcrofrecordid').getValue();
			s.PCRName = PCRLookupValue == null ? '' : PCRLookupValue[0].name;
			s.PCRId = PCRLookupValue == null ? '' : PCRLookupValue[0].id;
			s.StationNumber = null;

			if (s.PCRId != '') {
				CrmRestKit2011.Retrieve('SystemUser', s.PCRId, ['va_StationNumber'], false)
                 .done(function (data) {
                     s.StationNumber = data.d.va_StationNumber;
					 s.stage = 'getpcrfullname';
					 GenerateDocument(s);
					 return;
                 })
                .fail(function (err) {
                    HandleRestError(err, 'Failed to retrieve station data');
                });
			}
		case 'getpcrfullname':
			s.stage = 'getpcrfullname';
			var PCRNameArray = s.PCRName.split(',');
			s.PCRNameFullName = '';
			if (PCRNameArray.length > 0) {
				s.PCRNameFullName = (PCRNameArray[1] == undefined ? '' : PCRNameArray[1]) + ' ' + (PCRNameArray[0] == undefined ? '' : PCRNameArray[0]);
			}
			s.Description = Xrm.Page.getAttribute('udo_description').getValue();
			if (s.Description == null) {  //ensure no 'null' in email body
				s.Description = '';
			}
		*/
			
		case 'parse820action':
			s.stage = 'parse820action';
			s.sraction = Xrm.Page.getAttribute("udo_action").getSelectedOption().text;
			s.is0820 = (s.sraction == Globals.sraction.Action0820Text 
					    || s.sraction == Globals.sraction.Action0820aText 
						|| s.sraction == Globals.sraction.Action0820fText 
						|| s.sraction == Globals.sraction.Action0820dText 
						|| s.sraction == Globals.sraction.ActionEmailFormsText);
		case 'readscript':
			s.stage = 'readscript';
			Xrm.Page.getControl("udo_update").setDisabled(false);
			Xrm.Page.getAttribute("udo_update").setSubmitMode('always');
			if (s.is0820) {
				var readscript = emailMessage;
				if (s.sraction == Globals.sraction.Action0820dText) readscript += email0820Message;

				// send mail could be running after automatic save. In this case, script was already prompted
				if (!Globals.runningEmailGenAfterAutoSave) {
					if (!confirm(readscript)) return;
				}

				var rs = Xrm.Page.getAttribute('udo_readscript').getValue();
				if (rs == null || rs == false) Xrm.Page.getAttribute('udo_readscript').setValue(true);
			}
		case 'setreportname':
			s.stage = 'setreportname';
			s.reportName = '';

			switch (s.sraction) {
				case Globals.sraction.Action0820Text:
					s.reportName = "27-0820 - Report of General Information";
					break;
				case Globals.sraction.Action0820aText:
					s.reportName = "27-0820a - Report of First Notice of Death";
					break;
				case Globals.sraction.Action0820dText:
					s.reportName = "27-0820d - Report of Non-Receipt of Payment";
					break;
				case Globals.sraction.Action0820fText:
					s.reportName = "27-0820f - Report of Month of Death";
					break;
				default:
					break;
			}
			
			s.doCreatePDF = reportName && reportName.length>0;
			if (s.doCreatePDF) s.reportName += " - UDO";
		case 'getreports':
			if (s.doCreatePDF && !s.hasOwnProperty('reports')) {
				s.reports = {
					list: [],

					getReports: function () {
						var len, doRequest, me = this,
							url = Xrm.Page.context.getClientUrl();

						if (url[url.length - 1] != "/") { url = url + "/"; }
						url = url + 'xrmservices/2011/OrganizationData.svc/ReportSet?$select=ReportId,Name';

						doRequest = function (url) {
							$.ajax({
								type: 'GET',
								url: url,
								dataType: 'json'
							}).done(function (data) {
								if (data && data.d && data.d.results && data.d.results.length > 0) {
									len = data.d.results.length;

									for (var i = 0; i < len; i++) {
										me.list.push({ name: data.d.results[i].Name, reportId: data.d.results[i].ReportId });
									}

									if (data.d.__next) {
										doRequest(data.d.__next)
									}
								}
							}).fail(function () {
								// console.log("fail")
							});
						};

						doRequest(url);
					}
				};
			}
		case 'setdisposition':
			s.stage = 'setdisposition';
			s.dispSubType = Xrm.Page.getAttribute("udo_disposition").getText();
			if (s.dispSubType == undefined) s.dispSubType = '';
		/*
		case 'attachments':
			s.stage = 'attachments';
		    s.attachmentList = '';
			s.attachmentListx = '';
			s.enclosures = '';
		
			var srextdocquery = {};
			srextdocquery.entity = 'udo_udo_servicerequest_va_externaldocument';
			srextdocquery.filter = "(udo_servicerequestid eq guid'" + Xrm.Page.data.entity.getId() + "')";
			srextdocquery.columns = ['va_externaldocumentid'];
			CrmRestKit2011.ByQuery(srextdocquery.entity, srextdocquery.columns, srextdocquery.filter, false)
			.fail(function (err) {
				HandleRestError(err, 'Failed to retrieve service request external document data');
			})
			.done(function (data) {
				if (data && data.d.results.length > 0) {
					for (var i = 0; i < data.d.results.length; i++) {
						var r = data.d.results[i].va_externaldocumentid;

						var extdocquery = {};
						extdocquery.entity = 'va_externaldocument';
						extdocquery.columns = ['va_name', 'va_DocumentLocation'];
						extdocquery.filter = "(va_externaldocumentId eq guid'" + r + "')";

						CrmRestKit2011.ByQuery(extdocquery.entity, extdocquery.columns, extdocquery.filter, false)
							.fail(function (err) {
								HandleRestError(err, 'Failed to retrieve external document data');
							})
							.done(function (data) {
								if (data && data.d.results.length > 0) {
									if (data.d.results[0].va_name != 'Home Loans' && data.d.results[0].va_name != 'Education'
										&& data.d.results[0].va_name != 'VR&E' && data.d.results[0].va_name != 'Life Insurance' && data.d.results[0].va_name != 'Pension') {
										s.attachmentList += ("<a href='" + data.d.results[0].va_DocumentLocation + "'>" + data.d.results[0].va_name + "</a><br/>");
										s.attachUrlList.push(data.d.results[0].va_DocumentLocation);
									}
									else {
										s.attachmentListx += ("<a href='" + data.d.results[0].va_DocumentLocation + "'>" + data.d.results[0].va_name + "</a><br/>");
									}
									s.enclosures += (enclosures.length > 0 ? '\n' : '') + data.d.results[0].va_name;
								}
							});
					}
				}
			}).then(function() {
				s.stage='enclosures';
				GenerateDocument(s);
				return;
			});
			return;
		case 'enclosures':
			if (Xrm.Page.getAttribute('udo_enclosures').getValue() != s.enclosures) {
				Xrm.Page.getAttribute('udo_enclosures').setValue(s.enclosures);
				// update data to make sure enclosures get on report
				if (Xrm.Page.ui.getFormType() == CRM_FORM_TYPE_UPDATE) {
					CrmRestKit2011.Update('udo_servicerequest', Xrm.Page.data.entity.getId(), { udo_Enclosures: 'enclosures' }, false)
						.fail(function (err) {
							HandleRestError(err, 'Failed to update servicerequest enclosures');
						})
						.done(function (data, status, xhr) {
						});
				}
			}
		*/
		case 'createPDF':
			s.stage = 'createPDF';
			if (!s.doCreatePDF) {
				s.stage = 'email';
				GenerateDocument(s);
				return;
			}
			
			if (Xrm.Page.data.entity.getIsDirty()) {
				var message = 'Some of the data on the screen has changed and was not saved. Would you like to save the record before continuing?';
				Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Question+Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo, "Continue?", {width:400, height:165})
				.done(function() {
					Xrm.Page.data.entity.save();
				})
				.fail(function() {
					// continuing without saving
					s.stage = 'runreport';
					GenerateDocument(s.action, s.formatType, s);
				});

				return;
			}
		case 'runreport':
			s.reportId = getReportID(s.reports, s.reportName);

            if (s.reportId.length == 0) {
				var message = 'Fatal Error: Report was not found.';
				Va.Udo.Crm.Scripts.Popup.MsgBox(message, Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical, "Error", {width:400,height:165});
                return;
            }
			s.stage = s.action;
			GenerateDocument(s.action,s.formatType,s);
			return;
			
		case 'upload':
			// Generate Report
			ExecuteGenerateActionProcess('udo_GenerateSRDocument', s.formatType, true)
			.done(function (data, textStatus, jqXHR) {
				var mimeType = data.MimeType || "";
				var fileContents = data.Base64FileContents || "";
				var fileName = data.FileName || "";
				var uploaded = data.Uploaded || false;
				var uploadMsg = data.UploadMessage || "Document was not generated successfully.";
				var title = "Upload Error";

				if (!uploaded) {
					if (fileContents == null || fileContents.length == 0) {
						title += ": Open Document";
						uploadMsg += "\r\n\r\nWould you like to open the document?";
						var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo +
									  Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation;
						Va.Udo.Crm.Scripts.Popup.MsgBox(uploadMsg, buttons, title)
						.done(function (data) {
							ConfirmMarkAsSent().always(function () {
								// Create Note
								GenerateDocumentCreated(s.sraction);
								// Save Form
								Xrm.Page.data.entity.save();
								// Open Report
								window.open(serverUrl + source.replace("{0}", reportId) + "&p:ServiceRequestGUID=" + Xrm.Page.data.entity.getId());
							});
						});
						return;
					}
					title += ": Download Document";
					uploadMsg += "\r\n\r\nWould you like to download the document?";
					var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo +
								  Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation;
					Va.Udo.Crm.Scripts.Popup.MsgBox(uploadMsg, buttons, title)
					.done(function (data) {
						ConfirmMarkAsSent().always(function () {
							// Create Note
							GenerateDocumentCreated(s.sraction);
							// Save Form
							Xrm.Page.data.entity.save();
							// Download Document
							Va.Udo.Crm.Scripts.Download(fileName, mimeType, fileContents);
						});
					});
					return;
				}
				// Show success
				title = "Document Uploaded";
				uploadMsg = "Document Uploaded Successfully.";
				Va.Udo.Crm.Scripts.Popup.MsgBox(uploadMsg,
					Va.Udo.Crm.Scripts.Popup.PopupStyles.Information,
					title)
				.always(ConfirmMarkAsSent);
			}).fail(function (data) {
				//prompt and open??
				var title = "Upload Error: Open Document";
				var uploadMsg = data.UploadMessage || "Document was not generated successfully.";
				uploadMsg += "\r\n\r\nWould you like to open the document?";
				var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo +
							  Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation;
				Va.Udo.Crm.Scripts.Popup.MsgBox(uploadMsg, buttons, title)
				.done(function (data) {
					ConfirmMarkAsSent().always(function () {
						// Create Note
						GenerateDocumentCreated(s.sraction);
						// Save Form
						Xrm.Page.data.entity.save();
						// Open Report
						window.open(serverUrl + source.replace("{0}", reportId) + "&p:ServiceRequestGUID=" + Xrm.Page.data.entity.getId());
					});
				});
				return;
			});
			break;
		case 'download':
			// Save Form
			Xrm.Page.data.entity.save();
			// Generate Report/Letter
			ExecuteGenerateActionProcess('udo_GenerateSRDocument', s.formatType)
			.done(function (data) {
				var mimeType = data.MimeType || "";
				var fileContents = data.Base64FileContents || "";
				var fileName = data.FileName || "";

				if (fileContents == null || fileContents.length == 0) {
					var title = "Create Failed: Open Document";
					var uploadMsg = data.UploadMessage || "Document was not generated successfully.";
					uploadMsg += "\r\n\r\nWould you like to open the document?";
					var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo +
								  Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation;
					Va.Udo.Crm.Scripts.Popup.MsgBox(uploadMsg, buttons, title)
					.done(function (data) {
						ConfirmMarkAsSent().always(function () {
							// Create Note
							GenerateDocumentCreated(s.sraction);
							// Save Form
							Xrm.Page.data.entity.save();
							// Open Report
							window.open(serverUrl + source.replace("{0}", reportId) + "&p:ServiceRequestGUID=" + Xrm.Page.data.entity.getId());
						});
					});
					return;
				}

				// success - download file
				Va.Udo.Crm.Scripts.Download(fileName, mimeType, fileContents)
				.done(function () {
					ConfirmMarkAsSent();
					// Create Note
					GenerateDocumentCreated(s.sraction);
					// Save Form
					Xrm.Page.data.entity.save();
				});
			}).fail(function (data) {
				//prompt and open??
				var title = "Upload Error: Open Document";
				var uploadMsg = data.UploadMessage || "Document was not generated successfully.";
				uploadMsg += "\r\n\r\nWould you like to open the document?";
				var buttons = Va.Udo.Crm.Scripts.Popup.PopupStyles.YesNo +
							  Va.Udo.Crm.Scripts.Popup.PopupStyles.Exclamation;
				Va.Udo.Crm.Scripts.Popup.MsgBox(uploadMsg, buttons, title)
				.done(function (data) {
					ConfirmMarkAsSent().always(function () {
						// Create Note
						GenerateDocumentCreated(s.sraction);
						// Save Form
						Xrm.Page.data.entity.save();
						// Open Report
						window.open(serverUrl + source.replace("{0}", reportId) + "&p:ServiceRequestGUID=" + Xrm.Page.data.entity.getId());
					});
				});
				return;
			});
			break;
		case 'open': // open
			ConfirmMarkAsSent().always(function () {
				// Create Note
				GenerateDocumentCreated(s.sraction);
				// Save Form
				Xrm.Page.data.entity.save();
				// Open Report
				window.open(serverUrl + source.replace("{0}", reportId) + "&p:ServiceRequestGUID=" + Xrm.Page.data.entity.getId());
			});
			break;
		case 'email':
			//not yet implemented
			break;
		default:
			break;
	}
}