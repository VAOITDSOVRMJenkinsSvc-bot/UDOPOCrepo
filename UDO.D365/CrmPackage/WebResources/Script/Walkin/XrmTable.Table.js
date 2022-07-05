var XrmTable = XrmTable || {};

(function ($) {

    XrmTable.Table = function (element, options) {
        var xt = this;

        /**
         * _internal: object that controls how events are triggered, items are rendered, and how the xrmTable is configured
         */
        var _internal = {
            columns: null,
            query: null,
            entity: null,
            order: [],
            cfg: {},
            selected: [],
            table: null,
            parent: null,
            pageInfo: {},
            refreshing: false,
            cards: null,
            focus: null,
            focuscol: 0,
            viewRecordCount: 0
        };

        /**
         * _log : Log debug messages to the console
         * @param {} msg 
         * @returns {} 
         */

        function _log(msg) {
            if (_internal.cfg.debug === true) {
                msg = _internal.cfg.ModuleName + ": " + msg;
                if (_internal.cfg.logger) {
                    _internal.cfg.logger(msg);
                    return;
                }
                if (console === undefined) {
                    _internal.cfg.debug = false;
                    return;
                }
            }
        }

        ///**
        // * _error: returns a failed jQuery promise
        // * @param {} context 
        // * @returns {} jQuery promise
        // */
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

        ///**
        // * _event: Triggers an event.
        // * @param {} eventOrName : Either a string containing the event to be triggered or a jQuery Event object
        // * @param {} dataObject  : the data object to be passed to the triggered event
        // * @returns {} jQuery promise : successful when triggered event (that must also return a promise) is successful.
        // */
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
                    if (_internal.cfg.classes[k[i]]) {
                        o.addClass(_internal.cfg.classes[k[i]]);
                    }
                }
            }
        }

        ///**
        // * _init: Initialize the XrmTable.Table object.
        // * @param {} element 
        // * @param {} options 
        // * @returns {} this
        // */
        function _init(element, options) {
            // copy options (and DefaultSettings) to _internal.cfg
            _internal.cfg = $.extend(true, {}, XrmTable.Constants.DefaultSettings, $.extend({}, options));
            _internal.pageInfo = new XrmTable.PagingInfo();

            // parent should always be a div
            _internal.parent = $(element);

            if (!_internal.parent.is('div')) {
                _log("XrmTable parent element must be a div container.");
                return;
            }
            console.log(options);
            if (_internal.cfg.query) {
                xt.setQuery(_internal.cfg.query);
            }
            if (_internal.cfg.entity) {
                xt.setEntity(_internal.cfg.entity);
            }
            if (_internal.cfg.columns) {
                xt.setColumns(_internal.cfg.columns);
            }

            _registerEvents();

            return xt;
        }

        ///**
        // * _updatePagingInfo: Updates the query or fetchexpression with the paging info to handle proper pagination
        // * @returns {} 
        // */

        ///**
        // * Run the query in the _internal.query
        // * @returns {} jQuery Promise
        // */
        function _retrieveMultiple(setOrder) {
            if (_internal.refreshing) {
                return $.when();
            }
            _internal.refreshing = true;
            if (setOrder) _updateOrderExpressions();
            var data = {
                query: _internal.query,
                entity: _internal.entity
            };
            return (new _event('preRetrieveMultiple', data)).call(data.query)
                .then(function (reply) {
                    console.log(data);
                    if (data.query.charAt(0) === "%") { //first char is encoded <
                        return webApi.RetrieveByFetchXml("queueitems", data.query);
                    } else {
                        console.log("at correct retrieve");
                        return webApi.RetrieveMultiple(data.entity, null, data.query);
                    }
                })
                .then(setViewRecordCount)
                .then((new _event("postRetrieveMultiple", data)).call, (new _error("RetrieveMultiple")).call)
                .then(_render, (new _error("Render")).call)
                .then(function () {
                    _internal.refreshing = false;
                    return $.when();
                });
        }

        function setViewRecordCount(data) 
        {
            return new Promise(function (resolve, reject) {
                console.log(data);
                if (data && data.length) {
                    _internal.viewRecordCount = data.length;
                } else {
                    _internal.viewRecordCount = 0;
                }
                resolve(data);
            });
            //var dfd = new $.Deferred();
            
            //dfd.resolve(data);

            //return dfd.promise();
        }

        ///**
        // * _getAllColumns: Loops through the entitycollection in the data to parse out all available columns
        // * @param {} data 
        // * @returns {} XrmTable.Columns (collection of bindable columns)
        // */
        function _getAllColumns(data) {
            var i;
            var keys = [];

            var getKeys = function (o) {
                var d,
                    key;
                d = o;

                for (key in d) {
                    if ($.inArray(key, keys) === -1) {
                        keys.push(key);
                    }
                }
            };
            if (data.push !== undefined) {
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

        ///**
        // * _updateOrderExpressions: Updates the QE or FE with the appropriate order based on the sorted by
        // * @returns {} nothing
        // */
        function _updateOrderExpressions() {
            var i /*loop index*/;

            var order = _internal.columns.getSortOrder();

            if (_internal.cfg.debug === true) {
                _log("ORDER: Sorting " + order.length + " columns.");

                for (i = 0; i < order.length; i++) {
                    _log("Sort Order: Item " + i + " [" + order[i].colkey + "]: " + order[i].key + "  desc:" + order[i].desc);
                }
            }

            _internal.pageInfo.setPageNumber(1);
            _internal.pageInfo.setPagingCookie("");
        }

        ///**
        // * _sort: Sorts the items in the table
        // * @returns {} 
        // */
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
                            return $.text([a1[item.key]]) > $.text([b1[item.key]])
                                ? item.desc ? -1 : 1
                                : item.desc ? 1 : -1;
                        }
                    }
                    return 0;
                };

                var defer = new $.Deferred();

                setTimeout(function () {
                    _internal.table.find("tbody > tr").sortElements(cmp);
                    defer.resolve();
                },
                    0);

                return defer.promise();
            } else {
                return _retrieveMultiple();
            }
        }

        ///**
        //  * _toggleColumnSort: Sorts a column
        //  * @param {} th : the th
        //  * @returns {} 
        //  */
        function _toggleColumnSort(th) {
            var colkey = th.data("key");
            if (!colkey) return; // no key no sort.
            var col = _internal.columns.getColumn(colkey);
            if (!col) return; // no col no sort.
            _internal.columns.toggleColumnSort(col);

            var cols = _internal.columns.getSortColumns(col.getSortKey())
               ,headers = th.siblings()
               ,key;
            $.each(cols, function (i, c) {
                key = c.getKey();
                $.each(headers, function (j, h) {
                    if (key === $(h).data("key")) {
                        $(h).attr('aria-sort', c.getSortCondition());
                    }
                });
            });
            return _sort();
        }

        ///**
        // * _isTable: Is Render Type table?
        // * @returns {} TRUE if table, FALSE if not
        // */
        function _isTable() {
            return _internal.cfg.RenderType === 'table';
        }


        ///**
        // * _isCard: Is Render Type card?
        // * @returns {} TRUE if card, FALSE if not
        // */
        function _isCard() {
            return _internal.cfg.RenderType === 'card';
        }

        ///**
        // * _clearSelection: Clears any selections.
        // * @returns {} 
        // */
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
                if (_isTable()) {
                    _internal.table.find("tbody > tr").removeClass(XrmTable.Constants.CSSClass.SELECTED);
                    _internal.selected.length = 0;
                    _internal.table.attr('aria-activedescendant', "");
                }
                dfd.resolve.apply(this, arguments); //pass thru args
                return dfd.promise();
            }).then((new _event('postClearSelection')).call);
        }

        ///**
        // * _select: Select a row
        // * @param {} row : the row to be selected
        // * @returns {} 
        // */
        function _select(row) {
            var data = {
                row: row,
                selected: _internal.selected
            };

            return new _event("preSelect", data).call()
                .then((new _event("click", data)).call)
                .then(function () {
                    var dfd = new $.Deferred();
                    if (data.row.hasClass(XrmTable.Constants.CSSClass.SELECTED)) {
                        data.row.removeClass(XrmTable.Constants.CSSClass.SELECTED);
                        // remove item
                        _internal.selected.splice($.inArray(row.attr("id"), _internal.selected), 1);
                        dfd.resolve();
                    } else {
                        if (!_internal.cfg.MultiSelect && _internal.selected.length > 0) {
                            return _clearSelection(row).then(function () {
                                row.addClass(XrmTable.Constants.CSSClass.SELECTED);
                                _internal.table.attr('aria-activedescendant', data.row.attr("id"));
                                _internal.selected.push(data.row.attr("id"));
                                data.selected = _internal.selected;
                            });
                        }
                        row.addClass(XrmTable.Constants.CSSClass.SELECTED);
                        _internal.selected.push(data.row.attr("id"));
                        data.selected = _internal.selected;
                        dfd.resolve(); // no pass through on select
                    }
                    return dfd.promise();
                }).then((new _event("postSelect", data)).call);
        }

        ///**
        // * _gotoPage: Goes to a page
        // * @param {} pageNumber : the page number to go to
        // * @returns {} 
        // */
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

        ///**
        // * _firstPage: go to the first page.
        // * @returns {} 
        // */
        function _firstPage() {
            return _gotoPage(1);
        }

        ///**
        // * _prevPage: go to the previous page.
        // * @returns {} 
        // */
        function _prevPage() {
            return _gotoPage(_internal.pageInfo.getPageNumber() - 1);
        }

        ///**
        // * _nextPage: go to the next page
        // * @returns {} 
        // */
        function _nextPage() {
            return _gotoPage(_internal.pageInfo.getPageNumber() + 1);
        }

        ///**
        // * _lastPage: go to the last page.
        // * @returns {} 
        // */
        function _lastPage() {
            return _gotoPage(_internal.pageInfo.getTotalPages());
        }

        ///**
        // * _changePage: Handle the Keyboard Events to Change the Page.
        // * @param {} key : the key pressed
        // * @param {} ctrl : true if ctrl was held down, false if not
        // * @returns {} 
        // */
        function _changePage(key, ctrl) {
            if (!key) {
                return;
            }

            if (key === XrmTable.Constants.KeyCode.PAGE_UP || key === XrmTable.Constants.KeyCode.PAGE_DOWN) {
                var promise;
                if (ctrl) {
                    if (key === XrmTable.Constants.KeyCode.PAGE_UP) {
                        promise = _firstPage();
                    } else {
                        promise = _lastPage();
                    }
                } else {
                    if (key === XrmTable.Constants.KeyCode.PAGE_UP) {
                        promise = _prevPage();
                    } else {
                        promise = _nextPage();
                    }
                }

                promise.then(function () {
                    var cell = _internal.table.find("tbody > tr:first > td:first:not(." +
                        XrmTable.Constants.CSSClass.HIDDEN +
                        ")");
                    _focus(cell);
                });
            }
        };

        ///**
        // * _getCell: Gets a cell in the direction specified.
        // * @param {} o : the current cell
        // * @param {} direction : the direction to go in, XrmTable.Constants.Direction.*
        // * @param {} visibleonly : true if only move to visible cells
        // * @returns {} The cell navigated to, if unsuccessful, it will return the current cell.
        // */
        function _getCell(o, direction, visibleonly) {
            var cell = $(o);
            if (!cell.is("td") && !cell.is("th")) {
                cell = cell.closest("td");
            }

            var selector = "";
            if (visibleonly) {
                selector = ":not(." + XrmTable.Constants.CSSClass.HIDDEN + ")";
            }

            var row = $(o).closest("tr");

            switch (direction) {
                case XrmTable.Constants.Direction.LEFT:
                    var left = cell.prev(selector);
                    if (left.length === 0) return cell;
                    return left;
                case XrmTable.Constants.Direction.RIGHT:
                    var right = cell.next(selector);
                    if (right.length === 0) return cell;
                    if (visibleonly) {
                        // this is a catch for right movement not triggered by keyboard press, but instead by hiding a column
                        if (cell.is("." + XrmTable.Constants.CSSClass.HIDDEN))
                            return _getCell(cell, XrmTable.Constants.Direction.LEFT, true);
                    }
                    return right;
                case XrmTable.Constants.Direction.UP:
                    var up = row.prev(selector);
                    while (up.length !== 0 && (up.css('display') === 'none' || up.is(":hidden")))
                        up = up.prev(selector);
                    if (up.length === 0) {
                        if (_internal.cfg.AllowSort && row.parent().is("tbody")) {
                            up = _internal.table.find("thead > tr:last");
                        } else {
                            up = row;
                        }
                    }
                    if (up.children().length <= _internal.focuscol) {
                        return $(up.children().get(up.children().length - 1));
                    }
                    return $(up.children().get(_internal.focuscol));
                case XrmTable.Constants.Direction.DOWN:
                    var down = row.next(selector);
                    while (down.length !== 0 && (down.css('display') === 'none' || down.is(":hidden")))
                        down = down.next(selector);
                    if (down.length === 0) {
                        if (_internal.cfg.AllowSort && row.parent().is("thead")) {
                            down = _internal.table.find("tbody > tr:first");
                        } else {
                            down = row;
                        }
                    }
                    if (down.children().length <= _internal.focuscol) {
                        return $(down.children().get(down.children().length - 1));
                    }
                    return $(down.children().get(_internal.focuscol));
                case XrmTable.Constants.Direction.HOME:
                    return cell.siblings(":first" + selector);
                case XrmTable.Constants.Direction.END:
                    return cell.siblings(":last" + selector);
                default:
                    return cell;

            }
        }

        ///**
        // * _toggleColumnVisibility: Toggles a columns visibility
        // * @param {} key : the column key
        // * @returns {} 
        // */
        function _toggleColumnVisibility(key) {
            var col = _internal.columns.getColumn(key);
            var visible = col.getVisible();
            _setColumnVisibility(col, !visible);
        }

        ///**
        // * _setColumnVisibility: Sets the column visibility
        // * @param {} col : the XrmTable.Column
        // * @param {} visible : true if visible, false for hidden
        // * @returns {} 
        // */
        function _setColumnVisibility(col, visible) {
            var focus = $(":focus");
            var colIndex = col.getRenderIndex();

            var cellSelector = 'td[aria-colindex="' + colIndex + '"]';
            var columnCells = _internal.table.find(cellSelector);

            col.setVisible(visible);

            columnCells.each(function (i, v) {
                if (visible) {
                    $(v).removeClass(XrmTable.Constants.CSSClass.HIDDEN);
                } else {
                    $(v).addClass(XrmTable.Constants.CSSClass.HIDDEN);
                }
            });

            if (focus.is("." + XrmTable.Constants.CSSClass.HIDDEN)) {
                // if the focused element was hidden, then shift to the element to the right of it.
                var cell = _getCell(focus, XrmTable.Constants.Direction.RIGHT, true);
                _focus(cell);
            }
        }

        ///**
        // * _eventClick: Handle clicks on cells
        // * @param {} event : the click event
        // * @returns {} 
        // */
        function _eventClick(event) {
            var o = $(this), row = null;
            if (!o.is("[tabindex]")) {
                o = o.closest("[tabindex]");
            }
            if (o.is("button[aria-haspopup]")) return;
            _focus(o);

            if (o.is("th")) {
                if (_internal.cfg.AllowSort) _toggleColumnSort(o);
                return; // do not trigger click event
            } else if (o.is("td")) {
                row = o.closest("tr." + XrmTable.Constants.CSSClass.ROW);
                if (row && _internal.cfg.AllowSelect) _select(row);
            } else if (o.is("div")) {
                if (_internal.cfg.AllowSelect) _select(o);
            }

            new _event("click",
            {
                row: row,
                cell: o
            }).call();
        }

        ///**
        // * _eventDblClick: Handle the Double Click event on cells
        // * @param {} event : the click event
        // * @returns {} 
        // */
        function _eventDblClick(event) {
            var o = $(this), row = null;
            if (!o.is("[tabindex]")) {
                o = o.closest("[tabindex]");
            }
            if (o.is("button[aria-haspopup]")) return;
            _focus(o);

            if (o.is("th")) {
                return; // do not trigger click event on header doubleclick
            } else if (o.is("td")) {
                row = o.closest("tr." + XrmTable.Constants.CSSClass.ROW);
            }

            new _event("dblclick",
            {
                row: row,
                cell: o
            }).call();

        }

        ///**
        // * _eventKeyDown: Handle the key down / press events on cells
        // * @param {} event : the keydown event
        // * @returns {} 
        // */
        function _eventKeyDown(event) {
            if (!event || _internal.cfg.NavigationDisabled) {
                return;
            }

            var key = event.which || event.keyCode;
            if (key === XrmTable.Constants.KeyCode.SHIFT) return;
            _log("Key Pressed: " + key);

            switch (key) {
                case XrmTable.Constants.KeyCode.UP:
                case XrmTable.Constants.KeyCode.DOWN:
                case XrmTable.Constants.KeyCode.LEFT:
                case XrmTable.Constants.KeyCode.RIGHT:
                case XrmTable.Constants.KeyCode.HOME:
                case XrmTable.Constants.KeyCode.END:
                    var cell = _getCell($(this), key, true);
                    _focus(cell);
                    event.preventDefault();
                    break;
                case XrmTable.Constants.KeyCode.PAGE_UP:
                case XrmTable.Constants.KeyCode.PAGE_DOWN:
                    if (_internal.cfg.KeyboardPaginationEnabled) {
                        _changePage(key, event.ctrlKey);
                        event.preventDefault();
                    }
                    break;
                case XrmTable.Constants.KeyCode.ENTER:
                case XrmTable.Constants.KeyCode.SPACE:
                    if (_internal.cfg.AllowSelect) {
                        $(event.target).trigger("click");
                        event.preventDefault();
                    }
                    break;
                case XrmTable.Constants.KeyCode.TAB:
                    if (e.shiftKey) {
                        _internal.table.prev('input:not([tabindex^="-"]),select:not([tabindex^="-"]),input:not([tabindex^="-"]),[tabindex]:not([tabindex^="-"])').focus();
                        e.preventDefault();
                    }
                    break;
                default:
                    return;
            }
        }

        ///**
        // * _registerEvents: Registers the events using the parent object, allieviating the need 
        // * to re-register items during the refresh
        // * @returns {} 
        // */
        function _registerEvents() {
            if (_isTable()) {
                /**************************** TABLE EVENTS ***********************/
                _internal.parent.off("click", "table > tbody > tr > td", _eventClick);
                _internal.parent.on("click", "table > tbody > tr > td", _eventClick);

                _internal.parent.off("focusin", "table > tbody > tr > td", _eventCellFocusIn);
                _internal.parent.on("focusin", "table > tbody > tr > td", _eventCellFocusIn);
                
                _internal.parent.off("focusout", "table > tbody > tr > td", _eventCellFocusOut);
                _internal.parent.on("focusout", "table > tbody > tr > td", _eventCellFocusOut);

                _internal.parent.off("dblclick", "table > tbody > tr > td", _eventDblClick);
                _internal.parent.on("dblclick", "table > tbody > tr > td", _eventDblClick);

                _internal.parent.off("keydown", "table > tbody > tr > td", _eventKeyDown);
                _internal.parent.on("keydown", "table > tbody > tr > td", _eventKeyDown);

                if (_internal.cfg.AllowSort) {
                    _internal.parent.off("keydown", "table > thead > tr > th", _eventKeyDown);
                    _internal.parent.on("keydown", "table > thead > tr > th", _eventKeyDown);

                    _internal.parent.off("click", "table > thead > tr > th", _eventClick);
                    _internal.parent.on("click", "table > thead > tr > th", _eventClick);

                    _internal.parent.off("dblclick", "table > thead > tr > th", _eventDblClick);
                    _internal.parent.on("dblclick", "table > thead > tr > th", _eventDblClick);
                }

                _internal.parent.off("focusin", "table", _eventFocusIn);
                _internal.parent.on("focusin", "table", _eventFocusIn);
            }

            else if (_isCard()) {
                /**************************** CARD EVENTS ***********************/

            }
        }

        function _eventCellFocusOut(e) {
            var el = $(this);
            if (el.is("TD")) {
                el.parent().removeClass("kb-hover");
            }
        }

        function _eventCellFocusIn(e) {
            var el = $(this);
            if (el.is("TD")) {
                el.parent().addClass("kb-hover");
                _internal.focus = el;
            }
        }

        function _eventFocusIn(event) {
            //null relatedTarget means it is from itself (like shift tab)
            var from = $(event.relatedTarget || event.target), to = $(event.target);

            if (_isTable())
            {
                var toXrmTable = to.attr("id") === _internal.table.attr("id"),
                    fromXrmTable = from.attr("id") === _internal.table.attr("id"),
                    fromInTable = _internal.table.find(from).length > 0,
                    toInTable = _internal.table.find(to).length > 0;

                var fromTop = (_internal.cfg.caption && from.is("CAPTION") && fromInTable) ||
                              (!_internal.cfg.caption && _internal.table.find("tbody:first > tr:first > td:first").attr("id") === from.attr("id"));
                    

                if (toXrmTable || toInTable || fromInTable) {
                    if (fromTop) {
                        //focus from caption to something in table
                        if (toXrmTable || (to.is("CAPTION") && toInTable)) {
                            //SHIFTTAB
                            var tabitems = $('input:not([tabindex^="-"]),select:not([tabindex^="-"]),input:not([tabindex^="-"]),[tabindex]:not([tabindex^="-"])').sort(function (a, b) { return (0 || a.tabindex) > 0 || (0 || b.tabindex); });
                            var tableIndex = -1;
                            var prevItem = null;
                            tabitems.each(function (i, v) {
                                if (v.id === _internal.table.attr("id")) tableIndex = i;
                                if (tableIndex === -1) prevItem = v;
                            });
                            prevItem.focus();
                            event.stopPropagation();
                            event.preventDefault();
                            return;
                        }
                        _internal.table.find("tbody:first > tr:first > td:first").focus();
                        event.stopPropagation();
                        event.preventDefault();
                        return;
                    }
                    if (toXrmTable && _internal.cfg.caption && !fromInTable) {
                        _internal.table.find("caption:first").focus();
                        event.stopPropagation();
                        event.preventDefault();
                        return;
                    }
                    if (toInTable && !fromInTable && !fromXrmTable) {
                        //focus target: in Table
                        if (to.is("TD") || to.is("TR") || from.is("HTML")) {
                            //focus target: TR or TD
                            if (_internal.cfg.RememberFocus && _internal.focus) {
                                //return to remembered focus point
                                _focus(_internal.focus);
                                event.stopPropagation();
                                event.preventDefault();
                                return;
                            }
                            // Got to cell 1
                            _internal.table.find("tbody:first > tr:first > td:first").focus();
                            event.stopPropagation();
                            event.preventDefault();
                            return;
                        }
                    }
                }
            }
            
        }

        function _focus(el) {
            if (!el.is(":focus")) el.focus();
            if (el.is("[aria-colindex]")) {
                _internal.focuscol = el.attr("aria-colindex");
            }
            _internal.focus = el.id;
        }

        ///**
        // * _render: Renders the data
        // * @param {} data : the data from a retrieveMultiple to render
        // * @returns {} 
        // */
        function _render(data) {
            switch (_internal.cfg.RenderType) {
                case "table":
                    return _renderTable(data);
                case "card":
                    return _renderCards(data);
            }
        }

        ///**
        // * _renderCards: Renders Cards
        // * TODO: This needs to be revisited and events need to be registered in the _registerEvents
        // * @param {} data 
        // * @returns {} 
        // */
        function _renderCards(data) {
            if (!_internal.cards) _internal.cards = _internal.parent;
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
                        var card = $("<div id='" + data.rowid + " class='col-md-4 xrmcard'/>");
                        var panel = $("<div class='panel panel-primary xrmcardpanel'/>");
                        card.append(panel);
                        if (_internal.cfg.AllowSelect) {
                            card.click(function () {
                                _select(card);
                            });
                            if ($.inArray(data.rowid, _internal.selected) !== -1) {
                                card.addClass(XrmTable.Constants.CSSClass.SELECTED);
                            }
                        } else {
                            card.click(function () {
                                new _event("click",
                                {
                                    card: card
                                }).call();
                            });
                        }
                        card.dblclick(function () {
                            new _event("dblclick",
                            {
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

                        var j, col, cell, renderData, k=0,cellid;

                        for (j in cols) {
                            col = cols[j];
                            cellid = data.rowid+"-field_"+k;
                            cell = $("<div/>")
                                   .addClass(col.getClass())
                                   .addClass("text-left")
                                   .data("attributeName", col.key)
                                   .attr("id", cellid);

                            var name = col.getName();
                            renderData = col.Render(data, rows, col, xt);

                            if (typeof renderData === "undefined" ||
                                renderData === null ||
                                (renderData.trim && renderData.trim() === "")) {
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
                            }

                            if (col.getRenderType() === "column") {
                                var label = $("<div class='col-md-2 text-right'><b>" + name + ": </b></div>")
                                            .attr("id", cellid + "-label");
                                cell.attr("aria-labelledby", cellid + "-label");
                                if (_internal.cfg.AllowTabOnCardFields) cell.attr("tabIndex", 0);
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

            var waitfor = [];
            if (data.push !== undefined) {
                for (i in data) {
                    waitfor.push(createCard(i, data[i]).then(appendRow));
                }
            } else {
                waitfor.push(createCard('item', data).then(appendRow));
            }

            return $.when.apply($, waitfor).done(function () {
                _internal.cards.find("div.xrmtable-cards").remove();
                _internal.cards.append(div);
                _internal.cards.show();
            }).then(function () {
                // call postRender
                return new _event('postRender', data).call();
            });
        }

        function _renderTable(data) {
            /*********** Get Columns ****************/
            if (_internal.columns.getAllColumns()) _internal.columns = _getAllColumns(data);

            var i,
                cols = _internal.columns.getColumns(),
                colspan = 0;
            for (i in cols) {
                if (cols[i].getRenderType() === "column") {
                    colspan++;
                }
            }

            function createTableHeader(parent, cols, data) {
                var order = _internal.columns.getSortOrder();
                function createColumnHeader(i, col) {
                    col.setRenderIndex(i);

                    var th = $("<th/>")
                        .attr("scope", "col")
                        .prop("title", col.getTooltip())
                        .data("key", col.getKey())
                        .attr("aria-colindex", i)
                        .attr("id", _internal.table.attr("id") + "-col-" + col.getKey())
                        .text(col.getName());

                    if (order.length > 0) {
                        if (col.getSortIndex()>=0) {
                            var sort = col.getSortCondition();

                            var glyph = { set: false, className: "", alt: "", tooltip: "" };
                            if (sort === XrmTable.Constants.SortType.ASCENDING) {
                                glyph.className = XrmTable.Constants.CSSClass.SORT_GLYPH_ASCENDING;
                                glyph.alt = " sorted in ascending order ";
                                glyph.tooltip = "Sorted Ascending";
                                glyph.set = true;
                            } else if (sort === XrmTable.Constants.SortType.DESCENDING) {
                                glyph.className = XrmTable.Constants.CSSClass.SORT_GLYPH_DESCENDING;
                                glyph.alt = " sorted in descending order ";
                                glyph.tooltip = "Sorted Descending";
                                glyph.set = true;
                            }

                            if (glyph.set) th.append(
                                                $("<span/>")
                                                 .addClass("sr-only")
                                                 .attr("aria-hidden", "false")
                                                 .text(glyph.alt),
                                                $("<span/>")
                                                 .addClass(glyph.className)
                                                 .attr("aria-hidden", "true")
                                                 .attr("title", glyph.tooltip)
                                                
                                           );

                        }
                    }

                    _addClasses(th, ["tr > th", "th", "thead > tr > th"]);

                    if (_internal.cfg.AllowSort) {
                        th.attr('aria-sort', col.getSortCondition());
                        // If it is sortable, then it should be able to be clicked on and moved to using the kb
                        th.attr("tabIndex", "-1");
                    }
                    return th;
                }

                var thead = $("<thead/>")
                            .addClass(XrmTable.Constants.CSSClass.THEAD);
                if (_internal.cfg.caption) thead.addClass(XrmTable.Constants.CSSClass.THEADWITHCAPTION);
                _addClasses(thead);

                var tr = $("<tr/>");
                _addClasses(tr, ["tr", "thead > tr"]);

                var j = 0;

                for (i in cols) {
                    if (cols[i].getRenderType() === "column") {
                        tr.append(createColumnHeader(j, cols[i]));
                        j++;
                    }
                }
                parent.attr("aria-colcount", j);
                thead.append(tr);
                parent.find("thead").remove();
                parent.append(thead);

                var dfd = new $.Deferred();
                dfd.resolve(parent, data);
                return dfd.promise();
            }

            function createTableBody(parent, data) {
                function createRow(id, data) {
                    data.id = id;
                    data.rowid = "row-" + id;
                    return new _event("preRenderRow", data).call()
                        .then(function () {
                            var rows = [];
                            var row = $("<tr/>")
                                .attr("id", data.rowid)
                                .attr("aria-level", "1");
                            rows.push(row);

                            _addClasses(row, ["tr", "tbody > tr"]);

                            if (_internal.cfg.AllowSelect) {
                                row.addClass(XrmTable.Constants.CSSClass.ROW);
                            }

                            if (_internal.selected.length > 0) {
                                for (j in _internal.selected) {
                                    if (_internal.selected[j] === data.rowid)
                                        row.addClass(XrmTable.Constants.CSSClass.SELECTED);
                                }
                            }

                            if (_internal.cfg.AllowDrag) {
                                row.attr('aria-grabbed','false');
                                row.draggable({
                                    cursor: "move",
                                    helper: "clone",
                                    revert: "invalid",
                                    drag: function () {
                                        $(this).attr('aria-grabbed', 'true');
                                        new _event('dragStart', $(this)).call();
                                    },
                                    stop: function () {
                                        $(this).attr('aria-grabbed', 'false');
                                        new _event('dragStop', $(this)).call();
                                    }
                                });

                            }

                            if (typeof data.view === "function") {
                                row.data("obj", data.view());
                            } else {
                                row.data("obj", data);
                            }

                            var j,
                                col,
                                cell,
                                renderData;
                            for (j in cols) {
                                col = cols[j];
                                cell = $("<td/>")
                                    .data("attributeName", col.getKey())
                                    .attr("id", row.attr("id") + "-" + col.getRenderIndex())
                                    .attr("aria-colindex", col.getRenderIndex())
                                    .attr("tabIndex", "-1");

                                _addClasses(cell, ["td", "tbody > tr > td", "tr > td"]);

                                if (col.getClass() && col.getClass() !== "") {
                                    cell.addClass(col.getClass());
                                }

                                renderData = col.Render(data, rows, col, xt);
                                if (typeof renderData === "string") {
                                    cell.text(renderData);
                                } else if (renderData instanceof jQuery && renderData.prop("tagName") === "TR") {
                                    rows.push(renderData);
                                    _addClasses(row, ["tr", "tbody > tr"]);
                                }

                                if (col.getRenderType() === "column") {
                                    row.append(cell);
                                    if (col.getSRName() && col.getSRName() !== "") {
                                        var span = $("<span/>").addClass("sr-only").text(col.getSRName() + ": ");
                                        cell.prepend(span);
                                    }
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
                if (data.push !== undefined) {
                    for (i in data) {
                        var id = i;
                        if (data[i].queueitemid) {
                            id = data[i].queueitemid;
                        }
                        waitfor.push(createRow(id, data[i]).then(appendRow));
                    }
                } else {
                    waitfor.push(createRow('item', data).then(appendRow));
                }

                parent.find("tbody").remove();
                parent.append(tbody);

                var dfd = new $.Deferred();
                $.when.apply($, waitfor).done(function () {
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
                            li.addClass(XrmTable.Constants.CSSClass.PAGENUMBER_ACTIVE);
                        }
                    }
                    if (onclick && typeof onclick === "function") {
                        a.click(onclick);
                    } else {
                        a.click(function () {
                            var items = a.parent().parent().find("li");
                            items.removeClass(XrmTable.Constants.CSSClass.PAGENUMBER_ACTIVE);
                            items.removeClass(XrmTable.Constants.CSSClass.PAGENUMBER_DISABLED);
                            if (page === _internal.pageInfo.getTotalPages() || !_internal.pageInfo.getMoreRecords()) {
                                var last = items.filter(function (i, e) {
                                    e = $(e);
                                    return e.text().indexOf("last") !== -1 || e.text().indexOf("next") !== -1;
                                });
                                last.addClass(XrmTable.Constants.CSSClass.PAGENUMBER_DISABLED);
                            } else if (page === 1) {
                                var first = items.filter(function (i, e) {
                                    e = $(e);
                                    return e.text().indexOf("first") !== -1 || e.text().indexOf("prev") !== -1;
                                });
                                first.addClass(XrmTable.Constants.CSSClass.PAGENUMBER_DISABLED);
                            }
                            a.parent().addClass(XrmTable.Constants.CSSClass.PAGENUMBER_ACTIVE);
                            _gotoPage(page);
                        });
                    }
                    if ((label.indexOf("prev") !== -1 || label === "first") && _internal.pageInfo.getPageNumber() === 1) {
                        li.addClass(XrmTable.Constants.CSSClass.PAGENUMBER_DISABLED);
                    } else if ((label === "next" || label === "last") &&
                        _internal.pageInfo.getPageNumber() === (_internal.pageInfo.getTotalPages())) {
                        li.addClass(XrmTable.Constants.CSSClass.PAGENUMBER_DISABLED);
                    } else if (page === label && _internal.pageInfo.getPageNumber() === page) {
                        li.addClass(XrmTable.Constants.CSSClass.PAGENUMBER_ACTIVE);
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

                    var paging = $('<ul/>').addClass(XrmTable.Constants.CSSClass.PAGENUMBER_AREA);
                    cell.append(paging);

                    var first = [
                        $('<span aria-hidden="true" title="first"/>')
                        .addClass(XrmTable.Constants.CSSClass.PAGENUMBER_GLYPH_FIRST),
                        $('<span class="sr-only">first</span>')
                    ];
                    paging.append(createPageButton(1, first));

                    var prev = [
                        $('<span aria-hidden="true" title="prev"/>')
                        .addClass(XrmTable.Constants.CSSClass.PAGENUMBER_GLYPH_PREV), $('<span class="sr-only">prev</span>')
                    ];
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

                    var next = [
                        $('<span aria-hidden="true" title="next"/>')
                        .addClass(XrmTable.Constants.CSSClass.PAGENUMBER_GLYPH_NEXT), $('<span class="sr-only">next</span>')
                    ];
                    paging.append(createPageButton(page + 1, next));
                    var last = [
                        $('<span aria-hidden="true" title="last"/>')
                        .addClass(XrmTable.Constants.CSSClass.PAGENUMBER_GLYPH_LAST), $('<span class="sr-only">last</span>')
                    ];
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

            // Create the table if we don't have one yet....
            if (_internal.table === null) {
                _internal.table = $("<table/>")
                         .attr("tabIndex", "0")
                         .addClass(XrmTable.Constants.CSSClass.TABLE);
                if (typeof _internal.cfg.table === "string") {
                    _internal.table.attr("id", _internal.cfg.table);
                }

                if (_internal.cfg.caption) {
                    var caption = $("<caption/>")
                    caption.attr("contenteditable", "false");
                    caption.on("keydown", function (e) {
                        var key = e.which || e.keyCode;
                        if (key === XrmTable.Constants.KeyCode.TAB && !e.shiftKey) {
                            if (!_internal.cfg.NavigationDisabled) {
                                _internal.table.find("tbody:first > tr:first > td:first").focus();
                                e.preventDefault();
                                e.stopPropagation();
                            }
                        }
                    });

                    var captionText = _internal.cfg.caption;
                    if (_internal.cfg.caption.label) {
                        captionText = _internal.cfg.caption.label;
                    }

                    caption.text(captionText);

                    if (_internal.cfg.caption.hasOwnProperty("tabindex")) {
                        caption.attr("tabindex",_internal.cfg.caption.tabindex);
                    }

                    caption.addClass(XrmTable.Constants.CSSClass.CAPTION);
                    _internal.table.append(caption);
                }

                _internal.table.hide();
                _internal.parent.append(_internal.table);
            }

            // Get current focus if it is on the table...
            var focuson = null;
            if ($.contains(_internal.table[0], document.activeElement)) {
                if (document.activeElement.id) {
                    focuson = document.activeElement.id;
                }
            }

            return createTableHeader(_internal.table, cols, data)
                .then(createTableBody)
                .then(createTableFooter)
                .then(function (table) {
                    table.show();

                    $(document).scrollTop(pos);

                    var postRenderData = {
                        selected: _internal.selected,
                        data: data
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
        this.init = function () {
            console.log("in this.init");
            console.log(options);
            return _init(element, options);
        };
        this.clearSelection = _clearSelection;

        ///****************** GETTERS AND SETTERS *****************/
        this.getViewRecordCount = function () {
            return _internal.viewRecordCount;
        };
        this.setTable = function (o) {
            if (_internal.cfg.RenderType!=='table') return;
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
            var jqSearchItems = null;
        
            if (_internal.cfg.RenderType ==='table') {
                jqSearchItems = _internal.table.find("tbody > tr");
            } else if (_internal.cfg.RenderType ==='card') {
                jqSearchItems = _internal.parent.find("div.xrmcard");
            }

            if (jqSearchItems && jqSearchItems.length>0) {
                // Table View
                jqSearchItems.each(function (e) {
                    var me = $(this);
                    var id = me.attr('id');
                    if (id) {
                        // they are all considered rows even if they are rendered differently.
                        if (id.indexOf("row-") > -1) {
                            items.push(id.substring(4));
                        }
                    }
                });
            }
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
            console.log("setting query");
            _internal.refreshing = false;
            if (typeof v === "string") {
                _internal.cfg.query = v;
                _internal.query = v;
            } else {
                throw new
                    TypeError("Query must be a string fetchxml, the type Sdk.Query.FetchExpression or Sdk.Query.QueryExpression.");
            }
        };
        this.setEntity = function (e) {
            if (typeof e === "string") {
                _internal.cfg.entity = e;
                _internal.entity = e;
            }
        };
        this.getQuery = function () {
            return _internal.query;
        };

        this.setOrder = function (keys) {
            if (keys.push === undefined) {
                keys = keys.split(',');
            }

            $.each(_internal.columns, function (i, v) {
                v.setSortIndex(-1);
            });

            if (keys === undefined || keys === null) return;

            if (keys.push === undefined) {
                keys = keys.split(',');
            }

            var i,j,
                col,cols
                parts;
            for (i in keys) {
                parts = keys[i].split(' ');
                cols = _internal.columns.getSortColumns(parts[0]);
                for (j in cols) {
                    col = cols[j];

                    col.setSortIndex(i);
                    if (parts.length === 1 || parts[1] !== "desc") {
                        col.setSortCondition(XrmTable.Constants.SortType.ASCENDING);
                    } else {
                        col.setSortCondition(XrmTable.Constants.SortType.DESCENDING);
                    }
                }
            }
        };
        this.getOrder = function () {
            return _internal.columns.getSortOrder();
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
        this.destroy = function () {
            if (_internal.table) {
                _internal.table.remove();
                _internal.table = null;
            }
            if (_internal.cards) {
                _internal.cards.remove();
                _internal.cards = null;
            }
            _internal = null;
            return $.when();
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

        return this;
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
                table.refresh(true);
            }
        });

        if (tbls.length === 1) {
            return $(tbls[0]);
        }

        return $(tbls);
    };

    $.fn.xrmTable.defaults = {};
})(jQuery);