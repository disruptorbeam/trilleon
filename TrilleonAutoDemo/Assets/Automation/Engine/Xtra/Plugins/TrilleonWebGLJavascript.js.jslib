mergeInto(LibraryManager.library, {

  ReportXmlResults: function(xml) {

  	//Create hidden element with the test results stored in it.
	var el = document.createElement('input');
	el.id = "Trilleon_Xml_Results";
	el.type = "hidden";
	el.value = Pointer_stringify(xml);
	document.getElementsByTagName("body")[0].appendChild(el);   

  },

  ReportJsonResults: function(json) {

  	//Create hidden element with the test results stored in it.
	var el = document.createElement('input');
	el.id = "Trilleon_Json_Results";
	el.type = "hidden";
	el.value = Pointer_stringify(json);
	document.getElementsByTagName("body")[0].appendChild(el);   

  },

  ScreenshotResponse: function(json) {

  	//Create hidden element with the test results stored in it.
	var el = document.createElement('input');
	el.id = "screenshot_response";
	el.type = "hidden";
	el.value = Pointer_stringify(json);
	document.getElementsByTagName("body")[0].appendChild(el);   

  },

   AutomationReady: function() {

  	//Create hidden element with the test results stored in it.
	var el = document.createElement('input');
	el.id = "automation_ready";
	el.type = "hidden";
	el.value = true;
	document.getElementsByTagName("body")[0].appendChild(el);   

  },

});