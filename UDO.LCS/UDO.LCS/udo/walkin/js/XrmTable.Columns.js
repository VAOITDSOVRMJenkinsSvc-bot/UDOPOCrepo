"use strict";
var XrmTable = XrmTable || {};

(function ($) {
    XrmTable.Columns = function () {
        var n,
			u = "XrmTable.Columns constructor parameter can accept a boolean value as the first parameter, an array of strings, or an array of XrmTable.Columns";

        if (!(this instanceof XrmTable.Columns)) {
            if (arguments.length > 0) {
                if ((typeof arguments[0] === "boolean") || arguments[0].push !== undefined) {
                    return new XrmTable.Columns(arguments[0]);
                }
                if (typeof arguments[0] === "string") {
                    return new XrmTable.Columns(Array.prototype.slice.call(arguments));
                }
                if (arguments[0] instanceof XrmTable.Column) {
                    return new XrmTable.Columns(Array.prototype.slice.call(arguments));
                }
                throw new TypeError(u);
            } else {
                return new XrmTable.Columns();
            }
        }

        var _columns = [],
			_allcolumns = false;

        if (arguments.length > 0) {
            if (typeof arguments[0] === "boolean") {
                if (arguments[0]) {
                    _allcolumns = true;
                } else {
                    throw new TypeError(u);
                }
            } else if (arguments[0] instanceof XrmTable.Columns) {
                return arguments[0];
            } else if (arguments[0].push !== undefined) { //array
                for (n = 0; n < arguments[0].length; n++) {
                    _columns.push(new XrmTable.Column(arguments[0][n]));
                }
            } else {
                for (n = 0; n < arguments.length; n++) {
                    _columns.push(new XrmTable.Column(arguments[n]));
                }
            }
        }

        function getArrayOf(c) {
            var a = [],
				b;
            for (b in _columns) {
                a.push(_columns[b][c]);
            }
            return a;
        }

        this.getAllColumns = function () {
            return _allcolumns;
        };

        this.setAllColumns = function (b) {
            _allcolumns = b;
        };

        this.setColumns = function () {
            if (arguments[0] instanceof XrmTable.Columns) {
                _columns = arguments[0];
            }
            _columns = new XrmTable.Columns(arguments);
        };

        this.getColumns = function () {
            return _columns;
        };

        this.getSortColumns = function (key) {
            var cols = [];
            key = key.toLowerCase();
            var b, col;
            for (b in _columns) {
                col = _columns[b];
                if (col.getSortKey().toLowerCase() === key) cols.push(col);
            }
            return cols;
        }

        this.getColumn = function(key) {
            key = key.toLowerCase();
            var b, col;
            for (b in _columns) {
                col = _columns[b];
                if (col.getKey().toLowerCase() === key) return col;
            }
            return null;
        }

        this.getKeys = function () {
            return getArrayOf("key");
        };

        this.getNames = function () {
            return getArrayOf("name");
        };

        this.addColumn = function () {
            if (arguments[0] instanceof XrmTable.Column) {
                _columns.push(arguments[0]);
            } else {
                var c = new XrmTable.Column(arguments);
                if (c && c !== null) {
                    _columns.push(c);
                }
            }
        };

        this.toggleColumnSort = function (col, key) {
            if (!key) key = col.getSortKey();
            if (!key || key == "") return;
            var cols = this.getSortColumns(key);
            var sorttype = cols[0].getSortCondition();

            if (sorttype == XrmTable.Constants.SortType.ASCENDING) {
                sorttype = XrmTable.Constants.SortType.DESCENDING;
            } else {
                sorttype = XrmTable.Constants.SortType.ASCENDING;
            }

            $.each(cols, function (i, v) {
                v.setSortIndex(0);
                v.setSortCondition(sorttype);
            });

            $.each(_columns, function (i, v) {
                if (v !== col && v.getSortIndex()>=0) v.setSortIndex(v.getSortIndex() + 1);
            });
        }

        this.getSortOrder = function () {
            var cols = [],keys=[],exists,key;
            
            $.each(_columns, function(i,v) {
                if (v.getSortIndex() >= 0) {
                    key = v.getSortKey();
                    exists = false;
                    if ($.inArray(key, keys)>-1) exists = true;
                    keys.push(key);
                    cols.push({
                        key: v.getSortKey(),
                        desc: v.getSortCondition()==XrmTable.Constants.SortType.DESCENDING,
                        col: v,
                        colkey: v.getKey(),
                        index: v.getSortIndex(),
                        extra: exists
                    });
                }
            });

            cols.sort(function (a, b) {
                return a.index - b.index;
            });

            return cols;
        }
    };
})(jQuery);