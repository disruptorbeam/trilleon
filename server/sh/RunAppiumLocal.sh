echo "NOT_RUN" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
TEST_RUN_STATUS="NOT_RUN"

# Remove all previous .txt, .xml, .properties, and .zip files from top level workspace directory.
rm -f *.txt
rm -f *.xml
rm -f *.properties
rm -f *.zip
# Removes all but the newest ipa/apk. After this is run, only one ipa/apk should exist in the workspace.
rm -f $(ls -1t ${DEFAULT_DIRECTORY}/*.ipa | tail -n +2)
rm -f $(ls -1t ${DEFAULT_DIRECTORY}/*.apk | tail -n +2)

if [ ! -d ~/Appium ]; then
    echo "Creating ~/Appium."
    mkdir ~/Appium
fi

# Delete the previous test run's results so that, if this test run files to properly execute, old results are not displayed as if they are the current run's results.
if [ -d ${DEFAULT_DIRECTORY}/LatestTestResults ]; then
    echo "Deleting existing ${DEFAULT_DIRECTORY}/LatestTestResults directory."
    rm -rd ${DEFAULT_DIRECTORY}/LatestTestResults
fi
if [ -d ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport ]; then
    echo "Deleting existing ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport directory."
    rm -rd ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport
fi
if [ -f "${DEFAULT_DIRECTORY}/LatestTestResults" ]; then
    echo "Deleting existing ${DEFAULT_DIRECTORY}/LatestTestResults file."
    rm -f ${DEFAULT_DIRECTORY}/LatestTestResults
fi
echo "Making ${DEFAULT_DIRECTORY}/LatestTestResults directory."
mkdir ${DEFAULT_DIRECTORY}/LatestTestResults

export FILE_NAME=${BUILD_NUMBER}

# Create the node json that will be used to register with Selenium Grid (required for parallel execution).
chmod a+rwx ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.sh
${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.sh

# Kill pre-existing appium sessions for just this device.
PID=$(ps aux | grep a[p]pium | grep ${UNIQUE_PORT} | awk '{print $2}')
while read -r val; do
    echo "Killed PID ${val}"
    kill -9 $val
done <<< "$PID"

# Clear and recreate pre-existing device directory.
if [ -d ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} ]; then
    echo "Deleting ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} directory."
    rm -rd ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}
fi

# Prepare directory and files for test execution.
echo "mkdir ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} directory."
mkdir -p ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/RelevantPubNubCommunications.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/RelevantPubNubCommunications.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FormattedCommunicationHistory.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FormattedCommunicationHistory.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/PyLog.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/PyLog.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/client_request_queue.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/client_request_queue.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Screenshots_FPS_data.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Screenshots_FPS_data.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_status.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_status.txt
echo "mk file ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/pass_fail_count.txt"
echo "" > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/pass_fail_count.txt

if [ ${DEVICE_PLATFORM} = "ios" ]; then

    echo "Detected Devices"
    DEVICE_LIST=$(instruments -s devices)
    echo ${DEVICE_LIST}
    if [[ $DEVICE_LIST != *"${DEVICE_UDID}"* ]]; then
        echo "${APPIUM_DEVICE} (${DEVICE_UDID}) not found by instruments. Cannot execute test. Check to see if device is offline."
        exit 1
    fi

    # Set to kill proxy for this device.
    PID=$(ps aux | grep i[p]roxy | grep ${DEVICE_UDID} | awk '{print $2}')
    echo "Copying archived ipa to ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.ipa"
    cp ${DEFAULT_DIRECTORY}/*.ipa ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.ipa

else

    echo "Detected Devices"
    DEVICE_LIST=$(adb devices)
    echo ${DEVICE_LIST}
    if [[ $DEVICE_LIST != *"${DEVICE_UDID}"* ]]; then
        echo "${APPIUM_DEVICE} (${DEVICE_UDID}) not found by adb. Cannot execute test. Check to see if device is offline."
        exit 1
    fi

    # Set to kill and uninstall adb apps.
    PID=$(ps aux | grep i[o].appium | grep ${DEVICE_UDID} | awk '{print $2}')
    adb -s ${DEVICE_UDID} shell am force-stop io.appium.uiautomator2.server
    adb -s ${DEVICE_UDID} uninstall io.appium.settings
    adb -s ${DEVICE_UDID} uninstall io.appium.unlock
    adb -s ${DEVICE_UDID} uninstall io.appium.uiautomator2.server
    adb -s ${DEVICE_UDID} uninstall io.appium.uiautomator2.server.test
    echo "Copying archived apk to ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.apk"
    cp ${DEFAULT_DIRECTORY}/*.apk ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.apk

fi

# Kill PIDS set above.
while read -r val; do
    echo "Killed PID ${val}"
    kill -9 $val
done <<< "$PID"

# Create Test Results history directory (if necessary) and current results directory.
if [ ! -d ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/HtmlReport ]; then
    echo "Creating ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/HtmlReport directory."
    mkdir -p ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/HtmlReport
fi

# Remove old staging results file.
if [ -f TEST-all.xml ]; then
  echo "Deleting pre-existing TEST-all.xml file."
  rm -f TEST-all.xml
fi

# Device Python and Shell Scripts
echo "Removing locks on project files."
chmod a+rwx ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/BaseAppiumTest.py
chmod a+rwx ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/GameAppiumTest.py
chmod a+rwx ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/run-tests.sh
chmod a+rwx ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/js/TrilleonDefaultAutomationReportDatatable.js
cp "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/BaseAppiumTest.py" ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} # Inhereted UnitTest.TestCase class with utilities needed by test.
cp "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/GameAppiumTest.py" ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} # Actual test case that launches server test.
cp "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/run-tests.sh" ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} # Local Appium shell
cp "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/js/TrilleonDefaultAutomationReportDatatable.js" ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/HtmlReport/${BASE_SCRIPTS_PATH}/js/report.js # Javascript used in html report.
cp "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/js/TrilleonDefaultAutomationReportDatatable.js" ${DEFAULT_DIRECTORY}/LatestTestResults/${BASE_SCRIPTS_PATH}/js/report.js

# Copy json config files for nodes.
chmod +x ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.sh
${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.sh

# Launch Tests
cd ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}
sh ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/run-tests.sh

echo "Base Directory ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}"

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TEST-all.xml ]; then
    echo "Results found and copied to both ${DEFAULT_DIRECTORY}/TEST-all.xml AND ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}.xml"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TEST-all.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TEST-all.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/results.xml
else
    if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FatalErrorDetected.txt ]; then
        cp ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/GameUnavailable.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
        cp ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/GameUnavailable.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/results.xml
    elif [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test-reports.xml ]; then
        echo "RESULTS NOT FOUND. Copying testrunner xml instead to both ${DEFAULT_DIRECTORY}/TEST-all.xml AND ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}.xml"
        cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test-reports.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
        cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test-reports.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/results.xml
    else
        cp ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/Default.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
        cp ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/Default.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/results.xml
    fi
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt ]; then
    echo "screenshot_requests.txt found and copied to ${DEFAULT_DIRECTORY}/screenshot_requests.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt ${DEFAULT_DIRECTORY}/screenshot_requests.txt
else
    echo "WARNING: No screenshot_requests.txt found!"
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollection.txt ]; then
    echo "GarbageCollection.txt found and copied to ${DEFAULT_DIRECTORY}/GarbageCollection.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollection.txt ${DEFAULT_DIRECTORY}/GarbageCollection.txt
else
    echo "WARNING: No GarbageCollection.txt found!"
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollectionJson.txt ]; then
    echo "GarbageCollectionJson.txt found and copied to ${DEFAULT_DIRECTORY}/GarbageCollectionJson.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollectionJson.txt ${DEFAULT_DIRECTORY}/GarbageCollectionJson.txt
else
    echo "WARNING: No GarbageCollectionJson.txt found!"
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapSize.txt ]; then
    echo "HeapSize.txt found and copied to ${DEFAULT_DIRECTORY}/HeapSize.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapSize.txt ${DEFAULT_DIRECTORY}/HeapSize.txt
else
    echo "WARNING: No HeapSize.txt found!"
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapJson.txt ]; then
    echo "HeapJson.txt found and copied to ${DEFAULT_DIRECTORY}/HeapJson.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapJson.txt ${DEFAULT_DIRECTORY}/HeapJson.txt
else
    echo "WARNING: No HeapJson.txt found!"
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Fps.txt ]; then
    echo "Fps.txt found and copied to ${DEFAULT_DIRECTORY}/Fps.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Fps.txt ${DEFAULT_DIRECTORY}/Fps.txt
else
    echo "WARNING: No Fps.txt found!"
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FpsJson.txt ]; then
    echo "FpsJson.txt found and copied to ${DEFAULT_DIRECTORY}/FpsJson.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FpsJson.txt ${DEFAULT_DIRECTORY}/FpsJson.txt
else
    echo "WARNING: No FpsJson.txt found!"
fi

if [ -f ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GameInitializationTime.txt ]; then
    echo "GameInitializationTime.txt found and copied to ${DEFAULT_DIRECTORY}/GameInitializationTime.txt"
    cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GameInitializationTime.txt ${DEFAULT_DIRECTORY}/GameInitializationTime.txt
else
    echo "WARNING: No GameInitializationTime.txt found!"
fi

echo "Copying appium log to ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/Appium/appium.log"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/appium.log ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/appium.log

echo "Copying device id txt file to ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt ${DEFAULT_DIRECTORY}/test_run_id.txt

echo "Copying screenshots directory to ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/HtmlReport/screenshots"
cp -rv ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshots ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/HtmlReport

echo "Copying screenshots directory to ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport"
cp -rv ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshots ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport

echo "Copying test run header html directory to ${DEFAULT_DIRECTORY}/TestResultsArchive/${FILE_NAME}/HtmlReport/TestRunHeaderHtml.txt"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TestRunHeaderHtml.txt ${DEFAULT_DIRECTORY} # Unique device details for device used in this test run.

echo "Copying pubnub communications text to ${DEFAULT_DIRECTORY}/RelevantPubNubCommunications.txt"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/RelevantPubNubCommunications.txt ${DEFAULT_DIRECTORY}/RelevantPubNubCommunications.txt

echo "Copying formatted server-client communications text to ${DEFAULT_DIRECTORY}/FormattedCommunicationHistory.txt"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FormattedCommunicationHistory.txt ${DEFAULT_DIRECTORY}/FormattedCommunicationHistory.txt

echo "Copying screenshot fps data text to ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_Screenshots_FPS_data.txt"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Screenshots_FPS_data.txt ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_Screenshots_FPS_data.txt

echo "Copying PyLog logging text to ${DEFAULT_DIRECTORY}/PyLog.txt"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/PyLog.txt ${DEFAULT_DIRECTORY}/PyLog.txt

echo "Copying Pass/Fail count text to ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_pass_fail_count.txt"
cp ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/pass_fail_count.txt ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_pass_fail_count.txt

chmod a+rwx ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/BuildReport.sh
${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/BuildReport.sh

echo "Saving Results..."
TEST_RUN_STATUS=$(cat ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_status.txt)
echo ${TEST_RUN_STATUS} > ${DEFAULT_DIRECTORY}/test_status.txt

echo "Saving Build Number..."
echo ${BUILD_NUMBER} > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_Build_Number.txt

echo "Deleting app file..."
rm -f *.apk
rm -f *.ipa

XML=$(cat ${DEFAULT_DIRECTORY}/TEST-all.xml)
echo "Test Run Status = ${TEST_RUN_STATUS}"
if [[ ${TEST_RUN_STATUS} == "CRASH_DURING_RUN" ]]; then
    #Partial run completion, mark unstable.
    echo "UNSTABLE" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
elif [[ $XML == *"GameAppiumTest"* ]]; then
    echo "CRASH" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
    exit 1
elif [[ $XML == *"<failure "* ]]; then
    echo "UNSTABLE" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
else
    echo "SUCCESS" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
fi
echo -n "|${BUILD_NUMBER}" >> ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt

#Kill appium session(s) for just this device.
PID=$(ps aux | grep a[p]pium | grep ${UNIQUE_PORT} | awk '{print $2}')
while read -r val; do
    echo "Killed PID ${val}"
    kill -9 $val
done <<< "$PID"
exit 0
