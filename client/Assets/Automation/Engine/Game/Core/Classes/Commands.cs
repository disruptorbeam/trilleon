/* 
   This file is part of Trilleon.  Trilleon is a client automation framework.
  
   Copyright (C) 2017 Disruptor Beam
  
   Trilleon is free software: you can redistribute it and/or modify
   it under the terms of the GNU Lesser General Public License as published by
   the Free Software Foundation, either version 3 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License
   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
