#!/bin/bash
$TEST = ${TEST:="GameAppiumTest.py"} #Name of the test file

echo "Starting Appium ..."
If(${DEVICE_PLATFORM} -eq "ios") {
    $APPLICATION = C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.ipa
    # node ${APPIUM_LOCATION} --session-override --nodeconfig ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/${DEVICE_UDID}.json -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
    node ${APPIUM_LOCATION} --session-override -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
} ElseIf {
    $APPLICATION = C:/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.apk
    node ${APPIUM_LOCATION} -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
    # node ${APPIUM_LOCATION} --nodeconfig ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/${DEVICE_UDID}.json -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
    $DEVICE_MODEL = "$(adb -s ${DEVICE_UDID} shell getprop | grep ro.product.model)"
    $DEVICE_VERSION = "$(adb -s ${DEVICE_UDID} shell getprop | grep ro.build.version.release)"
    $DEVICE_MANUFACTURER = "$(adb -s ${DEVICE_UDID} shell getprop | grep ro.product.manufacturer)"
    $APILEVEL = $(adb -s ${DEVICE_UDID} shell getprop ro.build.version.sdk)
    $APILEVEL = "${APILEVEL//[$'\t\r\n']}"
    echo "API level is: ${APILEVEL}"
}

sleep 10 # Wait for appium to fully launch
Get-Process appium

echo "Running test ${TEST}"
python3 ${TEST}

echo "Completed! Reporting results."
If(Test-Path TEST-all.xml) {
    echo "File not found!"
    Move-Item -Path test-reports.xml -Destination ./TEST-all.xml
}
