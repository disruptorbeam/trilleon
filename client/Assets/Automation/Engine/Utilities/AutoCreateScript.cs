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
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;
using System;

namespace TrilleonAutomation {

   public enum AutoStepMaster { ObjectStep, SimpleWait, WaitFor };
   public enum AutoStepType { None, Action, ObjectAssert, BoolAssert };
   public enum AutoAction { None, Click, TryClick, ClickAndHold, SendKeys };
   public enum AutoBoolAssert { None, IsTrue, Fail };
   public enum AutoObjectAssert { None, IsNull, IsActiveVisibleAndInteractable };
   public enum AutoObjectType { None, InputField, Button, Text, GameObject };

   public class AutoCreateScript {

      static Overseer overseer {
         get { 
            if(inspectOverseer ==  null){
				inspectOverseer = GameObject.Find(TestMonitorHelpers.NAME).GetComponent<Overseer>();
            }
            return inspectOverseer;
         }
      }
      private static Overseer inspectOverseer;

      #region Code Pieces
      public const string TOP_LEVEL_TEST_OBJECT_NAME = "primary";
      public const string ONFAIL_MESSAGE_PREFIX = "Could not interact with";
      public const string ASSERTION_MESSAGE_PREFIX = "Assertion failed for";
      public const string START_COROUTINE = "yield return StartCoroutine(";

      public const string CLICK = "Q.driver.Click(";
      public const string CLICK_AND_HOLD = "Q.driver.ClickAndHold(";
      public const string TRY_CLICK = "Q.driver.TryClick(";
      public const string WAIT_FOR_CONDITION = "Q.driver.WaitForCondition(";
      public const string WAIT_REAL_TIME = "Q.driver.WaitRealTime(";
      public const string SEND_KEYS = "Q.driver.SendKeys(";

      public const string FAIL = "yield return StartCoroutine(Q.assert.Fail(";
      public const string IS_TRUE = "yield return StartCoroutine(Q.assert.IsTrue(";
      public const string ACTIVENESS = "yield return StartCoroutine(Q.assert.IsActiveVisibleAndInteractable(";
      public const string IS_NULL = "yield return StartCoroutine(Q.assert.IsNull(";
      #endregion

      public static string GenerateLineOfTestCode(string typeName){

         StringBuilder code = new StringBuilder();
         bool isAssert = false;
         if(overseer.step == AutoStepType.Action) {
            if(overseer.action != AutoAction.None) {
               code.Append(START_COROUTINE);
               switch(overseer.action) {
                  case AutoAction.Click:
                     code.Append(CLICK);
                     break;
                  case AutoAction.TryClick:
                     code.Append(TRY_CLICK);
                     break;
                  case AutoAction.ClickAndHold:
                     code.Append(CLICK_AND_HOLD);
                     break; 
                  case AutoAction.SendKeys:
                     code.Append(SEND_KEYS);
                     break;
               }

            }
         }

         string conditionArgument = string.Empty;
         if(overseer.assertObject != AutoObjectAssert.None) {
            isAssert = true;                  
            conditionArgument = string.Format("{0}.{1}_{2}", overseer.testObject, Enum.GetName(typeof(AutoObjectType), overseer.objectType).ToLower(), overseer.referencedScript.Value);
            switch(overseer.assertObject) {
               case AutoObjectAssert.IsActiveVisibleAndInteractable:
                  code.Append(ACTIVENESS);
                  break;
               case AutoObjectAssert.IsNull:
                  code.Append(IS_NULL);
                  break;
            }

         }


         string objectNamePrefix = string.Empty;
         if(overseer.objectType == AutoObjectType.Text) {
            typeName = overseer.textType;
            objectNamePrefix = typeName != "text" ? "gui" : typeName;
         } else if(overseer.objectType == AutoObjectType.GameObject) {
            typeName = "GameObject";
            objectNamePrefix = "obj";
         } else {
            typeName = Enum.GetName(typeof(AutoObjectType), overseer.objectType );
            objectNamePrefix = typeName;
         }

         if(GameMaster.SpecialAssetsAndTheirPrefixesForAssistantWindowCodeGeneration.ExtractListOfKeysFromKeyValList().Contains(overseer.textType)) {
            objectNamePrefix = GameMaster.SpecialAssetsAndTheirPrefixesForAssistantWindowCodeGeneration.Find(x => x.Key == overseer.textType).Value;
         }

         if(string.IsNullOrEmpty(overseer.testObject)) {
            code.Append("NOT IMPLEMENTED!");
         } else {
            if(isAssert) {
               string reverseCondition = overseer.reverseCondition ? ", true" : string.Empty;
               string codeSnippet = string.Format("{0}, \"{1} {2}.\"{3}", conditionArgument, ASSERTION_MESSAGE_PREFIX, overseer.objectName, reverseCondition);
               code.Append(codeSnippet);
            } else {
               string codeSnippet = string.Empty;
               //If we have an object reference for this in the master script, then point to that. Otherwise, generate search script for the object under the master script game object.
               codeSnippet = string.Format("{0}.{1}_{2}, \"{3} {2}.\"", overseer.testObject, objectNamePrefix, overseer.referencedScript.Value, ONFAIL_MESSAGE_PREFIX);
               code.Append(codeSnippet);
            }
         }
         code.Append("));");

         return code.ToString();

      }

      public static string GenerateTestObjectCode(){

         string objectType = string.Empty;
         switch(overseer.objectType){
            case AutoObjectType.Button:
               objectType = "button";
               break;
            case AutoObjectType.InputField:
               objectType = "input";
               break;
            case AutoObjectType.Text:
               objectType = "text";
               break;
            case AutoObjectType.GameObject:
               objectType = "object";
               break;
         }

         if(string.IsNullOrEmpty(overseer.referencedScript.Value)) {
            string[] pieces = overseer.objectName.Split('.');
            string name = string.Empty;
            if(pieces.Length > 1) {
               name = pieces[1];
            }
            overseer.referencedScript = new KeyValuePair<string, string>(string.Empty, string.Format("{0}_{1}", objectType, name.Replace(" ", string.Empty)));
         }

         string valueText = string.Empty;
         string findBy = string.Empty;
         switch(overseer.findBy) {
            case By.Name:
            case By.Default:
               findBy = "By.Name";
               valueText = overseer.inspectedObject.name;
               break;
            case By.TagName:
               findBy = "By.TagName";
               valueText = overseer.inspectedObject.tag;
               break;
            case By.ContainsComponent:
				//TODO UPDATE AS THIS SEARCH CONSTRAIT HAS BEEN IMPLEMENTED DIFFERENTLY THAN OTHERS.
               findBy = "By.ContainsComponent";
               throw new UnityException("Not implemented!");
            case By.TextContent:
               findBy = "By.TextContent";
               valueText = overseer.textContainsToUse;
               break;
            case By.ImageFileName:
               findBy = "By.ImageFileName";
               throw new UnityException("Not implemented!");
            case By.GameObjectType:
               findBy = "By.GameObjectType";
               throw new UnityException("Not implemented!");
         }

         string typeName = string.Empty;
         string objectNamePrefix = string.Empty;
         if(overseer.objectType == AutoObjectType.Text) {
            typeName = overseer.textType;
            objectNamePrefix = typeName != "text" ? "gui" : typeName;
         } else if(overseer.objectType == AutoObjectType.GameObject) {
            typeName = "GameObject";
            objectNamePrefix = "obj";
         } else {
            typeName = Enum.GetName(typeof(AutoObjectType), overseer.objectType );
            objectNamePrefix = typeName;
         }

         StringBuilder code = new StringBuilder();
         string[] objectNameBreadCrumbs = overseer.referencedScript.Value.Split('.');
         code.Append(string.Format("public static {0} {1}_{2} ", typeName, objectNamePrefix.ToLower(), objectNameBreadCrumbs[objectNameBreadCrumbs.Length-1]));
         code.AppendLine("{");
         code.AppendLine("   get {");

         //If we have an object reference for this in the master script, then point to that. Otherwise, generate search script for the object under the master script game object.
         if(!string.IsNullOrEmpty(overseer.referencedScript.Key)) {

            code.AppendLine(string.Format("      return {0}.{1}; ", TOP_LEVEL_TEST_OBJECT_NAME, overseer.referencedScript.Value));

         } else {

            code.AppendLine(string.Format("      return Q.driver.FindIn({0}, {1}, \"{2}\"); ", TOP_LEVEL_TEST_OBJECT_NAME, findBy, valueText));

         }

         code.AppendLine("   }");
         code.AppendLine("}");

         return code.ToString();

      }

      public static string GetFileContentsForNewTestObject(string fileName){

         string codeTypeOfTestObject = fileName.Replace("TestObject", string.Empty);
         string getReturnStatmentForThisType = GameMaster.GetReturnStatementForType(overseer.parentBaseType, codeTypeOfTestObject);

         StringBuilder baseObject = new StringBuilder();
         if(!string.IsNullOrEmpty(fileName)) {
            baseObject.Append(string.Format("      public static {0} {1} ", codeTypeOfTestObject, TOP_LEVEL_TEST_OBJECT_NAME));
            baseObject.AppendLine("{");
            baseObject.AppendLine("         get {");
            baseObject.AppendLine(string.Format("            return {0};", getReturnStatmentForThisType));
            baseObject.AppendLine("         }");
            baseObject.AppendLine("      }");
         }

         StringBuilder file = new StringBuilder();
         file.AppendLine("using UnityEngine;");
         file.AppendLine("using UnityEngine.UI;");
         file.AppendLine("using System.Collections;");
         file.AppendLine("using System.Collections.Generic;");
         file.AppendLine(string.Empty);
         file.AppendLine("namespace TrilleonAutomation {");
         file.AppendLine(string.Empty);
         file.AppendLine(string.Format("   public class {0} : MonoBehaviour {{", fileName));
         file.AppendLine(string.Empty);

         //Add Static Self step for Test Object IEnumerators.
         file.AppendLine(string.Format("   public static {0} steps {{", fileName));
         file.AppendLine("      get {");
         file.AppendLine(string.Format("         {0} self = AutomationMaster.StaticSelf.GetComponent<{0}>();", fileName));
         file.AppendLine(string.Format("         if(self == null) {{", fileName));
         file.AppendLine(string.Format("            return AutomationMaster.StaticSelf.AddComponent<{0}>();", fileName));
         file.AppendLine("         }");
         file.AppendLine("         return self;");
         file.AppendLine("      }");
         file.AppendLine("   }");
         file.AppendLine(string.Empty);

         //Add primary field/property reference object.
         file.Append(baseObject.ToString());
         file.AppendLine(string.Empty);
         file.AppendLine("   }");
         file.AppendLine(string.Empty);
         file.Append("}");

         return file.ToString();

      }

      public static string GetMetaFileNewTestObject(){

         StringBuilder meta = new StringBuilder();
         Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

         meta.AppendLine("fileFormatVersion: 2");
         meta.AppendLine(string.Format("guid: {0}", System.Guid.NewGuid().ToString()));
         meta.AppendLine(string.Format("timeCreated: {0}", unixTimestamp));
         meta.AppendLine("licenseType: Free");
         meta.AppendLine("MonoImporter:");
         meta.AppendLine("  serializedVersion: 2");
         meta.AppendLine("  defaultReferences: []");
         meta.AppendLine("  executionOrder: 0");
         meta.AppendLine("  icon: {instanceID: 0}");
         meta.AppendLine("  userData:");
         meta.AppendLine("  assetBundleName:");
         meta.AppendLine("  assetBundleVariant: ");

         return meta.ToString();
      }

   }

}
#endif
