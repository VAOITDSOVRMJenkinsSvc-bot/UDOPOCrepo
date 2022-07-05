"use strict";
var Va = Va || {};
Va.Udo = Va.Udo || {};
Va.Udo.Crm = Va.Udo.Crm || {};
Va.Udo.Crm.Uci = Va.Udo.Crm.Uci || {};
Va.Udo.Crm.Uci.Buttons = Va.Udo.Crm.Uci.Buttons || {};
Va.Udo.Crm.Uci.Scripts = Va.Udo.Crm.Uci.Scripts || {};
Va.Udo.Crm.Uci.Scripts.Buttons = Va.Udo.Crm.Uci.Scripts.Buttons || {};

Va.Udo.Crm.Uci.Scripts.Buttons.formContext = null;

/// <summary>Adds a button to the form using a web resource control and the udo_embeddedbutton_UCI.html web resource</summary>
/// <param name="formCon" type="Object">formContext passed from the form</param>
/// <param name="id" type="string">id of the created button</param>
/// <param name="name" type="string">name of the created button</param>
/// <param name="label" type="string">label of the created button</param>
/// <param name="method" type="function">method to be executed when the button is clicked</param>
/// <param name="image" type="Object">image to display on the button</param>
/// <param name="enabled" type="boolean">true/false indicating if the button is enabled/disabled</param>
/// <returns type="undefined">does not return data; adds a button to the form</returns>
Va.Udo.Crm.Uci.Scripts.Buttons.AddButton = function (formCon, id, name, label, method, image, enabled) {
    Va.Udo.Crm.Uci.Scripts.Buttons.formContext = formCon;

    if (typeof enabled === "undefined" || enabled === null) {
        enabled = true;
    }

    var config = {};
    config.name = name;
    config.label = label;
    config.onClick = method;
    config.image = image;
    config.id = id;
    config.enabled = enabled;

    Va.Udo.Crm.Uci.Scripts.setupButton(config);
    Va.Udo.Crm.Uci.Buttons[id] = config;
};

Va.Udo.Crm.Uci.Scripts.setupButton = function (config) {
    var control = formHelper.getControl(config.id);

    control.getContentWindow().then(
        function (contentWin) {
            var button = contentWin.document.getElementById("embeddedbutton");

            // Set button text
            button.innerText = config.name;

            // Set button type
            button.setAttribute("type", "button");

            // Check click event exists, remove if so (only one click event per button)
            if (contentWin.buttonConfig !== null && contentWin.buttonConfig !== undefined && contentWin.buttonConfig.onClick !== null && contentWin.buttonConfig.onClick !== undefined) {
                button.removeEventListener("click", contentWin.buttonConfig.onClick);
            }

            if (config.enabled) {
                // Set button action
                button.addEventListener("click", config.onClick);
            } else {
                button.style.color = "red";
                button.innerText = config.name + " (Disabled)";
            }

            // Track click event and buttonConfig
            contentWin.buttonConfig = config;

            // Add button aria-label
            button.setAttribute("aria-label", config.name);

            // Add button name
            button.setAttribute("name", config.name);

            // Add button title
            button.setAttribute("title", config.name);
        }
    );
};