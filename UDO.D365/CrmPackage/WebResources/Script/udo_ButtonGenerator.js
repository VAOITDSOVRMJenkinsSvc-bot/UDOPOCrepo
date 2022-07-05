var ButtonGenerator = (function () {
	/////
	// Private Fields
	/////
	var buttonIds = [];

	/////
	// Private Methods
	/////
	function logError(method, message) {
		console.log("An error occured within udo_ButtonGenerator.js. Method: " + method + "; Error Message: " + message);
	}

	/////
	// Public Methods
	/////

	/// <summary>Removes any current buttons on the document, and creates new ones to be presented in the form control</summary>
	/// <param name="buttonArray" type="Array">Array of button objects to be created; each button object should contain the following properties: id, text, action. The following properties are optional: name, title.</param>
	/// <returns type="Boolean">T/F value indicating if the function succeeded or not</returns>
	function CreateNewButtons(buttonArray) {
		try {
			// Remove all current buttons
			RemoveAllButtons();

			// Create new buttons
			buttonArray.forEach(function (btn) {
				buttonIds.push(btn.id);
				AppendButton(btn);
			});
			return true;
		} catch (e) {
			// Handle error 
			logError("CreateNewButtons", e.message);
			return false;
		}
	}

	/// <summary>Removes any current buttons on the document</summary>
	/// <param>None</param>
	/// <returns type="Boolean">T/F value indicating if the function succeeded or not</returns>
	function RemoveAllButtons() {
		try {
			// Iterate and remove each button
			while (buttonIds.length !== 0) {
				RemoveButton(buttonIds[0]);
			}
			return true;
		} catch (e) {
			// Handle error
			logError("RemoveAllButtons", e.message);
			return false;
		}
	}

	/// <summary>Creates a new button on the document</summary>
	/// <param name="buttonObj" type="Object">Button object to be created; should contain the following properties: id, text, action. The following properties are optional: name, title.</param>
	/// <returns type="Boolean">T/F value indicating if the function succeeded or not</returns>
	function AppendButton(buttonObj) {
		try {
			// Create new button element
			newButton = document.createElement("button");
			newButton.id = buttonObj.id;
			newButton.innerText = buttonObj.text;

			// Set button type
			newButton.setAttribute("type", "button");

			// Set button action
			newButton.addEventListener("click", buttonObj.action);

			// Add button aria-label
			newButton.setAttribute("aria-label", buttonObj.text);

			// Add button name
			if (buttonObj.name !== null && buttonObj.name !== undefined) {
				newButton.setAttribute("name", buttonObj.name);
			} else {
				newButton.setAttribute("name", buttonObj.text);
			}

			// Add button title
			if (buttonObj.title !== null && buttonObj.title !== undefined) {
				newButton.setAttribute("title", buttonObj.title);
			} else {
				newButton.setAttribute("title", buttonObj.text);
			}
			
			// Add button element to DOM
			document.getElementById("buttons-div").appendChild(newButton);
			return true;
		} catch (e) {
			// Handle error
			logError("AppendButton", e.message);
			return false;
		}
	}

	/// <summary>Removes the specified button from the document</summary>
	/// <param name="btnId" type="String">String ID (DOM ID) of the button to be removed from the document</param>
	/// <returns type="Boolean">T/F value indicating if the function succeeded or not</returns>
	function RemoveButton(btnId) {
		try {
			// Retrieve button index from internal tracking array
			var index = buttonIds.indexOf(btnId);

			//Confirm button exists
			if (index !== -1) {
				// Remove button from DOM 
				var buttonsDiv = document.getElementById("buttons-div");
				var btn = document.getElementById(btnId);
				buttonsDiv.removeChild(btn);

				// Remove button id from internal tracking array
				buttonIds.splice(index, 1);
			}
			return true;
		} catch (e) {
			// Handle error
			logError("RemoveButton", e.message);
			return false;
		}
	}

	/// <summary>Returns the number of buttons on the current document</summary>
	/// <param>None</param>
	/// <returns type="Number">Integer value indicating the number of buttons on the current document</returns>
	function GetNumberOfButtons() {
		return buttonIds.length;
	}

	// Return Public Object
	return {
		CreateNewButtons: CreateNewButtons,
		RemoveAllButtons: RemoveAllButtons,
		RemoveButton: RemoveButton,
		AppendButton: AppendButton,
		GetNumberOfButtons: GetNumberOfButtons
	}
})();