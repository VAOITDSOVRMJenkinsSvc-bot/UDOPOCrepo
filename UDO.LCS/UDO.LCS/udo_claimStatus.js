var nws = false;
var JustDoProcTimes = false;
var SOJ = null;

var UDO = UDO || {};
UDO.Claims = {
 // CSDev Left Intentionally Blank 
};

function formatDate(value) {
    // CSDev Left Intentionally Blank 
}

function getPhaseString(value) {
   // CSDev Left Intentionally Blank 
}

function LoadAll() {
  // CSDev Left Intentionally Blank 
}

function LoadScriptData(data) {
// CSDev Left Intentionally Blank 
}

//Function: Toggle Display of Sections
function toggleDiv(divid) {
  // CSDev Left Intentionally Blank 
}

function AverageClaimProcTimes() {
 // CSDev Left Intentionally Blank 
}

function validateSoj(soj) {
// CSDev Left Intentionally Blank 

}

function AverageClaimProcTimesFor0() {
// CSDev Left Intentionally Blank 
}

function BroomeClosetTimeframe(claim) {
 // CSDev Left Intentionally Blank 
}

function CalculateVAI(claim) {
// CSDev Left Intentionally Blank 
}

function CalculateVAIFor0(claim) {
// CSDev Left Intentionally Blank 
}

function LoadSOJAddress(ind) {
 // CSDev Left Intentionally Blank 
}

function RefreshScript() {
 // CSDev Left Intentionally Blank 
}

function TrackedItemAnalysis(claim, calculatedStatus) {
// CSDev Left Intentionally Blank 
}
function FormatScriptSub(s) {
  // CSDev Left Intentionally Blank 
}

function DecisionDevNoteNewEvidenceAnalysis(claim) {
  // CSDev Left Intentionally Blank 
}

function E(val) { return (!val || val == undefined || val.length == 0); }

function NoticeLn(trItem, recip, dateVal, dateDesc) {
   // CSDev Left Intentionally Blank 
}

//function LastCallFromSameVet(claim) {
//    var res = '';
//    var pid = '';
//    var ssn = '';
//    if (claim && claim) {
//        if (!E(claim.ssn)) { ssn = claim.ssn; }
//        if (claim && !E(claim.participantClaimantID)) {
//            pid = claim.participantClaimantID;
//        }
//        else {
//            var ptc = claim.payeeTypeCode;
//            if (ptc == '00') {
//                try { pid = claim.participantID; } catch (gpe) { }
//            }
//        }
//    }
//    if (E(ssn) && E(pid)) return res;

//    var columns = ['Subject', 'ActivityId', 'CreatedOn'];
//    // var thisCallId = parent.opener.parent.Xrm.Page.data.entity.getId();

//    var filter = (E(ssn) ? "va_ParticipantID eq '" + pid : "va_SSN eq '" + ssn) + "'";
//    //if (thisCallId) { filter += " and ActivityId ne guid'" + thisCallId + "'"; }

//    var uri = Xrm.Page.context.getClientUrl() + '/XRMServices/2011/OrganizationData.svc/PhoneCallSet?$select=' + columns.join(',') +
//        '&$filter=' + filter + '&$orderby=CreatedOn desc,Subject asc';
//    var calls = null;
//    CrmRestKit2011.ByQueryUrl(uri, false)
//    .done(function (data) {
//        if (data && data.d) {
//            calls = data.d;
//        }
//    }).fail(function (err) {
//        errorRestKit(err, 'Failed to retrieve phone calls');
//    });
//    var id = '';

//    if (calls && calls.results && calls.results.length > 0) {
//        for (var i = 0; i < calls.results.length; i++) {
//            id = calls.results[i].ActivityId;
//            var fieldValue = eval(calls.results[i].CreatedOn);
//            var dt = new Date(parseInt(fieldValue.toString().replace("/Date(", "").replace(")/", "")));

//            // TODO: also check for record id
//            // ignore first call within 45 min of the current time
//            var oldness = ((new Date()).getTime() - dt.getTime()) / 1000;
//            if (oldness <= 2700 && i == 0) { continue; } // up to 45 min

//            var dateValue = dt.toString(); //  dt.format("MM/dd/yyyy hh:mm");
//            res = /*'Last Call regarding Claimant: ' +*/dateValue + (E(calls.results[i].Subject) ? '' : '; Subject: ' + calls.results[0].Subject) + ' (click to open in new window)';
//            break;
//        }
//    }

//    if (!E(res)) {
//        document.getElementById('lastCallRow').style.display = 'block';
//        var url = Xrm.Page.context.getClientUrl() + "/main.aspx?etc=4210&id=" + id + "&pagetype=entityrecord";
//        var ref = '<a id="relCallLink" style="text-decoration: underline; cursor: pointer;" href="' + url +
//            '" target="_blank" title="Click to Open Prior Call Record">' + res + '</a>';
//        document.getElementById('LastCallReClaimantCell').innerHTML = ref;
//    }
//    return res;
//}

//function RatingAwardLastChanges(claim) {
//    var res = '';
//    var lastAward = '';
//    var lastRating = '';

//    // Get Award Store
//    //var awStore = parent.opener.Ext.data.StoreManager.lookup('AwardsStore');
//    var awStore = claim.awards;

//    if (awStore && awStore.length > 0) {
//        for (var i = 0; i < awStore.length; i++) {
//            if (awStore[i].payeeCd != claim.payeeTypeCode && awStore[i].payeeTypeCode != claim.payeeTypeCode) { continue; }
//            var beneTC = awStore[i].awardBeneTypeCd;
//            if (beneTC == undefined) { beneTC = awStore[i].benefitTypeCode; }
//            if (beneTC == undefined) { beneTC = ''; }
//            else { beneTC = '/' + beneTC; }
//            lastAward = lastAward + awStore[i].awardTypeCd + beneTC + '; ';
//        }
//        if (lastAward.length > 0) { lastAward = 'Awards: ' + lastAward; }
//    }

//    // Get Rating Store
//    try { store = claim.rating; } catch (per) { }
//    if (store && store.length > 0) {
//        var pc = store[0].diagnosticPercent;
//        lastRating = 'Rating: ' + store[0].diagnosticText +
//            (pc && pc.length > 0 ? '-' + pc + '%' : '') + '-' + store[0].disabilityDecisionTypeName;
//        var receivedO = store[0].beginDateFormatted; var received = '';
//        if (receivedO && receivedO != undefined) { received = receivedO.toString('M/dd/yyyy'); }
//        lastRating = lastRating + (E(received) ? '' : ' (' + received + ')') + '; ';
//    }

//    res = lastRating + lastAward;

//    if (!E(res)) {
//        document.getElementById('rateAwardChangeRow').style.display = 'block';
//        document.getElementById('ratingAwardLastChangeDate').innerHTML = res;
//    }
//}

function CCallWrapper(aObjectReference, aDelay, aMethodName, aArgument0, aArgument1, aArgument2, aArgument3, aArgument4, aArgument5, aArgument6, aArgument7, aArgument8, aArgument9) {
// CSDev Left Intentionally Blank 
}

CCallWrapper.prototype.execute = function () {
// CSDev Left Intentionally Blank 
};

CCallWrapper.prototype.cancel = function () {
// CSDev Left Intentionally Blank 
};

CCallWrapper.asyncExecute = function (/* CCallWrapper */callwrapper) {
   // CSDev Left Intentionally Blank 
};

CCallWrapper.mCounter = 0;
CCallWrapper.mPendingCalls = {};

function PostLoadOps(par) {
   // CSDev Left Intentionally Blank 
}
//This syntax below looks off.
PostLoadOps.prototype.get =
    function (doAlert, claim) {
        //Do not load last phone call as this will show in USD under Veteran History
        //var res = LastCallFromSameVet(claim);
        //if (doAlert && !E(res)) alert(res);

        //Do not show last award (UDO Defect 317467)
        //RatingAwardLastChanges(claim);
        return 0;
    };

function errorRestKit(err, message) {
// CSDev Left Intentionally Blank 
}

function toggleX(className) {
// CSDev Left Intentionally Blank 
}

function FindAverageProcessingTimes(idStr) {
// CSDev Left Intentionally Blank 
}
