using System.Collections;
using System.Collections.Generic;
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