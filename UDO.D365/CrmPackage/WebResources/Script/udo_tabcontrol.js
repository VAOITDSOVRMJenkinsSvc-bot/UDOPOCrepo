<html><head>
    <title></title>
    <meta charset="utf-8">
    <style type="text/css">
        body {
            margin: 0px;
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
            margin: 0px 4px 0px 0px;
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
            margin: 0px 4px 0px 0px;
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
        //OUT: Nothing
        Va.Udo.Crm.Scripts.Code.activateTab = function (tab) {
            if ((parent.Xrm.Page.ui.tabs == null) || (parent.Xrm.Page.ui.tabs.getLength() == 0)) return; //nothing to do 
            var tabControl = document.getElementById('tabControl');
            if (tabControl == null) return; //nothing to do
            var selectedTab = parent.Xrm.Page.ui.tabs.get(tab);
            if (selectedTab == null) return; //nothing to do

            parent.Xrm.Page.ui.tabs.forEach(function (control, index) {
                control.setDisplayState("collapsed");
                control.setVisible(false);
            });
            try {
                selectedTab.setVisible(true);
                selectedTab.setFocus();
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
                parent.Xrm.Page.ui.tabs.get(0).setFocus();
            }//End Try-Catch
        }// End Function activateTab

        //this runs as the page loads and creates the tabs by
        //walking though the tab contorls and grabing thier names and ids
        //
        //IN: nothing
        //OUT: nothing
        Va.Udo.Crm.Scripts.Code.buildTabs = function () {
            try {
                if ((parent.Xrm.Page.ui.tabs == null) || (parent.Xrm.Page.ui.tabs.getLength() == 0)) return; //nothing to do 
                var tabControl = document.getElementById("tabControl");
                if (tabControl == null) return; //nothing to do

                parent.Xrm.Page.ui.tabs.forEach(function (control, index) {
                    control.setDisplayState("collapsed");
                    control.setVisible(false);
                });
                parent.Xrm.Page.ui.tabs.get(0).setDisplayState("expanded");
                parent.Xrm.Page.ui.tabs.get(0).setVisible(true);
                var tabs = parent.Xrm.Page.ui.tabs.get();
                if (tabs == null) return; // there are no tabs to build
                for (var index in tabs) {
                    var tab = tabs[index];
                    var tabStyle = "inactiveTab";
                    if (index == 0) { //set first tab to active when the page loads
                        tabStyle = "activeTab"
                    }
                    if (tab.getLabel() != "") { //skip hidden tabs 
                        document.getElementById("tabControl").innerHTML = document.getElementById("tabControl").innerHTML + "<span class=" + tabStyle + " id=\"" + tab.getName() + "\" onclick=\"Va.Udo.Crm.Scripts.Code.activateTab('" + tab.getName() + "');\">" + tab.getLabel() + "</span>";
                    }
                }// For loop walking tabs
            } catch (err) {
                //nothing to do here but return 
            }
        }//End Function buildtabs

    </script>
<meta><meta><meta><meta><meta><meta></head>

<body style="-ms-word-wrap: break-word;">
    <div id="tabControl"></div>
    <script>
        //call the function to create the initial tab lay out
        Va.Udo.Crm.Scripts.Code.buildTabs();
    </script>



</body></html>