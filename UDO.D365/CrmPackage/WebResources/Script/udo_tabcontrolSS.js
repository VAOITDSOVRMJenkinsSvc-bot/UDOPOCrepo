<html><head>

    <title></title>
    <meta charset="utf-8">
    <style type="text/css">
		html,body {
			margin:0px;
			padding:0px;
        }

        .inactiveTab:hover {
            background-color: #C2C2C2;
        }

        .inactiveTab {
            background-color: #002050;
            color: #FFFFFF;
            font-family: Segoe UI, Tahoma, Arial;
            font-size: 12px;
            padding: 5px 10px 5px 10px;
            margin: 4px 4px 0px 0px;
            width: 120px;
            text-overflow: ellipsis;
            display: inline-block;
            text-align: center;
            overflow: hidden;
            white-space: nowrap;
        }

        .activeTab {
            background-color: #7A7A7A;
            color: #FFFFFF;
            font-family: Segoe UI, Tahoma, Arial;
            font-size: 12px;
            padding: 5px 10px 5px 10px;
            margin: 4px 4px 0px 0px;
            width: 120px;
            text-overflow: ellipsis;
            display: inline-block;
            text-align: center;
            overflow: hidden;
            white-space: nowrap;
        }
    </style>

    <script>

        //declare Namespaces
        var Va = Va || {};
        Va.Udo = Va.Udo || {};
        Va.Udo.Crm = Va.Udo.Crm || {};
        Va.Udo.Crm.Scripts = Va.Udo.Crm.Scripts || {};
        Va.Udo.Crm.Scripts.Code = Va.Udo.Crm.Scripts.Code || {};

        //this function is called from the onlick event on the tabs
        //it changes the CSS style associated with the tabs and sets 
        //the focus to the active section
        //
        //IN: tab - id of the tab to activate
        //IN:setFocusOnFirstControl - set to true or false. If set to true a method is invoked to set focus on the first visible control in the tab
        // If set to false or ignored, the method to setFocus is not invoked.
        //OUT: Nothing
        Va.Udo.Crm.Scripts.Code.activateTab = function (tab, setFocusOnFirstControl) {
            if ((parent.Xrm.Page.ui.tabs == null) || (parent.Xrm.Page.ui.tabs.getLength() < 2)) return; //nothing to do 
            var tabControl = document.getElementById('tabControl');
            if (tabControl == null) return; //nothing to do
            var selectedTab = parent.Xrm.Page.ui.tabs.get(tab);
            if (selectedTab == null) return; //nothing to do

            parent.Xrm.Page.ui.tabs.forEach(function (control, index) {
                if (!(control.getLabel() == "") || (control.getLabel() == "tabcontrol")) {
                    control.setDisplayState("collapsed");
                    control.setVisible(false);
                }
            });
            try {
                selectedTab.setVisible(true);
                selectedTab.setFocus();
                //Adding code to set focus on the first field of first section
                //if (setFocusOnFirstControl != 'undefined' && setFocusOnFirstControl === true) {
                Va.Udo.Crm.Scripts.Code.setFocusOnDefaultControl(selectedTab);
                // }
                selectedTab.setDisplayState("expanded");
                var tabToActivate = document.getElementById(tab);
                for (var index = 0; index < tabControl.childNodes.length; index++) {
                    var node = tabControl.childNodes[index];
                    if (node.nodeType == 1) { //make sure we have an element node
                        if (node == tabToActivate) {
                            node.className = "activeTab"
                        } else {
                            node.className = "inactiveTab";
                        } //if node is active
                    } //if nodeType
                } //For loop walking childNodes
            } catch (err) { //if error drop back to first tab
                parent.Xrm.Page.ui.tabs.forEach(function (control, index) {
                    control.setDisplayState("collapsed");
                    control.setVisible(false);
                });
                parent.Xrm.Page.ui.tabs.get(0).setVisible(true);
                parent.Xrm.Page.ui.tabs.get(0).setDisplayState("expanded");
                parent.Xrm.Page.ui.tabs.get(1).setVisible(true);
                parent.Xrm.Page.ui.tabs.get(1).setDisplayState("expanded");
                parent.Xrm.Page.ui.tabs.get(1).setFocus();
            }//End Try-Catch
        }// End Function activateTab

        ///This function sets the focus on the first visible control on the first visible section of xrmTab.
        ///Parameter: xrmTab should be the object returned from the Xrm.Page.ui.tabs.get(tab) script.
        ///All exceptions are swallowed as this is not a catastrophic bug.
        Va.Udo.Crm.Scripts.Code.setFocusOnDefaultControl = function (xrmTab) {
            var setFocusComplete = false;
            try {
                if (xrmTab != null) {
                    if (xrmTab.sections != null) {
                        xrmTab.sections.forEach(function (section, index) {
                            if (setFocusComplete === true)
                                return;
                            if (section.getVisible() === true) {
                                section.controls.forEach(function (control, index) {
                                    if (setFocusComplete === true)
                                        return;
                                    //if (control.getVisible() === true) {
                                        control.setFocus();
                                        setFocusComplete = true;
                                    //}
                                });
                            }
                        });
                    }
                }
            }
            catch (ex) {
                // Do not do anything. This exception just fails in setting focus. This is not a blocking exception.
            }

        }


        Va.Udo.Crm.Scripts.Code.WindowSize = { w: 0, h: 0 };
        //this runs as the page loads and creates the tabs by
        //walking though the tab contorls and grabing thier names and ids
        //
        //IN: nothing
        //OUT: nothing
        Va.Udo.Crm.Scripts.Code.buildTabs = function () {
            try {
                if ((parent.Xrm.Page.ui.tabs == null) || (parent.Xrm.Page.ui.tabs.getLength() < 2)) return; //nothing to do 
                var tabControl = document.getElementById("tabControl");
                if (tabControl == null) return; //nothing to do

                parent.Xrm.Page.ui.tabs.forEach(function (control, index) {
                    control.setDisplayState("collapsed");
                    control.setVisible(false);
                });
                parent.Xrm.Page.ui.tabs.get(0).setDisplayState("expanded");
                parent.Xrm.Page.ui.tabs.get(0).setVisible(true);
                parent.Xrm.Page.ui.tabs.get(1).setDisplayState("expanded");
                parent.Xrm.Page.ui.tabs.get(1).setVisible(true);
                var tabs = parent.Xrm.Page.ui.tabs.get();

                if (tabs == null) return; // there are no tabs to build

                for (var index in tabs) {
                    var tab = tabs[index];
                    var tabStyle = "inactiveTab";
                    if (index == 1) { //set first tab to active when the page loads
                        tabStyle = "activeTab"
                        Va.Udo.Crm.Scripts.Code.setFocusOnDefaultControl(tab);
                    }
                    var tabLabel = tab.getLabel();
                    var tabName = tab.getName();
                    if (!((tabLabel.indexOf("~hid~") == 0) || (tabName == "displaytabs"))) { //skip hidden tabs 
                        document.getElementById("tabControl").innerHTML +=
                            "<span class=" + tabStyle + " id=\"" + tabName + "\"" +
                            "role=\"button\" aria-label=\"Tab - " + tabLabel + "\" Title=\" Tab - " + tabLabel + "\"" +
                            "tabindex=\"0\"" +
                            "accesskey=\"" + index + "\"" +
                            "onclick=\"Va.Udo.Crm.Scripts.Code.activateTab('" + tabName + "');\" " +
                            "onkeypress=\"Va.Udo.Crm.Scripts.Code.activateTab('" + tabName + "');\" " +
                            ">" +
                            tabLabel + "</span>";
                    }
                }// For loop walking tabs

                // Handle Resize of Windows
                var addEvent = function (object, type, callback) {
                    if (object == null || typeof (object) == 'undefined') return;
                    if (object.addEventListener) {
                        object.addEventListener(type, callback, false);
                    } else if (object.attachEvent) {
                        object.attachEvent("on" + type, callback);
                    } else {
                        object["on" + type] = callback;
                    }
                };

                var onResize = function () {

                    if (Va.Udo.Crm.Scripts.Code.WindowSize.h == window.top.document.documentElement.clientHeight &&
					    Va.Udo.Crm.Scripts.Code.WindowSize.w == window.top.document.documentElement.clientWidth) {
                        return; // Window was not resized!
                    }

                    window.frameElement.style.height = "10px";
                    document.body.style.height = "10px";
                    document.body.style.height = "auto";

                    window.frameElement.style.height = document.body.offsetHeight + 'px';
                    var parent = window.frameElement;
                    var i = 0;
                    do {
                        parent = parent.parentElement; i++;
                    } while (parent.tagName.toUpperCase() !== "DIV" && i < 5 && parent.parentElement !== null)
                    if (parent !== null) parent.style.height = window.frameElement.style.height;
                    Va.Udo.Crm.Scripts.Code.WindowSize.h = window.top.document.documentElement.clientHeight;
                    Va.Udo.Crm.Scripts.Code.WindowSize.w = window.top.document.documentElement.clientWidth;
                }

                onResize();
                addEvent(window, "resize", onResize);

            } catch (err) {
                //nothing to do here but return 
            }
        }//End Function buildtabs

    </script>
</head>

<body>
    <div tabindex="0" title="Tab Menu" id="tabControl"></div>    
    <script>
        //call the function to create the initial tab lay out
        Va.Udo.Crm.Scripts.Code.buildTabs();
    </script>

</body></html>