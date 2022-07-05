//TODO: consider formatting methods to match
var nws = false;
var JustDoProcTimes = false;
var SOJ = null;

var UDO = UDO || {};

window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "udo_claim": "udo_claims",
    "udo_trackeditem": "udo_trackeditems",
    "udo_contention": "udo_contentions",
    "udo_award": "udo_awards",
    "udo_lifecycle": "udo_lifecycles",
    "udo_evidence": "udo_evidences",
    "udo_claimsuspense": "udo_claimsuspenses",
    "udo_claimstatus": "udo_claimstatuses",
    "va_claimprocessingtimes2": "va_claimprocessingtimes2s",
    "va_claimprocessingtimes": "va_claimprocessingtimeses"
});

UDO.Claims = {
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
            params = sPageUrl.split('&');
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

    setDateResult: function (obj, field, OdataDate) {
        var date = new Date(OdataDate);
        //var dateInt = OdataDate.replace(")/", "");
        //var date = new Date(parseInt(dateInt, 10));
        obj[field] = date.getUTCFullYear() + '/' + (date.getUTCMonth() + 1) + '/' + date.getUTCDate();
        obj[field + "_f"] = date.getUTCMonth() + 1 + "/" + date.getUTCDate() + "/" + date.getFullYear();
        return;
    },

    fetchClaim: function (idStr) {
        //_udo_idproofid_value eq 
        
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_epc,udo_claimantfirstname,udo_claimantlastname,udo_claimstatus,udo_participantid,udo_payeetypecode,udo_dateofclaim,udo_phasetype,udo_minestclaimcompletedt,udo_maxestclaimcompletedt,udo_claimantsuffix,udo_ssn&$filter=udo_claimid eq " + idStr;
            
            var me = UDO.Claims; //save this pointer through REST call
            Xrm.WebApi.retrieveMultipleRecords("udo_claim", filter)
                .then(
                function (data) {

                        if (data && data.value && data.value.length > 0) {
                            var entity = data.value[0];
                            me.results.claim.claimCode = entity["udo_epc"]; //('endProductTypeCode');
                            me.results.claim.claimantLastName = entity["udo_claimantfirstname"];
                            me.results.claim.claimantFirstName = entity["udo_claimantlastname"];
                            me.results.claim.claimantSuffix = entity["udo_claimantsuffix"];
                            me.results.claim.statusTypeCode = entity["udo_claimstatus"];
                            me.results.claim.participantID = entity["udo_participantid"];
                            me.results.claim.participantClaimantID = entity["udo_participantid"];
                            me.results.claim.payeeTypeCode = entity["udo_payeetypecode"];
                            me.results.claim.ssn = entity["udo_ssn"];
                            me.results.claim.Id = entity["udo_claimid"];
                            me.setDateResult(me.results.claim, "claimReceiveDate", entity["udo_dateofclaim"]);
                            //me.results.claim.claimReceiveDate = entity["udo_dateofclaim"];
                            me.results.claim.phaseType = entity["udo_phasetype"];
                            //me.results.claim.minEstClaimCompleteDt = entity["udo_minestclaimcompletedt"];//Util.FormatShortDate(entity["udo_minestclaimcompletedt"]);
                            //me.results.claim.maxEstClaimCompleteDt = entity["udo_maxestclaimcompletedt"];//Util.FormatShortDate(entity["udo_maxestclaimcompletedt"]);
                            me.setDateResult(me.results.claim, "minEstClaimCompleteDt", entity["udo_minestclaimcompletedt"]);//Util.FormatShortDate(entity["udo_minestclaimcompletedt"]);
                            me.setDateResult(me.results.claim, "maxEstClaimCompleteDt", entity["udo_maxestclaimcompletedt"]);//Util.FormatShortDate(entity["udo_maxestclaimcompletedt"]);
                    }
                    resolve();
                    });
        });
        
    },
    fetchTrackedItems: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_receiveddate,udo_requestdate,udo_receipient,udo_suspensedate,udo_acceptdate,udo_inerrordate,udo_followupdate,udo_secondfollowupdate,udo_developmentactionletter&$filter=_udo_claimid_value eq " + idStr;

            var me = UDO.Claims; //save this pointer through REST call
            Xrm.WebApi.retrieveMultipleRecords("udo_trackeditem", filter)
                .then(
                function (data) {                
                        me.results.claim.trackedItems = [];
                        if (data && data.value && data.value.length > 0) {
                            var entities = data.value;

                        for (var index = 0; index < entities.length; index++) {
                                me.results.claim.trackedItems[index] = {};
                            me.results.claim.trackedItems[index].acceptDate = me.convertODataDate(entities[index]["udo_acceptdate"]);
                            me.results.claim.trackedItems[index].inErrorDate = me.convertODataDate(entities[index]["udo_inerrordate"]);
                            me.results.claim.trackedItems[index].followupDate = me.convertODataDate(entities[index]["udo_followupdate"]);
                            me.results.claim.trackedItems[index].secondFollowUpDate = me.convertODataDate(entities[index]["udo_secondfollowupdate"]);
                            me.results.claim.trackedItems[index].shortName = me.truncateString(entities[index]["udo_developmentactionletter"]);
                            me.results.claim.trackedItems[index].receivedDate = me.convertODataDate(entities[index]["udo_receiveddate"]);
                            me.results.claim.trackedItems[index].requestDate = me.convertODataDate(entities[index]["udo_requestdate"]);
                            me.results.claim.trackedItems[index].recipient = entities[index]["udo_receipient"];
                            me.results.claim.trackedItems[index].suspenseDate = me.convertODataDate(entities[index]["udo_suspensedate"]);
                            }
                            
                    }
                    resolve();
                    });
        });
        

    },
    fetchContentionsRecords: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_contentionclassification&$filter=_udo_claimid_value eq " + idStr;
            var me = UDO.Claims; //save this pointer through REST call
           
           Xrm.WebApi.retrieveMultipleRecords('udo_contention', filter)

                .then(
                function (data) {
                        me.results.claim.contentionsRecords = [];
                        if (data && data.value && data.value.length > 0) {
                            var entities = data.value;
                            for (var index = 0; index < entities.length; index++) {
                                me.results.claim.contentionsRecords[index] = {};
                                me.results.claim.contentionsRecords[index].contentclass = entities[index]["udo_contentionclassification"];
                            }
                    }
                    resolve();

                    });
        });
        

    },
    fetchAwardsRecords: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_payeecode,udo_payeetypecode,udo_benefitcode,udo_benefittype";           
            var me = UDO.Claims; //save this pointer through REST call
            
            Xrm.WebApi.retrieveMultipleRecords("udo_award", filter)
                .then(
                    function (data) {
                
                        me.results.claim.awards = [];
                        if (data && data.value && data.value.length > 0) {
                            var entities = data.value;
                            for (var index = 0; index < entities.length; index++) {
                                me.results.claim.awards[index] = {};
                                me.results.claim.awards[index].payeeCd = entities[index]["udo_payeecode"];
                                me.results.claim.awards[index].payeeTypeCode = entities[index]["udo_payeetypecode"];
                                me.results.claim.awards[index].awardBeneTypeCd = entities[index]["udo_benefitcode"];
                                me.results.claim.awards[index].benefitTypeCode = entities[index]["udo_benefittype"];
                            }
                            
                        }
                        resolve();
                    });
        });
        

    },
    fetchRatingsRecords: function (idStr) {
        var filter = "?$select=udo_diagnosticpercent,udo_diagnostictype,udo_begindate";
        var me = this; //save this pointer through REST call
        
        Xrm.WebApi.retrieveMultipleRecords("udo_disabilityrating", filter)
        .fail(
            function (err) {
            })
        .done(
        function (data) {
            me.results.claim.rating = [];
            if (data && data.value && data.value.length > 0) {
                var entities = data.value;
                $.each(entities, function (index, entity) {
                    me.results.claim.rating[index] = {};
                    me.results.claim.rating[index].diagnosticPercent = entity["udo_diagnosticpercent"];
                    me.results.claim.rating[index].diagnosticText = entity["udo_diagnostictype"];
                    me.results.claim.rating[index].beginDate = entity["udo_begindate"];
                });
            }
        });

    },
    fetchLifeCycleRecords: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_changedate,udo_actionstation,udo_claimstation,udo_status&$filter=_udo_claimid_value eq " + idStr;
            
            var me = UDO.Claims; //save this pointer through REST call
            
            Xrm.WebApi.retrieveMultipleRecords("udo_lifecycle", filter)

                .then(
                function (data) {
                        me.results.claim.lifeCycles = [];
                        if (data && data.value && data.value.length > 0) {
                            var entities = data.value;
                            for (var index = 0; index < entities.length; index++) {
                                me.results.claim.lifeCycles[index] = {};
                                me.results.claim.lifeCycles[index].lifeCycleStatusTypeName = entities[index]["udo_status"];
                                me.results.claim.lifeCycles[index].actionStation = entities[index]["udo_actionstation"];
                                me.results.claim.lifeCycles[index].stationOfJurisdiction = entities[index]["udo_claimstation"];
                                me.results.claim.lifeCycles[index].changedDate = me.convertODataDate(entities[index]["udo_changedate"]);
                            }
                            
                    }
                    resolve();
                    });
        });
        

    },
    fetchEvidence: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_name,udo_datereceived&$filter=_udo_claimid_value eq " + idStr;
            
            var me = UDO.Claims; //save this pointer through REST call
            
            Xrm.WebApi.retrieveMultipleRecords('udo_evidence', filter)
               
                .then(
                function (data) {
                    
                        me.results.claim.evidenceRecords = [];
                        if (data && data.value && data.value.length > 0) {
                            var entities = data.value;
                            for (var index = 0; index < entities.length; index++) {                                
                                me.results.claim.evidenceRecords[index] = {};
                                me.results.claim.evidenceRecords[index].descriptionText = entities[index]["udo_name"];
                                me.results.claim.evidenceRecords[index].receivedDate = me.convertODataDate(entities[index]["udo_datereceived"]);

                            }
                            
                    }
                    resolve();
                    });
        });
        

    },
    fetchSuspenseRecords: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_suspensedate,udo_suspensereason,udo_actioncompletedon,udo_updatedby&$filter=_udo_claimid_value eq " + idStr;
            
            var me = UDO.Claims; //save this pointer through REST call
            
            Xrm.WebApi.retrieveMultipleRecords("udo_claimsuspense", filter)
               
                .then(
                function (data) {
                        me.results.claim.suspenseRecords = [];
                        if (data && data.value && data.value.length > 0) {
                            var entities = data.value;
                            for (var index = 0; index < entities.length; index++) {
                                me.results.claim.suspenseRecords[index] = {};
                                me.results.claim.suspenseRecords[index].suspenseDate = me.convertODataDate(entities[index]["udo_suspensedate"]);
                                me.results.claim.suspenseRecords[index].suspenseReason = entities[index]["udo_suspensereason"];
                                me.results.claim.suspenseRecords[index].actioncompletedon = me.convertODataDate(entities[index]["udo_actioncompletedon"]);
                                me.results.claim.suspenseRecords[index].updatedby = entities[index]["udo_updatedby"];
                            }
                    }
                    resolve();

                    });
        });
        
    },
    fetchStatusRecords: function (idStr) {
        return new Promise(function (resolve, reject) {
            var filter = "?$select=udo_claimidstring,udo_changedate,udo_actionlocation,udo_status,udo_daysinstatus&$filter=_udo_claimid_value eq " + idStr;
            
            var me = UDO.Claims; //save this pointer through REST call
            
            Xrm.WebApi.retrieveMultipleRecords("udo_claimstatus", filter)
               
                .then(
                function (data) {
                    
                    //his.results.claim.statusRecords = null;
                        me.results.claim.statusRecords = [];
                        if (data && data.value && data.value.length > 0) {
                            var entities = data.value;
                            for (var index = 0; index < entities.length; index++) {
                                me.results.claim.statusRecords[index] = {};
                                me.results.claim.statusRecords[index].udo_ClaimId = entities[index]["udo_claimidstring"];
                                me.results.claim.statusRecords[index].changedDate = me.convertODataDate(entities[index]["udo_changedate"]);
                                me.results.claim.statusRecords[index].actionLocationId = entities[index]["udo_actionlocation"];
                                me.results.claim.statusRecords[index].status = entities[index]["udo_status"];
                                me.results.claim.statusRecords[index].daysInStatus = entities[index]["udo_daysinstatus"];
                            }
                    }
                    resolve();

                    });
        });
        

    },
    convertODataDate: function (OdataDate) {
        if (OdataDate == null || OdataDate.length == 0) return OdataDate;
        var dateInt = OdataDate.replace(")/", "");
        var date = new Date(parseInt(dateInt, 10));
        var dateStr = date.getUTCFullYear() + '/' + (date.getUTCMonth() + 1) + '/' + date.getUTCDate();
        return dateStr;
    },
    getDataFromCRM: function () {
        return new Promise(function (resolve, reject) {
            //UDO.Claims.results = {};
            UDO.Claims.results.claim = {};
            var UrlParams = UDO.Claims.getUrlParams();
            if (UrlParams === null || UrlParams.id === undefined || UrlParams.id === null) return null;
            var promiseArray = [];
            promiseArray.push(UDO.Claims.fetchClaim(UrlParams.id));
			promiseArray.push(UDO.Claims.fetchTrackedItems(UrlParams.id));
            promiseArray.push(UDO.Claims.fetchContentionsRecords(UrlParams.id));
            promiseArray.push(UDO.Claims.fetchLifeCycleRecords(UrlParams.id));
            promiseArray.push(UDO.Claims.fetchStatusRecords(UrlParams.id));
            promiseArray.push(UDO.Claims.fetchSuspenseRecords(UrlParams.id));
            Promise.all(promiseArray).then(function () {
                 resolve(UDO.Claims.results);
            });
        });

    },
    getReducedDataFromCRM: function (idStr) {
        if (idStr == null) return null;
        this.fetchClaim(idStr);
        this.fetchLifeCycleRecords(idStr);
        this.fetchStatusRecords(idStr);
        return this.results;
    },
    truncateString: function (str) {
        if (str == null) return null;
        if (str.length <= 20)
            return str;
        str = str.substring(0, 17) + "...";
        return str;
    }
};

function formatDate(value) {
    return value.getMonth() + 1 + "/" + value.getDate() + "/" + value.getFullYear();
}

function getPhaseString(value) {
    switch (value) {
        case '1': return 'Claim Received';
        case '2': return 'Under Review';
        case '3': return 'Gathering of Evidence';
        case '4': return 'Review of Evidence';
        case '5': return 'Preparation for Decision';
        case '6': return 'Pending Decision Approved';
        case '7': return 'Claim Received';
        default: return "Unkown";
    }
    return "Unknown";
}

function LoadAll() {
    //var results = UDO.Claims.getDataFromCRM();
    UDO.Claims.getDataFromCRM().then(function (results) {
        if (UDO.Claims.results.claim.participantID) {
            var claim = LoadScriptData(results);
            UDO.Claims.claim = claim;
            if (!claim.phaseType)
                alert("The phase of the claim could not be retrieved.");
            if (claim) {
                var postLoadOps = new PostLoadOps();
                var callwrapper = new CCallWrapper(postLoadOps, 1000, 'get', false, claim);
                CCallWrapper.asyncExecute(callwrapper);
            }
        } else {
            LoadAll();
        }
        
    });
    
}

function LoadScriptData(data) {
    //TODO: break this out into differnt methods, too much is going on here at once
    //Claim Variables
    var claim = data.claim,
        contentions = '',
        contentionsCount = 0,
        trItCount = 0,
        defaultSOJ = null;

    //Tracked Items
    if (claim.trackedItems) trItCount = claim.trackedItems.length;

    //Contentions
    if (claim.contentionsRecords && claim.contentionsRecords.length > 0) {
        var allCont = claim.contentionsRecords;
        for (var i = 0; i < allCont.length; i++) {
            contentions += allCont[i].contentclass + '; ';
            contentionsCount++;
        }
        if (contentions.length > 1) {
            contentions = contentions.substr(0, contentions.length - 2);
        }
        else if (contentions.length == 0) {
            contentions = '(no contentions)'; contentionsCount = 0;
        }
    }

    // Life Cycle Variables
    var notDateRateDecComplete1 = document.getElementById('notDateRateDecComplete1'),
        notDateRateDecComplete2 = document.getElementById('notDateRateDecComplete2'),
        notDateRateDecCompleteVal = '(date of rating decision complete)',
        lifeCycleOpenDate = '',
        lifeCycleRecords = null,
        basedOn = null;

    //Set Life Cycle Records
    lifeCycleRecords = claim.lifeCycles;


    //SOJ: 1. Get from Life Cycle (Latest Transferred In, then Original Open)
    if (lifeCycleRecords) {
        basedOn = 'based on Life Cycle SOJ<span id="epc"></span>';
        //Lifecycle 1:M                     
        if (lifeCycleRecords.length > 0) {
            var latestTransferDate = null;

            for (var i = 0; i < lifeCycleRecords.length; i++) {
                var curLifeCycle = lifeCycleRecords[i].lifeCycleStatusTypeName;
                var curRecSOJ = lifeCycleRecords[i].stationOfJurisdiction;
                var curRecDate = (lifeCycleRecords[i].changedDate) ? (new Date(lifeCycleRecords[i].changedDate)) : null;

                switch (curLifeCycle) {
                    case 'Open':
                        if (!defaultSOJ) {
                            defaultSOJ = curRecSOJ;
                        }
                        break;
                    case 'Rating Decision Complete':
                        notDateRateDecCompleteVal = formatDate(new Date(curRecDate));
                        break;
                    case 'Transferred In':  //Get latest Transferred In 
                        if ((!latestTransferDate) || (latestTransferDate >= curRecDate)) {
                            defaultSOJ = curRecSOJ;
                            latestTransferDate = curRecDate;
                        }
                        break;
                }
            }
        }
        if (!JustDoProcTimes) {
            document.getElementById('sojaddress1').value = defaultSOJ;
            document.getElementById('sojaddress2').value = defaultSOJ;
        }
    }

    //SOJ: 2. Get from Status
    if (!defaultSOJ || defaultSOJ.length == 0) {
        var statusChangeDate = null,
            changeDate = null,
            statusRecords = claim.statusRecords;

        if (statusRecords && statusRecords.items) {
            basedOn = 'based on Status SOJ';
            for (var i = 0; i < statusRecords.length; i++) {
                changeDate = statusRecords[i].changedDate;
                var curSOJ = statusRecords[i].actionLocationId;
                if (new Date(changeDate) > new Date(statusChangeDate) || statusChangeDate == null) {
                    statusChangeDate = changeDate;
                    if (curSOJ && curSOJ.length > 0) { defaultSOJ = curSOJ; }
                }
            }
        }
    }

    if (defaultSOJ && defaultSOJ != undefined && defaultSOJ.length > 0) {
        if (!JustDoProcTimes) {
            document.getElementById('soj').value = defaultSOJ;
            var apcCom = document.getElementById('apcCom');
            apcCom.innerHTML = basedOn;
        }
        SOJ = defaultSOJ;
    }

	// SRD - 11/18/2020
	//console.log("claim.claimReceiveDate_f in LoadScriptData - SRD" + claim.claimReceiveDate_f);
    $('.change_date').text(claim.claimReceiveDate_f);
		
    // Global var for if we enter script from FindAverageProcessingTimes function and do not want to do the phase calculation, just return SOJ, and claim (with claimCode)
    if (JustDoProcTimes) return claim;

    // Claim fields
    var claimRecOpenOpen = document.getElementById('claimRecOpenOpen');
    var clType1NotPhase = document.getElementById('clType1NotPhase');
    var clDate1NotPhase = document.getElementById('clDate1NotPhase');
    var clType2NotPhase = document.getElementById('clType2NotPhase');
    var clDate2NotPhase = document.getElementById('clDate2NotPhase');
    var claimRecOpNotr = document.getElementById('claimRecOpNotr');
    var ln = document.getElementById('ln');
    var vet = document.getElementById('vet');
    var claimantFullName = claim.claimantLastName +
        (claim.claimantFirstName ? ', ' + claim.claimantFirstName : '') +
        (claim.claimantMiddleName ? ' ' + claim.claimantMiddleName : '') +
        (claim.claimantSuffix ? ' ' + claim.claimantSuffix : '');

    ln.innerHTML = claimantFullName;
    vet.innerHTML = claimantFullName;

    TrackedItemAnalysis(claim);
    DecisionDevNoteNewEvidenceAnalysis(claim);
    CalculateVAI(claim);
    BroomeClosetTimeframe(claim);

    return claim;
}

//Function: Toggle Display of Sections
function toggleDiv(divid) {
    var divs = ['claimReceived', 'underReview', 'gatheringOfEvidence', 'reviewOfEvidence', 'pendingDecisionApproval', 'preparationForDecision', 'preparationForNotification', 'complete', 'apology', 'VAIPending', 'repeatCaller', 'hardshipFinClaimed', 'hardshipTermIllClaimed'];
    var nwsDivs = ['claimReceived', 'underReview', 'gatheringOfEvidence', 'reviewOfEvidence', 'preparationForDecision', 'pendingDecisionApproval', 'preparationForNotification', 'complete'];

    if (nws && $.inArray(divid, nwsDivs) > -1) {
        divid += "_nws";
        for (var n = 0; n < nwsDivs.length; n++) {
            divs[divs.length] = nwsDivs[n] + "_nws";
        }
    }

    if (document.getElementById(divid).style.display == 'none') {
        document.getElementById(divid).style.display = 'block';

    } else {
        document.getElementById(divid).style.display = 'none';
    }

    for (var i in divs) {
        if (divs[i] == divid || divid == 'viatable') continue;

        var curDiv = document.getElementById(divs[i]);
        if (curDiv && curDiv.style.display != 'none') { curDiv.style.display = 'none'; }
    }
}

function AverageClaimProcTimes() {
    var claim = UDO.Claims.claim;
    if (claim) {
        claimCode = claim.claimCode;
    }

    if (!JustDoProcTimes)
        var soj = document.getElementById('soj').value;

    if (!soj || soj.length == 0) {
        //check global
        if (!SOJ || SOJ.length == 0) {
            return;
        }
        else {
            soj = SOJ;
        }

    }

    var sojCode = soj;
    if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
    var filter = "?$select=va_name,va_rojname,va_epc,va_mindaysforphase,va_maxdaysforphase,va_mindaysdurationforclaims,va_maxdaysdurationforclaims,va_phase&$filter=va_name eq '" + sojCode + "' and va_epc eq '" + claimCode + "'";
    Xrm.WebApi.retrieveMultipleRecords("va_claimprocessingtimes2", filter)
    .done(function (processing_time) {
        if (processing_time == null || processing_time.value == null || processing_time.value.length == 0) {
            AverageClaimProcTimesFor0();
            return;
        }

        var ss = '';

        if (processing_time && processing_time.value && processing_time.value.length > 0) {
            // get current claim type code               
            var entities = processing_time.value;

            $.each(entities, function (index, entity) {
                var phase = getPhaseString(entity["va_phase"]);

                ss += "Phase: " + phase + "\n";
                ss += "Min for Phase: " + entity["va_mindaysforphase"] + "\n";
                ss += "Max for Phase: " + entity["va_maxdaysforphase"] + "\n";
                ss += "Min Duration for Claims: " + entity["va_mindaysdurationforclaims"] + "\n";
                ss += "Max Duration for Claims: " + entity["va_maxdaysdurationforclaims"] + "\n";
                ss += "\n";
            });

            alert('Claim processing times for ' + soj + ' (days)' + '\n\n' + ss);

        }

    }).fail(function (err) {
        errorRestKit(err, 'Failed to retrieve the claims processing times');
    });
}

function validateSoj(soj) {
    var numberic = /^[0-9]+$/;
    if (soj.match(numberic)) {
        return true;
    } else {
        return false;
    }

}

function AverageClaimProcTimesFor0() {
    var claim = UDO.Claims.claim;
    if (claim) {
        claimCode = claim.claimCode;
        claimCode = claimCode.substr(0, 2) + '0';
    }

    if (!JustDoProcTimes) {
        var soj = document.getElementById('soj').value;
    }
    if (!soj || soj.length == 0) {
        //check global
        if (!SOJ || SOJ.length == 0) {
            return;
        }
        else {
            soj = SOJ;
        }
    }
    
    var sojCode = soj;
    if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
    var filter = "?$select=va_name,va_rojname,va_epc,va_mindaysforphase,va_maxdaysforphase,va_mindaysdurationforclaims,va_maxdaysdurationforclaims,va_phase&$filter=va_name eq '" + sojCode + "' and va_epc eq '" + claimCode + "'";
    
    Xrm.WebApi.retrieveMultipleRecords('va_claimprocessingtimes2', filter)
    .done(function (processing_time) {
        if (processing_time == null || processing_time.value == null || processing_time.value.length == 0) {
            alert('No data found');
            return;
        }

        // if there's more than one result, try to find one that has claim type for current claim
        var result = null;
        var anyCodeResult = null;
        var s = '', ss = '';

        if (processing_time.value.length > 0) {
            var entities = processing_time.value;
            // get current claim type code
            var claimCode = null;
            var recordIndex = 0;


            $.each(entites, function (index, entity) {
                var phase = getPhaseString(entity["va_phase"]);
                ss += "Phase: " + phase + "\n";
                ss += "Min for Phase: " + entity["va_mindaysforphase"] + "\n";
                ss += "Max for Phase: " + entity["va_maxdaysforphase"] + "\n";
                ss += "Min Duration for Claims: " + entity["va_mindaysdurationforclaims"] + "\n";
                ss += "Max Duration for Claims: " + entity["va_maxdaysdurationforclaims"] + "\n";
                ss += "\n";
            });
        }
        alert('Claim processing times for ' + soj + ' (days)' + '\n\n' + ss);

    }).fail(function (err) {
        errorRestKit(err, 'Failed to retrieve the claims processing times');
    });
}

function BroomeClosetTimeframe(claim) {
    var soj = document.getElementById('soj').value;
    if (!soj || soj.length == 0) {
        return;
    }

    
    var sojCode = soj;
    if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);

    var result = null;
    var anyCodeResult = null;
    
    var filter = "?$select=va_name,va_rojname,va_epc,va_form9,va_nod,va_developmentphase,va_decisionphase,va_notificationphase&$filter=va_name eq '" + sojCode + "' and va_epc eq null";
    
    Xrm.WebApi.retrieveMultipleRecords('va_claimprocessingtimes', filter)
        .then(function (processing_time) {
            
            if (processing_time && processing_time.value && processing_time.value.length > 0) {
            var entities = processing_time.value;
            var entity = undefined;
            //var entity = entities.find(entity => function () {
            //    console.log("getting entity");
            //    return (parseInt(entity["va_DevelopmentPhase"]) + parseInt(entity["va_DecisionPhase"]) + parseInt(entity["va_NotificationPhase"])) >= 1;
            //});

            if (entity != undefined) {
                var nptf = parseInt(entity["va_NotificationPhase"]);
                var s = (parseInt(entity["va_DevelopmentPhase"]) + parseInt(entity["va_DecisionPhase"]) + parseInt(entity["va_NotificationPhase"]));
                var bcmonths = (Math.round(((parseFloat(s) / 30) + .5)));
                var nptfm = (Math.round(((parseFloat(nptf) / 30) + .5)));

                //Pluralization
                var sDay = s + ' day';
                var nptfDay = nptf + ' day';
                var bcmonthsMonth = bcmonths + ' month';
                var nptfmMonth = nptfm + ' month';

                if (s > 1) {
                    sDay = sDay + 's';
                }
                if (nptf > 1) {
                    nptfDay = nptfDay + 's';
                }
                if (bcmonths > 1) {
                    bcmonthsMonth = bcmonthsMonth + 's';
                }
                if (nptfm > 1) {
                    nptfmMonth = nptfmMonth + 's';
                }
            }
        }
    });
}

function CalculateVAI(claim) {
    var soj = document.getElementById('soj').value;
    var epc = claim.claimCode;
    //VAI
    var vaiDec = "";
    var vaiData = "0";

    if (!soj || soj.length == 0) { return; }
    if (!epc || epc.length == 0) { return; }
    //TODO: clean up comments

    // to properly match EPC code, we need to replace last digit of Claim's EPC code with 0 (124 -> 120)
    //if (epc.length == 3 && epc[2] != '0') { epc = epc.toString().substring(0, 2) + '0'; }

    var sojCode = soj;
    if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
    
    var filter = "?$select=va_name,va_rojname,va_epc,va_mindaysforphase,va_maxdaysforphase,va_mindaysdurationforclaims,va_maxdaysdurationforclaims,va_phase&$filter=va_name eq '" + sojCode + "' and  va_epc eq '" + epc + "'";
    
    Xrm.WebApi.retrieveMultipleRecords('va_claimprocessingtimes2', filter)
        .then(function (processing_time) {
        
        //TODO: break out into sub method

        // SRD - 10/29/2020
        //debugger;
		//console.log("claim.claimReceiveDate_f - SRD" + claim.claimReceiveDate_f);
        $('.change_date').text(claim.claimReceiveDate_f);

        if (processing_time == null || processing_time.value == null || processing_time.value.length === 0) {
            CalculateVAIFor0(claim);
            return;
        }

        $('#epc').text(', EPC ' + epc);

        var result = null;

        //claim in status
        var statusChangeDate = null;
        var statusRecords = claim.statusRecords;
        var changeDate = null;

        if (statusRecords && statusRecords.items) {
            for (var i = 0; i < statusRecords.length; i++) {
                changeDate = statusRecords[i].changedDate;
                if (new Date(changeDate) > new Date(statusChangeDate) || statusChangeDate === null) {
                    statusChangeDate = changeDate;
                }
            }
        }

        if (vaiData == "0") {
            document.getElementById('vaiYN').innerHTML = "- NO DATA";
        }

        if (!statusChangeDate) { statusChangeDate = claim.claimReceiveDate }

        $('.change_date').text(claim.claimReceiveDate_f);
        var claimStatus = claim.statusTypeCode;
        var today = new Date();

        if (processing_time.value.length > 0) {
            result = processing_time.value[0];

            var epcArray1 = null,
                epcArray2 = null,
                extraDays = 0;

            //row 1 - development ********************************************************************************************
            var caseDt = new Date(statusChangeDate);
            
            extraDays = 10;
            caseDt.setDate(caseDt.getDate() + Number(result.va_avgdaysawaitingdevelopment) + extraDays);
            document.getElementById('days1').innerHTML = result.va_avgdaysawaitingdevelopment;
            document.getElementById('date1').innerHTML = formatDate(caseDt);

            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"];

            var trItCount = 0;
            if (claim.trackedItems) trItCount = claim.trackedItems.length;

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && claimStatus === "PEND" && trItCount === 0) {
                document.getElementById('recommended1').innerHTML = "X";
                document.getElementById('notrecommended1').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended1').innerHTML = "";
                document.getElementById('notrecommended1').innerHTML = "X";
                vaiData = "2";
            }

            //row 2 - evidence ***********************************************************************************************
            //TODO: another sub method
            caseDt = new Date(statusChangeDate); // TODO: check all dates not claim date

            extraDays = 10;

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingEvidence) + extraDays);
            document.getElementById('days2').innerHTML = result.va_AvgDaysAwaitingEvidence;
            document.getElementById('date2').innerHTML = formatDate(caseDt);

            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"]

            var sixtyDaysAgoDate = new Date();
            sixtyDaysAgoDate.setDate(sixtyDaysAgoDate.getDate() - 60);

            var mostRecentTrItemDate = null;
            if (trItCount > 0) {
                for (i = 0; i < claim.trackedItems.length; i++) {
                    var closed = claim.trackedItems[i].acceptDate;
                    received = claim.trackedItems[i].receiveDate;
                    var followup = claim.trackedItems[i].followupDate;
                    var followup2 = claim.trackedItems[i].secondFollowUpDate;
                    var err = claim.trackedItems[i].inErrorDate;
                    var request = claim.trackedItems[i].requestDate;

                    if (new Date(closed) > mostRecentTrItemDate) { mostRecentTrItemDate = closed; }
                    if (new Date(received) > mostRecentTrItemDate) { mostRecentTrItemDate = received; }
                    if (new Date(followup) > mostRecentTrItemDate) { mostRecentTrItemDate = followup; }
                    if (new Date(followup2) > mostRecentTrItemDate) { mostRecentTrItemDate = followup2; }
                    if (new Date(err) > mostRecentTrItemDate) { mostRecentTrItemDate = err; }
                    if (new Date(request) > mostRecentTrItemDate) { mostRecentTrItemDate = request; }

                }
            }

            if (new Date(mostRecentTrItemDate) <= sixtyDaysAgoDate && jQuery.inArray(epc, epcArray1) > -1 && claimStatus === "PEND"
                && trItCount > 0 && caseDt < new Date()) {
                document.getElementById('recommended2').innerHTML = "X";
                document.getElementById('notrecommended2').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended2').innerHTML = "";
                document.getElementById('notrecommended2').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: another sub method
            //row 3 - map-d ***********************************************************************************************
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139"];

            var seventyFiveDaysAgoDate = new Date();
            seventyFiveDaysAgoDate.setDate(seventyFiveDaysAgoDate.getDate() - 75);

            if (!claim.suspenseRecords) {

                return;
            }
            var suspenseCount = claim.suspenseRecords.length;
            var mostRecentSuspenseDate = null, mapdDate = null;
            mostRecentTrItemDate = null;

            if (trItCount > 0) {
                for (i = 0; i < claim.trackedItems.length; i++) {
                    var received = claim.trackedItems[i].receiveDate;
                    if (received && new Date(received) > mostRecentTrItemDate) { mostRecentTrItemDate = received; }
                }
                mapdDate = mostRecentTrItemDate;
            }

            if (suspenseCount > 0) {
                for (i = 0; i < claim.suspenseRecords.length; i++) {
                    var actioncompletedon = claim.suspenseRecords[i].actioncompletedon;

                    if (new Date(actioncompletedon) > mostRecentSuspenseDate) { mostRecentSuspenseDate = actioncompletedon; }
                }
                if (mostRecentSuspenseDate) {
                    if (!mapdDate) {
                        mapdDate = mostRecentSuspenseDate;
                    }
                    else if (new Date(mostRecentSuspenseDate) > new Date(mapdDate)) {
                        mapdDate = mostRecentSuspenseDate;
                    }
                }
            }

            document.getElementById('days3').innerHTML = "75";

            if (mapdDate) {
                var displayDate = new Date(mapdDate);
                displayDate.setDate(displayDate.getDate() + 75);
                document.getElementById('date3').innerHTML = formatDate(new Date(displayDate));
            }

            if (jQuery.inArray(epc, epcArray1) > -1 && claimStatus == "PEND" && (trItCount > 0 || suspenseCount > 0) &&
                mapdDate && new Date(mapdDate) < seventyFiveDaysAgoDate) {
                document.getElementById('recommended3').innerHTML = "X";
                document.getElementById('notrecommended3').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended3').innerHTML = "";
                document.getElementById('notrecommended3').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: another sub method
            //row 4 - decision ***********************************************************************************************
            caseDt = new Date(statusChangeDate);
            //TODO: this looks like the same array from above, this could probably be a global variable and be reused
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139"];
            epcArray2 = ["160", "130", "150", "154", "155"];

            if (jQuery.inArray(epc, epcArray1) > -1) {
                extraDays = 20;
            }

            if (jQuery.inArray(epc, epcArray2) > -1) {
                extraDays = 10;
            }

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingDecision) + extraDays);
            document.getElementById('days4').innerHTML = result.va_AvgDaysAwaitingDecision;
            document.getElementById('date4').innerHTML = formatDate(caseDt);
            //TODO: this looks like the same array from above, this could probably be a global variable and be reused
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139 150", "154", "155"];

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && (claimStatus == "RFD" || claimStatus == "SRFD" || claimStatus == "RI")) {
                document.getElementById('recommended4').innerHTML = "X";
                document.getElementById('notrecommended4').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended4').innerHTML = "";
                document.getElementById('notrecommended4').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: another sub method
            //row 5 - award *************************************************************************************************
            caseDt = new Date(statusChangeDate);

            extraDays = 10;

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingAward) + extraDays);
            document.getElementById('days5').innerHTML = result.va_AvgDaysAwaitingAward;
            document.getElementById('date5').innerHTML = formatDate(caseDt);
            //TODO: this looks like the same array from above, this could probably be a global variable and be reused
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"];

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && (claimStatus == "RC" || claimStatus == "RDC")) {
                document.getElementById('recommended5').innerHTML = "X";
                document.getElementById('notrecommended5').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended5').innerHTML = "";
                document.getElementById('notrecommended5').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: another sub method
            //row 6 - authorization *******************************************************************************************
            caseDt = new Date(statusChangeDate);

            extraDays = 10;

            epcArray1 = ["150", "154", "155"];

            if (jQuery.inArray(epc, epcArray1) > -1) {
                extraDays = 20;
            }

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingAuthorization) + extraDays);
            document.getElementById('days6').innerHTML = result.va_AvgDaysAwaitingAuthorization;
            document.getElementById('date6').innerHTML = formatDate(caseDt);
            //TODO: this looks like the same array from above, this could probably be a global variable and be reused
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"];

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && claimStatus == "AUTH") {
                document.getElementById('recommended6').innerHTML = "X";
                document.getElementById('notrecommended6').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended6').innerHTML = "";
                document.getElementById('notrecommended6').innerHTML = "X";
                vaiData = "2";
            }

            //VAI
            if (vaiData == "0") {
                document.getElementById('vaiYN').innerHTML = "- NO DATA";
            }

            if (vaiDec != "YES") {
                document.getElementById('vaiYN').innerHTML = "- NO";
            }

            if (vaiDec == "YES") {
                document.getElementById('vaiYN').innerHTML = "- YES";
            }
        }
    });
}

function CalculateVAIFor0(claim) {
    var soj = document.getElementById('soj').value;
    var epc = claim.claimCode;
    epc = epc.substr(0, 2) + '0';
    //VAI
    var vaiDec = "";
    var vaiData = "0";

    if (!soj || soj.length == 0) { return; }
    if (!epc || epc.length == 0) { return; }

    var sojCode = soj;
    if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
    var filter = "?$select=va_name,va_rojame,va_epc,va_mindaysforphase,va_maxdaysforphase,va_mindaysdurationforclaims,va_maxdaysdurationforclaims,va_phase&$filter=va_name eq '" + sojCode + "' and va_EPC eq '" + epc + "'";
    
    Xrm.WebApi.retrieveMultipleRecords('va_claimprocessingtimes2', filter)
    .done(function (processing_time) {

        $('#epc').text(', EPC ' + epc);

        var result = null;

        //claim in status
        var statusChangeDate = null;
        var statusRecords = claim.statusRecords;
        var changeDate = null;

        if (statusRecords && statusRecords.items) {
            for (var i = 0; i < statusRecords.length; i++) {
                changeDate = statusRecords[i].changedDate;
                if (new Date(changeDate) > new Date(statusChangeDate) || statusChangeDate == null) {
                    statusChangeDate = changeDate;
                }
            }
        }

        if (vaiData == "0") {
            document.getElementById('vaiYN').innerHTML = "- NO DATA";
        }

        if (!statusChangeDate) { statusChangeDate = claim.claimReceiveDate }

        $('.change_date').text(claim.claimReceiveDate_f);
        var claimStatus = claim.statusTypeCode;
        var today = new Date();

        if (processing_time && processing_time.value && processing_time.value.length > 0) {
            result = processing_time.value[0];

            var epcArray1 = null,
                epcArray2 = null,
                extraDays = 0;
            //TODO: submethod
            //row 1 - development ********************************************************************************************
            var caseDt = new Date(statusChangeDate);

            extraDays = 10;
            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingDevelopment) + extraDays);
            document.getElementById('days1').innerHTML = result.va_AvgDaysAwaitingDevelopment;
            document.getElementById('date1').innerHTML = formatDate(caseDt);
            //TODO: global variable
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"];

            var trItCount = 0;
            if (claim.trackedItems) trItCount = claim.trackedItems.length;

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && claimStatus == "PEND" && trItCount == 0) {
                document.getElementById('recommended1').innerHTML = "X";
                document.getElementById('notrecommended1').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended1').innerHTML = "";
                document.getElementById('notrecommended1').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: submethod
            //row 2 - evidence ***********************************************************************************************
            caseDt = new Date(statusChangeDate); // TODO: check all dates not claim date

            extraDays = 10;

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingEvidence) + extraDays);
            document.getElementById('days2').innerHTML = result.va_AvgDaysAwaitingEvidence;
            document.getElementById('date2').innerHTML = formatDate(caseDt);
            //TODO: global variable
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"]

            var sixtyDaysAgoDate = new Date();
            sixtyDaysAgoDate.setDate(sixtyDaysAgoDate.getDate() - 60);

            var mostRecentTrItemDate = null;
            if (trItCount > 0) {
                for (i = 0; i < claim.trackedItems.length; i++) {
                    var closed = claim.trackedItems[i].acceptDate;
                    received = claim.trackedItems[i].receiveDate;
                    var followup = claim.trackedItems[i].followupDate;
                    var followup2 = claim.trackedItems[i].secondFollowUpDate;
                    var err = claim.trackedItems[i].inErrorDate;
                    var request = claim.trackedItems[i].requestDate;

                    if (new Date(closed) > mostRecentTrItemDate) { mostRecentTrItemDate = closed; }
                    if (new Date(received) > mostRecentTrItemDate) { mostRecentTrItemDate = received; }
                    if (new Date(followup) > mostRecentTrItemDate) { mostRecentTrItemDate = followup; }
                    if (new Date(followup2) > mostRecentTrItemDate) { mostRecentTrItemDate = followup2; }
                    if (new Date(err) > mostRecentTrItemDate) { mostRecentTrItemDate = err; }
                    if (new Date(request) > mostRecentTrItemDate) { mostRecentTrItemDate = request; }

                }
            }

            if (new Date(mostRecentTrItemDate) <= sixtyDaysAgoDate && jQuery.inArray(epc, epcArray1) > -1 && claimStatus == "PEND" && trItCount > 0 && caseDt < new Date()) {
                document.getElementById('recommended2').innerHTML = "X";
                document.getElementById('notrecommended2').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended2').innerHTML = "";
                document.getElementById('notrecommended2').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: submethod
            //row 3 - map-d ***********************************************************************************************
            //TODO: global variable
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139"];

            var seventyFiveDaysAgoDate = new Date();
            seventyFiveDaysAgoDate.setDate(seventyFiveDaysAgoDate.getDate() - 75);

            if (!claim.suspenseRecords) {
                
                return;
            }
            var suspenseCount = claim.suspenseRecords.length;
            var mostRecentSuspenseDate = null, mapdDate = null;
            mostRecentTrItemDate = null;

            if (trItCount > 0) {
                for (i = 0; i < claim.trackedItems.length; i++) {
                    var received = claim.trackedItems[i].receiveDate;
                    if (received && new Date(received) > mostRecentTrItemDate) { mostRecentTrItemDate = received; }
                }
                mapdDate = mostRecentTrItemDate;
            }

            if (suspenseCount > 0) {
                for (i = 0; i < claim.suspenseRecords.length; i++) {
                    var actioncompletedon = claim.suspenseRecords[i].actioncompletedon;

                    if (new Date(actioncompletedon) > mostRecentSuspenseDate) { mostRecentSuspenseDate = actioncompletedon; }
                }
                if (mostRecentSuspenseDate) {
                    if (!mapdDate) {
                        mapdDate = mostRecentSuspenseDate;
                    }
                    else if (new Date(mostRecentSuspenseDate) > new Date(mapdDate)) {
                        mapdDate = mostRecentSuspenseDate;
                    }
                }
            }

            document.getElementById('days3').innerHTML = "75";

            if (mapdDate) {
                var displayDate = new Date(mapdDate);
                displayDate.setDate(displayDate.getDate() + 75);
                document.getElementById('date3').innerHTML = formatDate(new Date(displayDate));
            }

            if (jQuery.inArray(epc, epcArray1) > -1 && claimStatus == "PEND" && (trItCount > 0 || suspenseCount > 0) &&
                mapdDate && new Date(mapdDate) < seventyFiveDaysAgoDate) {
                document.getElementById('recommended3').innerHTML = "X";
                document.getElementById('notrecommended3').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended3').innerHTML = "";
                document.getElementById('notrecommended3').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: submethod
            //row 4 - decision ***********************************************************************************************
            caseDt = new Date(statusChangeDate);
            //TODO: global variable
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139"];
            epcArray2 = ["160", "130", "150", "154", "155"];

            if (jQuery.inArray(epc, epcArray1) > -1) {
                extraDays = 20;
            }

            if (jQuery.inArray(epc, epcArray2) > -1) {
                extraDays = 10;
            }

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingDecision) + extraDays);
            document.getElementById('days4').innerHTML = result.va_AvgDaysAwaitingDecision;
            document.getElementById('date4').innerHTML = formatDate(caseDt);
            //TODO: global variable
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139 150", "154", "155"];

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && (claimStatus == "RFD" || claimStatus == "SRFD" || claimStatus == "RI")) {
                document.getElementById('recommended4').innerHTML = "X";
                document.getElementById('notrecommended4').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended4').innerHTML = "";
                document.getElementById('notrecommended4').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: sub method
            //row 5 - award *************************************************************************************************
            caseDt = new Date(statusChangeDate);

            extraDays = 10;

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingAward) + extraDays);
            document.getElementById('days5').innerHTML = result.va_AvgDaysAwaitingAward;
            document.getElementById('date5').innerHTML = formatDate(caseDt);
            //TODO: global variable
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"];

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && (claimStatus == "RC" || claimStatus == "RDC")) {
                document.getElementById('recommended5').innerHTML = "X";
                document.getElementById('notrecommended5').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended5').innerHTML = "";
                document.getElementById('notrecommended5').innerHTML = "X";
                vaiData = "2";
            }
            //TODO: sub method
            //row 6 - authorization *******************************************************************************************
            caseDt = new Date(statusChangeDate);

            extraDays = 10;

            epcArray1 = ["150", "154", "155"];

            if (jQuery.inArray(epc, epcArray1) > -1) {
                extraDays = 20;
            }

            caseDt.setDate(caseDt.getDate() + Number(result.va_AvgDaysAwaitingAuthorization) + extraDays);
            document.getElementById('days6').innerHTML = result.va_AvgDaysAwaitingAuthorization;
            document.getElementById('date6').innerHTML = formatDate(caseDt);
            //TODO: global variable
            epcArray1 = ["110", "111", "112", "113", "114", "115", "116", "117", "118", "119", "010", "011", "012", "013", "014", "015", "016", "017", "018", "019", "020", "021", "022", "023", "024", "025", "026", "027", "028", "029", "140", "141", "141", "143", "120", "121", "122", "123", "124", "125", "126", "127", "128", "129", "180", "190", "160", "130", "131", "132", "133", "134", "135", "136", "137", "138", "139", "150", "154", "155"];

            if (caseDt < new Date() && jQuery.inArray(epc, epcArray1) > -1 && claimStatus == "AUTH") {
                document.getElementById('recommended6').innerHTML = "X";
                document.getElementById('notrecommended6').innerHTML = "";
                //VAI
                vaiDec = "YES";
                vaiData = "1";
            } else {
                document.getElementById('recommended6').innerHTML = "";
                document.getElementById('notrecommended6').innerHTML = "X";
                vaiData = "2";
            }

            //VAI
            if (vaiData == "0") {
                document.getElementById('vaiYN').innerHTML = "- NO DATA";
            }

            if (vaiDec != "YES") {
                document.getElementById('vaiYN').innerHTML = "- NO";
            }

            if (vaiDec == "YES") {
                document.getElementById('vaiYN').innerHTML = "- YES";
            }
        }
    }).fail(function (err) {
        //TODO: see how webapi handles errors
        errorRestKit(err, 'Failed to retrieve the claim processing times');
    });
}

function LoadSOJAddress(ind) {
    var soj = document.getElementById('sojaddress' + ind).value;
    if (!soj || soj.length == 0) {
        return;
    }

    var sojCode = soj;
    if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
    var filter = "?$select=va_name,va_alias,va_address1,va_address2,va_address3,va_city,va_state,va_zipcode,va_faxnumber,va_specialissuejurisdiction&$filter=va_code eq '" + sojCode + "'";
    
    Xrm.WebApi.retrieveMultipleRecords('va_regionaloffice', filter)
    .then(function (processing_time) {
        if (processing_time && processing_time.value && processing_time.value.length > 0) {
            var res = processing_time.value[0];
            var addr = '<br/>';
            if (res.va_alias && res.va_alias.length > 0) addr += res.va_alias + '<br/>';
            if (res.va_address1 && res.va_address1.length > 0) addr += res.va_address1 + '<br/>';
            if (res.va_address2 && res.va_address2.length > 0) addr += ' ' + res.va_address2 + '<br/>';
            if (res.va_address3 && res.va_address3.length > 0) addr += ' ' + res.va_address3 + '<br/>';
            if (res.va_city && res.va_city.length > 0) addr += res.va_city;
            if (res.va_state && res.va_state.length > 0) addr += ', ' + res.va_state;
            if (res.va_zipcode && res.va_zipcode.length > 0) addr += ' ' + res.va_zipcode;
            if (res.va_faxnumber && res.va_faxnumber.length > 0) addr += '<br/>Fax: ' + res.va_faxnumber;
            if (res.va_specialissuejurisdiction && res.va_specialissuejurisdiction.length > 0) addr += '<br/>Special Issue Jurisdiction: ' + res.va_specialissuejurisdiction;

            document.getElementById('soj' + ind).innerHTML = addr;
        }
        else {
            alert('SOJ lookup table does not have an entry for ROJ Code ' + soj);
        }
    });
}

function RefreshScript() {
    var soj = document.getElementById('soj').value;
    if (validateSoj(document.getElementById('soj').value) == false) {
        return;
    }
    if (!soj || soj.length == 0) {
        return;
    }
    
    var sojCode = soj;
    if (sojCode.length > 3) sojCode = sojCode.substring(0, 3);
    var filter = "?$select=va_name&$filter=va_code eq '" + sojCode + "'";
    
    Xrm.WebApi.retrieveMultipleRecords('va_regionaloffice', filter)
    .done(function (validsoj) {
        if (validsoj && validsoj.value && validsoj.value.length > 0) {
            var claim = UDO.Claims.claim;
            document.getElementById('sojaddress1').value = sojCode;
            document.getElementById('sojaddress2').value = sojCode;
            if (claim) {
                CalculateVAI(claim);
                BroomeClosetTimeframe(claim);
            }
        }
        else {
            alert('SOJ lookup table does not have an entry for ' + soj + '. Please enter a valid SOJ.');
        }
    }).fail(function (err) {
        //TODO: see how webapi handles errors
        errorRestKit(err, 'Failed to retrieve the regional office');
    });
}

function TrackedItemAnalysis(claim, calculatedStatus) {
    //TODO: this should be broken out into submethods, its far too long
    // top level table section
    var whatWeNeedFromYou = document.getElementById('whatWeNeedFromYou'), whatWeNeedFromYouVal = '',
        whatWeNeedFromYou2 = document.getElementById('whatWeNeedFromYou2'), whatWeNeedFromYouVal2 = '',
        whatWeNeedFromYou3 = document.getElementById('whatWeNeedFromYou3'), whatWeNeedFromYouVal3 = '',
        whatWeRecSOl = document.getElementById('whatWeRecSOl'), whatWeRecSOlVal = '',
        whatWeRecNotSOl = document.getElementById('whatWeRecNotSOl'), whatWeRecNotSOlVal = '',
        whatWeNeverRecSOl = document.getElementById('whatWeNeverRecSOl'), whatWeNeverRecSOlVal = '',
        whatWeNeedFromOthers = document.getElementById('whatWeNeedFromOthers'), whatWeNeedFromOthersVal = '',
        suspensesVal = '', firstSuspense = '', useclaimReOpenOrig = false;
    var suspenses = claim.suspenseRecords;
    if (suspenses && suspenses.length > 0) {
        for (var i = 0; i < suspenses.length; i++) {
            var susdtO = suspenses[i].suspenseDate,
                susdt = '';
            if (susdtO && susdtO != undefined) { susdt = susdtO.toString('M/dd/yyyy'); }
            suspensesVal = suspensesVal + suspenses[i].suspenseReason + (E(susdt) ? '' : ' (' + susdt + ')') + '; ';
            if (i == 0 && suspenses[0].suspenseReason !== null) { firstSuspense = suspenses[0].suspenseReason.toUpperCase(); }
        }
    }

    // Substatus values
    var claimReceivedSub = document.getElementById('claimReceivedSub'), claimReceivedSubVal = '',
        underReviewSub = document.getElementById('underReviewSub'), underReviewSubVal = '',
        gatheringOfEvidenceSub = document.getElementById('gatheringOfEvidenceSub'), gatheringOfEvidenceSubVal = '',
        reviewOfEvidenceSub = document.getElementById('reviewOfEvidenceSub'), reviewOfEvidenceSubVal = '',
        preparationForDecisionSub = document.getElementById('preparationForDecisionSub'), preparationForDecisionSubVal = '',
        preparationForNotificationSub = document.getElementById('preparationForNotificationSub'), preparationForNotificationSubVal = '',
        pendingDecisionApprovalSub = document.getElementById('pendingDecisionApprovalSub'), pendingDecisionApprovalSubVal = '',
        completeSub = document.getElementById('completeSub'), completeSubVal = '',
        defaultDiv = '', mostRecentLifeCycle = '', lifeCycleRecordsStore = null;

    lifeCycleRecordsStore = claim.lifeCycles;
    if (lifeCycleRecordsStore && lifeCycleRecordsStore.length > 0) {
        mostRecentLifeCycle = lifeCycleRecordsStore[0].lifeCycleStatusTypeName;
        if (mostRecentLifeCycle) { mostRecentLifeCycle = mostRecentLifeCycle.toString().toUpperCase(); }
    }

    // script selections
    var openNoTrItems_ClaimRec = (claim.statusTypeCode == 'PEND' && (!claim.trackedItems.items || claim.trackedItems.length == 0) && (!claim.contentionsRecords || claim.contentionsRecords.length == 0));
    var openNoTrItems_ContExist = (claim.statusTypeCode == 'PEND' && (!claim.trackedItems.items || claim.trackedItems.length == 0) && claim.contentionsRecords && claim.contentionsRecords.length > 0);
    var decisionCorr = (claim.statusTypeCode == 'RC');
    var notifRateDecComplete = (claim.statusTypeCode == 'RDC');
    var decisionRI = (claim.statusTypeCode == 'RI');
    var decisionReady = (claim.statusTypeCode == 'RFD');
    var notifRTND = (claim.statusTypeCode == 'RTND');
    var decisionSRFD = (claim.statusTypeCode == 'SRFD');
    var notifRTW = (claim.statusTypeCode == 'RTW');
    var cleared = (claim.statusTypeCode == 'CLR');
    var canceled = (claim.statusTypeCode == 'CAN');

    var gotStatus = (openNoTrItems_ClaimRec || openNoTrItems_ContExist || decisionCorr || notifRateDecComplete || decisionRI || decisionReady || notifRTND || decisionSRFD || notifRTW || canceled || cleared);

    var notifPendingAuth = (!gotStatus && claim.statusTypeCode == 'PEND' && firstSuspense == 'PENDING AUTHORIZATION'); // how? Claim Suspense Reason (under Suspenses tab) = Pending Authorization
    var notifPendingConcur = (!gotStatus && claim.statusTypeCode == 'RDC' && mostRecentLifeCycle == 'PENDING'); // how? Life Cycle Status = Pending
    var notifSelfRet = (!gotStatus && claim.statusTypeCode == 'RDC' && mostRecentLifeCycle == 'SELF RETURNED'); // how? AND Life Cycle Status = Self Returned
    gotStatus = (gotStatus || notifPendingAuth || notifPendingConcur || notifSelfRet);

    var openAllTrItemsClosed_ReviewOfEv = (gotStatus ? false : true);
    var openOpenTrItems_GatherEvidence = false;

    var today = new Date();
    // go through tracked items and analyze output
    if (claim && claim.trackedItems && claim.trackedItems.length > 0) {
        for (var i = 0; i < claim.trackedItems.length; i++) {
            var trItem = claim.trackedItems[i].shortName;
            var closed = claim.trackedItems[i].acceptDate;
            var received = claim.trackedItems[i].receiveDate;
            var followup = claim.trackedItems[i].followupDate;
            var followup2 = claim.trackedItems[i].secondFollowUpDate;
            var suspVal = claim.trackedItems[i].suspenseDate; var suspDate = null;
            var err = claim.trackedItems[i].inErrorDate;
            var recip = claim.trackedItems[i].recipient;
            var reqDateO = claim.trackedItems[i].requestDate; var reqDate = '';
            var recDateO = claim.trackedItems[i].receiveDate; var recDate = '';
            var closedO = claim.trackedItems[i].acceptDate; var closedDate = '';
            var followup2O = claim.trackedItems[i].secondFollowUpDate; var followup2Date = '';

            if (reqDateO && reqDateO != undefined) { reqDate = reqDateO.toString('M/dd/yyyy'); }
            if (recDateO && recDateO != undefined) { recDate = recDateO.toString('M/dd/yyyy'); }
            if (closedO && closedO != undefined) { closedDate = closedO.toString('M/dd/yyyy'); }
            if (followup2O && followup2O != undefined) { followup2Date = followup2O.toString('M/dd/yyyy'); }
            if (!E(suspVal)) { suspDate = new Date(claim.trackedItems[i].suspenseDate); }

            if (recip && recip.length > 0 && recip.lastIndexOf('   null   ') > 0) { recip = recip.substring(0, recip.lastIndexOf('   null   ')); }

            //Scenario 1: Tracked items associated to the claim that do NOT have a date entry in the Closed OR Received OR In Error columns
            if (E(closed) && E(received) && E(err)) {
                whatWeNeedFromYouVal = whatWeNeedFromYouVal + NoticeLn(trItem, recip, reqDate, 'req');
                whatWeNeedFromYouVal2 = whatWeNeedFromYouVal2 + NoticeLn(trItem, recip, reqDate, 'req');
                whatWeNeedFromYouVal3 = whatWeNeedFromYouVal3 + NoticeLn(trItem, recip, reqDate, 'req');
            }

            //Scenario 2: Tracked items that HAVE a date entry in Received column AND a date in the Follow Up OR 2nd Follow Up column
            if (!E(received) && (!E(followup) || !E(followup2))) { whatWeRecSOlVal = whatWeRecSOlVal + NoticeLn(trItem, recip, recDate, 'rec'); }

            //Scenario 3: Tracked items that HAVE a date in the Follow Up OR 2nd Follow Up column AND do NOT have a date in the Received column
            if (E(received) && (!E(followup) || !E(followup2))) { whatWeNeverRecSOlVal = whatWeNeverRecSOlVal + NoticeLn(trItem, recip, closedDate, 'closed'); }

            //Scenario 4: Items do not have a date entry in the Closed OR Received OR In Error columns AND a date in the 2nd Follow Up column
            if (E(closed) && E(received) && E(err) && !E(followup2)) { whatWeNeedFromOthers = whatWeNeedFromOthers + NoticeLn(trItem, recip, followup2Date, 'flw2'); }

            // **************************
            // analyze claim script selection
            if (!gotStatus && openAllTrItemsClosed_ReviewOfEv &&
                !(claim.statusTypeCode == 'PEND' && E(err) && (!E(closed) || !E(received) ||
                    (!E(suspDate) && suspDate < today)))) { openAllTrItemsClosed_ReviewOfEv = false; } // all must satisfy condition
            if (!gotStatus && !openOpenTrItems_GatherEvidence && claim.statusTypeCode == 'PEND' && today < suspDate &&
                (E(closed) || E(received) || E(err))) { openOpenTrItems_GatherEvidence = true; gotStatus = true; }

            //Tracked Items dynamic table
            var trackedItemsTableValues = '<table align="left" border="1" cellpadding="2">';

            //Column Names
            trackedItemsTableValues += '<tr><th colspan="9">Tracked Items</th></tr></br>';
            trackedItemsTableValues += '<tr><th>Recipient</th>';
            trackedItemsTableValues += '<th>Development Action</th>';
            trackedItemsTableValues += '<th>Request Date</th>';
            trackedItemsTableValues += '<th>Suspense Date</th>';
            trackedItemsTableValues += '<th>Receive Date</th>';
            trackedItemsTableValues += '<th>Closed Date</th>';
            trackedItemsTableValues += '<th>In Error</th>';
            trackedItemsTableValues += '<th>Follow Up</th>';
            trackedItemsTableValues += '<th>2nd Follow Up</th>';
            trackedItemsTableValues += '</tr>';

            var fnDtFormat = function (varDate) {
                var varOrig = varDate;
                var varModify = null;

                if (varOrig != null) {
                    varOrig = new Date(varOrig);
                    varModify = (varOrig.getMonth() + 1) + '/' + varOrig.getDate() + '/' + varOrig.getFullYear();
                }
                return varModify;
            }

            for (i = 0; i < claim.trackedItems.length; i++) {
                //Convert Long Dates to Short Date format
                var varReqDate = fnDtFormat(claim.trackedItems[i].requestDate);
                var varSusDate = fnDtFormat(claim.trackedItems[i].suspenseDate);
                var varRecDate = fnDtFormat(claim.trackedItems[i].receiveDate);
                var varAccDate = fnDtFormat(claim.trackedItems[i].acceptDate);
                var varErrDate = fnDtFormat(claim.trackedItems[i].inErrorDate);
                var varFolDate = fnDtFormat(claim.trackedItems[i].followupDate);
                var var2FolDate = fnDtFormat(claim.trackedItems[i].secondFollowUpDate);

                //Tracked Item table
                trackedItemsTableValues += '<tr><td>' + '' + (claim.trackedItems[i].recipient || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (claim.trackedItems[i].shortName || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (varReqDate || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (varSusDate || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (varRecDate || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (varAccDate || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (varErrDate || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (varFolDate || '&nbsp;') + '</td>';
                trackedItemsTableValues += '<td>' + (var2FolDate || '&nbsp;') + '</td></tr><br />';
            }
            trackedItemsTableValues += '</table>';
            //Set html tracked item table into page
            document.getElementById('trackedItemsTableValues').innerHTML = trackedItemsTableValues;
        }
    }
    else {
        //No Tracked Items found
        trackedItemsTableValues = '<table align="left" border="1" cellpadding="2"><tr><th colspan="2">No Tracked Items found.</th><tr></table>';
        //Set html tracked item table into page
        document.getElementById('trackedItemsTableValues').innerHTML = trackedItemsTableValues;
    }

    //BAS provided logic 8/29/13
    var statusStore = '',
        calculatedStatus = '';

    if ((claim) && (claim.statusRecords)) {
        statusStore = claim.statusRecords;
    }

    //Create ranking of status by placement in array
    //TODO: consider formatting arrays to match the other arrays in this file
    var statusRanks = new Array('ClaimReceived', 'UnderReview', 'GatheringOfEvidence', 'ReviewofEvidence', 'PreparationforDecision', 'PendingDecisionApproval', 'PreparationForNotification', 'ClosedClaim', 'Cancelled', 'Closed');

    if ((statusStore) && (statusStore.items) && (statusStore.length > 0)) {
        var lastRelevantDate = null;

        for (var i = 0; i < statusStore.length; i++) {
            var curRecordStatus = statusStore[i].claimLocationStatusTypeName;
            //*****Use above code to change to sort same day statuses regardless of time, instead of next line below. ****
            var curRecordDate = (statusStore[i].changedDate) ? (new Date(statusStore[i].changedDate)) : null;

            //Record Prior to TTO - Transferred Out, TTO - Transferred In, Brokered Out, Brokered In
            if ((curRecordStatus != 'TTO - Transferred Out') && (curRecordStatus != 'TTO - Transferred In') && (curRecordStatus != 'Brokered Out') && (curRecordStatus != 'Brokered In')) {
                //IF not one of these and more recent, then set the status
                lastRelevantDate = curRecordDate;
                calculatedStatus = curRecordStatus;
            }
        }
    }

    var imgPath = '',
            picPath = '';

    var phaseType;
    //TODO: clean up comments
    //BCD: get phase from claim which already made request of web service like in the ebsRecords scenario
    if (claim.phaseType) {

        //BCD: get phasetype and min\max completion dates from claim record
        phaseType = claim.phaseType;
        
        var completed_by = claim.maxEstClaimCompleteDt_f;
        var completed_by_fr = claim.minEstClaimCompleteDt_f;
        var completed_by_to = claim.maxEstClaimCompleteDt_f;
        if (completed_by != null && completed_by != '') {
            var d_completed_by = new Date(completed_by);

            if (d_completed_by > new Date()) {
                $('.not_past_due').show();
                $('.past_due').hide();
            }
            else {
                $('.not_past_due').hide();
                $('.past_due').show();
            }
        }
        else {
            completed_by = '<no ws value>';
            nws = true;
        }

        if (completed_by_fr != null && completed_by_fr != '') {
            var d_completed_by = new Date(completed_by_fr);
        }
        else {
            completed_by_fr = '<no ws value>';
        }


        if (completed_by_to != null && completed_by_to != '') {
            var d_completed_by = new Date(completed_by_to);
        }
        else {
            completed_by_to = '<no ws value>';
        }

        $('.completed_by').text(completed_by);
        $('.completed_by_fr').text(completed_by_fr);
        $('.completed_by_to').text(completed_by_to);

        $('.phase_type_cr').text('Claim Received');
        $('.phase_type_ur').text('Under Review');
        $('.phase_type_goe').text('Gathering of Evidence');
        $('.phase_type_roe').text('Review of Evidence');
        $('.phase_type_pfd').text('Preparation for Decision');
        $('.phase_type_pda').text('Pending Decision Approval');
        $('.phase_type_pfn').text('Preparation for Notification');
        $('.phase_type_c').text('Complete');

        switch (phaseType.toUpperCase()) {
            case 'CLAIM RECEIVED':
                $('.phase_type_cr').text(phaseType);
                ClaimReceivedSubVal = ' Claim Received';
                defaultDiv = 'claimReceived';
                imgPath = "udo_/images/ClaimStatusProcesses/ClaimReceived.jpg";
                break;
            case 'UNDER REVIEW':
                $('.phase_type_ur').text(phaseType);
                UnderReviewSubVal = ' Under Review';
                defaultDiv = 'underReview';
                imgPath = "udo_/images/ClaimStatusProcesses/UnderReview.jpg";
                break;
            case 'GATHERING OF EVIDENCE':
                $('.phase_type_goe').text(phaseType);
                GatheringofEvidenceSubVal = ' Gathering of Evidence';
                defaultDiv = 'gatheringOfEvidence';
                imgPath = "udo_/images/ClaimStatusProcesses/GatheringofEvidence.jpg";
                break;
            case 'REVIEW OF EVIDENCE':
                $('.phase_type_roe').text(phaseType);
                ReviewofEvidenceSubVal = ' Review of Evidence';
                defaultDiv = 'reviewOfEvidence';
                imgPath = "udo_/images/ClaimStatusProcesses/ReviewofEvidence.jpg";
                break;
            case 'PREPARATION FOR DECISION':
                $('.phase_type_pfd').text(phaseType);
                preparationForDecisionSubVal = ' Preparation for Decision';
                defaultDiv = 'preparationForDecision';
                imgPath = "udo_/images/ClaimStatusProcesses/PreprationforDecision.jpg";
                break;
            case 'PENDING DECISION APPROVAL':
                $('.phase_type_pda').text(phaseType);
                pendingDecisionApprovalSubVal = ' Pending Decision Approval';
                defaultDiv = 'pendingDecisionApproval';
                imgPath = "udo_/images/ClaimStatusProcesses/PendingDecisionApproval.jpg";
                break;
            case 'PREPARATION FOR NOTIFICATION':
                $('.phase_type_pfn').text(phaseType);
                preparationForNotificationSubVal = ' Preparation for Notification';
                defaultDiv = 'preparationForNotification';
                imgPath = "udo_/images/ClaimStatusProcesses/PreparationForNotification.jpg";
                break;
            case 'COMPLETE':
                $('.phase_type_c').text(phaseType);
                completeSubVal = ' Complete';
                defaultDiv = 'complete';
                imgPath = "udo_/images/ClaimStatusProcesses/Complete.jpg";
                break;
            default:
                $('.phase_type_c').text('<no ws value>');
                completeSubVal = ' Complete';
                defaultDiv = 'complete';
                imgPath = "udo_/images/ClaimStatusProcesses/Complete.jpg";
                break;
        }

        if (!E(defaultDiv)) {
            toggleDiv(defaultDiv);
        }
    }
    else {
        imgPath = "udo_/images/ClaimStatusProcesses/DefaultStatus.jpg";
    }

    picPath = '<img src="' + imgPath + ' ' + '"alt="Claim Status Process"/>';
    document.getElementById('processIMG').innerHTML = picPath;

    // analyze evidence
    var evidenceStore = null,
        ptc = '';

    if (claim) { evidenceStore = claim.evidenceRecords; }
    if (claim) { ptc = claim.payeeTypeCode; }
    if (claim && claim && ptc == '00') {
        if (evidenceStore && evidenceStore.length > 0) {
            // Anything that is entered on the Evidence Tab
            for (var i = 0; i < evidenceStore.length; i++) {
                var name = evidenceStore[i].descriptionText;
                var receivedO = evidenceStore[i].receivedDate; var received = '';
                if (receivedO && receivedO != undefined) { received = receivedO.toString('M/dd/yyyy'); }
                if (!E(name)) { whatWeRecNotSOlVal = whatWeRecNotSOlVal + name + (E(received) ? '' : ' (' + received + ')') + '; '; }
            }
        }
    }
    else { whatWeRecNotSOlVal = 'Evidence Retrieval for Spouse is not yet implemented.'; }
    claimReceivedSub.innerHTML = claimReceivedSubVal;
    underReviewSub.innerHTML = underReviewSubVal;
    gatheringOfEvidenceSub.innerHTML = gatheringOfEvidenceSubVal;
    reviewOfEvidenceSub.innerHTML = reviewOfEvidenceSubVal;
    preparationForDecisionSub.innerHTML = preparationForDecisionSubVal;
    preparationForNotificationSub.innerHTML = preparationForNotificationSubVal;
    pendingDecisionApprovalSub.innerHTML = pendingDecisionApprovalSubVal;
    completeSub.innerHTML = completeSubVal;

    // same subcaptions in script text
    document.getElementById('claimReceivedSub2').innerHTML = FormatScriptSub(claimReceivedSubVal);
    document.getElementById('underReviewSub2').innerHTML = FormatScriptSub(underReviewSubVal);
    document.getElementById('gatheringOfEvidenceSub2').innerHTML = FormatScriptSub(gatheringOfEvidenceSubVal);
    document.getElementById('reviewOfEvidenceSub2').innerHTML = FormatScriptSub(reviewOfEvidenceSubVal);
    document.getElementById('preparationForDecisionSub2').innerHTML = FormatScriptSub(preparationForDecisionSubVal);
    document.getElementById('preparationForNotificationSub2').innerHTML = FormatScriptSub(preparationForNotificationSubVal);
    document.getElementById('pendingDecisionApprovalSub2').innerHTML = FormatScriptSub(pendingDecisionApprovalSubVal);
    document.getElementById('completeSub2').innerHTML = FormatScriptSub(completeSubVal);
}
function FormatScriptSub(s) {
    if (E(s)) return s;
    return ' (' + s + ' )';
}

function DecisionDevNoteNewEvidenceAnalysis(claim) {
    var DecNoticeSent = document.getElementById('DecNoticeSent');
    var DevLetterSent = document.getElementById('DevLetterSent');
    var NewEvidenceRec = document.getElementById('NewEvidenceRec');
    var lifeCycle = document.getElementById('lifeCycle');

    // life cycle analysis
    var lf = '';
    var lifeCycleRecordsStore = null;

    //Technical Debt: Change to IF THEN
    try {
        lifeCycleRecordsStore = claim.lifeCycles;
    }
    catch (lce) {
    }

    if (lifeCycleRecordsStore && lifeCycleRecordsStore.length > 0) {
        // Anything that is entered on the Evidence Tab
        for (var i = 0; i < lifeCycleRecordsStore.length; i++) {
            var name = lifeCycleRecordsStore[i].lifeCycleStatusTypeName;
            var receivedO = lifeCycleRecordsStore[i].changedDate; var received = '';
            if (receivedO && receivedO != undefined) { received = receivedO.toString('M/dd/yyyy'); }
            if (!E(name)) { lf = lf + name + (E(received) ? '' : ' (' + received + ')') + '; '; }
        }
    }
}

function E(val) { return (!val || val == undefined || val.length == 0); }

function NoticeLn(trItem, recip, dateVal, dateDesc) {
    return (E(recip) ? '' : recip + ': ') + trItem + (E(dateVal) ? '' : ' (' + dateDesc + ' ' + dateVal + ')') + '; ';
}

function CCallWrapper(aObjectReference, aDelay, aMethodName, aArgument0, aArgument1, aArgument2, aArgument3, aArgument4, aArgument5, aArgument6, aArgument7, aArgument8, aArgument9) {
    this.mId = 'CCallWrapper_' + (CCallWrapper.mCounter++);
    this.mObjectReference = aObjectReference;
    this.mDelay = aDelay;
    this.mTimerId = 0;
    this.mMethodName = aMethodName;
    this.mArgument0 = aArgument0; this.mArgument1 = aArgument1; this.mArgument2 = aArgument2;
    this.mArgument3 = aArgument3; this.mArgument4 = aArgument4; this.mArgument5 = aArgument5;
    this.mArgument6 = aArgument6; this.mArgument7 = aArgument7; this.mArgument8 = aArgument8;
    this.mArgument9 = aArgument9;
    CCallWrapper.mPendingCalls[this.mId] = this;
}

CCallWrapper.prototype.execute = function () {
    this.mObjectReference[this.mMethodName](this.mArgument0, this.mArgument1, this.mArgument2, this.mArgument3,
        this.mArgument4, this.mArgument5, this.mArgument6, this.mArgument7, this.mArgument8, this.mArgument9);
    delete CCallWrapper.mPendingCalls[this.mId];
};

CCallWrapper.prototype.cancel = function () {
    clearTimeout(this.mTimerId);
    delete CCallWrapper.mPendingCalls[this.mId];
};

CCallWrapper.asyncExecute = function (/* CCallWrapper */callwrapper) {
    CCallWrapper.mPendingCalls[callwrapper.mId].mTimerId = setTimeout('CCallWrapper.mPendingCalls["' + callwrapper.mId + '"].execute()', callwrapper.mDelay);
};

CCallWrapper.mCounter = 0;
CCallWrapper.mPendingCalls = {};

function PostLoadOps(par) {
    this.par = par;
}
//This syntax below looks off.
PostLoadOps.prototype.get =
    function (doAlert, claim) {
        //Do not load last phone call as this will show in USD under Veteran History

        //Do not show last award (UDO Defect 317467)
        //RatingAwardLastChanges(claim);
        return 0;
    };

function errorRestKit(err, message) {
    var error = message + ':\r\nError: ' + err.message;
    if (err.status === 400 && err.responseText)
        error += '\r\n' + err.message;

    alert(error);
}

function toggleX(className) {
    $(className).toggle();
    return false;
}

function FindAverageProcessingTimes(idStr) {
    if (!idStr)
        return null;
    var results = UDO.Claims.getReducedDataFromCRM(idStr);
    JustDoProcTimes = true;
    var claim = LoadScriptData(results);
    UDO.Claims.claim = claim;
    AverageClaimProcTimes();
}
