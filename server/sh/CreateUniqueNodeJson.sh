echo "Creating Node Json for Selenium Grid for device ${DEVICE_UDID}"

export NODE_JSON="
{
  \"capabilities\":
      [
        {
          \"applicationName\":\"${APPIUM_DEVICE}\",
          \"automationName\":\"${DRIVER}\",
          \"browserName\":\"${DEVICE_PLATFORM}\",
          \"browser\":\"${DEVICE_PLATFORM}\",
          \"maxInstances\": 1,
          \"platform\":\"${PLATFORM_CAPS}\",
          \"newCommandTimeout\":\"30\",
          \"deviceReadyTimeout\": 5,
          \"deviceName\":\"${DEVICE_UDID}\"
        }
      ],
  \"configuration\":
  {
    \"proxy\": \"org.openqa.grid.internal.BaseRemoteProxy\",
    \"timeout\": 10800,
    \"hubPort\": 4444,
    \"hubHost\": \"${HUB_HOST}\",
    \"hub\": \"http://${HUB_HOST}:4444/grid/register\",
    \"url\":\"http://${HUB_HOST}:${UNIQUE_PORT}/wd/hub\",
    \"port\": ${UNIQUE_PORT},
    \"host\":\"${HOST}\",
    \"maxSession\": 1,
    \"register\": true,
    \"registerCycle\": 5000,
    \"cleanUpCycle\": 2000
  }
}
"

echo ${NODE_JSON} > ~/Appium/${DEVICE_PLATFORM}/${GAME}/${APPIUM_DEVICE}/${DEVICE_UDID}.json
