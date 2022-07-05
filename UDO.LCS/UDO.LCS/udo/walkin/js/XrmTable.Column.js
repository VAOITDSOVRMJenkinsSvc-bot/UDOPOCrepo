"use strict";
var XrmTable = XrmTable || {};

(function ($, Sdk) {
    XrmTable.Column = function () {
        var u = "XrmTable.Column constructor parameter can accept an array of strings.";
        var _key,
            _sortKey = "",
            _name,
            _render,
            _renderType = "column",
            _tooltip,
            _sortCondition,
            _sortIndex,
            _class,
            _renderIndex = -1,
            _visible = true,
            _srname;

        function render(data, rows, col, table) {
            // colspan should be included.
            if (_render && typeof _render === "function") {
                return _render(data, rows, col, table);
            }

            if (_key === null) {
                return "";
            }
            var k = _key;

            if (data instanceof Sdk.Entity) {
                data = data.view().attributes;
            }

            if (!data.hasOwnProperty(k)) {
                k = k.toLowerCase();
            }
            if (data.hasOwnProperty(k)) {
                if (data[k].hasOwnProperty("fValue")) {
                    return data[k].fValue;
                } else if (data[k].hasOwnProperty("value")) {
                    return data[k].value;
                } else {
                    return data[k];
                }
            }
            return "";
        }

        if (!(this instanceof XrmTable.Column)) {
            if (1 === arguments.length) {
                return new XrmTable.Column(arguments[0]);
            }
            if (2 === arguments.length) {
                return new XrmTable.Column({
                    name: arguments[0],
                    value: arguments[1]
                });
            }
            throw new TypeError(u);
        }

        if (arguments.length === 1) {
            var has = function (a, n, t) {
                return a.hasOwnProperty(n) && typeof a[n] === t;
            };
            var a = arguments[0];

            if (has(a, "name", "string") && has(a, "key", "string")) {
                _name = a.name;
                _key = a.key;
                if (has(a, "render", "function")) {
                    _render = a.render;
                }
                if (has(a, "sortkey", "string")) {
                    _sortKey = a.sortkey;
                }
                if (has(a, "type", "string")) {
                    _renderType = a.type;
                }
                if (has(a, "tooltip", "string")) {
                    _tooltip = a.tooltip;
                }
                if (has(a, "srname", "string")) {
                    _srname = a.srname;
                }
                if (has(a, "class", "string")) {
                    _class = a.class;
                }
                if (has(a, "sortcondition", "string")) {
                    switch (a.sortcondition) {
                        case XrmTable.Constants.SortType.ASCENDING:
                        case XrmTable.Constants.SortType.DESCENDING:
                        case XrmTable.Constants.SortType.NONE:
                            _sortCondition = a.sortcondition;
                            break;
                        default:
                            _sortCondition = XrmTable.Constants.SortType.NONE;
                            break;
                    }
                }
                if (has(a, "sortindex", "string")) {
                    _sortIndex = parseInt(a.sortindex);
                } else if (has(a, "sortindex", "number")) {
                    _sortIndex = a.sortindex;
                } else {
                    _sortIndex = -1;
                }

            } else if (has(a, "getName", "function") && has(a, "getKey", "function")) {
                _name = a.getName();
                _key = a.getKey();
                if (has(a, "getRender", "function")) {
                    _render = a.getRender();
                }
                if (has(a, "getSortKey", "function")) {
                    _sortKey = a.getSortKey();
                }
                if (has(a, "getRenderType", "function")) {
                    _renderType = a.getRenderType();
                }
                if (has(a, "getTooltip", "function")) {
                    _tooltip = a.getTooltip();
                }
                if (has(a, "getClass", "function")) {
                    _class = a.getClass();
                }
                if (has(a, "getSortCondition", "function")) {
                    _sortCondition = a.getSortCondition();
                }
                if (has(a, "getSortIndex", "function")) {
                    _sortIndex = a.getSortIndex();
                }
                if (has(a, "getSRName", "function")) {
                    _srname = a.getSRName();
                }
            }

            if (_sortKey === "") {
                _sortKey = _key;
            }
        }

        this.getSRName = function () {
            if (_srname && _srname != "") return _srname;
            if (_tooltip && _tooltip != "") return _tooltip;
            return _name;
        };
        this.setSRName = function (v) {
            _srname = v;
        };
        this.getTooltip = function () {
            if (_tooltip === null) return _name;
            return _tooltip;
        };
        this.setTooltip = function (v) {
            _tooltip = v;
        };
        this.getCanSort = function () {
            if (_sortKey) return true;
            return false;
        };
        this.getSortIndex = function () {
            return _sortIndex;
        };
        this.setSortIndex = function (v) {
            _sortIndex = v;
        };
        this.getSortCondition = function () {
            if (_sortCondition) return _sortCondition;
            return XrmTable.Constants.SortType.NONE;
        };
        this.setSortCondition = function (v) {
            _sortCondition = v;
        };
        this.getSortKey = function () {
            return _sortKey;
        };
        this.setSortKey = function (v) {
            _sortKey = v;
        };
        this.getKey = function () {
            return _key;
        };
        this.setKey = function (v) {
            _key = v;
        };
        this.getName = function () {
            return _name;
        };
        this.setName = function (v) {
            _name = v;
        };
        this.getRenderType = function () {
            return _renderType;
        };
        this.setRenderType = function (v) {
            _renderType = v;
        };
        this.getRender = function () {
            return _render;
        };
        this.setRender = function (v) {
            _render = v;
        };
        this.getClass = function () {
            return _class;
        }
        this.setClass = function (v) {
            _class = v;
        }
        this.getRenderIndex = function() {
            return _renderIndex;
        }
        this.setRenderIndex = function(v) {
            _renderIndex = v;
        }
        this.getVisible = function() {
            return _visible;
        }
        this.setVisible = function(v) {
            _visible = v;
        }
        this.Render = render;

        return this;
    };
})(jQuery, Sdk);