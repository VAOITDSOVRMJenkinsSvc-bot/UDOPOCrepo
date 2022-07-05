import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { stringify } from "querystring";

export class ClipboardTextbox implements ComponentFramework.StandardControl<IInputs, IOutputs> {


	private _value: string;
	// Power Apps component framework delegate which will be assigned to this object which would be called whenever any update happens.
	private _notifyOutputChanged: () => void;
	// label element created as part of this component
	private buttonElement: HTMLDivElement;
	// input element that is used to create the range slider
	private inputElement: HTMLInputElement;
	private textArea: HTMLTextAreaElement;
	// reference to the component container HTMLDivElement
	// This element contains all elements of our code component example
	private _container: HTMLDivElement;
	private _tableLayout: HTMLTableElement;
	// reference to Power Apps component framework Context object
	private _context: ComponentFramework.Context<IInputs>;
	// Event Handler 'refreshData' reference
	private _refreshData: EventListenerOrEventListenerObject;
	private _buttonClick: EventListenerOrEventListenerObject;
	private _keyDownDivButton: EventListenerOrEventListenerObject;


	/**
	 * Empty constructor.
	 */
	constructor() {

	}

	/**
	 * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
	 * Data-set values are not initialized here, use updateView.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
	 * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
	 * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
	 * @param container If a control is marked control-type='standard', it will receive an empty div element within which it can render its content.
	 */
	public init(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void, state: ComponentFramework.Dictionary, container: HTMLDivElement) {
		try {

			// Add control initialization code
			this._context = context;
			//this._container=container;
			this._container = document.createElement("div");
			this._container.className = "DivContainerStyle";

			//Create table Layout
			this._tableLayout = document.createElement("table");
			this._tableLayout.className = "TableLayout";
			let row = this._tableLayout.insertRow();
			row.tabIndex = -1;
			this._tableLayout.tabIndex = -1;
			//Cell where the texbox resides
			let textCell = row.insertCell();
			textCell.tabIndex = -1;
			textCell.className = "TextCell";
			//Cell where the copy button resides		
			let copyButtonCell = row.insertCell();
			copyButtonCell.className = "ButtonCell";
			copyButtonCell.tabIndex = -1;

			//Defne event handlers
			this._notifyOutputChanged = notifyOutputChanged;
			this._refreshData = this.refreshData.bind(this);
			this._buttonClick = this.buttonClick.bind(this);
			this._keyDownDivButton = this.divButtonKeyDown.bind(this);
			//Create Textbox
			this.inputElement = document.createElement("input");
			this.inputElement.tabIndex = 0;
			this.inputElement.setAttribute("id", "mcs_textboxClipboard");
			this.inputElement.className = "TextboxContainerStyle";
			this.inputElement.setAttribute("type", "text")
			this.inputElement.addEventListener("input", this._refreshData);
			let txtAriaLabel = (this._context.parameters.TextboxAriaLabel.raw as any) as string;
			this.inputElement.setAttribute("aria-label",txtAriaLabel);

			//Create the copy button

			this.buttonElement = document.createElement("div");
			this.buttonElement.className = "DivButton";
			//this.buttonElement.setAttribute("alt", "Click/Hit Enter to copy the phone number.");
			this.buttonElement.setAttribute("role", "button");
			let btnAriaLabel = (this._context.parameters.ButtonAriaLabel.raw as any) as string;
			this.buttonElement.setAttribute("aria-label",btnAriaLabel);
			//this.buttonElement.innerHTML="<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'><path class='icon-asx-grey' d='M10.707 3h-1l-3-3H1v13h4v3h10V7.293L10.707 3zM11 4.707L13.293 7H11V4.707zM2 12V1h4.293l2 2H5v9H2zm4 3V4h4v4h4v7H6z'/></svg>"
			
			let svgElement: SVGElement = document.createElementNS("http://www.w3.org/2000/svg", "svg");

			svgElement.setAttribute("viewBox", "0 0 16 16");
			svgElement.setAttribute("aria-label","Copy Button Graphic");
			svgElement.setAttribute("tabindex","-1");
			let svgPath: SVGPathElement = document.createElementNS("http://www.w3.org/2000/svg", "path");

		
			svgPath.setAttribute("class", "icon-asx-grey");
			svgPath.setAttribute("aria-label","Copy Path Graphic");
			svgPath.setAttribute("d", "M10.707 3h-1l-3-3H1v13h4v3h10V7.293L10.707 3zM11 4.707L13.293 7H11V4.707zM2 12V1h4.293l2 2H5v9H2zm4 3V4h4v4h4v7H6z");
			
			svgElement.appendChild(svgPath);
			this.buttonElement.appendChild(svgElement);

			this.buttonElement.addEventListener("click", this._buttonClick);
			this.buttonElement.addEventListener("keypress", this._keyDownDivButton);
			this.buttonElement.tabIndex = 0;;

			//Create copy control
			this.textArea = document.createElement("textarea");
			this.textArea.setAttribute("readonly", "");
			this.textArea.style.position = "absolute";
			this.textArea.style.left = '-9999px';
			this.textArea.tabIndex = -1;
			//this.textArea.hidden=true;		

			textCell.appendChild(this.inputElement);
			copyButtonCell.appendChild(this.buttonElement);
			this._container.appendChild(this._tableLayout);
			this._container.appendChild(this.textArea);
			this._value = (context.parameters.Attribute.raw as any) as string;
			container.appendChild(this._container);
			//somecomment
			this.inputElement.value = this._value;
		}
		catch (ex) {
			console.log(ex.message);
		}
	}

	/**
	 * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
	 * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
	 */
	public updateView(context: ComponentFramework.Context<IInputs>): void {
		try {
			// Add code to update control view
			this._value = (context.parameters.Attribute.raw as any) as string;
			this._context = context;
			this.inputElement.value = this._value;
		} catch (ex) {
			console.log(ex.message);
		}
	}

	/** 
	 * It is called by the framework prior to a control receiving new data. 
	 * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as “bound” or “output”
	 */
	public getOutputs(): IOutputs {
		return { Attribute: this._value };
	}

	public refreshData(evt: Event): void {
		try {
			this._value = (this.inputElement.value as any) as string;
			//this.inputElement.value = this.inputElement.value;
			this._notifyOutputChanged();
		} catch (ex) {
			console.log(ex.message);
		}
	}

	public divButtonKeyDown(evt: Event) {
		try {
			let kEvent = evt as KeyboardEvent;
			if (kEvent != null && kEvent.charCode === 13) {
				this.CopyText();
			}
		} catch (ex) {
			console.log(ex.message);
		}
		//}
	}

	private CopyText() {
		try {
			let re: RegExp = new RegExp(this._context.parameters.RegularExpresion.raw as any as string, "g");
			var formattedValue = this._value.replace(re, this._context.parameters.ReplaceWith.raw ?? "");
			//alert("Formatted Value:" + formattedValue);
			this.textArea.value = formattedValue;
			this.textArea.select();
			document.execCommand('copy');
		} catch (ex) {
			console.log(ex.message);
		}
	}

	public buttonClick(evt: Event): void {
		this.CopyText();
	}




	/** 
	 * Called when the control is to be removed from the DOM tree. Controls should use this call for cleanup.
	 * i.e. cancelling any pending remote calls, removing listeners, etc.
	 */
	public destroy(): void {
		// Add code to cleanup control if necessary
		this.inputElement.removeEventListener("input", this._refreshData);
		this.buttonElement.removeEventListener("click", this._buttonClick);
		this.buttonElement.removeEventListener("keypress", this._buttonClick);
	}
};