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

});