/// udo_tag.js
///   Example data params on WebResource:
///      entity=udo_flash&name=udo_name&fields=udo_name&filter=udo_VeteranId/Id eq guid'{id}'&orderby=udo_name asc
///
/// Params:
///    entity
///    orderby
///    fields
///    name
///    filter
///        Example: udo_VeteranId/Id eq guid'{id}' 
///    style - optional
///    format - optional
///        Example: {fieldname} - {fieldname}

var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};

Va.Udo.Crm.Scripts.Tag = {
    windowSize: {
        h: 0,
        w: 0
    },
    getFramedTags: function () {
        var id = Va.Udo.Crm.Scripts.Utility.getUrlParams().id;
        Va.Udo.Crm.Scripts.Tag.getTag(id);
    },
    getTag: function (idStr) {
        var par = Va.Udo.Crm.Scripts.Utility.getUrlParams();
        //var filter = "";
        var filter = par.data.filter.replace("{id}", idStr);
        var fields = par.data.fields.split(',');
        var foundName = false;
        for (var i = 0; i < fields.length; i++) {
            if (fields[i] == par.data.name) {
                foundName = true;
                break;
            }
        }
        if (!foundName) fields[fields.length] = par.data.name;
        CrmRestKit2011.ByQueryAllOrdered(par.data.entity, fields, filter, par.data.orderby, false)
        .done(function (data) {
            var tag = "";
            if (data && data.length > 0) {
                for (var index = 0; index < data.length; index++) {
                    var record = {};
                    for (var k in data[index]) {
                        record[k.toLowerCase()] = data[index][k];
                    }
                    var par = Va.Udo.Crm.Scripts.Utility.getUrlParams();
                    var style = "";
                    var tagName = record[par.data.name];
                    if ('format' in par.data) {
                        tagName = tagName.replace(/{(\d+)}/g, function (match, key) {
                            return typeof record[key] != 'undefined'
                              ? arr[key]
                              : match;
                        });
                    }
                    if ('style' in par.data) {
                        style = ' style="' + par.data.style + '"';
                    }
                    tag = '<p class="tag"' + style + '>' + tagName + '</p>';
                    $(tag).appendTo("#TagArea");
                }
            } else {
                // No Tag
            }

            //var frame = $(window.frameElement);
            //frame.height(document.body.offsetHeight);
            var onResize = function () {
                if (Va.Udo.Crm.Scripts.Tag.windowSize.h == window.top.document.documentElement.clientHeight &&
                    Va.Udo.Crm.Scripts.Tag.windowSize.w == window.top.document.documentElement.clientWidth) {
                    return;
                }
                // Reduce the height to 10px in order to recalculate on contracting height (reducing height)
                window.frameElement.style.height = "10px";
                document.body.style.height = "10px";
                // Set height back to auto on body, otherwise items will not be visible
                document.body.style.height = "auto";
                window.frameElement.style.height = document.body.offsetHeight + 'px';
                var parent = window.frameElement;
                var i = 0;
                do {
                    parent = parent.parentElement; i++;
                } while (parent.tagName.toUpperCase() !== "DIV" && i < 5)
                parent.style.height = window.frameElement.style.height;

                Va.Udo.Crm.Scripts.Tag.windowSize.h = window.top.document.documentElement.clientHeight;
                Va.Udo.Crm.Scripts.Tag.windowSize.w = window.top.document.documentElement.clientWidth;
            }
            var addEvent = function (el, event, callback) {
                if (el.addEventListener) {
                    el.addEventListener(event, callback, false);
                } else if (el.attachEvent) {
                    el.attachEvent('on' + event, callback);
                }
            };

            addEvent(window, "resize", onResize);
            onResize();
        })
        .fail(function (err) { })
    }
};
