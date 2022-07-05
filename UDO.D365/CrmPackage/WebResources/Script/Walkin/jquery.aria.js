; (function ($, window, document, undefined) {
    var pluginName = "aria",
        defaults = {
            debug: false,
            audibleAlertContainer: "scralerts",
            alertErrorClass: "alert alert-validationerror",
            alertInfoClass: "alert alert-info alert-dismissable",
            srOnlyClass: "sr-only",
            log: null,
            logPrefix: "ARIA: ",
            Name: "ARIA",
            bindtowindow: false
        };
    

    // private methods
    var _private = {
        exception : function(msg) {
            this.message = msg;
            this.name = "AriaException";
        }
        ,sronly: {
            text: {
                handler: function(plugin, text, type) {
                    _private.log(plugin, "sronly.text.handler: handling sronly text call");
                    if (text===undefined) return _private.sronly.text.getText(plugin);
                    return _private.sronly.text.setText(plugin, text, type);
                }
                ,find: function (plugin) {
                    _private.log(plugin, "sronly.text.find: find children with the sronly class");
                    return plugin.$element.children("span."+plugin.options.srOnlyClass);
                }
                ,setText: function(plugin, text, type)
                {
                    var append = true;
                    if (type && type.toLowerCase()==="prepend") append = false;
                    _private.log(plugin, "sronly.text.setText: type: "
                        + (append ? "append" : "prepend")
                        + " text: "+text);

                    _private.sronly.text._remove(plugin);
                    
                    if (!append) {
                        return _private.sronly.text._prepend(plugin, msg);
                    }
                    return _private.sronly.text._append(plugin, msg);
                }
                ,getText: function(plugin) {
                    _private.log(plugin, "sronly.text.getText: getting sronly text");
                    return _private.sronly.text._find(plugin).text();
                }
                ,remove: function(plugin) {
                    _private.log(plugin, "sronly.text.remove: removing sronly text");
                    _private.sronly.text._find(plugin).remove();
                }
                ,create: function (plugin, msg) {
                    _private.log(plugin, "sronly.text.create: creating span for hidden text: "+msg);
                    var span = $("<span/>")
                        .addClass(plugin.options.srOnlyClass)
                        .text(msg);
                    return span;
                }
                ,prepend: function (plugin, msg) {
                    var span = _private.sronly.text.create(plugin, msg);
                    if (span) {
                        _private.log(plugin, "sronly.text.prepend: prepending msg");
                        plugin.$element.prepend(span);
                    }
                }
                ,append: function (plugin, msg) {
                    var span = _private.sronly.text.create(plugin, msg);
                    if (span) {
                        _private.log(plugin, "sronly.text.append: appending msg");
                        plugin.$element.append(span);
                    }
                }
            }
            , alert: {
                msg: function (plugin, msg, donotclear) {
                    _private.log(plugin, "sronly.alert: " + msg);
                    var container;
                    if (donotclear) {
                        container = _private.sronly.alert.getContainer(plugin);
                    } else {
                        container = _private.sronly.alert.clearContainer(plugin);
                    }

                    var alert = $("<p/>").text(msg);
                    container.append(alert);
                }
                , clearContainer: function (plugin) {
                    _private.log(plugin, "sronly.alert.clear: Clearing Alerts");
                    var container = _private.sronly.alert.getContainer(plugin);
                    container.html("");
                    return container;
                }
                , getContainer: function (plugin) {
                    _private.log(plugin, "sronly.alert.get: Getting Alert Container");
                    var container = document.getElementById(plugin.options.audibleAlertContainer);
                    if (!container) {
                        container = null;
                        _private.log(plugin, "sronly.alert.get: Unable to find Alert Container: '" + plugin.options.audibleAlertContainer + "'");
                    }
                    return $(container);
                }
            }
        }
        , log: function (plugin, msg, error) {
            var dfd = new $.Deferred(), options = plugin.options;
            msg = options.logPrefix + msg;
            if (error) {
                console.log(msg);
                dfd.reject(plugin, msg);
            } else if (debug) {
                if (_private.isFunction(options.log)) {
                    options.log(msg);
                    return;
                }
                console.log(msg);
                dfd.resolve(plugin, msg);
            }
            return dfd.promise();
        }
        /**
         * attr : sets /gets the aria attributes on an element or elements
         * @param {} el : jQuery element / items
         * @param {} o : The aria key or an object contains values to set
         * @param {} v : The aria value to set
         * @returns {} el : jQuery element / items / object containing aria items
         *
         * EXMAPLE: get all-attrbiutes
         * $.aria.attr(element)
         * output: {label: 'label', hidden: 'true'}
         *
         * EXAMPLE: get aria-label
         * $.aria.attr(element, 'label');
         * $.aria.attr(element, 'aria-label');
         * output: "label"
         *
         * EXAMPLE: set aria-label
         * $.aria.attr(elements, 'hidden', true);
         * $.aria.attr(elements, 'hidden', {toggle:true});
         */
        , attr: function (plugin, o, v) {
            //log(plugin,"Attribute Modfication/Access");
            var el = plugin.$element;
            var j, k
                , key = function (s) {
                    s = s.toLowerCase();
                    if (s.indexOf('aria-') == 0) return s;
                    return 'aria-' + s;
                }
                , toggle = (v && v.toggle)
                , save = function (k, v) {
                    k = k.toLowerCase();
                    if (k.indexOf('aria-') != 0) k = 'aria-' + k;
                    if (v === undefined || v === null) {
                        return el.attr(k)
                    }
                    if (toggle) v = !(el.attr(k).toLowerCase() === "true");
                    if (v instanceof Boolean) v = v ? 'true' : 'false';
                    return el.attr(k, v);
                };

            if (o === undefined && el.length == 1) {
                v = {};
                $.each(el[0].attributes, function (i, av) {
                    if (av.nodeName.toLowerCase().indexOf('aria-') == 0) {
                        j = av.nodeValue;
                        if (typeof j === "string") {
                            if (j.toLowerCase() === "true") j = true;
                            else if (j.toLowerCase() === "false") j = false;
                            else if (parseInt(j) == j) j = parseInt(j);
                        }
                        v[av.nodeName.toLowerCase().replace(/^aria-/, '')] = j;
                    }
                });
                return v;
            }
            else if (typeof o === "string") {
                return save(k, v);
            }
            else if (o instanceof Object) {
                for (k in o) {
                    save(k, o[k]);
                }
                return el;
            }
        }
        /**
         * remove : removes aria attrubtes
         *
         * Exmaples:
         *
         * // remove all aria attrbiutes
         * $.aria.remove(elements)
         * 
         * // remove specific aria elements
         * $.aria.remove
         */
        , remove: function (plugin, keys) {
            var el = plugin.$element;

            if (arguments.length > 2) {
                keys = $.makeArray(arguments).slice(1);
            }

            if (typeof keys === "string") keys = [keys];

            el.each(function (i, v) {
                if (keys === undefined) {
                    $.each($(v)[0].attributes, function (i, av) {
                        if (av.nodeName.toLowerCase().indexOf('aria-') == 0) {
                            $(v).removeAttr(av.nodeName);
                        }
                    });
                } else {
                    $.each(keys, function (i, k) {
                        $(v).removeAttr(k);
                    });
                }
            });
        }
        /**
         * hasAria : removes aria attrubtes
         *
         * Exmaples:
         *
         * // get all-attrbiutes
         * $.aria.hasAria(elements)
         * output: true if all elements have aria tags
         *         false if one does not
         */
        , hasAria: function (plugin) {
            var el = plugin.$element;
            var av, count = 0;

            if (!el) return false;

            el = $(el);
            if (el.length == 0) return false;

            $el.each(function (i, v) {
                for (av in $(v)[0].attributes) {
                    if (av.nodeName.toLowerCase().indexOf('aria-') == 0) {
                        count++;
                        return;
                    }
                }
            });

            return count == el.length;
        }
        , _getValues: function (v) {
            if (!v) return [];
            var j, r = v;
            if (!$.isArray(r)) r = [r];

            if (r.length == 0) return [];

            r = $.merge([], v);
            $.each(values, function (i, v) {
                v = v.toLowerCase();
            });

            return r;
        }
        /**
         * getRoles : get roles on the role attribute
         *
         * Exmaples:
         *
         * // get all roles (if multiple elements, the list will be combined)
         * $.aria.getRoles(elements)
         * output: ['menu','navigation']
         */
        , getRoles: function (plugin) {
            var el = plugin.$element;

            var roles = [];

            el.each(function (i, k) {
                roles = $.unique($.merge(roles, $(k).attr("role").toLowerCase().split(" ")));
            });

            return roles;
        }
        /**
         * addRole : adds a role attribute
         *
         * Exmaples:
         *
         * $.aria.addRole(elements, 'role')
         * output: elements
         */
        , addRole: function (plugin, v) {
            var roles = "", values = _private._getValues(v);
            var el = plugin.$element;

            if (values.length == 0) return el;

            el.each(function (i, k) {
                roles = $.unique($.merge([], _private.getRoles(k), values)).join(" ");
                if (roles.length > 0) $(k).attr("role", roles);
            });

            return el;
        }
        /**
         * hasRole : Determines if elements have a role or roles
         *
         * Exmaples:
         *
         * $.aria.hasRole(elements, 'role menuitem')
         * output: true/false
         */
        
        , hasRole: function (plugin, v) {
            var roles = [], j, k, n, r, values = _private._getValues(v);
            var el = plugin.$element;

            if (values.length == 0) return false;

            for (k = 0; k < el.length; k++) {
                roles = _private.getRoles(el[k]);
                if (roles.length == 0) return false;
                for (n = 0; n < values.length; n++) {
                    if ($.inArray(values[n], roles) == -1) return false;
                }
            }
            return true;
        }
        /**
         * toggleRole : toggle a role attribute
         *
         * Exmaples:
         *
         * $.aria.toggleRole(elements, 'role')
         * output: elements
         */ 
        , toggleRole: function (plugin, v, add) {
            var roles = "", values = _private._getValues(v), pos = 0;
            var el = plugin.$element;

            if (values.length == 0) return el;

            el.each(function (i, k) {
                roles = _private.getRoles(k);
                $.each(values, function (i, v) {
                    if (pos = $.inArray(v, roles) !== -1) {
                        roles.splice(pos, 1);
                    } else {
                        if (add) roles.push(v);
                    }
                });
                roles = $.unique(roles).join(" ");
                $(k).attr("role", roles);
            });

            return el;
        }
        , controls: {
            radio: {
                bind: function (e, control) {
                    $(control).unbind('keydown', _private.controls.radio.keydown);
                    $(control).bind('keydown', _private.controls.radio.keydown);
                },
                keydown: function (e) {
                    var target = $(e.target),
                        key = e.which || e.keyCode,
                        name = e.target.name,
                        i;

                    if (key == 9 || key == 16) return; //shift or tab

                    if (name) {
                        var items = $("input:radio[name='" + name + "']");
                        i = 0;
                        while (target.attr("id") != $(items[i]).attr("id")) {
                            i++;
                        }
                        if (key == 37 || key == 38) {
                            i--;
                            if (i >= 0) {
                                target = $(items[i]);
                            }
                        }
                        else
                            if (key == 39 || key == 40) {
                                i++;
                                if (i < items.length) {
                                    target = $(items[i]);
                                }
                            }
                        target.focus();
                        e.preventDefault();
                        e.stopPropagation();
                    }
                    if (key == 13 || key == 32) {
                        e.target.checked = true;
                        e.preventDefault();
                        e.stopPropagation();
                        $(e.target).click();
                    }
                }
            },
            dropdown: {
                // bind - how to configure a drop down
                bind: function (e, control) {
                    control = $(control);
                    var _dropdown = control.find("[data-toggle=dropdown]"),
                        _menus = control.find("ul"),
                        _items = _menus.find("li"),
                        _links = _items.find("a");

                    var _lostfocus = function (e) {
                        if (!control.hasClass('open')) return;
                        var _c = control[0],
                            _active = document.activeElement;

                        if (_c && _active) {
                            if (_c.id && _active.id && _c.id == _active.id) return;
                            if ($.contains(_c, _active)) return;
                        }

                        _dropdown.dropdown('toggle');
                    };

                    _dropdown.attr('role', 'button');
                    _menus.attr('role', 'menu');
                    _items.attr('role', 'presentation');

                    _links.attr({ 'role': 'menuitem', 'tabIndex': '-1' });
                    _links.on('keydown', function (e) {
                        if (e.which == 32 || e.which == 13) $(this).trigger("click");
                    })
                    .focusout(_lostfocus);

                    _dropdown.attr({ 'aria-haspopup': true, 'aria-expanded': false });

                    var _focusFirst = function () {
                        var _i = control.find('.dropdown-menu [role=menuitem]:visible')
                        _i && _i[0] && _i[0].focus && _i[0].focus();
                    }

                    control.on('shown.bs.dropdown', // opened / expanded
                        function () {
                            _dropdown.attr('aria-expanded', true);
                            _dropdown.on('keydown.bs.dropdown', _focusFirst);
                        })
                    .on('hidden.bs.dropdown', // closed / unexpanded
                        function (e) {
                            _dropdown.unbind('keydown.bs.dropdown', _focusFirst);
                            _dropdown.attr('aria-expanded', false);
                        })
                    .on('keydown.bs.dropdown.data-api', '[data-toggle=dropdown], [role=menu]',
                        $.fn.dropdown.Constructor.prototype.keydown)
                    .focusout(_lostfocus);

                }
            }
        }
        ,bindControls: function () {
            //ariaControls.dropdown.init();
            $("[data-toggle=dropdown]").parent().each(_private.controls.dropdown.bind);
            $("input.radio").each(_private.controls.radio.bind);
        }
        , init: function (plugin) {
            if (!window.aria || plugin.bindtowindow) {
                window.aria = plugin;
            }
            if (plugin.options.type === 'radio') {
                plugin.$element.each(_private.controls.radio.bind);
            }
            else if (plugin.options.type === 'dropdown') {
                plugin.$element.each(_private.controls.dropdown.bind);
            }
            //initialize here
        }
    };

    // The actual plugin constructor
    function Plugin(element, options) {
        var plugin = this;
        this.element = element;
        this.$element = $(element);

        this.options = $.extend({}, defaults, options);

        this._defaults = defaults;
        this._name = pluginName;

        this.init();
        return this;
    }

    Plugin.prototype = {
        init: function () {
            _private.init(this);
        }
        , sronly: {
            text : function (msg, placement) {
                return _private.sronly.text.handler(this, msg, placement);
            },
            alert: function (msg, donotclear) {
                var message = msg;
                
                if (msg.message) message = msg.message;
                if (msg.donotclear) donotclear = msg.donotclear;
                
                return _private.sronly.alert.msg(plugin, message, donotclear);
            }
        }
        , ariaAttr: function(o,v) {
            return _private.attr(this,o,v);
        }
        , removeAria: function(keys) {
            return _private.remove(this, keys);
        }
        ,hasAria: function() {
            return _private.hasAria(this);
        }
        ,hasRole: function(v) {
            return _private.hasRole(this,v);
        }
        ,addRole: function(v) {
            return _private.addRole(this,v);
        }
        ,removeRole: function (v) { return _private.toggleRole(this, v, false); }
        ,toggleRole: function (v) { return _private.toggleRole(this, v, true); }
        ,dropdown: function () {
            _private.controls.dropdown.bind(null, this.$element);
        }
        ,radio: function () {
            _private.controls.radio.bind(null, this.$element);
        }
    };
    
    //autobind existing controls for aria support.
    $(document).ready(_private.bindControls());

    // preventing against multiple instantiations - allows the execution of methods when called with a single item.
    $.fn[pluginName] = function (options) {
        return this.each(function () {
            var me = $(this);

            if (!me.data("plugin_" + pluginName)) {
                me.data("plugin_" + pluginName, new Plugin(this, options));
            }

            var plugin = me.data("plugin_" + pluginName);
            if (plugin[options]) {
                return plugin[options].apply(plugin, Array.prototype.slice.call(arguments, 1));
            }
        });
    };
})(jQuery, window, document);
