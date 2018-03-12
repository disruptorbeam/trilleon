/* 
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
*/

﻿#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.EventSystems;

namespace TrilleonAutomation {
   
   public class Overseer : MonoBehaviour {

      public AutomationMaster AutomationMaster {
         get { 
            if(TestMonitorHelpers.Helper.GetComponent<AutomationMaster>() == null) {
               _automationMaster = TestMonitorHelpers.Helper.AddComponent<AutomationMaster>();
            } else if(_automationMaster == null) {
               _automationMaster = TestMonitorHelpers.Helper.GetComponent<AutomationMaster>();
            }
            return _automationMaster;
         }
      }
      private AutomationMaster _automationMaster;
         
      public GameObject inspectedObject = null;
      public GameObject storedObjectLast = null;
      public GameObject parentObject = null;
      public string parentBaseType = string.Empty;
      public AutoStepMaster stepMaster;

      public float waitTime = 1;

      public bool pauseOnFailure = false;
      public bool showBasic = true;
      public bool showAdvanced = true;
      public bool showComponents = false;
      public bool showFieldsAndProperties = false;
      public bool softReset = false;
      public bool ignoreDependentTestsForRun = false;

      public string objectName = string.Empty;
      public string countObjectsWithThisName = string.Empty;
      public string objectTag = string.Empty;
      public string objectActiveness = string.Empty;
      public string objectPropertyNameInTestObject = string.Empty;
      public AutoObjectType objectType = AutoObjectType.None;
      public Condition condition = Condition.Exists;
      public string textContainsValue = string.Empty;
      public string testObject = string.Empty;
      public string textType = string.Empty;
	   public string textVal = string.Empty;

      public MonoScript testObjectFile = null;
      public string[] componentsList;
      public string[] fieldsAndPropertiesList;
      public AutoStepType step;
      public AutoAction action;
      public AutoObjectAssert assertObject;
      public bool reverseCondition = false;
      public bool tryAction = false;
      public string textContainsToUse = string.Empty;
      public By findBy;
      public string testObjectCodeGenerated = string.Empty;
      public string testCodeGenerated = string.Empty;
      public bool generate = false;
      public KeyValuePair<string,string> referencedScript;
      public UnityEngine.Object nexus;

      //Test Manifest.
      public KeyValuePair<string,string> Master_Editor_Override {
         get { 
            if(string.IsNullOrEmpty(_Master_Editor_Override.Key)) {
               _Master_Editor_Override = new KeyValuePair<string,string>();
            }
            return _Master_Editor_Override; 
         }
         set { _Master_Editor_Override = value; }
      }
      private KeyValuePair<string,string> _Master_Editor_Override;

      #region Recorder
      public bool failedToBeginRecording = false;
      public bool record = false;
      public bool isAwaitingCodeGen = false;
      public static List<KeyValuePair<AutoAction,GameObject>> RecordedActions = new List<KeyValuePair<AutoAction,GameObject>>();
      #endregion

   }

}
#endif
