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
import sys
import unittest
import xmlrunner
import time
from BaseAppiumTest import BaseAppiumTest, log
from appium.webdriver.common.touch_action import TouchAction
import Globals

class GameAppiumTest(BaseAppiumTest):
    def setUp(self):
        # Setting up GameAppiumTest
        log("Setting up GameAppiumTest")
        
        # BaseAppiumTest takes settings (local or cloud) from environment variables
        log("Setting up Parent class")
        super(GameAppiumTest, self).setUp()
        log("setup complete!")
    
    # Test start.
    def test_stt(self):
        
        log("Grid Identity: " + self.gridIdentity)
        self.postMessage("{\"notification\":\"Appium server ready; waiting for application to load.\"}")
        self.take_screenshot("/app_launch.png")

        #Allow time for game to be launched
        time.sleep(20)
        self.postMessage("{\"notification\":\"Beggining the test.\"}")

        #Run all tests and wait for response.
        timeout = 0
        isReady = False
        while(isReady == False and timeout < self.timeout_default):
            isReady = self.check_for_client_responses("ready", False)
            time.sleep(10)
            timeout+=10

        if(timeout >= self.timeout_default):
            message = "Timed out waiting for client to start listening on PubNub channel."
            log(message)
            self.take_screenshot("/launch_failure.png")
            log(message)
            log("CRITICAL_SERVER_FAILURE_IN_APPIUM_TEST")
            return False
        else:
            self.take_screenshot("/test_start.png")

        self.postMessage("{\"set_test_run_id\":\"" + self.test_run_id_internal + "\"}")
        time.sleep(1)
        if os.environ.get('PRE_TEST_COMMANDS') is not None:
            self.postMessage("{\"console_command\":\"" + os.environ['PRE_TEST_COMMANDS'] + "\"}")
        time.sleep(5)
        if os.environ.get('LOOP_TESTS') is not None:
            self.postMessage("{\"loop_tests\":\"" + os.environ['LOOP_TESTS'] + "\"}")
        time.sleep(5)

        # Send test command to client.
        command_json = "{\"automation_command\":\"rt " + self.run_command + "\"},"
        if self.ignoreBuddyTests == 1 or self.ignoreBuddyTests == "1" :
            command_json += "{\"buddy_ignore_all\":0}"
            log("Ignoring Buddy tests.")
        else:
            command_json += "{\"" + self.buddyCommandName + "\":\"" + self.buddyName + "\"}"
            log("Running Buddy tests.")
        
        if self.additionalCommands != None and len(self.additionalCommands) > 1:
            command_json += "," + self.additionalCommands
        self.postMessage(command_json)
        
        timeout = 0
        isReady = False
        while(isReady == False and timeout < self.timeout_default):
            isReady = self.check_for_client_responses("started", False)
            time.sleep(5)
            timeout+=5
        
        if(timeout >= self.timeout_default):
            message = "Timed out waiting for client to start automation."
            log(message)
            self.take_screenshot("/launch_failure.png")
            log(message)
            log("CRITICAL_SERVER_FAILURE_IN_APPIUM_TEST")
            return False

        #Globals.ignore_console_printing_to_save_xml_cdata_buffer_size = True
        isReady = False
        indexVal = 0
        testRunTime = 0
        hasHeartbeat = True
        while(isReady == False and hasHeartbeat == True):
            if Globals.critical_exception == True:
                break
            if self.buddyCheckComplete == False and (self.ignoreBuddyTests == 0 or self.ignoreBuddyTests == "0") and testRunTime > self.timeout_default / 2:
                self.buddy_check() #Check if Buddy has declared itself ready now. If not, send command to ignore buddy tests.
            self.check_for_client_requests("handle_client_commands")
            self.check_for_client_requests("screenshot")
            testRunTime += 2
            time.sleep(2)
            #Every ten seconds, check these things.
            if testRunTime % 10 == 0:
                hasHeartbeat = self.has_heartbeat()
                isReady = self.check_for_client_responses("complete", False)
                self.check_for_client_responses("fatal_error_check", False)
                #Every twenty seconds, check these things.
                if testRunTime % 20 == 0:
                    self.handle_device_alert(True)
                    self.postMessage("{\"server_heartbeat\": " + str(testRunTime) + "}")
        
        is_failure = False
        failure_message = ""
        if testRunTime >= self.test_execution_timeout:
            failure_message = "Timed out waiting for client to complete automation."
            is_failure = True
        elif hasHeartbeat == False:
            failure_message = "Application crashed during test run."
            is_failure = True
        elif Globals.critical_exception == True:
            failure_message = "Trilleon test runner experienced an unhandled exception that stopped test execution."
            is_failure = True
        
        if is_failure == True:
            self.take_screenshot("/launch_failure.png")
            log(failure_message)
            log("CRITICAL_SERVER_FAILURE_IN_APPIUM_TEST")
            time.sleep(15)

        timeout = 0
        xmlRetrieved = False
        jsonRetrieved = False
        deviceDetailsRetrieved = False
        heapSizeRetrieved = False
        heapSizeJSONRetrieved = False
        fpsRetrieved = False
        fpsJSONRetrieved = False
        garbageCollectionRetrieved = False
        garbageCollectionJSONRetrieved = False
        gameLoadTimeRetrieved = False
        exceptionsDataRetrieved = False
        log("Waiting for all test results data to be communicated.")
        while((xmlRetrieved == False or jsonRetrieved == False or exceptionsDataRetrieved == False or fpsRetrieved == False or fpsJSONRetrieved == False or deviceDetailsRetrieved == False or heapSizeRetrieved == False or heapSizeJSONRetrieved == False or garbageCollectionRetrieved == False or garbageCollectionJSONRetrieved == False or gameLoadTimeRetrieved == False) and timeout < self.timeout_default and self.fatalErrorDetected == False):
            if Globals.critical_exception == True:
                break
            if xmlRetrieved == False:
                xmlRetrieved = self.get_xml_from_client_run()
            if jsonRetrieved == False:
                jsonRetrieved = self.format_json_from_client_run(False)
            if deviceDetailsRetrieved == False:
                deviceDetailsRetrieved = self.check_for_client_responses("device_details_html", True)
            if heapSizeRetrieved == False:
                heapSizeRetrieved = self.check_for_client_responses("heap_size", True)
            if heapSizeJSONRetrieved == False:
                heapSizeJSONRetrieved = self.check_for_client_responses("heap_json", True)
            if fpsRetrieved == False:
                fpsRetrieved = self.check_for_client_responses("fps", True)
            if fpsJSONRetrieved == False:
                fpsJSONRetrieved = self.check_for_client_responses("fps_json", True)
            if garbageCollectionRetrieved == False:
                garbageCollectionRetrieved = self.check_for_client_responses("garbage_collection", True)
            if garbageCollectionJSONRetrieved == False:
                garbageCollectionJSONRetrieved = self.check_for_client_responses("garbage_collection_json", True)
            if gameLoadTimeRetrieved == False:
                gameLoadTimeRetrieved = self.check_for_client_responses("game_launch_seconds", True)
            if exceptionsDataRetrieved == False:
                exceptionsDataRetrieved = self.check_for_client_responses("exceptions_data", True)
            if is_failure == True:
                self.format_json_from_client_run(True)
                return False # If a fatal error occurred, we run this check loop just once and stop execution here.
            time.sleep(15)
            timeout+=15

        Globals.ignore_console_printing_to_save_xml_cdata_buffer_size = True
        if Globals.critical_exception == False and timeout < self.timeout_default:
            log("Automation Script Completed!")
            self.postMessage("{\"notification\":\"Server Success: Automation script complete; shutting down...\"}",)
        else:
            if Globals.critical_exception == True:
                error_message = "Critical exception in Trilleon killed test execution."
            else:
                error_message = "Could not retrieve required test run results data. XML Retrieved? [" + str(xmlRetrieved) + "]. Json Retrieved? [" + str(jsonRetrieved) + "]. Heap Size Values Retrieved? [" + str(heapSizeRetrieved) + "]. Heap JSON Retrieved? [" + str(heapSizeJSONRetrieved) + "]. Garbage Collection Values Retrieved? [" + str(garbageCollectionRetrieved) + "]. Garbage Collection JSON Retrieved? [" + str(garbageCollectionJSONRetrieved) + "]. FPS Values Retrieved? [" + str(fpsRetrieved) + "]. FPS JSON Retrieved? [" + str(fpsJSONRetrieved) + "]"
            log(error_message)
            self.postMessage("{\"notification\":\"" + error_message + "\"}")
        
        self.postMessage("{\"notification\":\"Completed test, tearing down momentarily.\"}")
        time.sleep(5)
        # Test end.

if __name__ == '__main__':
    unittest.main(failfast=False,buffer=False,catchbreak=False)
    '''
        CUSTOMIZE: Do you want the xmlrunner report? This report is not needed by Trilleon, but can provide extra information for debugging.
        Essentially, it will generate an xml test report with a single test case that contains CDATA representing all console output.
    '''
    #with open('test-reports.xml', 'wb') as output:
        #unittest.main(testRunner=xmlrunner.XMLTestRunner(output=output),failfast=False, buffer=False,catchbreak=False)
