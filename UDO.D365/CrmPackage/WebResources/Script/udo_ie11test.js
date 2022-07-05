function ietestchange() {
	debugger;
	var attr = Xrm.Page.getAttribute("caseorigincode");
	var opts = attr.getOptions();
	var ctrl = Xrm.Page.getControl("caseorigincode");
	
	ctrl.removeOption(1);
	
	ctrl.addOption({text:'blahblah',value:1});
	
	var skip=false;
	
	
	
	if (!skip) {
	
	ctrl.clearOptions();
	}
	
	if (!skip) {
	for (var i = 1; i <= opts.length; i++) {	
        if (opts[opts.length - i].value != "null") {
			
            ctrl.addOption(opts[opts.length - i], i - 1);
        }
    }
	}
	
	if (!skip) {
	attr.setValue(1);
	}
}