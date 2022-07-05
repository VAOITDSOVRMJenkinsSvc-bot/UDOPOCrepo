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

        'div.popupButtons span': {
            backgroundColor: "ButtonFace",
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
        var crypt = window.crypto || window.msCrypto;
        if (crypt && crypt.getRandomValues) {
            var buf = new Uint16Array(8);
            crypt.getRandomValues(buf);
            var S4 = function (num) {
                var ret = num.toString(16);
                while (ret.length < 4) {
                    ret = "0" + ret;
                }
                return ret;
            };
            return (S4(buf[0]) + S4(buf[1]) + "-" + S4(buf[2]) + "-" + S4(buf[3]) + "-" + S4(buf[4]) + "-" + S4(buf[5]) + S4(buf[6]) + S4(buf[7]));
        }
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
            modal: false
        };

        var PutInRange = function (i, min, max) {
            return Math.min(max, Math.max(i, min));
        };

        if (typeof options == 'undefined' || options == null) options = {};
        var settings = $.extend(true, defaultOptions, options);

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
        if (title == null || title.length == 0) {
            title = defaultTitle;
        }

        // If content empty, set content to title
        if (content == null) {
            content = $("<span class='msgboxPrompt popupText'/>");
            content.text(title);
        }

        // Replace \r\n or \n with <br/>
        if (content.html) {
            content = content[0].outerHTML;
        } else {
            if (content.outerHTML) {
                content = content.outerHTML;
            }
            content = "<div>" + content + "</div>";
        }
        content = content.replace(/\r?\n/g, "<br/>");

        var popupStyle = "z-index:500;position:fixed;top:50%;left:50%;" +
                         "margin-left:-" + Math.floor(popupWidth / 2).toString() + "px;" +
                         "margin-top:-" + Math.floor(popupHeight / 2).toString() + "px;" +
                         "width:" + popupWidth.toString() + "px;height:" + popupHeight.toString() + "px;" +
                         "font-family:Segoe UI, Tahoma, Arial;font-size:11px;color:#000;background-color:#fff;" +
                         "border:3px solid #000;";

        var pid = 'popup-' + _guid();  //popupId
        var popupDiv = $("<div class='popup' style='" + popupStyle + "' id='" + pid + "'/>");

        var titleClass = "popupTitle popup" + _getKey(_styles, messageType).toString();
        var titleDiv = $("<div class='" + titleClass + "' id='popupTitle'/>");
        titleDiv.text(title);
        popupDiv.append(titleDiv);

        var contentDiv = $("<div class='popupContent'/>");
        contentDiv.html(content);
        contentDiv.height((popupHeight - 70).toString() + "px");

        // Show Image
        if (options.showImage) {
            var baseUrl = Xrm.Page.context.getClientUrl() + "/WebResources/udo_/popup/img/"
            var imgsrc = "";
            switch (messageType) {
                case _styles.Critical: imgsrc = baseUrl + "critical_32.png"; break;
                case _styles.Question: imgsrc = baseUrl + "question_32.png"; break;
                case _styles.Exclamation: imgsrc = baseUrl + "exclamation_32.png"; break;
                case _styles.Information: imgsrc = baseUrl + "information_32.png"; break;
                default: break;
            }
            if (imgsrc != "") {
                content = "<div id='popupLeftContent' style='float:left;width:40px'>" +
						  "<img src='" + imgsrc + "' alt='" + defaultTitle + "'/></div>" +
						  "<div id='popupRightContent' style='width:auto;margin-left:40px;'>" +
						  contentDiv.html() +
						  "</div>";
                contentDiv.html(content);
            }
        }

        popupDiv.append(contentDiv);

        var Button = function (popupId, deferred, buttonType) {
            var name = _getKey(_buttonTypes, buttonType);
            var btn = $("<span id='popupBtn" + name + "' class='popupButton'/>");
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
                var overlay = document.getElementById('overlay' + e.data.PopupId);
                if (typeof overlay !== "undefined" && overlay != null) {
                    $(overlay).remove();
                }
                if (clickType == _buttonTypes.Cancel ||
                    clickType == _buttonTypes.Abort ||
                    clickType == _buttonTypes.No) {
                    deferred.reject(result);
                    return;
                }
                deferred.resolve(result);
            });

            return btn;
        };

        var buttonsDiv = $("<div class='popupButtons'/>");
        switch (buttonType) {
            case _styles.OKCancel:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Cancel));
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Ok));
                break;
            case _styles.OKOnly:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Ok));
                break;
            case _styles.AbortRetryIgnore:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Abort), Button(pid, deferred, _buttonTypes.Retry), Button(pid, deferred, _buttonTypes.Ignore));
                break;
            case _styles.YesNoCancel:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Cancel));
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Yes), Button(pid, deferred, _buttonTypes.No));
                break;
            case _styles.YesNo:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Yes), Button(pid, deferred, _buttonTypes.No));
                break;
            case _styles.RetryCancel:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Retry), Button(pid, deferred_buttonTypes.Cancel));
                break;
            default:
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Cancel));
                buttonsDiv.append(Button(pid, deferred, _buttonTypes.Ok));
                break;
        }

        popupDiv.append(buttonsDiv);

        _applyCSS(popupDiv);

        if (settings.modal) {
            var overlay = $("<div id='overlay-" + pid + "' style='width:100%;height:100%;z-index:10;background-color:rgba(0,0,0,0.5)'/>");
            $(document.body).append(overlay);
        }
        $(document.body).append(popupDiv);

        return deferred.promise();
    };
    var _msgBox = function (textPrompt, buttons, title) {
        var content = $("<span class='msgboxPrompt popupText'/>");
        content.text(textPrompt);
        return _popup(title, content, { buttons: buttons, showImage: true })
    };
    var _inputBox = function (textPrompt, defaultValue, title) {
        if (typeof title == 'undefined' || title == null) {
            title = 'Input';
        }

        var label = $("<span/>");
        label.text(textPrompt);

        var input = $("<input type='text' name='popupText'/>");
        var content = $("<span/>").append(label, input);
        return _popup(title, content);
    };

    var _errorMsg = function (message) {
        _MsgBox(message, _styles.Critical, "Error");
    }

    return {
        Popup: _popup,
        InputBox: _inputBox,
        MsgBox: _msgBox,
        ButtonTypes: _buttonTypes,
        PopupStyles: _styles,
        Error: _errorMsg
    };
}();

function updateAttribute(attributeName, attributeValue) {
    try {

        var attribute = Xrm.Page.getAttribute(attributeName);
        attribute.setValue(attributeValue);
    }
    catch (e) {
        window.open("http://uii/Global Manager/ShowTab?RequestProcessHost");
        Va.Udo.Crm.Scripts.Popup
                .MsgBox("Could not set the relationship to Other.\nTry updating the relationship Interaction field and saving directly.",
                    Va.Udo.Crm.Scripts.Popup.PopupStyles.Critical,
                    "Interaction Update Error",
                { height: 200, width: 350 });
    }
}

updateAttribute("udo_relationship", 752280008);
Xrm.Page.data.save();