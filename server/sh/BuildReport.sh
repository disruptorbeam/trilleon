echo "Copying device results json to workspace directory."
if [ -f "${DEFAULT_DIRECTORY}/finalJson.txt" ]; then
    rm  ${DEFAULT_DIRECTORY}/finalJson.txt
fi
cat ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/testresultsjson.txt | sed -e :a -e '/^\n*$/{$d;N;ba' -e '}' | sed -e '$s/,$/]/'  > ${DEFAULT_DIRECTORY}/finalJson.txt
JSON=$(cat ${DEFAULT_DIRECTORY}/finalJson.txt)
DEVICE_DETAILS_HTML=$(cat ${DEFAULT_DIRECTORY}/TestRunHeaderHtml.txt)

# Set Heap data for html report.
HP=$(cat HeapSize.txt)
HEAP_DATA=$(echo $HP | tr ',' "\n")
INDEX=0
while read -r val; do
    if [ $INDEX = 0 ]; then
        HEAP_SIZE_MIN=$val
    fi
    if [ $INDEX = 1 ]; then
        HEAP_SIZE_AVG=$val
    fi
    if [ $INDEX = 2 ]; then
        HEAP_SIZE_MAX=$val
    fi
    INDEX=$(($INDEX + 1))
done <<< "$HEAP_DATA"

# Set Garbage Collection data for html report.
GC=$(cat GarbageCollection.txt)
GARBAGE_COLLECTION_DATA=$(echo $GC | tr ',' "\n")
INDEX=0
while read -r val; do
    if [ $INDEX = 0 ]; then
        GC_SIZE_MIN=$val
    fi
    if [ $INDEX = 1 ]; then
        GC_SIZE_AVG=$val
    fi
    if [ $INDEX = 2 ]; then
        GC_SIZE_MAX=$val
    fi
    INDEX=$(($INDEX + 1))
done <<< "$GARBAGE_COLLECTION_DATA"

# Set FPS data for html report.
FPS=$(cat Fps.txt)
FPS_DATA=$(echo $FPS | tr ',' "\n")
INDEX=0
while read -r val; do
    if [ $INDEX = 0 ]; then
        FPS_MIN=$val
    fi
    if [ $INDEX = 1 ]; then
        FPS_AVG=$val
    fi
    if [ $INDEX = 2 ]; then
        FPS_MAX=$val
    fi
    INDEX=$(($INDEX + 1))
done <<< "$FPS_DATA"

PUBSUB_HISTORY=$(cat ${DEFAULT_DIRECTORY}/FormattedCommunicationHistory.txt)
HEAP_JSON=$(cat ${DEFAULT_DIRECTORY}/HeapJson.txt)
GC_JSON=$(cat ${DEFAULT_DIRECTORY}/GarbageCollectionJson.txt)
FPS_JSON=$(cat ${DEFAULT_DIRECTORY}/FpsJson.txt)
EXCEPTIONS_JSON=$(cat ${DEFAULT_DIRECTORY}/ExceptionsJson.txt)
REPORT_CSS=$(cat ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportCss.css)
GAME_LOAD_TIME=$(cat ${DEFAULT_DIRECTORY}/GameInitializationTime.txt)
FPS_SCREENSHOT_DATA=$(cat ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_Screenshots_FPS_data.txt)
TEST_RUN_ID=$(cat ${DEFAULT_DIRECTORY}/test_run_id.txt)

# Set Jenkins graph data for test run.
echo "YVALUE=${GAME_LOAD_TIME}" >> ${DEFAULT_DIRECTORY}/game_load.properties
echo "YVALUE=${GC_SIZE_AVG}" >> ${DEFAULT_DIRECTORY}/gc_usage.properties
echo "YVALUE=${HEAP_SIZE_AVG}" >> ${DEFAULT_DIRECTORY}/heap_usage.properties
echo "YVALUE=${FPS_AVG}" >> ${DEFAULT_DIRECTORY}/fps.properties

echo "CD into screenshots directory."
cd screenshots
SCREENSHOT_NAMES=""
for i in `ls`; do
    echo "Adding screenshot name to list [$i]."
    SCREENSHOT_NAMES+=$i
    SCREENSHOT_NAMES+="|";
done;
echo "CD out of screenshots directory."
cd ..

HTML="
       <style>${REPORT_CSS}</style>
       <link rel='stylesheet' href='./scripts/bootstrap.min.css'>
       <script type='text/javascript' src='./scripts/jquery.min.js'></script>
       <script type='text/javascript' src='./scripts/jquery.dataTables.min.js'></script>
       <script type='text/javascript' src='./scripts/bootstrap.min.js'></script>
       <script type='text/javascript' src='./scripts/loader.js'></script>
       <script type='text/javascript' src='./scripts/TrilleonDefaultAutomationReportDatatable.js'></script>
       <body>
       <div class='container'>
       <h2>Trilleon: Automation Test Run Report</h2>
       ${DEVICE_DETAILS_HTML}
       <div class='switch_to_from_chart' type='button' onClick='ToggleChartView();'><div id='chart_image' class='switch_image_div chart_image_div chart_image'></div></div>
       <div id='memory_graphs'>        
           <div class='tab_toggle button tab_toggle_left_end status_success_show' type='button' onclick='ToggleChartDisplay(\$(this));'><div class='tab_toggle_name'>Garbage Collection</div></div>        
           <div class='tab_toggle button tab_toggle_inner status_success_hide' type='button' onclick='ToggleChartDisplay(\$(this));'><div class='tab_toggle_name'>Asset Run Time</div></div>
           <div class='tab_toggle button tab_toggle_right_end status_success_hide' type='button' onclick='ToggleChartDisplay(\$(this));'><div class='tab_toggle_name'>FPS</div></div>
           <div id='chart_gc_div'>
	           <span class='chart_result'><strong>Min GC Memory: </strong>${GC_SIZE_MIN} mb</span>
	           <span class='chart_result'><strong>Avg GC Memory: </strong>${GC_SIZE_AVG} mb</span>
	           <span class='chart_result'><strong>Max GC Memory: </strong>${GC_SIZE_MAX} mb</span>
	           <div id='chart_gc' class='memory_gc_chart'></div>
           </div>
           <div id='chart_rt_div'>
	           <span class='chart_result'><strong>Min RT Memory: </strong>${HEAP_SIZE_MIN} mb</span>
	           <span class='chart_result'><strong>Avg RT Memory: </strong>${HEAP_SIZE_AVG} mb</span>
	           <span class='chart_result'><strong>Max RT Memory: </strong>${HEAP_SIZE_MAX} mb</span>
	           <div id='chart_rt' class='memory_rt_chart'></div>
           </div>
           <div id='chart_fps_div'>
               <span class='chart_result'><strong>Min FPS: </strong>${FPS_MIN}</span>
               <span class='chart_result'><strong>Avg FPS: </strong>${FPS_AVG}</span>
               <span class='chart_result'><strong>Max FPS: </strong>${FPS_MAX}</span>
               <div id='chart_fps' class='fps_chart'></div>
           </div>
        </div>
        <input class='fps_screenshot' type='hidden' value='${FPS_SCREENSHOT_DATA}'/>
        <input id='test_run_type' type='hidden' value='full'/>
        <input id='domain_url_hidden' type='hidden' value='${DOMAIN_URL}'/>
        <input id='screenshot_file_names' type='hidden' value='${SCREENSHOT_NAMES}'/>
        <input id='exceptions_hidden' type='hidden' value='${EXCEPTIONS_JSON}'/>
        <input id='memory_usage_rt_hidden' type='hidden' value='${HEAP_JSON}'/>
        <input id='memory_usage_gc_hidden' type='hidden' value='${GC_JSON}'/>
        <input id='performance_fps_hidden' type='hidden' value='${FPS_JSON}'/>
        <input id='build_number_hidden' type='hidden' value='${BUILD_NUMBER}'/>
        <input id='build_name_hidden' type='hidden' value='${JOB_NAME}'/>
        <input id='test_run_hidden' type='hidden' value='${TEST_RUN_ID}'/>
        <input id='results_hidden' type='hidden' value='${JSON}'/>
        <div id='test_results_table_panel'>
            <div class='critical_error_detected status_critical_error_show'>Fatal Exception Encountered; Test Run Aborted\!</div>
            <div style='display:inline-block; margin-right:10px;'><strong>Info Panels:</strong></div>
            <div id='gallery_button' class='button screenshots_button' type='button' onClick='ShowPanel(\"Gallery\");'>Screenshots</div>
            <div id='communications_button' class='button_toggle button communications_button' type='button' onClick='ShowPanel(\"Communications\");'>Communications</div>
            <div id='warnings_button' class='button_toggle button warnings_button' type='button' onClick='ShowPanel(\"Warnings\");'>Warnings</div>
            <div id='exceptions_button' class='button_toggle button warnings_button' type='button' onClick='ShowPanel(\"Exceptions\");'>Exceptions</div>
            <br/>
            <div style='display:inline-block; margin-right:10px;'><strong>Show/Hide:</strong></div>
            <div class='button_toggle button status_success_show' type='button' onClick='ToggleVisibility(\"success\");'>Success</div>
            <div class='button_toggle button status_failure_show' type='button' onClick='ToggleVisibility(\"failure\");'>Failed</div>
            <div class='button_toggle button status_skipped_show' type='button' onClick='ToggleVisibility(\"skipped\");'>Skipped</div>
            <div class='button_toggle button status_ignored_show' type='button' onClick='ToggleVisibility(\"ignored\");'>Ignored</div>
            <table id='test_results' class='table'>
            <thead><tr class='table_header'><td>Test Name</td><td>Test Class</td><td>Test Categories</td><td>Status</td><td>Details</td><td>Order Ran</td></tr></thead>
            <tbody></tbody>
            <div id='show_results' class='details_panel'>
                    <div class='wrapper'>
                       <div class='close' onClick='HideDetails();'>X</div>
                       <div class='result_details_title'>Test Details</div>
                       <div id='result_details' class='result_details'>DETAILS</div>
                       <div class='details_title'>Error Message:</div>
                       <div id='result_message'class='error_message_details'>PLACEHOLDER</div>
                       <div class='details_title'>Error Details:</div>
                       <div id='result_message_details'class='error_message_details'>PLACEHOLDER</div>
                       <div id='assertion_title' class='details_title'>Assertions:</div>
                       <div id='assertions_list'class='error_message_details'>PLACEHOLDER</div>
                       <div id='reason_title' class='details_title'>Reason:</div>
                       <div id='reason_message_details'class='error_message_details'>PLACEHOLDER</div>
                    </div>
                </div>
            </div>
        </div>
        <div id='corner_tab' class='corner_tab'><div id='corner_tab_arrow' onClick='ToggleFooterPanel(\$(this));' class='corner_tab_arrow corner_tab_open'>&#10095;</div></div>
        <div id='bottom_panel' class='bottom_panel'>
        <div class='additional_tools_div'><a class='additional_tools' target='_blank' href='https://${COMPANY_JENKINS_BASE_URL}/job/${JOB_NAME}/${BUILD_NUMBER}' style='margin-left: 200px;'>Jenkins Build ${BUILD_NUMBER}</a></div>
        <div class='link_separator'>&#9679;</div>
        <div class='additional_tools_div'><a class='additional_tools' target='_blank' href='trilleonautomation.wiki/reports'>About This Report</a></div>
        <div class='link_separator'>&#9679;</div>
        <div class='additional_tools_div'><a class='additional_tools' target='_blank' href='trilleonautomation.wiki/reports'>About Trilleon</a></div>
        </div>
        <div id='screenshot_panel' class='display_panel'>
            <div class='close' onclick='\$(this).parent().hide(400); CloseTransparencyLayer();'>X</div>
            <div class='screenshot_image'></div>
        </div>
        <input id='communications_history' type='hidden' value='${PUBSUB_HISTORY}' />
        <div id='communications_panel' class='display_panel'>
            <div class='close' onclick='\$(this).parent().hide(400); CloseTransparencyLayer();'>X</div>
            <h2 style='margin-top: 10px;'>Communications</h2>
            <div style='width:10px;height:10px;background-color: #007AA2;display: inline-block;'></div> Server | <div style='width:10px;height:10px;background-color: #009056;display: inline-block;'></div> Client
            <div class='message_exchange_list'></div>
        </div>
        <div id='warnings_panel' class='display_panel'>
            <div class='close' onclick='\$(this).parent().hide(400); CloseTransparencyLayer();'>X</div>
            <h2 style='margin-top: 10px;'>Warnings</h2>
            <div class='warnings_list'></div>
        </div>
        <div id='gallery_panel' class='display_panel'>
            <div class='close' onclick='\$(this).parent().hide(400); CloseTransparencyLayer();'>X</div>
            <h2 style='margin: 10px; margin-bottom: 40px;'>Screenshot Gallery</h2>
        </div>
        <div id='exceptions_panel' class='display_panel'>
            <div class='close' onclick='\$(this).parent().hide(400); CloseTransparencyLayer();'>X</div>
            <h2 style='margin: 10px; margin-bottom: 40px;'>Non-Trilleon Exceptions</h2>
        </div>
        <div class='background_transparency'></div>
        </body>"

echo "Copying appium log to archive and latest."
cp ${DEFAULT_DIRECTORY}/appium.log ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}

echo "Copying py log to archive and latest."
cp ${DEFAULT_DIRECTORY}/PyLog.txt ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}

echo "Copying ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/js/. to ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/scripts"
cp -rv ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/js/. ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/scripts
echo "Copying ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/css/. to ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/scripts"
cp -rv ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/css/. ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/scripts
echo "cp file ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportDatatable.js ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/scripts/TrilleonDefaultAutomationReportDatatable.js"
cp -rv ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportDatatable.js ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/scripts/TrilleonDefaultAutomationReportDatatable.js

echo "HTML sent to ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/report.html"
echo ${HTML} > ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/report.html

echo "Copying appium log to archive and latest."
cp ${DEFAULT_DIRECTORY}/appium.log ${DEFAULT_DIRECTORY}/LatestTestResults

echo "Copying from archive folder to latest test results folder."
cp -rv ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/. ${DEFAULT_DIRECTORY}/LatestTestResults

if [ -f "${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}.zip" ]; then
    echo "Removing top level zip folders from previous run."
    rm  ${DEFAULT_DIRECTORY}/{APPIUM_DEVICE}.zip
fi

echo "Zipping latest test results folder."
cd ${DEFAULT_DIRECTORY}/LatestTestResults
zip -FSr ${APPIUM_DEVICE}.zip *
cd ..

echo "Zipping latest screenshot gallery report folder."
if [ -d ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport/screenshots ]; then
    zip -FSrj ${APPIUM_DEVICE}_ScreenshotGalleryReport.zip ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport/screenshots
else
    zip -FSrj ${APPIUM_DEVICE}_ScreenshotGalleryReport.zip ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport
fi

echo "Moving zip to workspace level."
mv ${DEFAULT_DIRECTORY}/LatestTestResults/${APPIUM_DEVICE}.zip  ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}.zip

echo "Moving zip to current build's archive level."
mv ${DEFAULT_DIRECTORY}/LatestTestResults/${APPIUM_DEVICE}.zip  ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/${APPIUM_DEVICE}.zip
