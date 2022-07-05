"use strict";

var Va = Va || {};
Va.Udo = Va.Udo || {};

Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};

Va.Udo.Crm.Scripts.Popup = function () {
    var _minWidth = 300;
    var _minHeight = 140;
    var _maxWidth = 600;
    var _maxHeight = 600;

    var _styles = {
        OKOnly: 0,
        OKCancel: 1,
        AbortRetryIgnore: 2,
        YesNoCancel: 3,
        YesNo: 4,
        RetryCancel: 5,
        NoButtons: 6,
        Normal: 0,
        Critical: 16,
        Question: 32,
        Exclamation: 48,
        Information: 64,
        DefaultButton1: 0,
        DefaultButton2: 256,
        DefaultButton3: 512
    };
    var _css = {
        //'selector': 'style'
        'div.popupNormal': {
            backgroundColor: "#5f697d",
            color: "#fff"
        },

        'div.popupCritical': {
            backgroundColor: "#AE1F23",
            color: "#fff"
        },

        'div.popupQuestion': {
            backgroundColor: "#0B5C9E",
            color: "#fff"
        },

        'div.popupExclamation': {
            backgroundColor: "#fc8f00",
            color: "#fff"
        },

        'div.popupInformation': {
            backgroundColor: "#0B5C9E",
            color: "#fff"
        },

        'div.popupContent': {
            backgroundColor: "#fff",
            padding: "10px",
            overflowX: "hidden",
            overflowY: "auto"
        },

        'div.popupTitle': {
            fontSize: "18px",
            padding: "10px"
        },

        'div.popupButtons': {
            backgroundColor: "#fff",
            color: "#000",
            fontSize: "11px",
            fontWeight: "600",
            position: "absolute",
            right: "10px",
            bottom: "10px"
        },

        'div.popupButtons button': {
            backgroundColor: "ButtonFace",
            fontSize: "11px",
            color: "ButtonText",
            cursor: "pointer",
            display: "inline-block",
            border: "1px solid ButtonText",
            height: "16px",
            margin: "3px 6px 2px",
            padding: "1px 15px",
            textAlign: "center"
        }
    };
    var _buttonTypes = {
        Ok: 1,
        Cancel: 2,
        Abort: 3,
        Retry: 4,
        Ignore: 5,
        Yes: 6,
        No: 7
    };
    _guid = function () {
        var randomValuesArray = new Uint16Array(8);
        var s4 = function (i) {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };

        var crypto = window.crypto;
        if (!crypto) crypto = window.msCrypto;
        if (crypto && crypto.getRandomValues) {
            crypto.getRandomValues(randomValuesArray);
            s4 = function (i) {
                var v = randomValuesArray[i].toString(16);
                while (v.length < 4) { v = "0" + v; }
                return v;
            };
        }

        // uses secure crypto is available
        return s4(0) + s4(1) + "-" + s4(2) + "-" + s4(3) + "-" + s4(4) + "-" + s4(5) + s4(6) + s4(7);
    };

    var _getButtonType = function (buttons) {
        return buttons & ((1 << 4) - 1);
    };
    var _getMessageType = function (buttons) {
        var withoutbuttontype = buttons & ~((1 << 4) - 1);
        return withoutbuttontype & ((1 << 8) - 1);
    };
    var _getDefaultBtn = function (buttons) {
        return buttons & ~((1 << 8) - 1);
    };
    var _getKey = function (arr, val) {
        for (var key in arr) {
            if (val == arr[key]) return key;
        }
        return "<null>";
    };
    var _applyCSS = function (el) {
        // Apply CSS to the element by class
        // This is done to support using the javascript without a corresponding css file
        for (var selector in _css) {
            var style = _css[selector];
            el.find(selector).css(style);
        }
    };
    var _popup = function (title, content, options) {
        var deferred = $.Deferred();

        var defaultOptions = {
            buttons: 1,
            width: 0,
            height: 0,
            modal: false,
            id: 'popup-' + _guid()
        };

        var PutInRange = function (i, min, max) {
            return Math.min(max, Math.max(i, min));
        };

        if (typeof options == 'undefined' || options == null) options = {};
        var settings = $.extend(true, defaultOptions, options);
        var pid = settings.id;  //popupId

        var popupWidth = PutInRange(settings.width, _minWidth, _maxWidth);
        var popupHeight = PutInRange(settings.height, _minHeight, _maxHeight);

        var buttonType = _getButtonType(settings.buttons);
        var messageType = _getMessageType(settings.buttons);
        var defaultBtn = _getDefaultBtn(settings.buttons);

        // Set Default Title
        var defaultTitle = 'Message';
        switch (messageType) {
            case _styles.Normal: defaultTitle = 'Message'; break;
            case _styles.Critical: defaultTitle = 'Critical Message'; break;
            case _styles.Question: defaultTitle = 'Question'; break;
            case _styles.Exclamation: defaultTitle = 'Important Message'; break;
            case _styles.Information: defaultTitle = 'Information'; break;
            default: defaultTitle = 'Message'; break;
        }
        if (title == null || title.length === 0) {
            title = defaultTitle;
        }

        // If content empty, set content to title
        if (content == null) {
            content = $("<span class='msgboxPrompt popupText' id='" + pid + "_popupMessage' />");
            content.text(title);
        }

        // Replace \r\n or \n with <br/>
        if (content.html) {
            if (content.attr) {
                content.attr("id", pid + "_popupMessage");
            }
            content = content[0].outerHTML;
        } else {
            if (content.outerHTML) {
                content = content.outerHTML;
            }
            content = "<div id='" + pid + "_popupMessage' >" + content + "</div>";
        }
        content = content.replace(/\r?\n/g, "<br/>");

        var popupStyle = "z-index:500;position:fixed;top:50%;left:50%;" +
                         "margin-left:-" + Math.floor(popupWidth / 2).toString() + "px;" +
                         "margin-top:-" + Math.floor(popupHeight / 2).toString() + "px;" +
                         "width:" + popupWidth.toString() + "px;height:" + popupHeight.toString() + "px;" +
                         "font-family:Segoe UI, Tahoma, Arial;font-size:11px;color:#000;background-color:#fff;" +
                         "border:3px solid #000;";


        var popupDiv = $("<div tabindex='0' class='popup' style='" + popupStyle + "' id='" + pid + "' role='alertdialog' aria-labelledby='" + pid + "_popupTitle' aria-describedby='" + pid + "_popupMessage' />");

        var titleClass = "popupTitle popup" + _getKey(_styles, messageType).toString();
        var titleDiv = $("<div class='" + titleClass + "' id='" + pid + "_popupTitle'/>");
        titleDiv.text(title);
        // draggable title area...

        var mousemoveEvent = function (e) {
            var popup = $("#" + pid);
            if (popup.data("dragging")) {
                var o = popup.data("dragoffset");
                popup.offset({
                    top: e.pageY - o.top,
                    left: e.pageX - o.left
                });
            }
        };

        var mouseupEvent = function (e) {
            var popup = $("#" + pid);
            popup.data("dragging", false);
            popup.data("dragoffset", null);
            $(this).removeAttr('unselectable');
            $(document.body).unbind('mousemove', mousemoveEvent);
            popup.unbind('mouseup', mouseupEvent);
        };

        titleDiv.css('cursor', 'move')
        .mousedown(function (e) {
            var popup = $("#" + pid);
            popup.data("dragging", true);
            var offset = {
                top: (e.pageY - $(this).offset().top),// + $(window).scrollTop(),
                left: (e.pageX - $(this).offset().left)// + $(window).scrollLeft()
            };
            $(this).attr('unselectable', 'on');
            popup.data("dragoffset", offset);
            // movement is tracked on body
            $(document.body).mousemove(mousemoveEvent);
            popup.mouseup(mouseupEvent);
        });

        popupDiv.append(titleDiv);

        var contentDiv = $("<div class='popupContent'/>");
        contentDiv.html(content);
        contentDiv.height((popupHeight - 70).toString() + "px");

        var buttonfocus = function () { };

        // Show Image
        if (options.showImage) {
            var baseUrl = Xrm.Utility.getGlobalContext().getClientUrl + "/WebResources/udo_/popup/img/"
            var imgsrc = "";
            switch (messageType) {
                case _styles.Critical: imgsrc = baseUrl + "critical_32.png"; break;
                case _styles.Question: imgsrc = baseUrl + "question_32.png"; break;
                case _styles.Exclamation: imgsrc = baseUrl + "exclamation_32.png"; break;
                case _styles.Information: imgsrc = baseUrl + "information_32.png"; break;
                default: break;
            }
            if (imgsrc !== "") {
                content = "<div id='popupLeftContent' style='float:left;width:40px'>" +
                          "<img src='" + imgsrc + "' alt='" + defaultTitle + "'/></div>" +
                          "<div id='popupRightContent' style='width:auto;margin-left:40px;'>" +
                          contentDiv.html() +
                          "</div>";
                contentDiv.html(content);
            }
        }

        popupDiv.append(contentDiv);

        var Button = function (popupId, deferred, buttonType, setfocus) {
            var name = _getKey(_buttonTypes, buttonType);
            var btn = $("<button type='button' id='popupBtn" + name + "' class='popupButton' />");
            //var btn = $("<button type='button' id='popupBtn" + name + "' class='popupButton' aria-label='" + ariaLabel + "' />");
            btn.text(name);
            var data = {
                PopupId: popupId,
                Deferred: deferred,
                ClickedButton: {
                    Name: name,
                    ButtonType: buttonType
                }
            };
            btn.click(data, function (e) {
                var deferred = e.data.Deferred;
                var clicked = e.data.ClickedButton;
                var clickType = e.data.ClickedButton.ButtonType;

                var values = {};
                var popup = $("#" + e.data.PopupId);
                $("#" + e.data.PopupId + " input").each(function () {
                    if (this.files) {
                        values[this.name] = { value: this.value, files: this.files };
                    } else {
                        values[this.name] = this.value;
                    }
                });
                $("#" + e.data.PopupId + " select").each(function () { values[this.name] = this.value; });
                $("#" + e.data.PopupId + " textarea").each(function () { values[this.name] = this.value; });

                var result = {
                    Clicked: clicked,
                    Values: values
                };

                popup.remove();  //get and remove the popup
                var overlay = document.getElementById('overlay-' + e.data.PopupId);
                if (typeof overlay !== "undefined" && overlay !== null) {
                    $(overlay).remove();
                }
                if (clickType === _buttonTypes.Cancel ||
                    clickType === _buttonTypes.Abort ||
                    clickType === _buttonTypes.No) {
                    deferred.reject(result);
                    return;
                }
                deferred.resolve(result);
            });

            if (setfocus) buttonfocus = function () { btn.focus(); }

            return btn;
        };

        var buttonsDiv = $("<div class='popupButtons'/>");
        switch (buttonType) {
            case _styles.OKCancel:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Cancel, defaultBtn == _styles.DefaultButton2));
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Ok, defaultBtn == _styles.DefaultButton1));
                break;
            case _styles.OKOnly:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Ok, defaultBtn == _styles.DefaultButton1));
                break;
            case _styles.AbortRetryIgnore:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Abort, defaultBtn == _styles.DefaultButton3),
                                  Button(pid, deferred, _buttonTypes.Retry, defaultBtn == _styles.DefaultButton2),
                                  Button(pid, deferred, _buttonTypes.Ignore, defaultBtn == _styles.DefaultButton1));
                break;
            case _styles.YesNoCancel:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Cancel, defaultBtn == _styles.DefaultButton3));
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Yes, defaultBtn == _styles.DefaultButton2),
                                  Button(pid, deferred, _buttonTypes.NodefaultBtn == _styles.DefaultButton1));
                break;
            case _styles.YesNo:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Yes, defaultBtn == _styles.DefaultButton1),
                                  Button(pid, deferred, _buttonTypes.No, defaultBtn == _styles.DefaultButton2));
                break;
            case _styles.RetryCancel:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Retry, defaultBtn == _styles.DefaultButton1),
                                  Button(pid, deferred_buttonTypes.Cancel, defaultBtn == _styles.DefaultButton2));
                break;
            case _styles.NoButtons:
                //No Buttons
                break;
            default:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Cancel, defaultBtn == _styles.DefaultButton2));
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Ok, defaultBtn == _styles.DefaultButton1));
                break;
        }

        popupDiv.append(buttonsDiv);

        _applyCSS(popupDiv);

        if (settings.modal) {
            var overlay = $("<div id='overlay-" + pid + "' style='top:0px:left:0px;position:absolute;width:100%;height:100%;z-index:10;background-color:rgba(0,0,0,0.5)'/>");
            $(document.body).append(overlay);
        }
        $(document.body).append(popupDiv);

        if (settings.hasOwnProperty("setFocus")) {
            settings.setFocus();
        } else if ($(popupDiv).find("input, select, textarea").length > 0) {
            popupDiv.find("input, select, textarea").first().focus();
        } else {
            buttonfocus();
        }

        return deferred.promise();
    };

    var _msgBox = function (textPrompt, buttons, title, options) {
        var content = $("<span class='msgboxPrompt popupText'/>");
        content.text(textPrompt);

        if (!options) options = {};
        options.buttons = buttons;
        if (!options.hasOwnProperty('showImage')) options.showImage = true;

        return _popup(title, content, options);
    };

    var _inputBox = function (textPrompt, defaultValue, title, options) {
        if (typeof title === 'undefined' || title === null) {
            title = 'Input';
        }

        var label = $("<span/>");
        label.text(textPrompt);

        var input = $("<input type='text' name='popupText'/>");
        var content = $("<span/>").append(label, input);
        return _popup(title, content, options);
    };

    var _errorMsg = function (message, options) {
        var title = (options && options.title) ? options.title : "Error";
        return _msgBox(message, _styles.Critical, title, options);
    };

    var _warning = function (message, options) {
        var title = (options && options.title) ? options.title : "Warning";
        return _msgBox(message, _styles.Exclamation, title, options);
    };

    var _alert = function (message, options) {
        var title = (options && options.title) ? options.title : "Alert";
        return _msgBox(message, _styles.Normal + _styles.OKOnly, title, options);
    };

    var _confirm = function (message, options) {
        var title = (options && options.title) ? options.title : "Confirm";
        return _msgBox(message, _styles.Normal + _styles.OKCancel, title, options);
    };

    var _remove = function (popupid) {
        var popup = $("#" + popupid);
        var modal = $("#modal-" + popupid);
        if (modal) modal.remove();
        if (popup) popup.remove();
    };

    return {
        Popup: _popup,
        InputBox: _inputBox,
        MsgBox: _msgBox,
        ButtonTypes: _buttonTypes,
        PopupStyles: _styles,
        Error: _errorMsg,
        Warning: _warning,
        Alert: _alert,
        Confirm: _confirm,
        Remove: _remove
    };
}();
