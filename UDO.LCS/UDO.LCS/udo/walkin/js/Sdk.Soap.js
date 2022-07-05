﻿// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================
"use strict";
var Sdk = window.Sdk || {
    __namespace: !0
}, constructorNotImplementedError;
Sdk.Query = Sdk.Query || {
    __namespace: !0
}, Sdk.Async = Sdk.Async || {
    __namespace: !0
}, Sdk.Q = Sdk.Q || {
    __namespace: !0
}, Sdk.jQ = Sdk.jQ || {
    __namespace: !0
}, Sdk.Sync = Sdk.Sync || {
    __namespace: !0
}, Sdk.Util = Sdk.Util || {
    __namespace: !0
}, Sdk.Xml = Sdk.Xml || {
    __namespace: !0
}, constructorNotImplementedError = "Constructor not implemented this is a static enum.",
function() {
    var n = {
        s: "http://schemas.xmlsoap.org/soap/envelope/",
        a: "http://schemas.microsoft.com/xrm/2011/Contracts",
        i: "http://www.w3.org/2001/XMLSchema-instance",
        b: "http://schemas.datacontract.org/2004/07/System.Collections.Generic",
        c: "http://www.w3.org/2001/XMLSchema",
        d: "http://schemas.microsoft.com/xrm/2011/Contracts/Services",
        e: "http://schemas.microsoft.com/2003/10/Serialization/",
        f: "http://schemas.microsoft.com/2003/10/Serialization/Arrays",
        g: "http://schemas.microsoft.com/crm/2011/Contracts",
        h: "http://schemas.microsoft.com/xrm/2011/Metadata",
        j: "http://schemas.microsoft.com/xrm/2011/Metadata/Query",
        k: "http://schemas.microsoft.com/xrm/2013/Metadata",
        l: "http://schemas.microsoft.com/xrm/2012/Contracts"
    };
    this.getEnvelopeHeader = function() {
        var t = ["<s:Envelope "],
            i;
        for (i in n) t.push(" xmlns:" + i + '="' + n[i] + '"');
        return t.push("><s:Header><a:SdkClientVersion>6.0</a:SdkClientVersion></s:Header>"), t.join("")
    }, this.setSelectionNamespaces = function(t) {
        var i, r;
        if (typeof t.setProperty != "undefined") {
            i = [];
            for (r in n) i.push("xmlns:" + r + "='" + n[r] + "'");
            t.setProperty("SelectionNamespaces", i.join(" "))
        }
    }, this.NSResolver = function(t) {
        return n[t] || null
    }, this.selectNodes = function(n, t) {
        if (typeof n.selectNodes != "undefined") return n.selectNodes(t);
        for (var r = [], u = n.evaluate(t, n, Sdk.Xml.NSResolver, XPathResult.ANY_TYPE, null), i = u.iterateNext(); i;) r.push(i), i = u.iterateNext();
        return r
    }, this.selectSingleNode = function(n, t) {
        if (typeof n.selectSingleNode != "undefined") return n.selectSingleNode(t);
        var r = new XPathEvaluator,
            i = r.evaluate(t, n, Sdk.Xml.NSResolver, XPathResult.FIRST_ORDERED_NODE_TYPE, null);
        return i != null ? i.singleNodeValue : null
    }, this.selectSingleNodeText = function(n, t) {
        var i = Sdk.Xml.selectSingleNode(n, t);
        return Sdk.Xml.isNodeNull(i) ? null : typeof i.text != "undefined" ? i.text : i.textContent
    }, this.getNodeText = function(n) {
        return Sdk.Xml.isNodeNull(n) ? null : typeof n.text != "undefined" ? n.text : n.textContent
    }, this.isNodeNull = function(n) {
        return n == null ? !0 : n.attributes != null && n.attributes.getNamedItem("i:nil") != null && n.attributes.getNamedItem("i:nil").value == "true" ? !0 : !1
    }, this.getNodeName = function(n) {
        return typeof n.baseName != "undefined" ? n.baseName : n.localName
    }, this.xmlEncode = function(n) {
        var t, i = "",
            r;
        if (n == null) return null;
        if (n == "") return "";
        for (r = 0; r < n.length; r++) t = n.charCodeAt(r), i = t > 96 && t < 123 || t > 64 && t < 91 || t == 32 || t > 47 && t < 58 || t == 46 || t == 44 || t == 45 || t == 95 ? i + String.fromCharCode(t) : i + "&#" + t + ";";
        return i
    }, this.serializeXmlNode = function(n) {
        return typeof n.xml != "undefined" ? n.xml : (new XMLSerializer).serializeToString(n)
    }
}.call(Sdk.Xml),
function() {
    this.AccessRights = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.AttributeBase = function() {
        function f(n) {
            if (typeof n == "string") i = n;
            else throw new Error("Sdk.AttributeBase Name property must be a string.");
        }

        function e(i) {
            if (typeof i == "undefined") throw new Error("Sdk.AttributeBase Value property must not be undefined.");
            if (i instanceof Date && n instanceof Date) {
                if (i.toISOString() == n.toISOString()) return
            } else if (i == n) return;
            n = i, t = !0, u = !0
        }

        function o(n) {
            for (var t in Sdk.ValueType)
                if (n == t) {
                    r = n;
                    return
                }
            throw new Error("Sdk.AttributeBase Type property must be an Sdk.ValueType.");
        }

        function s(n) {
            if (typeof n == "boolean") t = n;
            else throw new Error("Sdk.AttributeBase IsChanged property must be a Boolean.");
        }
        if (!(this instanceof Sdk.AttributeBase)) return new Sdk.AttributeBase;
        var i, r, n = null,
            t = !1,
            u = !1;
        this.getIsChanged = function() {
            return t
        }, this.getName = function() {
            return i
        }, this.getType = function() {
            return r
        }, this.getValue = function() {
            return n
        }, this.isValueSet = function() {
            return u
        }, this.setIsChanged = function(n) {
            s(n)
        }, this.setName = function(n) {
            f(n)
        }, this.setType = function(n) {
            o(n)
        }, this.setValidValue = function(n) {
            e(n)
        }
    }, this.Boolean = function(n, t) {
        if (!(this instanceof Sdk.Boolean)) return new Sdk.Boolean(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (typeof n == "boolean") this.setValidValue(n);
            else throw new Error("Sdk.Boolean value property must be a Boolean.");
        }, this.setName(n), this.setType(Sdk.ValueType.boolean), typeof t != "undefined" && this.setValue(t)
    }, this.Boolean.__class = !0, this.BooleanManagedProperty = function(n, t) {
        if (!(this instanceof Sdk.BooleanManagedProperty)) return new Sdk.BooleanManagedProperty(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n instanceof Sdk.BooleanManagedPropertyValue) this.setValidValue(n);
            else throw new Error("Sdk.BooleanManagedProperty value property must be a Sdk.BooleanManagedPropertyValue.");
        }, this.setName(n), this.setType(Sdk.ValueType.booleanManagedProperty), typeof t != "undefined" && this.setValue(t)
    }, this.BooleanManagedProperty.__class = !0, this.DateTime = function(n, t) {
        if (!(this instanceof Sdk.DateTime)) return new Sdk.DateTime(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n.getMonth == "function") this.setValidValue(n);
            else throw new Error("Sdk.DateTime value property must be a Date.");
        }, this.setName(n), this.setType(Sdk.ValueType.dateTime), typeof t != "undefined" && this.setValue(t)
    }, this.DateTime.__class = !0, this.Decimal = function(n, t) {
        if (!(this instanceof Sdk.Decimal)) return new Sdk.Decimal(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n == "number") this.setValidValue(n);
            else throw new Error("Sdk.Decimal value property must be a Number.");
        }, this.setName(n), this.setType(Sdk.ValueType.decimal), typeof t != "undefined" && this.setValue(t)
    }, this.Decimal.__class = !0, this.Double = function(n, t) {
        if (!(this instanceof Sdk.Double)) return new Sdk.Double(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n == "number") this.setValidValue(n);
            else throw new Error("Sdk.Double value property must be a Number.");
        }, this.setName(n), this.setType(Sdk.ValueType.double), typeof t != "undefined" && this.setValue(t)
    }, this.Double.__class = !0, this.Guid = function(n, t) {
        if (!(this instanceof Sdk.Guid)) return new Sdk.Guid(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (Sdk.Util.isGuidOrNull(n)) this.setValidValue(n);
            else throw new Error("Sdk.Guid value property must be a String representation of a Guid value.");
        }, this.setName(n), this.setType(Sdk.ValueType.guid), typeof t != "undefined" && this.setValue(t)
    }, this.Guid.__class = !0, this.Int = function(n, t) {
        if (!(this instanceof Sdk.Int)) return new Sdk.Int(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n == "number") this.setValidValue(parseInt(n, 10));
            else throw new Error("Sdk.Int value property must be a Number.");
        }, this.setName(n), this.setType(Sdk.ValueType.int), typeof t != "undefined" && this.setValue(t)
    }, this.Int.__class = !0, this.Long = function(n, t) {
        if (!(this instanceof Sdk.Long)) return new Sdk.Long(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n == "number") this.setValidValue(n);
            else throw new Error("Sdk.Long value property must be a Number.");
        }, this.setName(n), this.setType(Sdk.ValueType.long), typeof t != "undefined" && this.setValue(t)
    }, this.Long.__class = !0, this.Lookup = function(n, t) {
        if (!(this instanceof Sdk.Lookup)) return new Sdk.Lookup(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || n instanceof Sdk.EntityReference) this.setValidValue(n);
            else throw new Error("Sdk.Lookup value property must be a Sdk.EntityReference.");
        }, this.setName(n), this.setType(Sdk.ValueType.entityReference), typeof t != "undefined" && this.setValue(t)
    }, this.Lookup.__class = !0, this.Money = function(n, t) {
        if (!(this instanceof Sdk.Money)) return new Sdk.Money(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n == "number") this.setValidValue(n);
            else throw new Error("Sdk.Money value property must be a Number.");
        }, this.setName(n), this.setType(Sdk.ValueType.money), typeof t != "undefined" && this.setValue(t)
    }, this.Money.__class = !0, this.OptionSet = function(n, t) {
        if (!(this instanceof Sdk.OptionSet)) return new Sdk.OptionSet(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n == "number") this.setValidValue(n);
            else throw new Error("Sdk.OptionSet value property must be a Number.");
        }, this.setName(n), this.setType(Sdk.ValueType.optionSet), typeof t != "undefined" && this.setValue(t)
    }, this.OptionSet.__class = !0, this.PartyList = function(n, t) {
        if (!(this instanceof Sdk.PartyList)) return new Sdk.PartyList(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || n instanceof Sdk.EntityCollection) this.setValidValue(n);
            else throw new Error("Sdk.PartyList value property must be a Sdk.EntityCollection.");
        }, this.setName(n), this.setType(Sdk.ValueType.entityCollection), typeof t != "undefined" && this.setValue(t)
    }, this.PartyList.__class = !0, this.String = function(n, t) {
        if (!(this instanceof Sdk.String)) return new Sdk.String(n, t);
        Sdk.AttributeBase.call(this), this.setValue = function(n) {
            if (n == null || typeof n == "string") this.setValidValue(n);
            else throw new Error("Sdk.String value property must be a String.");
        }, this.setName(n), this.setType(Sdk.ValueType.string), typeof t != "undefined" && this.setValue(t)
    }, this.String.__class = !0, this.BooleanManagedPropertyValue = function(n, t, i) {
        function e(n) {
            if (typeof n == "boolean") r = n;
            else throw new Error("Sdk.BooleanManagedPropertyValue CanBeChanged property must be an Boolean.");
        }

        function o(n) {
            if (typeof n == "boolean") f = n;
            else throw new Error("Sdk.BooleanManagedPropertyValue Value property must be a Boolean.");
        }

        function s(n) {
            if (typeof n == "string" || n == null) u = n;
            else throw new Error("Sdk.BooleanManagedPropertyValue ManagedPropertyLogicalName property must be a String.");
        }
        if (!(this instanceof Sdk.BooleanManagedPropertyValue)) return new Sdk.BooleanManagedPropertyValue(n, t, i);
        var r = null,
            u = null,
            f = null;
        typeof n != "undefined" && e(n), typeof i != "undefined" && s(i), typeof t != "undefined" && o(t), this.getCanBeChanged = function() {
            return r
        }, this.setCanBeChanged = function(n) {
            e(n)
        }, this.getManagedPropertyLogicalName = function() {
            return u
        }, this.setManagedPropertyLogicalName = function(n) {
            s(n)
        }, this.getValue = function() {
            return f
        }, this.setValue = function(n) {
            o(n)
        }
    }, this.BooleanManagedPropertyValue.__class = !0, this.ValueType = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.AttributeCollection = function() {
        function t(t) {
            var i = !1;
            return n.forEach(function(n) {
                if (n.getName() == t) {
                    i = !0;
                    return
                }
            }), i
        }

        function i(i, r) {
            var f = !0,
                u;
            if (f = r != null && typeof r != "undefined" && typeof r == "boolean" ? r : i.getIsChanged(), i instanceof Sdk.AttributeBase) t(i.getName()) ? (n.forEach(function(n) {
                n.getName() == i.getName() && (u = n)
            }), u.getType() == i.getType() ? i.isValueSet() && u.getValue() != i.getValue() && u.setValue(i.getValue()) : (n.remove(u), i.setIsChanged(f), n.add(i))) : (i.setIsChanged(f), n.add(i));
            else throw new Error("Sdk.AttributeCollection add method requires an Sdk.AttributeBase parameter");
        }
        if (!(this instanceof Sdk.AttributeCollection)) return new Sdk.AttributeCollection;
        var n = new Sdk.Collection(Sdk.AttributeBase);
        this.add = function(n, t) {
            i(n, t)
        }, this.getAttributes = function() {
            return n
        }
    }, this.AttributeCollection.__class = !0, this.AuditDetail = function(n) {
        function i(n) {
            if (n instanceof Sdk.Entity) t = n;
            else throw new Error("Sdk.AuditDetail AuditRecord property must be an Sdk.Entity.");
        }
        if (!(this instanceof Sdk.AuditDetail)) return new Sdk.AuditDetail(n);
        var t = null;
        typeof n != "undefined" && i(n), this.getAuditRecord = function() {
            return t
        }, this.setAuditRecord = function(n) {
            i(n)
        }
    }, this.AuditDetail.__class = !0, this.AttributeAuditDetail = function(n) {
        function r(n) {
            if (n == null || n instanceof Sdk.Entity) t = n;
            else throw new Error("Sdk.AttributeAuditDetail NewValue property must be an Sdk.Entity or null.");
        }

        function u(n) {
            if (n == null || n instanceof Sdk.Entity) i = n;
            else throw new Error("Sdk.AttributeAuditDetail OldValue property must be an Sdk.Entity or null.");
        }
        if (!(this instanceof Sdk.AttributeAuditDetail)) return new Sdk.AttributeAuditDetail(n);
        Sdk.AuditDetail.call(this, n);
        var t = null,
            i = null;
        this.getNewValue = function() {
            return t
        }, this.getOldValue = function() {
            return i
        }, this.setNewValue = function(n) {
            r(n)
        }, this.setOldValue = function(n) {
            u(n)
        }
    }, this.AttributeAuditDetail.__class = !0, this.RelationshipAuditDetail = function(n) {
        function r(n) {
            if (typeof n == "string") t = n;
            else throw new Error("Sdk.RelationshipAuditDetail RelationshipName property must be an String.");
        }

        function u(n) {
            if (Sdk.Util.isCollectionOf(n, Sdk.EntityReference)) i = n;
            else throw new Error("Sdk.RelationshipAuditDetail TargetRecords property must be an Sdk.Collection of Sdk.EntityReference.");
        }
        if (!(this instanceof Sdk.RelationshipAuditDetail)) return new Sdk.RelationshipAuditDetail(n);
        Sdk.AuditDetail.call(this, n);
        var t = null,
            i = new Sdk.Collection(Sdk.EntityReference);
        this.getRelationshipName = function() {
            return t
        }, this.getTargetRecords = function() {
            return i
        }, this.setRelationshipName = function(n) {
            r(n)
        }, this.setTargetRecords = function(n) {
            u(n)
        }
    }, this.RelationshipAuditDetail.__class = !0, this.RolePrivilegeAuditDetail = function(n) {
        function r(n) {
            if (Sdk.Util.isCollectionOf(n, Sdk.RolePrivilege)) t = n;
            else throw new Error("Sdk.RolePrivilegeAuditDetail NewRolePrivileges property must be an Sdk.Collection of Sdk.RolePrivilege.");
        }

        function u(n) {
            if (Sdk.Util.isCollectionOf(n, Sdk.RolePrivilege)) i = n;
            else throw new Error("Sdk.RolePrivilegeAuditDetail OldRolePrivileges property must be an Sdk.Collection of Sdk.RolePrivilege.");
        }
        if (!(this instanceof Sdk.RolePrivilegeAuditDetail)) return new Sdk.RolePrivilegeAuditDetail(n);
        Sdk.AuditDetail.call(this, n);
        var t = new Sdk.Collection(Sdk.RolePrivilege),
            i = new Sdk.Collection(Sdk.RolePrivilege);
        this.getNewRolePrivileges = function() {
            return t
        }, this.getOldRolePrivileges = function() {
            return i
        }, this.setNewRolePrivileges = function(n) {
            r(n)
        }, this.setOldRolePrivileges = function(n) {
            u(n)
        }
    }, this.RolePrivilegeAuditDetail.__class = !0, this.ShareAuditDetail = function(n) {
        function u(n) {
            if (typeof n == "string") t = n;
            else throw new Error("Sdk.ShareAuditDetail NewPrivileges  property must be a String.");
        }

        function f(n) {
            if (typeof n == "string") i = n;
            else throw new Error("Sdk.ShareAuditDetail OldPrivileges  property must be a String.");
        }

        function e(n) {
            if (n instanceof Sdk.EntityReference) r = n;
            else throw new Error("Sdk.ShareAuditDetail Principal property must be an Sdk.EntityReference.");
        }
        if (!(this instanceof Sdk.ShareAuditDetail)) return new Sdk.ShareAuditDetail(n);
        Sdk.AuditDetail.call(this, n);
        var t = null,
            i = null,
            r = null;
        this.getNewPrivileges = function() {
            return t
        }, this.getOldPrivileges = function() {
            return i
        }, this.getPrincipal = function() {
            return r
        }, this.setNewPrivileges = function(n) {
            u(n)
        }, this.setOldPrivileges = function(n) {
            f(n)
        }, this.setPrincipal = function(n) {
            e(n)
        }
    }, this.ShareAuditDetail.__class = !0, this.AuditDetailCollection = function() {
        function u(t) {
            if (Sdk.Util.isCollectionOf(t, Sdk.AuditDetail)) n = t;
            else throw new Error("Sdk.AuditDetailCollection AuditDetails property must be an Sdk.Collection of Sdk.AuditDetail.");
        }

        function f(n) {
            if (typeof n == "boolean") t = n;
            else throw new Error("Sdk.AuditDetailCollection MoreRecords property must be a Boolean.");
        }

        function e(n) {
            if (typeof n == "string" || n == null) i = n;
            else throw new Error("Sdk.AuditDetailCollection PagingCookie property must be an String");
        }

        function o(n) {
            if (typeof n == "number") r = n;
            else throw new Error("Sdk.AuditDetailCollection TotalRecordCount property must be an Number");
        }
        if (!(this instanceof Sdk.AuditDetailCollection)) return new Sdk.AuditDetailCollection;
        var n = new Sdk.Collection(Sdk.AuditDetail),
            t = null,
            i = null,
            r = null;
        this.getAuditDetails = function() {
            return n
        }, this.getMoreRecords = function() {
            return t
        }, this.getPagingCookie = function() {
            return i
        }, this.getTotalRecordCount = function() {
            return r
        }, this.setAuditRecord = function(n) {
            u(n)
        }, this.setMoreRecords = function(n) {
            f(n)
        }, this.setPagingCookie = function(n) {
            e(n)
        }, this.setTotalRecordCount = function(n) {
            o(n)
        }
    }, this.AuditDetailCollection.__class = !0, this.Collection = function(n, t) {
        if (!(this instanceof Sdk.Collection)) return new Sdk.Collection(n, t);
        if (typeof n != "function") throw new Error("Sdk.Collection type parameter is required and must be a function.");
        var u = n,
            f = "The value being added to the Sdk.Collection is not the expected type. The expected type is " + u.toString(),
            i = [],
            r = 0;
        this.getType = function() {
            return u
        }, this.add = function(n) {
            if (u == String) {
                if (typeof n == "string") {
                    i.push(n), r++;
                    return
                }
                throw new Error(f);
            }
            if (u == Number) {
                if (typeof n == "number") {
                    i.push(n), r++;
                    return
                }
                throw new Error(f);
            }
            if (u == Boolean) {
                if (typeof n == "boolean") {
                    i.push(n), r++;
                    return
                }
                throw new Error(f);
            } else {
                if (n instanceof u) {
                    i.push(n), r++;
                    return
                }
                throw new Error(f);
            }
        }, this.addRange = function(n) {
            var i = "Sdk.Collection.addRange requires an array parameter.",
                t;
            if (n != null)
                if (typeof n.push != "undefined")
                    for (t = 0; t < n.length; t++) this.add(n[t]);
                else throw new Error(i);
                else throw new Error(i);
        }, this.clear = function() {
            i.length = 0, r = 0
        }, this.contains = function(n) {
            for (var t = 0; t < i.length; t++)
                if (n === i[t]) return !0;
            return !1
        }, this.forEach = function(n) {
            for (var t = 0; t < i.length; t++) n(i[t], t)
        }, this.getByIndex = function(n) {
            if (typeof n == "number")
                if (n >= r) throw new Error("Out of range. Sdk.Collection.getByIndex index parameter must be within the number of items in the collection.");
                else return i[n];
            throw new Error("Sdk.Collection.getByIndex index parameter must be a Number.");
        }, this.remove = function(n) {
            if (n != null && typeof n != "undefined") {
                for (var t = 0; t < i.length; t++)
                    if (n === i[t]) {
                        i.splice(t, 1), r--;
                        return
                    }
                throw new Error(Sdk.Util.format("Item {0} not found.", [n.toString()]));
            } else throw new Error("Sdk.Collection.remove item parameter must not be null or undefined.");
        }, this.toArray = function() {
            return i.slice(0)
        }, this.getCount = function() {
            return r
        }, t != null && this.addRange(t)
    }, this.Collection.__class = !0, this.ColumnSet = function() {
        function f(n) {
            if (typeof n == "boolean") i = n;
            else throw new Error("Sdk.ColumnSet AllColumns must be a boolean.");
        }

        function r(n) {
            if (typeof n == "string") t.add(n);
            else throw new Error("Sdk.ColumnSet columns must be strings");
        }
        var u = "Sdk.ColumnSet constructor parameter can accept a boolean value as the first parameter, an array of strings, or a series of string values.",
            t, i, n;
        if (!(this instanceof Sdk.ColumnSet))
            if (arguments.length > 0) {
                if (typeof arguments[0] == "boolean" || typeof arguments[0].push != "undefined") return new Sdk.ColumnSet(arguments[0]);
                if (typeof arguments[0] == "string") return new Sdk.ColumnSet(Array.prototype.slice.call(arguments));
                throw new Error(u);
            } else return new Sdk.ColumnSet;
        if (t = new Sdk.Collection(String), i = !1, arguments.length > 0)
            if (typeof arguments[0] == "boolean")
                if (arguments[0]) i = !0;
                else throw new Error(u);
                else if (typeof arguments[0].push != "undefined")
            for (n = 0; n < arguments[0].length; n++)
                if (typeof arguments[0][n] == "string") r(arguments[0][n].toLowerCase());
                else throw new Error("Sdk.ColumnSet constructor parameter can accept an Array of string values.");
                else
                    for (n = 0; n < arguments.length; n++)
                        if (typeof arguments[n] != "string") throw new Error("Sdk.ColumnSet constructor parameter can accept a series of string values.");
                        else r(arguments[n].toLowerCase());
        this.getColumns = function() {
            return t
        }, this.addColumn = function(n) {
            r(n)
        }, this.addColumns = function(n) {
            if (n instanceof Array)
                for (var t = 0; t < n.length; t++) r(n[t])
        }, this.setAllColumns = function(n) {
            f(n)
        }, this.getAllColumns = function() {
            return i
        }, this.getCount = function() {
            return t.getCount()
        }
    }, this.ColumnSet.__class = !0, this.Entity = function(n) {
        function s(n) {
            if (n instanceof Sdk.AttributeCollection) i = n;
            else throw new Error("Sdk.Entity Attributes property must be an Sdk.AttributeCollection");
        }

        function h(n) {
            if (n == null || n == Sdk.EntityState.Changed || n == Sdk.EntityState.Created || n == Sdk.EntityState.Unchanged) r = n;
            else throw new Error("Sdk.Entity EntityState property must be an Sdk.EntityState value or null.");
        }

        function c(n) {
            if (n instanceof Sdk.FormattedValueCollection) u = n;
            else throw new Error("Sdk.Entity FormattedValues property must be an Sdk.FormattedValueCollection");
        }

        function l(n, i) {
            if (t == null || i)
                if (n != null && Sdk.Util.isGuidOrNull(n)) t = n;
                else throw new Error("Sdk.Entity Id property must be an string representation of a GUID value");
                else if (n != t) throw new Error("Sdk.Entity Id cannot be changed once it is set without explicitly setting the override parameter.");
        }

        function o(n) {
            if (typeof n == "string") f = n;
            else throw new Error("Sdk.Entity Type property must be an String");
        }

        function a(n) {
            if (n instanceof Sdk.RelatedEntitiesCollection) e = n;
            else throw new Error("Sdk.Entity RelatedEntities property must be an Sdk.RelatedEntitiesCollection");
        }
        if (!(this instanceof Sdk.Entity)) return new Sdk.Entity(n);
        var i = new Sdk.AttributeCollection,
            r = null,
            u = new Sdk.FormattedValueCollection,
            t = null,
            f = null,
            e = new Sdk.RelatedEntitiesCollection;
        (typeof n != "undefined" || n != null) && o(n), this.getAttributes = function(n) {
                if (n == null) return i;
                if (typeof n == "string" || typeof n == "number") return i.get(n);
                throw new Error("Invalid argument: Sdk.Entity getAttributes method args parameter must be either null, a string or a number.");
        }, this.setAttributes = function(n) {
            s(n)
        }, this.getEntityState = function() {
            return r
        }, this.setEntityState = function(n) {
            h(n)
        }, this.getFormattedValues = function() {
            return u
        }, this.setFormattedValues = function(n) {
            c(n)
        }, this.getId = function() {
            return t
        }, this.setId = function(n, t) {
            l(n, t)
        }, this.getType = function() {
            return f
        }, this.setType = function(n) {
            o(n)
        }, this.getRelatedEntities = function() {
            return e
        }, this.setRelatedEntities = function(n) {
            a(n)
        }
    }, this.Entity.__class = !0, this.EntityCollection = function(n) {
        function s(n) {
            if (n instanceof Sdk.Collection && n.getType() == Sdk.Entity) t = n;
            else throw new Error("Sdk.EntityCollection entities property must be an Sdk.Collection of Sdk.Entity");
        }

        function h(n) {
            if (typeof n == "string") r = n;
            else throw new Error("Sdk.EntityCollection entityName property must be an String");
        }

        function c(n) {
            if (typeof n == "string") u = n;
            else throw new Error("Sdk.EntityCollection minActiveRowVersion property must be an String");
        }

        function l(n) {
            if (typeof n == "boolean") f = n;
            else throw new Error("Sdk.EntityCollection moreRecords property must be an Boolean");
        }

        function a(n) {
            if (typeof n == "string" || n == null) e = n;
            else throw new Error("Sdk.EntityCollection pagingCookie property must be an String");
        }

        function v(n) {
            if (typeof n == "boolean") o = n;
            else throw new Error("Sdk.EntityCollection totalRecordCountExeeded property must be an Boolean");
        }
        if (!(this instanceof Sdk.EntityCollection)) return new Sdk.EntityCollection(n);
        var t = new Sdk.Collection(Sdk.Entity),
            r = null,
            u = null,
            f = !1,
            e = null,
            i = 0,
            o = !1;
        (typeof n != "undefined" || n != null) && s(n), this.addEntity = function(n) {
                t.add(n)
        }, this.getEntities = function() {
            return t
        }, this.getEntity = function(n) {
            var r = "Sdk.EntityCollection getEntity method indexOrId parameter must be a number or an string value",
                i;
            if (typeof n == "undefined" || n == null) throw new Error(r);
            if (typeof n == "number") try {
                return t.getByIndex(n)
            } catch (u) {
                throw new Error("Index outside of range. The entities collection contains " + t.getCount() + " entities");
            } else {
                if (Sdk.Util.isGuidOrNull(n)) return i = null, t.forEach(function(t) {
                    t.getId() == n && (i = t)
                }), i;
                throw new Error(r);
            }
        }, this.setEntity = function(n, i) {
            var u = "Sdk.EntityCollection setEntity method indexOrId parameter must be a number or an string value",
                r;
            if (typeof n == "undefined" || n == null) throw new Error(u);
            if (i instanceof Sdk.Entity)
                if (typeof n == "number")
                    if (n <= t.getCount() - 1) t.forEach(function(t, r) {
                        r == n && (t = i)
                    });
                    else throw new Error("Index outside of range. The entities collection contains " + t.getCount() + " entities");
                    else if (Sdk.Util.isGuidOrNull(n)) {
                if (r = !1, t.forEach(function(t) {
                    t.getId() == n && (r = !0, t = i)
                }), !r) throw new Error("There is no Sdk.Entity in the Sdk.EntityCollection with the Id '" + n + "'");
            } else throw new Error("Sdk.EntityCollection setEntity method indexOrId parameter must be a number or an string");
            else throw new Error("Items in the collection must be Sdk.Entity instances.");
        }, this.getEntityName = function() {
            return r
        }, this.setEntityName = function(n) {
            h(n)
        }, this.getMinActiveRowVersion = function() {
            return u
        }, this.setMinActiveRowVersion = function(n) {
            c(n)
        }, this.getMoreRecords = function() {
            return f
        }, this.setMoreRecords = function(n) {
            l(n)
        }, this.getPagingCookie = function() {
            return e
        }, this.setPagingCookie = function(n) {
            a(n)
        }, this.getTotalRecordCount = function() {
            return i
        }, this.setTotalRecordCount = function(n) {
            typeof n == "number" && (i = n)
        }, this.getTotalRecordCountLimitExceeded = function() {
            return o
        }, this.setTotalRecordCountLimitExceeded = function(n) {
            v(n)
        }
    }, this.EntityCollection.__class = !0, this.EntityReference = function(n, t, i) {
        function e(n) {
            if (typeof n == "string") r = n;
            else throw new Error("Sdk.EntityReference constructor logicalName parameter is required and must be a String.");
        }

        function o(n) {
            if (n != null && Sdk.Util.isGuidOrNull(n)) u = n;
            else throw new Error("Sdk.EntityReference constructor id value property is required and must be a String representation of a GUID value.");
        }

        function s(n) {
            if (typeof n == "string") f = n;
            else throw new Error("Sdk.EntityReference constructor name parameter must be a String.");
        }
        if (!(this instanceof Sdk.EntityReference)) return new Sdk.EntityReference(n, t, i);
        var r, u, f = null;
        e(n), o(t), i != null && typeof i != "undefined" && s(i), this.getType = function() {
            return r
        }, this.getId = function() {
            return u
        }, this.getName = function() {
            return f
        }, this.setType = function(n) {
            e(n)
        }, this.setId = function(n) {
            o(n)
        }, this.setName = function(n) {
            s(n)
        }
    }, this.EntityReference.__class = !0, this.EntityReferenceCollection = function(n) {
        function i(n) {
            if (n instanceof Sdk.Collection && n.getType() == Sdk.EntityReference) t = n;
            else throw new Error("Sdk.EntityReferenceCollection EntityReferences property must be an Sdk.Collection of SdkEntityReference");
        }
        if (!(this instanceof Sdk.EntityReferenceCollection)) return new Sdk.EntityReferenceCollection(n);
        var t = Sdk.Collection(Sdk.EntityReference);
        (typeof n != "undefined" || n != null) && i(n), this.getEntityReferences = function() {
            return t
        }, this.setEntityReferences = function(n) {
            i(n)
        }
    }, this.EntityReferenceCollection.__class = !0, this.EntityState = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.FormattedValue = function(n, t) {
        function u(n) {
            if (typeof n == "string") i = n;
            else throw new Error("Sdk.FormattedValue Name property must be a string.");
        }

        function f(n) {
            if (typeof n == "string") r = n;
            else throw new Error("Sdk.FormattedValue Value property must be a string.");
        }
        if (!(this instanceof Sdk.FormattedValue)) return new Sdk.FormattedValue(n, t);
        var i = null,
            r = null;
        u(n), f(t), this.getName = function() {
            return i
        }, this.setName = function(n) {
            u(n)
        }, this.getValue = function() {
            return r
        }, this.setValue = function(n) {
            f(n)
        }
    }, this.FormattedValue.__class = !0, this.FormattedValueCollection = function() {
        function t(t) {
            var i = !1;
            return n.forEach(function(n) {
                if (n.getName() == t) {
                    i = !0;
                    return
                }
            }), i
        }

        function r(t) {
            var i;
            return n.forEach(function(n) {
                n.getName() == t && (i = n)
            }), i
        }

        function u(i, r) {
            if (t(i)) n.forEach(function(n) {
                n.getName() == i && n.setValue(r)
            });
            else throw new Error(Sdk.Util.format("Sdk.FormattedValueCollection error: An item with the name '{0}' does not exist in the collection.", [i]));
        }

        function i(i) {
            if (i instanceof Sdk.FormattedValue)
                if (t(i.getName())) throw new Error("An item with the name '" + i.getName() + "' already exists in the collection.");
                else n.add(i);
                else throw new Error("Sdk.FormattedValueCollection add method requires an Sdk.FormattedValue parameter");
        }

        function f(n) {
            if (typeof n != "undefined" && typeof n.push != "undefined")
                for (var t = 0; t < n.length; t++)
                    if (n[t] instanceof Sdk.FormattedValue) i(n[t]);
                    else throw new Error("The Sdk.FormattedValueCollection addRange method requires all the items in the items parameter are Sdk.FormattedValue.");
                    else throw new Error("The Sdk.FormattedValueCollection addRange method requires an Array of Sdk.FormattedValue.");
        }
        if (!(this instanceof Sdk.FormattedValueCollection)) return new Sdk.FormattedValueCollection;
        var n = new Sdk.Collection(Sdk.FormattedValue);
        this.add = function(n) {
            i(n)
        }, this.addRange = function(n) {
            f(n)
        }, this.containsName = function(n) {
            return t(n)
        }, this.getCollection = function() {
            return n
        }, this.getItem = function(n) {
            if (t(n)) return r(n);
            throw new Error(Sdk.Util.format("An item with the name '{0}' does not exist in the collection.", [n]));
        }, this.setItem = function(n, t) {
            u(n, t)
        }
    }, this.FormattedValueCollection.__class = !0, this.OrganizationRequest = function() {
        function i(t) {
            if (t.prototype.type == "Sdk.OrganizationResponse" && typeof t == "function") n = t;
            else throw new Error("Sdk.OrganizationRequest ResponseType property must be a Sdk.OrganizationResponse).");
        }

        function r(n) {
            if (typeof n == "string") t = n;
            else throw new Error("Sdk.OrganizationRequest RequestXml property must be a String.");
        }
        var n, t;
        this.setRequestXml = function(n) {
            r(n)
        }, this.getRequestXml = function() {
            return t
        }, this.setResponseType = function(n) {
            i(n)
        }, this.getResponseType = function() {
            return n
        }
    }, this.OrganizationRequest.__class = !0, this.OrganizationResponse = function() {
        this.type = "Sdk.OrganizationResponse"
    }, this.OrganizationResponse.__class = !0, this.PrincipalAccess = function(n, t) {
        function u(n) {
            if (typeof n == "number" && n >= 0 && n <= 255) i = n;
            else throw new Error("Sdk.PrincipalAccess AccessMask property must be a number represented by the Sdk.AccessRights enumeration.");
        }

        function f(n) {
            if (n instanceof Sdk.EntityReference && (n.getType() == "systemuser" || n.getType() == "team")) r = n;
            else throw new Error("Sdk.PrincipalAccess Principal property must be a Sdk.EntityReference for a systemuser or team entity instance.");
        }
        if (!(this instanceof Sdk.PrincipalAccess)) return new Sdk.PrincipalAccess;
        var i = Sdk.AccessRights.None,
            r = null;
        typeof n != "undefined" && u(n), typeof t != "undefined" && f(t), this.getAccessMaskAsNumber = function() {
            return i
        }, this.getAccessMaskAsString = function() {
            return Sdk.Util.convertAccessRightsToString(i)
        }, this.getPrincipal = function() {
            return r
        }, this.setAccessMask = function(n) {
            u(n)
        }, this.setPrincipal = function(n) {
            f(n)
        }
    }, this.PrincipalAccess.__class = !0, this.PrivilegeDepth = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.PropagationOwnershipOptions = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.RelatedEntitiesCollection = function() {
        if (!(this instanceof Sdk.RelatedEntitiesCollection)) return new Sdk.RelatedEntitiesCollection;
        var t = new Sdk.Collection(Sdk.RelationshipEntityCollection),
            n = !1;
        this.getRelationshipEntities = function() {
            return t
        }, this.getIsChanged = function() {
            return n
        }, this.setIsChanged = function(t) {
            if (typeof t == "boolean") n = t;
            else throw new Error("Sdk.RelatedEntitiesCollection setIsChanged method isChanged parameter must be a boolean value.");
        }
    }, this.RelatedEntitiesCollection.__class = !0, this.RelationshipEntityCollection = function(n) {
        function r(n) {
            if (n instanceof Sdk.EntityCollection) i = n;
            else throw new Error("Sdk.RelationshipEntityCollection Entities property must be an Sdk.EntityCollection.");
        }
        if (!(this instanceof Sdk.RelationshipEntityCollection)) return new Sdk.RelationshipEntityCollection(n);
        var t, i = new Sdk.EntityCollection;
        if (this.getEntityCollection = function() {
            return i
        }, this.getRelationship = function() {
            return t
        }, this.setEntityCollection = function(n) {
            r(n)
        }, n != null && typeof n == "string") t = n;
        else throw new Error("Sdk.RelationshipEntityCollection constructor relationshipSchemaName parameter is required and must be a string.");
    }, this.RelationshipEntityCollection.__class = !0, this.RelationshipQuery = function(n, t) {
        function u(n) {
            if (typeof n == "string") i = n;
            else throw new Error("Sdk.RelationshipQuery RelationshipName must be a string.");
        }

        function f(n) {
            if (n instanceof Sdk.Query.QueryBase) r = n;
            else throw new Error("Sdk.RelationshipQuery Query must be a class that inherits from Sdk.Query.QueryBase.");
        }
        if (!(this instanceof Sdk.RelationshipQuery)) return new Sdk.RelationshipQuery(n, t);
        var i = null,
            r = null;
        u(n), f(t), this.getQuery = function() {
            return r
        }, this.getRelationshipName = function() {
            return i
        }, this.setQuery = function(n) {
            f(n)
        }, this.setRelationshipName = function(n) {
            u(n)
        }
    }, this.RelationshipQuery.__class = !0, this.RelationshipQueryCollection = function(n) {
        function i(n) {
            if (n instanceof Sdk.Collection) n.forEach(function(n) {
                if (!(n instanceof Sdk.RelationshipQuery)) throw new Error(t);
            });
            else throw new Error(t);
        }
        if (!(this instanceof Sdk.RelationshipQueryCollection)) return new Sdk.RelationshipQueryCollection(n);
        var t = "Sdk.RelationshipQueryCollection RelationshipQueries must be an Sdk.Collection of Sdk.RelationshipQuery objects.",
            r = new Sdk.Collection(Sdk.RelationshipQuery);
        typeof n != "undefined" && i(n), this.getRelationshipQueries = function() {
            return r
        }, this.setRelationshipQueries = function(n) {
            i(n)
        }
    }, this.RelationshipQueryCollection.__class = !0, this.RolePrivilege = function(n, t, i) {
        function e(n) {
            if (n == "Basic" || n == "Deep" || n == "Global" || n == "Local") r = n;
            else throw new Error("Sdk.RolePrivilege Depth must be a Sdk.PrivilegeDepth value.");
        }

        function o(n) {
            if (Sdk.Util.isGuid(n)) u = n;
            else throw new Error("Sdk.RolePrivilege PrivilegeId must be a string representation of a guid value.");
        }

        function s(n) {
            if (Sdk.Util.isGuid(n)) f = n;
            else throw new Error("Sdk.RolePrivilege BusinessId must be a string representation of a guid value.");
        }
        if (!(this instanceof Sdk.RolePrivilege)) return new Sdk.RolePrivilege(n, t, i);
        var r = null,
            u = null,
            f = null;
        e(n), o(t), s(i), this.getBusinessId = function() {
            return f
        }, this.setBusinessId = function(n) {
            s(n)
        }, this.getDepth = function() {
            return r
        }, this.setDepth = function(n) {
            e(n)
        }, this.getPrivilegeId = function() {
            return u
        }, this.setPrivilegeId = function(n) {
            o(n)
        }
    }, this.RolePrivilege.__class = !0, this.RollupType = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.TargetFieldType = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.TimeInfo = function() {
        function a(t) {
            if (typeof t == "number") n = t;
            else throw new Error("Sdk.TimeInfo ActivityStatusCode  must be a Number.");
        }

        function v(n) {
            if (Sdk.Util.isGuid(n)) h = n;
            else throw new Error("Sdk.TimeInfo CalendarId must be a String representation of a Guid value.");
        }

        function y(n) {
            if (typeof n == "string") s = n;
            else throw new Error("Sdk.TimeInfo DisplayText must be a String.");
        }

        function p(n) {
            if (typeof n == "number") o = n;
            else throw new Error("Sdk.TimeInfo Effort must be a Number.");
        }

        function w(n) {
            if (n == null || n instanceof Date) e = n;
            else throw new Error("Sdk.TimeInfo End must be null or a Date .");
        }

        function b(n) {
            if (typeof n == "boolean") c = n;
            else throw new Error("Sdk.TimeInfo IsActivity must be a Boolean.");
        }

        function k(n) {
            if (Sdk.Util.isGuid(n)) u = n;
            else throw new Error("Sdk.TimeInfo SourceId must be a String representation of a Guid value.");
        }

        function d(n) {
            if (typeof n == "number") r = n;
            else throw new Error("Sdk.TimeInfo SourceTypeCode must be a Number.");
        }

        function g(n) {
            if (n == null || n instanceof Date) i = n;
            else throw new Error("Sdk.TimeInfo Start must be null or a Date.");
        }

        function l(n) {
            if (Sdk.Util.isValidEnumValue(Sdk.SubCode, n)) t = n;
            else throw new Error("Sdk.TimeInfo SubCode must be an Sdk.SubCode value.");
        }

        function nt(n) {
            if (Sdk.Util.isValidEnumValue(Sdk.TimeCode, n)) f = n;
            else throw new Error("Sdk.TimeInfo TimeCode must be an Sdk.TimeCode value.");
        }
        if (!(this instanceof Sdk.TimeInfo)) return new Sdk.TimeInfo;
        var n = null,
            h = null,
            s = null,
            o = null,
            e = null,
            c = null,
            u = null,
            r = null,
            i = null,
            t = null,
            f = null;
        this.getActivityStatusCode = function() {
            return n
        }, this.getCalendarId = function() {
            return h
        }, this.getDisplayText = function() {
            return s
        }, this.getEffort = function() {
            return o
        }, this.getEnd = function() {
            return e
        }, this.getIsActivity = function() {
            return c
        }, this.getSourceId = function() {
            return u
        }, this.getSourceTypeCode = function() {
            return r
        }, this.getStart = function() {
            return i
        }, this.getSubCode = function() {
            return t
        }, this.getTimeCode = function() {
            return f
        }, this.setActivityStatusCode = function(n) {
            a(n)
        }, this.setCalendarId = function(n) {
            v(n)
        }, this.setDisplayText = function(n) {
            y(n)
        }, this.setEffort = function(n) {
            p(n)
        }, this.setEnd = function(n) {
            w(n)
        }, this.setIsActivity = function(n) {
            b(n)
        }, this.setSourceId = function(n) {
            k(n)
        }, this.setSourceTypeCode = function(n) {
            d(n)
        }, this.setStart = function(n) {
            g(n)
        }, this.setSubCode = function(n) {
            l(n)
        }, this.setTimeCode = function(n) {
            nt(n)
        }
    }, this.TimeInfo.__class = !0, this.SubCode = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.TimeCode = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.TraceInfo = function() {
        function t(t) {
            if (Sdk.Util.isCollectionOf(t, Sdk.ErrorInfo)) n = t;
            else throw new Error("Sdk.TraceInfo ErrorInfoList must be a Sdk.Collection of Sdk.ErrorInfo.");
        }
        if (!(this instanceof Sdk.TraceInfo)) return new Sdk.TraceInfo;
        var n = new Sdk.Collection(Sdk.ErrorInfo);
        this.getErrorInfoList = function() {
            return n
        }, this.setErrorInfoList = function(n) {
            t(n)
        }
    }, this.TraceInfo.__class = !0, this.ErrorInfo = function() {
        function i(t) {
            if (typeof t == "string") n = t;
            else throw new Error("Sdk.ErrorInfo ErrorCode must be a String.");
        }

        function r(n) {
            if (Sdk.Util.isCollectionOf(n, Sdk.ResourceInfo)) t = n;
            else throw new Error("Sdk.ErrorInfo ResourceList must be an Sdk.Collection of Sdk.ResourceInfo.");
        }
        if (!(this instanceof Sdk.ErrorInfo)) return new Sdk.ErrorInfo;
        var n = null,
            t = new Sdk.Collection(Sdk.ResourceInfo);
        this.getErrorCode = function() {
            return n
        }, this.getResourceList = function() {
            return t
        }, this.setErrorCode = function(n) {
            i(n)
        }, this.setResourceList = function(n) {
            r(n)
        }
    }, this.ErrorInfo.__class = !0, this.ResourceInfo = function() {
        function r(t) {
            if (typeof t == "string") n = t;
            else throw new Error("Sdk.ResourceInfo DisplayName property must be a String.");
        }

        function u(n) {
            if (typeof n == "string") t = n;
            else throw new Error("Sdk.ResourceInfo EntityName property must be a String.");
        }

        function f(n) {
            if (Sdk.Util.isGuid(n)) i = n;
            else throw new Error("Sdk.ResourceInfo Id property must be a String.");
        }
        if (!(this instanceof Sdk.ResourceInfo)) return new Sdk.ResourceInfo;
        var n = null,
            t = null,
            i = null;
        this.getDisplayName = function() {
            return n
        }, this.getEntityName = function() {
            return t
        }, this.getId = function() {
            return i
        }, this.setDisplayName = function(n) {
            r(n)
        }, this.setEntityName = function(n) {
            u(n)
        }, this.setId = function(n) {
            f(n)
        }
    }, this.ResourceInfo.__class = !0, this.ValidationResult = function() {
        function r(t) {
            if (Sdk.Util.isGuid(t)) n = t;
            else throw new Error("Sdk.ValidationResult ActivityId  property must be a String representation of a Guid Value.");
        }

        function u(n) {
            if (n instanceof Sdk.TraceInfo) t = n;
            else throw new Error("Sdk.ValidationResult TraceInfo  property must be a Sdk.TraceInfo.");
        }

        function f(n) {
            if (typeof n == "boolean") i = n;
            else throw new Error("Sdk.ValidationResult ValidationSuccess  property must be a Boolean.");
        }
        if (!(this instanceof Sdk.ValidationResult)) return new Sdk.ValidationResult;
        var n = null,
            t = null,
            i = null;
        this.getActivityId = function() {
            return n
        }, this.getTraceInfo = function() {
            return t
        }, this.getValidationSuccess = function() {
            return i
        }, this.setActivityId = function(n) {
            r(n)
        }, this.setTraceInfo = function(n) {
            u(n)
        }, this.setValidationSuccess = function(n) {
            f(n)
        }
    }, this.ValidationResult.__class = !0
}.call(Sdk),
function() {
    function t(n) {
        for (var r = new Sdk.RelatedEntitiesCollection, u, t = 0; t < n.childNodes.length; t++) u = i(n.childNodes[t]), r.addRelationshipEntityCollection(u);
        return r
    }

    function i(n) {
        var i = Sdk.Xml.selectSingleNodeText(n, "b:key/a:SchemaName"),
            t = new Sdk.RelationshipEntityCollection(i),
            r = Sdk.Util.createEntityCollectionFromNode(Sdk.Xml.selectSingleNode(n, "b:value"));
        return t.setEntityCollection(r), t
    }

    function r(n) {
        var r = Sdk.Xml.selectSingleNodeText(n, "b:key"),
            t = null,
            i = Sdk.Xml.selectSingleNode(n, "b:value"),
            f = Sdk.Xml.getNodeText(i.attributes.getNamedItem("i:type"));
        return t = u(r, n, i, f), t.setIsChanged(!1), t
    }

    function u(n, t, i, r) {
		if (r==null) return new Sdk.String(n, null); //Updated 12-11-2016 by Carlton Colter - null values with null types are null strings by default.
        var d = r.split(":")[1],
            y, v, a, l, p, c, s, o, e, f, h, w;
        switch (d) {
            case "string":
                return new Sdk.String(n, Sdk.Xml.selectSingleNodeText(t, "b:value"));
            case "guid":
                return new Sdk.Guid(n, Sdk.Xml.selectSingleNodeText(t, "b:value"));
            case "Money":
                return new Sdk.Money(n, parseFloat(Sdk.Xml.getNodeText(i)));
            case "long":
                return new Sdk.Long(n, parseInt(Sdk.Xml.getNodeText(i), 10));
            case "decimal":
                return new Sdk.Decimal(n, parseFloat(Sdk.Xml.getNodeText(i)));
            case "double":
                return new Sdk.Double(n, parseFloat(Sdk.Xml.getNodeText(i)));
            case "int":
                return new Sdk.Int(n, parseInt(Sdk.Xml.getNodeText(i), 10));
            case "OptionSetValue":
                return new Sdk.OptionSet(n, parseInt(Sdk.Xml.selectSingleNodeText(i, "a:Value"), 10));
            case "boolean":
                return new Sdk.Boolean(n, Sdk.Xml.selectSingleNodeText(t, "b:value") == "true" ? !0 : !1);
            case "dateTime":
                return new Sdk.DateTime(n, new Date(Sdk.Xml.selectSingleNodeText(t, "b:value")));
            case "EntityReference":
                return y = Sdk.Xml.selectSingleNodeText(i, "a:LogicalName"), v = Sdk.Xml.selectSingleNodeText(i, "a:Id"), a = Sdk.Xml.selectSingleNodeText(i, "a:Name"), new Sdk.Lookup(n, new Sdk.EntityReference(y, v, a));
            case "BooleanManagedProperty":
                return l = Sdk.Xml.selectSingleNodeText(i, "a:CanBeChanged") == "true" ? !0 : !1, p = Sdk.Xml.selectSingleNodeText(i, "a:ManagedPropertyLogicalName"), c = Sdk.Xml.selectSingleNodeText(i, "a:Value") == "true" ? !0 : !1, new Sdk.BooleanManagedProperty(n, new Sdk.BooleanManagedPropertyValue(l, c, p));
            case "EntityCollection":
                return new Sdk.PartyList(n, Sdk.Util.createEntityCollectionFromNode(i));
            case "AliasedValue":
                var g, u = Sdk.Xml.selectSingleNode(i, "a:Value "),
                    b = Sdk.Xml.getNodeText(u.attributes.getNamedItem("i:type")),
                    k = b.split(":")[1];
                switch (k) {
                    case "string":
                        return new Sdk.String(n, Sdk.Xml.getNodeText(u));
                    case "guid":
                        return new Sdk.Guid(n, Sdk.Xml.getNodeText(u));
                    case "Money":
                        return new Sdk.Money(n, parseFloat(Sdk.Xml.getNodeText(u)));
                    case "long":
                        return new Sdk.Long(n, parseInt(Sdk.Xml.getNodeText(u), 10));
                    case "decimal":
                        return new Sdk.Decimal(n, parseFloat(Sdk.Xml.getNodeText(u)));
                    case "double":
                        return new Sdk.Double(n, parseInt(Sdk.Xml.getNodeText(u), 10));
                    case "int":
                        return new Sdk.Int(n, parseInt(Sdk.Xml.getNodeText(u), 10));
                    case "OptionSetValue":
                        return new Sdk.OptionSet(n, parseInt(Sdk.Xml.getNodeText(u), 10));
                    case "boolean":
                        return new Sdk.Boolean(n, Sdk.Xml.getNodeText(u) == "true" ? !0 : !1);
                    case "dateTime":
                        return new Sdk.DateTime(n, new Date(Sdk.Xml.getNodeText(u)));
                    case "EntityReference":
                        return s = Sdk.Xml.selectSingleNodeText(u, "a:LogicalName"), o = Sdk.Xml.selectSingleNodeText(u, "a:Id"), e = Sdk.Xml.selectSingleNodeText(u, "a:Name"), new Sdk.Lookup(n, new Sdk.EntityReference(s, o, e));
                    case "BooleanManagedProperty":
                        return f = Sdk.Xml.selectSingleNodeText(u, "a:CanBeChanged") == "true" ? !0 : !1, h = Sdk.Xml.selectSingleNodeText(u, "a:ManagedPropertyLogicalName"), w = Sdk.Xml.selectSingleNodeText(u, "a:Value") == "true" ? !0 : !1, new Sdk.BooleanManagedProperty(n, new Sdk.BooleanManagedPropertyValue(f, w, h));
                    case "EntityCollection":
                        return new Sdk.PartyList(n, Sdk.Util.createEntityCollectionFromNode(u));
                    default:
                        throw new Error("Sdk unimplemented AliasedValue type found in getTypedValue function");
                }
                break;
            default:
                throw new Error("Sdk unimplemented type found in getTypedValue function");
        }
    }

    function f(n) {
        var t = Sdk.Xml.selectSingleNodeText(n, "b:key"),
            i = Sdk.Xml.selectSingleNodeText(n, "b:value");
        return new Sdk.FormattedValue(t, i)
    }

    function e(n) {
        var t = Sdk.Util.convertAccessRightsFromString(Sdk.Xml.selectSingleNodeText(n, "g:AccessMask")).Value,
            i = Sdk.Util.createEntityReferenceFromNode(Sdk.Xml.selectSingleNode(n, "g:Principal"));
        return new Sdk.PrincipalAccess(t, i)
    }

    function o(n) {
        var t = Sdk.Xml.selectSingleNodeText(n, "g:BusinessUnitId"),
            i = Sdk.Xml.selectSingleNodeText(n, "g:Depth"),
            r = Sdk.Xml.selectSingleNodeText(n, "g:PrivilegeId");
        return new Sdk.RolePrivilege(i, r, t)
    }

    function s(n) {
        var i = parseInt(Sdk.Xml.getNodeText(n, "g:ActivityStatusCode"), 10),
            r = Sdk.Xml.selectSingleNodeText(n, "g:CalendarId"),
            u = Sdk.Xml.selectSingleNodeText(n, "g:DisplayText"),
            f = parseFloat(Sdk.Xml.selectSingleNodeText(n, "g:Effort")),
            e = null,
            o;
        try {
            o = new Date(Sdk.Xml.selectSingleNodeText(n, "g:End")), e = o
        } catch (y) {}
        var s = Sdk.Xml.selectSingleNodeText(n, "g:ActivityStatusCode") == "true" ? !0 : !1,
            h = Sdk.Xml.selectSingleNodeText(n, "g:SourceId"),
            c = parseInt(Sdk.Xml.selectSingleNodeText(n, "g:SourceTypeCode"), 10),
            l = null;
        try {
            testStartDate = new Date(Sdk.Xml.selectSingleNodeText(n, "g:Start")), l = testStartDate
        } catch (y) {}
        var a = Sdk.Xml.selectSingleNodeText(n, "g:SubCode"),
            v = Sdk.Xml.selectSingleNodeText(n, "g:TimeCode"),
            t = new Sdk.TimeInfo;
        return i != null && t.setActivityStatusCode(i), r != null && t.setCalendarId(r), u != null && t.setDisplayText(u), f != null && t.setEffort(f), t.setEnd(e), s != null && t.setIsActivity(s), h != null && t.setSourceId(h), c != null && t.setSourceTypeCode(c), t.setStart(l), a != null && t.setSubCode(a), v != null && t.setTimeCode(v), t
    }

    function h(n) {
        var i = new Sdk.ErrorInfo,
            r, u, t;
        for (i.setErrorCode(Sdk.Xml.selectSingleNodeText(n, "g:ErrorCode")), r = new Sdk.Collection(Sdk.ResourceInfo), u = Sdk.Xml.selectSingleNode(n, "g:ResourceList"), t = 0; t < u.childNodes.length; t++) r.add(c(u.childNodes[t]));
        return i.setResourceList(r), i
    }

    function c(n) {
        var t = new Sdk.ResourceInfo;
        return t.setDisplayName(Sdk.Xml.selectSingleNodeText(n, "g:DisplayName")), t.setEntityName(Sdk.Xml.selectSingleNodeText(n, "g:EntityName")), t.setId(Sdk.Xml.selectSingleNodeText(n, "g:Id")), t
    }
    var n = null;
    this.isGuid = function(n) {
        if (typeof n == "string") return /^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/.test(n) ? !0 : !1;
        throw new Error("Sdk.Util.isGuid value parameter must be a string.");
    }, this.isGuidOrNull = function(n) {
        if (n == null || typeof n == "string") return n == null ? !0 : Sdk.Util.isGuid(n);
        throw new Error("Sdk.Util.isGuidOrNull value parameter must be a string or null.");
    }, this.isValidEnumValue = function(n, t) {
        for (var i in n)
            if (t == i) return !0;
        return !1
    }, this.getEmptyGuid = function() {
        return "00000000-0000-0000-0000-000000000000"
    }, this.format = function(n, t) {
        var i, r;
        if (typeof n != "string") throw new Error("Sdk.Util.format string parameter is required and must be a string.");
        if (i = "Sdk.Util.format args parameter is required and must be an array of strings.", typeof t == "undefined" || t == null) throw new Error(i);
        if (typeof t.push != "function") throw new Error(i);
        else
            for (r = 0; r < t.length; r++)
                if (typeof t[r] != "string") throw new Error(i); return n.replace(/\{\{|\}\}|\{(\w+)\}/g, function(n, i) {
            return n == "{{" ? "{" : n == "}}" ? "}" : t[i]
        })
    }, this.getError = function(n) {
        var i, t, e, r, u, f, o;
        if (n.status == 12029) return new Error("The attempt to connect to the server failed.");
        if (n.status == 12007) return new Error("The server name could not be resolved.");
        if (i = "Unknown Error (Unable to parse the fault)", t = n.responseXML, t != null && typeof t.firstChild != "undefined" && t.firstChild != null && typeof t.firstChild.firstChild != "undefined" && t.firstChild.firstChild != null) try {
            for (e = t.firstChild.firstChild, r = 0; r < e.childNodes.length; r++)
                if (u = e.childNodes[r], "s:Fault" == u.nodeName) {
                    for (f = 0; f < u.childNodes.length; f++)
                        if (o = u.childNodes[f], "faultstring" == o.nodeName) {
                            i = Sdk.Xml.getNodeText(o);
                            break
                        }
                    break
                }
        } catch (s) {} else i = "status: " + n.status + ": " + n.statusText;
        return new Error(i)
    }, this.getClientUrl = function() {
        if (n != null) return n;
        try {
            return GetGlobalContext().getClientUrl()
        } catch (t) {
            try {
                return Xrm.Page.context.getClientUrl()
            } catch (t) {
                throw new Error("Sdk.Util.getClientUrl Unable to get clientUrl. Context not available.");
            }
        }
    }, this.setClientUrl = function(t) {
        if (typeof t == "string") n = t;
        else throw new Error("Sdk.Util.setClientUrl 'url' parameter must be a string.");
    }, this.getXMLHttpRequest = function(n, t) {
        var i = new XMLHttpRequest;
        i.open("POST", Sdk.Util.getClientUrl() + "/XRMServices/2011/Organization.svc/web", t);
        try {
            i.responseType = "msxml-document"
        } catch (r) {}
        return i.setRequestHeader("Accept", "application/xml, text/xml, */*"), i.setRequestHeader("Content-Type", "text/xml; charset=utf-8"), i.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/" + n), i
    }, this.convertAccessRightsToString = function(n) {
        var i = "None",
            t;
        return n != null && (t = [], (1 & n) == 1 && t.push("ReadAccess"), (2 & n) == 2 && t.push("WriteAccess"), (4 & n) == 4 && t.push("ShareAccess"), (8 & n) == 8 && t.push("AssignAccess"), (16 & n) == 16 && t.push("AppendAccess"), (32 & n) == 32 && t.push("AppendToAccess"), (64 & n) == 64 && t.push("CreateAccess"), (128 & n) == 128 && t.push("DeleteAccess"), i = t.join(" ")), i
    }, this.convertAccessRightsFromString = function(n) {
        var t = {};
        return t.Text = n, t.ReadAccess = n.indexOf("ReadAccess") > -1, t.WriteAccess = n.indexOf("WriteAccess") > -1, t.ShareAccess = n.indexOf("ShareAccess") > -1, t.AssignAccess = n.indexOf("AssignAccess") > -1, t.AppendAccess = n.indexOf("AppendAccess") > -1, t.AppendToAccess = n.indexOf("AppendToAccess") > -1, t.CreateAccess = n.indexOf("CreateAccess") > -1, t.DeleteAccess = n.indexOf("DeleteAccess") > -1, t.Value = 0, t.ReadAccess && (t.Value = t.Value + 1), t.WriteAccess && (t.Value = t.Value + 2), t.ShareAccess && (t.Value = t.Value + 4), t.AssignAccess && (t.Value = t.Value + 8), t.AppendAccess && (t.Value = t.Value + 16), t.AppendToAccess && (t.Value = t.Value + 32), t.CreateAccess && (t.Value = t.Value + 64), t.DeleteAccess && (t.Value = t.Value + 128), t
    }, this.createEntityFromNode = function(n) {
        var i, e, o, u, s;
        if (Sdk.Xml.isNodeNull(n)) return null;
        for (i = new Sdk.Entity, e = Sdk.Xml.selectSingleNode(n, "a:Attributes"), u = 0; u < e.childNodes.length; u++) i.getAttributes().add(r(e.childNodes[u]), !1);
        for (i.setEntityState(Sdk.Xml.selectSingleNodeText(n, "a:EntityState")), o = Sdk.Xml.selectSingleNode(n, "a:FormattedValues"), u = 0; u < o.childNodes.length; u++) i.getFormattedValues().add(f(o.childNodes[u]));
        return i.setId(Sdk.Xml.selectSingleNodeText(n, "a:Id")), i.setType(Sdk.Xml.selectSingleNodeText(n, "a:LogicalName")), i.setIsChanged(!1), s = t(Sdk.Xml.selectSingleNode(n, "a:RelatedEntities")), i.setRelatedEntities(s), i
    }, this.createEntityCollectionFromNode = function(n) {
        var t = new Sdk.EntityCollection,
            r, i;
        for (t.setEntityName(Sdk.Xml.selectSingleNodeText(n, "a:EntityName")), t.setMinActiveRowVersion(Sdk.Xml.selectSingleNodeText(n, "a:MinActiveRowVersion")), t.setMoreRecords(Sdk.Xml.selectSingleNodeText(n, "a:MoreRecords") == "true" ? !0 : !1), t.setPagingCookie(Sdk.Xml.selectSingleNodeText(n, "a:PagingCookie")), t.setTotalRecordCount(parseInt(Sdk.Xml.selectSingleNodeText(n, "a:TotalRecordCount"))), t.setTotalRecordCountLimitExceeded(Sdk.Xml.selectSingleNodeText(n, "a:TotalRecordCountLimitExceeded") == "true" ? !0 : !1), r = Sdk.Xml.selectSingleNode(n, "a:Entities"), i = 0; i < r.childNodes.length; i++) t.getEntities().add(Sdk.Util.createEntityFromNode(r.childNodes[i]));
        return t
    }, this.createEntityReferenceFromNode = function(n) {
        var t, i, r;
        return t = Sdk.Xml.selectSingleNodeText(n, "a:LogicalName"), i = Sdk.Xml.selectSingleNodeText(n, "a:Id"), r = Sdk.Xml.selectSingleNodeText(n, "a:Name"), new Sdk.EntityReference(t, i, r)
    }, this.createEntityReferenceCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Sdk.EntityReference), t = 0; t < n.childNodes.length; t++) i.add(Sdk.Util.createEntityReferenceFromNode(n.childNodes[t]));
        return new Sdk.EntityReferenceCollection(i)
    }, this.createPrincipleAccessCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Sdk.PrincipalAccess), t = 0; t < n.childNodes.length; t++) i.add(e(n.childNodes[t]));
        return i
    }, this.createRolePrivilegeCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Sdk.RolePrivilege), t = 0; t < n.childNodes.length; t++) i.add(o(n.childNodes[t]));
        return i
    }, this.createIntegerCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Number), t = 0; t < n.childNodes.length; t++) i.add(parseInt(Sdk.Xml.getNodeText(n.childNodes[t])), 10);
        return i
    }, this.createBooleanCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Boolean), t = 0; t < n.childNodes.length; t++) i.add(Sdk.Xml.getNodeText(n.childNodes[t]) == "true" ? !0 : !1);
        return i
    }, this.createCollectionOfTimeInfoCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Sdk.Collection), t = 0; t < n.childNodes.length; t++) i.add(Sdk.Util.createTimeInfoCollectionFromNode(n.childNodes[t]));
        return i
    }, this.createTimeInfoCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Sdk.TimeInfo), t = 0; t < n.childNodes.length; t++) i.add(s(n.childNodes[t]));
        return i
    }, this.createValidationResultCollectionFromNode = function(n) {
        for (var i = new Sdk.Collection(Sdk.ValidationResult), t = 0; t < n.childNodes.length; t++) i.add(Sdk.Util.createValidationResultFromNode(n.childNodes[t]));
        return i
    }, this.createValidationResultFromNode = function(n) {
        var t = new Sdk.ValidationResult;
        return t.setActivityId(Sdk.Xml.selectSingleNodeText(n, "g:ActivityId")), t.setTraceInfo(Sdk.Util.createTraceInfoFromNode(n)), t.setValidationSuccess(Sdk.Xml.selectSingleNodeText(n, "g:ValidationSuccess") == "true" ? !0 : !1), t
    }, this.createTraceInfoFromNode = function(n) {
        for (var i = new Sdk.TraceInfo, r = new Sdk.Collection(Sdk.ErrorInfo), u = Sdk.Xml.selectSingleNode(n, "g:TraceInfo/g:ErrorInfoList"), t = 0; t < u.childNodes.length; t++) r.add(h(u.childNodes[t]));
        return i.setErrorInfoList(r), i
    }, this.createAuditDetailCollectionFromNode = function(n) {
        var t = new Sdk.AuditDetailCollection,
            r, i;
        for (t.setMoreRecords(Sdk.Xml.selectSingleNodeText(n, "g:MoreRecords") == "true" ? !0 : !1), t.setTotalRecordCount(parseInt(Sdk.Xml.selectSingleNodeText(n, "g:TotalRecordCount"), 10)), t.setPagingCookie(Sdk.Xml.selectSingleNodeText(n, "g:PagingCookie")), r = Sdk.Xml.selectSingleNode(n, "g:AuditDetails"), i = 0; i < r.childNodes.length; i++) t.add(Sdk.Util.createAuditDetailFromNode(r.childNodes[i]));
        return t
    }, this.createAuditDetailFromNode = function(n) {
        var l, b, e, g, a, s, o, f, h, u, c, t, r, i, k, d, v;
        if (!Sdk.Xml.isNodeNull(n)) {
            if (n.attributes.getNamedItem("i:type") == null) return l = new Sdk.AuditDetail, i = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "g:AuditRecord")), l.setAuditRecord(i), l;
            b = n.attributes.getNamedItem("i:type").nodeValue.split(":")[1];
            switch (b) {
                case "AttributeAuditDetail":
                    return e = new Sdk.AttributeAuditDetail, i = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "g:AuditRecord")), e.setAuditRecord(i), g = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "g:NewValue")), e.setNewValue(g), a = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "g:OldValue")), e.setOldValue(a), e;
                case "RelationshipAuditDetail":
                    try {
                        for (s = new Sdk.RelationshipAuditDetail, i = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "g:AuditRecord")), _relationshipName = Sdk.Xml.selectSingleNodeText(n, "g:RelationshipName"), s.setRelationshipName(_relationshipName), _targetRecordsNode = Sdk.Xml.selectSingleNode(n, "g:TargetRecords"), _targetRecordsCollection = new Sdk.Collection(Sdk.EntityReference), t = 0; t < _targetRecordsNode.childNodes.length; t++) _targetRecordsCollection.add(Sdk.Util.createEntityReferenceFromNode(_targetRecordsNode.childNodes[t]));
                        return s.setTargetRecords(_targetRecordsCollection), s
                    } catch (nt) {
                        throw new Error("Sdk.Util.createAuditDetailFromNode not tested for RelationshipAuditDetail class.");
                    }
                    break;
                case "RolePrivilegeAuditDetail":
                    for (o = new Sdk.RolePrivilegeAuditDetail, i = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "g:AuditRecord")), o.setAuditRecord(i), f = Sdk.Xml.selectSingleNode(n, "g:NewRolePrivileges"), h = new Sdk.Collection(Sdk.RolePrivilege), t = 0; t < f.childNodes.length; t++)
                        if (!Sdk.Xml.isNodeNull(f.childNodes[t])) {
                            var y = Sdk.Xml.selectSingleNodeText(f.childNodes[t], "g:Depth"),
                                p = Sdk.Xml.selectSingleNodeText(f.childNodes[t], "g:PrivilegeId"),
                                w = Sdk.Xml.selectSingleNodeText(f.childNodes[t], "g:BusinessUnitId");
                            h.add(new Sdk.RolePrivilege(y, p, w))
                        }
                    for (o.setNewRolePrivileges(h), u = Sdk.Xml.selectSingleNode(n, "g:OldRolePrivileges"), c = new Sdk.Collection(Sdk.RolePrivilege), t = 0; t < u.childNodes.length; t++)
                        if (!Sdk.Xml.isNodeNull(u.childNodes[t])) {
                            var y = Sdk.Xml.selectSingleNodeText(u.childNodes[t], "g:Depth"),
                                p = Sdk.Xml.selectSingleNodeText(u.childNodes[t], "g:PrivilegeId"),
                                w = Sdk.Xml.selectSingleNodeText(u.childNodes[t], "g:BusinessUnitId");
                            c.add(new Sdk.RolePrivilege(y, p, w))
                        }
                    return o.setOldRolePrivileges(c), o;
                case "ShareAuditDetail":
                    return r = new Sdk.ShareAuditDetail, i = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "g:AuditRecord")), r.setAuditRecord(i), k = Sdk.Xml.selectSingleNodeText(n, "g:NewPrivileges"), r.setNewPrivileges(k), d = Sdk.Xml.selectSingleNodeText(n, "g:OldPrivileges"), r.setOldPrivileges(d), v = Sdk.Util.createEntityReferenceFromNode(Sdk.Xml.selectSingleNode(n, "g:Principal")), r.setPrincipal(v), r;
                default:
                    throw new Error("RetrieveAuditDetailsResponse unexpected return value");
            }
        }
    }, this.isCollectionOf = function(n, t) {
        if (n instanceof Sdk.Collection)
            if (typeof t == "function") {
                if (n.getType() == t) return !0
            } else throw new Error("Sdk.Util.isCollectionOf type parameter must be a Function.");
            else throw new Error("Sdk.Util.isCollectionOf collection parameter must be an Sdk.Collection.");
        return !1
    }
}.call(Sdk.Util),
function() {
    this.ConditionExpression = function(n, t, i, r) {
        function s(n) {
            if (typeof n == "string" || n == null) e = n;
            else throw new Error("Sdk.Query.ConditionExpression EntityName property must be a String or null");
        }

        function h(n) {
            if (typeof n == "string") o = n;
            else throw new Error("Sdk.Query.ConditionExpression AttributeName property is required and must be a String");
        }

        function c(n) {
            if (Sdk.Util.isValidEnumValue(Sdk.Query.ConditionOperator, n)) {
                u = n;
                switch (u) {
                    case "Null":
                    case "NotNull":
                    case "EqualUserLanguage":
                    case "Yesterday":
                    case "Today":
                    case "Tomorrow":
                    case "Last7Days":
                    case "Next7Days":
                    case "LastWeek":
                    case "ThisWeek":
                    case "NextWeek":
                    case "LastMonth":
                    case "ThisMonth":
                    case "NextMonth":
                    case "LastYear":
                    case "ThisYear":
                    case "NextYear":
                    case "ThisFiscalYear":
                    case "ThisFiscalPeriod":
                    case "NextFiscalYear":
                    case "NextFiscalPeriod":
                    case "LastFiscalYear":
                    case "LastFiscalPeriod":
                        f = null
                }
            } else throw new Error("Sdk.Query.ConditionExpression operator property must be an Sdk.Query.ConditionOperator");
        }

        function l(n) {
            if (n instanceof Sdk.Query.ValueBase || n == null) f = n;
            else throw new Error("Sdk.Query.ConditionExpression Values property must inherit from Sdk.Query.ValueBase");
        }
        if (!(this instanceof Sdk.Query.ConditionExpression)) return new Sdk.Query.ConditionExpression(n, t, i, r);
        var e = null,
            o = null,
            a = null,
            u = null,
            f = null;
        (typeof n != "undefined" || n != null) && s(n), (typeof t != "undefined" || t != null) && h(t), (typeof i != "undefined" || i != null) && c(i), typeof r != "undefined" && l(r), this.getEntityName = function() {
                return e
        }, this.setEntityName = function(n) {
            s(n)
        }, this.getAttributeName = function() {
            return o
        }, this.setAttributeName = function(n) {
            h(n)
        }, this.getOperator = function() {
            return u
        }, this.setOperator = function(n) {
            c(n)
        }, this.getValues = function() {
            return f
        }, this.setValues = function(n) {
            l(n)
        }
    }, this.ConditionExpression.__class = !0, this.ConditionOperator = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.FetchExpression = function(n) {
        function i(n) {
            if (typeof n == "string") t = n;
            else throw new Error("Sdk.Query.FetchExpression FetchXml property is required and must be a String.");
        }
        if (!(this instanceof Sdk.Query.FetchExpression)) return new Sdk.Query.FetchExpression(n);
        Sdk.Query.QueryBase.call(this, "FetchExpression");
        var t;
        i(n), this.getFetchXml = function() {
            return t
        }, this.setFetchXml = function(n) {
            i(n)
        }
    }, this.FetchExpression.__class = !0, this.FilterExpression = function(n) {
        function f(n) {
            if (n instanceof Sdk.Query.ConditionExpression) t.add(n);
            else throw new Error("An Sdk.Query.FilterExpression condition must be an Sdk.Query.ConditionExpression");
        }

        function e(n) {
            if (Sdk.Query.LogicalOperator.And == n || Sdk.Query.LogicalOperator.Or == n) i = n;
            else throw new Error("An Sdk.Query.FilterExpression condition must be an Sdk.Query.LogicalOperator");
        }

        function o(n) {
            if (n instanceof Sdk.Query.FilterExpression) r.add(n);
            else throw new Error("An Sdk.Query.FilterExpression filter must be an Sdk.Query.FilterExpression");
        }

        function s(n) {
            if (typeof n == "boolean") u = n;
            else throw new Error("An Sdk.Query.FilterExpression IsQuickfindFilter must be an Boolean Value");
        }
        if (!(this instanceof Sdk.Query.FilterExpression)) return new Sdk.Query.FilterExpression(n);
        var t = new Sdk.Collection(Sdk.Query.ConditionExpression),
            i = Sdk.Query.LogicalOperator.And,
            r = new Sdk.Collection(Sdk.Query.FilterExpression),
            u = !1;
        (typeof n != "undefined" || n != null) && e(n), this.addCondition = function(n, t, i, r) {
                n instanceof Sdk.Query.ConditionExpression ? f(n) : f(new Sdk.Query.ConditionExpression(n, t, i, r))
        }, this.addFilter = function(n) {
            if (n instanceof Sdk.Query.FilterExpression) o(n);
            else if (n == Sdk.Query.LogicalOperator.And || n == Sdk.Query.LogicalOperator.Or) o(new Sdk.Query.FilterExpression(n));
            else throw new Error("An Sdk.Query.FilterExpression addFilter method parameter must be an  must be an Sdk.Query.FilterExpression or an Sdk.Query.LogicalOperator");
        }, this.getConditions = function() {
            return t
        }, this.getFilterOperator = function() {
            return i
        }, this.getFilters = function() {
            return r
        }, this.getIsQuickFindFilter = function() {
            return u
        }, this.setFilterOperator = function(n) {
            e(n)
        }, this.setIsQuickFindFilter = function(n) {
            s(n)
        }
    }, this.FilterExpression.__class = !0, this.JoinOperator = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.LinkEntity = function(n, t, i, r, u, f) {
        function e(n) {
            if (n instanceof Sdk.ColumnSet) k = n;
            else throw new Error("Sdk.Query.LinkEntity Columns property must be an Sdk.ColumnSet");
        }

        function c(n) {
            if (typeof n == "string") b = n;
            else throw new Error("Sdk.Query.LinkEntity EntityAlias property must be an String");
        }

        function h(n) {
            if (n == Sdk.Query.JoinOperator.Inner || n == Sdk.Query.JoinOperator.LeftOuter || n == Sdk.Query.JoinOperator.Natural) w = n;
            else throw new Error("Sdk.Query.LinkEntity JoinOperator  property must be an Sdk.Query.JoinOperator value");
        }

        function tt(n) {
            if (n instanceof Sdk.Query.FilterExpression) p = n;
            else throw new Error("Sdk.Query.LinkEntity LinkCriteria property must be an Sdk.Query.FilterExpression");
        }

        function s(n) {
            if (typeof n == "string") v = n;
            else throw new Error("Sdk.Query.LinkEntity LinkFromAttributeName property must be an String");
        }

        function o(n) {
            if (typeof n == "string") d = n;
            else throw new Error("Sdk.Query.LinkEntity LinkFromEntityName property must be an String");
        }

        function y(n) {
            if (typeof n == "string") a = n;
            else throw new Error("Sdk.Query.LinkEntity LinkToAttributeName property must be an String");
        }

        function g(n) {
            if (typeof n == "string") l = n;
            else throw new Error("Sdk.Query.LinkEntity LinkToEntityName property must be an String");
        }
        if (!(this instanceof Sdk.Query.LinkEntity)) return new Sdk.Query.LinkEntity(n, t, i, r, u, f);
        var k = new Sdk.ColumnSet,
            b = null,
            w = null,
            p = null,
            nt = new Sdk.Collection(Sdk.Query.LinkEntity),
            v = null,
            d = null,
            a = null,
            l = null;
        (typeof n != "undefined" || n != null) && o(n), (typeof t != "undefined" || t != null) && g(t), (typeof i != "undefined" || i != null) && s(i), (typeof r != "undefined" || r != null) && y(r), (typeof u != "undefined" || u != null) && h(u), (typeof f != "undefined" || f != null) && c(f), this.addLink = function(n) {
                this.getLinkEntities().add(n)
        }, this.getColumns = function() {
            return k
        }, this.setColumns = function(n) {
            var i, t;
            if (n instanceof Sdk.ColumnSet) e(n);
            else if (typeof n != "undefined" && typeof n.push != "undefined") e(new Sdk.ColumnSet(n));
            else if (arguments.length > 0) {
                for (i = [], t = 0; t < arguments.length; t++) typeof arguments[t] == "string" && i.push(arguments[t]);
                e(new Sdk.ColumnSet(i))
            }
        }, this.getEntityAlias = function() {
            return b
        }, this.setEntityAlias = function(n) {
            c(n)
        }, this.getJoinOperator = function() {
            return w
        }, this.setJoinOperator = function(n) {
            h(n)
        }, this.getLinkCriteria = function() {
            return p
        }, this.setLinkCriteria = function(n) {
            tt(n)
        }, this.getLinkEntities = function() {
            return nt
        }, this.getLinkFromAttributeName = function() {
            return v
        }, this.setLinkFromAttributeName = function(n) {
            s(n)
        }, this.getLinkFromEntityName = function() {
            return d
        }, this.setLinkFromEntityName = function(n) {
            o(n)
        }, this.getLinkToAttributeName = function() {
            return a
        }, this.setLinkToAttributeName = function(n) {
            y(n)
        }, this.getLinkToEntityName = function() {
            return l
        }, this.setLinkToEntityName = function(n) {
            g(n)
        }
    }, this.LinkEntity.__class = !0, this.LogicalOperator = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.OrderExpression = function(n, t) {
        function i(n) {
            if (typeof n == "string") r = n;
            else throw new Error("Sdk.Query.OrderExpression AttributeName property must be an String");
        }

        function f(n) {
            if (n == Sdk.Query.OrderType.Ascending || n == Sdk.Query.OrderType.Descending) u = n;
            else throw new Error("Sdk.Query.OrderExpression OrderType property must be an Sdk.Query.OrderType");
        }
        if (!(this instanceof Sdk.Query.OrderExpression)) return new Sdk.Query.OrderExpression(n, t);
        var r = null,
            u = Sdk.Query.OrderType.Ascending;
        if ((typeof n != "undefined" || n != null) && (typeof t != "undefined" || t != null)) try {
            i(n), f(t)
        } catch (e) {
            throw e;
        } else(typeof n != "undefined" || n != null) && i(n);
        this.getAttributeName = function() {
            return r
        }, this.setAttributeName = function(n) {
            i(n)
        }, this.getOrderType = function() {
            return u
        }, this.setOrderType = function(n) {
            f(n)
        }
    }, this.OrderExpression.__class = !0, this.OrderType = function() {
        throw new Error("Constructor not implemented this is a static enum.");
    }, this.PagingInfo = function() {
        function u(t) {
            if (typeof t == "number") n = t;
            else throw new Error("Sdk.Query.PagingInfo Count property must be an Number");
        }

        function f(n) {
            if (typeof n == "number") t = n;
            else throw new Error("Sdk.Query.PagingInfo PageNumber property must be an Number");
        }

        function e(n) {
            if (typeof n == "string") i = n;
            else throw new Error("Sdk.Query.PagingInfo PagingCookie property must be an String");
        }

        function o(n) {
            if (typeof n == "boolean") r = n;
            else throw new Error("Sdk.Query.PagingInfo ReturnTotalRecordCount property must be an Boolean");
        }
        if (!(this instanceof Sdk.Query.PagingInfo)) return new Sdk.Query.PagingInfo;
        var n = 0,
            t = 0,
            i = null,
            r = !1;
        this.getCount = function() {
            return n
        }, this.setCount = function(n) {
            u(n)
        }, this.getPageNumber = function() {
            return t
        }, this.setPageNumber = function(n) {
            f(n)
        }, this.getPagingCookie = function() {
            return i
        }, this.setPagingCookie = function(n) {
            e(n)
        }, this.getReturnTotalRecordCount = function() {
            return r
        }, this.setReturnTotalRecordCount = function(n) {
            o(n)
        }
    }, this.PagingInfo.__class = !0, this.QueryBase = function(n) {
        function i(n) {
            if (typeof n == "string" && (n == "QueryExpression" || n == "FetchExpression" || n == "QueryByAttribute")) t = n;
            else throw new Error("Sdk.Query.QueryBase Type value must be a string value of either: 'QueryExpression','FetchExpression', or 'QueryByAttribute'");
        }
        if (!(this instanceof Sdk.Query.QueryBase)) return new Sdk.Query.QueryBase(n);
        var t;
        this.getQueryType = function() {
            return t
        }, i(n)
    }, this.QueryByAttribute = function(n) {
        function r(n) {
            if (n instanceof Sdk.ColumnSet) u = n;
            else throw new Error("Sdk.Query.QueryByAttribute ColumnSet property must be an Sdk.ColumnSet");
        }

        function h(n) {
            if (n instanceof Sdk.Collection && n.getType() == Sdk.Query.OrderExpression) e = n;
            else throw new Error("Sdk.Query.QueryByAttribute Oerders property must be an Sdk.Collection of Sdk.Query.OrderExpression instances.");
        }

        function o(n) {
            if (typeof n == "string") f = n;
            else throw new Error("Sdk.Query.QueryByAttribute EntityName property must be an String");
        }

        function c(n) {
            if (n instanceof Sdk.Query.PagingInfo) t = n;
            else throw new Error("Sdk.Query.QueryByAttribute PageInfo property must be an Sdk.Query.PagingInfo");
        }

        function l(n) {
            if (typeof n == "number") i = n;
            else throw new Error("Sdk.Query.QueryByAttribute TopCount property must be an Number");
        }
        if (!(this instanceof Sdk.Query.QueryByAttribute)) return new Sdk.Query.QueryByAttribute(n);
        Sdk.Query.QueryBase.call(this, "QueryByAttribute");
        var s = new Sdk.Collection(Sdk.AttributeBase),
            u = new Sdk.ColumnSet,
            f = null,
            e = new Sdk.Collection(Sdk.Query.OrderExpression),
            t = new Sdk.Query.PagingInfo,
            i = null;
        (typeof n != "undefined" || n != null) && o(n), this.getColumnSet = function() {
                return u
        }, this.setColumnSet = function(n) {
            var i, t;
            if (n instanceof Sdk.ColumnSet) r(n);
            else if (typeof n != "undefined" && typeof n.push != "undefined") r(new Sdk.ColumnSet(n));
            else if (arguments.length > 0) {
                for (i = [], t = 0; t < arguments.length; t++) typeof arguments[t] == "string" && i.push(arguments[t]);
                r(new Sdk.ColumnSet(i))
            }
        }, this.getEntityName = function() {
            return f
        }, this.setEntityName = function(n) {
            o(n)
        }, this.getAttributeValues = function() {
            return s
        }, this.getOrders = function() {
            return e
        }, this.setOrders = function(n) {
            h(n)
        }, this.getPageInfo = function() {
            return t
        }, this.setPageInfo = function(n) {
            c(n), i = null
        }, this.getTopCount = function() {
            return i
        }, this.setTopCount = function(n) {
            l(n), t = null
        }
    }, this.QueryByAttribute.__class = !0, this.QueryExpression = function(n) {
        function t(n) {
            if (n instanceof Sdk.ColumnSet) o = n;
            else throw new Error("Sdk.Query.QueryExpression ColumnSet property must be an Sdk.ColumnSet");
        }

        function z(n) { /* Added by Carlton 3/2/2017 - you should be able to setOrders on a QueryExpression */
            if (n instanceof Sdk.Collection && n.getType() == Sdk.Query.OrderExpression) l = n;
            else throw new Error("Sdk.Query.QueryByAttribute Oerders property must be an Sdk.Collection of Sdk.Query.OrderExpression instances.");
        }

        function v(n) {
            if (n instanceof Sdk.Query.FilterExpression) e = n;
            else throw new Error("Sdk.Query.QueryExpression Criteria property must be an Sdk.Query.FilterExpression");
        }

        function y(n) {
            if (typeof n == "boolean") h = n;
            else throw new Error("Sdk.Query.QueryExpression Distinct property must be an Boolean");
        }

        function s(n) {
            if (typeof n == "string") u = n;
            else throw new Error("Sdk.Query.QueryExpression EntityName property must be an String");
        }

        function p(n) {
            if (typeof n == "boolean") f = n;
            else throw new Error("Sdk.Query.QueryExpression NoLock property must be an Boolean");
        }

        function a(n) {
            if (n instanceof Sdk.Query.PagingInfo) r = n;
            else throw new Error("Sdk.Query.QueryExpression PageInfo property must be an Sdk.Query.PagingInfo");
        }

        function w(n) {
            if (typeof n == "number") i = n;
            else throw new Error("Sdk.Query.QueryExpression TopCount property must be an Number");
        }
        if (!(this instanceof Sdk.Query.QueryExpression)) return new Sdk.Query.QueryExpression(n);
        Sdk.Query.QueryBase.call(this, "QueryExpression");
        var o = new Sdk.ColumnSet,
            e = new Sdk.Query.FilterExpression(Sdk.Query.LogicalOperator.And),
            h = !1,
            u = null,
            c = new Sdk.Collection(Sdk.Query.LinkEntity),
            f = !1,
            l = new Sdk.Collection(Sdk.Query.OrderExpression),
            r = new Sdk.Query.PagingInfo,
            i = null;
        (typeof n != "undefined" || n != null) && s(n), this.getColumnSet = function() {
                return o
        }, this.setColumnSet = function(n) {
            var r, i;
            if (n instanceof Sdk.ColumnSet) t(n);
            else if (typeof n != "undefined" && typeof n.push != "undefined") t(new Sdk.ColumnSet(n));
            else if (arguments.length > 0) {
                for (r = [], i = 0; i < arguments.length; i++) typeof arguments[i] == "string" && r.push(arguments[i]);
                t(new Sdk.ColumnSet(r))
            }
        }, this.getCriteria = function() {
            return e
        }, this.setCriteria = function(n) {
            v(n)
        }, this.getDistinct = function() {
            return h
        }, this.setDistinct = function(n) {
            y(n)
        }, this.getEntityName = function() {
            return u
        }, this.setEntityName = function(n) {
            s(n)
        }, this.getLinkEntities = function() {
            return c
        }, this.getNoLock = function() {
            return f
        }, this.setNoLock = function(n) {
            p(n)
        }, this.getOrders = function() {
            return l
        }, this.setOrders = function (n) {
            z(n);
        }, this.getPageInfo = function () {
            return r
        }, this.setPageInfo = function(n) {
            a(n), i = null
        }, this.getTopCount = function() {
            return i
        }, this.setTopCount = function(n) {
            w(n), r = null
        }
    }, this.QueryExpression.__class = !0, this.ValueBase = function() {
        if (!(this instanceof Sdk.Query.ValueBase)) return new Sdk.Query.ValueBase
    }, this.Booleans = function(n) {
        function i(n) {
            var i = "Sdk.Query.Booleans Values property must be an Array of Boolean values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Boolean, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Booleans)) return new Sdk.Query.Booleans(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Boolean);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:boolean"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Booleans.__class = !0, this.BooleanManagedProperties = function(n) {
        function i(n) {
            var i = "Sdk.Query.BooleanManagedProperties Values property must be an Array of Boolean values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Boolean, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.BooleanManagedProperties)) return new Sdk.Query.BooleanManagedProperties(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Boolean);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:boolean"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.BooleanManagedProperties.__class = !0, this.Dates = function(n) {
        function i(n) {
            var i = "Sdk.Query.Dates Values property must be an Array of Date values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Date, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Dates)) return new Sdk.Query.Dates(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Date);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:dateTime"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Dates.__class = !0, this.Decimals = function(n) {
        function i(n) {
            var i = "Sdk.Query.Decimals Values property must be an Array of Number values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Number, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Decimals)) return new Sdk.Query.Decimals(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Number);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:decimal"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Decimals.__class = !0, this.Doubles = function(n) {
        function i(n) {
            var i = "Sdk.Query.Doubles Values property must be an Array of Number values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Number, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Doubles)) return new Sdk.Query.Doubles(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Number);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:double"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Doubles.__class = !0, this.EntityReferences = function(n) {
        function i(n) {
            var i = "Sdk.Query.EntityReferences Values property can be an Array of Sdk.EntityReference values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Sdk.EntityReference, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.EntityReferences)) return new Sdk.Query.EntityReferences(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Sdk.EntityReference);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "e:guid"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.EntityReferences.__class = !0, this.Guids = function(n) {
        function i(n) {
            var r = "Sdk.Query.Guids values must be an Array of String values.",
                i;
            if (typeof n.push == "function") {
                for (i = 0; i < n.length; i++)
                    if (!Sdk.Util.isGuid(n[i])) throw new Error("Sdk.Query.Guids values must be String representations of Guid values.");
                t = new Sdk.Collection(String, n)
            } else throw new Error(r);
        }
        if (!(this instanceof Sdk.Query.Guids)) return new Sdk.Query.Guids(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(String);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "e:guid"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Guids.__class = !0, this.Ints = function(n) {
        function i(n) {
            var i = "Sdk.Query.Ints Values property must be an Array of Number values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Number, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Ints)) return new Sdk.Query.Ints(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Number);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:int"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Ints.__class = !0, this.Longs = function(n) {
        function i(n) {
            var i = "Sdk.Query.Longs Values property can be an Array of Number values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Number, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Longs)) return new Sdk.Query.Longs(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Number);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:long"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Longs.__class = !0, this.Money = function(n) {
        function i(n) {
            var i = "Sdk.Query.Money Values property must be an Array of Number values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Number, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Money)) return new Sdk.Query.Money(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Number);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:decimal"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Money.__class = !0, this.OptionSets = function(n) {
        function i(n) {
            var i = "Sdk.Query.OptionSets Values property must be an Array of Number values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(Number, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.OptionSets)) return new Sdk.Query.OptionSets(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(Number);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:int"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.OptionSets.__class = !0, this.Strings = function(n) {
        function i(n) {
            var i = "Sdk.Query.Strings Values property must be an Array of String values.";
            if (typeof n.push == "function") try {
                t = new Sdk.Collection(String, n)
            } catch (r) {
                throw new Error(i);
            } else throw new Error(i);
        }
        if (!(this instanceof Sdk.Query.Strings)) return new Sdk.Query.Strings(n);
        Sdk.Query.ValueBase.call(this);
        var t = new Sdk.Collection(String);
        typeof n != "undefined" && i(n), this.getValues = function() {
            return t
        }, this.getType = function() {
            return "c:string"
        }, this.setValues = function(n) {
            i(n)
        }
    }, this.Strings.__class = !0
}.call(Sdk.Query),
function() {
    this.associate = function(n, t, i, r, u, f, e) {
        var s, h, o;
        if (typeof n != "string") throw new Error("Sdk.Async.associate entityName parameter is required and must be a String.");
        if (typeof t != "string" || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Async.associate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof i != "string") throw new Error("Sdk.Async.associate relationship parameter is required and must be a String.");
        if (!(r instanceof Sdk.Collection)) throw new Error("Sdk.Async.associate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (r.getCount() <= 0) throw new Error("Sdk.Async.associate relatedEntities parameter must contain entity references to associate.");
        if (r.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.Async.associate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), u != null && typeof u != "function") throw new Error("Sdk.Async.associate successCallBack parameter must be null or a function.");
        if (f != null && typeof f != "function") throw new Error("Sdk.Async.associate errorCallBack parameter must be null or a function.");
        s = [], r.forEach(function(n) {
            s.push(n.toXml())
        }), h = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Associate>", "<d:entityName>", n.toLowerCase(), "</d:entityName>", "<d:entityId>", t, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", i, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", s.join(""), "</d:relatedEntities>", "</d:Associate>", "</s:Body>", "</s:Envelope>"].join(""), o = Sdk.Util.getXMLHttpRequest("Associate", !0), o.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? u && u(e) : f && f(Sdk.Util.getError(this), e))
        }, o.send(h), o = null
    }, this.create = function(n, t, i, r) {
        if (!(n instanceof Sdk.Entity)) throw new Error("Sdk.Async.create entity parameter is required and must be an Sdk.Entity instance");
        if (t != null && typeof t != "function") throw new Error("Sdk.Async.create successCallBack parameter must be null or a function.");
        if (i != null && typeof i != "function") throw new Error("Sdk.Async.create errorCallBack parameter must be null or a function.");
        var f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Create>", n.toXml("create"), "</d:Create>", "</s:Body>", "</s:Envelope>"].join(""),
            u = Sdk.Util.getXMLHttpRequest("Create", !0);
        u.onreadystatechange = function() {
            var u, f;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (u = this.responseXML, Sdk.Xml.setSelectionNamespaces(u), f = Sdk.Xml.selectSingleNodeText(u, "s:Envelope/s:Body/d:CreateResponse/d:CreateResult"), n.setId(f, !0), n.setIsChanged(!1), t && t(f, r)) : i && i(Sdk.Util.getError(this), r))
        }, u.send(f), u = null
    }, this.del = function(n, t, i, r, u) {
        if (typeof n != "string") throw new Error("Sdk.Async.del entityName parameter is required and must be a String.");
        if (typeof t != "string" || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Async.del id is required and must be a string representation of a GUID value.");
        if (i != null && typeof i != "function") throw new Error("Sdk.Async.del successCallBack parameter must be null or a function.");
        if (r != null && typeof r != "function") throw new Error("Sdk.Async.del errorCallBack parameter must be null or a function.");
        var e = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Delete>", "<d:entityName>", n, "</d:entityName>", "<d:id>", t, "</d:id>", "</d:Delete>", "</s:Body>", "</s:Envelope>"].join(""),
            f = Sdk.Util.getXMLHttpRequest("Delete", !0);
        f.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? i && i(u) : r && r(Sdk.Util.getError(this), u))
        }, f.send(e), f = null
    }, this.disassociate = function(n, t, i, r, u, f, e) {
        var s, h, o;
        if (typeof n != "string") throw new Error("Sdk.Async.disassociate entityName parameter is required and must be a String.");
        if (typeof t != "string" || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Async.disassociate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof i != "string") throw new Error("Sdk.Async.disassociate relationship parameter is required and must be a String.");
        if (!(r instanceof Sdk.Collection)) throw new Error("Sdk.Async.disassociate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (r.getCount() <= 0) throw new Error("Sdk.Async.disassociate relatedEntities parameter must contain entity references to Dissassociate.");
        if (r.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.Async.disassociate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), u != null && typeof u != "function") throw new Error("Sdk.Async.disassociate successCallBack parameter must be null or a function.");
        if (f != null && typeof f != "function") throw new Error("Sdk.Async.disassociate errorCallBack parameter must be null or a function.");
        s = [], r.forEach(function(n) {
            s.push(n.toXml())
        }), h = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Disassociate>", "<d:entityName>", n.toLowerCase(), "</d:entityName>", "<d:entityId>", t, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", i, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", s.join(""), "</d:relatedEntities>", "</d:Disassociate>", "</s:Body>", "</s:Envelope>"].join(""), o = Sdk.Util.getXMLHttpRequest("Disassociate", !0), o.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? u && u(e) : f && f(Sdk.Util.getError(this), e))
        }, o.send(h), o = null
    }, this.execute = function(n, t, i, r) {
        if (!(n instanceof Sdk.OrganizationRequest)) throw new Error("Sdk.Async.execute request parameter must be an Sdk.OrganizationRequest .");
        if (t != null && typeof t != "function") throw new Error("Sdk.Async.execute successCallBack parameter must be null or a function.");
        if (i != null && typeof i != "function") throw new Error("Sdk.Async.execute errorCallBack parameter must be null or a function.");
        var f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Execute>", n.getRequestXml(), "</d:Execute>", "</s:Body>", "</s:Envelope>"].join(""),
            u = Sdk.Util.getXMLHttpRequest("Execute", !0);
        u.onreadystatechange = function() {
            if (this.readyState == 4)
                if (this.onreadystatechange = null, this.status == 200) {
                    if (t) {
                        var u = this.responseXML;
                        Sdk.Xml.setSelectionNamespaces(u), t(new n.getResponseType()(u), r)
                    }
                } else i && i(Sdk.Util.getError(this), r)
        }, u.send(f), u = null
    }, this.retrieve = function(n, t, i, r, u, f) {
        if (typeof n != "string") throw new Error("Sdk.Async.retrieve entityName parameter is required and must be a string.");
        if (t == null || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Async.retrieve id parameter is required and must be a string representation of a GUID.");
        if (!(i instanceof Sdk.ColumnSet)) throw new Error("Sdk.Async.retrieve columnSet parameter is required and must be a Sdk.ColumnSet.");
        if (typeof r != "function") throw new Error("Sdk.Async.retrieve successCallBack parameter must be null or a function.");
        if (u != null && typeof u != "function") throw new Error("Sdk.Async.retrieve errorCallBack parameter must be null or a function.");
        var o = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Retrieve>", "<d:entityName>", n, "</d:entityName>", "<d:id>", t, "</d:id>", i.toXml(), "</d:Retrieve>", "</s:Body>", "</s:Envelope>"].join(""),
            e = Sdk.Util.getXMLHttpRequest("Retrieve", !0);
        e.onreadystatechange = function() {
            var n, t;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), t = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "s:Envelope/s:Body/d:RetrieveResponse/d:RetrieveResult")), r && r(t, f)) : u && u(Sdk.Util.getError(this), f))
        }, e.send(o), e = null
    }, this.retrieveMultiple = function(n, t, i, r) {
        var u = null,
            e, f;
        if (n instanceof Sdk.Query.QueryBase) n instanceof Sdk.Query.FetchExpression && (u = "FetchExpression"), n instanceof Sdk.Query.QueryExpression && (u = "QueryExpression"), n instanceof Sdk.Query.QueryByAttribute && (u = "QueryByAttribute");
        else throw new Error("Sdk.Async.retrieveMultiple constructor query parameter is required and must be either an Sdk.Query.FetchExpression, Sdk.Query.QueryByAttribute, or an Sdk.Query.QueryExpression."); if (typeof t != "function") throw new Error("Sdk.Async.retrieveMultiple successCallBack parameter must be null or a Function.");
        if (i != null && typeof i != "function") throw new Error("Sdk.Async.retrieveMultiple errorCallBack parameter must be null or a Function.");
        e = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:RetrieveMultiple>", '<d:query i:type="a:', u, '">', n.toValueXml(), "</d:query>", "</d:RetrieveMultiple>", "</s:Body>", "</s:Envelope>"].join(""), f = Sdk.Util.getXMLHttpRequest("RetrieveMultiple", !0), f.onreadystatechange = function() {
            var n, u;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), u = Sdk.Util.createEntityCollectionFromNode(Sdk.Xml.selectSingleNode(n, "s:Envelope/s:Body/d:RetrieveMultipleResponse/d:RetrieveMultipleResult")), t && t(u, r)) : i && i(Sdk.Util.getError(this), r))
        }, f.send(e), f = null
    }, this.update = function(n, t, i, r) {
        if (!(n instanceof Sdk.Entity)) throw new Error("Sdk.Async.update entity parameter is required and must be an Sdk.Entity instance");
        if (t != null && typeof t != "function") throw new Error("Sdk.Async.update successCallBack parameter must be null or a function.");
        if (i != null && typeof i != "function") throw new Error("Sdk.Async.update errorCallBack parameter must be null or a function.");
        if (n.getIsChanged()) {
            var f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Update>", n.toXml("update"), "</d:Update>", "</s:Body>", "</s:Envelope>"].join(""),
                u = Sdk.Util.getXMLHttpRequest("Update", !0);
            u.onreadystatechange = function() {
                this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n.setIsChanged(!1), t && t(!0, r)) : i && i(Sdk.Util.getError(this), r))
            }, u.send(f), u = null
        } else t(!1, r)
    }
}.call(Sdk.Async),
function() {
    function t(t) {
        if (typeof n == "undefined") throw new Error(Sdk.Util.format("Sdk.jQ.{0} requires a specific jQuery variable set using Sdk.jQ.setJQueryVariable.", [t]));
    }
    this.associate = function(i, r, u, f) {
        var s, h, e, o;
        if (typeof i != "string") throw new Error("Sdk.jQ.associate entityName parameter is required and must be a String.");
        if (typeof r != "string" || !Sdk.Util.isGuidOrNull(r)) throw new Error("Sdk.jQ.associate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof u != "string") throw new Error("Sdk.jQ.associate relationship parameter is required and must be a String.");
        if (!(f instanceof Sdk.Collection)) throw new Error("Sdk.jQ.associate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (f.getCount() <= 0) throw new Error("Sdk.jQ.associate relatedEntities parameter must contain entity references to associate.");
        return f.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.jQ.associate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), s = [], f.forEach(function(n) {
            s.push(n.toXml())
        }), h = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Associate>", "<d:entityName>", i.toLowerCase(), "</d:entityName>", "<d:entityId>", r, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", u, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", s.join(""), "</d:relatedEntities>", "</d:Associate>", "</s:Body>", "</s:Envelope>"].join(""), t("associate"), e = n.Deferred(), o = Sdk.Util.getXMLHttpRequest("Associate", !0), o.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? e.resolve() : e.reject(Sdk.Util.getError(this)))
        }, o.send(h), o = null, e.promise()
    }, this.create = function(i) {
        var f, r, u;
        if (!(i instanceof Sdk.Entity)) throw new Error("Sdk.jQ.create entity parameter is required and must be an Sdk.Entity instance");
        return f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Create>", i.toXml("create"), "</d:Create>", "</s:Body>", "</s:Envelope>"].join(""), t("create"), r = n.Deferred(), u = Sdk.Util.getXMLHttpRequest("Create", !0), u.onreadystatechange = function() {
            var n, t;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), t = Sdk.Xml.selectSingleNodeText(n, "s:Envelope/s:Body/d:CreateResponse/d:CreateResult"), i.setId(t, !0), i.setIsChanged(!1), r.resolve(t)) : r.reject(Sdk.Util.getError(this)))
        }, u.send(f), u = null, r.promise()
    }, this.del = function(i, r) {
        var e, u, f;
        if (typeof i != "string") throw new Error("Sdk.jQ.del entityName parameter is required and must be a String.");
        if (typeof r != "string" || !Sdk.Util.isGuidOrNull(r)) throw new Error("Sdk.jQ.del id is required and must be a string representation of a GUID value.");
        return e = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Delete>", "<d:entityName>", i, "</d:entityName>", "<d:id>", r, "</d:id>", "</d:Delete>", "</s:Body>", "</s:Envelope>"].join(""), t("delete"), u = n.Deferred(), f = Sdk.Util.getXMLHttpRequest("Delete", !0), f.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? u.resolve() : u.reject(Sdk.Util.getError(this)))
        }, f.send(e), f = null, u.promise()
    }, this.disassociate = function(i, r, u, f) {
        var s, h, e, o;
        if (typeof i != "string") throw new Error("Sdk.jQ.disassociate entityName parameter is required and must be a String.");
        if (typeof r != "string" || !Sdk.Util.isGuidOrNull(r)) throw new Error("Sdk.jQ.disassociate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof u != "string") throw new Error("Sdk.jQ.disassociate relationship parameter is required and must be a String.");
        if (!(f instanceof Sdk.Collection)) throw new Error("Sdk.jQ.disassociate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (f.getCount() <= 0) throw new Error("Sdk.jQ.disassociate relatedEntities parameter must contain entity references to Dissassociate.");
        return f.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.jQ.disassociate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), s = [], f.forEach(function(n) {
            s.push(n.toXml())
        }), h = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Disassociate>", "<d:entityName>", i.toLowerCase(), "</d:entityName>", "<d:entityId>", r, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", u, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", s.join(""), "</d:relatedEntities>", "</d:Disassociate>", "</s:Body>", "</s:Envelope>"].join(""), t("disassociate"), e = n.Deferred(), o = Sdk.Util.getXMLHttpRequest("Disassociate", !0), o.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? e.resolve() : e.reject(Sdk.Util.getError(this)))
        }, o.send(h), o = null, e.promise()
    }, this.execute = function(i) {
        var f, r, u;
        if (!(i instanceof Sdk.OrganizationRequest)) throw new Error("Sdk.jQ.execute request parameter must be an Sdk.OrganizationRequest .");
        return f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Execute>", i.getRequestXml(), "</d:Execute>", "</s:Body>", "</s:Envelope>"].join(""), t("execute"), r = n.Deferred(), u = Sdk.Util.getXMLHttpRequest("Execute", !0), u.onreadystatechange = function() {
            if (this.readyState == 4)
                if (this.onreadystatechange = null, this.status == 200) {
                    var n = this.responseXML;
                    Sdk.Xml.setSelectionNamespaces(n), r.resolve(new i.getResponseType()(n))
                } else r.reject(Sdk.Util.getError(this))
        }, u.send(f), u = null, r.promise()
    }, this.retrieve = function(i, r, u) {
        var o, f, e;
        if (typeof i != "string") throw new Error("Sdk.jQ.retrieve entityName parameter is required and must be a string.");
        if (r == null || !Sdk.Util.isGuidOrNull(r)) throw new Error("Sdk.jQ.retrieve id parameter is required and must be a string representation of a GUID.");
        if (!(u instanceof Sdk.ColumnSet)) throw new Error("Sdk.jQ.retrieve columnSet parameter is required and must be a Sdk.ColumnSet.");
        return o = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Retrieve>", "<d:entityName>", i, "</d:entityName>", "<d:id>", r, "</d:id>", u.toXml(), "</d:Retrieve>", "</s:Body>", "</s:Envelope>"].join(""), t("retrieve"), f = n.Deferred(), e = Sdk.Util.getXMLHttpRequest("Retrieve", !0), e.onreadystatechange = function() {
            var n, t;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), t = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "s:Envelope/s:Body/d:RetrieveResponse/d:RetrieveResult")), f.resolve(t)) : f.reject(Sdk.Util.getError(this)))
        }, e.send(o), e = null, f.promise()
    }, this.retrieveMultiple = function(i) {
        var r = null,
            e, u, f;
        if (i instanceof Sdk.Query.QueryBase) i instanceof Sdk.Query.FetchExpression && (r = "FetchExpression"), i instanceof Sdk.Query.QueryExpression && (r = "QueryExpression"), i instanceof Sdk.Query.QueryByAttribute && (r = "QueryByAttribute");
        else throw new Error("Sdk.jQ.retrieveMultiple constructor query parameter is required and must be either an Sdk.Query.FetchExpression, Sdk.Query.QueryByAttribute, or an Sdk.Query.QueryExpression.");
        return e = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:RetrieveMultiple>", '<d:query i:type="a:', r, '">', i.toValueXml(), "</d:query>", "</d:RetrieveMultiple>", "</s:Body>", "</s:Envelope>"].join(""), t("retrieveMultiple"), u = n.Deferred(), f = Sdk.Util.getXMLHttpRequest("RetrieveMultiple", !0), f.onreadystatechange = function() {
            var n, t;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), t = Sdk.Util.createEntityCollectionFromNode(Sdk.Xml.selectSingleNode(n, "s:Envelope/s:Body/d:RetrieveMultipleResponse/d:RetrieveMultipleResult")), u.resolve(t)) : u.reject(Sdk.Util.getError(this)))
        }, f.send(e), f = null, u.promise()
    }, this.setJQueryVariable = function(t) {
        if (typeof t.support != "undefined") n = t;
        else throw new Error("The variable passed to Sdk.jQ.setJQueryVariable is not a valid global jQuery object.");
    }, this.update = function(i) {
        var r, f, u;
        if (!(i instanceof Sdk.Entity)) throw new Error("Sdk.jQ.update entity parameter is required and must be an Sdk.Entity instance");
        return t("update"), r = n.Deferred(), i.getIsChanged() ? (f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Update>", i.toXml("update"), "</d:Update>", "</s:Body>", "</s:Envelope>"].join(""), u = Sdk.Util.getXMLHttpRequest("Update", !0), u.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (i.setIsChanged(!1), r.resolve(!0)) : r.reject(Sdk.Util.getError(this)))
        }, u.send(f), u = null, r.promise()) : (r.resolve(!1), r.promise())
    };
    var n
}.call(Sdk.jQ),
function() {
    function n(n) {
        if (typeof Q == "undefined") throw new Error(Sdk.Util.format("Sdk.Q.{0} requires the Q.js library to define the global Q object.", [n]));
    }
    this.associate = function(t, i, r, u) {
        var o, s, f, e;
        if (typeof t != "string") throw new Error("Sdk.Q.associate entityName parameter is required and must be a String.");
        if (typeof i != "string" || !Sdk.Util.isGuidOrNull(i)) throw new Error("Sdk.Q.associate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof r != "string") throw new Error("Sdk.Q.associate relationship parameter is required and must be a String.");
        if (!(u instanceof Sdk.Collection)) throw new Error("Sdk.Q.associate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (u.getCount() <= 0) throw new Error("Sdk.Q.associate relatedEntities parameter must contain entity references to associate.");
        return u.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.Q.associate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), o = [], u.forEach(function(n) {
            o.push(n.toXml())
        }), s = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Associate>", "<d:entityName>", t.toLowerCase(), "</d:entityName>", "<d:entityId>", i, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", r, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", o.join(""), "</d:relatedEntities>", "</d:Associate>", "</s:Body>", "</s:Envelope>"].join(""), n("associate"), f = Q.defer(), e = Sdk.Util.getXMLHttpRequest("Associate", !0), e.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? f.resolve() : f.reject(Sdk.Util.getError(this)))
        }, e.send(s), e = null, f.promise
    }, this.create = function(t) {
        var u, i, r;
        if (!(t instanceof Sdk.Entity)) throw new Error("Sdk.Q.create entity parameter is required and must be an Sdk.Entity instance");
        return u = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Create>", t.toXml("create"), "</d:Create>", "</s:Body>", "</s:Envelope>"].join(""), n("create"), i = Q.defer(), r = Sdk.Util.getXMLHttpRequest("Create", !0), r.onreadystatechange = function() {
            var n, r;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), r = Sdk.Xml.selectSingleNodeText(n, "s:Envelope/s:Body/d:CreateResponse/d:CreateResult"), t.setId(r, !0), t.setIsChanged(!1), i.resolve(r)) : i.reject(Sdk.Util.getError(this)))
        }, r.send(u), r = null, i.promise
    }, this.del = function(t, i) {
        var f, r, u;
        if (typeof t != "string") throw new Error("Sdk.Q.del entityName parameter is required and must be a String.");
        if (typeof i != "string" || !Sdk.Util.isGuidOrNull(i)) throw new Error("Sdk.Q.del id is required and must be a string representation of a GUID value.");
        return f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Delete>", "<d:entityName>", t, "</d:entityName>", "<d:id>", i, "</d:id>", "</d:Delete>", "</s:Body>", "</s:Envelope>"].join(""), n("delete"), r = Q.defer(), u = Sdk.Util.getXMLHttpRequest("Delete", !0), u.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? r.resolve() : r.reject(Sdk.Util.getError(this)))
        }, u.send(f), u = null, r.promise
    }, this.disassociate = function(t, i, r, u) {
        var o, s, f, e;
        if (typeof t != "string") throw new Error("Sdk.Q.disassociate entityName parameter is required and must be a String.");
        if (typeof i != "string" || !Sdk.Util.isGuidOrNull(i)) throw new Error("Sdk.Q.disassociate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof r != "string") throw new Error("Sdk.Q.disassociate relationship parameter is required and must be a String.");
        if (!(u instanceof Sdk.Collection)) throw new Error("Sdk.Q.disassociate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (u.getCount() <= 0) throw new Error("Sdk.Q.disassociate relatedEntities parameter must contain entity references to Dissassociate.");
        return u.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.Q.disassociate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), o = [], u.forEach(function(n) {
            o.push(n.toXml())
        }), s = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Disassociate>", "<d:entityName>", t.toLowerCase(), "</d:entityName>", "<d:entityId>", i, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", r, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", o.join(""), "</d:relatedEntities>", "</d:Disassociate>", "</s:Body>", "</s:Envelope>"].join(""), n("disassociate"), f = Q.defer(), e = Sdk.Util.getXMLHttpRequest("Disassociate", !0), e.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? f.resolve() : f.reject(Sdk.Util.getError(this)))
        }, e.send(s), e = null, f.promise
    }, this.execute = function(t) {
        var u, i, r;
        if (!(t instanceof Sdk.OrganizationRequest)) throw new Error("Sdk.Q.execute request parameter must be an Sdk.OrganizationRequest.");
        return u = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Execute>", t.getRequestXml(), "</d:Execute>", "</s:Body>", "</s:Envelope>"].join(""), n("execute"), i = Q.defer(), r = Sdk.Util.getXMLHttpRequest("Execute", !0), r.onreadystatechange = function() {
            if (this.readyState == 4)
                if (this.onreadystatechange = null, this.status == 200) {
                    var n = this.responseXML;
                    Sdk.Xml.setSelectionNamespaces(n), i.resolve(new t.getResponseType()(n))
                } else i.reject(Sdk.Util.getError(this))
        }, r.send(u), r = null, i.promise
    }, this.retrieve = function(t, i, r) {
        var e, u, f;
        if (typeof t != "string") throw new Error("Sdk.Q.retrieve entityName parameter is required and must be a string.");
        if (i == null || !Sdk.Util.isGuidOrNull(i)) throw new Error("Sdk.Q.retrieve id parameter is required and must be a string representation of a GUID.");
        if (!(r instanceof Sdk.ColumnSet)) throw new Error("Sdk.Q.retrieve columnSet parameter is required and must be a Sdk.ColumnSet.");
        return e = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Retrieve>", "<d:entityName>", t, "</d:entityName>", "<d:id>", i, "</d:id>", r.toXml(), "</d:Retrieve>", "</s:Body>", "</s:Envelope>"].join(""), n("retrieve"), u = Q.defer(), f = Sdk.Util.getXMLHttpRequest("Retrieve", !0), f.onreadystatechange = function() {
            var n, t;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), t = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(n, "s:Envelope/s:Body/d:RetrieveResponse/d:RetrieveResult")), u.resolve(t)) : u.reject(Sdk.Util.getError(this)))
        }, f.send(e), f = null, u.promise
    }, this.retrieveMultiple = function(t) {
        var i = null,
            f, r, u;
        if (t instanceof Sdk.Query.QueryBase) t instanceof Sdk.Query.FetchExpression && (i = "FetchExpression"), t instanceof Sdk.Query.QueryExpression && (i = "QueryExpression"), t instanceof Sdk.Query.QueryByAttribute && (i = "QueryByAttribute");
        else throw new Error("Sdk.Q.retrieveMultiple constructor query parameter is required and must be either an Sdk.Query.FetchExpression, Sdk.Query.QueryByAttribute, or an Sdk.Query.QueryExpression.");
        return f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:RetrieveMultiple>", '<d:query i:type="a:', i, '">', t.toValueXml(), "</d:query>", "</d:RetrieveMultiple>", "</s:Body>", "</s:Envelope>"].join(""), n("retrieveMultiple"), r = Q.defer(), u = Sdk.Util.getXMLHttpRequest("RetrieveMultiple", !0), u.onreadystatechange = function() {
            var n, t;
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (n = this.responseXML, Sdk.Xml.setSelectionNamespaces(n), t = Sdk.Util.createEntityCollectionFromNode(Sdk.Xml.selectSingleNode(n, "s:Envelope/s:Body/d:RetrieveMultipleResponse/d:RetrieveMultipleResult")), r.resolve(t)) : r.reject(Sdk.Util.getError(this)))
        }, u.send(f), u = null, r.promise
    }, this.update = function(t) {
        var i, u, r;
        if (!(t instanceof Sdk.Entity)) throw new Error("Sdk.Q.update entity parameter is required and must be an Sdk.Entity instance");
        return n("update"), i = Q.defer(), t.getIsChanged() ? (u = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Update>", t.toXml("update"), "</d:Update>", "</s:Body>", "</s:Envelope>"].join(""), r = Sdk.Util.getXMLHttpRequest("Update", !0), r.onreadystatechange = function() {
            this.readyState == 4 && (this.onreadystatechange = null, this.status == 200 ? (t.setIsChanged(!1), i.resolve(!0)) : i.reject(Sdk.Util.getError(this)))
        }, r.send(u), r = null, i.promise) : (i.resolve(!1), i.promise)
    }
}.call(Sdk.Q),
function() {
    this.associate = function(n, t, i, r) {
        var f, e, u;
        if (typeof n != "string") throw new Error("Sdk.Sync.associate entityName parameter is required and must be a String.");
        if (typeof t != "string" || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Sync.associate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof i != "string") throw new Error("Sdk.Sync.associate relationship parameter is required and must be a String.");
        if (!(r instanceof Sdk.Collection)) throw new Error("Sdk.Sync.associate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (r.getCount() <= 0) throw new Error("Sdk.Sync.associate relatedEntities parameter must contain entity references to associate.");
        if (r.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.Sync.associate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), f = [], r.forEach(function(n) {
            f.push(n.toXml())
        }), e = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Associate>", "<d:entityName>", n.toLowerCase(), "</d:entityName>", "<d:entityId>", t, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", i, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", f.join(""), "</d:relatedEntities>", "</d:Associate>", "</s:Body>", "</s:Envelope>"].join(""), u = Sdk.Util.getXMLHttpRequest("Associate", !1), u.send(e), u.status != 200) throw new Error("Sdk.Sync.associate " + Sdk.Util.getError(u));
        u = null
    }, this.create = function(n) {
        var u, t, i, r;
        if (!(n instanceof Sdk.Entity)) throw new Error("Sdk.Sync.create entity parameter is required and must be an Sdk.Entity instance");
        if (u = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Create>", n.toXml("create"), "</d:Create>", "</s:Body>", "</s:Envelope>"].join(""), t = Sdk.Util.getXMLHttpRequest("Create", !1), t.send(u), t.status == 200) return i = t.responseXML, Sdk.Xml.setSelectionNamespaces(i), r = Sdk.Xml.selectSingleNodeText(i, "s:Envelope/s:Body/d:CreateResponse/d:CreateResult"), n.setId(r, !0), n.setIsChanged(!1), r;
        throw new Error("Sdk.Sync.create " + Sdk.Util.getError(t));
        t = null
    }, this.del = function(n, t) {
        if (typeof n != "string") throw new Error("Sdk.Sync.del entityName parameter is required and must be a String.");
        if (typeof t != "string" || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Sync.del id is required and must be a string representation of a GUID value.");
        var r = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Delete>", "<d:entityName>", n, "</d:entityName>", "<d:id>", t, "</d:id>", "</d:Delete>", "</s:Body>", "</s:Envelope>"].join(""),
            i = Sdk.Util.getXMLHttpRequest("Delete", !1);
        if (i.send(r), i.status != 200) throw new Error("Sdk.Sync.del " + Sdk.Util.getError(i));
        i = null
    }, this.disassociate = function(n, t, i, r) {
        var f, e, u;
        if (typeof n != "string") throw new Error("Sdk.Sync.disassociate entityName parameter is required and must be a String.");
        if (typeof t != "string" || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Sync.disassociate entityId parameter is required and must be an string representation of a GUID value.");
        if (typeof i != "string") throw new Error("Sdk.Sync.disassociate relationship parameter is required and must be a String.");
        if (!(r instanceof Sdk.Collection)) throw new Error("Sdk.Sync.disassociate relatedEntities parameter is required and must be an Sdk.Collection.");
        if (r.getCount() <= 0) throw new Error("Sdk.Sync.disassociate relatedEntities parameter must contain entity references to Dissassociate.");
        if (r.forEach(function(n) {
            if (!(n instanceof Sdk.EntityReference)) throw new Error("Sdk.Sync.disassociate relatedEntities parameter must contain only Sdk.EntityReference objects.");
        }), f = [], r.forEach(function(n) {
            f.push(n.toXml())
        }), e = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Disassociate>", "<d:entityName>", n.toLowerCase(), "</d:entityName>", "<d:entityId>", t, "</d:entityId>", "<d:relationship>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", i, "</a:SchemaName>", "</d:relationship>", "<d:relatedEntities>", f.join(""), "</d:relatedEntities>", "</d:Disassociate>", "</s:Body>", "</s:Envelope>"].join(""), u = Sdk.Util.getXMLHttpRequest("Disassociate", !1), u.send(e), u.status != 200) throw new Error("Sdk.DisassociateSync " + Sdk.Util.getError(u));
        u = null
    }, this.execute = function(n) {
        var r, t, i;
        if (!(n instanceof Sdk.OrganizationRequest)) throw new Error("Sdk.Sync.execute request parameter must be an Sdk.OrganizationRequest .");
        if (r = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Execute>", n.getRequestXml(), "</d:Execute>", "</s:Body>", "</s:Envelope>"].join(""), t = Sdk.Util.getXMLHttpRequest("Execute", !1), t.send(r), t.status == 200) return i = t.responseXML, Sdk.Xml.setSelectionNamespaces(i), new n.getResponseType()(i);
        throw new Error("Sdk.Sync.execute " + Sdk.Util.getError(t));
        t = null
    }, this.retrieve = function(n, t, i) {
        var f, r, u, e;
        if (typeof n != "string") throw new Error("Sdk.Sync.retrieve entityName parameter is required and must be a string.");
        if (t == null || !Sdk.Util.isGuidOrNull(t)) throw new Error("Sdk.Sync.retrieve id parameter is required and must be a string representation of a GUID.");
        if (!(i instanceof Sdk.ColumnSet)) throw new Error("Sdk.Sync.retrieve columnSet parameter is required and must be a Sdk.ColumnSet.");
        if (f = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Retrieve>", "<d:entityName>", n, "</d:entityName>", "<d:id>", t, "</d:id>", i.toXml(), "</d:Retrieve>", "</s:Body>", "</s:Envelope>"].join(""), r = Sdk.Util.getXMLHttpRequest("Retrieve", !1), r.send(f), r.status == 200) return u = r.responseXML, Sdk.Xml.setSelectionNamespaces(u), e = Sdk.Util.createEntityFromNode(Sdk.Xml.selectSingleNode(u, "s:Envelope/s:Body/d:RetrieveResponse/d:RetrieveResult"));
        throw new Error("Sdk.Sync.retrieve" + Sdk.Util.getError(r));
        r = null
    }, this.retrieveMultiple = function(n) {
        var i = null,
            u, t, r, f;
        if (n instanceof Sdk.Query.QueryBase) n instanceof Sdk.Query.FetchExpression && (i = "FetchExpression"), n instanceof Sdk.Query.QueryExpression && (i = "QueryExpression"), n instanceof Sdk.Query.QueryByAttribute && (i = "QueryByAttribute");
        else throw new Error("Sdk.Sync.retrieveMultiple constructor query parameter is required and must be either an Sdk.Query.FetchExpression, Sdk.Query.QueryByAttribute, or an Sdk.Query.QueryExpression."); if (u = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:RetrieveMultiple>", '<d:query i:type="a:', i, '">', n.toValueXml(), "</d:query>", "</d:RetrieveMultiple>", "</s:Body>", "</s:Envelope>"].join(""), t = Sdk.Util.getXMLHttpRequest("RetrieveMultiple", !1), t.send(u), t.status == 200) return r = t.responseXML, Sdk.Xml.setSelectionNamespaces(r), f = Sdk.Util.createEntityCollectionFromNode(Sdk.Xml.selectSingleNode(r, "s:Envelope/s:Body/d:RetrieveMultipleResponse/d:RetrieveMultipleResult"));
        throw new Error("Sdk.Sync.retrieveMultiple" + Sdk.Util.getError(t));
        t = null
    }, this.update = function(n) {
        if (!(n instanceof Sdk.Entity)) throw new Error("Sdk.Sync.update entity parameter is required and must be an Sdk.Entity instance");
        if (n.getIsChanged()) {
            var i = [Sdk.Xml.getEnvelopeHeader(), "<s:Body>", "<d:Update>", n.toXml("update"), "</d:Update>", "</s:Body>", "</s:Envelope>"].join(""),
                t = Sdk.Util.getXMLHttpRequest("Update", !1);
            if (t.send(i), t.status == 200) return n.setIsChanged(!1), !0;
            throw new Error("Sdk.Sync.update" + Sdk.Util.getError(t));
            t = null
        } else return !1
    }
}.call(Sdk.Sync), Sdk.AccessRights.prototype = {
    None: 0,
    ReadAccess: 1,
    WriteAccess: 2,
    ShareAccess: 4,
    AssignAccess: 8,
    AppendAccess: 16,
    AppendToAccess: 32,
    CreateAccess: 64,
    DeleteAccess: 128,
    All: 255
}, Sdk.AccessRights.None = 0, Sdk.AccessRights.ReadAccess = 1, Sdk.AccessRights.WriteAccess = 2, Sdk.AccessRights.ShareAccess = 4, Sdk.AccessRights.AssignAccess = 8, Sdk.AccessRights.AppendAccess = 16, Sdk.AccessRights.AppendToAccess = 32, Sdk.AccessRights.CreateAccess = 64, Sdk.AccessRights.DeleteAccess = 128, Sdk.AccessRights.All = 255, Sdk.AccessRights.__enum = !0, Sdk.AccessRights.__flags = !0, Sdk.Boolean.prototype = new Sdk.AttributeBase, Sdk.BooleanManagedProperty.prototype = new Sdk.AttributeBase, Sdk.DateTime.prototype = new Sdk.AttributeBase, Sdk.Decimal.prototype = new Sdk.AttributeBase, Sdk.Double.prototype = new Sdk.AttributeBase, Sdk.Guid.prototype = new Sdk.AttributeBase, Sdk.Int.prototype = new Sdk.AttributeBase, Sdk.Long.prototype = new Sdk.AttributeBase, Sdk.Lookup.prototype = new Sdk.AttributeBase, Sdk.Money.prototype = new Sdk.AttributeBase, Sdk.OptionSet.prototype = new Sdk.AttributeBase, Sdk.PartyList.prototype = new Sdk.AttributeBase, Sdk.String.prototype = new Sdk.AttributeBase, Sdk.Boolean.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="c:' + this.getType() + '">' + this.getValue() + "</b:value>"), n
}, Sdk.BooleanManagedProperty.prototype.toValueXml = function() {
    var t = '<b:value i:nil="true" />',
        n;
    return this.getValue() != null && (n = [], n.push('<b:value i:type="a:BooleanManagedProperty">'), n.push("<a:CanBeChanged>" + this.getValue().getCanBeChanged() + "</a:CanBeChanged>"), this.getValue().getManagedPropertyLogicalName() == null ? n.push('<a:ManagedPropertyLogicalName i:nil="true" />') : n.push("<a:ManagedPropertyLogicalName>" + this.getValue().getManagedPropertyLogicalName() + "</a:ManagedPropertyLogicalName>"), n.push("<a:Value>" + this.getValue().getValue() + "</a:Value>"), n.push("</b:value>"), t = n.join("")), t
}, Sdk.DateTime.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="c:dateTime">' + this.getValue().toISOString() + "</b:value>"), n
}, Sdk.Decimal.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="c:' + this.getType() + '">' + this.getValue() + "</b:value>"), n
}, Sdk.Double.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="c:' + this.getType() + '">' + this.getValue() + "</b:value>"), n
}, Sdk.Guid.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="e:guid">' + this.getValue() + "</b:value>"), n
}, Sdk.Int.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="c:' + this.getType() + '">' + this.getValue() + "</b:value>"), n
}, Sdk.Long.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="c:' + this.getType() + '">' + this.getValue() + "</b:value>"), n
}, Sdk.Lookup.prototype.toValueXml = function() {
    var t = '<b:value i:nil="true" />',
        n;
    return this.getValue() != null && (n = [], n.push('<b:value i:type="a:EntityReference">'), n.push("<a:Id>" + this.getValue().getId() + "</a:Id>"), n.push("<a:LogicalName>" + this.getValue().getType() + "</a:LogicalName>"), this.getValue().getName() == null ? n.push('<a:Name i:nil="true" />') : n.push("<a:Name>" + this.getValue().getName() + "</a:Name>"), n.push("</b:value>"), t = n.join("")), t
}, Sdk.Money.prototype.toValueXml = function() {
    var t = '<b:value i:nil="true" />',
        n;
    return this.getValue() != null && (n = [], n.push('<b:value i:type="a:Money">'), n.push("<a:Value>" + this.getValue() + "</a:Value>"), n.push("</b:value>"), t = n.join("")), t
}, Sdk.OptionSet.prototype.toValueXml = function() {
    var t = '<b:value i:nil="true" />',
        n;
    return this.getValue() != null && (n = [], n.push('<b:value i:type="a:OptionSetValue">'), n.push("<a:Value>" + this.getValue() + "</a:Value>"), n.push("</b:value>"), t = n.join("")), t
}, Sdk.PartyList.prototype.toValueXml = function() {
    var t = '<b:value i:nil="true" />',
        n;
    return this.getValue() != null && (n = [], n.push('<b:value i:type="a:EntityCollection">'), n.push(this.getValue().toValueXml()), n.push("</b:value>"), t = n.join("")), t
}, Sdk.String.prototype.toValueXml = function() {
    var n = '<b:value i:nil="true" />';
    return this.getValue() != null && (n = '<b:value i:type="c:' + this.getType() + '">' + Sdk.Xml.xmlEncode(this.getValue()) + "</b:value>"), n
}, Sdk.AttributeBase.prototype.toXml = function(n) {
    var t, i, r;
    return (typeof n == "string" && (n == "create" && (t = n), n == "update" && (t = n)), t == "update" && !this.getIsChanged()) ? "" : t == "create" && this.getValue() == null ? "" : typeof t == "undefined" && !this.isValueSet() ? "" : (i = this.toValueXml(), ["<a:KeyValuePairOfstringanyType>", "<b:key>" + this.getName() + "</b:key>", i, "</a:KeyValuePairOfstringanyType>"].join(""))
}, Sdk.ValueType.prototype = {
    boolean: "boolean",
    booleanManagedProperty: "booleanManagedProperty",
    dateTime: "dateTime",
    decimal: "decimal",
    double: "double",
    entityCollection: "entityCollection",
    entityReference: "entityReference",
    guid: "guid",
    int: "int",
    long: "long",
    money: "money",
    optionSet: "optionSet",
    string: "string"
}, Sdk.ValueType.boolean = "boolean", Sdk.ValueType.booleanManagedProperty = "booleanManagedProperty", Sdk.ValueType.dateTime = "dateTime", Sdk.ValueType.decimal = "decimal", Sdk.ValueType.double = "double", Sdk.ValueType.entityCollection = "entityCollection", Sdk.ValueType.entityReference = "entityReference", Sdk.ValueType.guid = "guid", Sdk.ValueType.int = "int", Sdk.ValueType.long = "long", Sdk.ValueType.money = "money", Sdk.ValueType.optionSet = "optionSet", Sdk.ValueType.string = "string", Sdk.ValueType.__enum = !0, Sdk.ValueType.__flags = !0, Sdk.AttributeCollection.prototype.forEach = function(n) {
    this.getAttributes().forEach(n)
}, Sdk.AttributeCollection.prototype.get = function(n) {
    if (n == null) return this.getAttributes();
    if (typeof n == "string") return this.getAttributeByName(n);
    if (typeof n == "number") return this.getAttributeByIndex(n);
    throw new Error("Sdk.AttributeCollection.get expects the args parameter to be Null, a String, or a Number");
}, Sdk.AttributeCollection.prototype.getAttributeByIndex = function(n) {
    var t = null;
    if (this.getAttributes().forEach(function(i, r) {
        if (n == r) {
            t == i;
            return
        }
    }), t != null) return t;
    throw new Error(Sdk.Util.format("Out of Range exception. There is no attribute at the index: {0}.", [n]));
}, Sdk.AttributeCollection.prototype.getAttributeByName = function(n) {
    var t = null;
    if (this.getAttributes().forEach(function(i) {
        if (i.getName() == n.toLowerCase()) {
            t = i;
            return
        }
    }), t != null) return t;
    throw new Error(Sdk.Util.format("An attribute named {0} does not exist in this entity.", [n]));
}, Sdk.AttributeCollection.prototype.getCount = function() {
    return this.getAttributes().getCount()
}, Sdk.AttributeCollection.prototype.getNames = function() {
    var n = [];
    return this.forEach(function(t) {
        n.push(t.getName())
    }), n
}, Sdk.AttributeCollection.prototype.toValueXml = function(n) {
    var t = [];
    return this.forEach(function(i) {
        t.push(i.toXml(n))
    }), t.join("")
}, Sdk.AttributeCollection.prototype.toXml = function(n) {
    var t = ["<a:Attributes>"];
    return t.push(this.toValueXml(n)), t.push("</a:Attributes>"), t.join("")
}, Sdk.AttributeAuditDetail.prototype = new Sdk.AuditDetail, Sdk.RelationshipAuditDetail.prototype = new Sdk.AuditDetail, Sdk.RolePrivilegeAuditDetail.prototype = new Sdk.AuditDetail, Sdk.ShareAuditDetail.prototype = new Sdk.AuditDetail, Sdk.AuditDetailCollection.prototype.getCount = function() {
    return this.getAuditDetails().getCount()
}, Sdk.AuditDetailCollection.prototype.getItem = function(n) {
    return this.getAuditDetails().getByIndex(n)
}, Sdk.AuditDetailCollection.prototype.add = function(n) {
    this.getAuditDetails().add(n)
}, Sdk.Collection.prototype.toValueXml = function() {
    var n = [];
    return this.forEach(function(t) {
        try {
            n.push(t.toXml())
        } catch (r) {
            throw new Error("Sdk.Collection.toValueXml error: " + r.message);
        }
    }), n.join("")
}, Sdk.Collection.prototype.toArrayOfValueXml = function(n) {
    var t = [];
    return this.forEach(function(i) {
        try {
            typeof i.toValueXml == "function" ? t.push(["<", n, ">", i.toValueXml(), "</", n, ">"].join("")) : t.push(["<", n, ">", i, "</", n, ">"].join(""))
        } catch (u) {
            throw new Error("Sdk.Collection.toArrayOfValueXml error: " + u.message);
        }
    }), t.join("")
}, Sdk.ColumnSet.prototype.removeColumn = function(n, t) {
    if (typeof n == "string") {
        var i, r = !1;
        if (this.getColumns().forEach(function(t) {
            t == n && (i = t, r = !0)
        }), r) this.getColumns().remove(i);
        else if (typeof t == "boolean" && t) throw new Error(Sdk.Util.format("A column named {0} was not found in the collection.", [n]));
    } else throw new Error("Sdk.ColumnSet.removeColumn method columnName parameter must be a string.");
}, Sdk.ColumnSet.prototype.toXml = function() {
    var n = [];
    return n.push("<d:columnSet>"), n.push(this.toValueXml()), n.push("</d:columnSet>"), n.join("")
}, Sdk.ColumnSet.prototype.toValueXml = function() {
    var n = [];
    return n.push("<a:AllColumns>" + this.getAllColumns() + "</a:AllColumns>"), this.getCount() == 0 ? n.push("<a:Columns />") : (n.push("<a:Columns>"), this.getColumns().forEach(function(t) {
        n.push("<f:string>" + t + "</f:string>")
    }), n.push("</a:Columns>")), n.join("")
}, Sdk.Entity.prototype.addAttribute = function(n, t) {
    this.getAttributes().add(n, t)
}, Sdk.Entity.prototype.addRelatedEntity = function(n, t) {
    this.getRelatedEntities().add(n, t)
}, Sdk.Entity.prototype.getIsChanged = function() {
    var n = !1;
    return (this.getAttributes().forEach(function(t) {
        t.getIsChanged() && (n = !0)
    }), n) ? !0 : this.getRelatedEntities().getIsChanged() ? !0 : (this.getRelatedEntities().getRelationshipEntities().forEach(function(t) {
        t.getEntityCollection().getEntities().forEach(function(t) {
            t.getIsChanged() && (n = !0)
        })
    }), n)
}, Sdk.Entity.prototype.setIsChanged = function(n) {
    this.getAttributes().forEach(function(t) {
        t.setIsChanged(n)
    }), this.getRelatedEntities().setIsChanged(n), this.getRelatedEntities().getRelationshipEntities().forEach(function(t) {
        t.getEntityCollection().getEntities().forEach(function(t) {
            t.setIsChanged(n)
        })
    })
}, Sdk.Entity.prototype.getValue = function(n, an) { /* modified by Carlton Colter 12/20/2016 - added an (allow null) to prevent throwing an error if value is not set */
    if (typeof n !== "string") throw new Error("Sdk.Entity.getValue logicalName parameter is required and must be a string.");
	if (an!==undefined && typeof an !== "boolean") throw new Error("Sdk.Entity.getValue AllowNull parameter is optional and must be a boolean.");
	if (an===undefined) an = false;
	var t = null;
    try {
		if (an && !this.hasAttribute(n)) 
		{
			t = null;
		} else {
			t = this.getAttributes(n).getValue();
		}
    } catch (i) {
        throw i;
    }
    return t;
}, Sdk.Entity.prototype.hasAttribute = function(n) { /* added 12-20-2016 by Carlton Colter */
    if (typeof n != "string") throw new Error("Sdk.Entity.getValue logicalName parameter is required and must be a string.");
    var t = null;
    try {
        t = (this.getAttributes().getNames().indexOf(n) !== -1);
    } catch (i) {
        throw i;
    }
    return t
}, Sdk.Entity.prototype.initializeSubClass = function(n) {
    var t, i;
    for (t in n) {
        try {
            i = this.getAttributes().get(t.toLowerCase())
        } catch (r) {
            i = new n[t].AttributeType(t.toLowerCase()), this.addAttribute(i, !1)
        }
        this[t] = i
    }
}, Sdk.Entity.prototype.setValue = function(n, t) {
    if (typeof n != "string") throw new Error("Sdk.Entity.setValue logicalName parameter is required and must be a string.");
    if (typeof t == "undefined") throw new Error("Sdk.Entity.setValue value parameter is required.");
    try {
        var i;
        i = this.getAttributes(n), i.setValue(t)
    } catch (r) {
        throw r;
    }
}, Sdk.Entity.prototype.toEntityReference = function() {
    if (this.getId() != null) return new Sdk.EntityReference(this.getType(), this.getId());
    throw new Error("Sdk.Entity.toEntityReference cannot be used on an entity that has not been saved.");
}, Sdk.Entity.prototype.toString = function() {
    return "[object Sdk.Entity " + this.getType() + "]"
}, Sdk.Entity.prototype.toValueXml = function(n) {
    var t = [];
    return t.push(this.getAttributes().toXml(n)), t.push(['<a:EntityState i:nil="true" />', "<a:FormattedValues />"].join("")), this.getId() == null ? t.push("<a:Id>00000000-0000-0000-0000-000000000000</a:Id>") : t.push("<a:Id>" + this.getId() + "</a:Id>"), t.push("<a:LogicalName>" + this.getType() + "</a:LogicalName>"), t.push(this.getRelatedEntities().toXml()), t.join("")
}, Sdk.Entity.prototype.toXml = function(n) {
    var t = ["<d:entity>"];
    return t.push(this.toValueXml(n)), t.push("</d:entity>"), t.join("")
}, Sdk.Entity.prototype.view = function() {
    var n = {}, t = this.getFormattedValues();
    return this.getAttributes().forEach(function(i, r) {
        var u, s = i.getName(),
            f, o = i.getType(),
            r, e, h;
        switch (o) {
            case "boolean":
            case "dateTime":
            case "guid":
            case "string":
            case "money":
            case "long":
            case "decimal":
            case "double":
            case "int":
            case "optionSet":
                f = i.getValue();
                break;
            case "booleanManagedProperty":
                f = {
                    canBeChanged: i.getValue().getCanBeChanged(),
                    managedPropertyLogicalName: i.getValue().getManagedPropertyLogicalName(),
                    value: i.getValue().getValue()
                };
                break;
            case "entityReference":
                i.getValue() == null ? (f = null, u = null) : (f = i.getValue().view(), u = i.getValue().getName());
                break;
            case "entityCollection":
                for (f = i.getValue().view(), u = "", r = 0; r < i.getValue().view().entities.length; r++) {
                    e = i.getValue().view().entities[r];
                    try {
                        u += "{" + e.attributes.partyid.fValue + "}"
                    } catch (c) {
                        u += "{" + e.logicalName + " id:" + e.id + "}"
                    }
                }
                break;
            default:
                throw new Error("Missing type " + o + " in Sdk.Entity.view");
        }
        if (typeof u == "undefined") try {
            h = t.getItem(s), u = h.getValue()
        } catch (c) {
            u = f
        }
        n[s] = {
            value: f,
            type: o,
            fValue: u
        }
    }), {
        attributes: n,
        entityState: this.getEntityState(),
        id: this.getId() == null ? null : this.getId(),
        logicalName: this.getType(),
        relatedEntities: this.getRelatedEntities().view()
    }
}, Sdk.EntityCollection.prototype.getCount = function() {
    return this.getEntities().getCount()
}, Sdk.EntityCollection.prototype.toValueXml = function() {
    var n = [];
    return n.push("<a:Entities>"), this.getEntities().forEach(function(t) {
        n.push(["<a:Entity>", t.toValueXml(), "</a:Entity>"].join(""))
    }), n.push("</a:Entities>"), this.getEntityName() == null ? n.push('<a:EntityName i:nil="true" />') : (n.push("<a:EntityName>"), n.push(this.getEntityName()), n.push("</a:EntityName>")), this.getMinActiveRowVersion() == null ? n.push('<a:MinActiveRowVersion i:nil="true" />') : (n.push("<a:MinActiveRowVersion>"), n.push(this.getMinActiveRowVersion()), n.push("</a:MinActiveRowVersion>")), n.push("<a:MoreRecords>" + this.getMoreRecords() + "</a:MoreRecords>"), this.getPagingCookie() == null ? n.push('<a:PagingCookie i:nil="true" />') : (n.push("<a:PagingCookie>"), n.push(this.getPagingCookie()), n.push("</a:PagingCookie>")), this.getTotalRecordCount() == null ? n.push('<a:TotalRecordCount i:nil="true" />') : n.push("<a:TotalRecordCount>" + this.getTotalRecordCount() + "</a:TotalRecordCount>"), this.getTotalRecordCountLimitExceeded() == null ? n.push('<a:TotalRecordCountLimitExceeded i:nil="true" />') : n.push("<a:TotalRecordCountLimitExceeded>" + this.getTotalRecordCountLimitExceeded() + "</a:TotalRecordCountLimitExceeded>"), n.join("")
}, Sdk.EntityCollection.prototype.view = function() {
    var n = [];
    return this.getEntities().forEach(function(t) {
        n.push(t.view())
    }), {
        entityName: this.getEntityName(),
        entities: n,
        minActiveRowVersion: this.getMinActiveRowVersion(),
        moreRecords: this.getMoreRecords(),
        pagingCookie: this.getPagingCookie(),
        totalRecordCount: this.getTotalRecordCount(),
        totalRecordCountLimitExceeded: this.getTotalRecordCountLimitExceeded()
    }
}, Sdk.EntityReference.prototype.toXml = function() {
    var n = [];
    return n.push("<a:EntityReference>"), n.push(this.toValueXml()), n.push("</a:EntityReference>"), n.join("")
}, Sdk.EntityReference.prototype.toValueXml = function() {
    var n = [];
    return n.push("<a:Id>" + this.getId() + "</a:Id>"), n.push("<a:LogicalName>" + this.getType() + "</a:LogicalName>"), this.getName() == null ? n.push('<a:Name i:nil="true" />') : n.push("<a:Name>" + this.getName() + "</a:Name>"), n.join("")
}, Sdk.EntityReference.prototype.view = function() {
    var n = {};
    return n.Id = this.getId(), n.Type = this.getType(), n.Name = this.getName(), n
}, Sdk.EntityReferenceCollection.prototype.add = function(n) {
    this.getEntityReferences().add(n)
}, Sdk.EntityReferenceCollection.prototype.remove = function(n) {
    var t = null;
    this.getEntityReferences().forEach(function(i) {
        i.getId() == n.getId() && i.getType() == n.getType() && (t = i)
    }), t != null && this.getEntityReferences().remove(t)
}, Sdk.EntityReferenceCollection.prototype.view = function() {
    var n = [];
    return this.getEntityReferences().forEach(function(t) {
        n.push(t.view())
    }), n
}, Sdk.EntityReferenceCollection.prototype.toValueXml = function() {
    var n = [];
    return this.getEntityReferences().forEach(function(t) {
        n.push(t.toXml())
    }), n.join("")
}, Sdk.EntityState.prototype = {
    Changed: "Changed",
    Created: "Created",
    Unchanged: "Unchanged"
}, Sdk.EntityState.Changed = "Changed", Sdk.EntityState.Created = "Created", Sdk.EntityState.Unchanged = "Unchanged", Sdk.EntityState.__enum = !0, Sdk.EntityState.__flags = !0, Sdk.FormattedValueCollection.prototype.getCount = function() {
    return this.getCollection().getCount()
}, Sdk.FormattedValueCollection.prototype.getItemByIndex = function(n) {
    return this.getCollection().getByIndex(n)
}, Sdk.FormattedValueCollection.prototype.getNames = function() {
    var n = [];
    return this.getCollection().forEach(function(t) {
        n.push(t.getName())
    }), n
}, Sdk.FormattedValueCollection.prototype.getValues = function() {
    var n = [];
    return this.getCollection().forEach(function(t) {
        n.push(t.getValue())
    }), n
}, Sdk.FormattedValueCollection.prototype.contains = function(n) {
    return this.getCollection().forEach(function(t) {
        if (n.getName() == t.getName() && n.getValue() == t.getValue() && n.getValueType() == t.getValueType()) return !0
    }), !1
}, Sdk.FormattedValueCollection.prototype.remove = function(n) {
    var t = null;
    this.getCollection().forEach(function(i) {
        i.getName() == n && (t = i)
    }), t != null && this.getFormattedValues().remove(t)
}, Sdk.FormattedValueCollection.prototype.clear = function() {
    this.getCollection().clear()
}, Sdk.FormattedValueCollection.prototype.forEach = function(n) {
    this.getCollection().forEach(n)
}, Sdk.PrincipalAccess.prototype.toValueXml = function() {
    if (this.getPrincipal() == null) throw new Error("Sdk.PrincipalAccess.toValueXml cannot be used when the Principal property is null.");
    return ["<g:AccessMask>", this.getAccessMaskAsString(), "</g:AccessMask>", "<g:Principal>", this.getPrincipal().toValueXml(), "</g:Principal>"].join("")
}, Sdk.PrivilegeDepth.prototype = {
    Basic: "Basic",
    Deep: "Deep",
    Global: "Global",
    Local: "Local"
}, Sdk.PrivilegeDepth.Basic = "Basic", Sdk.PrivilegeDepth.Deep = "Deep", Sdk.PrivilegeDepth.Global = "Global", Sdk.PrivilegeDepth.Local = "Local", Sdk.PrivilegeDepth.__enum = !0, Sdk.PrivilegeDepth.__flags = !0, Sdk.PropagationOwnershipOptions.prototype = {
    Caller: "Caller",
    ListMemberOwner: "ListMemberOwner",
    None: "None"
}, Sdk.PropagationOwnershipOptions.Caller = "Caller", Sdk.PropagationOwnershipOptions.ListMemberOwner = "ListMemberOwner", Sdk.PropagationOwnershipOptions.None = "None", Sdk.PropagationOwnershipOptions.__enum = !0, Sdk.PropagationOwnershipOptions.__flags = !0, Sdk.Query.ConditionExpression.prototype.toXml = function() {
    return ["<a:ConditionExpression>", this.toValueXml(), "</a:ConditionExpression>"].join("")
}, Sdk.Query.ConditionExpression.prototype.toValueXml = function() {
    var u = this.getAttributeName(),
        t, i, n, r;
    if (u == null) throw new Error("Sdk.Query.ConditionExpression AttributeName property must be set before a query can be performed.");
    if (t = this.getOperator(), t == null) throw new Error("Sdk.Query.ConditionExpression Operator property must be set before a query can be performed.");
    if (n = this.getValues(), n == null) i = "<a:Values />";
    else {
        var e = n.getValues(),
            o = n instanceof Sdk.Query.Dates,
            s = n instanceof Sdk.Query.EntityReferences,
            h = this.getValues().getType(),
            f = [];
        e.forEach(function(n) {
            var i;
            i = o ? n.toISOString() : s ? n.getId() : n.toString(), f.push(Sdk.Util.format('<f:anyType i:type="{0}">{1}</f:anyType>', [h, i]))
        }), i = ["<a:Values>", f.join(""), "</a:Values>"].join("")
    }
    return r = this.getEntityName(), ["<a:AttributeName>" + u + "</a:AttributeName>", "<a:Operator>" + t + "</a:Operator>", i, r == null ? '<a:EntityName i:nil="true" />' : "<a:EntityName>" + r + "</a:EntityName>"].join("")
}, Sdk.Query.ConditionOperator.prototype = {
    Equal: "Equal",
    NotEqual: "NotEqual",
    GreaterThan: "GreaterThan",
    LessThan: "LessThan",
    GreaterEqual: "GreaterEqual",
    LessEqual: "LessEqual",
    Like: "Like",
    NotLike: "NotLike",
    In: "In",
    NotIn: "NotIn",
    Between: "Between",
    NotBetween: "NotBetween",
    Null: "Null",
    NotNull: "NotNull",
    Yesterday: "Yesterday",
    Today: "Today",
    Tomorrow: "Tomorrow",
    Last7Days: "Last7Days",
    Next7Days: "Next7Days",
    LastWeek: "LastWeek",
    ThisWeek: "ThisWeek",
    NextWeek: "NextWeek",
    LastMonth: "LastMonth",
    ThisMonth: "ThisMonth",
    NextMonth: "NextMonth",
    On: "On",
    OnOrBefore: "OnOrBefore",
    OnOrAfter: "OnOrAfter",
    LastYear: "LastYear",
    ThisYear: "ThisYear",
    NextYear: "NextYear",
    LastXHours: "LastXHours",
    NextXHours: "NextXHours",
    LastXDays: "LastXDays",
    NextXDays: "NextXDays",
    LastXWeeks: "LastXWeeks",
    NextXWeeks: "NextXWeeks",
    LastXMonths: "LastXMonths",
    NextXMonths: "NextXMonths",
    LastXYears: "LastXYears",
    NextXYears: "NextXYears",
    EqualUserId: "EqualUserId",
    NotEqualUserId: "NotEqualUserId",
    EqualBusinessId: "EqualBusinessId",
    NotEqualBusinessId: "NotEqualBusinessId",
    Mask: "Mask",
    NotMask: "NotMask",
    Contains: "Contains",
    DoesNotContain: "DoesNotContain",
    EqualUserLanguage: "EqualUserLanguage",
    NotOn: "NotOn",
    OlderThanXMonths: "OlderThanXMonths",
    BeginsWith: "BeginsWith",
    DoesNotBeginWith: "DoesNotBeginWith",
    EndsWith: "EndsWith",
    DoesNotEndWith: "DoesNotEndWith",
    ThisFiscalYear: "ThisFiscalYear",
    ThisFiscalPeriod: "ThisFiscalPeriod",
    NextFiscalYear: "NextFiscalYear",
    NextFiscalPeriod: "NextFiscalPeriod",
    LastFiscalYear: "LastFiscalYear",
    LastFiscalPeriod: "LastFiscalPeriod",
    LastXFiscalYears: "LastXFiscalYears",
    LastXFiscalPeriods: "LastXFiscalPeriods",
    NextXFiscalYears: "NextXFiscalYears",
    NextXFiscalPeriods: "NextXFiscalPeriods",
    InFiscalYear: "InFiscalYear",
    InFiscalPeriod: "InFiscalPeriod",
    InFiscalPeriodAndYear: "InFiscalPeriodAndYear",
    InOrBeforeFiscalPeriodAndYear: "InOrBeforeFiscalPeriodAndYear",
    InOrAfterFiscalPeriodAndYear: "InOrAfterFiscalPeriodAndYear",
    EqualUserOrUserTeams: "EqualUserOrUserTeams",
    EqualUserTeams: "EqualUserTeams"
}, Sdk.Query.ConditionOperator.Equal = "Equal", Sdk.Query.ConditionOperator.NotEqual = "NotEqual", Sdk.Query.ConditionOperator.GreaterThan = "GreaterThan", Sdk.Query.ConditionOperator.LessThan = "LessThan", Sdk.Query.ConditionOperator.GreaterEqual = "GreaterEqual", Sdk.Query.ConditionOperator.LessEqual = "LessEqual", Sdk.Query.ConditionOperator.Like = "Like", Sdk.Query.ConditionOperator.NotLike = "NotLike", Sdk.Query.ConditionOperator.In = "In", Sdk.Query.ConditionOperator.NotIn = "NotIn", Sdk.Query.ConditionOperator.Between = "Between", Sdk.Query.ConditionOperator.NotBetween = "NotBetween", Sdk.Query.ConditionOperator.Null = "Null", Sdk.Query.ConditionOperator.NotNull = "NotNull", Sdk.Query.ConditionOperator.Yesterday = "Yesterday", Sdk.Query.ConditionOperator.Today = "Today", Sdk.Query.ConditionOperator.Tomorrow = "Tomorrow", Sdk.Query.ConditionOperator.Last7Days = "Last7Days", Sdk.Query.ConditionOperator.Next7Days = "Next7Days", Sdk.Query.ConditionOperator.LastWeek = "LastWeek", Sdk.Query.ConditionOperator.ThisWeek = "ThisWeek", Sdk.Query.ConditionOperator.NextWeek = "NextWeek", Sdk.Query.ConditionOperator.LastMonth = "LastMonth", Sdk.Query.ConditionOperator.ThisMonth = "ThisMonth", Sdk.Query.ConditionOperator.NextMonth = "NextMonth", Sdk.Query.ConditionOperator.On = "On", Sdk.Query.ConditionOperator.OnOrBefore = "OnOrBefore", Sdk.Query.ConditionOperator.OnOrAfter = "OnOrAfter", Sdk.Query.ConditionOperator.LastYear = "LastYear", Sdk.Query.ConditionOperator.ThisYear = "ThisYear", Sdk.Query.ConditionOperator.NextYear = "NextYear", Sdk.Query.ConditionOperator.LastXHours = "LastXHours", Sdk.Query.ConditionOperator.NextXHours = "NextXHours", Sdk.Query.ConditionOperator.LastXDays = "LastXDays", Sdk.Query.ConditionOperator.NextXDays = "NextXDays", Sdk.Query.ConditionOperator.LastXWeeks = "LastXWeeks", Sdk.Query.ConditionOperator.NextXWeeks = "NextXWeeks", Sdk.Query.ConditionOperator.LastXMonths = "LastXMonths", Sdk.Query.ConditionOperator.NextXMonths = "NextXMonths", Sdk.Query.ConditionOperator.LastXYears = "LastXYears", Sdk.Query.ConditionOperator.NextXYears = "NextXYears", Sdk.Query.ConditionOperator.EqualUserId = "EqualUserId", Sdk.Query.ConditionOperator.NotEqualUserId = "NotEqualUserId", Sdk.Query.ConditionOperator.EqualBusinessId = "EqualBusinessId", Sdk.Query.ConditionOperator.NotEqualBusinessId = "NotEqualBusinessId", Sdk.Query.ConditionOperator.Mask = "Mask", Sdk.Query.ConditionOperator.NotMask = "NotMask", Sdk.Query.ConditionOperator.Contains = "Contains", Sdk.Query.ConditionOperator.DoesNotContain = "DoesNotContain", Sdk.Query.ConditionOperator.EqualUserLanguage = "EqualUserLanguage", Sdk.Query.ConditionOperator.NotOn = "NotOn", Sdk.Query.ConditionOperator.OlderThanXMonths = "OlderThanXMonths", Sdk.Query.ConditionOperator.BeginsWith = "BeginsWith", Sdk.Query.ConditionOperator.DoesNotBeginWith = "DoesNotBeginWith", Sdk.Query.ConditionOperator.EndsWith = "EndsWith", Sdk.Query.ConditionOperator.DoesNotEndWith = "DoesNotEndWith", Sdk.Query.ConditionOperator.ThisFiscalYear = "ThisFiscalYear", Sdk.Query.ConditionOperator.ThisFiscalPeriod = "ThisFiscalPeriod", Sdk.Query.ConditionOperator.NextFiscalYear = "NextFiscalYear", Sdk.Query.ConditionOperator.NextFiscalPeriod = "NextFiscalPeriod", Sdk.Query.ConditionOperator.LastFiscalYear = "LastFiscalYear", Sdk.Query.ConditionOperator.LastFiscalPeriod = "LastFiscalPeriod", Sdk.Query.ConditionOperator.LastXFiscalYears = "LastXFiscalYears", Sdk.Query.ConditionOperator.LastXFiscalPeriods = "LastXFiscalPeriods", Sdk.Query.ConditionOperator.NextXFiscalYears = "NextXFiscalYears", Sdk.Query.ConditionOperator.NextXFiscalPeriods = "NextXFiscalPeriods", Sdk.Query.ConditionOperator.InFiscalYear = "InFiscalYear", Sdk.Query.ConditionOperator.InFiscalPeriod = "InFiscalPeriod", Sdk.Query.ConditionOperator.InFiscalPeriodAndYear = "InFiscalPeriodAndYear", Sdk.Query.ConditionOperator.InOrBeforeFiscalPeriodAndYear = "InOrBeforeFiscalPeriodAndYear", Sdk.Query.ConditionOperator.InOrAfterFiscalPeriodAndYear = "InOrAfterFiscalPeriodAndYear", Sdk.Query.ConditionOperator.EqualUserOrUserTeams = "EqualUserOrUserTeams", Sdk.Query.ConditionOperator.EqualUserTeams = "EqualUserTeams", Sdk.Query.ConditionOperator.__enum = !0, Sdk.Query.ConditionOperator.__flags = !0, Sdk.Query.FetchExpression.prototype = new Sdk.Query.QueryBase("FetchExpression"), Sdk.Query.FetchExpression.prototype.toValueXml = function() {
    return "<a:Query>" + Sdk.Xml.xmlEncode(this.getFetchXml()) + "</a:Query>"
}, Sdk.Query.FilterExpression.prototype.toXml = function() {
    return ["<a:FilterExpression>", this.toValueXml(), "</a:FilterExpression>"].join("")
}, Sdk.Query.FilterExpression.prototype.toValueXml = function() {
    var t = "",
        i, n, r, u;
    return this.getConditions().getCount() == 0 ? t = "<a:Conditions />" : (i = [], this.getConditions().forEach(function(n) {
        i.push(n.toXml())
    }), t = ["<a:Conditions>", i.join(""), "</a:Conditions>"].join("")), n = "", this.getFilters().getCount() == 0 ? n = "<a:Filters />" : (r = [], this.getFilters().forEach(function(n) {
        r.push(n.toXml())
    }), n = ["<a:Filters>", r.join(""), "</a:Filters>"].join("")), u = "", this.getIsQuickFindFilter() && (u = "<a:IsQuickFindFilter>true</a:IsQuickFindFilter>"), [t, "<a:FilterOperator>" + this.getFilterOperator() + "</a:FilterOperator>", n, u].join("")
}, Sdk.Query.JoinOperator.prototype = {
    Inner: "Inner",
    LeftOuter: "LeftOuter",
    Natural: "Natural"
}, Sdk.Query.JoinOperator.Inner = "Inner", Sdk.Query.JoinOperator.LeftOuter = "LeftOuter", Sdk.Query.JoinOperator.Natural = "Natural", Sdk.Query.JoinOperator.__enum = !0, Sdk.Query.JoinOperator.__flags = !0, Sdk.Query.LinkEntity.prototype.toXml = function() {
    return ["<a:LinkEntity>", this.toValueXml(), "</a:LinkEntity>"].join("")
}, Sdk.Query.LinkEntity.prototype.toValueXml = function() {
    var i, r, n, u, t, f, e, o;
    return i = this.getColumns().getCount() == 0 ? "<a:Columns />" : ["<a:Columns>", this.getColumns().toValueXml(), "</a:Columns>"].join(""), r = "<a:LinkCriteria />", this.getLinkCriteria() != null && (n = "", this.getLinkCriteria().getConditions().getCount() == 0 ? n = "<a:Conditions />" : (u = [], this.getLinkCriteria().getConditions().forEach(function(n) {
        u.push(n.toXml())
    }), n = ["<a:Conditions>", u.join(""), "</a:Conditions>"].join("")), t = "", this.getLinkCriteria().getFilters().getCount() == 0 ? t = "<a:Filters />" : (f = [], this.getLinkCriteria().getFilters().forEach(function(n) {
        f.push(n.toXml())
    }), t = ["<a:Filters>", f.join(""), "</a:Filters>"].join("")), r = ["<a:LinkCriteria>", n, "<a:FilterOperator>" + this.getLinkCriteria().getFilterOperator() + "</a:FilterOperator>", t, "</a:LinkCriteria>"].join("")), e = "<a:LinkEntities />", this.getLinkEntities().getCount() > 0 && (o = [], this.getLinkEntities().forEach(function(n) {
        o.push(n.toXml())
    }), e = ["<a:LinkEntities>", o.join(""), "</a:LinkEntities>"].join("")), [i, this.getEntityAlias() == null ? '<a:EntityAlias i:nil="true" />' : "<a:EntityAlias>" + this.getEntityAlias() + "</a:EntityAlias>", "<a:JoinOperator>" + this.getJoinOperator() + "</a:JoinOperator>", r, e, "<a:LinkFromAttributeName>" + this.getLinkFromAttributeName() + "</a:LinkFromAttributeName>", "<a:LinkFromEntityName>" + this.getLinkFromEntityName() + "</a:LinkFromEntityName>", "<a:LinkToAttributeName>" + this.getLinkToAttributeName() + "</a:LinkToAttributeName>", "<a:LinkToEntityName>" + this.getLinkToEntityName() + "</a:LinkToEntityName>"].join("")
}, Sdk.Query.LogicalOperator.prototype = {
    And: "And",
    Or: "Or"
}, Sdk.Query.LogicalOperator.And = "And", Sdk.Query.LogicalOperator.Or = "Or", Sdk.Query.LogicalOperator.__enum = !0, Sdk.Query.LogicalOperator.__flags = !0, Sdk.Query.OrderExpression.prototype.toXml = function() {
    return ["<a:OrderExpression>", this.toValueXml(), "</a:OrderExpression>"].join("")
}, Sdk.Query.OrderExpression.prototype.toValueXml = function() {
    return ["<a:AttributeName>" + this.getAttributeName() + "</a:AttributeName>", "<a:OrderType>" + this.getOrderType() + "</a:OrderType>"].join("")
}, Sdk.Query.OrderType.prototype = {
    Ascending: "Ascending",
    Descending: "Descending"
}, Sdk.Query.OrderType.Ascending = "Ascending", Sdk.Query.OrderType.Descending = "Descending", Sdk.Query.OrderType.__enum = !0, Sdk.Query.OrderType.__flags = !0, Sdk.Query.PagingInfo.prototype.toXml = function() {
    return ["<a:PageInfo>", this.toValueXml(), "</a:PageInfo>"].join("")
}, Sdk.Query.PagingInfo.prototype.toValueXml = function() {
    return ["<a:Count>" + this.getCount() + "</a:Count>", "<a:PageNumber>" + this.getPageNumber() + "</a:PageNumber>", this.getPagingCookie() == null ? '<a:PagingCookie i:nil="true" />' : "<a:PagingCookie>" + this.getPagingCookie() + "</a:PagingCookie>", "<a:ReturnTotalRecordCount>" + this.getReturnTotalRecordCount() + "</a:ReturnTotalRecordCount>"].join("")
}, Sdk.Query.QueryByAttribute.prototype = new Sdk.Query.QueryBase("QueryByAttribute"), Sdk.Query.QueryByAttribute.prototype.addAttributeValue = function(n) {
    if (n instanceof Sdk.AttributeBase) this.getAttributeValues().add(n);
    else throw new Error("Sdk.Query.QueryByAttribute addAttributeValue method attributeValue parameter must be an Sdk.AttributeBase.");
}, Sdk.Query.QueryByAttribute.prototype.addColumn = function(n) {
    var t = "Sdk.Query.QueryByAttribute addColumn ";
    if (typeof n == "string") try {
        this.getColumnSet().getColumns().add(n)
    } catch (i) {
        throw new Error(t + "Error: " + i.message);
    } else throw new Error(t + "columnName parameter must be a String.");
}, Sdk.Query.QueryByAttribute.prototype.addOrder = function(n) {
    if (n instanceof Sdk.Query.OrderExpression) this.getOrders().add(n);
    else throw new Error("Sdk.Query.QueryByAttribute addOrder method order parameter must be an Sdk.Query.OrderExpression.");
}, Sdk.Query.QueryByAttribute.prototype.removeAttributeValue = function(n, t) {
    if (n instanceof Sdk.AttributeBase) {
        var i, r = !1;
        if (this.getAttributeValues().forEach(function(t) {
            t.getName() == n.getName() && t.getType() == n.getType() && (i = t, r = !0)
        }), r) this.getAttributeValues().remove(i);
        else if (typeof t == "boolean" && t) throw new Error(Sdk.Util.format("An attribute named {0} of type {1} was not found in the collection.", [n.getName(), n.getType()]));
    } else throw new Error("Sdk.Query.QueryByAttribute.removeAttributeValue method attributeValue parameter must be one of the classes that inherit from Sdk.AttributeBase. ");
}, Sdk.Query.QueryByAttribute.prototype.removeColumn = function(n, t) {
    this.getColumnSet().removeColumn(n, t)
}, Sdk.Query.QueryByAttribute.prototype.toXml = function() {
    return ['<query i:type="a:QueryByAttribute">', this.toValueXml(), "</query>"].join("")
}, Sdk.Query.QueryByAttribute.prototype.toValueXml = function() {
    var u = "",
        f, t, i, r, n, e;
    return this.getOrders().getCount() == 0 ? u = "<a:Orders />" : (f = [], this.getOrders().forEach(function(n) {
        f.push(n.toXml())
    }), u = ["<a:Orders>", f.join(""), "</a:Orders>"].join("")), t = "", this.getAttributeValues().getCount() == 0 ? t = "<a:Attributes />" : (i = ["<a:Attributes>"], this.getAttributeValues().forEach(function(n) {
        i.push("<f:string>" + n.getName() + "</f:string>")
    }), i.push("</a:Attributes>"), t = i.join("")), r = "", this.getAttributeValues().getCount() == 0 ? r = "<a:Values />" : (n = ["<a:Values>"], this.getAttributeValues().forEach(function(t) {
        var e = t.getType(),
            u = e,
            f = "c",
            r = t.getValue();
        if (r == null) n.push('<f:anyType i:nil="true" />');
        else {
            switch (e) {
                case "boolean":
                case "decimal":
                case "double":
                case "int":
                case "long":
                case "string":
                    break;
                case "booleanManagedProperty":
                    u = "boolean", r = r.getValue();
                    break;
                case "dateTime":
                    r = r.toISOString();
                    break;
                case "entityCollection":
                    throw new error("Entity Collection type not implemented in Sdk.Query.QueryByAttribute");
                    break;
                case "entityReference":
                    u = "guid", f = "e", r = r.getId();
                    break;
                case "guid":
                    f = "e";
                    break;
                case "money":
                    u = "decimal";
                    break;
                case "optionSet":
                    u = "int";
                    break;
                default:
                    throw new Error("Unexpected type found in Sdk.Query.QueryByAttribute.toValueXml");
            }
            n.push(Sdk.Util.format('<f:anyType i:type="{0}:{1}">{2}</f:anyType>', [f, u, r.toString()]))
        }
    }), n.push("</a:Values>"), r = n.join("")), e = '<a:ColumnSet i:nil="true" />', this.getColumnSet().getCount() > 0 && (e = ["<a:ColumnSet>", this.getColumnSet().toValueXml(), "</a:ColumnSet>"].join("")), [t, e, "<a:EntityName>" + this.getEntityName() + "</a:EntityName>", u, r, this.getPageInfo() == null ? "" : this.getPageInfo().toXml(), this.getTopCount() == null ? "" : "<a:TopCount>" + this.getTopCount() + "</a:TopCount>"].join("")
}, Sdk.Query.QueryExpression.prototype = new Sdk.Query.QueryBase("QueryExpression"), Sdk.Query.QueryExpression.prototype.addColumn = function(n) {
    if (typeof n == "string") try {
        this.getColumnSet().addColumn(n)
    } catch (t) {
        throw new Error("Sdk.Query.QueryExpression addColumn error: " + t.message);
    } else throw new Error("Sdk.Query.QueryExpression addColumn method columnName parameter must be a String.");
}, Sdk.Query.QueryExpression.prototype.addCondition = function(n, t, i, r) {
    this.getCriteria().addCondition(n, t, i, r)
}, Sdk.Query.QueryExpression.prototype.addLink = function(n, t, i, r) {
    if (n instanceof Sdk.Query.LinkEntity) this.getLinkEntities().add(n);
    else if (typeof n == "string" && typeof t == "string" && typeof i == "string")
        if (r == null) this.getLinkEntities().add(new Sdk.Query.LinkEntity(this.getEntityName(), n, t, i, Sdk.Query.JoinOperator.Inner));
        else if (r == Sdk.Query.JoinOperator.Inner || Sdk.Query.JoinOperator.LeftOuter || Sdk.Query.JoinOperator.Natural) this.getLinkEntities().add(new Sdk.Query.LinkEntity(this.getEntityName(), n, t, i, r));
    else throw new Error("Sdk.Query.QueryExpression addLink method requires that the joinOperator parameter is an Sdk.Query.JoinOperator value");
    else throw new Error("Sdk.Query.QueryExpression addLink method requires that the linkToEntityName, linkFromAttributeName, and linkToAttributeName parameters are string values");
}, Sdk.Query.QueryExpression.prototype.addOrder = function(n, t) {
    if (typeof n == "string")
        if (t == null) this.getOrders().add(new Sdk.Query.OrderExpression(n));
        else if (t == Sdk.Query.OrderType.Ascending || t == Sdk.Query.OrderType.Descending) this.getOrders().add(new Sdk.Query.OrderExpression(n, t));
    else throw new Error("Sdk.Query.QueryExpression addOrder method requires that the orderType parameter is an Sdk.Query.OrderType value");
    else throw new Error("Sdk.Query.QueryExpression addOrder method requires that the attributeName parameter is a string value");
}, Sdk.Query.QueryExpression.prototype.toXml = function() {
    return ['<query i:type="a:QueryExpression">', this.toValueXml(), "</query>"].join("")
}, Sdk.Query.QueryExpression.prototype.toValueXml = function() {
    var t = "",
        i, n, r;
    return this.getLinkEntities().getCount() == 0 ? t = "<a:LinkEntities />" : (i = [], this.getLinkEntities().forEach(function(n) {
        i.push(n.toXml())
    }), t = ["<a:LinkEntities>", i.join(""), "</a:LinkEntities>"].join("")), n = "", this.getOrders().getCount() == 0 ? n = "<a:Orders />" : (r = [], this.getOrders().forEach(function(n) {
        r.push(n.toXml())
    }), n = ["<a:Orders>", r.join(""), "</a:Orders>"].join("")), ["<a:ColumnSet>", this.getColumnSet().toValueXml(), "</a:ColumnSet>", ["<a:Criteria>", this.getCriteria().toValueXml(), "</a:Criteria>"].join(""), "<a:Distinct>" + this.getDistinct() + "</a:Distinct>", "<a:EntityName>" + this.getEntityName() + "</a:EntityName>", t, n, this.getPageInfo() == null ? "" : this.getPageInfo().toXml(), "<a:NoLock>" + this.getNoLock() + "</a:NoLock>", this.getTopCount() == null ? "" : "<a:TopCount>" + this.getTopCount() + "</a:TopCount>"].join("")
}, Sdk.Query.Booleans.prototype = new Sdk.Query.ValueBase, Sdk.Query.BooleanManagedProperties.prototype = new Sdk.Query.ValueBase, Sdk.Query.Dates.prototype = new Sdk.Query.ValueBase, Sdk.Query.Decimals.prototype = new Sdk.Query.ValueBase, Sdk.Query.Doubles.prototype = new Sdk.Query.ValueBase, Sdk.Query.EntityReferences.prototype = new Sdk.Query.ValueBase, Sdk.Query.Guids.prototype = new Sdk.Query.ValueBase, Sdk.Query.Ints.prototype = new Sdk.Query.ValueBase, Sdk.Query.Longs.prototype = new Sdk.Query.ValueBase, Sdk.Query.Money.prototype = new Sdk.Query.ValueBase, Sdk.Query.OptionSets.prototype = new Sdk.Query.ValueBase, Sdk.Query.Strings.prototype = new Sdk.Query.ValueBase, Sdk.RelatedEntitiesCollection.prototype.add = function(n, t) {
    var i, u, r;
    if (n == null || typeof n != "string") throw new Error("Sdk.RelatedEntitiesCollection add method relationshipSchemaName parameter is required and must be a string.");
    if (t == null || !(t instanceof Sdk.Entity)) throw new Error("Sdk.RelatedEntitiesCollection add method entity parameter is required and must be an Sdk.Entity.");
    i = !1, u = this, this.getRelationshipEntities().forEach(function(r) {
        r.getRelationship() == n && (i = !0, r.getEntityCollection().addEntity(t), u.setIsChanged(!0))
    }), i || (r = new Sdk.RelationshipEntityCollection(n), r.getEntityCollection().addEntity(t), this.getRelationshipEntities().add(r), this.setIsChanged(!0))
}, Sdk.RelatedEntitiesCollection.prototype.addRelationship = function(n) {
    if (typeof n == "string") this.getRelationshipEntities().add(new Sdk.RelationshipEntityCollection(n)), this.setIsChanged(!0);
    else throw new Error("Sdk.RelatedEntitiesCollection.addRelationship method relationshipSchemaName parameter must be a string.");
}, Sdk.RelatedEntitiesCollection.prototype.addRelationshipEntityCollection = function(n) {
    if (!(n instanceof Sdk.RelationshipEntityCollection)) throw new Error("Sdk.RelatedEntitiesCollection addRelationshipEntityCollection method relationshipEntityCollection parameter is required and must be an Sdk.RelationshipEntityCollection.");
    var t = !1;
    this.getRelationshipEntities().forEach(function(i) {
        i.getRelationship() == n.getRelationship() && (t = !0, n.getEntityCollection().forEach(function(n) {
            i.getEntityCollection().add(n), this.setIsChanged(!0)
        }))
    }), t || (this.getRelationshipEntities().add(n), this.setIsChanged(!0))
}, Sdk.RelatedEntitiesCollection.prototype.clear = function() {
    this.getRelationshipEntities().clear(), this.setIsChanged(!0)
}, Sdk.RelatedEntitiesCollection.prototype.removeRelationship = function(n) {
    if (typeof n == "string") {
        var t = null;
        this.getRelationshipEntities().forEach(function(i) {
            i.getRelationship() == n && (t = i)
        }), t != null && (this.getRelationshipEntities().remove(t), this.setIsChanged(!0))
    } else throw new Error("Sdk.RelatedEntitiesCollection.removeRelationship method relationshipSchemaName parameter must be a string.");
}, Sdk.RelatedEntitiesCollection.prototype.toXml = function() {
    if (this.getRelationshipEntities().getCount() == 0) return "<a:RelatedEntities />";
    var n = ["<a:RelatedEntities>"];
    return this.getRelationshipEntities().forEach(function(t) {
        n.push(t.toXml())
    }), n.push("</a:RelatedEntities>"), n.join("")
}, Sdk.RelatedEntitiesCollection.prototype.view = function() {
    var n = {};
    return this.getRelationshipEntities().forEach(function(t) {
        n[t.getRelationship()] = t.getEntityCollection().view()
    }), n
}, Sdk.RelatedEntitiesCollection.prototype.getRelatedEntitiesByRelationshipName = function(n) {
    if (typeof n == "string") {
        var t = new Sdk.EntityCollection;
        return this.getRelationshipEntities().forEach(function(i) {
            i.getRelationship() == n && (t = i.getEntityCollection())
        }), t
    }
    throw new Error("Sdk.RelatedEntitiesCollection.getRelatedEntitiesByRelationshipName method relationshipSchemaName parameter must be a string.");
}, Sdk.RelationshipEntityCollection.prototype.toXml = function() {
    var n = ["<a:KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN>", "<b:key>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", this.getRelationship(), "</a:SchemaName>", "</b:key>", "<b:value>", this.getEntityCollection().toValueXml(), "</b:value>", "</a:KeyValuePairOfRelationshipEntityCollectionX_PsK4FkN>"];
    return n.join("")
}, Sdk.RelationshipQuery.prototype.toXml = function() {
    var n = ["<a:KeyValuePairOfRelationshipQueryBaseX_PsK4FkN>", this.toValueXml(), "</a:KeyValuePairOfRelationshipQueryBaseX_PsK4FkN>"];
    return n.join("")
}, Sdk.RelationshipQuery.prototype.toValueXml = function() {
    var n = "QueryExpression";
    return this.getQuery() instanceof Sdk.Query.FetchExpression && (n = "FetchExpression"), this.getQuery() instanceof Sdk.Query.QueryByAttribute && (n = "QueryByAttribute"), ["<b:key>", '<a:PrimaryEntityRole i:nil="true" />', "<a:SchemaName>", this.getRelationshipName(), "</a:SchemaName>", "</b:key>", '<b:value i:type="a:', n, '">', this.getQuery().toValueXml(), "</b:value>"].join("")
}, Sdk.RelationshipQueryCollection.prototype.add = function(n) {
    this.getRelationshipQueries().add(n)
}, Sdk.RelationshipQueryCollection.prototype.toValueXml = function() {
    var n = [];
    return this.getRelationshipQueries().forEach(function(t) {
        n.push(t.toXml())
    }), n.join("")
}, Sdk.RolePrivilege.prototype.toXml = function() {
    return ["<g:RolePrivilege>", this.toValueXml(), "</g:RolePrivilege>"].join("")
}, Sdk.RolePrivilege.prototype.toValueXml = function() {
    return ["<g:BusinessUnitId>", this.getBusinessId(), "</g:BusinessUnitId>", "<g:Depth>", this.getDepth(), "</g:Depth>", "<g:PrivilegeId>", this.getPrivilegeId(), "</g:PrivilegeId>"].join("")
}, Sdk.RolePrivilege.prototype.view = function() {
    var n = {};
    return n.BusinessId = this.getBusinessId(), n.Depth = this.getDepth(), n.PrivilegeId = this.getPrivilegeId(), n
}, Sdk.RollupType.prototype = {
    Extended: "Extended",
    None: "None",
    Related: "Related"
}, Sdk.RollupType.All = "All", Sdk.RollupType.Extended = "Extended", Sdk.RollupType.None = "None", Sdk.RollupType.Related = "Related", Sdk.RollupType.__enum = !0, Sdk.RollupType.__flags = !0, Sdk.TargetFieldType.prototype = {
    All: "All",
    ValidForCreate: "ValidForCreate",
    ValidForRead: "ValidForRead",
    ValidForUpdate: "ValidForUpdate"
}, Sdk.TargetFieldType.All = "All", Sdk.TargetFieldType.ValidForCreate = "ValidForCreate", Sdk.TargetFieldType.ValidForRead = "ValidForRead", Sdk.TargetFieldType.ValidForUpdate = "ValidForUpdate", Sdk.TargetFieldType.__enum = !0, Sdk.TargetFieldType.__flags = !0, Sdk.TimeInfo.prototype.view = function() {
    var n = {};
    return n.ActivityStatusCode = this.getActivityStatusCode(), n.CalendarId = this.getCalendarId(), n.DisplayText = this.getDisplayText(), n.Effort = this.getEffort(), n.End = this.getEnd(), n.IsActivity = this.getIsActivity(), n.SourceId = this.getSourceId(), n.SourceTypeCode = this.getSourceTypeCode(), n.Start = this.getStart(), n.SubCode = this.getSubCode(), n.TimeCode = this.getTimeCode(), n
}, Sdk.SubCode.prototype = {
    Appointment: "Appointment",
    Break: "Break",
    Committed: "Committed",
    Holiday: "Holiday",
    ResourceCapacity: "ResourceCapacity",
    ResourceServiceRestriction: "ResourceServiceRestriction",
    ResourceStartTime: "ResourceStartTime",
    Schedulable: "Schedulable",
    ServiceCost: "ServiceCost",
    ServiceRestriction: "ServiceRestriction",
    Uncommitted: "Uncommitted",
    Unspecified: "Unspecified",
    Vacation: "Vacation"
}, Sdk.SubCode.Appointment = "Appointment", Sdk.SubCode.Break = "Break", Sdk.SubCode.Committed = "Committed", Sdk.SubCode.Holiday = "Holiday", Sdk.SubCode.ResourceCapacity = "ResourceCapacity", Sdk.SubCode.ResourceServiceRestriction = "ResourceServiceRestriction", Sdk.SubCode.ResourceStartTime = "ResourceStartTime", Sdk.SubCode.Schedulable = "Schedulable", Sdk.SubCode.ServiceCost = "ServiceCost", Sdk.SubCode.ServiceRestriction = "ServiceRestriction", Sdk.SubCode.Uncommitted = "Uncommitted", Sdk.SubCode.Unspecified = "Unspecified", Sdk.SubCode.Vacation = "Vacation", Sdk.SubCode.__enum = !0, Sdk.SubCode.__flags = !0, Sdk.TimeCode.prototype = {
    Available: "Available",
    Busy: "Busy",
    Filter: "Filter",
    Unavailable: "Unavailable"
}, Sdk.TimeCode.Available = "Available", Sdk.TimeCode.Busy = "Busy", Sdk.TimeCode.Filter = "Filter", Sdk.TimeCode.Unavailable = "Unavailable", Sdk.TimeCode.__enum = !0, Sdk.TimeCode.__flags = !0, Sdk.TraceInfo.prototype.view = function() {
    var n = {};
    return n.ErrorInfoList = [], this.getErrorInfoList().forEach(function(t) {
        n.ErrorInfoList.push(t.view())
    }), n
}, Sdk.ErrorInfo.prototype.view = function() {
    var n = {};
    return n.ErrorCode = this.getErrorCode(), n.ResourceList = [], this.getResourceList().forEach(function(t) {
        n.ResourceList.push(t.view())
    }), n
}, Sdk.ResourceInfo.prototype.view = function() {
    var n = {};
    return n.DisplayName = this.getDisplayName(), n.EntityName = this.getEntityName(), n.Id = this.getId(), n
}, Sdk.ValidationResult.prototype.view = function() {
    var n = {}, t;
    return n.ActivityId = this.getActivityId(), t = this.getTraceInfo(), n.TraceInfo = t == null ? null : t.view(), n.ValidationSuccess = this.getValidationSuccess(), n
}, Date.prototype.toISOString || function() {
    function n(n) {
        return n < 10 ? "0" + n : n
    }
    Date.prototype.toISOString = function() {
        return [this.getUTCFullYear(), "-", n(this.getUTCMonth() + 1), "-", n(this.getUTCDate()), "T", n(this.getUTCHours()), ":", n(this.getUTCMinutes()), ":", n(this.getUTCSeconds()), ".", (this.getUTCMilliseconds() / 1e3).toFixed(3).slice(2, 5), "Z"].join("")
    }
}();