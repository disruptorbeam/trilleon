'''
+   This file is part of Trilleon.  Trilleon is a client automation framework.
+
+   Copyright (C) 2017 Disruptor Beam
+
+   Trilleon is free software: you can redistribute it and/or modify
+   it under the terms of the GNU Lesser General Public License as published by
+   the Free Software Foundation, either version 3 of the License, or
+   (at your option) any later version.
+
+   This program is distributed in the hope that it will be useful,
+   but WITHOUT ANY WARRANTY; without even the implied warranty of
+   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
+   GNU Lesser General Public License for more details.
+
+   You should have received a copy of the GNU Lesser General Public License
+   along with this program.  If not, see <http://www.gnu.org/licenses/>.
'''

import os
import os.path
import sys
import time
import subprocess
import unittest
from appium import webdriver
import pprint
import tempfile
import uuid
import urllib
from datetime import datetime
import redis
from threading import Thread
from pubnub.callbacks import SubscribeCallback
from pubnub.enums import PNStatusCategory
from pubnub.pnconfiguration import PNConfiguration
from pubnub.pubnub import PubNub
import Globals

def publish_callback(envelope, status):
    return #Do nothing.

class PubnubSubscribeCallback(SubscribeCallback):
    def message(self, pubnub, message):
        full_message = ""
        if type(message.message) is dict:
            for item in message.message:
                full_message += str(item) + ","
        elif type(message.message) is list:
            full_message = ",".join(message.message)
        else:
            full_message = message.message
        callback(full_message, "")

def error(message):
    log("PubNub Erred: %s" % (message))

def _callback(message):
    log("")

def callback(message, channel):
    # Differentiate intended client messages from its Buddy's messages.
    caller = message.split("grid_identity_buddy")
    identity = ""
    if len(caller) > 1:
        identity = caller[0].split("grid_identity")[1]    

    isValid = os.environ['GRID_IDENTITY_PREFIX'] in identity # Automatically valid if coming from our expected client.
    if isValid == False:
        # If this is not from our expected client, determine if it from our client's Buddy (will have our client's identifier along with "buddy_command_flag" in message).
        isValid = os.environ['GRID_IDENTITY_PREFIX'] in message and "buddy_command_flag" in message

    if isValid:
        message_text = urllib.parse.unquote(message).replace("\\", "").replace("\"", "'").replace("+", " ")
        server_client_log(message_text)
        with open("RelevantPubNubCommunications.txt", "a") as f:
            f.write(message_text)
        
        if "Exception in framework killed TestRunner" in message_text:
            Globals.critical_exception = True
        
        if "heartbeat_" in message_text:
            Globals.heartbeats += message_text
        # Store formatted message for final report.
        if "grid_source" in message_text and ("'message'" in message_text or "'notification'" in message_text):
            with open("FormattedCommunicationHistory.txt", "a") as f:
                try:
                    source = ""
                    new_message = ""
                    raw = message_text.split("grid_source")[1][0:10]
                    if "server" in raw:
                        source = "server"
                    elif "client" in raw:
                        source = "client"
                    if "'notification'" in message_text:
                        new_message = message_text.split("'notification'")[1][2:].split("}")[0]
                    elif "'message'" in message_text:
                        new_message = message_text.split("'message'")[1][2:].split("}")[0]
                    if len(new_message) > 0 and len(source) > 0:
                        log(source + "##" + new_message.replace("\"", "").replace("'", "") + "$$")
                        f.write(source + "##" + new_message.replace("\"", "").replace("'", "") + "$$")
                except Exception as e:
                    log("Error Saving Formatted Message [" + str(e) + "]")
        # See if client needs appium to take a screenshot.
        if "request_screenshot" in message:
            screenshot_test_name = message_text.split("request_screenshot||")[1].split("||")[0]
            with open("screenshot_requests.txt", "a") as f:
                f.write(screenshot_test_name)
            fps = message_text.split("FPS||")[1].split("||")[0]
            with open("Screenshots_FPS_data.txt", "a") as f:
                f.write(screenshot_test_name + "##" + fps + "##" + os.environ['APPIUM_DEVICE'] + "|")
        # Handle any server command.
        if "SERVER_BROKER_" in message:
            initialDelimiter = "SERVER_BROKER_COMMAND|"
            finalDelimiter = "|"
            raw = message.split(initialDelimiter)[1]
            request_command = raw.split(finalDelimiter)[0]
            initialDelimiter = "SERVER_BROKER_VALUE|"
            raw = message.split(initialDelimiter)[1]
            request_value = raw.split(finalDelimiter)[0]
            log("Logging SERVER BROKER command [ " + request_command + " : " + request_value + "]")
            with open("client_request_queue.txt", "a") as f:
                f.write(request_command + "|" + request_value)
        # Whether this is a partial json message, or full test message, we want to record the failure or success of this test. Note the lack of a pipe in this condition, versus the next condition where a pipe exists
        if "SINGLE_TEST_RESULTS_JSON" in message:
            if "@APOS@Passed@APOS@" in message:
                with open("pass_fail_count.txt", "a") as f:
                    f.write("P|")
            elif "@APOS@Failed@APOS@" in message:
                with open("pass_fail_count.txt", "a") as f:
                    f.write("F|")
        # Store results from a test where the JSON did not exceed the single-pubsub message limit.
        if "SINGLE_TEST_RESULTS_JSON|" in message:
            initialDelimiter = "SINGLE_TEST_RESULTS_JSON|"
            finalDelimiter = "|"
            raw = message.split(initialDelimiter)[1]
            json = raw.split(finalDelimiter)[0]
            with open("testresultsjson.txt", "a") as f:
                f.write(json)
        # Gather pieces of a single test's results as they come in. Recombine and save the full, single test's results when all pieces have arrived.
        if "SINGLE_TEST_RESULTS_JSON_MULTI_PART|" in message:
            initialDelimiter = "SINGLE_TEST_RESULTS_JSON_MULTI_PART|"
            finalDelimiter = "|"
            raw = message.split(initialDelimiter)[1]
            jsonPiece = raw.split(finalDelimiter)[0]
            metaData = jsonPiece.split("$$$")
            pieceId = metaData[0]
            pieceTotal = metaData[1]
            # testId = metaData[2] #TODO: Make it test specific so that two tests in a row dont cross contaminate. Very unlikely, but possible.
            actualJson = metaData[3]
            # Set piece to index value in array. When all expected pieces have arrived, reconstruct the JSON into a single, valid piece of JSON and save it.
            if len(Globals.test_json) == 0:
                Globals.test_json = ["" for x in range(int(pieceTotal))]
                Globals.test_json[int(pieceId)] = actualJson
            else:
                Globals.test_json[int(pieceId)] = actualJson
                if "" not in Globals.test_json:
                    Globals.test_json[int(pieceId)] = actualJson
                    with open("testresultsjson.txt", "a") as f:
                        for x in Globals.test_json:
                            f.write(x)
                    Globals.test_json = []

# Log message to console and a Py log.
def log(msg):
    if msg == "":
        return
    header = ''
    if os.environ.get('APPIUM_DEVICE'):
        header = '[%s] ' % os.environ.get('APPIUM_DEVICE')
    message = "%s%s: %s" % (header, time.strftime("%H:%M:%S"),msg)
    fileWrite = open("PyLog.log", "a")
    fileWrite.write(message)
    fileWrite.close()
    if Globals.ignore_console_printing_to_save_xml_cdata_buffer_size == False:
         print(message)
    sys.stdout.flush()

def server_client_log(msg):
    if msg == "":
        return
    header = ''
    if os.environ.get('APPIUM_DEVICE'):
        header = '[%s] ' % os.environ.get('APPIUM_DEVICE')
    message = "%s%s: %s" % (header, time.strftime("%H:%M:%S"),msg)
    fileWrite = open("ServerClientLogRaw.log", "a")
    fileWrite.write(message)
    fileWrite.close()
    if Globals.ignore_console_printing_to_save_xml_cdata_buffer_size == False:
        print(message)
    sys.stdout.flush()

class BaseAppiumTest(unittest.TestCase):

    socket = None
    thread = None
    socket_port = 9595  #Must match value in TrilleonConfig.txt
    socket_host = "127.0.0.1"  #Must match value in TrilleonConfig.txt
    pubsub = None
    channel = "Trilleon-Automation"  #Must match value in TrilleonConfig.txt
    pubnub = None

    driver = None # Appium driver that will load the app onto device, and dismiss device alerts or take screenshots.
    screenshot_count = 0 # Number of screenshots already taken successfully, and saved.
    screenshot_dir = None # URL to save screenshots at.
    appium_capabilities = {}
    ignoreBuddyTests = 0
    buddyCommandName = ""
    buddyName = ""
    buddyCheckFileName = ""
    buddyCheckComplete = False
    project_id = ""
    gridIdentity = ""
    additionalCommands = ""
    buddyIdentity = ""
    jsonGridPrefix = ""
    jsonBuddyGridPrefix = ""
    heartbeat_index = 1
    last_heartbeat_detected = None
    max_time_since_heartbeat = 80
    partialDataDelimiter = "$$$"
    
    fatalErrorDetected = False
    fatalErrorMessage = ""
    
    ready = False
    started = False
    complete = False
    timeout_default = 300
    test_execution_timeout = 0
    parsing_xml = False
    results = ""
    auto_screenshot_index = 0
    run_command = ""
    test_run_id_internal = ""

    def setUp(self, appium_url=None, platform_name=None, bundle_id = None, application_file=None, application_package=None, screenshot_dir=None,
                 application_activity=None, automation_name=None):
        Globals.test_run_id = str(uuid.uuid4())
        self.test_run_id_internal = Globals.test_run_id
        log("Globals.test_run_id [" + Globals.test_run_id + "]")
        with open("test_run_id.txt", "w") as f:
            f.write(Globals.test_run_id)
        self.setChannelPrefixes()
        self.set_screenshot_dir('%s/screenshots' % (os.getcwd()))

        if os.environ.get('CONNECTION_STRATEGY').lower() == "pubnub":
            pnconfig = PNConfiguration()
            pnconfig.subscribe_key = "YOUR_SUBSCRIBE_KEY_HERE"  #Must match value in TrilleonConfig.txt
            pnconfig.publish_key = "YOUR_PUBLISH_KEY_HERE"  #Must match value in TrilleonConfig.txt
            self.pubnub = PubNub(pnconfig)
            self.pubnub.add_listener(PubnubSubscribeCallback())
            self.pubnub.subscribe().channels(self.channel).execute()
        else:
            self.thread = Thread(target=self.socket_callback, args=(10, ))
            self.socket = redis.StrictRedis(host=self.socket_host, port=self.socket_port, db=0)
            self.pubsub = self.socket.pubsub()
            self.pubsub.subscribe(self.channel)
            self.thread.start()
        
        # Buddy test run information.
        self.buddyName = self.channel + "-" + os.environ.get('BUDDY')
        self.ignoreBuddyTests = os.environ.get('IGNOREBUDDYSYSTEM')
        self.buddyCommandName = "manual_set_buddy_" + os.environ.get('BUDDY_RELATIONSHIP')
        
        # Set Buddy Check-In file path.
        directoryPieces = os.getcwd().split("/")
        parentDirectory = ""
        index = len(directoryPieces)
        for piece in directoryPieces:
            index -= 1
            if index > 0:
                parentDirectory += piece + "/"
        self.buddyCheckFileName = parentDirectory + self.buddyName.replace(".", "").replace("-", "") + ".txt"
        
        # Set test run command.
        self.run_command = os.environ.get('RUN_COMMAND')
        with open("testresultsjson.txt", "w") as f:
            f.write("[")
        self.additionalCommands = os.environ.get('ADDITIONAL_COMMANDS')
        
        if os.environ.get('MAX_TEST_RUN_TIMEOUT'):
            self.test_execution_timeout = os.environ.get('MAX_TEST_RUN_TIMEOUT')
        else:
            self.test_execution_timeout = 5400 #Default test launch timeout is 1.5 hours.

        # Set up driver
        if os.environ['DEVICE_PLATFORM'] == "ios":
            self.appium_capabilities["showXcodeLog"] = True
            self.appium_capabilities["useNewWDA"] = False
            self.appium_capabilities["wdaLocalPort"] = os.environ['WDA_LOCAL_PORT']
            self.appium_capabilities["autoAcceptAlerts"] = True # iOS -- Dismisses device level popups automatically.
        else:
            self.appium_capabilities["fullReset"] = True
            self.appium_capabilities["systemPort"] = os.environ['UNIQUE_BOOT_PORT']
            self.appium_capabilities["autoGrantPermissions"] = True # Android 1.6.3+ -- Gives app permissions before starting.
            self.appium_capabilities["androidInstallTimeout"] = 180000 # Double the default timeout of 90 seconds.

        self.appium_capabilities["app"] = os.environ['APPLICATION']
        self.appium_capabilities["udid"] = os.environ['DEVICE_UDID']
        self.appium_capabilities["automationName"] = os.environ['DRIVER']
        self.appium_capabilities["platformName"] = os.environ['DEVICE_PLATFORM']
        if 'PLATFORM_VERSION' in os.environ:
            self.appium_capabilities["platformVersion"] = os.environ['PLATFORM_VERSION']
        self.appium_capabilities["deviceName"] = os.environ['DEVICE_UDID']
        self.appium_capabilities["newCommandTimeout"] = 999999
        self.appium_capabilities["launchTimeout"] = 99999
        time.sleep(5)
        #try:
        log("Starting driver on " + "http://localhost:" + os.environ['UNIQUE_PORT'] + "/wd/hub")
        self.driver = webdriver.Remote("http://localhost:" + os.environ['UNIQUE_PORT'] + "/wd/hub", self.appium_capabilities)
        time.sleep(10)

    def socket_callback(self, message):
        while Globals.test_done != True:
            time.sleep(0.05)
            message = self.pubsub.get_message()
            if message != None and str(message['data']) != "":
                callback(str(message['data']), "")

    def postMessage(self, message):
        postString = self.jsonGridPrefix + message
        if os.environ.get('CONNECTION_STRATEGY').lower() == "pubnub":
            self.pubnub.publish().channel(self.channel).message([postString]).async(publish_callback)
            log("Request posted to PubNub [" + postString + "]")
        else:
            self.socket.publish(self.channel, postString)
            log("Request posted to socket listeners [" + postString + "]")

    def setChannelPrefixes(self):
        log("PUBSUB CHANNEL ID: " + self.channel)
        self.gridIdentity = "Trilleon-Automation-" + os.environ['GRID_IDENTITY_PREFIX']
        self.jsonGridPrefix = "{\"test_run_id\":\"" + Globals.test_run_id + "\"},{\"grid_identity\":\"" + self.gridIdentity + "\"},{\"grid_source\":\"server\"},{\"grid_context\":\"StandardAlert\"},"
        self.jsonBuddyGridPrefix = "{\"test_run_id\":\"" + Globals.test_run_id + "\"},{\"grid_identity\":\"" + self.buddyName + "\"},{\"grid_source\":\"server\"},{\"grid_context\":\"BuddyMessage\"},"

    # Close down the driver, call XML and JSON finalizer, close PubNub connection, and handle any fatal execution error.
    def tearDown(self):
        Globals.test_done = True
        if os.environ.get('CONNECTION_STRATEGY').lower() == "pubnub":
            try:
                self.pubnub.remove_listener(PubnubSubscribeCallback())
            except Exception as e:
                log("Could not remove Pubnub listener. Error: " + str(e))
            self.pubnub.unsubscribe().channels(self.channel).execute()
        else:
            self.pubsub.unsubscribe()
            self.thread.join(10)

        #self.pubnub.unsubscribe(self.channel)
        self.writeAutomationResults('TEST-all.xml')
        if self.fatalErrorDetected == True:
            with open("FatalErrorDetected.txt", "w") as f:
                f.write(self.fatalErrorMessage)
        time.sleep(5)
        try:
            self.driver.quit()
        except Exception as e:
            log("Error encountered while quitting driver" + str(e))

        with open("test_status.txt", "w") as f:
            f.write(os.environ['TEST_RUN_STATUS'])
        pylogtext = ""
        with open("PyLog.log", "r") as f:
            pylogtext = f.read()
        if "CRITICAL_SERVER_FAILURE_IN_APPIUM_TEST" in pylogtext:
            raise Exception("CRITICAL_SERVER_FAILURE_IN_APPIUM_TEST Detected!")
        log("BaseAppiumTest.py and GameAppiumTest.py complete.")
        os._exit(1)

    # Writes all XML to storage file, and verifies JSON validity so that HTML report is rendered correctly.
    def writeAutomationResults(self, filename):
        # Verify that JSON recorded is valid. Add placeholder JSON if it is not.
        text = ""
        with open("testresultsjson.txt", "r") as f:
            text = f.read() # Replace any missing commas between JSON objects and attributes.
        
        if len(self.results) > 0:
            if "failure" in self.results:
                os.environ['TEST_RUN_STATUS'] = "UNSTABLE"
            else:
                os.environ['TEST_RUN_STATUS'] = "SUCCESS"
            # Game error popup check.
            if self.fatalErrorDetected:
                os.environ['TEST_RUN_STATUS'] = "CRASH_DURING_RUN"
                with open("testresultsjson.txt", "a") as f:
                    f.write("{\"order_ran\":\"999\", \"status\":\"Failed\", \"name\":\"ERROR_POPUP\", \"class\":\"FATAL_ERROR\", \"test_categories\":\"ERROR_POPUP\", \"result_details\":\"The game produced an error popup that affected automation execution. Check screenshot for details.\", \"assertions\":[]},")
            with open(os.getcwd() + "/" + filename, "w") as f:
                f.write(self.results)

        else:
            # Failure occurred - likely an app crash. Send PubNub History to help investigate where it happened.
            log("FATAL APPLICATION ERROR DISRUPTED AUTOMATION")
            # If there is existing JSON, report that app crashed or hanged.
            if len(text) > 5:
                os.environ['TEST_RUN_STATUS'] = "CRASH_DURING_RUN"
                with open("testresultsjson.txt", "a") as f:
                    f.write("{\"order_ran\":\"999\", \"status\":\"Failed\", \"name\":\"GAME_CRASHED_OR_HANGED\", \"class\":\"FATAL_ERROR\", \"test_categories\":\"CRASH_HANG\", \"result_details\":\"The game either suddenly crashed, or automation execution encountered an unhandled fatal error that halted test execution. View screenshot for details.\", \"assertions\":[]},")
            else:
                # If results were not recorded, then the app never loaded, crashed, or automation could not launch.
                if "checking_in" not in self.get_communication_history():
                    os.environ['TEST_RUN_STATUS'] = "CRASH_DURING_LAUNCH"
                    with open("testresultsjson.txt", "w") as f:
                        f.write("[{\"order_ran\":\"0\", \"status\":\"Failed\", \"name\":\"GAME_LAUNCH_FAILURE\", \"class\":\"FATAL_ERROR\", \"test_categories\":\"LAUNCH_FAILURE\", \"result_details\":\"The game crashed on load, or otherwise failed to reach a state where automation could register on the pubsub communication channel.\", \"assertions\":[]},")
                else:
                    os.environ['TEST_RUN_STATUS'] = "CRASH_AFTER_LAUNCH"
                    if "order_ran" in self.get_communication_history():
                        with open("testresultsjson.txt", "a") as f:
                            f.write(",{\"order_ran\":\"0\", \"status\":\"Failed\", \"name\":\"GAME_LAUNCH_FAILURE\", \"class\":\"FATAL_ERROR\", \"test_categories\":\"LAUNCH_FAILURE\", \"result_details\":\"The game launched, and automation registered on the pubsub communication channel, but either the app crashed, or automation was blocked from beginning its test run.\", \"assertions\":[]},")
                    else:
                        with open("testresultsjson.txt", "w") as f:
                            f.write("[{\"order_ran\":\"0\", \"status\":\"Failed\", \"name\":\"GAME_LAUNCH_FAILURE\", \"class\":\"FATAL_ERROR\", \"test_categories\":\"LAUNCH_FAILURE\", \"result_details\":\"The game launched, and automation registered on the pubsub communication channel, but either the app crashed, or automation was blocked from beginning its test run.\", \"assertions\":[]},")

    def set_screenshot_dir(self, screenshot_dir):
        log("Saving screenshots at: " + screenshot_dir)
        self.screenshot_dir = screenshot_dir
        if not os.path.exists(screenshot_dir):
            os.mkdir(self.screenshot_dir)

    def take_screenshot(self, path):
        log("Attempting to save screenshot at path [" + self.screenshot_dir + path + "]")
        try:
            self.driver.save_screenshot(self.screenshot_dir + path)
        except Exception as e:
            log("Exception! " + str(e))

    def handle_device_alert(self, accept):
        if os.environ['DEVICE_PLATFORM'] == "android":
            # Try to dismiss any Android account alerts.
            try:
                acceptButtons = self.driver.find_element_by_xpath("//*[@text='Allow']")
                if len(acceptButtons) > 0:
                    action = TouchAction(self.driver)
                    action.tap(acceptButton[0]).perform()
                    log("Chose 'Allow' for Google Play alert.")
            except Exception as e:
                log("No device level alerts found. Skipping alert dismissal.")
            try:
                acceptButtons = self.driver.find_element_by_xpath("//*[@text='OK']")
                if len(acceptButtons) > 0:
                    action = TouchAction(self.driver)
                    action.tap(acceptButton[0]).perform()
                    log("Chose 'OK' for Google Play alert.")
            except Exception as e:
                log("No device level alerts found. Skipping alert dismissal.")
        else:
            try:
                alert_text = self.driver.switch_to.alert.text
                if self.driver.switch_to.alert != None and len(alert_text) > 0:
                    log("Attempting to dismiss iOS device-level alert. Text: " + alert_text)
                    if accept == True:
                        self.driver.switch_to.alert.accept()
                    else:
                        self.driver.switch_to.alert.dismiss()
                else:
                    log("No device level alerts found. Skipping alert dismissal.")
            except Exception as e:
                log("No device level alerts found. Skipping alert dismissal.")

    # Write name of self to parent level file that selected Buddy will check to verify both Buddy's have begun their test runs.
    def buddy_check_in(self):
        #directoryPieces = os.getcwd().split("/")
        #self.parentDirectory = ""
        #index = len(directoryPieces)
        #for piece in directoryPieces:
            #index -= 1
            #if index > 0:
                #self.parentDirectory += piece + "/"
        #with open(self.buddyCheckFileName, "a") as f:
            #f.write(self.gridIdentity)
        log("Writing Device Identity [" + self.gridIdentity + "] To File [" + self.buddyCheckFileName + "].")

    # See if Buddy has checked in as active and running. Will be used to skip Buddy tests if associated Buddy cannot perform its function.
    def buddy_check(self):
        return True
        #fileReadTest = ""
        #with open(self.buddyCheckFileName, "r") as f:
            #fileReadTest = f.read()
        #self.buddyCheckComplete = True
        log("Reading Buddy Check File. Expecting To See Buddy Name [" + self.buddyName + "] In File Contents [" + fileReadTest + "].")
        #if self.buddyName not in fileReadTest:
            #TODO: self.postMessage("{\"buddy_ignore_all\":0}")
            #return False
        #else:
            #return True

    # Break final XML generated by AutomationReport.cs out from total pubsub history and format it properly before placing it in final xml file.
    def get_xml_from_client_run(self):
        log("Command Recieved: [Get XML]")
        recent_posts = self.get_communication_history()
        # Toke "xml_complete" is required for this script to know that a client has reached completion of its test run.
        if ("xml_complete" not in recent_posts and "completed_automation" not in recent_posts) or self.parsing_xml == True:
            return False
        self.parsing_xml = True
        log("Xml Complete token detected. Begining parsing of test results xml.")
        time.sleep(5)
        
        # Split wall of pubsub text on start token and determine how many fragments of XML our test run intends to communicate.
        splitPieces = recent_posts.split("xml_start")
        if len(splitPieces) < 2:
            log("Failed to detect xml_start token in recent posts [" + recent_posts + "]")
            return False
        rawSplit = splitPieces[1]
        delimiter = "|"
        start = rawSplit.index(delimiter) + len(delimiter)
        end = rawSplit.index(delimiter, start)
        xmlPiecesCount = int(rawSplit[start:end])

        # XML is potentially fragmented when large test runs exceed a 4000 character pubsub message limit.
        log("XML Pieces Count [" + str(xmlPiecesCount) + "]")

        # PubNub does not guarantee sequential order of messages. We know how many xml fragments to expect, but not the order they will be recieved.
        xml_parsed = [None] * xmlPiecesCount
        list_history = recent_posts.split(Globals.test_run_id)

        # Split each fragment from history and place it into a dictionary that guarantees the proper order of reconstituted xml.
        for v in list_history:
            post = v
            if "xml_fragment_" in post:
                log("XML Fragment Detected. Parsing.")
                delimiter = "||"
                start = post.find(delimiter, 0) + 2
                end = post.find(delimiter, start + 2)
                index = int(post.split("xml_fragment_")[1][0])
                xml_parsed[index] = post[start:end]
        all_xml = ""
        for p in xml_parsed:
            if p != None:
                all_xml += p
        self.results = all_xml.replace('@APOS@', '"')
        if len(all_xml) == 0:
            log("XML empty after processing. Recent posts [" + recent_posts + "]")
        log("ALL_XML_FINAL " + self.results)
        return True
            
    # Find, extract, format, and save all single-test JSON results for server HTML report
    def format_json_from_client_run(self, force):
        log("Command Recieved: [Get JSON]")
        json = ""
        recent_posts = self.get_communication_history()
        with open("testresultsjson.txt", "r") as f:
            json = f.read()
        if "completed_automation" in recent_posts or force == True:
            # "@APOS@" token is a special encoding of double qoutes to prevent issues with PubNub message encoding and proper formatting of JSON
            json = json.replace("@APOS@", "\"").replace("}{", "},{")
            if not json.endswith("]"):
                if json.endswith(","):
                    json = json[:-1] + "]"
                else:
                    json += "]"
            log("JSON FINAL LENGTH [" + str(len(json)) + "]")
            with open("testresultsjson.txt", "w") as f:
                f.write(json)
            return True
        else:
            return False

    # This is used by the server to check for client "heartbeats", or to request them, to verify that client has not crashed or hanged in execution.
    def has_heartbeat(self):
        if self.last_heartbeat_detected == None:
            self.last_heartbeat_detected = datetime.now()

        if len(Globals.heartbeats) > 0:
            log("Registered heartbeat #" + str(self.heartbeat_index))
            self.heartbeat_index += 1
            self.last_heartbeat_detected = datetime.now()

        timeDifference = (datetime.now() - self.last_heartbeat_detected).total_seconds()
        if timeDifference > self.max_time_since_heartbeat:
            self.postMessage("{\"health_check\":0}")
            log("Any heartbeat ??" + Globals.heartbeats)
            time.sleep(15)
            if len(Globals.heartbeats) > 0:
                log("Registered heartbeat #" + str(self.heartbeat_index) + " after explicit request")
                self.heartbeat_index += 1
                self.last_heartbeat_detected = datetime.now()
                return True
            else:
                log("Heartbeat not detected in expected timeframe. Time since last heartbeat [" + str(timeDifference) + " seconds]")
                return False
        else:
            return True

    # Get pubsub history saved in text file with each recieved callback.
    def get_communication_history(self):
        results = ""
        try:
            with open("RelevantPubNubCommunications.txt", "r") as f:
                results = f.read()
        except Exception as e:
            log("Exception Reading History Text: " + str(e))
        return results

    # Get only the pubsub message history that contains a provided search term.
    def get_specific_json_from_history(self, jsonFlag):
        #Return only messages bound for this server-client relationship.
        historyText = self.get_communication_history()
        if len(historyText) == 0:
            return ""
        history = historyText.split("messageFull")
        resultsString = ""
        for x in history:
            rawMessage = urllib.parse.unquote(str(x)).replace("\\", "").replace("\"", "'").replace("+", " ")
            splitMessage = rawMessage.split("test_run_id")
            for s in splitMessage:
                if Globals.test_run_id in s and (True if jsonFlag == None else (jsonFlag in s)):
                    resultsString += s
        return resultsString

    # See if client has requested the server take a screenshot.
    def check_for_client_requests(self, command):
        if command == "screenshot":
            fileName = ""
            with open("screenshot_requests.txt", "r") as f:
                fileName = f.read().strip()
            f = open("screenshot_requests.txt", "w")
            f.write("")
            f.close()
            if any(c.isalpha() for c in fileName):
                if "Interval" in fileName:
                    fileName = str(self.auto_screenshot_index) + "_" + "interval"
                    self.auto_screenshot_index += 1
                else:
                    fileName = "/" + fileName + ".png"
                    # If this file already exists, don't take it again.
                    if os.path.exists(self.screenshot_dir + fileName) == False:
                        try:
                            self.take_screenshot(fileName)
                            self.postMessage("{\"request_response\":\"screenshot\"}")
                            log("Screenshot successfully saved [" + fileName + "]")
                        except BaseException as e:
                            log("Exception Taking Screenshot! " + str(e))
        if command == "handle_client_commands":
            commandFull = ""
            with open("client_request_queue.txt", "r") as f:
                commandFull = f.read()
            if "|" in commandFull:
                # Handle command
                commandActual = commandFull.split("|")[0]
                commandValue = commandFull.split("|")[1]
                if commandActual == "HANDLE_DEVICE_ALERT":
                    self.handle_device_alert(False if commandValue == "0" else True)
                #Clear command queue
                f = open("client_request_queue.txt", "w")
                f.write("")
                f.close()
                self.postMessage("{\"server_broker_response\":\"" + commandActual + "\"}")
            else:
                try:
                    self.driver.switch_to.alert.accept()
                except BaseException as e:
                    log("")

    def find_json_for_performance_attribute(self, delimiter):
        recent_posts = self.get_communication_history()
        endDelimiter = "|"
        json = ""
        index = 0
        fullInitialDelimiter = delimiter + "_MULTI_PART|"
        rawAll = recent_posts.split(fullInitialDelimiter)
        piecesFinal = ["" for x in range(len(rawAll) - 1)]
        log("Finding " + delimiter)
        for x in rawAll:
            if index > 0 and len(x) > 0:
                # Handle test results reporting that was too large to send in a single message, requiring several parts of a single result report.
                rawPartials = x.split(fullInitialDelimiter) # All partial message pieces
                for z in rawPartials:
                    piecesPartial = z.split(self.partialDataDelimiter)
                    if index - 1 == int(piecesPartial[0]):
                        piecesFinal[int(piecesPartial[0])] = piecesPartial[1].split(endDelimiter)[0] # The first piece after splicing is the index/order of this piece. Set that piece equal to the actual message data.
                        break
            index += 1
        for f in piecesFinal:
            json += f # Should piece together valid json in correct order if previous for loop correctly handled ordering.

        # "@APOS@" token is a special encoding of double qoutes to prevent issues with PubNub message encoding and proper formatting of JSON
        json = json.replace("@APOS@", "\"").replace("}{", "},{")
        if not json.endswith("]"):
          json += "]"
        return json

    # Check if specific messages have been communicated over PubNub and extract relevant details.
    def check_for_client_responses(self, command, postAllParsed):
        historyString = ""
        if command == "heap_json":
            log("Collecting Heap Json")
            fileWrite = open("HeapJson.txt", "w")
            fileWrite.write(self.find_json_for_performance_attribute("HEAP_JSON"))
            fileWrite.close()
            return True
        if command == "garbage_collection_json":
            log("Collecting GC Json")
            fileWrite = open("GarbageCollectionJson.txt", "w")
            fileWrite.write(self.find_json_for_performance_attribute("GC_JSON"))
            fileWrite.close()
            return True
        if command == "fps_json":
            log("Collecting FPS Json")
            fileWrite = open("FpsJson.txt", "w")
            fileWrite.write(self.find_json_for_performance_attribute("FPS_JSON"))
            fileWrite.close()
            return True
        if command == "exceptions_data":
            log("Collecting Exceptions Values")
            fileWrite = open("ExceptionsJson.txt", "w")
            fileWrite.write(self.find_json_for_performance_attribute("EXCEPTION_DATA"))
            fileWrite.close()
            return True
        if command == "garbage_collection":
            historyString = self.get_specific_json_from_history("GARBAGE_COLLECTION")
            if len(historyString) > 0:
                log("Collecting Garbage Collection Values")
                initialDelimiter = "GARBAGE_COLLECTION|"
                finalDelimiter = "|"
                raw = historyString.split(initialDelimiter)[1]
                data = raw.split(finalDelimiter)[0]
                fileWrite = open("GarbageCollection.txt", "w")
                fileWrite.write(data)
                fileWrite.close()
                return True
        if command == "heap_size":
            historyString = self.get_specific_json_from_history("HEAP_SIZE")
            if len(historyString) > 0:
                log("Collecting Heap Size Values")
                initialDelimiter = "HEAP_SIZE|"
                finalDelimiter = "|"
                raw = historyString.split(initialDelimiter)[1]
                data = raw.split(finalDelimiter)[0]
                fileWrite = open("HeapSize.txt", "w")
                fileWrite.write(data)
                fileWrite.close()
                return True
        if command == "fps":
            historyString = self.get_specific_json_from_history("FPS_VALUES")
            if len(historyString) > 0:
                log("Collecting FPS Values")
                initialDelimiter = "FPS_VALUES|"
                finalDelimiter = "|"
                raw = historyString.split(initialDelimiter)[1]
                data = raw.split(finalDelimiter)[0]
                fileWrite = open("Fps.txt", "w")
                fileWrite.write(data)
                fileWrite.close()
                return True
        if command == "game_launch_seconds":
            historyString = self.get_specific_json_from_history("GAME_LAUNCH_SECONDS")
            if len(historyString) > 0:
                log("Collecting Game Initialization Time")
                initialDelimiter = "GAME_LAUNCH_SECONDS|"
                finalDelimiter = "|"
                raw = historyString.split(initialDelimiter)[1]
                data = raw.split(finalDelimiter)[0]
                fileWrite = open("GameInitializationTime.txt", "w")
                fileWrite.write(data)
                fileWrite.close()
                return True
        if command == "device_details_html":
            historyString = self.get_specific_json_from_history("DEVICE_DETAILS_HTML")
            if len(historyString) > 0:
                log("Collecting Device Details HTML")
                initialDelimiter = "DEVICE_DETAILS_HTML|"
                finalDelimiter = "|"
                raw = historyString.split(initialDelimiter)[1]
                html = raw.split(finalDelimiter)[0].replace("@APOS@", "\"").replace("@QUOT@", "'")
                fileWrite = open("TestRunHeaderHtml.txt", "w")
                fileWrite.write(html)
                fileWrite.close()
                return True

        recent_posts = self.get_communication_history()
        if command == "ready" and "checking_in" in recent_posts:
             return True
        if command == "started" and ("starting_automation" in recent_posts or "Starting Automation" in recent_posts or "SINGLE_TEST_RESULTS_JSON" in recent_posts):
            return True
        if command == "complete" and "completed_automation" in recent_posts:
            return True
        if command == "fatal_error_check" and "Fatal Error. Shutting down automation" in recent_posts:
            if self.fatalErrorDetected == False:
                self.fatalErrorDetected = True
                self.fatalErrorMessage = "Fatal Error popup occurred. Game Unavailable!"
                self.take_screenshot("/fatal_error.png")
            return True
        return False
