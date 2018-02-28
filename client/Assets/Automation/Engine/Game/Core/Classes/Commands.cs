using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace TrilleonAutomation {

	public class Commands : MonoBehaviour {

   	Arbiter arbiter { 
      	get{ return this.GetComponent<Arbiter>(); }
      }     

   	public IEnumerator CompleteAllActiveMissions() {
      
      	UnityConsoleBroker.SendCommand("raiden", "duration=1 outcome=win");
      	yield return StartCoroutine(Q.driver.WaitRealTime(2));

      }

   	public IEnumerator GrantSpeedups() {

      	UnityConsoleBroker.SendCommand("give 24_hour_speedup 100");
      	yield return StartCoroutine(Q.driver.WaitRealTime(2));

      }

   	public IEnumerator SkipTutorial() {
         
      	UnityConsoleBroker.SendCommand("TUTORIALSKIP");
      	GameMaster.TutorialSkipped = true;
      	yield return StartCoroutine(Q.driver.WaitRealTime(2));

      }

   }

}