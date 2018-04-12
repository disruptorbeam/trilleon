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
import requests
from datetime import datetime
from pubnub import Pubnub

test_run_id = ""
heartbeats = ""
def error(message):
    log("PubNub Erred: " + message)

def _callback(message):
    log("")

def callback(message, channel):
    global heartbeats
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
        message_text = urllib.unquote(message).replace("\\", "").replace("\"", "'").replace("+", " ")
        with open("RelevantPubNubCommunications.txt", "a") as f:
            f.write(message_text)
        
        if "heartbeat_" in message_text:
            heartbeats += message_text
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
        if "request_screenshot" in message:
            screenshot_test_name = message_text.split("request_screenshot||")[1].split("||")[0]
            with open("screenshot_requests.txt", "a") as f:
                f.write(screenshot_test_name)
            fps = message_text.split("FPS||")[1].split("||")[0]
            with open("Screenshots_FPS_data.txt", "a") as f:
                f.write(screenshot_test_name + "##" + fps + "##" + os.environ['APPIUM_DEVICE'] + "|")
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
        if "SINGLE_TEST_RESULTS_JSON" in message:
            if "@APOS@Passed@APOS@" in message:
                with open("pass_fail_count.txt", "a") as f:
                    f.write("P|")
            elif "@APOS@Failed@APOS@" in message:
                with open("pass_fail_count.txt", "a") as f:
                    f.write("F|")

def log(msg):
    header = ''
    if os.environ.get('APPIUM_DEVICE'):
        header = '[%s] ' % os.environ.get('APPIUM_DEVICE')
    message = "%s%s: %s" % (header, time.strftime("%H:%M:%S"),msg)
    fileWrite = open("PyLog.txt", "a")
    fileWrite.write(message)
    fileWrite.close()
    print(message)
    sys.stdout.flush()

class BaseAppiumTest(unittest.TestCase):
    
    driver = None
    screenshot_count = 0
    screenshot_dir = None
    desired_capabilities_cloud = {}
    ignoreBuddyTests = 0
    buddyCommandName = ""
    buddyName = ""
    buddyCheckFileName = ""
    buddyCheckComplete = False
    channel = "Trilleon-Automation"
    DBID = ""
    project_id = ""
    gridIdentity = ""
    buddyIdentity = ""
    pubnub = None
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
    test_execution_timeout = 4800
    parsing_xml = False
    results = ""
    auto_screenshot_index = 0
    run_command = ""
    device_height = 0
    device_width = 0
    test_run_id_internal = ""

    def setUp(self, appium_url=None, platform_name=None, bundle_id = None, application_file=None, application_package=None, screenshot_dir=None,
                 application_activity=None, automation_name=None):
        global test_run_id
        test_run_id = str(uuid.uuid4())
        self.test_run_id_internal = test_run_id
        log("test_run_id [" + test_run_id + "]")
        with open("test_run_id.txt", "w") as f:
            f.write(test_run_id)

        self.pubnub = Pubnub(publish_key="TODO: YOUR KEY HERE!",subscribe_key="TODO: YOUR KEY HERE!")
        self.setChannelPrefixes()
        self.pubnub.subscribe(channels=self.channel, callback=callback, error=error)
        self.set_screenshot_dir('%s/screenshots' % (os.getcwd()))
        
        # Buddy test run information.
        self.buddyName = "Trilleon-Automation-" + os.environ.get('BUDDY')
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
        
        # Buddy test run information.
        if os.environ.get('DEVICE_PLATFORM') == "ios":
            self.device_width = int(os.environ.get('DEVICE_WIDTH'))
            self.device_height = int(os.environ.get('DEVICE_HEIGHT'))
        with open("testresultsjson.txt", "w") as f:
            f.write("[")

        # Set up driver
        if os.environ['DEVICE_PLATFORM'] == "ios":
            self.desired_capabilities_cloud["showXcodeLog"] = True
            self.desired_capabilities_cloud["useNewWDA"] = False
            self.desired_capabilities_cloud["wdaLocalPort"] = os.environ['WDA_LOCAL_PORT']
            self.desired_capabilities_cloud["autoAcceptAlerts"] = True # iOS -- Dismisses device level popups automatically.
        else:
            self.desired_capabilities_cloud["systemPort"] = os.environ['UNIQUE_BOOT_PORT']
            self.desired_capabilities_cloud["autoGrantPermissions"] = True # Android 1.6.3+ -- Gives app permissions before starting.
        self.desired_capabilities_cloud["app"] = os.environ['APPLICATION']
        self.desired_capabilities_cloud["udid"] = os.environ['DEVICE_UDID']
        self.desired_capabilities_cloud["automationName"] = os.environ['DRIVER']
        self.desired_capabilities_cloud["platformName"] = os.environ['DEVICE_PLATFORM']
        if 'PLATFORM_VERSION' in os.environ:
            self.desired_capabilities_cloud["platformVersion"] = os.environ['PLATFORM_VERSION']
        self.desired_capabilities_cloud["deviceName"] = os.environ['DEVICE_UDID']
        self.desired_capabilities_cloud["newCommandTimeout"] = 99999
        self.desired_capabilities_cloud["launchTimeout"] = 99999
        time.sleep(5)
        #try:
        self.driver = webdriver.Remote("http://" + os.environ['HOST'] + ":" + os.environ['UNIQUE_PORT'] + "/wd/hub", self.desired_capabilities_cloud)
        #except Exception as e:
        #log("Error Instantiating Driver [" + str(e) + "]. Attempting to continue anyway.")
        time.sleep(10)

    def postMessage(self, message):
        postString = self.jsonGridPrefix + message
        self.pubnub.publish(self.channel, postString, callback=_callback, error=error)
        log("Request posted to PubNub [" + postString + "]")

    def setChannelPrefixes(self):
        global test_run_id
        log("PUBSUB CHANNEL ID: " + self.channel)
        self.gridIdentity = "Trilleon-Automation-" + os.environ['GRID_IDENTITY_PREFIX']
        self.jsonGridPrefix = "{\"test_run_id\":\"" + test_run_id + "\"},{\"grid_identity\":\"" + self.gridIdentity + "\"},{\"grid_source\":\"server\"},{\"grid_context\":\"StandardAlert\"},"
        self.jsonBuddyGridPrefix = "{\"test_run_id\":\"" + test_run_id + "\"},{\"grid_identity\":\"" + self.buddyName + "\"},{\"grid_source\":\"server\"},{\"grid_context\":\"BuddyMessage\"},"

    # Close down the driver, call XML and JSON finalizer, close PubNub connection, and handle any fatal execution error.
    def tearDown(self):
        self.pubnub.unsubscribe(self.channel)
        self.writeAutomationResults('TEST-all.xml')
        if self.fatalErrorDetected == True:
            with open("FatalErrorDetected.txt", "w") as f:
                f.write(self.fatalErrorMessage)
        time.sleep(5)
        try:
            self.driver.quit()
        except:
            log("Error encountered while quitting driver")

        with open("test_status.txt", "w") as f:
            f.write(os.environ['TEST_RUN_STATUS'])
        pylogtext = ""
        with open("PyLog.txt", "r") as f:
            pylogtext = f.read()
        if "CRITICAL_SERVER_FAILURE_IN_APPIUM_TEST" in pylogtext:
            raise Exception("CRITICAL_SERVER_FAILURE_IN_APPIUM_TEST Detected!")

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

    def handle_device_alert(accept):
        if os.environ['DEVICE_PLATFORM'] == "android":
            # Try to dismiss any Android account alerts.
            try:
                acceptButton = self.driver.find_element_by_xpath("//*[@text='Allow']")
                action = TouchAction(self.driver)
                action.tap(acceptButton).perform()
                log("Chose 'Allow' for Google Play alert.")
            except Exception as e:
                log("Exception accepting Android alert! " + str(e))
            try:
                acceptButton = self.driver.find_element_by_xpath("//*[@text='OK']")
                action = TouchAction(self.driver)
                action.tap(acceptButton).perform()
                log("Chose 'OK' for Google Play alert.")
            except Exception as e:
                log("Exception accepting Android alert! " + str(e))
        else:
            log("Attempting to dismiss any iOS device-level alert.")
            if accept == True:
                self.driver.switch_to.alert.accept()
            else:
                self.driver.switch_to.alert.dismiss()
            # Workaround for iOS XCTest bug that prevents clicking of device alerts in Landscape mode.
            middle_width = self.device_width / 2
            middle_height = self.device_height /2
            pixel_additive = 0;
            action = TouchAction(self.driver)
            action.tap(x=middle_width, y=middle_height).perform() # Exact Center for optionless alerts.
            while pixel_additive <= 20:
                try:
                    # Tap in square of increasing size.
                    if accept == True:
                        action.tap(x=middle_width, y=middle_height + pixel_additive).perform()
                        action.tap(x=middle_width + pixel_additive, y=middle_height).perform()
                        action.tap(x=middle_width + pixel_additive, y=middle_height + pixel_additive).perform()
                        action.tap(x=middle_width + pixel_additive, y=middle_height - pixel_additive).perform()
                    else:
                        action.tap(x=middle_width, y=middle_height - pixel_additive).perform()
                        action.tap(x=middle_width - pixel_additive, y=middle_height).perform()
                        action.tap(x=middle_width - pixel_additive, y=middle_height - pixel_additive).perform()
                        action.tap(x=middle_width - pixel_additive, y=middle_height + pixel_additive).perform()
                except Exception as e:
                    log("Exception accepting iOS alert! " + str(e))
                pixel_additive += 10


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
        global test_run_id
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
        list_history = recent_posts.split(test_run_id)

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
        log("ALL_XML_FINAL " + all_xml)
        return True
            
    # Find, extract, format, and save all single-test JSON results for server HTML report
    def get_json_from_client_run(self, force):
        log("Command Recieved: [Get JSON]")
        recent_posts = self.get_communication_history()
        if "completed_automation" in recent_posts or force == True:
            log("Generating JSON.")
            initialDelimiter = "SINGLE_TEST_RESULTS_JSON|"
            delimiter = "|"
            rawAll = recent_posts.split(initialDelimiter)
            json = ""
            index = 0
            partialInitialDelimiter = "SINGLE_TEST_RESULTS_JSON_MULTI_PART|"
            for x in rawAll:
                if index > 0 and len(x) > 0:
                    # Handle test results reporting that was too large to send in a single message, requiring several parts of a single result report.
                    if partialInitialDelimiter in x:
                        rawPartials = x.split(partialInitialDelimiter) # All partial message pieces
                        indexPartial = 0
                        piecesFinal = ["" for x in range(len(rawPartials))]
                        for z in rawPartials:
                            if indexPartial == 0:
                                json += rawPartials[0].split(delimiter)[0] # First, record the test that preceded the partial test details report.
                            else:
                                piecesPartial = z.split(self.partialDataDelimiter)
                                piecesFinal[int(piecesPartial[0])] = piecesPartial[1].split(delimiter)[0] # The first piece after splicing is the index/order of this piece. Set that piece equal to the actual message data.
                            indexPartial += 1
                        for f in piecesFinal:
                            json += f # Should piece together valid json in correct order if previous for loop correctly handled ordering.
                    else:
                        json += x.split(delimiter)[0]
                index += 1
            
            # "@APOS@" token is a special encoding of double qoutes to prevent issues with PubNub message encoding and proper formatting of JSON
            json = json.replace("@APOS@", "\"").replace("}{", "},{")
            if not json.endswith("]"):
                if json.endswith(","):
                    json = json[:-1] + "]"
                else:
                    json += "]"
            fileContent = ""
            with open("testresultsjson.txt", "r") as f:
                fileContent = f.read()
            log("JSON FINAL LENGTH [" + str(len(json)) + "]")
            try:
                log("JSON FINAL ACTUAL [" + json + "]")
            except Exception as e:
                log("Failed to parse final json [" + str(e) + "]")
            if json not in fileContent:
                with open("testresultsjson.txt", "a") as f:
                    f.write(json)
            return True
        else:
            return False

    # This is used by the server to check for client "heartbeats", or to request them, to verify that client has not crashed or hanged in execution.
    def has_heartbeat(self):
        global heartbeats
        if self.last_heartbeat_detected == None:
            self.last_heartbeat_detected = datetime.now()

        if len(heartbeats) > 0:
            log("Registered heartbeat #" + str(self.heartbeat_index))
            self.heartbeat_index += 1
            self.last_heartbeat_detected = datetime.now()

        timeDifference = (datetime.now() - self.last_heartbeat_detected).total_seconds()
        if timeDifference > self.max_time_since_heartbeat:
            self.postMessage("{\"health_check\":0}")
            log("Any heartbeat ??" + heartbeats)
            time.sleep(15)
            if len(heartbeats) > 0:
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
        global test_run_id
        resultsString = ""
        for x in history:
            rawMessage = urllib.unquote(str(x)).replace("\\", "").replace("\"", "'").replace("+", " ")
            splitMessage = rawMessage.split("test_run_id")
            for s in splitMessage:
                if test_run_id in s and (True if jsonFlag == None else (jsonFlag in s)):
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
        if command == "ready" and ("checking_in" in recent_posts or "DBID||" in recent_posts):
             time.sleep(5)
             if "DBID||" in recent_posts:
                 dbidPiece = recent_posts.split("DBID||")[1]
                 self.DBID = dbidPiece.split("||")[0]
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
