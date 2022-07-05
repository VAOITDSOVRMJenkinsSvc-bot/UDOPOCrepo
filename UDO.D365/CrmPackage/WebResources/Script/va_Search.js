var search = function (context) {
    this.context = context;
    //this.actionMessage.name = 'search';

    //this.webservices = new Array();
    //this.analyzers = new Array();
}
search.prototype.constructor = search;
search.prototype.executeSearchOperations = function (webservices) {
    //TODO fix typo in executeSearchOperations
    //Execution loop
    for (w in webservices) {
        webservices[w].initializeSearchParameters();
        var missingParameters = webservices[w].requiredParametersMissing();
        var doRunWS = true;

        if (missingParameters) {
            for (p in missingParameters) {
                for (s in webservices) {
                    if (!webservices[s].executed
                    && webservices[s].dataSourceForKey
                    && webservices[s].dataSourceForKey[p] == true) {
                        webservices[s].executeRequest();
                        //TODO Analyze Results
                        if (this.context.parameters[p]) break;
                    }
                }
                
                if (this.context.parameters[p] == null) {
                    // add warning if needed
                    if (webservices[w].ignoreRequiredParMissingWarning != true) {
                        this.actionMessage.warningFlag = true;
                        this.actionMessage.description = w + " Web Service call could not be executed because following required parameter is blank: '" + p + "'";
                        this.actionMessage.xmlResponse = this.webservices[w].responseXml;
                        this.actionMessage.pushMessage();

                        this.endState = true;
                        return;
                    }
                    else {
                        // ignore and go to next ws call
                        doRunWS = false;
                        break;
                    }
                }
            }
        }
        if (this.endState) return;

        if (doRunWS) {
            this.webservices[w].executeRequest();

            if (this.analyzers[w]) {
                this.analyzers[w](this);
            }
        }

        if (this.context.endState) return;
    }
}