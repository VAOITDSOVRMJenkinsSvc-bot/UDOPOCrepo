"use strict";
var XrmTable = {};
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
    };

    XrmTable.Column = function () {
        var u = "XrmTable.Column constructor parameter can accept an array of strings.";
        var _key,
		_sortKey = "",
		_name,
		_render,
		_renderType = "column",
		_tooltip,
        _class;

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
                if (has(a, "class", "string")) {
                    _class = a.class;
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
            }

            if (_sortKey === "") {
                _sortKey = _key;
            }
        }

        this.getTooltip = function () {
            if (_tooltip === null) return _name;
            return _tooltip;
        }
        this.setTooltip = function (v) {
            _tooltip = v;
        }

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
        this.Render = render;
        return this;
    };

    XrmTable.PagingInfo = function (options) {
        var defaultPageInfo = {
            moreRecords: false,
            pagingCookie: "",
            totalRecords: -1,
            exceededLimit: false,
            page: 1,
            totalPages: 1
        };
        if (!(this instanceof XrmTable.PagingInfo)) {
            return new XrmTable.PagingInfo(options);
        }
        if (options instanceof XrmTable.PagingInfo) {
            return new XrmTable.PagingInfo({
                moreRecords: options.getMoreRecords(),
                pagingCookie: options.getPagingCookie(),
                totalRecords: options.getTotalRecords(),
                exceededLimit: options.getExceededLimit(),
                page: options.getPageNumber(),
                totalPages: this.setTotalPages(options.getTotalPages())
            });
        }

        var _info = $.extend(true, {}, defaultPageInfo, options);

        this.setMoreRecords = function (v) {
            if (typeof v === "boolean") {
                _info.moreRecords = v;
            } else {
                throw new TypeError("MoreRecords must be a boolean.");
            }
        };
        this.getMoreRecords = function () {
            return _info.moreRecords;
        };

        this.setPagingCookie = function (v) {
            if (typeof v === "string") {
                _info.pagingCookie = v;
            } else {
                throw new TypeError("PagingCookie must be a string.");
            }
        };
        this.getPagingCookie = function () {
            return _info.pagingCookie;
        };

        this.setTotalRecords = function (v) {
            if (typeof v === "number") {
                _info.totalRecords = v;
            } else {
                throw new TypeError("PagingCookie must be a number.");
            }
        };
        this.getTotalRecords = function () {
            return _info.totalRecords;
        };

        this.setExceededLimit = function (v) {
            if (typeof v === "boolean") {
                _info.exceededLimit = v;
            } else {
                throw new TypeError("PagingCookie must be a boolean.");
            }
        };
        this.getExceededLimit = function () {
            return _info.exceededLimit;
        };

        this.setPageNumber = function (v) {
            if (typeof v === "number") {
                _info.page = v;
            } else {
                throw new TypeError("PagingCookie must be a number.");
            }
        };
        this.getPageNumber = function () {
            return _info.page;
        };

        this.setTotalPages = function (v) {
            if (typeof v === "number") {
                _info.totalPages = Math.ceil(v);
            } else {
                throw new TypeError("PagingCookie must be a number.");
            }
        };
        this.getTotalPages = function () {
            return _info.totalPages;
        };
    };

    XrmTable.Table = function (element, options) {
        var xt = this;

        var _internal = {
            columns: null,
            query: null,
            order: [],
            cfg: {},
            selected: [],
            table: null,
            parent: null,
            pageInfo: {},
            refreshing: false
        };

        var defaults = {
            SortOnHeaderClick: true,
            RecordsPerPage: 20,
            MultiSelect: false,
            AllowSelect: false,
            AllowDrag: false,
            QuickSort: true,
            name: "XrmTable",
            RenderType: "table"
        };

        /**************** INTERNAL METHODS **********************/
        function _log(msg) {

            if (_internal.cfg.debug === true) {
                msg = _internal.cfg.name + ": " + msg;
                if (console === undefined) {
                    _internal.cfg.debug = false;
                    return;
                }
                console.log(msg);
            }
        }

        function _init() {
            var appendTable = false;

            _internal.cfg = $.extend(true, {}, defaults, $.extend({}, options));
            _internal.pageInfo = new XrmTable.PagingInfo();
            _internal.parent = $(element);
            _internal.cards = null;
            if (_internal.parent.is("table")) {
                xt.setTable(_internal.parent);
                _internal.parent = _internal.parent.parent();
            }
            if (_internal.cfg.table) {
                xt.setTable(_internal.cfg.table);
            }
            if (_internal.cfg.query) {
                xt.setQuery(_internal.cfg.query);
            }
            if (_internal.cfg.columns) {
                xt.setColumns(_internal.cfg.columns);
            }
            if (_internal.cfg.cards) {
                _internal.cards = $("#" + _internal.cfg.cards);
            }
            if (_internal.table === null && _internal.cards === null) {
                appendTable = true;
                _internal.table = $("<table/>");
                if (typeof _internal.cfg.table === "string") {
                    _internal.table.attr("id", _internal.cfg.table);
                }
                _internal.table.hide();
            }

            if (_internal.table) {
                _internal.table.addClass("table table-bordered table-hover table-striped table-responsive");
            }

            if (appendTable) {
                if (_internal.table) {
                    _internal.parent.append(_internal.table);
                }
                if (_internal.cards) {
                    _internal.parent.append(_internal.cards);
                }
            }
        }

        function _updatePagingInfo() {
            if (_internal.query instanceof Sdk.Query.QueryExpression) {
                var queryPageInfo = new Sdk.Query.PagingInfo();

                if (_internal.pageInfo.getPagingCookie() !== "" && _internal.pageInfo.getPageNumber() > 1) {
                    queryPageInfo.setPagingCookie(_internal.pageInfo.getPagingCookie());
                    queryPageInfo.setPageNumber(_internal.pageInfo.getPageNumber());
                } else {
                    queryPageInfo.setPagingCookie("");
                    queryPageInfo.setPageNumber(1);
                }
                queryPageInfo.setReturnTotalRecordCount(true);
                queryPageInfo.setCount(_internal.cfg.RecordsPerPage);
                _internal.query.PageInfo = queryPageInfo;
            } else if (_internal.query instanceof Sdk.Query.FetchExpression) {
                var fetchXml = $($.parseXML(_internal.query.getFetchXml())).find("fetch").first();
                fetchXml.attr("returntotalrecordcount", "true");
                if (_internal.pageInfo.getPagingCookie() !== "" && _internal.pageInfo.getPageNumber() > 1) {
                    fetchXml.attr("paging-cookie", _internal.pageInfo.getPagingCookie());
                    fetchXml.attr("page", _internal.pageInfo.getPageNumber());
                } else {
                    fetchXml.removeAttr("paging-cookie");
                    fetchXml.removeAttr("page");
                }
                fetchXml.attr("count", _internal.cfg.RecordsPerPage);
                _internal.query.setFetchXml(fetchXml.toXmlString());
            }
        }

        function _retrieveMultiple() {
            if (_internal.refreshing) {
                return $.when();
            }
            _internal.refreshing = true;
            _updatePagingInfo();
            Sdk.jQ.setJQueryVariable($);
            var data = {
                query: _internal.query
            };
            return (new _event('preRetrieveMultiple', data)).call(data.query)
			.then(Sdk.jQ.retrieveMultiple)
			.then((new _event("postRetrieveMultiple", data)).call, (new _error("RetrieveMultiple")).call)
			.then(_render, (new _error("Render")).call)
            .then(function () {
                _internal.refreshing = false;
                return $.when();
            });
        }

        function _addClasses(o, k) {
            if (_internal.cfg.classes !== undefined) {
                if (undefined === k) {
                    k = o.prop("tagName").toLowerCase();
                }
                if (undefined === k.push) {
                    k = [k];
                }
                var i;
                for (i in k) {
                    if (_internal.cfg.classes[k[i]]) { o.addClass(_internal.cfg.classes[k[i]]); }
                }
            }
        }

        // This gets all of the columns from the data that was returned
        function _getAllColumns(data) {
            var i;
            var keys = [];

            var getKeys = function (o) {
                var d,
				key;
                if (o instanceof Sdk.Entity) {
                    d = o.view().attributes;
                } else {
                    d = o;
                }

                for (key in d) {
                    if ($.inArray(key, keys) === -1) {
                        keys.push(key);
                    }
                }
            };

            if (data instanceof Sdk.EntityCollection) {
                var key,
				a = {};
                data.forEach(function (e) {
                    var attribs = e.view().attributes;
                    for (key in attribs) {
                        if (!a[key]) {
                            a[key] = true;
                        }
                    }
                });

                for (key in a) {
                    keys.push(key);
                }
            } else if (data.push !== undefined) {
                for (i in data) {
                    getKeys(data[i]);
                }
            } else {
                getKeys(data);
            }

            var columns = new XrmTable.Columns(keys);
            columns.setAllColumns(true);
            return columns;
        }

        function _updateOrderExpressions() {
            var i /*loop index*/;

            _internal.pageInfo.setPageNumber(1);
            _internal.pageInfo.setPagingCookie("");

            if (_internal.query instanceof Sdk.Query.QueryExpression) {
                var orderby = new Sdk.Collection(Sdk.Query.OrderExpression);
                for (i in _internal.order) {
                    orderby.add(new Sdk.Query.OrderExpresison(_internal.order[i].key, _internal.order[i].desc ? Sdk.Query.OrderType.Descending : Sdk.Query.OrderType.Ascending));
                }
                //_internal.query.setOrders(orderby);

                return;
            } else if (_internal.query instanceof Sdk.Query.FetchExpression) {
                var key,
				attribute,
				fetchXml,
				keyparts,
				linkentity,
				orderxml,
				attributeFilter = function (i, e) {
				    e = $(e);
				    return (e.attr("alias") === key || e.attr("name") === key);
				},
				linkentityFilter = function (i, e) {
				    e = $(e);
				    return (e.attr("to") === keyparts[0] || e.attr("name") === keyparts[0]);
				};

                fetchXml = $($.parseXML(_internal.query.getFetchXml())).find("fetch").first();
                fetchXml.attr("returntotalrecordcount", "true");
                fetchXml.find("order").remove();

                for (i in _internal.order) {
                    key = _internal.order[i].key;

                    attribute = fetchXml.find("attribute").filter(attributeFilter);

                    if (attribute === null && key.indexOf(".") !== -1) {
                        keyparts = key.split(".");
                        linkentity = fetchXml.find("link-entity").filter(linkentityFilter);
                        if (linkentity !== null) {
                            attribute = linkentity.find("attribute").filter(attributeFilter);
                        }
                    }

                    if (attribute !== null) {
                        orderxml = '<order attribute="' + key + '"';
                        if (_internal.order[i].desc) {
                            orderxml += ' descending="true"';
                        }
                        orderxml += '/>';
                        attribute.parent().append($(orderxml));
                    }
                }

                _internal.query.setFetchXml(fetchXml.toXmlString());

            }
        }

        function _sort() {
            _updateOrderExpressions();

            if (_internal.cfg.QuickSort && $.sortElements && !_internal.pageInfo.getMoreRecords()) {
                var cmp = function (a, b) {

                    var i,
					a1 = $(a).data("obj"),
					b1 = $(b).data("obj"),
					item = null;
                    if (a1.attributes) {
                        a1 = a1.attributes;
                    }
                    if (b1.attributes) {
                        b1 = b1.attributes;
                    }

                    for (i in order) {
                        item = order[i];

                        if (a1[item.key].fValue !== b1[item.key].fValue) {
                            return $.text([a1[item.key]]) > $.text([b1[item.key]]) ?
							item.desc ? -1 : 1
							 : item.desc ? 1 : -1;
                        }
                    }
                    return 0;
                };

                var defer = new $.Deferred();

                setTimeout(function () {
                    _internal.table.find("tbody > tr").sortElements(cmp);
                    defer.resolve();
                }, 0);

                return defer.promise();
            } else {
                return _retrieveMultiple();
            }
        }

        function _toggleColumnSort(col) {
            var key = col.getSortKey();

            var i = 0,
			desc = false,
			item = null;

            for (i = _internal.order.length - 1; i >= 0; i--) {
                item = _internal.order[i];
                if (item.key === key) {
                    if (!item.desc) {
                        desc = true;
                    }
                    _internal.order.splice(i, 1);
                }
            }

            _internal.order.splice(0, 0, {
                key: key,
                desc: desc,
                col: col
            });

            return _sort();
        }

        function _error(context) {
            var _context = context;
            this.call = function (error) {
                var dfd = new $.Deferred();
                _log("Error: " + _context + ": " + error);
                dfd.reject({
                    context: _context,
                    error: error
                });
                return dfd.promise();
            };
        }

        function _event(name, o) {
            var _name = name;
            var _data = o;
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
                    event.data = _data;
                    $.when($(xt).trigger(event, _data)).done(function () {
                        if (event.isDefaultPrevented()) {
                            dfd.reject(event.error);
                        } else {
                            dfd.resolve.apply(this, event.arguments); //pass thru args
                        }
                    });
                } else {
                    $.when($(xt).trigger(event)).done(function () {
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

        function _clearSelection() {
            var data = {
                clear: true
            };
            return new _event('preClearSelection', data).call().then(function () {
                var dfd = new $.Deferred();
                if (!data.clear) {
                    dfd.reject();
                    return dfd.promise();
                }
                _internal.table.find("tbody > tr").removeClass("ui-selected");
                _internal.selected.length = 0;
                _internal.table.attr('aria-activedescendant', "");
                dfd.resolve.apply(this, arguments); //pass thru args
                return dfd.promise();
            })
			.then((new _event('postClearSelection')).call);
        }

        function _select(row) {
            var data = {
                row: row,
                selected: _internal.selected
            };

            return new _event("preSelect", data).call()
			.then((new _event("click", data)).call)
			.then(function () {
			    var dfd = new $.Deferred();
			    if (data.row.hasClass("ui-selected")) {
			        data.row.removeClass("ui-selected");
			        // remove item
			        _internal.selected.splice($.inArray(row.attr("id"), _internal.selected), 1);
			        dfd.resolve();
			    } else {
			        if (!_internal.cfg.MultiSelect && _internal.selected.length > 0) {
			            return _clearSelection(row).then(function () {
			                row.addClass("ui-selected");
			                _internal.table.attr('aria-activedescendant', data.row.attr("id"));
			                _internal.selected.push(data.row.attr("id"));
			                data.selected = _internal.selected;
			            });
			        }
			        row.addClass("ui-selected");
			        _internal.selected.push(data.row.attr("id"));
			        data.selected = _internal.selected;
			        dfd.resolve(); // no pass through on select
			        data.row.focus();
			    }
			    return dfd.promise();
			}).then((new _event("postSelect", data)).call);
        }

        function _gotoPage(pageNumber) {
            if (pageNumber > _internal.pageInfo.getTotalPages()) {
                pageNumber = _internal.pageInfo.getTotalPages();
            }
            if (pageNumber < 1) {
                pageNumber = 1;
            }

            var data = {
                PageInfo: _internal.pageInfo,
                DestinationPageNumber: pageNumber
            };

            return new _event("preGoToPage", data).call().then(function () {
                var dfd = new $.Deferred();
                _internal.pageInfo.setPageNumber(data.DestinationPageNumber);
                dfd.resolve();
                return dfd.promise();
            }).then((new _event("postGoToPage", data)).call)
			.then(_retrieveMultiple);
        }

        function _firstPage() {
            return _gotoPage(1);
        }

        function _prevPage() {
            return _gotoPage(_internal.pageInfo.getPageNumber() - 1);
        }

        function _nextPage() {
            return _gotoPage(_internal.pageInfo.getPageNumber() + 1);
        }

        function _lastPage() {
            return _gotoPage(_internal.pageInfo.getTotalPages());
        }

        function _clearTable() {
            _internal.table.find("thead, th, tr, tbody, tfoot").remove();
        }

        function _tabFocus(mod, selector, current) {
            if (!selector) selector = ":tabbable";
            var items = $(selector);
            if (!current) current = ':focus';
            current = $(current);

            items.sort(function (a, b) {
                var ai = $(a).attr("tabindex");
                var bi = $(b).attr("tabindex");
                if (!ai) ai = 0;
                if (!bi) bi = 0;
                return ai - bi;
            });

            var index = 0;
            var prevent = true;
            if (current.length === 1) {
                index = items.index(current) + mod;
                if (index >= items.length) {
                    prevent = false;
                    index--;
                }
            }
            if (mod == -1 && _internal.cfg.prev) {
                $("#" + _internal.cfg.prev).focus();
            } else {
                if (index < 0) index = 0;
                items.eq(index).focus();
            }
            return prevent;
        }

        function _render(data) {
            switch (_internal.cfg.RenderType) {
                case "table": return _renderTable(data);
                    break;
                case "card": return _renderCards(data);
                    break;
            }
        }

        function _renderCards(data) {
            if (_internal.columns.getAllColumns()) {
                _internal.columns = _getAllColumns(data);
            }

            var c;
            var cols = _internal.columns.getColumns();

            function createCard(id, data) {
                data.id = id;
                data.rowid = "row-" + id;
                return new _event("preRenderCard", data).call()
				.then(function () {
				    var card = $("<div id='" + data.rowid + " class='col-md-4'/>");
				    var panel = $("<div class='panel panel-primary'/>");
				    card.append(panel);
				    if (_internal.cfg.AllowSelect) {
				        card.click(function () {
				            _select(card);
				        });
				        if ($.inArray(data.rowid, _internal.selected) !== -1) {
				            card.addClass("ui-selected");
				        }
				    } else {
				        card.click(function () {
				            new _event("click", {
				                card: card
				            }).call();
				        });
				    }
				    card.dblclick(function () {
				        new _event("dblclick", {
				            row: card
				        }).call();
				    });

				    if (_internal.cfg.AllowDrag) {
				        card.draggable({
				            cursor: "move",
				            helper: "clone",
				            revert: "invalid"
				        });
				    }

				    if (typeof data.view === "function") {
				        card.data("obj", data.view());
				    } else {
				        card.data("obj", data);
				    }

				    card.attr("id", data.rowid);

				    var rows = [];
				    rows.push($("<div class='row'/>"));
				    var row = rows[0];

				    var header = $("<div class='panel-heading'/>");

				    var j, col, cell, renderData;

				    for (j in cols) {
				        col = cols[j];
				        cell = $("<div/>");
				        cell.addClass(col.getClass());
				        cell.addClass("text-left");
				        cell.data("attributeName", col.key);
				        //_addClasses(cell, ["td", "tbody > tr > td", "tr > td"]);

				        var name = col.getName();
				        renderData = col.Render(data, rows, col, xt);

				        if (typeof renderData === "undefined" || renderData === null || (renderData.trim && renderData.trim() === "")) {
				            renderData = "";
				        }

				        if (typeof renderData === "string") {
				            if (renderData === "") {
				                cell.html("&nbsp;");
				            } else {
				                cell.text(renderData);
				            }
				        } else if (renderData instanceof jQuery && renderData.prop("tagName") === "DIV") {
				            rows.push(renderData);
				            //_addClasses(row, ["tr", "tbody > tr"]);
				        }

				        if (col.getRenderType() === "column") {
				            var label = $("<div class='col-md-2 text-right'><b>" + name + ": </b></div>");
				            row.append(label);
				            row.append(cell);
				        } else if (col.getRenderType() === "header") {
				            header.text(renderData);
				        } else {
				            cell.remove();
				        }
				    }

				    data.rows = rows;

				    panel.append(header);
				    var body = $("<div class='panel-body'/>");
				    body.append(rows);
				    panel.append(body);

				    var dfd = new $.Deferred();
				    dfd.resolve(card);
				    return dfd.promise();
				})
				.then((new _event("postRenderCard", data)).call);
            }

            var i;
            var div = $("<div class='row xrmtable-cards'/>");

            var appendRow = function (r) {
                div.append(r);
            };

            if (data instanceof Sdk.Entity) {
                createCard(data.getId(), data).then(appendRow);
            } else if (data instanceof Sdk.EntityCollection) {
                _internal.pageInfo.setPagingCookie(data.getPagingCookie());
                _internal.pageInfo.setTotalRecords(data.getTotalRecordCount());
                _internal.pageInfo.setTotalPages(data.getTotalRecordCount() / _internal.cfg.RecordsPerPage);
                _internal.pageInfo.setMoreRecords(data.getMoreRecords());
                _internal.pageInfo.setExceededLimit(data.getTotalRecordCountLimitExceeded());

                var count = data.getCount();
                for (i = 0; i < count; i++) {
                    createCard(data.getEntity(i).getId(), data.getEntity(i)).then(appendRow);
                }
            } else if (data.push !== undefined) {
                for (i in data) {
                    createCard(i, data[i]).then(appendRow);
                }
            } else {
                createCard('item', data).then(appendRow);
            }

            // append div to parent
            _internal.cards.find("div.xrmtable-cards").remove();
            _internal.cards.append(div);

            // call postRender

            return new _event('postRender', data).call();
            
        }

        function _renderTable(data) {
            if (_internal.columns.getAllColumns()) {
                _internal.columns = _getAllColumns(data);
            }

            var i,
            cols = _internal.columns.getColumns(),
            colspan = 0;
            for (i in cols) {
                if (cols[i].getRenderType() === "column") {
                    colspan++;
                }
            }

            function createTableHeader(parent, cols, data) {
                function createColumnHeader(col) {
                    var th = $("<th/>");
                    th.attr("scope", "col");
                    _addClasses(th, ["tr > th", "th", "thead > tr > th"]);
                    th.text(col.getName());
                    th.prop("title", col.getTooltip());
                    th.data("key", col.getKey());
                    if (_internal.cfg.SortOnHeaderClick) {
                        th.click(function () {
                            _toggleColumnSort(col);
                        });
                    }
                    return th;
                }

                var thead = $("<thead/>");
                _addClasses(thead);
                var tr = $("<tr/>");
                _addClasses(tr, ["tr", "thead > tr"]);
                for (i in cols) {
                    if (cols[i].getRenderType() === "column") {
                        tr.append(createColumnHeader(cols[i]));
                    }
                }
                thead.append(tr);
                parent.find("thead").remove();
                parent.append(thead);

                var dfd = new $.Deferred();
                dfd.resolve(parent, data);
                return dfd.promise();
            }

            function createTableBody(parent, data) {
                var rowids = [];
                function createRow(id, data) {
                    data.id = id;
                    data.rowid = "row-" + id;
                    rowids.push(data.rowid);
                    return new _event("preRenderRow", data).call()
                    .then(function () {
                        var rows = [];
                        rows.push($("<tr/>"));
                        var row = rows[0];
                        //row.attr("role", "row");
                        _addClasses(row, ["tr", "tbody > tr"]);

                        if (_internal.cfg.AllowSelect) {
                            if ($.inArray(data.rowid, _internal.selected) !== -1) {
                                row.addClass("ui-selected");
                            }
                            row.click(function () {
                                _select(row);
                            });
                        } else {
                            row.click(function () {
                                new _event("click", {
                                    row: row
                                }).call();
                            });
                        }

                        row.dblclick(function () {
                            new _event("dblclick", {
                                row: row
                            }).call();
                        });

                        if (_internal.cfg.AllowDrag) {
                            row.draggable({
                                cursor: "move",
                                helper: "clone",
                                revert: "invalid"
                            });
                        }

                        if (typeof data.view === "function") {
                            row.data("obj", data.view());
                        } else {
                            row.data("obj", data);
                        }

                        row.attr("id", data.rowid);
                        //row.attr("role", "option");
                        //row.attr("tabIndex", "0");

                        var j,
                        col,
                        cell,
                        renderData;
                        for (j in cols) {
                            col = cols[j];
                            cell = $("<td/>");
                            cell.attr("tabindex", "-1");

                            //cell.attr("role", "gridcell");
                            cell.data("attributeName", col.key);
                            _addClasses(cell, ["td", "tbody > tr > td", "tr > td"]);

                            renderData = col.Render(data, rows, col, xt);
                            if (typeof renderData === "string") {
                                cell.text(renderData);
                            } else if (renderData instanceof jQuery && renderData.prop("tagName") === "TR") {
                                rows.push(renderData);
                                _addClasses(row, ["tr", "tbody > tr"]);
                            }

                            if (col.getRenderType() === "column") {
                                row.append(cell);
                            } else {
                                cell.remove();
                            }
                        }

                        data.rows = rows;

                        var dfd = new $.Deferred();
                        dfd.resolve(rows);
                        return dfd.promise();
                    })
                    .then((new _event("postRenderRow", data)).call);
                }

                var tbody = $("<tbody/>");
                _addClasses(tbody, "tbody");

                var appendRow = function (r) {
                    tbody.append(r);
                };

                var waitfor = [];

                if (data instanceof Sdk.Entity) {
                    createRow(data.getId(), data).then(appendRow);
                } else if (data instanceof Sdk.EntityCollection) {
                    _internal.pageInfo.setPagingCookie(data.getPagingCookie());
                    _internal.pageInfo.setTotalRecords(data.getTotalRecordCount());
                    _internal.pageInfo.setTotalPages(data.getTotalRecordCount() / _internal.cfg.RecordsPerPage);
                    _internal.pageInfo.setMoreRecords(data.getMoreRecords());
                    _internal.pageInfo.setExceededLimit(data.getTotalRecordCountLimitExceeded());

                    var count = data.getCount();
                   
                    for (i = 0; i < count; i++) {
                        waitfor.push(createRow(data.getEntity(i).getId(), data.getEntity(i)).then(appendRow));
                    }
                } else if (data.push !== undefined) {
                    for (i in data) {
                        waitfor.push(createRow(i, data[i]).then(appendRow));
                    }
                } else {
                    waitfor.push(createRow('item', data).then(appendRow));
                }

                parent.find("tbody").remove();
                parent.append(tbody);

                var dfd = new $.Deferred();
                $.when.apply($, waitfor).done(function () {
                    var end = rowids.length - 1;
                    //$.each(rowids, function (i, v) {
                    //    var kp = function (e) {
                    //        var key = e.which || e.keycode;
                    //        if (key == 32 /*space*/|| key == 13 /*enter*/) {
                    //            $(this).trigger("click");
                    //        }
                    //        if (key == 38 /*up*/ && i != 0) {
                    //            $("#"+rowids[i - 1]).focus();
                    //        }
                    //        if (key == 40 /*down*/ && i != end) {
                    //            $("#"+rowids[i + 1]).focus();
                    //        }
                    //        if (key == 9) /* tab */ {
                    //            if (e.shiftKey) {
                    //                if (_tabFocus(-1, ":tabbable", "#" + rowids[0])) e.preventDefault();
                    //            } else {
                    //                if (_tabFocus(1, ":tabbable", "#" + rowids[rowids.length - 1])) e.preventDefault();
                    //            }
                    //        }
                    //    }
                    //    $("#"+v).on('keydown',kp);
                    //});
                    
                    dfd.resolve(parent);
                });

                return dfd.promise();
            }

            function createTableFooter(parent) {
                function createPageButton(page, label, onclick) {
                    var li = $("<li/>");
                    var a = $('<a href="#"/>');
                    li.append(a);
                    if (label) {
                        a.append(label);
                    } else {
                        label = page.toString();
                        a.text(page);
                        if (page === _internal.pageInfo.getPageNumber()) {
                            li.addClass("active");
                        }
                    }
                    if (onclick && typeof onclick === "function") {
                        a.click(onclick);
                    } else {
                        a.click(function () {
                            var items = a.parent().parent().find("li");
                            items.removeClass("active");
                            items.removeClass("disabled");
                            if (page === _internal.pageInfo.getTotalPages() || !_internal.pageInfo.getMoreRecords()) {
                                var last = items.filter(function (i, e) {
                                    e = $(e);
                                    return e.text().indexOf("last") !== -1 || e.text().indexOf("next") !== -1;
                                });
                                last.addClass("disabled");
                            } else if (page === 1) {
                                var first = items.filter(function (i, e) {
                                    e = $(e);
                                    return e.text().indexOf("first") !== -1 || e.text().indexOf("prev") !== -1;
                                });
                                first.addClass("disabled");
                            }
                            a.parent().addClass("active");
                            _gotoPage(page);
                        });
                    }
                    if ((label.indexOf("prev") !== -1 || label === "first") && _internal.pageInfo.getPageNumber() === 1) {
                        li.addClass("disabled");
                    } else if ((label === "next" || label === "last") && _internal.pageInfo.getPageNumber() === (_internal.pageInfo.getTotalPages())) {
                        li.addClass("disabled");
                    } else if (page == label && _internal.pageInfo.getPageNumber() === page) {
                        li.addClass("active");
                    }
                    return li;
                }

                var page = _internal.pageInfo.getPageNumber();
                var totalPages = _internal.pageInfo.getTotalPages();

                if (_internal.pageInfo.getMoreRecords() || page > 1) {
                    var footer = $("<tfoot/>");

                    var row = $("<tr/>");
                    row.addClass("footer");
                    footer.append(row);

                    var cell = $('<td colspan="' + colspan + '"/>');
                    row.append(cell);

                    var paging = $('<ul class="pagination pagination-sm"/>');
                    cell.append(paging);

                    var first = [$('<span aria-hidden="true" class="glyphicon glyphicon-fast-backward" title="first"/>'), $('<span class="sr-only">first</span>')];
                    paging.append(createPageButton(1, first));

                    var prev = [$('<span aria-hidden="true" class="glyphicon glyphicon-backward" title="prev"/>'), $('<span class="sr-only">prev</span>')];
                    paging.append(createPageButton(page - 1, prev));

                    var k,
                    start,
                    end;
                    if (totalPages > 1) {
                        if (totalPages <= 7 || page <= 3) {
                            for (i = 1; i <= Math.min(7, totalPages) ; i++) {
                                paging.append(createPageButton(i));
                            }
                            // show each page
                        } else if (page > totalPages - 7) {
                            start = totalPages - 6;
                            end = start + 6;
                            for (k = start; k <= end; k++) {
                                paging.append(createPageButton(k));
                            }
                        } else {
                            start = page - 3;
                            end = start + 6;
                            for (k = start; k <= end; k++) {
                                paging.append(createPageButton(k));
                            }
                        }
                    }

                    var next = [$('<span aria-hidden="true" class="glyphicon glyphicon-forward" title="next"/>'), $('<span class="sr-only">next</span>')];
                    paging.append(createPageButton(page + 1, next));
                    var last = [$('<span aria-hidden="true" class="glyphicon glyphicon-fast-forward" title="last"/>'), $('<span class="sr-only">last</span>')];
                    paging.append(createPageButton(totalPages, last));

                    parent.find("tfoot").remove();
                    parent.append(footer);
                    //return footer;
                } else {
                    parent.find("tfoot").remove();
                    parent.append("<tfoot/>");
                }

                var dfd = new $.Deferred();
                dfd.resolve(parent);
                return dfd.promise();
            }

            var pos = $(document).scrollTop();

            var focuson = null;
            if ($.contains(_internal.table[0], document.activeElement)) {
                if (document.activeElement.id) {
                    focuson = document.activeElement.id;
                }
            }
            _clearTable();
            $(_internal.table).attr('role', 'grid');
            return createTableHeader(_internal.table, cols, data).then(createTableBody).then(createTableFooter)
            .then(function (table) {
                table.show();

                $(document).scrollTop(pos);

                var postRenderData = {
                    selected: _internal.selected
                };

                if (_internal.selected.length === 1) {
                    postRenderData.row = $("#" + _internal.selected[0]);
                }

                if (focuson) {
                    var focusel = document.getElementById(focuson);
                    if (focusel) $(focusel).focus();
                }

                return new _event('postRender', postRenderData).call();
            });
        }
        /******************* PUBLIC METHODS *********************/

        this.refresh = _retrieveMultiple;
        this.pages = {
            gotoPage: _gotoPage,
            first: _firstPage,
            next: _nextPage,
            prev: _prevPage,
            last: _lastPage
        };
        this.init = _init;
        this.clearSelection = _clearSelection;

        /****************** GETTERS AND SETTERS *****************/
        this.setTable = function (o) {
            if (o instanceof jQuery && o.is("table")) {
                _internal.table = o;
            } else if (typeof o === "string") {
                o = document.getElementById(o);
                if (o !== null) {
                    this.setTable(o);
                }
            } else if (o instanceof HTMLTableElement) {
                _internal.table = $(o);
            } else {
                throw new TypeError("Table must be a table element, table jQuery object, or the id of a table.");
            }
        };
        this.getTable = function () {
            return _internal.table;
        };

        this.setSelection = function (v) {
            if (v.push !== undefined) {
                _internal.selected = v;
                return;
            } else {
                _internal.selected = [v];
            }
        };

        this.getItems = function () {
            var items = [];
            _internal.table.find("tbody > tr").each(function (e) {
                var me = $(this);
                var id = me.attr('id');
                if (id) {
                    if (id.indexOf("row-") > -1) {
                        items.push(id.substring(4));
                    }
                }
            });
            return items;
        }

        this.getSelection = function () {
            return _internal.selected;
        };

        this.setColumns = function (v) {
            _internal.columns = new XrmTable.Columns(v);
        };
        this.getColumns = function () {
            return _internal.columns;
        };

        // This should have type restrictions
        this.setParent = function (v) {
            _internal.parent = v;
        };
        this.getParent = function () {
            return _internal.parent;
        };

        this.setPagingInfo = function (v) {
            _internal.pageInfo = new XrmTable.PagingInfo(v);
        };
        this.getPagingInfo = function () {
            return _internal.pageInfo;
        };

        this.setRecordsPerPage = function (v) {
            if ($.isNumeric(v)) {
                _internal.cfg.RecordsPerPage = v;
            } else {
                throw new TypeError("RecordsPerPage must be a number.");
            }
        };
        this.getRecordsPerPage = function () {
            return _internal.cfg.RecordsPerPage;
        };

        this.setQuery = function (v) {
            _internal.refreshing = false;
            if (typeof v === "string") {
                this.setQuery(new Sdk.FetchExpression(v));
            } else if (v instanceof Sdk.Query.FetchExpression || v instanceof Sdk.Query.QueryExpression) {
                _internal.cfg.query = v;
                _internal.query = v;
                _internal.pageInfo.setPageNumber(1);
                _internal.pageInfo.setPagingCookie("");
            } else {
                throw new TypeError("Query must be a string fetchxml, the type Sdk.Query.FetchExpression or Sdk.Query.QueryExpression.");
            }
        };
        this.getQuery = function () {
            return _internal.query;
        };

        this.setOrder = function (keys) {
            if (keys === undefined || keys === null) {
                _internal.order = [];
                return;
            }
            if (keys.push === undefined) {
                keys = keys.split(',');
            }
            var i,
            order = [],
            parts;
            for (i in keys) {
                parts = keys[i].split(' ');
                order.push({
                    key: parts[0],
                    desc: (parts.length === 1 || parts[1] !== "desc")
                });
            }
            _internal.order = order;
        };
        this.getOrder = function () {
            return _internal.order;
        };

        this.enableSortOnHeaderClick = function () {
            _internal.cfg.SortOnHeaderClick = true;
        };
        this.disableSortOnHeaderClick = function () {
            _internal.cfg.SortOnHeaderClick = false;
        };
        this.getSortOnHeaderClickEnabled = function () {
            return _internal.cfg.SortOnHeaderClick;
        };

        this.enableMultiSelect = function () {
            _internal.cfg.MultiSelect = true;
        };
        this.disableMultiSelect = function () {
            _internal.cfg.MultiSelect = false;
        };
        this.getMultiSelectEnabled = function () {
            return _internal.cfg.MultiSelect;
        };

        this.enableAllowSelect = function () {
            _internal.cfg.AllowSelect = true;
        };
        this.disableAllowSelect = function () {
            _internal.cfg.AllowSelect = false;
        };
        this.getAllowSelectEnabled = function () {
            return _internal.cfg.AllowSelect;
        };

        this.enableAllowDrag = function () {
            _internal.cfg.AllowDrag = true;
        };
        this.disableAllowDrag = function () {
            _internal.cfg.AllowDrag = false;
        };
        this.getAllowDragEnabled = function () {
            return _internal.cfg.AllowDrag;
        };

        this.enableQuickSort = function () {
            _internal.cfg.QuickSort = true;
        };
        this.disableQuickSort = function () {
            _internal.cfg.QuickSort = false;
        };
        this.getQuickSortEnabled = function () {
            return _internal.cfg.QuickSort;
        };

        this.on = $(this).on;
    };

    $.fn.xrmTable = function (options) {
        var obj = $(this);

        if (obj.length === 1 && obj.data('xrmTable')) {
            return obj.data('xrmTable');
        }

        var tbls = [];
        obj.each(function (/*i*/) {
            // assume $(this) is an element
            var me = $(this);

            // Return early if this element already has a plugin instance
            if (me.data('xrmTable')) {
                return $(me.data('xrmTable'));
            }

            var tag = this.nodeName.toLowerCase();

            if (tag === "table" || tag === "div") {
                var table = new XrmTable.Table(me, options);
                table.init();
                table.getParent().data("xrmTable", table);
                tbls.push(table);
                table.refresh();
            }
        });

        if (tbls.length === 1) {
            return $(tbls[0]);
        }

        return $(tbls);
    };

    $.fn.xrmTable.defaults = {};
})(jQuery);