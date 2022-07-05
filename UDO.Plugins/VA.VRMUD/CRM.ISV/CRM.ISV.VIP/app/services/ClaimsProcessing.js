Ext.define('VIP.services.ClaimsProcessing', {

    /*
    DisplayClaimProcessingTimes: function (soj, source) {
        //debugger;
        if (soj) {
            var sojAddress;
            //look up the alias, e.g., Hartford
            var columns = ['va_name', 'va_ROJName', 'va_EPC', 'va_Form9', 'va_NOD', 'va_DevelopmentPhase', 'va_DecisionPhase', 'va_NotificationPhase'];
            // 'va_RelatedClaimCodes', 'va_AvgControlTime', 'va_AvgDaysAwaitingAuthorization', 'va_AvgDaysAwaitingAward', 'va_AvgDaysAwaitingDecision', 'va_AvgDaysAwaitingDevelopment', 'va_AvgDaysAwaitingEvidence', 
            var sojCode = soj;
            if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
            parent.CrmRestKit2011.ByQuery('va_claimprocessingtimes', columns, "va_name eq '" + sojCode + "' and va_EPC eq null")
            .fail(function (err) {
                errorRestKit(err, 'Failed to retrieve the claims processing times');
            })
            .done(function (processing_time) {
                Ext.Msg.minWidth = 310;
                var s = '', result = null;

                if (processing_time.d.results.length > 0) {
                    // get current claim type code
                    var claimCode = null;
                    var recordIndex = 0;

                    if (!Ext.isEmpty(_selectedClaim)) {
                        claimCode = _selectedClaim.get('endProductTypeCode');
                    }

                    // collect all matching records
                    sojAddress = null;
                    for (var i = 0; i < processing_time.d.results.length; i++) {
                        var curResult = processing_time.d.results[i];
                        var curCodes = curResult.va_EPC;

                        if (!curCodes || curCodes.length == 0) {
                            // found our entry

                            if (i == 0) {
                                var roId = soj;
                                var columns = ['EmailAddress', 'va_Address1', 'va_Address2', 'va_Address3', 'va_City', 'va_FaxNumber', 'va_State', 'va_ZipCode', 'va_name', 'va_Alias'];
                                var filter = "(va_name eq '" + soj + ' - ' + curResult.va_ROJName + "')";
                                parent.CrmRestKit2011.ByQuery('va_regionaloffice', columns, filter, false)
                                .fail(function(err) {
                                    errorRestKit(err, 'Failed to retrieve the regional office');
                                })
                                .done(function (data) {
                                    if (data && data.d.results) {
                                        var r = data.d.results[0];
                                        sojAddress = (r.va_Alias && r.va_Alias.length > 0 ? r.va_Alias + '<br/>' : '') + (r.va_Address1 && r.va_Address1.length > 0 ? r.va_Address1 + '<br/>' : '') + (r.va_Address2 && r.va_Address2.length > 0 ? r.va_Address2 + '<br/>' : '') + (r.va_Address3 && r.va_Address3.length > 0 ? r.va_Address3 + '<br/>' : '') + (r.va_City && r.va_City.length > 0 ? r.va_City + ', ' : '') + (r.va_State && r.va_State.length > 0 ? r.va_State + ' ' : '') + (r.va_ZipCode && r.va_ZipCode.length > 0 ? r.va_ZipCode + '<br/>' : '') + (r.va_FaxNumber && r.va_FaxNumber.length > 0 ? 'FAX: ' + r.va_FaxNumber : '');
                                    }
                                    else {
                                        sojAddress = curResult.va_ROJName;
                                    }
                                });
                            }

                            recordIndex = i;
                            s += "<tr><td>" + curResult.va_ROJName + "&nbsp;&nbsp;</td><td>" +
						    curResult.va_DevelopmentPhase + "</td><td>" + curResult.va_DecisionPhase + "</td><td>" +
						    curResult.va_NotificationPhase + "</td><td>" + curResult.va_Form9 + "</td><td>" + curResult.va_NOD + "</td></tr>";
                            //break;
                        }
                    }

                    claimCode = " - " + claimCode;

                    result = processing_time.d.results[recordIndex];
                }

                if (result) {
                    s = sojAddress + "<br/><br/><table ><tr valign='top'><th><b>ROJ Name</b>&nbsp;&nbsp;</th><th><b>Development&nbsp;&nbsp;<br/>Phase</b>&nbsp;</th><th><b>Decision&nbsp;&nbsp;<br/>Phase</b></th><th><b>Notification&nbsp;&nbsp;<br/><b>Phase</b></th><th><b>Form 9</b>&nbsp;</th><th><b>NOD</b></th></tr><tr><td><br/></td></tr>" +
				    s + "</table>";
                    Ext.Msg.alert('Claim processing times for ' + soj + ' (days)', s);
                }
                else {
                    Ext.Msg.alert('Claim processing times for ' + soj + ' (days)', 'The Claim processing times lookup table does not have an entry for the selected ROJ Code');
                }
            });
        } else {
            alert('Station of Jurisdiction (Award - Award Details tab) is empty. Claim Processing Times information cannot be displayed.\n\nUse Life Cycle tab to view processing times for selected Life Cycle.\nUse Person Info - General tab to view processing times for BIRLS Claim Folder Location.');
        }
    }
    */

    DisplayClaimProcessingTimes: function (soj, source) {
        if (soj) {
            if (!Ext.isEmpty(_selectedClaim)) {
                claimCode = _selectedClaim.get('endProductTypeCode');
                claimCode0 = claimCode.substring(0, 2) + '0';
            }

            var sojAddress;
            //look up the alias, e.g., Hartford
            var columns = ['va_name', 'va_ROJName', 'va_EPC', 'va_MinDaysforPhase', 'va_MaxDaysforPhase', 'va_MinDaysDurationforClaims', 'va_MaxDaysDurationforClaims', 'va_Phase'];

            var sojCode = soj;
            if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
            parent.CrmRestKit2011.ByQuery('va_claimprocessingtimes2', columns, "(va_name eq '" + sojCode + "') and ((va_EPC eq '" + claimCode + "') or (va_EPC eq '" + claimCode0 + "'))")
            .fail(function (err) {
                errorRestKit(err, 'Failed to retrieve the claims processing times');
            })
            .done(function (processing_time) {
                Ext.Msg.minWidth = 310;
                var s = '', ss = '', result = null;
                var a = [7], b = [7], c = [7], d = [7];

                if (processing_time.d.results.length == 0) {
                    Ext.Msg.alert('Error', 'Nothing found');
                    return;
                }
                else {
                    // get current claim type code
                    var claimCode = null;
                    var recordIndex = 0;

                    //if (!Ext.isEmpty(_selectedClaim)) {
                    //    claimCode = _selectedClaim.get('endProductTypeCode');
                    //}

                    // collect all matching records
                    sojAddress = null;
                    for (var i = 0; i < processing_time.d.results.length; i++) {
                        var curResult = processing_time.d.results[i];
                        var curCodes = curResult.va_EPC;

                        if (i == 0) {
                            var roId = soj;
                            var columns = ['EmailAddress', 'va_Address1', 'va_Address2', 'va_Address3', 'va_City', 'va_FaxNumber', 'va_State', 'va_ZipCode', 'va_name', 'va_Alias'];
                            var filter = "(va_name eq '" + soj + ' - ' + curResult.va_ROJName + "')";
                            parent.CrmRestKit2011.ByQuery('va_regionaloffice', columns, filter, false)
                            .fail(function (err) {
                                errorRestKit(err, 'Failed to retrieve the regional office');
                            })
                            .done(function (data) {
                                if (data && data.d.results) {
                                    var r = data.d.results[0];
                                    sojAddress = (r.va_Alias && r.va_Alias.length > 0 ? r.va_Alias + '<br/>' : '') + (r.va_Address1 && r.va_Address1.length > 0 ? r.va_Address1 + '<br/>' : '') + (r.va_Address2 && r.va_Address2.length > 0 ? r.va_Address2 + '<br/>' : '') + (r.va_Address3 && r.va_Address3.length > 0 ? r.va_Address3 + '<br/>' : '') + (r.va_City && r.va_City.length > 0 ? r.va_City + ', ' : '') + (r.va_State && r.va_State.length > 0 ? r.va_State + ' ' : '') + (r.va_ZipCode && r.va_ZipCode.length > 0 ? r.va_ZipCode + '<br/>' : '') + (r.va_FaxNumber && r.va_FaxNumber.length > 0 ? 'FAX: ' + r.va_FaxNumber : '');
                                }
                                else {
                                    sojAddress = curResult.va_ROJName;
                                }
                            });
                        }

                        if (processing_time.d.results.length > 7 && curCodes.substr(2, 1) == '0') {
                            continue;
                        }

                        if (curCodes || curCodes.length > 0) {
                            // found our entry

                            recordIndex = i;
                            //s += "<tr><td>" + curResult.va_ROJName + "&nbsp;&nbsp;</td><td>" +
                            //curResult.va_DevelopmentPhase + "</td><td>" + curResult.va_DecisionPhase + "</td><td>" +
                            //curResult.va_NotificationPhase + "</td><td>" + curResult.va_Form9 + "</td><td>" + curResult.va_NOD + "</td></tr>";
                            //break;

                            if (curResult.va_Phase != null && curResult.va_Phase != '') {
                                var idx = parseInt(curResult.va_Phase) - 1;
                                a[idx] = curResult.va_MinDaysforPhase;
                                b[idx] = curResult.va_MaxDaysforPhase;
                                c[idx] = curResult.va_MinDaysDurationforClaims;
                                d[idx] = curResult.va_MaxDaysDurationforClaims;
                            }
                        }
                    }

                    claimCode = " - " + claimCode;

                    result = processing_time.d.results[recordIndex];
                }

                if (result) {
                    //s = sojAddress + "<br/><br/><table ><tr valign='top'><th><b>ROJ Name</b>&nbsp;&nbsp;</th><th><b>Development&nbsp;&nbsp;<br/>Phase</b>&nbsp;</th><th><b>Decision&nbsp;&nbsp;<br/>Phase</b></th><th><b>Notification&nbsp;&nbsp;<br/><b>Phase</b></th><th><b>Form 9</b>&nbsp;</th><th><b>NOD</b></th></tr><tr><td><br/></td></tr>" +
                    //s + "</table>";
                    s = sojAddress;
                    s += "<table>" +
                        "<tr>" +
                            "<td style=\"width: 200px;\">&nbsp;</td>" +
                            "<td style=\"width: 400px; white-space: nowrap\">&nbsp;</td>" +
                            "<td style=\"text-align: center;\" colspan=\"7\">Phase</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<td style=\"width: 200px;\">&nbsp;</td>" +
                            "<td style=\"width: 400px;\">&nbsp;</td>" +
                            "<td style=\"width: 150px;\">Claim Received</td>" +
                            "<td style=\"width: 150px;\">Under Review</td>" +
                            "<td style=\"width: 150px;\">Gathering of Evidence</td>" +
                            "<td style=\"width: 150px;\">Review of Evidence</td>" +
                            "<td style=\"width: 150px;\">Preparation for Decision</td>" +
                            "<td style=\"width: 150px;\">Pending Decision Approved</td>" +
                            "<td style=\"width: 150px;\">Preparation for Notification</td>" +
                        "</tr>" +
                        "<tr>" +
    "<td style=\"width: 200px;\" rowspan=\"4\">" + curResult.va_ROJName + "</td>" +
    "<td style=\"width: 400px; white-space: nowrap\">Min for Phase</td>" +
    "<td style=\"width: 75px;\">" + (a[0] == undefined ? '' : a[0]) + "</td>" +
    "<td style=\"width: 75px;\">" + (a[1] == undefined ? '' : a[1]) + "</td>" +
    "<td style=\"width: 75px;\">" + (a[2] == undefined ? '' : a[2]) + "</td>" +
    "<td style=\"width: 75px;\">" + (a[3] == undefined ? '' : a[3]) + "</td>" +
    "<td style=\"width: 75px;\">" + (a[4] == undefined ? '' : a[4]) + "</td>" +
    "<td style=\"width: 75px;\">" + (a[5] == undefined ? '' : a[5]) + "</td>" +
    "<td style=\"width: 75px;\">" + (a[6] == undefined ? '' : a[6]) + "</td>" +
"</tr>" +
                        "<tr>" +
    "<td style=\"width: 400px; white-space: nowrap\">Max for Phase</td>" +
    "<td style=\"width: 75px;\">" + (b[0] == undefined ? '' : b[0]) + "</td>" +
    "<td style=\"width: 75px;\">" + (b[1] == undefined ? '' : b[1]) + "</td>" +
    "<td style=\"width: 75px;\">" + (b[2] == undefined ? '' : b[2]) + "</td>" +
    "<td style=\"width: 75px;\">" + (b[3] == undefined ? '' : b[3]) + "</td>" +
    "<td style=\"width: 75px;\">" + (b[4] == undefined ? '' : b[4]) + "</td>" +
    "<td style=\"width: 75px;\">" + (b[5] == undefined ? '' : b[5]) + "</td>" +
    "<td style=\"width: 75px;\">" + (b[6] == undefined ? '' : b[6]) + "</td>" +
"</tr>" +
"<tr>" +
    "<td style=\"width: 400px; white-space: nowrap\">Min Duration for Claims</td>" +
    "<td style=\"width: 75px;\">" + (c[0] == undefined ? '' : c[0]) + "</td>" +
    "<td style=\"width: 75px;\">" + (c[1] == undefined ? '' : c[1]) + "</td>" +
    "<td style=\"width: 75px;\">" + (c[2] == undefined ? '' : c[2]) + "</td>" +
    "<td style=\"width: 75px;\">" + (c[3] == undefined ? '' : c[3]) + "</td>" +
    "<td style=\"width: 75px;\">" + (c[4] == undefined ? '' : c[4]) + "</td>" +
    "<td style=\"width: 75px;\">" + (c[5] == undefined ? '' : c[5]) + "</td>" +
    "<td style=\"width: 75px;\">" + (c[6] == undefined ? '' : c[6]) + "</td>" +
"</tr>" +
"<tr>" +
    "<td style=\"width: 400px; white-space: nowrap\">Max Duration for Claims</td>" +
    "<td style=\"width: 75px;\">" + (d[0] == undefined ? '' : d[0]) + "</td>" +
    "<td style=\"width: 75px;\">" + (d[1] == undefined ? '' : d[1]) + "</td>" +
    "<td style=\"width: 75px;\">" + (d[2] == undefined ? '' : d[2]) + "</td>" +
    "<td style=\"width: 75px;\">" + (d[3] == undefined ? '' : d[3]) + "</td>" +
    "<td style=\"width: 75px;\">" + (d[4] == undefined ? '' : d[4]) + "</td>" +
    "<td style=\"width: 75px;\">" + (d[5] == undefined ? '' : d[5]) + "</td>" +
    "<td style=\"width: 75px;\">" + (d[6] == undefined ? '' : d[6]) + "</td>" +
"</tr>";
                    "</table>";

                    Ext.Msg.alert('Claim processing times for ' + soj + ' (days)', s);
                }
                else {
                    Ext.Msg.alert('Claim processing times for ' + soj + ' (days)', 'The Claim processing times lookup table does not have an entry for the selected ROJ Code');
                }
            });
        } else {
            alert('Station of Jurisdiction (Award - Award Details tab) is empty. Claim Processing Times information cannot be displayed.\n\nUse Life Cycle tab to view processing times for selected Life Cycle.\nUse Person Info - General tab to view processing times for BIRLS Claim Folder Location.');
        }
    }
});