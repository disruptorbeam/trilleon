using UnityEngine;
using TMPro;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DisruptorEngine;
using DisruptorEngine.Net;

namespace TrilleonAutomation {

	public class GameMaster : MonoBehaviour {

   	public const string GAME_NAME = "TWD";

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

   	public static List<Type> AdditionalTextAssets = new List<Type>(){ typeof(TextMeshProUGUI) };
	   public static List<KeyValuePair<Type,ActableTypes>> AdditionalAssetsAll = new List<KeyValuePair<Type,ActableTypes>> { }; //new KeyValuePair<Type,ActableTypes>( typeof(MyCustomInputField), ActableTypes.Input)

   	public static List<KeyValuePair<string,string>> SpecialAssetsAndTheirPrefixesForAssistantWindowCodeGeneration = new List<KeyValuePair<string,string>>{
      	new KeyValuePair<string,string>("TextMeshProUGUI", "gui"),
      	new KeyValuePair<string,string>("TMP_InputField", "input")
      };

      #region DBeam-only code.

   	public static bool TutorialSkipped { get; set;}
   	public static long DBID {
      	get { return Account.Instance.dbid; }
      }

   	public static string Default_Player_Name {
      	get { 
         	return _Default_Player_Name; 
         }
      	set { _Default_Player_Name = value; }
      }
   	public static string _Default_Player_Name = string.Empty;

   	public static string Player_Name {
      	get { 
         	if(string.IsNullOrEmpty(_Player_Name)) {
            	_Player_Name = Character.Instance.displayName;
            }
         	return _Player_Name; 
         }
      	set { _Player_Name = value; }
      }
   	public static string _Player_Name = string.Empty;

      public static string BasicBuddyInformation() {

      	string information = string.Empty;
      	information += string.Format("DBID={1}{0}PLAYER_NAME={2}", AutomationMaster.DELIMITER, DBID.ToString(), Player_Name);
      	return information;

      }

   	public static void SetInfoForBuddyTests(string key, string value) {
      	SetInfoForBuddyTests(new List<KeyValuePair<string,string>>() { new KeyValuePair<string,string>(key, value) });
      }

   	public static void SetInfoForBuddyTests(List<KeyValuePair<string,string>> values) {

      	string thisTestName = AutomationMaster.CurrentTestContext.TestName;
      	string storageKey = string.Format("BUDDY_INFO_{0}", thisTestName);
      	string storageValues = string.Format("DBID={1}{0}PLAYER_NAME={2}", AutomationMaster.DELIMITER, DBID.ToString(), Player_Name);
      	for(int i = 0; i < values.Count; i++) {
         	storageValues += string.Format("|{0}={1}", values[i].Key, values[i].Value);
         }

      	Q.storage.Add(storageKey, storageValues);

      }

      //Put any game-specific methods of determining an objects visibility to a user here.
   	public static bool GameSpecificVisibilityChecks(GameObject g) {

      	return true;

      }

   	public static List<Type> ExpectedTopLevelMasterScripts = new List<Type>(){ typeof(UIPanel) };

   	public static string GetReturnStatementForType(string type, string name) {
      	switch(type.ToLower()){
      	case "uipanel":
         	return string.Format("UIMgr.Instance.FindPanel<{0}>()", name);
      	default:
         	return "TYPE NOT IMPLEMENTED IN GAMEMASTER";
         }
      } 

      #endregion

      #region DBeam only code

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
         	if(string.IsNullOrEmpty(_Asset_Bundle_Version) || _Asset_Bundle_Version != ConfigFromServer.Instance.assetBundleVersion) {
            	_Asset_Bundle_Version = ConfigFromServer.Instance.assetBundleVersion;
            }
         	return _Asset_Bundle_Version; 
         }
      }
   	private static string _Asset_Bundle_Version;

   	public static string Client_Version {
      	get{ 
         	if(_Client_Version == null) {
            	_Client_Version = BuildVersion.ClientVersion;
            }
         	return _Client_Version; 
         }
      }
   	private static string _Client_Version;

   	public static string Build_Number {
      	get{ 
         	if(_Build_Number == null) {
            	_Build_Number = BuildVersion.BuildNumber.ToString();
            }
         	return _Build_Number; 
         }
      }
   	private static string _Build_Number;

   	public static string Revision_Number {
      	get{ 
         	if(_Build_Number == null) {
            	_Revision_Number = BuildVersion.Revision;
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

      #if UNITY_EDITOR

   	public void ResetAccount() {

         //Reset account each time we launch.
      	AutomationMaster.IsDeadLaunch = true;
      	AccessToken.Clear();
      	AccountAuthService account = new AccountAuthService(this, Config.GetString("platform"), Config.GetString("client_id"));
      	account.ProvisionGuest(token => {
         	AccessToken.SaveInstance(token);
         }, null);

      }

      #endif 

   	private static AssetVariant GetAssetVariant(){
      	switch(AssetBundleManager.variantLevel) {
      	case(AssetBundleManager.BundleVariantLevel.ld): 
         	return AssetVariant.ld;
      	case(AssetBundleManager.BundleVariantLevel.sd): 
         	return AssetVariant.sd;
      	case(AssetBundleManager.BundleVariantLevel.hd): 
         	return AssetVariant.hd;
      	case(AssetBundleManager.BundleVariantLevel.xd): 
         	return AssetVariant.xd;
      	default:
         	return AssetVariant.none;
         }
      }

      #endregion

   	void Start() {

         #if UNITY_EDITOR
      	GameObject testMonitorHelper = GameObject.Find("TestMonitorHelper");
      	if(testMonitorHelper != null && testMonitorHelper.GetComponent<Overseer>().softReset == true) {
         	ResetAccount();
         }
         #endif

      }

   	public string GetGameSpecificEmailReportTagLine() {

      	string returnValue = string.Empty;
      	DateTime dateTime = DateTime.UtcNow;
      	string dateTimeString = string.Format("{0} {1}", dateTime.ToLongDateString(), dateTime.ToLongTimeString()); 

         #region DBeam only code

         string color = AutomationMaster.TestRunContext.passes.tests.Any() ? "green" : "red";
      	string passedTag = string.Format("<strong>Passed</strong> [ <span style='color:{0};'>{1}</span> ]", color, AutomationMaster.TestRunContext.passes.tests.Count);
         color = AutomationMaster.TestRunContext.fails.tests.Any() ? "red" : "green";
      	string skippedTag = AutomationReport.SkipFailCount > 0 ? string.Format(" (<span style='color:orange; padding: 2px;'>{0}</span>)", AutomationReport.SkipFailCount) : string.Empty;
      	string failedTag = string.Format("<strong>Failed</strong> [ <span style='color:{0};'>{1}</span>{2} ]", color, AutomationMaster.TestRunContext.fails.tests.Count, skippedTag);
         string ignoredTag = AutomationMaster.TestRunContext.ignored.tests.Any() ? string.Format("<strong>Ignored</strong> [<span style='color:blue;'> {0} </span>]", AutomationMaster.TestRunContext.ignored.tests.Count) : string.Empty;
      	returnValue = string.Format("<div class='automation_summary_collapse' onClick='ToggleSummaryCollapsable($(this));'></div><div class='automation_summary'><div><span class='tag_data'><strong>Device Time</strong> [ {0} ]</span></div><div><span class='tag_data'><strong>Model</strong> [ {1} ]</span></div>" +
            "<div><span class='tag_data'><strong>Client Version</strong> [ {2} ]</span></div><div><span class='tag_data'><strong>Asset Variant</strong> [ {3} ]</span></div><div><span class='tag_data'><strong>Bundle Version</strong> [ {4} ]</span></div><div><span class='tag_data'><strong>Platform</strong> [ {5} ]</span></div>" +
            "<div><span class='tag_data'><strong>Revision#</strong> [ {6} ]</span></div><div><span class='tag_data'><strong>Build Version</strong> [ {7} ]</span></div><div><span class='tag_data'><strong>Game Load Time</strong> [ {8} seconds ]</span></div><div><span class='tag_data'><strong>DBID</strong> [ {9} ]</span></div></div>" +
            "<div style='margin-top: 10px;'><span class='tag_data'>{10}</span><span class='tag_data'>{11}</span><span class='tag_data'>{12}</span></div>", 
         	dateTimeString, SystemInfo.deviceModel.Replace(",", "."), Client_Version, Asset_Variant, Asset_Bundle_Version, Platform, Revision_Number, Build_Number, AutomationMaster.LaunchTime.ToString(), DBID, passedTag, failedTag, ignoredTag);

         #endregion

      	return returnValue;

      }


   	public string GetDeviceDetails() {

      	string result = string.Empty;

         #region DBeam only code

      	result = string.Format("DEVICE MODEL: {0} || ASSET VARIANT: {1} || ASSET_BUNDLE_VERSION: {2} || CLIENT_VERSION: {3} || BUILD_NUMBER: {4} || REVISION NUMBER: {5} || PLATFORM: {6} || DBID: {7}", 
         	SystemInfo.deviceModel.Replace(",", "."), GameMaster.Asset_Variant, GameMaster.Asset_Bundle_Version, GameMaster.Client_Version, GameMaster.Build_Number, GameMaster.Revision_Number, GameMaster.Platform, GameMaster.DBID);

         #endregion

      	return result;

      }

      //Wait for game to finish initial load.
      public IEnumerator WaitForGameLoadingComplete() {

         yield return null;

      }

      //Pauses automation execution until game is in an interactable state.
      public IEnumerator WaitForNoLoadingIndicators() {

         yield return null;

      }

      #if UNITY_EDITOR
      public static List<KeyValuePair<Type,ActableTypes>> GameSpecificActableTypesForRecordAndPlayback = new List<KeyValuePair<Type,ActableTypes>> { 
         new KeyValuePair<Type,ActableTypes>(typeof(BuyButton), ActableTypes.Clickable) 
      };
      #endif

      //Global set of functionality that cannot be skipped and must be done once before test execution begins, regardless of the tests that are run (Click "Start" etc?).
   	public IEnumerator PreTestLaunch() {

      	if(AutomationMaster.ErrorPopupDetected && AutomationMaster.TryContinueAfterError) {

         	yield return StartCoroutine(Q.driver.Try.Click(ErrorPanelTestObject.button_close, 5f));

         }

      	yield return Q.driver.WaitRealTime(2);
      	while(AutoSignInPanelTestObject.primary != null) {
         	yield return Q.driver.WaitRealTime(2);
         }
      	yield return Q.driver.WaitRealTime(2);
      	while(PlayerNamePanelTestObject.primary == null) {
         	yield return Q.driver.WaitRealTime(2);
         }
      	yield return Q.driver.WaitRealTime(2);
      	if(PlayerNamePanelTestObject.button_continueBtn == null || !Q.driver.IsActiveVisibleAndInteractable(PlayerNamePanelTestObject.button_continueBtn)) {
         	yield return Q.driver.WaitRealTime(5);
         }

      	AutomationMaster.GameLaunchCompleted = DateTime.UtcNow;

      	yield return null;

      }

      //Try to Dismiss an error popup that is encountered during test execution. Called by AutomationMaster when game-specific errors are found in the GameAssert.cs extender "AssertNoErrorPopups".
      public IEnumerator TryDismissErrors() {

         if(ErrorPanelTestObject.primary != null) {
            yield return StartCoroutine(Q.driver.Try.Click(ErrorPanelTestObject.button_close, 2f));
         }

         if(ConfirmationPopupPanelTestObject.primary != null) {
            yield return StartCoroutine(Q.driver.Try.Click(ConfirmationPopupPanelTestObject.button_close, 2f));
         }

         yield return null;

      }

   	bool start_handled = false;
      public IEnumerator GlobalSetUpClass() {

         //If this test run has not already created an account AND the application was launch with an account reset OR the app is being run on a device - then create an account.
      	if(!start_handled && (AutomationMaster.IsDeadLaunch || !Application.isEditor)) {
   
            //Account creation is completed.
         	start_handled = true;
            if(!AutomationMaster.Methods.ExtractListOfKeysFromKeyValList(true, 0).Contains("TutorialTests")) {
            	yield return StartCoroutine(Q.game.commands.SkipTutorial());
            }

            Q.assert.MarkTestRailsTestCase(true, 1351, 2556916);
            Q.assert.MarkTestRailsTestCase(true, 1351, 2556917);

         }
            
      	yield return StartCoroutine(Q.help.ClickThroughAllNarrativeContent());
      	int max_tries = 5;
      	while(Q.driver.IsActiveVisibleAndInteractable(TutorialTooltipPanelTestObject.button_continue) && max_tries >= 0) {

         	yield return StartCoroutine(Q.driver.Try.Click(TutorialTooltipPanelTestObject.button_continue, 1));
         	max_tries--;

         }

         yield return null;

     }

     public IEnumerator GlobalTearDownClass() {

        yield return null;

     }

     public IEnumerator GlobalTearDown() {

         yield return null;

     }

     public IEnumerator GlobalSetUpTest() {

        //Close any open panels.
        List<GameObject> activePanelCloseButtons = MasterTestObject.buttons_active_closePanel;
        for(int x = 0; x < activePanelCloseButtons.Count; x++) {
           yield return StartCoroutine(Q.driver.Try.Click(activePanelCloseButtons[x], 1));
        }
        yield return StartCoroutine(Q.driver.Try.Click(TutorialTooltipPanelTestObject.button_continue));

			if(Q.driver.IsActiveVisibleAndInteractable(RewardsPanelTestObject.button_claim)) {
				yield return StartCoroutine(Q.driver.Try.Click(RewardsPanelTestObject.button_claim, 0.5f));
			}
			if(Q.driver.IsActiveVisibleAndInteractable(RewardsPanelTestObject.button_confirm)) {
				yield return StartCoroutine(Q.driver.Try.Click(RewardsPanelTestObject.button_confirm, 0.5f));
			}

     }

   }

}