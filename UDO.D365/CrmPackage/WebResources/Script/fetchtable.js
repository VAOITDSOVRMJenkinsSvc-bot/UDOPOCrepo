var FetchTable = function (options) {
    'use strict';

    var _windowSize = {
        h: 0,
        w: 0
    };
    var _resizeAttached = false;
    var _cache = null;
    var context = Xrm.Utility.getGlobalContext();
    //var webApi;


    function openEntityForm(targetEntity, id) {
        //TODO: change to form context
        var url = context.getClientUrl + "/main.aspx?etn=" + targetEntity + "&pagetype=entityrecord&id=" + id;
        //TODO: change to Xrm.Navigation
        window.open(url);
    }
    // This function checks for undefined, null, not of type, and empty (if array or string)
    function UndefinedNullOrEmpty(obj, type) {
        if (typeof obj === 'undefined' || obj === null) return true;
        if (typeof type !== 'undefined' && type !== null && typeof obj !== type) return true;
        // make sure we only check the length for strings and arrays.
        if ((typeof obj === 'string' || Object.prototype.toString.call(obj) === '[object Array]') && obj.length == 0) return true;
        return false;
    }

    // Used to quickly build elements and attach onclick events
    function el(tag, attr, text, action, data) {
        if (typeof attr == 'string') { attr = { 'class': attr }; }
        var pre = "<" + tag;
        for (var k in attr) {
            pre += " " + k + "='" + attr[k] + "'";
        }
        var obj = $(pre + "></" + tag + ">");
        if (!UndefinedNullOrEmpty(text)) { obj.text(text); }
        if (typeof action == 'function') {
            if (data !== null) {
                obj.click(data, action);
            } else {
                obj.click(action);
            }
        }
        return obj;
    }

    function showLoading() {
        $("#tmpDialog").show();
        $("#tmpDialog").focus();
    }

    function hideLoading() {
        $("#tmpDialog").hide();
    }

    function errorCallback(error) {
        alert(error);
    }

    var config = {
        entityname: "entityname",
        paginginfo: {
            page: 1,
            count: 100,
            pagingcookie: null
        },
        columns: [],
        table: {},
        requiredconditions: [],
        showfilterdetails: false,
        innerscroll: false
    };

    var _table = null, _settings = $.extend(true, config, options);
    _settings.paginginfo = $.extend(true, config.paginginfo, options.paginginfo);

    var defaultcolumn = {
        filter: {
            conditions: [],
            conditionDescriptions: [],
            options: null,
            selected: null,
            clear: function () {
                $(".filter-popup").remove();
            },
            refresh: function (h) {
                var col = _settings.columns[h.data];
                col.filter.clear();
                refreshtable(true);
            },
            'do': function (h) {
                var col = _settings.columns[h.data];
                col.filter.conditions = [];
                col.filter.conditionDescriptions = [];
                if (col.datatype == 'string') {
                    var data = $("#contains_" + col.fieldname).val();
                    col.filter.conditions.push({ attribute: col.fieldname, operator: 'like', value: '%' + data + '%' });
                    col.filter.conditionDescriptions.push(col.displayname + ': contains "' + data + '"');
                } else if (col.datatype == 'datetime') {
                    col.filter.selected = parseInt($("input:radio[name=fo_" + col.fieldname + "]:checked").val());
                    var dateOption = col.filter.options[col.filter.selected];
                    for (var k = 0; k < dateOption.Conditions.length; k++) {
                        col.filter.conditions.push(dateOption.Conditions[k]);
                    }
                    col.filter.conditionDescriptions.push(col.displayname + ': ' + dateOption.Name);
                } else if (col.datatype == 'lookup') {
                    var data = new Array();
                    var descArr = new Array();
                    var values = new Array();
                    $("input:checkbox[name=fo_" + col.fieldname + "]:checked").each(function (i, v) {
                        values[i] = v.value;
                    });
                    if (typeof values === 'string') values = [values];
                    col.filter.selected = values;
                    for (var j = 0; j < values.length; j++) {
                        var lookupindex = values[j];
                        if (lookupindex != 'all') {
                            var opt = col.filter.options[values[j]];
                            if (!UndefinedNullOrEmpty(opt.Value)) {
                                data.push(opt.Value);
                            }
                            if (!UndefinedNullOrEmpty(opt.Name)) {
                                descArr.push(opt.Name);
                            }
                        }
                    }
                    var condition = { attribute: col.fieldname, operator: 'in', value: data };
                    col.filter.conditions.push(condition);
                    if (descArr.length > 1) descArr[descArr.length - 1] = 'or ' + descArr[descArr.length - 1];
                    col.filter.conditionDescriptions.push(col.displayname + ': is ' + descArr.join(', '));
                }
                col.filter.clear();
                refreshtable();
            },
            clearSort: function () {
                for (var i in _settings.columns) {
                    _settings.columns[i].sort = "";
                }
            },
            sortAtoZ: function (h) {
                var col = _settings.columns[h.data];
                col.filter.clearSort();
                col.sort = "asc";
                col.filter.refresh(h);
            },
            sortZtoA: function (h) {
                var col = _settings.columns[h.data];
                col.filter.clearSort();
                col.sort = "desc";
                col.filter.refresh(h);
            },
            show: function (h) {
                var col = _settings.columns[h.data];
                var builditem = function (img, label, enabled, action, data) {
                    var suffix = (enabled != null && enabled == false) ? "-disabled" : "";
                    var li = el('li', 'item', null, action, data);
                    var imgspan = el('span', 'img');
                    if (img != null && img != "") imgspan.append(el('span', { 'class': 'ctrl ' + img + suffix, 'aria-label': label, title: label }));
                    var labelspan = el('span', 'suffix' + suffix, label);
                    li.append(imgspan, labelspan);
                    return li;
                };
                var buildradio = function (index, fieldname, name, selected, action) {
                    var li = el('li', 'item');
                    var imgspan = el('span', 'img');
                    if (UndefinedNullOrEmpty(selected)) selected = 0;
                    var radio = el('input', { type: 'radio', id: 'fo_' + fieldname + '_' + index, name: 'fo_' + fieldname, 'class': 'option', value: index.toString() });
                    radio.attr('checked', (selected == index));
                    imgspan.append(radio);
                    var label = el('span', 'label', name, function () {
                        var check = $(this).parent().find('span:first > input:first')[0];
                        check.checked = !check.checked;
                        if (typeof action == 'function') {
                            action(check);
                        }
                    });
                    li.append(imgspan, label);
                    return li;
                }
                var buildchkbox = function (index, fieldname, name, selected, action) {
                    var li = el('li', 'item');
                    var imgspan = el('span', 'img');
                    var isChecked = (selected) ? true : false;
                    var chkbox;

                    if (isChecked)
                        chkbox = el('input', { type: 'checkbox', id: 'fo_' + fieldname + '_' + index, name: 'fo_' + fieldname, 'class': 'option fo_' + fieldname, value: index.toString(), checked: isChecked }, '', function () {
                            if (typeof action == 'function') action(this);
                        });

                    else
                        chkbox = el('input', { type: 'checkbox', id: 'fo_' + fieldname + '_' + index, name: 'fo_' + fieldname, 'class': 'option fo_' + fieldname, value: index.toString() }, '', function () {
                            if (typeof action == 'function') action(this);
                        });
                    imgspan.append(chkbox);
                    var label = el('span', 'label', name, function () {
                        var check = $(this).parent().find('span:first > input:first')[0];
                        check.checked = !check.checked;
                        if (typeof action == 'function') action(check);
                    });
                    li.append(imgspan, label);
                    return li;
                };
                var buildbutton = function (label, action, data) {
                    var li = el('li', 'button');
                    li.append(el('span', 'button', label, action, data));
                    return li;
                };
                var buildseparator = function () {
                    return el('li', 'separator');
                };
                var getPos = function (e) {
                    var x = 0, y = 0;
                    while (e && !isNaN(e.offsetLeft) && !isNaN(e.offsetTop)) {
                        x += e.offsetLeft - e.scrollLeft;
                        y += e.offsetTop - e.scrollTop;
                        e = e.offsetParent;
                    }
                    return { x: x, y: y };
                };

                var isOpeningSameFilter = function (col) {
                    var obj = document.getElementById('filter-popup-' + col.fieldname);
                    if (obj != null) return true;
                    return false;
                }

                if (isOpeningSameFilter(col)) {
                    col.filter.clear();
                    return;
                }
                col.filter.clear();

                var pos = getPos(this);
                pos.y += this.clientHeight;
                if ((pos.x + 300) > document.body.clientWidth) pos.x -= (300 - this.clientWidth);
                var div = el('div', { 'class': 'filter-popup', id: 'filter-popup-' + col.fieldname, style: 'top:' + pos.y + 'px;left:' + pos.x + 'px;' });
                var ul = el('ul', 'clean');

                var options = col.filter.options;

                if (col.filter.conditions != null && col.filter.conditions.length > 0) {
                    ul.append(
                        builditem("resetfilter", "Reset Filter", true, col.filter['reset'], h.data),
                        buildseparator()
                    );
                }

                if (col.sortable) {
                    ul.append(
                        builditem("sortatoz", "Sort A to Z", true, col.filter.sortAtoZ, h.data),
                        builditem("sortztoa", "Sort Z to A", true, col.filter.sortZtoA, h.data),
                        buildseparator()
                    );
                }
                if (col.datatype == 'datetime') {
                    if (col.filter.selected == null) col.filter.selected = 0;
                    for (var i = 0; i < options.length; i++) {
                        ul.append(buildradio(i, col.fieldname, options[i].Name, col.filter.selected));
                    }
                } else if (options != null && options.length > 0) {
                    ul.append(buildchkbox('all', col.fieldname, 'All', false, function (chk) {
                        $(".fo_" + col.fieldname).attr('checked', chk.checked);
                    }));

                    for (var i = 0; i < options.length; i++) {
                        var selectedchkbox = (Object.prototype.toString.call(col.filter.selected) === '[object Array]' &&
                            col.filter.selected != null && $.inArray(i.toString(), col.filter.selected) != -1);
                        ul.append(buildchkbox(i, col.fieldname, options[i].Name, selectedchkbox));
                    }
                } else if (col.datatype == 'string') {
                    var contains = builditem('', 'Contains: ');
                    $(contains.find("span")[1]).append("<input type='text' class='contains' id='contains_" + col.fieldname + "' />");
                    ul.append(contains);
                    //if (!UndefinedNullOrEmpty(col.filter.options) {
                    //    if (col.filter.selected==null) col.filter.selected=0;
                    //    for(var i=0;i<options.length;i++) {
                    //        ul.append(buildradio(i, col.fieldname, options[i].Name, col.filter.selected));
                    //    }
                    //}
                };
                ul.append(
                    buildseparator(),
                    buildbutton("Apply Filter", col.filter['do'], h.data),
                    buildbutton("Cancel", col.filter.clear, h.data)
                );

                div.append(ul);
                $(document.body).append(div);
            },
            reset: function (h) {
                var col = _settings.columns[h.data];
                col.filter.conditions = [];
                col.filter.conditionDescriptions = [];
                col.filter.refresh(h);
            }
        },
        sortable: true,
        visible: true,
        detailfield: false,
        fieldname: '',
        filterable: true,
        displayname: 'Column',
        sort: "",
        datatype: 'string',
        add_header_to: function (row) {
            var cell1 = el('th', { id: 'th_' + this.fieldname }, this.displayname);
            if (this.sort == 'asc') cell1.append(el('span', { 'class': 'ctrl down', 'aria-label': 'Sorted in ascending order.', title: 'Sorted in ascending order.' }));
            if (this.sort == 'desc') cell1.append(el('span', { 'class': 'ctrl up', 'aria-label': 'Sorted in descending order.', title: 'Sorted in descending order.' }));
            var cell2 = '';
            if (this.filterable) {
                cell2 = el('th', 'filter');
                cell2.append(el('span', { 'class': 'ctrl expand', 'aria-label': 'Filter ' + this.displayname, title: 'Filter ' + this.displayname, id: 'filter_' + this.fieldname }));
                cell2.click(this.index, this.filter.show);
            }
            if (!UndefinedNullOrEmpty(_settings.onupdate)) {
                _settings.onupdate(_table, { col: this, row: row, header: cell1, filtercell: cell2 }, 'header');
            }
            row.append(cell1, cell2);
        }
    };

    //function initializeWebApi() {
    //    version = context.getVersion();
    //    webApi = new CrmCommonJS.WebApi(version);
    //}

    function isFiltered() {
        for (var j = 0; j < _settings.columns.length; j++) {
            var conditions = _settings.columns[j].filter.conditions;
            if (!UndefinedNullOrEmpty(conditions)) {
                return true;
            }
        }
        return false;
    }

    /* setters */
    function setPagingCookie(val) {
        if (val !== null) val = val.replace(/[<]/g, "&lt;").replace(/[>]/g, "&gt;");
        _settings.paginginfo.pagingcookie = val;
    }
    function setPage(val) {
        _settings.paginginfo.page = val;
    }
    function setTable(val) {
        if (val == null) val = 'fetchTable';
        _settings.table = val;
        _table = $("#" + _settings.table);
        //initializeWebApi();
        setupColumns();
    }
    function setStatus(options) {
        if (!UndefinedNullOrEmpty(_settings.statusarea)) {
            var statusarea = document.getElementById(_settings.statusarea.id);
            if (!UndefinedNullOrEmpty(options.status)) {
                _settings.statusarea.status = options.status;
                $(statusarea).text(options.status);
            }
            if (!UndefinedNullOrEmpty(options['class'])) {
                statusarea.className = options['class'] + ' statusarea';
                _settings.statusarea['class'] = options['class'];
            }

            if (!UndefinedNullOrEmpty(options.onrefresh)) _settings.statusarea.onrefresh = options.onrefresh;
        }
    }
    /*=========*/
    /* getters */
    function getFilterConditions() {
        var filterconditions = new Array();
        for (var j = 0; j < _settings.columns.length; j++) {
            var column = _settings.columns[j];
            //if (!UndefinedNullOrEmpty(column.filter.conditions.value))
            for (var i = 0; i < column.filter.conditions.length; i++) {
                filterconditions = filterconditions.concat(column.filter.conditions[i]);
            }
        }
        return filterconditions;
    }
    function getFilterConditionDescriptions() {
        var desc = new Array();
        for (var j = 0; j < _settings.columns.length; j++) {
            var column = _settings.columns[j];
            if (!UndefinedNullOrEmpty(column.filter.conditionDescriptions)) {
                desc = desc.concat(column.filter.conditionDescriptions);
            }
        }
        return desc;
    }
    /*=========*/

    function setupColumns() {
        var default_DateOptions = function (datefield) {
            var addDays = function (date, days) {
                var dat = new Date(date);
                dat.setDate(dat.getDate() + days);
                return dat;
            };

            var ISO = function (date) {
                if (Date.prototype.toISOString) return date.toISOString();

                function pad(number) {
                    return ((number < 10) ? '0' : '') + number;
                }

                return date.getUTCFullYear() +
                    '-' + pad(date.getUTCMonth() + 1) +
                    '-' + pad(date.getUTCDate()) +
                    'T' + pad(date.getUTCHours()) +
                    ':' + pad(date.getUTCMinutes()) +
                    ':' + pad(date.getUTCSeconds()) +
                    '.' + (date.getUTCMilliseconds() / 1000).toFixed(3).slice(2, 5) +
                    'Z';
            };

            var today = new Date();
            today.setHours(0);
            today.setMinutes(0);
            today.setSeconds(0);
            today.setMilliseconds(0);

            var d90 = addDays(today, -90);
            var d91 = addDays(d90, -1);
            var d120 = addDays(today, -120);
            var d121 = addDays(d120, -1);
            var d180 = addDays(today, -180);
            var d181 = addDays(d180, -1);
            var startofyear = today;
            startofyear.setMonth(1);
            startofyear.setDate(1);

            var datefilters = [
                { Name: "All", Conditions: [] },
                {
                    Name: "Last 90 Days",
                    Conditions: [{ attribute: datefield, operator: 'on-or-after', value: ISO(d90) }]
                },
                {
                    Name: "90 to 120 days",
                    Conditions: [{ attribute: datefield, operator: 'on-or-before', value: ISO(d91) },
                    { attribute: datefield, operator: 'on-or-after', value: ISO(d120) }]
                },
                {
                    Name: "120 to 180 days",
                    Conditions: [{ attribute: datefield, operator: 'on-or-before', value: ISO(d121) },
                    { attribute: datefield, operator: 'on-or-after', value: ISO(d180) }]
                },
                {
                    Name: "Older than 180 Days",
                    Conditions: [{ attribute: datefield, operator: 'on-or-after', value: ISO(d181) }]
                },
                {
                    Name: "This Year",
                    Conditions: [{ attribute: datefield, operator: 'on-or-after', value: ISO(startofyear) }]
                }
            ];

            return datefilters;
        };

        for (var i = 0; i < _settings.columns.length; i++) {
            var newcol = $.extend(true, [], defaultcolumn);
            var filter = $.extend(true, {}, defaultcolumn.filter);
            _settings.columns[i] = $.extend(newcol, _settings.columns[i]);
            if (_settings.columns[i].filter !== null) {
                $.extend(filter, _settings.columns[i].filter);
                _settings.columns[i].filter = filter;  // copy with the default methods...
                if (_settings.columns[i].datatype == 'datetime' &&
                    _settings.columns[i].filter.options == null) {
                    _settings.columns[i].filter.options = default_DateOptions(_settings.columns[i].fieldname);
                }
            }
        }
    }

    function getFetch(settings) {

        var fetch = {
            mapping: 'logical',
            page: 1,
            count: 100
        }

        //paginginfo is not copied directly to fetch because it could include items that are not page, count, or cookie.
        var fetch = $.extend(fetch,
            {
                page: settings.paginginfo.page,
                count: settings.paginginfo.count,
                pagingcookie: settings.paginginfo.pagingcookie
            });

        // don't want to serialize a null pagingcookie
        if (fetch.pagingcookie == null) delete fetch.pagingcookie;

        var attributes = [];
        var sort = [];
        for (var i = 0; i < settings.columns.length; i++) {
            var col = settings.columns[i];
            if (UndefinedNullOrEmpty(col.fieldname)) continue; // ignore empty or undefined names
            attributes.push({ name: col.fieldname });
            if (col.sort == "asc") {
                sort.push({ attribute: col.fieldname });
            }
            if (col.sort == "desc") {
                sort.push({ attribute: col.fieldname, descending: 'true' });
            }
        }

        var conditions = settings.requiredconditions.concat(getFilterConditions());

        fetch.entity = {
            name: settings.entityname,
            attribute: attributes,
            filter: {
                type: 'and',
                condition: conditions
            }
        };

        // no conditions == no filter
        if (conditions.length == 0) delete fetch.entity.filter;

        if (sort.length > 0) {
            fetch.entity.order = sort;
        }

        return fetch;
    }

    function getCountFetch(settings, conditions) {
        var fetch = {
            mapping: 'logical',
            aggregate: true,
            entity: {
                name: settings.entityname,
                attribute: [
                    {
                        name: settings.columns[0].fieldname,
                        aggregate: 'count',
                        alias: 'total'
                    }],
                filter: {
                    type: 'and',
                    condition: conditions
                }
            }
        }

        if (conditions == null || conditions.length == 0) delete fetch.entity.filter;

        return fetch;
    }

    function getTotal(settings) {
        return getCountFetch(settings, settings.requiredconditions);
    }

    function getFilterTotal(settings) {
        var conditions = settings.requiredconditions.concat(getFilterConditions());
        return getCountFetch(settings, conditions);
    }

    function toFetchXml(fetch) {
        function toXml(tag, obj) {
            if (tag == 'value') return "<value>" + obj + "</value>";
            var inner = "";
            var attr = [];
            for (var key in obj) {
                var value = obj[key];
                if (Object.prototype.toString.apply(value) === '[object Array]') {
                    for (var i = 0; i < value.length; i++) {
                        inner += toXml(key, value[i]);
                    }
                } else if (typeof value === 'object') {
                    inner += toXml(key, value);
                } else {
                    if (key == 'pagingcookie') key = 'paging-cookie';
                    attr.push(key + "='" + value.toString() + "'");
                }
            }
            var xml = "<" + tag;
            if (attr.length > 0) xml += " " + attr.join(" ");
            xml += ">" + inner + "</" + tag + ">";
            return xml;
        }
        return "?fetchXml=" + toXml("fetch", fetch);
    }

    function getStatus() {
        if (!UndefinedNullOrEmpty(_settings.statusarea)) {
            var status = _settings.statusarea;
            var defaultstatus = { id: 'status', 'class': '', status: 'loading' };
            var status = $.extend(true, defaultstatus, _settings.statusarea);
            status['class'] += ' statusarea';
            if (!UndefinedNullOrEmpty(status.onrefresh)) status.status = status.onrefresh;
            return '<span id="' + status.id + '" class="' + status['class'] + '">' + status.status + '</span>';
        }
        return null;
    }

    function refreshtable(clearcookie, data) {
        showLoading();  // show loading first.
        if (typeof clearcookie !== 'undefined' && clearcookie == true) {
            setPagingCookie(null);
        }

        var header = function (columns) {
            var colheaders = function (columns) {
                var row = el('tr', 'headerrow');
                row.append(el('th'));
                for (var k in columns) {
                    var col = columns[k];
                    if (!col.visible || col.detailfield) continue;
                    col.add_header_to(row);
                }
                return row;
            }

            var thead = el('thead');
            thead.append(colheaders(columns));
            return thead;
        }
        var footer = function (width, more, page, cookie) {
            var tfoot = el('tfoot'), row = el('tr'), cell = el('td', { colspan: width });

            // Set Paging Area
            var setRange = function (data) {
                var start = ((page - 1) * _settings.paginginfo.count) + 1;
                var end = start + _settings.paginginfo.count - 1;
                var total = data[0].total;
                if (end > total) end = total;
                more = more || (total > end);

                cell.append(el('span', 'recordcount', start + ' - ' + end + ' of ' + total));


                if (isFiltered()) {
                    var setTotal = function (data) {
                        cell.append('&nbsp;&nbsp;', el('span', 'recordcount', '(Total Unfiltered Records: ' + data[0].total + ')'));
                        $(".refreshedon").append(" - Total: " + data[0].total + " - Filtered To: " + total);
                    }
                    //TODO: update to WebAPI
                    var fetch = toFetchXml(getTotal(_settings));
                    Xrm.WebApi.retrieveMultipleRecords(_settings.entityname, fetch, 100)
                        .then(function (data) { setTotal(data.value); })
                        .catch(function (error) { errorCallback(error); })
                    /*XrmSvcToolkit.fetch({
                        fetchXml: toFetchXml(getTotal(_settings)),
                        async: true,
                        successCallback: setTotal,
                        errorCallback: errorCallback
                    });*/
                } else {
                    $(".refreshedon").append(" - Total: " + total);
                }

                if (!more & page == 1) return;
                var pages = Math.floor(total / _settings.paginginfo.count) + 1;

                var max = 10;
                var first = page - 5;
                if (first < 1) first = 1;
                var last = first + max;
                if (last > pages) last = pages;

                for (var i = first; i <= last; i++) {
                    if (page == i) {
                        cell.append(el('span', 'pageselector', '<' + i + '>'));
                    } else {
                        cell.append(el('span', 'pageselector', i, function (h) {
                            setPage(h.data);
                            refreshtable(false);
                        }, i));
                    }
                }

                cell.append(el('span', 'pageselector', 'Next Page', function () {
                    setPage(_settings.paginginfo.page + 1);
                    refreshtable(false);
                }));
            }
            //TODO: update to WebAPI
            var fetch = toFetchXml(getFilterTotal(_settings));
            Xrm.WebApi.retrieveMultipleRecords(_settings.entityname, fetch, 100)
                .then(function (data) { setRange(data.value); })
                .catch(function (error) { errorCallback(error); });
            /*XrmSvcToolkit.fetch({
                fetchXml: toFetchXml(getFilterTotal(_settings)),
                async: true,
                successCallback: setRange,
                errorCallback: errorCallback
            });*/

            row.append(cell);
            tfoot.append(row);
            return tfoot;
        };

        // Build the TBODY
        var contents = function (data, columns, tablewidth) {
            var tbody = el('tbody');
            if (data != null) for (var i = 0; i < data.length; i++) {
                var entity = data[i];
                var oddoreven = (i % 2 == 1) ? "odd" : "even";
                var rows = [el('tr', 'bordertop ' + oddoreven)];
                rows[0].append(el('td', 'expand')); // add empty expand column (might put edit in it)
                for (var k = 0; k < columns.length; k++) {
                    var col = columns[k];
                    var v = null;
                    if (col.datatype === "lookup") {
                        v = {
                            Name: entity["_" + col.fieldname + "_value@OData.Community.Display.V1.FormattedValue"],
                            Id: entity["_" + col.fieldname + "_value"],
                            LogicalName: entity["_" + col.fieldname + "_value@Microsoft.Dynamics.CRM.lookuplogicalname"]
                        };
                    }
                    else if (col.datatype === "datetime") {
                        v = entity[col.fieldname + "@OData.Community.Display.V1.FormattedValue"];
                    }
                    else {
                        v = entity[col.fieldname];
                    }
                    if (!UndefinedNullOrEmpty(col.render, 'function')) {
                        col.render(rows, v, entity);
                    } else if (col.visible) {
                        var colspan = 1;
                        if (col.detailfield) colspan = tablewidth - 1;
                        else if (col.filterable) colspan = 2;

                        var cell = el('td', { colspan: colspan });
                        if (v == null) {
                            v = '';
                            cell.text('');
                        } else {
                            switch (col.datatype) {
                                case 'datetime':
                                    var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
                                    var date = null;
                                    if (typeof v === "string") {
                                        date = new Date(v);
                                    }
                                    else if (Object.prototype.toString.call(v) === '[object Date]') {
                                        date = v;
                                    }
                                    if (date !== null) {
                                        /*var hour = date.getHours();
                                        var minute = date.getMinutes();                                        
                                        var ampm = hour > 12 ? "PM" : "AM";
                                        hour = hour % 12;
                                        hour = hour ? hour > 0 : 12;
                                        minute = ("0" + minute).slice(-2);
                                        var time = hour + ":" + minute + " " + ampm;*/

                                        var month = date.getMonth() + 1;
                                        //cell.text(days[date.getDay()] + ", " + month + "/" + date.getDate() + "/" + date.getFullYear() + " " + time);
                                        cell.text(days[date.getDay()] + ", " + v);
                                    }
                                    else {
                                        cell.text(" ");
                                    }
                                    break;
                                case 'lookup':
                                    //TODO

                                    if (!UndefinedNullOrEmpty(col.link) && col.link === true) {
                                        cell.append(el('a', { href: '#', 'class': 'lookup' }, v.Name, function (h) {
                                            var o = h.data;
                                            openEntityForm(o.entity, o.id);
                                        }, { entity: v.LogicalName, id: v.Id }));
                                    } else if (!UndefinedNullOrEmpty(v.Name)) {
                                        cell.text(v.Name);
                                    }
                                    break;
                                default:
                                    if (v.hasOwnProperty("FormattedValue")) cell.text(v.FormattedValue);
                                    else if (v.hasOwnProperty("Value")) cell.text(v.Value);
                                    else cell.text(v.toString());

                                    if (cell.text().length > 0) {
                                        cell.html(cell.html().replace(/\n/g, "<br/>"));
                                    }

                                    break;
                            }
                        }
                        if (col.detailfield) {
                            var row = rows.length;
                            rows[row] = el('tr', 'details ' + oddoreven);
                            rows[row].append(el('td', 'expand'));
                        } else row = 0;
                        if (cell.text() == '') {
                            cell.html("&nbsp;");
                        }
                        rows[row].append(cell);
                    }
                }

                if (!UndefinedNullOrEmpty(_settings.onupdate)) {
                    _settings.onupdate(_table, rows, 'rows');
                }

                for (var j in rows) {
                    tbody.append(rows[j]);
                }
            }
            return tbody;
        }

        var filter = function (width) {
            var descriptions = getFilterConditionDescriptions();
            if (UndefinedNullOrEmpty(descriptions)) return;

            var baseid = _table.attr('id') + '_filter';
            var togglefilter = function (h) {
                var id = h.data;
                $('#' + id + '_header').toggle();
                $('#' + id + '_detail').toggle();
                _settings.showfilterdetails = $('#' + id + '_detail').is(':visible');
                if (_settings.innerscroll) {
                    var dh = $(".fetchHead");
                    var df = $(".fetchFooter");
                    var dc = $(".fetchContent");
                    dc.css('top', dh.height() + 'px');
                    var contentHeight = $(window).height() - (dh.height() + df.height());
                    dc.height(contentHeight);
                }
            }

            // Build Filter Area
            var header_row = el('tr', { 'class': 'filterrow', id: baseid + '_header' });
            var togglefilter_button = function (type) {
                var th = el('th', 'expand');
                th.append(el('span', 'ctrl ' + type, null, togglefilter, baseid));
                return th;
            };
            header_row.append(togglefilter_button('expand'), el('th', { colspan: width - 1 }, 'Show Applied Filters', togglefilter, baseid));

            // Build Detail
            var detail_row = el('tr', { 'class': 'filterdetailrow', id: baseid + '_detail' });
            if (_settings.showfilterdetails) {
                header_row.hide();
            } else {
                detail_row.hide();
            }

            var detail_cell = el('th', { colspan: width - 1 });
            detail_cell.append(el('span', null, 'Hide Applied Filters', togglefilter, baseid), '<br/>',
                "<ul class='appliedfilters'><li>" + descriptions.join('</li><li>') + "</li></ul>");
            detail_row.append(togglefilter_button('contract'), detail_cell);

            return [header_row, detail_row];
        }

        var headerfilter = function (width) {
            var row = el('tr', 'headerfilter'), cell = el('th', { colspan: width });
            // Add Refresh Button to header filter, float right
            var refreshname = 'Refresh';
            if (!UndefinedNullOrEmpty(_settings.refreshname)) refreshname = _settings.refreshname;
            var refreshbutton = el('span', 'button floatright', '', function () {
                refreshtable(true);
            });
            refreshbutton.append(el('span', { title: 'Refresh', 'aria-label': 'Refresh', 'class': 'ctrl refresh' }), refreshname);
            cell.append(refreshbutton);

            // Filter Area in headerfilter
            var cols = _settings.columns;
            for (var j = 0; j < cols.length; j++) {
                var col = cols[j];
                if (col.detailfield && col.filterable) {
                    var divclass = 'headerfilter';
                    if (!UndefinedNullOrEmpty(col.detailclass)) divclass = col.detailclass;
                    var div = $('<div class="' + divclass + '"></div>');


                    // Get the current value from the filter condition
                    var value = '';
                    if (!UndefinedNullOrEmpty(col.filter.conditions)) {
                        for (var i = 0; i < col.filter.conditions.length; i++) {
                            if (col.filter.conditions[i].operator == 'like') {
                                value = col.filter.conditions[0].value.slice(1, -1);
                                break;
                            }
                        }
                    }

                    var inputbox = $("<input type='text' class='contains' id='contains_" + col.fieldname + "'/>");
                    inputbox.val(value);

                    // Append name, input, and Go button to div.
                    var search = col.search;
                    if (UndefinedNullOrEmpty(search)) {
                        search = col.displayname + ' contains: ';
                    }
                    div.append(el('span', 'label', search), inputbox, el('span', 'button', 'Go', col.filter['do'], j));

                    if (!UndefinedNullOrEmpty(value)) {
                        div.append(el('span', 'button', 'Reset', col.filter.reset, j));
                    }
                    cell.append(div);
                }
            }
            // Add Status Area
            cell.append(getStatus());

            // Add Refreshed On
            var getTime = function () {
                var pad = function (i) { return ((i < 10) ? '0' : '') + i; };
                var today = new Date();
                var hour = today.getHours();
                var pm = hour >= 12;
                if (pm) hour -= 12;
                return (hour + ':' + pad(today.getMinutes()) + ':' + pad(today.getSeconds()) + ' ' + (pm ? 'PM' : 'AM'));
            }
            cell.append(el('span', 'refreshedon info', 'Refreshed at ' + getTime()));

            // Post render filter header
            if (!UndefinedNullOrEmpty(_settings.onupdate)) {
                _settings.onupdate(_table, cell, 'filterheader');
            }

            // Build Row
            row.append(cell);


            return row;
        }

        var innerScroll = function (table) {
            $(document.body).css('overflow', 'hidden');
            $(document).css('overflow', 'hidden');

            var thead = table.find("thead");
            // Set column widths
            thead.find("tr.filterdetailrow").width($(window).width());

            var cols = [];
            thead.find("tr.headerrow").find("th").each(function () {
                var w = $(this).width();
                //var iw = $(this).innerWidth();
                //$(this).width(w);
                cols[cols.length] = w;
                //				widthRow.append(el('th',{'style':'width:'+w+'px'}));
            });

            var colwidth = function (columns) {
                var colparam = '';
                for (var i = 0; i < columns.length; i++) {
                    if (columns[i] == 'auto') {
                        colparam += "<col width='auto'/>";
                    } else {
                        colparam += "<col width='" + columns[i] + "px'/>";
                    }
                }
                return colparam;
            }

            //var fRow = widthRow.clone();
            //var lw = widthRow.find("th:last-child").width();
            //widthRow.find("th:last-child").width(lw);

            var tbody = table.find("tbody");
            //tbody.prepend(widthRow);

            var tfoot = table.find("tfoot");
            //tfoot.find("td").width(thead.width());
            //tfoot.append(fRow);


            var divs = [el('div', 'fetchHead'), el('div', 'fetchContent'), el('div', 'fetchFooter')];
            var tables = [el('table', 'fetchTable'), el('table', 'fetchTable fetchInnerContents'), el('table', 'fetchTable')];

            var cs = cols.length;
            cols[cs - 2] = "auto";
            cols[cs - 1] = "auto";
            var table_cols = colwidth(cols);

            tables[0].append(table_cols, thead);
            tables[0].css('table-layout', 'fixed');
            tables[1].append(table_cols, tbody);
            tables[1].css('table-layout', 'fixed');
            tables[1].css('width', '100%');
            tables[2].append(table_cols, tfoot);
            tables[2].css('table-layout', 'fixed');
            divs[0].append(tables[0]);
            divs[1].append(tables[1]);
            divs[2].append(tables[2]);

            var divTable = el('div');
            divTable.append(divs[0], divs[1], divs[2]);

            table.before(divTable);

            var id = table.attr('id');
            table.remove();

            divTable.attr('id', id);

            _table = divTable;

            //divs[1].hide();
            //window-(header+footer)
            divs[1].css('top', divs[0].height() + 'px');
            divs[1].css('position', 'absolute');
            //divs[2].css('bottom',0);
            var contentHeight = $(window).height() - (divs[0].height() + divs[2].height());
            divs[1].height(contentHeight);
            divs[1].show();


            _windowSize.h = window.top.document.documentElement.clientHeight;
            _windowSize.w = window.top.document.documentElement.clientWidth;

            var onResize = function () {
                if (_windowSize.h == window.top.document.documentElement.clientHeight &&
                    _windowSize.w == window.top.document.documentElement.clientWidth) {
                    return;
                }
                refreshtable(false, _cache);
            }

            if (!_resizeAttached) {
                _resizeAttached = true;
                if (window.attachEvent) {
                    window.attachEvent('onresize', onResize);
                }
                else if (window.addEventListener) {
                    window.addEventListener('resize', onResize, true);
                }
                //addEvent(window, "resize", onResize);
            }
        }

        var populateData = function (data) {
            if (_settings.innerscroll) _cache = data;
            if (!UndefinedNullOrEmpty(_settings.onupdate)) {
                _settings.onupdate(_table, data, 'pre');
            }
            var width = _table.find("thead > tr.headerrow").find("th").size();

            _table.append(contents(data, _settings.columns, width)); // add contents
            _table.append(footer(width, data.moreRecords, _settings.paginginfo.page, data.pagingCookie));

            if (_settings.innerscroll) {
                innerScroll(_table);
            }


            if (!UndefinedNullOrEmpty(_settings.onupdate)) {
                _settings.onupdate(_table, data, 'post');
            }
            hideLoading();
        }

        if (!UndefinedNullOrEmpty(_settings.onupdate)) {
            _settings.onupdate(_table, _settings.columns, 'beforerefresh');
        }
        _table.html(""); //clear table div

        if (_settings.innerscroll) {
            // change div back into table...
            var id = _table.attr('id');
            var nt = el('table', 'fetchTable');
            _table.before(nt);
            _table.remove();
            _table = nt;
            nt.attr('id', id);
        }

        _table.append(header(_settings.columns)); // add header
        var width = _table.find("thead > tr.headerrow").find("th").size();
        _table.find("thead").append(filter(width));
        _table.find("thead > tr.headerrow").before(headerfilter(width));

        if (typeof data !== 'undefined' && data != null) {
            populateData(data);
        } else {
            //TODO: update to WebAPI
            var fetch = toFetchXml(getFetch(_settings));
            Xrm.WebApi.retrieveMultipleRecords(_settings.entityname, fetch, 100)
                .then(function (data) { populateData(data.value); })
                .catch(function (error) { errorCallback(error); })
            /*XrmSvcToolkit.fetch({
                fetchXml: toFetchXml(getFetch(_settings)),
                async: true,
                successCallback: populateData,
                errorCallback: errorCallback
            });*/
        }
    };


    setTable(_settings.table);
    refreshtable(true);

    return {
        Refresh: refreshtable,
        SetPage: setPage,
        SetPagingCookie: setPagingCookie,
        GetFetch: function () { return getFetch(_settings); },
        GetFetchXML: function () { return toFetchXml(getFetch(_settings)); },
        SetTable: setTable,
        Filter: function (field, conditions) {
            for (var k = 0; k < _settings.columns.length; k++) {
                var col = _settings.columns[k];
                if (col.fieldname == field) {
                    col.filter.conditions = conditions;
                    col.filter.clear();
                    refreshtable(true);
                }
            }
        },
        SetStatus: setStatus
    };
};
