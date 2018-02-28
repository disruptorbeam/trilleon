var all_tests = []
var SCREENSHOT_STAGE_DELIMITER = "PT_STAGE_";
var last_opened_display_panel;
var is_screenshot_report = false;
var GAME_NAME, GAME_ENV, BUILD_NUMBER, DOMAIN_URL;

$(function() {

	GAME_NAME = $("#game_name_hidden").val();
	GAME_ENV = $("#game_env_hidden").val();
	BUILD_NUMBER = $("#build_number_hidden").val();
	DOMAIN_URL = $("#domain_url_hidden").val();
	if(typeof DOMAIN_URL != "undefined" && !DOMAIN_URL.endsWith("/")) {
		DOMAIN_URL += "/";
	}
	var test_run_type = $("#test_run_type").val();
	is_screenshot_report = test_run_type.toLowerCase() == "image_compare";

	if(!is_screenshot_report) {

		testResultsJson = currentJsonToRender = JSON.parse(document.getElementById("results_hidden").value.replace(/@APOS@/g,"\"").replace(/\n/g,"\\n").replace(/\t/g,"\\t").replace(/\r/g,"\\r"));

		//If this is a single-test temporary report, parse out all but requested test from table.
		var filter_test = $("#filter_single_test").val();
		if(filter_test != null && filter_test.length > 0) {
			currentJsonToRender = [];
			for(var j = 0; j < testResultsJson.length; j++) {
				if(testResultsJson[j].name == filter_test) {
					currentJsonToRender.push(testResultsJson[j]);
				}
			}

			//Hide elements irrelevant to a single test.
			$(".automation_summary").remove();
			$(".automation_summary_collapse").remove();
			$(".tag_data").remove();
			$(".button_toggle").remove();
			var critHeader = $(".critical_error_detected");
			critHeader.next().remove();
			critHeader.remove();
			$(".switch_to_from_chart").remove();
			var h2 = $(".container").find("h2");
			h2.text("Report For Single Test: " + filter_test);
			h2.css("margin-top", "100px").css("margin-bottom", "50px");

		}

		$(".automation_summary_collapse").html(arrowRight + " Details");
		$(".background_transparency").on("click", function() {
			if(typeof last_opened_display_panel != "undefined") {
				last_opened_display_panel.hide(400);
			}
			setTimeout(function() {
				if($(".display_panel:visible").length == 1) {
					$(".background_transparency").hide(400);
					$(".display_panel").hide(400);
				}
			}, 50);
		});

		BuildTable();

		if($("#communications_history").length > 0) {
			BuildCommunicationHistory();
		}

		//Build memory usage chart.
		google.charts.load("visualization", "1", {packages: ['corechart', 'line']});
		if($("#filter_single_test").length == 0) {
			google.charts.setOnLoadCallback(SetGraphs);
		}

		// Add build number with url to Details.
		var job_url_html = $("#build_name_hidden").lenght > 0 ? "<a href='" + DOMAIN_URL + $("#build_name_hidden").val() + "'>" + $("#build_name_hidden").val() + "</a>" : "No Link";
		var build_url_html = $("#build_name_hidden").lenght > 0 ? "<a href='" + DOMAIN_URL + $("#build_name_hidden").val() + "/" + BUILD_NUMBER + "'>" + BUILD_NUMBER + "</a>" : "No Link";
		$(".automation_summary").append("<div><span class='tag_data'><strong>Build Number</strong> Job [ " + job_url_html + " ] Build [ " + build_url_html + " ]</span></div>");
		$(".automation_summary").append("<div><span class='tag_data'><strong>Test Run ID</strong> [ " + $("#test_run_hidden").val() + " ]</span></div>");

	}

	if(is_screenshot_report || test_run_type == "full") {
		BuildScreenshotGallery();
	}

});

var testResultsJson = [];
var currentJsonToRender = [];
var arrowRight = "&#x25B6;";
var arrowDown = "&#x25BC;";

function BuildTable() {

	$("#bottom_panel").hide();
	var page_length = 15;
	var test_results = $("#test_results");
	test_results.DataTable({
        "aaData": currentJsonToRender,
        "bLengthChange": false,
        "bDestroy": true,
        "bSearchable": true,
        "bSort": true,
        "order": [[ 5, "asc" ]],
        "bPaginate": true,
        "pageLength": page_length,
        "rowCallback": function(row, data, index) {

         	 var details_cell = $(row).find(".assertions");
         	 var assertions = data.assertions;
         	 var assertions_html = "";

         	 if(assertions.length > 0) {

	         	for(var a = 0; a < assertions.length - 1; a++) {
	         		var assertion = unescape(assertions[a].assertion);
	         	 	if(assertion.indexOf("--WARNING--") >= 0) {
	         	 		$("#warnings_button").show();
	         	 		var warning_list = $("#warnings_panel").find(".message_exchange_list");
	         	 		var existing_warnings = warning_list.find(".warning_message");
	         	 		var already_reported = false;
	         	 		for(var w = 0; w < existing_warnings.length; w++) {
	         	 			if($(existing_warnings[w]).text() == assertion) {
	         	 				already_reported = true;
	         	 			}
	         	 		}
	         	 		if(!already_reported) {
	         	 			warning_list.append("<div class='warning_message'>" + assertion + "</div>");
	         			}
	         	 	}
	         	 	assertions_html += "<div><div class='check_green' style='width:5%;'>&#x2713;</div><div class='assertion_item' style='width:95%;'>" + assertion + "</div></div>";
	         	}
	         	if(data.status == "Failed") {
	         	 	var error = assertions[assertions.length - 1].assertion;
	         		details_cell.text(error);
	         		if(error.indexOf("SECONDARY CIRCULAR DEPENDENCY FAILURE DETECTED") >= 0 || error.indexOf("FATAL INTERNAL ERROR DETECTED; TEST RUN ABORTED!") >= 0){
	         	 		$(row).addClass("status_critical_error_show");
	         	 		$(".critical_error_detected").show();
	         	 	}
	         	 	assertions_html += "<div><span class='check_red' style='width:5%;'>&#x2717;</span><span class='assertion_item' style='width:95%;'>" + error + "</span></div>";
	         	}
	         	if(data.status == "Passed"){
	         	 	$(".error_button").removeClass("error_button").addClass("passed_button");
	         	 	assertions_html += "<div><div class='check_green' style='width:5%;'>&#x2713;</div><div class='assertion_item' style='width:95%;'>" + unescape(assertions[assertions.length - 1].assertion) + "</div></div>";
	         	}
	         	$(row).find(".assertions").html(assertions_html);

         	 }

	         switch($($(row).find("td")[3]).text()){
		     	case "Success":
	          		$(row).addClass("status_success_hide");
	          		break;
	          	case "Failure":
	          		if(!$(row).hasClass("status_critical_error_show")){
	          			$(row).addClass("status_failure_hide");
	          		} else {
	          			$(row).find(".status_failure").removeClass("status_failure").css("color", "white");
	          		}
	          		break;
	          	case "Ignored":
	          		$(row).addClass("status_ignored_hide");
	          		break;
	          	case "Skipped":
	          		$(row).addClass("status_skipped_hide");
	          		break;
	          }
		 },
        "aoColumns": [
          {
            "mDataProp": "name",
            "mRender": function (data) {
	              //Limit the rendered string, and allow it to be hovered over for the full test name.
	              all_tests.push(data);
	              if(data.length > 50) {
	            	return "<div title='" + data + " onmouseover='$(this).next().show(150);' onmouseout='$(this).next().hide(150);' style='cursor:pointer;'>" + data.substring(0, 50) + "...</div><div class='tooltip_expander'>" + data + "</div>";
	              }
	              return "<div title='" + data + "'>" + data + "</div>";
            }
          },
          {
            "mDataProp": "class",
            "mRender": function (data) {
              	return data;
            }
          },
          {
            "mDataProp": "test_categories",
            "mRender": function (data) {
	              //Limit the rendered string, and allow it to be hovered over for the full list of category names.
	              if(data.length > 10) {
	            	return "<div type='button' onmouseover='$(this).next().show(150);' onmouseout='$(this).next().hide(150);' style='cursor:pointer;'>" + data.substring(0, 10) + "...</div><div class='tooltip_expander'>" + data + "</div>";
	              }
	              return "<div title='" + data + "'>" + data + "</div>";
            }
          },
          {
            "mDataProp": "status",
            "mRender": function (data) {
	              var html = ""; 
	              switch(data){
	              	case "Passed":
	              		html = "<div class='status_success'>Success</div>";
	              		break;
	              	case "Failed":
	              		html = "<div class='status_failure'>Failure</div>";
	              		break;
	              	case "Ignored":
	              		html =  "<div class='status_ignored'>Ignored</div>";
	              		break;
	              	case "Skipped":
	              		html = "<div class='status_skipped'>Skipped</div>";
	              		break;
	                default:
	                	html = "<div class='status_unrecognized'>Unrecognized Status!</div>";
	                	break;
	              }
	              return html;
            }
          },
          {
          	"orderable": false,
            "mDataProp": "result_details",
            "mRender": function (data, type, full) {

	             var ignored = data.indexOf("#IGNORED#") == 0;
	             var skipped = data.indexOf("#SKIPPED#") == 0;
	             var passed = full.status == "Passed";
	             var className = "";
	             if(ignored){
	             	className = "ignored_button";
	             } else if(skipped){
	             	className = "skipped_button";
	             } else if(passed){
	             	className = "passed_button";
	             } else {
	             	className = "error_button";
	             }
	             var data = data.replace(/#SKIPPED#/g,"").replace(/#IGNORED#/g,"");
	             return "<div class='" + className + "' type='button' onClick='ShowDetails($(this));'>Details</div><div style='display:none;'>" + data + "</div><div style='display:none;' class='assertions'></div>";
	          }
            },
            {
	          "mDataProp": "order_ran",
	          "bVisible": false,
	          "mRender": function (data) {
		              return data;
		      }
            }
          ]
      });
		
	  if($(".warning_message").length == 0) {
		  $("#warnings_button").hide();
	  }

      if(test_results.DataTable().rows().count() == 1) {
        $("#test_results_filter").remove(); 
        $(".dataTables_paginate").remove(); 
        $(".dataTables_info").remove(); 
      } else if(test_results.DataTable().rows().count() <= page_length) {
        $(".dataTables_paginate").remove(); //Remove pagination buttons if there is only one page of results.
      }

      if($(".warning_message").length > 0) {
      	$("#warnings_button").after("<div class='notice_me_senpai'></div>");
      }

      $("#memory_graphs").hide(); //Hide graphs panel until user switches to it.

}

var last_selected_button;
var opening;

function SetGraphs() {

	//Garbage Collection
    var data = new google.visualization.DataTable();
    data.addColumn('date', 'Time');
	data.addColumn('number', 'GC Memory Allotted (mb)');
	data.addColumn({type:'string', role:'annotation'});
	data.addColumn({type:'string', role:'annotationText'});

	var GC_Usage_JSON = JSON.parse(document.getElementById("memory_usage_gc_hidden").value.replace(/@APOS@/g,"\"").replace(/\n/g,"\\n").replace(/\t/g,"\\t").replace(/\r/g,"\\r"));
    Data_Points = [];
    var min_val = 0;
    var max_val = 0;
    for(var i = 0; i < GC_Usage_JSON.length; i++) {
    	var memory_usage = GC_Usage_JSON[i].memory;
	    if(memory_usage > max_val){
	    	max_val = memory_usage;
	    } else if (memory_usage < min_val || min_val == 0) {
	    	min_val = memory_usage;
	    }
    	Data_Points.push([ new Date(GC_Usage_JSON[i].time), memory_usage, "*", GC_Usage_JSON[i].message]);
    }
    data.addRows(Data_Points);

	var options = {
        width: 1000,
        height: 1000,
        legend: {position: 'none'},
        title: 'Garbage Collection Memory Allotted',
        chartArea: {
            left: 100,
            top: 100,
            width: 1000,
            height: 500
		},
        vAxis: {
            minValue: min_val + 5,
            maxValue: max_val - 5,
        },
        series: {
          1: {curveType: 'function'}
        },
        annotation: {
       	 	1: {
            	style: 'default'
        	}
        }
    };

    var chart = new google.visualization.LineChart(document.getElementById('chart_gc'));
    chart.draw(data, options);

    //Heap Size
    data = new google.visualization.DataTable();
    data.addColumn('date', 'Time');
	data.addColumn('number', 'Run Time Asset Memory Usage (mb)');
	data.addColumn({type:'string', role:'annotation'});
	data.addColumn({type:'string', role:'annotationText'});

	var RT_Usage_JSON = JSON.parse(document.getElementById("memory_usage_rt_hidden").value.replace(/@APOS@/g,"\"").replace(/\n/g,"\\n").replace(/\t/g,"\\t").replace(/\r/g,"\\r"));
    Data_Points = [];
    min_val = 0;
    max_val = 0;
    for(var i = 0; i < RT_Usage_JSON.length; i++) {
    	var memory_usage = RT_Usage_JSON[i].memory;
    	if(memory_usage > max_val){
	    	max_val = memory_usage;
	    } else if (memory_usage < min_val || min_val == 0) {
	    	min_val = memory_usage;
	    }
    	Data_Points.push([ new Date(RT_Usage_JSON[i].time), memory_usage, "*", RT_Usage_JSON[i].message]);
    }
    data.addRows(Data_Points);

	options = {
        width: 1000,
        height: 1000,
        legend: {position: 'none'},
        title: 'Asset Run Time Memory Usage',
        chartArea: {
            left: 100,
            top: 100,
            width: 1000,
            height: 500
		},
        vAxis: {
            minValue: min_val + 5,
            maxValue: max_val - 5
        },
        series: {
          1: {curveType: 'function'}
        },
        annotation: {
       	 	1: {
            	style: 'default'
        	}
        }
    };

    chart = new google.visualization.LineChart(document.getElementById('chart_rt'));
    chart.draw(data, options);

    //Frames Per Second
    data = new google.visualization.DataTable();
    data.addColumn('date', 'Time');
	data.addColumn('number', 'Frames Per Second');
	data.addColumn({type:'string', role:'annotation'});
	data.addColumn({type:'string', role:'annotationText'});

	var FPS_JSON = JSON.parse(document.getElementById("performance_fps_hidden").value.replace(/@APOS@/g,"\"").replace(/\n/g,"\\n").replace(/\t/g,"\\t").replace(/\r/g,"\\r"));
    Data_Points = [];
    min_val = 0;
    max_val = 0;
    for(var i = 0; i < FPS_JSON.length; i++) {
    	var fps = FPS_JSON[i].fps;
    	if(fps > max_val){
	    	max_val = fps;
	    } else if (fps < min_val || min_val == 0) {
	    	min_val = fps;
	    }
    	Data_Points.push([ new Date(FPS_JSON[i].time), fps, "*", FPS_JSON[i].message]);
    }
    data.addRows(Data_Points);

	options = {
        width: 1000,
        height: 1000,
        legend: {position: 'none'},
        title: 'Frames Per Second',
        chartArea: {
            left: 100,
            top: 100,
            width: 1000,
            height: 500
		},
        vAxis: {
            minValue: min_val + 5,
            maxValue: max_val - 5
        },
        series: {
          1: {curveType: 'function'}
        },
        annotation: {
       	 	1: {
            	style: 'default'
        	}
        }
    };

    chart = new google.visualization.LineChart(document.getElementById('chart_fps'));
    chart.draw(data, options);

}

function ShowPanel(panel_name) {

	var panel;

	switch(panel_name) {
		case "Gallery":
			panel = $("#gallery_panel");
			break;
		case "Communications":
			panel = $("#communications_panel");
			break;
		case "Warnings":
			panel = $("#warnings_panel");
			break;
	}
	panel.show(400);
	$(".background_transparency").show(400);
	last_opened_display_panel = panel;

}

function ShowDetails(button) {

	var result_message_details_actual = button.next().text();
	var result_message_assertions = button.next().next();
	var result_message_actual = result_message_assertions.children().last().find(".assertion_item").text();
	var result_message = $("#result_message");
	var result_message_details = $("#result_message_details");

	opening = true;
	var show_results = $("#show_results");

	if(button.next().text() == $("#details_message_details").text()) {
		if( show_results.is(":visible")){
			//If details are already displayed, clicking same button again will close details instead.
			HideDetails();
			return;
		}
	}

	$(".details_title").hide();

	//Set all button background colors to default.
	$(".error_button").css("background-color", "#ff8080"); 
	$(".passed_button").css("background-color", "#74c97e");
	$(".ignored_button").css("background-color", "#007acc");
	$(".skipped_button").css("background-color", "#FFBE5D");

	var td_elements = button.parent().parent().find("td");
	var title = "<div><span class='result_details_span'>Test:  </span><span>" + $(td_elements[0]).text() + "</span></div><div><span class='result_details_span'>Class:  </span><span>" + 
		$(td_elements[1]).text() + "</span></div><div><span class='result_details_span'>Categories:  </span><span>" + $(td_elements[2]).children().last().text() + 
		"</span></div><div><span class='result_details_span'>Status:  </span>" + $(td_elements[3]).html() + "</div>";
	title += "</div>";

	if(button.hasClass("error_button")) {

		$(".details_title").show();
		result_message_details.text(result_message_details_actual);
		result_message.text(result_message_actual);

		button.css("background-color", "#f7b2b6"); //Change selected buttons color until window is closed.

		var error_td_elements = button.parent().parent().find("td");
		var showScreenShot = 'ShowScreenshot("' + $(error_td_elements[0]).text() + '", true);'
		if($(error_td_elements[3]).text() != "Skipped"){
			title += "<span class='result_details_span'>ScreenShot:  </span><div class='screenshot_open' onClick='" + showScreenShot +"'>Show</div>";
		}
		title += "</div>";
		$("#result_details").html(title);
		$("#assertion_title").show();
		$("#assertions_list").html(result_message_assertions.html());
		$("#reason_title").hide();
		$("#reason_message_details").text("");

	} else if(button.hasClass("ignored_button")) {

		$("#reason_title").show();
		$("#result_details").html(title);
		button.css("background-color", "#8adefc");
		$("#reason_title").show();
		$("#reason_message_details").text(result_message_details_actual).show();
		result_message.text("");
		result_message_details.text("");
		$("#assertions_list").html("");

	} else if (button.hasClass("skipped_button")) {

		$("#reason_title").show();
		$("#result_details").html(title);
		button.css("background-color", "#f7d8aa");
		$("#reason_title").show();
		$("#reason_message_details").text(result_message_details_actual).show();
		result_message.text("");
		result_message_details.text("");
		$("#assertions_list").html("");

	} else {

		$("#result_details").html(title);
		button.css("background-color", "#99db9c"); 
		$("#assertion_title").show();
		result_message.text("");
		result_message_details.text(result_message_details_actual);
		$("#result_details").html(title);
		$("#assertions_list").html(result_message_assertions.html());
		$("#reason_title").hide();
		$("#reason_message_details").text("");

	}

	show_results.css("display", "inline");
	show_results.removeClass("slider_reverse");
	show_results.addClass("slider");
    last_selected_button = button;
    setTimeout(function(){ opening = false; }, 500);

}

function HideDetails() {

	var error_button = $(".error_button");
	error_button.css("background-color", "#ff8080"); //Return color of selected button by resetting color to all of this class.
	var show_results = $("#show_results");
	show_results.removeClass("slider");
	show_results.addClass("slider_reverse");
	setTimeout(function(){ show_results.hide(); }, 1000);

}

function ToggleChartView() {

	var memory_graphs = $("#memory_graphs");
	var test_results_table_panel = $("#test_results_table_panel");
	var switch_image_div = $(".switch_image_div");
	if(memory_graphs.is(":visible")){
		memory_graphs.hide(200);
		setTimeout(function(){
			switch_image_div.addClass("chart_image"); 
			switch_image_div.addClass("chart_image_div"); 
			switch_image_div.removeClass("test_results_image");
			switch_image_div.removeClass("test_results_image_div");
			test_results_table_panel.show(200);
		}, 200);
	} else {
		test_results_table_panel.hide(200);
		$("#chart_gc_div").show();
		$("#chart_rt_div").hide();
		$("#chart_fps_div").hide();
		setTimeout(function(){
		switch_image_div.addClass("test_results_image");
			switch_image_div.addClass("test_results_image_div");
			switch_image_div.removeClass("chart_image");
			switch_image_div.removeClass("chart_image_div");
			memory_graphs.show(200);
		}, 200);
	}

}

function ToggleChartDisplay(toggle) {

	//Do nothing if the currently-selected tab is already displaying its associated chart.
	if(!toggle.hasClass("status_success_show")) {

		var allToggles = $(".tab_toggle");
		allToggles.removeClass("status_success_show").addClass("status_success_hide");
		toggle.removeClass("status_success_hide").addClass("status_success_show");
		allToggles.css("cursor", "pointer");
		toggle.css("cursor", "auto");

		var chart_gc_div = $("#chart_gc_div");
		var chart_rt_div = $("#chart_rt_div");
		var chart_fps_div = $("#chart_fps_div");
		chart_rt_div.hide(300);
		chart_gc_div.hide(300);
		chart_fps_div.hide(300);
		var chart_name = toggle.find(".tab_toggle_name").text();
		setTimeout(function() {
			if(chart_name.indexOf("Garbage") >= 0) {
				chart_gc_div.show(300);
			} else if(chart_name.indexOf("Asset") >= 0) {
				chart_rt_div.show(300);
			} else if(chart_name.indexOf("FPS") >= 0) {
				chart_fps_div.show(300);
			}
		}, 300);

	}

}

var passesVisible = true;
var failsVisible = true;
var skipsVisible = true;
var ignoresVisible = true;
var filteredRows = [];

function ToggleVisibility(status) {

    var toggle = $(this)
	switch(status.toLowerCase()) {
		case "success":
			passesVisible = !passesVisible;
			if(toggle.hasClass("status_success_show")){
				toggle.addClass("status_success_hide");
			    toggle.removeClass("status_success_show");
			} else {
				toggle.addClass("status_success_show");
				toggle.removeClass("status_success_hide");
			}
			break;
		case "failure":
			failsVisible = !failsVisible;
			if(toggle.hasClass("status_failure_show")){
				toggle.addClass("status_failure_hide");
			    toggle.removeClass("status_failure_show");
			} else {
				toggle.addClass("status_failure_show");
				toggle.removeClass("status_failure_hide");
			}
			break;
		case "skipped":
			skipsVisible = !skipsVisible;
			if(toggle.hasClass("status_skipped_show")){
				toggle.addClass("status_skipped_hide");
			    toggle.removeClass("status_skipped_show");
			} else {
				toggle.addClass("status_skipped_show");
				toggle.removeClass("status_skipped_hide");				
			}
			break;
		case "ignored":
			ignoresVisible = !ignoresVisible;
			if(toggle.hasClass("status_ignored_show")){
				toggle.addClass("status_ignored_hide");
			    toggle.removeClass("status_ignored_show");
			} else {
				toggle.addClass("status_ignored_show");
				toggle.removeClass("status_ignored_hide");
			}
			break;
	}

	currentJsonToRender = [];
	for(var x = 0; x < testResultsJson.length; x++) {
		var statusLower = testResultsJson[x].status.toLowerCase();
		var rowObj = [];
		if(statusLower == "passed" && passesVisible) {
			rowObj = testResultsJson.filter(function(item) { return item.name === testResultsJson[x].name; });
			currentJsonToRender.push(rowObj[0]);
		}
		if(statusLower == "failed" && failsVisible) {
			rowObj = testResultsJson.filter(function(item) { return item.name === testResultsJson[x].name; });
			currentJsonToRender.push(rowObj[0]);
		}
		if(statusLower == "skipped" && skipsVisible) {
			rowObj = testResultsJson.filter(function(item) { return item.name === testResultsJson[x].name });
			currentJsonToRender.push(rowObj[0]);
		}
		if(statusLower == "ignored" && ignoresVisible) {
			rowObj = testResultsJson.filter(function(item) { return item.name === testResultsJson[x].name; });
			currentJsonToRender.push(rowObj[0]);
		}
	}

	BuildTable();

}

function ToggleFooterPanel(arrow){

   	var bottom_panel = $("#bottom_panel");
	if(arrow.hasClass("corner_tab_open")){
		bottom_panel.show(500);
		arrow.removeClass("corner_tab_open");
		arrow.addClass("corner_tab_closed");
	} else {
		bottom_panel.hide(500);
		arrow.removeClass("corner_tab_closed");
		arrow.addClass("corner_tab_open");
	}

}

var openingScreenshotPanel = false;
function ShowScreenshot(errorName, literal, isGallery) {
	
	var errorNameParsed = errorName;
	if(isGallery) {
		errorNameParsed = errorName.split("/")[1];
	}
	last_opened_display_panel = $("#screenshot_panel");
  	var screenshot_panel = last_opened_display_panel.find(".screenshot_image");
  	var no_image = $("#fail_test_no_image_" + errorNameParsed);
  	var path_start = is_screenshot_report ? "./" : "./screenshots/";
  	if(no_image.length == 0){
  		if(literal == true) {
  			screenshot_panel.html("<div class='fail_test_image_" + errorNameParsed + "'><img src='" + path_start + errorName + ".png' /></div>");
  		} else {
  			$.ajax({
			  type: 'HEAD',
			  url: path_start + 'fatal_error.png',
			  complete: function (xhr) {
			    if (xhr.status != 404) {
  					screenshot_panel.html("<div class='fail_test_image_" + errorNameParsed + "'><img src='"  + path_start + "fatal_error.png' /></div>");
			    } else {
			    	$.ajax({
					  type: 'HEAD',
					  url: path_start + 'launch_failure.png',
					  complete: function (xhr) {
					    if (xhr.status != 404) {
					    	screenshot_panel.html("<div class='fail_test_image_" + errorNameParsed + "'><img src='" + path_start + "launch_failure.png' /></div>");
					    } else {
  		  					screenshot_panel.html("<div class='fail_test_image_" + errorNameParsed + "'><img src='" + path_start + errorName + ".png' /></div>");
					    }
					  }
					});
			    }
			  }
			});
  		}
  	} else {
  		screenshot_panel.html("<div style='position: relative; top: 48%; text-align: center; font-weight: bold; color: red;'>Unity was unable to save a screenshot at the time of this test's failure!</div>");
  	}
  	$(".screenshot_open").css("background-color", "#94d8fc");
	$(".background_transparency").show(400);
  	openingScreenshotPanel = true;
   	last_opened_display_panel.show(400, function() {
   		openingScreenshotPanel = false;
   	});

}

function BuildCommunicationHistory() {

	var message_exchange_list = $("#communications_panel").find(".message_exchange_list")
	var history = $("#communications_history").val();
	if(history.length == 0) {
		return;
	}
	var messages = history.split("$$");
	for(var x = 0; x < messages.length; x++) {

		var message = messages[x].split("##")[1];
		var className = messages[x].split("##")[0].toLowerCase() == "client" ? "message_right" : "message_left"
		if(typeof message != 'undefined' && message.length > 0) {
			message_exchange_list.append("<div class='" + className + "'>" + message.replace(/@APOS@/g, "\"") + "</div>");
		}

	}
}

function ToggleSummaryCollapsable(toggler) {

	if(toggler.hasClass("automation_summary_collapse")) {
		var automation_summary = $(".automation_summary");
		var automation_summary_collapse = $(".automation_summary_collapse");
		if(automation_summary.is(":visible")){
			automation_summary_collapse.html(arrowRight + " Details");
			automation_summary.slideUp("400");
		} else {
			automation_summary_collapse.html(arrowDown + " Details");
			automation_summary.slideDown("400");
		}
	} else if(toggler.hasClass("screenshots_all_list_collapse")) {
		var screenshots_all_list = $(".screenshots_all_list");
		var screenshots_all_list_collapse = $(".screenshots_all_list_collapse");
		if(screenshots_all_list.is(":visible")){
			screenshots_all_list_collapse.html(arrowRight + " All Screenshots");
			screenshots_all_list.slideUp("400");
		} else {
			screenshots_all_list_collapse.html(arrowDown + " All Screenshots");
			screenshots_all_list.slideDown("400");
		}
	}

}

function BuildScreenshotGallery() {

	var gallery_panel = $("#gallery_panel");
	var fps_screenshot_hidden = $(".fps_screenshot");

	//If this is a multi-device combined report.
	if(is_screenshot_report) {

		var path_start = is_screenshot_report ? "/" : "/screenshots/";
		var galleries_all = $(".gallery_hidden");
		var gallery_panel_screenshots = $("#gallery_panel_screenshots");
		var index_max = 0;
		var max_screenshots = 0;

		var build_numbers_devices = $("#build_numbers_hidden").val().split("||")

		var device_count = galleries_all.length;
		for(var g = 0; g < galleries_all.length; g++) {

			var gallery = $(galleries_all[g]);
			var device_name_current = gallery.prop("id").replace("_ScreenshotGalleryReport", "")
			var build_number_device = 0;

			//Get build number for device run.
			for(var b = 0; b < build_numbers_devices.length; b++) {
				var vals_split = build_numbers_devices[b].split("#");
				if(vals_split[0] == device_name_current) {
					build_number_device = vals_split[1];
				}
			}

			$("#full_device_report_header").after("<div style='margin: 5px; margin-left: 25px;'><a target='_blank;' href='" + DOMAIN_URL + "/job/" + GAME_NAME + "_" + GAME_ENV + "_Auto_Panels_" + gallery.prop("id").replace("_ScreenshotGalleryReport", "")  + "/ws/TestResultsArchive/"+ build_number_device + "/HtmlReport/report.html'>" + gallery.prop("id").replace("_ScreenshotGalleryReport", "").replace("_", " ") + "</a></div>");
			var image_count = gallery.val().split("|").length;
			if(image_count > max_screenshots) {
				index_max = g;
			}

		}

		var baseline_images = $(galleries_all[index_max]).val().split("|");
		for(var m = 0; m < baseline_images.length; m++) {

			if(baseline_images[m].length == 0) {
				continue;
			}

			var title_pieces = baseline_images[m].split(SCREENSHOT_STAGE_DELIMITER);
			var title = "";
			if(title_pieces.length == 2) {
				title = title_pieces[0].replace(/_/g, " ") + " - " + title_pieces[1].replace(/_/g, " ").replace(/.png/g,"");
			} else {
				continue; //Ignore this screenshot, as it lacks the screenshot report delimiter.
			}
			var html = "<div id='gallery_row' class='gallery_row'><div class='gallery_title'>" + title + "</div>";
			for(var i = 0; i < galleries_all.length; i++) {

				var fps_html = "";
				var gallery = $(galleries_all[i]);
				if(gallery.val().indexOf(baseline_images[m] + "|") >= 0) {

					var name = baseline_images[m].replace(/.png/g,"");
					var device_name = gallery.prop("id").replace("_ScreenshotGalleryReport", "")
					for(var f = 0; f < fps_screenshot_hidden.length; f++) {

						var fps_data = $(fps_screenshot_hidden[f]).val();
						if(fps_data.length == 0) {
							continue;
						}
						if(fps_data.indexOf(device_name) >= 0) {

							if(fps_data.indexOf(name) >= 0) {

								var rawFpsSplit = fps_data.split(name)[1].replace("##", "");
								fps_html = "<span id='fps_image' class='fps_display'>FPS " + parseInt(rawFpsSplit.split("##")[0]) + "</span>";

							}

						}
						
					}
					
					html += "<div class='gallery_image_div'>" + fps_html + "<div class='gallery_image_title'>" + gallery.prop("id").replace("_ScreenshotGalleryReport", "").replace(/_/g, " ") + "</div><img onClick='ShowScreenshot(\"" + gallery.prop("id") + "/" + name + "\", true, true)' class='gallery_image' src='./" + gallery.prop("id") + path_start + baseline_images[m] + "' /></div>";

				}

			}
			html += "</div>";
			gallery_panel_screenshots.append(html);
		}

	} else {
		
		var path_start = is_screenshot_report ? "./" : "./screenshots/";
		var screenshots_available = $("#screenshot_file_names").val().split("|");
		//Check each test for screenshots, and render them based on the tests that took the screenshot. Any left over will be put in their own "other" category.
		for(var x = 0; x < all_tests.length; x++) {

			var any = false;
			var html = "<div id='gallery_row' class='gallery_row'><div class='gallery_title'>" + all_tests[x] + "</div>";
			for(var i = 0; i < screenshots_available.length; i++) {

				var fps_html = "";
				var name = screenshots_available[i].replace(/.png/g,"");
				for(var f = 0; f < fps_screenshot_hidden.length; f++) {

					var fps_data = $(fps_screenshot_hidden[f]).val();
					if(fps_data.length == 0) {

						continue;
						
					}
					if(fps_data.indexOf(name) >= 0) {

						var rawFpsSplit = fps_data.split(name)[1].replace("##", "");
						fps_html = "<span id='fps_image' class='fps_display'>FPS " + parseInt(rawFpsSplit.split("##")[0]) + "</span>";

					}
						
				}
				//If a screenshot name contains this test name.
				if(screenshots_available[i].indexOf(all_tests[x]) >= 0) {
					any = true;
					var name = screenshots_available[i].replace(/.png/g,"");
					html += "<div class='gallery_image_div'>" + fps_html + "<div class='gallery_image_title'>" + name.replace(all_tests[x], "").replace(SCREENSHOT_STAGE_DELIMITER, "").replace(/_/g, " ") + "</div><img onClick='ShowScreenshot(\"" + name + "\", true)' class='gallery_image' src='" + path_start + screenshots_available[i] + "' /></div>";
					screenshots_available.splice(i,1); //Remove this screenshot from the list since it has been handled.
					i--;				
				}
			}
			html += "</div>";
			
			if(any) {
				gallery_panel.append(html);
			}

		}

		//If any orphan screenshots exist, show them in a final, "Other" category.
		if(screenshots_available.length > 0 && screenshots_available[0].length != 0) {

			var html = "<div id='gallery_row' class='gallery_row'><div class='gallery_title'>Other</div>";
			for(var i = 0; i < screenshots_available.length; i++) {
				var name = screenshots_available[i].replace(/.png/g,"");
				if(name.length == 0) {
					continue;
				}
				html += "<div class='gallery_image_div'><div class='gallery_image_title'>" + name.replace(all_tests[x], "").replace(SCREENSHOT_STAGE_DELIMITER, "").replace(/_/g, " ") + "</div><img onClick='ShowScreenshot(\"" + name + "\", true)' class='gallery_image' src='" + path_start + screenshots_available[i] + "' /></div>";
				screenshots_available.splice(i,1); //Remove this screenshot from the list since it has been handled.
				i--;
			}
			html += "</div>";
			gallery_panel.append(html);

		}

	}

}

function CloseTransparencyLayer() {

	setTimeout(function() {

		var any_open = 0;
		var panels = $(".display_panel");
		for(var x = 0; x < panels.length; x++) {

			if($(panels[x]).is(":visible")) {
				any_open++;
			}

		}

		if(any_open <= 1) {
			$(".background_transparency").hide(300);
		}

	}, 100);

}

function NotImplemented(message) {

	var notice_popup = $(".notice-popup");
	notice_popup.find(".message").text(message);
	$(".background_transparency").show(400);
	notice_popup.show(400);
	opening = true;
	setTimeout(function(){ opening = false; }, 500);

}

$(document).click(function(event) { 

	if(!is_screenshot_report) {

	  if(!$("#screenshot_panel").is(":visible")) {
		  if(!$(event.target).closest('#show_results').length) {
		        if($('#show_results').is(":visible")) {
		        	if(!opening){
		            	HideDetails();
		            }
		        }
		    }  
		}
	 
	    if(!$(event.target).closest('#screenshot_panel').length) {
		   if($('#screenshot_panel').is(":visible")) {
		      if(!openingScreenshotPanel){
		      	$(".screenshot_open").css("background-color", "#0290dd");
		          $("#screenshot_panel").hide(400);
		      }
		   }
	    } 

	    var any_open = 0;
		var panels = $(".display_panel");
		for(var x = 0; x < panels.length; x++) {

			if($(panels[x]).is(":visible")) {
				any_open++;
			}

		}

		if(any_open <= 0) {
			$(".background_transparency").hide(400);
		}

	} else {

	    if(!$(event.target).closest('#screenshot_panel').length) {
		   if($('#screenshot_panel').is(":visible")) {
		      if(!openingScreenshotPanel){
		      	$(".screenshot_open").css("background-color", "#0290dd");
		          $("#screenshot_panel").hide(400);
		          $(".background_transparency").hide(400);
		      }
		   }
	    } 

	}
		     
});

//Notification arrows have a limited life span.
$(document).on("DOMNodeInserted", ".notice_me_senpai", function() {
	$("#warnings_button").addClass("warning_pulse_timed");
	setTimeout(function() {
		$(".notice_me_senpai").fadeOut(4000);
		setTimeout(function() {
			$("#warnings_button").removeClass("warning_pulse_timed");
			$(".notice_me_senpai").remove();
		}, 4000);
	}, 4000);
});
