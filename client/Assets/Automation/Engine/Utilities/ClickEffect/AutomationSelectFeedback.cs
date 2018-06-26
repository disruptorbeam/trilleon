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

﻿using System.Collections;
using UnityEngine;

namespace TrilleonAutomation {
	
	public class AutomationSelectFeedback : MonoBehaviour {

		public float InitialDiameter { get; set; }
		public float FinalDiameter { get; set; }
		public float PulseLifeSpan { get; set; }
		public float RingLifeSpan { get; set; }
		public float TimeBetweenPulses { get; set; }

		void InstantiateNewRing() {
			
			GameObject ring = new GameObject("ring_pulsar", typeof(SpriteRenderer), typeof(AutomationSelectRings));
			ring.GetComponent<AutomationSelectRings>().container = this;
			ring.transform.SetParent(this.transform, false);

		}

		void Start() {

			InitialDiameter = 0.1f;
			FinalDiameter = 0.25f;
			PulseLifeSpan = 2.75f;
			RingLifeSpan = 1.20f;
			TimeBetweenPulses = 1f;
			StartCoroutine(Pulse());

		}

		IEnumerator Pulse() {

			GameObject ring = new GameObject("ring_center", typeof(SpriteRenderer));
			ring.transform.SetParent(this.transform, false);
			ring.GetComponent<SpriteRenderer>().sprite = Driver.MouseHand;
			ring.GetComponent<SpriteRenderer>().sortingLayerName = "foreground";
			ring.GetComponent<SpriteRenderer>().sortingOrder = 99;
			ring.transform.localScale = new Vector3(0.075f, 0.075f, 0.075f);

			float timer = 0f;
			do {

				InstantiateNewRing();
				yield return StartCoroutine(Q.driver.WaitRealTime(TimeBetweenPulses));
				timer += TimeBetweenPulses;

			} while(timer < PulseLifeSpan);

			DestroyImmediate(ring);
			DestroyImmediate(this.gameObject);
			yield return null;

		}
		
	}

}
