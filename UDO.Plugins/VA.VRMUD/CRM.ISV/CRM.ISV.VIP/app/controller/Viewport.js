/**
* @author Jonas Dawson
* @class VIP.controller.Viewport
*
* The main controller for the VIP application. Populates the viewport.
*/
Ext.define('VIP.controller.Viewport', {
	extend: 'Ext.app.Controller',

	refs: [
		{
			ref: 'viewport',
			selector: '*'
		}
	],

	init: function () {
		//var me = this;
		//this.application.fireEvent('viewportinit');
		Ext.log('The viewport controller has been initialized');
	},

	onLaunch: function () {
		Ext.log('The viewport has been created and added.');
	}
});