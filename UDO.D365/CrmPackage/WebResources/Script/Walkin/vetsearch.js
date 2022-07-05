; (function ($, window, document, undefined) {

    // Create the defaults once
    var pluginName = "VeteranSearch",
        defaults = {
            LoadingOn: null,
            LoadingOff: null,
            debug: false,
            id: "vetsearch",
            alertErrorClass: "alert alert-validationerror",
            alertInfoClass: "alert alert-info alert-dismissable",
            Log: null,
            LogPrefix: "VeteranSearch: ",
            Name: "Veteran Search"
        };

    var Enums = {
        STATUS: {
            INITIALIZED: { name: "Initialized", value: 0 },
            RUNNING: { name: "Running", value: 1 },
            COMPLETE: { name: "Complete", value: 2 },
            FAILED: { name: "Failed", value: 3 },
            ABORTED: { name: "Aborted", value: 4 }
        },
        IDPROOF: {
            NOTCHECKED: 752280000, SUCCESS: 752280001, FAILURE: 752280002,
            752280000: "Not Checked", 752280001: "Success", 752280002: "Failure"
        }
    };

    var _private = {
        setupDebug: function (plugin) {
            if (!plugin.options.debug) {
                if (window.location.search.indexOf("debug") > -1) plugin.options.debug = true;
            }
        }
        , isFunction: function (f) {
            var o = {};
            return f && o.toString.call(f) === '[object Function]';
        }
        , loadingOn: function (plugin) {
            _private.log(plugin, "Show Loading Image");
            if (_private.isFunction(plugin.options.LoadingOn)) plugin.options.LoadingOn();
            return $.when();
        }
        , loadingOff: function (plugin) {
            _private.log(plugin, "Hide Loading Image");
            if (plugin.options && _private.isFunction(plugin.options.LoadingOff)) plugin.options.LoadingOff();
            return $.when();
        }
        , abort: function (plugin) {
            if (plugin.status.value === Enums.STATUS.RUNNING.value) {
                _private.log(plugin, "Abort Search");
                plugin.status = Enums.STATUS.ABORTED;
            }
        }
        , reject: function (plugin, msg, dfd) {
            if (!dfd) dfd = new $.Deferred();
            _private.log(plugin, msg);
            dfd.reject(msg);
            return dfd.promise();
        }
        , destroy: function (plugin) {
            if (plugin.plugin) plugin = plugin;
            // this does not currently do anything
        }
        , clean: function (plugin, loading) {
            if (plugin.plugin) plugin = plugin.plugin;
            if(loading !== true) _private.loadingOff(plugin);
            plugin.$element.html("");
            return $.when();
        }
        , getSearchStatus: function (plugin) {
            _private.log(plugin, "getSearchStatus: " + plugin.status.name);
            return plugin.status;
        }
        , log: function (plugin, msg, error) {
            var dfd = new $.Deferred();
            if (plugin && plugin.options && plugin.options.LogPrefix)
                msg = plugin.options.LogPrefix + msg;
            if (error) {
                return _private.reject(plugin, msg, dfd);
            } else if (debug) {
                if (plugin && plugin.options && plugin.options.Log && _private.isFunction(plugin.options.Log)) {
                    plugin.options.Log(msg);
                    return;
                }
                dfd.resolve(plugin, msg);
            }
            return dfd.promise();
        }
        , init: function (plugin) {
            plugin.status = Enums.STATUS.INITIALIZED;
        }
        , trigger: function (plugin, ev, data) {
            var name = ev;
            if (ev instanceof jQuery.Event) {
                name = ev.type;
            }
            _private.log(plugin, "Triggering Event: " + name);
            plugin.$element.trigger(ev, data);

            return $.when({ plugin: plugin });
        }
        , vetSelected: function (plugin, ev, data, idproof) {
            _private.log(plugin, "vetSelected: Veteran Selected");
            console.log("in vet selected");
            var vet = _private.getVetFromRow(plugin, ev, data);
            
            if (plugin.$element.data("xrmTable").getViewRecordCount() > 1) {
                vet = vet.then(_private.selectedPersonSearch).then(_private.getVetFromEntity);
            }
            return vet.then(function (plugin, vet) {
                
                if (!vet.address || vet.address === "") {
                    var row = data.row;
                    var r = row.data("obj");
                    var address = row.find("td").filter(function (i, e) { return $(e).data("attributeName") === "address"; }).first().val();
                    vet["address"] = (typeof address === "undefined") ? null : address;
                }

                if (idproof && idproof > 0) {
                    vet["idproof"] = idproof;
                }
                return _private.trigger(plugin, "VeteranSelected", vet);
            })
			.done(_private.clean);
        }
        , vetSelectedError: function (error, plugin) {
            if (!plugin) plugin = this;
            _private.log(plugin, "Error: " + error.message);
            _private.loadingOff(plugin);
            var id = plugin.options.id + "_error",
                divs = plugin.$element.find("#" + id),
				eDiv = null;

            if (divs.length === 0) {
                eDiv = $("<div/>")
                    .attr("id", id)
                    .addClass(plugin.options.alertErrorClass);
                plugin.$element.prepend(eDiv);
            } else {
                eDiv = divs.eq(0);
            }
            eDiv.text(error);
            eDiv.focus();
            plugin.status = Enums.STATUS.FAILED;
        }
        , getVetFromRow: function (plugin, ev, data) {
            _private.log(plugin, "_getVetFromRow: Getting Veteran from Row");
            var row = data.row
				, obj = row.data("obj")
				, vet = _private.getVetFromEntity(plugin, obj)
				, address = row.find("td").filter(function (i, e) { return $(e).data("attributeName") === "address"; }).first().val();

            return vet.done(function (plugin, vet) {
                var dfd = new $.Deferred();
                vet.address = (typeof address === "undefined") ? null : address;
                dfd.resolve(plugin, vet);
                return dfd.promise();
            });
        }
        , getVetFromEntity: function (plugin, entity) {
            try {
                _private.log(plugin, "getVetFromEntity: Getting Veteran from Entity Record");
                var g = function (e, n) {
                    //if (e && e[n]) return e[n].value;
                    if (e && e[n]) return e[n];
                    return null;
                }, r;
                console.log(entity);
                if (entity) r = entity;
                if (r.view) r = r.view();
                if (r.attributes) r = r.attributes;
                var vet = {
                    fullname: g(r, "crme_fullname"),
                    prefix: g(r, "crme_prefix"),
                    firstname: g(r, "crme_firstname"),
                    middlename: g(r, "crme_middlename"),
                    lastname: g(r, "crme_lastname"),
                    suffix: g(r, "crme_suffix"),
                    ssid: g(r, "crme_ssn"),
                    dob: g(r, "crme_dobstring"),
                    sensitivity_level: g(r, "crme_veteransensitivitylevel"),
                    edipi: g(r, "crme_edipi"),
                    gender: g(r, "crme_gender"),
                    branchofservice: g(r, "crme_branchofservice"),
                    icn: g(r, "crme_icn"),
                    patientmviid: g(r, "crme_patientmviidentifier"),
                    recordsource: g(r, "crme_recordsource"),
                    deceaseddate: g(r, "crme_deceaseddate"),
                    identitytheft: g(r, "crme_identitiytheft"),
                    participantid: g(r, "crme_participantid"),
                    phonenumber: g(r, "crme_primaryphone"),
                    address: g(r, "crme_fulladdress"),
                    interactionid: null
                },
                dfd = new $.Deferred();
                if (!vet.sensitivity_level) vet.sensitivity_level = "0";
                dfd.resolve(plugin, vet);
                return dfd.promise();
            }
            catch (ex) {
                //if (ex.message) ex = ex.message;
                _private.log(plugin, ex.message);
                console.log(ex.message);
            }
        }
        , getVetFromForm: function (plugin) {
            //todo: validate here, return null if invalid.
            _private.log(plugin, "getVetFromForm: Getting Veteran Information from Form");
            return {
                firstname: $("#veteran_firstname").val(),
                middlename: $("#veteran_middlename").val(),
                lastname: $("#veteran_lastname").val(),
                ssid: $("#veteran_ssid").val(),
                claimnumber: $("#veteran_claimnumber").val(),
                dob: Tools.FormatDate($("#veteran_dob").val()),
                veteranid: null //maybe this should be in a hidden field...
            };
        }
        , spaceClick: function (e) {
            if (e.which === 32 /*|| e.which == 13*/) $(this).trigger("click");
        }
        , addFormActionButton: function (plugin, parentElement, htmlclass, name, id, clickHandler, visible) {
            _private.log(plugin, "addFormActionButton: Adding Button: " + name);
            var btn = null, index = 1, el = document.getElementById(id), $el = null;

            if (el !== null) {
                $el = $(el);
                $el.data("count", $el.data("count") + 1);
                index = $el.data("count");
            }

            btn = $('<a href="#"/>')
                .addClass("btn")
                .addClass(htmlclass)
                .attr("role", "button")
                .attr("id", id + "_" + index)
                .data("count", index)
                .text(name)
                .on("click", clickHandler)
                .bind("keypress", _private.spaceClick);

            if (visible !== undefined && visible === false) btn.hide();
            if (el === null) parentElement.append(btn);

            return btn;
        }
        , selectedPersonSearch: function (plugin, vet) {
            _private.log(plugin, "selectedPersonSearch: Running Selected Person Search");
            if (!vet || vet === null || !vet.ssid) {
                return _private.log(plugin, "Error: Selected Person Search: No Veteran Passed to Method.", true);
            }

            _private.loadingOn(plugin);

            Sdk.jQ.setJQueryVariable($);
            var eq = Sdk.Query.ConditionOperator.Equal
                , query = Sdk.Query.QueryExpression("crme_person")
                , querycols = new Sdk.ColumnSet(true)
                , addCondition = function (query, field, vet, key) {
                    if (!key) return;
                    if (!vet.hasOwnProperty(key)) return;
                    var value = vet[key];
                    if (typeof value === "undefined" || value === null || value === "") return;
                    query.addCondition("crme_person", field.toLowerCase(), eq, new Sdk.Query.Strings([value]));
                }
                , dfd = new $.Deferred();
            vet.ssid = vet.ssid.replace(/\D/g, ""); // rescrub just incase

            querycols.getAllColumns = function () { return true; };
            query.setColumnSet(querycols); //All Columns

            query.addCondition("crme_person", "crme_searchtype", eq, new Sdk.Query.Strings(['CombinedSearchByFilter']));
            query.addCondition("crme_person", "crme_isattended", eq, new Sdk.Query.Booleans([true]));

            addCondition(query, "crme_dobstring", vet, "dob");

            addCondition(query, "crme_ssn", vet, "ssid");
            addCondition(query, "crme_fullname", vet, "fullname");
            addCondition(query, "crme_prefix", vet, "prefix");
            addCondition(query, "crme_firstname", vet, "firstname");
            addCondition(query, "crme_middlename", vet, "middlename");
            addCondition(query, "crme_lastname", vet, "lastname");
            addCondition(query, "crme_suffix", vet, "suffix");
            addCondition(query, "crme_alias", vet, "alias");
            addCondition(query, "crme_gender", vet, "gender");

            addCondition(query, "crme_fulladdress", vet, "address");

            addCondition(query, "crme_icn", vet, "icn");
            addCondition(query, "crme_PatientMviIdentifier", vet, "patientmviid");

            addCondition(query, "crme_RecordSource", vet, "recordsource");
            addCondition(query, "crme_edipi", vet, "edipi");

            addCondition(query, "crme_deceaseddate", vet, "deceaseddate");
            addCondition(query, "crme_IdentityTheft", vet, "identitytheft");
            addCondition(query, "crme_ParticipantID", vet, "participantid");
            addCondition(query, "crme_primaryphone", vet, "phonenumber");

            addCondition(query, "crme_udointeractionid", vet, "interactionid");
            query = "$filter=crme_firstname eq '" + vet.firstname + "' and  crme_lastname eq '" + vet.lastname + "' and  crme_ssn eq '" + vet.ssid + "' and  crme_searchtype eq 'CombinedSearchByFilter' and crme_isattended eq true and crme_udointeractionid eq '00000000-0000-0000-0000-000000000000'";

            webApi.RetrieveMultiple("crme_person", null, query).then(function (reply) {
                if (reply.length > 0) {
                    dfd.resolve(plugin, reply[0]);
                } else {
                    dfd.reject(plugin, "Error: No Entity was found during selectedPersonSearch.");
                }
            });
            //Sdk.jQ.retrieveMultiple(query).then(function (ec) {
            //    //console.log(ec);
            //    //debugger;
            //    if (ec && ec.getCount && ec.getCount() > 0) {
            //        dfd.resolve(plugin, ec.getEntity(0));
            //    } else {
            //        dfd.reject(plugin, "Error: No Entity was found during selectedPersonSearch.");
            //    }
            //})
			//.fail(function (e) {
   //             if (e.message) e = e.message;
   //             dfd.reject(plugin, e);
			//});
            return dfd.promise();
        }
        , searchByName: function (plugin, vet) {
            var dfd = $.Deferred();

            if (plugin.status.value === Enums.STATUS.RUNNING.value) {
                return _private.reject(plugin, "Already Running Search.", dfd);
            }
            if (!vet || vet === null || !vet.ssid) {
                return _private.reject(plugin, "No veteran information to perform search.", dfd);
            }
            _private.loadingOn(plugin);
            _private.log(plugin, "Searching by name.");
            plugin.status = Enums.STATUS.RUNNING;
            //clean before search
            _private.clean(plugin, true).then(function () {
                // don't procede until cleaned
                vet.ssid = vet.ssid.replace(/\D/g, ""); // rescrub just incase
                var fetch = '<fetch>';
                fetch += '  <entity name="crme_person" >';
                fetch += '    <all-attributes/>';
                fetch += '    <filter>';
                fetch += '      <condition attribute="crme_firstname" operator="eq" value="' + vet.firstname + '" />';
                fetch += '      <condition attribute="crme_lastname" operator="eq" value="' + vet.lastname + '" />';
                fetch += '      <condition attribute="crme_ssn" operator="eq" value="' + vet.ssid + '" />';
                fetch += '    </filter>';
                fetch += '  </entity>';
                fetch += '</fetch>';

;
                var eq = Sdk.Query.ConditionOperator.Equal
                   //, query = Sdk.Query.QueryExpression("crme_person")
                    query = fetch
                   , querycols = new Sdk.ColumnSet(true);
                querycols.getAllColumns = function () { return true; };
                //query.setColumnSet(querycols); //All Columns

                ////query.addCondition("crme_person","crme_udointeractionid", eq, new Sdk.Query.Strings([newGuid()]));
                //query.addCondition("crme_person", "crme_firstname", eq, new Sdk.Query.Strings([vet.firstname]));
                //query.addCondition("crme_person", "crme_lastname", eq, new Sdk.Query.Strings([vet.lastname]));
                ////TODO: UNCOMMENT
                ////query.addCondition("crme_person", "crme_searchtype", eq, new Sdk.Query.Strings(['CombinedSearchByFilter']));
                ////query.addCondition("crme_person", "crme_isattended", eq, new Sdk.Query.Booleans([true]));
                //query.addCondition("crme_person", "crme_ssn", eq, new Sdk.Query.Strings([vet.ssid]));
                
                //if (vet.dob !== null && vet.dob !== "") {
                    
                //    //query.addCondition("crme_person", "crme_dobstring", eq, new Sdk.Query.Strings([vet.dob]));
                //    //query.addCondition("crme_person","crme_DOBString", eq, new Sdk.Query.Dates(new Sdk.DateTime([vet.dob])));
                //}
                query = "$filter=crme_firstname eq '" + vet.firstname + "' and  crme_lastname eq '" + vet.lastname + "' and  crme_ssn eq '" + vet.ssid + "' and  crme_searchtype eq 'CombinedSearchByFilter' and crme_isattended eq true and crme_udointeractionid eq '00000000-0000-0000-0000-000000000000'";
                if (!plugin.$element.data("xrmTable")) {
                    var columns = new XrmTable.Columns(
                    [
                        new XrmTable.Column({
                            name: "Full Name", key: "fullname", type: "header", render: function (data) {
                                if (data instanceof Sdk.Entity) data = data.view().attributes;
                                var name = "";
                                if (data.crme_firstname && data.crme_firstname.value) name = data.crme_firstname.value;
                                if (data.crme_middlename && data.crme_middlename.value && data.crme_middlename.value !== "") {
                                    if (name.length > 0) name += " ";
                                    name += data.crme_middlename.value;
                                }
                                if (data.crme_lastname && data.crme_lastname.value) {
                                    if (name.length > 0) name = ", " + name;
                                    name = data.crme_lastname.value + name;
                                }
                                return name;
                            }
                        }),
                        new XrmTable.Column({ name: "First Name", key: "crme_firstname", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "Middle Name", key: "crme_middlename", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "Last Name", key: "crme_lastname", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "SSN", key: "crme_ssn", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "DOB", key: "crme_dobstring", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "Br. of Svc", key: "crme_branchofservice", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "Gender", key: "crme_gender", "class": "col-md-2" }),
                        new XrmTable.Column({
                            name: "Address", key: "address", "class": "col-md-6",
                            render: function (data) {
                                if (data instanceof Sdk.Entity) data = data.view().attributes;
                                if (data.crme_fulladdress && data.crme_fulladdress.value) {
                                    return data.crme_fulladdress.value;
                                }

                                var street = data.crme_address1 && data.crme_address1.value ? data.crme_address1.value : "";
                                var city = data.crme_city && data.crme_city.value ? data.crme_city.value : "";
                                var state = data.crme_stateprovinceid && data.crme_stateprovinceid.value ? data.crme_stateprovinceid.value : "";
                                var zip = data.crme_zippostalcodeid && data.crme_zippostalcodeid ? data.crme_zippostalcodeid.value : "";

                                return street + " " + city + " " + state + " " + zip;
                            }
                        }),
                        new XrmTable.Column({ name: "EDIPI", key: "crme_edipi", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "Sensitivity Lvl", key: "crme_veteransensitivitylevel", "class": "col-md-2" }),
                        new XrmTable.Column({ name: "Source", key: "crme_recordsource", "class": "col-md-2" }),
                        new XrmTable.Column({
                            type: "custom", name: "", key: "idproofarea", "class": "col-md-12", render: function (data, rows, col, table) {
                                var row = $("<div class='row'/>");
                                var buttonarea = $("<div class='col-md-12 aspace20'/>");
                                row.append(buttonarea);

                                _private.addFormActionButton(plugin, buttonarea, "btn-success", "ID Proof Success", "idproof_success",
                                    function (e) {
                                        _private.vetSelected(plugin, e, { row: $("#" + data.rowid) }, Enums.IDPROOF.SUCCESS)
                                        .fail(_private.vetSelectedError)
                                        .always(_private.loadingOff);
                                        //alert("ID PROOF SUCCESS");
                                    }, true);

                                _private.addFormActionButton(plugin, buttonarea, "btn-danger", "ID Proof Failure", "idproof_failure",
                                    function (e) {
                                        // copy info back?
                                        _private.vetSelected(plugin, e, { row: $("#" + data.rowid) }, Enums.IDPROOF.FAILURE)
                                        .fail(_private.vetSelectedError)
                                        .always(_private.loadingOff);
                                        // alert("ID PROOF FAILED");
                                    }, true);

                                _private.addFormActionButton(plugin, buttonarea, "btn-info", "ID Not Verified", "idproof_notverified",
                                    function (e) {
                                        // copy info back?
                                        _private.vetSelected(plugin, e, { row: $("#" + data.rowid) }, Enums.IDPROOF.NOTCHECKED)
                                        .fail(_private.vetSelectedError)
                                        .always(_private.loadingOff);
                                        //alert("ID PROOF NOT VERIFIED");
                                    }, true);


                                rows.push(row);
                            }
                        })
                        ]);
                    //$filter = crme_firstname eq '' and crme_lastname eq 'n' and crme_searchtype eq 'CombinedSearchByFilter' and crme_udointeractionid eq '94aaefa7-ad92-e911-a978-001dd800ba25' and crme_isattended eq true and crme_dobstring eq '1/12/1923' and crme_ssn eq '666551414'
                    console.log("vet for combinedpersonsearch");
                    console.log(vet);
                    query = "$filter=crme_firstname eq '" + vet.firstname + "' and  crme_lastname eq '" + vet.lastname + "' and  crme_ssn eq '" + vet.ssid + "' and  crme_searchtype eq 'CombinedSearchByFilter' and crme_isattended eq true and crme_udointeractionid eq '00000000-0000-0000-0000-000000000000'";
                    entity = "crme_persons";
                    plugin.$element.xrmTable({
                        name: plugin.options.Name + " Results",
                        //table: "search_results_table",
                        cards: plugin.options.id + "_results",
                        query: query,
                        entity: entity,
                        columns: columns,
                        classes: {
                            table: "SearchResults"
                        },
                        RenderType: "card",
                        debug: debug
                    }).dblclick(function (e) {
                        vetSelected(e).fail(vetSelectedError).always(loadingOff);
                    })
                        .on("postRender", function (e, data) {
                        _private.log(plugin, "Executing postRender.");
                        _private.loadingOff(plugin);

                        plugin.$element.show();
                        plugin.status = Enums.STATUS.COMPLETE;

                        var idp = plugin.$element.find("#idproof_success_1");
                        if (idp !== null) {
                            idp.focus();
                        } else {
                            plugin.$element.focus();
                        }
                    })
                    .on("postRetrieveMultiple", function (e, data) {
                        _private.log(plugin, "Executing postRetrieveMultiple.");
                        if (plugin.status.value === Enums.STATUS.ABORTED.value) {
                            _private.log(plugin, "Executing postRetrieveMultiple ABORTED.");
                            _private.log(plugin, "Error: 499 - Aborted - Client Closed Request");
                            e.error.code = 499;
                            e.error.message = "Aborted";
                            e.preventDefault();
                            return;
                        }
                        // data contains the query, e.data contains the results.
                        var ec = e.arguments[0];

                        if (typeof ec === "undefined" || null === ec) {
                            _private.log(plugin, "Error: 404 - Results undefined or null");
                            e.error.code = 404;
                            e.error.message = "The search criteria did not return any results.";
                            e.preventDefault();
                            return;
                        }
                        var recordcount = ec.length;
                        _private.log(plugin, "Search returned " + recordcount + " records.");
                        
                        if (recordcount === 0) {
                            _private.log(plugin, "Error: 404 - Results empty");
                            e.error.code = 404;
                            e.error.message = "The search criteria did not return any results.";
                            e.preventDefault();
                            return;
                        }

                        if (recordcount > 0) {
                            //console.log(entity);
                            //var errormessage = entity.crme_returnmessage;
                            //if (errormessage && errormessage !== "") {
                            //    _private.log(plugin, "Error: 500 - " + errormessage);
                            //    e.error.code = 500;
                            //    e.error.message = errormessage;
                            //    if (entity.getAttributes().getCount() < 4) {
                            //        e.preventDefault();
                            //        return;
                            //    }
                            //    if (document.getElementById("vetsearch_error")) {
                            //        $("#vetsearch_error").text(e.error.message);
                            //    } else {
                            //        var alertdiv = $("<div/>")
                            //            .addClass(plugin.options.alertInfoClass)
                            //            .attr("id", plugin.options.id + "_error")
                            //            .attr("role", "alert");
                            //        var closebtn = $('<a href="#">&times;</a>')
                            //            .addClass("close")
                            //            .attr("data-dismiss", "alert")
                            //            .attr("aria-label", "close alert")
                            //        alertdiv.append(closebtn, e.error.message);
                            //        plugin.$element.append(alertdiv);
                            //    }
                            //    return;
                            //}
                        }
                    });

                } else {
                    console.log("at else block");
                    plugin.$element.data("xrmTable").setQuery(query);
                }

                return plugin.$element.data("xrmTable").refresh()
                .done(function () {
                    //_private.log(plugin, "search complete");
                    //_private.loadingOff(plugin);

                    // This just means that the refresh is done being sent....
                    plugin.status = Enums.STATUS.COMPLETE;
                })
                .fail(function (e) {
                    _private.log(plugin, "failure during search");
                    _private.loadingOff(plugin);
                    //TODO: POPUP ERROR MESSAGE FOR SEARCH FAILURE
                    var er = e;
                    if (er.error) er = e.error;
                    if (er.code !== 499) {
                        plugin.$element.html("");
                        plugin.$element.append("<div class='alert alert-validationerror' id='vetsearch_error'>" + er.message + "</div>");
                        plugin.$element.show();
                        $("#vetsearch_error").show();
                        $("#vetsearch_error").focus();
                        plugin.status = Enums.STATUS.FAILED;
                    }
                })
                .always(function () {
                    _private.log(plugin, "Executing Refresh Always.");

                    //_private.loadingOff(plugin);
                });
            });
        }
    };

    // The actual plugin constructor
    function Plugin(element, options) {
        console.log(options);
        var plugin = this;
        this.Enums = Enums;
        this.element = element;
        this.$element = $(element);
        
        this.options = $.extend({}, defaults, options);

        this._defaults = defaults;
        this._name = pluginName;

        this.init();
        return this;
    }

    Plugin.prototype = {
        init: function () {
            console.log(this);
            _private.init(this);
        },
        SearchByName: function (vet) {
            return _private.searchByName(this, vet);
        },
        GetVetFromForm: function () {
            return _private.getVetFromForm(this);
        },
        GetStatus: function () {
            return _private.getSearchStatus(plugin);
        },
        StatusEnum: Enums.STATUS,
        AbortSearch: function () {
            _private.abort(this);
        }
    };

    // preventing against multiple instantiations - allows the execution of methods when called with a single item.
    $.fn[pluginName] = function (options) {
        console.log(pluginName);
        if (this.length === 1 && $(this[0]).data("plugin_" + pluginName)) {
            var plugin = $(this[0]).data("plugin_" + pluginName);
            if (plugin[options]) {
                return plugin[options].apply(plugin, Array.prototype.slice.call( arguments, 1 ));
            }
        }
        return this.each(function () {
            var me = $(this);
            if (!me.data("plugin_"+pluginName)) {
                me.data("plugin_"+pluginName, new Plugin(this, options));
            }
        });
    };
})(jQuery, window, document);