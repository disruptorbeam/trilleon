echo "CD into ${DEFAULT_DIRECTORY}/report"
cd report

REPORT_CSS=$(cat ./css/TrilleonDefaultAutomationReportCss.css)
FPS_SCREENSHOT_DATA=$(cat ./Screenshots_FPS_data.txt)

HIDDEN_SCREENSHOT_HTML=""
for d in $(echo ${FOLDERS_ALL} | tr "|" "\n")
do
    if [ -d "$d" ]; then
        echo "CD into $d directory."
        cd $d
        SCREENSHOT_NAMES=""
        for i in `ls`; do
            if [[ $i == *".png"* ]]; then
                echo "Adding screenshot name to list [$i]."
                SCREENSHOT_NAMES+=$i
                SCREENSHOT_NAMES+="|";
            fi
        done;
        HIDDEN_SCREENSHOT_HTML+="<input id='$d' class='gallery_hidden' type='hidden' value='${SCREENSHOT_NAMES}'>"
        echo "CD out of $d directory."
        cd ..
    fi
done

HTML="
       <style>${REPORT_CSS}</style>
       <link rel='stylesheet' href='./${BASE_SCRIPTS_PATH}/css/bootstrap.min.css'>
       <script type='text/javascript' src='./${BASE_SCRIPTS_PATH}/js/jquery.min.js'></script>
       <script type='text/javascript' src='./${BASE_SCRIPTS_PATH}/js/jquery.dataTables.min.js'></script>
       <script type='text/javascript' src='./${BASE_SCRIPTS_PATH}/js/bootstrap.min.js'></script>
       <script type='text/javascript' src='./${BASE_SCRIPTS_PATH}/js/loader.js'></script>
       <script type='text/javascript' src='./${BASE_SCRIPTS_PATH}/js/TrilleonDefaultAutomationReportDatatable.js'></script>
       <body>
            ${HTML_FPS}
            <input id='game_env_hidden' type='hidden' value='${GAME_ENV}'/>
            <input id='game_name_hidden' type='hidden' value='${GAME}'/>
            <input id='build_number_hidden' type='hidden' value='0'/>
            <input id='build_numbers_hidden' type='hidden' value='${BUILD_NUMBERS_STRING}'/>
            <input id='domain_url_hidden' type='hidden' value='${DOMAIN_URL}'/>
            <input id='test_run_type' type='hidden' value='${TEST_RUN_TYPE}'/>
            ${HIDDEN_SCREENSHOT_HTML}
            <div id='gallery_panel' style='margin-top: 40px;'>
                <h2 style='margin: 10px; margin-bottom: 25px;'>Screenshot Gallery</h2>
                <h4 style='margin: 10px; margin-left: 25px;' id="full_device_report_header">Full Device Reports</h4>
                <div id='gallery_panel_screenshots' style='margin-top: 40px;'></div>
            </div>
            <div id='screenshot_panel' class='display_panel'>
                <div class='close' onclick='\$(this).parent().hide(400); CloseTransparencyLayer();'>X</div>
                <div class='screenshot_image'></div>
            </div>
            <div class='background_transparency'></div>
       </body>"

echo ${HTML} > ./report.html

if [ ! -d ${DEFAULT_DIRECTORY}/TestResultsArchive ]; then
    echo "Creating ${DEFAULT_DIRECTORY}/TestResultsArchive"
    mkdir ${DEFAULT_DIRECTORY}/TestResultsArchive
fi

echo "Creating ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}"
mkdir ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}

echo "Copying ${DEFAULT_DIRECTORY}/report to ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}"
cp -rv ${DEFAULT_DIRECTORY}/report ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}
