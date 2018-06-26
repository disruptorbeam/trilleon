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

	public class AutomationSelectRings : MonoBehaviour {

		public AutomationSelectFeedback container { get; set; }
		void Start() {

			GetComponent<SpriteRenderer>().sprite = Driver.BlueRing;
			GetComponent<SpriteRenderer>().sortingLayerName = "foreground";
			GetComponent<SpriteRenderer>().sortingOrder = 99;
			StartCoroutine(Grow());

		}

		IEnumerator Grow() {

			float currentSize = container.InitialDiameter;
			float sizeDifference = container.FinalDiameter - container.InitialDiameter;

			do {

				currentSize += sizeDifference * Time.deltaTime / container.RingLifeSpan;
				this.transform.localScale = new Vector3(currentSize, currentSize, currentSize);
				yield return new WaitForEndOfFrame();

				if(currentSize >= container.FinalDiameter * 0.75) {
				
					Color color = GetComponent<SpriteRenderer>().color;
					color.a -= 0.02f;
					GetComponent<SpriteRenderer>().color = color;
					if(currentSize > container.FinalDiameter * 1.5f) {

						break;

					}

				}

			} while(GetComponent<SpriteRenderer>().color.a > 0);

			DestroyImmediate(this.gameObject);
			yield return null;

		}

	}

}
