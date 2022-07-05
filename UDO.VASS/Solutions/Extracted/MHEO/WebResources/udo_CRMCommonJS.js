!function n(t, e, r) { function o(u, f) { if (!e[u]) { if (!t[u]) { var c = "function" == typeof require && require; if (!f && c) return c(u, !0); if (i) return i(u, !0); var s = new Error("Cannot find module '" + u + "'"); throw s.code = "MODULE_NOT_FOUND", s } var l = e[u] = { exports: {} }; t[u][0].call(l.exports, function (n) { var e = t[u][1][n]; return o(e ? e : n) }, l, l.exports, n, t, e, r) } return e[u].exports } for (var i = "function" == typeof require && require, u = 0; u < r.length; u++) o(r[u]); return o }({ 1: [function (n, t, e) { "use strict"; function r() { } function o(n) { try { return n.then } catch (t) { return d = t, w } } function i(n, t) { try { return n(t) } catch (e) { return d = e, w } } function u(n, t, e) { try { n(t, e) } catch (r) { return d = r, w } } function f(n) { if ("object" != typeof this) throw new TypeError("Promises must be constructed via new"); if ("function" != typeof n) throw new TypeError("not a function"); this._37 = 0, this._12 = null, this._59 = [], n !== r && v(n, this) } function c(n, t, e) { return new n.constructor(function (o, i) { var u = new f(r); u.then(o, i), s(n, new p(t, e, u)) }) } function s(n, t) { for (; 3 === n._37;) n = n._12; return 0 === n._37 ? void n._59.push(t) : void y(function () { var e = 1 === n._37 ? t.onFulfilled : t.onRejected; if (null === e) return void (1 === n._37 ? l(t.promise, n._12) : a(t.promise, n._12)); var r = i(e, n._12); r === w ? a(t.promise, d) : l(t.promise, r) }) } function l(n, t) { if (t === n) return a(n, new TypeError("A promise cannot be resolved with itself.")); if (t && ("object" == typeof t || "function" == typeof t)) { var e = o(t); if (e === w) return a(n, d); if (e === n.then && t instanceof f) return n._37 = 3, n._12 = t, void h(n); if ("function" == typeof e) return void v(e.bind(t), n) } n._37 = 1, n._12 = t, h(n) } function a(n, t) { n._37 = 2, n._12 = t, h(n) } function h(n) { for (var t = 0; t < n._59.length; t++) s(n, n._59[t]); n._59 = null } function p(n, t, e) { this.onFulfilled = "function" == typeof n ? n : null, this.onRejected = "function" == typeof t ? t : null, this.promise = e } function v(n, t) { var e = !1, r = u(n, function (n) { e || (e = !0, l(t, n)) }, function (n) { e || (e = !0, a(t, n)) }); e || r !== w || (e = !0, a(t, d)) } var y = n("asap/raw"), d = null, w = {}; t.exports = f, f._99 = r, f.prototype.then = function (n, t) { if (this.constructor !== f) return c(this, n, t); var e = new f(r); return s(this, new p(n, t, e)), e } }, { "asap/raw": 4 }], 2: [function (n, t, e) { "use strict"; function r(n) { var t = new o(o._99); return t._37 = 1, t._12 = n, t } var o = n("./core.js"); t.exports = o; var i = r(!0), u = r(!1), f = r(null), c = r(void 0), s = r(0), l = r(""); o.resolve = function (n) { if (n instanceof o) return n; if (null === n) return f; if (void 0 === n) return c; if (n === !0) return i; if (n === !1) return u; if (0 === n) return s; if ("" === n) return l; if ("object" == typeof n || "function" == typeof n) try { var t = n.then; if ("function" == typeof t) return new o(t.bind(n)) } catch (e) { return new o(function (n, t) { t(e) }) } return r(n) }, o.all = function (n) { var t = Array.prototype.slice.call(n); return new o(function (n, e) { function r(u, f) { if (f && ("object" == typeof f || "function" == typeof f)) { if (f instanceof o && f.then === o.prototype.then) { for (; 3 === f._37;) f = f._12; return 1 === f._37 ? r(u, f._12) : (2 === f._37 && e(f._12), void f.then(function (n) { r(u, n) }, e)) } var c = f.then; if ("function" == typeof c) { var s = new o(c.bind(f)); return void s.then(function (n) { r(u, n) }, e) } } t[u] = f, 0 === --i && n(t) } if (0 === t.length) return n([]); for (var i = t.length, u = 0; u < t.length; u++) r(u, t[u]) }) }, o.reject = function (n) { return new o(function (t, e) { e(n) }) }, o.race = function (n) { return new o(function (t, e) { n.forEach(function (n) { o.resolve(n).then(t, e) }) }) }, o.prototype["catch"] = function (n) { return this.then(null, n) } }, { "./core.js": 1 }], 3: [function (n, t, e) { "use strict"; function r() { if (c.length) throw c.shift() } function o(n) { var t; t = f.length ? f.pop() : new i, t.task = n, u(t) } function i() { this.task = null } var u = n("./raw"), f = [], c = [], s = u.makeRequestCallFromTimer(r); t.exports = o, i.prototype.call = function () { try { this.task.call() } catch (n) { o.onerror ? o.onerror(n) : (c.push(n), s()) } finally { this.task = null, f[f.length] = this } } }, { "./raw": 4 }], 4: [function (n, t, e) { (function (n) { "use strict"; function e(n) { f.length || (u(), c = !0), f[f.length] = n } function r() { for (; s < f.length;) { var n = s; if (s += 1, f[n].call(), s > l) { for (var t = 0, e = f.length - s; e > t; t++) f[t] = f[t + s]; f.length -= s, s = 0 } } f.length = 0, s = 0, c = !1 } function o(n) { var t = 1, e = new a(n), r = document.createTextNode(""); return e.observe(r, { characterData: !0 }), function () { t = -t, r.data = t } } function i(n) { return function () { function t() { clearTimeout(e), clearInterval(r), n() } var e = setTimeout(t, 0), r = setInterval(t, 50) } } t.exports = e; var u, f = [], c = !1, s = 0, l = 1024, a = n.MutationObserver || n.WebKitMutationObserver; u = "function" == typeof a ? o(r) : i(r), e.requestFlush = u, e.makeRequestCallFromTimer = i }).call(this, "undefined" != typeof global ? global : "undefined" != typeof self ? self : "undefined" != typeof window ? window : {}) }, {}], 5: [function (n, t, e) { "function" != typeof Promise.prototype.done && (Promise.prototype.done = function (n, t) { var e = arguments.length ? this.then.apply(this, arguments) : this; e.then(null, function (n) { setTimeout(function () { throw n }, 0) }) }) }, {}], 6: [function (n, t, e) { n("asap"); "undefined" == typeof Promise && (Promise = n("./lib/core.js"), n("./lib/es6-extensions.js")), n("./polyfill-done.js") }, { "./lib/core.js": 1, "./lib/es6-extensions.js": 2, "./polyfill-done.js": 5, asap: 3 }] }, {}, [6]);
//# sourceMappingURL=/polyfills/promise-7.0.4.min.js.map
; var CrmCommonJS;

(function (CrmCommonJS) {
    var CrmCommon = (function (crmVersion, exCon) {
        function CrmCommon(crmVersion, exCon) {
            this.crmVersion = crmVersion;
            this.exCon = exCon;
            if (typeof Xrm === 'undefined')
                Xrm = window.parent["Xrm"];
            var factory = new CrmCommonJS.ApiFactory(crmVersion, exCon);
            this.WebApi = factory.GetWebApi();
            this.Security = factory.GetSecurityApi(this.WebApi);
            this.Notification = factory.GetNotificationApi();
            this.FormHelper = factory.GetFormApi(exCon);
            this.Util = factory.GetUtilApi();
        }
        return CrmCommon;
    }());
    CrmCommonJS.CrmCommon = CrmCommon;
})(CrmCommonJS || (CrmCommonJS = {}));
//var CrmCommonJS;
(function (CrmCommonJS) {
    var ApiFactory = (function () {

        function ApiFactory(crmVersion, exCon) {
            this.crmVersion = crmVersion;
            this.exCon = exCon;
        }
        ApiFactory.prototype.GetWebApi = function () {
            //if (this.crmVersion < 2016) {
            //    return new CrmCommonJS.ODataApi(this.crmVersion);
            //}
            //else {
            //    return new CrmCommonJS.WebApi(this.crmVersion);
            //}
            return new CrmCommonJS.WebApi(this.crmVersion);
        };
        ApiFactory.prototype.GetSecurityApi = function (webApi) {
            return new CrmCommonJS.Security(webApi);
        };
        ApiFactory.prototype.GetFormApi = function () {
            return new CrmCommonJS.FormHelper(this.exCon);
        };
        ApiFactory.prototype.GetNotificationApi = function () {
            return new CrmCommonJS.Notification;
        };
        ApiFactory.prototype.GetUtilApi = function () {
            return new CrmCommonJS.Util
        };
        return ApiFactory;
    }());
    CrmCommonJS.ApiFactory = ApiFactory;
})(CrmCommonJS || (CrmCommonJS = {}));

//var CrmCommonJS;
(function (CrmCommonJS) {
    var FormHelper = (function () {
        function FormHelper(exCon) {
            FormHelper.exCon = exCon;
            FormHelper.formContext = FormHelper.prototype.GetFormContextOrDefault(exCon);
        }
        FormHelper.prototype.GetFormContextOrDefault = function (exCon) {
            console.log(!!exCon);
            console.log(exCon);

            if (!!exCon) {
                try {
                    console.log("attempt to return from exCon");
                    var executionFormContext = exCon.getFormContext();
                    return executionFormContext;
                } catch (e) {
                    console.log("reached error");
                    return Xrm.Page;
                }
            } else if (parent.Xrm) {
                return parent.Xrm.Page;
            } else {
                return Xrm.Page;
            }
        };

        FormHelper.prototype.setFieldVisible = function (fieldName, visibility) {
            if (visibility === void 0) { visibility = true }
            var control = typeof fieldName === "string" ? FormHelper.prototype.getControl(fieldName) : fieldName;
            if (!control || !control.setVisible) {
                return;
            }
            control.setVisible(visibility);
        }

        FormHelper.prototype.close = function () {
            return FormHelper.formContext.ui.close();
        };

        FormHelper.prototype.getFormType = function () {
            return FormHelper.formContext.ui.getFormType();
        }

        FormHelper.prototype.getCurrentRecordIdFormatted = function () {
            var id = FormHelper.formContext.data.entity.getId();
            return id.replace(/{/gi, "").replace(/}/, "");
        };

        FormHelper.prototype.getCurrentRecordId = function () {
            return FormHelper.formContext.data.entity.getId();
        };

        FormHelper.prototype.getSelectedLookupIdFormatted = function (fieldName) {
            var id = FormHelper.prototype.getSelectedLookupId(fieldName);
            return id === null ? id : id.replace(/{/gi, "").replace(/}/, "");
        };

        FormHelper.prototype.getSelectedLookupId = function (fieldName) {
            var lookup = FormHelper.prototype.getSelectedLookup(fieldName);
            return lookup ? lookup.id : null;
        };

        FormHelper.prototype.getSelectedLookup = function (fieldName) {
            var value = FormHelper.prototype.getValue(fieldName);
            return value ? value[0] : null;
        };

        FormHelper.prototype.setRequiredLevel = function (fieldName, level) {
            var att = FormHelper.prototype.getAttribute(fieldName);
            if (att) {
                if (typeof level === "string") {
                    level = level.toLowerCase();
                }
                switch (level) {
                    case true:
                    case "required":
                        level = "required";
                        break;
                    case "recommended":
                        level = "recommended";
                        break;
                    case false:
                    case "none":
                    case null:
                    case undefined:
                        level = "none";
                        break;
                    default:
                        alert(level + " is an invalid submit mode. Pass 'requried', 'recommended', 'none' or a boolean to this method");
                        return;

                }
                att.setRequiredLevel(level);
            }
        };

        FormHelper.prototype.setSubmitMode = function (fieldName, submitMode) {
            var att = FormHelper.prototype.getAttribute(fieldName);
            if (!att) { return; }
            if (typeof submitMode === "string") {
                submitMode = submitMode.toLowerCase();
            }
            switch (submitMode) {
                case true:
                case "always":
                    submitMode = "always";
                    break;
                case "dirty":
                    submitMode = "dirty";
                    break;
                case "never":
                case false:
                    submitMode = "never";
                    break;
                default:
                    alert(submitMode + " is an invalid submit mode. Pass 'always', 'dirty', 'never' or a boolean to this method");
                    return;
            }
            att.setSubmitMode(submitMode);
        };

        FormHelper.prototype.setFormNotification = function (message, level, messageName) {
            level = level.toUpperCase();
            switch (level) {
                case "ERROR":
                    level = "ERROR";
                    break;
                case "WARNING":
                    level = "WARNING";
                    break;
                case "INFO":
                    level = "INFO";
                    break;
                default:
                    alert("Invald notification level. Select 'ERROR', 'WARNING' or 'INFO'");
                    return;
            }
            FormHelper.formContext.ui.setFormNotification(message, level, messageName);
        };

        FormHelper.prototype.getTab = function (tabName) {
            if (!tabName) return null;
            return FormHelper.formContext.ui.tabs.get(tabName);
        };

        FormHelper.prototype.getControl = function (controlName) {
            if (!controlName) return null;
            var ctrl = FormHelper.formContext.getControl(controlName);
            ctrl = ctrl ? ctrl : null;
            if (!ctrl) console.log("No control found for " + controlName);
            return ctrl;
        };

        FormHelper.prototype.clearFormNotification = function (messageName) {
            if (messageName) {
                FormHelper.formContext.ui.clearFormNotification(messageName);
            }
        };

        FormHelper.prototype.setValue = function (fieldName, value, displayName, logicalName, fireOnChange) {
            if (fireOnChange === void 0) { fireOnChange = false }
            if (displayName !== null && typeof displayName === "boolean") {
                fireOnChange = displayName;
                displayName = null;
            }
            if (displayName !== null && logicalName && value && typeof value === "string") {
                value = [{ id: value, name: displayName, entityType: logicalName }];
            }
            var att = FormHelper.prototype.getAttribute(fieldName);
            if (att) {
                if (att.getAttributeType() === "lookup") {
                    value = [{ id: value.Id, name: value.Name, entityType: value.LogicalName }];
                }
                att.setValue(value);
                if (fireOnChange) { att.fireOnChange(); }
            }
        };

        FormHelper.prototype.setDisabled = function (controlName, disabled) {
            if (disabled === void 0) { disabled = true; }
            var control = FormHelper.prototype.getControl(controlName);
            if (control && control.setDisabled) {
                control.setDisabled(disabled);
            }
        }

        FormHelper.prototype.saveRecord = function (refresh) {
            if (refresh !== true) { refresh = false; }
            FormHelper.formContext.data.entity.save(refresh);
        };

        FormHelper.prototype.refreshRecord = function () {
            FormHelper.formContext.data.refresh();
        };

        FormHelper.prototype.getEntityLogicalName = function () {
            return FormHelper.formContext.data.entity.getEntityName();
        };

        FormHelper.prototype.getAttribute = function (fieldName) {
            if (!fieldName) return null;
            var att = FormHelper.formContext.getAttribute(fieldName);
            att = att ? att : null;
            return att;
        };

        FormHelper.prototype.getValue = function (fieldName) {
            var att = FormHelper.prototype.getAttribute(fieldName);
            return att ? att.getValue() : null;
        };

        FormHelper.prototype.SetLookupValue = function (fieldName, id, name, entityType) {
            if (fieldName !== null) {
                var lookupValue = new Array;
                lookupValue[0] = new Object;
                lookupValue[0].id = id;
                lookupValue[0].name = name;
                lookupValue[0].entityType = entityType;
                formContext.getAttribute(fieldName).setValue(lookupValue);
                formContext.getAttribute(fieldName).setSubmitMode("always");
            }
        };

        FormHelper.prototype.setTabVisible = function (tabName, visibility) {
            if (visibility === void 0) { visibility = true; }
            var tab = FormHelper.formContext.ui.tabs.get(tabName);
            if (!tab) { return; }
            tab.setVisible(visibility);
        };

        FormHelper.prototype.setSectionVisible = function (sectionName, tabName, visibility) {
            if (visibility === void 0) { visibility = true; }
            var tab = FormHelper.formContext.ui.tabs.get(tabName);
            if (!tab) { return; }
            var section = tab.sections.get(sectionName);
            if (!section) { return; }
            section.setVisible(visibility);
        }

        FormHelper.prototype.SetFieldVisibility = function (fieldName, value) {
            var ctrl = formContext.get(fieldName);
            if (ctrl === null)
                return;
            ctrl.setVisible(value);
        };
        FormHelper.prototype.SetFieldReadOnly = function (fieldName, value) {
            var ctrl = formContext.get(fieldName);
            if (ctrl === null)
                return;
            ctrl.setDisabled(value);
        };
        FormHelper.prototype.SetTabVisibility = function (tabName, value) {
            var tab = formContext.ui.tabs.get(tabName);
            if (tab === null) {
                return;
            }
            tab.setVisible(value);
        };
        FormHelper.prototype.GetContext = function () {
            this.formContext = formContext;
            this.executionContext = executionContext;
        };
        FormHelper.prototype.ui = function () {
            return FormHelper.formContext.ui;
        };
        FormHelper.prototype.DisplayDialog = function (dialogObj) {
            var method;
            var strings;
            var options = { height: dialogObj.height, width: dialogObj.width };
            //switch (dialogObj.type.toLowerCase()) {
            //    case "alert":
            //        method = openAlertDialog;
            //        strings = { confirmButtonLabel: dialogObj.btnLbl[0] != null ? dialogObj.btnLbl[0] : "Ok", text: dialogObj.message };
            //        break;
            //    case "confirm":
            //        method = openConfirmDialog;
            //        strings = {
            //            cancelButtonLabel: dialogObj.btnLbl[1] != null ? dialogObj.btnLbl[1] : "Cancel",
            //            confirmButtonLabel: dialogObj.btnLbl[0] != null ? dialogObj.btnLabel[0] : "Ok", text: dialogObj.message, title: dialogObj.title
            //        }
            //        break;
            //    case "error":
            //        strings = { message: dialogObj.message };
            //        Xrm.Navigation.openErrorDialog(strings)
            //            .then((data) => { dialogObj.success(data) })
            //            .catch((error) => { dialogObj.error(error) });

            //}
            //if (dialogObj.type == "Alert" || dialogObj.type == "Comfirm") {
            //    Xrm.Navigation.method(strings, options)
            //        .then((data) => { dialogObj.success(data) })
            //        .catch((error) => { dialogObj.error(error) });
            //}
        }
        return FormHelper;
    }());
    CrmCommonJS.FormHelper = FormHelper;
})(CrmCommonJS || (CrmCommonJS = {}));


//var CrmCommonJS;
(function (CrmCommonJS) {
    var Notification = (function () {
        function Notification() {
            this._ids = [];
            this._idc = 1;
        }
        Notification.prototype.SetInfo = function (msg, id) {
            if (id === void 0) {
                id = null;
            }
            this._AddNotification(msg, 'INFO', id);
        };
        Notification.prototype.SetWarning = function (msg, id) {
            if (id === void 0) {
                id = null;
            }
            this._AddNotification(msg, 'WARNING', id);
        };
        Notification.prototype.SetError = function (msg, id) {
            if (id === void 0) {
                id = null;
            }
            this._AddNotification(msg, 'ERROR', id);
        };
        Notification.prototype.ClearNotification = function (id) {
            if (id === void 0) {
                id = null;
            }
            if (id === null)
                this._ClearAllNotifications();
            else {
                Xrm.Page.ui.clearFormNotification(id);
                var i = this._ids.indexOf(id);
                if (i > -1)
                    this._ids.splice(i, 1);
            }
        };
        Notification.prototype._AddNotification = function (msg, lvl, id) {
            if (id === null) {
                id = this._idc.toString();
                ++this._idc;
            }
            Xrm.Page.ui.setFormNotification(msg, lvl, id);
            this._ids.push(id);
        };
        Notification.prototype._ClearAllNotifications = function () {
            this._ids.forEach(function (id) {
                Xrm.Page.ui.clearFormNotification(id);
            });
            this._ids = [];
        };
        return Notification;
    }());
    CrmCommonJS.Notification = Notification;
})(CrmCommonJS || (CrmCommonJS = {}));

//var CrmCommonJS;
(function (CrmCommonJS) {
    var ODataApi = (function () {
        function ODataApi(CrmVersion) {
            this.CrmVersion = CrmVersion;
            this._util = new CrmCommonJS.Util;
            this._clientUrl = Xrm.Page.context.getClientUrl();
            this.ODataEntityName = {
                Role: "RoleSet", Team: "TeamSet", TeamMembership: "TeamMembershipSet"
            };
        }
        ODataApi.prototype._MakeRequest = function (method, url, entity) {
            if (entity === void 0) {
                entity = null;
            }
            return new Promise(function (resolve, reject) {
                var req = new XMLHttpRequest;
                req.open(method, encodeURI(url), true);
                req.setRequestHeader("Accept", "application/json");
                req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
                req.onreadystatechange = function () {
                    if (req.readyState === 4) {
                        if (req.status >= 200 && req.status < 300) {
                            req.onreadystatechange = null;
                            resolve(req);
                        }
                        else {
                            reject(new Error(req.status + " - " + req.statusText));
                        }
                    }
                };
                req.onerror = function () {
                    reject(new Error(req.status + " - " + req.statusText));
                };
                if (typeof entity !== 'undefined')
                    req.send(JSON.stringify(entity));
                else
                    req.send(null);
            });
        };
        ODataApi.prototype._GetUrl = function (entityType, guid, filter) {
            if (guid === void 0) {
                guid = null;
            }
            var url = this._clientUrl + "/XRMServices/2011/OrganizationData.svc/" + entityType;
            if (guid !== null && typeof guid !== 'undefined') {
                guid = guid.toString().replace("{", "").replace("}", "");
                url = url + "(guid'" + guid + "')";
            }
            if (filter !== null && typeof filter !== 'undefined')
                url = url + filter;
            return url;
        };
        ODataApi.prototype.GetClientUrl = function () {
            return this._clientUrl;
        };
        ODataApi.prototype.RetrieveRecord = function (guid, entityType, cols, filter) {
            var _this = this;
            if (filter === void 0) {
                filter = null;
            }
            return new Promise(function (resolve, reject) {
                if (guid === null || typeof guid === 'undefined')
                    return reject(new Error("RetrieveRecord error: guid is null"));
                if (entityType === null || typeof entityType === 'undefined')
                    return reject(new Error("RetrieveRecord error: entityType is null"));
                var query = null;
                if (cols !== null && typeof cols !== 'undefined') {
                    query = "?$select=" + cols;
                }
                if (filter !== null && typeof filter !== 'undefined') {
                    query === null ? query = "?" + filter : query += "&" + filter;
                }
                var url = _this._GetUrl(entityType, guid, query);
                console.log(url);
                _this._MakeRequest("GET", url).then(function (resp) {
                    var entity = JSON.parse(resp.response);
                    resolve(entity.d);
                }).catch(function (error) {
                    console.log("Error during RetrieveRecord MakeRequest (Legacy): " + error);
                    reject(new Error(error));
                });
            });
        };
        ODataApi.prototype.RetrieveMultiple = function (entityType, cols, filter) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                if (entityType === null || typeof entityType === 'undefined')
                    return reject(new Error("RetrieveMultiple error: entityType is null"));
                var query = null;
                if (cols !== null && typeof cols !== 'undefined') {
                    query = "?$select=" + cols;
                }
                if (filter !== null && typeof filter !== 'undefined') {
                    query === null ? query = "?" + filter : query += "&" + filter;
                }
                var url = _this._GetUrl(entityType, null, query);
                console.log(url);
                _this._MakeRequest("GET", url).then(function (response) {
                    var data = JSON.parse(response.response);
                    resolve(data);
                }).catch(function (error) {
                    console.log("Error during RetrieveMultiple MakeRequest: " + error);
                    reject(new Error(error));
                });
            });
        };
        ODataApi.prototype.CreateRecord = function (entity, entityType) {
            return new Promise(function (resolve, reject) { });
        };
        ODataApi.prototype.DeleteRecord = function (guid, entityType) {
            return new Promise(function (resolve, reject) { });
        };
        ODataApi.prototype.DeleteProperty = function (guid, entityType, propertyName) {
            return new Promise(function (resolve, reject) { });
        };
        ODataApi.prototype.UpdateRecord = function (guid, entityType, entity) {
            return new Promise(function (resolve, reject) { });
        };
        ODataApi.prototype.PutRecord = function (guid, entityType, propertyName, value) {
            return new Promise(function (resolve, reject) { });
        };
        ODataApi.prototype.ExecuteRequest = function (method, msgName, data) {
            return new Promise(function (resolve, reject) { });
        };
        ODataApi.prototype.SetLookupValue = function (guid, entityType) {
            return null;
        };
        return ODataApi;
    }());
    CrmCommonJS.ODataApi = ODataApi;
})(CrmCommonJS || (CrmCommonJS = {}));
//;
//var CrmCommonJS;
(function (CrmCommonJS) {
    var Security = (function () {
        function Security(webApi) {

            this._userTeamNames = [];
            this._webApi = webApi;
            var globalCon = Xrm.Utility.getGlobalContext() !== null ? Xrm.Utility.getGlobalContext() : getGlobalContext();
            this._roleGuids = globalCon.userSettings.securityRoles;
        }
        Security.prototype._GetUsersTeamIds = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                var userId = _globalCon.userSettings.userId.substr(1, 36);
                _this._webApi.RetrieveMultiple(_this._webApi.ODataEntityName.TeamMembership, "teamid", "$filter=systemuserid eq " + userId).then(function (results) {
                    resolve(results);
                }).catch(function (error) {
                    reject(new Error(error));
                });
            });
        };
        Security.prototype._GetUsersTeamNames = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                if (_this._userTeamNames.length > 0)
                    return resolve(_this._userTeamNames);
                _this._GetUsersTeamIds().then(function (teams) {
                    return Promise.all(teams.value.map(function (team) {
                        return _this._webApi.RetrieveRecord(team.teamid, _this._webApi.ODataEntityName.Team, "name");
                    }));
                }).then(function (results) {
                    results.forEach(function (t) {
                        _this._userTeamNames.push(t.name);
                    });
                    resolve(_this._userTeamNames);
                }).catch(function (error) {
                    console.log("Error during GetUsersTeamNames: " + error);
                    reject(new Error(error));
                });
            });
        };
        Security.prototype.GetUsersRoleNames = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                var userRoleNames = [];
                Promise.resolve(_this._roleGuids).then(function (guids) {
                    return Promise.all(guids.map(function (guid) {
                        return _this._webApi.RetrieveRecord(guid, _this._webApi.ODataEntityName.Role, ["name"]);
                    }));
                }).then(function (results) {
                    results.forEach(function (r) {
                        userRoleNames.push(r.name);
                    });
                    resolve(userRoleNames);
                }).catch(function (error) {
                    console.log("Error during GetUsersRoleNames: " + error);
                    reject(new Error(error));
                });
            });
        };
        Security.prototype.GetUsersTeamNames = function () {
            return this._GetUsersTeamNames();
        };
        Security.prototype.UserHasRole = function (roleNames) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                if (roleNames === null || typeof roleNames === 'undefined')
                    return reject(new Error("Error in UserHasRole: roleNames is null"));
                _this.GetUsersRoleNames().then(function (usersRoles) {
                    var len = roleNames.length;
                    var found = false;
                    for (var i = 0; i < len; ++i) {
                        if (usersRoles.indexOf(roleNames[i]) > -1) {
                            found = true;
                            break;
                        }
                    }
                    resolve(found);
                }).catch(function (error) {
                    console.log("Error in UserHasRole: " + error);
                    reject(new Error(error));
                });
            });
        };
        Security.prototype.IsUserInTeam = function (teamName) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                if (teamName === null || typeof teamName === 'undefined')
                    return reject(new Error("Error in IsUserInTeam: teamName is null"));
                _this._GetUsersTeamNames().then(function (teams) {
                    resolve(teams.indexOf(teamName) > -1);
                }).catch(function (error) {
                    console.log("Error in IsUserInTeam: " + error);
                    reject(new Error(error));
                });
            });
        };
        Security.prototype.UserHasPrivilege = function (privilegeName) {
            var _this = this;
            return new Promise(function (resolve, reject) {
                var webApi = _this._webApi;
                var privilegeId = null;
                return webApi.RetrieveMultiple("privileges", "name,privilegeid", "$filter=name eq '" + privilegeName + "'").then(function (resp) {
                    if (resp === null || resp.value === null || typeof resp.value === 'undefined' || resp.value[0] === null)
                        return reject(new Error("UserHasPrivilege: could not find privilege " + privilegeName));
                    privilegeId = resp.value[0].privilegeid;
                    var userId = _globalCon.userSettings.userId.replace("{", "").replace("}", "");
                    return webApi.ExecuteRequest("GET", "systemusers(" + userId + ")/Microsoft.Dynamics.CRM.RetrieveUserPrivileges")
                }).then(function (resp) {
                    var hasPrivilege = false;
                    for (var i = 0; i < resp.RolePrivileges.length; ++i) {
                        if (resp.RolePrivileges[i].PrivilegeId === privilegeId) {
                            hasPrivilege = true;
                            break;
                        }
                    }
                    resolve(hasPrivilege)
                }).catch(function (error) {
                    console.log("Error in UserHasPrivilege: " + error.message);
                });
            });
        };
        return Security;
    }());
    CrmCommonJS.Security = Security
})(CrmCommonJS || (CrmCommonJS = {}));
//;
//var CrmCommonJS;
(function (CrmCommonJS) {
    var Util = (function () {
        function Util() { }
        Util.prototype.GetQueryString = function (key) {
            key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
            var qs = regex.exec(window.location.href);
            if (qs === null)
                return null;
            else
                return qs[1];
        };
        Util.prototype.GetQueryStringKey = function (key, query) {
            key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp(key + "=([^&#]*)");
            var qs = regex.exec(query);
            if (qs === null)
                return null;
            return qs[1];
        };
        Util.prototype.GetQueryStringParameterByName = function (name) {
            var url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results)
                return null;
            if (!results[2])
                return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        };
        Util.prototype.CapitalizeFirst = function (str) {
            if (str !== null)
                return str.charAt(0).toUpperCase() + str.slice(1);
            return null;
        };
        Util.FormatLongDate = function (dateStr) {
            var dateInt = dateStr.replace(")/", "");
            var date = new Date(parseInt(dateInt, 10));
            return date.getUTCFullYear() + "-" + (date.getUTCMonth() + 1) + "-" + date.getUTCDate();
        };
        Util.FormatShortDate = function (dateStr) {
            var dateInt = dateStr.replace(")/", "");
            var date = new Date(parseInt(dateInt, 10));
            return date.getUTCDate() + "/" + (date.getUTCMonth() + 1) + "/" + date.getUTCFullYear();
        };
        return Util;
    }());
    CrmCommonJS.Util = Util;
})(CrmCommonJS || (CrmCommonJS = {}));
;
//var CrmCommonJS;
(function (CrmCommonJS) {
    var WebApi = (function () {
        function WebApi(CrmVersion) {
            //console.log(exCon);
            //this.FormHelper = CrmCommonJS.FormHelper(exCon);
            //console.log(this.FormHelper);
            //this.formContext = FormHelper.GetFormContextOrDefault(exCon);
            //console.log(this.formContext);
            this.CrmVersion = CrmVersion;
            this._apiVersion = "v9.0";
            //this._clientUrl = Xrm.Page.context.getClientUrl();
            this._requestHeaders = [];
            this.ODataEntityName = {
                Role: "roles", Team: "teams", TeamMembership: "teammemberships"
            };
        }
        /*WebApi.prototype._GetUrl = function (entityType, guid, filter) {
            if (guid === void 0) {
                guid = null;
            }
            if (filter === void 0) {
                filter = null;
            }
            var url = this._clientUrl + "/api/data/" + this._apiVersion + "/" + entityType;
            if (guid !== null && typeof guid !== 'undefined') {
                guid = guid.toString().replace("{", "").replace("}", "");
                url = url + "(" + guid + ")";
            }
            if (filter !== null && typeof filter !== 'undefined')
                url = url + filter;
            return url;
        };
        WebApi.prototype.GetClientUrl = function () {
            return this._clientUrl;
        };*/
        WebApi.prototype.AddRequestHeader = function (key, value) {
            this._requestHeaders.push(new RequestHeader(key, value));
        };

        //To associate a record on create, set the lookup field using the following before calling method:
        //entity["<fieldnameoflookup>@odata.bind"] = "/<entitytypeofassociatedentity>(" + <guidofassociatedentity> +")
        WebApi.prototype.CreateRecord = function (entity, entityType) {
            return new Promise(function (resolve, reject) {
                if (entity === null || typeof entity === 'undefined') {
                    return reject("CreateRecord error: entity is null");
                }
                if (entityType === null || typeof entityType === 'undefined') {
                    return reject("CreateRecord error: entityType is null");
                }
                Xrm.WebApi.createRecord(entityType, entity).then(
                    function success(record) {
                        console.log(record);
                        resolve(record);
                    }, function (error) {
                        reject(error);
                    }
                );
            });
        };
        WebApi.prototype.DeleteRecord = function (guid, entityType) {
            return new Promise(function (resolve, reject) {
                if (guid === null || typeof guid === 'undefined')
                    return reject("DeleteRecord error: guid is null");

                if (entityType === null || typeof entityType === 'undefined')
                    return reject("DeleteRecord error: entityType is null");
                Xrm.WebApi.deleteRecord(entityType, guid).then(function () {
                    console.log("DELETE OK - " + guid);
                    resolve(true);
                }).catch(function (error) {
                    console.log("Error during DELETE of guid " + guid);
                    reject(new Error(error));
                });
            });
        };
        //To associate a record on update, set the lookup field using the following before calling method:
        WebApi.prototype.UpdateRecord = function (guid, entityType, entity) {
            return new Promise(function (resolve, reject) {
                if (guid === null || typeof guid === 'undefined')
                    return reject(new Error("UpdateRecord error: guid is null"));
                if (entityType === null || typeof entityType === 'undefined')
                    return reject(new Error("UpdateRecord error: entityType is null"));
                if (entity === null || typeof entity === 'undefined')
                    return reject(new Error("UpdateRecord error: entity is null"));
                Xrm.WebApi.updateRecord(entityType, guid, entity)
                    .then(
                        function success(record) {
                            resolve(record);
                        }, function (error) {
                            reject(error);
                        }
                    );
            });
        };
        WebApi.prototype.RetrieveRecord = function (guid, entityType, cols) {
            return new Promise(function (resolve, reject) {
                if (guid === null || typeof guid === 'undefined') {
                    return reject(new Error("RetrieveRecord error: guid is null"));
                } else {
                    guid = guid.replace(/{/gi, "").replace(/}/gi, "");
                }

                if (entityType === null || typeof entityType === 'undefined')
                    return reject(new Error("RetrieveRecord error: entityType is null"));
                var query = null;
                if (cols !== null && typeof cols !== 'undefined') {
                    query = "?$select=" + cols.join(',');
                }
                Xrm.WebApi.retrieveRecord(entityType, guid, query).then(
                    function (response) {
                        return resolve(response);
                    },
                    function (error) {
                        return reject(new Error(error));
                    });
            });
        };
        //returns array of records
        WebApi.prototype.RetrieveMultiple = function (entityType, cols, filter) {
            return new Promise(function (resolve, reject) {
                if (entityType === null || typeof entityType === 'undefined')
                    reject(new Error("RetrieveMultiple error: entityType is null"));
                var query = null;
                if (cols !== null && typeof cols !== 'undefined') {
                    query = "?$select=" + cols.join(',');
                }
                if (filter !== null && typeof filter !== 'undefined') {
                    query === null ? query = "?" + filter : query += "&" + filter;
                }
                var allRecords = [];
                //Retrieves all records by recursion.  Gets around record limit on retrieve.
                allRecords = WebApi.prototype.doRetrieve(entityType, query, allRecords);
                resolve(allRecords);

            });
        };
        WebApi.prototype.doRetrieve = function (entityType, query, records) {
            return new Promise(function (resolve, reject) {
                Xrm.WebApi.retrieveMultipleRecords(entityType, query).then(
                    function success(response) {
                        records = records.concat(response.entities);
                        if (response.nextLink !== undefined) {
                            var newQuery = response.nextLink.substring(response.nextLink.indexOf("?"), response.nextLink.length);
                            records = WebApi.prototype.doRetrieve(entityType, newQuery, records);
                        }
                        resolve(records);
                    }, function (error) {
                        console.log(error);
                        reject(error);
                    });
            });
        };

        WebApi.prototype.RetrieveByFetchXml = function (entityType, fetchXml) {
            return new Promise(function (resolve, reject) {
                if (fetchXml === null || typeof fetchXml === 'undefined')
                    return reject(new Error("RetrieveByFetchXml error: fetchXml is null"));
                if (entityType === null || typeof entityType === 'undefined')
                    return reject(new Error("RetrieveByFetchXml error: entityType is null"));
                var fetch = "?fetchXml=" + encodeURIComponent(fetchXml);
                //_this._MakeRequest("GET", url).then(function (response) {
                Xrm.WebApi.retrieveMultipleRecords(entityType, fetch, null).then(
                    function success(response) {
                        records = response.entities;
                        resolve(records);
                    },
                    function (error) {
                        console.log("Error during RetrieveByFetchXml MakeRequest: " + error);
                        reject(error);
                    });
            });
        };
        //For Associate/Dissassociate, use update or create
        //WebApi.prototype.AssociateRecords = function (guid1, type1, guid2, type2) { };
        //WebApi.prototype.DisassociateRecords = function (guid1, type1, guid2, type2) { };*/


        WebApi.prototype.executeAction = function (actionRequest) {
            return new Promise(function (resolve, reject) {
                if (Xrm.WebApi.execute) {
                    Xrm.WebApi.execute(actionRequest).then(
                        function success(result) {
                            return resolve(result);
                        },
                        function (error) {
                            console.log(error.message);
                            return reject(new Error(error));
                        }
                    );
                }
                else {
                    Xrm.WebApi.online.execute(actionRequest).then(
                        function success(result) {
                            return resolve(result);
                        },
                        function (error) {
                            console.log(error.message);
                            return reject(new Error(error));
                        }
                    );
                }
            });

        };
        //See https://docs.microsoft.com/en-us/dynamics365/customer-engagement/developer/clientapi/reference/xrm-webapi/online/execute
        //for details on how to construct Metadata object
        WebApi.prototype.ExecuteRequest = function (metaData, data) {
            //var _this = this;
            return new Promise(function (resolve, reject) {
                //if (method === null || typeof method === 'undefined')
                //    return reject(new Error("ExecuteRequest error: method is null"));
                if (metaData === null || typeof metaData === 'undefined')
                    return reject(new Error("ExecuteRequest error: metaData is null"));
                //var url = _this._GetUrl(msgName);
                var reqObj;
                if (data) {
                    reqObj = WebApi.prototype.RequestObj(metaData, data);
                } else {
                    reqObj = metaData;
                }
                // _this._MakeRequest(method, url, data).then(function (response) {    
                Xrm.WebApi.execute(reqObj).then(function (response) {
                    console.log("EXECUTE " + metaData.operationName + " - OK");
                    console.log(response);
                    if (response.responseText !== "")
                        resolve(response);
                    else
                        resolve(null);
                },
                    function (error) {
                        console.log("Error: during EXECUTE " + metaData.operationName + ": " + error.message);
                        reject(new Error(error.message));
                    }
                );
            });
        };
        //Creates a request object for use by the Web API execute method.
        WebApi.prototype.RequestObj = function (metaDataObj, data) {

            /*$.each(data, function(index, parameter){                
                this.parameter.key = parameter.value;
            });*/
            var request = {
                getMetadata: function () {
                    var metaData = {
                        boundParameter: metaDataObj.boundParameter,
                        parameterTypes: metaDataObj.parameterTypes,
                        operationName: metaDataObj.operationName,
                        operationType: metaDataObj.operationType
                    };
                    return metaData;
                }
            };
            $.each(data, function (key, value) {
                request[key] = value;
            });
            return request;
        };

        WebApi.prototype.SetLookupValue = function (guid, entityType) {
            var url = this._GetUrl(entityType, guid);
            return url;
        };

        WebApi.prototype.PromiseTimeout = function (ms, promise) {
            return new Promise(function (resolve, reject) {
                var timer = setTimeout(function () {
                    reject(new Error("timeout"));
                }, ms);

                promise
                    .then(function (response) {
                        clearTimeout(timer);
                        resolve(response);
                    })
                    .catch(function (error) {
                        clearTimeout(timer);
                        reject(error);
                    });
            });
        };

        return WebApi;
    }());
    CrmCommonJS.WebApi = WebApi;
    var RequestHeader = (function () {
        function RequestHeader(key, value) {
            this.key = key;
            this.value = value;
        }
        return RequestHeader;
    }());
})(CrmCommonJS || (CrmCommonJS = {}));