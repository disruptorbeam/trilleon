echo "NOT_RUN" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
$TEST_RUN_STATUS =  "NOT_RUN"

# Remove all previous .txt, .xml, .properties, and .zip files from top level workspace directory.
Get-ChildItem ${DEFAULT_DIRECTORY} -recurse -force -include *.txt | Remove-Item -force
Get-ChildItem ${DEFAULT_DIRECTORY} -recurse -force -include *.xml | Remove-Item -force
Get-ChildItem ${DEFAULT_DIRECTORY} -recurse -force -include *.properties | Remove-Item -force
Get-ChildItem ${DEFAULT_DIRECTORY} -recurse -force -include *.zip | Remove-Item -force
Get-ChildItem ${DEFAULT_DIRECTORY} -recurse -force -include *.log | Remove-Item -force

# Removes all but the newest ipa/apk. After this is run, only one ipa/apk should exist in the workspace.
Get-ChildItem ${DEFAULT_DIRECTORY} -Filter *.ipa | Sort CreationTime -desc | Select -Skip 1 | Remove-Item
Get-ChildItem ${DEFAULT_DIRECTORY} -Filter *.apk | Sort CreationTime -desc | Select -Skip 1 | Remove-Item

If(-Not (Test-Path -Path C:/Appium)) {
    echo "Creating C:/Appium"
    New-Item -ItemType Directory C:/Appium
}

If(-Not (Test-Path -Path ${DEFAULT_DIRECTORY}/resources)) {
    echo "Creating ${DEFAULT_DIRECTORY}/resources"
    New-Item -ItemType Directory ${DEFAULT_DIRECTORY}/resources
}

#Set IP Adress for client to hit Redis pubsub server on.
If(${IP_ADDRESS_SOCKET_SERVER}.Length -eq 0) {
    $IP_ADDRESS_SOCKET_SERVER = $(ipconfig | where {$_ -match 'IPv4.+\s(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})' } | out-null; $Matches[1])
}

# Delete the previous test run's results so that, if this test run files to properly execute, old results are not displayed as if they are the current run's results.
If(Test-Path -Path ${DEFAULT_DIRECTORY}/LatestTestResults) {
    echo "Deleting ${DEFAULT_DIRECTORY}/LatestTestResults"
    Remove-Item -Path ${DEFAULT_DIRECTORY}/LatestTestResults -Force -Recurse
}
If(Test-Path -Path ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport) {
    echo "Deleting existing ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport directory."
    Remove-Item -Path ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport -Force -Recurse
}
echo "Making ${DEFAULT_DIRECTORY}/LatestTestResults directory."
New-Item -ItemType Directory ${DEFAULT_DIRECTORY}/LatestTestResults

# Create the node json that will be used to register with Selenium Grid (required for parallel execution).
Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.ps1 -Name IsReadOnly -Value $false
Start-Process Powershell ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.ps1 -NoNewWindow -Wait

# Kill pre-existing appium sessions for just this device.
 Stop-Process -Id (Get-NetTCPConnection -LocalPort ${UNIQUE_PORT}).OwningProcess -Force

# Clear and recreate pre-existing device directory.
If(Test-Path -Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}) {
    echo "Deleting C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} directory."
    Remove-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} -Recurse -Force
fi

# Prepare directory and files for test execution.
echo "mkdir C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} directory."
New-Item -ItemType Directory C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/RelevantPubNubCommunications.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/RelevantPubNubCommunications.txt
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FormattedCommunicationHistory.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FormattedCommunicationHistory.txt
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/PyLog.log"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/PyLog.log
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/ServerClientLogRaw.log"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/ServerClientLogRaw.log
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/client_request_queue.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/client_request_queue.txt
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Screenshots_FPS_data.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Screenshots_FPS_data.txt
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_status.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_status.txt
echo "mk file C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/pass_fail_count.txt"
echo "" > C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/pass_fail_count.txt
echo "Copy folder ${DEFAULT_DIRECTORY}/${PATH_TO_HTML_REPORT_SCRIPTS}/TrilleonDefaultAutomationReportCss.txt to ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportCss.css"
Copy-Item ${DEFAULT_DIRECTORY}/${PATH_TO_HTML_REPORT_SCRIPTS}/TrilleonDefaultAutomationReportCss.txt ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportCss.css
echo "Copy folder ${DEFAULT_DIRECTORY}/${PATH_TO_HTML_REPORT_SCRIPTS}/TrilleonDefaultAutomationReportCss.txt to ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportDatatable.js"
Copy-Item ${DEFAULT_DIRECTORY}/${PATH_TO_HTML_REPORT_SCRIPTS}/TrilleonDefaultAutomationReportDatatable.txt ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportDatatable.js

If(${DEVICE_PLATFORM} -eq "ios") {

    echo "Detected Devices"
    $DEVICE_LIST = $(instruments -s devices)
    echo ${DEVICE_LIST}
    If($DEVICE_LIST.Contains("${DEVICE_UDID}")) {
        echo "${APPIUM_DEVICE} (${DEVICE_UDID}) not found by instruments. Cannot execute test. Check to see if device is offline."
        exit 1
    }

    echo "Copying archived ipa to C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.ipa"
    Copy-Item ${DEFAULT_DIRECTORY}/*.ipa C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.ipa

} Else {

    echo "Detected Devices"
    $DEVICE_LIST=$(adb devices)
    echo ${DEVICE_LIST}
    If($DEVICE_LIST.Contains("${DEVICE_UDID}")) {
        echo "${APPIUM_DEVICE} (${DEVICE_UDID}) not found by adb. Cannot execute test. Check to see if device is offline."
        exit 1
    }
    echo "Copying archived apk to C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.apk"
    Copy-Item ${DEFAULT_DIRECTORY}/*.apk C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.apk

}

# Create Test Results history directory (if necessary) and current results directory.
If(Test-Path ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport) {
    echo "Creating ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport directory."
    New-Item -ItemType Directory ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport
}

# Remove old staging results file.
If(Test-Path TEST-all.xml) {
  echo "Deleting pre-existing TEST-all.xml file."
  Remove-Item TEST-all.xml -Force
}

# Device Python and Shell Scripts
echo "Removing any source control locks on project files."
Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/Globals.py -Name IsReadOnly -Value $false
Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/BaseAppiumTest.py -Name IsReadOnly -Value $false
Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/GameAppiumTest.py -Name IsReadOnly -Value $false
Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/run-tests.ps1 -Name IsReadOnly -Value $false
Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportDatatable.js -Name IsReadOnly -Value $false
Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/resources/TrilleonDefaultAutomationReportCss.css -Name IsReadOnly -Value $false

#Copy files to directories
Copy-Item -Path "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/Globals.py" -Destination C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} # Global variables module needed by test.
Copy-Item -Path "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/BaseAppiumTest.py" -Destination C:Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} # Inhereted UnitTest.TestCase class with utilities needed by test.
Copy-Item -Path "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/py/GameAppiumTest.py" -Destination C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} # Actual test case that launches server test.
Copy-Item -Path "${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/run-tests.ps1" -Destination C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE} # Local Appium shell

# Copy json config files for nodes.
Copy-Item -Path ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.ps1 -Destination ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/CreateUniqueNodeJson.ps1

# Launch Tests
cd C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}
Start-Process Powershell C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/run-tests.ps1 -NoNewWindow -Wait
  
echo "Base Directory C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}"
If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TEST-all.xml) {
    echo "Results found and copied to both ${DEFAULT_DIRECTORY}/TEST-all.xml AND ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}.xml"
    cp C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TEST-all.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
    cp C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TEST-all.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/results.xml
Else {
    If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FatalErrorDetected.txt) {
        Copy-Item ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/GameUnavailable.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
        Copy-Item ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/GameUnavailable.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/results.xml
    } ElseIf(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test-reports.xml) {
        echo "RESULTS NOT FOUND. Copying testrunner xml instead to both ${DEFAULT_DIRECTORY}/TEST-all.xml AND ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}.xml"
        Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test-reports.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
        Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test-reports.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/results.xml
    } Else {
        Copy-Item ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/Default.xml ${DEFAULT_DIRECTORY}/TEST-all.xml
        Copy-Item ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/xml/Default.xml ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/results.xml
    }
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt) {
    echo "screenshot_requests.txt found and copied to ${DEFAULT_DIRECTORY}/screenshot_requests.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshot_requests.txt ${DEFAULT_DIRECTORY}/screenshot_requests.txt
} Else {
    echo "WARNING: No screenshot_requests.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollection.txt) {
    echo "GarbageCollection.txt found and copied to ${DEFAULT_DIRECTORY}/GarbageCollection.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollection.txt ${DEFAULT_DIRECTORY}/GarbageCollection.txt
} Else {
    echo "WARNING: No GarbageCollection.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollectionJson.txt) {
    echo "GarbageCollectionJson.txt found and copied to ${DEFAULT_DIRECTORY}/GarbageCollectionJson.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GarbageCollectionJson.txt ${DEFAULT_DIRECTORY}/GarbageCollectionJson.txt
} Else {
    echo "WARNING: No GarbageCollectionJson.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapSize.txt) {
    echo "HeapSize.txt found and copied to ${DEFAULT_DIRECTORY}/HeapSize.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapSize.txt ${DEFAULT_DIRECTORY}/HeapSize.txt
} Else {
    echo "WARNING: No HeapSize.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapJson.txt) {
    echo "HeapJson.txt found and copied to ${DEFAULT_DIRECTORY}/HeapJson.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/HeapJson.txt ${DEFAULT_DIRECTORY}/HeapJson.txt
} Else {
    echo "WARNING: No HeapJson.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Fps.txt) {
    echo "Fps.txt found and copied to ${DEFAULT_DIRECTORY}/Fps.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Fps.txt ${DEFAULT_DIRECTORY}/Fps.txt
} Else {
    echo "WARNING: No Fps.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FpsJson.txt) {
    echo "FpsJson.txt found and copied to ${DEFAULT_DIRECTORY}/FpsJson.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FpsJson.txt ${DEFAULT_DIRECTORY}/FpsJson.txt
} Else {
    echo "WARNING: No FpsJson.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GameInitializationTime.txt ]; then
    echo "GameInitializationTime.txt found and copied to ${DEFAULT_DIRECTORY}/GameInitializationTime.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/GameInitializationTime.txt ${DEFAULT_DIRECTORY}/GameInitializationTime.txt
} Else {
    echo "WARNING: No GameInitializationTime.txt found!"
}

If(Test-Path C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/ExceptionsJson.txt ]; then
    echo "ExceptionsJson.txt found and copied to ${DEFAULT_DIRECTORY}/ExceptionsJson.txt"
    Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/ExceptionsJson.txt ${DEFAULT_DIRECTORY}/ExceptionsJson.txt
} Else {
    echo "WARNING: No ExceptionsJson.txt found!"
}

echo "Copying appium log to ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/Appium/appium.log"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/appium.log ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/appium.log

echo "Copying device id txt file to C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_run_id.txt ${DEFAULT_DIRECTORY}/test_run_id.txt

echo "Copying screenshots directory to ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/screenshots"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshots ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport

echo "Copying screenshots directory to ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/screenshots ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_ScreenshotGalleryReport

echo "Copying test run header html directory to ${DEFAULT_DIRECTORY}/TestResultsArchive/${BUILD_NUMBER}/HtmlReport/TestRunHeaderHtml.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/TestRunHeaderHtml.txt ${DEFAULT_DIRECTORY} # Unique device details for device used in this test run.

echo "Copying pubnub communications text to ${DEFAULT_DIRECTORY}/RelevantPubNubCommunications.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/RelevantPubNubCommunications.txt ${DEFAULT_DIRECTORY}/RelevantPubNubCommunications.txt

echo "Copying formatted server-client communications text to ${DEFAULT_DIRECTORY}/FormattedCommunicationHistory.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/FormattedCommunicationHistory.txt ${DEFAULT_DIRECTORY}/FormattedCommunicationHistory.txt

echo "Copying screenshot fps data text to ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_Screenshots_FPS_data.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/Screenshots_FPS_data.txt ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_Screenshots_FPS_data.txt

echo "Copying PyLog logging text to ${DEFAULT_DIRECTORY}/PyLog.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/PyLog.txt ${DEFAULT_DIRECTORY}/PyLog.txt

echo "Copying PyLog logging text to ${DEFAULT_DIRECTORY}/PyLog.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/ServerClientLogRaw.log ${DEFAULT_DIRECTORY}/ServerClientLogRaw.log

echo "Copying Pass/Fail count text to ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_pass_fail_count.txt"
Copy-Item C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/pass_fail_count.txt ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_pass_fail_count.txt

Set-ItemProperty -Path ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/BuildReport.ps1 -Name IsReadOnly -Value $false
Start-Process Powershell ${DEFAULT_DIRECTORY}/${BASE_SCRIPTS_PATH}/sh/BuildReport.ps1 -NoNewWindow -Wait

echo "Saving Results..."
TEST_RUN_STATUS=$(cat C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/test_status.txt)
echo ${TEST_RUN_STATUS} > ${DEFAULT_DIRECTORY}/test_status.txt

echo "Saving Build Number..."
echo ${BUILD_NUMBER} > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_Build_Number.txt

echo "Deleting app file..."
Remove-Item *.apk -Force
Remove-Item *.ipa -Force

$XML=$(cat ${DEFAULT_DIRECTORY}/TEST-all.xml)
echo "Test Run Status = ${TEST_RUN_STATUS}"
If(${TEST_RUN_STATUS} -eq "CRASH_DURING_RUN") {
    #Partial run completion, mark unstable.
    echo "UNSTABLE" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
} ElseIf($XML.Contains("GameAppiumTest")) {
    echo "CRASH" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
    exit 1
} ElseIf($XML.Contains("<failure ")) {
    echo "UNSTABLE" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
} Else {
    echo "SUCCESS" > ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt
}
echo -n "|${BUILD_NUMBER}" >> ${DEFAULT_DIRECTORY}/${APPIUM_DEVICE}_results.txt

#Kill appium session(s) for just this device.
 Stop-Process -Id (Get-NetTCPConnection -LocalPort ${UNIQUE_PORT}).OwningProcess -Force
