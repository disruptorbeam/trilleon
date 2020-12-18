﻿using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TrilleonAutomation {

	public class GameMaster : MonoBehaviour {

		public const string GAME_NAME = "QwackulatorPro";
		public const string BASE_TEST_RAILS_URL = "https://testrail.qwackulator.com"; //Your TestRails url here!

		public static int PLAYER_ID { get; set; }
		public static string PLAYER_NAME { get; set; }
		public static string PLAYER_EMAIL { get; set; }
		public static string PLAYER_PASSWORD { get; set; }

		public static bool Enabled {
			get {
				//CUSTOMIZE: WHAT ACTIVATES OR NEUTERS THE FRAMEWORK?
				return true;
			}
		}

		/// <summary>
		/// Commands allowing you to set account information for the current player.
		/// </summary>
		/// <value>The game.</value>
		public  Commands commands {
			get { 
				if(_commands == null) {

					_commands = this.GetComponent<Commands>();

				}
				return _commands;
			}
		}
		private Commands _commands;

		//Type if your custom type. The string is the field that your custom text asset uses to store the text we are searching for.
		public static List<KeyValuePair<Type,string>> AdditionalTextAssets = new List<KeyValuePair<Type,string>>(); //ex: new List<KeyValuePair<Type,string>>(){ new KeyValuePair<Type,string>(typeof(TextMeshProUGUI),"text") };

		public static List<KeyValuePair<Type,ActableTypes>> AdditionalAssetsAll = new List<KeyValuePair<Type,ActableTypes>> { }; //new KeyValuePair<Type,ActableTypes>( typeof(MyCustomInputField), ActableTypes.Input)
		public static List<KeyValuePair<string,string>> SpecialAssetsAndTheirPrefixesForAssistantWindowCodeGeneration = new List<KeyValuePair<string,string>> { }; //new KeyValuePair<string,string> ("MyCustomInputField", "input")

		public static string BasicBuddyInformation() {

			string information = string.Empty;
			//CUSTOMIZE: information += string.Format("PLAYER_ID={0}", PLAYER_ID.ToString());
			return information;

		}

		//Put any game-specific methods of determining an objects visibility to a user here.
		public static bool GameSpecificVisibilityChecks(GameObject g) {

			//return g.IsVisibleInHierarchy() && g.alpha > 0 : true;
			return true;

		}

		#if UNITY_EDITOR
		public static List<KeyValuePair<Type,ActableTypes>> GameSpecificActableTypesForRecordAndPlayback = new List<KeyValuePair<Type,ActableTypes>>(); //new KeyValuePair<Type,ActableTypes>(typeof(BuyButton), ActableTypes.Clickable) 
		#endif

		public static List<Type> ExpectedTopLevelMasterScripts = new List<Type>() { }; //typeof(UIPanel)

		public static string GetReturnStatementForType(string type, string name) {
			switch(type.ToLower()){
			/* CUSTOMIZE: EXAMPLE
            	case "screenview":
					return string.Format("Q.help.GetView<{0}>();", name);
			*/
			default:
				return "TYPE NOT IMPLEMENTED IN GAMEMASTER";
			}
		} 

		public static AssetVariant Asset_Variant {
			get{ 
				if(_Asset_Variant == AssetVariant.none || _Asset_Variant != GetAssetVariant()) {
					_Asset_Variant = GetAssetVariant();
				}
				return _Asset_Variant; 
			}
		}
		private static AssetVariant _Asset_Variant ; //Default standard definition if not set.
		public enum AssetVariant { none, ld, sd, hd, xd };

		public static string Asset_Bundle_Version {
			get{ 
				if(_Asset_Bundle_Version == null) {
					_Asset_Bundle_Version =  string.Empty;
				}
				return _Asset_Bundle_Version; 
			}
		}
		private static string _Asset_Bundle_Version;

		public static string Client_Version {
			get{ 
				if(_Client_Version == null) {
					_Client_Version = string.Empty;
				}
				return _Client_Version; 
			}
		}
		private static string _Client_Version;

		public static string Build_Number {
			get{ 
				if(_Build_Number == null) {
					_Build_Number = string.Empty;
				}
				return _Build_Number; 
			}
		}
		private static string _Build_Number;

		public static string Revision_Number {
			get{ 
				if(_Build_Number == null) {
					_Revision_Number =  string.Empty;;
				}
				return _Revision_Number; 
			}
		}
		private static string _Revision_Number;

		public static string Platform {
			get{ 
				if(_Platform == null) {
					string os = SystemInfo.operatingSystem.ToString();
					if(os.ToLower().Contains("android")){
						_Platform = "Android";
					} else if(os.ToLower().Contains("iphone")){
						_Platform = "iPhone";
					} else if(os.ToLower().Contains("windows")){
						_Platform = "Windows Desktop";
					} else if(os.ToLower().Contains("mac")){
						_Platform = "Mac Desktop";
					}
				}
				return _Platform; 
			}
		}
		private static string _Platform;

		//TODO
		private static AssetVariant GetAssetVariant() {
			/*
			switch (AssetBundleManager.variantLevel)
			{
			case (AssetBundleManager.BundleVariantLevel.ld): 
				return AssetVariant.ld;
			case (AssetBundleManager.BundleVariantLevel.sd): 
				return AssetVariant.sd;
			case (AssetBundleManager.BundleVariantLevel.hd): 
				return AssetVariant.hd;
			case (AssetBundleManager.BundleVariantLevel.xd): 
				return AssetVariant.xd;
			default:
				return AssetVariant.none;
			}
			*/
			return AssetVariant.none;

		}

		public string GetGameSpecificEmailReportTagLine() {

			string returnValue = string.Empty;
			DateTime dateTime = DateTime.UtcNow;
			string dateTimeString = string.Format("{0} {1}", dateTime.ToLongDateString(), dateTime.ToLongTimeString()); 

			string color = AutomationMaster.TestRunContext.Passed.Tests.Any() ? "green" : "red";
			string passedTag = string.Format("<strong>Passed</strong> [ <span style='color:{0};'>{1}</span> ]", color, AutomationMaster.TestRunContext.Passed.Tests.Count);
			color = AutomationMaster.TestRunContext.Failed.Tests.Any() ? "red" : "green";
			string skippedTag = AutomationReport.SkipFailCount > 0 ? string.Format(" (<span style='color:orange; padding: 2px;'>{0}</span>)", AutomationMaster.TestRunContext.Skipped.Tests.Count) : string.Empty;
			string failedTag = string.Format("<strong>Failed</strong> [ <span style='color:{0};'>{1}</span>{2} ]", color, AutomationMaster.TestRunContext.Failed.Tests.Count, skippedTag);
			string ignoredTag = AutomationMaster.TestRunContext.Ignored.Tests.Any() ? string.Format("<strong>Ignored</strong> [<span style='color:blue;'> {0} </span>]", AutomationMaster.TestRunContext.Ignored.Tests.Count) : string.Empty;
			returnValue = string.Format("<div class='automation_summary_collapse' onClick='ToggleSummaryCollapsable($(this));'></div><div class='automation_summary'><div><span class='tag_data'><strong>Device Time</strong> [ {0} ]</span></div><div><span class='tag_data'><strong>Model</strong> [ {1} ]</span></div>" +
				"<div><span class='tag_data'><strong>Client Version</strong> [ {2} ]</span></div><div><span class='tag_data'><strong>Asset Variant</strong> [ {3} ]</span></div><div><span class='tag_data'><strong>Bundle Version</strong> [ {4} ]</span></div><div><span class='tag_data'><strong>Platform</strong> [ {5} ]</span></div>" +
				"<div><span class='tag_data'><strong>Revision#</strong> [ {6} ]</span></div><div><span class='tag_data'><strong>Build Version</strong> [ {7} ]</span></div><div><span class='tag_data'><strong>PLAYER_ID</strong> [ {8} ]</span></div></div><div style='margin-top: 10px;'><span class='tag_data'>{9}</span><span class='tag_data'>{10}</span><span class='tag_data'>{11}</span></div>", 
				dateTimeString, SystemInfo.deviceModel, Client_Version, Asset_Variant, Asset_Bundle_Version, Platform, Revision_Number, Build_Number, PLAYER_ID, passedTag, failedTag, ignoredTag);

			return returnValue;

		}


		public string GetDeviceDetails() {

			string result = string.Format("DEVICE MODEL: {0} || ASSET VARIANT: {1} || ASSET_BUNDLE_VERSION: {2} || CLIENT_VERSION: {3} || BUILD_NUMBER: {4} || REVISION NUMBER: {5} || PLATFORM: {6} || PLAYER_ID: {7}", 
				SystemInfo.deviceModel, GameMaster.Asset_Variant, GameMaster.Asset_Bundle_Version, GameMaster.Client_Version, GameMaster.Build_Number, GameMaster.Revision_Number, GameMaster.Platform, "CUSTOMIZE:");

			return result;

		}

		public bool IgnoreWaitForNoLoadingIndicators { get; set; } //Prevents recursion caused by using driver commands that reference this method as well.
		//In some cases, elements are active and visible, but a loading overlay prevents access to these elements. See if they are visible, and wait until they are not.
		public IEnumerator WaitForNoLoadingIndicators() {

			if(IgnoreWaitForNoLoadingIndicators) {

				yield break;

			}
			IgnoreWaitForNoLoadingIndicators = true;

			//CUSTOMIZE: Fill out this logic - add checks and waits for the disappearance of visible loading indicators custom to your game.

			IgnoreWaitForNoLoadingIndicators = false;
			yield return StartCoroutine(Q.driver.WaitRealTime(1));

		}

		public IEnumerator WaitForGameLoadingComplete() {

			//CUSTOMIZE: Add custom logic to wait for your game to finish its initial load.
			yield return null;

		}

		public IEnumerator TryDismissErrors() {

			yield return null;

		}

		public IEnumerator PreTestRunLaunch() {

			yield return null;

		}

		public IEnumerator GlobalSetUpTest() {

			RunnerFlagTests.SetUpGlobalRun = true;
			yield return null;

		}

		public IEnumerator GlobalSetUpClass() {

			RunnerFlagTests.SetUpClassGlobalRun = true; //Debug test value.
			yield return null;

		}

		public IEnumerator GlobalTearDownClass() {

			RunnerFlagTests.TearDownClassGlobalRun = true; //Debug test value.
			yield return null;

		}

		public IEnumerator GlobalTearDownTest() {

			RunnerFlagTests.TearDownGlobalRun = true;
			yield return null;

		}

	}

}