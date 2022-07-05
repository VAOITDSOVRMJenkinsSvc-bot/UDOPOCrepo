window.debug = (window.location.search.indexOf("debug") > -1) || (window.location.hash.indexOf("debug") > -1);
var globalContext = Xrm.Utility.getGlobalContext();
var version = globalContext.getVersion();
var lib = new CrmCommonJS.CrmCommon(version, null);
var webApi = lib.WebApi;

window["ENTITY_SET_NAMES"] = window["ENTITY_SET_NAMES"] || JSON.stringify({
    "GlobalOptionSetDefinitions": "GlobalOptionSetDefinitions",
    "udo_sitequeuesettings": "udo_sitequeuesettings",
    "teams": "teams",
    "udo_interactions": "udo_interactions",
    "udo_interaction": "udo_interactions",
    "queueitem": "queueitems",
    "queueitems": "queueitems",
    "queues": "queues",
    "queue": "queues",
    "AddToQueue": "AddToQueue",
    "udo_interactionspecialsituations": "udo_interactionspecialsituations",
    "udo_specialsituations": "udo_specialsituations",
    "PickFromQueue": "PickFromQueue",
    "ReleaseToQueue": "ReleaseToQueue",
    "systemuser": "systemusers",
    "udo_inquirycorrespondences": "udo_inquirycorrespondences",
    "udo_walkinregistration": "udo_walkinregistrations",
    "crme_person": "crme_persons"
});

window["ENTITY_PRIMARY_KEYS"] = ['{"queue":"queueid","udo_interaction":"udo_interactionid"}'];

var CONSTANTS = {
    STATECODE: { ACTIVE: 0, INACTIVE: 1, 0: "Active", 1: "Inactive" },
    STATUSCODE: {
        ACTIVE: 1, INACTIVE: 2, HOLD: 752280000, COMPLETE: 752280001, WALKEDOUT: 752280002,
        0: "Active", 1: "Inactive", 752280000: "Hold", 752280001: "Complete", 752280002: "Walkedout"
    },
    IDPROOF: {
        NOTCHECKED: 752280000, SUCCESS: 752280001, FAILURE: 752280002,
        752280000: "ID Not Checked", 752280001: "IDProof Success", 752280002: "IDProof Failed",
        styles: {
            752280000: "warning", 752280001: "success", 752280002: "danger"
        }
    },
    SENSITIVITY_LEVEL: {
        0: 752280000, 1: 752280001, 2: 752280002, 3: 752280003, 4: 752280004,
        5: 752280005, 6: 752280006, 7: 752280007, 8: 752280008, 9: 752280009
    },
    BUTTONTYPES: {
        ACTION: "action",
        TOOOLBAR: "toolbar"
    }
};

//===============================================================================================================
// GLOBAL TOOLS
//===============================================================================================================
var Tools = function () {
    function setupNumberInputCleaner(jqEl, maxlength) {
        function CleanNumberInput(jqEl, maxlength) {
            var val = jqEl.val();
            val = val.replace(/\D/g, '');
            if (val.length > maxlength) val = val.substring(0, maxlength);
            jqEl.val(val);
        }

        jqEl.keypress(function (e) {
            if (!e) e = window.event;
            if (e.which !== 8 && e.which !== 0 && (e.which < 48 || e.which > 57)) {
                return false;
            }
        });
        jqEl.change(function () {
            CleanNumberInput($(this), maxlength);
        });
        jqEl.keyup(function (e) {
            if (e.which === 86) { // Number only input, so if lowercase V was pressed, then clean it on the keyup.
                CleanNumberInput($(this), maxlength);
                //$this = $(this);
                //setTimeout(function() { CleanNumberInput($this, maxlength); },5);
            }
        });
    }

    function clickOnSpace(jqEl) {
        var spaceClick = function (e) {
            if (e.which === 32 /*|| e.which === 13*/) jqEl.trigger("click");
        };
        jqEl.unbind("keypress", spaceClick);
        jqEl.keypress(spaceClick);
    }

    function formatDate(txtDate) {
        var seperatorCheck = txtDate.replace(/\d+/g, '');
        var dateToReturn;
        if (seperatorCheck === "--") {
            var dateArray = txtDate.split("-");
            dateToReturn = dateArray[1] + "/" + dateArray[2] + "/" + dateArray[0];
        } else {
            dateToReturn = txtDate;
        }
        return dateToReturn;
    }

    function isDate(txtDate, separator) {
        var aoDate,           // needed for creating array and object
            ms,               // date in milliseconds
            month, day, year; // (integer) month, day and year
        // if separator is not defined then set '/'
        if (separator === undefined) {
            separator = '/';
        }
        // split input date to month, day and year
        aoDate = txtDate.split(separator);
        // array length should be exactly 3 (no more no less)
        if (aoDate.length !== 3) {
            return false;
        }
        // define month, day and year from array (expected format is m/d/yyyy)
        // subtraction will cast variables to integer implicitly
        month = aoDate[0] - 1; // because months in JS start from 0
        day = aoDate[1] - 0;
        year = aoDate[2] - 0;
        // test year range
        if (year < 1000 || year > 3000) {
            return false;
        }
        // convert input date to milliseconds
        ms = (new Date(year, month, day)).getTime();
        // initialize Date() object from milliseconds (reuse aoDate variable)
        aoDate = new Date();
        aoDate.setTime(ms);
        // compare input date and parts from Date() object
        // if difference exists then input date is not valid
        if (aoDate.getFullYear() !== year ||
            aoDate.getMonth() !== month ||
            aoDate.getDate() !== day) {
            return false;
        }
        // date is OK, return true
        return true;
    }

    return {
        SetupNumberInputCleaner: setupNumberInputCleaner,
        ClickOnSpace: clickOnSpace,
        IsDate: isDate,
        FormatDate: formatDate
    };
}();

//===============================================================================================================
// XRMFORM: Form Panel Controller with semi-automated mapping in FormController.Load to read data and populate form
//===============================================================================================================
var XrmForm = function (cfg) {
    //---------------------------------------------------------------------------------------------------------------
    // Common, Tools, Utility Functions
    //---------------------------------------------------------------------------------------------------------------
    function _log(msg) {
        if (cfg.DEBUG) console.log(cfg.NAME + ": " + msg);
        if (cfg.ALERTDEBUG) alert(cfg.NAME + ": " + msg);
    }

    function _(selector) {
        return $("#" + selector);
    }

    function _error(e, showOnForm) {
        if (typeof e === "string") {
            _log(e);
            if (!showOnForm) {
                alert(e);
                return;
            }

            var shown = false;
            if (cfg.ERROR) {
                var container = _(cfg.ERROR);
                if (container.length !== 0) {
                    container.text(e);
                    shown = true;
                }
            }
            if (!shown) {
                var pos = _(cfg.ERROR_CONTAINER).children("div").length + 1;
                var err = $("<div id='" + cfg.ERROR + "_" + pos + "' class='alert alert-validationerror' role='alert'/>");
                err.text(e);
                _(cfg.ERROR_CONTAINER).append(err);
            }

            return;
        }
        if (e && e.message) {
            _error(e.message, showOnForm);
        }
    }

    function _formError(e) {
        _error(e, true);
    }

    function _loadingOn() {
        return new _event("loadingOn").call();
    }

    function _loadingOff() {
        return new _event("loadingOff").call();
    }

    function _event(eventOrName, dataObject) {
        var _name = eventOrName;
        var _data = dataObject;
        this.call = function () {
            var dfd = new $.Deferred();
            var event = jQuery.Event(_name);
            event.error = {
                code: 0,
                message: "An error did not occur."
            };

            _log("Triggering Event: " + _name);

            event.arguments = arguments;

            if (_data) {
                if (arguments.length === 0 || (arguments.length === 1 && !arguments[0])) {
                    event.arguments = [_data];
                }
                event.data = _data;
                $.when(_(cfg.PANEL).trigger(event, _data)).done(function () {
                    if (event.isDefaultPrevented()) {
                        dfd.reject(event.error);
                    } else {
                        dfd.resolve.apply(this, event.arguments); //pass thru args
                    }
                });
            } else {
                $.when(_(cfg.PANEL).trigger(event)).done(function () {
                    if (event.isDefaultPrevented()) {
                        dfd.reject(event.error);
                    } else {
                        dfd.resolve.apply(this, event.arguments); //pass thru args
                    }
                });
            }

            return dfd.promise();
        };
    }

    //---------------------------------------------------------------------------------------------------------------
    // Internal Data Object
    //---------------------------------------------------------------------------------------------------------------
    var _internal = {
        FormType: "",
        Buttons: [],
        HiddenData: {}
    };

    //---------------------------------------------------------------------------------------------------------------
    // Form Control Methods
    //---------------------------------------------------------------------------------------------------------------
    var FormControl = {
        _Init: function () {
            _log("XrmForm.FormControl._Init");
            // register any close button to the close method
            var $pbc = _(cfg.PANEL).find(".panel-button-close");
            $pbc.addClass("glyphicon glyphicon-remove");
            $pbc.click(FormControl.Close);
            Tools.ClickOnSpace($pbc);
        },
        _ShowNewFormButtons: function () {
            _log("XrmForm.FormControl._ShowNewFormButtons");
            $.each(_internal.Buttons, function (i, v) {
                if (v.display.onnew) {
                    _(v.id).show();
                } else {
                    _(v.id).hide();
                }
            });
            return $.when();
        },
        _ShowEditFormButtons: function () {
            _log("XrmForm.FormControl._ShowEditFormButtons");
            $.each(_internal.Buttons, function (i, v) {
                if (v.display.onedit) {
                    _(v.id).show();
                } else {
                    _(v.id).hide();
                }
            });
            return $.when();
        },
        _ClearFormErrors: function () {
            _log("XrmForm.FormControl._ClearFormErrors");
            var panel = _(cfg.PANEL);
            panel.find("*").removeClass("form-control-error");
            panel.find(".errorlabel").text("");
            _(cfg.ERROR_CONTAINER).html("");
            $("#scralerts").html("");
            return $.when();
        },
        _ClearHiddenData: function () {
            _log("XrmForm.FormControl._ClearHiddenData");
            _internal.HiddenData = {};
            currentFormType = null;
            return $.when();
        },
        AddButton: function (parentid, name, id, clickHandler, displayOnNew, displayOnEdit) {
            _log("XrmForm.FormControl.AddButton");
            var button = {
                id: id,
                text: name,
                display: {
                    onnew: displayOnNew,
                    onedit: displayOnEdit
                }
            };

            var btn = null;
            var el = document.getElementById(id);
            if (el !== null) {
                btn = $(el);
                btn.unbind("onclick");
            } else {
                btn = $('<a href="#" class="btn btn-primary" role="button"/>');
                btn.attr("id", id);
            }
            btn.text(name);
            btn.click(clickHandler);

            if (!el) _(parentid).append(btn);

            Tools.ClickOnSpace(btn);
            _internal.Buttons.push(button);
            return btn;
        },
        Reset: function () {
            _log("XrmForm.FormControl.Reset");
            _internal.FormType = "";
            var dfd = new $.Deferred(), args = arguments;
            var label = document.getElementById(cfg.TITLE.LABEL);
            for (var style in CONSTANTS.IDPROOF.styles) {
                if ($(label).hasClass("label-" + CONSTANTS.IDPROOF.styles[style]))
                    $(label).removeClass("label-" + CONSTANTS.IDPROOF.styles[style]);
            }
            FormControl._ClearFormErrors()
                .then(FormControl._ClearHiddenData)
                .then(new _event("reset").call)
                .then(function () {
                    var panel = _(cfg.PANEL);
                    panel.find("form").trigger("reset");
                    panel.find("div.hide-on-reset").hide();
                    dfd.resolve.apply(this, args);
                })
                .fail(function () {
                    dfd.reject(this, args);
                });

            return dfd.promise();
        },
        Close: function () {
            _log("XrmForm.FormControl.Close");
            _(cfg.PANEL).hide();
            return FormControl.Reset()
                .then(new _event("close").call)
                .then(FormControl._HideButtons)
                .then(FormControl._ClearHiddenData);
        },
        Show: function () {
            _log("XrmForm.FormControl.Show");
            _(cfg.PANEL).show();
            if (cfg.STARTFOCUS) {
                if (typeof cfg.STARTFOCUS === "function") {
                    _(cfg.STARTFOCUS)();
                } else {
                    _(cfg.STARTFOCUS).focus();
                }
            }
            return _loadingOff().then(new _event("focus").call);
        },
        New: function () {
            _log("XrmForm.FormControl.New");
            _internal.FormType = "new";
            _(cfg.TITLE.LABEL).text(cfg.TITLE.NEWTITLE);
            return FormControl._ShowNewFormButtons()
                .then(new _event("new").call)
                .then(FormControl.Show);
        },
        Load: function (data) {
            _log("XrmForm.FormControl.Load");
            return new _event("load", data).call();
        },
        Edit: function (data) {
            _log("XrmForm.FormControl.Edit");
            _internal.FormType = "existing";
            var title = "Editing Record";
            if (_(cfg.TITLE.OPENTITLE)) {
                if (typeof cfg.TITLE.OPENTITLE === "function") {
                    title = _(cfg.TITLE.OPENTITLE)();
                } else {
                    title = cfg.TITLE.OPENTITLE;
                    if (data && data.title) {
                        title = title.replace("[NAME]", title);
                    }
                }
            }
            _(cfg.TITLE.LABEL).text(title);

            return FormControl._ShowEditFormButtons()
                .then(new _event("open", data).call)
                .then(FormControl.Load)
                .then(FormControl.Show);
        },
        GetFormType: function () {
            _log("XrmForm.FormControl.GetFormType");
            return _internal.FormType;
        },
        GetFormCategory: function () {
            _log("XrmForm.FormControl.GetFormCategory");
            return cfg.NAME.toUpperCase();
        }
    };

    var HiddenData = {
        Add: function (k, v) {
            _log("XrmForm.HiddenData.Add");
            _internal.HiddenData[k] = v;
        },
        Remove: function (k) {
            _log("XrmForm.HiddenData.Remove");
            _internal.HiddenData[k] = undefined;
        },
        Get: function (k) {
            _log("XrmForm.HiddenData.Get");
            return _internal.HiddenData[k];
        },
        Data: function () {
            _log("XrmForm.HiddenData.Data");
            var r = {}, arr = _internal.HiddenData;

            for (var k in arr) {
                if (arr.hasOwnProperty(k) && arr[k] !== undefined) r[k] = arr[k];
            }

            return r;
        }
    };

    FormControl._Init();

    return {
        AddButton: FormControl.AddButton,
        Close: FormControl.Close,
        Edit: FormControl.Edit,
        GetConfig: function () {
            return cfg;
        },
        GetFields: function () {
            return cfg.FIELDS;
        },
        GetFormType: FormControl.GetFormType,
        GetFormCategory: FormControl.GetFormCategory,
        HiddenData: HiddenData,
        Load: function (d) {
            _loadingOn().then(function () {
                return FormControl.Load(d);
            });
        },
        Log: _log,
        New: FormControl.New,
        RaiseError: _formError,
        Reset: FormControl.Reset,
        Show: FormControl.Show
    }
};

//===============================================================================================================
// FORM MANAGER
//===============================================================================================================

var FormManager = function () {
    //This GLOBAL stores the constants associated to the current active form type (walkin / correspondence)
    var currentFormType = null;
    var correspondenceObjectTypeCode = null;
    var vbmsdoctypeObjectTypeCode = null;

    getObjectTypeCodeByLogicalName("udo_inquirycorrespondence");
    getObjectTypeCodeByLogicalName("udo_vbmsdoctype");

    var FORMS = {
        WALKIN: {
            DEBUG: debug,
            ALERTDEBUG: false,
            STARTFOCUS: 'visitor_firstname',
            ERROR: 'walkin_form_error',
            ERROR_CONTAINER: 'walkin_form_error_container',
            FORM_ID: 'panel_walkin_form',
            NAME: 'Walkin',
            PANEL: 'panel_walkin',
            TITLE: {
                LABEL: 'panel_walkin_title',
                NEWTITLE: 'New Intake',
                OPENTITLE: 'Edit Intake'
            },
            ACTIONS: {
                SEARCH_FOR_VET: 'action_searchforvet',
                UPDATE: 'action_update_intake'
            },
            AREAS: {
                FIND_VETERAN_CONTROL: 'find_veteran_control',
                ACTION_AREA: 'form_action_area',
                SEARCH_RESULTS: 'search_results',
                VET_NAME_AREA: 'veteran_name_area'
            },
            FIELDS: {
                ADDITIONAL_DATA: 'additional_data',
                FIND_VETERAN_CONTROL: 'find_veteran_control',
                IS_VET: 'visitor_isvet',
                IS_VET_CONTROL: 'visitor_isvet_control',
                IS_VET_NO: 'visitor_isvet_no',
                IS_VET_YES: 'visitor_isvet_yes',
                SPECIAL_SITUATIONS: 'specialsituations',
                VET_DOB: 'veteran_dob',
                VET_DOB_DIV: 'veteran_dob_div',
                VET_DOB_ERROR: 'veteran_dob_error',
                VET_FIRSTNAME: 'veteran_firstname',
                VET_LASTNAME: 'veteran_lastname',
                VET_MIDDLENAME: 'veteran_middlename',
                VET_SSID: 'veteran_ssid',
                VET_SSID_ERROR: 'veteran_ssid_error',
                VISITOR_FIRSTNAME: 'visitor_firstname',
                VISITOR_FIRSTNAME_ERROR: 'visitor_firstname_error',
                VISITOR_LASTNAME: 'visitor_lastname',
                VISITOR_LASTNAME_ERROR: 'visitor_lastname_error',
                VISITOR_MIDDLENAME: 'visitor_middlename',
                VISITOR_PURPOSE: 'visitor_purpose',
                VISITOR_RELATION: 'visitor_relation',
                VISITOR_RELATION_CONTROL: 'visitor_relation_control'
            }
        },
        CORRESPONDENCE: {
            DEBUG: debug,
            ALERTDEBUG: false,
            STARTFOCUS: 'corr_requestor_firstname',
            NAME: 'Correspondence',
            ERROR: 'correspondence_form_error',
            ERROR_CONTAINER: 'correspondence_form_error_container',
            FORM_ID: 'panel_correspondence_form',
            PANEL: 'panel_correspondence',
            TITLE: {
                LABEL: 'panel_correspondence_title',
                NEWTITLE: 'New Correspondence',
                OPENTITLE: 'Edit Correspondence'
            },
            ACTIONS: {
                SEARCH_FOR_VET: 'action_searchforvet',
                UPDATE: 'action_update_corr'
            },
            AREAS: {
                ACTION_AREA: 'corr_form_action_area',
                FIND_VETERAN_CONTROL: 'find_veteran_control',
                SEARCH_RESULTS: 'corr_search_results',
                VET_NAME_AREA: 'corr_veteran_name_area'
            },
            FIELDS: {
                ADDITIONAL_DATA: 'corr_additional_data',
                IS_VET: 'corr_requestor_isvet',
                IS_VET_CONTROL: 'corr_requestor_isvet_control',
                IS_VET_NO: 'corr_requestor_isvet_no',
                IS_VET_YES: 'corr_requestor_isvet_yes',
                SPECIAL_SITUATIONS: 'corr_specialsituations',
                VET_DOB: 'corr_veteran_dob',
                VET_DOB_DIV: 'corr_veteran_dob_div',
                VET_DOB_ERROR: 'corr_veteran_dob_error',
                VET_FIRSTNAME: 'corr_veteran_firstname',
                VET_LASTNAME: 'corr_veteran_lastname',
                VET_MIDDLENAME: 'corr_veteran_middlename',
                VET_SSID: 'corr_veteran_ssid',
                VET_SSID_ERROR: 'corr_veteran_ssid_error',
                VISITOR_FIRSTNAME: 'corr_requestor_firstname',
                VISITOR_FIRSTNAME_ERROR: 'corr_requestor_firstname_error',
                VISITOR_LASTNAME: 'corr_requestor_lastname',
                VISITOR_LASTNAME_ERROR: 'corr_requestor_lastname_error',
                VISITOR_MIDDLENAME: 'corr_requestor_middlename',
                VISITOR_PURPOSE: 'corr_requestor_purpose',
                VISITOR_RELATION: 'corr_requestor_relation',
                VISITOR_RELATION_CONTROL: 'corr_requestor_relation_control'
            }
        }
    };

    //---------------------------------------------------------------------------------------------------------------
    // FormManager Variables
    //---------------------------------------------------------------------------------------------------------------
    var _forms = {}, _debug = window.debug, _specialsits = [], _teams = [];

    //Dialog controls for lookup elements
    var DialogOptions = new Xrm.DialogOptions();
    DialogOptions.width = 800;
    DialogOptions.height = 600;

    //---------------------------------------------------------------------------------------------------------------
    // Common, Tools, Utility Functions
    //---------------------------------------------------------------------------------------------------------------
    function _log(msg) {
        if (currentFormType.DEBUG) console.log(currentFormType.NAME + ": " + msg);
        if (currentFormType.ALERTDEBUG) alert(currentFormType.NAME + ": " + msg);
    }

    function _(selector) {
        return $("#" + selector);
    }

    function copy(from, to) {
        _(to).val(_(from).val());
    }

    function copy_if_vet(from, to) {
        if (is_vet()) copy(from, to);
    }

    function error(e) {
        if (typeof e === "string") {
            log(e);
            alert(e);
        } else if (e && e.message) {
            error(e.message);
        }
    }

    function loading_on() {
        $("div#tmpDialog").show();
        set_form_buttons_state(false);
        return $.when();
    }

    function loading_off() {
        $("div#tmpDialog").hide();
        set_form_buttons_state(true);
        return $.when();
    }

    function set_form_buttons_state(enabled) {
        if (enabled) {
            $(":button").prop("disabled", false);
        } else {
            $(":button").prop("disabled", true);
        }
    }

    function log(msg) {
        if (_debug) console.log("FormManager: " + msg);
        //alert(msg);
    }

    //---------------------------------------------------------------------------------------------------------------
    // CRM/XRM Utility & Misc Functions
    //---------------------------------------------------------------------------------------------------------------
    function get_client_url() {
        _log("XrmForm.XrmUtils.GetClientUrl");
        try {
            return GetGlobalContext().getClientUrl()
        } catch (t) {
            try {
                return Xrm.Page.context.getClientUrl()
            } catch (t) {
                return "../../.."; //relativePath to before WebResources
            }
        }
    }

    function change_status(stateCode, statusCode, target) {
        function internalChangeStatus(data) {
            log("Changing Status (internal)");
            var dfd = new $.Deferred();
            if (data.targets !== undefined) {
                var entity = {};
                entity.statecode = data.stateCode;
                var entityId = "";
                var entityName = "";
                if (data.targets[0]) {
                    if (data.targets[0].entity) {
                        if (data.targets[0].entity.entityType === "udo_interaction") {
                            entity.statuscode = data.statusCode;
                        } else {
                            entity.statuscode = 2;
                        }
                        entityId = data.targets[0].entity.id;
                        entityName = data.targets[0].entity.entityType;
                    } else {
                        if (data.targets[0].entityType === "udo_interaction") {
                            entity.statuscode = data.statusCode;
                        } else {
                            entity.statuscode = 2;
                        }
                        entityId = data.targets[0].id;
                        entityName = data.targets[0].entityType;
                    }
                    
                } else {
                    if (data.targets.entityType === "udo_interaction") {
                        entity.statuscode = data.statusCode;
                    } else {
                        entity.statuscode = 2;
                    }
                    entityId = data.targets.id;
                    entityName = data.targets.entityType;
                }
                
                webApi.UpdateRecord(entityId, entityName, entity).then(function reply(response) {
                    dfd.resolve(data);
                });
            } else {
                dfd.resolve(data); // nothing to do
            }

            return dfd.promise();
        }

        var data = { stateCode: stateCode, statusCode: statusCode };
        if (target) { //specific item
            data.targets = [target];
        }
        else if (form() && $("#" + currentFormType.PANEL).css('display') !== 'none') {
            if (form().GetFormType() === "new") {
                //new walkin
                return create_interaction(data).then(save_specialsituations).then(internalChangeStatus);
            }
            else if (form().GetFormType() === "existing") {
                //existing item being edited
                var items = QueueManager.GetSelectedQueueItemObjects();
                data.targets = items[0].objectid;
            }
        } else {
            //queueitem
            items = QueueManager.GetSelectedQueueItemObjects();
            data.targets = [];
            for (var i = 0; i < items.length; i++) {
                data.targets.push(items[i].objectid);
            }
        }
        return internalChangeStatus(data);
    }

    function getObjectTypeCodeByLogicalName(entityLogicalName) {
        var otc = -1;
        function successRetrieveEntity(entityLogicalName, entityMetadata) {
            //stop here and see what we get
            otc = entityMetadata.ObjectTypeCode;
            if (entityLogicalName === "udo_inquirycorrespondence") {
                correspondenceObjectTypeCode = otc;
            }
            else if (entityLogicalName === "udo_vbmsdoctype") {
                vbmsdoctypeObjectTypeCode = otc;
            }
        }
        webApi.RetrieveMultiple("EntityDefinitions").then(function success(data) {
            data.forEach(function (entDef) {
                if (entDef.LogicalName === entityLogicalName) {
                    successRetrieveEntity(entityLogicalName, entDef);
                }
            });
        });
    }

    function open_lookup_dialog(objecttypecode, callback) {
        _log("XrmForm.XrmUtils.Lookup");
        var url = get_client_url() + "/_controls/lookup/lookupsingle.aspx?class=null&objecttypes=" + objecttypecode + "&browse=0&ShowNewButton=0&ShowPropButton=1&DefaultType=0";
        Xrm.Internal.openDialog(url, DialogOptions, null, null, callback);
    }

    //---------------------------------------------------------------------------------------------------------------
    // Form Access and Wrapped Form Actions
    //---------------------------------------------------------------------------------------------------------------
    function form() {
        if (!currentFormType) return null;
        try {
            return _forms[currentFormType.NAME.toUpperCase()];
        } catch (e) {
            return null;
        }
    }

    function form_data() {
        return form().HiddenData;
    }

    function open_new_form() {
        return form().New();
    }

    function open_edit_form(data) {
        return form().Edit(data);
    }

    function reset_form() {
        return form().Reset.apply(this, arguments);
    }

    function is_form_category(t) {
        return !!form() && form().GetFormCategory() === t;
    }

    function is_walkin_form() {
        return is_form_category(FORMS.WALKIN.NAME.toUpperCase());
    }

    function is_correspondence_form() {
        return is_form_category(FORMS.CORRESPONDENCE.NAME.toUpperCase());
    }

    //---------------------------------------------------------------------------------------------------------------
    // Initialize Form
    //---------------------------------------------------------------------------------------------------------------
    function create_resolution_dropdown(container, controltype) {
        webApi.RetrieveMultiple("GlobalOptionSetDefinitions")
            .then(function success(data) {
                data.forEach(function (os) {
                    if (os.Name === "udo_resolutiondispositions") {
                        var options = os.Options;

                        var dd;
                        if (controltype === "toolbar") {
                            dd = $(document.getElementById(container));
                        } else {
                            dd = $("<ul class='dropdown-menu'/>");
                        }
                        var item, item_action, o;
                        for (var i = 0; i < options.length; i++) {
                            o = options[i];
                            var osName = o.Label.LocalizedLabels[0].Label;
                            item_action = $("<a/>");
                            item_action.text(osName);
                            item_action.attr("title", osName);
                            item_action.attr("aria-label", osName);
                            item_action.attr("id", "action_qr_" + controltype + "_" + i);

                            var disposition = o.Value;
                            item_action.click(function (e) {
                                var item_disposition = disposition;
                                var panelVisibility = $("#" + currentFormType.PANEL).css('display') !== 'none';
                                if (form() && validate_form("update") && panelVisibility) return;
                                loading_on();

                                form_data().Add("resolutiondisposition", item_disposition);
                                var interactionid;
                                if (!panelVisibility) {
                                    if (QueueManager.SelectedItems === null || QueueManager.SelectedItems.length === 0) return;
                                    var objects = QueueManager.GetSelectedQueueItemObjects();
                                    if (objects.length < 1) return;
                                    var entityType = "";
                                    if (objects[0]) {
                                        interactionid = objects[0].objectid.id;
                                        entityType = objects[0].objectid.entityType;
                                    } else {
                                        interactionid = objects.objectid.id;
                                        entityType = objects.objectid.entityType;
                                    }
                                    
                                    var entity = {};
                                    entity.udo_intakeresolutiondisposition = item_disposition;;
                                    webApi.UpdateRecord(interactionid, entityType, entity).then(function reply(response) {
                                        return change_status(CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.COMPLETE, { entity: response })
                                            .then(form().Close).then(QueueManager.Refresh).then(loading_off);
                                    });
                                    e.preventDefault();
                                    return;
                                }
                                if (form().GetFormType() === "existing") {
                                    update_interaction().then(function (data) {
                                        return change_status(CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.COMPLETE, false);
                                    }).then(form().Close).then(QueueManager.Refresh).then(loading_off)
                                        .done(function () {
                                        })
                                        .fail(function () {
                                            var msg = "Error: Unable to resolve interaction.";
                                            if (e.message !== undefined) msg += "\r\n\r\n" + e.message;
                                            error(msg);
                                        });
                                } else {
                                    change_status(CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.COMPLETE)
                                        .then(form().Close).then(QueueManager.Refresh).then(loading_off)
                                        .done(function () {
                                        })
                                        .fail(function () {
                                            var msg = "Error: Unable to resolve interaction.";
                                            if (e.message !== undefined) msg += "\r\n\r\n" + e.message;
                                            error(msg);
                                        });
                                }
                                e.preventDefault();
                                return false;
                            });
                            item = $("<li/>");
                            item.append(item_action);
                            dd.append(item);

                        }
                        var parent = $(document.getElementById(container));
                        if (controltype === "button") {

                            var btngroup = $("<div class='btn-group'/>");
                            btngroup.append("<a class='btn btn-primary dropdown-toggle' href='#' data-toggle='dropdown'>Quick Resolution<span class='caret'></span></button>");
                            btngroup.append(dd);

                            //btngroup.aria({ type: 'dropdown' });

                            parent.append(btngroup);
                        } else {
                            // toolbars are bound to the link item, not the root li.
                            //parent.parent().aria({ type: 'dropdown' });
                        }
                    }

                });

            });
    }

    // @@@@ need to differentiate this for the two form types
    function init_form_manager() {
        if (currentFormType === null) currentFormType = FORMS.WALKIN;
        
        function init_fields_and_map() {
            var cfg = form().GetConfig();
            var fields = cfg.FIELDS;
            console.log(fields);
            _(cfg.AREAS.SEARCH_RESULTS).VeteranSearch({
                debug: _debug,
                LoadingOn: loading_on,
                LoadingOff: loading_off
            }).on("VeteranSelected", load_veteraninformation);

            var ti = "udo_interaction";

            _(fields.IS_VET_YES).click(update_veteraninformation);
            _(fields.IS_VET_NO).click(update_veteraninformation);
            $("input[name=" + fields.IS_VET + "]:radio").aria({ type: 'radio' });

            //$("input[name=visitor_isvet]:radio").change({ formType: formType }, updateVeteranInformation);
            _(fields.VISITOR_RELATION).bind('input', update_veteranrelation);

            // bind behind the scene update to veteran name if the person is a vet, copy name into vet information
            _(fields.VISITOR_FIRSTNAME).change(function () {
                copy_if_vet(fields.VISITOR_FIRSTNAME, fields.VET_FIRSTNAME);
            });
            _(fields.VISITOR_MIDDLENAME).change(function () {
                copy_if_vet(fields.VISITOR_MIDDLENAME, fields.VET_MIDDLENAME);
            });
            _(fields.VISITOR_LASTNAME).change(function () {
                copy_if_vet(fields.VISITOR_LASTNAME, fields.VET_LASTNAME);
            });

            crm_optionset(ti, _(fields.VISITOR_PURPOSE), "udo_purposeofthevisit");
            crm_optionset(ti, _(fields.VISITOR_RELATION), "udo_relationship");

            crm_input(ti, _(fields.VET_FIRSTNAME), "udo_veteranfirstname");   //WALKIN AND CORR
            crm_input(ti, _(fields.VET_MIDDLENAME), "udo_veteranmiddlename"); //WALKIN AND CORR
            crm_input(ti, _(fields.VET_LASTNAME), "udo_veteranlastname");     //WALKIN AND CORR
            crm_input(ti, _(fields.VET_SSID), "udo_veteranssn");              //WALKIN AND CORR
            crm_input(ti, _(fields.VISITOR_FIRSTNAME), "udo_firstname");
            crm_input(ti, _(fields.VISITOR_MIDDLENAME), "udo_middlename");
            crm_input(ti, _(fields.VISITOR_LASTNAME), "udo_lastname");

            if (is_correspondence_form()) {
                var t = "udo_inquirycorrespondence";

                crm_optionset(t, $("#corr_requestor_relation"), "udo_relationshiptoveteran");
                crm_optionset(t, $("#corr_channel"), "udo_retrievecorrespondence");
                crm_optionset(t, $("#corr_type"), "udo_determinecorrespondence");
                crm_optionset(t, $("#corr_controlled_type"), "udo_controlledcorrespondencetype");

                var maxDate = new Date();
                maxDate.setFullYear(maxDate.getFullYear() + 3);
                // 2021/2022 Removed to use Chrome/Edge date
                //crm_datetime(t, $("#corr_due_date"), "udo_duedate", maxDate);
                
                crm_input(t, $("#corr_determine_purpose"), "udo_purpose");

                crm_input(t, $("#corr_requestor_firstname"), "udo_requestorfirstname");
                crm_input(t, $("#corr_requestor_lastname"), "udo_requestorlastname");
                crm_input(t, $("#corr_requestor_phone"), "udo_requestorphonenumber");
                crm_input(t, $("#corr_requestor_email"), "udo_requestoremailaddress");
                crm_input(t, $("#corr_requestor_address1"), "udo_requestoraddress_line1");
                crm_input(t, $("#corr_requestor_address2"), "udo_requestoraddress_line2");
                crm_input(t, $("#corr_requestor_address3"), "udo_requestoraddress_line3");
                crm_input(t, $("#corr_requestor_city"), "udo_requestoraddress_city");
                crm_input(t, $("#corr_requestor_state"), "udo_requestoraddress_state");
                crm_input(t, $("#corr_requestor_zip"), "udo_requestoraddress_postalcode");

                if ($(currentFormType.FIELDS.IS_VET_YES).is(":checked")) {
                    crm_input(ti, _(fields.VISITOR_FIRSTNAME), "udo_veteranfirstname");
                    crm_input(ti, _(fields.VISITOR_MIDDLENAME), "udo_veteranmiddlename");
                    crm_input(ti, _(fields.VISITOR_LASTNAME), "udo_veteranlastname");
                }
            }
            
            // 2021/2022 Removed to use Chrome/Edge date
            //crm_datetime(ti, _(fields.VET_DOB), "udo_veterandob", new Date());//WALKIN AND CORR

            Tools.SetupNumberInputCleaner(_(fields.VET_SSID), 9);

            _(fields.SPECIAL_SITUATIONS).magicSuggest({});

            $(".panel-title").attr("tabindex", "0");

            return retrieve_specialsituations(cfg);
        }

        function init_required_fields() {
            var field_selectors = ["#" + form().GetFields().VISITOR_FIRSTNAME, "#" + form().GetFields().VISITOR_LASTNAME];
            if (currentFormType.NAME === "Correspondence") {
                field_selectors.push("#" + form().GetFields().VISITOR_RELATION);
            }

            var i, sel, id;
            for (i = 0; i < field_selectors.length; i++) {
                sel = field_selectors[i];
                $(sel).attr("required", "required");

                $(sel).each(function () {
                    var id = $(this).attr("name");
                    if (!id || id === "") id = $(this).attr("id");
                    var label = $("label[for='" + id + "']");
                    label.addClass("required");
                    var title = label.attr("title");
                    if (!title) title = "";
                    title += " (Required)";
                    title = title.trim();
                    label.attr("title", title);
                    label.attr("aria-label", title);
                });
            }
            var search_field_selectors = [
                "#" + form().GetFields().VET_FIRSTNAME,
                "#" + form().GetFields().VET_LASTNAME,
                "#" + form().GetFields().VET_SSID,
                "#" + form().GetFields().VET_DOB];
            if (currentFormType.NAME === "Walkin") {
                search_field_selectors.push("#" + form().GetFields().VISITOR_RELATION);
            }
            for (i = 0; i < search_field_selectors.length; i++) {
                sel = search_field_selectors[i];

                if (is_correspondence_form()) {
                    $(sel).attr("required", "recommended");
                } else if (is_walkin_form()) {
                    $(sel).attr("required", "required for search");
                }

                $(sel).each(function () {
                    var id = $(this).attr("name");
                    if (!id || id === "") id = $(this).attr("id");
                    var label = $("label[for='" + id + "']");
                    label.addClass("recommended");
                    var title = label.attr("title");
                    if (!title) title = label.text();
                    title += " (Required for search)";
                    title = title.trim();
                    label.attr("title", title);
                    label.attr("aria-label", title);
                });
            }
        }

        function init_panelform(formType) {
            currentFormType = formType;
            var currentForm = (_forms[formType.NAME.toUpperCase()] = new XrmForm(formType));
            _(formType.PANEL)
                .on("loadingOn", loading_on)
                .on("loadingOff", loading_off);
            currentForm.Close();
            init_fields_and_map();
            init_required_fields();
            show_or_hide_veteran_area();
        }
        init_panelform(FORMS.CORRESPONDENCE);
        init_panelform(FORMS.WALKIN);
        retrieve_button_routes().then(post_retrieve_button_routes)
            .done(function () {
            init_actions();
            currentFormType = FORMS.WALKIN;
            action_new_walkin();
            });

        FormManager.Form = {
            Panel: function () {
                return _(currentFormType.PANEL);
            },
            HtmlForm: function () {
                return _(currentFormType.FORM_ID);
            },
            title: function () {
                if (arguments.length === 0) return $("#" + currentFormType.TITLE).text();
                _(currentFormType.TITLE).html("").text(arguments[0]);
            },
            hide: function () {
                $("#" + currentFormType.PANEL).hide();
            },
            show: function () {
                $("#" + currentFormType.PANEL).show();
            },
            reset: function () {
                $("#" + currentFormType.PANEL).trigger("reset");
            },
            Category: function () {
                return currentFormType.Name;
            }
        };

        /* @@@@ need to differentiate this code based on which queue we're in. Correspondence queue should hide and show different menu options. */
        /* also need to actually define the needed menu options for correspondence */

        QueueManager.Initialize({
            ShowQueueItemActions: function (args) {
                $("#action_open").show();
                $("#action_route").show();
                $("#action_walkout").show();
                $("#action_complete").show();
                if (typeof args.IsWorker === "undefined" || args.IsWorker === null) {
                    $("#action_pick").show();
                    $("#action_release").show();
                } else if (args.IsWorker === true) {
                    $("#action_pick").hide();
                    $("#action_release").show();
                } else if (args.IsWorker === false) {
                    $("#action_pick").hide();
                    $("#action_release").hide();
                } else if (args.IsWorker === "none") {
                    $("#action_pick").show();
                    $("#action_release").hide();
                }
            },
            HideQueueItemActions: function () {
                $("#action_open").hide();
                $("#action_route").hide();
                $("#action_pick").hide();
                $("#action_release").hide();
                $("#action_walkout").hide();
                $("#action_complete").hide();
            },
            UseShortNames: false,
            LoadingOn: loading_on,
            LoadingOff: loading_off,
            debug: _debug //debug
        });

        $("#action_route").hide();
        $("#action_pick").hide();
        $("#action_release").hide();
        $("#action_walkout").hide();
        $("#action_complete").hide();

        _(form().GetFields().VET_SSID).forceNumeric();
    }

    //---------------------------------------------------------------------------------------------------------------
    // Interaction & Queue Items
    //---------------------------------------------------------------------------------------------------------------
    function create_interaction_and_queue(queueid) {
        return create_interaction({ queueid: queueid }).then(save_specialsituations).then(create_queueitem);
    }

    function create_interaction(data) {
        var interactionToCreate = {};
        //TODO: remember to set correspondence and letter ids after read_form
        //NOTE: udo_veterandob is a string field in CRM; the form attribute is a datepicker so we need to override the date object with a string
        interactionToCreate.udo_veterandob = Tools.FormatDate(_(currentFormType.FIELDS.VET_DOB).val());
        interactionToCreate.udo_veteranssn = _(currentFormType.FIELDS.VET_SSID).val();
        
        if (currentFormType === FORMS.CORRESPONDENCE && !is_vet()) {
            if (_(currentFormType.FIELDS.VET_SSID).val() !== null && _(currentFormType.FIELDS.VET_SSID).val() !== "") {
                interactionToCreate.udo_title = [currentFormType.NAME + ":", _(currentFormType.FIELDS.VET_FIRSTNAME).val(), _(currentFormType.FIELDS.VET_LASTNAME).val()].join(" ");
                interactionToCreate.udo_firstname = _(currentFormType.FIELDS.VET_FIRSTNAME).val();
                interactionToCreate.udo_lastname = _(currentFormType.FIELDS.VET_LASTNAME).val();
                interactionToCreate.udo_veteranfirstname = _(currentFormType.FIELDS.VET_FIRSTNAME).val();
                interactionToCreate.udo_veteranlastname = _(currentFormType.FIELDS.VET_LASTNAME).val();
            } else {
                interactionToCreate.udo_title = [currentFormType.NAME + ":", _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val(), _(currentFormType.FIELDS.VISITOR_LASTNAME).val()].join(" ");
                interactionToCreate.udo_firstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
                interactionToCreate.udo_lastname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
                interactionToCreate.udo_veteranfirstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
                interactionToCreate.udo_veteranlastname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
            }
        } else {
            interactionToCreate.udo_title = [currentFormType.NAME + ":", _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val(), _(currentFormType.FIELDS.VISITOR_LASTNAME).val()].join(" ");
            interactionToCreate.udo_firstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
            interactionToCreate.udo_lastname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
            interactionToCreate.udo_veteranfirstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
            interactionToCreate.udo_veteranlastname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
        }
        var resolutiondisposition = form_data().Get("resolutiondisposition");
        if (resolutiondisposition > 0) {
            interactionToCreate.udo_intakeresolutiondisposition = resolutiondisposition;
        }

        var itype = _(currentFormType.FIELDS.VISITOR_PURPOSE).val();
        if (itype !== null && itype !== '') {
            interactionToCreate.udo_purposeofthevisit = itype;
        }

        // Update to ID Proof
        var idproof = form_data().Get("idproof");
        if (idproof > 0) {
            interactionToCreate.udo_intakeidproof = idproof;
        }
        if (is_walkin_form()) {
            interactionToCreate.udo_channel = 752280002;
            interactionToCreate.udo_interactiontype = 752280003;
        } else if (is_correspondence_form()) {
            interactionToCreate.udo_channel = 752280003;
            interactionToCreate.udo_interactiontype = 752280004;
            var correspondenceid = form_data().Get("correspondenceid");
            if (correspondenceid) {
                interactionToCreate["udo_correspondenceid@odata.bind"] = "/udo_inquirycorrespondences(" + correspondenceid.replace(/[{}]/g, "") + ")";
            }
        }
        interactionToCreate.udo_nophonenumberavailable = true;

        var sensitivitylevel = 0;
        var sensitivity_level_str = form_data().Get("sensitivity_level");
        if (sensitivity_level_str) {
            sensitivitylevel = parseInt(sensitivity_level_str);
        }

        if (sensitivitylevel < 10) sensitivitylevel = 752280000 + sensitivitylevel;

        form_data().Add("sensitivity_level", sensitivitylevel);

        var dfd = new $.Deferred();

        var team_sensitivitylevel = 752280001;
        if (sensitivitylevel > team_sensitivitylevel) team_sensitivitylevel = sensitivitylevel;
        retrieve_team(team_sensitivitylevel).then(post_retrieve_team).then(function (teamid) {
            interactionToCreate["ownerid@odata.bind"] = "/teams(" + teamid + ")";
            return webApi.CreateRecord(interactionToCreate, "udo_interactions").then(function success(interaction) {
                data.interaction = interaction;
                dfd.resolve(data);
            });
        });
        return dfd.promise();
    }

    function create_queueitem(data) {
        webApi.RetrieveRecord(data.interaction.id, data.interaction.entityType).done(function (interaction) {
            var entity = {};

            var title = interaction.udo_title;
            if (title === null) {
                title = form().GetConfig().NAME;
            }
            else if (title.length > 8) {
                title = title.substring(8);
            }
            entity.title = title;
            entity.udo_purposeofvisit = interaction.udo_purposeofthevisit;
            // Interaction Type
            var itype = interaction.udo_purposeofthevisit;
            if (itype !== null) {
                entity.udo_purposeofvisit = itype;
            }

            // Interaction Sub Type
            entity.udo_firstname = interaction.udo_firstname;
            entity.udo_lastname = interaction.udo_lastname;
            if (form_data().Get("siteid")) {
                entity["udo_siteid@odata.bind"] = "/udo_sites(" + form_data().Get("siteid") + ")";
            }
            var sensitivity_level = form_data().Get("sensitivity_level");
           if (!sensitivity_level) {
                sensitivity_level = CONSTANTS.SENSITIVITY_LEVEL[0];
            }
            if (sensitivity_level >= 752280000) {
               sensitivity_level = parseInt(sensitivity_level) - 752280000;
            }
            form_data().Add("sensitivity_level", sensitivity_level);
           entity.udo_sensitivitylevel = parseInt(sensitivity_level);
            if (data.specialsituations) {
                entity.udo_specialsituationyn = data.specialsituations;
            } else {
                entity.udo_specialsituationyn = false;
            }
            var corrid = form_data().Get("correspondenceid");
            if (corrid) {
                entity.udo_correspondenceid = "/udo_inquirycorrespondences(" + corrid.replace(/[{}]/g, "") + ")";
            }
            entity["objectid_udo_interaction@odata.bind"] = "/udo_interactions(" + interaction.udo_interactionid + ")";
            entity["queueid@odata.bind"] = "/queues(" + data.queueid + ")";
            return webApi.CreateRecord(entity, "queueitem").done(function success(qe) {
                data.queueitemid = qe.id;
                return data;
            });
        });
    }

    function add_to_queue(data) {
        return QueueManager.AddToQueue(data).then(function (reply) {
        });
    }

    function retrieve_interaction(objectid) {
        return webApi.RetrieveRecord(objectid.objectid.id, objectid.objectid.entityType).then(function success(result) {
            result.entityName = objectid.objectid.entityType;
            return result;
        });
    }

    function retrieveSpecialSituations(interactionId) {
        var filter = "$filter=_udo_interactionid_value eq " + interactionId;

        return webApi.RetrieveMultiple("udo_interactionspecialsituations", null, filter).then(function reply(response) {
            return response;
        });
    }

    function post_retrieve_interaction(resp) {
        var interaction = resp;
        if (interaction.entityName === "udo_interaction") {
            $("#" + form().GetFields().VISITOR_FIRSTNAME).val(interaction.udo_firstname);
            $("#" + form().GetFields().VISITOR_LASTNAME).val(interaction.udo_lastname);
            $("#" + form().GetFields().VISITOR_PURPOSE).val(interaction.udo_purposeofthevisit);
        } else {
            $("#" + form().GetFields().VISITOR_FIRSTNAME).val(interaction.udo_veteranfirstname);
            $("#" + form().GetFields().VISITOR_LASTNAME).val(interaction.udo_veteranlastname);
        }
        
        
        var actions = [];
        if (interaction.udo_interactionid) {
            retrieveSpecialSituations(interaction.udo_interactionid).then(function (specialsituations) {
                load_specialsituations(specialsituations);
            });
        }
        
        var correspondenceId = interaction.udo_correspondenceid;
        if (!correspondenceId || (correspondenceId === null)) {
            currentFormType = FORMS.WALKIN;
        }
        else {
            currentFormType = FORMS.CORRESPONDENCE;
            form_data().Add("correspondenceid", correspondenceId);
            $('#correspondence_lookup_name').val(correspondenceIdname);
            retrieve_correspondence(correspondenceId);
        }
        _(currentFormType.TITLE.LABEL).text(currentFormType.TITLE.OPENTITLE);
        show_or_hide_veteran_area();
        var idproof = interaction.udo_intakeidproof;
        if (idproof && idproof > 0) {
            form_data().Add("idproof", idproof);
        }
        var dfd = new $.Deferred();
        $.when(actions).then(function () {
            dfd.resolve({ entity: interaction });
        });
        return dfd.promise();
    }

    function update_interaction() {
        loading_on();
        var entity = {};
        var objects = QueueManager.GetSelectedQueueItemObjects();
        //NOTE: udo_veterandob is a string field in CRM; the form attribute is a datepicker so we need to override the date object with a string
        entity.udo_veterandob = Tools.FormatDate(_(currentFormType.FIELDS.VET_DOB).val());
        entity.udo_veteranssn = _(currentFormType.FIELDS.VET_SSID).val();
		
        /* Set properties specific to all walkins */
        if (currentFormType === FORMS.CORRESPONDENCE && !is_vet()) {
            if (_(currentFormType.FIELDS.VET_SSID).val() !== null && _(currentFormType.FIELDS.VET_SSID).val() !== "") {
                entity.udo_title = [currentFormType.NAME + ":", _(currentFormType.FIELDS.VET_FIRSTNAME).val(), _(currentFormType.FIELDS.VET_LASTNAME).val()].join(" ");
                entity.udo_firstname = _(currentFormType.FIELDS.VET_FIRSTNAME).val();
                entity.udo_lastname = _(currentFormType.FIELDS.VET_LASTNAME).val();
                entity.udo_veteranfirstname = _(currentFormType.FIELDS.VET_FIRSTNAME).val();
                entity.udo_veteranfirstname = _(currentFormType.FIELDS.VET_LASTNAME).val();
            } else {
                entity.udo_title = [currentFormType.NAME + ":", _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val(), _(currentFormType.FIELDS.VISITOR_LASTNAME).val()].join(" ");
                entity.udo_firstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
                entity.udo_lastname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
                entity.udo_veteranfirstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
                entity.udo_veteranfirstname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
            }
        } else {
            entity.udo_title = [currentFormType.NAME + ":", _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val(), _(currentFormType.FIELDS.VISITOR_LASTNAME).val()].join(" ");
            entity.udo_firstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
            entity.udo_lastname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
            entity.udo_veteranfirstname = _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val();
            entity.udo_veteranfirstname = _(currentFormType.FIELDS.VISITOR_LASTNAME).val();
        }

        if (is_walkin_form()) {
            entity.udo_channel = 752280002;
            entity.udo_interactiontype = 752280003;
        } else if (is_correspondence_form()) {
            entity.udo_channel = 752280003;
            entity.udo_interactiontype = 752280004;
        }
        entity.udo_nophonenumberavailable = true;

        var resolutiondisposition = form_data().Get("resolutiondisposition");
        if (resolutiondisposition && resolutiondisposition > 0) {
            entity.udo_intakeresolutiondisposition = resolutiondisposition;
        }

        // Update to ID Proof
        var idproof = form_data().Get("idproof");
        if (idproof && idproof > 0) {
            entity.udo_intakeidproof = idproof;
        }
        var data = { entity: entity };

        data.queueitemid = form_data().Get("queueitemid");
        var dfd = new $.Deferred();
        webApi.UpdateRecord(objects[0].objectid.id, "udo_interactions", entity).then(function reply(response) {
            loading_off();
            dfd.resolve(data);
        });

        return dfd.promise();
    }

    //---------------------------------------------------------------------------------------------------------------
    // Correspondence
    //---------------------------------------------------------------------------------------------------------------
    function create_or_update_correspondence() {
        var dfd = new $.Deferred();
        if (!is_correspondence_form()) {
            dfd.resolve();
            return dfd.promise();
        }
        var entity = {};

        var isvet = is_vet();
        entity.udo_isveteran = isvet;
        if (isvet) {
            entity.udo_requestorfirstname = $("#corr_veteran_firstname").val();
            entity.udo_requestorlastname = $("#corr_veteran_lastname").val();
            entity.udo_requestoremailaddress = $("#corr_veteran_email").val();
            entity.udo_requestorphonenumber = $("#corr_veteran_phone").val();
        } else {
            var vetRelationship = $("#corr_requestor_relation").val();
            if (vetRelationship === "") {
                entity.udo_relationshiptoveteran = 752280000;
            } else {
                entity.udo_relationshiptoveteran = parseInt(vetRelationship);
            }
        }

        var letterid = form_data().Get("letterid");
        if ($("#corr_interim_action_yes").is(":checked") && $("#corr_letter_lookup_name").val() !== "" && letterid && letterid !== "") {
            entity["udo_interimletterid@odata.bind"] = "/udo_vbmsdoctype(" + letterid.replace(/[{}]/g, "") + ")"
            entity.udo_interimletterid = {};
            entity.udo_interimletterrequired = true;
        } else {
            entity.udo_interimletterrequired = false;
        }

        if (form().GetFormType() === "existing"/* || Condition: formtype = New && Correspondence selected (NOTE: Need to ensure correspondence is deselected at appropriate locations)*/) {
            var data = { entity: entity };
            return webApi.UpdateRecord(form_data().Get("correspondenceid").replace(/[{}]/g, ""), "udo_inquirycorrespondences", entity).then(
                function reply(response) {
                    dfd.resolve(data);
                });
        } else {
            //Assign all Correspondence Records to the team for SL1
            var team_sensitivitylevel = 752280001;
            retrieve_team(team_sensitivitylevel).then(post_retrieve_team).then(function (teamid) {
                var data = { entity: entity };
                return webApi.CreateRecord(entity, "udo_inquirycorrespondences").then(function reply(response) {
                    form_data().Add("correspondenceid", id);
                    data.entity.setId(id);
                    dfd.resolve(data);
                });
            });
        }
        return dfd.promise();
    }

    function retrieve_correspondence(id) {
        // Is this neccessary?
        _forms[FORMS.WALKIN.NAME.toUpperCase()].Close();
        var correspondenceColumns = ["udo_relationshiptoveteran", "udo_purpose",
            "udo_duedate", "udo_retrievecorrespondence", "udo_determinecorrespondence",
            "udo_controlledcorrespondencetype", "udo_requestorfirstname", "udo_requestorlastname",
            "udo_requestorphonenumber", "udo_requestoremailaddress", "udo_requestoraddress_line1",
            "udo_requestoraddress_line2", "udo_requestoraddress_line3", "udo_requestoraddress_city",
            "udo_requestoraddress_state", "udo_requestoraddress_postalcode", "udo_interimletterid",
            "udo_name"];

        webApi.RetrieveRecord(id.replace(/[{}]/g, ""), "udo_inquirycorrespondences", correspondenceColumns).then(function reply(response) {
            post_retrieve_correspondence(response);
        });
    }

    function post_retrieve_correspondence(resp) {
        var correspondence = resp.getEntity();
        $(".errorlabel").text("");
        _(currentFormType.ERROR_CONTAINER).html("");

        form().Load({ entity: correspondence });

        //TODO: Add Letter from retrieve to context and field text
        var relationship = correspondence.getValue("udo_relationshiptoveteran", true);
        if (relationship && relationship === 752280000) {
            $("#corr_requestor_relation").val(relationship);
            _(form().GetFields().IS_VET_NO).prop("checked", false);
            _(form().GetFields().IS_VET_YES).prop("checked", true);
        } else {
            $("#corr_requestor_relation").val(relationship);
            _(form().GetFields().IS_VET_NO).prop("checked", true);
            _(form().GetFields().IS_VET_YES).prop("checked", false);
        }

        var controlledcorrtype = correspondence.getValue("udo_controlledcorrespondencetype", true);
        if (controlledcorrtype) {
            $("#corr_controlled_type").val(controlledcorrtype);
        }
        if (correspondence.getValue("udo_interimletterid", true)) {
            $("#corr_letter_lookup_name").val(correspondence.getValue("udo_interimletterid", true).getName());
            form_data().Add("letterid", correspondence.getValue("udo_interimletterid", true).getId());
            _("corr_interim_action_yes").prop("checked", true);
            _("corr_interim_action_no").prop("checked", false);
        } else {
            _("corr_interim_action_yes").prop("checked", false);
            _("corr_interim_action_no").prop("checked", true);
        }
        $('#correspondence_lookup_name').val(correspondence.getValue("udo_name", true));

        var dfd = new $.Deferred();

        show_or_hide_veteran_area().then(function () {
            dfd.resolve({ entity: correspondence }).then(loading_off);
        });

        return dfd.promise();
    }

    //---------------------------------------------------------------------------------------------------------------
    // Reasons
    //---------------------------------------------------------------------------------------------------------------

    //---------------------------------------------------------------------------------------------------------------
    // Special Situations
    //---------------------------------------------------------------------------------------------------------------
    function retrieve_specialsituations(formconfig) {
        if (_specialsits && _specialsits.length > 0) return setup_specialsituations(formconfig); // use local cache
        // no specialsituation data in _specialsits cache, get from crm...
        var filter = "$filter=statecode eq 0";
        return webApi.RetrieveMultiple("udo_specialsituations", ["udo_specialsituationid", "udo_name"], filter).then(function reply(response) {
            post_retrieve_specialsituations(response, formconfig);
        });
    }

    function post_retrieve_specialsituations(ec, formconfig) {
        var c = ec.length, e, a;
        if (_specialsits.length === 0) {
            for (var i = 0; i < c; i++) {
                e = ec[i];
                _specialsits.push({ id: e.udo_specialsituationid, name: e.udo_name, fromCRM: true });
            }
        }
        return setup_specialsituations(formconfig);
    }

    function setup_specialsituations(formconfig) {
        var field = formconfig.FIELDS.SPECIAL_SITUATIONS;
        if (field) {
            var ms = _(field).magicSuggest({});
            ms.setData(_specialsits);
        }
        return $.when();
    }

    function load_specialsituations(specialsituations) {
        var elid = form().GetFields().SPECIAL_SITUATIONS;
        if (!elid) return;
        var ms = _(elid).magicSuggest({});
        var options = _specialsits.slice(0), i, k, o, selected = [], e, eid;

        for (i = 0; i < specialsituations.length ; i++) {
            e = specialsituations[i];
            for (k = options.length - 1; k >= 0; k--) {
                o = options[k];
                eid = e.id;
                if (o.id === eid) {
                    options.splice(k, 1); //remove
                }
            }
            o = {
                id: e.udo_other,
                name: e.udo_other,
                existingid: e.udo_interactionspecialsituationid,
                fromCRM: true,
                existing: true
            };
            o.id = e["_udo_specialsituationid_value"];
            o.name = e["_udo_specialsituationid_value@OData.Community.Display.V1.FormattedValue"];
            options.push(o);
            selected.push(o.id);
        }

        form_data().Add("specialsituations", options);
        ms.setValue(selected);

        return $.when();
    }

    function save_specialsituations(data) {
        var i, dfd = new $.Deferred()
            , interactionid = data.interaction.id
            , ms = _(form().GetFields().SPECIAL_SITUATIONS).magicSuggest({})
            , items = ms.getSelection()
            , item
            , selected = [];
        data.specialsituations = items.length > 0;

        var existing = form_data().Get("specialsituations") || [];
        var findSpecialSituation = function (id, existing) {
            for (var j = 0; j < existing.length; j++) {
                if (existing[j].id === item.id) return existing[j];
            }
            return null;
        };

        for (i = 0; i < items.length; i++) {
            item = items[i];
            selected.push(item.id);
            var special = findSpecialSituation(item.id, existing);
            if (!special || !special.existing) {
                if (item.fromCRM !== undefined) {
                    create_specialsituation(interactionid, item.id, item.name);
                } else {
                    create_specialsituation(interactionid, null, item.name);
                }
                dowork = true;
            }
        }

        if (existing !== null) {
            for (i = 0; i < existing.length; i++) {
                item = existing[i];
                if ($.inArray(item.id, selected) !== -1) continue;
                if (!item.existing) continue;
                webApi.DeleteRecord(item.existingid, "udo_interactionspecialsituations");
                dowork = true;
            }
        }
        dfd.resolve(data);
        return dfd.promise();
    }

    function create_specialsituation(interactionid, situationid, name) {
        var specialStipulationToCreate = {};
        specialStipulationToCreate["udo_interactionid@odata.bind"] = "/udo_interactions(" + interactionid + ")";
        if (situationid !== undefined && situationid !== null) {
            specialStipulationToCreate["udo_specialsituationid@odata.bind"] = "/udo_specialsituations(" + situationid + ")";
        } else {
            specialStipulationToCreate.udo_other = name;
        }
        specialStipulationToCreate.udo_name = name;
        return webApi.CreateRecord(specialStipulationToCreate, "udo_interactionspecialsituations").then(function success(reply) {
            return;
        });
        
    }

    //---------------------------------------------------------------------------------------------------------------
    // Team
    //---------------------------------------------------------------------------------------------------------------
    function retrieve_team(sensitivitylevel) {
        var dfd = new $.Deferred();
        if (_teams[sensitivitylevel]) {
            dfd.resolve(_teams[sensitivitylevel], sensitivitylevel);
            return dfd.promise();
        }

        var fetch = "<fetch><entity name='team'><attribute name='teamid'/>"
            + "<link-entity name='businessunit' from='businessunitid' to='businessunitid'>"
            + "<filter><condition attribute='udo_veteransensitivitylevel' operator='eq' value='" + sensitivitylevel + "' /></filter>"
            + "</link-entity><filter>"
            + "<condition attribute='name' operator='eq' value='PCR'/>"
            + "</filter></entity></fetch>";

        var endcodedFetchXml = encodeURI(fetch);
        return webApi.RetrieveMultiple("teams?fetchXml=" + endcodedFetchXml).then(function success(data) {
            return data;
        });
    }

    function post_retrieve_team(ec, sensitivitylevel) {
        var dfd = new $.Deferred();
        if (!ec || !ec.length || ec.length === 0) {
            dfd.reject("Unable to find business unit");
            return dfd.promise();
        }
        _teams[sensitivitylevel] = ec;

        var team = ec[0];

        dfd.resolve(team.teamid);
        return dfd.promise();
    }

    //---------------------------------------------------------------------------------------------------------------
    // CRM Form Mapping and Copying
    //---------------------------------------------------------------------------------------------------------------
    function crm_input(et, el, v, t) {
        if (!(el instanceof jQuery)) el = $(el);
        var xrm = el.data("xrm");
        if (!xrm) xrm = [];
        var map = {
            Entity: et,
            LogicalName: v
        };

        if (t) map.Type = t;
        xrm.push(map);
        el.data("xrm", xrm);
        return el;
    }

    function crm_datetime(et, el, v, maxDate, defaultDate) {
        if (!defaultDate) defaultDate = '';
        return crm_input(et, el, v, "datetime").datepicker({
            dateFormat: 'm/d/yy',
            minDate: new Date(1776, 6, 4),
            maxDate: maxDate,
            defaultDate: defaultDate
        });
    }

    function crm_optionset(entity, el, attribute) {
        return webApi.RetrieveMultiple("GlobalOptionSetDefinitions").then(function success(optionSets) {
            optionSets.forEach(function (os) {
                if (os.Name === attribute) {
                    setup_options(el, os.Options, null);
                    crm_input(entity, el, os.Name, "OptionSet");
                    return;
                }
            });
        })
    }

    function setup_options(el, options, defaultOption) {
        var r = [];
        if (!defaultOption) {
            r.push($('<option value="">None</option>'));
        }
        for (var i = 0; i < options.length; i++) {
            var o = options[i];
            var e = $("<option/>");
            e.attr('value', o.Value);
            e.text(o.Label.UserLocalizedLabel.Label);
            e.attr('aria-label', o.Label.UserLocalizedLabel.Label);
            if (o.Value === defaultOption) e.attr("selected", true);
            r.push(e);
        }
        el.empty();
        el.append(r);
    }

    //---------------------------------------------------------------------------------------------------------------
    // Veteran Information and Methods
    //---------------------------------------------------------------------------------------------------------------
    function is_vet() {
        return _(form().GetFields().IS_VET_YES)[0].checked || _(form().GetFields().VISITOR_RELATION).val() === 752280000;
    }

    function show_or_hide_veteran_area() {
        if (is_vet()) {
            _(form().GetConfig().AREAS.VET_NAME_AREA).hide();
            _(form().GetFields().IS_VET_NO).prop("checked", false);
            _(form().GetFields().IS_VET_YES).prop("checked", true);
            _(form().GetFields().IS_VET_CONTROL).show();
            _(form().GetFields().VISITOR_RELATION_CONTROL).hide();
        } else {
            _(form().GetConfig().AREAS.VET_NAME_AREA).show();
            _(form().GetFields().IS_VET_NO).prop("checked", true);
            _(form().GetFields().IS_VET_YES).prop("checked", false);
            _(form().GetFields().IS_VET_CONTROL).hide();
            _(form().GetFields().VISITOR_RELATION_CONTROL).show();
        }
        return $.when();
    }

    function update_veteraninformation() {
        if (_(form().GetFields().IS_VET_YES)[0].checked) {
            _(form().GetConfig().AREAS.VET_NAME_AREA).hide();
            _(form().GetFields().IS_VET_CONTROL).show();
            _(form().GetFields().VISITOR_RELATION_CONTROL).hide();
            _(form().GetFields().VISITOR_RELATION).val(752280000);
            copy(form().GetFields().VISITOR_FIRSTNAME, form().GetFields().VET_FIRSTNAME);
            copy(form().GetFields().VISITOR_MIDDLENAME, form().GetFields().VET_MIDDLENAME);
            copy(form().GetFields().VISITOR_LASTNAME, form().GetFields().VET_LASTNAME);
        } else {
            _(form().GetFields().VISITOR_RELATION).val("");
            _(form().GetFields().VET_FIRSTNAME).val("");
            _(form().GetFields().VET_MIDDLENAME).val("");
            _(form().GetFields().VET_LASTNAME).val("");
            _(form().GetFields().IS_VET_CONTROL).hide();
            _(form().GetFields().VISITOR_RELATION_CONTROL).show();
            _(form().GetConfig().AREAS.VET_NAME_AREA).show();
            _(form().GetFields().VISITOR_RELATION).focus();
        }
    }

    function update_veteranrelation() {
        var yes = _(form().GetFields().VISITOR_RELATION).val() === 752280000;
        if (yes) {
            _(form().GetFields().IS_VET_NO).prop("checked", false);
            _(form().GetFields().IS_VET_YES).prop("checked", true);
            update_veteraninformation();
        }
    }

    function load_veteraninformation(e, vet) {
        console.log("load_veteraninformation");
        console.log(vet);
        if (is_vet()) {
            // copy to visitor too
            _(currentFormType.FIELDS.VISITOR_FIRSTNAME).val(vet.firstname);
            //_(form().GetFields().VISITOR_FIRSTNAME).val(vet.firstname);
            _(form().GetFields().VISITOR_MIDDLENAME).val(vet.middlename);
            _(form().GetFields().VISITOR_LASTNAME).val(vet.lastname);
        }
        _(currentFormType.FIELDS.VET_FIRSTNAME).val(vet.firstname);
        //_(form().GetFields().VET_FIRSTNAME).val(vet.firstname);
        _(form().GetFields().VET_MIDDLENAME).val(vet.middlename);
        _(form().GetFields().VET_LASTNAME).val(vet.lastname);
        _(form().GetFields().VET_SSID).val(vet.ssid);
        //_(currentFormType.FIELDS.VET_DOB).val().toString())).toISOString();
        //_(form().GetFields().VET_DOB).val(vet.dob.substring(0, vet.dob.indexOf("T")));

        var formattedDob = new Date(vet.dob).toISOString();
        console.log(formattedDob);
        _(form().GetFields().VET_DOB).val(formattedDob.substring(0, formattedDob.indexOf("T")));

        /* additional data */
        if (form_data()) {
            //Add("sensitivity_level", vet.sensitivity_level);
            //Add("edipi", vet.edipi);
            //Add("address", vet.address);
            //Add("gender", vet.gender);
            //Add("branchofservice", vet.branchofservice);
            form_data().Add("sensitivity_level", vet.sensitivity_level);
            form_data().Add("edipi", vet.edipi);
            form_data().Add("address", vet.address);
            form_data().Add("gender", vet.gender);
            form_data().Add("branchofservice", vet.branchofservice);
        }

        if (vet.idproof && is_walkin_form()) {
            form_data().Add("idproof", vet.idproof);
            set_idproof_title_label(vet.idproof);
        }

    }

    //---------------------------------------------------------------------------------------------------------------
    // idProof Label
    //---------------------------------------------------------------------------------------------------------------
    function set_idproof_title_label(idproof) {
        var label = document.getElementById(currentFormType.TITLE.LABEL);
        if (label !== null) $(label).text("");

        if (idproof === null) return;

        for (var style in CONSTANTS.IDPROOF.styles) {
            if ($(label).hasClass("label-" + CONSTANTS.IDPROOF.styles[style]))
                $(label).removeClass("label-" + CONSTANTS.IDPROOF.styles[style]);
        }

        style = "label-" + CONSTANTS.IDPROOF.styles[idproof];
        $(label).addClass(style);

        var text = CONSTANTS.IDPROOF[idproof];
        $(label).text(text);
    }

    
    function validate_form(button) {
        var errors = [];

        var RequiredFields = ["visitor_firstname", "visitor_lastname"];

        if (is_correspondence_form()) {
            RequiredFields = ["corr_requestor_firstname", "corr_requestor_lastname"];
            if (is_vet()) {
                RequiredFields.push("corr_veteran_ssid");
                RequiredFields.push("corr_veteran_dob");
            } else {
                if ($("#corr_veteran_firstname").val() !== "" || $("#corr_veteran_lastname").val() !== "" || $("#corr_veteran_ssid").val() !== "" || $("#corr_veteran_dob").val() !== "") {
                    RequiredFields.push("corr_veteran_firstname");
                    RequiredFields.push("corr_veteran_lastname");
                    RequiredFields.push("corr_veteran_ssid");
                    RequiredFields.push("corr_veteran_dob");
                }
            }
        }

        function val_req_field(id, name) {
            var el = _(id);
            var alwaysRequired = false;
            for (var i = 0; i < RequiredFields.length; i++) {
                if (RequiredFields[i] === id) alwaysRequired = true;
            }

            if ((button === "search") || alwaysRequired) {
                if (el.val().length === 0) {
                    errors.push({ type: "required", id: id, name: name });
                    el.addClass("form-control-error")
                        .attr("aria-invalid", "true");
                    var errorArea = _(id + "_error");
                    errorArea.text("This field is required.");
                } else {
                    el.removeClass("form-control-error");
                    el.attr("aria-invalid", "false");
                }
            } else {
                el.removeClass("form-control-error");
                el.attr("aria-invalid", "false");
            }
        }

        val_req_field(form().GetFields().VISITOR_FIRSTNAME, "Visitor First Name");
        val_req_field(form().GetFields().VISITOR_LASTNAME, "Visitor Last Name");

        if (!is_vet()) {
            val_req_field(form().GetFields().VISITOR_RELATION, "Visitor Relationship");
            val_req_field(form().GetFields().VET_FIRSTNAME, "Veteran First Name");
            val_req_field(form().GetFields().VET_LASTNAME, "Veteran Last Name");
        }
        val_req_field(form().GetFields().VET_SSID, "Veteran SSN");
        val_req_field(form().GetFields().VET_DOB, "Veteran DOB");

        if (errors.length === 0 && button === "search") {
            if (_(form().GetFields().VET_SSID).val().length !== 9) {
                errors.push({
                    type: "length",
                    id: form().GetFields().VET_SSID,
                    name: "Veteran SSN",
                    message: 'Invalid Social Security Number entered.'
                });
                _(form().GetFields().VET_SSID_ERROR).text("This field requires a 9 digit number with no dashes or spaces.");
                _(form().GetFields().VET_SSID).attr("aria-invalid", "true");
            }

            // define date string to test
            var txtDate = _(form().GetFields().VET_DOB).val();
            var dateToValidate = Tools.FormatDate(txtDate);
            // check date and print message            
            if (!Tools.IsDate(dateToValidate) && button === "search") {
                errors.push({
                    type: "length",
                    id: form().GetFields().VET_DOB,
                    name: "Veteran DOB",
                    message: 'Invalid Date of Birth entered.'
                });
            }
        }

        var requiredFields = [], i, error;
        for (i in errors) {
            error = errors[i];
            if (error.type === "required") {
                requiredFields.push(error.name);
            }
        }

        var messages = [], message;
        if (requiredFields.length > 0) {
            messages.push("The following are required fields: " + requiredFields.join(", "));
        } else {  // Only process other errors if all required fields are there.
            for (i in errors) {
                error = errors[i];
                messages.push(error.message);
            }
        }

        _(currentFormType.ERROR_CONTAINER).html("");
        if (errors.length > 0) {
            var alert = "";

            for (i in messages) {
                message = messages[i];
                var err = $("<div id='" + currentFormType.Name + "_form_error_" + i + "' class='alert alert-validationerror' role='alert'/>");
                err.text(message);
                alert += message + " ";

                _(currentFormType.ERROR_CONTAINER).append(err);
            }
            _(errors[0].id).focus();


            $("#scralerts").text(alert);
        }

        return (errors.length > 0);
    }

    //---------------------------------------------------------------------------------------------------------------
    // Routing Buttons
    //---------------------------------------------------------------------------------------------------------------
    function retrieve_button_routes() {
        var fetch = '<fetch><entity name="udo_sitequeuesetting">' +
            '<attribute name="udo_queueid"/>' +
            '<attribute name="udo_name"/>' +
            '<attribute name="udo_siteid"/>' +
            '<attribute name="udo_queuetype"/>' +
            '<link-entity name="site" from="siteid" to="udo_siteid">' +
            '<link-entity name="systemuser" from="siteid" to="siteid">' +
            '<filter><condition attribute="systemuserid" operator="eq-userid" /></filter>' +
            '</link-entity></link-entity>' +
            '<order attribute="udo_order"/>' +
            '</entity></fetch>';
        var endcodedFetchXml = encodeURI(fetch);
        return webApi.RetrieveMultiple("udo_sitequeuesettings?fetchXml=" + endcodedFetchXml).then(function success(data) {
            return data;
        });
    }

    function post_retrieve_button_routes(ec) {
        currentFormType = FORMS.CORRESPONDENCE;
        setup_routing_buttons(ec);
        setup_actions();
        currentFormType = FORMS.WALKIN;
        setup_routing_buttons(ec);
        setup_actions();
    }

    function setup_routing_buttons(ec) {
        function create_routing_buttons(data, id, queueid) {
            var cfg = form().GetConfig();
            try {
                form().AddButton(cfg.AREAS.ACTION_AREA, data.udo_name, id,
                    function (e) {
                        var frm = form();
                        if (frm) {
                            _(cfg.ERROR).text("");
                            if (!validate_form("route")) {
                                switch (frm.GetFormType()) {
                                    case "new":
                                        if (is_walkin_form()) {
                                            return create_interaction_and_queue(queueid)
                                                .then(form().Close)
                                                .then(QueueManager.Refresh)
                                                .done(function () { })
                                                .fail(function () {
                                                    var msg = "Error: Unable to route interaction.";
                                                    if (e.message !== undefined) msg += "\r\n\r\n" + e.message;
                                                    error(msg);
                                                });
                                        } else if (is_correspondence_form()) {
                                            return create_or_update_correspondence().then(function () {
                                                return create_interaction_and_queue(queueid).then(form().Close).then(QueueManager.Refresh)
                                                    .done(function () { })
                                                    .fail(function () {
                                                        var msg = "Error: Unable to route correspondence interaction.";
                                                        if (e.message !== undefined) msg += "\r\n\r\n" + e.message;
                                                        error(msg);
                                                    });
                                            });
                                        }
                                        break;
                                    case "existing":
                                        //existing item being edited
                                        return action_update().then(function (data) {
                                            return QueueManager.Route(data.queueitemid, queueid);
                                        });
                                    default:
                                        break;
                                }
                            }
                            e.preventDefault();
                            return false;
                        }
                    }, true, true
                );
            } catch (ex) { console.log(ex.message);}
            
        }

        var siteid = null;
        ec.forEach(function (e) {
            var data = e;
            var id = ("action_route_" + data.udo_name).replace(/[^a-zA-Z0-9]+/gi, '').toLowerCase();
            var queueid = data["_udo_queueid_value"];
            if (data["udo_siteid"]) siteid = data["_udo_siteid_value"];

            if (data["udo_queuetype"]) {
                var qt = data["udo_queuetype"].value;
                if ((is_walkin_form() && qt === 752280000)
                    || (is_correspondence_form() && qt === 752280001)) {
                    create_routing_buttons(data, id, queueid);
                }
            } else if (is_walkin_form()) {
                create_routing_buttons(data, id, queueid);
            }
        });
        form_data().Add("siteid", siteid);
    }

    //---------------------------------------------------------------------------------------------------------------
    // ACTIONS - INIT & SETUP
    //---------------------------------------------------------------------------------------------------------------
    // inits are run only once
    function init_actions() {
        var bind_action = function (id, clickHandler) {
            var jqEl = _(id);
            if (jqEl.length > 0) {
                jqEl.click(clickHandler);
                Tools.ClickOnSpace(jqEl);
            }
            return jqEl;
        };

        // Toolbar actions only
        bind_action("action_newwalkin", action_new_walkin);
        bind_action("action_newcorrespondence", action_new_correspondence);

        $(document).ready(function () {
            var li = $("<li/>");
            var btn = $("<a href='#'/>")
                .attr("id", "action_showspecial")
                .attr("role", "button")
                .text("Show All Special Situations")
                .addClass("navbtn-toggle-off");

            btn.click(function () {
                var b = $(this);
                var enabled = b.hasClass("navbtn-toggle-on");

                if (enabled) {
                    // disable
                    b.removeClass("navbtn-toggle-on");
                    b.addClass("navbtn-toggle-off");
                    b.text("Show All Special Situations");
                    enabled = false;
                } else {
                    // enable
                    b.addClass("navbtn-toggle-on");
                    b.removeClass("navbtn-toggle-off");
                    b.text("Hide All Special Situations");
                    enabled = true;
                }
                QueueManager.SetSpecialSituationVisibility(enabled);
            });
            li.append(btn);
            $("#nav_main_toolbar").append(li);
        });

        // Queue Manager Action Buttons - Queue Manager must be loaded first.
        bind_action("action_pick", action_pick);
        bind_action("action_release", action_release);

        // Close button
        create_resolution_dropdown("action_complete_items", "toolbar");
        //bind_action("action_complete",action_complete);

        // Walkout
        bind_action("action_walkout", action_walkout);

        // Open button
        bind_action("action_open", action_open_queueitem);

        bind_action("img_lookup_corr", action_lookup_corr);

        bind_action("img_lookup_letter", action_lookup_letter);
    }

    // Setups are ran multiple times for each form opened...
    function setup_actions() {
        var cfg = form().GetConfig();
        form().AddButton(cfg.AREAS.FIND_VETERAN_CONTROL, "Find Veteran", cfg.ACTIONS.SEARCH_FOR_VET, function (e) {
            _(currentFormType.ERROR).text("");
            if (!validate_form("search")) {
                loading_on();
                var vetSearch = _(cfg.AREAS.SEARCH_RESULTS);
                var vetData = vetSearch.VeteranSearch('GetVetFromForm');
                console.log(vetData);
                vetSearch.VeteranSearch('SearchByName', vetData);
            }

            e.preventDefault();
            return false;
        }, true, true);

        create_resolution_dropdown(cfg.AREAS.ACTION_AREA, "button");

        form().AddButton(cfg.AREAS.ACTION_AREA, "Update", cfg.ACTIONS.UPDATE, function (e) {
            action_update();
            e.preventDefault();
            return false;
        }, false, true);
    }

    //---------------------------------------------------------------------------------------------------------------
    // ACTIONS - ACTION METHODS
    //---------------------------------------------------------------------------------------------------------------
    function action_complete(e) {
        change_status(CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.COMPLETE).then(form().Close).then(QueueManager.Refresh)
        .done(function () {})
        .fail(function () {
            var msg = "Error: Unable to complete interaction.";
            if (e.message !== undefined) msg += "\r\n\r\n" + e.message;
            error(msg);
        });
        e.preventDefault();
        return false;
    }

    function action_lookup_corr() {
        return open_lookup_dialog(correspondenceObjectTypeCode, action_lookup_corr_callback);
    }

    function action_lookup_corr_callback(event) {
        $('#correspondence_lookup_name').val(event.items[0].name);
        form_data().Add("correspondenceid", event.items[0].id);
        retrieve_correspondence(event.items[0].id);
    }

    function action_lookup_letter() {
        return open_lookup_dialog(vbmsdoctypeObjectTypeCode, action_lookup_letter_callback);
    }

    function action_lookup_letter_callback(event) {
        $('#corr_letter_lookup_name').val(event.items[0].name);

        form_data().Add("letterid", event.items[0].id);
    }

    function action_new(formType) {
        //if other form exists, close it
        form().Close();
        currentFormType = formType;
        reset_form().then(open_new_form);

        _(form().GetFields().IS_VET_YES).prop('checked', true);
        _(form().GetFields().SPECIAL_SITUATIONS).magicSuggest({}).setSelection([]);
        update_veteraninformation(currentFormType);
    }

    function action_new_correspondence() {
        action_new(FORMS.CORRESPONDENCE);
        // custom newCorrespondence actions go here

        //if other form exists, close it
        return false;
    }

    function action_new_walkin() {
        action_new(FORMS.WALKIN);

        // custom newWalkin actions go here
        return false;
    }

    function action_open_queueitem() {
        var objects = QueueManager.GetSelectedQueueItemObjects();
        if (objects.length < 1) return;
        var objectid = objects[0].queueitemid;
        form_data().Add("id", objects[0].objectid.id);
        form_data().Add("interactionid", objects[0].objectid.id);
        /* EntityReference */
        var queueitemid = objects[0].queueitemid;
        /* Guid */

        if (objectid === null) return;

        if (currentFormType !== null) {
            form().Close();
        }

        var ms = _(form().GetFields().SPECIAL_SITUATIONS).magicSuggest({});
        ms.setSelection([]);
        _(currentFormType.ACTIONS.UPDATE).show();
        reset_form(objects[0])
            .then(retrieve_interaction)
            .then(post_retrieve_interaction)
            .then(open_edit_form)
            .then(function () {
                form_data().Add("queueitemid", queueitemid);
                var idproof = form_data().Get("idproof");
                if (idproof > 0) {
                    set_idproof_title_label(idproof);
                }
            })
            .fail(error);
    }

    function action_update() {
        var dfd = new $.Deferred();
        if (validate_form("update")) {
            dfd.reject("Validation Failed");
            return dfd.promise();
        }
        
        create_or_update_correspondence().then(update_interaction).then(dfd.resolve)
            .then(form().Close).then(QueueManager.Refresh)
            .done(function () {})
            .fail(function () {
                var msg = "Error: Unable to complete update.";
                if (e.message !== undefined) msg += "\r\n\r\n" + e.message;
                error(msg);
            });

        return dfd.promise();
    }

    function action_walkout(e) {
        var objects = QueueManager.GetSelectedQueueItemObjects();
        change_status(CONSTANTS.STATECODE.INACTIVE, CONSTANTS.STATUSCODE.WALKEDOUT, objects[0].objectid).then(form().Close).then(QueueManager.Refresh)
            .done(function () {})  
            .fail(function () {
                var msg = "Error: Unable to complete walkout.";
                if (e.message !== undefined) msg += "\r\n\r\n" + e.message;
                error(msg);
            });
        e.preventDefault();
        return false;
    }

    function action_pick() {
        return QueueManager.Pick();
    }

    function action_release() {
        return QueueManager.Release();
    }

    return {
        Initialize: init_form_manager,
        NewWalkin: action_new_walkin,
        NewCorrespondence: action_new_correspondence,
        RequestTypeId: "",
        RequestSubTypeId: "",
        Form: {}
    }
}();

$(document).ready(function () {
    FormManager.Initialize();
});
