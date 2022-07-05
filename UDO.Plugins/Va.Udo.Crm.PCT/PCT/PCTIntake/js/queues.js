// JavaScript Document
var QueueManager = function () {
    var _autorefresh = 60 * 1000;  // every 60 seconds
    var _interval = null;
    var _debug = true;
    var _currentUser = null;
    var _queues = [];
    var _queueItemTable = null;
    var _settings = {};
    var _showAllSpecialSituations = false;

    function loadingOn() {
        if (_settings && _settings.LoadingOn) _settings.LoadingOn();
        return $.when();
    }

    function loadingOff() {
        if (_settings && _settings.LoadingOff) _settings.LoadingOff();
        return $.when();
    }

    function log(msg) {
        if (_settings && _settings.debug) console.log("QueueManager: " + msg);
        //alert(msg);
    }

    function init(options) {
        if (options) _settings = options;
        QueueManager.UseShortNames = _settings.UseShortNames;

        if (_debug) log("init");
        loadQueues();
        $("#panel_queueitems_area > tbody").selectable();

        // Get User from context if possible.
        if (Xrm && Xrm.Page && Xrm.Page.context && Xrm.Page.context.getUserId) {
            _currentUser = cleanGuid(Xrm.Page.context.getUserId());
        } else {
            Sdk.jQ.execute(new Sdk.WhoAmIRequest())
              .done(
                  function (whoAmIResponse) {
        _currentUser = cleanGuid(whoAmIResponse.getUserId());
        if (_debug) executeForDebug("WhoAmI", "Retrieving current user", { _currentUser: _currentUser });
    }
            );
        }

        hideQueueItemActions();

        // Setup Interval
        _interval = window.setInterval(refresh, _autorefresh);
    }

    function setAutoRefresh(ms) {
        _autorefresh = ms;
        window.clearInterval(_interval);
        window.setInterval(refresh, _autorefresh);
    }

    function emptyPromise() {
        var dfd = new $.Deferred();
        dfd.resolve();
        return dfd.promise();
    }

    function refreshQueueItems() {
        if (_debug) log("refreshQueueItems");
        return loadQueueItems(QueueManager.SelectedQueue);
    }

    function loadQueues() {
        if (_debug) log("loadQueues");
        return retrieveQueues().done(postRetrieveQueues);
    }

    function loadQueueItems(queueid) {
        if (queueid === null) return emptyPromise();

        if (_debug) log("loadQueueItems");
        if (QueueManager.SelectedQueue == null || _queueItemTable == null) {
            QueueManager.SelectedQueue = queueid;

            $('li.queue').removeClass("list-group-item-active");
            $('li.queue[data-queueid="' + queueid + '"]').addClass("list-group-item-active");

            // do the query and update the stuff
            return retrieveQueueItems(QueueManager.SelectedQueue);
        }
        if (QueueManager.SelectedQueue != cleanGuid(queueid.toLowerCase())) {
            if (_queueItemTable) _queueItemTable.clearSelection().then(function () {
                QueueManager.SelectedQueue = null;
                return loadQueueItems(cleanGuid(queueid.toLowerCase()));
            });
        } else {
            // Update the selected/highlighted active queue
            return _queueItemTable.refresh();//.done(postQueueItemRefresh);
        }


        /*
                $('li.queue').removeClass("list-group-item-active");
                $('li.queue[data-queueid="'+queueid+'"]').addClass("list-group-item-active");
                        
                // do the query and update the stuff
                return retrieveQueueItems(QueueManager.SelectedQueue);
        */
    }

    function hideQueueItemActions() {
        if (_settings.HideQueueItemActions) _settings.HideQueueItemActions();
    }

    function showQueueItemActions(item) {
        log("Showing Queue Item Actions...")
        // Worker Actions
        var _isworker = null;
        if (item.hasOwnProperty("isworker")) {
            _isworker = item.isworker
        } else {
            if (item && item.row && item.row.length > 0) {
                var worker = item.row.data("obj").attributes.workerid;
                if (worker && worker.value) {
                    var id = worker.value.Id;
                    _isworker = (cleanGuid(id) == cleanGuid(_currentUser));
                } else { // worker not set
                    _isworker = "none";
                }
            } else {
                _isworker = "none";
            }
        }

        if (_settings.ShowQueueItemActions) _settings.ShowQueueItemActions({ IsWorker: _isworker });
    }

    function selectQueueItem(e, item) {
        if (e && e.target) {
            var xt = _queueItemTable;
            if (!item) item = {};
            if (!item.selected && xt.getSelection !== undefined) item.selected = xt.getSelection();
            if ((!item.row || (item.row.length && item.row.length == 0)) && item.selected.length == 1) item.row = $("#" + item.selected[0]);
        }

        //selected is an array or null
        if (!item.selected || (item.selected.push && item.selected.length == 0)) {
            hideQueueItemActions();
            return;
        }

        showQueueItemActions(item);

        if (item.selected) {
            QueueManager.SelectedItems.length = 0;
            $.each(item.selected, function (i, v) {
                var itm = v;
                if (itm.length > 3 && itm.substring(0, 3) == "row") itm = itm.substring(4);
                QueueManager.SelectedItems.push(itm);
                log("QueueManager.SelectedItems PUSH " + itm);
            });
        }
    }

    function postQueueItemRefresh(e, data) {
        var xt = _queueItemTable; //xrmTable object

        var selection = xt.getSelection();
        if (selection) {
            if (selection.length == 1) {
                var itemid = selection[0];
                // single item selected
                var row = xt.getTable().find("#" + selection[0]);
                showQueueItemActions({ selected: selection, row: row });
            } else if (selection.length > 1) {
                showQueueItemActions();
            }
        }

        if (data && data.data) {
            return retrieveSpecialSituations(data.data);
        }
        return emptyPromise();
    }

    function setupQueueItemsTable(q) {
        if (!_queueItemTable) {
            var columns = new XrmTable.Columns(
            [

                new XrmTable.Column({ name: "", key: "star", sortindex: 0, sortcondition: XrmTable.Constants.SortType.DESCENDING, sortkey: "udo_specialsituationyn" }),
                new XrmTable.Column({ name: "Name", key: "title", "class": "selector" }),
                new XrmTable.Column({ name: "Entered On", key: "enteredon", sortindex: 1, sortcondition: XrmTable.Constants.SortType.ASCENDING }),
                new XrmTable.Column({
                    name: "Wait", key: "wait", srname: "Wait Time", sortkey: "enteredon", render: function (data) {
                        if (data instanceof Sdk.Entity) data = data.view().attributes;
                        var endDate = new Date();
                        if (data.enteredon && data.enteredon.value) {
                            var duration = timeDiff(data.enteredon.value, endDate);
                            return duration.getDuration("minutes", "abbr");
                        }
                        return "";
                    }
                }),
                new XrmTable.Column({ name: "Purpose", tooltip: "Purpose of Visit", key: "udo_purposeofvisit" }),
                new XrmTable.Column({ name: "Worker", key: "workerid" }),
                new XrmTable.Column({ name: "SL", tooltip: "Sensitivity Level", key: "udo_sensitivitylevel" }),
                /*new XrmTable.Column({ name: "", key: "", "class": "selector" }),*/
                new XrmTable.Column({
                    name: "Special Situation", key: "udo_specialsituationyn", type: "row",

                    render: function (data, rows) {
                        if (data instanceof Sdk.Entity) {
                            e = data.view().attributes;
                        } else {
                            e = data;
                        }

                        if (e["udo_specialsituationyn"] !== undefined && e["udo_specialsituationyn"].value === true) {
                            rows[0].addClass("specialsituation");

                            // For screen readers only - they cannot see a visual class modification that highlights a row
                            // so we read the has one or more special situations.
                            var tds = $(rows[0]).find("td");
                            if (tds.length > 1) {
                                var td = tds[1];
                                var scrText = $("<span class='sr-only'/>");
                                scrText.text(" has one or more special situations.");
                                $(td).append(scrText);
                            }
                        }

                    }
                })
            ]);

            $("#panel_queueitems_div").xrmTable({
                name: "QueueItems"
                , table: "panel_queueitems_area"
                , query: q
                , columns: columns
                , AllowSelect: true
                , AllowDrag: true
                , RecordsPerPage: 10
                , debug: _settings.debug
                , prev: $("#panel_queues_area").find("li.queue")[0].id
                , caption: {
                    label: "Queue Items"
                    , tabindex: "0"
                }
                , logger: log
            })
            .on("preClearSelection", function () { log("QueueItemTable: ClearSelection"); })
            .on("postClearSelection", hideQueueItemActions)
            .on("preSelect", function (e, o) { log("QueueItemTable: Selecting Item: " + o.row.attr("id")); })
            .on("postSelect", selectQueueItem)
            .on("dragStart", queueItemDragStart)
            .on("dragStop", queueItemDragStop)
            .on("preRetrieveMultiple", function () { log("QueueItemTable: Refreshing"); })
            .on("postRender", postQueueItemRefresh);

            _queueItemTable = $("#panel_queueitems_div").data("xrmTable");

        } else {
            _queueItemTable.setQuery(q);
            _queueItemTable.refresh(true);
        }
    }

    function queueItemDragStart(row) {
        $("#panel_queues_area").find("li.queue").attr("aria-dropEffect", "move");
    }

    function queueItemDragStop(row) {
        $("#panel_queues_area").find("li.queue").attr("aria-dropEffect", "none");
    }

    function tabFocus(mod, selector, current) {
        if (!selector) selector = 'button:not([tabindex^=" - "]),select:not([tabindex^=" - "]),input:not([tabindex^=" - "]),[tabindex]:not([tabindex^=" - "])';
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
        items.eq(index).focus();
        return prevent;
    }

    function focusInQueueElement(event) {
        log("focusInQueueElement");
        event.stopPropagation();
        var from = $(event.relatedTarget),
            area = $("#panel_queues_area"),
            fromArea = area.find(from).length > 0,
            activeQueue = area.find(".list-group-item-active");
        if (event.relatedTarget && !fromArea && activeQueue.length > 0 && from.attr("id")!=activeQueue.attr("id")) {
            log("Focusing on active item");
            activeQueue.focus();
            event.preventDefault();
            return;
        }
        $(this).addClass("kb-hover");
    }

    function focusOutQueueElement(event) {
        log("focusOutQueueElement");
        event.stopPropagation();
        $(this).removeClass("kb-hover");
    }

    function clickQueueElement(event) {
        log("clickQueueElement");
        event.stopPropagation();
        var id = $(this).attr("data-queueid");
        loadQueueItems(id);
    }

    function keydownQueueElement(e) {
        e.stopPropagation();
        var tabbable = 'button:not([tabindex^="-"]),select:not([tabindex^="-"]),input:not([tabindex^="-"]),[tabindex]:not([tabindex^="-"])';
        var items = $("#panel_queues_area").find("li.queue");
        var key = e.which || e.keycode;
        if (key == 32 /*space*/ || key == 13 /*enter*/) {
            $(this).trigger("click");
        }
        var i = 0;
        for (i = 0; i < items.length; i++) {
            if (items[i].id == e.target.id) break;
        }

        if (key == 38 /*up*/ && i != 0) {
            $(items[i - 1]).focus();
        }
        if (key == 40 /*down*/ && i != items.length) {
            $(items[i + 1]).focus();
        }
        if (key == 9) /* tab */ {
            if (e.shiftKey) {
                if (tabFocus(-1, tabbable, items[0])) {
                    e.preventDefault();
                }
            } else {
                if (tabFocus(1, tabbable, items[items.length - 1])) {
                    e.preventDefault();
                }
            }
        }
    }

    function createQueueElement(qdata) {
        if (_debug) log("createQueueElement: Adding queue " + qdata.name.value +
                        " (" + qdata.queueid.value + ") with " +
                        qdata.numberofitems.value + " items in the queue.");

        _queues.push(qdata);
        var routeitem = $("<li/>");
        var routeitem_action = $("<a/>");
        routeitem_action.attr("href", "#");


        if (QueueManager.UseShortNames === true) {
            if (qdata["udo_shortname"] !== undefined) {
                routeitem_action.text(qdata["udo_shortname"].value);
                //maybe put some small subtext with the full name?
                routeitem_action.prop("title", qdata.name.value);
            } else {
                routeitem_action.text(qdata.name.value);
            }
        } else {
            routeitem_action.text(qdata.name.value);
        }

        routeitem_action.click(function () {
            routeSelectedItem(qdata.queueid.value);
        });
        routeitem.append(routeitem_action);
        $("#action_route_items").append(routeitem);

        var area = $("#panel_queues_area");
        var id = $("li.queue").length;
        id = "queue_" + id;

        var queue = $("<li class='queue list-group-item droppable'></li>");
        queue.attr("tabIndex", "0")
        .attr("id", id)
        .text(qdata.name.value)
        .attr("data-queueid", qdata.queueid.value)
        .on('focusin', focusInQueueElement)
        .on('focusout', focusOutQueueElement)
        .click(clickQueueElement)
        .on('keydown', keydownQueueElement);

        area.append(queue);

        var count = qdata.numberofitems.value;
        if (count == 0) count = "";
        var badge = $("<span class='badge' role='presentation'></span>");
        badge.text(count);
        queue.append(badge);

        queue.droppable({
            tolerance: "touch",
            classes: { "ui-droppable-hover": "list-group-item-drop-hover" },
            accept: "#panel_queueitems_div table:first > tbody > tr, #panel_queueitems_area > tbody > tr",
            drop: function (e, ui) {
                var itemid = ui.draggable.data("obj").id;
                selectQueueItem(e, $("#" + ui.draggable.attr(itemid)));
                var queueid = $(this).data("queueid");
                if (QueueManager.SelectedQueue != queueid) {
                    QueueManager.Route(itemid, queueid);
                    var items = [], item = null;
                    items.push($("#" + ui.draggable.id));
                    item = items[0].next();
                    while (item && item.attr("aria-level") > 1) {
                        items.push(item);
                        item = item.next();
                    }
                    for (var i = 0; i < items.length; i++) {
                        items[i].hide();
                        items[i].remove();
                    }
                    _queueItemTable.clearSelection();
                    ui.draggable.remove();
                    //refresh();
                    refreshQueues();
                }
            }
        });
    }

    function timeDiff(start, end) {
        //if (_debug) log("timeDiff: "+start+" " + end);
        function getTimeDiff(start, end) {
            var ONEDAY = 1000 * 60 * 60 * 24;
            var ONEHOUR = 1000 * 60 * 60;
            var ONEMINUTE = 1000 * 60;
            var ONESECOND = 1000;

            function getDaysInMonth(date) {
                var month = date.getMonth();
                var year = date.getFullYear();

                var currentMonth = new Date(year, month);
                var nextMonth = new Date(year, month + 1);

                // time diff divided by [(ms in a s) * (s in m) * (m in h) * (h in d)], i.e. One Day
                return Math.floor((nextMonth.getTime() - currentMonth.getTime()) / ONEDAY);
            }

            var result = {
                timediff: Math.floor(end.getTime() - start.getTime()),
                years: end.getFullYear() - start.getFullYear(),
                months: end.getMonth() - start.getMonth(),
                //days: Math.floor(Math.abs(end.getTime() - start.getTime()) / ONEDAY),
                days: end.getDate() - start.getDate(),
                hours: Math.floor(Math.abs(end.getTime() - start.getTime()) / ONEHOUR) % 24,
                minutes: Math.floor(Math.abs(end.getTime() - start.getTime()) / ONEMINUTE) % 60,
                seconds: Math.floor(Math.abs(end.getTime() - start.getTime()) / ONESECOND) % 60
            };

            if (result.seconds < 0) {
                result.minutes--;
                result.seconds += 60;
            }
            if (result.minutes < 0) {
                result.hours--;
                result.minutes += 60;
            }
            if (result.hours < 0) {
                result.days--;
                result.hours += 24;
            }
            while (result.days < 0) {
                result.months--;
                result.days += getDaysInMonth(end);
            }
            while (result.months < 0) {
                result.years--;
                result.months += 12;
            }
            if (result.years < 0) result.years = 0;

            return result;
        }

        // get the time difference data
        var diff;
        if (start <= end) {
            diff = getTimeDiff(start, end);
        } else {
            diff = getTimeDiff(end, start);
        }



        // attach functions
        diff.getDuration = function (minPart, format) {
            function formatDuration(format, val, long, abbr, short) {
                function plural(val, word) {
                    if (val > 0) {
                        return val + " " + word + (val == 1 ? "" : "s");
                    }
                    return "";
                }

                switch (format) {
                    case "long": return plural(val, long);
                    case "abbr": return plural(val, abbr);
                    case "short":
                        if (val > 0) {
                            return val + " " + short;
                        }
                        return "";
                    default: return plural(val, long);
                }
            }
            // get the duration string parts - the first two must be pluralizable.
            diff.formatted = {
                years: formatDuration(format, diff.years, "year", "yr", "yr"),
                months: formatDuration(format, diff.months, "month", "mnth", "mo"),
                days: formatDuration(format, diff.days, "day", "day", "d"),
                hours: formatDuration(format, diff.hours, "hour", "hr", "h"),
                minutes: formatDuration(format, diff.minutes, "minute", "min", "m"),
                seconds: formatDuration(format, diff.seconds, "second", "sec", "s")
            };

            var durText = [diff.formatted.years, diff.formatted.months, diff.formatted.days,
                           diff.formatted.hours, diff.formatted.minutes, diff.formatted.seconds];

            var minPartIndex = 0;
            if (!minPart) minPart = "seconds";
            switch (minPart) {
                case "years": minPartIndex = 0; break;
                case "months": minPartIndex = 1; break;
                case "days": minPartIndex = 2; break;
                case "hours": minPartIndex = 3; break;
                case "minutes": minPartIndex = 4; break;
                case "seconds": minPartIndex = 5; break;
                default: minPartIndex = 5; break;
            }

            var found = false;
            var i = 0;
            var result = [];
            do {
                if (durText[i] != "") {
                    found = true;
                    result.push(durText[i]);
                }
                i++;
            } while (!found || i <= minPartIndex);

            return result.join(", ");
        }

        diff.duration = diff.getDuration("seconds", "long");

        return diff;
    }

    function retrieveQueueItems(queueid) {
        if (_debug) log("retrieveQueueItems");
        Sdk.jQ.setJQueryVariable($);

        var fetch = '<fetch distinct="false" no-lock="true" ' +
                  'mapping="logical" returntotalrecordcount="false">' +
                  '<entity name="queueitem">' +
                  '<attribute name="queueitemid" />' +
                  '<attribute name="queueid"/>' +
                  '<attribute name="title"/>' +
                  '<attribute name="enteredon"/>' +
                  '<attribute name="udo_sensitivitylevel"/>' +
                  '<attribute name="udo_specialsituationyn"/>' +
                  '<attribute name="udo_purposeofvisit"/>' +
                  //'<attribute name="udo_siteid"/>' +
                  '<attribute name="priority"/>' +
                  '<attribute name="objectid"/>' +
                  '<attribute name="objecttypecode"/>' +
                  '<attribute name="workerid"/>' +
                  '<filter>' +
                  '<condition attribute="queueid" operator="eq" value="' + queueid + '"/>' +
                  '<condition attribute="statecode" operator="eq" value="0"/>' +
                  '</filter></entity></fetch>';
        if (_debug) {
            log("retrieveQueueItems: FetchXML Query:\r\n" + fetch);
        }

        var query = new Sdk.Query.FetchExpression(fetch);

        return setupQueueItemsTable(query);
    }


    function findQueueElement(queueid) {
        if (_debug) log("findQueueElement");
        var queues = $("#panel_queues_area > li");
        var queue = queues.filter(function (index) {
            return $(this).data("queueid") == queueid;
        });
        return queue;
    }

    function retrieveQueues() {
        if (_debug) log("retrieveQueues: Attempting to retrieve queues.");

        Sdk.jQ.setJQueryVariable($);

        var fetch = '<fetch distinct="true" no-lock="true" ' +
                  'mapping="logical">' +
                  '<entity name="queue">' +
                  '<attribute name="queueid"/>' +
                  '<attribute name="numberofitems" />' +
                  '<attribute name="name"/>' +
                  '<attribute name="udo_shortname"/>' +
                  '<attribute name="udo_siteid"/>' +
                  '<link-entity name="udo_udo_securitygroup_queue" from="queueid" to="queueid">' +
                  '<link-entity name="udo_udo_securitygroup_systemuser" from="udo_securitygroupid" to="udo_securitygroupid">' +
                  '<filter>' +
                  '<condition attribute="systemuserid" operator="eq-userid"/>' +
                  '</filter>' +
                  '</link-entity>' +
                  '</link-entity></entity></fetch>';
        if (_debug) {
            log("retrieveQueues: FetchXML Query:\r\n" + fetch);
        }

        var query = new Sdk.Query.FetchExpression(fetch);
        return Sdk.jQ.retrieveMultiple(query);
    }

    function retrieveSpecialSituations(ec) {

        if (_debug) log("retrieveSpecialSituations: Attempting to retrieve special situations for the rendered queueitems.");

        Sdk.jQ.setJQueryVariable($);

        var values = "";
        var i = 0;
        for (i = 0; i < ec.getCount() ; i++) {
            values += "<value>" + ec.getEntity(i).getId() + "</value>";
        }

        var fetch = '<fetch distinct="true" no-lock="true" mapping="logical" returntotalrecordcount="false">' +
                    '<entity name="udo_interactionspecialsituation">' +
                    '<attribute name="udo_name" />' +
                    '<link-entity name="udo_interaction" from="udo_interactionid" to="udo_interactionid" >' +
                    '<link-entity name="queueitem" from="objectid" to="udo_interactionid" >' +
                    '<attribute name="queueitemid"/>' +
                    '<filter>' +
                    '<condition attribute="queueitemid" operator="in" >' +
                    values +
                    '</condition></filter></link-entity></link-entity></entity></fetch>';

        if (_debug) {
            log("retrieveSpecialSituations: FetchXML Query:\r\n" + fetch);
        }

        var query = new Sdk.Query.FetchExpression(fetch);
        return Sdk.jQ.retrieveMultiple(query).then(postRetrieveSpecialSituations);

    }

    function postRetrieveSpecialSituations(ec) {
        var table = _queueItemTable.getTable();
        var colspan = table.find("thead > tr > th").length;
        var i, k, entity, rowid;
        var data = [];
        for (i = 0; i < ec.getCount() ; i++) {
            entity = ec.getEntity(i);
            rowid = "#row-" + entity.getValue("queueitem2.queueitemid");
            data[rowid] = data[rowid] || { special: [] };
            data[rowid].special.push(entity.getValue("udo_name"));
        }

        var row, cell, area, item;
        for (k in data) {
            row = $(k);
            cell = row.find("td:first");
            area = cell.find("ul.specialsituations");
            area.remove();
            area = $("<div>")//$("<ul>")
                .attr("id", rowid + "-special-ul")
                .addClass("specialsituations");

            area.append($("<div/>").addClass("tagtitle").text("Special Situations: "));

            for (i in data[k].special) {
                item = $("<div/>").addClass("tag").text(data[k].special[i]);
                //item = $("<li/>").text(data[k].special[i]);
                area.append(item);
                area.append($("<div/>").addClass("sr-only").text(", "));
            }

            //ul.hide();
            var specialcell = $("<td/>").attr("colspan", colspan).append(area);
            var specialrow = $("<tr/>")
                .addClass("specialsituations")
                .attr("aria-level", "2")
                .append(specialcell);

            if (_showAllSpecialSituations) {
                specialrow.show();
            } else {
                specialrow.hide();
            }
            specialrow.insertAfter(row);

            //var btn = $("<a/>")
            //           .attr("href", "#")
            cell
                       //.attr("role", "button")
                       //.addClass("btn btn-danger btn-xs align-right")
                       .attr("title", "Show Special Situations")
                       .attr("aria-label", "Show Special Situations")

                //       .attr("id", rowid + "-special-button")
                       .click(function (e) {
                           var t = $(this);
                           var row = $(this).closest("tr").next("tr.specialsituations");
                           row.toggle();
                           if (t.is(":visible")) {
                               t.attr("title", "Hide Special Situations");
                               t.attr("aria-label", "Hide Special Situations");
                           } else {
                               t.attr("title", "Show Special Situations");
                               t.attr("aria-label", "Show Special Situations");
                           }
                           e.preventDefault();
                           e.stopPropagation();
                       });

            var span = $("<span />").addClass("glyphicon glyphicon-star text-danger").attr("aria-hidden", "true");
            var srspan = $("<span />").addClass("sr-only").text("This item has special situations.");

            cell.append(span, srspan);
            //cell.append(btn);
            //cell.append(ul);


        }
    }

    function setSpecialSituationsVisibility(v) {
        _showAllSpecialSituations = v;
        if (v) {
            $("tr.specialsituations").show();
        } else {
            $("tr.specialsituations").hide();
        }

    }

    function postRetrieveQueues(ec) {
        if (_debug) log("postRetrieveQueues");
        //var id = ec.getEntities().toArray()[0].getId();
        ec.getEntities().forEach(function (e) {
            createQueueElement(e.view().attributes);
        });
        var id = $($("#panel_queues_area").find("li.queue")[0]).data("queueid");
        return loadQueueItems(id);
    }

    function retrieveQueueCounts() {
        if (_debug) log("retrieveQueueCounts: Attempting to update queue counts.");

        Sdk.jQ.setJQueryVariable($);

        var fetch = '<fetch distinct="false" no-lock="true" ' +
                  'mapping="logical">' +
                  '<entity name="queue">' +
                  '<attribute name="queueid"/>' +
                  '<attribute name="numberofitems" />' +
                  '<attribute name="name"/>' +
                  '<filter>' +
                  '<condition attribute="queueid" operator="in">';

        _queues.forEach(function (q) { fetch += "<value>" + q.queueid.value + "</value>"; });

        fetch += '</condition></filter></entity></fetch>';

        if (_debug) {
            log("retrieveQueueCounts: FetchXML Query:\r\n" + fetch);
        }

        var query = new Sdk.Query.FetchExpression(fetch);
        return Sdk.jQ.retrieveMultiple(query);
    }

    function postRetrieveQueueCounts(ec) {
        if (_debug) log("postRetrieveQueueCounts");
        ec.getEntities().forEach(function (e) {
            var qdata = e.view().attributes;

            var count = qdata["numberofitems"].value;
            if (count == 0) count = "";

            var el = findQueueElement(qdata["queueid"].value);

            if (el == null) return;
            var badge = el.find("span.badge");
            badge.text(count);
        });

        return emptyPromise();
    }

    function executeForDebug(cat, msg, data) {
        var defer = $.Deferred();
        log(cat + ": " + msg + "\r\nData:" + JSON.stringify(data));
        var promise = defer.promise();
        defer.resolve(data);
        return promise;
    }

    function executeWhoAmIRequest(callerArgs, depth, dfd) {
        if (_debug) log("executeWhoAmIRequest");

        Sdk.jQ.setJQueryVariable($);

        if (!dfd) {
            dfd = $.Deferred();
        }

        // use Sdk.Soap
        Sdk.jQ.execute(new Sdk.WhoAmIRequest())
          .done(
              function (whoAmIResponse) {
    _currentUser = whoAmIResponse.getUserId();
    if (_debug) executeForDebug("WhoAmI", "Retrieving current user", { _currentUser: _currentUser });
    dfd.resolve(callerArgs);
})
        .fail(
            function (e) {
                if (!depth) depth = 1;
                if (depth == 5) {
                    dfd.reject("Unable to execute WhoAmI to findout who I am.");
                    throw new Error("Unable to execute WhoAmI to findout who I am.");
                }
                executeWhoAmIRequest(callerArgs, depth + 1, dfd);
            }
        );

        return dfd.promise();
    }

    // a: arguments
    // n: array of names
    // l: min length of arguments
    function getArgs(a, n, l) {
        if (_debug) log("getArgs: Start");

        function isArray(a) {
            return typeof a.push !== "undefined";
        }
        if (!n || !a || !isArray(n) || a.length == 0) return null;
        if (isArray(a[0])) a = a[0];
        if (a[0].type && a[0].type == "args") return a[0];
        if (typeof l == "undefined") l = n.length;
        if (a.length < l) return null;
        var k, r = { type: "args" };
        for (k = 0; k < n.length; k++) {
            if (a.length >= k) {
                r[n[k]] = a[k];
            } else {
                r[n[k]] = null;
            }
            if (_debug) log('argument "' + n[k] + '"="' + r[n[k]] + '"');
        }
        return r;
    }

    function cleanGuid(s) {
        if (s == null) return s;
        if (s.length == 38) return s.slice(1, 37).toLowerCase();
        return s.toLowerCase();
    }

    // arguments data object or...
    // target, source (sourceid), destination (queueid), queueItemProperties
    function executeAddToQueue(d) {
        var reject = function (e) {
            var d = new $.Deferred();
            d.reject("AddToQueue: " + e);
            return d.promise();
        };

        var data = d;

        if (arguments.length > 1) data = getArgs(arguments, ["target", "sourceid", "queueid", "queueItemProperties"]);

        if (typeof data === "undefined" || data == null) return reject("No data found for request.");

        var target = null;
        if (data.target) {
            if (data.target instanceof Sdk.EntityReference) target = data.target;
            else if (data.target.logicalName && data.target.id) {
                target = new Sdk.EntityReference(data.target.logicalName, data.target.id);
            }
            else if (data.target instanceof Sdk.Entity) {
                var targetid = data.entity.getId();
                if (targetid == null) return reject("Target entity id is null.");
                target = new Sdk.EntityReference(data.target.getType(), targetid);
            }
        }

        if (data.entity && data.entity instanceof Sdk.Entity) {
            var entityid = data.entity.getId();
            if (entityid == null) return reject("Target entity id is null.");

            target = new Sdk.EntityReference(data.entity.getType(), entityid);
        }

        if (target == null) return reject("Unable to identify target.");

        var destinationQueueId = cleanGuid(data.queueid);
        if (!destinationQueueId) {
            destinationQueueId = QueueManager.SelectedQueue;
        }

        var sourceQueueId = null;
        if (data.sourceid) sourceQueueId = cleanGuid(data.sourceid);

        var queueItemProperties = null;
        if (data.queueItemProperties && data.queueItemProperties instanceof Sdk.Entity) {
            queueItemProperties = data.queueItemProperties;
        }

        Sdk.jQ.setJQueryVariable($);
        return Sdk.jQ.execute(new Sdk.AddToQueueRequest(target, sourceQueueId, destinationQueueId, queueItemProperties));
    }

    // arguments: queueItemId, workerId
    function executePickFromQueueRequest() {
        var args = getArgs(arguments, ["queueItemId", "workerId"]);

        if (_debug) log("executePickFromQueueRequest");
        Sdk.jQ.setJQueryVariable($);

        if (_currentUser == null && args.workerId == null) {
            return executeWhoAmIRequest(args).done(executePickFromQueueRequest);
        }

        if (args.workerId == null) args.workerId = _currentUser;

        if (_debug) executeForDebug("PickFromQueue", "Picking Item From Queue", args);

        //false: do not remove from queue
        return Sdk.jQ.execute(new Sdk.PickFromQueueRequest(
            cleanGuid(args.queueItemId),
            cleanGuid(args.workerId),
            false));
    }

    //arguments: queueItemId
    function executeReleaseToQueueRequest() {
        if (_debug) log("executeReleaseToQueueRequest");
        var args = getArgs(arguments, ["queueItemId"]);

        Sdk.jQ.setJQueryVariable($);
        if (_debug) executeForDebug("ReleaseToQueue", "Releasing Item Back To Queue", args);

        return Sdk.jQ.execute(new Sdk.ReleaseToQueueRequest(cleanGuid(args.queueItemId)));
    }

    function executeRouteToQueueRequest(queueItemId, queueId) {
        if (_debug) log("executeRouteToQueueRequest");
        var args = getArgs(arguments, ["queueItemId", "queueId"]);

        Sdk.jQ.setJQueryVariable($);
        if (_debug) executeForDebug("RouteTo", "Routing Item To Queue", args);

        return Sdk.jQ.execute(new Sdk.RouteToRequest(cleanGuid(args.queueItemId),
                                  new Sdk.EntityReference("queue", cleanGuid(args.queueId))));
    }

    function pickSelectedItem() {
        if (_debug) log("pickSelectedItem");
        loadingOn();
        var promises = [];
        $.each(QueueManager.SelectedItems, function (i, v) {
            //todo: use ExecuteMultiple
            var dfd = new $.Deferred();
            promises.push(dfd);
            executePickFromQueueRequest(v, _currentUser)
            .done(function () { dfd.resolve(); })
            .fail(function (msg) {
                log("pickSelectedItem error: " + msg);
            });
        });
        return $.when.apply(undefined, promises).promise().done(refreshQueueItems).done(function () {
            showQueueItemActions({ isworker: true });
        }).always(loadingOff);
    }

    function releaseSelectedItem() {
        if (_debug) log("releaseSelectedItem");
        loadingOn();
        var promises = [];
        $.each(QueueManager.SelectedItems, function (i, v) {
            var dfd = new $.Deferred();
            promises.push(dfd);
            executeReleaseToQueueRequest(v)
            .done(function () { dfd.resolve(); })
            .fail(function (msg) {
                log("releaseSelectedItem error: " + msg);
            });
        });
        return $.when.apply(undefined, promises).promise().done(refreshQueueItems).done(function () {
            showQueueItemActions({ isworker: "none" });
        }).always(loadingOff);
    }

    function refreshQueues() {
        if (_debug) log("refreshQueues");
        return retrieveQueueCounts().then(postRetrieveQueueCounts);
    }

    function route(itemid, queueid) {
        if (_debug) log("route");

        if (queueid == QueueManager.SelectedQueue) return new $.Deferred().promise().done();

        var promise = executeRouteToQueueRequest(itemid, queueid);

        var items = $("#panel_queueitems_area > tbody > tr");
        var item = items.filter(function (index) {
            var obj = $(this).data("obj");
            if (obj === undefined || obj === null) return false;
            return obj.id == itemid;
        });

        if (item) item.remove();

        return promise;
    }

    function routeSelectedItem(queueid) {
        if (_debug) log("routeSelectedItem");
        loadingOn();
        var promises = [];
        $.each(QueueManager.SelectedItems, function (i, v) {
            var dfd = new $.Deferred();
            promises.push(dfd.promise());
            route(v, queueid)
            .done(function () { dfd.resolve(); })
            .fail(function (msg) { log(msg); });
        });

        return $.when.apply(undefined, promises).promise().done(refresh).done(hideQueueItemActions).always(loadingOff);
    }

    function refresh() {
        return refreshQueues().then(refreshQueueItems);
    }

    function getSelectedQueueItemObjects() {
        var r = [];
        var rowids = _queueItemTable.getSelection(), row, data;
        $.each(rowids, function (i, v) {
            row = _queueItemTable.getTable().find("#" + v);
            data = row.data("obj");
            r.push({
                objectid: new Sdk.EntityReference(data.attributes.objectid.value.Type, data.attributes.objectid.value.Id),
                queueitemid: data.id,
                title: (data.attributes.title === undefined || data.attributes.title === null) ? "" : data.attributes.title.value,
                sensitivity_level: data.attributes["udo_sensitivitylevel"] !== undefined ? data.attributes["udo_sensitivitylevel"].value : null
            });
        });
        return r;
    }

    return {
        Queues: [],
        SelectedQueue: null,
        SelectedItems: [],
        GetItems: function () {
            if (_queueItemTable && _queueItemTable.getItems) return _queueItemTable.getItems();
            return null;
        },
        Pick: pickSelectedItem,
        Release: releaseSelectedItem,
        Route: route,
        Initialize: init,
        AddToQueue: executeAddToQueue,
        RefreshQueueItems: refreshQueueItems,
        RefreshQueues: refreshQueues,
        Refresh: refresh,
        SetAutoRefresh: setAutoRefresh,
        GetSelectedQueueItemObjects: getSelectedQueueItemObjects,
        UseShortNames: false,
        SetSpecialSituationVisibility: setSpecialSituationsVisibility
    }
}();