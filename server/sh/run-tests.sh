#!/bin/bash
set -e
set -x
TEST=${TEST:="GameAppiumTest.py"} #Name of the test file

echo "Starting Appium ..."
if [ ${DEVICE_PLATFORM} = "ios" ]; then
    export APPLICATION=~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.ipa
    # node ${APPIUM_LOCATION} --session-override --nodeconfig ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/${DEVICE_UDID}.json -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
    node ${APPIUM_LOCATION} --session-override -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
else
    export APPLICATION=~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/application.apk
    node ${APPIUM_LOCATION} -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
    # node ${APPIUM_LOCATION} --nodeconfig ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/${DEVICE_UDID}.json -p ${UNIQUE_PORT} -U ${DEVICE_UDID} --log ${DEFAULT_DIRECTORY}/appium.log 2>&1 &
    export DEVICE_MODEL="$(adb -s ${DEVICE_UDID} shell getprop | grep ro.product.model)"
    export DEVICE_VERSION="$(adb -s ${DEVICE_UDID} shell getprop | grep ro.build.version.release)"
    export DEVICE_MANUFACTURER="$(adb -s ${DEVICE_UDID} shell getprop | grep ro.product.manufacturer)"
    APILEVEL=$(adb -s ${DEVICE_UDID} shell getprop ro.build.version.sdk)
    APILEVEL="${APILEVEL//[$'\t\r\n']}"
    echo "API level is: ${APILEVEL}"
fi

sleep 10 # Wait for appium to fully launch
ps -ef|grep a[p]pium

echo "Running test ${TEST}"
python3 ${TEST}

echo "Completed! Reporting results."
if [ ! -f TEST-all.xml ]; then
    echo "File not found!"
    mv test-reports.xml /TEST-all.xml
fi
