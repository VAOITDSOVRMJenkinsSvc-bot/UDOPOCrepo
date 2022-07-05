var width=1080;
var height=900;
self.moveTo((screen.availwidth-width)/2,(screen.availheight-height)/2);
self.resizeTo(width,height);


function insertModDate(){
	lastmod = document.lastModified;
	lastmoddate = Date.parse(lastmod);
	//formatDate = lastmoddate.format("dddd, mmmm dS, yyyy, h:MM:ss TT");
	today = new Date();
	difference = today - lastmoddate;
		days = Math.round(difference/(1000*60*60*24));
	if (days <= 14) {
	document.writeln("<p align=center style=\"color:red; font-size:20px;\"><b><u>NOTE</u> - THIS WAS JUST UPDATED ON " + lastmod +"</b></p>");
	}
}

function LoadClaimScriptData() {
    debugger
    if (!parent || !parent.opener) {
        alert('Cannot update Claim Script window with the selected claim data because parent window with cached claim information is not available.');
        return;
    }

	var claim = parent.opener._selectedClaim;
	var contentions = '';
	if (claim.outerContentions() && claim.outerContentions().getCount() > 0) {
		claim.outerContentions().getAt(0).contentions().each(function concatenateContentionData(contention) { contentions += contention.data.clmntTxt + '; '; });

		if (contentions.length > 1) contentions = contentions.substr(0, contentions.length - 2);
	}

	if (contentions.length == 0) contentions = '(name contentions)';
	var clName = document.getElementById('claimName');
	var dateOpen = document.getElementById('dateOpen');
	var cont = document.getElementById('contentions');
	//var trItemsCount = document.getElementById('trItemsCount');

	clName.innerHTML = '<U>'+claim.data.claimTypeName +'</U>';
	dateOpen.innerHTML = '<U>' + claim.data.claimReceiveDate + '</U>';
	cont.innerHTML = '<U>' + contentions + '</U>';
	//trItemsCount.innerHTML = '123';
}